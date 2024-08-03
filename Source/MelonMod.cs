using MelonLoader;
using System.Reflection;

using BoneLib;
using LabFusion.SDK.Modules;
using Il2CppSLZ.Marrow;
using LabFusion.Entities;

namespace AdvancedAvatarControl
{
    internal partial class MelonMod : MelonLoader.MelonMod
    {
        internal static bool FusionInstalled = false;
        
        public override void OnEarlyInitializeMelon()
        {
            base.OnEarlyInitializeMelon();
        }

        public override void OnInitializeMelon()
        {
            Prefs.Initialize();
            ModuleHandler.LoadModule(Assembly.GetExecutingAssembly());
        }

        public override void OnLateInitializeMelon()
        {
            BoneMenu.BoneMenu.CreateBoneMenu();
            Hooking.OnLevelLoaded += AddEyeMovement;
            Hooking.OnSwitchAvatarPostfix += BoneMenu.BoneMenu.OnSwitchAvatar;
            NetworkPlayer.OnNetworkRigCreated += AddRepEyeMovement;
        }
        
        public void AddEyeMovement(LevelInfo levelInfo)
        {
            if (Player.Head.gameObject.GetComponent<PlayerEyeController>() != null)
            {
                MelonLogger.Msg("PlayerEyeController already exists");
                return;
            }
            Player.Head.gameObject.AddComponent<PlayerEyeController>();
#if DEBUG            
            MelonLogger.Msg("PlayerEyeController added to RigManager");
#endif            
        }
        
        public void AddRepEyeMovement(NetworkPlayer networkPlayer, RigManager rigManager)
        {
#if DEBUG            
            MelonLogger.Msg($"RepEyeController added to {rigManager.name}");
#endif            
            rigManager.physicsRig.m_head.gameObject.AddComponent<RepEyeController>();
        }
    }
}
