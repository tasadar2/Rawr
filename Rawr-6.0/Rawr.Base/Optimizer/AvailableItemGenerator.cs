﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rawr.Optimizer
{
    public class ItemAvailabilityInformation
    {
        public SuffixItem Item;
        public int GemCount;
        public Dictionary<string, bool> ItemAvailable = new Dictionary<string, bool>();
        public ItemInstance DefaultItemInstance;
        public List<ItemInstance> ItemList = new List<ItemInstance>();
        public List<List<DirectUpgradeEntry>> DirectUpgradeList;
        public List<DirectUpgradeEntry> MatchingDirectUpgradeList;
        public List<DirectUpgradeEntry> NonMatchingDirectUpgradeList;
        public List<DirectUpgradeEntry> SingleDirectUpgradeList;
        public Dictionary<string, DirectUpgradeEntry> MatchingMap;
        public Dictionary<string, DirectUpgradeEntry> NonMatchingMap;
        public List<Enchant> GenerativeEnchants;
        public List<Tinkering> GenerativeTinkerings;
        public int[] ValidSlots;
        public bool PositiveCostItem;
    }

    public class DirectUpgradeEntry
    {
        public ItemInstance ItemInstance { get; set; }
        public List<DirectUpgradeEntry> DirectUpgradeList { get; set; }
    }

    public struct SuffixItem
    {
        public Item Item { get; set; }
        public int RandomSuffixId { get; set; }

        public string Name
        {
            get
            {
                if (Item == null) return null;
                if (RandomSuffixId != 0)
                {
                    return Item.Name + " " + RandomSuffix.GetSuffix(RandomSuffixId);
                }
                else
                {
                    return Item.Name;
                }
            }
        }

        public string SuffixId
        {
            get
            {
                if (Item == null) return "0.0";
                return Item.Id + "." + RandomSuffixId;
            }
        }
    }

    /// <summary>
    /// Provides means to generate available items based on available item strings from character.
    /// </summary>
    public class AvailableItemGenerator
    {
        private Dictionary<string, bool> itemAvailable = new Dictionary<string, bool>();
        private List<string> availableItems;
        private bool overrideRegem;
        private bool overrideReenchant;
        private bool overrideReforge;
        private bool slotFiltering;
        private bool generateDirectUpgrades;
        private Character[] characters;
        private CalculationsBase[] models;

        private const int slotCount = 19;
        //private List<ItemInstance>[] slotItems = new List<ItemInstance>[slotCount];
        private Item[] metaGemItems;
        private Item[] gemItems;
        private Item[] cogwheelItems;
        private Item[] hydraulicItems;
        private Enchant[][] slotAvailableEnchants = new Enchant[slotCount][];
        private Tinkering[][] slotAvailableTinkerings = new Tinkering[slotCount][];
        //private List<List<DirectUpgradeEntry>>[] slotDirectUpgrades = new List<List<DirectUpgradeEntry>>[slotCount];
        private List<ItemAvailabilityInformation>[] slotRawItems = new List<ItemAvailabilityInformation>[slotCount];
        private List<ItemAvailabilityInformation> rawItems = new List<ItemAvailabilityInformation>();

        private bool ArrayContains<T>(T[] array, Func<T, bool> predicate)
        {
            return array.Any(predicate);
        }


        /*public List<List<DirectUpgradeEntry>>[] SlotDirectUpgrades
        {
            get
            {
                return slotDirectUpgrades;
            }
        }*/

        public Enchant[][] SlotEnchants
        {
            get
            {
                return slotAvailableEnchants;
            }
        }

        public Tinkering[][] SlotTinkerings
        {
            get
            {
                return slotAvailableTinkerings;
            }
        }


        public Dictionary<string, bool> ItemAvailable
        {
            get
            {
                return itemAvailable;
            }
        }

        /*public List<ItemInstance>[] SlotItems
        {
            get
            {
                return slotItems;
            }
        }*/

        public List<ItemAvailabilityInformation>[] SlotRawItems
        {
            get
            {
                return slotRawItems;
            }
        }

        public List<ItemAvailabilityInformation> RawItems
        {
            get
            {
                return rawItems;
            }
        }

        public Item[] MetaGemItems
        {
            get
            {
                return metaGemItems;
            }
        }

        public Item[] Cogwheeltems
        {
            get
            {
                return cogwheelItems;
            }
        }

        public Item[] HydraulicItems
        {
            get
            {
                return hydraulicItems;
            }
        }

        public Item[] GemItems
        {
            get
            {
                return gemItems;
            }
        }

        /// <summary>
        /// Replaces gems/enchants on the character to minimize the number of changes from root items while preserving total stats.
        /// </summary>
        public void NormalizeCharacter(Character character, bool invalidate)
        {
            // main assumption is that gems and enchants have fixed stat allocation
            // meaning that the matched socket bonuses must be maintained in order to preserve total stats

            // we're only looking at gem shuffling
            // it's possible that gem splits happen like 20+20 => 10/10+10/10
            // we don't handle those currently, to fully support things like that we'd need to add
            // logic that makes sure overall uniqeness is maintained and meta gem requirements don't break
            
            // we'll be doing a bunch of Stats comparisons, this is a potentially expensive operation

            // first check which slots are nondefault
            // and get a list of socket bonuses that are matched

            Stats zeroStats = new Stats();
            List<Stats> matchedSocketBonuses = new List<Stats>();
            List<int> matchedSocketBonusCount = new List<int>();
            int matchedBonusTotal = 0;
            Dictionary<Item, int> gems = new Dictionary<Item, int>();

            bool[] slotDefault = new bool[Character.OptimizableSlotCount];
            bool[] slotFilled = new bool[Character.OptimizableSlotCount];
            int[] socketBonusIndex = new int[Character.OptimizableSlotCount];
            bool[] hasSocketBonus = new bool[Character.OptimizableSlotCount];

            ItemInstance[] items = new ItemInstance[Character.OptimizableSlotCount];

            for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
            {
                ItemInstance item = character._item[slot];
                ItemAvailabilityInformation availabilityInformation;
                if (item != null && (availabilityInformation = item.ItemAvailabilityInformation) != null)
                {
                    slotDefault[slot] = (item == availabilityInformation.DefaultItemInstance);
                    if (!slotDefault[slot])
                    {
                        for (int gem = 1; gem <= 3; gem++)
                        {
                            Item g = item.GetGem(gem);
                            if (g != null)
                            {
                                int count;
                                gems.TryGetValue(g, out count);
                                gems[g] = count + 1;
                            }
                        }
                        if (item.Item.SocketBonus > zeroStats)
                        {
                            hasSocketBonus[slot] = true;
                            if (item.MatchesSocketBonus)
                            {
                                matchedBonusTotal++;
                                // check if we have it in list already
                                bool found = false;
                                for (int i = 0; i < matchedSocketBonuses.Count; i++)
                                {
                                    if (matchedSocketBonuses[i] == item.Item.SocketBonus)
                                    {
                                        matchedSocketBonusCount[i]++;
                                        found = true;
                                        break;
                                    }
                                }
                                if (!found)
                                {
                                    matchedSocketBonuses.Add(item.Item.SocketBonus);
                                    matchedSocketBonusCount.Add(1);
                                }
                            }
                        }
                    }
                    else
                    {
                        items[slot] = item;
                        slotFilled[slot] = true;
                    }
                }
                else
                {
                    slotDefault[slot] = true;
                    slotFilled[slot] = true;
                    items[slot] = item;
                }
            }

            for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
            {
                if (!slotFilled[slot])
                {
                    if (hasSocketBonus[slot])
                    {
                        ItemInstance item = character._item[slot];
                        for (int i = 0; i < matchedSocketBonuses.Count; i++)
                        {
                            if (matchedSocketBonuses[i] == item.Item.SocketBonus)
                            {
                                socketBonusIndex[slot] = i;
                                break;
                            }
                        }
                    }
                }
            }

            // from now on we only have to care about the slots that are nondefault (note we know all nondefault slots are not null)

            // now what we'll do is first make sure we meet all needed socket bonuses in some way

            // just going greedy about it doesn't always work
            // it can happen that by going for minimizing differences you run out of options
            // to fulfill socket bonuses, so it's necessary to keep track of colors you have left
            // to make sure you don't lock yourself out

            for (int bonusCounter = 0; bonusCounter < matchedBonusTotal; bonusCounter++)
            {
                // find an item with this socket bonus, with an item instance that has gems that are still available
                bool found = false;
                int bestSlot = -1;
                int bestBonusIndex = -1;
                int differentGems = 4;
                ItemInstance bestItem = null;
                for (int bonusIndex = 0; bonusIndex < matchedSocketBonuses.Count; bonusIndex++)
                {
                    if (matchedSocketBonusCount[bonusIndex] > 0)
                    {
                        Stats socketBonus = matchedSocketBonuses[bonusIndex];
                        for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
                        {
                            if (!slotFilled[slot])
                            {
                                ItemInstance item = character._item[slot];
                                var availabilityInformation = item.ItemAvailabilityInformation;
                                ItemInstance defaultItem = availabilityInformation.DefaultItemInstance;
                                if (item.Item.SocketBonus == socketBonus)
                                {
                                    // check all available items
                                    // only select those that match socket bonus and enchant and compare how much it differs from the default
                                    foreach (ItemInstance itemInstance in availabilityInformation.ItemList)
                                    {
#if RAWR4
                                        if (itemInstance.MatchesSocketBonus && itemInstance.Enchant == item.Enchant && itemInstance.ReforgeId == item.ReforgeId && itemInstance.TinkeringId == item.TinkeringId && itemInstance.RandomSuffixId == item.RandomSuffixId)
#else
                                        if (itemInstance.MatchesSocketBonus && itemInstance.Enchant == item.Enchant)
#endif
                                        {
                                            Dictionary<Item, int> gemCount = new Dictionary<Item, int>();
                                            // count gems
                                            for (int gem = 1; gem <= 3; gem++)
                                            {
                                                Item g = itemInstance.GetGem(gem);
                                                if (g != null)
                                                {
                                                    int count;
                                                    gemCount.TryGetValue(g, out count);
                                                    gemCount[g] = count + 1;
                                                }
                                            }
                                            // make sure the needed counts are still available
                                            bool available = true;
                                            foreach (KeyValuePair<Item, int> kvp in gemCount)
                                            {
                                                int count;
                                                gems.TryGetValue(kvp.Key, out count);
                                                if (count < kvp.Value)
                                                {
                                                    available = false;
                                                    break;
                                                }
                                            }
                                            if (available && RemainingSocketBonusesCanBeMatched(character, slotFilled, slot, matchedSocketBonusCount, hasSocketBonus, socketBonusIndex, bonusIndex, gemCount, gems)) // we could move this check further down, but make sure to make a copy of gemCount
                                            {
                                                // we found an item instance with matched enchant that matches socket bonus and has all needed gems at disposal
                                                found = true;
                                                // how much does it differ from default?
                                                int diff;
                                                if (defaultItem == null)
                                                {
                                                    diff = 0; // nothing to base from, we can make any gemming we want
                                                }
                                                else
                                                {
                                                    diff = 0;
                                                    for (int gem = 1; gem <= 3; gem++)
                                                    {
                                                        Item g = defaultItem.GetGem(gem);
                                                        if (g != null)
                                                        {
                                                            int count;
                                                            gemCount.TryGetValue(g, out count);
                                                            if (count > 0)
                                                            {
                                                                gemCount[g] = count - 1;
                                                            }
                                                            else
                                                            {
                                                                diff++;
                                                            }
                                                        }
                                                    }
                                                }
                                                if (diff < differentGems)
                                                {
                                                    bestSlot = slot;
                                                    bestBonusIndex = bonusIndex;
                                                    bestItem = itemInstance;
                                                    differentGems = diff;
                                                    if (diff == 0)
                                                    {
                                                        goto FILLSLOT;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            FILLSLOT:
                if (found)
                {
                    items[bestSlot] = bestItem;
                    slotFilled[bestSlot] = true;
                    matchedSocketBonusCount[bestBonusIndex]--;
                    // remove used up gems
                    for (int gem = 1; gem <= 3; gem++)
                    {
                        Item g = bestItem.GetGem(gem);
                        if (g != null)
                        {
                            int count;
                            gems.TryGetValue(g, out count);
                            gems[g] = count - 1;
                        }
                    }
                }
                else
                {
                    // we were not able to find any available item that would match the bonus and retain enchant
                    // which means we failed to find stat equivalent solution
                    // we have to return the input as we can't find anything better
                    return;
                }
            }

            // we matched all socket bonuses
            // now the easy stuff, just throw in the gems wherever you fill like it
            // just need to be careful about only using available items

            for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
            {
                if (!slotFilled[slot])
                {
                    ItemInstance item = character._item[slot];
                    var availabilityInformation = item.ItemAvailabilityInformation;
                    ItemInstance defaultItem = availabilityInformation.DefaultItemInstance;
                    bool found = false;
                    int differentGems = 4;
                    ItemInstance bestItem = null;
                    // check all available items
                    // only select those that match enchant and compare how much it differs from the default
                    foreach (ItemInstance itemInstance in availabilityInformation.ItemList)
                    {                        
#if RAWR4
                        if (itemInstance.Enchant == item.Enchant && itemInstance.ReforgeId == item.ReforgeId && itemInstance.TinkeringId == item.TinkeringId && itemInstance.RandomSuffixId == item.RandomSuffixId)
#else
                        if (itemInstance.Enchant == item.Enchant)
#endif
                        {
                            Dictionary<Item, int> gemCount = new Dictionary<Item, int>();
                            // count gems
                            gemCount.Clear();
                            for (int gem = 1; gem <= 3; gem++)
                            {
                                Item g = itemInstance.GetGem(gem);
                                if (g != null)
                                {
                                    int count;
                                    gemCount.TryGetValue(g, out count);
                                    gemCount[g] = count + 1;
                                }
                            }
                            // make sure the needed counts are still available
                            bool available = true;
                            foreach (KeyValuePair<Item, int> kvp in gemCount)
                            {
                                int count;
                                gems.TryGetValue(kvp.Key, out count);
                                if (count < kvp.Value)
                                {
                                    available = false;
                                    break;
                                }
                            }
                            if (available)
                            {
                                // we found an item instance with matched enchant that has all needed gems at disposal
                                found = true;
                                // how much does it differ from default?
                                int diff;
                                if (defaultItem == null)
                                {
                                    diff = 0; // nothing to base from, we can make any gemming we want
                                }
                                else
                                {
                                    diff = 0;
                                    for (int gem = 1; gem <= 3; gem++)
                                    {
                                        Item g = defaultItem.GetGem(gem);
                                        if (g != null)
                                        {
                                            int count;
                                            gemCount.TryGetValue(g, out count);
                                            if (count > 0)
                                            {
                                                gemCount[g] = count - 1;
                                            }
                                            else
                                            {
                                                diff++;
                                            }
                                        }
                                    }
                                }
                                if (diff < differentGems)
                                {
                                    bestItem = itemInstance;
                                    differentGems = diff;
                                    if (diff == 0)
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (found)
                    {
                        items[slot] = bestItem;
                        slotFilled[slot] = true;
                        // remove used up gems and slots
                        for (int gem = 1; gem <= 3; gem++)
                        {
                            Item g = bestItem.GetGem(gem);
                            if (g != null)
                            {
                                int count;
                                gems.TryGetValue(g, out count);
                                gems[g] = count - 1;
                            }
                        }
                    }
                    else
                    {
                        // probably a problem with availability
                        // we don't have the right gems anymore so we can't construct a character that maintains total stats
                        return;
                    }
                }
            }

            // we were able to fill all slots, that is greedy min diff character
            character.SetItems(items, invalidate);
        }

        private bool RemainingSocketBonusesCanBeMatched(Character character, bool[] slotFilled, int slot, List<int> matchedSocketBonusCount, bool[] hasSocketBonus, int[] socketBonusIndex, int bonusIndex, Dictionary<Item, int> gemCount, Dictionary<Item, int> gems)
        {
            // we need this wrapper because we can have several instances of same socket bonus, but we only need some of them matched, and they need not all have same sockets
            // we have to do an exhaustive enumeration of which items are actually used to fill sockets
            // only if all are bad the whole thing is bad

            bool[] hasMatchingSocketBonus = new bool[Character.OptimizableSlotCount];
            int[] bonusCount = new int[matchedSocketBonusCount.Count];
            int[] bonusCountActual = new int[matchedSocketBonusCount.Count];
            int matchCount = 0;
            for (int i = 0; i < matchedSocketBonusCount.Count; i++)
            {
                bonusCount[i] = matchedSocketBonusCount[i];
                matchCount += bonusCount[i];
            }
            bonusCount[bonusIndex]--;
            matchCount--;

            List<int> matchingIndex = new List<int>();
            for (int s = 0; s < Character.OptimizableSlotCount; s++)
            {
                if (!slotFilled[s] && s != slot)
                {
                    if (hasSocketBonus[s])
                    {
                        int sbindex = socketBonusIndex[s];
                        if (bonusCount[sbindex] > 0)
                        {
                            matchingIndex.Add(s);
                            hasMatchingSocketBonus[s] = true;
                        }
                    }
                }
            }

            // we could do decomposition on type of actual bonus and do a cross product on components
            // but we'll just do a simpler thing and do combinations of matchCount out of matchingIndex.Count

            bool[] selected = new bool[matchingIndex.Count];
            for (int i = matchingIndex.Count - matchCount; i < matchingIndex.Count; i++)
            {
                selected[i] = true;
            }

            do
            {
                // check if this combination is a valid selection of bonuses
                Array.Clear(bonusCountActual, 0, bonusCountActual.Length);
                int R = 0, Y = 0, B = 0, C = 0, H = 0;
                for (int k = 0; k < selected.Length; k++)
                {
                    if (selected[k])
                    {
                        bonusCountActual[socketBonusIndex[matchingIndex[k]]]++;
                        ItemInstance item = character._item[matchingIndex[k]];
                        for (int gem = 1; gem <= 3; gem++)
                        {
                            switch (item.Item.GetSocketColor(gem))
                            {
                                case ItemSlot.Red:
                                    R++;
                                    break;
                                case ItemSlot.Blue:
                                    B++;
                                    break;
                                case ItemSlot.Yellow:
                                    Y++;
                                    break;
                                case ItemSlot.Cogwheel:
                                    C++;
                                    break;
                                case ItemSlot.Hydraulic:
                                    H++;
                                    break;
                            }
                        }
                    }
                }
                bool valid = true;
                for (int k = 0; k < matchedSocketBonusCount.Count; k++)
                {
                    if (bonusCountActual[k] != bonusCount[k])
                    {
                        valid = false;
                        break;
                    }
                }

                if (valid)
                {
                    if (RemainingSocketsCanBeMatched(gems, gemCount, R, B, Y, C, H))
                    {
                        return true;
                    }
                }

                // move to next combination
                int i = selected.Length - 1;
                if (i == 0)
                {
                    // we enumerated all combinations, time to fail
                    return false;
                }

                while (selected[i - 1] || !selected[i])
                {
                    i = i - 1;
                    if (i == 0)
                    {
                        // we enumerated all combinations, time to fail
                        return false;
                    }
                }

                int j = selected.Length;

                while (!selected[j - 1] || selected[i - 1]) j = j - 1;

                // swap values at positions (i-1) and (j-1)
                bool tmp = selected[i - 1];
                selected[i - 1] = selected[j - 1];
                selected[j - 1] = tmp;

                i++; j = selected.Length;

                while (i < j)
                {
                    // swap values at positions (i-1) and (j-1)
                    tmp = selected[i - 1];
                    selected[i - 1] = selected[j - 1];
                    selected[j - 1] = tmp;
                    i++;
                    j--;
                }
            } while (true);
        }

        /// <summary>
        /// Checks if we can still match the socket colors.
        /// </summary>
        /// <param name="gemsAll">All gems available.</param>
        /// <param name="gemCount">Proposed gems to use.</param>
        /// <returns>Returns true if we can match the sockets.</returns>
        private bool RemainingSocketsCanBeMatched(Dictionary<Item, int> gemsAll, Dictionary<Item, int> gemCount, int R, int B, int Y, int C, int H)
        {
            // we'll do this by first matching pure colors, then bicolor and prismatic

            // init gems
            Dictionary<Item, int> gems = new Dictionary<Item, int>(gemsAll);
            foreach (var kvp in gemCount)
            {
                int v;
                gems.TryGetValue(kvp.Key, out v);
                gems[kvp.Key] = v - kvp.Value;
            }

            // pure pass
            int N = 0, O = 0, P = 0, G = 0;
            foreach (var kvp in gems)
            {
                ItemSlot color = kvp.Key.Slot;
                switch (color)
                {
                    case ItemSlot.Red:
                        R = Math.Max(0, R - kvp.Value);
                        break;
                    case ItemSlot.Yellow:
                        Y = Math.Max(0, Y - kvp.Value);
                        break;
                    case ItemSlot.Blue:
                        B = Math.Max(0, B - kvp.Value);
                        break;
                    case ItemSlot.Cogwheel:
                        C = Math.Max(0, C - kvp.Value);
                        break;
                    case ItemSlot.Hydraulic:
                        H = Math.Max(0, H - kvp.Value);
                        break;
                    case ItemSlot.Green:
                        G += kvp.Value;
                        break;
                    case ItemSlot.Purple:
                        P += kvp.Value;
                        break;
                    case ItemSlot.Orange:
                        O += kvp.Value;
                        break;
                    case ItemSlot.Prismatic:
                        N += kvp.Value;
                        break;
                }
            }

            // let's do some math
            // we have socket color values to fill and bicolor gem counts
            // each bicolor if used will be in one of the two colors, split the variable in two based on which one
            // vars: SocketRed, SocketBlue, SocketYellow
            //       GemPurple, GemGreen, GemOrange, GemPrismatic
            //       GemPurpleRed, GemPurpleBlue, GemGreenYellow, GemGreenBlue, GemOrangeYellow, GemOrangeRed
            //       GemPrismaticRed, GemPrismaticYellow, GemPrismaticBlue
            // GemPurple >= GemPurpleRed + GemPurpleBlue
            // GemGreen >= GemGreenYellow + GemGreenBlue
            // GemOrange >= GemOrangeYellow + GemOrangeRed
            // GemPrismatic >= GemPrismaticRed + GemPrismaticYellow + GemPrismaticBlue
            // if we can match the sockets then the following equations must hold
            // GemPurpleRed + GemOrangeRed + GemPrismaticRed >= SocketRed
            // GemPurpleBlue + GemGreenBlue + GemPrismaticBlue >= SocketBlue
            // GemGreenYellow + GemOrangeYellow + GemPrismaticYellow >= SocketYellow

            // this code is prone to copy/paste error
            // it tries to see if there is a solution in a way reminiscing phase I LP
            // if someone has a more elegant solution please replace
            int PR = P, PB = 0, GY = G, GB = 0, OY = O, OR = 0, NR = N, NY = 0, NB = 0;
            do
            {
                int Rdiff = R - PR - OR - NR;
                int Bdiff = B - PB - GB - NB;
                int Ydiff = Y - GY - OY - NY;
                if (Rdiff <= 0 && Bdiff <= 0 && Ydiff <= 0)
                {
                    // we can match all constraints
                    return true;
                }
                if (Rdiff > 0 && Bdiff < 0)
                {
                    // move from blue to red
                    if (NB > 0)
                    {
                        int move = Math.Min(NB, Rdiff);
                        NB -= move;
                        NR += move;
                        continue;
                    }
                    if (PB > 0)
                    {
                        int move = Math.Min(PB, Rdiff);
                        PB -= move;
                        PR += move;
                        continue;
                    }
                    // via yellow
                    if (GB > 0 && OY > 0)
                    {
                        int move = Math.Min(Math.Min(GB, Rdiff), OY);
                        GB -= move;
                        GY += move;
                        OY -= move;
                        OR += move;
                        continue;
                    }
                    if (GB > 0 && NY > 0)
                    {
                        int move = Math.Min(Math.Min(GB, Rdiff), NY);
                        GB -= move;
                        GY += move;
                        NY -= move;
                        NR += move;
                        continue;
                    }
                }
                if (Rdiff > 0 && Ydiff < 0)
                {
                    // move from yellow to red
                    if (NY > 0)
                    {
                        int move = Math.Min(NY, Rdiff);
                        NY -= move;
                        NR += move;
                        continue;
                    }
                    if (OY > 0)
                    {
                        int move = Math.Min(OY, Rdiff);
                        OY -= move;
                        OR += move;
                        continue;
                    }
                    // via blue
                    if (GY > 0 && PB > 0)
                    {
                        int move = Math.Min(Math.Min(GY, Rdiff), PB);
                        GY -= move;
                        GB += move;
                        PB -= move;
                        PR += move;
                        continue;
                    }
                    if (GY > 0 && NB > 0)
                    {
                        int move = Math.Min(Math.Min(GY, Rdiff), NB);
                        GY -= move;
                        GB += move;
                        NB -= move;
                        NR += move;
                        continue;
                    }
                }
                if (Bdiff > 0 && Rdiff < 0)
                {
                    // move from red to blue
                    if (NR > 0)
                    {
                        int move = Math.Min(NR, Bdiff);
                        NR -= move;
                        NB += move;
                        continue;
                    }
                    if (PR > 0)
                    {
                        int move = Math.Min(PR, Bdiff);
                        PR -= move;
                        PB += move;
                        continue;
                    }
                    // via yellow
                    if (OR > 0 && GY > 0)
                    {
                        int move = Math.Min(Math.Min(OR, Bdiff), GY);
                        OR -= move;
                        OY += move;
                        GY -= move;
                        GB += move;
                        continue;
                    }
                    if (OR > 0 && NY > 0)
                    {
                        int move = Math.Min(Math.Min(OR, Bdiff), NY);
                        OR -= move;
                        OY += move;
                        NY -= move;
                        NB += move;
                        continue;
                    }
                }
                if (Bdiff > 0 && Ydiff < 0)
                {
                    // move from yellow to blue
                    if (NY > 0)
                    {
                        int move = Math.Min(NY, Bdiff);
                        NY -= move;
                        NB += move;
                        continue;
                    }
                    if (GY > 0)
                    {
                        int move = Math.Min(GY, Bdiff);
                        GY -= move;
                        GB += move;
                        continue;
                    }
                    // via red
                    if (OY > 0 && PR > 0)
                    {
                        int move = Math.Min(Math.Min(OY, Bdiff), PR);
                        OY -= move;
                        OR += move;
                        PR -= move;
                        PB += move;
                        continue;
                    }
                    if (OY > 0 && NR > 0)
                    {
                        int move = Math.Min(Math.Min(OY, Bdiff), NR);
                        OY -= move;
                        OR += move;
                        NR -= move;
                        NB += move;
                        continue;
                    }
                }
                if (Ydiff > 0 && Bdiff < 0)
                {
                    // move from blue to yellow
                    if (NB > 0)
                    {
                        int move = Math.Min(NB, Ydiff);
                        NB -= move;
                        NY += move;
                        continue;
                    }
                    if (GB > 0)
                    {
                        int move = Math.Min(GB, Ydiff);
                        GB -= move;
                        GY += move;
                        continue;
                    }
                    // via red
                    if (PB > 0 && OR > 0)
                    {
                        int move = Math.Min(Math.Min(PB, Ydiff), OR);
                        PB -= move;
                        PR += move;
                        OR -= move;
                        OY += move;
                        continue;
                    }
                    if (PB > 0 && NR > 0)
                    {
                        int move = Math.Min(Math.Min(PB, Ydiff), NR);
                        PB -= move;
                        PR += move;
                        NR -= move;
                        NY += move;
                        continue;
                    }
                }
                if (Ydiff > 0 && Rdiff < 0)
                {
                    // move from red to yellow
                    if (NR > 0)
                    {
                        int move = Math.Min(NR, Ydiff);
                        NR -= move;
                        NY += move;
                        continue;
                    }
                    if (OR > 0)
                    {
                        int move = Math.Min(OR, Ydiff);
                        OR -= move;
                        OY += move;
                        continue;
                    }
                    // via blue
                    if (PR > 0 && GB > 0)
                    {
                        int move = Math.Min(Math.Min(PR, Ydiff), GB);
                        PR -= move;
                        PB += move;
                        GB -= move;
                        GY += move;
                        continue;
                    }
                    if (PR > 0 && NB > 0)
                    {
                        int move = Math.Min(Math.Min(PR, Ydiff), NB);
                        PR -= move;
                        PB += move;
                        NB -= move;
                        NY += move;
                        continue;
                    }
                }
                // if we found no valid move this means the system is not feasible
                return false;
            } while (true);
        }

        /// <summary>
        /// Checks whether the equipped items on the character are marked as available.
        /// </summary>
        public bool IsCharacterValid(Character character, out string warning, bool explain)
        {
            StringBuilder s = new StringBuilder();
            s.AppendLine("The following currently equipped items are not available");
            s.AppendLine();
            string line;
            List<string> warnings = new List<string>();
            bool valid = true; bool markGemTemplatesNotice = false;
            // if item is not available pick the one that is available
            for (int slot = 1; slot < Character.OptimizableSlotCount; slot++)
            {
                ItemInstance item = character._item[slot];
                if (item != null && item.Item != null)
                {
                    if (item.Item.AvailabilityInformation != null)
                    {
                        if (!itemAvailable.ContainsKey(item.GemmedId))
                        {
                            // gemming/enchant is not available
                            if (explain)
                            {
                                // try to determine what they have to do to make it available
                                switch (character.GetItemAvailability(item))
                                {
                                    case ItemAvailability.Available:
                                        // shouldn't happen
                                        break;
                                    case ItemAvailability.AvailableWithEnchantRestrictions:
                                        // they have marked this specific gemmming available, but it doesn't allow this enchant
                                        // warn about the enchant
                                        if (item.EnchantId != 0)
                                        {
                                            line = item.Enchant.Name + " is not available on " + item.Item.Name;
                                            if (!warnings.Contains(line))
                                            {
                                                warnings.Add(line);
                                                s.AppendLine(line);
                                            }
                                            valid = false;
                                        }
                                        break;
                                    case ItemAvailability.RegemmingAllowed:
                                        // all gemmings/enchants are available so the ones that are available must not be sufficient
                                        for (int gem = 1; gem <= 3; gem++)
                                        {
                                            Item g = item.GetGem(gem);
                                            if (g != null)
                                            {
                                                // ignore if we have something strictly better marked
                                                if ((g.Slot == ItemSlot.Meta && !ArrayContains(MetaGemItems, gg => gg != null && gg.Stats >= g.Stats)) ||
                                                    (g.Slot == ItemSlot.Cogwheel && !ArrayContains(Cogwheeltems, gg => gg != null && gg.Stats >= g.Stats)) ||
                                                    (g.Slot == ItemSlot.Hydraulic && !ArrayContains(HydraulicItems, gg => gg != null && gg.Stats >= g.Stats)) ||
                                                    (g.Slot != ItemSlot.Meta && g.Slot != ItemSlot.Cogwheel && g.Slot != ItemSlot.Hydraulic &&
                                                    !ArrayContains(GemItems, gg => gg != null && (gg.Id == g.Id || (gg.Stats >= g.Stats && !gg.IsLimitedGem)))))
                                                {
                                                    if (!markGemTemplatesNotice) { markGemTemplatesNotice = true; }
                                                    // gem is not available
                                                    line = g.Name + " is not available";
                                                    if (!warnings.Contains(line))
                                                    {
                                                        warnings.Add(line);
                                                        s.AppendLine(line);
                                                    }
                                                    valid = false;
                                                }
                                            }
                                        }
                                        Enchant enchant = item.Enchant;
                                        if (enchant != null && enchant.Id != 0 && character.CurrentCalculations.IsEnchantRelevant(enchant, character))
                                        {
                                            // ignore if we have something strictly better marked
                                            if (!ArrayContains(SlotEnchants[slot], e => e.Id == enchant.Id || e.Stats >= enchant.Stats))
                                            {
                                                // enchant is not available
                                                line = item.Enchant.Name + " is not available";
                                                if (!warnings.Contains(line))
                                                {
                                                    warnings.Add(line);
                                                    s.AppendLine(line);
                                                }
                                                valid = false;
                                            }
                                        }
                                        Tinkering tinkering = item.Tinkering;
                                        if (tinkering != null && tinkering.Id != 0 && character.CurrentCalculations.IsTinkeringRelevant(tinkering, character))
                                        {
                                            // ignore if we have something strictly better marked
                                            if (!ArrayContains(SlotTinkerings[slot], t => t.Id == tinkering.Id || t.Stats >= tinkering.Stats))
                                            {
                                                // tinkering is not available
                                                line = item.Tinkering.Name + " is not available";
                                                if (!warnings.Contains(line))
                                                {
                                                    warnings.Add(line);
                                                    s.AppendLine(line);
                                                }
                                                valid = false;
                                            }
                                        }
                                        break;
                                    case ItemAvailability.RegemmingAllowedWithEnchantRestrictions:
                                        // all gemmings are available so the ones that are available must not be sufficient
                                        for (int gem = 1; gem <= 3; gem++)
                                        {
                                            Item g = item.GetGem(gem);
                                            if (g != null)
                                            {
                                                // ignore if we have something strictly better marked
                                                if ((g.Slot == ItemSlot.Meta && !ArrayContains(MetaGemItems, gg => gg.Stats >= g.Stats)) ||
                                                    (g.Slot != ItemSlot.Meta && !ArrayContains(GemItems, gg => gg.Id == g.Id || (gg.Stats >= g.Stats && !gg.IsLimitedGem))))
                                                {
                                                    if (!markGemTemplatesNotice) { markGemTemplatesNotice = true; }
                                                    // gem is not available
                                                    line = g.Name + " is not available";
                                                    if (!warnings.Contains(line))
                                                    {
                                                        warnings.Add(line);
                                                        s.AppendLine(line);
                                                    }
                                                    valid = false;
                                                }
                                            }
                                        }
                                        if (item.EnchantId != 0 && !character.AvailableItems.Contains(item.Id + ".*.*.*." + item.EnchantId))
                                        {
                                            // this specific enchant is not valid                                        
                                            line = item.Enchant.Name + " is not available on " + item.Item.Name;
                                            if (!warnings.Contains(line))
                                            {
                                                warnings.Add(line);
                                                s.AppendLine(line);
                                            }
                                            valid = false;
                                        }
                                        break;
                                    case ItemAvailability.NotAvailable:
                                        if (!markGemTemplatesNotice) { markGemTemplatesNotice = true; }
                                        // they could have some other gemming/enchant marked as available, but not in general
                                        line = item.Item.Name + " gemming/enchant/tinkering is not available";
                                        if (!warnings.Contains(line))
                                        {
                                            warnings.Add(line);
                                            s.AppendLine(line);
                                        }
                                        valid = false;
                                        break;
                                }
                            }
                            else
                            {
                                if (!markGemTemplatesNotice) { markGemTemplatesNotice = true; }
                                line = item.Item.Name + " gemming/enchant/tinkering is not available";
                                if (!warnings.Contains(line))
                                {
                                    warnings.Add(line);
                                    s.AppendLine(line);
                                }
                                valid = false;
                            }
                        }
                    }
                    else
                    {
                        // item itself is not available
                        line = item.Item.Name + " is not available";
                        if (!warnings.Contains(line))
                        {
                            warnings.Add(line);
                            s.AppendLine(line);
                        }
                        valid = false;
                    }
                }
            }
            if (!valid)
            {
                if (markGemTemplatesNotice) {
                    s.AppendLine();
                    s.AppendLine("You most likely haven't enabled the correct Gemming Templates. See Options > Edit Gemming Templates ");
                }
                s.AppendLine();
#if SILVERLIGHT
                // In Silverlight the only real blocking option only gives two options, so deal with it
                s.AppendLine("Would you like to continue with the optimization?");
#else
                s.AppendLine("Would you like to make them available and continue with the optimization?");
#endif
                warning = s.ToString();
            }
            else
            {
                warning = null;
            }
            return valid;
        }

        /// <summary>
        /// Adds missing items/enchants/gems/tinkerings as available. This requires recreation of the item generator.
        /// </summary>
        public void MakeCharacterValid(Character character)
        {
            // if item is not available pick the one that is available
            for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
            {
                ItemInstance item = character._item[slot];
                if (item != null && item.Item != null)
                {
                    if (item.Item.AvailabilityInformation != null)
                    {
                        if (!itemAvailable.ContainsKey(item.GemmedId))
                        {
                            // gemming/enchant is not available
                            // try to determine what they have to do to make it available
                            switch (character.GetItemAvailability(item))
                            {
                                case ItemAvailability.Available:
                                    // shouldn't happen
                                    break;
                                case ItemAvailability.AvailableWithEnchantRestrictions:
                                    // they have marked this specific gemmming available, but it doesn't allow this enchant
                                    // warn about the enchant
                                    if (item.EnchantId != 0)
                                    {
                                        if (character.GetItemAvailability(item.Enchant) != ItemAvailability.Available)
                                        {
                                            character.ToggleItemAvailability(item.Enchant);
                                        }
                                    }
                                    break;
                                case ItemAvailability.RegemmingAllowed:
                                    // all gemmings/enchants are available so the ones that are available must not be sufficient
                                    for (int gem = 1; gem <= 3; gem++)
                                    {
                                        Item g = item.GetGem(gem);
                                        if (g != null)
                                        {
                                            // ignore if we have something strictly better marked
                                            if ((g.Slot == ItemSlot.Meta && !ArrayContains(MetaGemItems, gg => gg != null && gg.Stats >= g.Stats)) ||
                                                (g.Slot == ItemSlot.Cogwheel && !ArrayContains(Cogwheeltems, gg => gg != null && gg.Stats >= g.Stats)) ||
                                                (g.Slot == ItemSlot.Hydraulic && !ArrayContains(HydraulicItems, gg => gg != null && gg.Stats >= g.Stats)) ||
                                                (g.Slot != ItemSlot.Meta && g.Slot != ItemSlot.Cogwheel && g.Slot != ItemSlot.Hydraulic &&
                                                !ArrayContains(GemItems, gg => gg != null && (gg.Id == g.Id || (gg.Stats >= g.Stats && !gg.IsLimitedGem)))))
                                            {
                                                // gem is not available
                                                var availability = character.GetItemAvailability(g);
                                                if (availability != ItemAvailability.Available && availability != ItemAvailability.RegemmingAllowed) // i think only regemming allowed can happen, but ok
                                                {
                                                    character.ToggleItemAvailability(g, true);
                                                }
                                            }
                                        }
                                    }
                                    Enchant enchant = item.Enchant;
                                    if (enchant != null && enchant.Id != 0)
                                    {
                                        // ignore if we have something strictly better marked
                                        if (!ArrayContains(SlotEnchants[slot], e => e.Id == enchant.Id || e.Stats >= enchant.Stats))
                                        {
                                            // enchant is not available
                                            if (character.GetItemAvailability(item.Enchant) != ItemAvailability.Available)
                                            {
                                                character.ToggleItemAvailability(item.Enchant);
                                            }
                                        }
                                    }
                                    Tinkering tinkering = item.Tinkering;
                                    if (tinkering != null && tinkering.Id != 0)
                                    {
                                        // ignore if we have something strictly better marked
                                        if (!ArrayContains(SlotTinkerings[slot], t => t.Id == tinkering.Id || t.Stats >= tinkering.Stats))
                                        {
                                            // enchant is not available
                                            if (character.GetItemAvailability(item.Tinkering) != ItemAvailability.Available)
                                            {
                                                character.ToggleItemAvailability(item.Tinkering);
                                            }
                                        }
                                    }
                                    break;
                                case ItemAvailability.RegemmingAllowedWithEnchantRestrictions:
                                    // all gemmings are available so the ones that are available must not be sufficient
                                    for (int gem = 1; gem <= 3; gem++)
                                    {
                                        Item g = item.GetGem(gem);
                                        if (g != null)
                                        {
                                            // ignore if we have something strictly better marked
                                            if ((g.Slot == ItemSlot.Meta && !ArrayContains(MetaGemItems, gg => gg.Stats >= g.Stats)) ||
                                                (g.Slot != ItemSlot.Meta && !ArrayContains(GemItems, gg => gg.Id == g.Id || (gg.Stats >= g.Stats && !gg.IsLimitedGem))))
                                            {
                                                // gem is not available
                                                if (character.GetItemAvailability(g) != ItemAvailability.Available)
                                                {
                                                    character.ToggleItemAvailability(g, true);
                                                }
                                            }
                                        }
                                    }
                                    if (item.EnchantId != 0 && !character.AvailableItems.Contains(item.Id + ".*.*.*." + item.EnchantId))
                                    {
                                        // this specific enchant is not valid                                        
                                        if (character.GetItemAvailability(item.Enchant) != ItemAvailability.Available)
                                        {
                                            character.ToggleItemAvailability(item.Enchant);
                                        }
                                    }
                                    break;
                                case ItemAvailability.NotAvailable:
                                    // they could have some other gemming/enchant marked as available, but not in general
                                    if (character.GetItemAvailability(item.Item) != ItemAvailability.RegemmingAllowed)
                                    {
                                        character.ToggleItemAvailability(item.Item, true);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        // if it doesn't have availability information it means it wasn't available in any form
                        if (character.GetItemAvailability(item.Item) != ItemAvailability.RegemmingAllowed)
                        {
                            character.ToggleItemAvailability(item.Item, true);
                        }
                    }
                }
            }
        }

        public void AddItemRestrictions(ItemInstance[] items, bool includeOffHand)
        {
            for (int slot = 0; slot < items.Length; slot++)
            {
                if (slot != (int)CharacterSlot.OffHand || includeOffHand)
                {
                    AddItemRestriction(items[slot]);
                }
            }
        }


        public void AddItemRestrictions(Character character)
        {
            for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
            {
                if (slot != (int)CharacterSlot.OffHand || character.CurrentCalculations.IncludeOffHandInCalculations(character))
                {
                    AddItemRestriction(character._item[slot]);
                }
            }
        }

        public void AddItemRestriction(ItemInstance item)
        {
            if (item == null || item.Item == null) return;
            ItemAvailabilityInformation iai = item.ItemAvailabilityInformation;
            if (iai == null) return;
            // make all other gemmings/enchantings of this item unavailable
            iai.ItemList.Clear();
            iai.ItemList.Add(item);
            DirectUpgradeEntry singleEntry = null;
            if (generateDirectUpgrades)
            {
                iai.MatchingDirectUpgradeList.Clear();
                iai.NonMatchingDirectUpgradeList.Clear();
                iai.SingleDirectUpgradeList.Clear();
                singleEntry = new DirectUpgradeEntry() { ItemInstance = item };
                iai.SingleDirectUpgradeList.Add(singleEntry);
                iai.GenerativeEnchants.Clear();
                iai.GenerativeTinkerings.Clear();
            }
            // we know that itemAvailable has to contain exactly the values from
            // item.AvailabilityInformation.ItemAvailable
            // be careful if we have multiple of the same item not to remove too much
            foreach (string key in iai.ItemAvailable.Keys)
            {
                if (key != item.GemmedId)
                {
                    bool alternativeAvailable = false;
                    foreach (var iai2 in item.Item.AvailabilityInformation)
                    {
                        if (iai2 != iai && iai2.ItemAvailable.TryGetValue(key, out alternativeAvailable))
                        {
                            if (alternativeAvailable)
                            {
                                break;
                            }
                        }
                    }
                    if (!alternativeAvailable)
                    {
                        itemAvailable.Remove(key);
                    }
                }
            }
            iai.ItemAvailable.Clear();
            iai.ItemAvailable[item.GemmedId] = true;
            /*foreach (int slot in iai.ValidSlots)
            {
                slotItems[slot].RemoveAll(i => i != null && i.Id == item.Id && i.RandomSuffixId == item.RandomSuffixId && i.GemmedId != item.GemmedId);
                // it's possible that we're restricting to non-standard gemming of the item, in that case we have to add it in
                if (!slotItems[slot].Contains(item))
                {
                    slotItems[slot].Add(item);
                }
                if (generateDirectUpgrades)
                {
                    slotDirectUpgrades[slot][0].RemoveAll(i => i.ItemInstance.Id == item.Id && i.ItemInstance.RandomSuffixId == item.RandomSuffixId);
                    if (slotDirectUpgrades[slot].Contains(iai.MatchingDirectUpgradeList))
                    {
                        slotDirectUpgrades[slot][0].Add(singleEntry);
                    }
                }
            }*/
        }

        // saved availability data
        //List<ItemInstance>[] savedSlotItems;
        //List<DirectUpgradeEntry>[] savedSingleDirectUpgrades;
        Dictionary<string, bool> savedItemAvailable;
        List<ItemAvailabilityInformation> savedAvailabilityInformation;

        public void SaveAvailabilityInformation()
        {
            /*savedSlotItems = new List<ItemInstance>[slotCount];
            if (generateDirectUpgrades)
            {
                savedSingleDirectUpgrades = new List<DirectUpgradeEntry>[slotCount];
            }
            for (int slot = 0; slot < slotItems.Length; slot++)
            {
                savedSlotItems[slot] = new List<ItemInstance>(slotItems[slot]);
                if (generateDirectUpgrades)
                {
                    savedSingleDirectUpgrades[slot] = new List<DirectUpgradeEntry>(slotDirectUpgrades[slot][0]);
                }
            }*/
            savedItemAvailable = new Dictionary<string, bool>(itemAvailable);
            savedAvailabilityInformation = new List<ItemAvailabilityInformation>();

            foreach (var citemAvailabilityInformation in rawItems)
            {
                ItemAvailabilityInformation iai = new ItemAvailabilityInformation();
                iai.ItemAvailable = new Dictionary<string, bool>(citemAvailabilityInformation.ItemAvailable);
                iai.ItemList = new List<ItemInstance>(citemAvailabilityInformation.ItemList);
                if (generateDirectUpgrades)
                {
                    iai.MatchingDirectUpgradeList = new List<DirectUpgradeEntry>(citemAvailabilityInformation.MatchingDirectUpgradeList);
                    iai.NonMatchingDirectUpgradeList = new List<DirectUpgradeEntry>(citemAvailabilityInformation.NonMatchingDirectUpgradeList);
                    iai.SingleDirectUpgradeList = new List<DirectUpgradeEntry>(citemAvailabilityInformation.SingleDirectUpgradeList);
                    iai.GenerativeEnchants = new List<Enchant>(citemAvailabilityInformation.GenerativeEnchants);
                    iai.GenerativeTinkerings = new List<Tinkering>(citemAvailabilityInformation.GenerativeTinkerings);
                }
                savedAvailabilityInformation.Add(iai);
            }
        }

        /// <summary>
        /// Restores availability information that was saved previously. If the generator was used
        /// in an optimizer it is necessary to reinitialize the optimizer item cache
        /// </summary>
        public void RestoreAvailabilityInformation()
        {
            /*for (int slot = 0; slot < slotItems.Length; slot++)
            {
                slotItems[slot].Clear();
                slotItems[slot].AddRange(savedSlotItems[slot]);
                if (generateDirectUpgrades)
                {
                    slotDirectUpgrades[slot][0].Clear();
                    slotDirectUpgrades[slot][0].AddRange(savedSingleDirectUpgrades[slot]);
                }
            }*/
            itemAvailable = new Dictionary<string, bool>(savedItemAvailable);
            for (int i = 0; i < savedAvailabilityInformation.Count; i++)
            {
                var kvpValue = savedAvailabilityInformation[i];
                var iai = rawItems[i];
                iai.ItemAvailable = new Dictionary<string, bool>(kvpValue.ItemAvailable);
                iai.ItemList.Clear();
                iai.ItemList.AddRange(kvpValue.ItemList);
                if (generateDirectUpgrades)
                {
                    iai.MatchingDirectUpgradeList.Clear();
                    iai.MatchingDirectUpgradeList.AddRange(kvpValue.MatchingDirectUpgradeList);
                    iai.NonMatchingDirectUpgradeList.Clear();
                    iai.NonMatchingDirectUpgradeList.AddRange(kvpValue.NonMatchingDirectUpgradeList);
                    iai.SingleDirectUpgradeList.Clear();
                    iai.SingleDirectUpgradeList.AddRange(kvpValue.SingleDirectUpgradeList);
                    iai.GenerativeEnchants.Clear();
                    iai.GenerativeEnchants.AddRange(kvpValue.GenerativeEnchants);
                    iai.GenerativeTinkerings.Clear();
                    iai.GenerativeTinkerings.AddRange(kvpValue.GenerativeTinkerings);
                }
            }
        }

        /// <summary>
        /// Replaces unavailable items with available ones
        /// </summary>
        public void RegularizeCharacter(Character character)
        {
            // if item is not available pick the one that is available
            for (int slot = 0; slot < Character.OptimizableSlotCount; slot++)
            {
                ItemInstance item = character._item[slot];
                if (item != null)
                {
                    var availabilityInformation = item.ItemAvailabilityInformation;
                    if (availabilityInformation != null && !itemAvailable.ContainsKey(item.GemmedId))
                    {
                        character._item[slot] = availabilityInformation.ItemList[0];
                    }
                }
            }
        }

        public AvailableItemGenerator(List<string> availableItems, bool generateDirectUpgrades, bool templateGemsEnabled, bool overrideRegem, bool overrideReenchant, bool overrideReforge, bool slotFiltering, Character character, CalculationsBase model) : this(availableItems, generateDirectUpgrades, templateGemsEnabled, overrideRegem, overrideReenchant, overrideReforge, slotFiltering, false, new Character[] { character }, new CalculationsBase[] { model }) { }
        public AvailableItemGenerator(List<string> availableItems, bool generateDirectUpgrades, bool templateGemsEnabled, bool overrideRegem, bool overrideReenchant, bool overrideReforge, bool slotFiltering, Character[] characters, CalculationsBase[] models) : this(availableItems, generateDirectUpgrades, templateGemsEnabled, overrideRegem, overrideReenchant, overrideReforge, slotFiltering, false, characters, models) { }
        public AvailableItemGenerator(List<string> availableItems, bool generateDirectUpgrades, bool templateGemsEnabled, bool overrideRegem, bool overrideReenchant, bool overrideReforge, bool slotFiltering, bool positiveCostItemsAvailable, Character character, CalculationsBase model) : this(availableItems, generateDirectUpgrades, templateGemsEnabled, overrideRegem, overrideReenchant, overrideReforge, slotFiltering, positiveCostItemsAvailable, new Character[] { character }, new CalculationsBase[] { model }) { }

        public AvailableItemGenerator(List<string> availableItems, bool generateDirectUpgrades, bool templateGemsEnabled, bool overrideRegem, bool overrideReenchant, bool overrideReforge, bool slotFiltering, bool positiveCostItemsAvailable, Character[] characters, CalculationsBase[] models)
        {
            this.availableItems = availableItems;
            if (templateGemsEnabled)
            {
                this.availableItems = new List<string>(availableItems);
                List<string> templateGems = new List<string>();
                // this could actually be empty, but in practice they will populate it at least once before
                // however as a sanity check if it is null fetch the template from the model
                for (int index = 0; index < characters.Length; index++)
                {
                    foreach (GemmingTemplate template in characters[index].CurrentGemmingTemplates)
                    {
                        if (template.Enabled)
                        {
                            if (!templateGems.Contains(template.RedId.ToString())) templateGems.Add(template.RedId.ToString());
                            if (!templateGems.Contains(template.YellowId.ToString())) templateGems.Add(template.YellowId.ToString());
                            if (!templateGems.Contains(template.BlueId.ToString())) templateGems.Add(template.BlueId.ToString());
                            if (!templateGems.Contains(template.PrismaticId.ToString())) templateGems.Add(template.PrismaticId.ToString());
                            if (!templateGems.Contains(template.MetaId.ToString())) templateGems.Add(template.MetaId.ToString());
                            if (!templateGems.Contains(template.CogwheelId.ToString())) templateGems.Add(template.CogwheelId.ToString());
                            if (!templateGems.Contains(template.Cogwheel2Id.ToString())) templateGems.Add(template.Cogwheel2Id.ToString());
                            if (!templateGems.Contains(template.HydraulicId.ToString())) templateGems.Add(template.HydraulicId.ToString());
                        }
                    }
                    foreach (GemmingTemplate template in characters[index].CustomGemmingTemplates)
                    {
                        if (template.Enabled && template.Model == models[index].Name)
                        {
                            if (!templateGems.Contains(template.RedId.ToString())) templateGems.Add(template.RedId.ToString());
                            if (!templateGems.Contains(template.YellowId.ToString())) templateGems.Add(template.YellowId.ToString());
                            if (!templateGems.Contains(template.BlueId.ToString())) templateGems.Add(template.BlueId.ToString());
                            if (!templateGems.Contains(template.PrismaticId.ToString())) templateGems.Add(template.PrismaticId.ToString());
                            if (!templateGems.Contains(template.MetaId.ToString())) templateGems.Add(template.MetaId.ToString());
                            if (!templateGems.Contains(template.CogwheelId.ToString())) templateGems.Add(template.CogwheelId.ToString());
                            if (!templateGems.Contains(template.Cogwheel2Id.ToString())) templateGems.Add(template.Cogwheel2Id.ToString());
                            if (!templateGems.Contains(template.HydraulicId.ToString())) templateGems.Add(template.HydraulicId.ToString());
                        }
                    }
                }
                foreach (string gem in templateGems)
                {
                    if (!this.availableItems.Contains(gem)) this.availableItems.Add(gem);
                }
            }            
            this.overrideRegem = overrideRegem;
            this.overrideReenchant = overrideReenchant;
            this.slotFiltering = slotFiltering;
            this.characters = characters;
            this.models = models;
            this.generateDirectUpgrades = generateDirectUpgrades;
            this.overrideReforge = overrideReforge;

            bool oldVolatility = Item.OptimizerManagedVolatiliy;
            try
            {
                Item.OptimizerManagedVolatiliy = true;
                PopulateAvailableIds(positiveCostItemsAvailable);
            }
            finally
            {
                Item.OptimizerManagedVolatiliy = oldVolatility;
            }
        }

        public int GetItemGemCount(Item item)
        {
            int gemCount = 0;
            bool blacksmithingSocket = (item.Slot == ItemSlot.Waist && characters[0].WaistBlacksmithingSocketEnabled)
                                    || (item.Slot == ItemSlot.Hands && characters[0].HandsBlacksmithingSocketEnabled)
                                    || (item.Slot == ItemSlot.Wrist && characters[0].WristBlacksmithingSocketEnabled);
            switch (item.SocketColor1)
            {
                case ItemSlot.Meta:
                case ItemSlot.Red:
                case ItemSlot.Orange:
                case ItemSlot.Yellow:
                case ItemSlot.Green:
                case ItemSlot.Blue:
                case ItemSlot.Purple:
                case ItemSlot.Prismatic:
                case ItemSlot.Cogwheel:
                case ItemSlot.Hydraulic:
                    gemCount++;
                    break;
                default:
                    if (blacksmithingSocket) {
                        gemCount++;
                        blacksmithingSocket = false;
                    }
                    break;
            }
            switch (item.SocketColor2)
            {
                case ItemSlot.Meta:
                case ItemSlot.Red:
                case ItemSlot.Orange:
                case ItemSlot.Yellow:
                case ItemSlot.Green:
                case ItemSlot.Blue:
                case ItemSlot.Purple:
                case ItemSlot.Prismatic:
                case ItemSlot.Cogwheel:
                case ItemSlot.Hydraulic:
                    gemCount++;
                    break;
                default:
                    if (blacksmithingSocket) {
                        gemCount++;
                        blacksmithingSocket = false;
                    }
                    break;
            }
            switch (item.SocketColor3)
            {
                case ItemSlot.Meta:
                case ItemSlot.Red:
                case ItemSlot.Orange:
                case ItemSlot.Yellow:
                case ItemSlot.Green:
                case ItemSlot.Blue:
                case ItemSlot.Purple:
                case ItemSlot.Prismatic:
                case ItemSlot.Cogwheel:
                case ItemSlot.Hydraulic:
                    gemCount++;
                    break;
                default:
                    if (blacksmithingSocket) {
                        gemCount++;
                        blacksmithingSocket = false;
                    }
                    break;
            }
            return gemCount;
        }

        public ItemAvailabilityInformation GenerateItemAvailabilityInformation(Item item, int randomSuffixId)
        {
            var itemAvailabilityInformation = new ItemAvailabilityInformation();
            itemAvailabilityInformation.Item = new SuffixItem() { Item = item, RandomSuffixId = randomSuffixId };
            itemAvailabilityInformation.GemCount = GetItemGemCount(item);
            if (generateDirectUpgrades)
            {
                itemAvailabilityInformation.MatchingDirectUpgradeList = new List<DirectUpgradeEntry>();
                itemAvailabilityInformation.MatchingMap = new Dictionary<string, DirectUpgradeEntry>();
                itemAvailabilityInformation.NonMatchingDirectUpgradeList = new List<DirectUpgradeEntry>();
                itemAvailabilityInformation.NonMatchingMap = new Dictionary<string, DirectUpgradeEntry>();
                itemAvailabilityInformation.SingleDirectUpgradeList = new List<DirectUpgradeEntry>();
                itemAvailabilityInformation.GenerativeEnchants = new List<Enchant>();
                itemAvailabilityInformation.GenerativeTinkerings = new List<Tinkering>();

                itemAvailabilityInformation.DirectUpgradeList = new List<List<DirectUpgradeEntry>>();
                itemAvailabilityInformation.DirectUpgradeList.Add(itemAvailabilityInformation.SingleDirectUpgradeList);
                itemAvailabilityInformation.DirectUpgradeList.Add(itemAvailabilityInformation.MatchingDirectUpgradeList);
                itemAvailabilityInformation.DirectUpgradeList.Add(itemAvailabilityInformation.NonMatchingDirectUpgradeList);
            }
            return itemAvailabilityInformation;
        }

        private void PopulateAvailableIds(bool positiveCostItemsAvailable)
        {
            lock (ItemCache.Items)
            {
                foreach (Item citem in ItemCache.Items.Values)
                {
                    citem.AvailabilityInformation = null;
                }
            }

            List<string> itemIds = new List<string>(availableItems);
            List<string> removeIds = new List<string>();
            List<Item> metaGemItemList = new List<Item>();
            List<Item> gemItemList = new List<Item>();
            List<Item> cogwheelItemList = new List<Item>();
            List<Item> hydraulicItemList = new List<Item>();
            Dictionary<string, bool> validSocket = new Dictionary<string, bool>();
            foreach (string xid in availableItems)
            {
                int dot = xid.IndexOf('.');
                int id = int.Parse((dot >= 0) ? xid.Substring(0, dot) : xid, System.Globalization.CultureInfo.InvariantCulture);
                if (id > 0)
                {
                    Item availableItem;
                    availableItem = ItemCache.FindItemById(id);
                    if (availableItem != null)
                    {
                        switch (availableItem.Slot)
                        {
                            case ItemSlot.Meta:
                                metaGemItemList.Add(availableItem);
                                validSocket[xid] = true;
                                removeIds.Add(xid);
                                break;
                            case ItemSlot.Red:
                            case ItemSlot.Orange:
                            case ItemSlot.Yellow:
                            case ItemSlot.Green:
                            case ItemSlot.Blue:
                            case ItemSlot.Purple:
                            case ItemSlot.Prismatic:
                                gemItemList.Add(availableItem);
                                validSocket[xid] = true;
                                removeIds.Add(xid);
                                break;
                            case ItemSlot.Cogwheel:
                                cogwheelItemList.Add(availableItem);
                                validSocket[xid] = true;
                                removeIds.Add(xid);
                                break;
                            case ItemSlot.Hydraulic:
                                hydraulicItemList.Add(availableItem);
                                validSocket[xid] = true;
                                removeIds.Add(xid);
                                break;
                        }
                    }
                }
            }
            if (gemItemList.Count == 0) gemItemList.Add(null);
            if (metaGemItemList.Count == 0) metaGemItemList.Add(null);
            if (cogwheelItemList.Count == 0) cogwheelItemList.Add(null);
            if (hydraulicItemList.Count == 0) hydraulicItemList.Add(null);
            itemIds.RemoveAll(x => x.StartsWith("-", StringComparison.Ordinal) || removeIds.Contains(x));

            metaGemItems = metaGemItemList.ToArray();
            cogwheelItems = cogwheelItemList.ToArray();
            hydraulicItems = hydraulicItemList.ToArray();
            gemItems = FilterList(gemItemList);

            for (int i = 0; i < slotCount; i++)
            {
                //slotItems[i] = new List<ItemInstance>();
                slotRawItems[i] = new List<ItemAvailabilityInformation>();
            }

            slotAvailableEnchants[(int)CharacterSlot.Back] = FilterList(Enchant.FindEnchants(ItemSlot.Back, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Chest] = FilterList(Enchant.FindEnchants(ItemSlot.Chest, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Feet] = FilterList(Enchant.FindEnchants(ItemSlot.Feet, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Finger1] = slotAvailableEnchants[(int)CharacterSlot.Finger2] = FilterList(Enchant.FindEnchants(ItemSlot.Finger, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Hands] = FilterList(Enchant.FindEnchants(ItemSlot.Hands, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Head] = FilterList(Enchant.FindEnchants(ItemSlot.Head, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Legs] = FilterList(Enchant.FindEnchants(ItemSlot.Legs, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Shoulders] = FilterList(Enchant.FindEnchants(ItemSlot.Shoulders, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.MainHand] = FilterList(Enchant.FindEnchants(ItemSlot.MainHand, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.OffHand] = FilterList(Enchant.FindEnchants(ItemSlot.OffHand, characters, availableItems, models));
            //slotAvailableEnchants[(int)CharacterSlot.Ranged] = FilterList(Enchant.FindEnchants(ItemSlot.Ranged, characters, availableItems, models));
            slotAvailableEnchants[(int)CharacterSlot.Wrist] = FilterList(Enchant.FindEnchants(ItemSlot.Wrist, characters, availableItems, models));

            slotAvailableTinkerings[(int)CharacterSlot.Back] = FilterList(Tinkering.FindTinkerings(ItemSlot.Back, characters, availableItems, models));
            slotAvailableTinkerings[(int)CharacterSlot.Hands] = FilterList(Tinkering.FindTinkerings(ItemSlot.Hands, characters, availableItems, models));
            slotAvailableTinkerings[(int)CharacterSlot.Waist] = FilterList(Tinkering.FindTinkerings(ItemSlot.Waist, characters, availableItems, models));

            Item item = null;
            List<Dictionary<string, bool>> gemmedIdList = new List<Dictionary<string, bool>>();
            string[] ids = new string[8];
            string[] idsCopy = new string[8];
            int regemMax = overrideRegem ? 1 : 0;
            int reenchantMax = overrideReenchant ? 1 : 0;
            int reforgeMax = overrideReforge ? 1 : 0;
            int retinkeringMax = overrideReenchant ? 1 : 0;
            foreach (string xid in itemIds)
            {
                string[] idsRaw = xid.Split('.');
                int i = 0;
                for (; i < idsRaw.Length; i++)
                {
                    ids[i] = idsRaw[i];
                }
                for (; i < ids.Length; i++)
                {
                    if (i == 1)
                    {
                        ids[i] = "0";
                    }
                    else
                    {
                        ids[i] = "*";
                    }
                }
                int regemPlus = 0;
                if (overrideRegem)
                {
                    if (!validSocket.ContainsKey(ids[2]) || !validSocket.ContainsKey(ids[3]) || !validSocket.ContainsKey(ids[4]))
                    {
                        regemPlus = 1;
                    }
                }
                string id = ids[0] + "." + ids[1];
                Dictionary<string, bool> map = new Dictionary<string, bool>();
                gemmedIdList.Add(map);
                for (int regem = 0; regem <= regemMax + regemPlus; regem++)
                {
                    for (int reenchant = 0; reenchant <= reenchantMax; reenchant++)
                    {
                        for (int reforge = 0; reforge <= reforgeMax; reforge++)
                        {
                            for (int retinkering = 0; retinkering <= retinkeringMax; retinkering++)
                            {
                                Array.Copy(ids, idsCopy, 8);
                                if (regem == 1)
                                {
                                    idsCopy[2] = "*";
                                    idsCopy[3] = "*";
                                    idsCopy[4] = "*";
                                }
                                else if (regem == 2)
                                {
                                    if (validSocket.ContainsKey(ids[2])) idsCopy[2] = "*";
                                    if (validSocket.ContainsKey(ids[3])) idsCopy[3] = "*";
                                    if (validSocket.ContainsKey(ids[4])) idsCopy[4] = "*";
                                }
                                if (reenchant == 1)
                                {
                                    idsCopy[5] = "*";
                                }
                                if (reforge == 1)
                                {
                                    idsCopy[6] = "*";
                                }
                                if (retinkering == 1)
                                {
                                    idsCopy[7] = "*";
                                }
                                map[string.Join(".", idsCopy)] = true;
                            }
                        }
                    }
                }
            }
            /*if (generateDirectUpgrades)
            {
                for (int i = 0; i < slotCount; i++)
                {
                    slotDirectUpgrades[i] = new List<List<DirectUpgradeEntry>>();
                    slotDirectUpgrades[i].Add(new List<DirectUpgradeEntry>()); // add a list for all singles
                }
            }*/
            if (positiveCostItemsAvailable)
            {
                // add items that have positive cost and are not available yet in any form
                // but DONT add the item if it requires a different class
                lock (ItemCache.Items)
                {
                    foreach (Item citem in ItemCache.Items.Values)
                    {
                        if (citem.Cost > 0.0f && (citem.RequiredClasses == null || (citem.RequiredClasses != null && citem.RequiredClasses.Contains(characters[0].Class.ToString()))))
                        {
                            if (citem.AllowedRandomSuffixes == null || citem.AllowedRandomSuffixes.Count == 0)
                            {
                                string key = citem.Id.ToString() + ".0";
                                Dictionary<string, bool> map = new Dictionary<string, bool>();
                                gemmedIdList.Add(map);
                                map["C" + key + ".*.*.*.*.*.*"] = true;
                            }
                            else
                            {
                                foreach (var suffix in citem.AllowedRandomSuffixes)
                                {
                                    string key = citem.Id.ToString() + "." + suffix;
                                    Dictionary<string, bool> map = new Dictionary<string, bool>();
                                    gemmedIdList.Add(map);
                                    map["C" + key + ".*.*.*.*.*.*"] = true;
                                }
                            }
                        }
                    }
                }
            }
            foreach (Dictionary<string, bool> keyMap in gemmedIdList)
            {
                var tmp = keyMap.First().Key.TrimStart('C').Split('.');
                int itemId = int.Parse(tmp[0], System.Globalization.CultureInfo.InvariantCulture);
                item = ItemCache.FindItemById(itemId);
                int randomSuffixId = tmp.Length < 2 ? 0 : int.Parse(tmp[1], System.Globalization.CultureInfo.InvariantCulture);

                // make sure that any random suffix id is actually available
                // this most often happens when item cache doesn't have accurate data
                if (randomSuffixId != 0)
                {
                    if (item.AllowedRandomSuffixes == null)
                    {
                        item.AllowedRandomSuffixes = new List<int>();
                    }
                    if (!item.AllowedRandomSuffixes.Contains(randomSuffixId))
                    {
                        item.AllowedRandomSuffixes.Add(randomSuffixId);
                    }
                }

                // disallow non-equippable items, this can happen for example when loading from character profiler

                bool isRelevant = false;
                foreach (CalculationsBase model in models)
                {
                    // have to use the relevant types from the model, so we get types that can be equipped at all
                    //if (item != null && ItemFilter.GetRelevantItemTypesList(model).Contains(item.Type))
                    if (item != null)
                    {
                        if (model.RelevantItemTypes.Contains(item.Type))
                        {
                            isRelevant = true;
                            break;
                        }
                        // workaround for some models not correctly reporting what can be equipped
                        if (item.Type >= ItemType.Cloth && item.Type <= ItemType.Plate)
                        {
                            // find highest equippable type and assume all lower can be also
                            ItemType max = ItemType.None;
                            foreach (var t in model.RelevantItemTypes)
                            {
                                if (t <= ItemType.Plate && t > max)
                                {
                                    max = t;
                                }
                            }
                            if (item.Type <= max)
                            {
                                isRelevant = true;
                                break;
                            }
                        }
                    }
                }

                if (isRelevant)
                {
                    int slot = (int)Character.GetCharacterSlotByItemSlot(item.Slot);
                    if (slot < 0 || slot >= slotCount) continue;

                    var availabilityInformation = GenerateItemAvailabilityInformation(item, randomSuffixId);

                    if (item.AvailabilityInformation == null)
                    {
                        item.AvailabilityInformation = new List<ItemAvailabilityInformation>();
                    }
                    item.AvailabilityInformation.Add(availabilityInformation);
                    rawItems.Add(availabilityInformation);

                    var gemmedIds = new List<string>(keyMap.Keys);
                    Dictionary<string, bool> uniqueStore = new Dictionary<string, bool>();
                    var possibleGemmedItems = new List<ItemInstance>();
                    foreach (string gid in gemmedIds)
                    {
                        if (gid.StartsWith("C", StringComparison.Ordinal))
                        {
                            availabilityInformation.PositiveCostItem = true;
                        }
                        string ggid = gid.TrimStart('C');
                        string[] idTokens = ggid.Split('.');
                        bool blueDiamond = (idTokens.Length > 2 && idTokens[2] != "*");
                        foreach (ItemInstance gemmedItem in GetPossibleGemmedItemsForItem(item, randomSuffixId, ggid, availabilityInformation))
                        {
                            if (availabilityInformation.DefaultItemInstance == null && blueDiamond)
                            {
                                availabilityInformation.DefaultItemInstance = gemmedItem;
                            }
                            if (!uniqueStore.ContainsKey(gemmedItem.GemmedId))
                            {
                                possibleGemmedItems.Add(gemmedItem);
                                uniqueStore.Add(gemmedItem.GemmedId, true);
                            }
                        }
                    }
                    foreach (KeyValuePair<string, bool> kvp in availabilityInformation.ItemAvailable)
                    {
                        itemAvailable[kvp.Key] = kvp.Value;
                    }
                    possibleGemmedItems = FilterList(possibleGemmedItems, false);
                    availabilityInformation.ItemList = possibleGemmedItems;
                    List<int> slotList = null;
                    if (availabilityInformation.ValidSlots == null)
                    {
                        slotList = new List<int>();
                    }
                    for (int i = 0; i < slotCount; i++)
                    {
                        bool fits = false;
                        foreach (Character character in characters)
                        {
                            if (item.FitsInSlot((CharacterSlot)i, character, true))
                            {
                                fits = true;
                                break;
                            }
                        }
                        if (fits)
                        {
                            if (slotList != null)
                            {
                                slotList.Add(i);
                            }
                            //slotItems[i].AddRange(possibleGemmedItems);
                            slotRawItems[i].Add(availabilityInformation);
                            /*if (generateDirectUpgrades)
                            {
                                slotDirectUpgrades[i][0].AddRange(availabilityInformation.SingleDirectUpgradeList);
                                slotDirectUpgrades[i].Add(availabilityInformation.MatchingDirectUpgradeList);
                                slotDirectUpgrades[i].Add(availabilityInformation.NonMatchingDirectUpgradeList);
                            }*/
                        }
                    }
                    if (slotList != null)
                    {
                        availabilityInformation.ValidSlots = slotList.ToArray();
                    }
                }
            }

            /*for (int i = 0; i < slotCount; i++)
            {
                CharacterSlot slot = (CharacterSlot)i;
                if (slot == CharacterSlot.Finger1 || slot == CharacterSlot.Finger2 || slot == CharacterSlot.Trinket1 || slot == CharacterSlot.Trinket2 || slot == CharacterSlot.MainHand || slot == CharacterSlot.OffHand || slotItems[i].Count == 0)
                {
                    slotItems[i].Add(null);
                }
            }*/

            /*if (slotFiltering)
            {
                for (int i = 0; i < slotCount; i++)
                {
                    CharacterSlot slot = (CharacterSlot)i;
                    if (slot != CharacterSlot.Finger1 && slot != CharacterSlot.Finger2 && slot != CharacterSlot.Trinket1 && slot != CharacterSlot.Trinket2)
                    {
                        slotItems[i] = FilterList(slotItems[i], true);
                    }
                }
            }*/

            // mark currently equipped items as default if they're not overriden by blue diamond
            foreach (Character character in characters)
            {
                for (int i = 0; i < slotCount; i++)
                {
                    ItemInstance itemInstance = character._item[i];
                    if (itemInstance != null)
                    {
                        // make sure suffix is available if this is random suffix item
                        if (itemInstance.RandomSuffixId != 0)
                        {
                            if (itemInstance.Item.AllowedRandomSuffixes == null)
                            {
                                itemInstance.Item.AllowedRandomSuffixes = new List<int>();
                            }
                            if (!itemInstance.Item.AllowedRandomSuffixes.Contains(itemInstance.RandomSuffixId))
                            {
                                itemInstance.Item.AllowedRandomSuffixes.Add(itemInstance.RandomSuffixId);
                            }
                        }
                        if (itemInstance.Item.AvailabilityInformation != null)
                        {
                            foreach (var availabilityInformation in itemInstance.Item.AvailabilityInformation)
                            {
                                if (availabilityInformation.Item.RandomSuffixId == itemInstance.RandomSuffixId && availabilityInformation.DefaultItemInstance == null)
                                {
                                    availabilityInformation.DefaultItemInstance = itemInstance.Clone();
                                    availabilityInformation.DefaultItemInstance.ItemAvailabilityInformation = availabilityInformation;
                                }
                            }
                        }
                    }
                }
            }
        }

        public List<ItemInstance> GetPossibleGemmedItemsForItem(Item item, int randomSuffixId, string gemmedId)
        {
            return GetPossibleGemmedItemsForItem(item, randomSuffixId, gemmedId, null);
        }

        public List<ItemInstance> GetPossibleGemmedItemsForItem(Item item, int randomSuffixId, string gemmedId, ItemAvailabilityInformation availability)
        {
            List<ItemInstance> possibleGemmedItems = new List<ItemInstance>();
            string[] ids = gemmedId.Split('.');
            Item[] possibleGem1s, possibleGem2s, possibleGem3s = null;
            Enchant[] possibleEnchants = null;
            Reforging[] possibleReforgings = null;
            Tinkering[] possibleTinkerings = null;
            bool blacksmithingSocket = (item.Slot == ItemSlot.Waist && characters[0].WaistBlacksmithingSocketEnabled)
                                    || (item.Slot == ItemSlot.Hands && characters[0].HandsBlacksmithingSocketEnabled)
                                    || (item.Slot == ItemSlot.Wrist && characters[0].WristBlacksmithingSocketEnabled);
            int[] possibleUpgradeLevels = new int[item.UpgradeLevels.Count + 1];
            for (int i = 0; i < possibleUpgradeLevels.Length; ++i)
            {
                if (i == 0) possibleUpgradeLevels[i] = 0;
                else possibleUpgradeLevels[i] = item.UpgradeLevels[i - 1];
            }
            if (ids.Length <= 2 || (ids.Length > 2 && ids[2] == "*"))
            {
                switch (item.SocketColor1)
                {
                    case ItemSlot.Meta:
                        possibleGem1s = metaGemItems;
                        break;
                    case ItemSlot.Red:
                    case ItemSlot.Orange:
                    case ItemSlot.Yellow:
                    case ItemSlot.Green:
                    case ItemSlot.Blue:
                    case ItemSlot.Purple:
                    case ItemSlot.Prismatic:
                        possibleGem1s = gemItems;
                        break;
                    case ItemSlot.Cogwheel:
                        possibleGem1s = cogwheelItems;
                        break;
                    case ItemSlot.Hydraulic:
                        possibleGem1s = hydraulicItems;
                        break;
                    default:
                        if (blacksmithingSocket) {
                            possibleGem1s = gemItems;
                            blacksmithingSocket = false;
                        } else { possibleGem1s = new Item[] { null }; }
                        break;
                }
            }
            else
            {
                possibleGem1s = new Item[] { ItemCache.FindItemById(int.Parse(ids[2], System.Globalization.CultureInfo.InvariantCulture)) };
            }

            if (ids.Length <= 3 || (ids.Length > 3 && ids[3] == "*"))
            {
                switch (item.SocketColor2)
                {
                    case ItemSlot.Meta:
                        possibleGem2s = metaGemItems;
                        break;
                    case ItemSlot.Red:
                    case ItemSlot.Orange:
                    case ItemSlot.Yellow:
                    case ItemSlot.Green:
                    case ItemSlot.Blue:
                    case ItemSlot.Purple:
                    case ItemSlot.Prismatic:
                        possibleGem2s = gemItems;
                        break;
                    case ItemSlot.Cogwheel:
                        possibleGem2s = cogwheelItems;
                        break;
                    case ItemSlot.Hydraulic:
                        possibleGem2s = hydraulicItems;
                        break;
                    default:
                        if (blacksmithingSocket) {
                            possibleGem2s = gemItems;
                            blacksmithingSocket = false;
                        } else { possibleGem2s = new Item[] { null }; }
                        break;
                }
            }
            else
            {
                possibleGem2s = new Item[] { ItemCache.FindItemById(int.Parse(ids[3], System.Globalization.CultureInfo.InvariantCulture)) };
            }

            if (ids.Length <= 4 || (ids.Length > 4 && ids[4] == "*"))
            {
                switch (item.SocketColor3)
                {
                    case ItemSlot.Meta:
                        possibleGem3s = metaGemItems;
                        break;
                    case ItemSlot.Red:
                    case ItemSlot.Orange:
                    case ItemSlot.Yellow:
                    case ItemSlot.Green:
                    case ItemSlot.Blue:
                    case ItemSlot.Purple:
                    case ItemSlot.Prismatic:
                        possibleGem3s = gemItems;
                        break;
                    case ItemSlot.Cogwheel:
                        possibleGem3s = cogwheelItems;
                        break;
                    case ItemSlot.Hydraulic:
                        possibleGem3s = hydraulicItems;
                        break;
                    default:
                        if (blacksmithingSocket) {
                            possibleGem3s = gemItems;
                            blacksmithingSocket = false;
                        } else { possibleGem3s = new Item[] { null }; }
                        break;
                }
            } else {
                possibleGem3s = new Item[] { ItemCache.FindItemById(int.Parse(ids[4], System.Globalization.CultureInfo.InvariantCulture)) };
            }

            if (ids.Length <= 5 || (ids.Length > 5 && ids[5] == "*")) {
                int slotIndex = (int)Character.GetCharacterSlotByItemSlot(item.Slot);
                if (slotIndex < slotAvailableEnchants.Length)
                    possibleEnchants = slotAvailableEnchants[slotIndex];
                if (possibleEnchants == null) possibleEnchants = new Enchant[] { null };
            } else {
                possibleEnchants = new Enchant[] { Enchant.FindEnchant(int.Parse(ids[5], System.Globalization.CultureInfo.InvariantCulture), item.Slot, null) };
            }

            if (ids.Length <= 6 || (ids.Length > 6 && ids[6] == "*")) 
            {
                if (models.Length > 1)
                {
                    possibleReforgings = models.SelectMany(model => model.GetReforgingOptions(item, randomSuffixId, 0)).Distinct(Reforging.IdComparer).ToArray();
                }
                else
                {
                    possibleReforgings = models[0].GetReforgingOptions(item, randomSuffixId, 0).ToArray();
                }
            } else 
            {
                possibleReforgings = new Reforging[] { new Reforging(item, randomSuffixId, 0, int.Parse(ids[6], System.Globalization.CultureInfo.InvariantCulture)) };
            }

            if (ids.Length <= 7 || (ids.Length > 7 && ids[7] == "*"))
            {
                int slotIndex = (int)Character.GetCharacterSlotByItemSlot(item.Slot);
                if (slotIndex < slotAvailableTinkerings.Length)
                    possibleTinkerings = slotAvailableTinkerings[slotIndex];
                if (possibleTinkerings == null) possibleTinkerings = new Tinkering[] { null };
            }
            else
            {
                possibleTinkerings = new Tinkering[] { Tinkering.FindTinkering(int.Parse(ids[7], System.Globalization.CultureInfo.InvariantCulture), item.Slot, null) };
            }

            bool generative = (ids.Length <= 2 || (ids.Length > 2 && ids[2] == "*")) && (ids.Length <= 3 || (ids.Length > 3 && ids[3] == "*")) && (ids.Length <= 4 || (ids.Length > 4 && ids[4] == "*"));
            if (generative && availability != null)
            {
                if (availability.GemCount == 0)
                {
                    generative = false;
                }
            }

            if (generative && availability != null && generateDirectUpgrades)
            {
                foreach (Enchant enchant in possibleEnchants)
                {
                    if (!availability.GenerativeEnchants.Contains(enchant))
                    {
                        availability.GenerativeEnchants.Add(enchant);
                    }
                }
                foreach (Tinkering tinkering in possibleTinkerings)
                {
                    if (!availability.GenerativeTinkerings.Contains(tinkering))
                    {
                        availability.GenerativeTinkerings.Add(tinkering);
                    }
                }
            }

            foreach (Item gem1 in possibleGem1s)
            {
                foreach (Item gem2 in possibleGem2s)
                {
                    // prevent combinations that break unique constraints
                    if (gem1 != null && !gem1.IsJewelersGem && !gem1.IsJewelersFacet && gem1.Unique && gem2 != null && gem1.Id == gem2.Id) continue;
                    foreach (Item gem3 in possibleGem3s)
                    {
                        if (gem3 != null && !gem3.IsJewelersGem && !gem3.IsJewelersFacet && gem3.Unique)
                        {
                            if (gem1 != null && gem1.Id == gem3.Id) continue;
                            if (gem2 != null && gem2.Id == gem3.Id) continue;
                        }
                        foreach (Enchant enchant in possibleEnchants)
                        {
                            foreach (Tinkering tinkering in possibleTinkerings)
                            {
                                foreach (Reforging reforging in possibleReforgings)
                                {
                                    foreach (int upgrade in possibleUpgradeLevels)
                                    {
                                        if (availability != null)
                                        {
                                            // any combination is actually available
                                            gemmedId = string.Format("{0}.{1}.{2}.{3}.{4}.{5}.{6}.{7}.{8}", item.Id, randomSuffixId,
                                                gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0,
                                                enchant != null ? enchant.Id : 0,
                                                reforging != null ? reforging.Id : 0,
                                                tinkering != null ? tinkering.Id : 0,
                                                upgrade
                                                );
                                            availability.ItemAvailable[gemmedId] = true;
                                        }
                                        // if gems do not match socket colors then it does not matter in what order they are placed (except for meta)
                                        // make it easy on filtering and only add one canon version of the item in which normal gem ids are nondecreasing
                                        // if that happens to be an ordering that matches colors it doesn't matter since socket bonuses only add value
                                        // either all gems are * or all are specified, obviously if all are specified we can't do this
                                        bool add = true;
                                        if (!Item.GemMatchesSlot(gem1, item.SocketColor1) || !Item.GemMatchesSlot(gem2, item.SocketColor2) || !Item.GemMatchesSlot(gem3, item.SocketColor3))
                                        {
                                            if (generative)
                                            {
                                                List<int> gemOrder = new List<int>();
                                                if (gem1 != null && gem1.Slot != ItemSlot.Meta) gemOrder.Add(gem1.Id);
                                                if (gem2 != null && gem2.Slot != ItemSlot.Meta) gemOrder.Add(gem2.Id);
                                                if (gem3 != null && gem3.Slot != ItemSlot.Meta) gemOrder.Add(gem3.Id);
                                                for (int i = 0; i < gemOrder.Count - 1; i++)
                                                {
                                                    if (gemOrder[i] > gemOrder[i + 1])
                                                    {
                                                        add = false;
                                                        break;
                                                    }
                                                }
                                                if (availability != null && generateDirectUpgrades)
                                                {
                                                    var map = availability.NonMatchingMap;
                                                    string gemId1 = string.Format("{0}.0.0.0.0.0.{1}", gem1 != null ? gem1.Id : 0, upgrade);
                                                    string gemId2 = string.Format("{0}.{1}.0.0.0.0.{2}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, upgrade);
                                                    string gemId3 = string.Format("{0}.{1}.{2}.0.0.0.{3}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, upgrade);
                                                    string gemId4 = string.Format("{0}.{1}.{2}.{3}.0.0.{4}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, enchant != null ? enchant.Id : 0, upgrade);
                                                    string gemId5 = string.Format("{0}.{1}.{2}.{3}.{4}.0.{5}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, enchant != null ? enchant.Id : 0, reforging != null ? reforging.Id : 0, upgrade);
                                                    string gemId6 = string.Format("{0}.{1}.{2}.{3}.{4}.{5}.{6}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, enchant != null ? enchant.Id : 0, reforging != null ? reforging.Id : 0, tinkering != null ? tinkering.Id : 0, upgrade);
                                                    DirectUpgradeEntry entry1, entry2, entry3, entry4, entry5, entry6;
                                                    List<DirectUpgradeEntry> list = availability.NonMatchingDirectUpgradeList;
                                                    if (gem1 != null)
                                                    {
                                                        if (!map.TryGetValue(gemId1, out entry1))
                                                        {
                                                            entry1 = new DirectUpgradeEntry();
                                                            entry1.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, null, null, null, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                            entry1.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                            map[gemId1] = entry1;
                                                            list.Add(entry1);
                                                        }
                                                        list = entry1.DirectUpgradeList;
                                                    }
                                                    if (gem2 != null)
                                                    {
                                                        if (!map.TryGetValue(gemId2, out entry2))
                                                        {
                                                            entry2 = new DirectUpgradeEntry();
                                                            entry2.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, null, null, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                            entry2.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                            map[gemId2] = entry2;
                                                            list.Add(entry2);
                                                        }
                                                        list = entry2.DirectUpgradeList;
                                                    }
                                                    if (gem3 != null)
                                                    {
                                                        if (!map.TryGetValue(gemId3, out entry3))
                                                        {
                                                            entry3 = new DirectUpgradeEntry();
                                                            entry3.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, null, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                            entry3.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                            map[gemId3] = entry3;
                                                            list.Add(entry3);
                                                        }
                                                        list = entry3.DirectUpgradeList;
                                                    }
                                                    if (enchant != null)
                                                    {
                                                        if (!map.TryGetValue(gemId4, out entry4))
                                                        {
                                                            entry4 = new DirectUpgradeEntry();
                                                            entry4.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, enchant, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                            entry4.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                            map[gemId4] = entry4;
                                                            list.Add(entry4);
                                                        }
                                                        list = entry4.DirectUpgradeList;
                                                    }
                                                    if (reforging != null)
                                                    {
                                                        if (!map.TryGetValue(gemId5, out entry5))
                                                        {
                                                            entry5 = new DirectUpgradeEntry();
                                                            entry5.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, enchant, reforging, null, upgrade) { ItemAvailabilityInformation = availability };
                                                            entry5.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                            map[gemId5] = entry5;
                                                            list.Add(entry5);
                                                        }
                                                        list = entry5.DirectUpgradeList;
                                                    }
                                                    if (tinkering != null)
                                                    {
                                                        if (!map.TryGetValue(gemId6, out entry6))
                                                        {
                                                            entry6 = new DirectUpgradeEntry();
                                                            entry6.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, enchant, reforging, tinkering, upgrade) { ItemAvailabilityInformation = availability };
                                                            map[gemId6] = entry6;
                                                            list.Add(entry6);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (generative && availability != null && generateDirectUpgrades)
                                            {
                                                var map = availability.MatchingMap;
                                                string gemId1 = string.Format("{0}.0.0.0.0.0.{1}", gem1 != null ? gem1.Id : 0, upgrade);
                                                string gemId2 = string.Format("{0}.{1}.0.0.0.0.{2}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, upgrade);
                                                string gemId3 = string.Format("{0}.{1}.{2}.0.0.0.{3}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, upgrade);
                                                string gemId4 = string.Format("{0}.{1}.{2}.{3}.0.0.{4}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, enchant != null ? enchant.Id : 0, upgrade);
                                                string gemId5 = string.Format("{0}.{1}.{2}.{3}.{4}.0.{5}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, enchant != null ? enchant.Id : 0, reforging != null ? reforging.Id : 0, upgrade);
                                                string gemId6 = string.Format("{0}.{1}.{2}.{3}.{4}.{5}.{6}", gem1 != null ? gem1.Id : 0, gem2 != null ? gem2.Id : 0, gem3 != null ? gem3.Id : 0, enchant != null ? enchant.Id : 0, reforging != null ? reforging.Id : 0, tinkering != null ? tinkering.Id : 0, upgrade);
                                                DirectUpgradeEntry entry1, entry2, entry3, entry4, entry5, entry6;
                                                List<DirectUpgradeEntry> list = availability.MatchingDirectUpgradeList;
                                                if (gem1 != null)
                                                {
                                                    if (!map.TryGetValue(gemId1, out entry1))
                                                    {
                                                        entry1 = new DirectUpgradeEntry();
                                                        entry1.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, null, null, null, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                        entry1.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                        map[gemId1] = entry1;
                                                        list.Add(entry1);
                                                    }
                                                    list = entry1.DirectUpgradeList;
                                                }
                                                if (gem2 != null)
                                                {
                                                    if (!map.TryGetValue(gemId2, out entry2))
                                                    {
                                                        entry2 = new DirectUpgradeEntry();
                                                        entry2.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, null, null, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                        entry2.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                        map[gemId2] = entry2;
                                                        list.Add(entry2);
                                                    }
                                                    list = entry2.DirectUpgradeList;
                                                }
                                                if (gem3 != null)
                                                {
                                                    if (!map.TryGetValue(gemId3, out entry3))
                                                    {
                                                        entry3 = new DirectUpgradeEntry();
                                                        entry3.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, null, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                        entry3.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                        map[gemId3] = entry3;
                                                        list.Add(entry3);
                                                    }
                                                    list = entry3.DirectUpgradeList;
                                                }
                                                if (enchant != null)
                                                {
                                                    if (!map.TryGetValue(gemId4, out entry4))
                                                    {
                                                        entry4 = new DirectUpgradeEntry();
                                                        entry4.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, enchant, null, null, upgrade) { ItemAvailabilityInformation = availability };
                                                        entry4.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                        map[gemId4] = entry4;
                                                        list.Add(entry4);
                                                    }
                                                    list = entry4.DirectUpgradeList;
                                                }
                                                if (reforging != null)
                                                {
                                                    if (!map.TryGetValue(gemId5, out entry5))
                                                    {
                                                        entry5 = new DirectUpgradeEntry();
                                                        entry5.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, enchant, reforging, null, upgrade) { ItemAvailabilityInformation = availability };
                                                        entry5.DirectUpgradeList = new List<DirectUpgradeEntry>();
                                                        map[gemId5] = entry5;
                                                        list.Add(entry5);
                                                    }
                                                    list = entry5.DirectUpgradeList;
                                                }
                                                if (tinkering != null)
                                                {
                                                    if (!map.TryGetValue(gemId6, out entry6))
                                                    {
                                                        entry6 = new DirectUpgradeEntry();
                                                        entry6.ItemInstance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, enchant, reforging, tinkering, upgrade) { ItemAvailabilityInformation = availability };
                                                        map[gemId6] = entry6;
                                                        list.Add(entry6);
                                                    }
                                                }
                                            }
                                        }
                                        if (add)
                                        {
                                            ItemInstance instance = new ItemInstance(item, randomSuffixId, gem1, gem2, gem3, enchant, reforging, tinkering, upgrade) { ItemAvailabilityInformation = availability };
                                            possibleGemmedItems.Add(instance);
                                            if (availability != null && !generative && generateDirectUpgrades)
                                            {
                                                bool addSingle = true;
                                                foreach (DirectUpgradeEntry entry in availability.SingleDirectUpgradeList)
                                                {
                                                    if (entry.ItemInstance == instance)
                                                    {
                                                        addSingle = false;
                                                        break;
                                                    }
                                                }
                                                if (addSingle)
                                                {
                                                    availability.SingleDirectUpgradeList.Add(new DirectUpgradeEntry() { ItemInstance = instance });
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        
            return possibleGemmedItems;
        }

        public static List<Buff> FilterList(List<Buff> unfilteredList)
        {
            List<Buff> filteredList = new List<Buff>();
            foreach (Buff buff in unfilteredList)
            {
                if (buff == null)
                {
                    filteredList.Add(buff);
                    continue;
                }

                bool addBuff = true;
                List<Buff> removeBuffs = new List<Buff>();
                foreach (Buff buff2 in filteredList)
                {
                    ArrayUtils.CompareResult compare = buff.Stats.CompareTo(buff2.Stats);
                    if (compare == ArrayUtils.CompareResult.GreaterThan) //A>B
                    {
                        removeBuffs.Add(buff2);
                    }
                    else if (compare == ArrayUtils.CompareResult.Equal || compare == ArrayUtils.CompareResult.LessThan)
                    {
                        addBuff = false;
                        break;
                    }
                }
                foreach (Buff removeBuff in removeBuffs)
                    filteredList.Remove(removeBuff);
                if (addBuff) filteredList.Add(buff);
            }
            return filteredList;
        }

        public static Enchant[] FilterList(List<Enchant> unfilteredList)
        {
            Enchant noenchant = null;
            List<Enchant> filteredList = new List<Enchant>();
            foreach (Enchant enchant in unfilteredList)
            {
                if (enchant.Id == 0)
                {
                    // only add no enchant if it is the only enchant
                    // it won't be filtered because it has a null slot
                    noenchant = enchant;
                    continue;
                }

                bool addEnchant = true;
                List<Enchant> removeEnchants = new List<Enchant>();
                foreach (Enchant enchant2 in filteredList)
                {
                    if (enchant.Slot == enchant2.Slot)
                    {
                        ArrayUtils.CompareResult compare = enchant.Stats.CompareTo(enchant2.Stats);
                        if (compare == ArrayUtils.CompareResult.GreaterThan) //A>B
                        {
                            removeEnchants.Add(enchant2);
                        }
                        else if (compare == ArrayUtils.CompareResult.Equal || compare == ArrayUtils.CompareResult.LessThan)
                        {
                            addEnchant = false;
                            break;
                        }
                    }
                }
                foreach (Enchant removeEnchant in removeEnchants)
                    filteredList.Remove(removeEnchant);
                if (addEnchant) filteredList.Add(enchant);
            }
            if (filteredList.Count == 0)
            {
                filteredList.Add(noenchant);
            }
            return filteredList.ToArray();
        }

        public static Tinkering[] FilterList(List<Tinkering> unfilteredList)
        {
            List<Tinkering> filteredList = new List<Tinkering>();
            foreach (Tinkering tinkering in unfilteredList)
            {
                if (tinkering.Id == 0)
                {
                    // only add no enchant if it is the only enchant
                    // it won't be filtered because it has a null slot
                    if (unfilteredList.Count == 1)
                    {
                        filteredList.Add(tinkering);
                    }
                    continue;
                }

                bool addTinkering = true;
                List<Tinkering> removeTinkerings = new List<Tinkering>();
                foreach (Tinkering tinkering2 in filteredList)
                {
                    if (tinkering.Slot == tinkering2.Slot)
                    {
                        ArrayUtils.CompareResult compare = tinkering.Stats.CompareTo(tinkering2.Stats);
                        if (compare == ArrayUtils.CompareResult.GreaterThan) //A>B
                        {
                            removeTinkerings.Add(tinkering2);
                        }
                        else if (compare == ArrayUtils.CompareResult.Equal || compare == ArrayUtils.CompareResult.LessThan)
                        {
                            addTinkering = false;
                            break;
                        }
                    }
                }
                foreach (Tinkering removeTinkering in removeTinkerings)
                    filteredList.Remove(removeTinkering);
                if (addTinkering) filteredList.Add(tinkering);
            }
            return filteredList.ToArray();
        }

        public static Item[] FilterList(List<Item> unfilteredList)
        {
            List<Item> filteredList = new List<Item>();
            List<StatsColors> filteredStatsColors = new List<StatsColors>();
            foreach (Item item in unfilteredList)
            {
                if (item == null)
                {
                    filteredList.Add(item);
                    continue;
                }

                StatsColors statsColorsA = new StatsColors()
                {
                    Item = item,
                    ItemIsJewelersGem = item.IsJewelersGem || item.IsJewelersFacet,
                    Ignore = item.IsGem && !item.IsJewelersGem && !item.IsJewelersFacet && item.Unique,
                    SetName = item.SetName,
                    Stats = item.Stats,
                };
                bool addItem = true;
                List<StatsColors> removeItems = new List<StatsColors>();
                foreach (StatsColors statsColorsB in filteredStatsColors)
                {
                    ArrayUtils.CompareResult compare = statsColorsA.CompareTo(statsColorsB);
                    if (compare == ArrayUtils.CompareResult.GreaterThan) //A>B
                    {
                        removeItems.Add(statsColorsB);
                    }
                    else if (compare == ArrayUtils.CompareResult.Equal || compare == ArrayUtils.CompareResult.LessThan)
                    {
                        addItem = false;
                        break;
                    }
                }
                foreach (StatsColors removeItem in removeItems)
                    filteredStatsColors.Remove(removeItem);
                if (addItem) filteredStatsColors.Add(statsColorsA);
            }
            foreach (StatsColors statsColors in filteredStatsColors)
            {
                filteredList.Add(statsColors.Item);
            }
            return filteredList.ToArray();
        }

        public static List<ItemInstance> FilterList(List<ItemInstance> unfilteredList, bool itemsFilteredHint)
        {
            List<ItemInstance> filteredList = new List<ItemInstance>();
            List<StatsColors> filteredStatsColors = new List<StatsColors>();
            foreach (ItemInstance gemmedItem in unfilteredList)
            {
                if ((object)gemmedItem == null)
                {
                    filteredList.Add(gemmedItem);
                    continue;
                }
                int meta = 0, red = 0, yellow = 0, blue = 0, jeweler = 0, cog = 0, hyd = 0;
                bool ignore = false;
                foreach (Item gem in new Item[] { gemmedItem.Gem1, gemmedItem.Gem2, gemmedItem.Gem3 })
                    if (gem != null)
                    {
                        switch (gem.Slot)
                        {
                            case ItemSlot.Meta: meta++; break;
                            case ItemSlot.Red: red++; break;
                            case ItemSlot.Orange: red++; yellow++; break;
                            case ItemSlot.Yellow: yellow++; break;
                            case ItemSlot.Green: yellow++; blue++; break;
                            case ItemSlot.Blue: blue++; break;
                            case ItemSlot.Purple: blue++; red++; break;
                            case ItemSlot.Prismatic: red++; yellow++; blue++; break;
                            case ItemSlot.Cogwheel: cog++; break;
                            case ItemSlot.Hydraulic: hyd++; break;
                        }
                        if (gem.IsJewelersGem || gem.IsJewelersFacet)
                        {
                            jeweler++;
                        }
                        else if (gem.Unique)
                        {
                            ignore = true;
                        }
                    }

                StatsColors statsColorsA = new StatsColors()
                {
                    ItemInstance = gemmedItem,
                    Item = gemmedItem.Item,
                    SetName = gemmedItem.Item.SetName,
                    Stats = gemmedItem.GetTotalStats(),
                    Meta = meta,
                    Red = red,
                    Yellow = yellow,
                    Blue = blue,
                    Cogwheel = cog,
                    Hydraulic = hyd,
                    Jeweler = jeweler,
                    Ignore = ignore
                };
                bool addItem = true;
                List<StatsColors> removeItems = new List<StatsColors>();
                foreach (StatsColors statsColorsB in filteredStatsColors)
                {
                    if (!itemsFilteredHint || statsColorsA.Item.Id != statsColorsB.Item.Id)
                    {
                        ArrayUtils.CompareResult compare = statsColorsA.CompareTo(statsColorsB);
                        if (compare == ArrayUtils.CompareResult.GreaterThan) //A>B
                        {
                            removeItems.Add(statsColorsB);
                        }
                        else if (compare == ArrayUtils.CompareResult.Equal || compare == ArrayUtils.CompareResult.LessThan)
                        {
                            addItem = false;
                            break;
                        }
                    }
                }
                foreach (StatsColors removeItem in removeItems)
                    filteredStatsColors.Remove(removeItem);
                if (addItem) filteredStatsColors.Add(statsColorsA);
            }
            foreach (StatsColors statsColors in filteredStatsColors)
            {
                filteredList.Add(statsColors.ItemInstance);
            }
            return filteredList;
        }

        private class StatsColors
        {
            public ItemInstance ItemInstance;
            public Item Item;
            public Stats Stats;
            public int Meta;
            public int Red;
            public int Yellow;
            public int Blue;
            public int Cogwheel;
            public int Hydraulic;
            public int Jeweler;
            public bool Ignore;

            private string setName;

            public string SetName
            {
                get { return setName; }
                set {
                    setName = value;
                    if (setName == null) setName = "";
                }
            }

            public bool ItemIsJewelersGem;

            public ArrayUtils.CompareResult CompareTo(StatsColors other)
            {
                if (Ignore || other.Ignore) return ArrayUtils.CompareResult.Unequal;
                if (ItemIsJewelersGem != other.ItemIsJewelersGem) return ArrayUtils.CompareResult.Unequal;
                if (Jeweler != other.Jeweler) return ArrayUtils.CompareResult.Unequal;
                if (ItemInstance != null && other.ItemInstance != null && ItemInstance.ItemAvailabilityInformation != null && other.ItemInstance.ItemAvailabilityInformation != null && ItemInstance.ItemAvailabilityInformation.PositiveCostItem != other.ItemInstance.ItemAvailabilityInformation.PositiveCostItem) return ArrayUtils.CompareResult.Unequal;

                if (this.SetName != other.SetName) return ArrayUtils.CompareResult.Unequal;

                int compare = Meta.CompareTo(other.Meta);
                bool haveLessThan = compare < 0;
                bool haveGreaterThan = compare > 0;
                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;

                compare = Red.CompareTo(other.Red);
                haveLessThan |= compare < 0;
                haveGreaterThan |= compare > 0;
                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;

                compare = Yellow.CompareTo(other.Yellow);
                haveLessThan |= compare < 0;
                haveGreaterThan |= compare > 0;
                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;

                compare = Blue.CompareTo(other.Blue);
                haveLessThan |= compare < 0;
                haveGreaterThan |= compare > 0;
                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;

                compare = Cogwheel.CompareTo(other.Cogwheel);
                haveLessThan |= compare < 0;
                haveGreaterThan |= compare > 0;
                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;

                compare = Hydraulic.CompareTo(other.Hydraulic);
                haveLessThan |= compare < 0;
                haveGreaterThan |= compare > 0;
                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;

                ArrayUtils.CompareResult compareResult = Stats.CompareTo(other.Stats);
                if (compareResult == ArrayUtils.CompareResult.Unequal) return ArrayUtils.CompareResult.Unequal;
                haveLessThan |= compareResult == ArrayUtils.CompareResult.LessThan;
                haveGreaterThan |= compareResult == ArrayUtils.CompareResult.GreaterThan;

                if (Item != null && other.Item != null)
                {
                    compare = Item.MinDamage.CompareTo(other.Item.MinDamage);
                    haveLessThan |= compare < 0;
                    haveGreaterThan |= compare > 0;
                    if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;
                }

                if (Item != null && (Item.Slot == ItemSlot.MainHand || Item.Slot == ItemSlot.OneHand || Item.Slot == ItemSlot.TwoHand || Item.Slot == ItemSlot.Ranged))
                {
                    if (Item.Slot == ItemSlot.TwoHand && other.Item.Slot != ItemSlot.TwoHand && haveGreaterThan)
                    {
                        return ArrayUtils.CompareResult.Unequal;
                    }
                    if (Item.Slot != ItemSlot.TwoHand && other.Item.Slot == ItemSlot.TwoHand && haveLessThan)
                    {
                        return ArrayUtils.CompareResult.Unequal;
                    }
                }

                if (haveGreaterThan && haveLessThan) return ArrayUtils.CompareResult.Unequal;
                else if (haveGreaterThan)
                {
                    if (Item != null && Item.IsGem && !Item.IsJewelersGem && !Item.IsJewelersFacet && Item.Unique) return ArrayUtils.CompareResult.Unequal;
                    return ArrayUtils.CompareResult.GreaterThan;
                }
                else if (haveLessThan)
                {
                    if (other.Item != null && other.Item.IsGem && !other.Item.IsJewelersGem && other.Item.IsJewelersFacet && other.Item.Unique) return ArrayUtils.CompareResult.Unequal;
                    return ArrayUtils.CompareResult.LessThan;
                }
                else
                {
                    return ArrayUtils.CompareResult.Equal;
                }
            }
            public static bool operator ==(StatsColors x, StatsColors y)
            {
                if (ReferenceEquals(x, null) || ReferenceEquals(y, null)) return false;
                return x.Meta == y.Meta && x.Red == y.Red && x.Yellow == y.Yellow
                    && x.Blue == y.Blue && x.Jeweler == y.Jeweler && x.Stats == y.Stats;
            }
            public override int GetHashCode()
            {
                return Stats.GetHashCode() ^ Item.GetHashCode();
            }
            public override bool Equals(object obj)
            {
                if (obj != null && obj.GetType() == this.GetType())
                {
                    return this == (obj as StatsColors);
                }
                return base.Equals(obj);
            }
            public static bool operator !=(StatsColors x, StatsColors y)
            {
                return !(x == y);
            }
        }
    }
}
