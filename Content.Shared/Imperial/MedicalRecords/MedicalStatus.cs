namespace Content.Shared.Imperial.Medical;

/// <summary>
/// Status used in Medical Records.
///
/// None - the default value
/// Dead - dead
/// DeadWithoutSoul - dead without soul, medics can't clone this guy
/// DeadNonClone - bad guy, dont clone him
/// </summary>
public enum MedicalStatus : byte
{
    None,
    Dead,
    DeadWithoutSoul,
    DeadNonClone
}