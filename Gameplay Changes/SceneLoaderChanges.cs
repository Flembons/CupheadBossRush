using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class SceneLoaderChanges
    {
        public void Init()
        {
            On.SceneLoader.LoadLastMap += LoadLastMap;
        }

        public static void LoadLastMap(On.SceneLoader.orig_LoadLastMap orig)
        {
            if (Level.IsGraveyard)
            {
                SceneLoader.LoadScene(Scenes.scene_map_world_DLC, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
                return;
            }
            Scenes scene = PlayerData.Data.CurrentMap;
            if (Level.IsChessBoss)
            {
                PlayerData.Data.IncrementKingOfGamesCounter();
                PlayerData.SaveCurrentFile();
                if (PlayerData.Data.CountLevelsCompleted(Level.kingOfGamesLevels) == Level.kingOfGamesLevels.Length)
                {
                    scene = Scenes.scene_level_chess_castle;
                }
            }
            SceneLoader.LoadScene(scene, SceneLoader.Transition.Iris, SceneLoader.Transition.Iris, SceneLoader.Icon.Hourglass, null);
        }
    }
}
