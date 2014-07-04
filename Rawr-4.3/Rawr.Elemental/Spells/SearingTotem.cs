﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Elemental.Spells
{
    public class SearingTotem : Totem
    {
        public SearingTotem(ISpellArgs args)
            : base()
        {
        }

        protected override void SetBaseValues()
        {
            
            base.SetBaseValues();

            periodicTick = (81 + 110)/2f; // 81-110 range
            dotSpCoef = 1f / 6f;
            periodicTicks = 27f;
            periodicTickTime = 2.2f;
            manaCost = .07f * Constants.BaseMana;
            shortName = "ST";
            dotCanCrit = 1;
        }

        public override void Initialize(ISpellArgs args)
        {
            totalCoef *= 1 + (.1f * args.Talents.CallOfFlame);

            manaCost -= args.Stats.NatureSpellsManaCostReduction;

            periodicTicks *= 1 + (.2f * args.Talents.TotemicFocus);

            base.Initialize(args);
        }

        public static SearingTotem operator +(SearingTotem A, SearingTotem B)
        {
            SearingTotem C = (SearingTotem)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static SearingTotem operator *(SearingTotem A, float b)
        {
            SearingTotem C = (SearingTotem)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }
    }
}
