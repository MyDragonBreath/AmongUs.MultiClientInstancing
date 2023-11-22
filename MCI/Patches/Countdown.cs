using HarmonyLib;

namespace MCI.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.Update))]
public static class CountdownPatch
{
    public static void Prefix(GameStartManager __instance) => __instance.countDownTimer = 0;
}
