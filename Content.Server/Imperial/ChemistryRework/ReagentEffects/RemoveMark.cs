using Content.Server.Humanoid;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Удаляет часть сущности.
/// </summary>

[DataDefinition]
public sealed partial class RemoveMark : ReagentEffect
{
    /// <summary>
    ///     Все типы слоев можно увидеть в <see cref="Shared.Humanoid.Markings.MarkingCategories"/>.
    /// </summary>
    [DataField("MarkingCategory")]
    public string MarkingCategory = "Hair";

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-remove-mark",
            ("chance", Probability),
            ("category", MarkingCategory)
        );

    public override void Effect(ReagentEffectArgs args)
    {
        if (!Enum.TryParse(MarkingCategory, out Shared.Humanoid.Markings.MarkingCategories marking)) return;

        var humSystem = args.EntityManager.System<HumanoidAppearanceSystem>();

        humSystem.RemoveMarking(args.SolutionEntity, marking, 0);
    }
}
