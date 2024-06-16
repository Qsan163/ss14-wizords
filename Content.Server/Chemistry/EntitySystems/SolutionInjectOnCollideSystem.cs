using Content.Server.Body.Components;
using Content.Server.Body.Systems;
using Content.Server.Chemistry.Components;
using Content.Server.Chemistry.Containers.EntitySystems;
using Content.Shared.Inventory;
using Content.Shared.Projectiles;

// Imperial Space arrow-fix Dependency Start
using Content.Shared.Armor;
using Content.Shared.Clothing.Components;
// Imperial Space arrow-fix Dependency End

namespace Content.Server.Chemistry.EntitySystems;

public sealed class SolutionInjectOnCollideSystem : EntitySystem
{
    [Dependency] private readonly SolutionContainerSystem _solutionContainersSystem = default!;
    [Dependency] private readonly BloodstreamSystem _bloodstreamSystem = default!;
    [Dependency] private readonly InventorySystem _inventorySystem = default!;

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SolutionInjectOnCollideComponent, ProjectileHitEvent>(HandleInjection);
    }

    private void HandleInjection(Entity<SolutionInjectOnCollideComponent> ent, ref ProjectileHitEvent args)
    {
        var component = ent.Comp;
        var target = args.Target;

        if (!TryComp<BloodstreamComponent>(target, out var bloodstream) ||
            !_solutionContainersSystem.TryGetInjectableSolution(ent.Owner, out var solution, out _))
        {
            return;
        }

        if (component.BlockSlots != 0x0)
        {
            var containerEnumerator = _inventorySystem.GetSlotEnumerator(target, component.BlockSlots);

            // TODO add a helper method for this?
            if (containerEnumerator.MoveNext(out _))
                return;
        }

        if (ImperialCheckHardsuit(target)) return; // Imperial Space arrow-fix

        var solRemoved = _solutionContainersSystem.SplitSolution(solution.Value, component.TransferAmount);
        var solRemovedVol = solRemoved.Volume;

        var solToInject = solRemoved.SplitSolution(solRemovedVol * component.TransferEfficiency);

        _bloodstreamSystem.TryAddToChemicals(target, solToInject, bloodstream);
    }

    // Imperial Space arrow-fix Start
    private bool ImperialCheckHardsuit(EntityUid target)
    {
        // Okay... Maybe I'll need refactoring this someday...

        if (!EntityManager.TryGetComponent<InventoryComponent>(target, out var inventory)) return false;

        if (!_inventorySystem.TryGetSlotContainer(target, "outerClothing", out var outerClothingContainer, out var _, inventory)) return false;
        if (!_inventorySystem.TryGetSlotContainer(target, "head", out var headContainer, out var _, inventory)) return false;

        if (!TryComp(outerClothingContainer.ContainedEntity, out InjectComponent? injectComponent)) return false;
        if (!TryComp(headContainer.ContainedEntity, out ArmorComponent? armorHeadComponent)) return false;

        if (injectComponent.Locked && armorHeadComponent.AntiHypo) return true;

        return false;
    }
    // Imperial Space arrow-fix End
}
