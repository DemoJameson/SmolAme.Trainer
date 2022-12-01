using BepInEx.Configuration;
using HarmonyLib;

namespace SmolAme.Trainer.Components; 

[HarmonyPatch]
public class PreventSavingToDisk : PluginComponent {
    private static ConfigEntry<bool> preventSavingToDisk; 
    
    private void Awake() {
        preventSavingToDisk = Plugin.Instance.Config.Bind("General", "Prevent Saving To Disk", false);
    }

    [HarmonyPatch(typeof(MainScript), nameof(MainScript.SaveSaveData))]
    [HarmonyPrefix]
    private static bool MainScriptSaveSaveData() {
        return !preventSavingToDisk.Value;
    }
}