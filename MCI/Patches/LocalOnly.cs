using HarmonyLib;
using UnityEngine;

namespace MCI.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public sealed class LocalOnly
    {
        public static void Postfix()
        {
            Object.Destroy(GameObject.Find("HowToPlayButton"));
            Object.Destroy(GameObject.Find("PlayOnlineButton"));
            Object.Destroy(GameObject.Find("FreePlayButton"));
            GameObject.Find("PlayLocalButton").transform.localPosition = new Vector3(0, -1f, 0);

            var inf = new GameObject("Info");
            inf.transform.position = new Vector3(0, -1.75f, 0);
            var tmp = inf.AddComponent<TMPro.TextMeshPro>();
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            tmp.text = "MCI only supports localhosted lobbies.";
            tmp.color = Color.red;
            tmp.fontSize = 3.25f;
        }
    }
}
