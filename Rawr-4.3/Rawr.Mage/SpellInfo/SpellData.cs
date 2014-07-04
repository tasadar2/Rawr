﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Rawr.Mage
{
    public enum SpellId
    {
        None,
        [Description("Arcane Barrage (0)")]
        ArcaneBarrage,
        [Description("Arcane Barrage (1)")]
        ArcaneBarrage1,
        [Description("Arcane Barrage (2)")]
        ArcaneBarrage2,
        [Description("Arcane Barrage (3)")]
        ArcaneBarrage3,
        [Description("Arcane Barrage (4)")]
        ArcaneBarrage4,
        [Description("Arcane Missiles (0)")]
        ArcaneMissiles,
        [Description("Arcane Missiles (1)")]
        ArcaneMissiles1,
        [Description("Arcane Missiles (2)")]
        ArcaneMissiles2,
        [Description("Arcane Missiles (3)")]
        ArcaneMissiles3,
        [Description("Arcane Missiles (4)")]
        ArcaneMissiles4,
        [Description("MBAM (0)")]
        ArcaneMissilesMB,
        [Description("MBAM (1)")]
        ArcaneMissilesMB1,
        [Description("MBAM (2)")]
        ArcaneMissilesMB2,
        [Description("MBAM (3)")]
        ArcaneMissilesMB3,
        [Description("MBAM (4)")]
        ArcaneMissilesMB4,
        ArcaneMissilesNoProc,
        [Description("Frostbolt")]
        Frostbolt,
        [Description("POM+Frostbolt")]
        FrostboltPOM,
        FrostboltNoCC,
        [Description("Deep Freeze")]
        DeepFreeze,
        [Description("Fireball")]
        Fireball,
        [Description("POM+Fireball")]
        FireballPOM,
        FireballBF,
        FrostfireBoltBF,
        [Description("Frostfire Bolt")]
        FrostfireBolt,
        FrostfireBoltFC,
        [Description("Pyroblast")]        
        PyroblastSpammed,
        PyroblastDotUptime,
        [Description("Pyroblast!")]
        PyroblastPOM,
        PyroblastPOMSpammed,
        PyroblastPOMDotUptime,
        [Description("Fire Blast")]
        FireBlast,
        [Description("Flame Orb")]
        FlameOrb,
        Combustion,
        [Description("Scorch")]
        Scorch,
        ScorchNoCC,
        [Description("Living Bomb")]
        LivingBomb,
        [Description("Living Bomb AOE")]
        LivingBombAOE,
        ArcaneBomb,
        [Description("Arcane Bomb AOE")]
        ArcaneBombAOE,
        ArcaneBlast3NoCC,
        ArcaneBlastRaw,
        [Description("Arcane Blast (0)")]
        ArcaneBlast0,
        ArcaneBlast0NoCC,
        ArcaneBlast0POM,
        [Description("Arcane Blast (1)")]
        ArcaneBlast1,
        ArcaneBlast1NoCC,
        [Description("Arcane Blast (2)")]
        ArcaneBlast2,
        [Description("Arcane Blast (3)")]
        ArcaneBlast3,
        [Description("Arcane Blast (4)")]
        ArcaneBlast4,
        ArcaneBlast2NoCC,
        ArcaneBlast0Hit,
        ArcaneBlast1Hit,
        ArcaneBlast2Hit,
        ArcaneBlast3Hit,
        ArcaneBlast0Miss,
        ArcaneBlast1Miss,
        ArcaneBlast2Miss,
        ArcaneBlast3Miss,
        Slow,
        IceLance,
        [Description("Arcane Explosion")]
        ArcaneExplosion0,
        ArcaneExplosion1,
        ArcaneExplosion2,
        ArcaneExplosion3,
        ArcaneExplosion4,
        FlamestrikeSpammed,
        [Description("Flamestrike")]
        FlamestrikeSingle,
        [Description("Blizzard")]
        Blizzard,
        [Description("Blast Wave")]
        BlastWave,
        [Description("Dragon's Breath")]
        DragonsBreath,
        [Description("Cone of Cold")]
        ConeOfCold,
        MageWard,
        Waterbolt,
        MirrorImage,
    }

    // spell id: 31707, scaling id: 650
    public class WaterboltTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Waterbolt";
            InitializeCastTime(false, false, 2.5f, 0);
            InitializeScaledDamage(solver, false, 45, MagicSchool.Frost, 0.01f, 0.405000001192093f, 0.25f, 0, 0.833000004291534f, 0, 1, 1, 0);
            if (solver.Character.Race == CharacterRace.Orc)
            {
                BaseSpellModifier *= 1.05f;
            }
            // TODO recheck all buffs that apply
            Dirty = false;
        }

        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.RawSpellDamage *= 0.4f;
            spell.CalculateDerivedStats(castingState);
            spell.DamagePerSpellPower *= 0.4f;
            return spell;
        }
    }

    // spell id: 88084, scaling id: 674 (Arcane Blast)  0.275000005960464
    // spell id: 88082, scaling id: 673 (Fireball)      0.337999999523163
    // spell id: 59637, scaling id: -   (Fire Blast)    0.150000005960464 (base doesn't scale with level, fixed at 88-98?)
    // spell id: 59638, scaling id: 672 (Frostbolt)     0.25
    public class MirrorImageTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Mirror Image";
            if (solver.Specialization == Specialization.Arcane && solver.MageTalents.GlyphOfMirrorImage)
            {
                // TODO should do something about AB debuff stacking
                InitializeCastTime(false, false, 2.5f, 0);
                InitializeScaledDamage(solver, false, 40, MagicSchool.Arcane, 0, 0.25900000333786f, 0.150000005960464f, 0, 0.275000005960464f, 0, 0, 0, 0);
            }
            else if (solver.Specialization == Specialization.Fire && solver.MageTalents.GlyphOfMirrorImage)
            {
                InitializeCastTime(false, false, 2.5f, 0);
                InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0, 0.328000009059906f, 0.241999998688698f, 0, 0.337999999523163f, 0, 0, 0, 0);
                IgniteFactor = 0;
            }
            else
            {
                // 10 * Frostbolt + 4 * Fire Blast
                InitializeCastTime(false, false, 10f * 2f + 4f * 1.5f, 0);
                var options = solver.CalculationOptions;
                var average = options.GetSpellValue(0.234999999403954f);
                var halfSpread = (float)Math.Floor(average * 0.100000001490116f / 2f);
                average = (float)Math.Round(average);
                InitializeDamage(solver, false, 20, MagicSchool.Frost, 0, 10f * (average - halfSpread) + 4f * 88f, 10f * (average + halfSpread) + 4f * 98f, 10f * 0.25f + 4f * 0.150000005960464f, 0, 0, 0, 0, 0);
            }
            // TODO recheck all buffs that apply
            Dirty = false;
        }

        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.RawSpellDamage *= 0.4f;
            spell.CalculateDerivedStats(castingState);
            spell.DamagePerSpellPower *= 0.4f;
            return spell;
        }
    }

    public class WandTemplate : SpellTemplate
    {
        private float speed;

        public WandTemplate()
        {
        }

        public WandTemplate(Solver solver, MagicSchool school, int minDamage, int maxDamage, float speed)
        {
            Initialize(solver, school, minDamage, maxDamage, speed);
        }

        public void Initialize(Solver solver, MagicSchool school, int minDamage, int maxDamage, float speed)
        {
            Name = "Wand";
            // Tested: affected by Arcane Instability, affected by Chaotic meta, not affected by Arcane Power
            InitializeEffectDamage(solver, school, minDamage, maxDamage);
            Range = 30;
            this.speed = speed;
            CritBonus = 1.5f * 1.33f * (1 + solver.BaseStats.BonusSpellCritDamageMultiplier);
            BaseSpellModifier = (1 + solver.BaseStats.BonusDamageMultiplier);
            switch (school)
            {
                case MagicSchool.Arcane:
                    BaseSpellModifier *= (1 + solver.BaseStats.BonusArcaneDamageMultiplier);
                    break;
                case MagicSchool.Fire:
                    BaseSpellModifier *= (1 + solver.BaseStats.BonusFireDamageMultiplier);
                    break;
                case MagicSchool.Frost:
                    BaseSpellModifier *= (1 + solver.BaseStats.BonusFrostDamageMultiplier);
                    break;
                case MagicSchool.Nature:
                    BaseSpellModifier *= (1 + solver.BaseStats.BonusNatureDamageMultiplier);
                    break;
                case MagicSchool.Shadow:
                    BaseSpellModifier *= (1 + solver.BaseStats.BonusShadowDamageMultiplier);
                    break;
            }
        }

        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CastTime = speed;
            spell.CritRate = castingState.CritRate;

            if (spell.CritRate < 0.0f) spell.CritRate = 0.0f;
            if (spell.CritRate > 1.0f) spell.CritRate = 1.0f;

            spell.SpellModifier = BaseSpellModifier;

            spell.HitProcs = HitRate;
            spell.CritProcs = spell.HitProcs * spell.CritRate;
            spell.TargetProcs = spell.HitProcs;

            spell.CalculateAverageDamage(castingState.Solver, 0, false, false);
            spell.DamagePerSpellPower = 0;
            spell.DamagePerMastery = 0;
            spell.DamagePerCrit = 0;
            spell.AverageThreat = spell.AverageDamage * ThreatMultiplier;
            spell.IgniteDamage = 0;
            spell.IgniteDamagePerSpellPower = 0;
            spell.IgniteDamagePerMastery = 0;
            spell.IgniteDamagePerCrit = 0;
            spell.AverageCost = 0;
            spell.OO5SR = 1;
            return spell;
        }
    }

    // spell id: 82739/83619, scaling id: 572/583
    public class FlameOrbTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            // TODO consider splitting explosion out for display purposes
            Name = "Flame Orb";
            InitializeCastTime(false, true, 0, 60);
            float explosion = solver.MageTalents.FirePower == 3 ? 1f : 0.33f * solver.MageTalents.FirePower;
            InitializeScaledDamage(solver, false, 40, solver.MageTalents.FrostfireOrb > 0 ? MagicSchool.FrostFire : MagicSchool.Fire, 0.06f, 15 * 0.277999997138977f + explosion * 1.317999958992f, 0.25f, 0, 15 * 0.13400000333786f + explosion * 0.193000003695488f, 0, 15 + explosion, 15 + explosion, 0);
            IgniteFactor = 0;
            BaseDirectDamageModifier *= (1f + 0.05f * solver.MageTalents.CriticalMass);
            BaseDotDamageModifier += 0.05f * solver.MageTalents.CriticalMass; // additive with mastery
            Dirty = false;
        }
    }

    // spell id: 2948, scaling id: 27
    public class ScorchTemplate : SpellTemplate
    {
        public virtual Spell GetSpell(CastingState castingState, bool clearcastingActive)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateManualClearcasting(true, false, clearcastingActive);
            spell.CalculateDerivedStats(castingState);
            spell.CalculateManualClearcastingCost(castingState.Solver, false, true, false, clearcastingActive);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Scorch";
            InitializeCastTime(false, false, 1.5f, 0);
            InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0.08f, 0.781000018119812f, 0.170000001788139f, 0, 0.512000024318695f, 0, 1, 1, 0);
            BaseCostAmplifier *= (1 - 0.5f * solver.MageTalents.ImprovedScorch);
            if (solver.Mage4PVP)
            {
                BaseSpellModifier *= 1.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 2120, scaling id: 19
    public class FlamestrikeTemplate : SpellTemplate
    {
        public virtual Spell GetSpell(CastingState castingState, bool spammedDot)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateDerivedStats(castingState, false, false, spammedDot);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Flamestrike";
            InitializeCastTime(false, false, 2, 0);
            InitializeScaledDamage(solver, true, 40, MagicSchool.Fire, 0.3f, 0.662000000476837f, 0.202000007033348f, 4 * 0.103000000119209f, 0.145999997854233f, 4 * 0.0610000006854534f, 1, 1, 8f);
            DotTickInterval = 2;
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    public class ConjureManaGemTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Conjure Mana Gem";
            InitializeCastTime(false, false, 3, 0);
            InitializeScaledDamage(solver, false, 0, MagicSchool.Arcane, 0.75f, 0, 0, 0, 0, 0, 0, 1, 0);
            Dirty = false;
        }
    }

    public class MageWardTemplate : SpellTemplate
    {
        private const float spellPowerCoefficient = 0.807f;

        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateDerivedStats(castingState);
            // 70% absorbed, 30% negated
            // number of negates until absorb is distributed negative binomial
            // mean number of negated is then (1-p)/p = 0.3 / 0.7 times the absorb value
            // however on average it can't be more than (1-p) * incoming damage
            float q = 0f;
            float absorb = castingState.CalculationOptions.GetSpellValue(2.32399988174438f) + spellPowerCoefficient * castingState.ArcaneSpellPower;
            spell.Absorb = absorb;
            // in 3.3.3 warding doesn't count as absorb for IA, assume that we'll get to normal absorb at least once in 30 sec (i.e. we're not lucky enough to continue proccing warding for the whole 30 sec)
            float dps = (float)(castingState.Solver.IncomingDamageDpsFire + castingState.Solver.IncomingDamageDpsArcane + castingState.Solver.IncomingDamageDpsFrost);
            spell.TotalAbsorb = Math.Min(absorb, 30f * dps);
            //spell.TotalAbsorb = Math.Min((1 + q / (1 - q)) * absorb, 30f * dps);
            spell.AverageCost -= Math.Min(q / (1 - q) * absorb, q * 30f * dps);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Mage Ward";
            InitializeCastTime(false, true, 0, 30);
            InitializeScaledDamage(solver, false, 0, MagicSchool.Arcane, 0.16f, 0, 0, 0, 0, 0, 0, 1, 0);
            Dirty = false;
        }
    }

    // spell id: 122, scaling id: 20
    public class FrostNovaTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Frost Nova";
            InitializeCastTime(false, true, 0, 25);
            InitializeScaledDamage(solver, true, 0, MagicSchool.Frost, 0.07f, 0.42399999499321f, 0.150000005960464f, 0, 0.193000003695488f, 0, 1, 1, 0);
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 116, scaling id: 21
    public class FrostboltTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool manualClearcasting, bool clearcastingActive, bool pom)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (manualClearcasting) spell.CalculateManualClearcasting(true, false, clearcastingActive);
            if (castingState.Frozen)
            {
                spell.SpellModifier *= (1f + 0.1f * castingState.MageTalents.Shatter);
            }
            spell.CalculateDerivedStats(castingState, false, pom, false);
            if (manualClearcasting) spell.CalculateManualClearcastingCost(castingState.Solver, false, true, false, clearcastingActive);
            return spell;
        }

        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (castingState.Frozen)
            {
                spell.SpellModifier *= (1f + 0.1f * castingState.MageTalents.Shatter);
            }
            spell.CalculateDerivedStats(castingState);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Frostbolt";
            InitializeCastTime(false, false, 2f, 0);
            if (solver.Mage4T11)
            {
                BaseCastTime *= 0.9f;
            }
            InitializeScaledDamage(solver, false, 35, MagicSchool.Frost, 0.13f, 0.8844000220298771f, 0.241999998688698f, 0, 0.9430000186f, 0, 1, 1, 0);
            if (solver.MageTalents.GlyphOfFrostbolt)
            {
                BaseCritRate += 0.05f;
            }
            if (solver.Specialization == Specialization.Frost)
            {
                BaseAdditiveSpellModifier += 0.15f;
            }
            //BaseCritRate += 0.05f * solver.BaseStats.Mage4T9;
            NukeProcs = 1;
            NukeProcs2 = 1;
            if (solver.Mage4PVP)
            {
                BaseSpellModifier *= 1.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 71757, scaling id: 408
    public class DeepFreezeTemplate : SpellTemplate
    {
        //float fingersOfFrostCritRate;

        // 30 sec cooldown!!!
        public void Initialize(Solver solver)
        {
            Name = "Deep Freeze";
            InitializeCastTime(false, true, 0, 30);
            InitializeScaledDamage(solver, false, 35, MagicSchool.Frost, 0.09f, 1.3919999599f, 0.224999994039536f, 0, 2.0580000877f, 0, 1, 1, 0);
            if (solver.MageTalents.GlyphOfDeepFreeze)
            {
                BaseSpellModifier *= 1.2f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                // TODO proc not benefitting from shatter?
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 2136, scaling id: 17
    public class FireBlastTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Fire Blast";
            InitializeCastTime(false, true, 0, 8);
            InitializeScaledDamage(solver, false, 30 + 5 * solver.MageTalents.ImprovedFireBlast, MagicSchool.Fire, 0.21f, 1.11300003528595f, 0.170000001788139f, 0, 0.428999990224838f, 0, 1, 1, 0);
            BaseCritRate += 0.04f * solver.MageTalents.ImprovedFireBlast;
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 133, scaling id: 18
    public class FireballTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool pom, bool brainFreeze)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (brainFreeze)
            {
                spell.CostAmplifier = 0;
            }
            spell.CalculateDerivedStats(castingState, false, pom || brainFreeze, true);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Fireball";
            InitializeCastTime(false, false, 2.5f, 0);
            if (solver.Mage4T11)
            {
                BaseCastTime *= 0.9f;
            }
            InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0.09f, 1.2000000477f, 0.2419999987f, 0, 1.2359999418f, 0, 1, 1, 0);
            if (solver.MageTalents.GlyphOfFireball)
            {
                BaseCritRate += 0.05f;
                NonHSCritRate += 0.05f;
            }
            //BaseCritRate += 0.05f * solver.BaseStats.Mage4T9;
            //BaseSpellModifier *= (1 + solver.BaseStats.BonusMageNukeMultiplier);
            NukeProcs = 1;
            NukeProcs2 = 1;
            if (solver.Mage4PVP)
            {
                BaseSpellModifier *= 1.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 44614, scaling id: 22
    public class FrostfireBoltTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool pom, bool brainFreeze)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (brainFreeze)
            {
                spell.CostAmplifier = 0;
            }
            spell.CalculateDerivedStats(castingState, false, pom || brainFreeze, true);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Frostfire Bolt";
            InitializeCastTime(false, false, 2.5f, 0);
            if (solver.Mage4T11)
            {
                BaseCastTime *= 0.9f;
            }
            InitializeScaledDamage(solver, false, 40, MagicSchool.FrostFire, 0.09f, 0.949000000953674f, 0.241999998688698f, 0 /*0.00712000019848347*/, 0.976999998092651f, 0 /*0.00732999993488193*/, 1, 1, 0);
            if (solver.MageTalents.GlyphOfFrostfire)
            {
                BaseDirectDamageModifier *= 1.15f;
                BasePeriodicDamage = 3 * 0.03f * (BaseMinDamage + BaseMaxDamage) / 2f;
                DotDamageCoefficient = 3 * 0.03f * SpellDamageCoefficient;
                DotDuration = 12;
                DotTickInterval = 3;
            }
            //BaseCritRate += 0.05f * solver.BaseStats.Mage4T9;
            NukeProcs = 1;
            NukeProcs2 = 1;
            if (solver.Mage4PVP)
            {
                BaseSpellModifier *= 1.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 11366 (92315 for Pyroblast!, it actually has different scaling with spell power), scaling id: 26
    public class PyroblastTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool pom, bool spammedDot)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateDerivedStats(castingState, false, pom, spammedDot);
            return spell;
        }

        public Spell GetSpell(CastingState castingState, bool pom)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateDerivedStats(castingState, false, pom, false, false, false, false, true);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Pyroblast!";
            InitializeCastTime(false, false, /*3.5f*/0, 0);
            InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0f, 1.5000000000f, 0.2380000055f, 4 * 0.1749999970f, 1.5449999571f /*1.25f*/, 4 * 0.1800000072f, 1, 1, 12);
            DotDuration = 12;
            DotTickInterval = 3;
            NukeProcs2 = 1;
            if (solver.MageTalents.GlyphOfPyroblast)
            {
                BaseCritRate += 0.05f;
            }
            if (solver.Mage2T11)
            {
                BaseCritRate += 0.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    public class PyroblastHardCastTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool pom, bool spammedDot)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateDerivedStats(castingState, false, pom, spammedDot);
            return spell;
        }

        public Spell GetSpell(CastingState castingState, bool pom)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateDerivedStats(castingState, false, pom, false, false, false, false, true);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Pyroblast";
            InitializeCastTime(false, false, 3.5f, 0);
            InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0f, 1.5000000000f, 0.2380000055f, 4 * 0.1749999970f, 1.5449999571f /*1.25f*/, 4 * 0.1800000072f, 1, 1, 12);
            DotDuration = 12;
            DotTickInterval = 3;
            NukeProcs2 = 1;
            if (solver.MageTalents.GlyphOfPyroblast)
            {
                BaseCritRate += 0.05f;
            }
            if (solver.Mage2T11)
            {
                BaseCritRate += 0.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    public class ArcaneBombTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool aoe)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (aoe)
            {
                spell.AreaEffectDot = true;
            }
            spell.CalculateDerivedStats(castingState, false, false, false);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Arcane Bomb";
            InitializeCastTime(false, true, 0f, 0f);
            InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0.033333f, 0, 0, 3.060f, 0, 3.060f, 0, 1, 0);
            MaximumAOETargets = 2;
            AOEMultiplier = 0.5f;
            DotDuration = 12;
            DotTickInterval = 3;
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 44457/44461, scaling id: 24/25
    public class LivingBombTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool aoe)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (aoe)
            {
                spell.AreaEffect = true; // but leave aoe dot at false
            }
            /*if (castingState.MageTalents.GlyphOfLivingBomb)
            {
                spell.DotDamageModifier = (1 + Math.Max(0.0f, Math.Min(1.0f, castingState.FireCritRate)) * (castingState.FireCritBonus - 1));
            }*/
            spell.CalculateDerivedStats(castingState, false, false, false);
            /*if (castingState.MageTalents.GlyphOfLivingBomb)
            {
                spell.IgniteProcs *= 5; // 4 ticks can proc ignite in addition to the explosion
                // add ignite contribution from dot
                if (castingState.Solver.NeedsDisplayCalculations)
                {
                    float igniteFactor = spell.SpellModifier * spell.HitRate * spell.PartialResistFactor * Math.Max(0.0f, Math.Min(1.0f, castingState.FireCritRate)) * castingState.FireCritBonus * castingState.Solver.IgniteFactor / (1 + castingState.Solver.IgniteFactor);
                    spell.IgniteDamage += spell.BasePeriodicDamage * igniteFactor;
                    spell.IgniteDamagePerSpellPower += spell.DotDamageCoefficient * igniteFactor;
                }
            }*/
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Living Bomb";
            InitializeCastTime(false, true, 0f, 0f);
            if (solver.CalculationOptions.ModeMOP)
            {
                InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0.02f, 1.534f, 0, 1.524f, 1.534f, 1.524f, 1, 1, 0);
            }
            else
            {
                InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0.17f, 0.5000000000f, 0, 4 * 0.2500000000f, 0.5149999857f, 4 * 0.2579999864f, 1, 1, 0);
            }
            MaximumAOETargets = 3;
            DotDuration = 12;
            DotTickInterval = 3;
            BaseDirectDamageModifier *= (1f + 0.05f * solver.MageTalents.CriticalMass);
            BaseDotDamageModifier += 0.05f * solver.MageTalents.CriticalMass; // additive with mastery
            if (solver.MageTalents.GlyphOfLivingBomb)
            {
                BaseSpellModifier *= 1.03f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 11129, scaling id: 726
    public class CombustionTemplate : SpellTemplate
    {
        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            // remove overestimated part of ignite contribution from on use spell power effects
            // let's try a fixed percentage reduction, meaning it's very unlikely that if on use effect
            // is active during combustion that it had any effect on building ignite
            float extraSpellPower = 0.8f * castingState.StateSpellPower;
            spell.BasePeriodicDamage -= IgniteContributionCoefficient * extraSpellPower;
            spell.CalculateDerivedStats(castingState, false, false, false);
            spell.CastTime = 0;
            return spell;
        }

        private float IgniteContributionCoefficient;

        public void Initialize(Solver solver)
        {
            Name = "Combustion";
            InitializeCastTime(false, true, 0f, 0f);
            InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0f, 1.11300003528595f, 0.170000001788139f, 0, 0.428999990224838f, 0, 1, 1, 0);
            DotDuration = 10;
            DotTickInterval = 1;
            UseMaxT13 = true;
            // estimate combustion dot tick
            // based on http://elitistjerks.com/f75/t110187-cataclysm_mage_simulators_formulators/p3/#post1824829
            // and http://elitistjerks.com/f75/t110187-cataclysm_mage_simulators_formulators/p3/#post1825299
            // presume LB is up, average the dps
            // presume FFB dot is up, if the glyph is present (regardless if the main rotation is FFB)
            // calculate dps from ignite by averaging fireball and pyroblast ignite dps
            if (solver.MageTalents.GlyphOfFrostfire)
            {
                FrostfireBoltTemplate FFB = solver.FrostfireBoltTemplate;
                BasePeriodicDamage += FFB.BasePeriodicDamage / FFB.DotDuration;
                DotDamageCoefficient += FFB.DotDamageCoefficient / FFB.DotDuration;
            }
            // combustion_tick*3=(.2678930839158054*spellpower+518.6158840934165) * (1+mastery)*(1+mastery+CM)
            // 0.20807229818703332038834951456311*spellpower + 402.8084536647895145631067961165
            // double dip mastery!!
            // TODO I don't know how mastery double dip works in relation to on use mastery effects
            // for now treat it the same as proc mastery effects which might underestimate
            float lbMult = 1 + solver.FlashburnBonus + 0.05f * solver.MageTalents.CriticalMass;
            if (solver.MageTalents.GlyphOfLivingBomb)
            {
                lbMult *= 1.03f;
            }
            BasePeriodicDamage += 402.8084536647895145631067961165f / 3f * lbMult;
            DotDamageCoefficient += 0.20807229818703332038834951456311f / 3f * lbMult;
            // combustion_tick*3 ~= (.1005*spellpower + 283.9)* (1+mastery)^2
            // 0.078058252427184466019417475728155*spellpower + 220.50485436893203883495145631068
            float pyroMult = 1 + solver.FlashburnBonus;
            BasePeriodicDamage += 220.50485436893203883495145631068f / 3f * pyroMult;
            DotDamageCoefficient += 0.078058252427184466019417475728155f / 3f * pyroMult;

            // ignite part doesn't double-dip mastery
            // solver.IgniteFactor has flashburn and munching in, we want to remove both to get clean ignite
            // estimate ignite
            // on use spell power for building ignite is hard to coordinate for combustion purposes
            // assume it is no better than random proc effect
            float ignite = (0.13f * solver.MageTalents.Ignite + (solver.MageTalents.Ignite == 3 ? 0.01f : 0.0f));
            // TODO probabilistic model for rolling multiplier based on haste and crit
            float rollingMultiplier = 1f;
            FireballTemplate FB = solver.FireballTemplate;
            PyroblastTemplate Pyro = solver.PyroblastTemplate;
            // the way we'll remove the on use spell power part will be by deducting it from the base
            // so we'll need to remember the coefficient part associated to ignite contribution
            IgniteContributionCoefficient = 0;
            BasePeriodicDamage += rollingMultiplier * (FB.BaseMinDamage + FB.BaseMaxDamage) / 4.0f * solver.BaseFireCritBonus * ignite / 4.0f;
            IgniteContributionCoefficient += rollingMultiplier * FB.SpellDamageCoefficient / 2.0f * solver.BaseFireCritBonus * ignite / 4.0f;
            BasePeriodicDamage += rollingMultiplier * (Pyro.BaseMinDamage + Pyro.BaseMaxDamage) / 4.0f * solver.BaseFireCritBonus * ignite / 4.0f;
            IgniteContributionCoefficient += rollingMultiplier * Pyro.SpellDamageCoefficient / 2.0f * solver.BaseFireCritBonus * ignite / 4.0f;
            DotDamageCoefficient += IgniteContributionCoefficient;
            // extend to 10 ticks
            BasePeriodicDamage *= 10;
            DotDamageCoefficient *= 10;
            IgniteContributionCoefficient *= 10;
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }

        public void Initialize(Solver solver, bool pyroDot, float pyroRoll, float fbRoll)
        {
            Name = "Combustion";
            InitializeCastTime(false, true, 0f, 0f);
            InitializeScaledDamage(solver, false, 40, MagicSchool.Fire, 0f, 1.11300003528595f, 0.170000001788139f, 0, 0.428999990224838f, 0, 1, 1, 0);
            DotDuration = 10;
            DotTickInterval = 1;
            UseMaxT13 = true;
            float lbMult = 1 + solver.FlashburnBonus + 0.05f * solver.MageTalents.CriticalMass;
            if (solver.MageTalents.GlyphOfLivingBomb)
            {
                lbMult *= 1.03f;
            }
            BasePeriodicDamage += 402.8084536647895145631067961165f / 3f * lbMult;
            DotDamageCoefficient += 0.20807229818703332038834951456311f / 3f * lbMult;

            if (pyroDot)
            {
                float pyroMult = 1 + solver.FlashburnBonus;
                BasePeriodicDamage += 220.50485436893203883495145631068f / 3f * pyroMult;
                DotDamageCoefficient += 0.078058252427184466019417475728155f / 3f * pyroMult;
            }

            float ignite = (0.13f * solver.MageTalents.Ignite + (solver.MageTalents.Ignite == 3 ? 0.01f : 0.0f));

            FireballTemplate FB = solver.FireballTemplate;
            PyroblastTemplate Pyro = solver.PyroblastTemplate;

            IgniteContributionCoefficient = 0;
            BasePeriodicDamage += fbRoll * (FB.BaseMinDamage + FB.BaseMaxDamage) / 2.0f * solver.BaseFireCritBonus * ignite / 4.0f;
            IgniteContributionCoefficient += fbRoll * FB.SpellDamageCoefficient * solver.BaseFireCritBonus * ignite / 4.0f;
            BasePeriodicDamage += pyroRoll * (Pyro.BaseMinDamage + Pyro.BaseMaxDamage) / 2.0f * solver.BaseFireCritBonus * ignite / 4.0f;
            IgniteContributionCoefficient += pyroRoll * Pyro.SpellDamageCoefficient * solver.BaseFireCritBonus * ignite / 4.0f;
            DotDamageCoefficient += IgniteContributionCoefficient;
            // extend to 10 ticks
            BasePeriodicDamage *= 10;
            DotDamageCoefficient *= 10;
            IgniteContributionCoefficient *= 10;
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    public class SlowTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Slow";
            InitializeCastTime(false, true, 0f, 0f);
            InitializeScaledDamage(solver, false, 35, MagicSchool.Arcane, 0.12f, 0, 0, 0, 0, 0, 1, 1, 0);
            Dirty = false;
        }
    }

    // spell id: 120, scaling id: 15
    public class ConeOfColdTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Cone of Cold";
            InitializeCastTime(false, true, 0, 10);
            InitializeScaledDamage(solver, true, 0, MagicSchool.Frost, 0.25f, 0.839999973773956f, 0.0900000035762787f, 0, 0.214000001549721f, 0, 1, 1, 0);
            Cooldown *= (1 - 0.07f * solver.MageTalents.IceFloes + (solver.MageTalents.IceFloes == 3 ? 0.01f : 0.00f));
            if (solver.MageTalents.GlyphOfConeOfCold)
            {
                BaseSpellModifier *= 1.25f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 30455, scaling id: 23
    public class IceLanceTemplate : SpellTemplate
    {
        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (castingState.Frozen)
            {
                spell.SpellModifier *= 2;
                spell.AdditiveSpellModifier += 0.25f;
            }
            spell.CalculateDerivedStats(castingState);
            return spell;
        }

        public void Initialize(Solver solver)
        {
            Name = "Ice Lance";
            InitializeCastTime(false, true, 0, 0);
            InitializeScaledDamage(solver, false, 35, MagicSchool.Frost, 0.06f, 0.432000011205673f, 0.241999998688698f, 0, 0.377999991178513f, 0, 1, 1, 0);
            if (solver.MageTalents.GlyphOfIceLance)
            {
                BaseSpellModifier *= 1.05f;
            }
            if (solver.Mage2T11)
            {
                BaseCritRate += 0.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 44425, scaling id: 8
    public class ArcaneBarrageTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, float arcaneBlastDebuff, bool aoe)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * arcaneBlastDebuff;
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            if (aoe)
            {
                spell.AreaEffect = true;
                spell.MaximumAOETargets = Math.Max(MaximumAOETargets, (int)arcaneBlastDebuff + 1);
            }
            spell.CalculateDerivedStats(castingState);
            return spell;
        }

        private float arcaneBlastDamageMultiplier;
        private float tormentTheWeak;

        public void Initialize(Solver solver)
        {
            Name = "Arcane Barrage";
            if (solver.CalculationOptions.ModeMOP)
            {
                InitializeCastTime(false, true, 0, 3);
                InitializeScaledDamage(solver, false, 40, MagicSchool.Arcane, 0.005f, 1.425f, 0.200000002980232f, 0, 1.425f, 0, 1, 1, 0);
                arcaneBlastDamageMultiplier = 0.25f;
            }
            else
            {
                InitializeCastTime(false, true, 0, 4);
                InitializeScaledDamage(solver, false, 40, MagicSchool.Arcane, 0.11f, 1.4125f, 0.200000002980232f, 0, 0.90738996982574447f, 0, 1, 1, 0);
                arcaneBlastDamageMultiplier = 0f;
            }
            tormentTheWeak = 0.02f * solver.MageTalents.TormentTheWeak;
            if (solver.MageTalents.GlyphOfArcaneBarrage)
            {
                BaseDirectDamageModifier *= 1.04f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 30451, scaling id: 9
    public class ArcaneBlastTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, int debuff, bool manualClearcasting, bool clearcastingActive, bool pom)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (!castingState.CalculationOptions.ModeMOP)
            {
                spell.BaseCastTime -= debuff * 0.1f * castTimeMultiplier;
            }
            if (manualClearcasting) spell.CalculateManualClearcasting(true, false, clearcastingActive);
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * debuff;
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CostModifier += arcaneBlastManaMultiplier * debuff;
            spell.CalculateDerivedStats(castingState, false, pom, false, true, false, false);
            if (manualClearcasting) spell.CalculateManualClearcastingCost(castingState.Solver, false, true, false, clearcastingActive);
            return spell;
        }

        public Spell GetSpell(CastingState castingState, int debuff, bool forceHit)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (!castingState.CalculationOptions.ModeMOP)
            {
                spell.BaseCastTime -= debuff * 0.1f * castTimeMultiplier;
            }
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * debuff;
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CostModifier += arcaneBlastManaMultiplier * debuff;
            spell.CalculateDerivedStats(castingState, false, false, false, true, forceHit, !forceHit);
            return spell;
        }

        public Spell GetSpell(CastingState castingState, int debuff)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (!castingState.CalculationOptions.ModeMOP)
            {
                spell.BaseCastTime -= debuff * 0.1f * castTimeMultiplier;
            }
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * debuff;
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CostModifier += arcaneBlastManaMultiplier * debuff;
            spell.CalculateDerivedStats(castingState, false, false, false, true, false, false);
            return spell;
        }

        public Spell GetSpell(CastingState castingState, int debuff, int castDebuff)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            if (!castingState.CalculationOptions.ModeMOP)
            {
                spell.BaseCastTime -= castDebuff * 0.1f * castTimeMultiplier;
            }
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * debuff;
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CostModifier += arcaneBlastManaMultiplier * debuff;
            spell.CalculateDerivedStats(castingState, false, false, false, true, false, false);
            return spell;
        }

        public Spell GetSpell(CastingState castingState, int debuff, int dwdebuff, int castDebuff, bool dragonwrathProc)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            // we're not averaging dragonwrath so take the average out            
            if (castingState.BaseStats.DragonwrathProc > 0)
            {
                spell.SpellModifier /= 1.1f;
            }
            if (dragonwrathProc)
            {
                spell.SpellModifier *= 2f;
                spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * (debuff + dwdebuff) / 2f;
            }
            else
            {
                spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * debuff;
            }
            if (!castingState.CalculationOptions.ModeMOP)
            {
                spell.BaseCastTime -= castDebuff * 0.1f * castTimeMultiplier;
            }
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CostModifier += arcaneBlastManaMultiplier * debuff;
            spell.CalculateDerivedStats(castingState, false, false, false, true, false, false);
            return spell;
        }

        public Spell GetSpellNoDW(CastingState castingState, int debuff, int castDebuff)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            // we're not averaging dragonwrath so take the average out            
            if (castingState.BaseStats.DragonwrathProc > 0)
            {
                spell.SpellModifier /= 1.1f;
            }
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * debuff;
            if (!castingState.CalculationOptions.ModeMOP)
            {
                spell.BaseCastTime -= castDebuff * 0.1f * castTimeMultiplier;
            }
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CostModifier += arcaneBlastManaMultiplier * debuff;
            spell.CalculateDerivedStats(castingState, false, false, false, true, false, false);
            return spell;
        }

        public Spell GetSpellDW(CastingState castingState, int debuff, int castDebuff)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            // we're not averaging dragonwrath so take the average out
            if (castingState.BaseStats.DragonwrathProc > 0)
            {
                spell.SpellModifier /= 1.1f;
            }
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * debuff;
            if (!castingState.CalculationOptions.ModeMOP)
            {
                spell.BaseCastTime -= castDebuff * 0.1f * castTimeMultiplier;
            }
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CostModifier += arcaneBlastManaMultiplier * debuff;
            spell.CalculateDerivedStats(castingState, false, false, false, true, false, false);
            spell.CastTime = 0;
            spell.AverageCost = 0;
            return spell;
        }

        public override Spell GetSpell(CastingState castingState)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.CalculateDerivedStats(castingState, false, false, false, true, false, false);
            return spell;
        }

        public void AddToCycle(Solver solver, Cycle cycle, Spell rawSpell, float weight0, float weight1, float weight2, float weight3)
        {
            MageTalents mageTalents = solver.MageTalents;
            float weight = weight0 + weight1 + weight2 + weight3;
            float hasteMultiplier = (rawSpell.CastTime - rawSpell.Latency) / (rawSpell.BaseCastTime);
            // if some are below gcd then we have to use different calculations
            if (hasteMultiplier * (rawSpell.BaseCastTime - castTimeMultiplier * 0.3f) <= Math.Max(hasteMultiplier * rawSpell.GlobalCooldown, Spell.GlobalCooldownLimit))
            {
                float channelReduction;
                float averageCastingSpeed;
                cycle.CastTime += weight0 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime, out channelReduction, out averageCastingSpeed);
                cycle.CastTime += weight1 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime - castTimeMultiplier * 0.1f, out channelReduction, out averageCastingSpeed);
                cycle.CastTime += weight2 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime - castTimeMultiplier * 0.2f, out channelReduction, out averageCastingSpeed);
                cycle.CastTime += weight3 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime - castTimeMultiplier * 0.3f, out channelReduction, out averageCastingSpeed);
            }
            else
            {
                cycle.CastTime += weight * rawSpell.CastTime - castTimeMultiplier * (weight1 * 0.1f + weight2 * 0.2f + weight3 * 0.3f) * hasteMultiplier;
            }
            cycle.CastProcs += weight * rawSpell.CastProcs;
            cycle.CastProcs2 += weight * rawSpell.CastProcs2;
            cycle.NukeProcs += weight * rawSpell.NukeProcs;
            cycle.NukeProcs2 += weight * rawSpell.NukeProcs2;
            cycle.Ticks += weight * rawSpell.Ticks;
            cycle.HitProcs += weight * rawSpell.HitProcs;
            cycle.CritProcs += weight * rawSpell.CritProcs;
            cycle.TargetProcs += weight * rawSpell.TargetProcs;
            cycle.damageProcs += weight * rawSpell.HitProcs;

            double roundCost = Math.Round(rawSpell.BaseCost * rawSpell.CostAmplifier);
            cycle.costPerSecond += (1 - solver.ClearcastingChance) * (weight0 * (float)Math.Floor(roundCost * rawSpell.CostModifier) + weight1 * (float)Math.Floor(roundCost * (rawSpell.CostModifier + arcaneBlastManaMultiplier)) + weight2 * (float)Math.Floor(roundCost * (rawSpell.CostModifier + 2 * arcaneBlastManaMultiplier)) + weight3 * (float)Math.Floor(roundCost * (rawSpell.CostModifier + 3 * arcaneBlastManaMultiplier)));
            cycle.costPerSecond -= weight * rawSpell.CritRate * rawSpell.BaseCost * 0.15f * mageTalents.MasterOfElements;
            //cycle.costPerSecond -= weight * BaseUntalentedCastTime / 60f * solver.BaseStats.ManaRestoreFromBaseManaPPM * solver.CalculationOptions.BaseMana;

            float multiplier = (weight * rawSpell.AdditiveSpellModifier + arcaneBlastDamageMultiplier * (weight1 + 2 * weight2 + 3 * weight3)) / rawSpell.AdditiveSpellModifier;
            cycle.DpsPerSpellPower += multiplier * rawSpell.DamagePerSpellPower;
            cycle.DpsPerCrit += multiplier * rawSpell.DamagePerCrit;
            //cycle.DpsPerMastery += multiplier * rawSpell.DamagePerMastery;
            cycle.damagePerSecond += multiplier * rawSpell.AverageDamage;
            cycle.threatPerSecond += multiplier * rawSpell.AverageThreat;
        }

        public void AddToCycle(Solver solver, Cycle cycle, Spell rawSpell, float weight0, float weight1, float weight2, float weight3, float weight4)
        {
            MageTalents mageTalents = solver.MageTalents;
            float weight = weight0 + weight1 + weight2 + weight3 + weight4;
            float hasteMultiplier = (rawSpell.CastTime - rawSpell.Latency) / (rawSpell.BaseCastTime);
            // if some are below gcd then we have to use different calculations
            if (hasteMultiplier * (rawSpell.BaseCastTime - castTimeMultiplier * 0.4f) <= Math.Max(hasteMultiplier * rawSpell.GlobalCooldown, Spell.GlobalCooldownLimit))
            {
                float channelReduction;
                float averageCastingSpeed;
                cycle.CastTime += weight0 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime, out channelReduction, out averageCastingSpeed);
                cycle.CastTime += weight1 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime - castTimeMultiplier * 0.1f, out channelReduction, out averageCastingSpeed);
                cycle.CastTime += weight2 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime - castTimeMultiplier * 0.2f, out channelReduction, out averageCastingSpeed);
                cycle.CastTime += weight3 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime - castTimeMultiplier * 0.3f, out channelReduction, out averageCastingSpeed);
                cycle.CastTime += weight4 * CalculateCastTime(cycle.CastingState, rawSpell.InterruptProtection, rawSpell.CritRate, false, rawSpell.BaseCastTime - castTimeMultiplier * 0.4f, out channelReduction, out averageCastingSpeed);
            }
            else
            {
                cycle.CastTime += weight * rawSpell.CastTime - castTimeMultiplier * (weight1 * 0.1f + weight2 * 0.2f + weight3 * 0.3f + weight4 * 0.4f) * hasteMultiplier;
            }
            cycle.CastProcs += weight * rawSpell.CastProcs;
            cycle.CastProcs2 += weight * rawSpell.CastProcs2;
            cycle.NukeProcs += weight * rawSpell.NukeProcs;
            cycle.NukeProcs2 += weight * rawSpell.NukeProcs2;
            cycle.Ticks += weight * rawSpell.Ticks;
            cycle.HitProcs += weight * rawSpell.HitProcs;
            cycle.CritProcs += weight * rawSpell.CritProcs;
            cycle.TargetProcs += weight * rawSpell.TargetProcs;
            cycle.damageProcs += weight * rawSpell.HitProcs;

            double roundCost = Math.Round(rawSpell.BaseCost * rawSpell.CostAmplifier);
            cycle.costPerSecond += (1 - solver.ClearcastingChance) * (weight0 * (float)Math.Floor(roundCost * rawSpell.CostModifier) + weight1 * (float)Math.Floor(roundCost * (rawSpell.CostModifier + arcaneBlastManaMultiplier)) + weight2 * (float)Math.Floor(roundCost * (rawSpell.CostModifier + 2 * arcaneBlastManaMultiplier)) + weight3 * (float)Math.Floor(roundCost * (rawSpell.CostModifier + 3 * arcaneBlastManaMultiplier)) + weight4 * (float)Math.Floor(roundCost * (rawSpell.CostModifier + 4 * arcaneBlastManaMultiplier)));
            cycle.costPerSecond -= weight * rawSpell.CritRate * rawSpell.BaseCost * 0.15f * mageTalents.MasterOfElements;
            //cycle.costPerSecond -= weight * BaseUntalentedCastTime / 60f * solver.BaseStats.ManaRestoreFromBaseManaPPM * solver.CalculationOptions.BaseMana;

            float multiplier = (weight * rawSpell.AdditiveSpellModifier + arcaneBlastDamageMultiplier * (weight1 + 2 * weight2 + 3 * weight3 + 4 * weight4)) / rawSpell.AdditiveSpellModifier;
            cycle.DpsPerSpellPower += multiplier * rawSpell.DamagePerSpellPower;
            cycle.DpsPerCrit += multiplier * rawSpell.DamagePerCrit;
            //cycle.DpsPerMastery += multiplier * rawSpell.DamagePerMastery;
            cycle.damagePerSecond += multiplier * rawSpell.AverageDamage;
            cycle.threatPerSecond += multiplier * rawSpell.AverageThreat;
        }

        private float arcaneBlastDamageMultiplier;
        private float tormentTheWeak;
        private float arcaneBlastManaMultiplier;
        private float castTimeMultiplier;

        public void Initialize(Solver solver)
        {
            Name = "Arcane Blast";
            InitializeCastTime(false, false, 2.0f, 0);
            if (solver.Mage4T11)
            {
                BaseCastTime *= 0.9f;
                castTimeMultiplier = 0.9f;
            }
            else
            {
                castTimeMultiplier = 1f;
            }
            if (solver.CalculationOptions.ModeMOP)
            {
                InitializeScaledDamage(solver, false, 40, MagicSchool.Arcane, 0.015f, 1.899f, 0.150000005960464f, 0, 1.899f, 0, 1, 1, 0);
            }
            else
            {
                InitializeScaledDamage(solver, false, 40, MagicSchool.Arcane, 0.05f, 0.95f * 2.03500008583069f, 0.150000005960464f, 0, 0.95f * 1.057000041008f, 0, 1, 1, 0);
            }
            Stats baseStats = solver.BaseStats;
            MageTalents mageTalents = solver.MageTalents;
            //BaseCostModifier += baseStats.ArcaneBlastBonus;
            //BaseCritRate += 0.05f * solver.BaseStats.Mage4T9;
            tormentTheWeak = 0.02f * solver.MageTalents.TormentTheWeak;
            if (solver.CalculationOptions.ModeMOP)
            {
                arcaneBlastDamageMultiplier = 0.25f;
            }
            else
            {
                arcaneBlastDamageMultiplier = mageTalents.GlyphOfArcaneBlast ? 0.13f : 0.1f;
            }
            arcaneBlastManaMultiplier = 1.5f;
            NukeProcs = 1;
            NukeProcs2 = 1;
            if (solver.Mage4PVP)
            {
                BaseSpellModifier *= 1.05f;
            }
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 7268, scaling id: 12
    public class ArcaneMissilesTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, bool barrage, bool clearcastingAveraged, bool clearcastingActive, bool clearcastingProccing, int arcaneBlastDebuff)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            spell.CalculateManualClearcasting(true, clearcastingAveraged, clearcastingActive);
            //spell.BaseCastTime = ticks;
            if (barrage)
            {
                spell.BaseCastTime *= 0.5f;
                spell.CostModifier = Math.Max(spell.CostModifier - 1, 0);
            }
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * arcaneBlastDebuff;
            //spell.SpellModifier *= ticks / 5.0f;
            spell.CalculateDerivedStats(castingState);
            spell.CalculateManualClearcastingCost(castingState.Solver, false, true, clearcastingAveraged, clearcastingActive);
            return spell;
        }

        /*public Spell GetSpell(CastingState castingState, bool barrage, int arcaneBlastDebuff)
        {
            return GetSpell(castingState, barrage, arcaneBlastDebuff, 5);
        }*/

        public Spell GetSpell(CastingState castingState, bool barrage, int arcaneBlastDebuff)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            //spell.BaseCastTime = ticks;
            if (barrage)
            {
                spell.BaseCastTime *= 0.5f;
                spell.CostModifier = Math.Max(spell.CostModifier - 1, 0);
            }
            spell.SpellModifier *= (1 + tormentTheWeak * castingState.SnaredTime);
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * arcaneBlastDebuff;
            //spell.SpellModifier *= ticks / 5.0f;
            spell.CalculateDerivedStats(castingState);
            return spell;
        }

        public void AddToCycle(Solver solver, Cycle cycle, Spell rawSpell, float weight0, float weight1, float weight2, float weight3, float weight4)
        {
            MageTalents mageTalents = solver.MageTalents;
            float weight = weight0 + weight1 + weight2 + weight3 + weight4;
            cycle.CastTime += weight * rawSpell.CastTime;
            cycle.CastProcs += weight * rawSpell.CastProcs;
            cycle.CastProcs2 += weight * rawSpell.CastProcs2;
            cycle.NukeProcs += weight * rawSpell.NukeProcs;
            cycle.NukeProcs2 += weight * rawSpell.NukeProcs2;
            cycle.Ticks += weight * rawSpell.Ticks;
            cycle.HitProcs += weight * rawSpell.HitProcs;
            cycle.CritProcs += weight * rawSpell.CritProcs;
            cycle.TargetProcs += weight * rawSpell.TargetProcs;
            cycle.costPerSecond += weight * rawSpell.AverageCost;
            cycle.damageProcs += weight * rawSpell.HitProcs;
            float multiplier = (weight * rawSpell.AdditiveSpellModifier + arcaneBlastDamageMultiplier * (weight1 + 2 * weight2 + 3 * weight3 + 4 * weight4)) / rawSpell.AdditiveSpellModifier;
            cycle.DpsPerSpellPower += multiplier * rawSpell.DamagePerSpellPower;
            //cycle.DpsPerMastery += multiplier * rawSpell.DamagePerMastery;
            cycle.damagePerSecond += multiplier * rawSpell.AverageDamage;
            cycle.threatPerSecond += multiplier * rawSpell.AverageThreat;
        }

        float tormentTheWeak;
        float arcaneBlastDamageMultiplier;

        public void Initialize(Solver solver)
        {
            Name = "Arcane Missiles";
            float castTime = 0.75f;
            if (solver.CalculationOptions.ModeMOP)
            {
                castTime = 0.4f;
            }
            else if (solver.MageTalents.MissileBarrage == 1)
            {
                castTime = 0.6f;
            }
            else if (solver.MageTalents.MissileBarrage == 2)
            {
                castTime = 0.5f;
            }
            int missiles = 3 + solver.MageTalents.ImprovedArcaneMissiles;
            InitializeCastTime(true, false, castTime * missiles, 0);
            if (solver.CalculationOptions.ModeMOP)
            {
                InitializeScaledDamage(solver, false, 40, MagicSchool.Arcane, 0, 0.475f, 0, 0, 0.475f, 0, missiles, missiles + 1, 0);
            }
            else
            {
                InitializeScaledDamage(solver, false, 40, MagicSchool.Arcane, 0, 0.43165999919176072f, 0, 0, 0.2779800076782709f, 0, missiles, missiles + 1, 0);
            }
            BaseMinDamage *= missiles;
            BaseMaxDamage *= missiles;
            SpellDamageCoefficient *= missiles;
            CastProcs2 = 1;
            if (solver.MageTalents.GlyphOfArcaneMissiles)
            {
                BaseCritRate += 0.05f;
            }
            if (solver.Mage2T11)
            {
                BaseCritRate += 0.05f;
            }
            tormentTheWeak = 0.02f * solver.MageTalents.TormentTheWeak;
            if (solver.CalculationOptions.ModeMOP)
            {
                arcaneBlastDamageMultiplier = 0.25f;
            }
            else
            {
                arcaneBlastDamageMultiplier = 0f;
            }
            //BaseSpellModifier *= (1 + solver.BaseStats.BonusMageNukeMultiplier);
            //BaseCritRate += 0.05f * solver.BaseStats.Mage4T9;
            // Arcane Potency bug/feature
            float arcanePotency;
            switch (solver.MageTalents.ArcanePotency)
            {
                case 0:
                default:
                    arcanePotency = 0;
                    break;
                case 1:
                    arcanePotency = 0.07f;
                    break;
                case 2:
                    arcanePotency = 0.15f;
                    break;
            }
            // not tested this, but assuming each wave has 1/5th chance even if less waves present
            // this is assuming two spells before AM are not AM, not completely accurate, but close enough
            float potencyChance = 11f / 125f * (10f - 3f * solver.ClearcastingChance) * solver.ClearcastingChance;
            BaseCritRate = BaseCritRate - solver.ArcanePotencyCrit + arcanePotency * potencyChance;
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 1449, scaling id: 10
    public class ArcaneExplosionTemplate : SpellTemplate
    {
        public Spell GetSpell(CastingState castingState, float arcaneBlastDebuff)
        {
            Spell spell = Spell.New(this, castingState.Solver);
            spell.Calculate(castingState);
            //if (castingState.CalculationOptions.ModePTR)
            //{
            //    spell.CostAmplifier = 1f - 0.25f * castingState.MageTalents.ImprovedArcaneExplosion;
            //}
            spell.AdditiveSpellModifier += arcaneBlastDamageMultiplier * arcaneBlastDebuff;
            spell.CalculateDerivedStats(castingState);
            return spell;
        }

        private float arcaneBlastDamageMultiplier;

        public void Initialize(Solver solver)
        {
            Name = "Arcane Explosion";
            InitializeCastTime(false, true, 0, 0);
            InitializeScaledDamage(solver, true, 0, MagicSchool.Arcane, 0.15f * (1 - 0.25f * solver.MageTalents.ImprovedArcaneExplosion), 0.3677584900856011975f, 0.0799999982118607f, 0, 0.18582850867509814f, 0, 1, 1, 0);
            // should we count torment the weak?
            arcaneBlastDamageMultiplier = solver.MageTalents.GlyphOfArcaneBlast ? 0.13f : 0.1f;
            GlobalCooldown -= 0.25f * solver.MageTalents.ImprovedArcaneExplosion;
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 11113, scaling id: 13
    public class BlastWaveTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Blast Wave";
            InitializeCastTime(false, true, 0, 15);
            InitializeScaledDamage(solver, true, 0, MagicSchool.Fire, 0.07f, 0.989000022411346f, 0.164000004529953f, 0, 0.14300000667572f, 0, 1, 1, 0);
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 31661, scaling id: 16
    public class DragonsBreathTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Dragon's Breath";
            InitializeCastTime(false, true, 0, 20);
            InitializeScaledDamage(solver, true, 0, MagicSchool.Fire, 0.07f, 1.37800002098083f, 0.150000005960464f, 0, 0.193000003695488f, 0, 1, 1, 0);
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    // spell id: 42208, scaling id: 14
    public class BlizzardTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Blizzard";
            InitializeCastTime(true, false, 8, 0);
            InitializeScaledDamage(solver, true, 30, MagicSchool.Frost, 0.74f, 8 * 0.5419999957f, 0, 0, 8 * 0.1620000005f, 0, 8, 1, 0);
            if (solver.BaseStats.DragonwrathProc > 0)
            {
                BaseSpellModifier *= 1.1f;
                Ticks *= 1.1f;
                CastProcs *= 1.1f;
                CastProcs2 *= 1.1f;
            }
            Dirty = false;
        }
    }

    public class ArcaneDamageTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Arcane Damage";
            InitializeEffectDamage(solver, MagicSchool.Arcane, 1, 1);
            CritBonus = 2.0f * 1.33f * (1 + solver.BaseStats.BonusSpellCritDamageMultiplier);
            BaseSpellModifier = solver.BaseSpellModifier * (1 + solver.BaseStats.BonusArcaneDamageMultiplier);
            BaseCritRate = solver.BaseCritRate;
            Dirty = false;
        }
    }

    public class FireDamageTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Fire Damage";
            InitializeEffectDamage(solver, MagicSchool.Fire, 1, 1);
            CritBonus = 2.0f * 1.33f * (1 + solver.BaseStats.BonusSpellCritDamageMultiplier);
            BaseSpellModifier = solver.BaseSpellModifier * (1 + solver.BaseStats.BonusFireDamageMultiplier);
            BaseCritRate = solver.BaseCritRate;
            Dirty = false;
        }
    }

    public class FrostDamageTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Frost Damage";
            InitializeEffectDamage(solver, MagicSchool.Frost, 1, 1);
            CritBonus = 2.0f * 1.33f * (1 + solver.BaseStats.BonusSpellCritDamageMultiplier);
            BaseSpellModifier = solver.BaseSpellModifier * (1 + solver.BaseStats.BonusFrostDamageMultiplier);
            BaseCritRate = solver.BaseCritRate;
            Dirty = false;
        }
    }

    public class ShadowDamageTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Shadow Damage";
            InitializeEffectDamage(solver, MagicSchool.Shadow, 1, 1);
            CritBonus = 2.0f * 1.33f * (1 + solver.BaseStats.BonusSpellCritDamageMultiplier);
            BaseSpellModifier = solver.BaseSpellModifier * (1 + solver.BaseStats.BonusShadowDamageMultiplier);
            BaseCritRate = solver.BaseCritRate;
            Dirty = false;
        }
    }

    public class NatureDamageTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Nature Damage";
            InitializeEffectDamage(solver, MagicSchool.Nature, 1, 1);
            CritBonus = 2.0f * 1.33f * (1 + solver.BaseStats.BonusSpellCritDamageMultiplier);
            BaseSpellModifier = solver.BaseSpellModifier * (1 + solver.BaseStats.BonusNatureDamageMultiplier);
            BaseCritRate = solver.BaseCritRate;
            Dirty = false;
        }
    }

    public class HolyDamageTemplate : SpellTemplate
    {
        public void Initialize(Solver solver)
        {
            Name = "Holy Damage";
            InitializeEffectDamage(solver, MagicSchool.Holy, 1, 1);
            CritBonus = 2.0f * 1.33f * (1 + solver.BaseStats.BonusSpellCritDamageMultiplier);
            BaseSpellModifier = solver.BaseSpellModifier * (1 + solver.BaseStats.BonusHolyDamageMultiplier);
            BaseCritRate = solver.BaseCritRate;
            Dirty = false;
        }
    }

    public class HolySummonedDamageTemplate : SpellTemplate
    {
        public float Multiplier;

        public void Initialize(Solver solver)
        {
            Name = "Holy Damage";
            // TODO recheck all buffs that apply
            float spellCrit = 0.05f + solver.TargetDebuffs.SpellCritOnTarget;
            // summoned always hit
            RealResistance = solver.CalculationOptions.HolyResist;
            PartialResistFactor = (RealResistance == -1) ? 0 : (1 - StatConversion.GetAverageResistance(solver.CalculationOptions.PlayerLevel, solver.CalculationOptions.TargetLevel, RealResistance, 0));
            Multiplier = PartialResistFactor * (1 + solver.TargetDebuffs.BonusDamageMultiplier) * (1 + solver.TargetDebuffs.BonusHolyDamageMultiplier) * (1 + (1.5f * 1.33f - 1) * spellCrit);
            Dirty = false;
        }
    }

    public class FireSummonedDamageTemplate : SpellTemplate
    {
        public float Multiplier;

        public void Initialize(Solver solver)
        {
            Name = "Fire Damage";
            // TODO recheck all buffs that apply
            float spellCrit = 0.05f + solver.TargetDebuffs.SpellCritOnTarget;
            // summoned always hit
            RealResistance = solver.CalculationOptions.FireResist;
            PartialResistFactor = (RealResistance == -1) ? 0 : (1 - StatConversion.GetAverageResistance(solver.CalculationOptions.PlayerLevel, solver.CalculationOptions.TargetLevel, RealResistance, 0));
            Multiplier = PartialResistFactor * (1 + solver.TargetDebuffs.BonusDamageMultiplier) * (1 + solver.TargetDebuffs.BonusFireDamageMultiplier) * (1 + (1.5f * 1.33f - 1) * spellCrit);
            Dirty = false;
        }
    }
}
