﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace Rawr.UI
{
    public partial class ItemListControl : UserControl
    {

        #region Getters/Setters
        private bool isPopulated;
        public bool IsPopulated {
            get { return isPopulated; }
            set { isPopulated = value; if (!isPopulated) { if (IsShown) BuildListItems(); } }
        }

        public bool IsEnchantList { get; set; }
        public bool IsReforgeList { get; set; }
        public bool IsTinkeringList { get; set; }
        private bool IsGemList { get { return GemSlot == CharacterSlot.Gems || GemSlot == CharacterSlot.Metas; } }
        private bool IsCogwheelList { get { return GemSlot == CharacterSlot.Cogwheels; } }
        private bool IsHydraulicList { get { return GemSlot == CharacterSlot.Hydraulics; } }
        private bool isAnItemsGem = false;
        public bool IsAnItemsGem { get { return isAnItemsGem; } set { isAnItemsGem = true; } }

        private CharacterSlot _slot;
        public CharacterSlot Slot
        {
            get { return _slot; }
            set { _slot = value; IsPopulated = false; }
        }

        private CharacterSlot _gemSlot;
        public CharacterSlot GemSlot
        {
            get { return _gemSlot; }
            set { _gemSlot = value; IsPopulated = false; }
        }

        private int _gemIndex;
        /// <summary>
        /// GemIndex should be index of the gem evaluated if used in combination with Slot or 0 if evaluated on its own.
        /// </summary>
        public int GemIndex
        {
            get { return _gemIndex; }
            set { _gemIndex = value; IsPopulated = false; }
        }

        private Character character;
        public Character Character {
            get { return character; }
            set {
                if (character != null) character.CalculationsInvalidated -= new EventHandler(character_CalculationsInvalidated);
                character = value;
                character.CalculationsInvalidated += new EventHandler(character_CalculationsInvalidated);
                IsPopulated = false;
            }
        }

        private Item selectedItem;
        public Item SelectedItem
        {
            get { return selectedItem; }
            set {
                selectedItem = value;
                IsPopulated = false;
                if (SelectedItemChanged != null) SelectedItemChanged(this, EventArgs.Empty);
            }
        }

        public event EventHandler SelectedItemChanged;
        public event EventHandler SelectedItemsGemChanged;

        private void character_CalculationsInvalidated(object sender, EventArgs e)
        {
            IsPopulated = false;
        }

        private bool isShown;
        public bool IsShown
        {
            get { return isShown; }
            set
            {
                isShown = value;
                if (isShown && !IsPopulated) BuildListItems();
                if (isShown) listBoxItems.Focus();
            }
        }

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", 
            typeof(ItemListItemCollection), 
            typeof(ItemListControl),
            new PropertyMetadata(null));
        public ItemListItemCollection Items
        {
            get
            {
                ItemListItemCollection items = (ItemListItemCollection)GetValue(ItemsProperty);
                if (items == null)
                {
                    Items = items = new ItemListItemCollection();
                }
                return items;
            }
            set { SetValue(ItemsProperty, value); }
        }
        #endregion

        private bool _buildingListItems = false;
        private void BuildListItems()
        {
            try
            {
                _buildingListItems = true;
                if (Character == null) return;
                bool seenEquippedItem = (Character[Slot] == null);

                List<ComparisonCalculationBase> itemCalculations = new List<ComparisonCalculationBase>();
                if (IsEnchantList)
                {
                    CharacterCalculationsBase current = Calculations.GetCharacterCalculations(Character);
                    if (Character != null && current != null)
                    {
                        itemCalculations = Calculations.GetEnchantCalculations(Item.GetItemSlotByCharacterSlot(Slot), Character, current, false);
                    }
                }
                else if (IsTinkeringList)
                {
                    CharacterCalculationsBase current = Calculations.GetCharacterCalculations(Character);
                    if (Character != null && current != null)
                    {
                        itemCalculations = Calculations.GetTinkeringCalculations(Item.GetItemSlotByCharacterSlot(Slot), Character, current, false);
                    }
                }
                else if (IsReforgeList)
                {
                    CharacterCalculationsBase current = Calculations.GetCharacterCalculations(Character);
                    if (Character != null && current != null)
                    {
                        itemCalculations = Calculations.GetReforgeCalculations(Slot, Character, current, false);
                    }
                }
                else if (IsCogwheelList)
                {
                    Calculations.ClearCache();
                    List<Item> relevantItems = Character.GetRelevantItems(GemSlot);
                    foreach (Item item in relevantItems)
                    {
                        ComparisonCalculationBase itemCalc = Calculations.GetItemGemCalculations(item, Character, Slot, GemIndex);
                        if (SelectedItem != null && SelectedItem.Id == item.Id)
                        {
                            itemCalc.Equipped = true;
                            seenEquippedItem = true;
                        }
                        itemCalculations.Add(itemCalc);
                    }
                    if (!seenEquippedItem) itemCalculations.Add(Calculations.GetItemGemCalculations(SelectedItem, Character, Slot, GemIndex));
                }
                else if (IsHydraulicList)
                {
                    Calculations.ClearCache();
                    List<Item> relevantItems = Character.GetRelevantItems(GemSlot);
                    foreach (Item item in relevantItems)
                    {
                        ComparisonCalculationBase itemCalc = Calculations.GetItemGemCalculations(item, Character, Slot, GemIndex);
                        if (SelectedItem != null && SelectedItem.Id == item.Id)
                        {
                            itemCalc.Equipped = true;
                            seenEquippedItem = true;
                        }
                        itemCalculations.Add(itemCalc);
                    }
                    if (!seenEquippedItem) itemCalculations.Add(Calculations.GetItemGemCalculations(SelectedItem, Character, Slot, GemIndex));
                }
                else if (IsGemList)
                {
                    Calculations.ClearCache();
                    List<Item> relevantItems = Character.GetRelevantItems(GemSlot);
                    foreach (Item item in relevantItems)
                    {
                        ComparisonCalculationBase itemCalc = Calculations.GetItemGemCalculations(item, Character, Slot, GemIndex);
                        if (SelectedItem != null && SelectedItem.Id == item.Id)
                        {
                            itemCalc.Equipped = true;
                            seenEquippedItem = true;
                        }
                        itemCalculations.Add(itemCalc);
                    }
                    if (!seenEquippedItem) itemCalculations.Add(Calculations.GetItemGemCalculations(SelectedItem, Character, Slot, GemIndex));
                }
                else
                {
                    Calculations.ClearCache();
                    List<ItemInstance> relevantItemInstances = Character.GetRelevantItemInstances(Slot);
                    if (relevantItemInstances.Count > 0)
                    {
                        foreach (ItemInstance item in relevantItemInstances)
                        {
                            if (!seenEquippedItem && Character[Slot].Equals(item)) seenEquippedItem = true;
                            itemCalculations.Add(Calculations.GetItemCalculations((ItemInstance)item, Character, Slot));
                        }
                    }
                    if (!seenEquippedItem) itemCalculations.Add(Calculations.GetItemCalculations(Character[Slot], Character, Slot));
                }

                float maxValue = itemCalculations.Count == 0 ? 100f : itemCalculations.Max(c => c.OverallPoints);

                Items.Clear();
                List<ItemListItem> itemListItems = new List<ItemListItem>();
                ItemListItem selectedListItem = null;
                int selectedEnchantId = 0, selectedReforgingId = 0, selectedTinkeringId = 0;
                if (IsEnchantList)
                {
                    Enchant selectedEnchant = Character.GetEnchantBySlot(Slot);
                    if (selectedEnchant != null) selectedEnchantId = selectedEnchant.Id;
                }
                else if (IsTinkeringList)
                {
                    Tinkering selectedTinkering = Character.GetTinkeringBySlot(Slot);
                    if (selectedTinkering != null) selectedTinkeringId = selectedTinkering.Id;
                }
                else if (IsReforgeList)
                {
                    Reforging selectedReforging = Character.GetReforgingBySlot(Slot);
                    if (selectedReforging != null) selectedReforgingId = selectedReforging.Id;
                }
                else if (IsGemList)
                {
                    ComparisonCalculationBase emptyCalcs = Calculations.CreateNewComparisonCalculation();
                    emptyCalcs.Name = "Empty";
                    emptyCalcs.Item = new Item();
                    emptyCalcs.Item.Name = "Empty";
                    emptyCalcs.Item.Id = -1;
                    emptyCalcs.Equipped = SelectedItem == null;
                    itemCalculations.Add(emptyCalcs);
                }
                else
                {
                    ComparisonCalculationBase emptyCalcs = Calculations.CreateNewComparisonCalculation();
                    emptyCalcs.Name = "Empty";
                    emptyCalcs.Item = new Item();
                    emptyCalcs.Item.Name = "Empty";
                    emptyCalcs.ItemInstance = new ItemInstance();
                    emptyCalcs.ItemInstance.Id = -1;
                    emptyCalcs.Equipped = Character[Slot] == null;
                    itemCalculations.Add(emptyCalcs);
                }
                foreach (ComparisonCalculationBase calc in itemCalculations)
                {
                    ItemListItem itemListItem = new ItemListItem(calc, maxValue, 344d);
                    itemListItems.Add(itemListItem);
                    if (IsEnchantList)
                    {
                        if (itemListItem.EnchantId == selectedEnchantId)
                            selectedListItem = itemListItem;
                    }
                    else if (IsTinkeringList)
                    {
                        if (itemListItem.TinkeringId == selectedTinkeringId)
                            selectedListItem = itemListItem;
                    }
                    else if (IsReforgeList)
                    {
                        if (itemListItem.ReforgeId == selectedReforgingId)
                            selectedListItem = itemListItem;
                    }
                    else if (IsGemList || IsHydraulicList || IsCogwheelList)
                    {
                        if (calc.Item == null)
                        {
                            if (SelectedItem == null)
                            {
                                selectedListItem = itemListItem;
                            }
                        }
                        else
                        {
                            if ((SelectedItem != null && calc.Item.Id == SelectedItem.Id) || (calc.Item.Id == -1 && SelectedItem == null))
                                selectedListItem = itemListItem;
                        }
                    }
                    else
                    {
                        if (calc.ItemInstance == Character[Slot] || (calc.ItemInstance.Id == -1 && Character[Slot] == null))
                            selectedListItem = itemListItem;
                    }
                }
                Items.AddRange(itemListItems);
                listBoxItems.SelectedItem = selectedListItem;
                listBoxItems.Focus();
                IsPopulated = true;
            }
            finally
            {
                _buildingListItems = false;
            }
        }

        private void BuildSorts()
        {
            comboBoxSort.Items.Clear();
            comboBoxSort.Items.Add("Alphabetical");
            comboBoxSort.Items.Add("Overall");
            if (Calculations.Instance != null)
            {
                foreach (var kvp in Calculations.SubPointNameColors) comboBoxSort.Items.Add(kvp.Key);
            }
            comboBoxSort.SelectedIndex = 1;
        }

        private void item_MouseLeave(object sender, MouseEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            MainPage.Tooltip.Hide();
        }

        private void item_MouseEnter(object sender, MouseEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ItemListItem listItem = fe.DataContext as ItemListItem;

            if (listItem.ItemInstance != null) MainPage.Tooltip.ItemInstance = listItem.ItemInstance;
            else MainPage.Tooltip.Item = listItem.Item;

            MainPage.Tooltip.Show(fe, 367, 0);
        }

        #region Context Menu
        private void MainButton_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            //TODO: *Usually* this is enough, but sometimes the positioning gets off, and it stays where the last context menu was.
            //Not really sure why, but I guess we should set the context menu location here too?
            ContextMenuService.GetContextMenu((Rectangle)sender).IsOpen = true;
            if (e != null) { e.Handled = true; }
        }
        private void ContextMenuItem_Opened(object sender, RoutedEventArgs e)
        {
            //FrameworkElement fe = sender as FrameworkElement;
            //ItemListItem listItem = fe.DataContext as ItemListItem;
            //if (listItem.ItemInstance == null) { ContextMenuItem.IsOpen = false; }
            //else { ContextItemName.Header = listItem.ItemInstance.Name; }
        }
        private void EditItem(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ItemListItem listItem = fe.DataContext as ItemListItem;
            new ItemEditor() { CurrentItem = listItem.ItemInstance.Item }.Show();
        }
#if SILVERLIGHT
        public class MyHyperlinkButton : HyperlinkButton
        {
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
            FrameworkElement fe = sender as FrameworkElement;
            ItemListItem listItem = fe.DataContext as ItemListItem;
#if SILVERLIGHT
            if (App.Current.IsRunningOutOfBrowser) {
                MyHyperlinkButton button = new MyHyperlinkButton("http://www.wowhead.com/?item=" + listItem.ItemInstance.Id);
                button.ClickMe();
            } else {
                System.Windows.Browser.HtmlPage.Window.Navigate(new Uri("http://www.wowhead.com/?item=" + listItem.ItemInstance.Id), "_blank");
            }
#else
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("http://www.wowhead.com/?item=" + listItem.ItemInstance.Id));
#endif
        }
        private void RefreshItemFromWowhead(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            ItemListItem listItem = fe.DataContext as ItemListItem;
            if (listItem.ItemInstance != null)
            {
                Item.LoadFromId(listItem.ItemInstance.Id, true, true, true, false);
            }
            else
            {
                Item.LoadFromId(listItem.Item.Id, true, true, true, false);
            }
        }
        #endregion

        private void item_Clicked(object sender, MouseButtonEventArgs e) {
            if (!_buildingListItems && IsEnchantList)
            {
                FrameworkElement fe = sender as FrameworkElement;
                ItemListItem listItem = fe.DataContext as ItemListItem;

                int selectedEnchantId = -1;
                Enchant selectedEnchant = Character.GetEnchantBySlot(Slot);
                if (selectedEnchant != null) selectedEnchantId = selectedEnchant.Id;

                if (listItem.EnchantId == selectedEnchantId) { this.Close(); }
            }
            if (!_buildingListItems && IsTinkeringList)
            {
                FrameworkElement fe = sender as FrameworkElement;
                ItemListItem listItem = fe.DataContext as ItemListItem;

                int selectedTinkeringId = -1;
                Tinkering selectedTinkering = Character.GetTinkeringBySlot(Slot);
                if (selectedTinkering != null) selectedTinkeringId = selectedTinkering.Id;

                if (listItem.TinkeringId == selectedTinkeringId) { this.Close(); }
            }
            if (!_buildingListItems && IsReforgeList)
            {
                FrameworkElement fe = sender as FrameworkElement;
                ItemListItem listItem = fe.DataContext as ItemListItem;

                int selectedReforgeId = -1;
                Reforging selectedReforging = Character.GetReforgingBySlot(Slot);
                if (selectedReforging != null) selectedReforgeId = selectedReforging.Id;

                if (listItem.ReforgeId == selectedReforgeId) { this.Close(); }
            }
        }

        public ItemListControl()
        {
            // Required to initialize variables
            InitializeComponent();

            this.DataContext = this;
            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);
            BuildSorts();
        }

        private void Calculations_ModelChanged(object sender, EventArgs e)
        {
            BuildSorts();
        }

        private void listBoxItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!_buildingListItems && ((ListBox)sender).SelectedItem != null)
            {
                try
                {
                    if (IsEnchantList)
                    {
                        ItemListItem listItem = ((ListBox)sender).SelectedItem as ItemListItem;
                        ItemInstance copy = Character[Slot].Clone();
                        copy.EnchantId = listItem.EnchantId;
                        IsShown = false;
                        IsPopulated = false;
                        Character[Slot] = copy;
                    }
                    else if (IsTinkeringList)
                    {
                        ItemListItem listItem = ((ListBox)sender).SelectedItem as ItemListItem;
                        ItemInstance copy = Character[Slot].Clone();
                        copy.TinkeringId = listItem.TinkeringId;
                        IsShown = false;
                        IsPopulated = false;
                        Character[Slot] = copy;
                    }
                    else if (IsReforgeList)
                    {
                        ItemListItem listItem = ((ListBox)sender).SelectedItem as ItemListItem;
                        ItemInstance copy = Character[Slot].Clone();
                        copy.ReforgeId = listItem.ReforgeId;
                        IsShown = false;
                        IsPopulated = false;
                        Character[Slot] = copy;
                    }
                    else if (IsGemList || IsCogwheelList || IsHydraulicList)
                    {
                        ItemListItem listItem = ((ListBox)sender).SelectedItem as ItemListItem;
                        IsShown = false;
                        IsPopulated = false;
                        SelectedItem = listItem.Item.Id == -1 ? null : listItem.Item;
                        if (IsAnItemsGem) { SelectedItemsGemChanged(this, new EventArgs()); }
                    }
                    else
                    {
                        ItemListItem listItem = ((ListBox)sender).SelectedItem as ItemListItem;
                        IsShown = false;
                        IsPopulated = false;
                        if (listItem != null)
                            Character[Slot] = listItem.ItemInstance.Id == -1 ? null : listItem.ItemInstance;
                    }
                }
                catch { }
                this.Close();
            }
        }

        //private void UserControl_LostFocus(object sender, RoutedEventArgs e)
        //{
        //    FrameworkElement focus = (App.GetFocusedElement() as FrameworkElement);
        //    if (focus is ComboBoxItem && comboBoxSort.Items.Contains(focus.DataContext))
        //        focus = comboBoxSort;
        //    DependencyObject parent = VisualTreeHelper.GetParent(focus);
        //    while (parent != null && parent != this)
        //        parent = VisualTreeHelper.GetParent(parent);
        //    if (parent == null)
        //    {
        //        //focus isn't within this control
        //        this.Close();
        //    }
        //}

        private void Background_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.OriginalSource == sender)
                Close();
        }

        private void Close()
        {
            this.IsShown = false;
            if (this.Parent is Popup)
                (this.Parent as Popup).IsOpen = false;
        }

        private void Filter_TextChanged(object sender, TextChangedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
        }

        private void comboBoxSort_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (comboBoxSort.SelectedIndex >= 0)
                Items.Sort = (ComparisonSort)(comboBoxSort.SelectedIndex - 2);
        }

        private void listBoxItems_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            int direction = Math.Sign(e.Delta);

            bool shiftKey = (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift; 
            
            int scrollAmount = (shiftKey // Holding Shift makes it jump 3 times as far for fast scrolling
                ? (direction < 0) ? ScrollAmount.LargeIncrement : ScrollAmount.LargeDecrement
                : (direction < 0) ? ScrollAmount.SmallIncrement : ScrollAmount.SmallDecrement);

#if SILVERLIGHT
            ScrollViewer scrollProvider = listBoxItems.GetScrollHost();
#else
            ScrollViewer scrollProvider = listBoxItems.FindName("ScrollViewer") as ScrollViewer;
#endif
            scrollProvider.ScrollToVerticalOffset(scrollProvider.VerticalOffset + scrollAmount);
        }

        private void Rectangle_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
    public static class ScrollAmount {
        private static int _NoAmount = 0;
        private static int _SmallDecrement = -4;
        private static int _SmallIncrement =  4;
        private static int _LargeDecrement = -4 * 3;
        private static int _LargeIncrement =  4 * 3;
        public static int NoAmount { get { return _NoAmount; } set {  _NoAmount = value; } }
        public static int SmallDecrement { get { return _SmallDecrement; } set { _SmallDecrement = value; } }
        public static int SmallIncrement { get { return _SmallIncrement; } set { _SmallIncrement = value; } }
        public static int LargeDecrement { get { return _LargeDecrement; } set { _LargeDecrement = value; } }
        public static int LargeIncrement { get { return _LargeIncrement; } set { _LargeIncrement = value; } }
    }
}