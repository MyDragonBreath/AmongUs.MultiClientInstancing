﻿using BepInEx;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes;
using MCI.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/// <summary>
/// ///
/// https://www.youtube.com/watch?v=-1qju6V1jLM
/// - MDB
///
/// </summary>
namespace MCI
{
    public class SubmergedCompatibility
    {
        public const string SUBMERGED_GUID = "Submerged";
        public const ShipStatus.MapType SUBMERGED_MAP_TYPE = (ShipStatus.MapType)5;

        public static SemanticVersioning.Version Version { get; private set; }
        public static bool Loaded { get; private set; }
        public static BasePlugin Plugin { get; private set; }
        public static Assembly Assembly { get; private set; }

        public static Type[] Types { get; private set; }
        public static Dictionary<string, Type> InjectedTypes { get; private set; }

        private static Type CustomPlayerData;
        private static FieldInfo hasMap;

        private static Type SpawnInState;
        private static FieldInfo currentState;

        public static void Initialize()
        {
            Loaded = IL2CPPChainloader.Instance.Plugins.TryGetValue(SUBMERGED_GUID, out PluginInfo plugin);
            if (!Loaded) return;

            Plugin = plugin!.Instance as BasePlugin;
            Version = plugin.Metadata.Version;

            Assembly = Plugin!.GetType().Assembly;
            Types = AccessTools.GetTypesFromAssembly(Assembly);
            InjectedTypes = (Dictionary<string, Type>)AccessTools.PropertyGetter(Array.Find(Types, t => t.Name == "ComponentExtensions"), "RegisteredTypes")
                .Invoke(null, Array.Empty<object>());

            CustomPlayerData = InjectedTypes.Where(t => t.Key == "CustomPlayerData").Select(x => x.Value).First();
            hasMap = AccessTools.Field(CustomPlayerData, "_hasMap");

            SpawnInState = Types.First(t => t.Name == "SpawnInState");

            var subSpawnSystem = Types.First(t => t.Name == "SubmarineSpawnInSystem");
            var GetReadyPlayerAmount = AccessTools.Method(subSpawnSystem, "GetReadyPlayerAmount");
            currentState = AccessTools.Field(subSpawnSystem, "currentState");

            MCIPlugin.Singleton.Harmony.Patch(GetReadyPlayerAmount, new HarmonyMethod(AccessTools.Method(typeof(SubmergedCompatibility), nameof(ReadyPlayerAmount))));
        }

        public static bool ReadyPlayerAmount(dynamic __instance, ref int __result)
        {
            if (!Loaded) return true;
            if (ModState.Enabled)
            {
                __result = __instance.GetTotalPlayerAmount();
                Enum.TryParse(SpawnInState, "Done", true, out object e);
                currentState.SetValue(__instance, e);
                return false;
            }
            return true;
        }

        public static void ImpartSub(PlayerControl bot)
        {
            var comp = TryCast(bot.gameObject.AddComponent(Il2CppType.From(CustomPlayerData)), CustomPlayerData);
            hasMap.SetValue(comp, true);
        }

        public static object TryCast(Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }
    }
}