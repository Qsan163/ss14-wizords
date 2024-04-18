using Content.Server.Flash;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReactionEffects;

[DataDefinition]
public sealed partial class FlashReactionEffect : ReagentEffect
{
    [DataField("maxRange", required: true)]
    public float MaxRange = default!;

    [DataField("maxDuration")]
    public float MaxDuration = 3.0f;

    [DataField("slowTo")]
    public float SlowTo = 0.8f;

    [DataField("powerPerUnit")]
    public float PowerPerUnit = 0.25f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-flash",
            ("chance", Probability)
        );

    public override void Effect(ReagentEffectArgs args)
    {
        var flasSystem = args.EntityManager.EntitySysManager.GetEntitySystem<FlashSystem>();

        var range = MathF.Min((float) (args.Quantity * PowerPerUnit), MaxRange);
        var duration = MathF.Min((float) (args.Quantity * PowerPerUnit), MaxDuration) * 1000f;

        flasSystem.FlashArea(
            args.SolutionEntity,
            null,
            range,
            duration,
            SlowTo
        );
    }
}
