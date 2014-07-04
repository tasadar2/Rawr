using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Load_SimC_DBC
{
    class SpellEffect
    {
        private uint id;
        private uint flags;
        private uint spellID;
        private uint index;
        private EffectType type;
        private EffectSubtype sub_type;
        private float average;
        private float delta;
        private float bonus;
        private float coefficient;
        private float amplitude;
        private float radius;
        private float max_radius;
        private int base_value;
        private int misc_value;
        private int misc_value2;
        private uint trigger_spell;
        private float chain;
        private float combo_points;
        private float level;
        private int damage_range;

        public SpellEffect()
        {
            id = 0;
            flags = 0x00;
            spellID = 0;
            index = 0;
            type = EffectType.E_NONE;
            sub_type = EffectSubtype.A_NONE;
            average = 0;
            delta = 0;
            bonus = 0;
            coefficient = 0;
            amplitude = 0;
            radius = 0;
            max_radius = 0;
            base_value = 0;
            misc_value = 0;
            misc_value2 = 0;
            trigger_spell = 0;
            chain = 0;
            combo_points = 0;
            level = 0;
            damage_range = 0;
        }

        public SpellEffect(List<string> list)
        {
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            id = Convert.ToUInt32(list[0]);
            flags = Convert.ToUInt32(list[1], 16);
            spellID = Convert.ToUInt32(list[2]);
            index = Convert.ToUInt32(list[3]);
            type = (EffectType)Enum.Parse(typeof(EffectType), list[4]);
            sub_type = (EffectSubtype)Enum.Parse(typeof(EffectSubtype), list[5]);
            average = float.Parse(list[6], culture);
            delta = float.Parse(list[7], culture);
            bonus = float.Parse(list[8], culture);
            coefficient = float.Parse(list[9], culture);
            amplitude = float.Parse(list[10], culture);
            radius = float.Parse(list[11], culture);
            max_radius = float.Parse(list[12], culture);
            base_value = Convert.ToInt32(list[13]);
            misc_value = Convert.ToInt32(list[14]);
            misc_value2 = Convert.ToInt32(list[15]);
            trigger_spell = Convert.ToUInt32(list[16]);
            chain = float.Parse(list[17], culture);
            combo_points = float.Parse(list[18], culture);
            level = float.Parse(list[19], culture);
            damage_range = Convert.ToInt32(list[20]);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            sb.Append("\t\t\t\tthis.Add( new DBCSpellEffect ( ");
            sb.Append(String.Format("{0}, ", id));
            sb.Append(String.Format("{0}, ", flags));
            sb.Append(String.Format("{0}, ", spellID));
            sb.Append(String.Format("{0}, ", index));
            sb.Append(String.Format("EffectType.{0}, ", type));
            sb.Append(String.Format("EffectSubtype.{0}, ", sub_type));
            sb.Append(String.Format("{0}f, ", average));
            sb.Append(String.Format("{0}f, ", delta));
            sb.Append(String.Format("{0}f, ", bonus));
            sb.Append(String.Format("{0}f, ", level));
            sb.Append(String.Format("{0}f, ", coefficient));
            sb.Append(String.Format("{0}f, ", radius));
            sb.Append(String.Format("{0}f, ", max_radius));
            sb.Append(String.Format("{0}, ", base_value));
            sb.Append(String.Format("{0}, ", misc_value));
            sb.Append(String.Format("{0}, ", misc_value2));
            sb.Append(String.Format("{0}, ", trigger_spell));
            sb.Append(String.Format("{0}f, ", chain));
            sb.Append(String.Format("{0}f, ", combo_points));
            sb.Append(String.Format("{0}f, ", level));
            sb.Append(String.Format("{0}", damage_range));
            sb.Append(" ) );");
            return sb.ToString();
        }
    }
}
