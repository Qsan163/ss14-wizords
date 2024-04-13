using Content.Server.Radio.EntitySystems;
using Content.Shared.Chat;
using Content.Shared.Radio;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Server.Radio.Components;

/// <summary>
///     Listens for radio messages and relays them to local chat.
/// </summary>
[RegisterComponent]
[Access(typeof(RadioDeviceSystem))]
public sealed partial class RadioSpeakerComponent : Component
{
    public enum SpeachType  //CUSTOM. отвечает за то, как будет сказано предложение при получении
    {
        Whisper,
        Speak,
        Emote
    }

    /// <summary>
    /// Whether or not interacting with this entity
    /// toggles it on or off.
    /// </summary>
    [DataField("toggleOnInteract")]
    public bool ToggleOnInteract = true;

    [DataField("channels", customTypeSerializer: typeof(PrototypeIdHashSetSerializer<RadioChannelPrototype>))]
    public HashSet<string> Channels = new () { SharedChatSystem.CommonChannel };

    [DataField("enabled")]
    public bool Enabled;

    [DataField("speachType")]  //CUSTOM. отвечает за то, как будет сказано предложение при получении
    public SpeachType speachType = SpeachType.Whisper;
}
