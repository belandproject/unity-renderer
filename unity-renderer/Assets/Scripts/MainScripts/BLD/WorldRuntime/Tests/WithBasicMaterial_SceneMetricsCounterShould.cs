using System.Collections;
using BLD;
using BLD.Components;
using BLD.Helpers;
using BLD.Models;
using NUnit.Framework;
using UnityEngine.TestTools;

public class WithBasicMaterial_SceneMetricsCounterShould : IntegrationTestSuite_SceneMetricsCounter
{
    [UnityTest]
    public IEnumerator NotCountBasicMaterialsWhenNoShapeIsPresent()
    {
        BasicMaterial material1 = CreateBasicMaterial("");

        yield return material1.routine;

        Assert.That( scene.metricsCounter.currentCount.materials, Is.EqualTo(0) );
    }


    [UnityTest]
    public IEnumerator NotCountWhenAttachedToIgnoredEntities()
    {
        IBLDEntity entity = CreateEntityWithTransform();
        DataStore.i.sceneWorldObjects.AddExcludedOwner(scene.sceneData.id, entity.entityId);

        BLDTexture texture = CreateTexture(texturePaths[0]);
        BasicMaterial material = CreateBasicMaterial(texture.id);
        PlaneShape planeShape = CreatePlane();

        yield return texture.routine;

        TestUtils.SharedComponentAttach(texture, entity);
        TestUtils.SharedComponentAttach(material, entity);
        TestUtils.SharedComponentAttach(planeShape, entity);

        var sceneMetrics = scene.metricsCounter.currentCount;

        Assert.That( sceneMetrics.materials, Is.EqualTo(0) );

        material.Dispose();
        texture.Dispose();
        planeShape.Dispose();
        DataStore.i.sceneWorldObjects.RemoveExcludedOwner(scene.sceneData.id, entity.entityId);
    }


    [UnityTest]
    public IEnumerator CountWhenAdded()
    {
        IBLDEntity entity = CreateEntityWithTransform();
        BasicMaterial material = CreateBasicMaterial("");
        PlaneShape planeShape = CreatePlane();

        TestUtils.SharedComponentAttach(material, entity);
        TestUtils.SharedComponentAttach(planeShape, entity);

        yield return planeShape.routine;

        var sceneMetrics = scene.metricsCounter.currentCount;

        Assert.That( sceneMetrics.materials, Is.EqualTo(1) );

        material.Dispose();
        planeShape.Dispose();
    }

    [UnityTest]
    public IEnumerator CountWhenRemoved()
    {
        IBLDEntity entity = CreateEntityWithTransform();
        BasicMaterial material = CreateBasicMaterial("");
        PlaneShape planeShape = CreatePlane();

        TestUtils.SharedComponentAttach(material, entity);
        TestUtils.SharedComponentAttach(planeShape, entity);

        yield return planeShape.routine;

        material.Dispose();
        planeShape.Dispose();

        var sceneMetrics = scene.metricsCounter.currentCount;

        Assert.That( sceneMetrics.materials, Is.EqualTo(0) );
    }
}