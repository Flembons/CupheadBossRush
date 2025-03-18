using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace HelluvaRush
{
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
