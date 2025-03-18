using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class TutorialChanges
    {
        public void Init()
        {
            On.ShmupTutorialExitSign.go_cr += go_cr;
            On.AbstractLevelInteractiveEntity.FixedUpdate += FixedUpdate;
            On.AbstractLevelInteractiveEntity.Show += Show;
            On.AbstractLevelInteractiveEntity.Awake += Awake;
        }

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
