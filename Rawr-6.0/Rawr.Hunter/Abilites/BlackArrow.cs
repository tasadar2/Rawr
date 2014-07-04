﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Hunter.Skills
{
    public class BlackArrow : DoT
    {
        private float _basefocuscost = 35f;
        static public int Cooldown = 30;

        /// <summary>
        /// TODO Zhok: Thrill of the Hunt, Toxicology, Trap Mastery
        /// <b>Black Arrow</b>, 35 Focus, 5-40yd, Instant, 30 sec Cd
        /// <para>Fires a Black Arrow at the target, dealing 2035 Shadow damage over 15 sec. 
        /// Black Arrow shares a cooldown with other Fire Trap spells.</para>
        /// </summary>
        /// <TalentsAffecting>Black Arrow (Requires Talent)
        /// Resourcefulness - Reduces the cooldown of all traps and Black Arrow by 2/4/6 sec.
        /// T.N.T - When you deal periodic damage with your Immolation Trap, Explosive Trap or Black Arrow you have a 6/12% chance to trigger Lock and Load.
        /// Thrill of the Hunt - You have a 5/10/15% chance when you use Arcane Shot, Explosive Shot or Black Arrow to instantly regain 40% of the base focus cost of the shot.
        /// Toxicology - Increases the periodic critical damage of your Serpent Sting and Black Arrow by 50/100%.
        /// Trap Mastery - Immolation Trap, Explosive Trap and Black Arrow - Increases the periodic damage done by 10/20/30%.
        /// </TalentsAffecting>
        /// <GlyphsAffecting></GlyphsAffecting>
        public BlackArrow(Character c, StatsHunter s, CombatFactors cf, WhiteAttacks wa, CalculationOptionsHunter co)
        {
            Char = c; StatS = s; combatFactors = cf; Whiteattacks = wa; CalcOpts = co;

            Name = "Black Arrow";
            shortName = "BA";

            SpellId = 3674;
            ReqTalent = true;

            Talent2ChksValue = (c.HunterTalents.Specialization == (int)Specialization.Survival );
            
            ReqRangedWeap = true;
            ReqSkillsRange = true;
            DamageType = ItemDamageType.Shadow;

            if (c.HunterTalents.Specialization == (int)Specialization.Survival)
                Cd = Cooldown * .8f;
            else
                Cd =  Cooldown;
            
            Duration = 20f;
            TimeBtwnTicks = 2f; // TODO Zhok: Haste?
            FocusCost = _basefocuscost;
            // 47.35% RAP + 2035 (total damage)
            // 4.2 Increased the damage by 40%
            DamageBase = (s.RangedAttackPower*1.5f) + 1870f;
            DamageBonus = 1f;
            
            eShot = Shots.BlackArrow;

            Initialize();
        }
        public override float TickSize
        {
            get
            {
                if (!Validated) { return 0f; }
                return DamageBase * DamageBonus * (1f + StatS.BonusDamageMultiplier) * (1f + StatS.BonusShadowDamageMultiplier) / NumTicks;
            }
        }
        public override float GetDPS(float acts)
        {
            float dmgonuse = TickSize;
            float numticks = NumTicks * (acts /*- addMisses - addDodges - addParrys*/);
            float result = GetDmgOverTickingTime(acts) / FightDuration;
            return result;
        }
        public float GainedThrilloftheHuntFocus() { return _basefocuscost * 0.40f; }
    }
}
