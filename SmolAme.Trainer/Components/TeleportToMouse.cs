using System;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;

namespace SmolAme.Trainer.Components;

public class TeleportToMouse : PluginComponent {
    private static ConfigEntry<KeyboardShortcut> teleportHotkey;
    private Plane plane = new(Vector3.forward, 0);

    private void Awake() {
        teleportHotkey = Plugin.Instance.Config.Bind("General", "Teleport To Mouse", new KeyboardShortcut(KeyCode.T));
        Camera.onPreRender += PreRender;
    }

    private void OnDestroy() {
        Camera.onPreRender -= PreRender;
    }

    private void Update() {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void PreRender(Camera cam) {
        if (teleportHotkey.IsDownEx() || UnityInput.Current.GetMouseButtonDown(1)) {
            PlayerScript player = PlayerScript.player;
            Ray ray = cam.ScreenPointToRay(UnityInput.Current.mousePosition);
            if (plane.Raycast(ray, out var enter)) {
                player.transform.position = ray.GetPoint(enter);
                player.rb.velocity = Vector2.zero;
                player.yV = 0;
            }
        }
    }
}