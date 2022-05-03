namespace BLD.Controllers
{
    public interface IOutOfSceneBoundariesHandler
    {
        void UpdateOutOfBoundariesState(bool enable);
    }
}