using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rewired;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace HelluvaRush
{
    /*
     * This class changes the text when selecting a boss node on the map. Normally, the display will have options for 
     * Easy, Normal, and Hard difficulties. When Boss Rush is active, there will only be one option that reflects the 
     * chosen Boss Rush difficulty.
     */
    public class MapDifficultySelectStartUIChanges
    {
        public void Init()
        {
            On.MapDifficultySelectStartUI.Awake += Awake;
            On.MapDifficultySelectStartUI.SetDifficultyAvailability += SetDifficultyAvailability;
            On.MapDifficultySelectStartUI.In += In;
            On.MapDifficultySelectStartUI.UpdateCursor += UpdateCursor;
        }

        protected void Awake(On.MapDifficultySelectStartUI.orig_Awake orig, MapDifficultySelectStartUI self)
        {
            self.useGUILayout = false;
            self.timeLayer = CupheadTime.Layer.UI;
            self.ignoreGlobalTime = true;
            self.canvasGroup = self.GetComponent<CanvasGroup>();
            self.SetAlpha(0f);
            MapDifficultySelectStartUI.Current = self;
            switch (Level.CurrentMode)
            {
                case Level.Mode.Easy:
                    self.index = 0;
                    break;
                case Level.Mode.Normal:
                    self.index = 1;
                    break;
                case Level.Mode.Hard:
                    self.index = 2;
                    break;
            }
            self.options = new Level.Mode[]
            {
            Level.Mode.Easy,
            Level.Mode.Normal,
            Level.Mode.Hard
            };
            self.difficulyTexts = new TMP_Text[3];
            self.difficulyTexts[0] = self.easy.GetComponent<TMP_Text>();
            self.difficulyTexts[1] = self.normal.GetComponent<TMP_Text>();
            self.difficulyTexts[2] = self.hard.GetComponent<TMP_Text>();
            self.SetDifficultyAvailability();
            if (self.bossImage != null && self.bossImage.textComponent != null)
            {
                self.initialMaxFontSize = self.bossImage.textComponent.resizeTextMaxSize;
            }
            self.initialinImagePosX = self.inAnimated.rectTransform.offsetMin;
            self.initialinImagePosY = self.inAnimated.rectTransform.offsetMax;
            self.initialinDifficultyPos = self.difficultyImage.rectTransform.anchoredPosition;
            self.initialDifficultyPos = self.difficultySelectionText.rectTransform.anchoredPosition;
            self.initialBossNamePos = self.bossNameImage.rectTransform.anchoredPosition;
        }

        private void SetDifficultyAvailability(On.MapDifficultySelectStartUI.orig_SetDifficultyAvailability orig, MapDifficultySelectStartUI self)
        {
            if (BossRushManager.bossRushActive)
            {
                self.options = new Level.Mode[]
                {
                    Level.Mode.Normal
                };
                self.hard.gameObject.SetActive(false);
                self.easy.gameObject.SetActive(false);
                self.normalSeparator.gameObject.SetActive(false);
                self.hardSeparator.gameObject.SetActive(false);
                self.index = 0;
                if (BossRushManager.bossRushDiff == Level.Mode.Easy)
                {
                    self.difficulyTexts[1].text = "SIMPLE BOSS RUSH";
                    return;
                }
                if (BossRushManager.bossRushDiff == Level.Mode.Normal)
                {
                    self.difficulyTexts[1].text = "REGULAR BOSS RUSH";
                    return;
                }
                self.difficulyTexts[1].text = "EXPERT BOSS RUSH";
                return;
            }
            else
            {
                self.difficulyTexts[1].text = "REGULAR";
                self.hard.gameObject.SetActive(true);
                self.easy.gameObject.SetActive(true);
                self.normalSeparator.gameObject.SetActive(true);
                self.hardSeparator.gameObject.SetActive(true);
                self.options = new Level.Mode[]
                {
                    Level.Mode.Easy,
                    Level.Mode.Normal,
                    Level.Mode.Hard
                };
                switch (Level.CurrentMode)
                {
                    case Level.Mode.Easy:
                        self.index = 0;
                        break;
                    case Level.Mode.Normal:
                        self.index = 1;
                        break;
                    case Level.Mode.Hard:
                        self.index = 2;
                        break;
                }
                if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_4)
                {
                    if (!PlayerData.Data.IsHardModeAvailable)
                    {
                        self.options = new Level.Mode[]
                        {
                        Level.Mode.Normal
                        };
                        self.hard.gameObject.SetActive(false);
                        self.hardSeparator.gameObject.SetActive(false);
                    }
                    else
                    {
                        self.options = new Level.Mode[]
                        {
                        Level.Mode.Normal,
                        Level.Mode.Hard
                        };
                    }
                    self.index = Mathf.Max(0, self.index - 1);
                    self.easy.gameObject.SetActive(false);
                    self.normalSeparator.gameObject.SetActive(false);
                    return;
                }
                if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_DLC)
                {
                    if (self.level == "Saltbaker")
                    {
                        if (!PlayerData.Data.IsHardModeAvailableDLC)
                        {
                            self.options = new Level.Mode[]
                            {
                            Level.Mode.Normal
                            };
                        }
                        else
                        {
                            self.options = new Level.Mode[]
                            {
                            Level.Mode.Normal,
                            Level.Mode.Hard
                            };
                        }
                    }
                    else if (!PlayerData.Data.IsHardModeAvailableDLC)
                    {
                        self.options = new Level.Mode[]
                        {
                        Level.Mode.Easy,
                        Level.Mode.Normal
                        };
                    }
                    else
                    {
                        self.options = new Level.Mode[]
                        {
                        Level.Mode.Easy,
                        Level.Mode.Normal,
                        Level.Mode.Hard
                        };
                    }
                    self.easy.gameObject.SetActive(self.level != "Saltbaker");
                    self.normalSeparator.gameObject.SetActive(self.level != "Saltbaker");
                    self.hard.gameObject.SetActive(PlayerData.Data.IsHardModeAvailableDLC);
                    self.hardSeparator.gameObject.SetActive(PlayerData.Data.IsHardModeAvailableDLC);
                    return;
                }
                if (!PlayerData.Data.IsHardModeAvailable)
                {
                    self.options = new Level.Mode[]
                    {
                    Level.Mode.Easy,
                    Level.Mode.Normal
                    };
                }
                self.hard.gameObject.SetActive(PlayerData.Data.IsHardModeAvailable);
                self.hardSeparator.gameObject.SetActive(PlayerData.Data.IsHardModeAvailable);
                return;
            }
        }

        public void In(On.MapDifficultySelectStartUI.orig_In orig, MapDifficultySelectStartUI self, MapPlayerController playerController)
        {
            self.player = playerController.input.actions;
            self.StartCoroutine(self.fade_cr(1f, AbstractMapSceneStartUI.State.Active));
            self.SetDifficultyAvailability();
            if (Level.CurrentMode == Level.Mode.Easy && PlayerData.Data.CurrentMap == Scenes.scene_map_world_4)
            {
                Level.SetCurrentMode(Level.Mode.Normal);
                switch (Level.CurrentMode)
                {
                    case Level.Mode.Easy:
                        self.index = 0;
                        break;
                    case Level.Mode.Normal:
                        self.index = 1;
                        break;
                    case Level.Mode.Hard:
                        self.index = 2;
                        break;
                }
            }
            if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_DLC)
            {
                self.SetDifficultyAvailability();
                if (self.level == "Saltbaker" && Level.CurrentMode == Level.Mode.Easy)
                {
                    Level.SetCurrentMode(Level.Mode.Normal);
                }
            }
            if (self.animator != null)
            {
                self.animator.SetTrigger("ZoomIn");
                AudioManager.Play("world_map_level_menu_open");
            }
            self.InWordSetup();
            self.difficultyImage.enabled = (Localization.language == Localization.Languages.Japanese);
            self.difficultyImage.rectTransform.anchoredPosition = self.initialinDifficultyPos;
            for (int i = 0; i < self.separatorsAnimated.Length; i++)
            {
                self.separatorsAnimated[i].sprite = self.separatorsSprites[UnityEngine.Random.Range(0, self.separatorsSprites.Length)];
            }
            bool flag = Localization.language == Localization.Languages.Korean || Localization.language == Localization.Languages.SimplifiedChinese || Localization.language == Localization.Languages.Japanese;
            self.bossTitleImage.enabled = (Localization.language == Localization.Languages.English || flag || PlayerData.Data.CurrentMap == Scenes.scene_map_world_DLC);
            self.glowScript.StopGlow();
            self.glowScript.DisableTMPText();
            self.glowScript.DisableImages();
            if (Localization.language == Localization.Languages.SimplifiedChinese)
            {
                self.difficultySelectionText.rectTransform.anchoredPosition = new Vector2(self.difficultySelectionText.rectTransform.anchoredPosition.x, -70f);
            }
            else
            {
                self.difficultySelectionText.rectTransform.anchoredPosition = self.initialDifficultyPos;
            }
            TranslationElement translationElement = Localization.Find(self.level + "Selection");
            if (self.bossImage != null && translationElement != null)
            {
                self.bossImage.ApplyTranslation(translationElement, null);
                if (self.bossImage.textComponent != null)
                {
                    if (Localization.language == Localization.Languages.Korean)
                    {
                        self.bossImage.textComponent.resizeTextMaxSize = 100;
                    }
                    else
                    {
                        self.bossImage.textComponent.resizeTextMaxSize = self.initialMaxFontSize;
                    }
                }
                if (flag)
                {
                    self.SetupAsianBossCard(translationElement, self.bossTitleImage);
                }
                else
                {
                    self.bossImage.transform.localScale = Vector3.one;
                    self.bossImage.transform.localPosition = Vector3.zero;
                    self.bossTitleImage.rectTransform.offsetMax = new Vector2(self.bossTitleImage.rectTransform.offsetMax.x, 0.5f);
                    self.bossTitleImage.rectTransform.offsetMin = new Vector2(self.bossTitleImage.rectTransform.offsetMin.x, 0.5f);
                    self.inAnimated.rectTransform.offsetMin = self.initialinImagePosX;
                    self.inAnimated.rectTransform.offsetMax = self.initialinImagePosY;
                    self.inText.fontStyle = FontStyle.Italic;
                }
            }
            TranslationElement translationElement2 = Localization.Find(self.level + "WorldMap");
            if (translationElement2 != null)
            {
                self.bossName.ApplyTranslation(translationElement2, null);
                if (self.bossName.textComponent != null && self.bossName.textComponent.enabled)
                {
                    self.bossName.textComponent.font = FontLoader.GetFont(FontLoader.FontType.CupheadHenriette_A_merged);
                }
                self.bossNameImage.transform.localScale = Vector3.one;
                self.bossNameImage.rectTransform.anchoredPosition = self.initialBossNamePos;
                if (flag)
                {
                    self.bossNameImage.material = self.bossCardWhiteMaterial;
                    if (Localization.language == Localization.Languages.Korean || Localization.language == Localization.Languages.Japanese)
                    {
                        self.bossNameImage.transform.localScale = new Vector3(1.2f, 1.2f, 1f);
                        if (Localization.language == Localization.Languages.Japanese)
                        {
                            self.bossNameImage.rectTransform.anchoredPosition = new Vector2(0f, 214.2f);
                        }
                    }
                }
                self.bossName.gameObject.SetActive(Localization.language != Localization.Languages.English && !flag && PlayerData.Data.CurrentMap != Scenes.scene_map_world_DLC);
            }
            TranslationElement translationElement3 = Localization.Find(self.level + "Glow");
            if (Localization.language != Localization.Languages.English)
            {
                if (translationElement3 != null && flag)
                {
                    self.bossGlow.ApplyTranslation(translationElement3, null);
                }
                else
                {
                    self.glowScript.InitTMPText(new MaskableGraphic[]
                    {
                    self.bossImage.textMeshProComponent,
                    self.bossName.textComponent
                    });
                    self.glowScript.BeginGlow();
                }
            }
            self.bossGlow.gameObject.SetActive(flag && PlayerData.Data.CurrentMap != Scenes.scene_map_world_DLC);
            for (int j = 0; j < self.difficulyTexts.Length; j++)
            {
                self.difficulyTexts[j].color = self.unselectedColor;
            }
            self.difficulyTexts[(int)Level.CurrentMode].color = self.selectedColor;
            if (!BossRushManager.bossRushActive)
            {
                return;
            }
            Level.SetCurrentMode(BossRushManager.bossRushDiff);
            self.difficulyTexts[1].color = self.selectedColor;
        }

        private void UpdateCursor(On.MapDifficultySelectStartUI.orig_UpdateCursor orig, MapDifficultySelectStartUI self)
        {
            Vector3 position = self.cursor.transform.position;
            position.y = self.normal.position.y;
            Level.Mode mode = Level.CurrentMode;
            if (PlayerData.Data.CurrentMap == Scenes.scene_map_world_4 && mode == Level.Mode.Easy)
            {
                mode = Level.Mode.Normal;
            }
            if (BossRushManager.bossRushActive)
            {
                mode = Level.Mode.Normal;
            }
            switch (mode)
            {
                case Level.Mode.Easy:
                    position.x = self.easy.position.x;
                    self.cursor.sizeDelta = new Vector2(self.easy.sizeDelta.x + 30f, self.easy.sizeDelta.y + 20f);
                    break;
                case Level.Mode.Normal:
                    position.x = self.normal.position.x;
                    self.cursor.sizeDelta = new Vector2(self.normal.sizeDelta.x + 30f, self.normal.sizeDelta.y + 20f);
                    break;
                case Level.Mode.Hard:
                    position.x = self.hard.position.x;
                    self.cursor.sizeDelta = new Vector2(self.hard.sizeDelta.x + 30f, self.hard.sizeDelta.y + 20f);
                    break;
            }
            self.cursor.transform.position = position;
        }
    }
}
