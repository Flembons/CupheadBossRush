using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class PlayerStatsManagerChanges
    {
        public void Init()
        {
            On.PlayerStatsManager.CalculateHealthMax += CalculateHealthMax;
            On.PlayerStatsManager.OnParry += OnParry;
            On.PlayerStatsManager.HealerCharm += HealerCharm;
        }

        private void CalculateHealthMax(On.PlayerStatsManager.orig_CalculateHealthMax orig_CalculateHealthMax, PlayerStatsManager self)
        {
            self.HealthMax = 3;
            if (self.Loadout.charm == Charm.charm_health_up_1 && !Level.IsChessBoss)
            {
                self.HealthMax += WeaponProperties.CharmHealthUpOne.healthIncrease;
            }
            else if (self.Loadout.charm == Charm.charm_health_up_2 && !Level.IsChessBoss)
            {
                self.HealthMax += WeaponProperties.CharmHealthUpTwo.healthIncrease;
            }
            else if (self.Loadout.charm == Charm.charm_healer && !Level.IsChessBoss)
            {
                self.HealthMax += self.HealerHP;
            }
            else if (self.Loadout.charm == Charm.charm_curse && self.CurseCharmLevel >= 0 && !Level.IsChessBoss)
            {
                self.HealthMax += self.HealerHP;
                self.HealthMax += CharmCurse.GetHealthModifier(self.CurseCharmLevel);
            }
            else if (self.isChalice)
            {
                int healthMax = self.HealthMax;
                self.HealthMax = healthMax + 1;
            }
            if (self.DjimmiInUse())
            {
                self.HealthMax *= 2;
            }
            if (Level.IsInBossesHub)
            {
                PlayersStatsBossesHub playerStats = Level.GetPlayerStats(self.basePlayer.id);
                if (playerStats != null)
                {
                    self.HealthMax += playerStats.BonusHP;
                }
            }
            if (BossRushManager.inBossRush)
            {
                if (BossRushManager.bossRushJustStarted)
                {
                    if (!BossRushManager.bossRushMultiplayer)
                    {
                        BossRushManager.bossRushJustStarted = false;
                    }
                    else
                    {
                        BossRushManager.bossRushMultiplayer = false;
                    }
                }
                else
                {
                    self.HealthMax = 9;
                }
            }
            if (self.HealthMax > 9)
            {
                self.HealthMax = 9;
            }
        }

        public void OnParry(On.PlayerStatsManager.orig_OnParry orig, PlayerStatsManager self, float multiplier = 1f, bool countParryTowardsScore = true)
        {
            if (((BossRushManager.inBossRush && (SceneLoader.CurrentLevel == Levels.ChaliceTutorial || SceneLoader.CurrentLevel == Levels.ShmupTutorial)) || self.Loadout.charm == Charm.charm_healer || (self.Loadout.charm == Charm.charm_curse && self.CurseCharmLevel >= 0)) && !Level.IsChessBoss)
            {
                if (self.HealerHPReceived < 3)
                {
                    self.HealerCharm();
                }
                else
                {
                    self.SuperChangedFromParry(multiplier);
                }
            }
            else
            {
                self.SuperChangedFromParry(multiplier);
            }
            if (countParryTowardsScore && !Level.Current.Ending)
            {
                Level.ScoringData.numParries++;
            }
            OnlineManager.Instance.Interface.IncrementStat(self.basePlayer.id, "Parries", 1);
            if (Level.Current.CurrentLevel != Levels.Tutorial && Level.Current.CurrentLevel != Levels.ShmupTutorial && (Level.Current.playerMode == PlayerMode.Level || Level.Current.playerMode == PlayerMode.Arcade))
            {
                int parriesThisJump = self.ParriesThisJump;
                self.ParriesThisJump = parriesThisJump + 1;
                if (self.ParriesThisJump > PlayerData.Data.GetNumParriesInRow(self.basePlayer.id))
                {
                    PlayerData.Data.SetNumParriesInRow(self.basePlayer.id, self.ParriesThisJump);
                }
                if (self.ParriesThisJump == 5)
                {
                    OnlineManager.Instance.Interface.UnlockAchievement(self.basePlayer.id, "ParryChain");
                }
            }
            if (self.SuperMeter == self.SuperMeterMax)
            {
                AudioManager.Play("player_parry_power_up_full");
                return;
            }
            AudioManager.Play("player_parry_power_up");
        }

        private void HealerCharm(On.PlayerStatsManager.orig_HealerCharm orig, PlayerStatsManager self)
        {
            if (BossRushManager.inBossRush && (SceneLoader.CurrentLevel == Levels.ChaliceTutorial || SceneLoader.CurrentLevel == Levels.ShmupTutorial))
            {
                int healerHPReceived = self.HealerHPReceived;
                int healerHP = self.HealerHP;
                self.HealerHP = healerHP + 1;
                int healerHPReceived2 = self.HealerHPReceived;
                self.HealerHPReceived = healerHPReceived2 + 1;
                self.SetHealth(self.Health + 1);
                self.OnHealthChanged();
                self.HealerHPCounter = 0;
                LevelPlayerController levelPlayerController = self.basePlayer as LevelPlayerController;
                PlanePlayerController planePlayerController = self.basePlayer as PlanePlayerController;
                if (levelPlayerController != null)
                {
                    levelPlayerController.animationController.OnHealerCharm();
                    return;
                }
                if (planePlayerController != null)
                {
                    planePlayerController.animationController.OnHealerCharm();
                    return;
                }
            }
            else
            {
                int num = self.HealerHPReceived + 1;
                if (self.Loadout.charm == Charm.charm_curse)
                {
                    num = CharmCurse.GetHealerInterval(self.CurseCharmLevel, self.HealerHPReceived);
                }
                int num2 = self.HealerHPCounter;
                self.HealerHPCounter = num2 + 1;
                if (self.HealerHPCounter >= num)
                {
                    num2 = self.HealerHP;
                    self.HealerHP = num2 + 1;
                    int healerHPReceived3 = self.HealerHPReceived;
                    self.HealerHPReceived = healerHPReceived3 + 1;
                    self.SetHealth(self.Health + 1);
                    self.OnHealthChanged();
                    self.HealerHPCounter = 0;
                    LevelPlayerController levelPlayerController2 = self.basePlayer as LevelPlayerController;
                    PlanePlayerController planePlayerController2 = self.basePlayer as PlanePlayerController;
                    if (levelPlayerController2 != null)
                    {
                        levelPlayerController2.animationController.OnHealerCharm();
                        return;
                    }
                    if (planePlayerController2 != null)
                    {
                        planePlayerController2.animationController.OnHealerCharm();
                    }
                }
            }
        }
    }
}
