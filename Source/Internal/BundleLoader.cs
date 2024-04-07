using System.IO;
using System.Reflection;
using BoneLib;
using MelonLoader;
using UnityEngine;

namespace AdvancedAvatarControl.Internal
{
    public class BundleLoader
    {
        private static string path;

        public static void GetBundles()
        {
            var assembly = Assembly.GetExecutingAssembly();

            if (HelperMethods.IsAndroid())
            {
                path = Path.Combine(MelonUtils.UserDataDirectory, "AdvancedAvatarControl", "Bundles", "Android");
            }
            else
            {
                path = Path.Combine(MelonUtils.UserDataDirectory, "AdvancedAvatarControl", "Bundles", "Windows64");
            }
            
            if (!Directory.Exists(path))
            {
                MelonLogger.Msg("Directory does not exist, creating it now");
                Directory.CreateDirectory(path);
            }

            string[] bundleFiles = Directory.GetFiles(path, "*.bundle");

            // Array of asset bundle names
            string[] assetBundleNames = {};
            
            if (bundleFiles.Length == 0)
            {
                MelonLogger.Msg("No bundles found in the directory");
                return;
            }

            foreach (string bundleFile in bundleFiles)
            {
                // Process each bundle file
                // For now, we'll just print the file path
                MelonLogger.Msg(bundleFile);

                // Load each asset bundle
                foreach (string assetBundleName in assetBundleNames)
                {
                    var resourceBundle = HelperMethods.LoadEmbeddedAssetBundle(assembly, assetBundleName);
                    // Process the loaded asset bundle
                }
            }
        }
    }
}