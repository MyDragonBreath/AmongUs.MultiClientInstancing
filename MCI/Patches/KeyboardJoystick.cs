using HarmonyLib;
using UnityEngine;

namespace MCI.Patches
{
    [HarmonyPatch(typeof(KeyboardJoystick), nameof(KeyboardJoystick.Update))]
    class Keyboard_Joystick
    {
        public static int controllingFigure = 0;
        public static void Postfix()
        {
            if (Input.GetKeyDown(KeyCode.F5))
            {
                controllingFigure = PlayerControl.LocalPlayer.PlayerId;
                Utils.CleanUpLoad();
                Utils.CreatePlayerInstance("Robot");
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                controllingFigure++;
                InstanceControl.SwitchTo((byte)controllingFigure);
            }

            if (Input.GetKeyDown(KeyCode.F10))
            {
                controllingFigure--;
                InstanceControl.SwitchTo((byte)controllingFigure);
            }
        }
    }
}
