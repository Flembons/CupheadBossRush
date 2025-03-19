using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace HelluvaRush
{
    /*
     * The gravestone in the DLC graveyard area has a special map loader, so it needed to be individually changed.
     * Now interacting with the gravestone while Boss Rush is active will start a boss rush at the Angel/Devil fight
     */

    public class MapGraveyardHandlerChanges
    {
        public void Init()
        {
            On.MapGraveyardHandler.load_fight_cr += load_fight_cr;
        }
        private IEnumerator load_fight_cr(On.MapGraveyardHandler.orig_load_fight_cr orig, MapGraveyardHandler self)
        {
            PlayerManager.SetPlayerCanJoin(PlayerId.PlayerTwo, false, false);
            self.SetPlayerReturnPos();
            Map.Current.CurrentState = Map.State.Graveyard;
            if (Map.Current.players[0] != null)
            {
                Map.Current.players[0].animator.SetTrigger("Sleep");
            }
            if (Map.Current.players[1] != null)
            {
                Map.Current.players[1].animator.SetTrigger("Sleep");
            }
            yield return new WaitForSeconds(1f);

            if (BossRushManager.bossRushActive)
            {
                BossRushManager.startBossRushAtIndex(Array.IndexOf<Levels>(BossRushManager.bossLevels, Levels.Graveyard));
            }
            else
            {
                SceneLoader.LoadScene(Scenes.scene_level_graveyard, SceneLoader.Transition.Blur, SceneLoader.Transition.Blur, SceneLoader.Icon.HourglassBroken, null);
            }

            yield break;
        }
    }
}
