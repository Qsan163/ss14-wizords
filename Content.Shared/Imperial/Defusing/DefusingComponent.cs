using Robust.Shared.Serialization;
using Content.Shared.DoAfter;

namespace Content.Shared.Defusing.Components;   // Imperial Space | defusing comp | KAZAK1984
[RegisterComponent]
public sealed partial class DefusingComponent : Component
{
    [DataField] public int DefuseTime = 8;
    [DataField] public int DefuseTimeItem = 4;
}

[Serializable, NetSerializable]
public sealed partial class DefusingDoAfter : DoAfterEvent
{
    public override DoAfterEvent Clone()
    {
        return this;
    }
}
