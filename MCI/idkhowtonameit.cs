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

                if (CustomPlayer.Local && !NoLobby && !CustomPlayer.LocalCustom.IsDead && !IsEnded && !GameHasEnded)
                    CustomPlayer.Local.Collider.enabled = GUILayout.Toggle(CustomPlayer.Local.Collider.enabled, "Enable Player Collider");

                if (IsLobby)
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
                        MCIUtils.RemovePlayer((byte)MCIUtils.Clients.Count);

                        if (MCIUtils.Clients.Count == 0)
                    }

                    if (GUILayout.Button("Remove All Bots"))
                    {
                        MCIUtils.RemoveAllPlayers();
                    }
                }
                else if (IsGame)
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
                        GUILayout.Toggle(Role.Impostor);
                    }

                    if (GUILayout.Button("Toggle Crewmate"))
                    {
                        GUILayout.Toggle(Role.Crewmate);
                    }

                    if (GUILayout.Button("End Game"))
                    {
                        RpcEndGame;
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
                        DefaultOutfitAll();
                    }

                    if (GUILayout.Button("Complete Tasks"))
                        CustomPlayer.Local.myTasks.ForEach(x => CustomPlayer.Local.RpcCompleteTask(x.Id));

                    if (GUILayout.Button("Complete Everyone's Tasks"))
                        CustomPlayer.AllPlayers.ForEach(x => x.myTasks.ForEach(y => x.RpcCompleteTask(y.Id)));

                    if (GUILayout.Button("Redo Intro Sequence"))
                    {
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoFadeFullScreen(Color.clear, Color.black));
                        HudManager.Instance.StartCoroutine(HudManager.Instance.CoShowIntro());
                    }

                    if (GUILayout.Button("Start Meeting") && !Meeting)
                    {
                        CustomPlayer.Local.RemainingEmergencies++;
                        CustomPlayer.Local.CmdReportDeadBody(null);
                    }

                    if (GUILayout.Button("End Meeting") && Meeting)
                        Meeting.RpcClose();

                    if (GUILayout.Button("Kill Self"))
                        RpcMurderPlayer(CustomPlayer.Local, CustomPlayer.Local);

                    if (GUILayout.Button("Kill All"))
                        foreach (var player in PlayerControl.AllPlayerControls)
                        {
                          player.RpcMurderPlayer(player);
                        }

                    if (GUILayout.Button("Revive Self"))
                        CustomPlayer.Local.Revive();

                    if (GUILayout.Button("Revive All"))
                        CustomPlayer.AllPlayers.ForEach(x => x.Revive());

                if (CustomPlayer.Local)
                {
                    var position = CustomPlayer.LocalCustom.Position;
                    GUILayout.Label($"Player Position\nx: {position.x:00.00} y: {position.y:00.00} z: {position.z:00.00}");

                    var mouse = Input.mousePosition;
                    GUILayout.Label($"Mouse Position\nx: {mouse.x:00.00} y: {mouse.y:00.00} z: {mouse.z:00.00}");
                }
            }
        }

        public void Update()
        {
            if (NoPlayers || !IsLocalGame)
            {
                if (TestWindow.Enabled)
                    TestWindow.Enabled = false;

                return; //MCI does only support localhosted lobbies.
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                TestWindow.Enabled = !TestWindow.Enabled;
                SettingsPatches.PresetButton.LoadPreset("Last Used", true);

                if (!TestWindow.Enabled)
                {
                    MCIUtils.RemoveAllPlayers();
                }
            }

            if (Input.GetKeyDown(KeyCode.F2))
                TestWindow.Enabled = !TestWindow.Enabled;
        }

        public void OnGUI() => TestWindow.OnGUI();
    }
}
