namespace Content.Shared.Prying.Systems;


public sealed class PryingEvent(EntityUid user) : EventArgs
{
    public EntityUid User = user;
}
