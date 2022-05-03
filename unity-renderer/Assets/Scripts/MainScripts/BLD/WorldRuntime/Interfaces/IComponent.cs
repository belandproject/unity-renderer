using System.Collections;
using BLD.Controllers;

namespace BLD.Components
{
    public interface IComponent
    {
        IParcelScene scene { get; }
        string componentName { get; }
        void UpdateFromJSON(string json);
        void UpdateFromModel(BaseModel model);
        IEnumerator ApplyChanges(BaseModel model);
        void RaiseOnAppliedChanges();
        bool IsValid();
        BaseModel GetModel();
        int GetClassId();
    }
}