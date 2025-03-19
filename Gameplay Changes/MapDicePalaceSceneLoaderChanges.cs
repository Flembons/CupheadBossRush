using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    /*
     * King Dice's fight has a special map loader separate from all the other MapLevelLoaders, so
     * it needed to be changed to specifically start a boss rush at King Dice
     */
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
            // Start a boss rush beginning at the King Dice fight
            if (BossRushManager.bossRushActive)
            {
                BossRushManager.initBossLevels();
                BossRushManager.startBossRushAtIndex(Array.IndexOf<Levels>(BossRushManager.bossLevels, Levels.DicePalaceMain));
                return;
            }

            // Otherwise load the regular King Dice/Casino bosses fight
            SceneLoader.LoadScene(Scenes.scene_level_dice_palace_main, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
        }

    }
}
