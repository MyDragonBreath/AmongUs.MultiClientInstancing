using HarmonyLib;
using UnityEngine;

namespace MCI.Patches
{
    [HarmonyPriority(Priority.Low)]
    [HarmonyPatch(typeof(PingTracker), nameof(PingTracker.Update))]
    public static class PingTracker_Update
    {
        [HarmonyPostfix]
        public static void Postfix(PingTracker __instance)
        {
            var position = __instance.GetComponent<AspectPosition>();
            position.DistanceFromEdge = new Vector3(3.6f, 0.1f, 0);
            position.AdjustPosition();
            __instance.text.text +=
                "\n<color=#ff6700FF>MCI v" + MCIPlugin.VersionString + "</color>";
            if (UpdateChecker.needsUpdate) __instance.text.text += " - <color=#ff0000FF>UPDATE AVAILABLE</color>";
            __instance.text.text +=
                "\n    by MyDragonBreath, whichTwix";
        }
    }
}
