﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.ProtPaladin
{
    public static class Lookup
    {
        public static float TargetCritChance(Character character, Stats stats, int targetLevel)
        {
            return 0f;
        }

        public static float TargetAvoidanceChance(int attackerLevel, HitResult avoidanceType, int targetLevel)
        {
            switch (avoidanceType)
            {

                case HitResult.Miss:   return StatConversion.WHITE_MISS_CHANCE_CAP[  targetLevel - attackerLevel];
                case HitResult.Dodge : return StatConversion.WHITE_DODGE_CHANCE_CAP[ targetLevel - attackerLevel];
                case HitResult.Parry : return StatConversion.WHITE_PARRY_CHANCE_CAP[ targetLevel - attackerLevel];
                case HitResult.Glance: return StatConversion.WHITE_GLANCE_CHANCE_CAP[targetLevel - attackerLevel];
                case HitResult.Block : return StatConversion.WHITE_BLOCK_CHANCE_CAP[ targetLevel - attackerLevel];
                default: return 0.0f;
            }
        }

        public static float DamageReduction(Stats stats) {
            return (1f - stats.DamageTakenReductionMultiplier) * (1f - stats.BossPhysicalDamageDealtReductionMultiplier) * (1f - stats.PhysicalDamageTakenReductionMultiplier);
        }

        public static float BonusExpertisePercentage(Stats stats) {
            return StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Paladin) + stats.Expertise, CharacterClass.Paladin);
        }
        
        public static float BonusPhysicalHastePercentage(Stats stats) {
            return StatConversion.GetHasteFromRating(stats.HasteRating, CharacterClass.Paladin) + stats.PhysicalHaste;
        }

        public static float BonusSpellHastePercentage(Stats stats) {
            return StatConversion.GetSpellHasteFromRating(stats.HasteRating, CharacterClass.Paladin) + stats.SpellHaste;
        }

        public static float HitChance(Stats stats, int targetLevel, int attackerLevel) {
            float physicalHit = StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Paladin) + stats.PhysicalHit;
            return Math.Min(1f, (1f - StatConversion.WHITE_MISS_CHANCE_CAP[targetLevel - attackerLevel]) + physicalHit);
        }

        public static float SpellHitChance(int attackerLevel, Stats stats, int targetLevel) {
            float spellHit = StatConversion.GetSpellHitFromRating(stats.HitRating, CharacterClass.Paladin) + stats.SpellHit;

            int DeltaLevel = targetLevel - attackerLevel;

            return Math.Min(1f, (1 - StatConversion.GetSpellMiss(DeltaLevel, false)) + spellHit);
        }

        public static float CritChance(Stats stats, int targetLevel, int attackerLevel) {
            return Math.Max(0f, Math.Min(1f, StatConversion.GetCritFromRating(stats.CritRating, CharacterClass.Paladin)
                                             + StatConversion.GetCritFromAgility(stats.Agility, CharacterClass.Paladin)
                                             + StatConversion.NPC_LEVEL_CRIT_MOD[targetLevel - attackerLevel]
                                             + stats.PhysicalCrit));
        }

        /// <summary>
        /// Unlike the melee combat system, spell crit makes absolutely no difference to hit chance. 
        /// All spells, regardless of whether they are treated as binary or not, roll hit and crit separately. 
        /// Conceptually, the game rolls for your hit chance first, and if the spell hits you have a separate roll for whether it crits. 
        /// Overall chance to crit over all spells cast is thus affected by hit rate. 
        /// To calculate overall crit rate, multiplying the two chances together: 
        /// Crit rate over all spell casts = crit/// hit
        /// 
        /// For example, a caster with no spell hit rating gear or talents, 
        /// against a mob 3 levels higher (83% hit chance), and 30% crit rating from gear and talents: 
        /// crit rate over all spell casts = 30%/// 83% = 24.9%
        /// </summary>
        /// <param name="character"></param>
        /// <param name="stats"></param>
        /// <returns></returns>
        public static float SpellCritChance(int attackerLevel, Stats stats, int targetLevel) {
            float spellCrit = Math.Min(1f, StatConversion.GetSpellCritFromRating(stats.CritRating, CharacterClass.Paladin) + StatConversion.GetSpellCritFromIntellect(stats.Intellect, CharacterClass.Paladin) + stats.SpellCrit + stats.SpellCritOnTarget);

            return spellCrit * SpellHitChance(attackerLevel, stats, targetLevel);
        }
        
        public static float BonusCritPercentage(Character character, Stats stats, Ability ability, int targetLevel)
        {
            float abilityCritChance = CritChance(stats, targetLevel, character.Level);
            float spellCritChance = SpellCritChance(character.Level, stats, targetLevel);
            
            switch (ability)
            {
                case Ability.SealOfRighteousness:
                case Ability.MeleeSwing:
                case Ability.SealOfTruth:
                case Ability.CensureTick:
                case Ability.CrusaderStrike:
                case Ability.HammerOfTheRighteous:
                case Ability.ShieldOfTheRighteous:
                    // crit chance = melee
                    break;
                case Ability.HammerOfTheRighteousProc:
                case Ability.HammerOfWrath:
                case Ability.Consecration:
                case Ability.HolyWrath:
                case Ability.AvengersShield:
                case Ability.Judgment:
                    abilityCritChance = spellCritChance;
                    break;
            }
            return Math.Min(1.0f, abilityCritChance);
        }

        public static float WeaponDamage(Character character, float attackPower, bool normalized)
        {
            float weaponSpeed     = 0.0f;
            float weaponMinDamage = 0.0f;
            float weaponMaxDamage = 0.0f;
            
            if (character.MainHand == null) // unarmed
            {
                weaponSpeed     = 2.0f;
                weaponMinDamage = 1.0f;
                weaponMaxDamage = 2.0f;
            }
            else
            {
                weaponSpeed     = character.MainHand.Speed;
                weaponMinDamage = character.MainHand.MinDamage;
                weaponMaxDamage = character.MainHand.MaxDamage;
            }
            // Non-Normalized Hits
            if (!normalized)
                return ((weaponMinDamage + weaponMaxDamage) / 2.0f + (weaponSpeed * attackPower / 14.0f));
            // Normalized Hits
            // Protection paladins currently do not have normalized instant attacks.
            else
                return ((weaponMinDamage + weaponMaxDamage) / 2.0f + (2.4f * attackPower / 14.0f));
        }

        public static float WeaponSpeed(Character character, Stats stats) {
            if (character.MainHand != null)
                return Math.Max(1.0f, character.MainHand.Speed / (1.0f + BonusPhysicalHastePercentage(stats)));
            else
                return Math.Max(1.0f, 2.0f / (1.0f + BonusPhysicalHastePercentage(stats)));
        }

        public static float GlancingReduction(int attackerLevel, int targetLevel)
        {
            // The character is a melee class, lowEnd is element of [0.01, 0.91]
            float lowEnd = Math.Max(0.01f, Math.Min(0.91f, 1.3f - (0.05f * (float)(targetLevel - attackerLevel) * 5.0f)));
            // The character is a melee class, highEnd is element of [0.20, 0.99]
            float highEnd = Math.Max(0.20f, Math.Min(0.99f, 1.2f - (0.03f * (float)(targetLevel - attackerLevel) * 5.0f)));
            
            return (lowEnd + highEnd) / 2.0f;
        }

        public static float ArmorReduction(float armor, int targetLevel, int playerLevel) // incoming damage
        {
            return StatConversion.GetArmorDamageReduction(targetLevel, playerLevel, armor, 0, 0);
        }

        public static float ActiveBlockReduction(float bonusBlockValueMultiplier)
        {
            return 0.3f + bonusBlockValueMultiplier;
        }

        public static float AvoidanceChance(Character character, Stats stats, HitResult avoidanceType, int targetLevel)
        {
            float avoidanceChance = StatConversion.GetDRAvoidanceChance(character, stats, avoidanceType, targetLevel);

            return avoidanceChance;
        }

        // Combination nCk
        public static float NComb(float n, float k)
        {
            float result = 1;

            for (float i = Math.Max(k,n-k) + 1; i <= n; ++i)
                result *= i;

            for (float i = 2; i <= Math.Min(k,n-k); ++i)
                result /= i;

            return result;
        }

        public static float GetConsecrationTickChances(float Ticks, float TickDamage, float Miss)
        {                                      // 10+2 ticks
            float[] ConsecrationTable = new float[12];// Debug Array
            float p = 1.0f - Miss;
            int n = (int)Math.Floor(Ticks); // Number of possible ticks, backwards compatible to Ticks as time.
            int k;
            float Damage = 0.0f;

            for (k = n-1; k > -1 ; k--)
            {   // The probability P(X=k) that k out of n possible ticks hit :
                float ProbabilityOfTicks = NComb(n, k) * (float)Math.Pow(p, k) * (float)Math.Pow((1 - p), (n-k));
                // The damage those ticks deal
                Damage += TickDamage * ProbabilityOfTicks * k;
                
                ConsecrationTable[k] += ProbabilityOfTicks;// Debug Array
            }
            // Total average damage over all probabilities
            return Damage;
        }

        public static bool IsAvoidable(Ability ability)
        {
            if (ability == Ability.CensureTick)
                return false;
            return true;
        }

        public static bool CanCrit(Ability ability)
        {   
            return true;
        }

        public static bool IsSpell(Ability ability)
        {   
            switch (ability)
            {
                case Ability.AvengersShield:
                case Ability.HolyWrath:
                case Ability.HammerOfTheRighteousProc:
                case Ability.CensureTick:
                case Ability.Consecration:
                case Ability.Judgment:
                case Ability.HammerOfWrath:
                    return true;
                default:
                    return false;
            }
        }

        public static string Name(Ability ability)
        {
            switch (ability)
            {
                case Ability.AvengersShield: return "Avenger's Shield";
                case Ability.HammerOfWrath: return "Hammer of Wrath";
                case Ability.HolyWrath: return "Holy Wrath";

                case Ability.CrusaderStrike: return "Crusader Strike";
                case Ability.HammerOfTheRighteous: return "Hammer of the Righteous";
                case Ability.HammerOfTheRighteousProc: return "Hammer of the Righteous AoE Proc";
                case Ability.Judgment: return "Judgment";
                case Ability.MeleeSwing: return "Melee Swing";
                case Ability.SealOfRighteousness: return "Seal of Righteousness";
                case Ability.SealOfTruth: return "Seal of Truth";
                case Ability.ShieldOfTheRighteous: return "Shield of the Righteous";
                
                case Ability.CensureTick: return "Censure";
                case Ability.Consecration: return "Consecration";
                default: return "";
            }
        }
    }
}
