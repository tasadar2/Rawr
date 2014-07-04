﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
#if !SILVERLIGHT
using Microsoft.Win32;
#endif
using Rawr;

namespace Rawr.UI
{
    public partial class MainPage : UserControl
    {
        public MainPage() : this("Bear") { }

        public MainPage(string configModel)
        {
            Instance = this;
            InitializeComponent();
            if (App.Current.IsRunningOutOfBrowser) OfflineInstallButton.Visibility = Visibility.Collapsed;

            // Assign the Version number to the status bar
            Assembly asm = Assembly.GetExecutingAssembly();
            string version = "Debug";
            if (asm.FullName != null) {
                string[] parts = asm.FullName.Split(',');
                version = parts[1];
                while (version.Contains("Version=")) { version = version.Replace("Version=",""); }
                while (version.Contains(" ")) { version = version.Replace(" ", ""); }
            }
            VersionText.Text = string.Format("Rawr {0}", version);

            asyncCalculationCompleted = new SendOrPostCallback(AsyncCalculationCompleted);

            Tooltip = ItemTooltip;

            HeadButton.Slot = CharacterSlot.Head;
            NeckButton.Slot = CharacterSlot.Neck;
            ShoulderButton.Slot = CharacterSlot.Shoulders;
            BackButton.Slot = CharacterSlot.Back;
            ChestButton.Slot = CharacterSlot.Chest;
            ShirtButton.Slot = CharacterSlot.Shirt;
            TabardButton.Slot = CharacterSlot.Tabard;
            WristButton.Slot = CharacterSlot.Wrist;
            HandButton.Slot = CharacterSlot.Hands;
            BeltButton.Slot = CharacterSlot.Waist;
            LegButton.Slot = CharacterSlot.Legs;
            FeetButton.Slot = CharacterSlot.Feet;
            Ring1Button.Slot = CharacterSlot.Finger1;
            Ring2Button.Slot = CharacterSlot.Finger2;
            Trinket1Button.Slot = CharacterSlot.Trinket1;
            Trinket2Button.Slot = CharacterSlot.Trinket2;
            MainHandButton.Slot = CharacterSlot.MainHand;
            OffHandButton.Slot = CharacterSlot.OffHand;

            ModelCombo.ItemsSource = new string[] { "Death Knight:Blood", "Death Knight:Frost", "Death Knight:Unholy", "Druid:Balance", "Druid:Feral", "Druid:Guardian", "Druid:Restoration", "Hunter:Beast Mastery", "Hunter:Marksmanship", "Hunter:Survival", "Mage:Arcane", "Mage:Fire", "Mage:Frost", "Monk:Brewmaster", "Monk:Mistweaver", "Monk:Windwalker", "Paladin:Holy", "Paladin:Protection", "Paladin:Retribution", "Priest:Discipline", "Priest:Holy", "Priest:Shadow", "Rogue:Assasination", "Rogue:Combat", "Rogue:Subtlety", "Shaman:Elemental", "Shaman:Enhancement", "Shaman:Restoration", "Warlock:Affliction", "Warlock:Demonology", "Warlock:Destruction", "Warrior:Arms", "Warrior:Fury", "Warrior:Protection" };

            Calculations.ModelChanged += new EventHandler(Calculations_ModelChanged);

            Character c = new Character() { IsLoading = false };
            //c.CurrentModel = configModel;
            c.Class = Calculations.ModelClasses[c.CurrentModel];
            c.RecalculateSetBonuses(); // otherwise you'll get null ref exception
            Character = c;

            Calculations.ModelChanging += new EventHandler(Calculations_ModelChanging);
            ItemCache.Instance.ItemsChanged += new EventHandler(ItemCacheInstance_ItemsChanged);

            SetCompactModeUp();

#if !SILVERLIGHT
            WaitAndShowWelcomeScreen();
            WaitAndShowFirstTimeWebHelpWindow();
#else
            if (TMI_SaveAs != null) { TMI_SaveAs.Visibility = Visibility.Collapsed; }
#endif

            StatusMessaging.Ready = true;

            ResetItemCacheBecauseOfNewVersion();
            //BossHandlerHasAnIssue(null, null);
        }

        #region Variables
        public static ItemTooltip Tooltip { get; private set; }
        public static MainPage Instance { get; private set; }

        private bool _unsavedChanges = false;

        public bool UpgradeListOpen = false;

        private Status status;
        public Status Status {
            set { status = value; }
            get { return status ?? (status = new Status()); }
        }

        private ItemBrowser itemSearch = null;
        public ItemBrowser ItemSearch { get { return itemSearch ?? (itemSearch = new ItemBrowser()); } }

        private Character character;
        public Character Character
        {
            get { return character; }
            set
            {
                if (value != null)
                {
                    if (character != null)
                    {
                        character.CalculationsInvalidated -= new EventHandler(character_CalculationsInvalidated);
                        character.AvailableItemsChanged -= new Character.AvailableItemsChangedEventHandler(character_AvailableItemsChanged);
                        character.ClassChanged -= new EventHandler(character_ClassChanged);
                    }

                    character = value;
                    if (_batchCharacter != null && _batchCharacter.Character != character)
                    {
                        // we're loading a character that is not a batch character
                        _batchCharacter = null;
                    }
                    character.IsLoading = true;

                    DataContext = character;
                    // ugly, but works...                    
                    foreach (Rawr.UI.ClassModelPicker.ClassModelPickerItem i in CMP_ClassModel.Items)
                    {
                        if (Character.Class == Calculations.CharacterClasses[i.Header])
                        {
                            foreach (Rawr.UI.ClassModelPicker.ClassModelPickerItem j in i.Items)
                            {
                                if (j.Header == Character.CurrentTalents.SpecializationName)
                                {
                                    CMP_ClassModel.PrimaryItem = i;
                                    CMP_ClassModel.PrimaryItem.SelectedItem = j;
                                    CMP_ClassModel.TextBlockClassModelButtonPrimary.Text = CMP_ClassModel.PrimaryItem.Header;
                                    CMP_ClassModel.TextBlockClassModelButtonSecondary.Text = CMP_ClassModel.PrimaryItem.SelectedItem.Header;
                                    goto CMPDONE;
                                }
                            }
                        }
                    }
                CMPDONE:
                    Calculations.LoadModel(Calculations.Models[character.CurrentModel]);
                    Calculations.ClearCache();
                    Calculations.CalculationOptionsPanel.Character = character;
                    HeadButton.Character = character;
                    NeckButton.Character = character;
                    ShoulderButton.Character = character;
                    BackButton.Character = character;
                    ChestButton.Character = character;
                    ShirtButton.Character = character;
                    TabardButton.Character = character;
                    WristButton.Character = character;
                    HandButton.Character = character;
                    BeltButton.Character = character;
                    LegButton.Character = character;
                    FeetButton.Character = character;
                    Ring1Button.Character = character;
                    Ring2Button.Character = character;
                    Trinket1Button.Character = character;
                    Trinket2Button.Character = character;
                    MainHandButton.Character = character;
                    OffHandButton.Character = character;
                    TalentPicker.Character = character;

                    BuffControl.Character = character;

                    BossPicker.Character = character;

                    character.CalculationsInvalidated += new EventHandler(character_CalculationsInvalidated);
                    character.ClassChanged += new EventHandler(character_ClassChanged);
                    character.AvailableItemsChanged += new Character.AvailableItemsChangedEventHandler(character_AvailableItemsChanged);

                    character_ClassChanged(this, EventArgs.Empty);

                    ItemCache.Instance.Character = character;

                    WristButton.CK_BSSocket.SetBinding(CheckBox.IsCheckedProperty, new System.Windows.Data.Binding("WristBlacksmithingSocketEnabled"));
                    HandButton.CK_BSSocket.SetBinding(CheckBox.IsCheckedProperty, new System.Windows.Data.Binding("HandsBlacksmithingSocketEnabled"));
                    BeltButton.CK_BSSocket.SetBinding(CheckBox.IsCheckedProperty, new System.Windows.Data.Binding("WaistBlacksmithingSocketEnabled"));

                    LB_Models.Text = ModelStatus(character.CurrentModel);

                    // we might be loading a character with different filters, make sure to invalidate item cache
                    ItemCache.OnItemsChanged();

                    Character.IsLoading = false;
                    character_CalculationsInvalidated(this, EventArgs.Empty);

                    // it's important that ComparisonGraph hooks CalculationsInvalidated event after we do because
                    // the reference calculation must happen first
                    // it also triggers update graph when we change character so make sure we've already done 
                    // the reference calculation
                    ComparisonGraph.Character = character;
                }
            }
        }
        #endregion

        private Color _ModelStatusColor = SystemColors.WindowTextColor;
        private Color ModelStatusColor { get { return _ModelStatusColor; } set { _ModelStatusColor = value; LB_Models.Foreground = new SolidColorBrush(value); } }
        private string ModelStatus(string Model) {
            string retVal = "The Model Status is Unknown, please see the Model Status Page.";
            string format = "[" + Model + " | Maintained: {0}, MoP Ready: {1}, Developer(s): {2}]";
            ModelStatusColor = SystemColors.WindowTextColor;

            string[] maint = { "Never", "New Dev", "Periodically", "Actively" };
            string[] funct = { "Not", "Partially", "Mostly", "Fully" };

            switch (Model) {
                // Druids
                case "Bear"     : { retVal    = string.Format(format, maint[3], funct[3], "Hinalover"); break; }
                case "Feral"      : { retVal    = string.Format(format, maint[3], funct[2], "Hinalover"); break; }
                case "Moonkin"  : { retVal    = string.Format(format, maint[3], funct[3], "Dopefish"); break; }
                case "Tree"     : { retVal    = string.Format(format, maint[2], funct[1], "Wildebees"); break; }
                // Death Knights
                case "DPSDK"    : { retVal    = string.Format(format, maint[3], funct[1], "Shazear"); break; }
                case "TankDK"   : { retVal    = string.Format(format, maint[3], funct[1], "Shazear"); break; }
                // Hunters
                case "Hunter"   : { retVal    = string.Format(format, maint[1], funct[1], "OperatorOverload"); break; }
                // Mages
                case "Mage"     : { retVal    = string.Format(format, maint[0], funct[2], "None"); break; }
                // Monks
                case "Brewmaster": { retVal = string.Format(format, maint[1], funct[0], "Hinalover"); break; }
                case "Mistweaver": { retVal = string.Format(format, maint[0], funct[0], "None"); break; }
                case "Windwalker": { retVal = string.Format(format, maint[1], funct[0], "Hinalover"); break; }
                // Paladins
                case "Healadin" : { retVal    = string.Format(format, maint[0], funct[0], "None"); break; }
                case "ProtPaladin":{retVal    = string.Format(format, maint[3], funct[2], "Dopefish"); break; }
                case "Retribution":{retVal    = string.Format(format, maint[0], funct[0], "None"); break; }
                // Priests
                case "HealPriest":{retVal     = string.Format(format, maint[0], funct[0], "None"); break; }
                case "ShadowPriest":{retVal   = string.Format(format, maint[0], funct[0], "None"); break; }
                // Rogues
                case "Rogue"    : { retVal    = string.Format(format, maint[0], funct[0], "None"); break; }
                // Shamans
                case "Elemental": { retVal    = string.Format(format, maint[0], funct[0], "None"); break; }
                case "Enhance"  : { retVal    = string.Format(format, maint[2], funct[0], "TimeToDance"); break; }
                case "RestoSham": { retVal    = string.Format(format, maint[2], funct[0], "Antivyris"); break; }
                // Warriors
                case "DPSWarr"  : { retVal    = string.Format(format, maint[0], funct[0], "None"); break; }
                case "ProtWarr" : { retVal    = string.Format(format, maint[0], funct[0], "None"); break; }
                // Warlocks
                case "Warlock"  : { retVal    = string.Format(format, maint[0], funct[0], "None"); break; }
                default: { break; }
            }

            if        (retVal.Contains("Not")) {
                ModelStatusColor = Color.FromArgb(255, 255, 000, 000);
                retVal += " WARNING! THIS MODEL *IS NOT* MoP READY!";
            } else if (retVal.Contains("Partially")) {
                ModelStatusColor = Color.FromArgb(255, 255, 000, 255);
                retVal += " WARNING! THIS MODEL *MAY NOT* BE MoP READY!";
            } else if (retVal.Contains("Mostly")) {
                ModelStatusColor = Color.FromArgb(255, 000, 000, 255);
            } else if (retVal.Contains("Fully")) {
                ModelStatusColor = Color.FromArgb(255, 000, 128, 000);
            } else {
                ModelStatusColor = SystemColors.WindowTextColor;
            }

            return retVal;
        }

        #region Events
        void character_AvailableItemsChanged(object sender, EventArgs e)
        {
            _unsavedChanges = true;
            SetTitle();
        }
        private void character_ClassChanged(object sender, EventArgs e)
        {
            ClassCombo.SelectedIndex = Character.ClassIndex;
        }
        private void ClassCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Character.ClassIndex = ClassCombo.SelectedIndex;
        }
        public void ProfChanged(object sender, SelectionChangedEventArgs e) {
            if (character != null && !character.IsLoading)
            {
                character_CalculationsInvalidated(null, null);
            }
            RepopulateBasicInfosLabel();
        }
        private void BasicInfoField_TextChanged(object sender, TextChangedEventArgs e) { RepopulateBasicInfosLabel(); }
        private void BasicInfoField_SelectionChanged(object sender, SelectionChangedEventArgs e) { RepopulateBasicInfosLabel(); }
        private void RepopulateBasicInfosLabel()
        {
            if (Character != null)
            {
                LB_BasicInfos.Text = string.Format("{0}@{1}-{2}\r\n{3} {4}-{5}, {6}/{7}",
                    (!String.IsNullOrEmpty(Character.Name)) ? Character.Name : "Unnamed",
                    Character.Region.ToString().Substring(0, 2),
                    (!String.IsNullOrEmpty(Character.Realm)) ? Character.Realm : "NoServer",
                    Character.Race, Character.Class, Character.CurrentTalents.SpecializationName,
                    Character.PrimaryProfession, Character.SecondaryProfession);
                //_unsavedChanges = true;
            }
            else
            {
                LB_BasicInfos.Text = "";
            }
        }
        public void character_CalculationsInvalidated(object sender, EventArgs e)
        {
#if DEBUG
            DateTime start = DateTime.Now;
#endif
            this.Cursor = Cursors.Wait;
            if (asyncCalculation != null)
            {
                CharacterCalculationsBase oldCalcs = referenceCalculation;
                referenceCalculation = null;
                oldCalcs.CancelAsynchronousCharacterDisplayCalculation();
                asyncCalculation = null;
            }
            if (!_unsavedChanges && !(sender == null || sender == this))
            {
                _unsavedChanges = true;
                SetTitle();
            }
            referenceCalculation = Calculations.GetCharacterCalculations(character, null, true, true, true);
            UpdateDisplayCalculationValues(referenceCalculation.GetCharacterDisplayCalculationValues(), referenceCalculation);
            if (Character.PrimaryProfession == Profession.Blacksmithing || Character.SecondaryProfession == Profession.Blacksmithing) {
                HandButton.EnableBSSocket = true;
                WristButton.EnableBSSocket = true;
            } else {
                HandButton.BSSocketIsChecked = false;
                WristButton.BSSocketIsChecked = false;
                HandButton.EnableBSSocket = false;
                WristButton.EnableBSSocket = false;
            }
            if (referenceCalculation.RequiresAsynchronousDisplayCalculation)
            {
                asyncCalculation = AsyncOperationManager.CreateOperation(null);
                ThreadPool.QueueUserWorkItem(delegate
                {
                    AsyncCalculationStart(referenceCalculation, asyncCalculation);
                });
            }
            this.Cursor = Cursors.Arrow;
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("Finished MainPage CalculationsInvalidated: {0}ms", DateTime.Now.Subtract(start).TotalMilliseconds));
#endif
        }
        private void Calculations_ModelChanged(object sender, EventArgs e)
        {
            foreach (string item in ModelCombo.Items)
            {
                if (item == Character.CurrentModel)
                {
                    ModelCombo.SelectedItem = item;
                }
            }
            if (CMP_ClassModel != null)
            {
                try {
                    bool found = false;
                    foreach (Rawr.UI.ClassModelPicker.ClassModelPickerItem i in CMP_ClassModel.Items)
                    {
                        foreach (Rawr.UI.ClassModelPicker.ClassModelPickerItem j in i.Items)
                        {
                            if (j.Header == Character.CurrentModel)
                            {
                                CMP_ClassModel.PrimaryItem = i;
                                CMP_ClassModel.PrimaryItem.SelectedItem = j;
                                CMP_ClassModel.TextBlockClassModelButtonPrimary.Text = CMP_ClassModel.PrimaryItem.Header;
                                CMP_ClassModel.TextBlockClassModelButtonSecondary.Text = CMP_ClassModel.PrimaryItem.SelectedItem.Header;
                                found = true;
                                break;
                            }
                        }
                        if (found) break;
                    }
                } catch (Exception ex) {
                    new Base.ErrorBox()
                    {
                        Title = "Error Selecting Class",
                        Function = "Calculations_ModelChanged()",
                        TheException = ex,
                    }.Show();
                }
            }
            UpdateRaceLimitations();
            OptionsView.Content = Calculations.CalculationOptionsPanel;
            if (!Character.IsLoading) Character = Character;
        }
        private void Calculations_ModelChanging(object sender, EventArgs e)
        {
            Character.SerializeCalculationOptions();
        }
        private void ModelCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!Character.IsLoading)
            {
                string key = (string)ModelCombo.SelectedItem;
                string[] k = key.Split(':');                
                Character.Class = Calculations.CharacterClasses[k[0]];
                Character.CurrentTalents.SpecializationName = k[1];
                Character.UpdateCurrentModel();
                var newModel = Calculations.GetModel(Character.CurrentModel);
                if (Calculations.Instance != newModel)
                {
                    Calculations.LoadModel(newModel);
                }
                else
                {
                    // no model change, so force graph recalc
                    Character.OnCalculationsInvalidated();
                }
            }
            RepopulateBasicInfosLabel();
        }
        private void ItemCacheInstance_ItemsChanged(object sender, EventArgs e)
        {
            //Character.InvalidateItemInstances(); // this is already called by OnCalculationsInvalidated
            Character.OnCalculationsInvalidated();
            /*if (!Character.IsLoading) // this is already called by OnCalculationsInvalidated
            {
                ComparisonGraph.UpdateGraph();
            }*/
        }
        #endregion

        #region Batch Tools
        private Character _storedCharacter;
        private BatchCharacter _batchCharacter;
        public void BatchCharacterSaved(BatchCharacter character)
        {
            if (_batchCharacter == character)
            {
                _unsavedChanges = false;
                SetTitle();
            }
        }

        public void LoadBatchCharacter(BatchCharacter character)
        {
            if (character.Character != null)
            {
                if (_batchCharacter == null)
                {
                    _storedCharacter = this.character;
                    _storedLastSavedPath = lastSavedPath;
                    _storedUnsavedChanged = _unsavedChanges;
                }
                _batchCharacter = character;
                lastSavedPath = character.AbsolutePath;
                _unsavedChanges = character.UnsavedChanges;
                EnsureItemsLoaded(character.Character.GetAllEquippedAndAvailableGearIds());
                Character = character.Character;
                SetTitle();
            }
        }

        public void UnloadBatchCharacter()
        {
            if (_batchCharacter != null)
            {
                _batchCharacter = null;
                lastSavedPath = _storedLastSavedPath;
                _unsavedChanges = _storedUnsavedChanged;
                Character = _storedCharacter;
                _storedCharacter = null;
                SetTitle();
            }
        }
        #endregion

        #region Run a Calculation
        CharacterCalculationsBase referenceCalculation;
        SendOrPostCallback asyncCalculationCompleted;
        AsyncOperation asyncCalculation;
        private class AsyncCalculationResult
        {
            public CharacterCalculationsBase Calculations;
            public Dictionary<string, string> DisplayCalculationValues;
        }
        private void AsyncCalculationStart(CharacterCalculationsBase calculations, AsyncOperation asyncCalculation)
        {
            Dictionary<string, string> result = calculations.GetAsynchronousCharacterDisplayCalculationValues();
            asyncCalculation.PostOperationCompleted(asyncCalculationCompleted, new AsyncCalculationResult() { Calculations = calculations, DisplayCalculationValues = result });
        }
        private void AsyncCalculationCompleted(object arg)
        {
            AsyncCalculationResult result = (AsyncCalculationResult)arg;
            if (result.DisplayCalculationValues != null && result.Calculations == referenceCalculation)
            {
                UpdateDisplayCalculationValues(result.DisplayCalculationValues, referenceCalculation);
                // refresh chart
                ComparisonGraph.UpdateGraph();
                asyncCalculation = null;
            }
        }
        #endregion

        public void UpdateDisplayCalculationValues(Dictionary<string, string> displayCalculationValues, CharacterCalculationsBase _calculatedStats)
        {
            CalculationDisplay.SetCalculations(displayCalculationValues);
            string status;
            if (!displayCalculationValues.TryGetValue("Status", out status))
            {
                int i = 0;
                status = "Overall: " + Math.Round(_calculatedStats.OverallPoints);
                foreach (KeyValuePair<string, Color> kvp in Calculations.SubPointNameColors)
                {
                    status += ", " + kvp.Key + ": " + Math.Round(_calculatedStats.SubPoints[i]);
                    i++;
                }
                //status = "Rawr version " + typeof(Calculations).Assembly.GetName().Version.ToString();
            }
            StatusText.Text = status;
        }

        private void UpdateRaceLimitations()
        {
            foreach (ComboBoxItem i in RaceCombo.Items)
            {
                if (i.Content != null && i.Content.ToString() != ""
                    && GetClassAllowableRaces[Character.CurrentModel].Contains(i.Content.ToString())) {
                    i.Visibility = Visibility.Visible;
                }else{
                    i.Visibility = Visibility.Collapsed;
                }
            }
        }
        private static Dictionary<string, List<string>> classAllowableRaces = null;
        public static Dictionary<string, List<string>> GetClassAllowableRaces
        {
            get
            {
                if (classAllowableRaces == null)
                {
                    classAllowableRaces = new Dictionary<string, List<string>>();
                    classAllowableRaces.Add("DPSDK", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", "Worgen", "Goblin", /*"Pandaren",*/ });
                    classAllowableRaces.Add("TankDK", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", "Worgen", "Goblin", /*"Pandaren",*/ });
                    classAllowableRaces.Add("Bear", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", "Troll", /*"Undead",*/ "Worgen", /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("Cat", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", "Troll", /*"Undead",*/ "Worgen", /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("Feral", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", "Troll", /*"Undead",*/ "Worgen", /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("Moonkin", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", "Troll", /*"Undead",*/ "Worgen", /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("Tree", new List<string>() { /*"Draenei",*/ /*"Dwarf",*/ /*"Gnome",*/ /*"Human",*/ "Night Elf", /*"Blood Elf",*/ /*"Orc",*/ "Tauren", "Troll", /*"Undead",*/ "Worgen", /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("Hunter", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", "Worgen", "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("Mage", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", /*"Tauren",*/ "Troll", "Undead", "Worgen", "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("Healadin", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", /*"Night Elf",*/ "Blood Elf", /*"Orc",*/ "Tauren", /*"Troll",*/ /*"Undead",*/ /*"Worgen",*/ /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("ProtPaladin", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", /*"Night Elf",*/ "Blood Elf", /*"Orc",*/ "Tauren", /*"Troll",*/ /*"Undead",*/ /*"Worgen",*/ /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("Retribution", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ "Human", /*"Night Elf",*/ "Blood Elf", /*"Orc",*/ "Tauren", /*"Troll",*/ /*"Undead",*/ /*"Worgen",*/ /*"Goblin", "Pandaren",*/ });
                    classAllowableRaces.Add("HealPriest", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", /*"Orc",*/ "Tauren", "Troll", "Undead", "Worgen", "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("ShadowPriest", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", /*"Orc",*/ "Tauren", "Troll", "Undead", "Worgen", "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("Rogue", new List<string>() { /*"Draenei",*/ "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", /*"Tauren",*/ "Troll", "Undead", "Worgen", "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("Enhance", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ /*"Human",*/ /*"Night Elf",*/ /*"Blood Elf",*/ "Orc", "Tauren", "Troll", /*"Undead",*/ /*"Worgen",*/ "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("Elemental", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ /*"Human",*/ /*"Night Elf",*/ /*"Blood Elf",*/ "Orc", "Tauren", "Troll", /*"Undead",*/ /*"Worgen",*/ "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("RestoSham", new List<string>() { "Draenei", "Dwarf", /*"Gnome",*/ /*"Human",*/ /*"Night Elf",*/ /*"Blood Elf",*/ "Orc", "Tauren", "Troll", /*"Undead",*/ /*"Worgen",*/ "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("Warlock", new List<string>() { /*"Draenei",*/ "Dwarf", "Gnome", "Human", /*"Night Elf",*/ "Blood Elf", "Orc", /*"Tauren",*/ "Troll", "Undead", "Worgen", "Goblin", /*"Pandaren",*/ });
                    classAllowableRaces.Add("DPSWarr", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", "Worgen", "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("ProtWarr", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", "Worgen", "Goblin", "Pandaren (Alliance)", "Pandaren (Horde)" }); // "Pandaren", });
                    classAllowableRaces.Add("Brewmaster", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", /*"Worgen", "Goblin",*/ "Pandaren (Alliance)", "Pandaren (Horde)" });
                    classAllowableRaces.Add("Mistweaver", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", /*"Worgen", "Goblin",*/ "Pandaren (Alliance)", "Pandaren (Horde)" });
                    classAllowableRaces.Add("Windwalker", new List<string>() { "Draenei", "Dwarf", "Gnome", "Human", "Night Elf", "Blood Elf", "Orc", "Tauren", "Troll", "Undead", /*"Worgen", "Goblin",*/ "Pandaren (Alliance)", "Pandaren (Horde)" });
                }
                return classAllowableRaces;
            }
            set { classAllowableRaces = value; }
        }

        private void InstallOffline(object sender, System.Windows.RoutedEventArgs e)
        {
#if SILVERLIGHT
            if (Application.Current.InstallState == InstallState.NotInstalled) {
                Application.Current.Install();
            } else {
                MessageBox.Show(
@"We detected that you already have an installed copy of Rawr on your system.

If you are attempting to Update Rawr using this button, you are doing it wrong. The installed copy of Rawr should automatically update when launched, you will be notified about a minute after launch when this occurs by a Popup.

If that is not working for you, then your page is somehow being cached and that is preventing Rawr from updating. This is a common problem with Google Chrome users. If you are using Chrome, press Shift+F5 to forcibly refresh the page and ensure the cached version is updated then relaunch your installed Rawr.

If that is still not working for you, right-click anywhere within the web version of Rawr and select Remove Application. Then click this button again to reinstall Rawr.",
"Rawr is Already Installed", MessageBoxButton.OK);
            }
#endif
        }

        private void SetCompactModeUp()
        {
            if (Rawr.Properties.GeneralSettings.Default.DisplayInCompactMode)
            {
                ShirtButton.Visibility = Visibility.Collapsed;
                TabardButton.Visibility = Visibility.Collapsed;
                if (Wrap_RightSide.Children.Contains(HandButton)) { Wrap_RightSide.Children.Remove(HandButton); }
                if (!Wrap_LeftSide.Children.Contains(HandButton)) { Wrap_LeftSide.Children.Add(HandButton); }
            }
            else
            {
                ShirtButton.Visibility = Visibility.Visible;
                TabardButton.Visibility = Visibility.Visible;
                if (Wrap_LeftSide.Children.Contains(HandButton)) { Wrap_LeftSide.Children.Remove(HandButton); }
                if (!Wrap_RightSide.Children.Contains(HandButton)) { Wrap_RightSide.Children.Insert(0, HandButton); }
            }
            // X
            (HeadButton.RenderTransform as ScaleTransform).ScaleX =
            (NeckButton.RenderTransform as ScaleTransform).ScaleX =
            (ShoulderButton.RenderTransform as ScaleTransform).ScaleX =
            (BackButton.RenderTransform as ScaleTransform).ScaleX =
            (ChestButton.RenderTransform as ScaleTransform).ScaleX =
            (ShirtButton.RenderTransform as ScaleTransform).ScaleX =
            (TabardButton.RenderTransform as ScaleTransform).ScaleX =
            (WristButton.RenderTransform as ScaleTransform).ScaleX =
            (HandButton.RenderTransform as ScaleTransform).ScaleX =
            (BeltButton.RenderTransform as ScaleTransform).ScaleX =
            (LegButton.RenderTransform as ScaleTransform).ScaleX =
            (FeetButton.RenderTransform as ScaleTransform).ScaleX =
            (Ring1Button.RenderTransform as ScaleTransform).ScaleX =
            (Ring2Button.RenderTransform as ScaleTransform).ScaleX =
            (Trinket1Button.RenderTransform as ScaleTransform).ScaleX =
            (Trinket2Button.RenderTransform as ScaleTransform).ScaleX =
            // Y
            (HeadButton.RenderTransform as ScaleTransform).ScaleY =
            (NeckButton.RenderTransform as ScaleTransform).ScaleY =
            (ShoulderButton.RenderTransform as ScaleTransform).ScaleY =
            (BackButton.RenderTransform as ScaleTransform).ScaleY =
            (ChestButton.RenderTransform as ScaleTransform).ScaleY =
            (ShirtButton.RenderTransform as ScaleTransform).ScaleY =
            (TabardButton.RenderTransform as ScaleTransform).ScaleY =
            (WristButton.RenderTransform as ScaleTransform).ScaleY =
            (HandButton.RenderTransform as ScaleTransform).ScaleY =
            (BeltButton.RenderTransform as ScaleTransform).ScaleY =
            (LegButton.RenderTransform as ScaleTransform).ScaleY =
            (FeetButton.RenderTransform as ScaleTransform).ScaleY =
            (Ring1Button.RenderTransform as ScaleTransform).ScaleY =
            (Ring2Button.RenderTransform as ScaleTransform).ScaleY =
            (Trinket1Button.RenderTransform as ScaleTransform).ScaleY =
            (Trinket2Button.RenderTransform as ScaleTransform).ScaleY =
            // Value
            (Rawr.Properties.GeneralSettings.Default.DisplayInCompactMode) ? Rawr.Properties.GeneralSettings.Default.UIScale : 1.00;
            Thickness thickness = new Thickness(2);
            if (Rawr.Properties.GeneralSettings.Default.DisplayInCompactMode)
            {
                double widthLoss = HeadButton.ActualWidth * (1d - Rawr.Properties.GeneralSettings.Default.UIScale);
                double heightLoss = HeadButton.ActualHeight * (1d - Rawr.Properties.GeneralSettings.Default.UIScale);
                thickness = new Thickness(2, 2, 2 - widthLoss, 2 - heightLoss);
            }
            HeadButton.Margin =
            NeckButton.Margin =
            ShoulderButton.Margin =
            BackButton.Margin =
            ChestButton.Margin =
            ShirtButton.Margin =
            TabardButton.Margin =
            WristButton.Margin =
            HandButton.Margin =
            BeltButton.Margin =
            LegButton.Margin =
            FeetButton.Margin =
            Ring1Button.Margin =
            Ring2Button.Margin =
            Trinket1Button.Margin =
            Trinket2Button.Margin =
                thickness;
        }

        #region Character Importing Functions
        // Utility
        private void EnsureItemsLoaded_Helper(List<string> aeids)
        {
            List<int> idList = new List<int>();
            // Convert the string item ids to ints
            // Removing suffixes where necessary
            foreach (string s in aeids) {
                // do something
                string ids = (s.Contains(".")
                        ? s.Substring(0, s.IndexOf("."))
                        : s);
                int id = int.Parse(ids);
                if (!idList.Contains(id)) { idList.Add(id); }
            }
            // Remove all invalid numbers
            while (idList.Contains(0)) { idList.Remove(0); }
            // Remove IDs where we already have the info for the item
            for(int i=0;i <idList.Count; ) {
                Item toCheck = ItemCache.FindItemById(idList[i]);
                if (toCheck != null && !toCheck.Name.ToLower().Contains("wowhead")) {
                    idList.RemoveAt(i);
                } else { i++; }
            }
            // Do the pull
            ItemBrowser.AddItemsById(idList.ToArray(), false, true);
        }
        private void EnsureWornItemsLoaded()
        {
            // Lets make sure we are calling for items that aren't in the database
            List<string> equippedIds = new List<string>(this.Character.GetAllEquippedGearIds());
            EnsureItemsLoaded_Helper(equippedIds);
        }
        private void EnsureItemsLoaded()
        {
            // Lets make sure we are calling for items that aren't in the database
            List<string> availAndEquippedIds = new List<string>(this.Character.GetAllEquippedAndAvailableGearIds());
            EnsureItemsLoaded_Helper(availAndEquippedIds);
        }
        private void EnsureItemsLoaded(string[] ids)
        {
            //List<Item> items = new List<Item>();
            for (int i = 0; i < ids.Length; i++)
            {
                string id = ids[i];
                if (id != null)
                {
                    if (id.IndexOf('.') < 0 && ItemCache.ContainsItemId(int.Parse(id, System.Globalization.CultureInfo.InvariantCulture))) continue;
                    string[] s = id.Split('.');
                    Item newItem = Item.LoadFromId(int.Parse(s[0], System.Globalization.CultureInfo.InvariantCulture), false, false, true, false); // changed to Wowhead until we have battle.net parsing working
                    if (s.Length >= 5)
                    {
                        Item gem;
                        if (s[2] != "*" && s[2] != "0") gem = Item.LoadFromId(int.Parse(s[2], System.Globalization.CultureInfo.InvariantCulture), false, false, true, false);
                        if (s[3] != "*" && s[3] != "0") gem = Item.LoadFromId(int.Parse(s[3], System.Globalization.CultureInfo.InvariantCulture), false, false, true, false);
                        if (s[4] != "*" && s[4] != "0") gem = Item.LoadFromId(int.Parse(s[4], System.Globalization.CultureInfo.InvariantCulture), false, false, true, false);
                    }
                    /*if (newItem != null)
                    {
                        items.Add(newItem);
                    }*/
                }
            }
            // this is only called in the context before loading a character into form
            // also since we're using async item load the items aren't ready yet
            // don't trigger item cache event since we'll load the graphs when character is loaded anyway
            //ItemCache.OnItemsChanged();
        }
        private void ItemsAreLoaded(Character character)
        {
            Status.Close();
            Character = character;
        }
        private void ReloadCharacter(Character character)
        {
            // Loads the Updated Character's into the Form, but doesn't replace it
            this.Character.IsLoading = true;
            this.Character.SetItems(character, true, true);
            List<string> toRemove = new List<string>();
            foreach (string existingAvailableItem in character.AvailableItems)
            {
                string itemId = existingAvailableItem.Split('.')[0];
                if (this.Character.AvailableItems.Contains(itemId)) { toRemove.Add(itemId); } // add items to a remove list 
            }
            foreach (string itemId in toRemove)
            { // and now remove them from AvailableItems - fixes issue 19657
                character.AvailableItems.Remove(itemId);
            }
            this.Character.AvailableItems.AddRange(character.AvailableItems);
            this.Character.AssignAllTalentsFromCharacter(character, false);
            this.Character.PrimaryProfession = character.PrimaryProfession;
            this.Character.SecondaryProfession = character.SecondaryProfession;
            #region Hunter Pets if a Hunter
            if (this.Character.Class == CharacterClass.Hunter)
            {
                // Pull Pet(s) Info if you are a Hunter
                //List<ArmoryPet> pets = Armory.GetPet(this.Character.Region, this.Character.Realm, this.Character.Name);
                //if (pets != null) { this.Character.ArmoryPets = pets; }
            }
            #endregion
            this.Character.IsLoading = false;
            this.Character.OnCalculationsInvalidated();
        }
        private void EnforceAvailability()
        {
            foreach (CharacterSlot cs in Character.CharacterSlots)
            {
                ItemInstance toMakeAvail = null;
                if ((toMakeAvail = this.Character[cs]) != null)
                {
                    if (this.Character.GetItemAvailability(toMakeAvail) == ItemAvailability.NotAvailable)
                    {
                        this.Character.ToggleItemAvailability(toMakeAvail, true);
                    }
                    if (this.Character.GetItemAvailability(toMakeAvail.Enchant) == ItemAvailability.NotAvailable)
                    {
                        this.Character.ToggleItemAvailability(toMakeAvail.Enchant);
                    }
                }
            }
        }
        private void UpdateLastLoadedSet()
        {
            ItemSet last = this.Character.GetItemSetByName("Current");
            last.Name = "Last Loaded Set";
            this.Character.AddToItemSetList(last);
        }
        // Battle.Net
        public void LoadFromBNet(object sender, RoutedEventArgs args)
        {
            if (_unsavedChanges) { NeedToSaveCharacter(); }
            if (CancelToSave) { CancelToSave = false; return; }
            BNetLoadDialog armoryLoad = new BNetLoadDialog();
            armoryLoad.Closed += new EventHandler(bnetLoad_Closed);
            armoryLoad.Show();
        }
        /// <summary>
        /// This override is used for Bookmark Processing. It will automatically
        /// call the character from Battle.Net and load it into the form.
        /// </summary>
        /// <param name="characterName">The Name of the Character. E.g.- Astrylian</param>
        /// <param name="region">The Region the Character is in. E.g.- US</param>
        /// <param name="realm">The Realm the Character is on. E.g.- Stormrage</param>
        public void LoadCharacterFromBNet(string characterName, CharacterRegion region, string realm)
        {
            // There shouldn't be any unsaved changes to a character
            // in the form as this is direct from a Bookmark
            BNetLoadDialog armoryLoad = new BNetLoadDialog();
            armoryLoad.Closed += new EventHandler(bnetLoad_Closed);
            armoryLoad.Show();
            armoryLoad.Load(characterName, region, realm);
        }
        private void bnetLoad_Closed(object sender, EventArgs e)
        {
            BNetLoadDialog ald = sender as BNetLoadDialog;
            if (((BNetLoadDialog)sender).DialogResult.GetValueOrDefault(false))
            {
                Character character = ald.Character;
                #region Recent Settings Update
                // The removes force it to put those items at the end.
                // So we can use that for recall later on what was last used
                if (Rawr.Properties.RecentSettings.Default.RecentChars.Contains(character.Name))
                {
                    Rawr.Properties.RecentSettings.Default.RecentChars.Remove(character.Name);
                }
                if (Rawr.Properties.RecentSettings.Default.RecentServers.Contains(character.Realm))
                {
                    Rawr.Properties.RecentSettings.Default.RecentServers.Remove(character.Realm);
                }
                Rawr.Properties.RecentSettings.Default.RecentChars.Add(character.Name);
                Rawr.Properties.RecentSettings.Default.RecentServers.Add(character.Realm);
                Rawr.Properties.RecentSettings.Default.RecentRegion = character.Region.ToString();
                #endregion

                // Loads the Character into the Form
                this.Character = character;

                EnsureItemsLoaded();
                EnforceAvailability();
                UpdateLastLoadedSet();
                this.Character.ValidateActiveBuffs();
            }
        }
        private void ReloadFromBNet_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(NameText.Text) || String.IsNullOrEmpty(RealmText.Text)) { return; } // can't do this on an empty form
            BNetLoadDialog armoryReload = new BNetLoadDialog();
            armoryReload.Closed += new EventHandler(bnetReload_Closed);
            // Pre-populate the dialog
            armoryReload.RegionCombo.SelectedIndex = RegionCombo.SelectedIndex;
            armoryReload.NameText.Text = Character.Name;
            armoryReload.RealmText.Text = Character.Realm;
            //
            armoryReload.ShowReload();
        }
        private void bnetReload_Closed(object sender, EventArgs e)
        {
            BNetLoadDialog ald = sender as BNetLoadDialog;
            Character character = ald.Character;
            if (character == null) { return; } // Couldn't load it
            #region Recent Settings Update
            // The removes force it to put those items at the end.
            // So we can use that for recall later on what was last used
            if (Rawr.Properties.RecentSettings.Default.RecentChars.Contains(character.Name))
            {
                Rawr.Properties.RecentSettings.Default.RecentChars.Remove(character.Name);
            }
            if (Rawr.Properties.RecentSettings.Default.RecentServers.Contains(character.Realm))
            {
                Rawr.Properties.RecentSettings.Default.RecentServers.Remove(character.Realm);
            }
            Rawr.Properties.RecentSettings.Default.RecentChars.Add(character.Name);
            Rawr.Properties.RecentSettings.Default.RecentServers.Add(character.Realm);
            Rawr.Properties.RecentSettings.Default.RecentRegion = character.Region.ToString();
            #endregion

            // Loads the new information into the current character in the form
            ReloadCharacter(character);

            EnsureItemsLoaded();
            EnforceAvailability();
            UpdateLastLoadedSet();
        }
        // Rawr Addon
        public void LoadFromRawrAddon(object sender, RoutedEventArgs args)
        {
            if (_unsavedChanges) { NeedToSaveCharacter(); }
            if (CancelToSave) { CancelToSave = false; return; }
            RawrAddonLoadDialog rawrAddonLoad = new RawrAddonLoadDialog();
            rawrAddonLoad.Closed += new EventHandler(RawrAddonLoad_Closed);
            rawrAddonLoad.Show();
        }
        private void RawrAddonLoad_Closed(object sender, EventArgs e)
        {
            RawrAddonLoadDialog rald = sender as RawrAddonLoadDialog;
            if (rald.DialogResult.GetValueOrDefault(false))
            {
                RawrAddonCharacter rac = new RawrAddonCharacter(rald.TB_XMLDump.Text, rald.ImportType, rald.CK_MarkGemsToo.IsChecked.GetValueOrDefault(false));

                this.Character = rac.Character;

                //EnforceAvailability(); // Taken care of inside RAC
                UpdateLastLoadedSet();
                this.Character.ValidateActiveBuffs();
            }
        }
        private void RawrAddonReload_Click(object sender, RoutedEventArgs e)
        {
            RawrAddonLoadDialog rawrAddonLoad = new RawrAddonLoadDialog();
            rawrAddonLoad.Closed += new EventHandler(RawrAddonReload_Closed);
            rawrAddonLoad.Show();
        }
        private void RawrAddonReload_Closed(object sender, EventArgs e)
        {
            RawrAddonLoadDialog rald = sender as RawrAddonLoadDialog;
            if (rald.DialogResult.GetValueOrDefault(false))
            {
                RawrAddonCharacter rac = new RawrAddonCharacter(rald.TB_XMLDump.Text, rald.ImportType, rald.CK_MarkGemsToo.IsChecked.GetValueOrDefault(false));

                ReloadCharacter(rac.Character);

                EnforceAvailability();
                UpdateLastLoadedSet();
                this.Character.ValidateActiveBuffs();
            }
        }
        // Rawr4 Repository
        public void LoadFromRawr4Repository(object sender, RoutedEventArgs args)
        {
            if (_unsavedChanges) { NeedToSaveCharacter(); }
            if (CancelToSave) { CancelToSave = false; return; }
            Rawr4RepoLoadDialog armoryLoad = new Rawr4RepoLoadDialog();
            armoryLoad.Closed += new EventHandler(rawr4RepoLoad_Closed);
            armoryLoad.Show();
        }
        /// <summary>
        /// This override is used for Bookmark Processing. It will automatically
        /// call the character from Battle.Net and load it into the form.
        /// </summary>
        /// <param name="characterName">The Name of the Character. Eg- Astrylian</param>
        /// <param name="region">The Region the Character is in. Eg- US</param>
        /// <param name="realm">The Realm the Character is on. Eg- Stormrage</param>
        public void LoadCharacterFromRawr4Repository(string identifier)
        {
            // There shouldn't be any unsaved changes to a character
            // in the form as this is direct from a Bookmark
            Rawr4RepoLoadDialog armoryLoad = new Rawr4RepoLoadDialog();
            armoryLoad.Closed += new EventHandler(rawr4RepoLoad_Closed);
            armoryLoad.Show();
            armoryLoad.Load(identifier);
        }
        private void rawr4RepoLoad_Closed(object sender, EventArgs e)
        {
            Rawr4RepoLoadDialog ald = sender as Rawr4RepoLoadDialog;
            if (ald.DialogResult.GetValueOrDefault(false))
            {
                Character character = ald.Character;
                #region Recent Settings Update
                // The removes force it to put those items at the end.
                // So we can use that for recall later on what was last used
                if (Rawr.Properties.RecentSettings.Default.RecentRepoChars.Contains(ald.NameText.Text))
                {
                    Rawr.Properties.RecentSettings.Default.RecentRepoChars.Remove(ald.NameText.Text);
                }
                Rawr.Properties.RecentSettings.Default.RecentRepoChars.Add(ald.NameText.Text);
                #endregion

                // Loads the Character into the Form
                this.Character = character;

                EnsureItemsLoaded();
                this.Character.ValidateActiveBuffs();
            }
        }
        private void SaveToRawr4Repository(object sender, RoutedEventArgs args)
        {
            Rawr4RepoSaveDialog armorySave = new Rawr4RepoSaveDialog();
            armorySave.Closed += new EventHandler(rawr4RepoSave_Closed);
            armorySave.Show();
        }
        private void rawr4RepoSave_Closed(object sender, EventArgs e)
        {
            Rawr4RepoSaveDialog ald = sender as Rawr4RepoSaveDialog;
            if (ald.DialogResult.GetValueOrDefault(false))
            {
                #region Recent Settings Update
                // The removes force it to put those items at the end.
                // So we can use that for recall later on what was last used
                if (Rawr.Properties.RecentSettings.Default.RecentRepoChars.Contains(ald.NameText.Text))
                {
                    Rawr.Properties.RecentSettings.Default.RecentRepoChars.Remove(ald.NameText.Text);
                }
                Rawr.Properties.RecentSettings.Default.RecentRepoChars.Add(ald.NameText.Text);
                #endregion
            }
        }
        #region Retired Functions
        // Armory (Retired)
        public void LoadCharacterFromArmory(string characterName, CharacterRegion region, string realm)
        {
            ArmoryLoadDialog armoryLoad = new ArmoryLoadDialog();
            armoryLoad.Closed += new EventHandler(armoryLoad_Closed);
            armoryLoad.Show();
            armoryLoad.Load(characterName, region, realm);
        }
        private void armoryLoad_Closed(object sender, EventArgs e)
        {
            ArmoryLoadDialog ald = sender as ArmoryLoadDialog;
            if (((ArmoryLoadDialog)sender).DialogResult.GetValueOrDefault(false))
            {
                Character character = ald.Character;
                // The removes force it to put those items at the end.
                // So we can use that for recall later on what was last used
                if (Rawr.Properties.RecentSettings.Default.RecentChars.Contains(character.Name)) {
                    Rawr.Properties.RecentSettings.Default.RecentChars.Remove(character.Name);
                }
                if (Rawr.Properties.RecentSettings.Default.RecentServers.Contains(character.Realm)) {
                    Rawr.Properties.RecentSettings.Default.RecentServers.Remove(character.Realm);
                }
                Rawr.Properties.RecentSettings.Default.RecentChars.Add(character.Name);
                Rawr.Properties.RecentSettings.Default.RecentServers.Add(character.Realm);
                Rawr.Properties.RecentSettings.Default.RecentRegion = character.Region.ToString();

                this.Character = character;
            }
        }
        private void armory_ResultReady(Character newChar)
        {
            if (newChar != null)
            {
                Character = newChar;
            }
            Status = null;
        }
        // Character Profiler (Retired)
        private void charprofLoad_Closed(object sender, EventArgs e)
        {
            CharProfLoadDialog cpld = sender as CharProfLoadDialog;
            if (((CharProfLoadDialog)sender).DialogResult.GetValueOrDefault(false))
            {
                Character character = cpld.Character;

                // So we can use that for recall later on what was last used
                //Rawr.Properties.RecentSettings.Default.RecentCharProfiler = cpld.TB_FilePath.Text;

                this.Character = character;
            }
        }
        #endregion
        #endregion

        #region Menus
        #region File Menu
        public string lastSavedPath = "";
        private string _storedLastSavedPath;
        private bool _storedUnsavedChanged;

        public void NewCharacter(object sender, RoutedEventArgs args)
        {
            if (_unsavedChanges) { NeedToSaveCharacter(); }
            if (CancelToSave) { CancelToSave = false; return; }
            lastSavedPath = ""; // blank out the path
            Character c = new Character() { IsLoading = false };
            c.Class = Character.Class;
            c.CurrentTalents.Specialization = c.CurrentTalents.Specialization;
            c.UpdateCurrentModel();
            c.Race = Character.Race;
            c.RecalculateSetBonuses();
            Character = c;
            _unsavedChanges = false;
            SetTitle();
        }

        public void OpenCharacter(object sender, RoutedEventArgs args)
        {
            if (_unsavedChanges) { NeedToSaveCharacter(); }
            if (CancelToSave) { CancelToSave = false; return; }
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "character file (*.xml)|*.xml";
            if (ofd.ShowDialog().GetValueOrDefault(false))
            {
                #if SILVERLIGHT
                using (StreamReader reader = ofd.File.OpenText()) {
                    lastSavedPath = "";// ofd.File.DirectoryName;//FullName; // populate the path, but we can't do that in Silverlight because of permissions issues
                #else
                using (StreamReader reader = new StreamReader(ofd.OpenFile())) {
                    lastSavedPath = ofd.FileName; // populate the path
                    while (Rawr.Properties.RecentSettings.Default.RecentFiles.Contains(lastSavedPath)) {
                        Rawr.Properties.RecentSettings.Default.RecentFiles.Remove(lastSavedPath);
                    }
                    while (Rawr.Properties.RecentSettings.Default.RecentFiles.Count > 4) {
                        Rawr.Properties.RecentSettings.Default.RecentFiles.RemoveAt(0);
                    }
                    Rawr.Properties.RecentSettings.Default.RecentFiles.Add(lastSavedPath);
                #endif
                    // TODO: we'll have to expand this considerably to get to Rawr2 functionality
                    Character loadedCharacter = Character.LoadFromXml(reader.ReadToEnd());
                    EnsureItemsLoaded(loadedCharacter.GetAllEquippedAndAvailableGearIds());
                    Character = loadedCharacter;
                }
                _unsavedChanges = false;
                SetTitle();
            }
        }

#if !SILVERLIGHT
        public void OpenCharacter(string path) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.FileName = path;
            using (StreamReader reader = new StreamReader(ofd.OpenFile())) {
                lastSavedPath = ofd.FileName; // populate the path
                while (Rawr.Properties.RecentSettings.Default.RecentFiles.Contains(lastSavedPath)) {
                    Rawr.Properties.RecentSettings.Default.RecentFiles.Remove(lastSavedPath);
                }
                while (Rawr.Properties.RecentSettings.Default.RecentFiles.Count > 4) {
                    Rawr.Properties.RecentSettings.Default.RecentFiles.RemoveAt(0);
                }
                Rawr.Properties.RecentSettings.Default.RecentFiles.Add(lastSavedPath);
                // TODO: we'll have to expand this considerably to get to Rawr2 functionality // uhh what?
                Character loadedCharacter = Character.LoadFromXml(reader.ReadToEnd());
                EnsureItemsLoaded(loadedCharacter.GetAllEquippedAndAvailableGearIds());
                _unsavedChanges = false;
                Character = loadedCharacter;
                SetTitle();
            }
        }
#endif

        private void SetTitle()
        {
#if !SILVERLIGHT
            if (string.IsNullOrEmpty(lastSavedPath))
            {
                App.Current.MainWindow.Title = "Rawr";
            }
            else
            {
                App.Current.MainWindow.Title = "Rawr - " + System.IO.Path.GetFileName(lastSavedPath) + (_unsavedChanges ? " *" : "");
            }
#endif
        }

        private static bool CancelToSave = false;
        public void NeedToSaveCharacter() {
            if (MessageBox.Show("There are unsaved changes to the current character, do you want to save them?"
                                + "\n\nWARNING: Selecting Cancel will lose the unsaved changes."
#if SILVERLIGHT
                                + "\n\nNOTE: You will need to select your menu option again due to limitations in Silverlight."
#endif
                                , "Unsaved Changes Detected", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
            {
#if !SILVERLIGHT
                SaveCharacter(null, null);
#else
                CancelToSave = true;
#endif
            }
#if SILVERLIGHT
            else
            {
                CancelToSave = true; _unsavedChanges = false;
            }
#endif
        }
        private void SaveCharacter(object sender, RoutedEventArgs args)
        {
            if (lastSavedPath == "")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.DefaultExt = ".xml";
                sfd.Filter = "character file (*.xml)|*.xml";
                if (sfd.ShowDialog().GetValueOrDefault(false))
                {
                    using (Stream s = sfd.OpenFile())
                    {
                        Character.Save(s);
                        s.Close();
                    }
                    _unsavedChanges = false;
                    CancelToSave = false;
#if !SILVERLIGHT
                    lastSavedPath = sfd.FileName;
                    SetTitle();
#endif
                }
            }
#if !SILVERLIGHT
            else
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.FileName = lastSavedPath;
                // TODO: Error checking
                using (Stream s = sfd.OpenFile())
                {
                    Character.Save(s);
                    s.Close();
                }
                _unsavedChanges = false;
                CancelToSave = false;
                lastSavedPath = sfd.FileName;
                SetTitle();
            }
#endif
        }
        private void SaveAsCharacter(object sender, RoutedEventArgs args)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "character file (*.xml)|*.xml";
            if (sfd.ShowDialog().GetValueOrDefault(false))
            {
#if !SILVERLIGHT
                lastSavedPath = sfd.FileName;
#endif
                using (Stream s = sfd.OpenFile())
                {
                    Character.Save(s);
                    s.Close();
                }
                _unsavedChanges = false;
                CancelToSave = false;
                SetTitle();
            }
        }

        public UpgradesComparison DG_UpgradesComparison = null;
        private void OpenSavedUpgradeList(object sender, RoutedEventArgs args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Rawr Upgrade List Files|*.xml";
            if (ofd.ShowDialog().GetValueOrDefault())
            {
                DG_UpgradesComparison = new UpgradesComparison
#if SILVERLIGHT
                    (ofd.File.OpenText());
#else
                    (new StreamReader(ofd.OpenFile()));
#endif
                DG_UpgradesComparison.Closed += new EventHandler(DG_UpgradesComparison_Closed);
                DG_UpgradesComparison.Show();
            }
        }

        void DG_UpgradesComparison_Closed(object sender, EventArgs e)
        {
            UpgradeListOpen = false;
        }

        private void ExportToRawrAddon(object sender, RoutedEventArgs e)
        {
            this.Cursor = Cursors.Wait;
            ComparisonCalculationBase[] ducalcs = ComparisonGraph.GetDirectUpgradesGearCalcs(false);
            this.Cursor = Cursors.Arrow;
            RawrAddonSaveDialog rawrAddonSave = new RawrAddonSaveDialog(Character, ducalcs);
            rawrAddonSave.Show();
        }
        #endregion
        #region Tools Menu
        private void ShowOptimizer(object sender, RoutedEventArgs args)
        {
            new OptimizeWindow(Character).Show();
        }
        private void ShowBatchTools(object sender, RoutedEventArgs args)
        {
#if SILVERLIGHT
            // in Silverlight we want to reuse current batch tools
            if (BatchTools.Instance != null)
            {
                App.Current.ShowWindow(BatchTools.Instance);
            }
            else
            {
                App.Current.OpenNewWindow("Batch Tools", new BatchTools());
            }
#else
            App.Current.OpenNewWindow("Batch Tools", new BatchTools());
#endif
        }
        #region Item Sets
        private void SaveItemSet_Click(object sender, RoutedEventArgs e)
        {
            // This generates a list of all your equipped items, as they are
            // We'll use this list to make a comparison chart
            ItemSet newItemSet = new ItemSet();
            foreach (CharacterSlot cs in Character.EquippableCharacterSlots)
            {
                newItemSet.Add(Character[cs]);
            }
            DG_ItemSetName.ShowDialog(Character, newItemSet, SaveItemSet_Confirmation);
        }
        private void SaveItemSet_Confirmation(object sender, EventArgs e) {
            if ((sender as DG_ItemSetName).DialogResult == true)
            {
                (sender as DG_ItemSetName).newItemSet.Name = (sender as DG_ItemSetName).NewSetName;
                Character.AddToItemSetList((sender as DG_ItemSetName).newItemSet);
            }
        }
        private void RemoveItemSet_Click(object sender, RoutedEventArgs e)
        {
            DG_ItemSetNameToRemove.ShowDialog(Character, RemoveItemSet_Confirmation);
        }
        private void RemoveItemSet_Confirmation(object sender, EventArgs e)
        {
            if ((sender as DG_ItemSetNameToRemove).DialogResult.GetValueOrDefault(false))
            {
                Character.RemoveFromItemSetListByName((sender as DG_ItemSetNameToRemove).SetNameToRemove);
            }
        }
        private void EquipItemSet_Click(object sender, RoutedEventArgs e)
        {
            DG_ItemSetNameToEquip.ShowDialog(Character, EquipItemSet_Confirmation);
        }
        private void EquipItemSet_Confirmation(object sender, EventArgs e)
        {
            if ((sender as DG_ItemSetNameToEquip).DialogResult.GetValueOrDefault(false))
            {
                Character.EquipItemSetByName((sender as DG_ItemSetNameToEquip).SetNameToEquip);
            }
        }
        private void CompareItemSet_Click(object sender, RoutedEventArgs e) {
            DG_ItemSetNameToCompare.ShowDialog(Character, CompareItemSet_Confirmation);
        }
        private void CompareItemSet_Confirmation(object sender, EventArgs e) {
            if (sender is DG_ItemSetNameToCompare && (sender as DG_ItemSetNameToCompare).DialogResult.GetValueOrDefault(false)) {
                Character newCharacter = Character.Clone();
                newCharacter.EquipItemSetByName((sender as DG_ItemSetNameToCompare).SetNameToEquip);
                OptimizerResults or = new OptimizerResults(Character, newCharacter, false);
                // Set up the Dialog, its not supposed to look the same as an actual Optimizer Results
                or.BT_StoreIt.Visibility = Visibility.Collapsed;
                or.Title = "Comparing Currently Equipped Set to Other Set";
                or.OKButton.Content = "Equip Other Set";
               CharacterCalculationsBase currentCalc = Character.CurrentCalculations.GetCharacterCalculations(Character, null, false, true, true);
               CharacterCalculationsBase optimizedCalc = newCharacter.CurrentCalculations.GetCharacterCalculations(newCharacter, null, false, true, true);

               float oldValue = Optimizer.ItemInstanceOptimizer.GetOptimizationValue(Character, currentCalc);
               float newValue = Optimizer.ItemInstanceOptimizer.GetOptimizationValue(newCharacter, optimizedCalc);
               or.OptimizedScoreLabel.Text = string.Format("Other Set: {0} ({1:P} change)", newValue, (newValue - oldValue) / oldValue);
                //
                or.Closed += new EventHandler(CompareItemSetEquip_Confirmation);
                or.Show();
            }
        }
        private void CompareItemSetEquip_Confirmation(object sender, EventArgs e) {
            if ((sender as OptimizerResults).DialogResult == true) {
                OptimizerResults or = (sender as OptimizerResults);
                //Character.EquipItemSetByName((sender as OptimizerResults).SetNameToEquip);
                character.IsLoading = true;
                character.SetItems(or.BestCharacter);
                /* Not setting Buffs or Talents because that stuff isn't stored in Item Sets
                character.ActiveBuffs = results.BestCharacter.ActiveBuffs;
                if (CK_Talents_Points.IsChecked.GetValueOrDefault())
                {
                    character.CurrentTalents = results.BestCharacter.CurrentTalents;
                    MainPage.Instance.TalentPicker.RefreshSpec();
                }*/
                character.IsLoading = false;
                character.OnCalculationsInvalidated();
            }
        }
        #endregion
        #region Item Cost
        private void ResetItemCost_Click(object sender, RoutedEventArgs args)
        {
            ItemCache.ResetItemCost();
        }
        private void LoadItemCostJustice_Click(object sender, RoutedEventArgs args)
        {
            ItemCache.LoadTokenItemCost("Justice Points");
        }
        private void LoadItemCostValor_Click(object sender, RoutedEventArgs args)
        {
            ItemCache.LoadTokenItemCost("Valor Points");
        }
        private void LoadItemCostHonor_Click(object sender, RoutedEventArgs args)
        {
            ItemCache.LoadPointItemCost("Honor");
        }
        private void LoadItemCostConquest_Click(object sender, RoutedEventArgs args)
        {
            ItemCache.LoadPointItemCost("Conquest");
        }
        private void LoadItemCostCrystallizedFirestone_Click(object sender, RoutedEventArgs args)
        {
            ItemCache.LoadAvailablePrerequisiteItemCost("Crystallized Firestone", Character);
        }
        #endregion
        private void CompareCharacters_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "character file (*.xml)|*.xml";
            if (ofd.ShowDialog().GetValueOrDefault(false))
            {
#if SILVERLIGHT
                using (StreamReader reader = ofd.File.OpenText())
#else
                using (StreamReader reader = new StreamReader(ofd.OpenFile()))
#endif
                {
                    Character newCharacter = Character.LoadFromXml(reader.ReadToEnd());
                    OptimizerResults or = new OptimizerResults(Character, newCharacter, false);
                    // Set up the Dialog, its not supposed to look the same as an actual Optimizer Results
                    or.BT_StoreIt.Visibility = Visibility.Collapsed;
                    or.Title = "Comparing Current Character to Another";
                    or.CancelButton.Visibility = Visibility.Collapsed;
                    or.OKButton.Content = "OK";
                    or.OptimizedScoreLabel.Text = string.Format("Other Character: {0}", or.optimizedCalc.OverallPoints);
                    //
                    or.Show();
                }
                _unsavedChanges = false;
            }
        }
        #endregion
        #region Import Menu
        // TODO: Load Possible Upgrades from Wowhead
        // TODO: Import from Wowhead Filter
        #region Update Item Cache from Wowhead
        private void StartProcessing()
        {
            //Cursor = Cursors.WaitCursor;
            if (status == null /*|| status.IsDisposed*/)
            {
                status = new Status();
            }
            status.Owner = Application.Current.MainWindow;
            //menuStripMain.Enabled = false;
            //ItemContextualMenu.Instance.Enabled = false;
            //FormItemSelection.Enabled = false;
        }
        private void bw_UpdateItemCacheWowhead(object sender, DoWorkEventArgs e)
        {
            // check for slot parameter
            var slot = (e.Argument != null && e.Argument is CharacterSlot
                                      ? (CharacterSlot)e.Argument
                                      : CharacterSlot.None);

            // fire 
            this.UpdateItemCacheWowhead(slot, true/*(ModifierKeys & Keys.Shift) != 0*/ );
        }
        void bw_RunWowheadQuery(object sender, DoWorkEventArgs e)
        {
            this.ExecuteWowheadQuery();
        }
        private void bw_StatusCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show("Error processing request: " + e.Error.Message);
            }
            FinishedProcessing();
        }
        private void FinishedProcessing()
        {
            if (status != null /*&& !status.IsDisposed*/)
            {
                status.AllowedToClose = true;
                if (status.HasErrors)
                {
                    status.SwitchToErrorTab();
                }
                else
                {
                    status.Close();
                    //status.Dispose();
                }
            }
            //ItemContextualMenu.Instance.Enabled = true;
            //menuStripMain.Enabled = true;
            //FormItemSelection.Enabled = true;
            //this.Cursor = Cursors.Default;
        }
        public static string UpdateCacheStatusKey(CharacterSlot slot, bool bWowhead)
        {
            return string.Format("Update {0} from {1}",
                                 slot == CharacterSlot.None ? "All Items" : slot.ToString(),
                                 bWowhead ? "Wowhead" : "Armory"
                );
        }
        private bool UpgradeCancelPending()
        {
            return Status != null && Status.CancelPending;
        }
        public Item[] ItemsForUpdate(CharacterSlot slot)
        {
            // unknown? update everything
            if (slot == CharacterSlot.None)
                return ItemCache.AllItems;

            // get relevant items
            var list = Character.GetRelevantItems(slot);

            // fix the currently equipped: it might be non-relevant...
            var equipped = Character[slot];
            if (equipped != null && equipped.Item != null)
            {
                // check if it is
                if (!list.Contains(equipped.Item))
                    list.Add(equipped.Item);
            }

            // retval
            return list.ToArray();
        }
        public void UpdateItemCacheWowhead(CharacterSlot slot, bool bOnlyNonLocalized)
        {
            //WebRequestWrapper.ResetFatalErrorIndicator();
            StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, true), "Beginning Update");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Not Started");
            StringBuilder sbChanges = new StringBuilder();

            bool multithreaded = false;// Rawr.Properties.GeneralSettings.Default.UseMultithreading;
            Base.ItemUpdater updater = new Base.ItemUpdater(multithreaded, false, false /*usePTRDataToolStripMenuItem.Checked*/, 20, UpgradeCancelPending);
            int skippedItems = 0;

            // get list of the items to be updated
            var allItems = ItemsForUpdate(slot);

            // an index of added items
            var addedItems = 0;

            foreach (Item item in allItems)
            {
                if (item.Id < 90000)
                {
                    if (!bOnlyNonLocalized || string.IsNullOrEmpty(item.LocalizedName))
                    {
                        updater.AddItem(addedItems++, item);
                        if (!multithreaded)
                        {
                            StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, true), "Updating " + (skippedItems + addedItems) + " of " + allItems.Length + " items");
                            if (UpgradeCancelPending())
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    skippedItems++;
                }
            }

            updater.FinishAdding();

            while (!updater.Done)
            {
                StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, true), "Updating " + (skippedItems + updater.ItemsDone) + " of " + updater.ItemsToDo + " items");
                Thread.Sleep(1000);
            }

            for (int i = 0; i < allItems.Length; i++)
            {
                Item item = allItems[i];
                Item newItem = updater[i];

                if (item.Id < 90000 && newItem != null)
                {
                    string before = item.Stats.ToString();
                    string after = newItem.Stats.ToString();
                    if (before != after)
                    {
                        sbChanges.AppendFormat("[{0}] {1}\r\n", item.Id, item.Name);
                        sbChanges.AppendFormat("BEFORE: {0}\r\n", before);
                        sbChanges.AppendFormat("AFTER: {0}\r\n\r\n", after);
                    }
                }
            }
#if DEBUG
            /*if (sbChanges.Length > 0)
            {
                ScrollableMessageBox msgBox = new ScrollableMessageBox();
                msgBox.Show(sbChanges.ToString());
            }*/
#endif
            StatusMessaging.UpdateStatusFinished(UpdateCacheStatusKey(slot, true));
            //ItemIcons.CacheAllIcons(ItemCache.AllItems);
            Dispatcher.Invoke(new Action(ItemCache.OnItemsChanged));
            Dispatcher.Invoke(new Action(character.InvalidateItemInstances));

            // save stuff
            //SaveSettingsAndCaches();
        }
        public void ExecuteWowheadQuery()
        {
            CharacterSlot slot = CharacterSlot.None;
            StatusMessaging.UpdateStatus(UpdateCacheStatusKey(slot, true), "Beginning Update");
            StatusMessaging.UpdateStatus("Cache Item Icons", "Not Started");
            StringBuilder sbChanges = new StringBuilder();

            WowheadService service = new WowheadService();
            service.ImportItemsFromWowhead("filter-patch=17056&filter-quality=16&filter-slot=2359278");

            StatusMessaging.UpdateStatusFinished(UpdateCacheStatusKey(slot, true));
            Dispatcher.Invoke(new Action(ItemCache.OnItemsChanged));
            Dispatcher.Invoke(new Action(character.InvalidateItemInstances));
        }
        private bool ConfirmUpdateItemCache()
        {
            return MessageBox.Show("Are you sure you would like to update the item cache? This process takes significant time, and the default item cache is fully updated as of the time of release. This does not add any new items, it only updates the data about items already in your itemcache.",
                "Update Item Cache?", MessageBoxButton.OKCancel) == MessageBoxResult.OK;
        }
        public void RunItemCacheWowheadUpdate(CharacterSlot slot)
        {
            if (slot != CharacterSlot.None || ConfirmUpdateItemCache())
            {
                StartProcessing();
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_UpdateItemCacheWowhead);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_StatusCompleted);
                bw.RunWorkerAsync(slot);
                // WPF implementation of .Show() calls Window.ShowDialog(), which blocks (unlike Silverlight version)
                status.Show();
            }
        }
        public void RunWowheadQuery()
        {
            if (ConfirmUpdateItemCache())
            {
                status = null;
                StartProcessing();
                BackgroundWorker bw = new BackgroundWorker();
                bw.DoWork += new DoWorkEventHandler(bw_RunWowheadQuery);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_StatusCompleted);
                bw.RunWorkerAsync();
                status.Show();
            }
        }
        #endregion
        private void UpdateItemCacheFromWowhead_Click(object sender, RoutedEventArgs e)
        {
#if DEBUG
            // Dont run in Release versions because it doesn't work yet
            RunItemCacheWowheadUpdate(CharacterSlot.None);
#endif
        }
        private void LoadItemsFromWowheadFilter_Click(object sender, RoutedEventArgs e)
        {
            RunWowheadQuery();
#if DEBUG
            // Dont run in Release versions because it doesn't work yet
#endif
        }
        private void RefreshItemsCurrentlyWorn_Click(object sender, RoutedEventArgs e)
        {
            if (Character == null) { return; }
            EnsureWornItemsLoaded();
        }
        #endregion
        #region Options Menu
        private void ShowOptions(object sender, RoutedEventArgs e)
        {
            OptionsDialog od = new OptionsDialog();
            od.Closed += new EventHandler(ShowOptions_Closed);
            od.Show();
        }
        private void ShowOptions_Closed(object sender, EventArgs e) {
            OptionsDialog od = sender as OptionsDialog;
            if (od.DialogResult.GetValueOrDefault(false) == true) {
                SetCompactModeUp();
            }
        }

        private void ShowItemEditor(object sender, RoutedEventArgs args)
        {
#if !SILVERLIGHT
            itemSearch = new ItemBrowser(); // can't reuse in WPF after closed?
            ItemSearch.Owner = Application.Current.MainWindow;
            ItemSearch.Show();
#else
            ItemSearch.Show();
#endif
        }

        private void ShowGemmingTemplates(object sender, RoutedEventArgs args)
        {
            new GemmingTemplates(Character).Show();
        }

        private void ShowItemRefinement(object sender, RoutedEventArgs args)
        {
            new RelevantItemRefinement().Show();
        }

        private void ShowItemFilters(object sender, RoutedEventArgs args)
        {
            new EditItemFilter().Show();
        }

        #region Emblem Costs (No Longer Used)
        private void ResetItemCost(object sender, RoutedEventArgs args)
        {
            ItemCache.ResetItemCost();
        }
        private void LoadItemCost(object sender, RoutedEventArgs args)
        {
            OpenFileDialog dialog = new OpenFileDialog();
#if !SILVERLIGHT
            dialog.DefaultExt = ".cost.xml";
#endif
            dialog.Filter = "Rawr Xml Item Cost Files | *.cost.xml";
            dialog.Multiselect = false;
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
#if SILVERLIGHT
                using (StreamReader reader = dialog.File.OpenText())
#else
                using (StreamReader reader = new StreamReader(dialog.OpenFile()))
#endif
                {
                    ItemCache.LoadItemCost(reader);
                }
            }
        }
        private void SaveItemCost(object sender, RoutedEventArgs args)
        {
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.DefaultExt = ".cost.xml";
            dialog.Filter = "Rawr Xml Item Cost Files | *.cost.xml";
            if (dialog.ShowDialog().GetValueOrDefault(false))
            {
                this.Cursor = Cursors.Wait;
                using (StreamWriter writer = new StreamWriter(dialog.OpenFile()))
                {
                    ItemCache.SaveItemCost(writer);
                }
                this.Cursor = Cursors.Arrow;
            }
        }
        private void LoadEmblemOfFrostCost(object sender, RoutedEventArgs args)
        {
            ItemCache.LoadTokenItemCost("Emblem of Frost");
        }
        #endregion

        #region Reset Caches
        private void ResetAllCaches(object sender, RoutedEventArgs e)
        {
            ConfirmationWindow.ShowDialog("Are you sure you'd like to clear and redownload all caches?\r\n\r\nWARNING: This will also unload the current character, so be sure to save first!",
                "Are you sure?",
                new EventHandler(ResetAllCaches_Confirmation));
        }

        private void ResetAllCaches_Confirmation(object sender, EventArgs e)
        {
            if (((ConfirmationWindow)sender).DialogResult == true)
            {
                ResetAllCaches_Action();
            }
        }

        public void ResetAllCaches_Action()
        {
            Character = new Character() { IsLoading = false };
            string dir = "";
#if !SILVERLIGHT
            dir = "ClientBin\\";
#endif
            new FileUtils(new string[] {
                dir+"BuffSets.xml", 
                dir+"ItemCache.xml",
                dir+"ItemFilter.xml",
                dir+"PetTalents.xml",
                dir+"Settings.xml",
                dir+"Talents.xml",
            }).Delete();
            LoadScreen ls = new LoadScreen();
            (App.Current.RootVisual as Grid).Children.Add(ls);
            this.Visibility = Visibility.Collapsed;
            ls.StartLoading(new EventHandler(ResetCaches_Finished));
            Character = new Character() { IsLoading = false };
        }

        private void ResetItemCacheBecauseOfNewVersion()
        {
#if SILVERLIGHT
            if (!Rawr.Properties.GeneralSettings.Default.IsNewVersionRunning) { return; }
            ConfirmationWindow.ShowDialog("We detected that you are running a New Version for the first time. Would you like to Reset the Item Cache?"
                + "\r\n\r\nThis is generally a good idea for new releases as we provide many updates to Item Sources, Stat Numbers and other fixes.", "Are you sure?",
                new EventHandler(ResetItemCaches_Confirmation));
#endif
        }

        private void ResetItemCache(object sender, RoutedEventArgs e)
        {
            ConfirmationWindow.ShowDialog("Are you sure you'd like to clear and redownload the item cache?\r\n\r\nWARNING: This will also unload the current character, so be sure to save first!",
                "Are you sure?",
                new EventHandler(ResetItemCaches_Confirmation));
        }

        private void ResetItemCaches_Confirmation(object sender, EventArgs e)
        {
            if (((ConfirmationWindow)sender).DialogResult == true)
            {
                Rawr.Properties.GeneralSettings.Default.IsNewVersionRunning = false;
                Character = new Character() { IsLoading = false };
                string dir = "";
#if !SILVERLIGHT
                dir = "ClientBin\\";
#endif
                new FileUtils(new string[] { dir + "ItemCache.xml", }).Delete();
                LoadScreen ls = new LoadScreen();
                (App.Current.RootVisual as Grid).Children.Add(ls);
                this.Visibility = Visibility.Collapsed;
                ls.StartLoading(new EventHandler(ResetCaches_Finished));
                Character = new Character() { IsLoading = false };
            }
        }

        public void ResetCaches_Finished(object sender, EventArgs e)
        {
            (App.Current.RootVisual as Grid).Children.Remove(sender as LoadScreen);
            this.Visibility = Visibility.Visible;
        }
        #endregion
        #endregion
        #region Help Menu
        private void ShowHelp(string uri)
        {
#if SILVERLIGHT
            System.Windows.Browser.HtmlPage.Window.Navigate(new Uri(uri, UriKind.Absolute), "_blank");
#else
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(uri));
#endif
        }

        private void ShowRawrHelpPage(object sender, RoutedEventArgs args)
        {
            ShowHelp("http://rawr.codeplex.com/documentation");
        }

        private void ShowTourOfRawr(object sender, RoutedEventArgs args)
        {
            ShowHelp("http://www.youtube.com/watch?v=OjRM5SUoOoQ");
        }

        private void ShowGemmingsHelp(object sender, RoutedEventArgs args)
        {
            ShowHelp("http://rawr.codeplex.com/wikipage?title=Gemmings");
        }

        private void ShowGearOptimizationHelp(object sender, RoutedEventArgs args)
        {
            ShowHelp("http://rawr.codeplex.com/wikipage?title=GearOptimization");
        }

        private void ShowItemFilteringHelp(object sender, RoutedEventArgs args)
        {
            ShowHelp("http://rawr.codeplex.com/wikipage?title=ItemFiltering");
        }

        private void ShowModelStatusHelp(object sender, RoutedEventArgs args)
        {
            ShowHelp("http://rawr.codeplex.com/wikipage?title=Models");
        }

        private void ShowRawrWebsite(object sender, RoutedEventArgs args)
        {
            ShowHelp("http://rawr.codeplex.com/");
        }

        private void ShowDonate(object sender, RoutedEventArgs args)
        {
            ShowHelp("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=2451163");
        }

        private void ShowWelcomeScreen(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            new WelcomeWindow().Show();
#else
            WelcomeWindow w = new WelcomeWindow();
            w.ShowDialog();
            w.Activate();
#endif
        }

#if !SILVERLIGHT
        private void WaitAndShowWelcomeScreen() {
            WaitCallback w = new WaitCallback(WaitAndShowWelcomeScreen_completed);
        }
        private void WaitAndShowWelcomeScreen_completed(object sender) {
            ShowWelcomeScreen(null, null);
        }
#endif
        #endregion
        #endregion

        private void PerformanceTest(object sender, System.Windows.RoutedEventArgs e)
        {
            DateTime start = DateTime.Now;
            int count = 10000;
            for (int i = 0; i < count; i++)
                Calculations.GetCharacterCalculations(Character);
            TimeSpan ts = DateTime.Now.Subtract(start);
            //Clipboard.SetText(ts.ToString());
            MessageBox.Show(string.Format("This model took {0} seconds to run calculations {1} times.", ts, count),
                "Performance Test", MessageBoxButton.OK);
        }

#if !SILVERLIGHT
        private void WaitAndShowFirstTimeWebHelpWindow()
        {
            WaitCallback w = new WaitCallback(WaitAndShowFirstTimeWebHelpWindow_completed);
        }
        private void WaitAndShowFirstTimeWebHelpWindow_completed(object sender)
        {
            if (!Rawr.Properties.GeneralSettings.Default.HasSeenHelpWindow)
            {
                MessageBox.Show(
@"Rawr has added a new feature, the Rawr Web Help Window.

The Web Help Window is designed to provide fast access to common help topics.

In the Downloadable Version of Rawr (WPF), you will be provided with a mini-webbrowser which displays relevant topics as seen on our Documentation pages.

In Silverlight, the online version of Rawr, you will be prompted with quick links to open the content in your browser. We are working to provide web content directly in here as well.
",
                    "New Information!", MessageBoxButton.OK);
                Rawr.Properties.GeneralSettings.Default.HasSeenHelpWindow = true;
                Button_Click(null, null);
            }
        }
#endif

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            WebHelp webHelp = new WebHelp("WhereToStart");
            webHelp.Show();
        }

        private void SubmitIssue_Click(object sender, RoutedEventArgs e)
        {
#if SILVERLIGHT
            WebHelp webHelp = new WebHelp("PostingGuidelines", "Before submitting an issue, please read and follow the Posting Guidelines. They can be found using the View Content in Browser link below.");
#else
            WebHelp webHelp = new WebHelp("PostingGuidelines");
#endif
            webHelp.Show();
        }

        /*private System.Timers.Timer bh_hasIssueTimer = null;
        private bool blinkOn = true;
        private bool bhisbusy = false;
        //private System.Timers.TimerCallback asd;
        private void BossHandlerHasAnIssue(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (bh_hasIssueTimer == null) {
                bh_hasIssueTimer = new System.Timers.Timer(2 * 1000);
                bh_hasIssueTimer.Elapsed += new System.Timers.ElapsedEventHandler(BossHandlerHasAnIssue);
                bh_hasIssueTimer.Enabled = true;
            }
            blinkOn = !blinkOn;
            if (!bhisbusy) {
                bhisbusy = true;
                //try {
                    // Normal: #FF8C8E94
                    SolidColorBrush brush = new SolidColorBrush(Color.FromArgb(255, 140, 142, 148));
                    if (Character.BossOptions.HasAProblem && blinkOn) {
                        // Bad: Red
                        brush = new SolidColorBrush(Colors.Red);
                    }
                    if (BossTab.Dispatcher.CheckAccess()) {
                        BossTab.BorderBrush = brush;
                    } else {
                        BossTab.Dispatcher.BeginInvoke(DispatcherPriority.Normal, (Action)(() => { BossTab.BorderBrush = brush; }));
                    }
                //} catch (System.InvalidOperationException) { }
                bhisbusy = false;
            }
        }*/
    }
}
