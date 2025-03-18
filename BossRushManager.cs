using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blender;
using UnityEngine;
using TMPro;

namespace HelluvaRush
{
    public class BossRushManager
    {
        public static bool bossRushActive;
        public static Level.Mode bossRushDiff;
        public static int levelsUntilTutorial;
        public static int tutorialEveryXLevels;
        public static bool breakForTutorial;
        public static bool bossRushHealBetweenFights;
        public static bool bossRushJustStarted;
        public static bool bossRushMultiplayer;
        public static bool bossRushPreserveSuper;

        public static bool isBossOrderRandom;
        public static int levelIndex;
        public static int bossesDefeated;
        public static int bossRushStartingIndex;
        public static Levels[] bossLevels;

        public static bool inBossRush;
        public static bool bossRushOptions;
        public static bool refreshTutorialDialogue;

        public static TMP_Text bossCounter;
        public static GameObject bossCounterObj;

        public BossRushManager()
        {
            bossRushActive = false;
            bossRushDiff = Level.Mode.Normal;
            levelsUntilTutorial = 4;
            tutorialEveryXLevels = 4;
            breakForTutorial = true;
            bossRushHealBetweenFights = false;
            bossRushJustStarted = false;
            bossRushMultiplayer = false;
            bossRushPreserveSuper = true;
            inBossRush = false;
            bossRushOptions = false;
            refreshTutorialDialogue = false;
            initBossLevels();
        }

        public static void initBossLevels()
        {
            isBossOrderRandom = false;
            levelIndex = 0;
            bossesDefeated = 0;
            bossRushStartingIndex = 0;

            DicePalaceMainLevelGameInfo gameInfo = DicePalaceMainLevelGameInfo.GameInfo;

            if (BlenderAPI.HasDLC)
            {
                bossLevels = new Levels[]
                {
                    Levels.Slime,
                    Levels.Veggies,
                    Levels.Frogs,
                    Levels.FlyingBlimp,
                    Levels.Flower,
                    Levels.Clown,
                    Levels.FlyingGenie,
                    Levels.Baroness,
                    Levels.FlyingBird,
                    Levels.Dragon,
                    Levels.Bee,
                    Levels.Mouse,
                    Levels.Pirate,
                    Levels.FlyingMermaid,
                    Levels.Robot,
                    Levels.SallyStagePlay,
                    Levels.Train,
                    Levels.DicePalaceBooze,
                    Levels.DicePalaceChips,
                    Levels.DicePalaceCigar,
                    Levels.DicePalaceDomino,
                    Levels.DicePalaceRabbit,
                    Levels.DicePalaceFlyingHorse,
                    Levels.DicePalaceRoulette,
                    Levels.DicePalaceEightBall,
                    Levels.DicePalaceFlyingMemory,
                    Levels.DicePalaceMain,
                    Levels.Devil,
                    Levels.OldMan,
                    Levels.SnowCult,
                    Levels.Airplane,
                    Levels.Graveyard,
                    Levels.FlyingCowboy,
                    Levels.RumRunners,
                    Levels.ChessPawn,
                    Levels.ChessKnight,
                    Levels.ChessBishop,
                    Levels.ChessRook,
                    Levels.ChessQueen,
                    Levels.Saltbaker
                };
                return;
            }
            bossLevels = new Levels[]
            {
                Levels.Slime,
                Levels.Veggies,
                Levels.Frogs,
                Levels.FlyingBlimp,
                Levels.Flower,
                Levels.Clown,
                Levels.FlyingGenie,
                Levels.Baroness,
                Levels.FlyingBird,
                Levels.Dragon,
                Levels.Bee,
                Levels.Mouse,
                Levels.Pirate,
                Levels.FlyingMermaid,
                Levels.Robot,
                Levels.SallyStagePlay,
                Levels.Train,
                Levels.DicePalaceBooze,
                Levels.DicePalaceChips,
                Levels.DicePalaceCigar,
                Levels.DicePalaceDomino,
                Levels.DicePalaceRabbit,
                Levels.DicePalaceFlyingHorse,
                Levels.DicePalaceRoulette,
                Levels.DicePalaceEightBall,
                Levels.DicePalaceFlyingMemory,
                Levels.DicePalaceMain,
                Levels.Devil
            };
        }
        public static void randomizeBossOrder()
        {
            Levels[] bosses = bossLevels;

            for (int i = bossLevels.Length - 1; i > 0; i--)
            {
                int num = UnityEngine.Random.Range(0, i);
                Levels level = bosses[i];
                bosses[i] = bosses[num];
                bosses[num] = level;
            }

            bossLevels = bosses;
            levelIndex = 0;
            isBossOrderRandom = true;
        }

        public static void startBossRush(bool isRandom)
        {
            inBossRush = true;
            if (isRandom)
            {
                randomizeBossOrder();
            }

            Level.IsDicePalace = true;
            bossRushJustStarted = true;
           
            if (PlayerManager.Multiplayer)
            {
                bossRushMultiplayer = true;
            }
           
            bossesDefeated = 0;
            levelIndex = bossRushStartingIndex;
            levelIndex++;
           
            if (bossRushDiff == Level.Mode.Easy)
            {
                tutorialEveryXLevels = 3;
            }
            else if (bossRushDiff == Level.Mode.Normal)
            {
                tutorialEveryXLevels = 4;
            }
            else if (bossRushDiff == Level.Mode.Hard)
            {
                tutorialEveryXLevels = 5;
            }
            
            levelsUntilTutorial = tutorialEveryXLevels;
           
            if (levelIndex == bossLevels.Length)
            {
                levelIndex = 0;
            }

            SceneLoader.LoadLevel(bossLevels[bossRushStartingIndex], SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
        }

        public static void endBossRush()
        {
            initBossLevels();
            Level.IsDicePalace = false;
            inBossRush = false;
            bossesDefeated = 0;
            DicePalaceMainLevelGameInfo.CleanUpRetry();
        }
        public static void startBossRushAtIndex(int levelIndex)
        {
            bossRushStartingIndex = levelIndex;
            startBossRush(false);
        }
        public static void loadNextBossRushLevel()
        {
            if (!inBossRush)
            {
                return;
            }

            Level.IsDicePalace = true;
            bossesDefeated++;
            if (breakForTutorial)
            {
                levelsUntilTutorial--;
                if (levelsUntilTutorial == 0 && bossesDefeated != bossLevels.Length)
                {
                    PlayerStatsManager stats = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats;
                    stats.HealerHPReceived = 0;
                    if (PlayerManager.Multiplayer)
                    {
                        PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.HealerHPReceived = 0;
                    }
                    DicePalaceMainLevelGameInfo.SetPlayersStats();
                    if (stats.Loadout.charm == Charm.charm_chalice)
                    {
                        SceneLoader.LoadLevel(Levels.ChaliceTutorial, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
                    }
                    else
                    {
                        SceneLoader.LoadLevel(Levels.ShmupTutorial, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
                    }
                    levelsUntilTutorial = tutorialEveryXLevels + 1;
                    return;
                }
            }
            DicePalaceMainLevelGameInfo.SetPlayersStats();
            if (bossesDefeated != bossLevels.Length)
            {
                SceneLoader.LoadLevel(bossLevels[levelIndex], SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
            }
            else
            {
                endBossRush();
                SceneLoader.LoadScene(Scenes.scene_win, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.None, null);
            }
            levelIndex++;
            if (levelIndex != bossLevels.Length || isBossOrderRandom)
            {
                return;
            }
            levelIndex = 0;
        }
    }
}
