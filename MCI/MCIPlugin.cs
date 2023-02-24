using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Reactor;

namespace MCI
{
    [BepInAutoPlugin]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(ReactorPlugin.Id)]
    public partial class MCIPlugin : BasePlugin
    {
        public Harmony Harmony { get; } = new(Id);
        public override void Load()
        {
            Harmony.PatchAll();
        }
    }
}