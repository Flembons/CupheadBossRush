using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace HelluvaRush
{
    /*
     * Changed the behavior of the Parryable object's in Ms. Chalice's tutorial. Normally, they would not be able to heal you or 
     * give super meter, but the change to OnParryPrePause alters that so healing and super meter charge are now gained by parrying these
     */
    public class ChaliceTutorialLevelParryableChanges
    {
        public void Init()
        {
            On.ChaliceTutorialLevelParryable.OnParryPrePause += OnParryPrePause;
        }

        public void OnParryPrePause(On.ChaliceTutorialLevelParryable.orig_OnParryPrePause orig, ChaliceTutorialLevelParryable self, AbstractPlayerController player)
        {
            orig(self, player);
            player.stats.OnParry(1f, false);
        }

    }
}
