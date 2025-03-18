using BepInEx;

namespace HelluvaRush
{
    [BepInPlugin("CupheadBossRushMod", "HelluvaRush", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        public BossRushManager bossRushManager;
        private void Awake()
        {
            bossRushManager = new BossRushManager();
            new ChaliceTutorialLevelParryableChanges().Init();
            new DicePalaceChanges().Init();
            new HouseLevelTutorialChanges().Init();
            new LevelChanges().Init();
            new LevelHUDChanges().Init();
            new LevelPauseGUIChanges().Init();
            new LevelGameOverGUIChanges().Init();  
            new MapBasicStartUIChanges().Init();
            new MapDicePalaceSceneLoaderChanges().Init();
            new MapDifficultySelectStartUIChanges().Init();
            new MapGraveyardHandlerChanges().Init();
            new MapLevelLoaderChanges().Init();
            new OptionsGUIChanges().Init();
            new PlayerStatsManagerChanges().Init();
            new SceneLoaderChanges().Init();
            new TutorialChanges().Init();
            Logger.LogInfo($"Helluva Rush is loaded!");
        }
    }
}
