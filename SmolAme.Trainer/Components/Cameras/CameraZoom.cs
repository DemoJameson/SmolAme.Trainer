using System;
using BepInEx.Configuration;
using UnityEngine;

namespace SmolAme.Trainer.Components.Cameras;

public class CameraZoom : PluginComponent {
    private static ConfigEntry<KeyboardShortcut> cameraZoomInHotkey;
    private static ConfigEntry<KeyboardShortcut> cameraZoomOutHotkey;
    private static ConfigEntry<KeyboardShortcut> cameraResetHotkey;
    private static Vector3? origPosition;
    private const float step = 0.99f;
    private static float zoom = 1f;

    private void PreCull(Camera cam) {
        if (cam != Camera.main) {
            return;
        }

        if (Math.Abs(zoom) > 0.01f) {
            Vector3 position = cam.transform.position;
            origPosition = position;
            cam.transform.position = new Vector3(position.x, position.y, position.z * zoom);
        }
    }

    private void PostRender(Camera cam) {
        if (cam != Camera.main) {
            return;
        }

        if (origPosition.HasValue) {
            cam.transform.position = origPosition.Value;
            origPosition = null;
        }
    }

    private void Awake() {
        cameraZoomInHotkey = Plugin.Instance.Config.Bind("Camera", "Camera Zoom In", new KeyboardShortcut(KeyCode.PageDown, KeyCode.LeftControl));
        cameraZoomOutHotkey = Plugin.Instance.Config.Bind("Camera", "Camera Zoom Out", new KeyboardShortcut(KeyCode.PageUp, KeyCode.LeftControl));
        cameraResetHotkey = Plugin.Instance.Config.Bind("Camera", "Camera Reset", new KeyboardShortcut(KeyCode.End, KeyCode.LeftControl));
        Camera.onPreCull += PreCull;
        Camera.onPostRender += PostRender;
    }

    private void OnDestroy() {
        Camera.onPreCull -= PreCull;
        Camera.onPostRender -= PostRender;
    }

    private void Update() {
        if (cameraZoomInHotkey.IsPressedEx()) {
            zoom *= step;
        }

        if (cameraZoomOutHotkey.IsPressedEx()) {
            zoom /= step;
        }

        if (cameraResetHotkey.IsDownEx()) {
            zoom = 1f;
            CameraMove.Offset = Vector2.zero;
        }
    }
}