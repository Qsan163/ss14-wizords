using Content.Shared.Radio;
using JetBrains.Annotations;
using Robust.Client.GameObjects;

namespace Content.Client.Radio.Ui;

[UsedImplicitly]
public sealed class MicrophoneBoundUserInterface : BoundUserInterface
{
    [ViewVariables]
    private MicrophoneMenu? _menu;

    public MicrophoneBoundUserInterface(EntityUid owner, Enum uiKey) : base(owner, uiKey)
    {

    }

    protected override void Open()
    {
        base.Open();

        _menu = new();

        _menu.OnMicPressed += enabled =>
        {
            SendMessage(new ToggleIntercomMicMessage(enabled));
        };
        _menu.OnChannelSelected += channel =>
        {
            SendMessage(new SelectIntercomChannelMessage(channel));
        };

        _menu.OnClose += Close;
        _menu.OpenCentered();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (!disposing)
            return;
        _menu?.Close();
    }

    protected override void UpdateState(BoundUserInterfaceState state)
    {
        base.UpdateState(state);

        if (state is not IntercomBoundUIState msg)
            return;

        _menu?.Update(msg);
    }
}
