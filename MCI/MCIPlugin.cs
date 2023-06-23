using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;

namespace MCI
{
    [BepInAutoPlugin("dragonbreath.au.mci", "MCI", VersionString)]
    [BepInProcess("Among Us.exe")]
    [BepInDependency(SubmergedCompatibility.SUBMERGED_GUID, BepInDependency.DependencyFlags.SoftDependency)]
    public partial class MCIPlugin : BasePlugin
    {
        public const string VersionString = "0.0.6";
        public static System.Version vVersion = new(VersionString);
        public Harmony Harmony { get; } = new(Id);

        public static MCIPlugin singleton { get; private set; } = null;

        public static bool Enabled { get; set; } = true;
        public static bool IKnowWhatImDoing { get; set; } = false;
        public override void Load()
        {
            if (singleton != null) return;
            singleton = this;
            
            Harmony.PatchAll();
            UpdateChecker.checkForUpdate();

            SubmergedCompatibility.Initialize();

            SceneManager.add_sceneLoaded((Action<Scene, LoadSceneMode>)((scene, _) =>
            {
                if (scene.name == "MainMenu")
                {
                    ModManager.Instance.ShowModStamp();
                }
            }));
        }


        public static bool Persistence = true;
        
    }


    [HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
    public static class CountdownPatch
    {
        public static void Prefix(GameStartManager __instance)
        {
            __instance.countDownTimer = 0;
        }
    }
}