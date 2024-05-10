using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Causes the entity to glow.
/// </summary>
/// <remarks>
///     Since I made a crutch again, it does not work when applying the essence to the skin.
/// </remarks>

[DataDefinition]
public sealed partial class Luminescence : ReagentEffect
{
    /// <summary>
    ///     If the color was not transferred, then generates a random color every metabolization cycle
    /// </summary>
    [DataField("color")]
    public string? PaintingСolor;

    [DataField("minEnergy")]
    public float MinEnergy = 2;

    [DataField("maxEnergy")]
    public float MaxEnergy = float.PositiveInfinity;

    [DataField("minRange")]
    public float MinRange = 2;

    [DataField("maxRange")]
    public float MaxRange = float.PositiveInfinity;

    [DataField("rangePerUnit")]
    public float RangePerUnit = 0.1f;

    [DataField("energyPerUnit")]
    public float EnergyPerUnit = 0.1f;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-luminescence",
            ("chance", Probability)
        );

    public override void Effect(ReagentEffectArgs args)
    {
        var pointLightSystem = args.EntityManager.System<PointLightSystem>();

        // If the next time the reagent metabolization ends, we turn off the glow.

        var totalReagentCount = GetReagentCount(args);

        if (totalReagentCount - args.Quantity <= FixedPoint2.Zero)
        {
            if (pointLightSystem.TryGetLight(args.SolutionEntity, out var lightConp))
                pointLightSystem.SetEnabled(args.SolutionEntity, false, lightConp);

            return;
        }

        // If the light source has already been applied to nature, then we simply change its parameters, rather than creating new ones.

        if (pointLightSystem.TryGetLight(args.SolutionEntity, out var existLight))
        {
            if (!existLight.Enabled) pointLightSystem.SetEnabled(args.SolutionEntity, true, existLight);

            SetLightColor(args, existLight);
            ScaleLightPower(args, existLight);

            return;
        }

        // Add point light to the entity

        var light = pointLightSystem.EnsureLight(args.SolutionEntity);

        SetLightColor(args, light);
        ScaleLightPower(args, light);

        pointLightSystem.SetEnabled(args.SolutionEntity, true, light);
    }

    private static Color GenerateRandomColor()
    {
        var random = IoCManager.Resolve<IRobustRandom>();

        var r = random.NextByte(255);
        var g = random.NextByte(255);
        var b = random.NextByte(255);

        return new Color(r, g, b);
    }

    private void SetLightColor(ReagentEffectArgs args, SharedPointLightComponent light)
    {
        var pointLightSystem = args.EntityManager.System<PointLightSystem>();

        if (PaintingСolor == null)
            pointLightSystem.SetColor(args.SolutionEntity, GenerateRandomColor(), light);
        else
            pointLightSystem.SetColor(args.SolutionEntity, Color.FromHex(PaintingСolor), light);
    }

    private void ScaleLightPower(ReagentEffectArgs args, SharedPointLightComponent light)
    {
        var pointLightSystem = args.EntityManager.System<PointLightSystem>();
        var reagentCount = GetReagentCount(args);

        var energy = MathF.Max(
            MathF.Min((float) (reagentCount * EnergyPerUnit), MaxEnergy),
            MinEnergy
        );
        var range = MathF.Max(
            MathF.Min((float) (reagentCount * RangePerUnit), MaxEnergy),
            MinRange
        );

        pointLightSystem.SetEnergy(args.SolutionEntity, energy, light);
        pointLightSystem.SetRadius(args.SolutionEntity, range, light);
    }

    private static FixedPoint2 GetReagentCount(ReagentEffectArgs args)
    {
        if (args.Source != null && args.Reagent?.ID != null) return args.Source!.GetTotalPrototypeQuantity(args.Reagent!.ID);

        return FixedPoint2.Zero;
    }
}
