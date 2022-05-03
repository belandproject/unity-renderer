using BLD;
using BLD.Components;
using BLD.Helpers;
using BLD.Models;
using NUnit.Framework;
using System.Collections;
using BLD.Controllers;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;

namespace Tests
{
    public class FontTests : IntegrationTestSuite
    {
        const string TEST_BUILTIN_FONT_NAME = "builtin:SF-UI-Text-Regular SDF";

        private ParcelScene scene;

        protected override void InitializeServices(ServiceLocator serviceLocator)
        {
            serviceLocator.Register<ISceneController>(() => new SceneController());
            serviceLocator.Register<IWorldState>(() => new WorldState());
            serviceLocator.Register<IRuntimeComponentFactory>(() => new RuntimeComponentFactory());
        }

        [UnitySetUp]
        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            scene = TestUtils.CreateTestScene();
        }

        [UnityTest]
        public IEnumerator BuiltInFontCreateAndLoadTest()
        {
            BLDFont font =
                TestUtils.SharedComponentCreate<BLDFont, BLDFont.Model>(scene, CLASS_ID.FONT, new BLDFont.Model() { src = TEST_BUILTIN_FONT_NAME });
            yield return font.routine;

            var entity = TestUtils.CreateSceneEntity(scene);

            TextShape textShape =
                TestUtils.EntityComponentCreate<TextShape, TextShape.Model>(scene, entity, new TextShape.Model() { font = font.id });
            yield return textShape.routine;

            Assert.IsTrue(font.loaded, "Built-in font didn't load");
            Assert.IsFalse(font.error, "Built-in font has error");

            TextMeshPro tmpro = textShape.GetComponentInChildren<TextMeshPro>();
            Assert.IsTrue(font.fontAsset == tmpro.font, "Built-in font didn't apply correctly");
        }

        [UnityTest]
        public IEnumerator BuiltInFontHandleErrorProperly()
        {
            var entity = TestUtils.CreateSceneEntity(scene);

            TextShape textShape =
                TestUtils.EntityComponentCreate<TextShape, TextShape.Model>(scene, entity, new TextShape.Model());
            yield return textShape.routine;

            TMP_FontAsset defaultFont = textShape.GetComponentInChildren<TextMeshPro>().font;

            BLDFont font =
                TestUtils.SharedComponentCreate<BLDFont, BLDFont.Model>(scene, CLASS_ID.FONT, new BLDFont.Model() { src = "no-valid-font" });
            yield return font.routine;

            scene.EntityComponentUpdate(entity, CLASS_ID_COMPONENT.TEXT_SHAPE,
                JsonUtility.ToJson(new TextShape.Model { font = font.id }));
            yield return textShape.routine;

            Assert.IsTrue(font.error, "Built-in font error has not araise properly");
            Assert.IsTrue(textShape.GetComponentInChildren<TextMeshPro>().font == defaultFont, "Built-in font didn't apply correctly");
        }

        [UnityTest]
        public IEnumerator BuiltInFontAttachCorrectlyOnTextComponentUpdate()
        {
            var entity = TestUtils.CreateSceneEntity(scene);

            TextShape textShape =
                TestUtils.EntityComponentCreate<TextShape, TextShape.Model>(scene, entity, new TextShape.Model());
            yield return textShape.routine;

            BLDFont font =
                TestUtils.SharedComponentCreate<BLDFont, BLDFont.Model>(scene, CLASS_ID.FONT, new BLDFont.Model() { src = TEST_BUILTIN_FONT_NAME });
            yield return font.routine;

            scene.EntityComponentUpdate(entity, CLASS_ID_COMPONENT.TEXT_SHAPE,
                JsonUtility.ToJson(new TextShape.Model { font = font.id }));
            yield return textShape.routine;

            Assert.IsTrue(font.loaded, "Built-in font didn't load");
            Assert.IsFalse(font.error, "Built-in font has error");

            TextMeshPro tmpro = textShape.GetComponentInChildren<TextMeshPro>();
            Assert.IsTrue(font.fontAsset == tmpro.font, "Built-in font didn't apply correctly");
        }
    }
}