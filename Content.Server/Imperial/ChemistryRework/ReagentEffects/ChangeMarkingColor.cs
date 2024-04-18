using Content.Server.Humanoid;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Humanoid;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Изменяет цвет слоя сущности. Также способен менять цвет ее кожи.
/// </summary>

[DataDefinition]
public sealed partial class ChangeMarkingColor : ReagentEffect
{
    /// <summary>
    ///     Включает все енумы отсюда <see cref="Shared.Humanoid.Markings.MarkingCategories"/> и кожу.
    /// </summary>
    [DataField("markingCategory")]
    public string MarkingCategory = "Hair";

    /// <summary>
    ///     При этом параметре игнорирует параметр <see cref="PaintingСolor"/> и инвертирует цвет.
    /// </summary>
    [DataField("invertColor")]
    public bool InvertColor = false;

    /// <summary>
    ///     Если цвет не был передан и <see cref="InvertColor"/> в значении false, то генерирует случайный цвет каждый цикл метаболизации
    /// </summary>
    [DataField("color")]
    public string? PaintingСolor;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-change-marking-color",
            ("chance", Probability),
            ("category", MarkingCategory)
        );

    public override void Effect(ReagentEffectArgs args)
    {
        if (!Enum.TryParse(MarkingCategory, out Shared.Humanoid.Markings.MarkingCategories marking) && MarkingCategory != "Skin") return;

        var humSystem = args.EntityManager.System<HumanoidAppearanceSystem>();
        var color = InvertColor ? InvertMarkingColor(args, marking) : GenerateColor();

        if (MarkingCategory == "Skin")
            humSystem.SetSkinColor(args.SolutionEntity, color);
        else
            humSystem.SetMarkingColor(args.SolutionEntity, marking, 0, new List<Color> { color });
    }

    private Color GenerateColor()
    {
        if (PaintingСolor != null) return Color.FromHex(PaintingСolor);

        var random = IoCManager.Resolve<IRobustRandom>();

        var r = random.NextByte(255);
        var g = random.NextByte(255);
        var b = random.NextByte(255);

        return new Color(r, g, b);
    }

    private Color InvertMarkingColor(ReagentEffectArgs args, Shared.Humanoid.Markings.MarkingCategories marking)
    {
        if (
            !args.EntityManager.TryGetComponent<HumanoidAppearanceComponent>(args.SolutionEntity, out var hum)
        ) return GenerateColor();

        if (!hum.MarkingSet.TryGetCategory(marking, out var markings) || MarkingCategory == "Skin")
        {
            if (MarkingCategory != "Skin") return GenerateColor();

            return Invert(hum.SkinColor);
        }

        foreach (var mark in markings)
            return Invert(mark.MarkingColors[0]);

        return GenerateColor();
    }

    private static Color Invert(Color color)
    {
        if (!SkinColor.VerifySkinColor(HumanoidSkinColor.HumanToned, color)) return new Color(1 - color.R, 1 - color.G, 1 - color.B);

        return SkinColor.HumanSkinTone(
            Convert.ToInt32(
                100 - SkinColor.HumanSkinToneFromColor(color)
            )
        );
    }
}
