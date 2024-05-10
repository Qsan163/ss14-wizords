using Content.Server.Humanoid;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Damage;
using Content.Shared.Damage.Prototypes;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Explodes the body
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

        damageSystem.TryChangeDamage( // I could use the BodySystem, but for some reason the brain and organs don't fall out when it gibs.
            args.SolutionEntity,
            new DamageSpecifier(protoManager.Index<DamageTypePrototype>("Blunt"), 10000),
            true
        );
    }
}
