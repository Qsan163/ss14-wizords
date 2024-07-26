using Robust.Shared.GameStates;
using Robust.Shared.Serialization;

namespace Content.Shared.NoVisorVisible;

//If enabled, remove job icon and medical bar on visors

[RegisterComponent]
[NetworkedComponent]

public sealed partial class NoVisorVisibleComponent : Component
{
    [DataField("Enabled")]
    public bool enabled = true;
}
