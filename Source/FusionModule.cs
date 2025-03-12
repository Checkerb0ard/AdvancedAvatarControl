using System;
using AdvancedAvatarControl.Messages;
using LabFusion.Network;
using LabFusion.Player;
using LabFusion.SDK.Modules;

namespace AdvancedAvatarControl
{
    public class FusionModule : Module
    {
        public static FusionModule Instance { get; private set; }

        public override string Name => "AdvancedAvatarContol";

        public override string Author => "Checkerboard";

        public override Version Version => new (1, 0, 0);

        public override ConsoleColor Color => ConsoleColor.Cyan;

        protected override void OnModuleRegistered()
        {
            Instance = this;
            
            LoggerInstance.Log("Module was loaded!");
        }

        protected override void OnModuleUnregistered()
        {
            Instance = null;

            LoggerInstance.Log("Module was unloaded!");
        }

        public void SendBlendShapeMessage(float boneData, int boneIndex)
        {
            using var writer = FusionWriter.Create();
            using var data = BlendShapes.BasicNumericData.Create(boneIndex, boneData, PlayerIdManager.LocalSmallId);
            writer.Write(data);

            using var message = FusionMessage.ModuleCreate<BlendShapes.BasicNumericMessage>(writer);
            MessageSender.SendToServer(NetworkChannel.Reliable, message);
        }
    }
}