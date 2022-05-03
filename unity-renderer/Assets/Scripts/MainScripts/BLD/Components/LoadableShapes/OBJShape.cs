using System;
using BLD.Controllers;

namespace BLD.Components
{
    public class OBJShape : LoadableShape<LoadWrapper_OBJ, LoadableShape.Model>
    {
        public OBJShape() { }

        public override void CallWhenReady(Action<ISharedComponent> callback) { callback.Invoke(this); }
    }
}