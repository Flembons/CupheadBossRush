using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class MapDicePalaceSceneLoaderChanges
    {
        public void Init()
        {
            On.MapDicePalaceSceneLoader.LoadScene += LoadScene;
        }
        protected void LoadScene(On.MapDicePalaceSceneLoader.orig_LoadScene orig, MapDicePalaceSceneLoader self)
        {
            if (!PlayerData.Data.GetLevelData(Levels.DicePalaceMain).played)
            {
                SceneLoader.LoadScene(self.scene, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
                return;
            }
            if (BossRushManager.bossRushActive)
            {
                BossRushManager.initBossLevels();
                BossRushManager.startBossRushAtIndex(Array.IndexOf<Levels>(BossRushManager.bossLevels, Levels.DicePalaceMain));
                return;
            }
            SceneLoader.LoadScene(Scenes.scene_level_dice_palace_main, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
        }

    }
}
