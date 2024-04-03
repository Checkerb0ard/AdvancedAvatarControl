using System.Reflection;
using AdvancedAvatarControl;
using LabFusion.SDK.Modules;
using MelonLoader;
using ModuleInfo = AdvancedAvatarControl.ModuleInfo;

[assembly: AssemblyTitle(AdvancedAvatarControl.MelonMod.Description)]
[assembly: AssemblyDescription(AdvancedAvatarControl.MelonMod.Description)]
[assembly: AssemblyCompany(AdvancedAvatarControl.MelonMod.Company)]
[assembly: AssemblyProduct(AdvancedAvatarControl.MelonMod.Name)]
[assembly: AssemblyCopyright("Developed by " + AdvancedAvatarControl.MelonMod.Author)]
[assembly: AssemblyTrademark(AdvancedAvatarControl.MelonMod.Company)]
[assembly: AssemblyVersion(AdvancedAvatarControl.MelonMod.Version)]
[assembly: AssemblyFileVersion(AdvancedAvatarControl.MelonMod.Version)]
[assembly: MelonInfo(typeof(AdvancedAvatarControl.MelonMod), AdvancedAvatarControl.MelonMod.Name, AdvancedAvatarControl.MelonMod.Version, AdvancedAvatarControl.MelonMod.Author, AdvancedAvatarControl.MelonMod.DownloadLink)]
[assembly: MelonColor(System.ConsoleColor.Cyan)]

[assembly: LabFusion.SDK.Modules.ModuleInfo(typeof(FusionModule), ModuleInfo.Name, ModuleInfo.Version, ModuleInfo.Author, ModuleInfo.Abbreviation, ModuleInfo.AutoRegister, ModuleInfo.Color)]

// Create and Setup a MelonGame Attribute to mark a Melon as Universal or Compatible with specific Games.
// If no MelonGame Attribute is found or any of the Values for any MelonGame Attribute on the Melon is null or empty it will be assumed the Melon is Universal.
// Values for MelonGame Attribute can be found in the Game's app.info file or printed at the top of every log directly beneath the Unity version.
[assembly: MelonGame("Stress Level Zero", "BONELAB")]
[assembly: MelonOptionalDependencies("labfusion")]