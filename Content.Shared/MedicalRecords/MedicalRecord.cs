using Content.Shared.Medical;
using Robust.Shared.Serialization;

namespace Content.Shared.MedicalRecords;

/// <summary>
/// Criminal record for a crewmember.
/// Can be viewed and edited in a criminal records console by security.
/// </summary>
[Serializable, NetSerializable, DataRecord]
public sealed record MedicalRecord
{
    /// <summary>
    /// Status of the person (None, Wanted, Detained).
    /// </summary>
    [DataField]
    public MedicalStatus Status = MedicalStatus.None;

    /// <summary>
    /// When Status is Wanted, the reason for it.
    /// Should never be set otherwise.
    /// </summary>
    [DataField]
    public string? Reason;

    /// <summary>
    /// Criminal history of the person.
    /// This should have charges and time served added after someone is detained.
    /// </summary>
    [DataField]
    public List<MedicalHistory> History = new();
}

/// <summary>
/// A line of criminal activity and the time it was added at.
/// </summary>
[Serializable, NetSerializable]
public record struct MedicalHistory(TimeSpan AddTime, string Medical);
