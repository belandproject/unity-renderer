using BLD;

public class PlacesAndEventsFeature : IPlugin
{
    public PlacesAndEventsFeature() { DataStore.i.exploreV2.isPlacesAndEventsSectionInitialized.Set(true); }

    public void Dispose() { }
}