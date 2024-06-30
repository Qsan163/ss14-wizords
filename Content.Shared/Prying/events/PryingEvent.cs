namespace Content.Shared.Prying.Systems;


public sealed class PryingEvent(EntityUid user) : CancellableEntityEventArgs
{
    public EntityUid User = user;
}
