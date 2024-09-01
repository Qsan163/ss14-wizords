using Content.Shared.Interaction;
using Content.Shared.Explosion.Components;
using Content.Shared.Defusing.Components;
using Content.Shared.Verbs;
using Content.Shared.DoAfter;
using Content.Shared.Trigger;
using Content.Shared.Popups;
using Content.Server.Administration.Logs;
using Content.Shared.Database;
using Content.Shared.Tools.Systems;

namespace Content.Server.Explosion.EntitySystems;   // Imperial Space | defusing comp | KAZAK1984

public sealed partial class DefusingSystem : EntitySystem
{
    [Dependency] private readonly IEntityManager _entities = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly EntityManager _entityManager = default!;
    [Dependency] private readonly SharedAppearanceSystem _appearance = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly IAdminLogManager _adminLogs = default!;
    [Dependency] private readonly SharedToolSystem _tool = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<DefusingComponent, GetVerbsEvent<AlternativeVerb>>(AddDefusingVerb);
        SubscribeLocalEvent<DefusingComponent, DefusingDoAfter>(OnDoAfterDefusing);
        SubscribeLocalEvent<DefusingComponent, InteractUsingEvent>(OnInteractUsing);
    }

    private void AddDefusingVerb(EntityUid uid, DefusingComponent comp, ref GetVerbsEvent<AlternativeVerb> ev)
    {
        var user = ev.User;
        var target = ev.Target;
        if (TryComp(uid, out ActiveTimerTriggerComponent? trigger))
        {
            AlternativeVerb verb = new()
            {
                Act = () =>
                {
                    TryDefusing(uid, comp, user, target);
                },
                Text = Loc.GetString("defusing-system-verb"),
                Priority = -1
            };
            ev.Verbs.Add(verb);
        }
    }

    public void OnInteractUsing(EntityUid uid, DefusingComponent comp, ref InteractUsingEvent args)
    {
        var user = args.User;
        var target = args.Target;
        if (TryComp(uid, out ActiveTimerTriggerComponent? trigger))
            if (_tool.HasQuality(args.Used, "Cutting"))
                TryDefusingItem(uid, comp, user, target);
    }

    private void OnDoAfterDefusing(EntityUid uid, DefusingComponent comp, DefusingDoAfter args)
    {
        if (args.Cancelled)
        {
            _popup.PopupEntity(Loc.GetString("defusing-cancel-popup"), uid);
            return;
        }
        if (TryComp(uid, out ActiveTimerTriggerComponent? trigger))
        {
            _entities.RemoveComponent(uid, trigger);
            _popup.PopupEntity(Loc.GetString("defusing-ending-popup"), uid);
            _adminLogs.Add(LogType.InteractActivate, LogImpact.Medium, $"{ToPrettyString(args.User)} defusing the {ToPrettyString(uid)}.");
            if (TryComp<AppearanceComponent>(uid, out var appearance))
                _appearance.SetData(uid, TriggerVisuals.VisualState, TriggerVisualState.Defused, appearance);
        }

    }

    private void TryDefusing(EntityUid uid, DefusingComponent comp, EntityUid user, EntityUid target)
    {
        _popup.PopupEntity(Loc.GetString("defusing-start-popup"), uid);
        var doAfterArgs = new DoAfterArgs(_entityManager, user, TimeSpan.FromSeconds(comp.DefuseTime), new DefusingDoAfter(), uid, target)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
            DistanceThreshold = 2f,
        };
        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }

    private void TryDefusingItem(EntityUid uid, DefusingComponent comp, EntityUid user, EntityUid target)
    {
        _popup.PopupEntity(Loc.GetString("defusing-start-popup"), uid);
        var doAfterArgs = new DoAfterArgs(_entityManager, user, TimeSpan.FromSeconds(comp.DefuseTimeItem), new DefusingDoAfter(), uid, target)
        {
            BreakOnMove = true,
            BreakOnDamage = true,
            NeedHand = true,
            DistanceThreshold = 2f,
        };
        _doAfterSystem.TryStartDoAfter(doAfterArgs);
    }
}
