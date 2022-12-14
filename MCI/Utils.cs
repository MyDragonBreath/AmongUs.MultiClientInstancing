using InnerNet;
using UnityEngine;

namespace MCI
{
    public static class Utils
    {
        public static void CleanUpLoad()
        {
            if (GameData.Instance.AllPlayers.Count == 1)
            {
                InstanceControl.clients.Clear();
                InstanceControl.PlayerIdClientId.Clear();
            } 
        }
        public static PlayerControl CreatePlayerInstance(string name = "", int id = -1)
        {
            PlatformSpecificData samplePSD = new PlatformSpecificData();
            samplePSD.Platform = Platforms.StandaloneItch;
            samplePSD.PlatformName = "Robot";

            int sampleId = id;
            if (sampleId == -1) sampleId = InstanceControl.availableId();

            var sampleC = new ClientData(sampleId, name + $"-{sampleId}", samplePSD, 5, "", "");
            PlayerControl playerControl = UnityEngine.Object.Instantiate<PlayerControl>(AmongUsClient.Instance.PlayerPrefab, Vector3.zero, Quaternion.identity);
            playerControl.PlayerId = (byte)GameData.Instance.GetAvailableId();
            playerControl.FriendCode = sampleC.FriendCode;
            playerControl.Puid = sampleC.ProductUserId;
            sampleC.Character = playerControl;

            if (ShipStatus.Instance) ShipStatus.Instance.SpawnPlayer(playerControl, Palette.PlayerColors.Length, false);
            AmongUsClient.Instance.Spawn(playerControl, sampleC.Id, SpawnFlags.IsClientCharacter);
            GameData.Instance.AddPlayer(playerControl);
            
            playerControl.SetName(name + $" {{{playerControl.PlayerId}:{sampleId}}}");
            playerControl.SetSkin(HatManager.Instance.allSkins[UnityEngine.Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            playerControl.SetColor(UnityEngine.Random.Range(0, Palette.PlayerColors.Length));

            //PlayerControl.AllPlayerControls.Add(playerControl);
            InstanceControl.clients.Add(sampleId, sampleC);
            InstanceControl.PlayerIdClientId.Add(playerControl.PlayerId, sampleId);
            return playerControl;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;
            return null;
        }
    }
}
