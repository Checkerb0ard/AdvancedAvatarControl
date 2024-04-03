using System.Linq;
using MelonLoader;
using System.Reflection;

using AdvancedAvatarControl.BoneMenu;
using AdvancedAvatarControl.Patches;

using BoneLib;
using Il2CppSystem;
using LabFusion.SDK.Modules;
using LabFusion.Utilities;
using SLZ.Rig;

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
            Hooking.OnLevelInitialized += AddEyeMovement;
            Hooking.OnSwitchAvatarPostfix += BoneMenu.BoneMenu.OnSwitchAvatar;
            MultiplayerHooking.OnPlayerRepCreated += AddRepEyeMovement;

            FusionInstalled = HelperMethods.CheckIfAssemblyLoaded("labfusion");
        }
        
        public void AddEyeMovement(LevelInfo levelInfo)
        {
            if (Player.playerHead.gameObject.GetComponent<PlayerEyeController>() != null)
            {
                MelonLogger.Msg("PlayerEyeController already exists");
                return;
            }
            Player.playerHead.gameObject.AddComponent<PlayerEyeController>();
#if DEBUG            
            MelonLogger.Msg("PlayerEyeController added to RigManager");
#endif            
        }
        
        public void AddRepEyeMovement(RigManager playerRep)
        {
#if DEBUG            
            MelonLogger.Msg($"RepEyeController added to {playerRep.name}");
#endif            
            playerRep.physicsRig.m_head.gameObject.AddComponent<RepEyeController>();
        }
    }
}
