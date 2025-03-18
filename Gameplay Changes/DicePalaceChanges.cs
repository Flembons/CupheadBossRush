using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelluvaRush
{
    public class DicePalaceChanges
    {
        public void Init()
        {
            On.DicePalaceMainLevelGameInfo.SetPlayersStats += SetPlayersStats;
            On.DicePalaceMainLevelGameManager.LevelInit += LevelInit;
        }

        public static void SetPlayersStats(On.DicePalaceMainLevelGameInfo.orig_SetPlayersStats orig)
        {
            if (DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS == null)
                DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS = new PlayersStatsBossesHub();
            PlayerStatsManager stats1 = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats;
            DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.healerHP = stats1.HealerHP;
            DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.healerHPReceived = stats1.HealerHPReceived;
            DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.healerHPCounter = stats1.HealerHPCounter;
            DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.HP = stats1.Health;
            if (DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.HP == 0 && BossRushManager.inBossRush)
                DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.HP = 1;
            if (BossRushManager.bossRushHealBetweenFights && BossRushManager.inBossRush && stats1.Health <= 3)
            {
                DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.HP = 3;
                if (stats1.Loadout.charm == Charm.charm_chalice)
                    ++DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.HP;
            }
            DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.SuperCharge = stats1.SuperMeter;
            if (!BossRushManager.bossRushPreserveSuper && BossRushManager.inBossRush)
                DicePalaceMainLevelGameInfo.PLAYER_ONE_STATS.SuperCharge = 0.0f;
            if (!PlayerManager.Multiplayer)
                return;
            if (DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS == null)
                DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS = new PlayersStatsBossesHub();
            PlayerStatsManager stats2 = PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats;
            DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.healerHP = stats2.HealerHP;
            DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.healerHPReceived = stats2.HealerHPReceived;
            DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.healerHPCounter = stats2.HealerHPCounter;
            DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.HP = stats2.Health;
            if (DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.HP == 0 && BossRushManager.inBossRush && stats2.Health <= 3)
            {
                DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.HP = 3;
                if (stats2.Loadout.charm == Charm.charm_chalice)
                    ++DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.HP;
            }
            if (BossRushManager.bossRushHealBetweenFights && BossRushManager.inBossRush)
                DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.HP = stats2.HealthMax;
            DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.SuperCharge = stats2.SuperMeter;
            if (BossRushManager.bossRushPreserveSuper || !BossRushManager.inBossRush)
                return;
            DicePalaceMainLevelGameInfo.PLAYER_TWO_STATS.SuperCharge = 0.0f;
        }

        public void LevelInit(On.DicePalaceMainLevelGameManager.orig_LevelInit orig, DicePalaceMainLevelGameManager self, LevelProperties.DicePalaceMain properties)
        {
            self.properties = properties;
            Level.Current.OnIntroEvent += new System.Action(self.StartDice);
            self.kingDiceAni = ((Component)self.kingDice).GetComponent<Animator>();
            self.maxSpaces = self.allBoardSpaces.Length;
            self.GameSetup();
            if (BossRushManager.bossRushActive)
                DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED = 14;
            self.marker.position = self.boardSpacesObj[DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED].Pivot.position;
            self.marker.rotation = self.boardSpacesObj[DicePalaceMainLevelGameInfo.PLAYER_SPACES_MOVED].Pivot.rotation;
            if (DicePalaceMainLevelGameInfo.PLAYED_INTRO_SFX)
                return;
            AudioManager.Play("vox_intro");
            self.emitAudioFromObject.Add("vox_intro");
            DicePalaceMainLevelGameInfo.PLAYED_INTRO_SFX = true;
        }
    }
}
