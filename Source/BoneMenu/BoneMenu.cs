using System.Collections.Generic;
using System.Linq;
using AdvancedAvatarControl.Patches;
using MelonLoader;

using BoneLib;
using BoneLib.BoneMenu.Elements;
using BoneLib.Notifications;
using LabFusion.Network;
using SLZ.VRMK;
using UnityEngine;

namespace AdvancedAvatarControl.BoneMenu
{
    public class BoneMenu
    {
        public static SkinnedMeshRenderer SelectedMeshRenderer;
        public static SkinnedMeshRenderer[] SkinnedMeshRenderers;
        private static MenuCategory blendShapes;
        private static MenuCategory meshRenderersMenu;

        private static Dictionary<int, List<float>> initialBlendShapeWeights = new Dictionary<int, List<float>>();

        public static void CreateBoneMenu()
        {
            MenuCategory menuMain = BoneLib.BoneMenu.MenuManager.CreateCategory("Advanced Avatar Control", Color.cyan);
            blendShapes = menuMain.CreateCategory("Blend Shapes", Color.green);
            MenuCategory meshRenderersCategory = blendShapes.CreateCategory("Select Mesh Renderer", Color.green);
            MenuCategory eyeMovement = menuMain.CreateCategory("Eye Movement", Color.green);

            blendShapes.CreateFunctionElement("Refresh", Color.green, () => RefreshBlendShapes(blendShapes));
            meshRenderersCategory.CreateFunctionElement("Refresh", Color.green, () => RefreshMeshRenderers(meshRenderersCategory));

#if DEBUG
            eyeMovement.CreateFunctionElement("Add Component", Color.green, () =>
            {
                if (Player.playerHead.gameObject.GetComponent<PlayerEyeController>() != null)
                {
                    MelonLogger.Msg("PlayerEyeController already exists");
                    BoneLib.Notifications.Notifier.Send(new BoneLib.Notifications.Notification()
                    {
                        Title = "Opps!",
                        ShowTitleOnPopup = true,
                        Message = $"You already have the ParlayerEyeController component silly!",
                        Type = NotificationType.Warning
                    });
                    return;
                }

                Player.playerHead.gameObject.AddComponent<PlayerEyeController>();
            });
            eyeMovement.CreateFunctionElement("Remove Component", Color.red, () =>
            {
                if (Player.playerHead.gameObject.GetComponent<PlayerEyeController>() == null)
                {
                    MelonLogger.Msg("PlayerEyeController does not exist");
                    BoneLib.Notifications.Notifier.Send(new BoneLib.Notifications.Notification()
                    {
                        Title = "Opps!",
                        ShowTitleOnPopup = true,
                        Message = $"You do not have the PlayerEyeController component silly!",
                        Type = NotificationType.Warning
                    });
                    return;
                }

                Player.playerHead.gameObject.GetComponent<PlayerEyeController>().enabled = false;
                UnityEngine.Object.Destroy(Player.playerHead.gameObject.GetComponent<PlayerEyeController>());
            });
#endif

            eyeMovement.CreateFloatElement("Movement Speed", Color.white, Prefs.EyeMovementSpeed.Value, 5, 5, 25, (float value) =>
            {
                PlayerEyeController playerEyeController =
                    Player.playerHead.gameObject.GetComponent<PlayerEyeController>();
                if (playerEyeController != null)
                {
                    Prefs.EyeMovementSpeed.Value = value;
                }
                else
                {
                    BoneLib.Notifications.Notifier.Send(new BoneLib.Notifications.Notification()
                    {
                        Title = "Opps!",
                        ShowTitleOnPopup = true,
                        Message =
                            $"The player does not seem to have the PlayerEyeController component. Please add it first.",
                        Type = NotificationType.Warning
                    });
                }
            });
        }

        public static void RefreshMeshRenderers(MenuCategory meshRenderersCategory)
        {
            meshRenderersCategory.Elements.Clear();

            meshRenderersCategory.CreateFunctionElement("Refresh", Color.green, () => RefreshMeshRenderers(meshRenderersCategory));

            SkinnedMeshRenderers = Player.GetCurrentAvatar().gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var skinnedMeshRenderer in SkinnedMeshRenderers)
            {
                meshRenderersCategory.CreateFunctionElement(skinnedMeshRenderer.gameObject.name, Color.white, () =>
                {
                    SelectedMeshRenderer = skinnedMeshRenderer;
                    
                    if (!initialBlendShapeWeights.ContainsKey(skinnedMeshRenderer.gameObject.GetInstanceID()))
                    {
                        var blendShapeWeights = new List<float>();
                        for (int i = 0; i < skinnedMeshRenderer.sharedMesh.blendShapeCount; i++)
                        {
                            blendShapeWeights.Add(skinnedMeshRenderer.GetBlendShapeWeight(i));
                        }
                        initialBlendShapeWeights[skinnedMeshRenderer.gameObject.GetInstanceID()] = blendShapeWeights;
                    }
                });
            }
        }

        public static void RefreshBlendShapes(MenuCategory blendShapes)
        {
            blendShapes.Elements.Clear();

            MenuCategory meshRenderersCategory = blendShapes.CreateCategory("Mesh Renderers", Color.green);

            meshRenderersCategory.CreateFunctionElement("Refresh", Color.green, () => RefreshMeshRenderers(meshRenderersCategory));

            blendShapes.CreateFunctionElement("Refresh", Color.green, () => RefreshBlendShapes(blendShapes));
            
            if (SelectedMeshRenderer != null && SelectedMeshRenderer.sharedMesh != null)
            {
                int blendShapeCount = SelectedMeshRenderer.sharedMesh.blendShapeCount;
                foreach (int i in Enumerable.Range(0, blendShapeCount))
                {
                    float weight = SelectedMeshRenderer.GetBlendShapeWeight(i);
                    string blendShapeName = SelectedMeshRenderer.sharedMesh.GetBlendShapeName(i);

                    FloatElement floatElement = blendShapes.CreateFloatElement(blendShapeName, Color.white, weight, 10,
                        0, 100, (float value) =>
                        {
                            if (MelonMod.FusionInstalled && NetworkInfo.HasServer)
                            {
                                FusionModule.Instance.SendBlendBoneMessage(value, i);
                            }
                            else
                            {
                                SelectedMeshRenderer.SetBlendShapeWeight(i, value);
                                MirrorExtensions.UpdateMirror();
                            }
                        });
                }
            }
        }

        public static void OnSwitchAvatar(Avatar avatar)
        {
            if (SelectedMeshRenderer != null &&
                initialBlendShapeWeights.TryGetValue(SelectedMeshRenderer.gameObject.GetInstanceID(), out var blendShapeWeights))
            {
                for (int i = 0; i < blendShapeWeights.Count; i++)
                {
                    SelectedMeshRenderer.SetBlendShapeWeight(i, blendShapeWeights[i]);
                }
            }
        }
    }
}