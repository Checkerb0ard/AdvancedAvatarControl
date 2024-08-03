using System.Collections.Generic;
using System.Linq;
using AdvancedAvatarControl.Patches;
using MelonLoader;

using BoneLib;
using BoneLib.Notifications;
using LabFusion.Network;
using UnityEngine;
using BoneLib.BoneMenu;
using Il2CppSLZ.VRMK;

namespace AdvancedAvatarControl.BoneMenu
{
    public class BoneMenu
    {
        public static SkinnedMeshRenderer SelectedMeshRenderer;
        public static SkinnedMeshRenderer[] SkinnedMeshRenderers;
        private static Page blendShapes;
        private static Page meshRenderersMenu;

        private static Dictionary<int, List<float>> initialBlendShapeWeights = new Dictionary<int, List<float>>();

        public static void CreateBoneMenu()
        {
            Page menuMain = Menu.CreatePage("Advanced Avatar Control", Color.cyan);
            blendShapes = menuMain.CreatePage("Blend Shapes", Color.green);
            Page meshRenderersCategory = blendShapes.CreatePage("Select Mesh Renderer", Color.green);
            Page eyeMovement = menuMain.CreatePage("Eye Movement", Color.green);

            blendShapes.CreateFunction("Refresh", Color.green, () => RefreshBlendShapes(blendShapes));
            meshRenderersCategory.CreateFunction("Refresh", Color.green, () => RefreshMeshRenderers(meshRenderersCategory));

#if DEBUG
            eyeMovement.CreateFunction("Add Component", Color.green, () =>
            {
                if (Player.Head.gameObject.AddComponent<PlayerEyeController>() != null)
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

                Player.Head.gameObject.AddComponent<PlayerEyeController>();
            });
            eyeMovement.CreateFunction("Remove Component", Color.red, () =>
            {
                if (Player.Head.gameObject.AddComponent<PlayerEyeController>() == null)
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

                Player.Head.gameObject.GetComponent<PlayerEyeController>().enabled = false;
                UnityEngine.Object.Destroy(Player.Head.gameObject.GetComponent<PlayerEyeController>());
            });
#endif

            eyeMovement.CreateFloat("Movement Speed", Color.white, Prefs.EyeMovementSpeed.Value, 1, 1, 25, (float value) =>
            {
                PlayerEyeController playerEyeController =
                    Player.Head.gameObject.GetComponent<PlayerEyeController>();
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

        public static void RefreshMeshRenderers(Page meshRenderersCategory)
        {
            meshRenderersCategory.RemoveAll();

            meshRenderersCategory.CreateFunction("Refresh", Color.green, () => RefreshMeshRenderers(meshRenderersCategory));

            SkinnedMeshRenderers = Player.Avatar.gameObject.GetComponentsInChildren<SkinnedMeshRenderer>();

            foreach (var skinnedMeshRenderer in SkinnedMeshRenderers)
            {
                meshRenderersCategory.CreateFunction(skinnedMeshRenderer.gameObject.name, Color.white, () =>
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

        public static void RefreshBlendShapes(Page blendShapes)
        {
            blendShapes.RemoveAll();

            Page meshRenderersCategory = blendShapes.CreatePage("Select Mesh Renderer", Color.green);

            meshRenderersCategory.CreateFunction("Refresh", Color.green, () => RefreshMeshRenderers(meshRenderersCategory));

            blendShapes.CreateFunction("Refresh", Color.green, () => RefreshBlendShapes(blendShapes));
            
            if (SelectedMeshRenderer != null && SelectedMeshRenderer.sharedMesh != null)
            {
                int blendShapeCount = SelectedMeshRenderer.sharedMesh.blendShapeCount;
                foreach (int i in Enumerable.Range(0, blendShapeCount))
                {
                    float weight = SelectedMeshRenderer.GetBlendShapeWeight(i);
                    string blendShapeName = SelectedMeshRenderer.sharedMesh.GetBlendShapeName(i);

                    FloatElement floatElement = blendShapes.CreateFloat(blendShapeName, Color.white, weight, 10,
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