﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
    [Flags]
    public enum StandardEffect
    {
        None = 0,
        FlameOrb = 0x1,
        PowerInfusion = 0x2,
        VolcanicPotion = 0x4,
        ArcanePower = 0x8,
        Combustion = 0x10,
        Berserking = 0x20,
        FlameCap = 0x40,
        Heroism = 0x80,
        IcyVeins = 0x100,
        MoltenFury = 0x200,
        Evocation = 0x400,
        ManaGemEffect = 0x800,
        MirrorImage = 0x1000,
        BloodFury = 0x2000,
        MageArmor = 0x4000,
        MoltenArmor = 0x8000,
        FrostArmor = 0x10000, // make sure to update shifting of item based effects if this changes (Solver.standardEffectCount)
        NonItemBasedMask = PowerInfusion | VolcanicPotion | ArcanePower | Combustion | Berserking | FlameCap | Heroism | IcyVeins | MoltenFury | ManaGemEffect | MirrorImage | FlameOrb | BloodFury | MageArmor | MoltenArmor | FrostArmor
    }

    public class CastingState
    {
        public CastingState ReferenceCastingState { get; set; } // state from where we take base spell calculations for adjusting

        public Solver Solver { get; private set; }

        public CalculationOptionsMage CalculationOptions { get; set; }
        public MageTalents MageTalents { get; set; }
        public Stats BaseStats { get; set; }

        public float SpellHit { get { return Solver.BaseSpellHit; } }
        public float ArcaneHitRate { get { return Solver.BaseArcaneHitRate; } }
        public float FireHitRate { get { return Solver.BaseFireHitRate; } }
        public float FrostHitRate { get { return Solver.BaseFrostHitRate; } }
        public float NatureHitRate { get { return Solver.BaseNatureHitRate; } }
        public float ShadowHitRate { get { return Solver.BaseShadowHitRate; } }
        public float FrostFireHitRate { get { return Solver.BaseFrostFireHitRate; } }
        public float HolyHitRate { get { return Solver.BaseHolyHitRate; } }

        public float ArcaneThreatMultiplier { get { return Solver.ArcaneThreatMultiplier; } }
        public float FireThreatMultiplier { get { return Solver.FireThreatMultiplier; } }
        public float FrostThreatMultiplier { get { return Solver.FrostThreatMultiplier; } }
        public float NatureThreatMultiplier { get { return Solver.NatureThreatMultiplier; } }
        public float ShadowThreatMultiplier { get { return Solver.ShadowThreatMultiplier; } }
        public float FrostFireThreatMultiplier { get { return Solver.FrostFireThreatMultiplier; } }
        public float HolyThreatMultiplier { get { return Solver.HolyThreatMultiplier; } }

        public float CastingSpeed { get; set; }

        public float StateSpellPower { get; set; }

        public float ArcaneSpellPower { get { return Solver.BaseArcaneSpellPower + StateSpellPower; } }
        public float FireSpellPower { get { return Solver.BaseFireSpellPower + StateSpellPower + (FlameCap ? 80.0f : 0.0f); } }
        public float FrostSpellPower { get { return Solver.BaseFrostSpellPower + StateSpellPower; } }
        public float NatureSpellPower { get { return Solver.BaseNatureSpellPower + StateSpellPower; } }
        public float ShadowSpellPower { get { return Solver.BaseShadowSpellPower + StateSpellPower; } }
        public float FrostFireSpellPower { get { return Math.Max(FrostSpellPower, FireSpellPower); } }
        public float HolySpellPower { get { return Solver.BaseHolySpellPower + StateSpellPower; } }

        public float SpiritRegen { get { return Solver.SpiritRegen; } }
        public float ManaRegen { get { return Solver.ManaRegen + StateManaRegen; } }
        public float ManaRegen5SR { get { return Solver.ManaRegen5SR + StateManaRegen; } }
        public float ManaRegenDrinking { get { return Solver.ManaRegenDrinking + StateManaRegen; } }
        public float HealthRegen { get { return Solver.HealthRegen; } }
        public float HealthRegenCombat { get { return Solver.HealthRegenCombat; } }
        public float HealthRegenEating { get { return Solver.HealthRegenEating; } }
        public float MeleeMitigation { get { return Solver.MeleeMitigation; } }
        //public float Defense { get { return Solver.Defense; } }
        public float PhysicalCritReduction { get { return Solver.PhysicalCritReduction; } }
        public float SpellCritReduction { get { return Solver.SpellCritReduction; } }
        public float CritDamageReduction { get { return Solver.CritDamageReduction; } }
        public float Dodge { get { return Solver.Dodge; } }

        public float StateManaRegen { get; set; }
        public float StateSpellModifier { get; set; }
        public float StateAdditiveSpellModifier { get; set; }
        public float StateEffectMaxMana { get; set; }

        public float ArcaneCritBonus { get { return Solver.BaseArcaneCritBonus; } }
        public float FireCritBonus { get { return Solver.BaseFireCritBonus; } }
        public float FrostCritBonus { get { return Solver.BaseFrostCritBonus; } }
        public float NatureCritBonus { get { return Solver.BaseNatureCritBonus; } }
        public float ShadowCritBonus { get { return Solver.BaseShadowCritBonus; } }
        public float FrostFireCritBonus { get { return Solver.BaseFrostFireCritBonus; } }
        public float HolyCritBonus { get { return Solver.BaseHolyCritBonus; } }

        public float StateCritRate { get; set; }

        public float CritRate { get { return StateCritRate + Solver.BaseCritRate; } }
        public float ArcaneCritRate { get { return StateCritRate + Solver.BaseArcaneCritRate; } }
        public float FireCritRate { get { return StateCritRate + Solver.BaseFireCritRate; } }
        public float FrostCritRate { get { return StateCritRate + Solver.BaseFrostCritRate; } }
        public float NatureCritRate { get { return StateCritRate + Solver.BaseNatureCritRate; } }
        public float ShadowCritRate { get { return StateCritRate + Solver.BaseShadowCritRate; } }
        public float FrostFireCritRate { get { return StateCritRate + Solver.BaseFrostFireCritRate; } }
        public float HolyCritRate { get { return StateCritRate + Solver.BaseHolyCritRate; } }

        public float StateMastery { get; set; }

        public float Mastery { get { return StateMastery + Solver.Mastery; } }

        public float ManaAdeptBonus { get { return Solver.ManaAdeptMultiplier * Mastery; } }
        public float FlashburnBonus { get { return Solver.FlashburnMultiplier * Mastery; } }
        public float FrostburnBonus { get { return Solver.FrostburnMultiplier * Mastery; } }

        //public float ResilienceCritDamageReduction { get; set; }
        //public float ResilienceCritRateReduction { get; set; }

        public float SnaredTime { get; set; }

        public bool EffectsActive(int effects)
        {
            return (effects & Effects) == effects;
        }

        public bool Evocation { get; private set; }
        public bool ArcanePower { get; private set; }
        public bool IcyVeins { get; private set; }
        public bool MoltenFury { get; private set; }
        public bool Heroism { get; private set; }
        public bool VolcanicPotion { get; private set; }
        public bool FlameCap { get; private set; }
        public bool ManaGemEffect { get; private set; }
        public bool Berserking { get; private set; }
        public bool Combustion { get; private set; }
        public bool FlameOrb { get; private set; }
        public bool WaterElemental { get { return Solver.Specialization == Specialization.Frost; } }
        public bool MirrorImage { get; private set; }
        public bool PowerInfusion { get; private set; }
        public bool BloodFury { get; private set; }
        public bool MageArmor { get; private set; }
        public bool MoltenArmor { get; private set; }
        public bool FrostArmor { get; private set; }
        public bool Frozen { get; set; }
        public bool UseMageWard { get; set; }

        private int effects;
        public int Effects
        {
            get
            {
                return effects;
            }
            set
            {
                effects = value;
                Evocation = (value & (int)StandardEffect.Evocation) != 0;
                ArcanePower = (value & (int)StandardEffect.ArcanePower) != 0;
                IcyVeins = (value & (int)StandardEffect.IcyVeins) != 0;
                MoltenFury = (value & (int)StandardEffect.MoltenFury) != 0;
                Heroism = (value & (int)StandardEffect.Heroism) != 0;
                VolcanicPotion = (value & (int)StandardEffect.VolcanicPotion) != 0;
                FlameCap = (value & (int)StandardEffect.FlameCap) != 0;
                ManaGemEffect = (value & (int)StandardEffect.ManaGemEffect) != 0;
                Berserking = (value & (int)StandardEffect.Berserking) != 0;
                Combustion = (value & (int)StandardEffect.Combustion) != 0;
                FlameOrb = (value & (int)StandardEffect.FlameOrb) != 0;
                //WaterElemental = (value & (int)StandardEffect.WaterElemental) != 0;
                MirrorImage = (value & (int)StandardEffect.MirrorImage) != 0;
                PowerInfusion = (value & (int)StandardEffect.PowerInfusion) != 0;
                BloodFury = (value & (int)StandardEffect.BloodFury) != 0;
                MageArmor = (value & (int)StandardEffect.MageArmor) != 0;
                MoltenArmor = (value & (int)StandardEffect.MoltenArmor) != 0;
                FrostArmor = (value & (int)StandardEffect.FrostArmor) != 0;
            }
        }

        public float SpellHasteRating { get; set; }
        public float ProcHasteRating { get; set; }
        public bool ForceT13 { get; set; } // set to true to treat T13 as on even when state doesn't have it for advanced haste procs to achieve combustion on max stack only

        private string buffLabel;
        public string BuffLabel
        {
            get
            {
                if (buffLabel == null)
                {
                    buffLabel = Solver.EffectsDescription(Effects);
                }
                return buffLabel;
            }
        }

        public override string ToString()
        {
            return BuffLabel;
        }

        private CastingState maintainSnareState;
        public CastingState MaintainSnareState
        {
            get
            {
                if (maintainSnareState == null)
                {
                    if (SnaredTime == 1.0f)
                    {
                        maintainSnareState = this;
                    }
                    else
                    {
                        maintainSnareState = Clone();
                        maintainSnareState.SnaredTime = 1.0f;
                    }
                }
                return maintainSnareState;
            }
        }

        private CastingState frozenState;
        public CastingState FrozenState
        {
            get
            {
                if (frozenState == null)
                {
                    if (Frozen)
                    {
                        frozenState = this;
                    }
                    else
                    {
                        frozenState = Clone();
                        frozenState.Frozen = true;
                        frozenState.StateAdditiveSpellModifier += FrostburnBonus;
                    }
                }
                return frozenState;
            }
        }

        private CastingState tier10TwoPieceState;
        public CastingState Tier10TwoPieceState
        {
            get
            {
                if (tier10TwoPieceState == null)
                {
                    tier10TwoPieceState = Clone();
                    tier10TwoPieceState.CastingSpeed *= 1.12f;
                    tier10TwoPieceState.ReferenceCastingState = this;
                }
                return tier10TwoPieceState;
            }
        }

        private CastingState[] HasteProcs { get; set; }

        public CastingState()
        {
        }

        public CastingState(Solver solver, int effects, bool frozen, float procHasteRating)
        {
            Initialize(solver, effects, frozen, procHasteRating);
        }

        public static CastingState New(Solver solver, int effects, bool frozen, float procHasteRating)
        {
            CastingState state;
            if (solver.NeedsDisplayCalculations || solver.ArraySet == null)
            {
                state = new CastingState();
            }
            else
            {
                state = solver.ArraySet.NewCastingState();
            }
            state.Initialize(solver, effects, frozen, procHasteRating);
            return state;
        }

        public static CastingState NewRaw(Solver solver, int effects)
        {
            CastingState state = solver.NeedsDisplayCalculations ? new CastingState() : solver.ArraySet.NewCastingState();
            state.Solver = solver;
            state.ReferenceCastingState = null;
            state.UseMageWard = false;
            state.Effects = effects;
            state.buffLabel = null;
            state.SpellsCount = 0;
            state.CyclesCount = 0;
            state.ProcHasteRating = 0;
            state.ForceT13 = false;
            return state;
        }

        public CastingState Clone()
        {
            CastingState state;
            if (Solver.NeedsDisplayCalculations || Solver.ArraySet == null)
            {
                state = new CastingState();
            }
            else
            {
                state = Solver.ArraySet.NewCastingState();
            }
            state.frozenState = null;
            state.UseMageWard = UseMageWard;
            state.maintainSnareState = null;
            state.tier10TwoPieceState = null;
            state.ReferenceCastingState = null;

            state.buffLabel = null;

            state.Solver = Solver;
            state.CalculationOptions = CalculationOptions;
            state.MageTalents = MageTalents;
            state.BaseStats = BaseStats;
            state.HasteProcs = null;

            state.SnaredTime = SnaredTime;
            state.ProcHasteRating = ProcHasteRating;
            state.ForceT13 = ForceT13;
            state.Effects = Effects;
            state.StateSpellPower = StateSpellPower;
            state.SpellHasteRating = SpellHasteRating;
            state.StateCritRate = StateCritRate;
            state.StateMastery = StateMastery;
            state.Frozen = Frozen;
            state.CastingSpeed = CastingSpeed;
            state.StateAdditiveSpellModifier = StateAdditiveSpellModifier;
            state.StateSpellModifier = StateSpellModifier;
            state.StateEffectMaxMana = StateEffectMaxMana;
            state.StateManaRegen = StateManaRegen;

            state.SpellsCount = 0;
            state.CyclesCount = 0;

            return state;
        }


        public void Initialize(Solver solver, int effects, bool frozen, float procHasteRating)
        {
            frozenState = null;
            UseMageWard = false;
            maintainSnareState = null;
            tier10TwoPieceState = null;
            ReferenceCastingState = null;
            ForceT13 = false;

            StateSpellPower = 0;
            StateAdditiveSpellModifier = 0;
            buffLabel = null;

            //MageTalents = calculations.MageTalents;
            //BaseStats = calculations.BaseStats; // == characterStats
            //CalculationOptions = calculations.CalculationOptions;
            Character character = solver.Character;
            Solver = solver;
            CalculationOptions = solver.CalculationOptions;
            MageTalents = solver.MageTalents;
            BaseStats = solver.BaseStats;

            HasteProcs = null;

            float levelScalingFactor = CalculationOptions.LevelScalingFactor;

            SnaredTime = CalculationOptions.SnaredTime;
            if (CalculationOptions.MaintainSnare) SnaredTime = 1.0f;

            float stateCritRating = 0.0f;
            float stateMasteryRating = 0.0f;
            StateCritRate = 0.0f;
            StateMastery = 0.0f;
            ProcHasteRating = procHasteRating;
            SpellHasteRating = BaseStats.HasteRating + procHasteRating;

            Effects = effects;

            if (BloodFury)
            {
                StateSpellPower += 584;
            }

            List<EffectCooldown> cooldownList = solver.CooldownList;
            StateEffectMaxMana = 0;
            for (int i = 0; i < cooldownList.Count; i++)
            {
                EffectCooldown effect = cooldownList[i];
                if (effect.SpecialEffect != null && (effects & effect.Mask) == effect.Mask)
                {
                    StateSpellPower += effect.SpecialEffect.Stats.SpellPower;
                    SpellHasteRating += effect.SpecialEffect.Stats.HasteRating;
                    stateMasteryRating += effect.SpecialEffect.Stats.MasteryRating;
                    if (effect.SpecialEffect.Stats.Intellect + effect.SpecialEffect.Stats.HighestStat > 0)
                    {
                        float effectIntellect = (effect.SpecialEffect.Stats.Intellect + effect.SpecialEffect.Stats.HighestStat) * (1 + BaseStats.BonusIntellectMultiplier);
                        StateCritRate += 0.01f * (effectIntellect * solver.SpellCritPerInt);
                        StateSpellPower += effectIntellect;
                        if (!CalculationOptions.ModeMOP)
                        {
                            StateEffectMaxMana += effectIntellect * 15 * (1 + BaseStats.BonusManaMultiplier);
                        }
                    }
                }
            }
            if (VolcanicPotion)
            {
                float effectIntellect = 1200 * (1 + BaseStats.BonusIntellectMultiplier);
                StateCritRate += 0.01f * (effectIntellect * solver.SpellCritPerInt);
                StateSpellPower += effectIntellect;
                if (!CalculationOptions.ModeMOP)
                {
                    StateEffectMaxMana += effectIntellect * 15 * (1 + BaseStats.BonusManaMultiplier);
                }
            }

            if (ManaGemEffect)
            {
                StateSpellPower += 0.01f * MageTalents.ImprovedManaGem * (BaseStats.Mana + StateEffectMaxMana);
            }

            CastingSpeed = (1 + SpellHasteRating / 1000f * levelScalingFactor) * solver.CastingSpeedMultiplier;

            StateCritRate += stateCritRating / 1400f * levelScalingFactor;
            StateMastery += stateMasteryRating / 14f * levelScalingFactor;

            // spell calculations

            Frozen = frozen;

            if (IcyVeins)
            {
                CastingSpeed *= 1.2f;
            }
            if (Berserking)
            {
                CastingSpeed *= 1.2f;
            }
            if (Heroism)
            {
                CastingSpeed *= 1.3f;
            }
            else if (PowerInfusion)
            {
                CastingSpeed *= 1.2f;
            }

            StateSpellModifier = 1.0f;
            if (ArcanePower)
            {
                StateAdditiveSpellModifier += 0.2f;
            }
            if (MoltenFury)
            {
                StateSpellModifier *= (1 + 0.04f * MageTalents.MoltenFury);
            }
            if (MirrorImage && Solver.Mage4T10)
            {
                StateSpellModifier *= 1.18f;
            }
            if (Frozen)
            {
                StateAdditiveSpellModifier += FrostburnBonus;
            }

            StateManaRegen = 0;
            if (MageArmor)
            {
                StateManaRegen += 0.006f * (MageTalents.GlyphOfMageArmor ? 1.2f : 1.0f) * BaseStats.Mana;
            }
            if (MoltenArmor)
            {
                StateCritRate += 0.03f + (MageTalents.GlyphOfMoltenArmor ? 0.02f : 0.0f);
            }

            SpellsCount = 0;
            CyclesCount = 0;

            //ResilienceCritDamageReduction = 1;
            //ResilienceCritRateReduction = 0;

            if (Solver.Mage2T13 && CalculationOptions.AdvancedHasteProcs)
            {
                ForceT13 = true; // only set to false for sub states where T13 is already included
            }
        }

        public float ArcaneAverageDamage { get { return Solver.ArcaneDamageTemplate.GetEffectAverageDamage(this); } }
        public float FireAverageDamage { get { return Solver.FireDamageTemplate.GetEffectAverageDamage(this); } }
        public float FrostAverageDamage { get { return Solver.FrostDamageTemplate.GetEffectAverageDamage(this); } }
        public float ShadowAverageDamage { get { return Solver.ShadowDamageTemplate.GetEffectAverageDamage(this); } }
        public float NatureAverageDamage { get { return Solver.NatureDamageTemplate.GetEffectAverageDamage(this); } }
        public float HolyAverageDamage { get { return Solver.HolyDamageTemplate.GetEffectAverageDamage(this); } }
        public float HolySummonedAverageDamage { get { return Solver.HolySummonedDamageTemplate.Multiplier; } }
        public float FireSummonedAverageDamage { get { return Solver.FireSummonedDamageTemplate.Multiplier; } }

        //private static int CycleIdCount;
        //private static int SpellIdCount;

        //static CastingState()
        //{
        //    CycleIdCount = Enum.GetValues(typeof(CycleId)).Length;
        //    SpellIdCount = Enum.GetValues(typeof(SpellId)).Length;
        //}

        //private Cycle[] Cycles = new Cycle[CycleIdCount];
        //private Spell[] Spells = new Spell[SpellIdCount];

        //private Dictionary<int, Spell> Spells = new Dictionary<int, Spell>(7);
        //private Dictionary<int, Cycle> Cycles = new Dictionary<int, Cycle>(7);

        // typical sizes are below 10, so it is more efficient to just have a list
        // and look through the entries already stored for a match
        private Spell[] Spells = new Spell[8];
        private int SpellsCount;
        private Cycle[] Cycles = new Cycle[8];
        private int CyclesCount;

        public Cycle GetCycle(CycleId cycleId)
        {
            //Cycle c = Cycles[(int)cycleId];
            //if (c != null) return c;
            Cycle c = null;
            //if (Cycles.TryGetValue((int)cycleId, out c)) return c;
            for (int i = 0; i < CyclesCount; i++)
            {
                Cycle cycle = Cycles[i];
                if (cycle.CycleId == cycleId) return cycle;
            }

            if (CalculationOptions.AdvancedHasteProcs && cycleId != CycleId.ArcaneManaNeutral)
            {
                c = GetAveragedHasteCycle(cycleId);
            }
            if (c == null)
            {
                c = GetNewCycle(cycleId);
            }

            if (c != null)
            {
                c.CycleId = cycleId;
                //Cycles[(int)cycleId] = c;
                //Cycles.Add(c);
                if (CyclesCount >= Cycles.Length)
                {
                    int length = 2 * Cycles.Length;
                    Cycle[] destinationArray = new Cycle[length];
                    Array.Copy(Cycles, 0, destinationArray, 0, CyclesCount);
                    Cycles = destinationArray;
                }
                Cycles[CyclesCount++] = c;
            }

            return c;
        }

        public Cycle GetNewCycle(CycleId cycleId)
        {
            Cycle c = GetRawCycle(cycleId);

            if (c != null)
            {
                if (cycleId != CycleId.ArcaneManaNeutral) // if cycle is based on other cycles make sure we don't double count mixins
                {
                    if (UseMageWard)
                    {
                        c = MageWardCycle.GetCycle(Solver.NeedsDisplayCalculations, this, c);
                    }
                    if (CalculationOptions.MirrorImage == 1)
                    {
                        c = MirrorImageCycle.GetCycle(Solver.NeedsDisplayCalculations, this, c);
                    }
                    if (FlameOrb)
                    {
                        // add flame orb mix-in
                        c = FlameOrbCycle.GetCycle(Solver.NeedsDisplayCalculations, this, c, false);
                    }
                    if (CalculationOptions.PlayerLevel >= 81 && CalculationOptions.FlameOrb == 1)
                    {
                        c = FlameOrbCycle.GetCycle(Solver.NeedsDisplayCalculations, this, c, true);
                    }
                    if (Combustion)
                    {
                        // add combustion mix-in
                        c = CombustionCycle.GetCycle(Solver.NeedsDisplayCalculations, this, c);
                    }
                }

                c.CycleId = cycleId;
            }

            return c;
        }

        private Cycle GetAveragedHasteCycle(CycleId cycleId)
        {
            float baseProcHaste = 0;
            // construct possible proc combinations
            // for T13 use base cycle with T13 stacked
            Cycle baseCycle;
            if (Solver.Mage2T13)
            {
                CastingState subState = CastingState.New(Solver, Effects, Frozen, 500);
                subState.ForceT13 = false;
                subState.ReferenceCastingState = this;
                baseCycle = subState.GetNewCycle(cycleId);
            }
            else
            {
                baseCycle = GetNewCycle(cycleId);
            }
            // on use stacking items
            for (int i = 0; i < Solver.StackingHasteEffectCooldownsCount; i++)
            {
                EffectCooldown effectCooldown = Solver.StackingHasteEffectCooldowns[i];
                if (EffectsActive(effectCooldown.Mask))
                {
                    Stats specialStats = effectCooldown.SpecialEffect.Stats;
                    for (int j = 0; j < specialStats._rawSpecialEffectDataSize; j++)
                    {
                        SpecialEffect effect = specialStats._rawSpecialEffectData[j];
                        if (effect.Stats.HasteRating > 0)
                        {
                            float trigInterval;
                            float trigChance;
                            if (baseCycle.GetTriggerData(effect, out trigInterval, out trigChance))
                            {
                                baseProcHaste += effect.GetAverageStackSize(trigInterval, trigChance, 3.0f, effectCooldown.SpecialEffect.Duration) * effect.Stats.HasteRating;
                            }
                        }
                    }
                }
            }
            int N = Solver.HasteRatingEffectsCount;
            SpecialEffect[] hasteRatingEffects = new SpecialEffect[N];
            Array.Copy(Solver.HasteRatingEffects, 0, hasteRatingEffects, 0, N);
            if (baseProcHaste == 0 && N == 0) return baseCycle;
            float[] triggerInterval = new float[N];
            float[] triggerChance = new float[N];
            float[] offset = new float[N];
            float[] resets = new float[N];
            for (int i = 0; i < N; i++)
            {
                baseCycle.GetTriggerData(hasteRatingEffects[i], out triggerInterval[i], out triggerChance[i]);
                if (hasteRatingEffects[i].MaxStack > 1)
                {
                    if (hasteRatingEffects[i] == Solver.SpecialEffect2T13)
                    {
                        switch (Solver.Specialization) // guesstimates
                        {
                            case Specialization.Arcane:
                                resets[i] = 1 + (int)(CalculationOptions.FightDuration / Solver.ArcanePowerCooldown);
                                break;
                            case Specialization.Fire:
                                resets[i] = 1 + (int)(CalculationOptions.FightDuration / Solver.CombustionCooldown);
                                break;
                            case Specialization.Frost:
                                resets[i] = 1 + (int)(CalculationOptions.FightDuration / Solver.IcyVeinsCooldown);
                                break;
                        }
                    }
                    else
                    {
                        resets[i] = 1;
                    }
                }
            }
            WeightedStat[] result = SpecialEffect.GetAverageCombinedUptimeCombinations(hasteRatingEffects, triggerInterval, triggerChance, offset, 3.0f, CalculationOptions.FightDuration, AdditiveStat.HasteRating, resets);
            if (HasteProcs == null)
            {
                HasteProcs = new CastingState[result.Length];
                int t13mask = 1 << (N - 1);
                for (int i = 0; i < result.Length; i++)
                {
                    CastingState subState = CastingState.New(Solver, Effects, Frozen, baseProcHaste + result[i].Value);
                    subState.ReferenceCastingState = this;
                    subState.ForceT13 = false;
                    if (Solver.Mage2T13 && (t13mask & i) == 0)
                    {
                        subState.ForceT13 = true;
                    }
                    HasteProcs[i] = subState;
                }
            }
            Cycle c = Cycle.New(Solver.NeedsDisplayCalculations, this);
            c.HasteProcs = HasteProcs;
            c.HasteProcsUptime = result;
            c.Name = baseCycle.Name;
            c.AreaEffect = baseCycle.AreaEffect;
            for (int i = 0; i < result.Length; i++)
            {
                Cycle subCycle = null;
                if ((Solver.Mage2T13 && HasteProcs[i].ProcHasteRating == 500) || (!Solver.Mage2T13 && HasteProcs[i].ProcHasteRating == 0))
                {
                    subCycle = baseCycle;
                }
                else
                {
                    subCycle = HasteProcs[i].GetNewCycle(cycleId);
                }
                c.AddCycle(Solver.NeedsDisplayCalculations, subCycle, result[i].Chance / subCycle.CastTime);
            }
            // if base proc is not zero then the substates are different for each cycle
            if (baseProcHaste > 0)
            {
                HasteProcs = null;
            }
            return c;
        }

        private Cycle GetRawCycle(CycleId cycleId)
        {
            Cycle c = null;
            switch (cycleId)
            {
                case CycleId.ABSpam234AM:
                    c = ABSpam234AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam34AM:
                    c = ABSpam34AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam4AM:
                    c = ABSpam4AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam0234AMABar:
                    c = ABSpam0234AMABar.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam0234AMABABar:
                    c = ABSpam0234AMABABar.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2ABar2AMABar0AMABABar:
                    c = AB2ABar2AMABar0AMABABar.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3ABar023AM:
                    c = AB3ABar023AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB23ABar023AM:
                    c = AB23ABar023AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2ABar02AMABABar:
                    c = AB2ABar02AMABABar.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar1AM:
                    c = ABABar1AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2ABar12AMABABar:
                    c = AB2ABar12AMABABar.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.Frostbolt:
                    c = GetSpell(SpellId.Frostbolt);
                    break;
                case CycleId.Fireball:
                    c = GetSpell(SpellId.Fireball);
                    break;
                case CycleId.FBPyro:
                    c = FBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, 0);
                    c.CalculateEffects();
                    if (c.effectCrit > 0)
                    {
                        c = FBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, (float)c.effectCrit);
                    }
                    break;
                case CycleId.FBLBPyro:
                    c = FBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, 1, 0);
                    c.CalculateEffects();
                    if (c.effectCrit > 0)
                    {
                        c = FBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, 1, (float)c.effectCrit);
                    }
                    break;
                case CycleId.FFBLBPyro:
                    c = FFBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, 1, 0);
                    c.CalculateEffects();
                    if (c.effectCrit > 0)
                    {
                        c = FFBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, 1, (float)c.effectCrit);
                    }
                    break;
                case CycleId.FBLB3Pyro:
                    c = FBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, Math.Min(3, CalculationOptions.AoeTargets), 0);
                    c.CalculateEffects();
                    if (c.effectCrit > 0)
                    {
                        c = FBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, Math.Min(3, CalculationOptions.AoeTargets), (float)c.effectCrit);
                    }
                    break;
                case CycleId.FFBLB3Pyro:
                    c = FFBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, Math.Min(3, CalculationOptions.AoeTargets), 0);
                    c.CalculateEffects();
                    if (c.effectCrit > 0)
                    {
                        c = FFBLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, Math.Min(3, CalculationOptions.AoeTargets), (float)c.effectCrit);
                    }
                    break;
                case CycleId.FBScPyro:
                    c = FBScPyro.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FFBPyro:
                    c = FFBPyro.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FFBScPyro:
                    c = FFBScPyro.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FFBScLBPyro:
                    c = FFBScLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrostfireBolt:
                    c = GetSpell(SpellId.FrostfireBolt);
                    break;
                case CycleId.ArcaneBlastSpam:
                    c = GetSpell(SpellId.ArcaneBlast4);
                    break;
                case CycleId.ABSpam04MBAM:
                    c = ABSpam04MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam024MBAM:
                    c = ABSpam024MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam034MBAM:
                    c = ABSpam034MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam0234MBAM:
                    c = ABSpam0234MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam4MBAM:
                    c = ABSpam4MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam24MBAM:
                    c = ABSpam24MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam234MBAM:
                    c = ABSpam234MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB4AM234MBAM:
                    c = AB4AM234MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3AM23MBAM:
                    c = AB3AM23MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB4AM0234MBAM:
                    c = AB4AM0234MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3AM023MBAM:
                    c = AB3AM023MBAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABarAM:
                    c = new ABarAM(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.ABP:
                    c = new ABP(this);
                    break;*/
                case CycleId.ABAM:
                    c = ABAM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpamMBAM:
                    c = new ABSpamMBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam3C:
                    c = new ABSpam3C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam03C:
                    c = new ABSpam03C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2ABar3C:
                    c = new AB2ABar3C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar2C:
                    c = new ABABar2C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar2MBAM:
                    c = new ABABar2MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar1MBAM:
                    c = new ABABar1MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar3C:
                    c = new ABABar3C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3ABar3MBAM:
                    c = new AB3ABar3MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3AM:
                    c = AB3AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2AM:
                    c = AB2AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3AM2MBAM:
                    c = new AB3AM2MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2ABar2MBAM:
                    c = new AB2ABar2MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar0MBAM:
                    c = new ABABar0MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.ABABar:
                    c = new ABABar(this);
                    break;*/
                case CycleId.ABSpam3MBAM:
                    c = new ABSpam3MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABSpam03MBAM:
                    c = new ABSpam03MBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.ABAMABar:
                    c = new ABAMABar(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2AMABar:
                    c = new AB2AMABar(Calculations.NeedsDisplayCalculations, this);
                    break;*/
                case CycleId.AB3AMABar:
                    c = AB3AMABar.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3ABar123AM:
                    c = AB3ABar123AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB4ABar1234AM:
                    c = AB4ABar1234AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB4ABar34AM:
                    c = AB4ABar34AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB4ABar4AM:
                    c = AB4ABar4AM.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3AMABar2C:
                    c = new AB3AMABar2C(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.AB32AMABar:
                    c = new AB32AMABar(Calculations.NeedsDisplayCalculations, this);
                    break;*/
                case CycleId.AB3ABar3C:
                    c = new AB3ABar3C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar0C:
                    c = new ABABar0C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ABABar1C:
                    c = new ABABar1C(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.ABABarY:
                    c = new ABABarY(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2ABar:
                    c = new AB2ABar(Calculations.NeedsDisplayCalculations, this);
                    break;*/
                case CycleId.AB2ABar2C:
                    c = new AB2ABar2C(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB2ABarMBAM:
                    c = new AB2ABarMBAM(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ArcaneManaNeutral:
                    c = ArcaneManaNeutral.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.AB3ABar:
                    c = new AB3ABar(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3ABarX:
                    c = new AB3ABarX(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AB3ABarY:
                    c = new AB3ABarY(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FBABar:
                    c = new FBABar(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBABar:
                    c = new FrBABar(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FFBABar:
                    c = new FFBABar(Calculations.NeedsDisplayCalculations, this);
                    break;*/
                /*case CycleId.ABAMP:
                    c = new ABAMP(this);
                    break;
                case CycleId.AB3AMSc:
                    c = new AB3AMSc(this);
                    break;
                case CycleId.ABAM3Sc:
                    c = new ABAM3Sc(this);
                    break;
                case CycleId.ABAM3Sc2:
                    c = new ABAM3Sc2(this);
                    break;
                case CycleId.ABAM3FrB:
                    c = new ABAM3FrB(this);
                    break;
                case CycleId.ABAM3FrB2:
                    c = new ABAM3FrB2(this);
                    break;
                case CycleId.ABFrB:
                    c = new ABFrB(this);
                    break;
                case CycleId.AB3FrB:
                    c = new AB3FrB(this);
                    break;
                case CycleId.ABFrB3FrB:
                    c = new ABFrB3FrB(this);
                    break;
                case CycleId.ABFrB3FrB2:
                    c = new ABFrB3FrB2(this);
                    break;
                case CycleId.ABFrB3FrBSc:
                    c = new ABFrB3FrBSc(this);
                    break;
                case CycleId.ABFB3FBSc:
                    c = new ABFB3FBSc(this);
                    break;
                case CycleId.AB3Sc:
                    c = new AB3Sc(this);
                    break;*/
                /*case CycleId.FBSc:
                    c = new FBSc(this);
                    break;
                case CycleId.FBFBlast:
                    c = new FBFBlast(this);
                    break;*/
                case CycleId.FrBFBIL:
                    c = FrBFBIL.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBDFFBIL:
                    c = FrBDFFBIL.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBDFFFBIL:
                    c = FrBDFFFBIL.GetSolvedCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBDFFFB:
                    c = FrBDFFFB.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBILFB:
                    c = FrBILFB.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBIL:
                    c = FrBIL.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBFB:
                    c = FrBFB.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FBScLBPyro:
                    c = FBScLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.FB2ABar:
                    c = new FB2ABar(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrB2ABar:
                    c = new FrB2ABar(Calculations.NeedsDisplayCalculations, this);
                    break;*/
                case CycleId.ScLBPyro:
                    c = ScLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, 0);
                    c.CalculateEffects();
                    if (c.effectCrit > 0)
                    {
                        c = ScLBPyro.GetCycle(Solver.NeedsDisplayCalculations, this, (float)c.effectCrit);
                    }
                    break;
                case CycleId.ABABarSlow:
                    c = new ABABarSlow(Solver.NeedsDisplayCalculations, this);
                    break;
                /*case CycleId.FBABarSlow:
                    c = new FBABarSlow(Calculations.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FrBABarSlow:
                    c = new FrBABarSlow(Calculations.NeedsDisplayCalculations, this);
                    break;*/
                case CycleId.CustomSpellMix:
                    c = new SpellCustomMix(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.ArcaneMissiles:
                    c = GetSpell(SpellId.ArcaneMissiles);
                    break;
                case CycleId.Scorch:
                    c = GetSpell(SpellId.Scorch);
                    break;
                case CycleId.ArcaneExplosion:
                    c = GetSpell(SpellId.ArcaneExplosion0);
                    break;
                case CycleId.AE4AB:
                    c = AE4AB.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.AERampAB:
                    c = AERampAB.GetCycle(Solver.NeedsDisplayCalculations, this);
                    break;
                case CycleId.FlamestrikeSpammed:
                    c = GetSpell(SpellId.FlamestrikeSpammed);
                    break;
                case CycleId.FlamestrikeSingle:
                    c = GetSpell(SpellId.FlamestrikeSingle);
                    break;
                case CycleId.Blizzard:
                    c = GetSpell(SpellId.Blizzard);
                    break;
                case CycleId.BlastWave:
                    c = GetSpell(SpellId.BlastWave);
                    break;
                case CycleId.DragonsBreath:
                    c = GetSpell(SpellId.DragonsBreath);
                    break;
                case CycleId.ConeOfCold:
                    c = GetSpell(SpellId.ConeOfCold);
                    break;
            }
            return c;
        }

        public virtual Spell GetSpell(SpellId spellId)
        {
            //Spell s = Spells[(int)spellId];
            //if (s != null) return s;
            Spell s = null;
            //if (Spells.TryGetValue((int)spellId, out s)) return s;
            for (int i = 0; i < SpellsCount; i++)
            {
                Spell spell = Spells[i];
                if (spell.SpellId == spellId) return spell;
            }
            switch (spellId)
            {
                case SpellId.Waterbolt:
                    s = Solver.WaterboltTemplate.GetSpell(this);
                    break;
                case SpellId.MirrorImage:
                    s = Solver.MirrorImageTemplate.GetSpell(this);
                    break;
            }
            if (s == null && ReferenceCastingState != null)
            {
                // get base spell and recalculate cast time
                Spell reference = ReferenceCastingState.GetSpell(spellId);
                if (reference.DotTickInterval == 0) // WORKAROUND: calculating from reference doesn't support recalculation of hasted ticks at the moment
                {
                    s = Spell.NewFromReference(reference, this);
                }
            }
            if (s == null)
            {
                switch (spellId)
                {
                    case SpellId.Combustion:
                        s = Solver.CombustionTemplate.GetSpell(this);
                        break;
                    case SpellId.ArcaneMissiles:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, false, 0);
                        break;
                    case SpellId.ArcaneMissiles1:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, false, 1);
                        break;
                    case SpellId.ArcaneMissiles2:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, false, 2);
                        break;
                    case SpellId.ArcaneMissiles3:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, false, 3);
                        break;
                    case SpellId.ArcaneMissiles4:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, false, 4);
                        break;
                    case SpellId.ArcaneMissilesMB:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, true, 0);
                        break;
                    case SpellId.ArcaneMissilesMB1:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, true, 1);
                        break;
                    case SpellId.ArcaneMissilesMB2:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, true, 2);
                        break;
                    case SpellId.ArcaneMissilesMB3:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, true, 3);
                        break;
                    case SpellId.ArcaneMissilesMB4:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, true, 4);
                        break;
                    case SpellId.ArcaneMissilesNoProc:
                        s = Solver.ArcaneMissilesTemplate.GetSpell(this, false, true, false, false, 0);
                        break;
                    case SpellId.Frostbolt:
                        s = Solver.FrostboltTemplate.GetSpell(this);
                        break;
                    case SpellId.FrostboltNoCC:
                        s = Solver.FrostboltTemplate.GetSpell(this, true, false, false);
                        break;
                    case SpellId.DeepFreeze:
                        s = Solver.DeepFreezeTemplate.GetSpell(this);
                        break;
                    case SpellId.FlameOrb:
                        s = Solver.FlameOrbTemplate.GetSpell(this);
                        break;
                    case SpellId.Fireball:
                        s = Solver.FireballTemplate.GetSpell(this, false, false);
                        break;
                    case SpellId.FireballBF:
                        s = Solver.FireballTemplate.GetSpell(this, false, true);
                        break;
                    case SpellId.FrostfireBolt:
                        s = Solver.FrostfireBoltTemplate.GetSpell(this, false, false);
                        break;
                    case SpellId.FrostfireBoltBF:
                        s = Solver.FrostfireBoltTemplate.GetSpell(this, false, true);
                        break;
                    case SpellId.PyroblastSpammed:
                        s = Solver.PyroblastHardCastTemplate.GetSpell(this, false, true);
                        break;
                    case SpellId.PyroblastDotUptime:
                        s = Solver.PyroblastHardCastTemplate.GetSpell(this, false);
                        break;
                    case SpellId.FireBlast:
                        s = Solver.FireBlastTemplate.GetSpell(this);
                        break;
                    case SpellId.Scorch:
                        s = Solver.ScorchTemplate.GetSpell(this);
                        break;
                    case SpellId.ScorchNoCC:
                        s = Solver.ScorchTemplate.GetSpell(this, false);
                        break;
                    case SpellId.ArcaneBarrage:
                        s = Solver.ArcaneBarrageTemplate.GetSpell(this, 0, false);
                        break;
                    case SpellId.ArcaneBarrage1:
                        s = Solver.ArcaneBarrageTemplate.GetSpell(this, 1, false);
                        break;
                    case SpellId.ArcaneBarrage2:
                        s = Solver.ArcaneBarrageTemplate.GetSpell(this, 2, false);
                        break;
                    case SpellId.ArcaneBarrage3:
                        s = Solver.ArcaneBarrageTemplate.GetSpell(this, 3, false);
                        break;
                    case SpellId.ArcaneBarrage4:
                        s = Solver.ArcaneBarrageTemplate.GetSpell(this, 4, false);
                        break;
                    case SpellId.ArcaneBlast3:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 3);
                        break;
                    case SpellId.ArcaneBlast4:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 4);
                        break;
                    case SpellId.ArcaneBlast3NoCC:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 3, true, false, false);
                        break;
                    case SpellId.ArcaneBlastRaw:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this);
                        break;
                    case SpellId.ArcaneBlast0:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 0);
                        break;
                    case SpellId.ArcaneBlast0NoCC:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 0, true, false, false);
                        break;
                    case SpellId.ArcaneBlast1:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 1);
                        break;
                    case SpellId.ArcaneBlast1NoCC:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 1, true, false, false);
                        break;
                    case SpellId.ArcaneBlast2:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 2);
                        break;
                    case SpellId.ArcaneBlast2NoCC:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 2, true, false, false);
                        break;
                    case SpellId.ArcaneBlast0Hit:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 0, true);
                        break;
                    case SpellId.ArcaneBlast1Hit:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 1, true);
                        break;
                    case SpellId.ArcaneBlast2Hit:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 2, true);
                        break;
                    case SpellId.ArcaneBlast3Hit:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 3, true);
                        break;
                    case SpellId.ArcaneBlast0Miss:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 0, false);
                        break;
                    case SpellId.ArcaneBlast1Miss:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 1, false);
                        break;
                    case SpellId.ArcaneBlast2Miss:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 2, false);
                        break;
                    case SpellId.ArcaneBlast3Miss:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 3, false);
                        break;
                    case SpellId.IceLance:
                        s = Solver.IceLanceTemplate.GetSpell(this);
                        break;
                    case SpellId.ArcaneExplosion0:
                        s = Solver.ArcaneExplosionTemplate.GetSpell(this, 0);
                        break;
                    case SpellId.ArcaneExplosion1:
                        s = Solver.ArcaneExplosionTemplate.GetSpell(this, 1);
                        break;
                    case SpellId.ArcaneExplosion2:
                        s = Solver.ArcaneExplosionTemplate.GetSpell(this, 2);
                        break;
                    case SpellId.ArcaneExplosion3:
                        s = Solver.ArcaneExplosionTemplate.GetSpell(this, 3);
                        break;
                    case SpellId.ArcaneExplosion4:
                        s = Solver.ArcaneExplosionTemplate.GetSpell(this, 4);
                        break;
                    case SpellId.FlamestrikeSpammed:
                        s = Solver.FlamestrikeTemplate.GetSpell(this, true);
                        break;
                    case SpellId.FlamestrikeSingle:
                        s = Solver.FlamestrikeTemplate.GetSpell(this, false);
                        break;
                    case SpellId.Blizzard:
                        s = Solver.BlizzardTemplate.GetSpell(this);
                        break;
                    case SpellId.BlastWave:
                        s = Solver.BlastWaveTemplate.GetSpell(this);
                        break;
                    case SpellId.DragonsBreath:
                        s = Solver.DragonsBreathTemplate.GetSpell(this);
                        break;
                    case SpellId.ConeOfCold:
                        s = Solver.ConeOfColdTemplate.GetSpell(this);
                        break;
                    case SpellId.ArcaneBlast0POM:
                        s = Solver.ArcaneBlastTemplate.GetSpell(this, 0, false, false, true);
                        break;
                    case SpellId.FireballPOM:
                        s = Solver.FireballTemplate.GetSpell(this, true, false);
                        break;
                    case SpellId.Slow:
                        s = Solver.SlowTemplate.GetSpell(this);
                        break;
                    case SpellId.FrostboltPOM:
                        s = Solver.FrostboltTemplate.GetSpell(this, false, false, true);
                        break;
                    case SpellId.PyroblastPOM:
                        s = Solver.PyroblastTemplate.GetSpell(this, true, false);
                        break;
                    case SpellId.PyroblastPOMSpammed:
                        s = Solver.PyroblastTemplate.GetSpell(this, true, true);
                        break;
                    case SpellId.PyroblastPOMDotUptime:
                        s = Solver.PyroblastTemplate.GetSpell(this, true);
                        break;
                    case SpellId.LivingBomb:
                        s = Solver.LivingBombTemplate.GetSpell(this, false);
                        break;
                    case SpellId.LivingBombAOE:
                        s = Solver.LivingBombTemplate.GetSpell(this, true);
                        break;
                    case SpellId.ArcaneBomb:
                        s = Solver.ArcaneBombTemplate.GetSpell(this, false);
                        break;
                    case SpellId.ArcaneBombAOE:
                        s = Solver.ArcaneBombTemplate.GetSpell(this, true);
                        break;
                    case SpellId.MageWard:
                        s = Solver.MageWardTemplate.GetSpell(this);
                        break;
                }
            }

            if (s != null)
            {
                s.SpellId = spellId;
                //Spells[(int)spellId] = s;
                //Spells.Add(s);
                if (SpellsCount >= Spells.Length)
                {
                    int length = 2 * Spells.Length;
                    Spell[] destinationArray = new Spell[length];
                    Array.Copy(Spells, 0, destinationArray, 0, SpellsCount);
                    Spells = destinationArray;
                }
                Spells[SpellsCount++] = s;
            }

            return s;
        }
    }
}
