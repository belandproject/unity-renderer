using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BLD.Builder
{
    public interface IPublisher
    {
        void Initialize();
        void Dipose();
        void Publish(IBuilderScene scene);
    }
}