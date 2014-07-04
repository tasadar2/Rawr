﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Cat
{
	public class StatsCat : Stats
	{
		public float NonShredBleedDamageMultiplier { get; set; }
		public float MangleDamageMultiplier { get; set; }
		public float ShredDamageMultiplier { get; set; }
		public float FerociousBiteDamageMultiplier { get; set; }
		public float SavageRoarDamageMultiplierIncrease { get; set; }
		public float EnergyOnTigersFury { get; set; }
		public float MaxEnergyOnTigersFuryBerserk { get; set; }
		public float RavageCritChanceOnTargetsAbove80Percent { get; set; }
		public float FurySwipesChance { get; set; }
		public float BonusBerserkDuration { get; set; }
		public float TigersFuryCooldownReduction { get; set; }
		public float FeralChargeCatCooldownReduction { get; set; }
		public float CPOnCrit { get; set; }
		public float FerociousBiteMaxExtraEnergyReduction { get; set; }
		public float FreeRavageOnFeralChargeChance { get; set; }
		public float RipRefreshChanceOnFerociousBiteOnTargetsBelow25Percent { get; set; }
        public bool Tier_11_4pc { get; set; }
        public bool Tier_12_4pc { get; set; }
        public bool Tier_13_2_piece { get; set; }
        public bool Tier_13_4_piece { get; set; }
        public float PhysicalDamageProcs { get; set; }
		
		public WeightedStat[] TemporaryCritRatingUptimes { get; set; }
	}
}
