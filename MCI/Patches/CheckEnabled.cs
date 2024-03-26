
using System.Linq;

namespace MCI.Patches
{
    public sealed class ModState
    {
        public static bool Enabled
        {
            get
            {
                bool result = AmongUsClient.Instance.NetworkMode == NetworkModes.LocalGame 
                || PlayerControl.AllPlayerControls.ToArray().Count(x => !InstanceControl.PlayerIdClientId.ContainsKey(x.PlayerId)) == 1
                && !AmongUsClient.Instance.IsGamePublic;

                if (!result) Utils.RemoveAllPlayers(false);

                return result;
            }
        }
    }
}
