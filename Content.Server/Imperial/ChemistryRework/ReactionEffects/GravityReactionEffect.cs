using Content.Server.Singularity.EntitySystems;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Creates a gravitational pulse, shoving around all entities within some distance of an epicenter.
/// </summary>

[DataDefinition]
public sealed partial class GravityReactionEffect : ReagentEffect
{
    [DataField("minRange")]
    public float MinRange = default!;

    [DataField("maxRange", required: true)]
    public float MaxRange = default!;

    /// <summary>
    ///     The base radial velocity that will be added to entities within range towards the center of the gravitational pulse
    /// </summary>
    [DataField("baseRadialDeltaV")]
    public float BaseRadialDeltaV = default!;

    /// <summary>
    ///     The base tangential velocity that will be added to entities within countrclockwise around the center of the gravitational pulse.
    /// </summary>
    [DataField("baseTangentialDeltaV")]
    public float BaseTangentialDeltaV = default!;

    [DataField("impulsePerUnit")]
    public float ImpulsePerUnit = 0.1f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-gravity",
            ("chance", Probability),
            ("type", BaseRadialDeltaV > 0 ? "pulls" : "repels")
        );

    public override void Effect(ReagentEffectArgs args)
    {
        var gravityWell = args.EntityManager.EntitySysManager.GetEntitySystem<GravityWellSystem>();
        var range = MathF.Min((float) (args.Quantity * ImpulsePerUnit), MaxRange);

        gravityWell.GravPulse(
            args.SolutionEntity,
            range,
            MinRange,
            BaseRadialDeltaV,
            BaseTangentialDeltaV
        );
    }
}
