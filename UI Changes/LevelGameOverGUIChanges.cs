using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelluvaRush
{
    public class LevelGameOverGUIChanges
    {
        public void Init()
        {
            On.LevelGameOverGUI.Retry += Retry;
            On.LevelGameOverGUI.ExitToMap += ExitToMap;
            On.LevelGameOverGUI.Update += Update;
        }
        private void Retry(On.LevelGameOverGUI.orig_Retry orig, LevelGameOverGUI self)
        {
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

        private void ExitToMap(On.LevelGameOverGUI.orig_ExitToMap orig, LevelGameOverGUI self)
        {
            SceneLoader.LoadLastMap();
            if (!BossRushManager.inBossRush)
            {
                return;
            }
            BossRushManager.endBossRush();
        }

        private void Update(On.LevelGameOverGUI.orig_Update orig, LevelGameOverGUI self)
        {
            if (self.state != LevelGameOverGUI.State.Ready)
            {
                return;
            }
            self.menuItems[0].text = ((!BossRushManager.inBossRush) ? "RETRY" : "RETRY FROM BEGINNING");
            if (self.selection == 2 && Level.Current != null && Level.Current.CurrentLevel == Levels.Airplane && (self.getButtonDown(CupheadButton.Accept) || self.getButtonDown(CupheadButton.MenuLeft) || self.getButtonDown(CupheadButton.MenuRight)))
            {
                AudioManager.Play("level_menu_card_down");
                self.toggleRotateControls();
                return;
            }
            int num = 0;
            if (self.getButtonDown(CupheadButton.Accept))
            {
                self.Select();
                AudioManager.Play("level_menu_select");
                self.state = LevelGameOverGUI.State.Exiting;
            }
            if (!Level.IsTowerOfPower && self.getButtonDown(CupheadButton.EquipMenu))
            {
                self.ChangeEquipment();
            }
            if (self.getButtonDown(CupheadButton.MenuDown))
            {
                AudioManager.Play("level_menu_move");
                num++;
            }
            if (self.getButtonDown(CupheadButton.MenuUp))
            {
                AudioManager.Play("level_menu_move");
                num--;
            }
            self.selection += num;
            self.selection = Mathf.Clamp(self.selection, 0, self.menuItems.Length - 1);
            if (!self.menuItems[self.selection].gameObject.activeSelf)
            {
                self.selection -= num;
                self.selection = Mathf.Clamp(self.selection, 0, self.menuItems.Length - 1);
            }
            self.UpdateSelection();
        }
    }
}
