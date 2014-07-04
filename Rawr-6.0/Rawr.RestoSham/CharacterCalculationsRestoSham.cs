﻿using System;
using System.Collections.Generic;

namespace Rawr.RestoSham
{
    internal sealed class CharacterCalculationsRestoSham : CharacterCalculationsBase
    {
        private float _OverallPoints = 0f;
        public override float OverallPoints
        {
            get { return _OverallPoints; }
            set { _OverallPoints = value; }
        }

        private float[] _SubPoints = new float[] { 0f, 0f, 0f };
        public override float[] SubPoints
        {
            get { return _SubPoints; }
            set { _SubPoints = value; }
        }

        public Stats BasicStats { get; set; }

        #region Displayed Calculations

        public float SpellCrit { get; set; }
        public float DeepHeals { get; set; }
        public float SpellHaste { get; set; }
        public float FightHPS { get; set; }
        public string SustainedSequence { get; set; }
        public string BurstSequence { get; set; }
        public string SustainedSequenceShort { get; set; }
        public string BurstSequenceShort { get; set; }
        public float MAPS { get; set; }
        public float ManaUsed { get; set; }
        public float HSTHeals { get; set; }
        public float ESHPS { get; set; }
        public float HWSpamHPS { get; set; }
        public float HWSpamMPS { get; set; }
        public float GHWSpamHPS { get; set; }
        public float GHWSpamMPS { get; set; }
        public float HSrgSpamHPS { get; set; }
        public float HSrgSpamMPS { get; set; }
        public float CHSpamHPS { get; set; }
        public float CHSpamMPS { get; set; }
        public float RTHWHPS { get; set; }
        public float RTHWMPS { get; set; }
        public float RTGHWHPS { get; set; }
        public float RTGHWMPS { get; set; }
        public float RTHSrgHPS { get; set; }
        public float RTHSrgMPS { get; set; }
        public float RTCHHPS { get; set; }
        public float RTCHMPS { get; set; }
        public float RealHWCast { get; set; }
        public float RealGHWCast { get; set; }
        public float RealHSrgCast { get; set; }
        public float RealCHCast { get; set; }
        public float LBCast { get; set; }
        public float LBRestore { get; set; }
        public float LBNumber { get; set; }
        public float BurstHPS { get; set; }
        public float SustainedHPS { get; set; }
        public float MUPS { get; set; }
        public float Survival { get; set; }
        public float MailSpecialization { get; set; }
        public float RSTankHit { get; set; }
        public float RSTankTick { get; set; }
        public float _HealPerSec { get; set; }
        public float _HealHitPerSec { get; set; }
        public float _CritPerSec { get; set; }
        public float _FightSeconds { get; set; }
        public float _CastingActivity { get; set; }

        #endregion

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            values.Add("HPS - Burst", Math.Round(BurstHPS, 0).ToString());
            values.Add("HPS - Sustained", Math.Round(SustainedHPS, 0).ToString());
            values.Add("Survival", Math.Round(Survival, 0).ToString());
            values.Add("Health", string.Format("{0:N0}", BasicStats.Health));
            values.Add("Stamina", string.Format("{0:N0}", BasicStats.Stamina));
            values.Add("Intellect", string.Format("{0:N0}", BasicStats.Intellect));
            values.Add("Spell Power", string.Format("{0:N0}", BasicStats.SpellPower));
            values.Add("Mana", string.Format("{0:N0}", BasicStats.Mana));
            values.Add("MP5", string.Format("{0:N0}", BasicStats.Mp5));
            values.Add("Deep Healing %", string.Format("{0}%*{1:N0} mastery rating",
                       Math.Round(DeepHeals), BasicStats.MasteryRating));
            values.Add("Mana Available", Math.Round(ManaUsed, 0).ToString());
            values.Add("Heal Spell Crit", string.Format("{0:p2}*{1} spell crit rating",
                       BasicStats.SpellCrit, BasicStats.CritRating));
            values.Add("Spell Haste", string.Format("{0:p2}*{1} spell haste rating",
                       BasicStats.SpellHaste, BasicStats.HasteRating));
            values.Add("TC Mana Restore", string.Format("{0:N2}", LBRestore));
            values.Add("Mail Specialization", string.Format("{0:p0}", MailSpecialization));
            values.Add("Burst Sequence", string.Format("{0}*{1}", BurstSequenceShort, BurstSequence));
            values.Add("Sustained Sequence", string.Format("{0}*{1}", SustainedSequenceShort, SustainedSequence));
            values.Add("Mana Available per Second", Math.Round(MAPS, 0).ToString());
            values.Add("Mana Used per Second", Math.Round(MUPS, 0).ToString());
            values.Add("Healing Stream HPS", Math.Round(HSTHeals, 0).ToString());
            values.Add("Earth Shield HPS", string.Format("{0:N0}", ESHPS));
            values.Add("RT+HW HPS", Math.Round(RTHWHPS, 0).ToString());
            values.Add("RT+GHW HPS", Math.Round(RTGHWHPS, 0).ToString());
            values.Add("RT+HSrg HPS", Math.Round(RTHSrgHPS, 0).ToString());
            values.Add("RT+CH HPS", Math.Round(RTCHHPS, 0).ToString());
            values.Add("HW Spam HPS", Math.Round(HWSpamHPS, 0).ToString());
            values.Add("GHW Spam HPS", Math.Round(GHWSpamHPS, 0).ToString());
            values.Add("HS Spam HPS", Math.Round(HSrgSpamHPS, 0).ToString());
            values.Add("CH Spam HPS", Math.Round(CHSpamHPS, 0).ToString());
            values.Add("Global Cooldown", string.Format("{0:0.00}s", Math.Max(1.5f / (1f + SpellHaste), 1f)));

            // These all use string.Format() so they always have 2 digits after the decimal
            values.Add("Healing Wave", string.Format("{0:0.00}s / {1:0.00}s", RealHWCast, RealHWCast * 0.7));
            values.Add("Greater Healing Wave", string.Format("{0:0.00}s / {1:0.00}s", RealGHWCast, RealGHWCast * 0.7));
            values.Add("Healing Surge", string.Format("{0:0.00}s",RealHSrgCast));
            values.Add("Chain Heal", string.Format("{0:0.00}s", RSTankTick));
            values.Add("Lightning Bolt", string.Format("{0:0.00}s", RSTankHit));

            return values;
        }
        
        /// <summary>
        /// Gets the optimizable calculation value.
        /// </summary>
        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Mana Usable per Second":
                    return MAPS;
                case "Health":
                    return BasicStats.Health;
                case "Haste %":
                    return (float)Math.Round(SpellHaste * 100, 2);
                case "Crit %":
                    return (float)Math.Round(SpellCrit * 100, 2);
            }
            return 0f;
        }
    }
}
