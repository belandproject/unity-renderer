using System.Collections;
using System.Collections.Generic;
using BLD.Builder;
using BLD.Controllers;
using UnityEngine;

public interface IBIWController
{
    void Initialize(IContext context);
    void EnterEditMode(IParcelScene scene);
    void ExitEditMode();
    void OnGUI();

    void LateUpdate();

    void Update();
    void Dispose();
}