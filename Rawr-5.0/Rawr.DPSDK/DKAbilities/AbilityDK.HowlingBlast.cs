﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.DK
{
    /// <summary>
    /// This class is the implmentation of the Howling Blast Ability based on the AbilityDK_Base class.
    /// </summary>
    class AbilityDK_HowlingBlast : AbilityDK_Base
    {
        public AbilityDK_HowlingBlast(CombatState CS)
        {
            this.CState = CS;
            this.szName = "Howling Blast";
            this.AbilityCost[(int)DKCostTypes.Frost] = 1;
            this.uMinDamage = (uint)(1152 / 2 * 1.2); // 4.1 increases damage by 20%
            this.uMaxDamage = (uint)(1250 / 2 * 1.2); // 4.1 increases damage by 20%
            this.tDamageType = ItemDamageType.Frost;
            this.bWeaponRequired = false;
            this.fWeaponDamageModifier = 0;
            this.bTriggersGCD = true;
            this.uRange = 20;
            this.uArea = 10;
            this.bAOE = true;
            this.AbilityIndex = (int)DKability.HowlingBlast;
            UpdateCombatState(CS);
        }

        public override void UpdateCombatState(CombatState CS)
        {
            base.UpdateCombatState(CS);
            if (CS.m_Talents.GlyphofHowlingBlast && CS.m_uDiseaseCount < 2)
            {
                this.ml_TriggeredAbility = new AbilityDK_Base[1];
                this.ml_TriggeredAbility[0] = new AbilityDK_FrostFever(CS);
            }
            this.AbilityCost[(int)DKCostTypes.RunicPower] = -10;
        }

        private int _DamageAdditiveModifer = 0;
        /// <summary>Setup the modifier formula for a given ability</summary>
        public override int DamageAdditiveModifer { get { return (int)(this.CState.m_Stats.AttackPower * 0.48f); } set { _DamageAdditiveModifer = value; } }

        public override float GetTotalDamage()
        {
            if (CState.m_Spec == Rotation.Type.Frost)
            {
                // Start w/ getting the base damage values.
                float iDamage = GetTickDamage();
                // Assuming full duration, or standard impact.
                // But I want this in whole numbers.
                // Also need to decide if I want this to be whole ticks, or if partial ticks will be allowed.
                float fDamageCount = (float)(uDuration / Math.Max(1, uTickRate));

                iDamage *= fDamageCount * (1f + CritChance) * Math.Min(1f, HitChance);
                if (bAOE == true) 
                {
                    // Need to ensure this value is reasonable for all abilities.
                    iDamage += (iDamage * 0.50f) * (Math.Max(1f, this.CState.m_NumberOfTargets) - 1); // damage to secondary targets @ 50% (patch 4.1)
                }
                return iDamage;
            }
            else
                return 0;
        }
    }
}
