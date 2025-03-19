using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    /*
     * This class contains changes made to the tutorial (both the ShmupTutorial and Elder Kettle House Tutorial)
     * The Shmup tutorial now loads the next boss if boss rush is active, and the Elder Kettle tutorial now starts
     * a randomized boss rush when interacted with.
     */

    public class TutorialChanges
    {
        public void Init()
        {
            On.ShmupTutorialExitSign.go_cr += go_cr;
            On.HouseLevelTutorial.go_cr += go_cr;
            On.AbstractLevelInteractiveEntity.FixedUpdate += FixedUpdate;
            On.AbstractLevelInteractiveEntity.Show += Show;
            On.AbstractLevelInteractiveEntity.Awake += Awake;
        }

        // Altered the exit of the Tutorial to load the next boss rush boss if it is active
        private IEnumerator go_cr(On.ShmupTutorialExitSign.orig_go_cr orig, ShmupTutorialExitSign self)
        {
            self.activated = true;
            PlayerData.SaveCurrentFile();
            CupheadTime.SetLayerSpeed(CupheadTime.Layer.Player, 0f);
            PlanePlayerController[] array = UnityEngine.Object.FindObjectsOfType<PlanePlayerController>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].PauseAll();
            }
            PlaneSuperBomb[] array2 = UnityEngine.Object.FindObjectsOfType<PlaneSuperBomb>();
            for (int i = 0; i < array2.Length; i++)
            {
                array2[i].Pause();
            }
            PlaneSuperChalice[] array3 = UnityEngine.Object.FindObjectsOfType<PlaneSuperChalice>();
            for (int i = 0; i < array3.Length; i++)
            {
                array3[i].Pause();
            }
            yield return CupheadTime.WaitForSeconds(self, 1f);
            if (BossRushManager.inBossRush)
            {
                BossRushManager.bossesDefeated--;
                BossRushManager.loadNextBossRushLevel();
            }
            else
            {
                SceneLoader.LoadScene(Scenes.scene_map_world_1, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
            }
            yield break;
        }

        // A randomized boss rush will now start if the Elder Kettle tutorial is interacted with while Boss Rush is active
        private IEnumerator go_cr(On.HouseLevelTutorial.orig_go_cr orig, HouseLevelTutorial self)
        {
            self.activated = true;
            HouseLevel houseLevel = Level.Current as HouseLevel;
            if (houseLevel)
            {
                houseLevel.StartTutorial();
            }
            yield return CupheadTime.WaitForSeconds(self, 0.2f);

            // Check if boss rush is active and start a randomized boss rush if it is active
            if (BossRushManager.bossRushActive)
            {
                BossRushManager.initBossLevels();
                BossRushManager.startBossRush(true);
            }
            // otherwise, load the tutorial as normal
            else
            {
                SceneLoader.LoadScene(Scenes.scene_level_tutorial, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
            }
            yield break;
        }

        // Whenever Boss Rush is active, the text on the tutorial prompt will change to reflect that a random boss rush
        // for your currently selected difficulty will start when the tutorial is selected. Turning Boss Rush off will change
        // the tutorial prompt back to it's original text
        private void FixedUpdate (On.AbstractLevelInteractiveEntity.orig_FixedUpdate orig, AbstractLevelInteractiveEntity self)
        {
            if (self as HouseLevelTutorial)
            {
                if (BossRushManager.bossRushActive && BossRushManager.refreshTutorialDialogue)
                {
                    Level.Mode bossRushDiff = BossRushManager.bossRushDiff;

                    switch (bossRushDiff)
                    {
                        case Level.Mode.Easy:
                            self.dialogueProperties.text = "RANDOM BOSS RUSH (DIFFICULTY: SIMPLE)";
                            break;
                        case Level.Mode.Normal:
                            self.dialogueProperties.text = "RANDOM BOSS RUSH (DIFFICULTY: REGULAR)";
                            break;
                        case Level.Mode.Hard:
                            self.dialogueProperties.text = "RANDOM BOSS RUSH (DIFFICULTY: EXPERT)";
                            break;
                    }
                    BossRushManager.refreshTutorialDialogue = false;
                    if (self.state == AbstractLevelInteractiveEntity.State.Ready)
                    {
                        self.Hide(PlayerId.PlayerOne);
                        self.Show(PlayerId.PlayerOne);
                    }
                    return;
                }
                else if (!BossRushManager.bossRushActive && BossRushManager.refreshTutorialDialogue)
                {
                    self.dialogueProperties.text = "TUTORIAL";
                    BossRushManager.refreshTutorialDialogue = false;

                    if (self.state == AbstractLevelInteractiveEntity.State.Ready)
                    {
                        self.Hide(PlayerId.PlayerOne);
                        self.Show(PlayerId.PlayerOne);
                    }
                    return;

                }
            }
            orig(self);
        }


        // Added a check to ensure that the Tutorial object is the only text box that is altered by Boss Rush
        // (without this, Elder Kettle's text would also change because the tutorial and him share the same class)
        protected virtual void Show(On.AbstractLevelInteractiveEntity.orig_Show orig, AbstractLevelInteractiveEntity self, PlayerId playerId)
        {
            self.state = AbstractLevelInteractiveEntity.State.Ready;
            if (self.gameObject.name != "Tutorial")
            {
                self.dialogueProperties.text = string.Empty;
                self.dialogue = LevelUIInteractionDialogue.Create(self.dialogueProperties, PlayerManager.GetPlayer(playerId).input, self.dialogueOffset, 0f, LevelUIInteractionDialogue.TailPosition.Bottom, self.hasTarget);
            }
            else
            {
                self.dialogue = LevelUIInteractionDialogue.Create(self.dialogueProperties, PlayerManager.GetPlayer(playerId).input, self.dialogueOffset, 0f, LevelUIInteractionDialogue.TailPosition.Bottom, false);
            }
        }

        protected void Awake(On.AbstractLevelInteractiveEntity.orig_Awake orig, AbstractLevelInteractiveEntity self)
        {
            BossRushManager.refreshTutorialDialogue = true;
            orig(self);
        }


    }
}
