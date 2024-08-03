using MelonLoader;
using MelonLoader.Utils;
using System.IO;

namespace AdvancedAvatarControl
{
    public static class Prefs
    {
        public static readonly MelonPreferences_Category MainCategory = MelonPreferences.CreateCategory("AdvancedAvatarControl");

        public static MelonPreferences_Entry<float> EyeMovementSpeed;

        public static void Initialize()
        {
            EyeMovementSpeed = MainCategory.GetEntry<float>("Eye Movement Speed") ?? MainCategory.CreateEntry<float>("Eye Movement Speed", 10f, "Eye Movement Speed");

            MainCategory.SetFilePath(Path.Combine(MelonEnvironment.UserDataDirectory, "AdvancedAvatarControl.cfg"));
            MainCategory.SaveToFile(false);
        }
    }
}
