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
            var directIndexVarType = chip.GetType().Assembly.GetType("Assets.Scripts.Objects.Electrical.ProgrammableChip/_Operation/DirectIndexVariable");
            var directAliasVarType = chip.GetType().Assembly.GetType("Assets.Scripts.Objects.Electrical.ProgrammableChip/_Operation/DirectAliasVariable");
            var properties = InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex;
            // if (deviceCode.Length > 0 && (deviceCode[0] == '_'))
            // {
            //     __result = Activator.CreateInstance(directDeviceVarType, chip, lineNumber, deviceCode, properties, false);
            //     return false;
            // }

            if (deviceCode.Length > 0 && (deviceCode[0] == '$' || deviceCode[0] == '%' || char.IsDigit(deviceCode[0])))
            {
                properties = InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex;
                __result = Activator.CreateInstance(directDeviceVarType, chip, lineNumber, deviceCode, properties, false);
                // __result = (ProgrammableChip._Operation.IDeviceVariable)new ProgrammableChip._Operation.DirectDeviceVariable(chip, lineNumber, deviceCode, InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex, false);
                return false;
            }
            if (deviceCode.Length > 1 && deviceCode[0] == 'r' && char.IsDigit(deviceCode[1]))
            {
                properties = InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex;
                __result = Activator.CreateInstance(directDeviceVarType, chip, lineNumber, deviceCode, properties, false);
                // __result = (ProgrammableChip._Operation.IDeviceVariable)new ProgrammableChip._Operation.DirectDeviceVariable(chip, lineNumber, deviceCode, InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex, false);
                return false;
            }
            string[] strArray = deviceCode.Split(new string[] { ":" }, StringSplitOptions.None);
            if (strArray.Length != 0 && strArray[0].StartsWith("d"))
            {
                string input = strArray[0];
                if (input == "db")
                {
                    properties = InstructionInclude.MaskDeviceIndex;
                    __result = Activator.CreateInstance(directIndexVarType, chip, lineNumber, deviceCode, properties, false);
                    // __result = (ProgrammableChip._Operation.IDeviceVariable)new ProgrammableChip._Operation.DeviceIndexVariable(chip, lineNumber, deviceCode, InstructionInclude.MaskDeviceIndex, false);
                    return false;
                }
                if (Regex.IsMatch(input, "^(d[0-9]|dr*[r0-9][0-9])$"))
                {
                    properties = InstructionInclude.MaskDeviceIndex;
                    __result = Activator.CreateInstance(directIndexVarType, chip, lineNumber, deviceCode, properties, false);
                    // __result = (ProgrammableChip._Operation.IDeviceVariable)new ProgrammableChip._Operation.DeviceIndexVariable(chip, lineNumber, deviceCode, InstructionInclude.MaskDeviceIndex, false);
                    return false;
                }
            }

            var definesField = AccessTools.Field(chip.GetType(), "_Defines");
            var defines = definesField?.GetValue(chip) as System.Collections.Generic.Dictionary<string, double>;
            if (defines.ContainsKey(deviceCode))
            {
                properties = InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex;
                __result = Activator.CreateInstance(directDeviceVarType, chip, lineNumber, deviceCode, properties, false);
                return false;
            }

            properties = InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex;
            __result = Activator.CreateInstance(directAliasVarType, chip, lineNumber, deviceCode, properties, false);
            // __result = (ProgrammableChip._Operation.IDeviceVariable)new ProgrammableChip._Operation.DeviceAliasVariable(chip, lineNumber, deviceCode, InstructionInclude.MaskDoubleValue | InstructionInclude.DeviceIndex | InstructionInclude.NetworkIndex, false);
            return false;
        }
    }
}
