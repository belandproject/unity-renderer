using BLD;
using BLD.Components;
using BLD.Controllers;
using BLD.Helpers;
using BLD.Models;
using UnityEngine;

public class EntityMaterialUpdateTestController : MonoBehaviour
{
    void Start()
    {
        var sceneController = Environment.i.world.sceneController;
        var scenesToLoad = (Resources.Load("TestJSON/SceneLoadingTest") as TextAsset).text;

        sceneController.UnloadAllScenes();
        sceneController.LoadParcelScenes(scenesToLoad);

        var scene = Environment.i.world.state.loadedScenes["0,0"] as ParcelScene;

        BLDTexture bldAtlasTexture = TestUtils.CreateBLDTexture(
            scene,
            TestAssetsUtils.GetPath() + "/Images/atlas.png",
            BLDTexture.BabylonWrapMode.CLAMP,
            FilterMode.Bilinear);

        BLDTexture bldAvatarTexture = TestUtils.CreateBLDTexture(
            scene,
            TestAssetsUtils.GetPath() + "/Images/avatar.png",
            BLDTexture.BabylonWrapMode.CLAMP,
            FilterMode.Bilinear);


        IBLDEntity entity;

        TestUtils.CreateEntityWithBasicMaterial(
            scene,
            new BasicMaterial.Model
            {
                texture = bldAtlasTexture.id,
            },
            out entity);

        TestUtils.CreateEntityWithPBRMaterial(scene,
            new PBRMaterial.Model
            {
                albedoTexture = bldAvatarTexture.id,
                metallic = 0,
                roughness = 1,
            },
            out entity);

        PBRMaterial mat = TestUtils.CreateEntityWithPBRMaterial(scene,
            new PBRMaterial.Model
            {
                albedoTexture = bldAvatarTexture.id,
                metallic = 1,
                roughness = 1,
                alphaTexture = bldAvatarTexture.id,
            },
            out entity);

        // Re-assign last PBR material to new entity
        BoxShape shape = TestUtils.CreateEntityWithBoxShape(scene, new Vector3(5, 1, 2));
        BasicMaterial m =
            TestUtils.SharedComponentCreate<BasicMaterial, BasicMaterial.Model>(scene, CLASS_ID.BASIC_MATERIAL);

        Color color1;
        ColorUtility.TryParseHtmlString("#FF9292", out color1);

        // Update material attached to 2 entities, adding albedoColor
        scene.SharedComponentUpdate(m.id, JsonUtility.ToJson(new BLD.Components.PBRMaterial.Model
        {
            albedoTexture = bldAvatarTexture.id,
            metallic = 1,
            roughness = 1,
            alphaTexture = bldAvatarTexture.id,
            albedoColor = color1
        }));
    }
}