using HarmonyLib;

namespace MCI.Patches
{
    [HarmonyPatch]
    public sealed class OnGameStart
    {
        [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.CoStartGameHost))]
        [HarmonyPrefix]

        public static void Postfix(AmongUsClient __instance)
        {
            foreach (var p in __instance.allClients)
            {
                p.IsReady = true;
            }
        }
    }
}
