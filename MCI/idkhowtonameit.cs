using Reactor.Utilities.ImGui;

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
            TestWindow = new(new(20, 20, 0, 0), "BetterMCI", () =>
            {
                GUILayout.Label("Name: " + DataManager.Player.Customization.Name);

                if (PlayerControl.LocalPlayer && !DestroyableSingleton<LobbyBehavior>.Instance && !PlayerControl.LocalPlayer.IsDead && AmongUsClient.Instance.GameState != InnerNetClient.GameStates.Ended);
                    PlayerControl.LocalPlayer.Collider.enabled = GUILayout.Toggle(PlayerControl.LocalPlayer.Collider.enabled, "Enable Player Collider");

                if (DestroyableSingleton<LobbyBehavior>.Instance)
                {
                    GUILayout.Label("You are sus.")
                
                    if (GUILayout.Button("Spawn Bot"))
                    {
                        if (PlayerControl.AllPlayerControls.Count < 127)
                        {
                            InstanceControl.CleanUpLoad();
                            InstanceControl.CreatePlayerInstance();
                        }
                    }

                    if (GUILayout.Button("Remove Last Bot"))
                    {
                        InstanceControl.RemovePlayer((byte)MCIUtils.Clients.Count);

                        if (InstanceControl.Clients.Count == 0)
                    }

                    if (GUILayout.Button("Remove All Bots"))
                    {
                        InstanceControl.RemoveAllPlayers();
                    }
                }
                else if (GameManager.Instance.GameHasStarted)
                {

                    if (GUILayout.Button("Next Player"))
                    {
                    controllingFigure++;
                        controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
                        InstanceControl.SwitchTo((byte)controllingFigure);
                    }
                    else if (GUILayout.Button("Previous Player"))
                    {
                    controllingFigure--;
                        controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
                        InstanceControl.SwitchTo((byte)controllingFigure);
                    }

                    if (GUILayout.Button("Toggle Impostor"))
                    {
                        GUILayout.Toggle(AmongUs.GameOptions.RoleTypes.Impostor);
                    }

                    if (GUILayout.Button("Toggle Crewmate"))
                    {
                        GUILayout.Toggle(AmongUs.GameOptions.RoleTypes.Crewmate);
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
                        PlayerControl.LocalPlayer.myTasks.ForEach(x => CustomPlayer.Local.RpcCompleteTask(x.Id));

                    if (GUILayout.Button("Complete Everyone's Tasks"))
                        PlayerControl.AllPlayerControls.ForEach(x => x.myTasks.ForEach(y => x.RpcCompleteTask(y.Id)));

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
                        player.RpcMurderPlayer(player);

                    if (GUILayout.Button("Kill All"))
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                          player.RpcMurderPlayer(player);
                        }

                    if (GUILayout.Button("Revive Self"))
                        PlayerControl.LocalPlayer.Revive();

                    if (GUILayout.Button("Revive All"))
                        PlayerControl.AllPlayersControls.ForEach(x => x.Revive());

                if (PlayerControl.LocalPlayer)
                {
                    var position = PlayerControl.LocalPlayer.Position;
                    GUILayout.Label($"Player Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");

                    var mouse = Input.mousePosition;
                    GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");
                }
            }
        }

        public void Update()
        {
            if ((PlayerControl.AllPlayerControls.Count < 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null)) || AmongUsClient.Instance.NetworkMode != NetworkModes.LocalGame)
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
                    InstanceControl.RemoveAllPlayers();
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
                TestWindow.Enabled = !TestWindow.Enabled;
        }

        public void OnGUI() => TestWindow.OnGUI();
    }
}
