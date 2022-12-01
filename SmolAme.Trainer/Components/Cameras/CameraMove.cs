using BepInEx.Configuration;
using UnityEngine;

namespace SmolAme.Trainer.Components.Cameras;

public class CameraMove : PluginComponent {
    private static ConfigEntry<KeyboardShortcut> cameraMoveLeftHotkey;
    private static ConfigEntry<KeyboardShortcut> cameraMoveRightHotkey;
    private static ConfigEntry<KeyboardShortcut> cameraMoveDownHotkey;
    private static ConfigEntry<KeyboardShortcut> cameraMoveUpHotkey;
    private const float step = 0.2f;
    public static Vector2 Offset = Vector2.zero;

    private void Awake() {
        int order = 0;
        cameraMoveLeftHotkey = Plugin.Instance.Config.Bind("Camera Move (Require Camera Follow)", "Camera Move Left",
            new KeyboardShortcut(KeyCode.J, KeyCode.LeftControl), --order);
        cameraMoveRightHotkey = Plugin.Instance.Config.Bind("Camera Move (Require Camera Follow)", "Camera Move Right",
            new KeyboardShortcut(KeyCode.L, KeyCode.LeftControl), --order);
        cameraMoveDownHotkey = Plugin.Instance.Config.Bind("Camera Move (Require Camera Follow)", "Camera Move Down",
            new KeyboardShortcut(KeyCode.K, KeyCode.LeftControl), --order);
        cameraMoveUpHotkey = Plugin.Instance.Config.Bind("Camera Move (Require Camera Follow)", "Camera Move Up",
            new KeyboardShortcut(KeyCode.I, KeyCode.LeftControl), --order);
    }

    private void Update() {
        if (!CameraFollow.Following) {
            return;
        }

        if (cameraMoveLeftHotkey.IsPressedEx()) {
            Offset.x -= step;
        }

        if (cameraMoveRightHotkey.IsPressedEx()) {
            Offset.x += step;
        }

        if (cameraMoveDownHotkey.IsPressedEx()) {
            Offset.y -= step;
        }

        if (cameraMoveUpHotkey.IsPressedEx()) {
            Offset.y += step;
        }
    }
}