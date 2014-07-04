﻿using System;

namespace Rawr.ShadowPriest.Spells
{
    public class MindSpike : DD
    {
        public MindSpike()
            : base()
        {
        }
        protected override void SetDDValues()
        {
            base.SetDDValues();

            baseSpread = 0.0549999997019768f;
        }
        protected override void SetBaseValues()
        {
            base.SetBaseValues();

            baseScaling = 1.17799997329712f;
            manaCost = 0.12f * Constants.BaseMana;
            shortName = "Spike";
            name = "Mind Spike";
        }

        public override float SpellPowerCoef
        {
            get
            {
                return 0.556999981403351f;
            }
        }

        public override void Initialize(ISpellArgs args)
        {
            base.Initialize(args);
        }
                #region hide
        public MindSpike(ISpellArgs args)
            : this()
        {
            Initialize(args);
        }

        public static MindSpike operator +(MindSpike A, MindSpike B)
        {
            MindSpike C = (MindSpike)A.MemberwiseClone();
            add(A, B, C);
            return C;
        }

        public static MindSpike operator *(MindSpike A, float b)
        {
            MindSpike C = (MindSpike)A.MemberwiseClone();
            multiply(A, b, C);
            return C;
        }
        #endregion

    }
}
