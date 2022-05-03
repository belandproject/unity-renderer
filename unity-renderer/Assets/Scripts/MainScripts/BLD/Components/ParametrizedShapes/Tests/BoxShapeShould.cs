using System.Collections;
using BLD.Components;
using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class BoxShapeShould : IntegrationTestSuite_Legacy
{
    private ParcelScene scene;

    protected override IEnumerator SetUp()
    {
        yield return base.SetUp();
        scene = TestUtils.CreateTestScene();
    }

    [UnityTest]
    public IEnumerator BeUpdatedCorrectly()
    {
        string entityId = "3";
        TestUtils.InstantiateEntityWithShape(scene, entityId, BLD.Models.CLASS_ID.BOX_SHAPE, Vector3.zero);

        var meshName = scene.entities[entityId].gameObject.GetComponentInChildren<MeshFilter>().mesh.name;
        Assert.AreEqual("BLD Box Instance", meshName);
        yield break;
    }

    [UnityTest]
    public IEnumerator UpdateUVsCorrectly()
    {
        float[] uvs = new float[]
        {
            0, 0.75f, 0.25f, 0.75f, 0.25f, 1, 0, 1,
            0, 0.75f, 0.25f, 0.75f, 0.25f, 1, 0, 1,
            0, 0.75f, 0.25f, 0.75f, 0.25f, 1, 0, 1,
            0, 0.75f, 0.25f, 0.75f, 0.25f, 1, 0, 1,
            0, 0.75f, 0.25f, 0.75f, 0.25f, 1, 0, 1,
            0, 0.75f, 0.25f, 0.75f, 0.25f, 1, 0, 1
        };

        IBLDEntity entity;

        BoxShape box = TestUtils.InstantiateEntityWithShape<BoxShape, BoxShape.Model>(
            scene,
            BLD.Models.CLASS_ID.BOX_SHAPE,
            Vector3.zero,
            out entity,
            new BoxShape.Model()
            {
                uvs = uvs
            });

        yield return box.routine;

        Assert.IsTrue(entity != null);
        Assert.IsTrue(box != null);
        Assert.IsTrue(box.currentMesh != null);
        CollectionAssert.AreEqual(Utils.FloatArrayToV2List(uvs), box.currentMesh.uv);
    }

    [UnityTest]
    public IEnumerator DefaultMissingValuesOnUpdate()
    {
        var component =
            TestUtils.SharedComponentCreate<BoxShape, BoxShape.Model>(scene, CLASS_ID.BOX_SHAPE);
        yield return component.routine;

        Assert.IsFalse(component == null);

        yield return TestUtils.TestSharedComponentDefaultsOnUpdate<BoxShape.Model, BoxShape>(scene,
            CLASS_ID.BOX_SHAPE);
    }

    [UnityTest]
    public IEnumerator BeReplacedCorrectlyWhenAnotherComponentIsAttached()
    {
        yield return TestUtils.TestAttachedSharedComponentOfSameTypeIsReplaced<BoxShape.Model, BoxShape>(
            scene, CLASS_ID.BOX_SHAPE);
    }
}