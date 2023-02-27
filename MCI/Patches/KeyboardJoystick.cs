using HarmonyLib;
using UnityEngine;

namespace MCI.Patches
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    public sealed class Keyboard_Joystick
    {
        private static int controllingFigure;

        public static void Postfix()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                controllingFigure = PlayerControl.LocalPlayer.PlayerId;
                if (PlayerControl.AllPlayerControls.Count == 15 && !Input.GetKeyDown(KeyCode.F6)) return; //press f6 and f5 to bypass limit
                Utils.CleanUpLoad();
                Utils.CreatePlayerInstance("Robot");
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                controllingFigure++;
                controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
                InstanceControl.SwitchTo((byte)controllingFigure);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                controllingFigure--;
                controllingFigure = Mathf.Clamp(controllingFigure, 0, PlayerControl.AllPlayerControls.Count - 1);
                InstanceControl.SwitchTo((byte)controllingFigure);
            }
        }
    }
}
