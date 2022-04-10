﻿using System.Collections.Generic;
using System.Reflection;
using RiskOfOptions.Containers;
using RiskOfOptions.Lib;
using RiskOfOptions.Options;
using UnityEngine;

using static RiskOfOptions.ExtensionMethods;
using ConCommandArgs = RoR2.ConCommandArgs;
using PauseManager = On.RoR2.PauseManager;

#pragma warning disable 618

namespace RiskOfOptions
{
    public static class ModSettingsManager
    {
        internal static readonly ModIndexedOptionCollection OptionCollection = new();

        internal const string StartingText = "risk_of_options";

        internal static bool doingKeyBind = false;
        
        internal static readonly List<string> RestartRequiredOptions = new();
        

        internal static void Init()
        {
            LanguageApi.Init();
            
            Resources.Assets.LoadAssets();
            Resources.Prefabs.Init();

            LanguageTokens.Register();
            
            SettingsModifier.Init();

            PauseManager.CCTogglePause += PauseManagerOnCCTogglePause;
        }

        private static void PauseManagerOnCCTogglePause(PauseManager.orig_CCTogglePause orig, ConCommandArgs args)
        {
            if (doingKeyBind)
                return;

            orig(args);
        }

        public static void SetModDescription(string description)
        {
            ModInfo modInfo = Assembly.GetCallingAssembly().GetModInfo();
            
            if (!OptionCollection.ContainsModGuid(modInfo.ModGuid))
                OptionCollection[modInfo.ModGuid] = new OptionCollection(modInfo.ModName, modInfo.ModGuid);

            OptionCollection[modInfo.ModGuid].description = description;
        }

        public static void SetModIcon(Sprite iconSprite)
        {
            ModInfo modInfo = Assembly.GetCallingAssembly().GetModInfo();

            if (!OptionCollection.ContainsModGuid(modInfo.ModGuid))
                OptionCollection[modInfo.ModGuid] = new OptionCollection(modInfo.ModName, modInfo.ModGuid);

            OptionCollection[modInfo.ModGuid].icon = iconSprite;
        }

        public static void AddOption(BaseOption option)
        {
            ModInfo modInfo = Assembly.GetCallingAssembly().GetModInfo();
            
            option.SetProperties();

            option.ModGuid = modInfo.ModGuid;
            option.ModName = modInfo.ModName;
            option.Identifier = $"{modInfo.ModGuid}.{option.Category}.{option.Name}.{option.OptionTypeName}".Replace(" ", "_").ToUpper();
            
            option.RegisterTokens();
            
            OptionCollection.AddOption(ref option);
        }
    }
}