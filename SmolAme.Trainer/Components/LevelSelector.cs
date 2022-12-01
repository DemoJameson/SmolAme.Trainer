using System.ComponentModel;
using BepInEx.Configuration;
using SmolAme.Trainer.Components.Helpers;
using UnityEngine.SceneManagement;

namespace SmolAme.Trainer.Components;

public class LevelSelector : PluginComponent {
    private static ConfigEntry<Level> levelConfig;

    public enum Level {
        MainMenu,
        [Description("Ame's Office")]
        AmeOffice,
        [Description("Ppo on Rocks")]
        PpoOnRocks,
        RedHeart,
        Pekoland,
        [Description("Ame's Office REVERSED")]
        AmeOfficeReversed,
        ToTheMoon,
        Nothing,
        MoguMogu,
        Inumore,
        Rushia,
        InascapableMadness,
        HereComesHope,
        Reflect
    }

    private void Awake() {
        levelConfig = Plugin.Instance.Config.Bind("General", "Level Selector", Level.MainMenu);
        levelConfig.SettingChanged += (_, _) => {
            if (SceneManager.GetActiveScene().buildIndex != (int) levelConfig.Value) {
                LevelLoader.loader.LoadLevel((int) levelConfig.Value);
            }
        };
        HookHelper.ActiveSceneChanged((_, scene) => levelConfig.Value = (Level) scene.buildIndex);
    }
}