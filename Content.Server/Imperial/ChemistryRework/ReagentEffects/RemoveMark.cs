using Content.Server.Humanoid;
using Content.Shared.Chemistry.Reagent;
using Robust.Shared.Prototypes;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Remove part of an entity.
/// </summary>

[DataDefinition]
public sealed partial class RemoveMark : ReagentEffect
{
    /// <summary>
    ///     All types of marks can be seen in <see cref="Shared.Humanoid.Markings.MarkingCategories"/>.
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
