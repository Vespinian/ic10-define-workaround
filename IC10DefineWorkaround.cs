using System;
using System.Reflection;
using HarmonyLib;
using StationeersMods.Interface;
using Assets.Scripts.Objects.Electrical;
using System.Text.RegularExpressions;

namespace IC10DefineWorkaround
{
    [StationeersMod("IC10DefineWorkaround", "IC10DefineWorkaround", "1.0.0")]
    class IC10DefineWorkaround : ModBehaviour
    {

        public override void OnLoaded(ContentHandler contentHandler)
        {
            base.OnLoaded(contentHandler);


            Harmony harmony = new Harmony("IC10DefineWorkaround");
            harmony.PatchAll();
            UnityEngine.Debug.Log("IC10 Define Workaround Loaded!");
        }
    }

    [HarmonyPatch]
    public class GetInstructionDescriptionPatch
    {
        public static MethodBase TargetMethod()
        {
            var pcType = typeof(ProgrammableChip);
            var operationType = pcType.Assembly.GetType("Assets.Scripts.Objects.Electrical.ProgrammableChip/_Operation");
            return AccessTools.Method(operationType, "_MakeDeviceVariable");
        }

        [HarmonyPrefix]
        static bool Prefix(ref object __result, ProgrammableChip chip, int lineNumber, string deviceCode)
        {
            var directDeviceVarType = chip.GetType().Assembly.GetType("Assets.Scripts.Objects.Electrical.ProgrammableChip/_Operation/DirectDeviceVariable");

            var definesField = AccessTools.Field(chip.GetType(), "_Defines");
            var defines = definesField?.GetValue(chip) as System.Collections.Generic.Dictionary<string, double>;
            if (defines.ContainsKey(deviceCode))
            {
                var properties = InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex;
                __result = Activator.CreateInstance(directDeviceVarType, chip, lineNumber, deviceCode, properties, false);
                return false;
            }

            return true;
        }
    }
}
