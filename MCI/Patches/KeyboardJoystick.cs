using HarmonyLib;
using UnityEngine;

namespace MCI.Patches;

[HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
public static class Keyboard_Joystick
{
    public static int ControllingFigure;

    public static void Postfix()
    {
        if (!MCIPlugin.Enabled)
            return;

        if (Input.GetKeyDown(KeyCode.F5))
        {
            ControllingFigure = PlayerControl.LocalPlayer.PlayerId;

            if (PlayerControl.AllPlayerControls.Count == 15 && !Input.GetKeyDown(KeyCode.F6))
                return; //press f6 and f5 to bypass limit

            InstanceControl.CleanUpLoad();
            InstanceControl.CreatePlayerInstance();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            if (LobbyBehaviour.Instance)
                return;

            Cycle(true);
            InstanceControl.SwitchTo((byte)ControllingFigure);
        }

        if (Input.GetKeyDown(KeyCode.F10))
        {
            if (LobbyBehaviour.Instance)
                return;

            Cycle(false);
            InstanceControl.SwitchTo((byte)ControllingFigure);
        }

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.F6))
        {
            MCIPlugin.IKnowWhatImDoing = !MCIPlugin.IKnowWhatImDoing;
            InstanceControl.UpdateNames(MCIPlugin.RobotName);
        }
        else if (Input.GetKeyDown(KeyCode.F6))
            MCIPlugin.Persistence = !MCIPlugin.Persistence;

        if (Input.GetKeyDown(KeyCode.F11))
            InstanceControl.RemoveAllPlayers();
    }

    private static void Cycle(bool increment)
    {
        if (increment)
            ControllingFigure++;
        else
            ControllingFigure--;

        if (ControllingFigure < 0)
            ControllingFigure = InstanceControl.Clients.Count - 1;
        else if (ControllingFigure >= InstanceControl.Clients.Count)
            ControllingFigure = 0;
    }
}
