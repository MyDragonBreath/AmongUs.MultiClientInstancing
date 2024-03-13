using AmongUs.Data;
using Reactor.Utilities.ImGui;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;
using InnerNet;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace MCI.Patches
{
    public class DebuggerBehaviour : MonoBehaviour
    {
        [HideFromIl2Cpp]
        public DragWindow TestWindow { get; }
        private static byte ControllingFigure;
        public DebuggerBehaviour(System.IntPtr ptr) : base(ptr)
        {
            TestWindow = new(new(20, 20, 0, 0), "MCI Debugger", () =>
            {
                GUILayout.Label("Name: " + DataManager.Player.Customization.Name);
                GUILayout.Label("Made by le killer, AlchlcDvl with help from whichTwix"); //based off from Reactor.Debugger but remade by AlchlcDvl and updated to vanilla
                
                if (PlayerControl.LocalPlayer)
                {
                    var position = PlayerControl.LocalPlayer.gameObject.transform.position;
                    GUILayout.Label($"Player Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");

                    var mouse = Input.mousePosition;
                    GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");
                }

                if (PlayerControl.LocalPlayer && !PlayerControl.LocalPlayer.Data.IsDead && MCIPlugin.Enabled)
                    {
                    PlayerControl.LocalPlayer.Collider.enabled = GUILayout.Toggle(PlayerControl.LocalPlayer.Collider.enabled, "Enable Player Collider");
                    }

                if (!MCIPlugin.Enabled)
                {
                    GUILayout.Label("Debugger features only work on localhosted lobbies");
                }

                if (!MCIPlugin.Enabled) return;
                if (PlayerControl.LocalPlayer && AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Joined && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Ended)
                    {
                
                    if (GUILayout.Button("Spawn Bot"))
                    {
                        if (PlayerControl.AllPlayerControls.Count < 15)
                        {
                            Utils.CleanUpLoad();
                            Utils.CreatePlayerInstance(MCIPlugin.RobotName);
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
                else if (AmongUsClient.Instance?.GameState == InnerNetClient.GameStates.Started || GameManager.Instance?.GameHasStarted == true ||
                        AmongUsClient.Instance?.IsGameStarted == true && AmongUsClient.Instance.GameState != InnerNet.InnerNetClient.GameStates.Ended)
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

                    if (GUILayout.Button("Turn Impostor"))
                    {
                        PlayerControl.LocalPlayer.Data.Role.TeamType = RoleTeamTypes.Impostor;
                    if (PlayerControl.LocalPlayer.Data.IsDead != PlayerControl.LocalPlayer)
                    {
                        RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.Impostor);
                        DestroyableSingleton<HudManager>.Instance.KillButton.gameObject.SetActive(true);
                        PlayerControl.LocalPlayer.SetKillTimer(GameOptionsManager.Instance.currentNormalGameOptions.KillCooldown);
                    }
                    else
                    {
                        RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.ImpostorGhost);
                    }
                    }

                    if (GUILayout.Button("Turn Crewmate"))
                    {
                        PlayerControl.LocalPlayer.Data.Role.TeamType = RoleTeamTypes.Crewmate;
                    if (PlayerControl.LocalPlayer.Data.IsDead != PlayerControl.LocalPlayer)
                    {
                        RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.Crewmate);
                    }
                    else
                    {
                        RoleManager.Instance.SetRole(PlayerControl.LocalPlayer, AmongUs.GameOptions.RoleTypes.CrewmateGhost);
                    }
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

                    if (GUILayout.Button("End Meeting") && MeetingHud.Instance)
                        MeetingHud.Instance.RpcClose();

                    if (GUILayout.Button("Kill Self"))
                        PlayerControl.LocalPlayer.RpcMurderPlayer(PlayerControl.LocalPlayer, true);

                    if (GUILayout.Button("Kill All"))
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                          player.RpcMurderPlayer(player, true);
                        }

                    if (GUILayout.Button("Revive Self"))
                        PlayerControl.LocalPlayer.Revive();

                    if (GUILayout.Button("Revive All"))
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                            player.Revive();
                        }
                }
            });
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
                TestWindow.Enabled = !TestWindow.Enabled;
        }

        public void OnGUI()
        {
        TestWindow.OnGUI();
        }
    }
}
