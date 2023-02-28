using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using UnityEngine.SceneManagement;

namespace MCI
{
    [BepInAutoPlugin("dragonbreath.au.mci", "MCI", VersionString)]
    [BepInProcess("Among Us.exe")]
    public partial class MCIPlugin : BasePlugin
    {
        public const string VersionString = "0.0.4";
        public static System.Version vVersion = new(VersionString);
        public Harmony Harmony { get; } = new(Id);
        public override void Load()
        {
            Harmony.PatchAll();
            UpdateChecker.checkForUpdate();

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
}