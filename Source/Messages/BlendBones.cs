using System;
using AdvancedAvatarControl.Patches;
using BoneLib;
using LabFusion.Data;
using LabFusion.Network;
using LabFusion.Representation;
using SLZ.VRMK;
using UnityEngine;

namespace AdvancedAvatarControl.Messages
{
    public class BlendBones
    {
        public class BasicNumericData : IFusionSerializable, IDisposable
        {
            public int intData;
            public float floatData;
            public byte shortId;

            public void Dispose()
            {
                GC.SuppressFinalize(this);
            }

            public void Serialize(FusionWriter writer)
            {
                writer.Write(intData);
                writer.Write(floatData);
                writer.Write(shortId);
            }

            public void Deserialize(FusionReader reader)
            {
                intData = reader.ReadInt32();
                floatData = reader.ReadSingle();
                shortId = reader.ReadByte();
            }

            public static BasicNumericData Create(int intData, float floatData, byte shortId)
            {
                return new BasicNumericData()
                {
                    intData = intData,
                    floatData = floatData,
                    shortId = shortId
                };
            }
        }
        
        public class BasicNumericMessage : ModuleMessageHandler
        {
            public override void HandleMessage(byte[] bytes, bool isServerHandled = false)
            {
                using (var reader = FusionReader.Create(bytes))
                {
                    using (var data = reader.ReadFusionSerializable<BasicNumericData>())
                    {
                        if (NetworkInfo.IsServer && isServerHandled)
                        {
                            using (var message = FusionMessage.ModuleCreate<BasicNumericMessage>(bytes))
                            {
                                MessageSender.BroadcastMessage(NetworkChannel.Reliable, message);
                            }
                        }
                        else
                        {
                            var boneIndex = data.intData;
                            var boneValue = data.floatData;
                            var shortId = data.shortId;

#if DEBUG
                            FusionModule.Instance.LoggerInstance.Log("Received Blend Bone Message: " + boneIndex + " = " + boneValue + " from " + shortId);
#endif

                            Avatar avatar;

                            if (shortId == PlayerIdManager.LocalSmallId)
                            {
                                avatar = Player.GetCurrentAvatar();
                            }
                            else
                            {
                                PlayerRepManager.TryGetPlayerRep(shortId, out PlayerRep playerRep);
                                avatar = playerRep.RigReferences.RigManager.avatar;
                            }
                            
                            if (avatar != null)
                            {
                                var skinnedMeshRenderer = avatar.gameObject.GetComponentInChildren<SkinnedMeshRenderer>();

                                skinnedMeshRenderer.SetBlendShapeWeight(boneIndex, boneValue);
                            
                                MirrorExtensions.UpdateMirror();   
                            }
                            else
                            {
                                FusionModule.Instance.LoggerInstance.Error($"Failed to obtain Avatar for PlayerId {shortId}");
                            }
                        }
                    }
                }
            }
        }
    }
}