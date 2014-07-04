﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Rogue
{
    public class CharacterCalculationsRogue : CharacterCalculationsBase
    {
        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DPSPoints
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float SurvivabilityPoints
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        public Stats BasicStats { get; set; }
        public int TargetLevel { get; set; }
        public int Spec { get; set; }

        public float AvoidedWhiteMHAttacks { get; set; }
        public float AvoidedWhiteOHAttacks { get; set; }
        public float AvoidedAttacks { get; set; }
        public float AvoidedFinisherAttacks { get; set; }
        public float AvoidedPoisonAttacks { get; set; }
        public float DodgedMHAttacks { get; set; }
        public float DodgedOHAttacks { get; set; }
        public float ParriedAttacks { get; set; }
        public float MissedWhiteAttacks { get; set; }
        public float MissedAttacks { get; set; }
        public float MissedPoisonAttacks { get; set; }
        public float CritChanceYellow { get; set; }
        public float CritChanceMHTotal { get; set; }
        public float CritChanceMH { get; set; }
        public float CritChanceOHTotal { get; set; }
        public float CritChanceOH { get; set; }
        public float MainHandSpeed { get; set; }
        public float OffHandSpeed { get; set; }
        public float ArmorMitigation { get; set; }
        public float Duration { get; set; }

        public RogueAbilityStats MainHandStats { get; set; }
        public RogueAbilityStats OffHandStats { get; set; }
        public RogueAbilityStats MainGaucheStats { get; set; }
        public RogueAbilityStats BackstabStats { get; set; }
        public RogueAbilityStats HemoStats { get; set; }
        public RogueAbilityStats SStrikeStats { get; set; }
        public RogueAbilityStats MutiStats { get; set; }
        public RogueAbilityStats RStrikeStats { get; set; }
        public RogueAbilityStats RuptStats { get; set; }
        public RogueAbilityStats SnDStats { get; set; }
        public RogueAbilityStats EvisStats { get; set; }
        public RogueAbilityStats EnvenomStats { get; set; }
        public RogueAbilityStats IPStats { get; set; }
        public RogueAbilityStats DPStats { get; set; }
        public RogueAbilityStats WPStats { get; set; }
        public RogueAbilityStats VenomousWoundsStats { get; set; }

        public RogueRotationCalculator.RogueRotationCalculation HighestDPSRotation { get; set; }
        public RogueRotationCalculator.RogueRotationCalculation CustomRotation { get; set; }

        public string Rotations { get; set; }

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            dictValues.Add("Overall Points", OverallPoints.ToString());
            dictValues.Add("DPS Points", DPSPoints.ToString());
            dictValues.Add("Survivability Points", SurvivabilityPoints.ToString());

            float baseMiss = StatConversion.WHITE_MISS_CHANCE_CAP_DW[TargetLevel - 85] - BasicStats.PhysicalHit;
            float baseYellowMiss = StatConversion.WHITE_MISS_CHANCE_CAP[TargetLevel - 85] - BasicStats.PhysicalHit;
            float basePoisonMiss = StatConversion.GetSpellMiss(85 - TargetLevel, false) - BasicStats.SpellHit;
            float baseDodge = StatConversion.WHITE_DODGE_CHANCE_CAP[TargetLevel - 85] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
            float baseParry = 0f;// StatConversion.WHITE_PARRY_CHANCE_CAP[TargetLevel - 85] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
            float baseWhiteMHCrit = CritChanceMHTotal;
            float baseWhiteOHCrit = CritChanceOHTotal;
            float capMiss = (float)Math.Ceiling(baseMiss * StatConversion.RATING_PER_PHYSICALHIT);
            float capYellowMiss = (float)Math.Ceiling(baseYellowMiss * StatConversion.RATING_PER_PHYSICALHIT);
            float capPoisonMiss = (float)Math.Ceiling(basePoisonMiss * StatConversion.RATING_PER_SPELLHIT);
            float capDodge = (float)Math.Ceiling(baseDodge * 100f * StatConversion.RATING_PER_EXPERTISE / (StatConversion.RATING_PER_DODGEPARRYREDUC * 100f));
            float capParry = (float)Math.Ceiling(baseParry * 100f * 32.78998947f); // TODO: Check this value
            float capWhiteMHCrit = 100 - StatConversion.WHITE_GLANCE_CHANCE_CAP[TargetLevel - 85] * 100 - MissedWhiteAttacks - DodgedMHAttacks;
            float capWhiteOHCrit = 100 - StatConversion.WHITE_GLANCE_CHANCE_CAP[TargetLevel - 85] * 100 - MissedWhiteAttacks - DodgedOHAttacks;

            string tipMiss = "*White: ";
            if (BasicStats.HitRating > capMiss)
                tipMiss += string.Format("Over the cap ({1}) by {0} Hit Rating", BasicStats.HitRating - capMiss, capMiss);
            else if (BasicStats.HitRating < capMiss)
                tipMiss += string.Format("Under the cap ({1}) by {0} Hit Rating", capMiss - BasicStats.HitRating, capMiss);
            else
                tipMiss += string.Format("Exactly at the cap ({0})", capMiss);

            tipMiss += "\r\nYellow: ";
            if (BasicStats.HitRating > capYellowMiss)
                tipMiss += string.Format("Over the cap ({1}) by {0} Hit Rating", BasicStats.HitRating - capYellowMiss, capYellowMiss);
            else if (BasicStats.HitRating < capYellowMiss)
                tipMiss += string.Format("Under the cap ({1}) by {0} Hit Rating", capYellowMiss - BasicStats.HitRating, capYellowMiss);
            else
                tipMiss += string.Format("Exactly at the cap ({0})", capYellowMiss);

            tipMiss += "\r\nPoison: ";
            if (BasicStats.HitRating > capPoisonMiss)
                tipMiss += string.Format("Over the cap ({1}) by {0} Hit Rating", BasicStats.HitRating - capPoisonMiss, capPoisonMiss);
            else if (BasicStats.HitRating < capPoisonMiss)
                tipMiss += string.Format("Under the cap ({1}) by {0} Hit Rating", capPoisonMiss - BasicStats.HitRating, capPoisonMiss);
            else
                tipMiss += string.Format("Exactly at the cap ({0})", capPoisonMiss);

            string tipDodge = string.Empty;
            if (BasicStats.ExpertiseRating > capDodge)
                tipDodge = string.Format("*Over the cap ({1}) by {0} Expertise Rating", BasicStats.ExpertiseRating - capDodge, capDodge);
            else if (BasicStats.ExpertiseRating < capDodge)
                tipDodge = string.Format("*Under the cap ({1}) by {0} Expertise Rating", capDodge - BasicStats.ExpertiseRating, capDodge);
            else
                tipDodge = string.Format("*Exactly at the cap ({0})", capDodge);

            string tipCrit = string.Format("Mainhand: {0}, ", CritChanceMH);
            if (CritChanceMHTotal > capWhiteMHCrit)
                tipCrit += string.Format("over the Crit cap ({1}) by {0}%", CritChanceMHTotal - capWhiteMHCrit, capWhiteMHCrit);
            else if (CritChanceMHTotal < capWhiteMHCrit)
                tipCrit += string.Format("under the Crit cap ({1}) by {0}%", capWhiteMHCrit - CritChanceMHTotal, capWhiteMHCrit);
            else tipCrit += string.Format("exactly at the Crit cap ({0})", capWhiteMHCrit);

            tipCrit += string.Format("\nOffhand: {0}, ", CritChanceOH);
            if (CritChanceOHTotal > capWhiteOHCrit)
                tipCrit += string.Format("over the Crit cap ({1}) by {0}%", CritChanceOHTotal - capWhiteOHCrit, capWhiteOHCrit);
            else if (CritChanceOHTotal < capWhiteOHCrit)
                tipCrit += string.Format("under the Crit cap ({1}) by {0}%", capWhiteOHCrit - CritChanceOHTotal, capWhiteOHCrit);
            else tipCrit += string.Format("exactly at the Crit cap ({0})", capWhiteOHCrit);

            string tipMastery = "*";
            if (Spec == 0) tipMastery += String.Format("{0}% increased Poison damage", BasicStats.MasteryRating * RV.Mastery.PotentPoisonsDmgMultPerMast);
            else if (Spec == 1) tipMastery += String.Format("{0}% chance on an extra mainhand attack", BasicStats.MasteryRating * RV.Mastery.MainGauchePerMast);
            else tipMastery += String.Format("{0}% increased finishing move damage and Slice and Dice effectiveness", BasicStats.MasteryRating * RV.Mastery.ExecutionerPerMast);

            dictValues.Add("Health", BasicStats.Health.ToString());
            dictValues.Add("Attack Power", BasicStats.AttackPower.ToString());
            dictValues.Add("Agility", BasicStats.Agility.ToString());
            dictValues.Add("Strength", BasicStats.Strength.ToString());
            dictValues.Add("Crit Rating", BasicStats.CritRating.ToString());
            dictValues.Add("Hit Rating", BasicStats.HitRating.ToString() + tipMiss);
            dictValues.Add("Expertise Rating", BasicStats.ExpertiseRating.ToString() + tipDodge);
            dictValues.Add("Haste Rating", BasicStats.HasteRating.ToString());
            dictValues.Add("Mastery Rating", BasicStats.MasteryRating.ToString() + tipMastery);
            dictValues.Add("Armor Penetration", BasicStats.ArmorPenetration.ToString());
            dictValues.Add("Weapon Damage", "+" + BasicStats.WeaponDamage.ToString());

            dictValues.Add("Avoided White Attacks", string.Format("{0}% / {1}%*Mainhand: {2}% Dodged, {3}% Missed\n   Offhand: {4}% Dodged, {3}% Missed", AvoidedWhiteMHAttacks, AvoidedWhiteOHAttacks, DodgedMHAttacks, MissedWhiteAttacks, DodgedOHAttacks));
            dictValues.Add("Avoided Yellow Attacks", string.Format("{0}%*{1}% Dodged, {2}% Missed", AvoidedAttacks, DodgedMHAttacks, MissedAttacks));
            dictValues.Add("Avoided Poison Attacks", string.Format("{0}%*{1}% Missed", AvoidedPoisonAttacks, MissedPoisonAttacks));
            dictValues.Add("Crit Chance", CritChanceYellow.ToString() + "%*" + tipCrit);
            dictValues.Add("MainHand Speed", MainHandSpeed.ToString() + "s");
            dictValues.Add("OffHand Speed", OffHandSpeed.ToString() + "s");
            dictValues.Add("Armor Mitigation", ArmorMitigation.ToString() + "%");

            dictValues.Add("Optimal Rotation", HighestDPSRotation.ToString());
            dictValues.Add("Optimal Rotation DPS", HighestDPSRotation.DPS.ToString());
            dictValues.Add("Custom Rotation DPS", CustomRotation.DPS.ToString());

            float chanceWhiteMHNonAvoided = 1f - (AvoidedWhiteMHAttacks / 100f);
            float chanceWhiteOHNonAvoided = 1f - (AvoidedWhiteOHAttacks / 100f);
            float chanceNonAvoided = 1f - (AvoidedAttacks / 100f);
            float chancePoisonNonAvoided = 1f - (AvoidedPoisonAttacks / 100f);
            dictValues.Add("MainHand", MainHandStats.GetStatsTexts(HighestDPSRotation.MainHandCount, 0, HighestDPSRotation.TotalDamage, chanceWhiteMHNonAvoided, Duration));
            dictValues.Add("OffHand", OffHandStats.GetStatsTexts(HighestDPSRotation.OffHandCount, 0, HighestDPSRotation.TotalDamage, chanceWhiteOHNonAvoided, Duration));
            dictValues.Add("Backstab", BackstabStats.GetStatsTexts(HighestDPSRotation.BackstabCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Hemorrhage", HemoStats.GetStatsTexts(HighestDPSRotation.HemoCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Sinister Strike", SStrikeStats.GetStatsTexts(HighestDPSRotation.SStrikeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Mutilate", MutiStats.GetStatsTexts(HighestDPSRotation.MutiCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Revealing Strike", RStrikeStats.GetStatsTexts(HighestDPSRotation.RStrikeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Rupture", RuptStats.GetStatsTexts(HighestDPSRotation.RuptCount, HighestDPSRotation.RuptCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Slice and Dice", SnDStats.GetStatsTexts(HighestDPSRotation.SnDCount, HighestDPSRotation.SnDCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Eviscerate", EvisStats.GetStatsTexts(HighestDPSRotation.EvisCount, Math.Max(HighestDPSRotation.EvisCP, HighestDPSRotation.EnvenomCP), HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Envenom", EnvenomStats.GetStatsTexts(HighestDPSRotation.EnvenomCount, Math.Max(HighestDPSRotation.EvisCP, HighestDPSRotation.EnvenomCP), HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));
            dictValues.Add("Instant Poison", IPStats.GetStatsTexts(HighestDPSRotation.IPCount, 0, HighestDPSRotation.TotalDamage, chancePoisonNonAvoided, Duration));
            dictValues.Add("Deadly Poison", DPStats.GetStatsTexts(HighestDPSRotation.DPCount, 0, HighestDPSRotation.TotalDamage, chancePoisonNonAvoided, Duration));
            dictValues.Add("Wound Poison", WPStats.GetStatsTexts(HighestDPSRotation.WPCount, 0, HighestDPSRotation.TotalDamage, chancePoisonNonAvoided, Duration));
            dictValues.Add("Venomous Wounds", VenomousWoundsStats.GetStatsTexts(HighestDPSRotation.VenomousWoundsCount, 0, HighestDPSRotation.TotalDamage, 1f, Duration));
            dictValues.Add("Main Gauche", MainGaucheStats.GetStatsTexts(HighestDPSRotation.MGCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration));

            return dictValues;
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Avoided Yellow Attacks %": return AvoidedAttacks;
                case "Custom Rotation DPS": return CustomRotation.DPS;
            }
            return 0f;
        }
    }
}
