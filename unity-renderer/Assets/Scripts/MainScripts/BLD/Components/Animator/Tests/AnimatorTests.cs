using BLD.Components;
using BLD.Helpers;
using BLD.Models;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections;
using System.Linq;
using BLD.Controllers;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests
{
    public class AnimatorTests : IntegrationTestSuite_Legacy
    {
        private ParcelScene scene;

        protected override IEnumerator SetUp()
        {
            yield return base.SetUp();
            scene = TestUtils.CreateTestScene();
        }

        [UnityTest]
        public IEnumerator CreateAnimationComponent()
        {
            var entity = TestUtils.CreateSceneEntity(scene);

            Assert.IsTrue(entity.gameObject.GetComponentInChildren<UnityGLTF.InstantiatedGLTFObject>() == null,
                "Since the shape hasn't been updated yet, the 'GLTFScene' child object shouldn't exist");

            TestUtils.CreateAndSetShape(scene, entity.entityId, BLD.Models.CLASS_ID.GLTF_SHAPE,
                JsonConvert.SerializeObject(new
                {
                    src = TestAssetsUtils.GetPath() + "/GLB/CesiumMan/CesiumMan.glb"
                }));

            BLDAnimator.Model animatorModel = new BLDAnimator.Model
            {
                states = new []
                {
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "clip01",
                        clip = "animation:0",
                        playing = true,
                        weight = 1,
                        speed = 1
                    }
                }
            };

            BLDAnimator animator =
                TestUtils.EntityComponentCreate<BLDAnimator, BLDAnimator.Model>(scene, entity, animatorModel);

            LoadWrapper gltfShape = GLTFShape.GetLoaderForEntity(entity);
            yield return new WaitUntil(() => gltfShape.alreadyLoaded == true);

            Assert.IsTrue(entity.gameObject.GetComponentInChildren<Animation>() != null,
                "'GLTFScene' child object with 'Animator' component should exist if the GLTF was loaded correctly.");
            Assert.IsTrue(entity.gameObject.GetComponentInChildren<BLDAnimator>() != null,
                "'GLTFScene' child object with 'BLDAnimator' component should exist if the GLTF was loaded correctly.");

            yield return animator.routine;

            animator = entity.gameObject.GetComponentInChildren<BLDAnimator>();

            Assert.IsTrue(animator.GetStateByString("clip01") != null, "bldAnimator.GetStateByString fail!");
            Assert.IsTrue(animator.GetModel().states[0].clip != null, "bldAnimator clipReference is null!");
        }

        [UnityTest]
        public IEnumerator BLDAnimatorResetAnimation()
        {
            GLTFShape gltfShape = TestUtils.CreateEntityWithGLTFShape(scene, Vector3.zero,
                new LoadableShape.Model
                {
                    src = TestAssetsUtils.GetPath() + "/GLB/Shark/shark_anim.gltf"
                });
            var entity = gltfShape.attachedEntities.First();

            BLDAnimator.Model animatorModel = new BLDAnimator.Model
            {
                states = new[]
                {
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "Bite",
                        clip = "shark_skeleton_bite",
                        playing = true,
                        weight = 1,
                        speed = 1
                    },
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "Swim",
                        clip = "shark_skeleton_swim",
                        playing = true,
                        weight = 1,
                        speed = 1
                    }
                }
            };

            BLDAnimator animator =
                TestUtils.EntityComponentCreate<BLDAnimator, BLDAnimator.Model>(scene, entity, animatorModel);
            LoadWrapper gltfLoader = GLTFShape.GetLoaderForEntity(entity);
            yield return new WaitUntil(() => gltfLoader.alreadyLoaded);

            yield return animator.routine;

            animator.animComponent.cullingType = AnimationCullingType.AlwaysAnimate;

            yield return null;

            Animation animation = entity.gameObject.GetComponentInChildren<Animation>();
            foreach (AnimationState animState in animation)
            {
                Assert.AreNotEqual(0f, animState.time);
            }

            animatorModel.states[1].shouldReset = true;

            yield return TestUtils.EntityComponentUpdate(animator, animatorModel);

            animator.ResetAnimation(animator.GetStateByString("Swim"));
            foreach (AnimationState animState in animation)
            {
                if (animator.GetStateByString("Swim").clipReference.name == animState.clip.name)
                {
                    Assert.AreEqual(0f, animState.time);
                }
                else
                {
                    Assert.AreNotEqual(0f, animState.time);
                }
            }
        }

        [UnityTest]
        public IEnumerator BLDAnimatorResetAllAnimations()
        {
            var gltfShape = TestUtils.CreateEntityWithGLTFShape(scene, Vector3.zero,
                new LoadableShape.Model
                {
                    src = TestAssetsUtils.GetPath() + "/GLB/Shark/shark_anim.gltf"
                });
            var entity = gltfShape.attachedEntities.First();

            BLDAnimator.Model animatorModel = new BLDAnimator.Model
            {
                states = new[]
                {
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "Bite",
                        clip = "shark_skeleton_bite",
                        playing = true,
                        weight = 1,
                        speed = 1
                    },
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "Swim",
                        clip = "shark_skeleton_swim",
                        playing = true,
                        weight = 1,
                        speed = 1
                    }
                }
            };

            BLDAnimator animator =
                TestUtils.EntityComponentCreate<BLDAnimator, BLDAnimator.Model>(scene, entity, animatorModel);
            LoadWrapper gltfLoader = GLTFShape.GetLoaderForEntity(entity);
            yield return new WaitUntil(() => gltfLoader.alreadyLoaded);

            yield return animator.routine;

            animator.animComponent.cullingType = AnimationCullingType.AlwaysAnimate;

            yield return null;

            Animation animation = entity.gameObject.GetComponentInChildren<Animation>();

            foreach (AnimationState animState in animation)
            {
                Assert.AreNotEqual(0f, animState.time);
            }

            animatorModel.states[0].shouldReset = true;
            animatorModel.states[1].shouldReset = true;

            yield return TestUtils.EntityComponentUpdate(animator, animatorModel);

            foreach (AnimationState animState in animation)
            {
                Assert.AreEqual(0f, animState.time);
            }
        }

        [UnityTest]
        public IEnumerator AnimationComponentMissingValuesGetDefaultedOnUpdate() { yield return TestUtils.TestEntityComponentDefaultsOnUpdate<BLDAnimator.Model, BLDAnimator>(scene); }

        [UnityTest]
        public IEnumerator UpdateAnimationComponent()
        {
            var entity = TestUtils.CreateSceneEntity(scene);

            Assert.IsTrue(entity.gameObject.GetComponentInChildren<UnityGLTF.InstantiatedGLTFObject>() == null,
                "Since the shape hasn't been updated yet, the 'GLTFScene' child object shouldn't exist");

            TestUtils.CreateAndSetShape(scene, entity.entityId, BLD.Models.CLASS_ID.GLTF_SHAPE,
                JsonConvert.SerializeObject(new
                {
                    src = TestAssetsUtils.GetPath() + "/GLB/CesiumMan/CesiumMan.glb"
                }));

            string clipName = "animation:0";
            BLDAnimator.Model animatorModel = new BLDAnimator.Model
            {
                states = new BLDAnimator.Model.BLDAnimationState[]
                {
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "clip01",
                        clip = clipName,
                        playing = true,
                        weight = 1,
                        speed = 1,
                        looping = false
                    }
                }
            };

            BLDAnimator animator = TestUtils.EntityComponentCreate<BLDAnimator, BLDAnimator.Model>(scene, entity, animatorModel);

            LoadWrapper gltfShape = GLTFShape.GetLoaderForEntity(entity);
            yield return new WaitUntil(() => gltfShape.alreadyLoaded == true);

            Assert.IsTrue(animator.animComponent.isPlaying);
            Assert.AreEqual(animator.animComponent.clip.name, clipName);
            Assert.IsFalse(animator.animComponent.clip.wrapMode == WrapMode.Loop);

            yield return null;

            // update component properties
            animatorModel.states[0].playing = false;
            animatorModel.states[0].looping = true;
            yield return TestUtils.EntityComponentUpdate(animator, animatorModel);

            Assert.IsFalse(animator.animComponent.isPlaying);
            Assert.IsTrue(animator.animComponent.clip.wrapMode == WrapMode.Loop);
        }

        [UnityTest]
        [Explicit]
        [Category("Explicit")]
        public IEnumerator AnimationStartsAutomaticallyWithNoBLDAnimator()
        {
            // GLTFShape without BLDAnimator
            var entity = TestUtils.CreateSceneEntity(scene);

            Assert.IsTrue(entity.gameObject.GetComponentInChildren<UnityGLTF.InstantiatedGLTFObject>() == null,
                "Since the shape hasn't been updated yet, the 'GLTFScene' child object shouldn't exist");

            TestUtils.CreateAndSetShape(scene, entity.entityId, BLD.Models.CLASS_ID.GLTF_SHAPE,
                JsonConvert.SerializeObject(new
                {
                    src = TestAssetsUtils.GetPath() + "/GLB/CesiumMan/CesiumMan.glb"
                }));

            LoadWrapper gltfShape = GLTFShape.GetLoaderForEntity(entity);
            yield return new WaitUntil(() => gltfShape.alreadyLoaded == true);

            Animation animation = entity.meshRootGameObject.GetComponentInChildren<Animation>();

            Assert.IsTrue(animation != null);
            Assert.IsTrue(animation.isPlaying);

            // GLTFShape with BLDAnimator
            var entity2 = TestUtils.CreateSceneEntity(scene);

            Assert.IsTrue(entity2.gameObject.GetComponentInChildren<UnityGLTF.InstantiatedGLTFObject>() == null,
                "Since the shape hasn't been updated yet, the 'GLTFScene' child object shouldn't exist");

            TestUtils.CreateAndSetShape(scene, entity2.entityId, BLD.Models.CLASS_ID.GLTF_SHAPE,
                JsonConvert.SerializeObject(new
                {
                    src = TestAssetsUtils.GetPath() + "/GLB/CesiumMan/CesiumMan.glb"
                }));

            string clipName = "animation:0";
            BLDAnimator.Model animatorModel = new BLDAnimator.Model
            {
                states = new BLDAnimator.Model.BLDAnimationState[]
                {
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "clip01",
                        clip = clipName,
                        playing = false,
                        weight = 1,
                        speed = 1,
                        looping = false
                    }
                }
            };

            BLDAnimator animator = TestUtils.EntityComponentCreate<BLDAnimator, BLDAnimator.Model>(scene, entity, animatorModel);

            LoadWrapper gltfShape2 = GLTFShape.GetLoaderForEntity(entity);
            yield return new WaitUntil(() => gltfShape2.alreadyLoaded == true);

            Assert.IsTrue(animator.animComponent != null);
            Assert.AreEqual(animator.animComponent.clip.name, clipName);
            Assert.IsFalse(animator.animComponent.isPlaying);
        }

        [UnityTest]
        public IEnumerator NonSkeletalAnimationsSupport()
        {
            var entity = TestUtils.CreateSceneEntity(scene);

            TestUtils.SetEntityTransform(scene, entity, new Vector3(8, 2, 8), Quaternion.identity, Vector3.one);

            Assert.IsTrue(entity.gameObject.GetComponentInChildren<UnityGLTF.InstantiatedGLTFObject>() == null,
                "Since the shape hasn't been updated yet, the 'GLTFScene' child object shouldn't exist");

            TestUtils.CreateAndSetShape(scene, entity.entityId, BLD.Models.CLASS_ID.GLTF_SHAPE,
                JsonConvert.SerializeObject(new
                {
                    src = TestAssetsUtils.GetPath() + "/GLB/non-skeletal-3-transformations.glb"
                }));

            string clipName = "All";
            BLDAnimator.Model animatorModel = new BLDAnimator.Model
            {
                states = new BLDAnimator.Model.BLDAnimationState[]
                {
                    new BLDAnimator.Model.BLDAnimationState
                    {
                        name = "clip01",
                        clip = clipName,
                        playing = false,
                        weight = 1,
                        speed = 1,
                        looping = false
                    }
                }
            };

            BLDAnimator animator = TestUtils.EntityComponentCreate<BLDAnimator, BLDAnimator.Model>(scene, entity, animatorModel);

            LoadWrapper gltfShape = GLTFShape.GetLoaderForEntity(entity);
            yield return new WaitUntil(() => gltfShape.alreadyLoaded == true);

            animator.animComponent.cullingType = AnimationCullingType.AlwaysAnimate;

            Assert.IsTrue(!animator.animComponent.isPlaying);
            Assert.AreEqual(animator.animComponent.clip.name, clipName);
            Assert.IsFalse(animator.animComponent.clip.wrapMode == WrapMode.Loop);

            Transform animatedGameObject = animator.animComponent.transform.GetChild(0);

            Vector3 originalScale = animatedGameObject.transform.localScale;
            Vector3 originalPos = animatedGameObject.transform.localPosition;
            Quaternion originalRot = animatedGameObject.transform.localRotation;

            // start animation
            animatorModel.states[0].playing = true;
            yield return TestUtils.EntityComponentUpdate(animator, animatorModel);

            yield return new WaitForSeconds(0.1f);

            Assert.IsFalse(animatedGameObject.localScale == originalScale);
            Assert.IsFalse(animatedGameObject.localPosition == originalPos);
            Assert.IsFalse(animatedGameObject.localRotation == originalRot);
        }
    }
}