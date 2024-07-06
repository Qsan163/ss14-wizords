using Content.Shared.IdentityManagement;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Medical;

namespace Content.Shared.MedicalRecords.Systems;

public abstract class SharedMedicalRecordsConsoleSystem : EntitySystem
{
    /// <summary>
    /// Any entity that has a the name of the record that was just changed as their visible name will get their icon
    /// updated with the new status, if the record got removed their icon will be removed too.
    /// </summary>
    public void UpdateMedicalIdentity(string name, MedicalStatus status)
    {
        var query = EntityQueryEnumerator<IdentityComponent>();

        // while (query.MoveNext(out var uid, out var identity))
        // {
        //     if (!Identity.Name(uid, EntityManager).Equals(name))
        //         continue;

        //     if (status == SecurityStatus.None)
        //         RemComp<CriminalRecordComponent>(uid);
        //     else
        //         SetCriminalIcon(name, status, uid);
        // }
    }

    /// <summary>
    /// Decides the icon that should be displayed on the entity based on the security status
    /// </summary>
    // public void SetMedicalIcon(string name, MedicalStatus status, EntityUid characterUid)
    // {
    //     EnsureComp<MedicalRecordComponent>(characterUid, out var record);

    //     var previousIcon = record.StatusIcon;

    //     record.StatusIcon = status switch
    //     {
    //         MedicalStatus.Dead => "SecurityIconParoled",
    //         _ => record.StatusIcon
    //     };

    //     if(previousIcon != record.StatusIcon)
    //         Dirty(characterUid, record);
    // }
}
