using System;
using System.Reflection;
using ICities;
using HarmonyLib;
using CitiesHarmony.API;
using ColossalFramework.PlatformServices;

namespace DLCEnabler
{
    public class Mod : IUserMod
    {
        public string Name
        {
            get { return "DLC Enabler & Asset Restorer"; }
        }

        public string Description
        {
            get { return "Ensures all installed DLCs, networks, and assets load reliably."; }
        }

        public void OnEnabled()
        {
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.PatchAll();
            }
            else
            {
                HarmonyHelper.DoOnHarmonyReady(Patcher.PatchAll);
            }
        }

        public void OnDisabled()
        {
            Patcher.UnpatchAll();
        }
    }

    public static class Patcher
    {
        private const string HarmonyId = "com.antigravity.dlcenabler";
        private static bool patched = false;

        public static void PatchAll()
        {
            if (patched) return;
            try
            {
                var harmony = new Harmony(HarmonyId);
                harmony.PatchAll(Assembly.GetExecutingAssembly());
                patched = true;
                UnityEngine.Debug.Log("[DLCEnabler] Successfully applied DLC patches!");
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[DLCEnabler] Failed to patch: " + ex);
            }
        }

        public static void UnpatchAll()
        {
            if (!patched) return;
            try
            {
                var harmony = new Harmony(HarmonyId);
                harmony.UnpatchAll(HarmonyId);
                patched = false;
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogError("[DLCEnabler] Failed to unpatch: " + ex);
            }
        }
    }

    [HarmonyPatch(typeof(SteamHelper), "IsDLCOwned", new Type[] { typeof(SteamHelper.DLC) })]
    public static class SteamHelper_IsDLCOwned_Patch
    {
        public static bool Prefix(SteamHelper.DLC dlc, ref bool __result)
        {
            if (dlc == SteamHelper.DLC.None || dlc == SteamHelper.DLC.NotAllowed)
            {
                __result = false;
            }
            else
            {
                __result = true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SteamHelper), "IsDLCAvailable", new Type[] { typeof(SteamHelper.DLC) })]
    public static class SteamHelper_IsDLCAvailable_Patch
    {
        public static bool Prefix(SteamHelper.DLC dlc, ref bool __result)
        {
            if (dlc == SteamHelper.DLC.None || dlc == SteamHelper.DLC.NotAllowed)
            {
                __result = false;
            }
            else
            {
                __result = true;
            }
            return false;
        }
    }

    [HarmonyPatch(typeof(SteamHelper), "GetOwnedExpansionMask")]
    public static class SteamHelper_GetOwnedExpansionMask_Patch
    {
        public static bool Prefix(ref SteamHelper.ExpansionBitMask __result)
        {
            __result = (SteamHelper.ExpansionBitMask)int.MaxValue;
            return false;
        }
    }

    [HarmonyPatch(typeof(SteamHelper), "GetOwnedModderPackMask")]
    public static class SteamHelper_GetOwnedModderPackMask_Patch
    {
        public static bool Prefix(ref SteamHelper.ModderPackBitMask __result)
        {
            __result = (SteamHelper.ModderPackBitMask)int.MaxValue;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlatformService), "IsDlcInstalled", new Type[] { typeof(uint) })]
    public static class PlatformService_IsDlcInstalled_Patch
    {
        public static bool Prefix(uint appid, ref bool __result)
        {
            __result = true;
            return false;
        }
    }

    [HarmonyPatch(typeof(PlatformService), "IsDlcAvailable", new Type[] { typeof(uint) })]
    public static class PlatformService_IsDlcAvailable_Patch
    {
        public static bool Prefix(uint appid, ref bool __result)
        {
            __result = true;
            return false;
        }
    }
}
