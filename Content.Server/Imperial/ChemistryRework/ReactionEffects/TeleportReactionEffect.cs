using System.Numerics;
using Content.Server.Popups;
using Content.Shared.Chemistry.Reagent;
using Robust.Server.GameObjects;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Chemistry.ReactionEffects;

/// <summary>
///     Teleports a entity within a certain radius from the epicenter to X coordinates.
/// </summary>

[DataDefinition]
public sealed partial class Teleport : ReagentEffect
{
    [DataField("radiusPerUnit")]
    public float RadiusPerUnit = 0;

    [DataField("energyPerUnit")]
    public float EnergyPerUnit = 1;

    [DataField("minEnergy")]
    public float MinEnergy = 1;

    [DataField("maxEnergy")]
    public float MaxEnergy = float.MaxValue;

    [DataField("minRange")]
    public float MinRange = 1;

    [DataField("maxRange")]
    public float MaxRange = float.MaxValue;

    /// <summary>
    ///     Random or FaceRotation.
    ///     FaceRotation - teleportation along the direct sector of the view of the entity
    /// </summary>

    [DataField("teleportType")]
    public TeleportTypes? TeleportType;

    [DataField("coordinates")]
    public Vector2? Coordinates;

    protected override string? ReagentEffectGuidebookText(IPrototypeManager prototype, IEntitySystemManager entSys) =>
        Loc.GetString("reagent-effect-guidebook-teleport",
            ("chance", Probability)
        );

    public override void Effect(ReagentEffectArgs args)
    {
        var lookupSystem = args.EntityManager.System<EntityLookupSystem>();
        var xformSystem = args.EntityManager.System<TransformSystem>();

        var energy = MathF.Max(
            MathF.Min((float) (args.Quantity * EnergyPerUnit), MaxEnergy),
            MinEnergy
        );
        var range = MathF.Max(
            MathF.Min((float) (args.Quantity * RadiusPerUnit), MaxRange),
            MinRange
        );

        // Raffle Start

        if (args.Quantity < 1)
        {
            var popupSystem = args.EntityManager.System<PopupSystem>();
            var random = IoCManager.Resolve<IRobustRandom>();

            List<string> raffleVerbs = [
                "Хорошая попытка",
                "Увынск",
                "Не сегодня",
                "Атата, плохой химик",
                "Не нарушай законов природы",
                "Рецепт блинчиков: А.. Не важно",
                "Увы",
                "Яша 500 метров от вас",
                "Яша 10 метров от вас",
                "Вобля...",
                "Ох бля, это не к добру...",
                "Каждое такое переливание убивает одного химика",
                "Незя",
                "Не-а",
                "Лучше бы тессин сварил",
                "Тебе не надоело?",
                "Лучше бы в душ сходил",
                "Ультрагеймер, прекращай",
                "Шутка. Почему семья кенгуру не заводит второго ребенка? Он им не по карману",
                "Шутка. В Австрии из зоопарка сбежал крокодил. Полиция 6 часов гоняла крокодила по Вене",
                "Шутка. Заходят два дракона в бар. Один говорит другому: — Что-то здесь жарковато. А тот отвечает: — Рот закрой.",
                "Ищи другой способ",
                "Эту игру еще можно сломать, не расстраивайся",
            ];

            var raffleEntities = lookupSystem.GetEntitiesInRange(args.SolutionEntity, 1, LookupFlags.Dynamic);

            foreach (var uid in raffleEntities)
                popupSystem.PopupEntity(random.Pick(raffleVerbs), uid, uid);

            return;
        }

        // Raffle End

        var entities = lookupSystem.GetEntitiesInRange(args.SolutionEntity, range, LookupFlags.Dynamic);
        var mapPosition = xformSystem.GetWorldPosition(args.SolutionEntity);
        var reactionBounds = new Box2(mapPosition - new Vector2(energy, energy), mapPosition + new Vector2(energy, energy));

        foreach (var entity in entities)
        {
            var newPosition = Coordinates;

            if (TeleportType == TeleportTypes.Random)
                newPosition = GetRandomCoords(reactionBounds);
            else if (TeleportType == TeleportTypes.FaceRotation)
                newPosition = GetPositionFromRotation(args, reactionBounds, energy, entity);

            if (newPosition != null)
                xformSystem.SetWorldPosition(
                    entity,
                    (Vector2) newPosition
                );
        }
    }

    private static Vector2 GetRandomCoords(Box2 reactionBounds)
    {
        var random = IoCManager.Resolve<IRobustRandom>();

        var randomX = random.NextFloat(reactionBounds.Left, reactionBounds.Right);
        var randomY = random.NextFloat(reactionBounds.Bottom, reactionBounds.Top);

        return new Vector2(randomX, randomY);
    }

    private static Vector2 GetPositionFromRotation(ReagentEffectArgs args, Box2 reactionBounds, float energy, EntityUid uid)
    {
        var xformSystem = args.EntityManager.System<TransformSystem>();

        var resultVector = Angle.FromDegrees(45).RotateVec(
            xformSystem.GetWorldRotation(uid).RotateVec(new Vector2(energy, energy))
        );

        return reactionBounds.Center - resultVector;
    }
}
