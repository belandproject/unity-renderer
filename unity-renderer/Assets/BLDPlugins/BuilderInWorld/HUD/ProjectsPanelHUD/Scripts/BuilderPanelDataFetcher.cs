using System.Collections;
using System.Collections.Generic;
using BLD.Builder;
using BLD.Helpers;
using UnityEngine;

public static class BuilderPanelDataFetcher
{
    public static Promise<ProjectData[]> FetchProjectData(IBuilderAPIController apiController)
    {
        var promise = new Promise<ProjectData[]>();
        var manifestPromise = apiController.GetAllManifests();
        manifestPromise.Then(projectList =>
        {
            promise.Resolve(projectList.ToArray());
        });
        manifestPromise.Catch(error =>
        {
            promise.Reject(error);
        });
 
        return promise;
    }
}