using Content.Shared.IdentityManagement;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Imperial.Medical;
﻿using Robust.Shared.Serialization;

namespace Content.Shared.Imperial.MedicalRecords.Systems;

public abstract class SharedMedicalRecordsConsoleSystem : EntitySystem
{
    public void UpdateMedicalIdentity(string name, MedicalStatus status)
    {
        var query = EntityQueryEnumerator<IdentityComponent>();
    }
}
