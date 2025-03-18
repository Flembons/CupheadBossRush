using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Diagnostics;
using System.Collections;

namespace HelluvaRush
{
    public class LevelChanges
    {
        public void Init()
        {
            On.Level.Awake += Awake;
            On.LevelEnd.win_cr += win_cr;
        }

        protected void Awake(On.Level.orig_Awake orig, Level self)
        {
            self.useGUILayout = false;
            PauseManager.AddChild(self);
            self.preEnabled = self.enabled;
            self.emitAudioFromObject = new SoundEmitter(self);
            if (!BossRushManager.inBossRush)
            {
                self.CheckIfInABossesHub();
            }
            Cuphead.Init(false);
            PlayerManager.OnPlayerJoinedEvent += self.OnPlayerJoined;
            PlayerManager.OnPlayerLeaveEvent += self.OnPlayerLeave;
            DamageDealer.didDamageWithNonSmallPlaneWeapon = false;
            Levels currentLevel = self.CurrentLevel;
            if (currentLevel == Levels.Platforming_Level_1_1 || currentLevel == Levels.Platforming_Level_1_2 || currentLevel == Levels.Platforming_Level_2_1 || currentLevel == Levels.Platforming_Level_2_2 || currentLevel == Levels.Platforming_Level_3_1 || currentLevel == Levels.Platforming_Level_3_2)
            {
                self.mode = Level.Mode.Normal;
            }
            else
            {
                self.mode = Level.CurrentMode;
            }
            if (BossRushManager.bossRushActive)
            {
                self.mode = BossRushManager.bossRushDiff;
                if (self.CurrentLevel == Levels.Saltbaker && BossRushManager.bossRushDiff == Level.Mode.Easy)
                {
                    self.mode = Level.Mode.Normal;
                }
            }
            if (self.CurrentLevel == Levels.ChessBishop || self.CurrentLevel == Levels.ChessKnight || self.CurrentLevel == Levels.ChessPawn || self.CurrentLevel == Levels.ChessQueen || self.CurrentLevel == Levels.ChessRook || self.CurrentLevel == Levels.Graveyard)
            {
                self.mode = Level.Mode.Normal;
            }
            Level.Current = self;
            PlayerData.PlayerLevelDataObject levelData = PlayerData.Data.GetLevelData(self.CurrentLevel);
            Level.Won = false;
            self.BGMPlaylistCurrent = levelData.bgmPlayListCurrent;
            Level.PreviousLevel = self.CurrentLevel;
            Level.PreviousLevelType = self.type;
            Level.PreviouslyWon = levelData.completed;
            Level.PreviousGrade = levelData.grade;
            Level.PreviousDifficulty = levelData.difficultyBeaten;
            Level.SuperUnlocked = false;
            Level.IsChessBoss = false;
            Level.IsGraveyard = false;
            self.Ending = false;
            self.PartialInit();
            Application.targetFrameRate = 60;
            self.CreateUI();
            self.CreateHUD();
            LevelCoin.OnLevelStart();
            SceneLoader.SetCurrentLevel(self.CurrentLevel);
        }

        private IEnumerator win_cr(On.LevelEnd.orig_win_cr orig, LevelEnd self, IEnumerator knockoutSFXCoroutine, Action onBossDeathCallback, Action explosionsCallback, Action explosionsFalloffCallback, Action explosionsEndCallback, AbstractPlayerController[] players, float bossDeathTime, bool goToWinScreen, bool isMausoleum, bool isDevil, bool isTowerOfPower)
        {
            PauseManager.Pause();
            LevelKOAnimation levelKOAnimation = LevelKOAnimation.Create(isMausoleum);
            if (Level.IsChessBoss)
            {
                AudioManager.StartBGMAlternate(0);
            }
            if (Level.Current.CurrentLevel == Levels.Saltbaker)
            {
                AudioManager.StartBGMAlternate(2);
            }
            self.StartCoroutine(knockoutSFXCoroutine);
            yield return levelKOAnimation.StartCoroutine(levelKOAnimation.anim_cr());
            PauseManager.Unpause();
            explosionsCallback();
            CupheadTime.SetAll(1f);
            if (!isMausoleum)
            {
                foreach (AbstractPlayerController abstractPlayerController in PlayerManager.GetAllPlayers())
                {
                    if (!(abstractPlayerController == null))
                    {
                        abstractPlayerController.OnLevelWin();
                    }
                }
            }
            if (onBossDeathCallback != null)
            {
                onBossDeathCallback();
            }
            yield return new WaitForSeconds(bossDeathTime + 0.3f);
            AbstractProjectile[] array = UnityEngine.Object.FindObjectsOfType<AbstractProjectile>();
            for (int i = 0; i < array.Length; i++)
            {
                array[i].OnLevelEnd();
            }
            if (Level.IsTowerOfPower)
            {
                TowerOfPowerLevelGameInfo.SetPlayersStats(PlayerId.PlayerOne);
                if (PlayerManager.Multiplayer)
                {
                    TowerOfPowerLevelGameInfo.SetPlayersStats(PlayerId.PlayerTwo);
                }
            }
            else if (Level.IsDicePalace && !Level.IsDicePalaceMain)
            {
                DicePalaceMainLevelGameInfo.SetPlayersStats();
            }
            SceneLoader.properties.transitionStart = SceneLoader.Transition.Fade;
            SceneLoader.properties.transitionStartTime = 3f;
            if (Level.IsChessBoss || Level.Current.CurrentLevel == Levels.Saltbaker)
            {
                yield return new WaitForSeconds(2f);
            }
            if (BossRushManager.inBossRush)
            {
                BossRushManager.loadNextBossRushLevel();
            }
            else if (goToWinScreen && !BossRushManager.inBossRush)
            {
                SceneLoader.LoadScene(Scenes.scene_win, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.None, null);
            }
            else if (Level.IsTowerOfPower)
            {
                SceneLoader.ContinueTowerOfPower();
            }
            else if (Level.IsGraveyard)
            {
                SceneLoader.LoadScene(Scenes.scene_map_world_DLC, SceneLoader.Transition.Fade, SceneLoader.Transition.Iris, SceneLoader.Icon.None, null);
            }
            else if (Level.IsChessBoss)
            {
                if (SceneLoader.CurrentContext is GauntletContext)
                {
                    int num = MathUtilities.NextIndex(Array.IndexOf<Levels>(Level.kingOfGamesLevels, Level.Current.CurrentLevel), Level.kingOfGamesLevels.Length);
                    if (num == 0)
                    {
                        SceneLoader.LoadScene(Scenes.scene_level_chess_castle, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.None, new GauntletContext(true));
                    }
                    else
                    {
                        Levels level = Level.kingOfGamesLevels[num];
                        SceneLoader.Transition transitionStart = SceneLoader.Transition.Fade;
                        GauntletContext context = new GauntletContext(false);
                        SceneLoader.LoadLevel(level, transitionStart, SceneLoader.Icon.Hourglass, context);
                    }
                }
                else
                {
                    SceneLoader.LoadScene(Scenes.scene_level_chess_castle, SceneLoader.Transition.Fade, SceneLoader.Transition.Fade, SceneLoader.Icon.None, null);
                }
            }
            else if (!isMausoleum)
            {
                SceneLoader.ReloadLevel();
            }
            yield return new WaitForSeconds(2.5f);
            explosionsEndCallback();
            yield break;
        }
    }


}
