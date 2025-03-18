using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class MapLevelLoaderChanges
    {
        public void Init()
        {
            On.MapLevelLoader.OnLoadLevel += OnLoadLevel;
            On.MapBakeryLoader.OnLoadLevel += OnLoadLevel;
        }

        private void OnLoadLevel(On.MapLevelLoader.orig_OnLoadLevel orig, MapLevelLoader self)
        {
            AbstractMapInteractiveEntity.HasPopupOpened = false;
            AudioManager.HandleSnapshot(AudioManager.Snapshots.Paused.ToString(), 0.5f);
            AudioNoiseHandler.Instance.BoingSound();
            if (Array.IndexOf<Levels>(BossRushManager.bossLevels, self.level) != -1 && BossRushManager.bossRushActive)
            {
                BossRushManager.initBossLevels();
                BossRushManager.startBossRushAtIndex(Array.IndexOf<Levels>(BossRushManager.bossLevels, self.level));
            }
            else if (self.level == Levels.Devil)
                SceneLoader.LoadScene(Scenes.scene_cutscene_devil, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris);
            else
                SceneLoader.LoadLevel(self.level, SceneLoader.Transition.Iris);
        }

        private void OnLoadLevel(On.MapBakeryLoader.orig_OnLoadLevel orig, MapBakeryLoader self)
        {
            AbstractMapInteractiveEntity.HasPopupOpened = false;
            AudioNoiseHandler.Instance.BoingSound();
            if (self.loadKitchen)
            {
                SceneLoader.LoadScene(Scenes.scene_level_kitchen, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.None, null);
                return;
            }
            if (BossRushManager.bossRushActive)
            {
                BossRushManager.initBossLevels();
                BossRushManager.startBossRushAtIndex(Array.IndexOf<Levels>(BossRushManager.bossLevels, Levels.Saltbaker));
                return;
            }
            SceneLoader.LoadScene(Scenes.scene_level_saltbaker, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
        }
    }
}
