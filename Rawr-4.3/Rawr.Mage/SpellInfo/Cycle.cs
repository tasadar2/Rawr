﻿using System;
using System.Collections.Generic;
#if RAWR3 || RAWR4
using System.Linq;
#endif
using System.Text;

namespace Rawr.Mage
{
    public enum CycleId
    {
        None,
        ArcaneMissiles,
        Scorch,
        Frostbolt,
        Fireball,
        FrostfireBolt,
        ArcaneManaNeutral,
        ArcaneBlastSpam,
        ABSpam234AM,
        ABSpam34AM,
        ABSpam4AM,
        AB3ABar023AM,
        AB23ABar023AM,
        AB2ABar02AMABABar,
        AB2ABar12AMABABar,
        ABarAM,
        ABP,
        ABAM,
        ABABar,
        ABABar3C,
        ABABar2C,
        ABABar2MBAM,
        ABABar1MBAM,
        ABABar0MBAM,
        ABSpamMBAM,
        ABSpam03C,
        ABSpam3C,
        ABSpam03MBAM,
        ABSpam3MBAM,
        ABAMABar,
        AB2AMABar,
        AB32AMABar,
        AB3ABar3MBAM,
        AB2AM,
        AB3AM,
        ABABar1AM,
        AB3AM23MBAM,
        AB3ABar123AM,
        AB4ABar1234AM,
        AB4ABar34AM,
        AB4ABar4AM,
        AB4AM234MBAM,
        AB3AM023MBAM,
        AB4AM0234MBAM,
        ABSpam04MBAM,
        ABSpam024MBAM,
        ABSpam034MBAM,
        ABSpam0234MBAM,
        ABSpam0234AMABar,
        ABSpam0234AMABABar,
        AB2ABar2AMABar0AMABABar,
        ABSpam4MBAM,
        ABSpam24MBAM,
        ABSpam234MBAM,
        AB3AM2MBAM,
        ABABar0C,
        ABABar1C,
        ABABarY,
        AB2ABar,
        AB2ABarMBAM,
        AB2ABar2C,
        AB2ABar2MBAM,
        AB2ABar3C,
        AB3AMABar,
        AB3AMABar2C,
        AB3ABar,
        AB3ABar3C,
        AB3ABarX,
        AB3ABarY,
        FBABar,
        FrBABar,
        FFBABar,
        //ABAMP,
        //AB3AMSc,
        //ABAM3Sc,
        //ABAM3Sc2,
        //ABAM3FrB,
        //ABAM3FrB2,
        //ABFrB,
        //AB3FrB,
        //ABFrB3FrB,
        //ABFrB3FrB2,
        //ABFrB3FrBSc,
        //ABFB3FBSc,
        //AB3Sc,
        FB2ABar,
        FrB2ABar,
        ScLBPyro,
        FrBFB,
        FrBIL,
        FrBILFB,
        FrBDFFBIL,
        FrBDFFFBIL,
        FrBDFFFB,
        FrBFBIL,
        FrBFFB,
        FBSc,
        FBFBlast,
        FBPyro,
        FBLBPyro,
        FBLB3Pyro,
        FFBLBPyro,
        FFBLB3Pyro,
        FFBPyro,
        FBScPyro,
        FFBScPyro,
        FBScLBPyro,
        FFBScLBPyro,
        ABABarSlow,
        FBABarSlow,
        FrBABarSlow,
        /*ABAM3ScCCAM,
        ABAM3Sc2CCAM,
        ABAM3FrBCCAM,
        ABAM3FrBCCAMFail,
        ABAM3FrBScCCAM,
        ABAMCCAM,
        ABAM3CCAM,*/
        CustomSpellMix,
        ArcaneExplosion,
        AE4AB,
        AERampAB,
        FlamestrikeSpammed,
        FlamestrikeSingle,
        Blizzard,
        BlastWave,
        DragonsBreath,
        ConeOfCold
    }

    public class Cycle
    {
        public string Name;
        public string Note;
        public double DpmConversion; // used by mana neutral cycle to improve numerical stability of solver
        public CycleId Mix1;
        public CycleId Mix2;
        public CycleId CycleId;

        public override string ToString()
        {
            return Name;
        }

        public CastingState CastingState;

        internal struct SpellData
        {
            public Spell Spell;
            public float Weight;
            public float DotUptime;
        }

        internal List<SpellData> Spell;

        public Cycle()
        {
        }

        public Cycle(bool needsDisplayCalculations, CastingState castingState)
        {
            CastingState = castingState;
            if (needsDisplayCalculations)
            {
                Spell = new List<SpellData>();
            }
        }

        public static Cycle New(bool needsDisplayCalculations, CastingState castingState)
        {
            ArraySet arraySet = castingState.Solver.ArraySet;
            if (needsDisplayCalculations || arraySet == null)
            {
                return new Cycle(needsDisplayCalculations, castingState);
            }
            else
            {
                Cycle cycle = arraySet.NewCycle();
                cycle.Initialize(castingState);
                return cycle;
            }
        }

        public void AddCycle(bool needsDisplayCalculations, Cycle cycle, double weight)
        {
            if (needsDisplayCalculations)
            {
                AddSpellsFromCycle(cycle, weight);
            }
            CastTime += weight * cycle.CastTime;
            CastProcs += weight * cycle.CastProcs;
            CastProcs2 += weight * cycle.CastProcs2;
            NukeProcs += weight * cycle.NukeProcs;
            NukeProcs2 += weight * cycle.NukeProcs2;
            Ticks += weight * cycle.Ticks;
            HitProcs += weight * cycle.HitProcs;
            CritProcs += weight * cycle.CritProcs;
            IgniteProcs += weight * cycle.IgniteProcs;
            dotProcs += weight * cycle.dotProcs;
            TargetProcs += weight * cycle.TargetProcs;
            damageProcs += weight * cycle.damageProcs;
            Absorbed += weight * cycle.Absorbed;
            costPerSecond += weight * cycle.CastTime * cycle.costPerSecond;
            damagePerSecond += weight * cycle.CastTime * cycle.damagePerSecond;
            threatPerSecond += weight * cycle.CastTime * cycle.threatPerSecond;
            DpsPerSpellPower += weight * cycle.CastTime * cycle.DpsPerSpellPower;
            DpsPerMastery += weight * cycle.CastTime * cycle.DpsPerMastery;
            DpsPerCrit += weight * cycle.CastTime * cycle.DpsPerCrit;
        }

        private void AddSpellsFromCycle(Cycle cycle, double weight)
        {
            foreach (var spell in cycle.Spell)
            {
                Spell.Add(new SpellData() { Spell = spell.Spell, DotUptime = spell.DotUptime, Weight = (float)weight * spell.Weight });
            }
        }

        public void AddSpell(bool needsDisplayCalculations, Spell spell, double weight)
        {
            if (needsDisplayCalculations)
            {
                Spell.Add(new SpellData() { Spell = spell, Weight = (float)weight });
            }
            CastTime += weight * spell.CastTime;
            CastProcs += weight * spell.CastProcs;
            CastProcs2 += weight * spell.CastProcs2;
            NukeProcs += weight * spell.NukeProcs;
            NukeProcs2 += weight * spell.NukeProcs2;
            Ticks += weight * spell.Ticks;
            HitProcs += weight * spell.HitProcs;
            CritProcs += weight * spell.CritProcs;
            IgniteProcs += weight * spell.IgniteProcs;
            dotProcs += weight * spell.DotProcs;
            TargetProcs += weight * spell.TargetProcs;
            damageProcs += weight * (spell.HitProcs + spell.DotProcs);
            Absorbed += weight * spell.TotalAbsorb;
            costPerSecond += weight * spell.AverageCost;
            damagePerSecond += weight * spell.AverageDamage;
            threatPerSecond += weight * spell.AverageThreat;
            DpsPerSpellPower += weight * spell.DamagePerSpellPower;
            DpsPerMastery += weight * spell.DamagePerMastery;
            DpsPerCrit += weight * spell.DamagePerCrit;
        }

        public void AddSpell(bool needsDisplayCalculations, Spell spell, double weight, double dotUptime)
        {
            if (needsDisplayCalculations)
            {
                Spell.Add(new SpellData() { Spell = spell, Weight = (float)weight, DotUptime = (float)dotUptime });
            }
            CastTime += weight * spell.CastTime;
            CastProcs += weight * spell.CastProcs;
            CastProcs2 += weight * spell.CastProcs2;
            NukeProcs += weight * spell.NukeProcs;
            NukeProcs2 += weight * spell.NukeProcs2;
            Ticks += weight * spell.Ticks;
            HitProcs += weight * spell.HitProcs;
            CritProcs += weight * spell.CritProcs;
            IgniteProcs += weight * spell.IgniteProcs;
            dotProcs += weight * dotUptime * spell.DotProcs;
            TargetProcs += weight * spell.TargetProcs;
            damageProcs += weight * (spell.HitProcs + dotUptime * spell.DotProcs);
            Absorbed += weight * spell.TotalAbsorb;
            costPerSecond += weight * spell.AverageCost;
            damagePerSecond += weight * (spell.AverageDamage + dotUptime * spell.DotAverageDamage);
            threatPerSecond += weight * (spell.AverageThreat + dotUptime * spell.DotAverageThreat);
            DpsPerSpellPower += weight * (spell.DamagePerSpellPower + dotUptime * spell.DotDamagePerSpellPower);
            DpsPerMastery += weight * (spell.DamagePerMastery + dotUptime * spell.DotDamagePerMastery);
            DpsPerCrit += weight * (spell.DamagePerCrit + dotUptime * spell.DotDamagePerCrit);
        }

        public void AddPause(double duration, double weight)
        {
            CastTime += weight * duration;
        }

        public void Calculate()
        {
            costPerSecond /= CastTime;
            damagePerSecond /= CastTime;
            threatPerSecond /= CastTime;
            DpsPerSpellPower /= CastTime;
            DpsPerMastery /= CastTime;
            DpsPerCrit /= CastTime;
        }

        public virtual void AddSpellContribution(Dictionary<string, SpellContribution> dict, float duration, float effectSpellPower, float effectMastery, float effectCrit, float averageMana)
        {
            foreach (var spell in Spell)
            {
                spell.Spell.AddSpellContribution(dict, spell.Weight * duration / (float)CastTime, spell.DotUptime, effectSpellPower, effectMastery, effectCrit, (float)effectManaAdeptMultiplier, averageMana);
            }
        }

        public virtual void AddManaUsageContribution(Dictionary<string, float> dict, float duration)
        {
            foreach (var spell in Spell)
            {
                spell.Spell.AddManaUsageContribution(dict, spell.Weight * duration / (float)CastTime);
            }
        }

        public void Initialize(CastingState castingState)
        {
            Note = null;
            DpmConversion = 0;
            Mix1 = CycleId.None;
            Mix2 = CycleId.None;

            CastingState = castingState;
            calculated = false;
            damagePerSecond = 0;
            effectDamagePerSecondSpell = 0;
            effectDamagePerSecondProc = 0;
            effectSpellPower = 0;
            effectIntellect = 0;
            effectMasteryRating = 0;
            effectMastery = 0;
            effectCrit = 0;
            effectDamageProcs = 0;
            effectDotProcs = 0;
            effectManaAdeptMultiplier = 1;
            threatPerSecond = 0;
            effectThreatPerSecond = 0;
            costPerSecond = 0;
            manaRegenPerSecond = 0;
            DpsPerSpellPower = 0;
            DpsPerMastery = 0;
            DpsPerCrit = 0;
            Absorbed = 0;

            ProvidesSnare = false;
            ProvidesScorch = false;

            AreaEffect = false;
            //AoeSpell = null;

            HitProcs = 0;
            Ticks = 0;
            CastProcs = 0;
            CastProcs2 = 0;
            NukeProcs = 0;
            NukeProcs2 = 0;
            CritProcs = 0;
            IgniteProcs = 0;
            dotProcs = 0;
            CastTime = 0;
            TargetProcs = 0;
            damageProcs = 0;
            OO5SR = 0;

            HasteProcs = null;
            HasteProcsUptime = null;
        }

        private bool calculated;

        internal double damagePerSecond;
        internal double effectManaAdeptMultiplier; // to account for mana adept depression from int procs
        internal double effectDamagePerSecondSpell;
        internal double effectDamagePerSecondProc;
        internal double effectSpellPower;
        internal double effectIntellect;
        internal double effectMasteryRating;
        internal double effectMastery;
        internal double effectCrit;
        internal double effectDamageProcs;
        internal double effectDotProcs;
        public double DamagePerSecond
        {
            get
            {
                CalculateEffects();
                return damagePerSecond + effectDamagePerSecondSpell + effectDamagePerSecondProc;
            }
        }

        public void FixManaNeutral()
        {
            // costPerSecond + x - manaRegenPerSecond = 0
            CalculateEffects();
            double x = manaRegenPerSecond - costPerSecond;
            if (DpmConversion != 0)
            {
                costPerSecond += x;
                damagePerSecond += x * DpmConversion;
            }
        }

        public double GetDamagePerSecond(double manaAdeptBonus, double manaPercent)
        {
            CalculateEffects();

            // support for mastery procs
            // this isn't totally accurate, because the solver assumes constant mana adept factor
            // but it's as close as we can get without major rework, will probably overestimate a bit
            if (CastingState.Solver.Specialization == Specialization.Arcane)
            {
                manaAdeptBonus += 0.015f * effectMasteryRating / 14 * CastingState.CalculationOptions.LevelScalingFactor;
            }

            return (damagePerSecond + effectDamagePerSecondSpell) * (1 + manaPercent * manaAdeptBonus * effectManaAdeptMultiplier) + effectDamagePerSecondProc;
        }

        public double GetQuadraticSpellDPS()
        {
            CalculateEffects();

            // combining a number of factors into the dps portion of the Q matrix in quadratic formulation

            // sum_i dps[i] * (1 + mm[i] * (k + mas[i])) * x[i]  -  0.5 * sum_i sum_j dps[i] * mps[j] / M * mm[i] * (k + mas[i])) * x[i] * x[j]
            // dps[i] * mps[j] / M * k   =>  [dps[i] * mm[i] * (k + mas[i])] * mps[j] / M

            return (damagePerSecond + effectDamagePerSecondSpell) * (CastingState.ManaAdeptBonus + CastingState.Solver.ManaAdeptMultiplier * effectMasteryRating / 14 * CastingState.CalculationOptions.LevelScalingFactor) * effectManaAdeptMultiplier;
        }

        internal double threatPerSecond;
        internal double effectThreatPerSecond;
        public double ThreatPerSecond
        {
            get
            {
                CalculateEffects();
                return threatPerSecond + effectThreatPerSecond;
            }
        }

        internal double costPerSecond;
        public double CostPerSecond
        {
            get
            {
                CalculateEffects();
                return costPerSecond;
            }
        }

        private double manaRegenPerSecond;
        public double ManaRegenPerSecond
        {
            get
            {
                CalculateEffects();
                return manaRegenPerSecond;
            }
        }

        public double ManaPerSecond
        {
            get
            {
                return CostPerSecond - ManaRegenPerSecond;
            }
        }

        public double Absorbed;
        public double DpsPerSpellPower;
        public double DpsPerMastery;
        public double DpsPerCrit;

        public bool ProvidesSnare;
        public bool ProvidesScorch;

        public bool AreaEffect;
        //public Spell AoeSpell;

        public double HitProcs;
        public double Ticks;
        public double CastProcs;
        public double CastProcs2; // variant with only one proc from AM
        public double NukeProcs;  // Fireball, Frostfire Bolt, Frostbolt, Arcane Blast
        public double NukeProcs2; // Fireball, Pyroblast, Frostfire Bolt, Frostbolt, Arcane Blast
        public double CritProcs;
        private double dotProcs;
        public double IgniteProcs;
        public double CastTime;
        public double TargetProcs;
        internal double damageProcs;
        public double OO5SR;

        public double DamageProcs
        {
            get
            {
                CalculateEffects();
                return damageProcs + effectDamageProcs;
            }
        }

        public double DotProcs
        {
            get
            {
                CalculateEffects();
                return dotProcs + effectDotProcs;
            }
        }

        public CastingState[] HasteProcs;
        public WeightedStat[] HasteProcsUptime;

        public void AddDamageContribution(Dictionary<string, SpellContribution> dict, float duration, float averageMana)
        {
            AddSpellContribution(dict, duration, (float)effectSpellPower, (float)effectMastery, (float)effectCrit, averageMana);
            AddEffectContribution(dict, duration, averageMana);
        }

        public void CalculateEffects()
        {
            if (!calculated)
            {
                CalculateIgniteDamageProcs();
                CalculateEffectDamage();
                CalculateManaRegen();
                calculated = true;
            }
        }

        private void CalculateIgniteDamageProcs()
        {
            if (IgniteProcs > 0)
            {
                double rate = IgniteProcs / CastTime;
                double k = Math.Exp(-2 * rate);
                double ticks = k * (1 + k);
                effectDamageProcs += (float)(IgniteProcs * ticks);
                effectDotProcs += (float)(IgniteProcs * ticks);
            }
        }

        private void CalculateEffectDamage()
        {
            Stats baseStats = CastingState.BaseStats;
            double spellPower = 0;
            effectMasteryRating = 0;
            double stateMaxMana = CastingState.BaseStats.Mana + CastingState.StateEffectMaxMana;
            effectManaAdeptMultiplier = CastingState.BaseStats.Mana / stateMaxMana;
            if (Ticks > 0)
            {
                for (int i = 0; i < CastingState.Solver.StackingIntellectEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.StackingIntellectEffects[i];
                    float uptime = GetAverageFactor(effect);
                    effectIntellect += (effect.Stats.Intellect + effect.Stats.HighestStat) * uptime;
                }
                for (int i = 0; i < CastingState.Solver.SpellPowerEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.SpellPowerEffects[i];
                    spellPower += effect.Stats.SpellPower * GetAverageFactor(effect);
                }
                for (int i = 0; i < CastingState.Solver.IntellectEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.IntellectEffects[i];
                    float uptime = GetAverageFactor(effect);
                    effectIntellect += (effect.Stats.Intellect + effect.Stats.HighestStat) * uptime;
                    if (CastingState.Solver.Specialization == Specialization.Arcane && !CastingState.CalculationOptions.ModeMOP)
                    {
                        effectManaAdeptMultiplier *= 1 - uptime + uptime * stateMaxMana / (stateMaxMana + (effect.Stats.Intellect + effect.Stats.HighestStat) * (1 + CastingState.BaseStats.BonusIntellectMultiplier) * 15 * (1 + CastingState.BaseStats.BonusManaMultiplier));
                    }
                }
                for (int i = 0; i < CastingState.Solver.MasteryRatingEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.MasteryRatingEffects[i];
                    effectMasteryRating += effect.Stats.MasteryRating * GetAverageFactor(effect);
                }
                for (int i = 0; i < CastingState.Solver.ResetStackingEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.ResetStackingEffects[i];
                    float outerUptime = GetAverageFactor(effect);
                    for (int j = 0; j < effect.Stats._rawSpecialEffectDataSize; j++)
                    {
                        SpecialEffect e = effect.Stats._rawSpecialEffectData[i];
                        if (e.Chance == 1f && e.Cooldown == 0f && e.MaxStack > 1 && (e.Trigger == Trigger.DamageSpellCast || e.Trigger == Trigger.DamageSpellHit || e.Trigger == Trigger.SpellCast || e.Trigger == Trigger.SpellHit))
                        {
                            if (e.Stats.SpellPower > 0)
                            {
                                float triggerInterval;
                                float triggerChance;
                                float fightDuration = CastingState.CalculationOptions.FightDuration;
                                if (GetTriggerData(e, out triggerInterval, out triggerChance))
                                {
                                    int stackReset = 2 + (int)(fightDuration / effect.Cooldown); // assume after first initial stack they use and start stacking again (i.e. Heart of Ignacius)
                                    spellPower += outerUptime * e.Stats.SpellPower * e.GetAverageStackSize(triggerInterval, triggerChance, 3, fightDuration, stackReset);
                                }
                            }
                        }
                    }
                }
            }
            if (CastingState.MageTalents.IncantersAbsorption > 0)
            {
                //float incanterSpellPower = Math.Min((float)Math.Min(calculationOptions.AbsorptionPerSecond, calculationResult.IncomingDamageDps) * 0.05f * talents.IncantersAbsorption * 10, 0.05f * baseStats.Health);
                spellPower += Absorbed / CastTime * 0.05f * CastingState.MageTalents.IncantersAbsorption * 10;
                //spellPower += Math.Min((float)Math.Min(CastingState.CalculationOptions.AbsorptionPerSecond + Absorbed / CastTime, CastingState.Calculations.IncomingDamageDps) * 0.05f * CastingState.MageTalents.IncantersAbsorption * 10, 0.05f * baseStats.Health);
            }
            effectIntellect *= (1 + CastingState.BaseStats.BonusIntellectMultiplier);
            spellPower += effectIntellect;
            spellPower *= (1 + CastingState.BaseStats.BonusSpellPowerMultiplier);
            effectSpellPower = spellPower;
            effectDamagePerSecondSpell += spellPower * DpsPerSpellPower;
            effectMastery = effectMasteryRating / 14 * CastingState.CalculationOptions.LevelScalingFactor;
            effectDamagePerSecondSpell += effectMastery * DpsPerMastery;
            effectCrit += effectIntellect * 0.01f * CastingState.Solver.SpellCritPerInt;
            effectDamagePerSecondSpell += effectCrit * DpsPerCrit;
            //effectThreatPerSecond += spellPower * TpsPerSpellPower; // do we really need more threat calculations???
            if (CastingState.WaterElemental)
            {
                Spell waterbolt = CastingState.GetSpell(SpellId.Waterbolt);
                effectDamagePerSecondProc += (waterbolt.AverageDamage + spellPower * waterbolt.DamagePerSpellPower) / waterbolt.CastTime;
            }
            if (CastingState.MirrorImage)
            {
                Spell mirrorImage = CastingState.GetSpell(SpellId.MirrorImage);
                effectDamagePerSecondProc += (mirrorImage.AverageDamage + spellPower * mirrorImage.DamagePerSpellPower) / mirrorImage.CastTime;
            }
            if (CastingState.CalculationOptions.ProcCombustion && CastingState.MageTalents.Combustion == 1)
            {
                double damage;
                if (HasteProcs != null)
                {
                    damage = 0f;
                    for (int i = 0; i < HasteProcsUptime.Length; i++)
                    {
                        Spell combustion = HasteProcs[i].GetSpell(SpellId.Combustion);
                        damage += HasteProcsUptime[i].Chance * (combustion.AverageDamage + combustion.DamagePerSpellPower * effectSpellPower + combustion.DamagePerMastery * effectMastery + combustion.DamagePerCrit * effectCrit) / 10f;
                    }
                }
                else
                {
                    Spell combustion = CastingState.GetSpell(SpellId.Combustion);
                    damage = (combustion.AverageDamage + combustion.DamagePerSpellPower * effectSpellPower + combustion.DamagePerMastery * effectMastery + combustion.DamagePerCrit * effectCrit) / 10f;
                }
                double factor;
                if (CastingState.Solver.Mage4T13)
                {
                    factor = GetAverageFactor(Solver.SpecialEffectCombustion4T13);
                }
                else
                {
                    factor = GetAverageFactor(Solver.SpecialEffectCombustion);
                }
                effectDamagePerSecondSpell += damage * factor;
            }
            if (Ticks > 0)
            {
                for (int i = 0; i < CastingState.Solver.DamageProcEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.DamageProcEffects[i];
                    float effectsPerSecond = GetAverageFactor(effect);
                    if (effect.Stats.ArcaneDamage > 0)
                    {
                        float boltDps = CastingState.ArcaneAverageDamage * (effect.Stats.ArcaneDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond;
                        effectDamagePerSecondSpell += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.ArcaneThreatMultiplier;
                    }
                    if (effect.Stats.FireDamage > 0)
                    {
                        float boltDps = CastingState.FireAverageDamage * (effect.Stats.FireDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond;
                        effectDamagePerSecondSpell += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.FireThreatMultiplier;
                    }
                    if (effect.Stats.FrostDamage > 0)
                    {
                        float boltDps = CastingState.FrostAverageDamage * (effect.Stats.FrostDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond;
                        effectDamagePerSecondSpell += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.FrostThreatMultiplier;
                    }
                    if (effect.Stats.ShadowDamage > 0)
                    {
                        float boltDps = CastingState.ShadowAverageDamage * (effect.Stats.ShadowDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond;
                        effectDamagePerSecondSpell += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.ShadowThreatMultiplier;
                    }
                    if (effect.Stats.NatureDamage > 0)
                    {
                        float boltDps = CastingState.NatureAverageDamage * (effect.Stats.NatureDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond;
                        effectDamagePerSecondSpell += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.NatureThreatMultiplier;
                    }
                    if (effect.Stats.HolyDamage > 0)
                    {
                        float boltDps = CastingState.HolyAverageDamage * (effect.Stats.HolyDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond;
                        effectDamagePerSecondSpell += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.HolyThreatMultiplier;
                    }
                    if (effect.Stats.HolySummonedDamage > 0)
                    {
                        float boltDps = CastingState.HolySummonedAverageDamage * effect.Stats.HolySummonedDamage * effectsPerSecond;
                        effectDamagePerSecondProc += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.HolyThreatMultiplier;
                    }
                    if (effect.Stats.FireSummonedDamage > 0)
                    {
                        float boltDps = CastingState.FireSummonedAverageDamage * effect.Stats.FireSummonedDamage * effectsPerSecond;
                        effectDamagePerSecondProc += boltDps;
                        effectThreatPerSecond += boltDps * CastingState.FireThreatMultiplier;
                    }
                }
            }
        }

        public bool GetTriggerData(SpecialEffect effect, out float triggerInterval, out float triggerChance)
        {
            switch (effect.Trigger)
            {
                case Trigger.Use:
                    triggerInterval = 0;
                    triggerChance = 1;
                    break;
                case Trigger.DamageSpellCrit:
                case Trigger.SpellCrit:
                    triggerInterval = (float)(CastTime / Ticks);
                    triggerChance = (float)(CritProcs / Ticks);
                    return true;
                case Trigger.DamageSpellHit:
                case Trigger.SpellHit:
                    triggerInterval = (float)(CastTime / Ticks);
                    triggerChance = (float)(HitProcs / Ticks);
                    return true;
                case Trigger.SpellMiss:
                    triggerInterval = (float)(CastTime / Ticks);
                    triggerChance = (float)(1 - HitProcs / Ticks);
                    return true;
                case Trigger.DamageSpellCast:
                case Trigger.SpellCast:
                    if (CastProcs > 0)
                    {
                        if (effect.Stats.HolySummonedDamage > 0)
                        {
                            triggerInterval = (float)(CastTime / CastProcs2);
                        }
                        else
                        {
                            triggerInterval = (float)(CastTime / CastProcs);
                        }
                        triggerChance = 1;
                        return true;
                    }
                    break;
                case Trigger.MageNukeCast:
                    if (NukeProcs > 0)
                    {
                        triggerInterval = (float)(CastTime / NukeProcs);
                        triggerChance = 1;
                        return true;
                    }
                    break;
                case Trigger.MageNukeCast2:
                    if (NukeProcs2 > 0)
                    {
                        triggerInterval = (float)(CastTime / NukeProcs2);
                        triggerChance = 1;
                        return true;
                    }
                    break;
                case Trigger.DamageDone:
                case Trigger.DamageOrHealingDone:
                    if (damageProcs + effectDamageProcs > 0)
                    {
                        triggerInterval = (float)(CastTime / (damageProcs + effectDamageProcs));
                        triggerChance = 1;
                        return true;
                    }
                    break;
                case Trigger.DoTTick:
                    if (dotProcs + effectDotProcs > 0)
                    {
                        triggerInterval = (float)(CastTime / (dotProcs + effectDotProcs));
                        triggerChance = 1;
                        return true;
                    }
                    break;
            }
            triggerInterval = 0;
            triggerChance = 0;
            return false;
        }

        private void CalculateManaRegen()
        {
            if (CastingState.CalculationOptions.EffectDisableManaSources) return;
            Stats baseStats = CastingState.BaseStats;
            manaRegenPerSecond = CastingState.ManaRegen5SR + OO5SR * (CastingState.ManaRegen - CastingState.ManaRegen5SR) + baseStats.ManaRestoreFromMaxManaPerSecond * effectIntellect * 15 * (1 + CastingState.BaseStats.BonusManaMultiplier);
            for (int i = 0; i < CastingState.Solver.ManaRestoreEffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.ManaRestoreEffects[i];
                manaRegenPerSecond += effect.Stats.ManaRestore * GetAverageFactor(effect);
            }
            for (int i = 0; i < CastingState.Solver.Mp5EffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.Mp5Effects[i];
                manaRegenPerSecond += effect.Stats.Mp5 / 5f * GetAverageFactor(effect);
            }
            //threatPerSecond += (baseStats.ManaRestoreFromBaseManaPPM * 3268 / CastTime * HitProcs) * 0.5f * (1 + baseStats.ThreatIncreaseMultiplier) * (1 - baseStats.ThreatReductionMultiplier);
        }

        public virtual void AddManaSourcesContribution(Dictionary<string, float> dict, float duration)
        {
            if (CastingState.CalculationOptions.EffectDisableManaSources) return;
            dict["Intellect/Spirit"] += (float)(duration * (CastingState.SpiritRegen * CastingState.BaseStats.SpellCombatManaRegeneration + OO5SR * (CastingState.SpiritRegen - CastingState.SpiritRegen * CastingState.BaseStats.SpellCombatManaRegeneration)));
            dict["MP5"] += duration * CastingState.BaseStats.Mp5 / 5f;
            dict["Innervate"] += duration * (15732 * CastingState.CalculationOptions.Innervate / CastingState.CalculationOptions.FightDuration);
            dict["Replenishment"] += duration * CastingState.BaseStats.ManaRestoreFromMaxManaPerSecond * CastingState.BaseStats.Mana;
            //dict["Judgement of Wisdom"] += duration * CastingState.BaseStats.ManaRestoreFromBaseManaPPM * 3268 / CastTime * HitProcs;
            for (int i = 0; i < CastingState.Solver.ManaRestoreEffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.ManaRestoreEffects[i];
                dict["Other"] += duration * effect.Stats.ManaRestore * GetAverageFactor(effect);
            }
            for (int i = 0; i < CastingState.Solver.Mp5EffectsCount; i++)
            {
                SpecialEffect effect = CastingState.Solver.Mp5Effects[i];
                dict["Other"] += duration * effect.Stats.Mp5 / 5f * GetAverageFactor(effect);
            }
        }

        public void AddEffectContribution(Dictionary<string, SpellContribution> dict, float duration, float averageMana)
        {
            SpellContribution contrib;
            if (CastingState.WaterElemental)
            {
                Spell waterbolt = CastingState.GetSpell(SpellId.Waterbolt);
                if (!dict.TryGetValue(waterbolt.Name, out contrib))
                {
                    contrib = new SpellContribution() { Name = waterbolt.Name };
                    dict[waterbolt.Name] = contrib;
                }
                contrib.Hits += duration / waterbolt.CastTime;
                contrib.Damage += (float)(waterbolt.AverageDamage + effectSpellPower * waterbolt.DamagePerSpellPower) / waterbolt.CastTime * duration;
            }
            if (CastingState.MirrorImage)
            {
                Spell mirrorImage = CastingState.GetSpell(SpellId.MirrorImage);
                if (!dict.TryGetValue("Mirror Image", out contrib))
                {
                    contrib = new SpellContribution() { Name = "Mirror Image" };
                    dict["Mirror Image"] = contrib;
                }
                contrib.Hits += 3 * (CastingState.MageTalents.GlyphOfMirrorImage ? 4 : 3) * duration / mirrorImage.CastTime;
                contrib.Damage += (float)(mirrorImage.AverageDamage + effectSpellPower * mirrorImage.DamagePerSpellPower) / mirrorImage.CastTime * duration;
            }
            if (CastingState.CalculationOptions.ProcCombustion && CastingState.MageTalents.Combustion == 1)
            {
                float factor;
                if (CastingState.Solver.Mage4T13)
                {
                    factor = GetAverageFactor(Solver.SpecialEffectCombustion4T13);
                }
                else
                {
                    factor = GetAverageFactor(Solver.SpecialEffectCombustion);
                }
                if (HasteProcs != null)
                {
                    for (int i = 0; i < HasteProcsUptime.Length; i++)
                    {
                        Spell combustion = HasteProcs[i].GetSpell(SpellId.Combustion);
                        combustion.AddSpellContribution(dict, HasteProcsUptime[i].Chance * factor * duration / 10f, 0, (float)effectSpellPower, (float)effectMastery, (float)effectCrit, (float)effectManaAdeptMultiplier, averageMana);
                    }
                }
                else
                {
                    Spell combustion = CastingState.GetSpell(SpellId.Combustion);
                    combustion.AddSpellContribution(dict, factor * duration / 10f, 0, (float)effectSpellPower, (float)effectMastery, (float)effectCrit, (float)effectManaAdeptMultiplier, averageMana);
                }
            }
            if (Ticks > 0)
            {
                for (int i = 0; i < CastingState.Solver.DamageProcEffectsCount; i++)
                {
                    SpecialEffect effect = CastingState.Solver.DamageProcEffects[i];
                    string name = null;
                    float effectsPerSecond = GetAverageFactor(effect);
                    float boltDps = 0f;
                    float spellMultiplier = 1f;
                    if (CastingState.Solver.Specialization == Specialization.Arcane)
                    {
                        double manaAdeptBonus = CastingState.ManaAdeptBonus + 0.015f * effectMasteryRating / 14 * CastingState.CalculationOptions.LevelScalingFactor;
                        spellMultiplier = (float)(1 + averageMana / CastingState.BaseStats.Mana * manaAdeptBonus * effectManaAdeptMultiplier);
                    }
                    if (effect.Stats.ArcaneDamage > 0)
                    {
                        boltDps = CastingState.ArcaneAverageDamage * (effect.Stats.ArcaneDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond * spellMultiplier;
                        name = "Arcane Damage Proc";
                    }
                    if (effect.Stats.FireDamage > 0)
                    {
                        boltDps = CastingState.FireAverageDamage * (effect.Stats.FireDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond * spellMultiplier;
                        name = "Fire Damage Proc";
                    }
                    if (effect.Stats.FrostDamage > 0)
                    {
                        boltDps = CastingState.FrostAverageDamage * (effect.Stats.FrostDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond * spellMultiplier;
                        name = "Frost Damage Proc";
                    }
                    if (effect.Stats.ShadowDamage > 0)
                    {
                        boltDps = CastingState.ShadowAverageDamage * (effect.Stats.ShadowDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond * spellMultiplier;
                        name = "Shadow Damage Proc";
                    }
                    if (effect.Stats.NatureDamage > 0)
                    {
                        boltDps = CastingState.NatureAverageDamage * (effect.Stats.NatureDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond * spellMultiplier;
                        name = "Nature Damage Proc";
                    }
                    if (effect.Stats.HolyDamage > 0)
                    {
                        boltDps = CastingState.HolyAverageDamage * (effect.Stats.HolyDamage + effect.SpellPowerScaling * CastingState.BaseStats.SpellPower) * effectsPerSecond * spellMultiplier;
                        name = "Holy Damage Proc";
                    }
                    if (effect.Stats.HolySummonedDamage > 0)
                    {
                        boltDps = CastingState.HolySummonedAverageDamage * effect.Stats.HolySummonedDamage * effectsPerSecond;
                        name = "Holy Damage";
                    }
                    if (effect.Stats.FireSummonedDamage > 0)
                    {
                        boltDps = CastingState.FireSummonedAverageDamage * effect.Stats.FireSummonedDamage * effectsPerSecond;
                        name = "Fire Damage";
                    }
                    if (!dict.TryGetValue(name, out contrib))
                    {
                        contrib = new SpellContribution() { Name = name };
                        dict[name] = contrib;
                    }
                    if (effect.Duration == 0)
                    {
                        contrib.Hits += effectsPerSecond * duration;
                    }
                    contrib.Damage += boltDps * duration;
                }
            }
            if (IgniteProcs > 0 && dict.TryGetValue("Ignite", out contrib))
            {
                double rate = IgniteProcs / CastTime;
                double k = Math.Exp(-2 * rate);
                double ticks = k * (1 + k);
                double ticksPerSecond = rate * ticks;
                contrib.Hits += duration * (float)ticksPerSecond;
            }
        }

        private float GetAverageFactor(SpecialEffect effect)
        {
            float triggerInterval;
            float triggerChance;
            if (GetTriggerData(effect, out triggerInterval, out triggerChance))
            {
                float durationMultiplier = 1;
                if (effect.LimitedToExecutePhase)
                {
                    durationMultiplier = CastingState.CalculationOptions.MoltenFuryPercentage;
                }
                if (effect.MaxStack > 1)
                {
                    return durationMultiplier * effect.GetAverageStackSize(triggerInterval, triggerChance, 3f, durationMultiplier * CastingState.CalculationOptions.FightDuration);
                }
                else if (effect.Duration > 0)
                {
                    return durationMultiplier * effect.GetAverageUptime(triggerInterval, triggerChance, 3f, durationMultiplier * CastingState.CalculationOptions.FightDuration);
                }
                else
                {
                    return durationMultiplier * effect.GetAverageProcsPerSecond(triggerInterval, triggerChance, 3f, durationMultiplier * CastingState.CalculationOptions.FightDuration);
                }
            }
            return 0;
        }
    }

    public class SpellCustomMix : Cycle
    {
        public SpellCustomMix(bool needsDisplayCalculations, CastingState castingState)
            : base(needsDisplayCalculations, castingState)
        {
            Name = "Custom Mix";
            if (castingState.CalculationOptions.CustomSpellMix == null) return;
            for (int i = 0; i < castingState.CalculationOptions.CustomSpellMix.Count; i++)
            {
                SpellWeight spellWeight = castingState.CalculationOptions.CustomSpellMix[i];
                AddSpell(needsDisplayCalculations, castingState.GetSpell(spellWeight.Spell), spellWeight.Weight);
            }
            Calculate();
        }
    }

    public class CycleState
    {
        public List<CycleStateTransition> Transitions { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class CycleStateTransition
    {
        public CycleState TargetState { get; set; }
        public Cycle Cycle { get; set; }
        public Spell Spell { get; set; }
        public Spell DWSpell { get; set; } // legendary proc
        public float Pause { get; set; }
        public virtual float TransitionProbability { get; set; }

        public override string ToString()
        {
            if (Spell != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Spell.Name, TargetState, 100 * TransitionProbability);
            }
            else if (Cycle != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Cycle.Name, TargetState, 100 * TransitionProbability);
            }
            else if (Pause > 0)
            {
                return string.Format("{0:F} sec => {1} : {2:F}%", Pause, TargetState, 100 * TransitionProbability);
            }
            return base.ToString();
        }
    }

    public class GenericCycle : Cycle
    {
        public List<CycleState> StateList;
        public double[] StateWeight;
        Dictionary<Spell, double> SpellWeight = new Dictionary<Spell, double>();
        Dictionary<Spell, double> DWSpellWeight = new Dictionary<Spell, double>();
        Dictionary<Cycle, double> CycleWeight = new Dictionary<Cycle, double>();
        public string SpellDistribution;

#if SILVERLIGHT
        public GenericCycle(string name, CastingState castingState, List<CycleState> stateDescription, bool needsDisplayCalculations)
#else
        public unsafe GenericCycle(string name, CastingState castingState, List<CycleState> stateDescription, bool needsDisplayCalculations)
#endif
            : base(needsDisplayCalculations, castingState)
        {
            Name = name;

            StateList = stateDescription;
            for (int i = 0; i < StateList.Count; i++)
            {
                StateList[i].Index = i;
            }

            int size = StateList.Count + 1;

            ArraySet arraySet = ArrayPool.RequestArraySet(false);
            try
            {

                LU M = new LU(size, arraySet);

                StateWeight = new double[size];

#if SILVERLIGHT
            M.BeginSafe();

            Array.Clear(arraySet.LU_U, 0, size * size);

            //U[i * rows + j]

            foreach (CycleState state in StateList)
            {
                foreach (CycleStateTransition transition in state.Transitions)
                {
                    arraySet.LU_U[transition.TargetState.Index * size + state.Index] += transition.TransitionProbability;
                }
                arraySet.LU_U[state.Index * size + state.Index] -= 1.0;
            }

            for (int i = 0; i < size - 1; i++)
            {
                arraySet.LU_U[(size - 1) * size + i] = 1;
            }

            StateWeight[size - 1] = 1;

            M.Decompose();
            M.FSolve(StateWeight);

            M.EndUnsafe();            
#else
                fixed (double* U = arraySet.LU_U, x = StateWeight)
                fixed (double* sL = arraySet.LUsparseL, column = arraySet.LUcolumn, column2 = arraySet.LUcolumn2)
                fixed (int* P = arraySet.LU_P, Q = arraySet.LU_Q, LJ = arraySet.LU_LJ, sLI = arraySet.LUsparseLI, sLstart = arraySet.LUsparseLstart)
                {
                    M.BeginUnsafe(U, sL, P, Q, LJ, sLI, sLstart, column, column2);

                    Array.Clear(arraySet.LU_U, 0, size * size);

                    //U[i * rows + j]

                    foreach (CycleState state in StateList)
                    {
                        foreach (CycleStateTransition transition in state.Transitions)
                        {
                            U[transition.TargetState.Index * size + state.Index] += transition.TransitionProbability;
                        }
                        U[state.Index * size + state.Index] -= 1.0;
                    }

                    for (int i = 0; i < size - 1; i++)
                    {
                        U[(size - 1) * size + i] = 1;
                    }

                    x[size - 1] = 1;

                    M.Decompose();
                    M.FSolve(x);

                    M.EndUnsafe();
                }
#endif

                SpellWeight = new Dictionary<Spell, double>();
                CycleWeight = new Dictionary<Cycle, double>();
                DWSpellWeight = new Dictionary<Spell, double>();

                foreach (CycleState state in StateList)
                {
                    double stateWeight = StateWeight[state.Index];
                    if (stateWeight > 0)
                    {
                        foreach (CycleStateTransition transition in state.Transitions)
                        {
                            float transitionProbability = transition.TransitionProbability;
                            if (transitionProbability > 0)
                            {
                                if (transition.Spell != null)
                                {
                                    double weight;
                                    SpellWeight.TryGetValue(transition.Spell, out weight);
                                    SpellWeight[transition.Spell] = weight + stateWeight * transitionProbability;
                                }
                                if (transition.DWSpell != null)
                                {
                                    double weight;
                                    DWSpellWeight.TryGetValue(transition.DWSpell, out weight);
                                    DWSpellWeight[transition.DWSpell] = weight + stateWeight * transitionProbability;
                                }
                                if (transition.Cycle != null)
                                {
                                    double weight;
                                    CycleWeight.TryGetValue(transition.Cycle, out weight);
                                    CycleWeight[transition.Cycle] = weight + stateWeight * transitionProbability;
                                }
                                if (transition.Pause > 0)
                                {
                                    AddPause(transition.Pause, (float)(stateWeight * transitionProbability));
                                }
                            }
                        }
                    }
                }

                StringBuilder sb = new StringBuilder();
                foreach (KeyValuePair<Spell, double> kvp in SpellWeight)
                {
                    AddSpell(needsDisplayCalculations, kvp.Key, (float)kvp.Value);
                    if (kvp.Value > 0) sb.AppendFormat("{0}:\t{1:F}%\r\n", kvp.Key.Label ?? kvp.Key.SpellId.ToString(), 100.0 * kvp.Value);
                }
                foreach (KeyValuePair<Spell, double> kvp in DWSpellWeight)
                {
                    AddSpell(needsDisplayCalculations, kvp.Key, (float)kvp.Value);
                }
                foreach (KeyValuePair<Cycle, double> kvp in CycleWeight)
                {
                    AddCycle(needsDisplayCalculations, kvp.Key, (float)kvp.Value);
                    if (kvp.Value > 0) sb.AppendFormat("{0}:\t{1:F}%\r\n", kvp.Key.CycleId, 100.0 * kvp.Value);
                }

                Calculate();

                SpellDistribution = sb.ToString();
            }
            finally
            {
                ArrayPool.ReleaseArraySet(arraySet);
            }
        }
    }

    public class CycleControlledStateTransition : CycleStateTransition
    {
        private float rawProbability;
        private int controlIndex;
        private int controlValue;
        private int[] controlStates;

        public void SetControls(int controlIndex, int[] controlStates, int controlValue)
        {
            this.controlIndex = controlIndex;
            this.controlStates = controlStates;
            this.controlValue = controlValue;
        }

        public override float TransitionProbability
        {
            get
            {
                return (controlStates[controlIndex] == controlValue) ? rawProbability : 0.0f;
            }
            set
            {
                rawProbability = value;
            }
        }

        public override string ToString()
        {
            if (Spell != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Spell.Name, TargetState, 100 * rawProbability);
            }
            else if (Cycle != null)
            {
                return string.Format("{0} => {1} : {2:F}%", Cycle.Name, TargetState, 100 * rawProbability);
            }
            else if (Pause > 0)
            {
                return string.Format("{0:F} sec => {1} : {2:F}%", Pause, TargetState, 100 * rawProbability);
            }
            return base.ToString();
        }
    }

    public abstract class CycleGenerator
    {
        public List<CycleState> StateList;
        public int[] ControlOptions;
        public int[] ControlValue;
        public int[] ControlIndex;
        public Dictionary<string, int>[] SpellMap;
        public List<string> SpellList;
        public virtual string StateDescription
        {
            get
            {
                return "";
            }
        }

        public string ConvertCycleNameInternalToEasy(string code)
        {
            if (code.Length > 0 && !char.IsNumber(code[0]))
            {
                return code;
            }
            string ret = "";
            for (int i = 0; i < code.Length; i++)
            {
                if (i > 0 && i % 5 == 0)
                {
                    ret += " ";
                }
                int n = int.Parse("" + code[i]);
                foreach (var kvp in SpellMap[i])
                {
                    if (kvp.Value == n)
                    {
                        n = SpellList.IndexOf(kvp.Key);
                        ret += n;
                        break;
                    }
                }
            }
            return ret;
        }

        public string ConvertCycleNameEasyToInternal(string code)
        {
            code = code.Replace(" ", "");
            string ret = "";
            for (int i = 0; i < code.Length; i++)
            {
                int n = int.Parse("" + code[i]);
                string spell = SpellList[n];
                n = SpellMap[i][spell];
                ret += n;
            }
            return ret;
        }

        public void GenerateStateDescription()
        {
            List<CycleState> remainingStates = new List<CycleState>();
            List<CycleState> processedStates = new List<CycleState>();
            remainingStates.Add(GetInitialState());

            while (remainingStates.Count > 0)
            {
                CycleState state = remainingStates[remainingStates.Count - 1];
                remainingStates.RemoveAt(remainingStates.Count - 1);

                List<CycleControlledStateTransition> transitions = GetStateTransitions(state);
#if SILVERLIGHT
                state.Transitions = transitions.ConvertAll(transition => (CycleStateTransition)transition).ToList();
#else
                state.Transitions = transitions.ConvertAll(transition => (CycleStateTransition)transition);
#endif
                foreach (CycleControlledStateTransition transition in transitions)
                {
                    if (transition.TargetState != state && !processedStates.Contains(transition.TargetState) && !remainingStates.Contains(transition.TargetState))
                    {
                        remainingStates.Add(transition.TargetState);
                    }
                }

                processedStates.Add(state);
            }

            StateList = processedStates;
            for (int i = 0; i < StateList.Count; i++)
            {
                StateList[i].Index = i;
            }

            ControlIndex = new int[StateList.Count];
            List<CycleState> controlledStates = new List<CycleState>();
            foreach (CycleState state in StateList)
            {
                int controlIndex = -1;
                foreach (CycleState controlledState in controlledStates)
                {
                    if (!CanStatesBeDistinguished(state, controlledState))
                    {
                        controlIndex = ControlIndex[controlledState.Index];
                        break;
                    }
                }
                if (controlIndex == -1)
                {
                    controlIndex = controlledStates.Count;
                    controlledStates.Add(state);
                }
                ControlIndex[state.Index] = controlIndex;
            }

            ControlOptions = new int[controlledStates.Count];
            ControlValue = new int[controlledStates.Count];

            SpellMap = new Dictionary<string, int>[controlledStates.Count];
            SpellList = new List<string>();

            foreach (CycleState state in StateList)
            {
                int controlIndex = ControlIndex[state.Index];
                if (SpellMap[controlIndex] == null)
                {
                    SpellMap[controlIndex] = new Dictionary<string, int>();
                }
                foreach (CycleControlledStateTransition transition in state.Transitions)
                {
                    string n;
                    if (transition.Spell != null)
                    {
                        n = transition.Spell.Name;
                    }
                    else
                    {
                        n = "Pause";
                    }
                    int controlValue;
                    if (!SpellMap[controlIndex].TryGetValue(n, out controlValue))
                    {
                        controlValue = SpellMap[controlIndex].Keys.Count;
                        SpellMap[controlIndex][n] = controlValue;
                    }
                    transition.SetControls(controlIndex, ControlValue, controlValue);
                    if (!SpellList.Contains(n))
                    {
                        SpellList.Add(n);
                    }
                }
            }

            for (int i = 0; i < ControlOptions.Length; i++)
            {
                ControlOptions[i] = SpellMap[i].Keys.Count;
            }
        }

        public GenericCycle GenerateCycle(string name, CastingState castingState)
        {
            return new GenericCycle(name, castingState, StateList, false);
        }

        public List<Cycle> Analyze(CastingState castingState, Cycle wand)
        {
            return Analyze(castingState, wand, null);
        }

        public List<Cycle> Analyze(CastingState castingState, Cycle wand, System.ComponentModel.BackgroundWorker worker)
        {
            Dictionary<string, Cycle> cycleDict = new Dictionary<string, Cycle>();
            int j;
            // reset
            for (int i = 0; i < ControlValue.Length; i++)
            {
                ControlValue[i] = 0;
            }
            // count total cycles
            int total = 0;
            do
            {
                total++;
                j = ControlValue.Length - 1;
                ControlValue[j]++;
                while (ControlValue[j] >= ControlOptions[j])
                {
                    ControlValue[j] = 0;
                    j--;
                    if (j < 0)
                    {
                        break;
                    }
                    ControlValue[j]++;
                }
            } while (j >= 0);
            // reset
            for (int i = 0; i < ControlValue.Length; i++)
            {
                ControlValue[i] = 0;
            }
            int count = 0;
            do
            {
                if (worker != null && worker.CancellationPending)
                {
                    break;
                }
                if (worker != null && count % 100 == 0)
                {
                    worker.ReportProgress((100 * count) / total, count + "/" + total);
                }
                count++;
                string name = "";
                for (int i = 0; i < ControlValue.Length; i++)
                {
                    name += ControlValue[i].ToString();
                }
                GenericCycle generic = new GenericCycle(name, castingState, StateList, false);
                if (!cycleDict.ContainsKey(generic.SpellDistribution))
                {
                    cycleDict.Add(generic.SpellDistribution, generic);
                }
                // increment control
                j = ControlValue.Length - 1;
                ControlValue[j]++;
                while (ControlValue[j] >= ControlOptions[j])
                {
                    ControlValue[j] = 0;
                    j--;
                    if (j < 0)
                    {
                        break;
                    }
                    ControlValue[j]++;
                }
            } while (j >= 0);

            if (wand != null)
            {
                cycleDict["Wand"] = wand;
            }

            List<Cycle> cyclePalette = new List<Cycle>();

            double maxdps = 0;
            Cycle maxdpsCycle = null;
            foreach (Cycle cycle in cycleDict.Values)
            {
                if (cycle.DamagePerSecond > maxdps)
                {
                    maxdpsCycle = cycle;
                    maxdps = cycle.DamagePerSecond;
                }
            }

            cyclePalette.Add(maxdpsCycle);

            Cycle mindpmCycle;
            do
            {
                Cycle highdpsCycle = cyclePalette[cyclePalette.Count - 1];
            RESTART:
                mindpmCycle = null;
                double mindpm = double.PositiveInfinity;
                foreach (Cycle cycle in cycleDict.Values)
                {
                    double dpm = (cycle.DamagePerSecond - highdpsCycle.DamagePerSecond) / (cycle.ManaPerSecond - highdpsCycle.ManaPerSecond);
                    if (dpm > 0 && dpm < mindpm && cycle.ManaPerSecond < highdpsCycle.ManaPerSecond)
                    {
                        mindpm = dpm;
                        mindpmCycle = cycle;
                    }
                }
                if (mindpmCycle != null)
                {
                    // validate cycle pair theory
                    foreach (Cycle cycle in cycleDict.Values)
                    {
                        double dpm = (cycle.DamagePerSecond - mindpmCycle.DamagePerSecond) / (cycle.ManaPerSecond - mindpmCycle.ManaPerSecond);
                        if (cycle != highdpsCycle && cycle.DamagePerSecond > mindpmCycle.DamagePerSecond && dpm > mindpm + 0.000001)
                        {
                            highdpsCycle = cycle;
                            goto RESTART;
                        }
                    }
                    cyclePalette.Add(mindpmCycle);
                }
            } while (mindpmCycle != null);
            return cyclePalette;
        }

        protected abstract CycleState GetInitialState();
        // the transition probabilities should be set as given the spell/pause is executed 100%
        // the transitions should all be spell transitions and at most one can be a state changing pause
        protected abstract List<CycleControlledStateTransition> GetStateTransitions(CycleState state);
        // the states must form equivalence classes
        protected abstract bool CanStatesBeDistinguished(CycleState state1, CycleState state2);
    }
}
