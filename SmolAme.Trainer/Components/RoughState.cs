using BepInEx.Configuration;
using HarmonyLib;
using SmolAme.Trainer.Components.Helpers;
using UnityEngine;

namespace SmolAme.Trainer.Components;

[HarmonyPatch]
public class RoughState : PluginComponent {
    private static ConfigEntry<KeyboardShortcut> saveHotkey;
    private static ConfigEntry<KeyboardShortcut> loadHotkey;
    private static ConfigEntry<KeyboardShortcut> clearHotkey;
    private static ConfigEntry<bool> loadStateWhenDeath;

    private static string savedSceneName;
    private static Vector3? savedPosition;
    private static Vector3? savedCamTarget;
    private static Vector3? savedCamLookTarget;
    private static Vector2? savedVelocity;
    private static float? savedYv;
    private static PlayerState? savedCurrentState;
    private static bool? savedFacingLeft;
    private static float? savedTime;
    private static bool HasSaved => savedSceneName == CurrentSceneName;

    private void Awake() {
        int order = 0;
        saveHotkey = Plugin.Instance.Config.Bind("Rough State", "Save State", new KeyboardShortcut(KeyCode.F10), --order);
        loadHotkey = Plugin.Instance.Config.Bind("Rough State", "Load State", new KeyboardShortcut(KeyCode.F11), --order);
        clearHotkey = Plugin.Instance.Config.Bind("Rough State", "Clear State", new KeyboardShortcut(KeyCode.F12), --order);
        loadStateWhenDeath = Plugin.Instance.Config.Bind("Rough State", "Load State When Death", false, --order);
    }

    private void Update() {
        if (saveHotkey.IsDownEx()) {
            SaveState();
        } else if (loadHotkey.IsDownEx()) {
            LoadState();
        } else if (clearHotkey.IsDownEx()) {
            ClearState();
        }
    }

    private static void SaveState() {
        Toast.Show("Save State");

        savedSceneName = CurrentSceneName;
        savedPosition = Player.transform.position;
        savedCamTarget = Player.camTarget.position;
        savedCamLookTarget = Player.camLookTarget.position;
        savedVelocity = Player.rb.velocity;
        savedYv = Player.yV;
        savedCurrentState = Player.currentState;
        savedFacingLeft = Player.facingLeft;
        savedTime = Main.levelTime;
    }

    private static void LoadState() {
        if (!HasSaved) {
            Toast.Show("No Saved State");
            return;
        }

        Player.transform.position = savedPosition.Value;
        Player.camTarget.position = savedCamTarget.Value;
        Player.camLookTarget.position = savedCamLookTarget.Value;
        Player.rb.velocity = savedVelocity.Value;
        Player.yV = savedYv.Value;
        Player.currentState = savedCurrentState.Value;
        Player.facingLeft = savedFacingLeft.Value;
        Main.ResetLevel();
        Cam.SnapToPos();
        Main.levelTime = savedTime.Value;
    }

    private static void ClearState() {
        savedSceneName = null;
        Toast.Show("Clear State");
    }

    [HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.Kill))]
    [HarmonyPrefix]
    private static bool PlayerScriptKill(PlayerScript __instance) {
        if (HasSaved && loadStateWhenDeath.Value && !MainScript.victory && __instance.currentState != PlayerState.Dead) {
            LoadState();
            return false;
        } else {
            return true;
        }
    }
}