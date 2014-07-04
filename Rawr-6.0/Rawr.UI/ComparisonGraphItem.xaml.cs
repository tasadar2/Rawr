﻿using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Rawr.UI
{
    public partial class ComparisonGraphItem : UserControl
    {
        public ComparisonGraphItem(IEnumerable<Color> colors) : this() { SetColors(colors); }
        public ComparisonGraphItem()
        {
            // Required to initialize variables
            InitializeComponent();
            NameGrid.Width = GraphBarStart;
        }

        private ItemInstance itemInstance;
        public ItemInstance ItemInstance
        {
            get { return itemInstance; }
            set
            {
                itemInstance = value;
                if (itemInstance != null) {
                    ContextItemName.Header = itemInstance.Item != null ? itemInstance.Item.Name : string.Empty;
                    ItemImage.Source = Icons.AnIcon(ItemInstance.Item.IconPath);
                } else {
                    ItemImage.Source = null;
                }
                character_AvailableItemsChanged(this, EventArgs.Empty);
            }
        }

        private String nonItemImageSource = null;
        public String NonItemImageSource {
            get { return nonItemImageSource; }
            set {
                if (nonItemImageSource != value) {
                    nonItemImageSource = value;
                    if (NonItemImageSource != null)
                    {
                        ItemImage.Source = Icons.AnIcon(NonItemImageSource);
                    }
                }
            }
        }

        private CharacterSlot slot;
        public CharacterSlot Slot { get { return slot; } set { slot = value; } }

        private Item item;
        public Item OtherItem
        {
            get { return item; }
            set
            {
                item = value;
                if (item != null)
                {
                    ContextItemName.Header = item.Name;
                }
                character_AvailableItemsChanged(this, EventArgs.Empty);
            }
        }

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                if (character != null)
                    character.AvailableItemsChanged -= new Character.AvailableItemsChangedEventHandler(character_AvailableItemsChanged);
                
                character = value;

                if (character != null)
                    character.AvailableItemsChanged += new Character.AvailableItemsChangedEventHandler(character_AvailableItemsChanged);
                
                character_AvailableItemsChanged(this, EventArgs.Empty);
            }
        }

        private void character_AvailableItemsChanged(object sender, EventArgs e)
        {
            if (Character != null && ((ItemInstance != null && ItemInstance.Id > 0) || (OtherItem != null && OtherItem.Id != 0)))
            {
                AvailableImage.Visibility = Visibility.Visible;
                ItemAvailability itemAvailability;
                if (ItemInstance != null) itemAvailability = Character.GetItemAvailability(ItemInstance);
                else itemAvailability = Character.GetItemAvailability(OtherItem);
                switch (itemAvailability)
                {
                    case ItemAvailability.RegemmingAllowed:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond.png", UriKind.Relative));
                        break;
                    case ItemAvailability.RegemmingAllowedWithEnchantRestrictions:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond3.png", UriKind.Relative));
                        break;
                    case ItemAvailability.Available:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond2.png", UriKind.Relative));
                        break;
                    case ItemAvailability.AvailableWithEnchantRestrictions:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/Diamond4.png", UriKind.Relative));
                        break;
                    case ItemAvailability.NotAvailable:
                        AvailableImage.Source = new BitmapImage(new Uri("Images/DiamondOutline.png", UriKind.Relative));
                        break;
                }

            } else { AvailableImage.Visibility = Visibility.Collapsed; }
        }

        public string Title
        {
            get { return TextLabel.Text; }
            set { TextLabel.Text = value; }
        }

        public bool Equipped
        {
            get { return EquippedRect.Visibility == Visibility.Visible; }
            set { EquippedRect.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        public bool PartEquipped
        {
            get { return PartEquippedRect.Visibility == Visibility.Visible; }
            set { PartEquippedRect.Visibility = value ? Visibility.Visible : Visibility.Collapsed; }
        }

        public float MinScale { get; set; }
        public float MaxScale { get; set; }

        private List<ComparisonGraphBar> rects;
        private List<float> values;
        public float this[int i]
        {
            get { return values[i]; }
            set
            {
                if (i > values.Count - 1) return; // We have a custom chart with a different number of subpoints
                values[i] = value;
                rects[i].Title = Math.Round(value, 2).ToString();
                TotalLabel.Text = Math.Round(values.Sum(), 2).ToString();
                ChangedSize(this, null);
            }
        }

        public void SetColors(IEnumerable<Color> colors)
        {
            rects = new List<ComparisonGraphBar>();
            values = new List<float>(rects.Count);

            foreach (Color c in colors)
            {
                ComparisonGraphBar r = new ComparisonGraphBar() { Color = c };
                r.Height = 30;
                r.Margin = new Thickness(0, 4, 0, 4);
                rects.Add(r);
                values.Add(0f);
            }
        }

        private float _GraphBarStart = 162f;
        public float GraphBarStart
        {
            get { return _GraphBarStart + (Rawr.Properties.GeneralSettings.Default.ItemNameWidthSetting * 20); }
            set { _GraphBarStart = value; }
        }
        /// <summary>This should help prevent the total label's clipping on the right side</summary>
        public float GraphBarEnd
        {
            get {
                //bool addZero = TotalLabel.Text.Contains(".") && TotalLabel.Text.Split('.')[1].Length < 2;
                //return 6 * (TotalLabel.Text.Length + (addZero ? 1 : 0)) - 20;
                //return (float)(TotalLabel.ActualWidth - (TotalLabel.Margin.Left + Math.Abs(TotalLabel.Margin.Right)));
                return (float)(TotalLabel.ActualWidth /*- 20*/);
            }
        }
        public float SecondLastColumnWidth
        {
            get
            {
                return (float)MainPage.Instance.ComparisonGraph.ComparisonGraph.EndPaddingColumn.ActualWidth;
            }
        }

        private void ChangedSize(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (ActualWidth > (GraphBarStart + 8 + SecondLastColumnWidth/*+ GraphBarEnd*/))//170
            {
                if (NameGridCol.Width == null || NameGridCol.Width.Value != GraphBarStart)
                    NameGridCol.Width = new GridLength(GraphBarStart);
                if (NameGrid.Width != GraphBarStart)
                    NameGrid.Width = GraphBarStart;
                for (int i = PositiveStack.Children.Count - 1; i >= 0; i--)
                    if (PositiveStack.Children[i] is ComparisonGraphBar && !rects.Contains(PositiveStack.Children[i] as ComparisonGraphBar))
                        PositiveStack.Children.RemoveAt(i);
                for (int i = NegativeStack.Children.Count - 1; i >= 0; i--)
                    if (NegativeStack.Children[i] is ComparisonGraphBar && !rects.Contains(NegativeStack.Children[i] as ComparisonGraphBar))
                        NegativeStack.Children.RemoveAt(i);
                //PositiveStack.Children.Clear();
                //NegativeStack.Children.Clear();

                int minTick = (int)(-MinScale / (MaxScale - MinScale) * 8);
                int maxTick = (int)( MaxScale / (MaxScale - MinScale) * 8);

                if (minTick == 0) NegativeStack.Visibility = Visibility.Collapsed;
                else
                {
                    Grid.SetColumnSpan(NegativeStack, minTick);
                    NegativeStack.Visibility = Visibility.Visible;
                }
                if (maxTick == 0) PositiveStack.Visibility = Visibility.Collapsed;
                else
                {
                    Grid.SetColumn(PositiveStack, minTick + 1);
                    Grid.SetColumnSpan(PositiveStack, maxTick +1 /*+ 1*/);
                    PositiveStack.Visibility = Visibility.Visible;
                }
                int negIndex = 0, posIndex = 0;
                for (int i = 0; i < values.Count; i++)
                {
                    // Your chart broke, so lets handle it so it doesn't crash
                    if (float.IsNaN(values[i]) || float.IsInfinity(values[i])) { values[i] = 0f; }
                    if (values[i] > 0) {
                        int newWidth = (int)Math.Round((ActualWidth - (GraphBarStart + 9 + SecondLastColumnWidth /*+ GraphBarEnd*/)) * (values[i] / (MaxScale - MinScale))); // 171
                        if ((int)rects[i].Width != newWidth)
                            rects[i].Width = newWidth;
                        AddRectToStack(rects[i], true, posIndex++);
                    } else {
                        int newWidth = (int)Math.Round((ActualWidth - (GraphBarStart + 9 + SecondLastColumnWidth /*+ GraphBarEnd*/)) * (-values[i] / (MaxScale - MinScale))); // 171
                        if ((int)rects[i].Width != newWidth)
                            rects[i].Width = newWidth;
                        AddRectToStack(rects[i], false, negIndex++);
                    }
                }
                if (!PositiveStack.Children.Contains(TotalLabel))
                    PositiveStack.Children.Add(TotalLabel);
            }
        }

        private void AddRectToStack(ComparisonGraphBar rect, bool positive, int index)
        {
            if (positive)
            {
                if (NegativeStack.Children.Contains(rect))
                    NegativeStack.Children.Remove(rect);
                if (!PositiveStack.Children.Contains(rect))
                    PositiveStack.Children.Insert(index, rect);
            }
            else
            {
                if (PositiveStack.Children.Contains(rect))
                    PositiveStack.Children.Remove(rect);
                if (!NegativeStack.Children.Contains(rect))
                    NegativeStack.Children.Insert(index, rect);
            }
        }

        private void AvailableClicked(object sender, MouseButtonEventArgs e)
        {
            if (ItemInstance != null && ItemInstance.Id != 0)
                Character.ToggleItemAvailability(ItemInstance, (Keyboard.Modifiers & ModifierKeys.Shift) == 0);
            else if (OtherItem != null && OtherItem.Id != 0)
                Character.ToggleItemAvailability(OtherItem, (Keyboard.Modifiers & ModifierKeys.Shift) == 0);

            e.Handled = true;
        }

        #region Context Menu Items
        private void EditItem(object sender, RoutedEventArgs e)
        {
            new ItemEditor() { CurrentItem = ItemInstance.Item }.Show();
        }

#if SILVERLIGHT
        public class MyHyperlinkButton : HyperlinkButton {
            public MyHyperlinkButton(string navigateUri)
            {
                base.NavigateUri = new Uri(navigateUri);
                TargetName = "_blank";
            }
            public void ClickMe() { base.OnClick(); }
        }
#endif

        private void OpenInWowhead(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            if (App.Current.IsRunningOutOfBrowser) {
                MyHyperlinkButton button = new MyHyperlinkButton("http://www.wowhead.com/?item=" + ItemInstance.Id);
                button.ClickMe();
            } else {
                System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.wowhead.com/?item=" + ItemInstance.Id), "_blank");
            }
#else
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("http://www.wowhead.com/?item=" + ItemInstance.Id));
#endif
        }

        private void RefreshItemFromArmory(object sender, RoutedEventArgs e)
        {
            if (ItemInstance != null)
            {
                Item.LoadFromId(ItemInstance.Id, true, true, false, false);
            }
            else
            {
                Item.LoadFromId(OtherItem.Id, true, true, false, false);
            }
        }

        private void RefreshItemFromWowhead(object sender, RoutedEventArgs e)
        {
            if (ItemInstance != null)
            {
                Item.LoadFromId(ItemInstance.Id, true, true, true, false);
            }
            else
            {
                Item.LoadFromId(OtherItem.Id, true, true, true, false);
            }
        }

        private void EquipItem(object sender, RoutedEventArgs e)
        {
            if (MainPage.Instance.ComparisonGraph.CurrentGraph.Split('|')[0] == "Direct Upgrades"
                || MainPage.Instance.ComparisonGraph.CurrentGraph.Split('|')[0] == "Search Results"
                || MainPage.Instance.ComparisonGraph.CurrentGraph.Split('|')[0] == "Equipped"
                || MainPage.Instance.ComparisonGraph.CurrentGraph.Split('|')[0] == "Available")
            {
                Character[Character.GetCharacterSlotByItemSlot(ItemInstance.Slot)] = ItemInstance;
            } else {
                Character[Slot] = ItemInstance;
            }
        }

        private void EquipUpgradeSet(object sender, RoutedEventArgs e)
        {
            var c = NameGrid.Tag as ComparisonCalculationBase;
            if (c != null && c.CharacterItems != null)
            {
                Character.IsLoading = true;
                Character.SetItems(c.CharacterItems, false);
                Character.IsLoading = false;
                Character.OnCalculationsInvalidated();
            }
        }

        private void AddCustomGemming(object sender, RoutedEventArgs e)
        {
            CustomItemInstance custom = new CustomItemInstance(Character, ItemInstance);
            custom.Show();
        }

        private void DeleteCustomGemming(object sender, RoutedEventArgs e)
        {
            Character.CustomItemInstances.Remove(ItemInstance);
            ItemCache.OnItemsChanged();
        }

        private void EvaluateUpgrade(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            OptimizeWindow optimizer = new OptimizeWindow(Character);
            optimizer.Show();
            optimizer.EvaluateUpgrades(new Optimizer.SuffixItem() { Item = ItemInstance.Item, RandomSuffixId = ItemInstance.RandomSuffixId });
#else
            // in WPF show blocks until window is closed
            OptimizeWindow optimizer = new OptimizeWindow(Character);
            optimizer.EvaluateUpgrades(new Optimizer.SuffixItem() { Item = ItemInstance.Item, RandomSuffixId = ItemInstance.RandomSuffixId });
            optimizer.Show();
#endif
        }
        private void EvaluateUpgradesbySlot(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            OptimizeWindow optimizer = new OptimizeWindow(Character);
            optimizer.Show();
            optimizer.EvaluateUpgradesBySlot(new Optimizer.SuffixItem() { Item = ItemInstance.Item, RandomSuffixId = ItemInstance.RandomSuffixId });
#else
            // in WPF show blocks until window is closed
            OptimizeWindow optimizer = new OptimizeWindow(Character);
            optimizer.EvaluateUpgradesBySlot(new Optimizer.SuffixItem() { Item = ItemInstance.Item, RandomSuffixId = ItemInstance.RandomSuffixId });
            optimizer.Show();
#endif
        }

#if SILVERLIGHT
        private void ContextMenuItem_Opened(object sender, RoutedEventArgs e)
        {
            if (ItemInstance == null)
                ContextMenuItem.IsOpen = false;
            var c = NameGrid.Tag as ComparisonCalculationBase;
            if (c != null && c.CharacterItems != null)
            {
                ContextEquipUpgradeSet.Visibility = Visibility.Visible;
            }
            else
            {
                ContextEquipUpgradeSet.Visibility = Visibility.Collapsed;
            }
        }
#else
        private void ContextMenuItem_Opening(object sender, ContextMenuEventArgs e)
        {
            if (ItemInstance == null)
                e.Handled = true;
            var c = NameGrid.Tag as ComparisonCalculationBase;
            if (c != null && c.CharacterItems != null)
            {
                ContextEquipUpgradeSet.Visibility = Visibility.Visible;
            }
            else
            {
                ContextEquipUpgradeSet.Visibility = Visibility.Collapsed;
            }
        }
#endif


        private void ContextRemoveItemFromUpgradeList_Click(object sender, RoutedEventArgs e)
        {
            UpgradesComparison uc = MainPage.Instance.DG_UpgradesComparison;
            if (uc == null) { return; }
            foreach (CharacterSlot slot in Character.EquippableCharacterSlots)
            {
                // Fix it each of the slot calcs
                if (slot == CharacterSlot.Finger2 || slot == CharacterSlot.Trinket2 || slot == CharacterSlot.Tabard || slot == CharacterSlot.Shirt) { continue; }
                string key = "Gear." + slot.ToString();
                List<Optimizer.ComparisonCalculationUpgrades> list = uc.itemCalculations[key].ToList();
                list.RemoveAll(gg => gg.ItemInstance == this.ItemInstance);
                uc.itemCalculations[key] = list.ToArray();
            }
            {
                // Fix it in the All calcs
                string key = "Gear.All";
                List<Optimizer.ComparisonCalculationUpgrades> list = uc.itemCalculations[key].ToList();
                list.RemoveAll(gg => gg.ItemInstance == this.ItemInstance);
                uc.itemCalculations[key] = list.ToArray();
            }
            uc.UpdateGraph();
        }
        #endregion
    }
}
