using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedAvatarControl
{
    public static class Prefs
    {
        public static readonly MelonPreferences_Category MainCategory = MelonPreferences.CreateCategory("AdvancedAvatarControl");

        public static MelonPreferences_Entry<float> EyeMovementSpeed;

        public static void Initialize()
        {
            EyeMovementSpeed = MainCategory.GetEntry<float>("Eye Movement Speed") ?? MainCategory.CreateEntry<float>("Eye Movement Speed", 15f, "Eye Movement Speed");

            MainCategory.SetFilePath(MelonUtils.UserDataDirectory + "/AdvancedAvatarControl.cfg");
            MainCategory.SaveToFile(false);
        }
    }
}
