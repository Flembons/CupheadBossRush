using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;

namespace HelluvaRush
{
    /*
     * A new counter was added to the HUD to keep track of how many Bosses are left in the Boss Rush
     * When in a boss rush, there will now be a "BOSSES DEFEATED: X" counter above the hud in the bottom left
     */
    public class LevelHUDChanges
    {
        public void Init()
        {
            On.LevelHUD.LevelInit += LevelInit;
        }

        public void LevelInit(On.LevelHUD.orig_LevelInit orig, LevelHUD self)
        {
            AbstractPlayerController player = PlayerManager.GetPlayer(PlayerId.PlayerOne);
            self.levelHudTemplate = UnityEngine.Object.Instantiate<LevelHUDPlayer>(self.cuphead);
            self.levelHudTemplate.gameObject.SetActive(false);
            if (PlayerManager.Multiplayer)
            {
                AbstractPlayerController player2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo);
                self.mugman = UnityEngine.Object.Instantiate<LevelHUDPlayer>(self.levelHudTemplate);
                self.mugman.gameObject.SetActive(true);
                self.mugman.transform.SetParent(self.cuphead.transform.parent, false);
                self.mugman.Init(player2, false);
            }
            self.cuphead.Init(player, false);

            // Create a new TMP text object for the boss counter and place it in the bottom left of the screen
            BossRushManager.bossCounterObj = new GameObject("Boss Counter");
            if (!BossRushManager.inBossRush)
            {
                BossRushManager.bossCounterObj.SetActive(false);
            }
            BossRushManager.bossCounterObj.transform.SetParent(self.cuphead.transform);
            if (BossRushManager.bossCounterObj.GetComponent<TextMeshProUGUI>() == null)
            {
                BossRushManager.bossCounter = BossRushManager.bossCounterObj.AddComponent<TextMeshProUGUI>();
            }
            BossRushManager.bossCounter.text = "BOSSES LEFT: " + (BossRushManager.bossLevels.Length - BossRushManager.bossesDefeated);
            BossRushManager.bossCounter.font = Localization.Instance.fonts[(int)Localization.language][41].fontAsset;
            BossRushManager.bossCounter.enableWordWrapping = false;
            BossRushManager.bossCounter.transform.localPosition = new Vector3(44f, 36f, 0f);
            BossRushManager.bossCounter.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
        }
    }
}
