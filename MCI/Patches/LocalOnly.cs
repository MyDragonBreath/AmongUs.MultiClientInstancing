using HarmonyLib;
using UnityEngine;

namespace MCI.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), nameof(MainMenuManager.Start))]
    public sealed class LocalOnly
    {
        public static void Postfix()
        {
            var inf = new GameObject("Info");
            inf.transform.position = new Vector3(0, -1.75f, 0);
            var tmp = inf.AddComponent<TMPro.TextMeshPro>();
            tmp.alignment = TMPro.TextAlignmentOptions.Center;
            tmp.horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center;
            tmp.text = "MCI only supports localhosted lobbies.";
            tmp.color = Color.red;
            tmp.fontSize = 2.25f;

            var pos = inf.AddComponent<AspectPosition>();
            pos.Alignment = AspectPosition.EdgeAlignments.RightBottom;
            pos.DistanceFromEdge = new Vector3(3.2f, 1, 200);
            pos.AdjustPosition();
        }
    }

    [HarmonyPatch(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    public sealed class LobbyCheck
    {
        public static void Postfix()
        {
            MCIPlugin.Enabled = AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame;
        }
    }
}
