using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;

namespace MCI
{
    [BepInAutoPlugin("dragonbreath.au.mci", "MCI", VersionString)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public partial class MCIPlugin : BasePlugin
    {
        public const string VersionString = "0.0.4";
        public static System.Version vVersion = new(VersionString);
        public Harmony Harmony { get; } = new(Id);
        public override void Load()
        {
            Harmony.PatchAll();
            UpdateChecker.checkForUpdate();
        }
    }
}