using System.Collections;
using BLD;
using BLD.Components;
using BLD.Helpers;
using BLD.Models;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class WithGltfShape_SceneMetricsCounterShould : IntegrationTestSuite_SceneMetricsCounter
{
    [UnityTest]
    public IEnumerator NotCountWhenAttachedToIgnoredEntities()
    {
        IBLDEntity entity = TestUtils.CreateSceneEntity(scene);
        DataStore.i.sceneWorldObjects.AddExcludedOwner(scene.sceneData.id, entity.entityId);
        GLTFShape gltfShape = TestUtils.AttachGLTFShape(entity,
            scene,
            new Vector3(8, 1, 8),
            new LoadableShape.Model()
            {
                src = TestAssetsUtils.GetPath() + "/GLB/Trunk/Trunk.glb"
            });

        yield return TestUtils.WaitForGLTFLoad(entity);

        SceneMetricsModel inputModel = scene.metricsCounter.currentCount;

        Assert.That( inputModel.bodies, Is.EqualTo(0), "Bodies should not be counted!" );
        Assert.That( inputModel.materials, Is.EqualTo(0), "Materials should not be counted!" );
        Assert.That( inputModel.textures, Is.EqualTo(0), "Textures should not be counted!" );
        Assert.That( inputModel.entities, Is.EqualTo(0), "Entities should not be counted!" );
    }

    [UnityTest]
    public IEnumerator CountWhenAdded()
    {
        IBLDEntity entity1 = TestUtils.CreateSceneEntity(scene);
        GLTFShape entity1shape = TestUtils.AttachGLTFShape(entity1,
            scene,
            new Vector3(8, 1, 8),
            new LoadableShape.Model()
            {
                src = TestAssetsUtils.GetPath() + "/GLB/Trunk/Trunk.glb"
            });

        yield return TestUtils.WaitForGLTFLoad(entity1);

        IBLDEntity entity2 = TestUtils.CreateSceneEntity(scene);
        GLTFShape entity2shape = TestUtils.AttachGLTFShape(entity2,
            scene,
            new Vector3(8, 1, 8),
            new LoadableShape.Model()
            {
                src = TestAssetsUtils.GetPath() + "/GLB/Trunk/Trunk.glb"
            });

        yield return TestUtils.WaitForGLTFLoad(entity2);

        yield return null;

        AssertMetricsModel(scene,
            triangles: 612,
            materials: 1,
            entities: 2,
            meshes: 1,
            bodies: 2,
            textures: 1);

        yield return null;
    }

    [UnityTest]
    public IEnumerator CountWhenRemoved()
    {
        IBLDEntity entity1 = TestUtils.CreateSceneEntity(scene);
        GLTFShape entity1shape = TestUtils.AttachGLTFShape(entity1,
            scene,
            new Vector3(8, 1, 8),
            new LoadableShape.Model()
            {
                src = TestAssetsUtils.GetPath() + "/GLB/Trunk/Trunk.glb"
            });

        yield return TestUtils.WaitForGLTFLoad(entity1);

        IBLDEntity entity2 = TestUtils.CreateSceneEntity(scene);
        GLTFShape entity2shape = TestUtils.AttachGLTFShape(entity2,
            scene,
            new Vector3(8, 1, 8),
            new LoadableShape.Model()
            {
                src = TestAssetsUtils.GetPath() + "/GLB/Trunk/Trunk.glb"
            });

        yield return TestUtils.WaitForGLTFLoad(entity2);

        entity1shape.Dispose();
        entity2shape.Dispose();

        yield return null;

        AssertMetricsModel(scene,
            triangles: 0,
            materials: 0,
            entities: 0,
            meshes: 0,
            bodies: 0,
            textures: 0);
    }
}