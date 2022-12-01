using BepInEx.Configuration;
using HarmonyLib;
using SmolAme.Trainer.Components.Helpers;
using UnityEngine;

namespace SmolAme.Trainer.Components.Cameras;

[HarmonyPatch]
public class CameraFollow : PluginComponent {
    public static bool Following { get; private set; }
    private static ConfigEntry<KeyboardShortcut> cameraFollowHotkey;
    private static Vector2 FixedPosition => PlayerScript.player.transform.position + (Vector3) CameraMove.Offset;

    [HarmonyPatch(typeof(PlayerScript), nameof(PlayerScript.LateUpdate))]
    [HarmonyPostfix]
    private static void PlayerLateUpdate() {
        if (Following) {
            PlayerScript player = PlayerScript.player;
            player.camTarget.position = FixedPosition;
            player.camLookTarget.position = FixedPosition;
            player.camHeight = FixedPosition.y;
        }
    }

    [HarmonyPatch(typeof(CamScript), nameof(CamScript.LateUpdate))]
    [HarmonyPostfix]
    private static void CamScriptLateUpdate() {
        if (Following) {
            CamScript camScript = CamScript.camScript;
            camScript.transform.position = FixedPosition;
            camScript.targetPos.position = FixedPosition;
            camScript.lookPos.position = FixedPosition;
            camScript.targetLook.position = FixedPosition;
        }
    }

    private void Awake() {
        cameraFollowHotkey = Plugin.Instance.Config.Bind("Camera", "Toggle Camera Follow", new KeyboardShortcut(KeyCode.F, KeyCode.LeftControl));
    }

    private void Update() {
        if (cameraFollowHotkey.IsDownEx()) {
            Following = !Following;
            Toast.Show($"Camera Follow: {(Following ? "ON" : "OFF")}");
        }
    }
}