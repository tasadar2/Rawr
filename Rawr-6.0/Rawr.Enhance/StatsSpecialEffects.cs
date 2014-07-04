﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Enhance
{
    class StatsSpecialEffects
    {
        private Character _character;
        private Stats _stats;
        private CombatStats _cs;
        float interval = 0f;
        float chance = 1f;
        float unhastedAttackSpeed = 3f;
        SpecialEffect mainHandEnchant = null;
        SpecialEffect offHandEnchant = null;
        bool mhProcessed = false;
        bool ohProcessed = false;

        public StatsSpecialEffects(Character character, Stats stats, CalculationOptionsEnhance calcOpts, BossOptions bossOpts)
        {
            _character = character;
            _stats = stats;
            _cs = new CombatStats(_character, _stats, calcOpts, bossOpts);

            if (character.MainHandEnchant != null)
            { 
                Stats.SpecialEffectEnumerator mhEffects = character.MainHandEnchant.Stats.SpecialEffects();
                if (mhEffects.MoveNext()) { mainHandEnchant = mhEffects.Current; }
            }
            if (character.OffHandEnchant != null)
            {
                Stats.SpecialEffectEnumerator ohEffects = character.OffHandEnchant.Stats.SpecialEffects();
                if (ohEffects.MoveNext()) { offHandEnchant = ohEffects.Current; }
            }
        }

        public StatsEnhance getSpecialEffects()
        {
            StatsEnhance statsAverage = new StatsEnhance();
            foreach (SpecialEffect effect in _stats.SpecialEffects())
            {
                statsAverage.Accumulate(getSpecialEffects(effect));
            }
            AddParagon(statsAverage);
            AddHighestStat(statsAverage);
            AddHighestSecondaryStat(statsAverage);
            return statsAverage;
        }

        public StatsEnhance getSpecialEffects(SpecialEffect effect)
        {
            StatsEnhance statsAverage = new StatsEnhance();
            if (effect == mainHandEnchant || effect == offHandEnchant)
            {
                if (mainHandEnchant != null && !mhProcessed)
                {
                    statsAverage.Accumulate(mainHandEnchant.Stats, GetMHUptime());
                    mhProcessed = true; 
                }
                else if (offHandEnchant != null && !ohProcessed)
                {
                    statsAverage.Accumulate(offHandEnchant.Stats, GetOHUptime());
                    ohProcessed = true; 
                }
            }
            else if (effect.Trigger == Trigger.Use)
            {
                effect.AccumulateAverageStats(statsAverage);
                foreach (SpecialEffect e in effect.Stats.SpecialEffects())
                    statsAverage.Accumulate(this.getSpecialEffects(e) * (effect.Duration / effect.Cooldown));
            }
            /*else if (effect.AttackPowerScaling > 0)
            {
                statsAverage.PhysicalDamageProcs = effect.AttackPowerScaling;
            }*/
            else
            {
                SetTriggerChanceAndSpeed(effect);
                foreach (SpecialEffect e in effect.Stats.SpecialEffects())  // deal with secondary effects
                {
                    statsAverage.Accumulate(this.getSpecialEffects(e));
                }
                if (effect.MaxStack > 1)
                {
                    if (effect.Stats.MoteOfAnger > 0)
                    {
                        // When in effect stats, MoteOfAnger is % of melee hits
                        // When in character stats, MoteOfAnger is average procs per second
                        statsAverage.Accumulate(new Stats() { MoteOfAnger = effect.Stats.MoteOfAnger * effect.GetAverageProcsPerSecond(interval, chance, unhastedAttackSpeed, 1f, 0f) / effect.MaxStack }); // FIXME: Pass haste for Real PPM effects
                    }
                    else
                    {
                        float timeToMax = (float)Math.Min(_cs.FightLength, effect.GetChance(unhastedAttackSpeed, 1f, 1f) * interval * effect.MaxStack); // FIXME: Pass trigger interval and haste for Real PPM effects
                        float buffDuration = _cs.FightLength;
                        if (effect.Stats.AttackPower == 250f || effect.Stats.AttackPower == 215f || effect.Stats.HasteRating == 57f || effect.Stats.HasteRating == 64f)
                        {
                            buffDuration = 20f;
                        }
                        if (timeToMax * .5f > buffDuration)
                        {
                            timeToMax = 2 * buffDuration;
                        }
                        statsAverage.Accumulate(effect.Stats * (effect.MaxStack * (((buffDuration) - .5f * timeToMax) / (buffDuration))));
                    }
                }
                else
                {
                    effect.AccumulateAverageStats(statsAverage, interval, chance, unhastedAttackSpeed, 1f);
                }
            }
            return statsAverage;
        }

        private void SetTriggerChanceAndSpeed(SpecialEffect effect)
        {
            interval = 0f;
            chance = 1f;
            unhastedAttackSpeed = 3f;
            switch (effect.Trigger)
            {
                case Trigger.DamageDone:
                    interval = (_cs.HastedMHSpeed + 1f / _cs.GetSpellAttacksPerSec()) / 2f;
                    chance = (float)Math.Min(1.0f, _cs.AverageWhiteHitChance + _cs.ChanceSpellHit); // limit to 100% chance
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.DamageOrHealingDone:
                    // Need to add Self Heals
                    interval = (_cs.HastedMHSpeed + 1f / _cs.GetSpellAttacksPerSec()) / 2f;
                    chance = (float)Math.Min(1.0f, _cs.AverageWhiteHitChance + _cs.ChanceSpellHit); // limit to 100% chance
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.MeleeCrit:
                case Trigger.PhysicalCrit:
                    interval = _cs.HastedMHSpeed;
                    chance = _cs.AverageWhiteCritChance;
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.MeleeHit:
                    interval = _cs.HastedMHSpeed;
                    chance = _cs.AverageWhiteHitChance;
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.PhysicalHit:
                    interval = _cs.HastedMHSpeed;
                    chance = _cs.AverageWhiteHitChance;
                    unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    break;
                case Trigger.PhysicalAttack:
                case Trigger.MeleeAttack:
                    if (_cs.UnhastedOHSpeed != 0) {
                        interval = (_cs.HastedMHSpeed + _cs.HastedOHSpeed) / 2;
                        chance = 1f;
                        unhastedAttackSpeed = (_cs.UnhastedMHSpeed + _cs.UnhastedOHSpeed) / 2;
                    } else {
                        interval = _cs.HastedMHSpeed;
                        chance = 1f;
                        unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    }
                    break;
                case Trigger.CurrentHandHit:
                    // Need to fix this, for now set the same as MeleeAttack
                    if (_cs.UnhastedOHSpeed != 0)
                    {
                        interval = (_cs.HastedMHSpeed + _cs.HastedOHSpeed) / 2;
                        chance = 1f;
                        unhastedAttackSpeed = (_cs.UnhastedMHSpeed + _cs.UnhastedOHSpeed) / 2;
                    }
                    else
                    {
                        interval = _cs.HastedMHSpeed;
                        chance = 1f;
                        unhastedAttackSpeed = _cs.UnhastedMHSpeed;
                    }
                    break;
                case Trigger.DamageSpellCast:
                case Trigger.SpellCast:
                    interval = 1f / _cs.GetSpellCastsPerSec();
                    chance = 1f;
                    break;
                case Trigger.DamageSpellHit:
                case Trigger.SpellHit:
                    interval = 1f / _cs.GetSpellAttacksPerSec();
                    chance = _cs.ChanceSpellHit;
                    break;
                case Trigger.DamageSpellCrit:
                case Trigger.SpellCrit:
                    interval = 1f / _cs.GetSpellCritsPerSec();
                    chance = _cs.ChanceSpellCrit;
                    break;
                case Trigger.SpellMiss:
                    interval = 1f / _cs.GetSpellMissesPerSec();
                    chance = 1 - _cs.ChanceSpellHit;
                    break;
                case Trigger.ShamanLightningBolt:
                    interval = 1f / _cs.AbilityCooldown(EnhanceAbility.LightningBolt);
                    chance = _cs.ChanceSpellHit;
                    break;
                case Trigger.ShamanStormStrike:
                    interval = 1f / _cs.AbilityCooldown(EnhanceAbility.StormStrike);
                    chance = _cs.ChanceYellowHitMH;
                    break;
                case Trigger.ShamanShock:
                    interval = 1f / _cs.AbilityCooldown(EnhanceAbility.EarthShock);
                    chance = _cs.ChanceSpellHit;
                    break;
                case Trigger.ShamanLavaLash:
                    interval = 1f / _cs.AbilityCooldown(EnhanceAbility.LavaLash);
                    chance = _cs.ChanceYellowHitOH;
                    unhastedAttackSpeed = _cs.UnhastedOHSpeed;
                    break;
                case Trigger.ShamanShamanisticRage:
                    interval = 1f / _cs.AbilityCooldown(EnhanceAbility.ShamanisticRage);
                    chance = 1f;
                    break;
                case Trigger.ShamanFlameShockDoTTick:
                case Trigger.DoTTick:
                    interval = 1f / _cs.AbilityCooldown(EnhanceAbility.FlameShock);
                    chance = 1f;
                    break;
            }
        }

        public float GetUptime(ItemInstance item)
        {
            float uptime = 0;
            if (item != null)
                foreach (SpecialEffect effect in item.GetTotalStats().SpecialEffects())
                {
                    SetTriggerChanceAndSpeed(effect);
                    uptime = effect.GetAverageUptime(interval, chance, unhastedAttackSpeed, 1f); // FIXME: Pass haste for Real PPM effects
                }
            return uptime;
        }

        public float GetMHUptime()
        {
            if (mainHandEnchant != null && mainHandEnchant.Trigger == Trigger.SpellHit)
                return mainHandEnchant.GetAverageUptime(1f / _cs.GetSpellAttacksPerSec(), _cs.ChanceSpellHit, _cs.UnhastedMHSpeed, 1f, _cs.FightLength);
            return mainHandEnchant == null ? 0f : mainHandEnchant.GetAverageUptime(_cs.HastedMHSpeed, _cs.ChanceWhiteHitMH, _cs.UnhastedMHSpeed, 1f, _cs.FightLength);
        }

        public float GetOHUptime()
        {
            if (offHandEnchant != null && offHandEnchant.Trigger == Trigger.SpellHit)
                return offHandEnchant.GetAverageUptime(1f / _cs.GetSpellAttacksPerSec(), _cs.ChanceSpellHit, _cs.UnhastedOHSpeed, 1f, _cs.FightLength);
            return offHandEnchant == null ? 0f : offHandEnchant.GetAverageUptime(_cs.HastedOHSpeed, _cs.ChanceWhiteHitOH, _cs.UnhastedOHSpeed, 1f, _cs.FightLength);
        }

        // Handling for Paragon trinket procs
        private void AddParagon(Stats statsAverage)
        {
            if (statsAverage.Paragon > 0)
            {
                float paragon = 0f;
                if (_stats.Strength > _stats.Agility)
                {
                    paragon = statsAverage.Paragon * (1 + _stats.BonusStrengthMultiplier);
                    statsAverage.Strength += paragon;
                    statsAverage.AttackPower += paragon;
                }
                else
                {
                    paragon = statsAverage.Paragon * (1 + _stats.BonusAgilityMultiplier);
                    statsAverage.Agility += paragon;
                    statsAverage.AttackPower += paragon;
                }
            }
        }

        private void AddHighestStat(Stats statsAverage)
        {
            //trinket procs
            if (statsAverage.HighestStat > 0)
            {
                // Highest stat
                float highestStat = 0f;
                if (_stats.Agility > _stats.Strength)
                {
                    if (_stats.Agility > _stats.Intellect)
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusAgilityMultiplier);
                        statsAverage.Agility += highestStat;
                        statsAverage.AttackPower += highestStat;
                    }
                    else
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusIntellectMultiplier);
                        statsAverage.Intellect += highestStat;
                        statsAverage.SpellPower += highestStat;
                    }
                }
                else
                {
                    if (_stats.Strength > _stats.Intellect)
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusStrengthMultiplier);
                        statsAverage.Strength += highestStat;
                        statsAverage.AttackPower += highestStat;
                    }
                    else
                    {
                        highestStat = statsAverage.HighestStat * (1 + _stats.BonusIntellectMultiplier);
                        statsAverage.Intellect += highestStat;
                        statsAverage.SpellPower += highestStat;
                    }
                }
            }
        }

        private void AddHighestSecondaryStat(Stats statsAverage)
        {
            if (statsAverage.HighestSecondaryStat > 0)
            {
                float highestSecondaryStat = 0f;
                if (_stats.CritRating > _stats.HasteRating)
                {
                    if (_stats.CritRating > _stats.MasteryRating)
                    {
                        highestSecondaryStat = statsAverage.HighestSecondaryStat;
                        statsAverage.CritRating += highestSecondaryStat;
                    }
                    else
                    {
                        highestSecondaryStat = statsAverage.HighestSecondaryStat;
                        statsAverage.MasteryRating += highestSecondaryStat;
                    }
                }
                else
                {
                    if (_stats.HasteRating > _stats.MasteryRating)
                    {
                        highestSecondaryStat = statsAverage.HighestSecondaryStat;
                        statsAverage.HasteRating += highestSecondaryStat;
                    }
                    else
                    {
                        highestSecondaryStat = statsAverage.HighestSecondaryStat;
                        statsAverage.MasteryRating += highestSecondaryStat;
                    }
                }
            }
        }
    }
}



