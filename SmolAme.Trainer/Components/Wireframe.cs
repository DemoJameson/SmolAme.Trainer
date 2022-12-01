using System.Collections.Generic;
using BepInEx.Configuration;
using UnityEngine;

namespace SmolAme.Trainer.Components;

// modify from https://github.com/ManlyMarco/RuntimeUnityEditor/blob/master/RuntimeUnityEditor/Features/WireframeFeature.cs
public class Wireframe : PluginComponent {
    private static ConfigEntry<KeyboardShortcut> wireframeHotkey;
    private static bool wireframeMode;
    private static readonly Dictionary<Camera, CameraClearFlags> origFlags = new();
    private static Color? origBackgroundColor;

    private void Awake() {
        wireframeHotkey = Plugin.Instance.Config.Bind("General", "Toggle Wireframe Mode", new KeyboardShortcut(KeyCode.W, KeyCode.LeftControl));
        Camera.onPreRender += PreRender;
        Camera.onPostRender += PostRender;
    }

    private void OnDestroy() {
        Camera.onPreRender -= PreRender;
        Camera.onPostRender -= PostRender;
    }

    private void Update() {
        if (wireframeHotkey.IsDownEx()) {
            wireframeMode = !wireframeMode;
        }
    }

    private static void PreRender(Camera cam) {
        if (GL.wireframe) {
            return;
        }

        if (!wireframeMode) {
            return;
        }

        origBackgroundColor = Camera.main.backgroundColor;
        Camera.main.backgroundColor = Color.black;

        if (!origFlags.ContainsKey(cam)) {
            origFlags.Add(cam, cam.clearFlags);
        }

        cam.clearFlags = CameraClearFlags.Color;
        GL.wireframe = true;
    }

    private static void PostRender(Camera cam) {
        if (origBackgroundColor.HasValue) {
            cam.backgroundColor = origBackgroundColor.Value;
        }

        if (origFlags.TryGetValue(cam, out CameraClearFlags flags)) {
            cam.clearFlags = flags;
            GL.wireframe = false;
        }
    }
}