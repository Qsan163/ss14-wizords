using Robust.Shared.Serialization;
using Robust.Shared.Audio;

namespace Content.Shared.Paper;

[RegisterComponent]
public sealed partial class EverythingStampComponent : Component
{
    /// <summary>
    ///     The loc string name that will be stamped to the piece of paper on examine.
    /// </summary>
    [DataField("collectedStamps")]
    public List<StampDisplayInfo> CollectedStamps = [];
    [DataField("currentStamp")]
    public string CurrentStampName = "stamp-component-stamped-name-default";
}
