using BepInEx.Unity.IL2CPP;
using InnerNet;
using System.Linq;
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
            PlatformSpecificData samplePSD = new()
            {
                Platform = Platforms.StandaloneWin10,
                PlatformName = "Bot"
            };

            int sampleId = id;
            if (sampleId == -1) sampleId = InstanceControl.AvailableId();

            var sampleC = new ClientData(sampleId, name + $"-{sampleId}", samplePSD, 5, "", "");

            AmongUsClient.Instance.CreatePlayer(sampleC);
            AmongUsClient.Instance.allClients.Add(sampleC);

            sampleC.Character.SetName(name + $" {sampleC.Character.PlayerId}");
            if (MCIPlugin.IKnowWhatImDoing) sampleC.Character.SetName(name + $" {{{sampleC.Character.PlayerId}:{sampleId}}}");
            sampleC.Character.SetSkin(HatManager.Instance.allSkins[Random.Range(0, HatManager.Instance.allSkins.Count)].ProdId, 0);
            sampleC.Character.SetColor(Random.Range(0, Palette.PlayerColors.Length));
            sampleC.Character.SetHat("hat_NoHat", 0);

            InstanceControl.clients.Add(sampleId, sampleC);
            InstanceControl.PlayerIdClientId.Add(sampleC.Character.PlayerId, sampleId);
            sampleC.Character.MyPhysics.ResetAnimState();
            sampleC.Character.MyPhysics.ResetMoveState();

            if (SubmergedCompatibility.Loaded)
            {
                SubmergedCompatibility.ImpartSub(sampleC.Character);
            }
            if (IL2CPPChainloader.Instance.Plugins.TryGetValue("me.eisbison.theotherroles", out _))
            {
                sampleC.Character.GetComponent<DummyBehaviour>().enabled = true;
            }

            return sampleC.Character;
        }

        public static void UpdateNames(string name)
        {
            foreach (byte playerId in InstanceControl.PlayerIdClientId.Keys)
            {
                PlayerById(playerId).SetName(name + $" {playerId}");
                if (MCIPlugin.IKnowWhatImDoing) PlayerById(playerId).SetName(name + $" {{{PlayerById(playerId).PlayerId}:{InstanceControl.PlayerIdClientId[playerId]}}}");
            }
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                    return player;
            }
            return null;
        }

        public static void RemovePlayer(byte id)
        {
            int clientId = InstanceControl.clients.FirstOrDefault(x => x.Value.Character.PlayerId == id).Key;
            InstanceControl.clients.Remove(clientId, out ClientData outputData);
            InstanceControl.PlayerIdClientId.Remove(id);
            AmongUsClient.Instance.RemovePlayer(clientId, DisconnectReasons.ExitGame);
            AmongUsClient.Instance.allClients.Remove(outputData);
        }

        public static void RemoveAllPlayers(bool DoSwitch)
        {
            foreach (byte playerId in InstanceControl.PlayerIdClientId.Keys) RemovePlayer(playerId);
            if (DoSwitch) InstanceControl.SwitchTo(AmongUsClient.Instance.allClients[0].Character.PlayerId);
        }
    }
}
