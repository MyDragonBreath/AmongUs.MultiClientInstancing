using AmongUs.Data;
using Reactor.Utilities.ImGui;
using Il2CppInterop.Runtime.Attributes;
using System;
using UnityEngine;

namespace MCI
{
    //le killer is going to kill you fr
    public class DebuggerBehaviour : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public DragWindow TestWindow { get; }
        private static byte ControllingFigure;

        public DebuggerBehaviour(IntPtr ptr) : base(ptr)
        {
            TestWindow = new(new(20, 20, 0, 0), "MCI Debugger", () =>
            {
                GUILayout.Label("Name: " + DataManager.Player.Customization.Name);

                if (PlayerControl.LocalPlayer && !DestroyableSingleton<LobbyBehaviour>.Instance && !PlayerControl.LocalPlayer.Data.IsDead && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Ended)
                    PlayerControl.LocalPlayer.Collider.enabled = GUILayout.Toggle(PlayerControl.LocalPlayer.Collider.enabled, "Enable Player Collider");

                if (DestroyableSingleton<LobbyBehaviour>.Instance)
                {
                    GUILayout.Label("You are sus.");
                
                    if (GUILayout.Button("Spawn Bot"))
                    {
                        if (PlayerControl.AllPlayerControls.Count < 127)
                        {
                            Utils.CleanUpLoad();
                            Utils.CreatePlayerInstance();
                        }
                    }

                    if (GUILayout.Button("Remove Last Bot"))
                    {
                        Utils.RemovePlayer((byte)InstanceControl.clients.Count);
                    }

                    if (GUILayout.Button("Remove All Bots"))
                    {
                        Utils.RemoveAllPlayers();
                    }
                }
                else if (GameManager.Instance.GameHasStarted)
                {

                    if (GUILayout.Button("Next Player"))
                    {
                        ControllingFigure++;
                        ControllingFigure = (byte) Mathf.Clamp(ControllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
                        InstanceControl.SwitchTo(ControllingFigure);
                    }
                    else if (GUILayout.Button("Previous Player"))
                    {
                        ControllingFigure--;
                        ControllingFigure = (byte) Mathf.Clamp(ControllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
                        InstanceControl.SwitchTo(ControllingFigure);
                    }

                    if (GUILayout.Button("End Game"))
                    {
                        GameManager.Instance.RpcEndGame(GameOverReason.ImpostorBySabotage, false);
                    }

                    if (GUILayout.Button("Fix All Sabotages"))
                    {
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 79);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 80);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 81);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Doors, 82);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.LifeSupp, 16);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Laboratory, 16);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 0);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Reactor, 16 | 1);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 0);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 16 | 1);
                        ShipStatus.Instance.RpcRepairSystem(SystemTypes.Comms, 0);
                    }

                    if (GUILayout.Button("Complete Tasks"))
                        foreach (var task in PlayerControl.LocalPlayer.myTasks)
                        {
                            PlayerControl.LocalPlayer.RpcCompleteTask(task.Id);
                        }

                    if (GUILayout.Button("Complete Everyone's Tasks"))
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            foreach (var task in player.myTasks)
                            {
                                player.RpcCompleteTask(task.Id);
                            }
                        }

                    if (GUILayout.Button("Redo Intro Sequence"))
                    {
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoShowIntro());
                    }

                    if (GUILayout.Button("Start Meeting") && !MeetingHud.Instance)
                    {
                        PlayerControl.LocalPlayer.RemainingEmergencies++;
                        PlayerControl.LocalPlayer.CmdReportDeadBody(null);
                    }

                    if (GUILayout.Button("End Meeting") && !MeetingHud.Instance)
                        MeetingHud.Instance.RpcClose();

                    if (GUILayout.Button("Kill Self"))
                        PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer);

                    if (GUILayout.Button("Kill All"))
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                          player.RpcMurderPlayer(player);
                        }

                    if (GUILayout.Button("Revive Self"))
                        PlayerControl.LocalPlayer.Revive();

                    if (GUILayout.Button("Revive All"))
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            player.Revive();
                        }

                if (PlayerControl.LocalPlayer)
                {
                    var position = PlayerControl.LocalPlayer.gameObject.transform.position;
                    GUILayout.Label($"Player Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");

                    var mouse = Input.mousePosition;
                    GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");
                }
            }});
        }

        void Update()
        {
            (System.Console.WriteLine("a");
        }
            if ((PlayerControl.AllPlayerControls.Count < 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null) || AmongUsClient.Instance.NetworkMode != NetworkModes.LocalGame)
            {
                if (TestWindow.Enabled)
                    TestWindow.Enabled = false;

                return; //MCI does only support localhosted lobbies.
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestWindow.Enabled = !TestWindow.Enabled;

                if (!TestWindow.Enabled)
                {
                    Utils.RemoveAllPlayers();
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
                TestWindow.Enabled = !TestWindow.Enabled;
        }

        public void OnGUI() => TestWindow.OnGUI();
    }
}
