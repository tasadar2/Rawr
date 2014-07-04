﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace Rawr
{
    public class ItemFilterEnabledOverride
    {
        public string Name { get; set; }
        public bool? Enabled { get; set; }
        public List<ItemFilterEnabledOverride> SubFilterOverride { get; set; }
    }

    public class ItemFilterRegexList : ObservableCollection<ItemFilterRegex> 
    {

        public ItemFilterRegex parent;
        public ItemFilterRegex Parent {
            get { return parent; }
            set
            {
                parent = value;
                other.Parent = parent;
                foreach (ItemFilterRegex filter in this) filter.Parent = parent;
            }
        }
        internal ItemFilterOther other;

        [XmlIgnore]
        public ObservableCollection<ItemFilter> FilterList { get; private set; }

        public ItemFilterRegexList() : this(null) { }
        public ItemFilterRegexList(ItemFilterRegex parent) : base()
        {
            other = new ItemFilterOther(parent);
            other.PropertyChanged += new PropertyChangedEventHandler(filter_PropertyChanged);

            Parent = parent;

            FilterList = new ObservableCollection<ItemFilter>();
            CollectionChanged += new NotifyCollectionChangedEventHandler(RegexList_CollectionChanged);
        }

        public void UpdateStates()
        {
            if (parent == null) return;
            bool? enabled = parent.Enabled;
            foreach (ItemFilter filter in FilterList)
            {
                filter.UpdateEnabled(enabled.GetValueOrDefault(false));
                if (filter.Filters != null && filter.Filters.Count > 0) {
                    foreach (ItemFilter subFilter in filter.Filters) {
                        subFilter.UpdateEnabled(enabled.GetValueOrDefault(false));
                        if (subFilter.Filters != null && subFilter.Filters.Count > 0) {
                            foreach (ItemFilter subsubFilter in subFilter.Filters) {
                                subsubFilter.UpdateEnabled(enabled.GetValueOrDefault(false));
                                if (subsubFilter.Filters != null && subsubFilter.Filters.Count > 0) {
                                    foreach (ItemFilter subsubsubFilter in subsubFilter.Filters) {
                                        subsubsubFilter.UpdateEnabled(enabled.GetValueOrDefault(false));
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool? State()
        {
            bool allEnabled = true;
            bool someEnabled = false;
            foreach (ItemFilter filter in FilterList)
            {
                allEnabled = filter.Enabled.GetValueOrDefault(false) && allEnabled;
                someEnabled = filter.Enabled.GetValueOrDefault(true) || someEnabled;
            }
            if (allEnabled) return true;
            else if (someEnabled) return null;
            else return false;
        }

        private void filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Enabled" && parent != null)
            {
                if (!ItemFilter.IsLoading)
                {
                    parent.UpdateEnabled(null);
                }
            }
        }

        private void RegexList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
                foreach (ItemFilterRegex filter in e.OldItems)
                {
                    filter.Parent = null;
                    filter.PropertyChanged -= new PropertyChangedEventHandler(filter_PropertyChanged);
                    FilterList.Remove(filter);
                }

            if (e.NewItems != null)
                foreach (ItemFilterRegex filter in e.NewItems)
                {
                    filter.Parent = Parent;
                    filter.PropertyChanged += new PropertyChangedEventHandler(filter_PropertyChanged);
                    FilterList.Add(filter);
                }

            FilterList.Remove(other);
            if (Count > 0) FilterList.Add(other);
        }

    }

    public class ItemFilterOther : ItemFilter
    {

        public ItemFilterOther(ItemFilterRegex parent) : base()
        {
            this.Parent = parent;
            Name = "Other";
        }

        public ItemFilterRegex Parent { get; set; }

        public override bool? Enabled
        {
            get
            {
                if (Parent == null) return ItemFilter.OtherEnabled;
                else return Parent.OtherRegexEnabled;
            }
            set
            {
                if (Parent == null)
                {
                    ItemFilter.OtherEnabled = value.GetValueOrDefault(false);
                    //Parent.UpdateEnabled(null);
                }
                else Parent.OtherRegexEnabled = value.GetValueOrDefault(false);
                OnEnabledChanged(true);
            }
        }

        public override void UpdateEnabled(bool? setValue)
        {
            if (Parent == null) ItemFilter.OtherEnabled = setValue.GetValueOrDefault(false);
            else Parent.UpdateOtherRegexEnabled(setValue.GetValueOrDefault(false), false);
            OnEnabledChanged(false);
        }
    }

    public class ItemFilterRegex : ItemFilter
    {
        public ItemFilterRegex()
        {
            Name = "New Filter";
            RegexList = new ItemFilterRegexList(this);
            Enabled = true;
            MinItemLevel = 0;
            MaxItemLevel = 1000;
            MinDropRatePerc = 000.0d;
            MaxDropRatePerc = 100.0d;
            MinItemQuality = ItemQuality.Temp;
            MaxItemQuality = ItemQuality.Heirloom;
            BoP = true;
            BoE = true;
            BoU = true;
            BoN = true;
            BoA = true;
            AdditiveFilter = true;
            AppliesToItems = true;
            AppliesToGems = false;
        }

        [XmlIgnore]
        private string _pattern;
        public string Pattern {
            get { return _pattern; }
            set {
                _pattern = value;
                _regex = null;
            }
        }

        [XmlIgnore]
        public ItemFilterRegex Parent { get; internal set; }

        public bool BoA { get; set; }
        public bool BoE { get; set; }
        public bool BoP { get; set; }
        public bool BoU { get; set; }
        public bool BoN { get; set; }

        public int MinItemLevel { get; set; }
        public int MaxItemLevel { get; set; }

        public double MinDropRatePerc { get; set; }
        public double MaxDropRatePerc { get; set; }

        public ItemQuality MinItemQuality { get; set; }
        public int MinItemQualityIndex
        {
            get { return (int)MinItemQuality + 1; }
            set { MinItemQuality = (ItemQuality)(value - 1); }
        }

        public ItemQuality MaxItemQuality { get; set; }
        public int MaxItemQualityIndex
        {
            get { return (int)(MaxItemQuality + 1); }
            set { MaxItemQuality = (ItemQuality)(value - 1); }
        }

        public bool AdditiveFilter { get; set; }
        public bool AppliesToItems { get; set; }
        public bool AppliesToGems { get; set; }

        private bool? enabled;
        public override bool? Enabled
        {
            get { return enabled; }
            set
            {
                if (ItemFilter.IsLoading)
                {
                    enabled = value;
                }
                else
                {
                    enabled = value.GetValueOrDefault(false);
                    RegexList.UpdateStates();
                }
                OnEnabledChanged(true);
            }
        }

        public override void UpdateEnabled(bool? setValue) {
            if (setValue.HasValue) {
                enabled = setValue.GetValueOrDefault(false);
            } else {
                enabled = RegexList.State();
            }
            OnEnabledChanged(false);
        }

        [XmlIgnore]
        public override ObservableCollection<ItemFilter> Filters { get { return RegexList.FilterList; } }

        private ItemFilterRegexList regexList;
        public ItemFilterRegexList RegexList
        {
            get { return regexList; }
            set
            {
                regexList = value;
                regexList.Parent = this;
            }
        }

        private bool otherRegexEnabled;
        public bool OtherRegexEnabled
        {
            get
            {
                return otherRegexEnabled;
            }
            set
            {
                UpdateOtherRegexEnabled(value, true);
            }
        }

        internal void UpdateOtherRegexEnabled(bool value, bool originator)
        {
            otherRegexEnabled = value;
            if (regexList != null && regexList.other != null)
            {
                regexList.other.OnEnabledChanged(originator);
            }
        }

        [XmlIgnore]
        private Regex _regex;

        [XmlIgnore]
        public Regex Regex
        {
            get
            {
                if (_regex == null)
                {
                    if (_pattern == null) _pattern = "";
                    _regex = new Regex(_pattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
                }
                return _regex;
            }
        }

        public bool IsMatch(Item item)
        {
            //bool locListIsValid = item.LocationInfo.Count > 0;
            bool loc0IsValid = false, loc0IsValidn = false, loc1IsValid = false, loc2IsValid = false;
            if (item.LocationInfo.Count() > 0) {
                loc0IsValid = /*locListIsValid &&*/ /*item.LocationInfo.Count > 0 &&*/ /*item.LocationInfo[0] != null &&*/ !string.IsNullOrEmpty(item.LocationInfo[0].Description) && Regex.IsMatch(item.LocationInfo[0].Description);
                loc0IsValidn = /*locListIsValid &&*/ /*item.LocationInfo.Count > 0 &&*/ /*item.LocationInfo[0] != null &&*/ !string.IsNullOrEmpty(item.LocationInfo[0].Note) && Regex.IsMatch(item.LocationInfo[0].Note);
                loc1IsValid = /*locListIsValid &&*/   item.LocationInfo.Count > 1 &&   /*item.LocationInfo[1] != null &&*/ !string.IsNullOrEmpty(item.LocationInfo[1].Description) && Regex.IsMatch(item.LocationInfo[1].Description);
                loc2IsValid = /*locListIsValid &&*/   item.LocationInfo.Count > 2 &&   /*item.LocationInfo[2] != null &&*/ !string.IsNullOrEmpty(item.LocationInfo[2].Description) && Regex.IsMatch(item.LocationInfo[2].Description);
            }

            if (string.IsNullOrEmpty(_pattern) || (loc0IsValid) || (loc0IsValidn) || (loc1IsValid) || (loc2IsValid))
            {
                if      (!(item.ItemLevel >= MinItemLevel && item.ItemLevel <= MaxItemLevel)) { return false; } // Verify ilevel
                //else if (!(item.ItemLevel >= MinItemLevel && item.ItemLevel <= MaxItemLevel)) { return false; } // Verify UI Ilevel (not part of the filter tree)
                else if (!(item.Quality >= MinItemQuality && item.Quality <= MaxItemQuality)) { return false; } // Verify Quality
                else if (!(item.DropRate >= MinDropRatePerc && item.DropRate <= MaxDropRatePerc)) { return false; } // Verify Quality
                else if (!ValidateBinding(item)) { return false; } // Verify Binding
                else { return true; }
            }
            return false;
        }

        private bool ValidateBinding(Item item)
        {
            // no bind specified - we're fine
            if (!(BoA || BoE || BoP || BoU || BoN)
                || (!BoN && item.Bind == BindsOn.None))
                return true;
            if (BoA && item.Bind == BindsOn.BoA) return true;
            if (BoE && item.Bind == BindsOn.BoE) return true;
            if (BoP && item.Bind == BindsOn.BoP) return true;
            if (BoU && item.Bind == BindsOn.BoU) return true;
            if (BoN && item.Bind == BindsOn.None) return true;

            return false;
        }

        public bool AppliesTo(Item item)
        {
            return (item.IsGem && AppliesToGems) || (!item.IsGem && AppliesToItems);
        }

        public bool IsEnabled(Item item)
        {
            if (!Enabled.GetValueOrDefault(true)) return false;
            if (RegexList.Count == 0) return true;
            bool enabledMatch = false;
            bool anyMatch = false;
            foreach (ItemFilterRegex regex in RegexList)
            {
                if (regex.AppliesTo(item) && regex.IsMatch(item))
                {
                    anyMatch = true;
                    if (regex.IsEnabled(item))
                    {
                        enabledMatch = true;
                        break;
                    }
                }
            }
            if (anyMatch)
            {
                return enabledMatch;
            }
            else
            {
                return OtherRegexEnabled;
            }
        }

    }

    [GenerateSerializer]
    public class ItemTypeList : List<ItemType>
    {
        public ItemTypeList() : base() { }
        public ItemTypeList(IEnumerable<ItemType> collection) : base(collection) { }
    }

    [GenerateSerializer]
    public class ItemFilterData
    {
        public SerializableDictionary<string, ItemTypeList> RelevantItemTypes = new SerializableDictionary<string, ItemTypeList>();
        public ItemFilterRegexList RegexList = new ItemFilterRegexList();
        public bool OtherRegexEnabled = true;
    }

    public abstract class ItemFilter : INotifyPropertyChanged
    {
        #region Static
        static ItemFilterData data = new ItemFilterData();

        /// <summary>
        /// Returns a list that can be used to set relevant item types list. Call ItemCache.OnItemsChanged()
        /// when you finish making changes to the list.
        /// </summary>
        /// <remarks>
        /// Since it doesn't make much sense to disable jewelry, the ItemType.None only applies to off hand slot.
        /// </remarks>
        public static List<ItemType> GetRelevantItemTypesList(CalculationsBase model)
        {
            ItemTypeList list;
            if (!data.RelevantItemTypes.TryGetValue(model.Name, out list))
            {
                list = new ItemTypeList(model.RelevantItemTypes);
                data.RelevantItemTypes[model.Name] = list;
            }
            return list;
        }

        public static ItemFilterRegexList FilterList
        {
            get
            {
                return data.RegexList;
            }
        }

        public static bool OtherEnabled
        {
            get
            {
                return data.OtherRegexEnabled;
            }
            set
            {
                data.OtherRegexEnabled = value;
                if (data.RegexList != null && data.RegexList.other != null)
                {
                    data.RegexList.other.OnEnabledChanged(true);
                }
            }
        }

        public static bool IsItemRelevant(CalculationsBase model, Item item)
        {
            if (GetRelevantItemTypesList(model).Contains(item.Type) || (item.Type == ItemType.None && item.Slot != ItemSlot.OffHand))
            {
                bool added = false;
                bool anyMatch = false;
                bool enabledMatch = false;
                foreach (ItemFilterRegex regex in data.RegexList)
                {
                    if (regex.AdditiveFilter && regex.AppliesTo(item) && regex.IsMatch(item))
                    {
                        anyMatch = true;
                        if (regex.IsEnabled(item))
                        {
                            enabledMatch = true;
                            break;
                        }
                    }
                }
                if (anyMatch)
                {
                    added = enabledMatch;
                }
                else
                {
                    added = OtherEnabled;
                }
                if (added)
                {
                    foreach (ItemFilterRegex regex in data.RegexList)
                    {
                        if (!regex.AdditiveFilter && regex.Enabled.GetValueOrDefault(true) && regex.AppliesTo(item) && regex.IsMatch(item) && regex.IsEnabled(item))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static void Save(TextWriter writer)
        {
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ItemFilterData));
            serializer.Serialize(writer, data);
            writer.Close();
        }

        public static bool IsLoading { get; set; }

        public static void Load(TextReader reader)
        {
            IsLoading = true;
            try
            {
                System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ItemFilterData));
                data = (ItemFilterData)serializer.Deserialize(reader);
                reader.Close();
            }
            catch (Exception)
            {
                data = new ItemFilterData();
            }
            IsLoading = false;
            ItemCache.OnItemsChanged();
        }
        #endregion

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }

        [XmlIgnore]
        public virtual ObservableCollection<ItemFilter> Filters { get { return null; } }

        public abstract bool? Enabled { get; set; }
        public abstract void UpdateEnabled(bool? value);

        public void OnEnabledChanged(bool orginator)
        {
            OnPropertyChanged("Enabled");
            if (orginator && !IsLoading) ItemCache.OnItemsChanged();
        }

        #region INotifyPropertyChanged Members
        protected void OnPropertyChanged(string property)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

    }
}
