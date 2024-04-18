using Content.Server.Humanoid;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Взрывает тело.
/// </summary>

[DataDefinition]
public sealed partial class Gib : ReagentEffect
{
    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-gib",
            ("chance", Probability)
        );

    public override void Effect(ReagentEffectArgs args)
    {
        var damageSystem = args.EntityManager.System<DamageableSystem>();
        var protoManager = IoCManager.Resolve<IPrototypeManager>();

        damageSystem.TryChangeDamage( // Я бы мог использовать BodySystem, но при гибе почему-то не выпадает мозг и органы.
            args.SolutionEntity,
            new DamageSpecifier(protoManager.Index<DamageTypePrototype>("Blunt"), 10000),
            true
        );
    }
}
