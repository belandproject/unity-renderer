﻿using System.Collections;
using BLD;
using BLD.Components;
using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.TestTools;
using Assert = UnityEngine.Assertions.Assert;

public class BasicMaterialShould : IntegrationTestSuite_Legacy
{
    private ParcelScene scene;

    [UnitySetUp]
    protected override IEnumerator SetUp()
    {
        yield return base.SetUp();
        scene = TestUtils.CreateTestScene();
        Environment.i.world.sceneBoundsChecker.Stop();
    }

    [UnityTearDown]
    protected override IEnumerator TearDown()
    {
        Object.Destroy(scene.gameObject);
        yield return base.TearDown();
    }

    [UnityTest]
    public IEnumerator NotDestroySharedTextureWhenDisposed()
    {
        BLDTexture texture =
            TestUtils.CreateBLDTexture(scene, TestAssetsUtils.GetPath() + "/Images/atlas.png");

        yield return texture.routine;

        BasicMaterial mat = TestUtils.CreateEntityWithBasicMaterial(scene,
            new BasicMaterial.Model
            {
                texture = texture.id,
                alphaTest = 1,
            },
            out IBLDEntity entity1);

        yield return mat.routine;

        BasicMaterial mat2 = TestUtils.CreateEntityWithBasicMaterial(scene,
            new BasicMaterial.Model
            {
                texture = texture.id,
                alphaTest = 1,
            },
            out IBLDEntity entity2);

        yield return mat2.routine;

        TestUtils.SharedComponentDispose(mat);
        Assert.IsTrue(texture.texture != null, "Texture should persist because is used by the other material!!");
    }

    [UnityTest]
    public IEnumerator WorkCorrectlyWhenAttachedBeforeShape()
    {
        IBLDEntity entity = TestUtils.CreateSceneEntity(scene);

        BLDTexture bldTexture = TestUtils.CreateBLDTexture(
            scene,
            TestAssetsUtils.GetPath() + "/Images/atlas.png",
            BLDTexture.BabylonWrapMode.CLAMP,
            FilterMode.Bilinear);

        yield return bldTexture.routine;

        BasicMaterial mat = TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>
        (scene, CLASS_ID.BASIC_MATERIAL,
            new BasicMaterial.Model
            {
                texture = bldTexture.id,
                alphaTest = 0.5f
            });

        yield return mat.routine;

        TestUtils.SharedComponentAttach(mat, entity);

        SphereShape shape = TestUtils.SharedComponentCreate<SphereShape, SphereShape.Model>(scene,
            CLASS_ID.SPHERE_SHAPE,
            new SphereShape.Model { });

        TestUtils.SharedComponentAttach(shape, entity);

        Assert.IsTrue(entity.meshRootGameObject != null);
        Assert.IsTrue(entity.meshRootGameObject.GetComponent<MeshRenderer>() != null);
        Assert.AreEqual(entity.meshRootGameObject.GetComponent<MeshRenderer>().sharedMaterial, mat.material);
    }

    [UnityTest]
    public IEnumerator GetReplacedWhenAnotherMaterialIsAttached()
    {
        yield return
            TestUtils.TestAttachedSharedComponentOfSameTypeIsReplaced<BasicMaterial.Model, BasicMaterial>(scene,
                CLASS_ID.BASIC_MATERIAL);
    }

    [UnityTest]
    public IEnumerator BeDetachedCorrectly()
    {
        string entityId = "1";
        string materialID = "a-material";

        TestUtils.InstantiateEntityWithMaterial(scene, entityId, Vector3.zero,
            new BasicMaterial.Model(), materialID);

        Assert.IsTrue(scene.entities[entityId].meshRootGameObject != null,
            "Every entity with a shape should have the mandatory 'Mesh' object as a child");

        var meshRenderer = scene.entities[entityId].meshRootGameObject.GetComponent<MeshRenderer>();
        var materialComponent = scene.disposableComponents[materialID] as BasicMaterial;

        yield return materialComponent.routine;

        // Check if material initialized correctly
        {
            Assert.IsTrue(meshRenderer != null, "MeshRenderer must exist");

            Assert.AreEqual(meshRenderer.sharedMaterial, materialComponent.material, "Assigned material");
        }

        // Remove material
        materialComponent.DetachFrom(scene.entities[entityId]);

        // Check if material was removed correctly
        Assert.IsTrue(meshRenderer.sharedMaterial == null,
            "Assigned material should be null as it has been removed");
    }

    [UnityTest]
    public IEnumerator BeDetachedOnDispose()
    {
        string firstEntityId = "1";
        string secondEntityId = "2";
        string materialID = "a-material";

        // Instantiate entity with material
        TestUtils.InstantiateEntityWithMaterial(scene, firstEntityId, Vector3.zero,
            new BasicMaterial.Model(), materialID);

        Assert.IsTrue(scene.entities[firstEntityId].meshRootGameObject != null,
            "Every entity with a shape should have the mandatory 'Mesh' object as a child");

        // Create 2nd entity and attach same material to it
        TestUtils.InstantiateEntityWithShape(scene, secondEntityId, CLASS_ID.BOX_SHAPE, Vector3.zero);
        scene.SharedComponentAttach(
            secondEntityId,
            materialID
        );

        Assert.IsTrue(scene.entities[secondEntityId].meshRootGameObject != null,
            "Every entity with a shape should have the mandatory 'Mesh' object as a child");

        var firstMeshRenderer = scene.entities[firstEntityId].meshRootGameObject.GetComponent<MeshRenderer>();
        var secondMeshRenderer = scene.entities[secondEntityId].meshRootGameObject.GetComponent<MeshRenderer>();
        var materialComponent = scene.disposableComponents[materialID] as BLD.Components.BasicMaterial;

        yield return materialComponent.routine;

        // Check if material attached correctly
        {
            Assert.IsTrue(firstMeshRenderer != null, "MeshRenderer must exist");
            Assert.AreEqual(firstMeshRenderer.sharedMaterial, materialComponent.material, "Assigned material");

            Assert.IsTrue(secondMeshRenderer != null, "MeshRenderer must exist");
            Assert.AreEqual(secondMeshRenderer.sharedMaterial, materialComponent.material, "Assigned material");
        }

        // Dispose material
        scene.SharedComponentDispose(materialID);

        // Check if material detached correctly
        Assert.IsTrue(firstMeshRenderer.sharedMaterial == null, "MeshRenderer must exist");
        Assert.IsTrue(secondMeshRenderer.sharedMaterial == null, "MeshRenderer must exist");
    }

    [UnityTest]
    public IEnumerator EntityBasicMaterialUpdate()
    {
        string entityId = "1";
        string materialID = "a-material";

        Assert.IsFalse(scene.disposableComponents.ContainsKey(materialID));

        // Instantiate entity with default material
        TestUtils.InstantiateEntityWithMaterial(scene, entityId, new Vector3(8, 1, 8),
            new BasicMaterial.Model(), materialID);

        var meshObject = scene.entities[entityId].meshRootGameObject;
        Assert.IsTrue(meshObject != null,
            "Every entity with a shape should have the mandatory 'Mesh' object as a child");

        var meshRenderer = meshObject.GetComponent<MeshRenderer>();
        var materialComponent = scene.disposableComponents[materialID] as BasicMaterial;

        yield return materialComponent.routine;

        // Check if material initialized correctly
        {
            Assert.IsTrue(meshRenderer != null, "MeshRenderer must exist");

            var assignedMaterial = meshRenderer.sharedMaterial;
            Assert.IsTrue(meshRenderer != null, "MeshRenderer.sharedMaterial must be the same as assignedMaterial");

            Assert.AreEqual(assignedMaterial, materialComponent.material, "Assigned material");
        }

        // Check default properties
        {
            Assert.IsTrue(materialComponent.material.GetTexture("_BaseMap") == null);
            Assert.AreApproximatelyEqual(1.0f, materialComponent.material.GetFloat("_AlphaClip"));
        }

        BLDTexture bldTexture = TestUtils.CreateBLDTexture(
            scene,
            TestAssetsUtils.GetPath() + "/Images/atlas.png",
            BLDTexture.BabylonWrapMode.MIRROR,
            FilterMode.Bilinear);

        // Update material
        scene.SharedComponentUpdate(materialID, JsonUtility.ToJson(new BasicMaterial.Model
        {
            texture = bldTexture.id,
            alphaTest = 0.5f,
        }));

        yield return materialComponent.routine;
        yield return bldTexture.routine;

        // Check updated properties
        {
            Texture mainTex = materialComponent.material.GetTexture("_BaseMap");
            Assert.IsTrue(mainTex != null);
            Assert.AreApproximatelyEqual(0.5f, materialComponent.material.GetFloat("_Cutoff"));
            Assert.AreApproximatelyEqual(1.0f, materialComponent.material.GetFloat("_AlphaClip"));
            Assert.AreEqual(TextureWrapMode.Mirror, mainTex.wrapMode);
            Assert.AreEqual(FilterMode.Bilinear, mainTex.filterMode);
        }

        bldTexture.Dispose();
        materialComponent.Dispose();
    }

    [UnityTest]
    public IEnumerator DefaultMissingValuesPropertyOnUpdate()
    {
        // 1. Create component with non-default configs
        BasicMaterial basicMaterialComponent =
            TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL,
                new BasicMaterial.Model
                {
                    alphaTest = 1f
                });

        yield return basicMaterialComponent.routine;

        // 2. Check configured values
        Assert.AreEqual(1f, basicMaterialComponent.GetModel().alphaTest);

        // 3. Update component with missing values

        scene.SharedComponentUpdate(basicMaterialComponent.id, JsonUtility.ToJson(new BasicMaterial.Model { }));

        yield return basicMaterialComponent.routine;

        // 4. Check defaulted values
        Assert.AreEqual(0.5f, basicMaterialComponent.GetModel().alphaTest);
    }

    [UnityTest]
    public IEnumerator ProcessCastShadowProperty_True()
    {
        BasicMaterial basicMaterialComponent = TestUtils.CreateEntityWithBasicMaterial(scene, new BasicMaterial.Model
        {
            alphaTest = 1f,
            castShadows = true
        }, out IBLDEntity entity);
        yield return basicMaterialComponent.routine;

        Assert.AreEqual(true, basicMaterialComponent.GetModel().castShadows);
        Assert.AreEqual(ShadowCastingMode.On, entity.meshRootGameObject.GetComponent<MeshRenderer>().shadowCastingMode);
    }

    [UnityTest]
    public IEnumerator ProcessCastShadowProperty_False()
    {
        BasicMaterial basicMaterialComponent = TestUtils.CreateEntityWithBasicMaterial(scene, new BasicMaterial.Model
        {
            alphaTest = 1f,
            castShadows = false
        }, out IBLDEntity entity);
        yield return basicMaterialComponent.routine;

        Assert.AreEqual(false, basicMaterialComponent.GetModel().castShadows);
        Assert.AreEqual(ShadowCastingMode.Off, entity.meshRootGameObject.GetComponent<MeshRenderer>().shadowCastingMode);
    }
}