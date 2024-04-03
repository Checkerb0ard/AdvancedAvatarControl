using System.Linq;
using AdvancedAvatarControl.BoneMenu;
using BoneLib;
using HarmonyLib;
using UnityEngine;
using MelonLoader;
using SLZ.VRMK;

namespace AdvancedAvatarControl.Patches
{
    [HarmonyPatch(typeof(Mirror), "OnTriggerEnter")]
    public class MirrorPatch
    {
        public static void Postfix(Mirror __instance, Collider c)
        {
            MirrorExtensions.UpdateMirror();
        }
    }

    public class MirrorExtensions
    {
        public static void UpdateMirror()
        {
            if (BoneMenu.BoneMenu.SelectedMeshRenderer == null)
                return;
            foreach (Avatar item in Object.FindObjectsOfType<Avatar>())
            {
                if (((Object)((Component)item)).name == Player.GetCurrentAvatar().gameObject.name)
                {
                    SkinnedMeshRenderer skinnedMeshRenderer = item.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();
                    if (skinnedMeshRenderer != null && skinnedMeshRenderer.sharedMesh != null)
                    {
                        int blendShapeCount = skinnedMeshRenderer.sharedMesh.blendShapeCount;
                        foreach (int i in Enumerable.Range(0, blendShapeCount))
                        {
                            float weightTarget = BoneMenu.BoneMenu.SelectedMeshRenderer.GetBlendShapeWeight(i);
                            string blendShapeName = skinnedMeshRenderer.sharedMesh.GetBlendShapeName(i);
                            skinnedMeshRenderer.SetBlendShapeWeight(i, weightTarget);
                        }
                    }
                }
            }
            
#if DEBUG
            MelonLogger.Msg("UpdateMirror called");
#endif            
        }
    }
}