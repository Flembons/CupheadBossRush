using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class HouseLevelTutorialChanges
    {
        public void Init()
        {
            On.HouseLevelTutorial.go_cr += go_cr;
            // On.HouseLevelTutorial.Update += Update;
        }

        private IEnumerator go_cr(On.HouseLevelTutorial.orig_go_cr orig, HouseLevelTutorial self)
        {
            self.activated = true;
            HouseLevel houseLevel = Level.Current as HouseLevel;
            if (houseLevel)
            {
                houseLevel.StartTutorial();
            }
            yield return CupheadTime.WaitForSeconds(self, 0.2f);
            if (BossRushManager.bossRushActive)
            {
                BossRushManager.initBossLevels();
                BossRushManager.startBossRush(true);
            }
            else
            {
                SceneLoader.LoadScene(Scenes.scene_level_tutorial, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
            }
            yield break;
        }

        /*
        private void Update(On.HouseLevelTutorial.orig_Update orig, HouseLevelTutorial self)
        {
            if (BossRushManager.bossRushActive)
            {
                Level.Mode bossRushDiff = BossRushManager.bossRushDiff;
                if (bossRushDiff != Level.Mode.Easy)
                {
                    if (bossRushDiff != Level.Mode.Normal)
                    {
                        self.dialogueProperties.text = "RANDOM BOSS RUSH (DIFFICULTY: EXPERT)";
                    }
                    else
                    {
                        self.dialogueProperties.text = "RANDOM BOSS RUSH (DIFFICULTY: REGULAR)";
                    }
                }
                else
                {
                    self.dialogueProperties.text = "RANDOM BOSS RUSH (DIFFICULTY: SIMPLE)";
                }
                if (!BossRushManager.refreshTutorialDialogue)
                {
                    return;
                }
                BossRushManager.refreshTutorialDialogue = false;
                if (self.state != AbstractLevelInteractiveEntity.State.Ready)
                {
                    return;
                }
                self.Hide(PlayerId.PlayerOne);
                self.Show(PlayerId.PlayerOne);
                return;
            }
            else
            {
                self.dialogueProperties.text = "TUTORIAL";
                if (!BossRushManager.refreshTutorialDialogue)
                {
                    return;
                }
                BossRushManager.refreshTutorialDialogue = false;
                if (self.state != AbstractLevelInteractiveEntity.State.Ready)
                {
                    return;
                }
                self.Hide(PlayerId.PlayerOne);
                self.Show(PlayerId.PlayerOne);
                return;
            }
        } */
    }
}
