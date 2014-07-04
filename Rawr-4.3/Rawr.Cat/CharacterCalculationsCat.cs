﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Cat
{
	public class CharacterCalculationsCat : CharacterCalculationsBase
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

		public float AvoidedAttacks { get; set; }
		public float DodgedAttacks { get; set; }
		public float ParriedAttacks { get; set; }
		public float MissedAttacks { get; set; }
		public float CritChance { get; set; }
		public float AttackSpeed { get; set; }
		public float ArmorMitigation { get; set; }
		public float Duration { get; set; }

		public CatAbilityBuilder Abilities;
		public CatRotationCalculation HighestDPSRotation { get; set; }
		//public CatRotationCalculator.CatRotationCalculation CustomRotation { get; set; }

		public string Rotations { get; set; }

		public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
		{
			Dictionary<string, string> dictValues = new Dictionary<string, string>();
			try
			{
				dictValues.Add("Overall Points", OverallPoints.ToString());
				dictValues.Add("DPS Points", DPSPoints.ToString());
				dictValues.Add("Survivability Points", SurvivabilityPoints.ToString());

				float baseMiss = StatConversion.WHITE_MISS_CHANCE_CAP[TargetLevel - 85] - BasicStats.PhysicalHit;
				float baseDodge = StatConversion.WHITE_DODGE_CHANCE_CAP[TargetLevel - 85] - StatConversion.GetDodgeParryReducFromExpertise(BasicStats.Expertise);
				float capMiss = (float)Math.Ceiling(baseMiss * StatConversion.RATING_PER_PHYSICALHIT);
				float capDodge = (float)Math.Ceiling(baseDodge * 400f * StatConversion.RATING_PER_EXPERTISE);
				
				string tipMiss = string.Empty;
				if (BasicStats.HitRating > capMiss)
                    tipMiss = string.Format("*Hit Rating %: {0}%\nOver the cap by {1} Hit Rating", StatConversion.GetPhysicalHitFromRating(BasicStats.HitRating) * 100, BasicStats.HitRating - capMiss);
				else if (BasicStats.HitRating < capMiss)
                    tipMiss = string.Format("*Hit Rating %: {0}%\nUnder the cap by {1} Hit Rating", StatConversion.GetPhysicalHitFromRating(BasicStats.HitRating) * 100, capMiss - BasicStats.HitRating);
				else
                    tipMiss = string.Format("*Hit Rating %: {0}%\nExactly at the cap", StatConversion.GetPhysicalHitFromRating(BasicStats.HitRating) * 100);

				string tipDodge = string.Empty;
				if (BasicStats.ExpertiseRating > capDodge)
                    tipDodge = string.Format("*Expertise Rating %: {0}%\nOver the cap by {1} Expertise Rating", StatConversion.GetExpertiseFromRating(BasicStats.ExpertiseRating) * 0.25f, BasicStats.ExpertiseRating - capDodge);
				else if (BasicStats.ExpertiseRating < capDodge)
                    tipDodge = string.Format("*Expertise Rating %: {0}%\nUnder the cap by {1} Expertise Rating", StatConversion.GetExpertiseFromRating(BasicStats.ExpertiseRating) * 0.25f, capDodge - BasicStats.ExpertiseRating);
				else
                    tipDodge = string.Format("*Expertise Rating %: {0}%\nExactly at the cap", StatConversion.GetExpertiseFromRating(BasicStats.ExpertiseRating) * 0.25f);

                string tipHaste = string.Format("*Haste Rating %: {0}%", StatConversion.GetPhysicalHasteFromRating(BasicStats.HasteRating, CharacterClass.Druid) * 100f);

                string tipMastery = string.Format("*Increases the damage done by your bleed abilities by {0}%", ((StatConversion.GetMasteryFromRating(BasicStats.MasteryRating, CharacterClass.Druid) + 8f) * 0.031f) * 100f);

				dictValues.Add("Health", BasicStats.Health.ToString());
				dictValues.Add("Attack Power", BasicStats.AttackPower.ToString());
				dictValues.Add("Agility", BasicStats.Agility.ToString());
				dictValues.Add("Strength", BasicStats.Strength.ToString());
				dictValues.Add("Crit Rating", BasicStats.CritRating.ToString());
				dictValues.Add("Hit Rating", BasicStats.HitRating.ToString() + tipMiss);
				dictValues.Add("Expertise Rating", BasicStats.ExpertiseRating.ToString() + tipDodge);
				dictValues.Add("Mastery Rating", BasicStats.MasteryRating.ToString() + tipMastery);
                dictValues.Add("Haste Rating", BasicStats.HasteRating.ToString() + tipHaste);
				
				dictValues.Add("Avoided Attacks", string.Format("{0}%*{1}% Dodged, {2}% Missed", AvoidedAttacks, DodgedAttacks, MissedAttacks));
				dictValues.Add("Crit Chance", CritChance.ToString() + "%");
				dictValues.Add("Attack Speed", AttackSpeed.ToString() + "s");
				dictValues.Add("Armor Mitigation", ArmorMitigation.ToString() + "%");

				dictValues.Add("Optimal Rotation", HighestDPSRotation.ToString());
				//dictValues.Add("Optimal Rotation DPS", HighestDPSRotation.DPS.ToString());
				//dictValues.Add("Custom Rotation DPS", CustomRotation.DPS.ToString());


				float chanceNonAvoided = 1f - (AvoidedAttacks / 100f);
				dictValues.Add("Melee", Abilities.MeleeStats.ToString());
				dictValues.Add("Mangle", Abilities.MangleStats.ToString());
				dictValues.Add("Shred", Abilities.ShredStats.ToString());
				dictValues.Add("Ravage", Abilities.RavageStats.ToString());
				dictValues.Add("Rake", Abilities.RakeStats.ToString());
				dictValues.Add("Rip", Abilities.RipStats.ToString());
				dictValues.Add("Bite", Abilities.BiteStats.ToString());


				//string[] abilityStats = MeleeStats.GetStatsTexts(HighestDPSRotation.MeleeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
				//dictValues.Add("Melee Usage", abilityStats[0]);
				//dictValues.Add("Melee Stats", abilityStats[1]);
				//abilityStats = MangleStats.GetStatsTexts(HighestDPSRotation.MangleCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
				//dictValues.Add("Mangle Usage", abilityStats[0]);
				//dictValues.Add("Mangle Stats", abilityStats[1]);
				//abilityStats = ShredStats.GetStatsTexts(HighestDPSRotation.ShredCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
				//dictValues.Add("Shred Usage", abilityStats[0]);
				//dictValues.Add("Shred Stats", abilityStats[1]);
				//abilityStats = RakeStats.GetStatsTexts(HighestDPSRotation.RakeCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
				//dictValues.Add("Rake Usage", abilityStats[0]);
				//dictValues.Add("Rake Stats", abilityStats[1]);
				//abilityStats = RipStats.GetStatsTexts(HighestDPSRotation.RipCount, 0, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
				//dictValues.Add("Rip Usage", abilityStats[0]);
				//dictValues.Add("Rip Stats", abilityStats[1]);
				//abilityStats = RoarStats.GetStatsTexts(HighestDPSRotation.RoarCount, HighestDPSRotation.RoarCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
				//dictValues.Add("Roar Usage", abilityStats[0]);
				//dictValues.Add("Roar Stats", abilityStats[1]);
				//abilityStats = BiteStats.GetStatsTexts(HighestDPSRotation.BiteCount, HighestDPSRotation.BiteCP, HighestDPSRotation.TotalDamage, chanceNonAvoided, Duration);
				//dictValues.Add("Bite Usage", abilityStats[0]);
				//dictValues.Add("Bite Stats", abilityStats[1]);

				//string attackFormat = "{0}%*Damage Per Hit: {1}, Damage Per Swing: {2}\r\n{0}% of Total Damage, {3} Damage Done";
				//dictValues.Add("Melee Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.MeleeDamageTotal / HighestDPSRotation.DamageTotal, MeleeDamagePerHit, MeleeDamagePerSwing, HighestDPSRotation.MeleeDamageTotal));
				//dictValues.Add("Mangle Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.MangleDamageTotal / HighestDPSRotation.DamageTotal, MangleDamagePerHit, MangleDamagePerSwing, HighestDPSRotation.MangleDamageTotal));
				//dictValues.Add("Shred Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.ShredDamageTotal / HighestDPSRotation.DamageTotal, ShredDamagePerHit, ShredDamagePerSwing, HighestDPSRotation.ShredDamageTotal));
				//dictValues.Add("Rake Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.RakeDamageTotal / HighestDPSRotation.DamageTotal, RakeDamagePerHit, RakeDamagePerSwing, HighestDPSRotation.RakeDamageTotal));
				//dictValues.Add("Rip Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.RipDamageTotal / HighestDPSRotation.DamageTotal, RipDamagePerHit, RipDamagePerSwing, HighestDPSRotation.RipDamageTotal));
				//dictValues.Add("Bite Damage", string.Empty);//.Format(attackFormat, 100f * HighestDPSRotation.BiteDamageTotal / HighestDPSRotation.DamageTotal, BiteDamagePerHit, BiteDamagePerSwing, HighestDPSRotation.BiteDamageTotal));

				//string rotationDescription = string.Empty;
				//try
				//{
				//    rotationDescription = string.Format("{0}*Keep {1}cp Savage Roar up.\r\n{2}{3}{4}{5}Use {6} for combo points.",
				//        HighestDPSRotation.Name.Replace(" + ", "+"), HighestDPSRotation.RoarCP,
				//        HighestDPSRotation.Name.Contains("Rake") ? "Keep Rake up.\r\n" : "",
				//        HighestDPSRotation.Name.Contains("Rip") ? "Keep 5cp Rip up.\r\n" : "",
				//        HighestDPSRotation.Name.Contains("Mangle") ? "Keep Mangle up.\r\n" : "",
				//        HighestDPSRotation.Name.Contains("Bite") ? string.Format("Use {0}cp Ferocious Bites to spend extra combo points.\r\n", HighestDPSRotation.BiteCP) : "",
				//        HighestDPSRotation.Name.Contains("Shred") ? "Shred" : "Mangle");
				//}
				//catch (Exception ex)
				//{
				//    ex.ToString();
				//}

			}
			catch (Exception ex)
			{
                new Base.ErrorBox()
                {
                    Title = "Error Getting Cat Dictionary Values",
                    Function = "GetCharacterDisplayCalculationValues()",
                    TheException = ex,
                }.Show();
			}
			return dictValues;
		}

		public override float GetOptimizableCalculationValue(string calculation)
		{
			switch (calculation)
			{
				case "Health": return BasicStats.Health;
				case "Avoided Attacks %": return AvoidedAttacks;
                case "Hit Rating": return BasicStats.HitRating;
                case "Hit Rating %": return StatConversion.GetPhysicalHitFromRating(BasicStats.HitRating) * 100;
                case "Expertise Rating": return BasicStats.ExpertiseRating;
                case "Expertise": return StatConversion.GetExpertiseFromRating(BasicStats.ExpertiseRating);
                case "Expertise Rating %": return StatConversion.GetExpertiseFromRating(BasicStats.ExpertiseRating) * 0.25f;
				case "Nature Resist": return BasicStats.NatureResistance;
				case "Fire Resist": return BasicStats.FireResistance;
				case "Frost Resist": return BasicStats.FrostResistance;
				case "Shadow Resist": return BasicStats.ShadowResistance;
				case "Arcane Resist": return BasicStats.ArcaneResistance;
				//case "Custom Rotation DPS": return CustomRotation.DPS;
			}
			return 0f;
		}
	}
}
