﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Rawr
{
    public class Reforging
    {
        private class ReforgingIdComparer : EqualityComparer<Reforging>
        {
            public override bool Equals(Reforging x, Reforging y)
            {
                int xid = x != null ? x.Id : 0;
                int yid = y != null ? y.Id : 0;
                return xid == yid;
            }

            public override int GetHashCode(Reforging obj)
            {
                int id = obj != null ? obj.Id : 0;
                return id.GetHashCode();
            }
        }

        public static readonly EqualityComparer<Reforging> IdComparer = new ReforgingIdComparer();

        private AdditiveStat reforgeFrom;
        public AdditiveStat ReforgeFrom 
        {
            get { return reforgeFrom; }
            set {
                reforgeFrom = value;
                id = StatsToId(reforgeFrom, reforgeTo);
            }
        }

        private AdditiveStat reforgeTo;
        public AdditiveStat ReforgeTo 
        {
            get { return reforgeTo; }
            set {
                reforgeTo = value;
                id = StatsToId(reforgeFrom, reforgeTo);
            }
        }

        private int id;
        public int Id
        {
            get { return id; }
            set {
                id = value;
                while (id > 112) id -= 56;
                IdToStats(id, out reforgeFrom, out reforgeTo);
            }
        }

        private static AdditiveStat IdToStat(int id)
        {
            switch (id)
            {
                case 0:
                    return AdditiveStat.Spirit;
                case 1:
                    return AdditiveStat.DodgeRating;
                case 2:
                    return AdditiveStat.ParryRating;
                case 3:
                    return AdditiveStat.HitRating;
                case 4:
                    return AdditiveStat.CritRating;
                case 5:
                    return AdditiveStat.HasteRating;
                case 6:
                    return AdditiveStat.ExpertiseRating;
                case 7:
                    return AdditiveStat.MasteryRating;
                default:
                    return (AdditiveStat)(-1);
            }
        }

        private static int StatToId(AdditiveStat stat)
        {
            switch (stat)
            {
                case AdditiveStat.Spirit:
                    return 0;
                case AdditiveStat.DodgeRating:
                    return 1;
                case AdditiveStat.ParryRating:
                    return 2;
                case AdditiveStat.HitRating:
                    return 3;
                case AdditiveStat.CritRating:
                    return 4;
                case AdditiveStat.HasteRating:
                    return 5;
                case AdditiveStat.ExpertiseRating:
                    return 6;
                case AdditiveStat.MasteryRating:
                    return 7;
                default:
                    return -1;
            }
        }

        public static void IdToStats(int id, out AdditiveStat reforgeFrom, out AdditiveStat reforgeTo)
        {
            while (id > 56) id -= 56;
			id--;
            int from = id / 7;
            reforgeFrom = IdToStat(from);
            id %= 7;
            if (from <= id)
            {
                id++;
            }
            reforgeTo = IdToStat(id);
        }

        public static int StatsToId(AdditiveStat reforgeFrom, AdditiveStat reforgeTo)
        {
            int from = StatToId(reforgeFrom);
            int to = StatToId(reforgeTo);
            if (from == -1 || to == -1 || to == from)
            {
                return 0;
            }
            if (to > from)
            {
                to--;
            }
            return 57 + 7 * from + to;
        }

        public float ReforgeAmount { get; set; }

        public Reforging Clone()
        {
            return new Reforging()
            {
                reforgeFrom = this.reforgeFrom,
                reforgeTo = this.reforgeTo,
                id = this.id,
                ReforgeAmount = this.ReforgeAmount
            };
        }

        public Reforging() { }

        public Reforging(Item baseItem, int randomSuffixId, AdditiveStat reforgeFrom, AdditiveStat reforgeTo)
        {
            ApplyReforging(baseItem, randomSuffixId, reforgeFrom, reforgeTo);
        }

        public Reforging(Item baseItem, int randomSuffixId, int id)
        {
            ApplyReforging(baseItem, randomSuffixId, id);
        }

        public void ApplyReforging(Item baseItem, int randomSuffixId, AdditiveStat reforgeFrom, AdditiveStat reforgeTo)
        {
            this.reforgeFrom = reforgeFrom;
            this.reforgeTo = reforgeTo;
            id = StatsToId(reforgeFrom, reforgeTo);
            ApplyReforging(baseItem, randomSuffixId);
        }

        public void ApplyReforging(Item baseItem, int randomSuffixId, int id)
        {
            Id = id;
            ApplyReforging(baseItem, randomSuffixId);
        }

        public void ApplyReforging(Item baseItem, int randomSuffixId)
        {
            if (baseItem != null && id != 0 && Validate)
            {
                float currentFrom = CurrentStatValue(baseItem, randomSuffixId, ReforgeFrom);
                float currentTo = CurrentStatValue(baseItem, randomSuffixId, ReforgeTo);
                if (currentFrom > 0 && currentTo == 0)
                {
                    ReforgeAmount = (float)Math.Floor(currentFrom * 0.4);
                    return;
                }
            }
            ReforgeAmount = 0;
        }

        public static List<Reforging> GetReforgingOptions(Item baseItem, int randomSuffixId, AdditiveStat[] reforgeStatsFrom, AdditiveStat[] reforgeStatsTo)
        {
            List<Reforging> options = new List<Reforging>();
            options.Add(null);
            if (baseItem.ItemLevel >= 200)
            {
                foreach (var from in reforgeStatsFrom)
                {
                    float currentFrom = CurrentStatValue(baseItem, randomSuffixId, from);
                    if (currentFrom > 0)
                    {
                        foreach (var to in reforgeStatsTo)
                        {
                            float currentTo = CurrentStatValue(baseItem, randomSuffixId, to);
                            if (currentTo == 0)
                            {
                                options.Add(new Reforging(baseItem, randomSuffixId, from, to));
                            }
                        }
                    }
                }
            }
            return options;
        }

        public static float CurrentStatValue(Item baseItem, int randomSuffixId, AdditiveStat stat)
        {
            if (randomSuffixId == 0)
            {
                if ((int)stat >= 0 && (int)stat < baseItem.Stats._rawAdditiveData.Length)
                {
                        return baseItem.Stats._rawAdditiveData[(int)stat];
                }
                else 
                {
#if DEBUG
                    throw new IndexOutOfRangeException(string.Format("Invalid Stat index"));
#else
                    return 0f;
#endif
                }
            }
            else
            {
                return RandomSuffix.GetStatValue(baseItem, randomSuffixId, stat);
            }
        }

        public bool Validate { get { return (ReforgeFrom != (AdditiveStat)(-1)) && (ReforgeTo != (AdditiveStat)(-1)); } }

        /// <summary>
        /// A very short name for the button
        /// <example>Cr2Ha = Crit to Haste</example>
        /// </summary>
        public string VeryShortName {
            get {
                if (reforgeFrom == (AdditiveStat)(-1) || reforgeTo == (AdditiveStat)(-1)) { return "NR"; }
                return ReforgeTo.ToString().Substring(0, 3);
            }
        }

        public override string ToString()
        {
            if (Id <= 0 || !Validate) { return "Not Reforged"; }
            return string.Format("Reforge {0} {1} → {2}", ReforgeAmount, Extensions.SpaceCamel(ReforgeFrom.ToString()), Extensions.SpaceCamel(ReforgeTo.ToString()));
        }
    }
}
