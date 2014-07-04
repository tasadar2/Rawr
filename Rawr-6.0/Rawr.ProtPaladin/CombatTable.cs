﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtPaladin
{
    public abstract class CombatTable
    {
        protected Character Character;
        protected CalculationOptionsProtPaladin CalcOpts;
        protected BossOptions BossOpts;
        protected Stats Stats;
        protected Ability Ability;//TODO: expand the Ability Class to include DamageType(School) and AttackType

        public float Miss { get; protected set; }
        public float Dodge { get; protected set; }
        public float Parry { get; protected set; }
        public float Block { get; protected set; }
        public float Glance { get; protected set; }
        public float Critical { get; protected set; }
        public float Hit { get; protected set; }
        public float BlockOverCap { get; protected set; }
        public float EffectiveBlock { get; protected set; }

        public float AnyHit
        {
            get { return (1.0f - (Miss + Dodge + Parry)); }
        }

        public float AnyMiss
        {
            get { return (Miss + Dodge + Parry); }
        }

        protected virtual void Calculate() {}

        protected void Initialize(Character character, Stats stats, Ability ability, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts)
        {
            Character   = character;
            CalcOpts    = calcOpts;
            BossOpts    = bossOpts;
            Stats       = stats;
            Ability     = ability;
            Calculate();
        }
    }

    public class DefendTable : CombatTable
    {
        protected override void Calculate()
        {
            float tableSize = 0.0f;
            const float CTCap = 1.0f;   // Combat table size is 102.4% against a level+3 mob, but these adjustments are factored into the Lookup.AvoidanceChance functions

            int targetLevel = BossOpts.Level;

            // Miss
            Miss = Math.Min(CTCap - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Miss, targetLevel));
            tableSize += Miss;
            // Dodge
            Dodge = Math.Min(CTCap - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Dodge, targetLevel));
            tableSize += Dodge;
            // Parry
            Parry = Math.Min(CTCap - tableSize, Lookup.AvoidanceChance(Character, Stats, HitResult.Parry, targetLevel));
            tableSize += Parry;
            // Critical Hit
            Critical = Math.Min(CTCap - tableSize, Lookup.TargetCritChance(Character, Stats, targetLevel));
            tableSize += Critical;
            // Normal Hit and Block are now on a two-roll system
            // Block
            if (Character.OffHand != null && Character.OffHand.Type == ItemType.Shield)
            {
                Block = Lookup.AvoidanceChance(Character, Stats, HitResult.Block, targetLevel);
                if (Block > 1)
                {
                    BlockOverCap = Block - 1;
                }
                else
                {
                    BlockOverCap = 0.0f;
                }
                // Effective block is a multiplier on the remaining combat table size beyond Critical
                EffectiveBlock = (CTCap - tableSize) * (Block - BlockOverCap);
                tableSize += EffectiveBlock;
            }
            // Hit
            Hit = Math.Max(0.0f, CTCap - tableSize);
        }

        public DefendTable(Character character, Stats stats, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts)
        {
            Initialize(character, stats, Ability.MeleeSwing, calcOpts, bossOpts);
        }
    }

    public class AttackTable : CombatTable
    {
        protected override void Calculate()
        {
            float tableSize = 0.0f;

            int targetLevel = BossOpts.Level;

            float SpellHitChance = Lookup.SpellHitChance(Character.Level, Stats, targetLevel);
            float bonusExpertise = Lookup.BonusExpertisePercentage(Stats);

            if (Lookup.IsSpell(Ability))
            {
                // Miss
                Miss = Math.Min(1.0f - tableSize, 1.0f - SpellHitChance);
                tableSize += Miss;
                // Crit
                Critical = Lookup.SpellCritChance(Character.Level, Stats, targetLevel);
                tableSize += Critical;
            }
            else
            {
                float bonusHit = Lookup.HitChance(Stats, targetLevel, Character.Level);
                
                // Miss
                // Miss = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character, Stats, HitResult.Miss) - bonusHit));
                Miss = Math.Min(1.0f - tableSize, Math.Max(0.0f, 1.0f - bonusHit));
                tableSize += Miss;
                // Avoidance
                if (Lookup.IsAvoidable(Ability))
                {
                    // Dodge
                    Dodge = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character.Level, HitResult.Dodge, targetLevel) - bonusExpertise));
                    tableSize += Dodge;
                    // Parry
                    Parry = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character.Level, HitResult.Parry, targetLevel) - bonusExpertise));
                    tableSize += Parry;
                }
                // Glancing Blow
                if (Ability == Ability.MeleeSwing)
                {
                    Glance = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character.Level, HitResult.Glance, targetLevel)));
                    tableSize += Glance;
                }
                // Block
                if (Ability == Ability.MeleeSwing || Ability == Ability.HammerOfTheRighteous)
                {
                    Block = Math.Min(1.0f - tableSize, Math.Max(0.0f, Lookup.TargetAvoidanceChance(Character.Level, HitResult.Block, targetLevel)));
                    tableSize += Block;
                }
                // Critical Hit
                Critical = Math.Min(1.0f - tableSize, Lookup.BonusCritPercentage(Character, Stats, Ability, targetLevel));
                if (Critical > 0)
                {
                    tableSize += Critical;
                }
            }

            // Normal Hit
            Hit = Math.Max(0.0f, 1.0f - tableSize);
        }

        public AttackTable(Character character, Stats stats, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts)
        {
            Initialize(character, stats, Ability.MeleeSwing, calcOpts, bossOpts);
        }

        public AttackTable(Character character, Stats stats, Ability ability, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts)
        {
            Initialize(character, stats, ability, calcOpts, bossOpts);
        }
    }
}
