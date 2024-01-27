using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// RENAME 'OutwardModTemplate' TO SOMETHING ELSE
namespace ProControllerFix
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        // Choose a GUID for your project. Change "myname" and "mymod".
        public const string GUID = "technicalslayer.procontrollerfix";
        // Choose a NAME for your project, generally the same as your Assembly Name.
        public const string NAME = "Pro Controller Fix";
        // Increment the VERSION when you release a new version of your mod.
        public const string VERSION = "0.0.1";

        // For accessing your BepInEx Logger from outside of this class (eg Plugin.Log.LogMessage("");)
        internal static ManualLogSource Log;

        // If you need settings, define them like so:
        public static ConfigEntry<bool> ExampleConfig;

        // Awake is called when your plugin is created. Use this to set up your mod.
        internal void Awake()
        {
            Log = this.Logger;
            Log.LogMessage($"Hello world from {NAME} {VERSION}!");

            // Any config settings you define should be set up like this:
            ExampleConfig = Config.Bind("ExampleCategory", "ExampleSetting", false, "This is an example setting.");

            // Harmony is for patching methods. If you're not patching anything, you can comment-out or delete this line.
            new Harmony(GUID).PatchAll();
        }


        //[HarmonyPatch(typeof(SplitScreenManager), "Awake")]
        //public class SplitScreenManager_Awake
        //{
        //    [HarmonyPostfix]
        //    public static void Postfix(SplitScreenManager __instance) {
        //        UnityEngine.Object.DontDestroyOnLoad((UnityEngine.Object)(object)__instance.m_charUIPrefab);
        //        SetupQuickslotPanels(__instance.m_charUIPrefab);
        //    }
        //}

        //[HarmonyPatch(typeof(CharacterQuickSlotManager), "Awake")]
        //public class CharacterQuickSlotManager_Awake
        //{
        //    [HarmonyPrefix]
        //    public static void Prefix(CharacterQuickSlotManager __instance) {
        //        SetupQuickslots(((Component)__instance).transform.Find("QuickSlots"));
        //    }
        //}

        // using nameof instead of quotations allowed this to work for some reason
        [HarmonyPatch(typeof(QuickSlotDisplay), nameof(QuickSlotDisplay.RefreshInput))]
        public class QuickslotDisplay_RefreshInput
        {
            [HarmonyPostfix]
            public static void Postfix(QuickSlotDisplay __instance) {
                int replacementSlotID = 0;
                switch (__instance.RefSlotID) {
                    case 0:
                        replacementSlotID = 2;
                        break;
                    case 1:
                        replacementSlotID = 1;
                        break;
                    case 2:
                        replacementSlotID = 4;
                        break;
                    case 3:
                        replacementSlotID = 3;
                        break;
                    case 4:
                        replacementSlotID = 6;
                        break;
                    case 5:
                        replacementSlotID = 5;
                        break;
                    case 6:
                        replacementSlotID = 8;
                        break;
                    case 7:
                        replacementSlotID = 7;
                        break;
                    default:
                        replacementSlotID = 0;
                        break;
                }

                GlyphData testingForGlyphs = ControlsInput.GetFirstElementMapWithQuickSlot(__instance.m_characterUI.RewiredID, 
                    (QuickSlot.QuickSlotIDs)replacementSlotID, __instance.m_gamepadMode);
                __instance.m_inputIcon.overrideSprite = UIUtilities.Controller.GetGlyph(testingForGlyphs.controllerGUI, testingForGlyphs.aem, null);
   
                //Log.LogMessage("Quick Slot Ref ID: " + __instance.RefSlotID + ", and Name: " + __instance.name);
            }
        }

        [HarmonyPatch(typeof(ControlsInput), "QuickSlot1")]
        public class ControlsInput_QuickSlot1
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result, ControlsInput __instance, ref int _playerID) {
                //__result = ControlsInput.QuickSlot2(_playerID);
                __result = ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot2");
            }
        }

        [HarmonyPatch(typeof(ControlsInput), "QuickSlot2")]
        public class ControlsInput_QuickSlot2
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result, ControlsInput __instance, ref int _playerID) {
                __result = ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot1");
            }
        }

        [HarmonyPatch(typeof(ControlsInput), "QuickSlot3")]
        public class ControlsInput_QuickSlot3
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result, ControlsInput __instance, ref int _playerID) {
                //__result = ControlsInput.QuickSlot2(_playerID);
                __result = ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot4");
            }
        }

        [HarmonyPatch(typeof(ControlsInput), "QuickSlot4")]
        public class ControlsInput_QuickSlot4
        {
            [HarmonyPostfix]
            public static void Postfix(ref bool __result, ControlsInput __instance, ref int _playerID) {
                __result = ControlsInput.m_playerInputManager[_playerID].GetButtonDown("QuickSlot3");
            }
        }


        private static Transform GetMenuPanelsHolder(CharacterUI ui) {
            return ((Component)ui).transform.Find("Canvas/GameplayPanels/Menus/CharacterMenus/MainPanel/Content/MiddlePanel/QuickSlotPanel/PanelSwitcher/Controller/LT-RT");
        }

        private static Transform GetGameQuickSlotsRT(CharacterUI ui) {
            return ((Component)ui).transform.Find("Canvas/GameplayPanels/HUD/QuickSlot/Controller/LT-RT/RT/QuickSlots");
        }

        private static Transform GetGameQuickSlotsLT(CharacterUI ui) {
            return ((Component)ui).transform.Find("Canvas/GameplayPanels/HUD/QuickSlot/Controller/LT-RT/LT/QuickSlots");
        }

        //private static void SetupQuickslots(Transform quickslotsHolder) {
        //    Transform val = quickslotsHolder.Find("1");
        //    for (int i = quickslotsHolder.childCount; i < 16; i++) {
        //        UnityEngine.Object.Instantiate<Transform>(val, quickslotsHolder);
        //    }
        //    QuickSlot[] componentsInChildren = ((Component)quickslotsHolder).GetComponentsInChildren<QuickSlot>();
        //    for (int j = 0; j < componentsInChildren.Length; j++) {
        //        ((UnityEngine.Object)componentsInChildren[j]).name = (j + 1).ToString();
        //        componentsInChildren[j].ItemQuickSlot = false;
        //    }
        //}

        //private static void SetupQuickslotPanels(CharacterUI ui) {
        //    Transform menuPanelsHolder = GetMenuPanelsHolder(ui);
        //    Transform gamePanelsHolderRT = GetGameQuickSlotsRT(ui);
        //    Transform gamePanelsHolderLT = GetGameQuickSlotsLT(ui);

        //    // swap "A" and "B" positions and parents
        //    String glyphAPath = "Slot_A/QuickSlotDisplay(Clone)/Panel/Input/imgInput";
        //    String glyphBPath = "Slot_B/QuickSlotDisplay(Clone)/Panel/Input/imgInput";
        //    String glyphXPath = "Slot_X/QuickSlotDisplay(Clone)/Panel/Input/imgInput";
        //    String glyphYPath = "Slot_Y/QuickSlotDisplay(Clone)/Panel/Input/imgInput";
        //    gamePanelsHolderLT.Find(glyphAPath).gameObject.SetActive(false);
        //    gamePanelsHolderLT.Find(glyphBPath).gameObject.SetActive(false);
        //    gamePanelsHolderLT.Find(glyphXPath).gameObject.SetActive(false);
        //    gamePanelsHolderLT.Find(glyphYPath).gameObject.SetActive(false);
        //    //Vector3 glyphPosHelper = gamePanelsHolderLT.Find(glyphAPath).localPosition; // Store A pos
        //    //Transform glyphTransHelper = gamePanelsHolderLT.Find(glyphAPath).parent; // Store A parent
        //    //gamePanelsHolderLT.Find(glyphAPath).parent = gamePanelsHolderLT.Find(glyphBPath).parent; // Move A transform
        //    //gamePanelsHolderLT.Find(glyphAPath).localPosition = gamePanelsHolderLT.Find(glyphBPath).localPosition; // Move A position
        //    //gamePanelsHolderLT.Find(glyphBPath).parent = glyphTransHelper; // Move B transform to parent
        //    //gamePanelsHolderLT.Find(glyphBPath).localPosition = glyphPosHelper; // Move B position

        //    // swap "X" and "Y"


        //    // Idk what this does
        //    ((Component)menuPanelsHolder.Find("LeftDecoration")).gameObject.SetActive(false);
        //    ((Component)menuPanelsHolder.Find("RightDecoration")).gameObject.SetActive(false);
        //}
    }
}
