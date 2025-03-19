using BepInEx;

namespace HelluvaRush
{
    [BepInPlugin("CupheadBossRushMod", "HelluvaRush", "1.1.0")]
    public class Plugin : BaseUnityPlugin
    {
        /*
         *  Helluva Rush is a Boss Rush mode for Cuphead 1.3.4. It adds a new mode allowing you to fight
         *  every boss in the game back-to-back with only a single life. It can be toggled on in the options menu,
         *  and other difficulty settings can be adjusted in that menu as well. 
         *  
         *  The Boss Rush can be started from any boss on the map, or started at Elder Kettle's house for a boss rush with a 
         *  randomized boss order.
         */

        public BossRushManager bossRushManager;
        private void Awake()
        {
            bossRushManager = new BossRushManager();
            new ChaliceTutorialLevelParryableChanges().Init();
            new DicePalaceChanges().Init();
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
            new TutorialChanges().Init();
            Logger.LogInfo($"Helluva Rush is loaded!");
        }
    }
}
