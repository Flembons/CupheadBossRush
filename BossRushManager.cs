using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Blender;
using UnityEngine;
using TMPro;

namespace HelluvaRush
{

    /*
     * This class manages all of the Boss Rush's properties.
     * Initializing the boss order, starting and ending the boss rush, and 
     * loading the next boss level are all handled here.
     */
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


        // Initializes the default settings for Boss Rush
        public static void initBossLevels()
        {
            isBossOrderRandom = false;
            levelIndex = 0;
            bossesDefeated = 0;
            bossRushStartingIndex = 0;

            DicePalaceMainLevelGameInfo gameInfo = DicePalaceMainLevelGameInfo.GameInfo;

            // Check if the DLC is installed and adjust the possible levels accordingly
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

        // Randomizes the order of boss levels whenever a new random boss rush is started
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


        // Begins the boss rush
        public static void startBossRush(bool isRandom)
        {
            inBossRush = true;

            // If this was started from Elder Kettle's house, then the boss order will be randomized
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
           
            // Depending on the difficulty, the boss rush will break every 3/4/5 levels for a rest area
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
           
            // If the current boss is at the end of the array and there are still bosses left, loop back to the front of the array
            if (levelIndex == bossLevels.Length)
            {
                levelIndex = 0;
            }

            // Load the first boss level
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

        // If boss rush is started from any of the map nodes, this will start the boss rush from that specific node
        public static void startBossRushAtIndex(int levelIndex)
        {
            bossRushStartingIndex = levelIndex;
            startBossRush(false);
        }

        // Loads the next boss rush level in the bossLevels array
        public static void loadNextBossRushLevel()
        {
            if (!inBossRush)
            {
                return;
            }

            Level.IsDicePalace = true;
            bossesDefeated++;

            // Check if the next level should be a rest area
            if (breakForTutorial)
            {
                levelsUntilTutorial--;
                if (levelsUntilTutorial == 0 && bossesDefeated != bossLevels.Length)
                {
                    PlayerStatsManager stats = PlayerManager.GetPlayer(PlayerId.PlayerOne).stats;

                    // Reset Healer Charm HP to 0, so the player is able to heal 3 more times
                    stats.HealerHPReceived = 0;

                    // If a second player is present, set player2's HealerHP to 0 as well so they can heal too
                    if (PlayerManager.Multiplayer)
                    {
                        PlayerManager.GetPlayer(PlayerId.PlayerTwo).stats.HealerHPReceived = 0;
                    }
                    DicePalaceMainLevelGameInfo.SetPlayersStats();

                    // Load the Chalice tutorial if the player is Chalice
                    if (stats.Loadout.charm == Charm.charm_chalice)
                    {
                        SceneLoader.LoadLevel(Levels.ChaliceTutorial, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
                    }
                    // Otherwise, load the shmup tutorial level since Cuphead can't interact with Chalice's tutorial
                    else
                    {
                        SceneLoader.LoadLevel(Levels.ShmupTutorial, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
                    }
                    levelsUntilTutorial = tutorialEveryXLevels + 1;
                    return;
                }
            }
            DicePalaceMainLevelGameInfo.SetPlayersStats();

            // Load the next boss if we're not at the end
            if (bossesDefeated != bossLevels.Length)
            {
                SceneLoader.LoadLevel(bossLevels[levelIndex], SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
            }
            // End the boss rush if all bosses have been defeated
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
