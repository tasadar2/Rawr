using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Rawr.Mage
{
    public enum MagicSchool
    {
        Holy = 1,
        Fire = 2,
        Nature,
        Frost,
        Shadow,
        Arcane,
        FrostFire
    }

    public struct SpellData
    {
        public float MinDamage;
        public float MaxDamage;
        public float PeriodicDamage;
        public int Cost;
        public float SpellDamageCoefficient;
        public float DotDamageCoefficient;
    }

    public class SpellContribution : IComparable<SpellContribution>
    {
        public string Name;
        public float Hits;
        public float Crits;
        public float Ticks;
        public float Damage;
        public float HitDamage;
        public float CritDamage;
        public float TickDamage;
        public float DotUptime;
        public float Range;

        public int CompareTo(SpellContribution other)
        {
            return other.Damage.CompareTo(this.Damage);
        }
    }

    public class Spell
    {
        public SpellId SpellId; // set in CastingState.GetSpell
        private SpellTemplate template; // set in constructor/Intitialize

        public SpellTemplate SpellTemplate { get { return template; } }

        public void Initialize(SpellTemplate template)
        {
            this.template = template;
            cycle = null;
        }

        // Variables that have to be initialized in Calculate and can be modifier between Calculate and CalculateDerivedStats
        private CastingState castingState;
        public float BaseCastTime;
        public float CostModifier;
        public float CostAmplifier;
        public float SpellModifier;
        public float AdditiveSpellModifier;
        public float DirectDamageModifier;
        public float DotDamageModifier;
        public float CritRate;
        public float CritBonus;
        public float RawSpellDamage;
        public float InterruptProtection;
        public bool AreaEffect;
        public bool AreaEffectDot;
        public int MaximumAOETargets;
        public float BasePeriodicDamage;

        // Variables that have to be initialized in CalculateDerivedStats and can be modified after
        public bool SpammedDot;
        public bool Pom;
        public float Ticks;
        public float CastProcs;
        public float HitProcs;
        public float CritProcs;
        public float IgniteProcs;
        public float TargetProcs;
        public float DotProcs;
        public float ChannelReduction;
        public float AverageCastingSpeed;
        public float CastTime;
        public float OO5SR;
        public float AverageDamage;
        public float AverageThreat;
        public float AverageCost;
        public float Absorb; // max absorb on single impact
        public float TotalAbsorb; // total absorb with combined warding negates
        public float DamagePerSpellPower;
        public float DamagePerMastery;
        public float DamagePerCrit;
        // ignite tracking for separation in spell distribution
        public float IgniteDamage;
        public float IgniteDamagePerSpellPower;
        public float IgniteDamagePerMastery;
        public float IgniteDamagePerCrit;
        // stats valid for dot spells only
        public float DotAverageDamage;
        public float DotAverageThreat;
        public float DotDamagePerSpellPower;
        public float DotDamagePerMastery;
        public float DotDamagePerCrit;
        public float DotFullDuration;
        public float DotExtraTicks;

        public float DamagePerSecond
        {
            get
            {
                return AverageDamage / CastTime;
            }
        }

        public float ThreatPerSecond
        {
            get
            {
                return AverageThreat / CastTime;
            }
        }

        public float CostPerSecond
        {
            get
            {
                return AverageCost / CastTime;
            }
        }

        public string Label { get; set; }

        // Properties pulling data directly from template
        public string Name { get { return template.Name; } }
        public float AOEMultiplier { get { return template.AOEMultiplier; } }
        public bool Channeled { get { return template.Channeled; } }
        public float CastProcs2 { get { return template.CastProcs2; } }
        public float NukeProcs { get { return template.NukeProcs; } }
        public float NukeProcs2 { get { return template.NukeProcs2; } }
        public bool Instant { get { return template.Instant; } }
        public int BaseCost { get { return template.BaseCost; } }
        public float BaseCooldown { get { return template.BaseCooldown; } }
        public float GlobalCooldown { get { return template.GlobalCooldown; } }
        public MagicSchool MagicSchool { get { return template.MagicSchool; } }
        public float BaseMinDamage { get { return template.BaseMinDamage; } }
        public float BaseMaxDamage { get { return template.BaseMaxDamage; } }
        public float SpellDamageCoefficient { get { return template.SpellDamageCoefficient; } }
        public float DotDamageCoefficient { get { return template.DotDamageCoefficient; } }
        public float DotDuration { get { return template.DotDuration; } }
        public float DotTickInterval { get { return template.DotTickInterval; } }
        public float Range { get { return template.Range; } }
        public float RealResistance { get { return template.RealResistance; } }
        public float ThreatMultiplier { get { return template.ThreatMultiplier; } }
        public float HitRate { get { return template.HitRate; } }
        public float PartialResistFactor { get { return template.PartialResistFactor; } }
        public float Cooldown { get { return template.Cooldown; } }
        public float NonHSCritRate { get { return template.NonHSCritRate; } }       

        public float Cost
        {
            get
            {
                return (float)Math.Floor(BaseCost * CostAmplifier * CostModifier);
            }
        }

        public float ABCost
        {
            get
            {
                return (float)Math.Floor(Math.Round(BaseCost * CostAmplifier) * CostModifier);
            }
        }

        public float Latency
        {
            get
            {
                if (BaseCastTime <= GlobalCooldown || Instant)
                {
                    return castingState.CalculationOptions.LatencyGCD;
                }
                else if (Channeled)
                {
                    return castingState.CalculationOptions.LatencyChannel;
                }
                else
                {
                    return castingState.CalculationOptions.LatencyCast;
                }
            }
        }

        public float MinHitDamage
        {
            get
            {
                return (BaseMinDamage + RawSpellDamage * SpellDamageCoefficient) * SpellModifier * DirectDamageModifier / template.Ticks;
            }
        }

        public float MaxHitDamage
        {
            get
            {
                return (BaseMaxDamage + RawSpellDamage * SpellDamageCoefficient) * SpellModifier * DirectDamageModifier / template.Ticks;
            }
        }

        public float MinCritDamage
        {
            get
            {
                if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire)
                {
                    return MinHitDamage * CritBonus;
                }
                else
                {
                    return MinHitDamage * CritBonus;
                }
            }
        }

        public float MaxCritDamage
        {
            get
            {
                if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire)
                {
                    return MaxHitDamage * CritBonus;
                }
                else
                {
                    return MaxHitDamage * CritBonus;
                }
            }
        }

        public float DotTickHitDamage
        {
            get
            {
                return (BasePeriodicDamage + DotDamageCoefficient * RawSpellDamage) * SpellModifier * DotDamageModifier * DotTickInterval / DotDuration;
            }
        }

        public float DotTickCritDamage
        {
            get
            {
                if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire)
                {
                    return DotTickHitDamage * CritBonus;
                }
                else
                {
                    return DotTickHitDamage * CritBonus;
                }
            }
        }

        public const float GlobalCooldownLimit = 1.0f;
        public const float MaxHitRate = 1.0f;

        private Cycle cycle;
        public static implicit operator Cycle(Spell spell)
        {
            if (spell.cycle == null)
            {
                bool needsDisplayCalculations = spell.castingState.Solver.NeedsDisplayCalculations;
                spell.cycle = Cycle.New(needsDisplayCalculations, spell.castingState);
                spell.cycle.AddSpell(needsDisplayCalculations, spell, 1.0f);
                spell.cycle.Calculate();
                spell.cycle.Name = spell.Name;
                if (spell.AreaEffect || spell.AreaEffectDot)
                {
                    spell.cycle.AreaEffect = true;
                    //spell.cycle.AoeSpell = spell;
                }
            }
            return spell.cycle;
        }

        public Spell()
        {
        }

        public Spell(SpellTemplate template)
        {
            this.template = template;
        }

        public static Spell New(SpellTemplate template, Solver solver)
        {
            if (solver.NeedsDisplayCalculations || solver.ArraySet == null)
            {
                return new Spell(template);
            }
            else
            {
                Spell spell = solver.ArraySet.NewSpell();
                spell.Initialize(template);
                return spell;
            }
        }

        public static Spell NewFromReference(Spell reference, CastingState castingState)
        {
            Spell s = New(reference.template, castingState.Solver);
            s.castingState = castingState;

            s.BasePeriodicDamage = reference.BasePeriodicDamage;
            s.BaseCastTime = reference.BaseCastTime;
            s.CostModifier = reference.CostModifier;
            s.CostAmplifier = reference.CostAmplifier;
            s.SpellModifier = reference.SpellModifier;
            s.AdditiveSpellModifier = reference.AdditiveSpellModifier;
            s.DirectDamageModifier = reference.DirectDamageModifier;
            s.DotDamageModifier = reference.DotDamageModifier;
            s.CritRate = reference.CritRate;
            s.CritBonus = reference.CritBonus;
            s.RawSpellDamage = reference.RawSpellDamage;
            s.InterruptProtection = reference.InterruptProtection;
            s.AreaEffect = reference.AreaEffect;
            s.AreaEffectDot = reference.AreaEffectDot;
            s.MaximumAOETargets = reference.MaximumAOETargets;

            s.SpammedDot = reference.SpammedDot;
            s.Pom = reference.Pom;
            s.Ticks = reference.Ticks;
            s.CastProcs = reference.CastProcs;
            s.HitProcs = reference.HitProcs;
            s.CritProcs = reference.CritProcs;
            s.IgniteProcs = reference.IgniteProcs;
            s.TargetProcs = reference.TargetProcs;
            s.DotProcs = reference.DotProcs;
            s.ChannelReduction = reference.ChannelReduction;
            s.AverageCastingSpeed = reference.AverageCastingSpeed;
            s.CastTime = reference.CastTime;
            s.OO5SR = reference.OO5SR;
            s.AverageDamage = reference.AverageDamage;
            s.AverageThreat = reference.AverageThreat;
            s.AverageCost = reference.AverageCost;
            s.DamagePerSpellPower = reference.DamagePerSpellPower;
            s.DamagePerMastery = reference.DamagePerMastery;
            s.DamagePerCrit = reference.DamagePerCrit;
            // ignite tracking for separation in spell distribution
            s.IgniteDamage = reference.IgniteDamage;
            s.IgniteDamagePerSpellPower = reference.IgniteDamagePerSpellPower;
            s.IgniteDamagePerMastery = reference.IgniteDamagePerMastery;
            s.IgniteDamagePerCrit = reference.IgniteDamagePerCrit;
            // absorb spells
            s.Absorb = reference.Absorb;
            s.TotalAbsorb = reference.TotalAbsorb;
            // dot spells
            s.DotAverageDamage = reference.DotAverageDamage;
            s.DotAverageThreat = reference.DotAverageThreat;
            s.DotDamagePerSpellPower = reference.DotDamagePerSpellPower;
            s.DotDamagePerMastery = reference.DotDamagePerMastery;
            s.DotDamagePerCrit = reference.DotDamagePerCrit;
            s.DotFullDuration = reference.DotFullDuration;
            s.DotExtraTicks = reference.DotExtraTicks;

            s.RecalculateCastTime(castingState);

            return s;
        }

        public virtual void Calculate(CastingState castingState)
        {
            this.castingState = castingState;

            BasePeriodicDamage = template.BasePeriodicDamage;
            BaseCastTime = template.BaseCastTime;
            CostModifier = template.BaseCostModifier;
            CostAmplifier = template.BaseCostAmplifier;
            DirectDamageModifier = template.BaseDirectDamageModifier;
            DotDamageModifier = template.BaseDotDamageModifier * castingState.Solver.DarkIntentDotDamageAmplifier;
            if (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire)
            {
                DotDamageModifier *= (1 + castingState.FlashburnBonus);
            }
            if (castingState.PowerInfusion) CostModifier -= 0.2f; // don't have any information on this, going by best guess
            if (castingState.ArcanePower)
            {
                if (castingState.Solver.Mage4T12)
                {
                    CostAmplifier *= 0.9f;
                }
                else
                {
                    CostModifier += 0.1f;
                }
            }
            InterruptProtection = template.BaseInterruptProtection;

            SpellModifier = template.BaseSpellModifier * castingState.StateSpellModifier;
            AdditiveSpellModifier = template.BaseAdditiveSpellModifier + castingState.StateAdditiveSpellModifier;
            CritBonus = template.CritBonus;
            CritRate = template.BaseCritRate + castingState.StateCritRate;

            switch (MagicSchool)
            {
                case MagicSchool.Arcane:
                    RawSpellDamage = castingState.ArcaneSpellPower;
                    break;
                case MagicSchool.Fire:
                    RawSpellDamage = castingState.FireSpellPower;
                    break;
                case MagicSchool.FrostFire:
                    RawSpellDamage = castingState.FrostFireSpellPower;
                    break;
                case MagicSchool.Frost:
                    RawSpellDamage = castingState.FrostSpellPower;
                    break;
                case MagicSchool.Nature:
                    RawSpellDamage = castingState.NatureSpellPower;
                    break;
                case MagicSchool.Shadow:
                    RawSpellDamage = castingState.ShadowSpellPower;
                    break;
                case MagicSchool.Holy:
                    RawSpellDamage = castingState.HolySpellPower;
                    break;
            }
            RawSpellDamage *= (1 + castingState.BaseStats.BonusSpellPowerMultiplier);

            AreaEffect = template.AreaEffect;
            AreaEffectDot = template.AreaEffectDot;
            MaximumAOETargets = template.MaximumAOETargets;
        }

        public void CalculateManualClearcasting(bool manualClearcasting, bool clearcastingAveraged, bool clearcastingActive)
        {
            // if we want to continue manual clearcasting support we should completely overhaul this
            // because we would need to properly support lower chance of proc on AM and 2 charges of arcane potency
            if (manualClearcasting && !clearcastingAveraged)
            {
                CritRate -= 0.15f * castingState.Solver.ClearcastingChance * castingState.MageTalents.ArcanePotency; // replace averaged arcane potency with actual % chance
                if (clearcastingActive) CritRate += 0.15f * castingState.MageTalents.ArcanePotency;
            }
        }

        public static float ProcBuffUp(float procChance, float buffDuration, float triggerInterval)
        {
            if (triggerInterval <= 0)
                return 0;
            else
                return 1 - (float)Math.Pow(1 - procChance, buffDuration / triggerInterval);
        }

        public void CalculateDerivedStats(CastingState castingState)
        {
            CalculateDerivedStats(castingState, false, false, true, false, false, false, false);
        }

        public void CalculateDerivedStats(CastingState castingState, bool outOfFiveSecondRule, bool pom, bool spammedDot)
        {
            CalculateDerivedStats(castingState, outOfFiveSecondRule, pom, spammedDot, false, false, false, false);
        }

        public virtual void CalculateDerivedStats(CastingState castingState, bool outOfFiveSecondRule, bool pom, bool spammedDot, bool round, bool forceHit, bool forceMiss)
        {
            CalculateDerivedStats(castingState, outOfFiveSecondRule, pom, spammedDot, round, forceHit, forceMiss, false);
        }

        public virtual void CalculateDerivedStats(CastingState castingState, bool outOfFiveSecondRule, bool pom, bool spammedDot, bool round, bool forceHit, bool forceMiss, bool dotUptime)
        {
            MageTalents mageTalents = castingState.MageTalents;
            Stats baseStats = castingState.BaseStats;
            CalculationOptionsMage calculationOptions = castingState.CalculationOptions;

            if (AreaEffect)
            {
                // do not count debuffs for aoe effects, can't assume it will be up on all
                // do not include molten fury (molten fury relates to boss), instead amplify all by average
                if (castingState.MoltenFury)
                {
                    SpellModifier /= (1 + 0.04f * castingState.MageTalents.MoltenFury);
                }
                if (castingState.MageTalents.MoltenFury > 0)
                {
                    SpellModifier *= (1 + 0.04f * castingState.MageTalents.MoltenFury * castingState.CalculationOptions.MoltenFuryPercentage);
                }
            }

            SpellModifier *= AdditiveSpellModifier;

            if (CritRate < 0.0f) CritRate = 0.0f;
            if (CritRate > 1.0f) CritRate = 1.0f;

            Ticks = template.Ticks;
            CastProcs = template.CastProcs;
            HitProcs = Ticks * HitRate;
            if (AreaEffect)
            {
                TargetProcs = HitProcs * castingState.CalculationOptions.AoeTargets;
            }
            else
            {
                TargetProcs = HitProcs;
            }

            Pom = pom;
            if (Instant) InterruptProtection = 1;

            CastTime = template.CalculateCastTime(castingState, InterruptProtection, CritRate, pom, BaseCastTime, out ChannelReduction, out AverageCastingSpeed);

            // add crit rate for on use stacking crit effects (would be better if it was computed
            // on cycle level, but right now the architecture doesn't allow that too well)
            // we'd actually need some iterations of this as cast time can depend on crit etc, just ignore that for now
            for (int i = 0; i < castingState.Solver.StackingNonHasteEffectCooldownsCount; i++)
            {
                EffectCooldown effectCooldown = castingState.Solver.StackingNonHasteEffectCooldowns[i];
                if (castingState.EffectsActive(effectCooldown.Mask))
                {
                    Stats stats = effectCooldown.SpecialEffect.Stats;
                    for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                    {
                        SpecialEffect effect = stats._rawSpecialEffectData[j];
                        if (effect.Chance == 1f && effect.Cooldown == 0f && (effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.SpellCrit))
                        {
                            if (effect.Stats.CritRating < 0 && effectCooldown.SpecialEffect.Stats.CritRating > 0)
                            {
                                float critScale = castingState.CalculationOptions.LevelScalingFactor / 1400f;
                                CritRate += SpecialEffect.GetAverageStackingCritRate(CastTime, effectCooldown.SpecialEffect.Duration, HitRate, CritRate, effectCooldown.SpecialEffect.Stats.CritRating * critScale, effect.Stats.CritRating * critScale, effect.MaxStack);
                                if (CritRate > 1.0f) CritRate = 1.0f;
                            }
                        }
                    }
                }
            }

            if (castingState.Frozen)
            {
                CritRate *= (1 + castingState.MageTalents.Shatter);
                if (CritRate > 1.0f) CritRate = 1.0f;
            }

            CritProcs = HitProcs * CritRate;
            if ((MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) && mageTalents.Ignite > 0)
            {
                IgniteProcs = CritProcs;
            }
            else
            {
                IgniteProcs = 0;
            }

            if (DotTickInterval > 0)
            {
                // non-spammed we have to take into account haste on dot duration and increase in number of ticks
                // probably don't want to take into account haste procs as that might skew optimization thresholds as we're averaging cast time over procs
                // reevaluate this if needed
                //float x = DotTickInterval / DotDuration;
                //DotExtraTicks = (float)Math.Floor((castingState.CastingSpeed - 1) / x + 0.5);
                DotExtraTicks = (float)Math.Round(DotDuration / Math.Round(DotTickInterval / AverageCastingSpeed, 3)) - DotDuration / DotTickInterval;
                DotFullDuration = (DotDuration + DotTickInterval * DotExtraTicks) / AverageCastingSpeed;
                if (spammedDot)
                {
                    // spammed dots no longer clip on reapplication
                    DotProcs = Math.Min(CastTime, DotDuration) / DotTickInterval;
                }
                else
                {
                    DotProcs = DotDuration / DotTickInterval + DotExtraTicks;
                }
            }
            else
            {
                DotProcs = 0;
                DotFullDuration = 0;
                DotExtraTicks = 0;
            }

            SpammedDot = spammedDot;
            if ((BaseMinDamage > 0 || BasePeriodicDamage > 0) && !forceMiss)
            {
                if (dotUptime)
                {
                    CalculateDirectAverageDamage(castingState.Solver, RawSpellDamage, forceHit);
                    AverageThreat = AverageDamage * ThreatMultiplier;

                    CalculateDotAverageDamage(castingState.Solver, RawSpellDamage, forceHit);
                    DotAverageThreat = DotAverageDamage * ThreatMultiplier;
                }
                else
                {
                    CalculateAverageDamage(castingState.Solver, RawSpellDamage, spammedDot, forceHit);
                    AverageThreat = AverageDamage * ThreatMultiplier;
                }
            }
            else
            {
                AverageDamage = 0;
                AverageThreat = 0;
                DamagePerSpellPower = 0;
                DamagePerMastery = 0;
                DamagePerCrit = 0;
                IgniteDamage = 0;
                IgniteDamagePerSpellPower = 0;
                IgniteDamagePerMastery = 0;
                IgniteDamagePerCrit = 0;
                if (dotUptime)
                {
                    DotAverageDamage = 0;
                    DotAverageThreat = 0;
                    DotDamagePerSpellPower = 0;
                    DotDamagePerMastery = 0;
                    DotDamagePerCrit = 0;
                }
            }
            if (ChannelReduction != 0)
            {
                Ticks *= (1 - ChannelReduction);
                HitProcs *= (1 - ChannelReduction);
                CritProcs *= (1 - ChannelReduction);
                TargetProcs *= (1 - ChannelReduction);
                CastProcs = CastProcs2 + (CastProcs - CastProcs2) * (1 - ChannelReduction);
                AverageDamage *= (1 - ChannelReduction);
                AverageThreat *= (1 - ChannelReduction);
                DamagePerSpellPower *= (1 - ChannelReduction);
                DamagePerMastery *= (1 - ChannelReduction);
                DamagePerCrit *= (1 - ChannelReduction);
            }
            AverageCost = CalculateCost(castingState.Solver, round);

            Absorb = 0;
            TotalAbsorb = 0;

            if (outOfFiveSecondRule)
            {
                OO5SR = 1;
            }
            else
            {
                OO5SR = 0;
            }
        }

        public virtual void RecalculateCastTime(CastingState castingState)
        {
            if (ChannelReduction != 0)
            {
                Ticks /= (1 - ChannelReduction);
                HitProcs /= (1 - ChannelReduction);
                CritProcs /= (1 - ChannelReduction);
                TargetProcs /= (1 - ChannelReduction);
                CastProcs = template.CastProcs;
                AverageDamage /= (1 - ChannelReduction);
                AverageThreat /= (1 - ChannelReduction);
                DamagePerSpellPower /= (1 - ChannelReduction);
                DamagePerMastery /= (1 - ChannelReduction);
                DamagePerCrit /= (1 - ChannelReduction);
            }
            CastTime = template.CalculateCastTime(castingState, InterruptProtection, CritRate, Pom, BaseCastTime, out ChannelReduction, out AverageCastingSpeed);
            if (ChannelReduction != 0)
            {
                Ticks *= (1 - ChannelReduction);
                HitProcs *= (1 - ChannelReduction);
                CritProcs *= (1 - ChannelReduction);
                TargetProcs *= (1 - ChannelReduction);
                CastProcs = template.CastProcs2 + (template.CastProcs - template.CastProcs2) * (1 - ChannelReduction);
                AverageDamage *= (1 - ChannelReduction);
                AverageThreat *= (1 - ChannelReduction);
                DamagePerSpellPower *= (1 - ChannelReduction);
                DamagePerMastery *= (1 - ChannelReduction);
                DamagePerCrit *= (1 - ChannelReduction);
            }
        }

        public virtual void CalculateAverageDamage(Solver solver, float spellPower, bool spammedDot, bool forceHit)
        {
            // in cata all dots can crit (starting with 4.1 they no longer ignite)
            float baseAverage = (BaseMinDamage + BaseMaxDamage) / 2f;
            float critBonus = CritBonus;
            float igniteFactor = template.IgniteFactor * (1 + castingState.FlashburnBonus);
            float critMultiplier = 1 + (critBonus * (1 + igniteFactor) - 1) * Math.Max(0, CritRate);
            float dotCritMultiplier = 1 + (critBonus - 1) * Math.Max(0, CritRate);
            float resistMultiplier = (forceHit ? 1.0f : HitRate) * PartialResistFactor;
            float commonMultiplier = SpellModifier * resistMultiplier;
            float nukeMultiplier = commonMultiplier * DirectDamageModifier * critMultiplier;
            if (AreaEffect)
            {
                nukeMultiplier *= 1 + (Math.Min(MaximumAOETargets, solver.CalculationOptions.AoeTargets) - 1) * AOEMultiplier;
            }
            float averageDamage = baseAverage * nukeMultiplier;
            DamagePerSpellPower = SpellDamageCoefficient * nukeMultiplier;
            if ((solver.NeedsDisplayCalculations || solver.Specialization == Specialization.Fire) && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) && solver.MageTalents.Ignite > 0)
            {
                float igniteMultiplier = nukeMultiplier / critMultiplier * critBonus * igniteFactor * Math.Max(0, CritRate);
                IgniteDamage = (baseAverage + SpellDamageCoefficient * spellPower) * igniteMultiplier;
                IgniteDamagePerSpellPower = SpellDamageCoefficient * igniteMultiplier;
                IgniteDamagePerCrit = IgniteDamage / CritRate;
                if (solver.Specialization == Specialization.Fire)
                {
                    DamagePerMastery = IgniteDamage / (1 + castingState.FlashburnBonus) * solver.FlashburnMultiplier;
                    IgniteDamagePerMastery = DamagePerMastery;
                }
                else
                {
                    DamagePerMastery = 0;
                    IgniteDamagePerMastery = 0;
                }
            }
            else
            {
                IgniteDamage = 0;
                IgniteDamagePerSpellPower = 0;
                IgniteDamagePerMastery = 0;
                IgniteDamagePerCrit = 0;
                DamagePerMastery = 0;
            }
            float totalDamage = averageDamage + DamagePerSpellPower * spellPower;
            DamagePerCrit = totalDamage / critMultiplier * (critBonus * (1 + igniteFactor) - 1); // part from direct damage
            if (BasePeriodicDamage > 0.0f)
            {
                float dotFactor = commonMultiplier * DotDamageModifier * dotCritMultiplier;
                if (AreaEffectDot)
                {
                    dotFactor *= 1 + (Math.Min(MaximumAOETargets, solver.CalculationOptions.AoeTargets) - 1) * AOEMultiplier;
                }
                if (spammedDot)
                {
                    // spammed dots no longer clip on reapplication
                    dotFactor *= Math.Min(CastTime / DotDuration, 1.0f);
                }
                else
                {
                    // take into account extra ticks
                    dotFactor *= (1 + DotExtraTicks * DotTickInterval / DotDuration);
                }
                averageDamage += BasePeriodicDamage * dotFactor;
                DamagePerSpellPower += DotDamageCoefficient * dotFactor;
                float periodicDamage = (BasePeriodicDamage + DotDamageCoefficient * spellPower) * dotFactor;
                if (solver.Specialization == Specialization.Fire && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire))
                {
                    DamagePerMastery += periodicDamage / (1 + castingState.FlashburnBonus) * solver.FlashburnMultiplier;
                }
                totalDamage = averageDamage + DamagePerSpellPower * spellPower;
                DamagePerCrit += periodicDamage / dotCritMultiplier * (critBonus - 1); // part from dot damage
            }
            if (solver.Specialization == Specialization.Frost && castingState.Frozen)
            {
                DamagePerMastery = totalDamage / AdditiveSpellModifier * solver.FrostburnMultiplier;
            }
            AverageDamage = totalDamage;
        }

        public virtual void CalculateDirectAverageDamage(Solver solver, float spellPower, bool forceHit)
        {
            float baseAverage = (BaseMinDamage + BaseMaxDamage) / 2f;
            float critBonus = CritBonus;
            float igniteFactor = template.IgniteFactor * (1 + castingState.FlashburnBonus);
            float critMultiplier = 1 + (critBonus * (1 + igniteFactor) - 1) * Math.Max(0, CritRate);
            float resistMultiplier = (forceHit ? 1.0f : HitRate) * PartialResistFactor;
            float commonMultiplier = SpellModifier * resistMultiplier;
            float nukeMultiplier = commonMultiplier * DirectDamageModifier * critMultiplier;
            if (AreaEffect)
            {
                nukeMultiplier *= 1 + (Math.Min(MaximumAOETargets, solver.CalculationOptions.AoeTargets) - 1) * AOEMultiplier;
            }
            float averageDamage = baseAverage * nukeMultiplier;
            DamagePerSpellPower = SpellDamageCoefficient * nukeMultiplier;
            if ((solver.NeedsDisplayCalculations || solver.Specialization == Specialization.Fire) && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire) && solver.MageTalents.Ignite > 0)
            {
                float igniteMultiplier = nukeMultiplier / critMultiplier * critBonus * igniteFactor * Math.Max(0, CritRate);
                IgniteDamage = (baseAverage + SpellDamageCoefficient * spellPower) * igniteMultiplier;
                IgniteDamagePerSpellPower = SpellDamageCoefficient * igniteMultiplier;
                IgniteDamagePerCrit = IgniteDamage / CritRate;
                if (solver.Specialization == Specialization.Fire)
                {
                    DamagePerMastery = IgniteDamage / (1 + castingState.FlashburnBonus) * solver.FlashburnMultiplier;
                    IgniteDamagePerMastery = DamagePerMastery;
                }
                else
                {
                    DamagePerMastery = 0;
                    IgniteDamagePerMastery = 0;
                }
            }
            else
            {
                IgniteDamage = 0;
                IgniteDamagePerSpellPower = 0;
                IgniteDamagePerMastery = 0;
                IgniteDamagePerCrit = 0;
                DamagePerMastery = 0;
            }
            float totalDamage = averageDamage + DamagePerSpellPower * spellPower;
            if (solver.Specialization == Specialization.Frost && castingState.Frozen)
            {
                DamagePerMastery = totalDamage / AdditiveSpellModifier * solver.FrostburnMultiplier;
            }
            DamagePerCrit = totalDamage / critMultiplier * (critBonus - 1);
            AverageDamage = totalDamage;
        }

        public virtual void CalculateDotAverageDamage(Solver solver, float spellPower, bool forceHit)
        {
            float resistMultiplier = (forceHit ? 1.0f : HitRate) * PartialResistFactor;
            float critBonus = CritBonus;
            float igniteFactor = template.IgniteFactor * (1 + castingState.FlashburnBonus);
            float critMultiplier;
            critMultiplier = 1 + (critBonus - 1) * Math.Max(0, CritRate);
            float commonMultiplier = SpellModifier * resistMultiplier * critMultiplier;
            float averageDamage = 0.0f;
            DotDamagePerSpellPower = 0.0f;
            DotDamagePerMastery = 0.0f;
            if (BasePeriodicDamage > 0.0f)
            {
                float dotFactor = commonMultiplier * DotDamageModifier;
                if (AreaEffectDot)
                {
                    dotFactor *= 1 + (Math.Min(MaximumAOETargets, solver.CalculationOptions.AoeTargets) - 1) * AOEMultiplier;
                }
                // take into account extra ticks
                dotFactor *= (1 + DotExtraTicks * DotTickInterval / DotDuration);
                averageDamage = BasePeriodicDamage * dotFactor;
                DotDamagePerSpellPower = DotDamageCoefficient * dotFactor;
            }
            float totalDamage = averageDamage + DotDamagePerSpellPower * spellPower;
            if (solver.Specialization == Specialization.Frost && castingState.Frozen)
            {
                DotDamagePerMastery = totalDamage / AdditiveSpellModifier * solver.FrostburnMultiplier;
            }
            if (solver.Specialization == Specialization.Fire && (MagicSchool == MagicSchool.Fire || MagicSchool == MagicSchool.FrostFire))
            {
                DotDamagePerMastery += totalDamage / (1 + castingState.FlashburnBonus) * solver.FlashburnMultiplier;
            }
            DotDamagePerCrit = totalDamage / critMultiplier * (critBonus - 1);
            DotAverageDamage = totalDamage;
        }

        protected float CalculateCost(Solver solver, bool round)
        {
            float cost;
            if (round)
            {
                cost = (float)Math.Floor(Math.Round(BaseCost * CostAmplifier) * CostModifier);
            }
            else
            {
                cost = (float)Math.Floor(BaseCost * CostAmplifier * CostModifier);
            }

            cost *= (1 - solver.ClearcastingChance);

            // from what I know MOE works on base cost
            // not tested, but I think if you get MOE proc on a spell while CC is active you still get mana return
            if (!solver.CalculationOptions.EffectDisableManaSources)
            {
                if (AreaEffect)
                {
                    // if any target crits we get a return on MoE
                    cost -= (float)(1 - Math.Pow(1 - CritRate, solver.CalculationOptions.AoeTargets)) * BaseCost * 0.15f * solver.MageTalents.MasterOfElements;
                }
                else
                {
                    cost -= CritRate * BaseCost * 0.15f * solver.MageTalents.MasterOfElements;
                }
                // Judgement of Wisdom
                // this is actually a PPM
                //cost -= template.BaseUntalentedCastTime / 60f * solver.BaseStats.ManaRestoreFromBaseManaPPM * solver.CalculationOptions.BaseMana;
            }
            return cost;
        }

        public void CalculateManualClearcastingCost(Solver solver, bool round, bool manualClearcasting, bool clearcastingAveraged, bool clearcastingActive)
        {
            float cost;
            if (round)
            {
                cost = (float)Math.Floor(Math.Round(BaseCost * CostAmplifier) * CostModifier);
            }
            else
            {
                cost = (float)Math.Floor(BaseCost * CostAmplifier * CostModifier);
            }

            if (!manualClearcasting || clearcastingAveraged)
            {
                cost *= (1 - solver.ClearcastingChance);
            }
            else if (clearcastingActive)
            {
                cost = 0;
            }

            // from what I know MOE works on base cost
            // not tested, but I think if you get MOE proc on a spell while CC is active you still get mana return
            if (!solver.CalculationOptions.EffectDisableManaSources)
            {
                cost -= CritRate * BaseCost * 0.15f * solver.MageTalents.MasterOfElements;
                // Judgement of Wisdom
                // this is actually a PPM
                //cost -= template.BaseUntalentedCastTime / 60f * solver.BaseStats.ManaRestoreFromBaseManaPPM * solver.CalculationOptions.BaseMana;
            }
            AverageCost = cost;
        }

        public void AddSpellContribution(Dictionary<string, SpellContribution> dict, float count, float dotUptime, float effectSpellPower, float effectMastery, float effectCrit, float effectManaAdeptMultiplier, float averageMana)
        {
            SpellContribution contrib;
            if (!dict.TryGetValue(Name, out contrib))
            {
                contrib = new SpellContribution() { Name = Name };
                dict[Name] = contrib;
            }
            float igniteContribution = 0;
            float critBonus = CritBonus;
            if (IgniteDamage > 0)
            {
                igniteContribution = (IgniteDamage + effectSpellPower * IgniteDamagePerSpellPower + effectMastery * IgniteDamagePerMastery + effectCrit * IgniteDamagePerCrit) * count;
                SpellContribution igniteContrib;
                if (!dict.TryGetValue("Ignite", out igniteContrib))
                {
                    igniteContrib = new SpellContribution() { Name = "Ignite" };
                    dict["Ignite"] = igniteContrib;
                }
                igniteContrib.Damage += igniteContribution;
            }
            // adjust crits by effectCrit
            contrib.Hits += (HitProcs - CritProcs - effectCrit / HitProcs) * count;
            contrib.Crits += (CritProcs + effectCrit / HitProcs) * count;
            float damage = (AverageDamage + effectSpellPower * DamagePerSpellPower + effectMastery * DamagePerMastery + effectCrit * DamagePerCrit) * count - igniteContribution;
            if (castingState.Solver.Specialization == Specialization.Arcane)
            {
                double manaAdeptBonus = castingState.ManaAdeptBonus + 0.015f * effectMastery;
                float spellMultiplier = (float)(1 + averageMana / castingState.BaseStats.Mana * manaAdeptBonus * effectManaAdeptMultiplier);
                damage *= spellMultiplier;
            }
            contrib.Damage += damage;
            if (dotUptime > 0)
            {
                float tickDamage = dotUptime * (DotAverageDamage + effectSpellPower * DotDamagePerSpellPower + effectMastery * DotDamagePerMastery + effectCrit * DotDamagePerCrit) * count;
                contrib.Damage += tickDamage;
                // dotUptime = DotProcs / (DotDuration / DotTickInterval)
                contrib.Ticks += dotUptime * (DotDuration / DotTickInterval) * count;
                contrib.TickDamage += tickDamage;
            }
            else
            {
                if (DotTickInterval > 0)
                {
                    contrib.Ticks += DotProcs * count;
                    float dotFactor = DotProcs / (DotDuration / DotTickInterval) * SpellModifier * DotDamageModifier * PartialResistFactor * HitRate * (1 + (CritBonus - 1) * Math.Max(0, CritRate));
                    float tickDamage = dotFactor * (BasePeriodicDamage + (RawSpellDamage + effectSpellPower) * DotDamageCoefficient) * count;
                    contrib.TickDamage += tickDamage;
                    damage -= tickDamage;
                }
            }
            // damage = baseDamage * (1 + (CritBonus - 1) * CritRate)
            float baseDamage = damage / (1 + (critBonus - 1) * (CritRate + effectCrit));
            contrib.HitDamage += baseDamage * (1 - CritRate - effectCrit);
            contrib.CritDamage += baseDamage * (CritRate + effectCrit) * critBonus;
            contrib.Range = Range;
        }

        public void AddManaUsageContribution(Dictionary<string, float> dict, float count)
        {
            float contrib;
            dict.TryGetValue(Name, out contrib);
            contrib += AverageCost * count;
            dict[Name] = contrib;
        }
    }
}
