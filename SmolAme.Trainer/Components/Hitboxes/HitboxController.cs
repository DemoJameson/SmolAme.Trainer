using System.Collections;
using BepInEx.Configuration;
using HarmonyLib;
using Mono.Cecil.Cil;
using MonoMod.Cil;
using SmolAme.Trainer.Components.Helpers;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SmolAme.Trainer.Components.Hitboxes;

[HarmonyPatch]
public class HitboxController : PluginComponent {
    public static bool Show { get; private set; }
    private static ConfigEntry<KeyboardShortcut> hitboxHotkey;

    [HarmonyPatch(typeof(MoguCloudSpawner), nameof(MoguCloudScript.Update))]
    [HarmonyPatch(typeof(GenericSpawner), nameof(GenericSpawner.Update))]
    [HarmonyPatch(typeof(PlatformSpawner), nameof(PlatformSpawner.Update))]
    [HarmonyPatch(typeof(PekoArrowSpawner), nameof(PekoArrowSpawner.Update))]
    [HarmonyPatch(typeof(TarantulaSpawnerScript), nameof(TarantulaSpawnerScript.SpawnNew))]
    [HarmonyILManipulator]
    private static void SpawnerUpdate(ILContext ilContext) {
        ILCursor ilCursor = new(ilContext);
        if (ilCursor.TryGotoNext(i => i.OpCode == OpCodes.Call && i.Operand.ToString().Contains("UnityEngine.Object::Instantiate"))) {
            ilCursor.Index++;
            ilCursor.Emit(OpCodes.Dup).EmitDelegate(AddHitboxRenderer);
        }
    }

    public void Awake() {
        hitboxHotkey = Plugin.Instance.Config.Bind("General", "Toggle Hitboxes", new KeyboardShortcut(KeyCode.H, KeyCode.LeftControl));
        HookHelper.ActiveSceneChanged(() => StartCoroutine(DelayedSetupHitboxes()));
        SetupHitboxes();
    }

    private void OnDestroy() {
        HitboxRenderer[] renderers = FindObjectsOfType<HitboxRenderer>();
        foreach (HitboxRenderer renderer in renderers) {
            Destroy(renderer);
        }
    }

    private void Update() {
        if (hitboxHotkey.IsDownEx()) {
            ToggleHitboxes();
        }
    }

    private static void ToggleHitboxes() {
        Show = !Show;

        HitboxRenderer[] renderers = FindObjectsOfType<HitboxRenderer>();
        foreach (HitboxRenderer renderer in renderers) {
            renderer.SetEnabled(Show);
        }
    }

    private static IEnumerator DelayedSetupHitboxes() {
        yield return null;
        SetupHitboxes();
    }

    private static void SetupHitboxes() {
        foreach (GameObject rootGameObject in SceneManager.GetActiveScene().GetRootGameObjects()) {
            AddHitboxRenderer(rootGameObject);
        }
    }

    private static void AddHitboxRenderer(GameObject gameObject) {
        Collider2D[] componentsInChildren = gameObject.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D collider2D in componentsInChildren) {
            if (!collider2D.gameObject.GetComponent<HitboxRenderer>()) {
                collider2D.gameObject.AddComponent<HitboxRenderer>();
            }
        }
    }
}