using System;
using AdvancedAvatarControl.Messages;
using LabFusion.Network;
using LabFusion.Representation;
using LabFusion.SDK.Modules;
using UnityEngine;

namespace AdvancedAvatarControl
{
    public static class ModuleInfo
    {
        public const string Name = "AdvancedAvatarContol"; // Name of the Module.  (MUST BE SET)
        public const string Version = "1.0.0"; // Version of the Module.  (MUST BE SET)
        public const string Author = "Checkerboard"; // Author of the Module.  (MUST BE SET)
        public const string Abbreviation = null; // Abbreviation of the Module. (Set as null if none)
        public const bool AutoRegister = true; // Should the Module auto register when the assembly is loaded?
        public const ConsoleColor Color = ConsoleColor.Cyan; // The color of the logged load info. (MUST BE SET)
    }
    
    public class FusionModule : Module
    {
        public static FusionModule Instance { get; private set; }
        
        public override void OnModuleLoaded()
        {
            Instance = this;
            
            LoggerInstance.Log("Module was loaded!");
        }

        public void SendBlendBoneMessage(float boneData, int boneIndex)
        {
            using var writer = FusionWriter.Create();
            using var data = BlendBones.BasicNumericData.Create(boneIndex, boneData, PlayerIdManager.LocalSmallId);
            writer.Write(data);

            using var message = FusionMessage.ModuleCreate<BlendBones.BasicNumericMessage>(writer);
            MessageSender.SendToServer(NetworkChannel.Reliable, message);
        }
    }
}