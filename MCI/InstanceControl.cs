using InnerNet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MCI
{
    public static class InstanceControl
    {
        public static Dictionary<int, ClientData> clients = new();

        public static Dictionary<byte, int> PlayerIdClientId = new();

        public const int MaxID = 100;

        public static int AvailableId()
        {
            for (int i = 2; i < MaxID; i++)
            {
                if (!clients.ContainsKey(i) && PlayerControl.LocalPlayer.OwnerId != i)
                    return i;
            }
            return -1;
        }

        public static PlayerControl CurrentPlayerInPower { get; private set; }

        public static void SwitchTo(byte playerId)
        {
            PlayerControl.LocalPlayer.NetTransform.RpcSnapTo(PlayerControl.LocalPlayer.transform.position);
            PlayerControl.LocalPlayer.moveable = false;

            Object.Destroy(PlayerControl.LocalPlayer.lightSource);

            var newPlayer = Utils.PlayerById(playerId);
            PlayerControl.LocalPlayer = newPlayer;
            PlayerControl.LocalPlayer.moveable = true;

            AmongUsClient.Instance.ClientId = PlayerControl.LocalPlayer.OwnerId;
            AmongUsClient.Instance.HostId = PlayerControl.LocalPlayer.OwnerId;

            HudManager.Instance.SetHudActive(true);

            //hacky "fix" for twix and det
            HudManager.Instance.KillButton.transform.parent.GetComponentsInChildren<Transform>().ToList().ForEach((x) => { if (x.gameObject.name == "KillButton(Clone)") Object.Destroy(x.gameObject); });
            HudManager.Instance.transform.GetComponentsInChildren<Transform>().ToList().ForEach((x) => { if (x.gameObject.name == "KillButton(Clone)") Object.Destroy(x.gameObject); });

            PlayerControl.LocalPlayer.lightSource = Object.Instantiate(PlayerControl.LocalPlayer.LightPrefab);
            PlayerControl.LocalPlayer.lightSource.transform.SetParent(PlayerControl.LocalPlayer.transform);
            PlayerControl.LocalPlayer.lightSource.transform.localPosition = PlayerControl.LocalPlayer.Collider.offset;
            PlayerControl.LocalPlayer.lightSource.Initialize(PlayerControl.LocalPlayer.Collider.offset * 0.5f);
            Camera.main.GetComponent<FollowerCamera>().SetTarget(PlayerControl.LocalPlayer);
            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState(true);
            KillAnimation.SetMovement(PlayerControl.LocalPlayer, true);
            PlayerControl.LocalPlayer.MyPhysics.inputHandler.enabled = true;
            CurrentPlayerInPower = newPlayer;
        }

        public static void SwitchTo(int clientId)
        {
            byte? id = PlayerIdClientId.Keys.FirstOrDefault((byte x) => PlayerIdClientId[x] == clientId);
            if (id != null) SwitchTo((byte)id);
        }
    }
}
