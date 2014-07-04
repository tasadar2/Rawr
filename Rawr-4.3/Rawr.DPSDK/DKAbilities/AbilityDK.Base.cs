﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DK
{
    public enum DKCostTypes : int
    {
        None = 0,
        Blood,
        Frost,
        UnHoly,
        Death, // Have abilities that convert runes to death runes provide a negative value here just like RP.
        RunicPower,
        // TIME is in ms to keep the int structure.
        CastTime, // How long does it cost to activate the ability?
        CooldownTime, // How long until we can use the ability again.  This is ability specific CD.  Not counting Rune CD. Whatever solver we use will have to keep track of Rune CDs.
        DurationTime, // How long does the ability last?
    }

    /// <summary>
    /// This class will be the base class for abilities.
    /// Each ability will inherit from this class and have to implement their own functions
    /// for the various spells/abilities.
    /// Then we will want to be able to aggregate all the data to have a solid picture of what's
    /// going on when the abilities are used.
    /// EG. IT * 2 should give us the values of 2 ITs in cost, damage, time, etc.
    /// </summary>
    abstract public class AbilityDK_Base
    {
        // This needs to then be calculated whenever someone calls for the value of a given ability.
        // Similar to the way special effects are handled w/ stats.

        #region Constants
        public const uint MIN_GCD_MS_UH = 1000;
        public const uint MIN_GCD_MS = 1500;
        public const uint INSTANT = 1;
        public const uint MELEE_RANGE = 5;
        #endregion

        #region Constructors
        /// <summary>
        /// Default constructor for the DK's abilities.
        /// </summary>
        public AbilityDK_Base()
        {
            this.szName = "";
            this.AbilityCost[(int)DKCostTypes.Blood] = 0;
            this.AbilityCost[(int)DKCostTypes.Frost] = 0;
            this.AbilityCost[(int)DKCostTypes.UnHoly] = 0;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 0;
            this.uBaseDamage = 0;
            this.uRange = MELEE_RANGE;
            this.tDamageType = ItemDamageType.Physical;
            this.Cooldown = 1500; // GCD
            this.CastTime = INSTANT;
            this.uTickRate = INSTANT;
            this.uDuration = INSTANT;
        }
        public AbilityDK_Base(string Name)
        {
            this.szName = Name;
            this.AbilityCost[(int)DKCostTypes.Blood] = 0;
            this.AbilityCost[(int)DKCostTypes.Frost] = 0;
            this.AbilityCost[(int)DKCostTypes.UnHoly] = 0;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 0;
            this.uBaseDamage = 0;
            this.uRange = MELEE_RANGE;
            this.tDamageType = ItemDamageType.Physical;
            this.Cooldown = 1500; // GCD
            this.CastTime = INSTANT;
            this.uTickRate = INSTANT;
            this.uDuration = INSTANT;
        }
        public AbilityDK_Base(CombatState CS)
        {
            this.CState = CS;
            this.szName = "";
            this.AbilityCost[(int)DKCostTypes.Blood] = 0;
            this.AbilityCost[(int)DKCostTypes.Frost] = 0;
            this.AbilityCost[(int)DKCostTypes.UnHoly] = 0;
            this.AbilityCost[(int)DKCostTypes.RunicPower] = 0;
            this.uBaseDamage = 0;
            this.uRange = MELEE_RANGE;
            this.tDamageType = ItemDamageType.Physical;
            this.Cooldown = 1500; // GCD
            this.CastTime = INSTANT;
            this.uTickRate = INSTANT;
            this.uDuration = INSTANT;
        }
        #endregion 

        /// <summary>
        /// The state of combat at the time the ability is uesd.
        /// </summary>
        public CombatState CState;

        public virtual void UpdateCombatState(CombatState CS)
        {
            CState = CS;
        }

        /// <summary>
        /// Any DK ability triggered by this ability.  
        /// Should not be recursive.
        /// This would mean FF when using IT or Glyphed HB.
        /// </summary>
        public AbilityDK_Base[] ml_TriggeredAbility;

        /// <summary>Name of the ability.</summary>
        public string szName { get; set; }
        
        /// <summary>
        /// What is the cost of the ability?
        /// INTs representing the 3 Rune Types, Runic Power, Time
        /// Use enum (int)DKCostTypes for placement.
        /// Negative costs mean they grant that item.
        /// </summary>
        public int[] AbilityCost = new int[EnumHelper.GetCount(typeof(DKCostTypes))];
        public int AbilityIndex;

        // Maybe Create a Damage object since we have abilities that return 2 types of damage.
        /// <summary>
        /// What min damage does the ability cause?
        /// For DOTs damage is on a per-tick basis.
        /// </summary>
        public uint uMinDamage { get; set; }
        /// <summary>
        /// What max damage does the ability cause?
        /// For DOTs damage is on a per-tick basis.
        /// </summary>
        public uint uMaxDamage { get; set; }
        /// <summary>
        /// Get the average value between Max and Min damage
        /// For DOTs damage is on a per-tick basis.
        /// </summary>
        virtual public uint uBaseDamage { 
            get
            {
                uint AvgDam = (this.uMinDamage + this.uMaxDamage) / 2;
                uint WDam = 0;
                // Handle non-weapon based effects:
                if (this.bWeaponRequired == true && null != this.wMH)
                {
                    WDam = (uint)(this.wMH.damage * this.fWeaponDamageModifier);
                }
                // Average out the min & max damage, then add in baseDamage from the weapon.
                // Miss rate factored in GetTotalDamage()

                return (uint)(AvgDam + WDam);
            }
            set 
            {
                // Setup so that we can just set a base damage w/o having to 
                // manually set Min & Max all the time.
                uMaxDamage = uMinDamage = value;
            }
        }
        /// <summary>
        /// What's the range of a given ability?
        /// The idea is that we want to quantify the range buffing talents.
        /// </summary>
        public uint uRange { get; set; }
        /// <summary>
        /// What's the size of the area of a given ability?
        /// The idea is that we want to quantify the area buffing talents.
        /// </summary>
        public uint uArea { get; set; }

        /// <summary>
        /// Is this an AOE ability?
        /// </summary>
        public bool bAOE { get; set; }

        /// <summary>
        /// What type of damage does this ability do?
        /// </summary>
        public ItemDamageType tDamageType  { get; set; }

        public int RunicPower 
        { 
            get
            {
                int rp = this.AbilityCost[(int)DKCostTypes.RunicPower];
                if (rp < 0 && CState != null && CState.m_Stats != null)
                    rp = (int)(rp * (1 + CState.m_Stats.BonusRPMultiplier));
                return rp;
            }
            set
            {
                this.AbilityCost[(int)DKCostTypes.RunicPower] = value;
            }
        }

        #region Time Based Items
        ///////////////////////////////////////////////////////
        // Time based items.  
        // These will all be effected by haste.
        // Haste effects GCD to a max hasted of 1.5 sec to 1 sec.
        /// <summary>
        /// How long does it take to cast in ms?
        /// 1 == instant
        /// </summary>
        public uint CastTime 
        {
            get
            {
                return Math.Max(INSTANT, (uint)this.AbilityCost[(int)DKCostTypes.CastTime]);
            }
            set
            {
                AbilityCost[(int)DKCostTypes.CastTime] = (int)value;
            }
        }
        /// <summary>
        /// Cooldown in seconds
        /// Default = 1500 millisecs == Global Cooldown
        /// GCD min == 1000 millisecs.
        /// </summary>
        public uint Cooldown 
        { 
            get 
            {
                
                uint cd = (uint)AbilityCost[(int)DKCostTypes.CooldownTime];
                if (CState != null)
                {
                    if (this.bTriggersGCD)
                        return Math.Max((CState.m_Presence == Presence.Unholy ? MIN_GCD_MS_UH : MIN_GCD_MS), cd);
                    if (CState.m_Stats != null)
                        cd = (uint)(cd / (1 + CState.m_Stats.PhysicalHaste));
                }
                return cd;
            } 
            set
            { 
                AbilityCost[(int)DKCostTypes.CooldownTime] = (int)value; 
            }
        }

        /// <summary>
        /// Does this ability trigger the GCD?
        /// </summary>
        public bool bTriggersGCD { get; set; }
        /// <summary>
        /// Is this Ability a Partial to average out values or left over resources?
        /// </summary>
        public bool bPartial { get; set; }
        private float _fPartialValue = 1f;
        public float fPartialValue 
        { 
            get
            {
                if (bPartial) return _fPartialValue;
                else return 1;
            }
            set
            {
                if (value == 1)
                {
                    bPartial = false;
                }
                else
                {
                    bPartial = true;
                }
                _fPartialValue = value;
            } 
        }
        /// <summary>
        /// How long does the effect last?
        /// This is in millisecs.
        /// </summary>
        public uint uDuration
        {
            get
            {
                // Factor in haste:
                uint tr = Math.Max(1, (uint)AbilityCost[(int)DKCostTypes.DurationTime]);
                return Math.Max(INSTANT, tr);
            }
            set
            {
                AbilityCost[(int)DKCostTypes.DurationTime] = (int)Math.Max(INSTANT, value);
            }
        }

        private uint _uTickRate;
        /// <summary>
        /// How often does the effect proc for?
        /// Tick rate is millisecs.
        /// Ensure that we don't have a 0 value.  
        /// 1 ms == instant.
        /// </summary>
        public uint uTickRate 
        {
            get
            {
                // Factor in haste:
                uint tr = _uTickRate;
                if (this.bWeaponRequired)
                {
                    if (CState != null && CState.m_Stats != null)
                        tr = (uint)(tr / (1 + CState.m_Stats.PhysicalHaste));
                }
                else
                {
                    if (CState != null && CState.m_Stats != null)
                        tr = (uint)(tr / (1 + CState.m_Stats.SpellHaste));
                }
                return Math.Max(INSTANT, tr);
            }
            set
            {
                _uTickRate = Math.Max(INSTANT, value);
            }
        }
        #endregion 

        #region Weapon related Items
        /////////////////////////////////////////////////
        // Weapon related items.
        public bool bWeaponRequired { get; set; }
        public Weapon wMH, wOH;
        public float fWeaponDamageModifier { get; set; }
        #endregion 

        /// <summary>
        /// The Crit Chance for the ability.  
        /// </summary>
        [Percentage]
        virtual public float CritChance 
        { 
            get
            {
                if (this.bWeaponRequired)
                {
                    // Crit is modified by Hit, not the other way around.
                    float crit = .0065f;
                    if (CState != null && CState.m_Stats != null)
                        crit += CState.m_Stats.PhysicalCrit;
                    crit += StatConversion.NPC_LEVEL_CRIT_MOD[3];
                    crit *= HitChance;
                    return Math.Max(0, Math.Min(1, crit));
                }
                else
                    if (CState != null && CState.m_Stats != null)
                        return Math.Min(1, .0065f + CState.m_Stats.SpellCrit + StatConversion.NPC_LEVEL_CRIT_MOD[3]);
                return .0065f + StatConversion.NPC_LEVEL_CRIT_MOD[3];
            }
        }

        /// <summary>
        /// Chance for the ability to hit the target.  
        /// Includes Expertise
        /// </summary>
        [Percentage]
        virtual public float HitChance
        {
            get
            {
                if (this.bWeaponRequired)
                {
                    // Determine Miss Chance
                    float ChanceToHit = 1 - MissChance; // Ensure that crit is no lower than 0.
                    // Determine Dodge chance
                    float fDodgeChanceForTarget = StatConversion.WHITE_DODGE_CHANCE_CAP[3];
                    if (wMH != null) fDodgeChanceForTarget = Math.Max(Math.Min(wMH.chanceDodged, fDodgeChanceForTarget), 0);
                    // Determine Parry Chance  (Only for Tank... Since only they should be in front of the target.)
                    float fParryChanceForTarget = StatConversion.WHITE_PARRY_CHANCE_CAP[3];
                    if (wMH != null) fParryChanceForTarget = Math.Max(Math.Min(wMH.chanceParried, fParryChanceForTarget), 0);
                    ChanceToHit -= fDodgeChanceForTarget;
                    if (CState != null && !CState.m_bAttackingFromBehind)
                        ChanceToHit -= fParryChanceForTarget;
#if DEBUG
                    if (ChanceToHit < 0 || ChanceToHit > 1
                        || fDodgeChanceForTarget < 0
                        || fParryChanceForTarget < 0)
                        throw new Exception("Chance to hit out of range.");
#endif
                    return Math.Max(0, Math.Min(1, ChanceToHit));
                }
                else
                    return 1 - MissChance;
            }
        }

        /// <summary>
        /// Chance for the ability to Miss the target.  
        /// Hit only
        /// </summary>
        [Percentage]
        virtual public float MissChance
        {
            get
            {
                float fMissChance;
                if (this.bWeaponRequired)
                {
                    // Determine Miss Chance
                    fMissChance = Math.Max(0, (StatConversion.YELLOW_MISS_CHANCE_CAP[3] - CState.m_Stats.PhysicalHit));
                }
                else if (CState != null && CState.m_Stats != null)
                    fMissChance = Math.Max(0, (StatConversion.GetSpellMiss(3, false) - CState.m_Stats.SpellHit));
                else if (this.uRange == 0)
                    fMissChance = 0;
                else
                    fMissChance = StatConversion.GetSpellMiss(3, false);
                return fMissChance;
            }
        }

        #region Damage
        /// <summary>
        /// Get the single instance damage of this ability.
        /// </summary>
        /// <returns>Float that represents a fully buffed single instance of this ability.</returns>
        virtual public float GetTickDamage()
        {
            if (_tickDamage == -1f)
            {
                // Start w/ getting the base damage values.
                _tickDamage = (int)this.uBaseDamage;
                // Apply modifiers.
                _tickDamage += this.DamageAdditiveModifer;
                _tickDamage *= (1f + this.DamageMultiplierModifer);
                if (bPartial) { _tickDamage *= fPartialValue; }
            }
            return _tickDamage;
        }
        private float _tickDamage = -1f;

        virtual public float TotalDamage { get { return GetTotalDamage(); } }
        /// <summary>
        /// Get the full effect over the lifetime of the ability.
        /// </summary>
        /// <returns>int that is TickDamage * duration</returns>
        virtual public float GetTotalDamage()
        {
            if (this.bWeaponRequired == true && (null == this.wMH && null == this.wOH))
            {
                return 0;
            }
            // Start w/ getting the base damage values.
            float fDamage = this.GetTickDamage();
            // Assuming full duration, or standard impact.
            // But I want this in whole numbers.
            // Also need to decide if I want this to be whole ticks, or if partial ticks will be allowed.
            float fDamageCount = (float)(this.uDuration / Math.Max(1, this.uTickRate));
            if (bAOE == true)
            {
                // Need to ensure this value is reasonable for all abilities.
                fDamageCount *= Math.Max(1, this.CState.m_NumberOfTargets);
            }

            fDamage *= fDamageCount;
            float fCritDamage = 2 * fDamage * CritChance;
            fDamage = (fDamage * (Math.Min(1, HitChance) - CritChance)) + fCritDamage;

            if (bWeaponRequired && wMH.twohander && CState != null)
                fDamage = (fDamage * (1 + .04f * CState.m_Talents.MightOfTheFrozenWastes));
            return fDamage;
        }

        public float DPS { get { return GetDPS(); } }
        virtual public float GetDPS()
        {
            uint sub = MIN_GCD_MS;
            if (CState != null && bTriggersGCD)
            {
                sub = (CState.m_Presence == Presence.Unholy) ? MIN_GCD_MS_UH : MIN_GCD_MS;
            }
            sub = Math.Max(sub, (uDuration + CastTime));
            float dps = (float)(TotalDamage * 1000) / sub ;
            return dps;
        }

        public float TPS { get { return GetTPS(); } }
        virtual public float GetTPS()
        {
            uint sub = MIN_GCD_MS;
            if (CState != null && bTriggersGCD)
            {
                sub = (CState.m_Presence == Presence.Unholy) ? MIN_GCD_MS_UH : MIN_GCD_MS;
            }
            sub = Math.Max(sub, (uDuration + CastTime));
            float dps = (float)(TotalThreat * 1000) / sub;
            return dps;
        }

        private int _DamageAdditiveModifer;
        /// <summary>
        /// Setup the modifier formula for a given ability.
        /// </summary>
        virtual public int DamageAdditiveModifer
        {
            get 
            {
                return _DamageAdditiveModifer;
            }
            set 
            {
                _DamageAdditiveModifer = value;
            }
        }

        private float _DamageMultiplierModifer;
        /// <summary>
        /// 0 indexed multiplier.
        /// Setup the modifier formula for a given ability.
        /// </summary>
        virtual public float DamageMultiplierModifer
        {
            get
            {
                float DMM = 0;
                if (CState != null && CState.m_Stats != null)
                {
                    switch (tDamageType)
                    {
                        case ItemDamageType.Arcane:
                            DMM += CState.m_Stats.BonusArcaneDamageMultiplier;
                            break;
                        case ItemDamageType.Fire:
                            DMM += CState.m_Stats.BonusFireDamageMultiplier;
                            break;
                        case ItemDamageType.Frost:
                            DMM += CState.m_Stats.BonusFrostDamageMultiplier + CState.m_Stats.BonusFrostDamageMultiplierFromMastery;
                            break;
                        case ItemDamageType.Holy:
                            DMM += CState.m_Stats.BonusHolyDamageMultiplier;
                            break;
                        case ItemDamageType.Nature:
                            DMM += CState.m_Stats.BonusNatureDamageMultiplier;
                            break;
                        case ItemDamageType.Physical:
                            DMM += CState.m_Stats.BonusPhysicalDamageMultiplier;
                            DMM -= Math.Max(0f, StatConversion.GetArmorDamageReduction(85, CState.fBossArmor,0,0));
                            break;
                        case ItemDamageType.Shadow:
                            DMM += CState.m_Stats.BonusShadowDamageMultiplier + CState.m_Stats.BonusShadowDamageMultiplierFromMastery;
                            break;
                    }
                }
                return _DamageMultiplierModifer + DMM;
            }
            set
            {
                _DamageMultiplierModifer = value;
            }
        }
        #endregion

        #region Threat
        /// <summary>How much to multiply the damage by to generate threat.</summary>
        // TODO: Update for DRW uptime.
        // JOTHAY NOTE: You are handling the Dancing Rune Threat increase in with the DRW SpecialEffect, see _SE_DRW
        public float ThreatMultiplier { get { return 0f /*+ (CState.m_Talents.GlyphofDancingRuneWeapon ? 0.50f : 0f)*/; } }

        private float _ThreatAdditiveModifier;
        /// <summary>
        /// Get the full effect of threat over the lifetime of the ability.
        /// </summary>
        /// <returns>float that is (GetTotalDamage * ThreatModifiers) * Threat For Frost Presence</returns>
        public float GetTotalThreat() { return TotalThreat; } 
        public float TotalThreat {
            get {
                float Threat = StatConversion.ApplyMultiplier(GetTotalDamage(), ThreatMultiplier) + _ThreatAdditiveModifier;
                if (null != CState && null != CState.m_Stats)
                {
                    Threat = StatConversion.ApplyMultiplier(Threat, CState.m_Stats.ThreatIncreaseMultiplier);
                    Threat = StatConversion.ApplyInverseMultiplier(Threat, CState.m_Stats.ThreatReductionMultiplier);
                }
                return Threat;
            }
            set { _ThreatAdditiveModifier = value; }
        }
        #endregion

        #region Comparisons
        /// <summary>
        /// Compare the output of an ability either Threat or Damage.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t">Using Death Runes will be total Rune Cost.</param>
        /// <param name="bThreat">True if Threat, False if Damage</param>
        /// <returns></returns>
        public static int CompareXPerCost(AbilityDK_Base a, AbilityDK_Base b, DKCostTypes t, bool bThreat)
        {
            int ic = 0;
            float aRunes = 1;
            float bRunes = 1;

            // Sum of cost:
            switch (t)
            {
                case DKCostTypes.Blood:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Blood];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Blood];
                    break;
                case DKCostTypes.Frost:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Frost];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Frost];
                    break;
                case DKCostTypes.UnHoly:
                    aRunes = a.AbilityCost[(int)DKCostTypes.UnHoly];
                    bRunes = b.AbilityCost[(int)DKCostTypes.UnHoly];
                    break;
                case DKCostTypes.Death:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Blood] + a.AbilityCost[(int)DKCostTypes.Frost] + a.AbilityCost[(int)DKCostTypes.UnHoly];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Blood] + b.AbilityCost[(int)DKCostTypes.Frost] + b.AbilityCost[(int)DKCostTypes.UnHoly];
                    break;
                case DKCostTypes.CastTime:
                    aRunes = a.CastTime;
                    bRunes = b.CastTime;
                    break;
                case DKCostTypes.CooldownTime:
                    aRunes = a.Cooldown;
                    bRunes = b.Cooldown;
                    break;
                case DKCostTypes.DurationTime:
                    aRunes = a.uDuration;
                    foreach (AbilityDK_Base TriggerByA in a.ml_TriggeredAbility)
                    {
                        aRunes += TriggerByA.uDuration;
                    }
                    bRunes = b.uDuration;
                    foreach (AbilityDK_Base TriggerByB in b.ml_TriggeredAbility)
                    {
                        bRunes += TriggerByB.uDuration;
                    }
                    break;
                case DKCostTypes.RunicPower:
                    aRunes = a.RunicPower;
                    bRunes = b.RunicPower;
                    break;
                default:
                    aRunes = 1;
                    bRunes = 1;
                    break;

            }
            if (aRunes != 0 || bRunes != 0)
            {
                if (aRunes != 0 && bRunes != 0)
                {
                    float avalue = 0;
                    float bvalue = 0;
                    if (bThreat)
                    {
                        // Let's expand this to include triggered values as well.
                        avalue = a.GetTotalThreat();
                        if (a.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in a.ml_TriggeredAbility)
                            {
                                avalue += TriggerByA.GetTotalThreat();
                            }
                        }
                        avalue /= aRunes;
                        bvalue = b.GetTotalThreat();
                        if (b.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in b.ml_TriggeredAbility)
                            {
                                bvalue += TriggerByA.GetTotalThreat();
                            }
                        }
                        bvalue /= bRunes;
                    }
                    else
                    {
                        // Let's expand this to include triggered values as well.
                        avalue = a.TotalDamage;
                        if (a.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in a.ml_TriggeredAbility)
                            {
                                avalue += TriggerByA.TotalDamage;
                            }
                        }
                        avalue /= aRunes;
                        bvalue = b.TotalDamage;
                        if (b.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in b.ml_TriggeredAbility)
                            {
                                bvalue += TriggerByA.TotalDamage;
                            }
                        }
                        bvalue /= bRunes;
                    }
                    if (avalue != bvalue)
                    {
                        // This is setup where we want a descending order.
                        if (avalue > bvalue)
                            ic = -1;
                        else
                            ic = 1;
                    }
                }
                else // one of them are 0
                {
                    if (aRunes > bRunes )
                        ic = -1;
                    else
                        ic = 1;
                }
            }
            return ic;
        }
        public static int CompareValuePSPerRune(AbilityDK_Base a, AbilityDK_Base b, DKCostTypes t, bool bThreat)
        {
            int ic = 0;
            float aRunes = 1;
            float bRunes = 1;

            // Sum of cost:
            switch (t)
            {
                case DKCostTypes.Blood:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Blood];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Blood];
                    break;
                case DKCostTypes.Frost:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Frost];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Frost];
                    break;
                case DKCostTypes.UnHoly:
                    aRunes = a.AbilityCost[(int)DKCostTypes.UnHoly];
                    bRunes = b.AbilityCost[(int)DKCostTypes.UnHoly];
                    break;
                case DKCostTypes.Death:
                    aRunes = a.AbilityCost[(int)DKCostTypes.Blood] + a.AbilityCost[(int)DKCostTypes.Frost] + a.AbilityCost[(int)DKCostTypes.UnHoly];
                    bRunes = b.AbilityCost[(int)DKCostTypes.Blood] + b.AbilityCost[(int)DKCostTypes.Frost] + b.AbilityCost[(int)DKCostTypes.UnHoly];
                    break;
                case DKCostTypes.RunicPower:
                    aRunes = a.RunicPower;
                    bRunes = b.RunicPower;
                    break;
                case DKCostTypes.CastTime:
                case DKCostTypes.CooldownTime:
                case DKCostTypes.DurationTime:
                default:
                    aRunes = 1;
                    bRunes = 1;
                    break;

            }
            if (aRunes != 0 || bRunes != 0)
            {
                if (aRunes != 0 && bRunes != 0)
                {
                    float avalue = 0;
                    float bvalue = 0;
                    if (bThreat)
                    {
                        // Let's expand this to include triggered values as well.
                        avalue = a.GetTPS();
                        if (a.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in a.ml_TriggeredAbility)
                            {
                                avalue += TriggerByA.GetTPS();
                            }
                        }
                        avalue /= aRunes;
                        bvalue = b.GetTPS();
                        if (b.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in b.ml_TriggeredAbility)
                            {
                                bvalue += TriggerByA.GetTPS();
                            }
                        }
                        bvalue /= bRunes;
                    }
                    else
                    {
                        // Let's expand this to include triggered values as well.
                        avalue = a.GetDPS();
                        if (a.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in a.ml_TriggeredAbility)
                            {
                                avalue += TriggerByA.GetDPS();
                            }
                        }
                        avalue /= aRunes;
                        bvalue = b.GetDPS();
                        if (b.ml_TriggeredAbility != null)
                        {
                            foreach (AbilityDK_Base TriggerByA in b.ml_TriggeredAbility)
                            {
                                bvalue += TriggerByA.GetDPS();
                            }
                        }
                        bvalue /= bRunes;
                    }
                    if (avalue != bvalue)
                    {
                        // This is setup where we want a descending order.
                        if (avalue > bvalue)
                            ic = -1;
                        else
                            ic = 1;
                    }
                }
                else // one of them are 0
                {
                    if (aRunes > bRunes)
                        ic = -1;
                    else
                        ic = 1;
                }
            }
            return ic;
        }
        #region Damage
        public static int CompareByTotalDamage(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.None, false);            
        }

        public static int CompareDamageByCooldown(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.CooldownTime, false);
        }

        public static int CompareDamageByRunes(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.Death, false);
        }

        public static int CompareDPSByRunes(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareValuePSPerRune(a, b, DKCostTypes.Death, false);
        }

        public static int CompareDamageByRP(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.RunicPower, false);
        }

        
        #endregion
        #region Threat
        public static int CompareByTotalThreat(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.None, true);
        }

        public static int CompareThreatByCooldown(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.CooldownTime, true);
        }

        public static int CompareThreatByRunes(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.Death, true);
        }
        public static int CompareTPSByRunes(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareValuePSPerRune(a, b, DKCostTypes.Death, true);
        }
        public static int CompareThreatByRP(AbilityDK_Base a, AbilityDK_Base b)
        {
            return CompareXPerCost(a, b, DKCostTypes.RunicPower, true);
        }
        #endregion
        #endregion

        public override string ToString()
        {
            return string.Format("{0}: {1} {2}", szName, TotalDamage, tDamageType.ToString());
        }
    }
}
