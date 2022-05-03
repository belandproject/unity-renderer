using BLD.Controllers;
using BLD.Models;

namespace BLD.Components
{
    public interface IEntityComponent : IComponent, ICleanable, IMonoBehaviour
    {
        IBLDEntity entity { get; }
        void Initialize(IParcelScene scene, IBLDEntity entity);
    }
}