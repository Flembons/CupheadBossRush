using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HelluvaRush
{
    public class MapBasicStartUIChanges
    {
        public void Init()
        {
            On.MapBasicStartUI.InitUI += InitUI;
        }
        public void InitUI(On.MapBasicStartUI.orig_InitUI orig, MapBasicStartUI self, string level)
        {
            TranslationElement translationElement = Localization.Find(level);
            if (translationElement != null)
            {
                self.Title.GetComponent<LocalizationHelper>().ApplyTranslation(translationElement, null);
                if (level == "ElderKettleLevel")
                {
                    self.Title.text = ((!BossRushManager.bossRushActive) ? "THE ELDER KETTLE" : "RANDOMIZED BOSS RUSH");
                }
                if (Localization.language == Localization.Languages.Japanese)
                {
                    self.Title.lineSpacing = 0f;
                    return;
                }
                self.Title.lineSpacing = 17.46f;
            }
        }
    }
}
