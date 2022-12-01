using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace SmolAme.Trainer;

[HarmonyPatch]
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Smol Ame.exe")]
public class Plugin : BaseUnityPlugin {
    public static Plugin Instance { get; private set; }
    public static ManualLogSource Log => Instance.Logger;

    private void Awake() {
        Instance = this;
        PluginComponent.Initialize(gameObject);
    }
}