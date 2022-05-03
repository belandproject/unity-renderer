using Cinemachine;
using BLD.Camera;
using UnityEngine;
using UnityEngine.Rendering;

namespace BLD
{
    public interface ISceneReferences
    {
        MouseCatcher mouseCatcher { get ;  }
        GameObject groundVisual { get ;  }
        GameObject biwCameraParent { get ;  }
        InputController inputController { get ;  }
        GameObject cursorCanvas { get ;  }
        GameObject biwBridgeGameObject { get ;  }
        PlayerAvatarController playerAvatarController { get ;  }
        CameraController cameraController { get ;  }
        UnityEngine.Camera mainCamera { get ;  }
        GameObject bridgeGameObject { get ;  }
        Light environmentLight { get ;  }
        Volume postProcessVolume { get ;  }
        CinemachineFreeLook thirdPersonCamera { get ; }
        CinemachineVirtualCamera firstPersonCamera { get ; }
        void Dispose();
    }
}