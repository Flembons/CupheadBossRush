using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class OptionsGUIChanges
    {
        public void Init()
        {
            On.OptionsGUI.HideMainOptionMenu += HideMainOptionMenu;
            On.OptionsGUI.ToVisual += ToVisual;
            On.OptionsGUI.ToAudio += ToAudio;
            On.OptionsGUI.ToLanguage += ToLanguage;
            On.OptionsGUI.ToControls += ToControls;
            On.OptionsGUI.ToggleSubMenu += ToggleSubMenu;
        }

        public void HideMainOptionMenu(On.OptionsGUI.orig_HideMainOptionMenu orig, OptionsGUI self)
        {
            if (BossRushManager.bossRushOptions)
            {
                BossRushManager.bossRushOptions = false;
                self.mainObjectButtons[0].text.text = "AUDIO";
                self.mainObjectButtons[1].text.text = "VISUAL";
                self.mainObjectButtons[2].text.text = "CONTROLS";
                self.mainObjectButtons[3].text.text = "LANGUAGE";
            }
            SettingsData.Save();
            if (PlatformHelper.IsConsole)
            {
                SettingsData.SaveToCloud();
            }
            if (self.savePlayerData)
            {
                PlayerData.SaveCurrentFile();
            }
            self.savePlayerData = false;
            self.verticalSelection = 0;
            self.canvasGroup.alpha = 0f;
            self.canvasGroup.interactable = false;
            self.canvasGroup.blocksRaycasts = false;
            self.inputEnabled = false;
            self.optionMenuOpen = false;
            self.justClosed = true;
        }

        private void ToVisual(On.OptionsGUI.orig_ToVisual orig, OptionsGUI self)
        {
            if (!BossRushManager.bossRushOptions)
            {
                self.state = OptionsGUI.State.Visual;
                self.CenterVisual();
                if (!self.isConsole)
                {
                    self.ChangeStateCustomLayoutScripts();
                }
                self.ToggleSubMenu(self.state);
                return;
            }
            Level.Mode bossRushDiff = BossRushManager.bossRushDiff;
            if (bossRushDiff != Level.Mode.Easy)
            {
                if (bossRushDiff != Level.Mode.Normal)
                {
                    BossRushManager.bossRushDiff = Level.Mode.Easy;
                    self.mainObjectButtons[1].text.text = "DIFFICULTY: SIMPLE";
                    if (BossRushManager.breakForTutorial)
                    {
                        self.mainObjectButtons[3].text.text = "REST AFTER 3 FIGHTS: ON";
                    }
                    else
                    {
                        self.mainObjectButtons[3].text.text = "REST AFTER 3 FIGHTS: OFF";
                    }
                    if (SceneLoader.CurrentLevel == Levels.House)
                    {
                        BossRushManager.refreshTutorialDialogue = true;
                        return;
                    }
                }
                else
                {
                    BossRushManager.bossRushDiff = Level.Mode.Hard;
                    self.mainObjectButtons[1].text.text = "DIFFICULTY: EXPERT";
                    if (BossRushManager.breakForTutorial)
                    {
                        self.mainObjectButtons[3].text.text = "REST AFTER 5 FIGHTS: ON";
                    }
                    else
                    {
                        self.mainObjectButtons[3].text.text = "REST AFTER 5 FIGHTS: OFF";
                    }
                    if (SceneLoader.CurrentLevel == Levels.House)
                    {
                        BossRushManager.refreshTutorialDialogue = true;
                        return;
                    }
                }
            }
            else
            {
                BossRushManager.bossRushDiff = Level.Mode.Normal;
                self.mainObjectButtons[1].text.text = "DIFFICULTY: REGULAR";
                if (BossRushManager.breakForTutorial)
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 4 FIGHTS: ON";
                }
                else
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 4 FIGHTS: OFF";
                }
                if (SceneLoader.CurrentLevel == Levels.House)
                {
                    BossRushManager.refreshTutorialDialogue = true;
                    return;
                }
            }
        }

        private void ToAudio(On.OptionsGUI.orig_ToAudio orig, OptionsGUI self)
        {
            if (!BossRushManager.bossRushOptions)
            {
                self.state = OptionsGUI.State.Audio;
                self.CenterAudio();
                self.ToggleSubMenu(self.state);
                return;
            }
            if (BossRushManager.bossRushActive)
            {
                BossRushManager.bossRushActive = false;
                self.mainObjectButtons[0].text.text = "BOSS RUSH: OFF";
                if (SceneLoader.CurrentLevel != Levels.House)
                {
                    return;
                }
                BossRushManager.refreshTutorialDialogue = true;
                return;
            }
            else
            {
                BossRushManager.bossRushActive = true;
                self.mainObjectButtons[0].text.text = "BOSS RUSH: ON";
                if (SceneLoader.CurrentLevel != Levels.House)
                {
                    return;
                }
                BossRushManager.refreshTutorialDialogue = true;
                return;
            }
        }

        private void ToLanguage(On.OptionsGUI.orig_ToLanguage orig, OptionsGUI self)
        {
            if (!BossRushManager.bossRushOptions)
            {
                self.state = OptionsGUI.State.Language;
                self.ToggleSubMenu(self.state);
                return;
            }
            if (BossRushManager.breakForTutorial)
            {
                BossRushManager.breakForTutorial = false;
                if (BossRushManager.bossRushDiff == Level.Mode.Easy)
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 3 FIGHTS: OFF";
                    return;
                }
                if (BossRushManager.bossRushDiff == Level.Mode.Normal)
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 4 FIGHTS: OFF";
                    return;
                }
                if (BossRushManager.bossRushDiff == Level.Mode.Hard)
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 5 FIGHTS: OFF";
                }
                return;
            }
            else
            {
                BossRushManager.breakForTutorial = true;
                if (BossRushManager.bossRushDiff == Level.Mode.Easy)
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 3 FIGHTS: ON";
                    return;
                }
                if (BossRushManager.bossRushDiff == Level.Mode.Normal)
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 4 FIGHTS: ON";
                    return;
                }
                if (BossRushManager.bossRushDiff == Level.Mode.Hard)
                {
                    self.mainObjectButtons[3].text.text = "REST AFTER 5 FIGHTS: ON";
                }
                return;
            }
        }

        private void ToControls(On.OptionsGUI.orig_ToControls orig, OptionsGUI self)
        {
            if (!BossRushManager.bossRushOptions)
            {
                self.state = OptionsGUI.State.Controls;
                self.ToggleSubMenu(self.state);
                return;
            }
            if (BossRushManager.bossRushHealBetweenFights)
            {
                BossRushManager.bossRushHealBetweenFights = false;
                self.mainObjectButtons[2].text.text = "HEAL BETWEEN FIGHTS: OFF";
                return;
            }
            BossRushManager.bossRushHealBetweenFights = true;
            self.mainObjectButtons[2].text.text = "HEAL BETWEEN FIGHTS: ON";
        }

        private void ToggleSubMenu(On.OptionsGUI.orig_ToggleSubMenu orig, OptionsGUI self, OptionsGUI.State state)
        {
            self.currentItems.Clear();
            switch (state)
            {
                case OptionsGUI.State.MainOptions:
                    self.mainObject.SetActive(true);
                    self.visualObject.SetActive(false);
                    self.audioObject.SetActive(false);
                    self.languageObject.SetActive(false);
                    self.bigCard.SetActive(false);
                    self.bigNoise.SetActive(false);
                    if (BossRushManager.bossRushOptions)
                    {
                        self.mainObjectButtons[0].text.text = ((!BossRushManager.bossRushActive) ? "BOSS RUSH: OFF" : "BOSS RUSH: ON");
                        Level.Mode bossRushDiff = BossRushManager.bossRushDiff;
                        if (bossRushDiff != Level.Mode.Easy)
                        {
                            if (bossRushDiff != Level.Mode.Normal)
                            {
                                self.mainObjectButtons[1].text.text = "DIFFICULTY: EXPERT";
                                if (BossRushManager.breakForTutorial)
                                {
                                    self.mainObjectButtons[3].text.text = "REST AFTER 5 FIGHTS: ON";
                                }
                                else
                                {
                                    self.mainObjectButtons[3].text.text = "REST AFTER 5 FIGHTS: OFF";
                                }
                            }
                            else
                            {
                                self.mainObjectButtons[1].text.text = "DIFFICULTY: REGULAR";
                                if (BossRushManager.breakForTutorial)
                                {
                                    self.mainObjectButtons[3].text.text = "REST AFTER 4 FIGHTS: ON";
                                }
                                else
                                {
                                    self.mainObjectButtons[3].text.text = "REST AFTER 4 FIGHTS: OFF";
                                }
                            }
                        }
                        else
                        {
                            self.mainObjectButtons[1].text.text = "DIFFICULTY: SIMPLE";
                            if (BossRushManager.breakForTutorial)
                            {
                                self.mainObjectButtons[3].text.text = "REST AFTER 3 FIGHTS: ON";
                            }
                            else
                            {
                                self.mainObjectButtons[3].text.text = "REST AFTER 3 FIGHTS: OFF";
                            }
                        }
                        self.mainObjectButtons[2].text.text = ((!BossRushManager.bossRushHealBetweenFights) ? "HEAL BETWEEN FIGHTS: OFF" : "HEAL BETWEEN FIGHTS: ON");
                    }
                    self.currentItems.AddRange(self.mainObjectButtons);
                    break;
                case OptionsGUI.State.Visual:
                    self.mainObject.SetActive(false);
                    self.visualObject.SetActive(true);
                    self.audioObject.SetActive(false);
                    self.bigCard.SetActive(true);
                    self.bigNoise.SetActive(true);
                    self.currentItems.AddRange(self.visualObjectButtons);
                    break;
                case OptionsGUI.State.Audio:
                    self.mainObject.SetActive(false);
                    self.visualObject.SetActive(false);
                    self.audioObject.SetActive(true);
                    self.languageObject.SetActive(false);
                    self.bigCard.SetActive(true);
                    self.bigNoise.SetActive(true);
                    self.currentItems.AddRange(self.audioObjectButtons);
                    break;
                case OptionsGUI.State.Controls:
                    self.mainObject.SetActive(false);
                    self.visualObject.SetActive(false);
                    self.audioObject.SetActive(false);
                    self.languageObject.SetActive(false);
                    self.ShowControlMapper();
                    break;
                case OptionsGUI.State.Language:
                    self.languageObjectButtons[0].updateSelection((int)Localization.language);
                    self.mainObject.SetActive(false);
                    self.audioObject.SetActive(false);
                    self.languageObject.SetActive(true);
                    self.bigCard.SetActive(false);
                    self.bigNoise.SetActive(false);
                    self.currentItems.AddRange(self.languageObjectButtons);
                    break;
            }
            if (state != OptionsGUI.State.Controls)
            {
                self.verticalSelection = 0;
                self.UpdateVerticalSelection();
            }
        }
    }
}
