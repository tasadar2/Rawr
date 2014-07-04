﻿using System;
using System.Collections.Generic;

namespace Rawr.Retribution
{
    public abstract class Skill : Ability<PaladinTalents, StatsRetri, CalculationOptionsRetribution>
    {
        public Skill(string name, Character character, StatsRetri stats, AbilityType abilityType, DamageType damageType, bool hasGCD = true, bool noMultiplier = false)
            : base(name, character, stats, abilityType, damageType, PLAYER_ROLES.MeleeDPS, hasGCD, noMultiplier) 
        {
            AbilityDamageMultiplierOthersString = "Two-Handed Spec";
        }

        public override DamageType DamageType { get { return base.DamageType; }
            set {
                base.DamageType = value;
                switch (value)
                {
                    case DamageType.Physical:
                        AbilityDamageMulitplier[Multiplier.Others] = (1f + PaladinConstants.TWO_H_SPEC);
                        break;
                    case DamageType.Holy:
                        AbilityDamageMulitplier[Multiplier.Magical] *= (1f + _inqUptime * PaladinConstants.INQ_COEFF);
                        break;
                }
            }
        }

        private float _inqUptime = 1f;
        public float InqUptime { get { return _inqUptime; } 
            set {
                if (DamageType == DamageType.Holy) {
                    AbilityDamageMulitplier[Multiplier.Magical] /= _inqUptime * (PaladinConstants.INQ_COEFF + 1f);
                    AbilityDamageMulitplier[Multiplier.Magical] *= value * (PaladinConstants.INQ_COEFF + 1f);
                }
                _inqUptime = value;
                foreach (Skill ability in _triggers)
                    ability.InqUptime = _inqUptime;
                if (_AbilityDamage > 1f)
                    RecalculateDamageValues();
            }
        }
    }
    
    public class Judgement : Skill
    {
        public Judgement(string name, Character character, StatsRetri stats) : base(name, character, stats, AbilityType.Range, DamageType.Holy) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.Ranged);
            CT.AbilityCritCorr = Talents.ArbiterOfTheLight * PaladinConstants.ARBITER_OF_THE_LIGHT;
            AbilityDamageMulitplier[Multiplier.Glyphs] = (1f + (Talents.GlyphOfJudgement ? PaladinConstants.GLYPH_OF_JUDGEMENT : 0f));
            AbilityDamageMulitplier[Multiplier.Others] = (1f + PaladinConstants.TWO_H_SPEC);
            Cooldown = PaladinConstants.JUDGE_COOLDOWN - _stats.JudgementCDReduction;
            AbilityDamage = PaladinConstants.JUDGE_DMG;
        }
    }

    public class JudgementOfRighteousness : Judgement
    {
        public JudgementOfRighteousness(Character character, StatsRetri stats) : base("Judgement of Righteousness", character, stats) 
        {
            AbilityDamage += _stats.SpellPower * PaladinConstants.JOR_COEFF_SP +
                             _stats.AttackPower * PaladinConstants.JOR_COEFF_AP;
        }
    }

    public class JudgementOfTruth : Judgement
    {
        public JudgementOfTruth(Character character, StatsRetri stats, float averageStack) : base("Judgement of Truth", character, stats)
        {
            AverageStackSize = averageStack;
            AbilityDamage += (_stats.SpellPower * PaladinConstants.JOT_JUDGE_COEFF_SP + 
                              _stats.AttackPower * PaladinConstants.JOT_JUDGE_COEFF_AP)
                             * (1f + PaladinConstants.JOT_JUDGE_COEFF_STACK * AverageStackSize);
        }

        public float AverageStackSize { get; private set; }
    }

    public class Inquisition : Skill
    {
        public Inquisition(Character character, StatsRetri stats, int hp) : base("Inquisition", character, stats, AbilityType.Spell, DamageType.Holy) 
        { 
            HP = hp;
            Cooldown = Duration;
        }
        private int HP;
        public float Duration { get { return (HP + (_stats.T11_4P ? 1f : 0f)) * 4f * (1f + Talents.InquiryOfFaith * PaladinConstants.INQUIRY_OF_FAITH_INQ); } }
    }

    public class TemplarsVerdict : Skill
    {
        public TemplarsVerdict(Character character, StatsRetri stats) : base("Templar's Verdict", character, stats, AbilityType.Melee, DamageType.Physical) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            CT.AbilityCritCorr = Talents.ArbiterOfTheLight * PaladinConstants.ARBITER_OF_THE_LIGHT;
            AbilityDamageMulitplier[Multiplier.Talents] = (1f + PaladinConstants.CRUSADE * Talents.Crusade);
            AbilityDamageMulitplier[Multiplier.Glyphs] = (1f + (Talents.GlyphOfTemplarsVerdict ? PaladinConstants.GLYPH_OF_TEMPLARS_VERDICT : .0f)); 
            AbilityDamageMulitplier[Multiplier.Sets] = (1f + (Stats.T11_2P ? .1f : 0f));
            AbilityDamage = AbilityHelper.WeaponDamage(Character, _stats.AttackPower) * PaladinConstants.TV_THREE_STK;

            //Hand of Light
            AddTrigger(new HandofLight(Character, Stats, AverageDamage));
        }

        //Some things are additive not multiplicative for TV
        public override float GetMulitplier()
        {
            float multiplier = 1f;
            float add = 1f;
            foreach (KeyValuePair<Multiplier, float> kvp in AbilityDamageMulitplier)
            { 
                if (kvp.Key == Multiplier.Glyphs ||
                    kvp.Key == Multiplier.Talents ||
                    kvp.Key == Multiplier.Sets)
                    add += kvp.Value - 1f;
                else
                    multiplier *= kvp.Value;
            }
            return multiplier * add;
        }
    }

    public class CrusaderStrike : Skill
    {
        public CrusaderStrike(Character character, StatsRetri stats) : base("Crusader Strike", character, stats, AbilityType.Melee, DamageType.Physical, true) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            CT.AbilityCritCorr = Talents.RuleOfLaw * PaladinConstants.RULE_OF_LAW +
                                 (Talents.GlyphOfCrusaderStrike ? PaladinConstants.GLYPH_OF_CRUSADER_STRIKE : 0);
            AbilityDamageMulitplier[Multiplier.Talents] = (1f + PaladinConstants.CRUSADE * Talents.Crusade);
            AbilityDamageMulitplier[Multiplier.Sets] = (1f + Stats.BonusDamageMultiplierCrusaderStrike);
            Cooldown = PaladinConstants.CS_COOLDOWN / (Talents.SanctityOfBattle > 0 ? (1f + _stats.SpellHaste) : 1f);
            AbilityDamage = AbilityHelper.WeaponDamage(_character, _stats.AttackPower, true) * PaladinConstants.CS_DMG_BONUS;

            if (_stats.T12_2P)
            {
                MagicDamage spell = new MagicDamage("T12 2P Bonus", _character, new StatsRetri(), DamageType.Fire, true);
                spell.CT.CanCrit = false;
                spell.CT.CanMiss = false;
                spell.AbilityDamage = AverageDamage * .15f;
                AddTrigger(spell);
            }
            //Hand of Light
            AddTrigger(new HandofLight(Character, Stats, AverageDamage));

            _CycleTime = GCD * 2f + (1.5f / (1 + Stats.SpellHaste) + Latency);
        }

        private float _CycleTime;
        public override float CooldownWithLatency
        {
            get
            {
                return _CycleTime;
            }
        }
    }

    public class HandofLight : Skill
    {
        public HandofLight(Character character, StatsRetri stats, float amountBefore) : base("Hand of Light", character, stats, AbilityType.Spell, DamageType.Holy, false) 
        {
            AmountBefore = amountBefore;
            CT = new BaseSpellCombatTable(Character.BossOptions, _stats, Attacktype.Spell);
            CT.CanMiss = false;
            CT.CanCrit = false;
            AbilityDamageMulitplier.Clear(); //Only benefit from Magical
            AbilityDamageMulitplier[Multiplier.Magical] = (1f + _stats.BonusHolyDamageMultiplier) * (1f + InqUptime * PaladinConstants.INQ_COEFF);
            AbilityDamage = AmountBefore * (8f + StatConversion.GetMasteryFromRating(_stats.MasteryRating, CharacterClass.Paladin)) * PaladinConstants.HOL_COEFF;
        }

        public float AmountBefore { get; set; }
    }

    public class DivineStorm : Skill
    {
        public DivineStorm(Character character, StatsRetri stats) : base("Divine Storm", character, stats, AbilityType.Melee, DamageType.Physical) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            Cooldown = PaladinConstants.DS_COOLDOWN / (Talents.SanctityOfBattle > 0 ? (1f + _stats.PhysicalHaste) : 1f);
            MaxTargets = 100;
            AbilityDamage = AbilityHelper.WeaponDamage(_character, _stats.AttackPower, true) * PaladinConstants.DS_DMG_BONUS; 
        }
    }
    
    public class HammerOfWrath : Skill
    {
        public HammerOfWrath(Character character, StatsRetri stats) : base("Hammer of Wrath", character, stats, AbilityType.Range, DamageType.Holy) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.Ranged);
            CT.AbilityCritCorr = Talents.SanctifiedWrath * PaladinConstants.SANCTIFIED_WRATH_CRIT;
            Cooldown = PaladinConstants.HOW_COOLDOWN;
            AbilityDamage = PaladinConstants.HOW_AVG_DMG +
                            PaladinConstants.HOW_COEFF_SP * _stats.SpellPower +
                            PaladinConstants.HOW_COEFF_AP * _stats.AttackPower;
        }
    }

    public class Exorcism : Skill
    {
        public Exorcism(Character character, StatsRetri stats, float chanceToProc) : base("Exorcism", character, stats, AbilityType.Spell, DamageType.Holy) 
        {
            CT = new BaseSpellCombatTable(Character.BossOptions, _stats, Attacktype.Spell);
            CT.AbilityCritCorr = (Character.BossOptions.MobType == (int)MOB_TYPES.DEMON || Character.BossOptions.MobType == (int)MOB_TYPES.UNDEAD) ? 1f : 0;
            AbilityDamageMulitplier[Multiplier.Talents] = (1f + (Talents.TheArtOfWar > 0 ? PaladinConstants.THE_ART_OF_WAR : 0f) +
                                                                PaladinConstants.BLAZING_LIGHT * Talents.BlazingLight);
            AbilityDamageMulitplier[Multiplier.Glyphs] = (1f + (Talents.GlyphOfExorcism ? PaladinConstants.GLYPH_OF_EXORCISM : 0f));
            Cooldown = AbilityHelper.WeaponSpeed(_character, _stats.PhysicalHaste) * (1f / (Talents.TheArtOfWar * PaladinConstants.EXO_PROC_CHANCE)) / chanceToProc;
            AbilityDamage = PaladinConstants.EXO_AVG_DMG +
                            PaladinConstants.EXO_COEFF * Math.Max(_stats.SpellPower, _stats.AttackPower);
        }
    }

    public class HolyWrath : Skill
    {
        public HolyWrath(Character character, StatsRetri stats) : base("Holy Wrath", character, stats, AbilityType.Spell, DamageType.Holy) 
        {
            CT = new BaseSpellCombatTable(Character.BossOptions, _stats, Attacktype.Spell);
            CT.AbilityCritCorr = (Character.BossOptions.MobType == (int)MOB_TYPES.DEMON || Character.BossOptions.MobType == (int)MOB_TYPES.UNDEAD) ? 1f : 0;
            Cooldown = PaladinConstants.HOLY_WRATH_COOLDOWN;
            Meteor = true;
            MaxTargets = 100;
            AbilityDamage = PaladinConstants.HOLY_WRATH_BASE_DMG +
                            PaladinConstants.HOLY_WRATH_COEFF * _stats.SpellPower;
        }
    }

    public class Consecration : Skill
    {
        public Consecration(Character character, StatsRetri stats) : base("Consecration", character, stats, AbilityType.Spell, DamageType.Holy) 
        {
            CT = new BaseSpellCombatTable(Character.BossOptions, _stats, Attacktype.Spell);
            CT.CanCrit = false;
            Cooldown = PaladinConstants.CONS_COOLDOWN;
            TickCount = (Talents.GlyphOfConsecration ? 12f : 10f);
            MaxTargets = 100;
            AbilityDamage = (PaladinConstants.CONS_BASE_DMG +
                             PaladinConstants.CONS_COEFF_SP * _stats.SpellPower +
                             PaladinConstants.CONS_COEFF_AP * _stats.AttackPower)
                            / 10;
        }
    }

    public class GuardianOfTheAncientKings : Skill
    {
        public GuardianOfTheAncientKings(Character character, StatsRetri stats) : base("Guardian of the ancient Kings", character, new StatsRetri(), AbilityType.Melee, DamageType.Physical, false)
        {
            CT = new BasePhysicalWhiteCombatTable(character.BossOptions, _stats, Attacktype.MeleeMH);
            TickCount = PaladinConstants.GOAK_ATTACKS_PER_CAST;
            AbilityDamageMulitplier.Remove(Multiplier.Others); //Remove Two handed Spec
            AbilityDamage = 7000f;

            //Final Cast
            AddTrigger(new GuardianOfTheAncientKingsFinal(character, stats));
        }
    }

    public class GuardianOfTheAncientKingsFinal : Skill
    {
        public GuardianOfTheAncientKingsFinal(Character character, StatsRetri stats) : base("Ancient Fury", character, stats, AbilityType.Spell, DamageType.Holy, false)
        {
            CT = new BaseSpellCombatTable(character.BossOptions, _stats, Attacktype.Spell);
            AbilityDamage = (PaladinConstants.AF_BASE_DMG * 20f) + 
                            PaladinConstants.AF_COEFF_AP * _stats.AttackPower;
        }
    }

    public class SealOfCommand : Skill
    {
        public SealOfCommand(Character character, StatsRetri stats) : base("Seal of Command", character, stats, AbilityType.Melee, DamageType.Holy, false) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            AbilityDamageMulitplier[Multiplier.Others] = (1f + PaladinConstants.TWO_H_SPEC);
            AbilityDamage = (Talents.SealsOfCommand > 0 ? AbilityHelper.WeaponDamage(_character, _stats.AttackPower) * PaladinConstants.SOC_COEFF : 0f);
        }
    }
    
    public abstract class Seal : Skill
    {
        public Seal(string name, Character character, StatsRetri stats, AbilityType abilityType) : base(name, character, stats, abilityType, DamageType.Holy, false)
        {
            AbilityDamageMulitplier[Multiplier.Talents] = (1f + PaladinConstants.SEALS_OF_THE_PURE * Talents.SealsOfThePure);
            AbilityDamageMulitplier[Multiplier.Others] = (1f + PaladinConstants.TWO_H_SPEC);
        }
    }

    public class SealOfRighteousness : Seal
    {
        public SealOfRighteousness(Character character, StatsRetri stats) : base("Seal of Righteousness", character, stats, AbilityType.Spell) 
        {
            CT = new BaseSpellCombatTable(Character.BossOptions, _stats, Attacktype.Spell);
            CT.CanMiss = false;
            MaxTargets = 100;
            AbilityDamage = AbilityHelper.BaseWeaponSpeed(_character) * (PaladinConstants.SOR_COEFF_AP * _stats.AttackPower +
                                                                         PaladinConstants.SOR_COEFF_SP * _stats.SpellPower);
        }
    }

    public class SealOfTruth : Seal
    {
        public SealOfTruth(Character character, StatsRetri stats) : base("Seal of Truth", character, stats, AbilityType.Melee) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            CT.CanMiss = false;
            AbilityDamage = AbilityHelper.WeaponDamage(_character, _stats.AttackPower) * PaladinConstants.SOT_SEAL_COEFF;
        }
    }  

    public class SealOfTruthDoT : Skill
    {
        public SealOfTruthDoT(Character character, StatsRetri stats, float averageStack) : base("Seal of Truth DoT", character, stats, AbilityType.Melee, DamageType.Holy, false)
        {
            AverageStackSize = averageStack;
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            CT.CanMiss = false;
            AbilityDamageMulitplier[Multiplier.Talents] = (1f + PaladinConstants.SEALS_OF_THE_PURE * Talents.SealsOfThePure +
                                                                PaladinConstants.INQUIRY_OF_FAITH_SEAL * Talents.InquiryOfFaith);
            AbilityDamage = AverageStackSize * (_stats.AttackPower * PaladinConstants.SOT_CENSURE_COEFF_AP +
                                                _stats.SpellPower * PaladinConstants.SOT_CENSURE_COEFF_SP);
        }

        public float AverageStackSize { get; private set; }
    }
    
    public class White : Skill
    {
        public White(Character character, StatsRetri stats) : base("Autoattack", character, stats, AbilityType.Melee, DamageType.Physical, false) 
        {
            CT = new BasePhysicalWhiteCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            AbilityDamage = AbilityHelper.WeaponDamage(_character, _stats.AttackPower);
        }
    }

    #region NULL Things
    public class NullSeal : Seal
    {
        public NullSeal(Character character, StatsRetri stats) : base("Seal", character, stats, AbilityType.Melee) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            AbilityDamage = 0;
        }
    }

    public class NullSealDoT : Skill
    {
        public NullSealDoT(Character character, StatsRetri stats) : base("Seal Dot", character, stats, AbilityType.Melee, DamageType.Holy) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.MeleeMH);
            AbilityDamage = 0;
        }
    }

    public class NullJudgement : Judgement
    {
        public NullJudgement(Character character, StatsRetri stats) : base("Judgement", character, stats) 
        {
            CT = new BasePhysicalYellowCombatTable(Character.BossOptions, _stats, Attacktype.Ranged);
            AbilityDamage = 0;
        }
    }
    #endregion

    public class MagicDamage : Skill
    {
        public MagicDamage(string name, Character character, StatsRetri stats, DamageType damageType, bool noMultiplier = false) : base(name, character, stats, AbilityType.Spell, damageType, false, noMultiplier)
        {
            CT = new BaseSpellCombatTable(Character.BossOptions, _stats, Attacktype.Spell);
            switch ((int)damageType)
            {
                case (int)DamageType.Holy:
                    AbilityDamage = stats.HolyDamage;
                    break;
                case (int)DamageType.Fire:
                    AbilityDamage = stats.FireDamage;
                    break;
                case (int)DamageType.Nature:
                    AbilityDamage = stats.NatureDamage;
                    break;
                case (int)DamageType.Frost:
                    AbilityDamage = stats.FrostDamage;
                    break;
                case (int)DamageType.Shadow:
                    AbilityDamage = stats.ShadowDamage;
                    break;
                case (int)DamageType.Arcane:
                    AbilityDamage = stats.ArcaneDamage;
                    break;
                default:
                    AbilityDamage = 0f;
                    break;
            }
        }
    }

}
