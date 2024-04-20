using System.Numerics;
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
