using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;

namespace HelluvaRush
{
    public class LevelPauseGUIChanges
    {
        /*
         * This class adds a new option to the Pause Menu when on the overworld map screen.
         * "Boss Rush Options" will now be an option in the pause menu when the player is not in a fight.
         * 
         * Additionally, the pause menu was changed to restart or end the boss rush when those respective options
         * were chosen while the player was paused in a fight
         */
        public void Init()
        {
            On.LevelPauseGUI.Init_bool_OptionsGUI_AchievementsGUI_RestartTowerConfirmGUI += Init;
            On.LevelPauseGUI.Update += Update;
            On.LevelPauseGUI.Select += Select;
            On.LevelPauseGUI.UpdateSelection += UpdateSelection;
            On.LevelPauseGUI.Restart += Restart;
            On.LevelPauseGUI.Exit += Exit;
        }

        // Sets menuItems[2] to be active if we're not in a fight
        // This menuItem is normally not active unless we're in the Dog Airplane fight
        public void Init(On.LevelPauseGUI.orig_Init_bool_OptionsGUI_AchievementsGUI_RestartTowerConfirmGUI orig, LevelPauseGUI self, bool checkIfDead, OptionsGUI options, AchievementsGUI achievements, RestartTowerConfirmGUI restartTowerConfirm)
        {
            self.input = new CupheadInput.AnyPlayerInput(checkIfDead);
            self.options = options;
            self.achievements = achievements;
            self.restartTowerConfirm = restartTowerConfirm;
            if (PlatformHelper.IsConsole && self.menuItems.Length > 7)
            {
                self.menuItems[7].gameObject.SetActive(false);
            }
            if (!SceneLoader.SceneName.StartsWith("scene_level") || SceneLoader.CurrentLevel == Levels.House)
            {
                self.menuItems[2].gameObject.SetActive(true);
                self.updateRotateControlsToggleVisualValue();
            }
            else if (!PlatformHelper.ShowAchievements && self.menuItems.Length > 2)
            {
                self.menuItems[2].gameObject.SetActive(false);
            }
            if (Level.IsTowerOfPower)
            {
                self.ReplaceRestartWRestartTowerOfPower();
            }
            options.Init(checkIfDead);
            if (achievements != null)
            {
                achievements.Init(checkIfDead);
            }
            if (restartTowerConfirm != null)
            {
                restartTowerConfirm.Init(checkIfDead);
            }
        }

        // Adds a new "Boss Rush Options" option to the pause menu by hijacking menuItems[2] (which is normally Achievements)
        public void Update(On.LevelPauseGUI.orig_Update orig, LevelPauseGUI self)
        {
            if (!BossRushManager.inBossRush && self.state != AbstractPauseGUI.State.Animating)
            {
                self.menuItems[2].text = "BOSS RUSH OPTIONS";
                self.menuItems[1].text = "RETRY";
            }
            if (BossRushManager.inBossRush)
            {
                self.menuItems[1].text = "RETRY FROM BEGINNING";
            }
            orig(self);
        }

        // Added behavior for when Boss Rush Options is selected in the menu
        private void Select(On.LevelPauseGUI.orig_Select orig, LevelPauseGUI self)
        {
            switch (self.selection)
            {
                case 0:
                    self.Unpause();
                    return;
                case 1:
                    self.Restart();
                    return;
                case 2:
                    BossRushManager.bossRushOptions = true;
                    self.Options();
                    return;
                case 3:
                    self.Options();
                    return;
                case 4:
                    self.Player2Leave();
                    return;
                case 5:
                    self.Exit();
                    return;
                case 6:
                    self.ExitToTitle();
                    return;
                case 7:
                    self.ExitToDesktop();
                    return;
                default:
                    return;
            }
        }

        private void UpdateSelection(On.LevelPauseGUI.orig_UpdateSelection orig, LevelPauseGUI self)
        {
            self._selectionTimer = 0f;
            for (int i = 0; i < self.menuItems.Length; i++)
            {
                Text text = self.menuItems[i];
                if (i != 2 || !BossRushManager.inBossRush)
                {
                    if (i == self.selection)
                    {
                        text.color = LevelPauseGUI.COLOR_SELECTED;
                    }
                    else
                    {
                        text.color = LevelPauseGUI.COLOR_INACTIVE;
                    }
                }
            }
        }

        // When hitting Retry in the pause menu, the boss rush will restart if it is currently active
        private void Restart(On.LevelPauseGUI.orig_Restart orig, LevelPauseGUI self)
        {
            if (Level.IsTowerOfPower)
            {
                self.RestartTowerConfirm();
                return;
            }
            self.OnUnpauseSound();
            self.state = AbstractPauseGUI.State.Animating;
            PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, false);
            PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, false);
            Dialoguer.EndDialogue();
            if (Level.IsDicePalaceMain || Level.IsDicePalace)
            {
                DicePalaceMainLevelGameInfo.CleanUpRetry();
            }
            if (BossRushManager.inBossRush)
            {
                BossRushManager.startBossRush(BossRushManager.isBossOrderRandom);
                return;
            }
            SceneLoader.ReloadLevel();
        }

        // End the boss rush if the player exits to map during a fight
        private void Exit(On.LevelPauseGUI.orig_Exit orig, LevelPauseGUI self)
        {
            self.state = AbstractPauseGUI.State.Animating;
            PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerOne, false);
            PlayerManager.SetPlayerCanSwitch(PlayerId.PlayerTwo, false);
            Dialoguer.EndDialogue();
            if (Level.IsDicePalaceMain || Level.IsDicePalace)
            {
                DicePalaceMainLevelGameInfo.CleanUpRetry();
            }
            if (BossRushManager.inBossRush)
            {
                BossRushManager.endBossRush();
            }
            SceneLoader.LoadLastMap();
        }
    }
}
