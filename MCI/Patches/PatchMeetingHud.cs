using HarmonyLib;

namespace MCI.Patches
{
    [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Confirm))]
    public sealed class SameVoteAll
    {
        public static void Postfix(MeetingHud __instance, ref byte suspectStateIdx)
        {
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                __instance.CmdCastVote(player.PlayerId, suspectStateIdx);
            }
        }
    }
}
