using Content.Shared.Chemistry.Reagent;
using Content.Shared.FixedPoint;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Заставляет сущность светиться.
/// </summary>
/// <remarks>
///     Поскольку я опять сделал костыль, то это не работает при нанесении на кожу сущности.
/// </remarks>

[DataDefinition]
public sealed partial class Luminescence : ReagentEffect
{
    /// <summary>
    ///     Если цвет не был передан, то генерирует случайный цвет каждый цикл метаболизации
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

        // Если на следующем тике метаболизации реагент закончится, то мы вырубаем свечение.

        var totalReagentCount = GetReagentCount(args);

        if (totalReagentCount - args.Quantity <= FixedPoint2.Zero)
        {
            if (pointLightSystem.TryGetLight(args.SolutionEntity, out var lightConp))
                pointLightSystem.SetEnabled(args.SolutionEntity, false, lightConp);

            return;
        }

        // Если источник света уже была привязан к сущности, то мы просто изменяем его параметры, а не создаем новый.

        if (pointLightSystem.TryGetLight(args.SolutionEntity, out var existLight))
        {
            if (!existLight.Enabled) pointLightSystem.SetEnabled(args.SolutionEntity, true, existLight);

            SetLightColor(args, existLight);
            ScaleLightPower(args, existLight);

            return;
        }

        // Добавление источника света к существу.

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
