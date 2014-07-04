﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Hunter.Skills
{
    public class ChimeraShot : Ability
    {
        /// <summary>
        /// TODO Zhok: Add Efficiency, Piercing Shots
        /// <b>Chimera Shot</b>, 50 Focus, 5-40yd, Instant, 10 sec Cd
        /// <para>An instant shot that causes ranged weapon damage 
        /// plus RAP*0.732+1620, refreshing the duration of your 
        /// Serpent Sting and healing you for 5% of your total health.</para>
        /// </summary>
        /// <TalentsAffecting>Chimera Shot (Requires Talent)
        /// Concussive Barrage - Your successful Chimera Shot and Multi-Shot attacks have a 50/100% chance to daze the target for 4 sec.
        /// Efficiency - Reduces the focus cost of your Arcane Shot by 1/2/3, and your Explosive Shot and Chimera Shot by 2/4/6.
        /// Marked for Death - Your Arcane Shot and Chimera Shot have a 50/100% chance to automatically apply the Marked for Death effect.
        /// Piercing Shots - Your critical Aimed, Steady and Chimera Shots cause the target to bleed for 10/20/30% of the damage dealt over 8 sec.
        /// </TalentsAffecting>
        /// <GlyphsAffecting>Glyph of Chimera Shot [-1 sec Cd]</GlyphsAffecting>
        public ChimeraShot(Character c, StatsHunter s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;

            Name = "Chimera Shot";
            shortName = "Chim";

            ReqTalent = true;
            
            Talent2ChksValue = (c.HunterTalents.Specialization == (int)Specialization.Marksmanship ? 1 : 0);
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            DamageType = ItemDamageType.Nature;
            //Targets += StatS.BonusTargets;
            Cd = 9f;
            FocusCost = 45f;
            MaxRange = 40f;
            FocusCost = 45f;
            DamageBase = combatFactors.NormalizedRwWeaponDmg * 2.1f + 2617f;
            RefreshesSS = true;

            //TODO: Heals you for 3% of total health

            eShot = Shots.ChimeraShot;


            Initialize();
        }

        public float piercingShots_TickSize()
        {
            float damage = Damage * Talents.PiercingShots * 0.10f;
            float NumTicks = 8f;
            return damage * DamageBonus * (1f + StatS.BonusDamageMultiplier) * (1f + StatS.BonusNatureDamageMultiplier) / NumTicks;
        }

        public float piercingShots_GetDPS(float acts)
        {
            float dmgonuse = piercingShots_TickSize();
            float numticks = 8f * acts;
            float result = (dmgonuse * numticks) / FightDuration;
            return result;
        }
    }
}
