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
            if (!MCIPlugin.Enabled) return;
            foreach (var p in __instance.allClients)
            {
                p.IsReady = true;
                p.Character.gameObject.GetComponent<DummyBehaviour>().enabled = false;
            }
        }
    }
}
