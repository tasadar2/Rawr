﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Feral
{
    /// <summary>
    /// This Class has gone off the rails.
    /// </summary>
    public class FeralCombatTable
    {
        #region Member Vars
        #region Basic setup variables
//        public CombatState m_CState;
        public CharacterCalculationsFeral m_Calcs;
        public CalculationOptionsFeral m_Opts;
        public BossOptions m_BO;
//        public float physCrits { get { return m_CState.m_Stats.PhysicalCrit; } }
//        public float spellCrits { get { return m_CState.m_Stats.SpellCrit; } }
        /// <summary>
        /// Swing time in Seconds.
        /// </summary>
/*        public float combinedSwingTime
        {
            get
            {
                if (DW)
                    return 60 / ((60 / MH.hastedSpeed) + (60 / OH.hastedSpeed));
                else
                {
                    if (MH != null)
                        return MH.hastedSpeed;
                    else
                        // Just throw in some slow 2h weapon time as the default if .
                        return 3.8f;
                }
            }
        }
 */
//        public Weapon MH { get { return m_CState.MH; } }
//        public Weapon OH { get { return m_CState.OH; } }
        public float missedSpecial { get; set; }
        public float dodgedSpecial { get; set; }
        public float parriedSpecial { get; set; }
        public float totalMHMiss { get; set; }
        public bool DW { get; set; }
        #endregion
        #endregion

        public FeralCombatTable(Character c, StatsFeral stats, CharacterCalculationsBase calcs, ICalculationOptionBase calcOpts, BossOptions bossOpts)
        {
            /*m_CState = new CombatState();
            if (c != null)
            {
                m_CState.m_Char = c;
                m_CState.m_Talents = c.DeathKnightTalents;
                m_CState.m_Spec = CalculationsDPSDK.GetSpec(c.DeathKnightTalents);
            }
            m_CState.m_Stats = stats;
            // TODO: Put in check here for null.
            m_Calcs = calcs as CharacterCalculationsDPSDK;
            m_Opts = calcOpts as CalculationOptionsDPSDK;
            m_CState.m_Presence = Presence.Frost;
            if (calcOpts != null && m_Opts == null)
            {
                //throw new Exception("Opts not converted properly.");
                m_Opts = new CalculationOptionsDPSDK();
            }
            try { m_CState.m_Presence = m_Opts.presence; }
            catch { } // pass  stay w/ default
            m_BO = bossOpts;
            if (m_BO == null) { m_BO = new BossOptions(); }
            // JOTHAY TODO: Kind of an Ugly Hack to do this, but it will give them a value
            m_CState.m_NumberOfTargets = m_BO.MultiTargs ? m_BO.DynamicCompiler_MultiTargs.GetAverageTargetGroupSize(m_BO.BerserkTimer) : 1f;
            //
            m_CState.m_bAttackingFromBehind = m_BO.InBack;
            m_CState.fBossArmor = m_BO.Armor;
            SetupExpertise(c);
             */
        }

        private void SetupExpertise(Character c)
        {
            /*float MHExpertise = m_CState.m_Stats.Expertise;
            float OHExpertise = m_CState.m_Stats.Expertise;

            if (c.Race == CharacterRace.Dwarf)
            {
                if (c.MainHand != null &&
                    (c.MainHand.Item.Type == ItemType.OneHandMace ||
                     c.MainHand.Item.Type == ItemType.TwoHandMace))
                {
                    MHExpertise += 5f;
                }

                if (c.OffHand != null && c.OffHand.Item.Type == ItemType.OneHandMace)
                {
                    OHExpertise += 5f;
                }
            }
            else if (c.Race == CharacterRace.Orc)
            {
                if (c.MainHand != null &&
                    (c.MainHand.Item.Type == ItemType.OneHandAxe ||
                     c.MainHand.Item.Type == ItemType.TwoHandAxe))
                {
                    MHExpertise += 3f;
                }

                if (c.OffHand != null && c.OffHand.Item.Type == ItemType.OneHandAxe)
                {
                    OHExpertise += 3f;
                }
            }
            if (c.Race == CharacterRace.Human)
            {
                if (c.MainHand != null &&
                    (c.MainHand.Item.Type == ItemType.OneHandSword ||
                     c.MainHand.Item.Type == ItemType.TwoHandSword ||
                     c.MainHand.Item.Type == ItemType.OneHandMace ||
                     c.MainHand.Item.Type == ItemType.TwoHandMace))
                {
                    MHExpertise += 3f;
                }

                if (c.OffHand != null &&
                    (c.OffHand.Item.Type == ItemType.OneHandSword ||
                    c.OffHand.Item.Type == ItemType.OneHandMace))
                {
                    OHExpertise += 3f;
                }
            }

            if (c.MainHand != null && c.MainHand.Item.Type != ItemType.None)
            {
                m_CState.MH = new Weapon(c.MainHand.Item, m_CState.m_Stats, m_Opts, m_BO, m_CState.m_Talents, MHExpertise, CharacterSlot.MainHand);
                m_CState.OH = null;
                m_Calcs.MHExpertise = m_CState.MH.effectiveExpertise;
                if (c.MainHand.Slot != ItemSlot.TwoHand)
                {
                    if (c.OffHand != null && c.OffHand.Item.Type != ItemType.None)
                    {
                        DW = true;
                        m_CState.OH = new Weapon(c.OffHand.Item, m_CState.m_Stats, m_Opts, m_BO, m_CState.m_Talents, OHExpertise, CharacterSlot.OffHand);
                        m_Calcs.OHExpertise = m_CState.OH.effectiveExpertise;
                    }
                }
            }
            else
            {
                m_CState.MH = null;
                m_CState.OH = null;
            }
             */
        }

        /// <summary>
        /// Generate a rotation based on available resources.
        /// </summary>
        public void PostAbilitiesSingleUse(bool bThreat)
        {
            /*
            // Setup an instance of each ability.
            // No runes:
            AbilityDK_Outbreak OutB = new AbilityDK_Outbreak(m_CState);
            // Single Runes:
            AbilityDK_IcyTouch IT = new AbilityDK_IcyTouch(m_CState);
            PostData(IT);
            AbilityDK_FrostFever FF = new AbilityDK_FrostFever(m_CState);
            PostData(FF);
            AbilityDK_PlagueStrike PS = new AbilityDK_PlagueStrike(m_CState);
            PostData(PS);
            AbilityDK_BloodPlague BP = new AbilityDK_BloodPlague(m_CState);
            PostData(BP);
            AbilityDK_BloodStrike BS = new AbilityDK_BloodStrike(m_CState);
            PostData(BS);
            AbilityDK_HeartStrike HS = new AbilityDK_HeartStrike(m_CState);
            PostData(HS);
            AbilityDK_NecroticStrike NS = new AbilityDK_NecroticStrike(m_CState);
            PostData(NS);
            //            AbilityDK_Pestilence Pest = new AbilityDK_Pestilence(m_CState);
            AbilityDK_BloodBoil BB = new AbilityDK_BloodBoil(m_CState);
            PostData(BB);
            AbilityDK_HowlingBlast HB = new AbilityDK_HowlingBlast(m_CState);
            PostData(HB);
            AbilityDK_ScourgeStrike SS = new AbilityDK_ScourgeStrike(m_CState);
            PostData(SS);
            AbilityDK_DeathNDecay DnD = new AbilityDK_DeathNDecay(m_CState);
            PostData(DnD);
            // Multi Runes:
            AbilityDK_DeathStrike DS = new AbilityDK_DeathStrike(m_CState);
            PostData(DS);
            AbilityDK_FesteringStrike Fest = new AbilityDK_FesteringStrike(m_CState);
            PostData(Fest);
            AbilityDK_Obliterate OB = new AbilityDK_Obliterate(m_CState);
            PostData(OB);
            // RP:
            AbilityDK_RuneStrike RS = new AbilityDK_RuneStrike(m_CState);
            PostData(RS);
            AbilityDK_DeathCoil DC = new AbilityDK_DeathCoil(m_CState);
            PostData(DC);
            AbilityDK_FrostStrike FS = new AbilityDK_FrostStrike(m_CState);
            PostData(FS);
             */
        }

        /*
        /// <summary>
        /// Pass in the AblityCost[] to process the DeathRunes
        /// </summary>
        /// <param name="AbilityCost"></param>
        public static int SpendDeathRunes(int[] AbilityCost, int DRSpent)
        {
            
            // Need to figure out how to factor in Death Runes
            // Since each death rune replaces any other rune on the rotation,
            // for each death rune, cut the cost of the highest other rune by 1.
            // Do not run this if there are no DeathRunes to spend.
            // But does not replace rune CD.
            if ((AbilityCost[(int)DKCostTypes.Death] * -1) > DRSpent)
            {
                int iHighestCostAbilityIndex = GetHighestRuneCountIndex(AbilityCost);
                // After going through the full list, spend a death rune and 
                // then iterate through that list again. 
                AbilityCost[iHighestCostAbilityIndex]--;
                // increment the death runes.
                DRSpent++;
                DRSpent = SpendDeathRunes(AbilityCost, DRSpent);
            }
            return DRSpent;
        }

        public static int GetHighestRuneCountIndex(int[] AbilityCost)
        {
            int i = (int)DKCostTypes.None;
            int iPreviousCostValue = 0;
            for (int t = 0; t < (int)DKCostTypes.Death; t++)
            {
                if (t == (int)DKCostTypes.RunicPower)
                    break;
                // Is the cost higher than our previous checked value.
                if (AbilityCost[t] > iPreviousCostValue)
                {
                    // If so, save off the index of that ability.
                    i = t;
                }
                iPreviousCostValue = AbilityCost[t];
            }
            return i;
        }

        private void PostData(AbilityDK_Base ability)
        {
            int i = ability.AbilityIndex;
            m_Calcs.DPUdamSub[i] = ability.GetTotalDamage();
            m_Calcs.DPUthreatSub[i] = ability.GetTotalThreat();
            m_Calcs.DPUdpsSub[i] = ability.GetDPS();
            m_Calcs.DPUtpsSub[i] = ability.GetTPS();
        }

         */
    }
}
