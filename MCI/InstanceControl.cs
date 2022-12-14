using InnerNet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MCI
{
    public static class InstanceControl
    {
        public static Dictionary<int, ClientData> clients = new Dictionary<int, ClientData>();
        public static Dictionary<byte, int> PlayerIdClientId = new Dictionary<byte, int>();
        public const int MaxID = 100;
        public static int availableId()
        {
            for (int i = 2; i < MaxID; i++)
                if (!clients.ContainsKey(i)) 
                    if (PlayerControl.LocalPlayer.OwnerId != i) return i;
            return -1;
        }






        public static PlayerControl CurrentPlayerInPower { get; private set; }

        public static void SwitchTo(byte playerId)
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
            PlayerControl.LocalPlayer.moveable = false;

            Object.Destroy(PlayerControl.LocalPlayer.myLight);
            

            var newPlayer = Utils.PlayerById(playerId);
            PlayerControl.LocalPlayer = newPlayer;
            PlayerControl.LocalPlayer.moveable = true;

            AmongUsClient.Instance.ClientId = PlayerControl.LocalPlayer.OwnerId;
            AmongUsClient.Instance.HostId = PlayerControl.LocalPlayer.OwnerId;

            DestroyableSingleton<HudManager>.Instance.SetHudActive(true);

            //hacky "fix" for twix and det
            DestroyableSingleton<HudManager>.Instance.KillButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach((x) => { if (x.gameObject.name == "KillButton(Clone)") Object.Destroy(x.gameObject); });
            DestroyableSingleton<HudManager>.Instance.transform.GetComponentsInChildren<Transform>().ToList().ForEach((x) => { if (x.gameObject.name == "KillButton(Clone)") Object.Destroy(x.gameObject); });

            PlayerControl.LocalPlayer.myLight = UnityEngine.Object.Instantiate<LightSource>(PlayerControl.LocalPlayer.LightPrefab);
            PlayerControl.LocalPlayer.myLight.transform.SetParent(PlayerControl.LocalPlayer.transform);
            PlayerControl.LocalPlayer.myLight.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;
            PlayerControl.LocalPlayer.myLight.Initialize();
            Camera.main.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState(true);
            KillAnimation.SetMovement(PlayerControl.LocalPlayer, true);
            PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
            CurrentPlayerInPower = newPlayer;
        }

        public static void SwitchTo(int clientId)
        {
            byte? id = Enumerable.FirstOrDefault(InstanceControl.PlayerIdClientId.Keys, (byte x) => InstanceControl.PlayerIdClientId[x] == clientId);
            if (id != null) SwitchTo((byte)id);
        }
    }
}
