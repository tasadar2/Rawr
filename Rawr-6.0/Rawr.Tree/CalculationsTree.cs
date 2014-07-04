﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System.Xml.Serialization;

namespace Rawr.Tree {
    enum PointsTree : int
    {
        RaidSustained,
        RaidBurst,
        TankSustained,
        TankBurst,
        Count
    }

    enum TreeSpell
    {
        Nourish,
        HealingTouch,
        Regrowth,
        Lifebloom,
        Rejuvenation,
        Tranquility,
        Swiftmend,
        WildGrowth,
        Count
    }

    sealed internal class SpellData
    {
        public string Name;
        public int ID;
        public int Mana;
        public int BaseTimeMS;
        public int MinHeal;
        public int MaxHeal;
        public double AvgHeal { get { return (MinHeal + MaxHeal) * 0.5; } }
        public double Coeff;
        public int TickHeal;
        public double TickCoeff;
        public int BaseTickRateMS;
        public int BaseDurationMS;

        public SpellData(string name, int id, float baseManaPercent, int baseTimeMS, int minHeal, int maxHeal, int coeff, int tickHeal = 0, int tickCoeff = 0, int baseTickRateMS = 0, int baseDurationMS = 0)
        {
            this.Name = name;
            this.ID = id;
            this.Mana = (int)Math.Floor(CalculationsTree.BaseMana * baseManaPercent / 100f);
            this.BaseTimeMS = baseTimeMS;
            this.MinHeal = minHeal;
            this.MaxHeal = maxHeal;
            this.Coeff = coeff / 10000.0;
            this.TickHeal = tickHeal;
            this.TickCoeff = tickCoeff / 10000.0;
            this.BaseTickRateMS = baseTickRateMS;
            this.BaseDurationMS = baseDurationMS;
        }

        public SpellData Clone()
        {
            return (SpellData)this.MemberwiseClone();
        }
    }


    [Rawr.Calculations.RawrModelInfo("Tree", "Ability_Druid_TreeofLife", CharacterClass.Druid)]
    public class CalculationsTree : CalculationsBase
    {
        internal static float BaseMana = BaseCombatRating.DruidBaseMana(90);

        internal static SpellData[] SpellData = 
        {            
            /* Numbers are:
             * base mana percentage
             * cast time in ms (including GCD)
             * min heal, max heal, coefficient*10000
             * tick heal, tick coefficient*10000
             * tick rate in ms, hot duration in ms
             * 
             * Swiftmend's HoT part is Efflorescence, later multiplied by the talent-specific percentage
             * Multi-target hots are later multiplier by the number of targets
             * Tranquility is later multiplier by 4 * 5 (4 smart heals to 5 people each)
             */
            new SpellData("Nourish", 50464,
                10.2f,
                2500,
                6151, 7148, 6140
                ),
            new SpellData("Healing Touch", 5185,
                28.9f,
                2500,
                18460, 21800, 18600
                ),
            new SpellData("Regrowth", 8936,
                29.7f,
                1500,
                9813, 10954, 9580,
                2361/3, 730,
                2000, 6000
                ),
            new SpellData("Lifebloom", 33763,
                5.9f,
                1500,
                8150, 8150, 7520,
                621, 570,
                1000, 15000
                ),
            new SpellData("Rejuvenation", 774,
                16.0f,
                1500,
                4234, 4234, 3920,
                4234, 3920,
                3000, 12000
                ),
            new SpellData("Tranquility", 740,
                27.1f,
                8000,
                9037, 9037, 8350,
                1542, 1420,
                2000, 8000
                ),
            new SpellData("Swiftmend", 18562,
                8.5f,
                1500,
                13966, 13966, 12900,
                1676, 1548,   // 0.12 of base values, still need times 3 for targets?
                1000, 7000
                ),
            new SpellData("Wild Growth", 48438,
                22.9f,
                1500,
                0, 0, 0,
                6930/7, 920, // TODO: decreasing rate?
                1000, 7000
                )
        };

        internal static TreeSpell[] T13Spells = new TreeSpell[] { TreeSpell.Rejuvenation, TreeSpell.Regrowth };

        private static SpellData[] t13SpellData;

        internal static SpellData[] T13SpellData
        {
            get
            {
                if (t13SpellData == null)
                {
                    t13SpellData = new SpellData[T13Spells.Length];
                    for (int i = 0; i < T13Spells.Length; ++i)
                    {
                        t13SpellData[i] = SpellData[(int)T13Spells[i]].Clone();
                        t13SpellData[i].BaseDurationMS *= 2;
                    }
                }
                return t13SpellData;
            }
        }

        #region Variables and Properties
        #region Gemming Templates
        private string[] tierNames = { "Uncommon", "Rare", "Epic", "Jewelcrafter" };

        // Red
        private int[] brilliant = { 52173, 52207, 71881, 52257 };

        // Orange
        private int[] reckless = { 52144, 52208, 71850, 52208 };
        private int[] artful = { 52140, 52205, 71854, 52205 };
        private int[] potent = { 52147, 52239, 71842, 52239 };

        // Purple
        private int[] purified = { 52100, 52236, 71868, 52236 };

        // Meta
        private int[] metas = { 68780, 52296, 41333 };

        //Cogwheel
        private int cog_fractured = 59480;  //Mastery
        private int cog_sparkling = 59496;  //Spirit
        private int cog_quick = 59479;  //Haste
        private int cog_smooth = 59478;  //Crit

        /// <summary>
        /// List of gemming templates available to Rawr.
        /// </summary>
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                List<GemmingTemplate> retval = new List<GemmingTemplate>();
                
                for (int tier = 0; tier < 4; ++tier)
                {
                    foreach (int meta in metas)
                        retval.AddRange(TreeGemmingTemplateBlock(tier, meta));
                }

                foreach (int meta in metas)
                    retval.AddRange(TreeCogwheelTemplateBlock(meta));

                return retval;
            }
        }

        private List<GemmingTemplate> TreeGemmingTemplateBlock(int tier, int meta)
        {
            List<GemmingTemplate> retval = new List<GemmingTemplate>();
            retval.AddRange(new GemmingTemplate[] {
                CreateTreeGemmingTemplate(tier, tierNames, brilliant, brilliant, brilliant, brilliant, meta), // Straight Intellect
                CreateTreeGemmingTemplate(tier, tierNames, brilliant, reckless, brilliant, brilliant, meta), // Int/Haste/Int
                CreateTreeGemmingTemplate(tier, tierNames, brilliant, potent, brilliant, brilliant, meta), // Int/Crit/Int
                CreateTreeGemmingTemplate(tier, tierNames, brilliant, artful, brilliant, brilliant, meta), // Int/Mastery/Int
                CreateTreeGemmingTemplate(tier, tierNames, brilliant, reckless, purified, brilliant, meta), // Int/Haste/Spirit
                CreateTreeGemmingTemplate(tier, tierNames, brilliant, potent, purified, brilliant, meta), // Int/Crit/Spirit
                CreateTreeGemmingTemplate(tier, tierNames, brilliant, artful, purified, brilliant, meta), // Int/Mastery/Spirit
            });
            return retval;
        }

        const int DEFAULT_GEMMING_TIER = 1;
        private GemmingTemplate CreateTreeGemmingTemplate(int tier, string[] tierNames, int[] red, int[] yellow, int[] blue, int[] prismatic, int meta)
        {
            return new GemmingTemplate
            {
                Model = "Tree",
                Group = tierNames[tier],
                Enabled = (tier == DEFAULT_GEMMING_TIER),
                RedId = red[tier],
                YellowId = yellow[tier],
                BlueId = blue[tier],
                PrismaticId = prismatic[tier],
                MetaId = meta
            };
        }

        private List<GemmingTemplate> TreeCogwheelTemplateBlock(int meta)
        {
            List<GemmingTemplate> retval = new List<GemmingTemplate>();
            retval.AddRange(new GemmingTemplate[] {
                    // Engineering cogwheel templates (meta and 2 cogs each, no repeats)
                    CreateTreeCogwheelTemplate(meta, cog_fractured, cog_quick),
                    CreateTreeCogwheelTemplate(meta, cog_fractured, cog_smooth),
                    CreateTreeCogwheelTemplate(meta, cog_fractured, cog_sparkling),
                    CreateTreeCogwheelTemplate(meta, cog_quick, cog_smooth),
                    CreateTreeCogwheelTemplate(meta, cog_quick, cog_sparkling),
                    CreateTreeCogwheelTemplate(meta, cog_smooth, cog_sparkling),
                });
            return retval;
        }

        private GemmingTemplate CreateTreeCogwheelTemplate(int meta, int cogwheel1, int cogwheel2)
        {
            return new GemmingTemplate
            {
                Model = "Tree",
                Group = "Engineer",
                Enabled = false,
                MetaId = meta,
                CogwheelId = cogwheel1,
                Cogwheel2Id = cogwheel2
            };
        }
        #endregion

        /// <summary>Labels of the stats available to the Optimizer</summary>
        public override string[] OptimizableCalculationLabels
        {
            get { return _optimizableCalculationLabels; }
        }
        
        private static string[] _optimizableCalculationLabels = new string[] {
            "Intellect",
            "Spirit",
            "Haste Rating",
            "Crit Rating",
            "Mastery Rating",
            "Health",
            "Mana",
            "Mana Regen"
        };

        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, Color>();
                    
                    _subPointNameColors.Add("Raid Sustained", Colors.Green);
                    _subPointNameColors.Add("Raid Burst", Colors.Yellow);
                    _subPointNameColors.Add("Tank Sustained", Colors.Blue);
                    _subPointNameColors.Add("Tank Burst", Colors.Red);

                }
                return _subPointNameColors;
            }
        }
        private Dictionary<string, Color> _subPointNameColors = null;

        private static string[] _actionNames;

        public static string[] ActionNames
        {
            get
            {
                if(_actionNames == null)
                {
                    string[] actionNames = new string[(int)TreeAction.Count];
                    for (int i = 0; i < (int)TreeAction.Count; ++i)
                        actionNames[i] = Utilities.CamelCaseToSpaced(Enum.GetName(typeof(TreeAction), (TreeAction)i));
                    _actionNames = actionNames;
                }
                return _actionNames;
            }
        }

        private static string[] _passiveNames;

        public static string[] PassiveNames
        {
            get
            {
                if (_passiveNames == null)
                {
                    string[] passiveNames = new string[(int)TreePassive.Count];
                    for (int i = 0; i < (int)TreePassive.Count; ++i)
                        passiveNames[i] = Utilities.CamelCaseToSpaced(Enum.GetName(typeof(TreePassive), (TreePassive)i));
                    _passiveNames = passiveNames;
                }
                return _passiveNames;
            }
        }


        private string[] characterDisplayCalculationLabels = null;
        public override string[] CharacterDisplayCalculationLabels
        {
            get
            {
                if (characterDisplayCalculationLabels != null)
                    return characterDisplayCalculationLabels;

                string[] basic = new string[] {
                    "Basic Stats:Health",
                    "Basic Stats:Mana",
                    "Basic Stats:Agility",
                    "Basic Stats:Stamina",
                    "Basic Stats:Intellect",
                    "Basic Stats:Spirit",
                    "Basic Stats:Armor",

                    "Fight:Fight Length",
                    "Fight:Divisions",
                    "Fight:Innervates",
                    "Fight:Innervate Effect Delay",
                    "Fight:Mean Mana",
                    "Fight:Innervate Mana",
                    "Fight:Innervate Size",

                    "Fight Stats:Spell Power",
                    "Fight Stats:Spell Crit",
                    "Fight Stats:Spell Haste",
                    "Fight Stats:Harmony",
                    "Fight Stats:Spell Mana Cost Reduction",
                    "Fight Stats:Spell Crit Extra Bonus",

                    "Mana Regen:Mana Regen",
                    "Mana Regen:Initial Mana Pool Regen",
                    "Mana Regen:Base Mana Regen",
                    "Mana Regen:Spirit Mana Regen",
                    "Mana Regen:Replenishment Mana Regen",
                    "Mana Regen:Revitalize Mana Regen",
                    "Mana Regen:Innervate Mana Regen",
                    "Mana Regen:Potion Mana Regen",
                    
                    "Solution:Total Score",
                    };

                List<string> list = new List<string>();
                list.AddRange(basic);

                string[] longNames = { "Raid Sustained", "Raid Burst", "Tank Sustained", "Tank Burst" };
                string[] names = { "Raid S.", "Raid B.", "Tank S.", "Tank B." };
                for (int i = 0; i < longNames.Length; ++i)
                    list.Add("Solution:" + longNames[i] + " HPS");

                list.Add("Proc Triggers:Proc trigger interval");
                list.Add("Proc Triggers:Proc periodic trigger interval");

                for (int i = 0; i < longNames.Length; ++i)
                    list.Add("Proc Triggers:" + longNames[i] + " Ticks/s");
                for (int i = 0; i < longNames.Length; ++i)
                    list.Add("Proc Triggers:" + longNames[i] + " Directs/s");

                for (int i = 0; i < longNames.Length; ++i)
                {
                    List<string> distPassiveList = new List<string>();
                    for (int j = 0; j < (int)TreePassive.Count; ++j)
                        distPassiveList.Add(longNames[i] + ':' + names[i] + ' ' + PassiveNames[j]);
                    distPassiveList.Sort();
                    list.AddRange(distPassiveList);

                    List<string> distActionList = new List<string>();
                    for (int j = 0; j < (int)TreeAction.Count; ++j)
                    {
                        if (names[i].Substring(0, 4) == "Raid" || ActionNames[j].Substring(0, 4) != "Raid")
                            distActionList.Add(longNames[i] + ':' + names[i] + ' ' + ActionNames[j]);
                    }
                    distActionList.Sort();
                    list.AddRange(distActionList);
                    
                    list.Add(longNames[i] + ':' + names[i] + " Idle");
                }

                List<string> actionList = new List<string>();
                for (int i = 0; i < (int)TreeAction.Count; ++i)
                    actionList.Add("Action HPCT:" + ActionNames[i] + " HPCT");
                for (int i = 0; i < (int)TreeAction.Count; ++i)
                    actionList.Add("Action HPM:" + ActionNames[i] + " HPM");
                for (int i = 0; i < (int)TreeAction.Count; ++i)
                    actionList.Add("Action MPCT:" + ActionNames[i] + " MPCT");
                actionList.Sort();
                list.AddRange(actionList);

                string[] spellProperties = { "Time", "Duration", "Mana", "Direct", "Tick", "Ticks", "Periodic", "Raid Direct", "Raid Periodic", "Tank Direct", "Tank Periodic" };
                for (int i = 0; i < (int)TreeSpell.Count; ++i)
                {
                    for (int j = 0; j < spellProperties.Length; ++j)
                        list.Add(SpellData[i].Name + ":" + SpellData[i].Name + ' ' + spellProperties[j]);
                }

                characterDisplayCalculationLabels = list.ToArray();
                return characterDisplayCalculationLabels;
            }
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Druid; } }
        public override ICalculationOptionsPanel CalculationOptionsPanel { get { if (calculationOptionsPanel == null) { calculationOptionsPanel = new CalculationOptionsPanelTree(); } return calculationOptionsPanel; } }
        private ICalculationOptionsPanel calculationOptionsPanel = null;
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationTree(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsTree(); }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsTree));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsTree calcOpts = serializer.Deserialize(reader) as CalculationOptionsTree;
            return calcOpts;
        }

        #endregion

        #region Relevancy
        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot)
        {
            // No ranged enchants allowed
            if (enchant.Slot == ItemSlot.Ranged) return false;
            // Make an exception for enchant 4091 - Enchant Off-Hand - Superior Intellect
            if (slot == ItemSlot.OffHand) return (enchant.Id == 4091 || enchant.Id == 4434);
            // Otherwise, return the base value
            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique)
        {
            if (slot == CharacterSlot.OffHand && item.Slot == ItemSlot.OneHand) return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs()
        {
            if (_relevantGlyphs == null)
            {
                // TODO: implement the commented out glyphs

                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Lifebloom");
                _relevantGlyphs.Add("Glyph of Rejuvenation");
                _relevantGlyphs.Add("Glyph of Regrowth");

                _relevantGlyphs.Add("Glyph of Wild Growth");
                _relevantGlyphs.Add("Glyph of Healing Touch");
                _relevantGlyphs.Add("Glyph of Innervate");
                _relevantGlyphs.Add("Glyph of Rebirth");
                _relevantGlyphs.Add("Glyph of Blooming");
            }
            return _relevantGlyphs;
        }

        /// <summary>
        /// List of itemtypes that are relevant for Tree
        /// </summary>
        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes
        {
            get
            {
                return _relevantItemTypes ?? (_relevantItemTypes = new List<ItemType>(new ItemType[]
                {
                            ItemType.None,
                            ItemType.Leather,
                            ItemType.Dagger,
                            ItemType.Staff,
                            ItemType.FistWeapon,
                            ItemType.OneHandMace,
                            ItemType.TwoHandMace,
                            ItemType.Idol,
                            ItemType.Relic,
                }));
            }
        }


        /// <summary>
        /// List of SpecialEffect Triggers that are relevant for tree model
        /// Every trigger listed here needs an implementation in ProcessSpecialEffects()
        /// A trigger not listed here should not appear in ProcessSpecialEffects()
        /// </summary>
        internal static List<Trigger> _RelevantTriggers = null;
        internal static List<Trigger> RelevantTriggers
        {
            get
            {
                return _RelevantTriggers ?? (_RelevantTriggers = new List<Trigger>() {
                            Trigger.Use,
                            Trigger.HealingSpellCast,
                            Trigger.HealingSpellCrit,
                            Trigger.HealingSpellHit,
                            Trigger.SpellCast,
                            Trigger.SpellCrit,        
                            Trigger.SpellHit, 
                            Trigger.HoTTick,
                            Trigger.DamageOrHealingDone,

                            Trigger.DamageSpellCast,
                            Trigger.DamageSpellHit,
                            Trigger.DamageSpellCrit,
                            Trigger.DoTTick,
                        });
            }
            //set { _RelevantTriggers = value; }
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            if (buff.SetName == "Stormrider's Vestments" || buff.SetName == "Obsidian Arborweave Vestments" || buff.SetName == "Deep Earth Vestments" || 
                buff.SetName == "Vestments of the Eternal Blossom" || buff.SetName == "Gladiator's Refuge")
                return true;

            // for buffs that are non-exclusive, allow anything that could be useful even slightly
            if (buff.Group == "Elixirs and Flasks" || buff.Group == "Potion" || buff.Group == "Food" || buff.Group == "Scrolls")
                return base.IsBuffRelevant(buff, character);
            else
            {
                if (character != null && Rawr.Properties.GeneralSettings.Default.HideProfEnchants && !character.HasProfession(buff.Professions))
                    return false;
                // Class Restrictions Enforcement
                else if (character != null && !buff.AllowedClasses.Contains(character.Class))
                    return false;

                return HasPrimaryStats(buff.Stats) || HasSecondaryStats(buff.Stats) || HasExtraStats(buff.Stats);
            }
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                // -- State Properties --
                // Base Stats
                Health = stats.Health,
                Mana = stats.Mana,
                Agility = stats.Agility,
                Stamina = stats.Stamina,
                Intellect = stats.Intellect,
                Spirit = stats.Spirit,
                Armor = stats.Armor,
                HasteRating = stats.HasteRating,
                CritRating = stats.CritRating,
                SpellPower = stats.SpellPower,
                Mastery = stats.Mastery,
                MasteryRating = stats.MasteryRating,
                // SpellPenetration = stats.SpellPenetration,
                Mp5 = stats.Mp5,
                BonusArmor = stats.BonusArmor,

                // Buffs / Debuffs
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,

                // Combat Values
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                SpellHit = stats.SpellHit,
                SpellCombatManaRegeneration = stats.SpellCombatManaRegeneration,
                TargetArmorReduction = stats.TargetArmorReduction,

                // Spell Combat Ratings
                SpellArcaneDamageRating = stats.SpellArcaneDamageRating,
                SpellNatureDamageRating = stats.SpellNatureDamageRating,

                // Equipment Effects
                ManaRestore = stats.ManaRestore,
                SpellsManaCostReduction = stats.SpellsManaCostReduction,
                NatureSpellsManaCostReduction = stats.NatureSpellsManaCostReduction,
                ManaCostReductionMultiplier = stats.ManaCostReductionMultiplier,
                Healed = stats.Healed,
                HealedPerSP = stats.HealedPerSP,

                // -- MultiplicativeStats --
                // Buffs / Debuffs
                BonusHealthMultiplier = stats.BonusHealthMultiplier,
                BonusManaMultiplier = stats.BonusManaMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusSpiritMultiplier = stats.BonusSpiritMultiplier,
                BaseArmorMultiplier = stats.BaseArmorMultiplier,
                BonusArmorMultiplier = stats.BonusArmorMultiplier,
                SpellHaste = stats.SpellHaste,
                BonusSpellPowerMultiplier = stats.BonusSpellPowerMultiplier,
                BonusHealingDoneMultiplier = stats.BonusHealingDoneMultiplier,
                BonusCritHealMultiplier = stats.BonusCritHealMultiplier,

                // -- NoStackStats
                MovementSpeed = stats.MovementSpeed,
                SnareRootDurReduc = stats.SnareRootDurReduc,
                FearDurReduc = stats.FearDurReduc,
                StunDurReduc = stats.StunDurReduc,
                BonusManaPotionEffectMultiplier = stats.BonusManaPotionEffectMultiplier,
                HighestStat = stats.HighestStat,
            };

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (RelevantTriggers.Contains(effect.Trigger) && HasRelevantStats(effect.Stats))
                    s.AddSpecialEffect(effect);
            }
            return s;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            return HasPrimaryStats(stats) || (HasSecondaryStats(stats) && !HasUnwantedStats(stats));
        }

        /// <summary>
        /// HasPrimaryStats() should return true if the Stats object has any stats that define the item
        /// as being 'for your class/spec'. For melee classes this is typical melee stats like Strength, 
        /// Agility, AP, Expertise... For casters it would be spellpower, intellect, ...
        /// As soon as an item/enchant/buff has any of the stats listed here, it will be assumed to be 
        /// relevant unless explicitely filtered out.
        /// Stats that could be usefull for both casters and melee such as HitRating, CritRating and Haste
        /// don't belong here, but are SecondaryStats. Specific melee versions of these do belong here 
        /// for melee, spell versions would fit here for casters.
        /// </summary>
        public bool HasPrimaryStats(Stats stats)
        {
            bool PrimaryStats =
                // -- State Properties --
                // Base Stats
                stats.Intellect != 0 ||
                stats.Spirit != 0 ||
                stats.SpellPower != 0 ||
                // stats.SpellPenetration != 0 ||

                // Combat Values
                stats.SpellHaste != 0 ||
                stats.SpellCrit != 0 ||
                stats.SpellCritOnTarget != 0 ||

                stats.Mp5 != 0 ||
                stats.ManaRestoreFromMaxManaPerSecond != 0 ||
                stats.SpellCombatManaRegeneration != 0 ||
                stats.ManaRestore != 0 ||
                stats.SpellsManaCostReduction != 0 ||
                stats.NatureSpellsManaCostReduction != 0 ||
                stats.ManaCostReductionMultiplier != 0 ||
                stats.Healed != 0 ||
                stats.HealedPerSP != 0 ||

                // Spell Combat Ratings

                // -- MultiplicativeStats --
                // Buffs / Debuffs
                stats.BonusIntellectMultiplier != 0 ||
                stats.BonusSpiritMultiplier != 0 ||
                stats.BonusCritHealMultiplier != 0 ||
                stats.BonusSpellPowerMultiplier != 0 ||
                stats.BonusHealingDoneMultiplier != 0 ||
                stats.BonusManaPotionEffectMultiplier != 0 ||
                stats.BonusPeriodicHealingMultiplier != 0
                ;

            if (!PrimaryStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (RelevantTriggers.Contains(effect.Trigger) && HasPrimaryStats(effect.Stats))
                    {
                        PrimaryStats = true;
                        break;
                    }
                }
            }

            return PrimaryStats;
        }

        /// <summary>
        /// HasSecondaryStats() should return true if the Stats object has any stats that are relevant for the 
        /// model but only to a smaller degree, so small that you wouldn't typically consider the item.
        /// Stats that are usefull to both melee and casters (HitRating, CritRating & Haste) fit in here also.
        /// An item/enchant/buff having these stats would be considered only if it doesn't have any of the 
        /// unwanted stats.  Group/Party buffs are slighly different, they would be considered regardless if 
        /// they have unwanted stats.
        /// Note that a stat may be listed here since it impacts the model, but may also be listed as an unwanted stat.
        /// </summary>
        public bool HasSecondaryStats(Stats stats)
        {
            bool SecondaryStats =
                // -- State Properties --
                // Base Stats
                stats.Mana != 0 ||
                stats.HasteRating != 0 ||
                stats.CritRating != 0 ||
                stats.Mastery != 0 ||
                stats.MasteryRating != 0 ||

                // -- MultiplicativeStats --
                // Buffs / Debuffs
                stats.BonusManaMultiplier != 0 ||

                // -- NoStackStats
                stats.MovementSpeed != 0 ||
                stats.SnareRootDurReduc != 0 ||
                stats.FearDurReduc != 0 ||
                stats.StunDurReduc != 0 ||
                stats.HighestStat != 0;

            if (!SecondaryStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (RelevantTriggers.Contains(effect.Trigger) && HasSecondaryStats(effect.Stats))
                    {
                        SecondaryStats = true;
                        break;
                    }
                }
            }

            return SecondaryStats;
        }

        /// <summary>
        /// Return true if the Stats object has any stats that don't influence the model but that you do want 
        /// to display in tooltips and in calculated summary values.
        /// </summary>
        public bool HasExtraStats(Stats stats)
        {
            bool ExtraStats =
                stats.Health != 0 ||
                stats.Stamina != 0 ||
                stats.Armor != 0 ||
                stats.BonusArmor != 0 ||
                stats.BonusHealthMultiplier != 0 ||
                stats.BonusStaminaMultiplier != 0 ||
                stats.BaseArmorMultiplier != 0 ||
                stats.BonusArmorMultiplier != 0;

            if (!ExtraStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (RelevantTriggers.Contains(effect.Trigger) && HasExtraStats(effect.Stats))
                    {
                        ExtraStats = true;
                        break;
                    }
                }
            }

            return ExtraStats;
        }

        /// <summary>
        /// Return true if the Stats object contains any stats that are making the item undesired.
        /// Any item having only Secondary stats would be removed if it also has one of these.
        /// </summary>
        public bool HasUnwantedStats(Stats stats)
        {
            /// List of stats that will filter out some buffs (Flasks, Elixirs & Scrolls), Enchants and Items.
            bool UnwantedStats =
                stats.Strength > 0 ||
                stats.Agility > 0 ||
                stats.AttackPower > 0 ||
                stats.ExpertiseRating > 0 ||
                stats.DodgeRating > 0 ||
                stats.ParryRating > 0 ||
                stats.BlockRating > 0 ||
                stats.Resilience > 0;

            if (!UnwantedStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (/*RelevantTriggers.Contains(effect.Trigger) && */HasUnwantedStats(effect.Stats))    // An unwanted stat could be behind a trigger we don't model.
                    {
                        UnwantedStats = true;
                        break;
                    }
                }
            }

            return UnwantedStats;
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsTree calcOpts)
        {
            List<Buff> removedBuffs = new List<Buff>();
            List<Buff> addedBuffs = new List<Buff>();

            Stats statsBuffs = GetBuffsStats(character.ActiveBuffs, character.SetBonusCount);

            foreach (Buff b in removedBuffs)
            {
                character.ActiveBuffsAdd(b);
            }
            foreach (Buff b in addedBuffs)
            {
                character.ActiveBuffs.Remove(b);
            }

            return statsBuffs;
        }

        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd("Devotion Aura");
            character.ActiveBuffsAdd("Arcane Brilliance (Mana)");
            character.ActiveBuffsAdd("Arcane Brilliance (SP%)");
            character.ActiveBuffsAdd("Blessing of Might (Mp5)");
            character.ActiveBuffsAdd("Leader of the Pack");
            character.ActiveBuffsAdd("Revitalize");
            character.ActiveBuffsAdd("Power Word: Fortitude");
            character.ActiveBuffsAdd("Mark of the Wild");
            character.ActiveBuffsAdd("Heroism/Bloodlust");
            character.ActiveBuffsAdd("Moonkin Form");
            character.ActiveBuffsAdd("Flask of the Draconic Mind");
            character.ActiveBuffsAdd("Intellect Food");
            character.ActiveBuffsAdd("Mythical Mana Potion");
        }
        #endregion

        private static SpecialEffect[] naturesGrace = null;
        public static SpecialEffect[] NaturesGrace
        {
            get
            {
                if (naturesGrace == null)
                {
                    naturesGrace = new SpecialEffect[4];
                    for (int i = 1; i <= 3; ++i)
                        naturesGrace[i] = new SpecialEffect(Trigger.Use, new Stats() { SpellHaste = 0.05f * i }, 15.0f, 60.0f, 1f);
                }
                return naturesGrace;
            }
        }

        public class TreeOfLifeStats : Stats
        {
            public override string ToString() { return "Tree of Life"; }
        }

        private static SpecialEffect treeOfLife = null;
        public static SpecialEffect TreeOfLife
        {
            get
            {
                if (treeOfLife == null)
                {
                        treeOfLife = new SpecialEffect(Trigger.Use, new TreeOfLifeStats(), 30.0f, 180.0f);
                }
                return treeOfLife;
            }
        }

        private static SpecialEffect t13innervateManaCostReduction = null;

        public static SpecialEffect T13InnervateManaCostReduction
        {
            get
            {
                if (t13innervateManaCostReduction == null)
                    t13innervateManaCostReduction = new SpecialEffect(Trigger.Use, new Stats() { ManaCostReductionMultiplier = 0.25f }, 15, 180);
                return t13innervateManaCostReduction;
            }
        }

        public class InactiveStats : Stats
        {
            public override string ToString() { return "Inactive"; }
        }

        private static SpecialEffect inactiveEffect = null;
        public static SpecialEffect InactiveEffect
        {
            get
            {
                if (inactiveEffect == null)
                {
                    inactiveEffect = new SpecialEffect(Trigger.Use, new InactiveStats(), 0, 0.0f);
                }
                return inactiveEffect;
            }
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsTree opts = character.CalculationOptions as CalculationOptionsTree;
            if (opts == null)
                opts = new CalculationOptionsTree();

            Stats stats = new Stats();
            Stats statsBase = BaseStats.GetBaseStats(character.Level, character.Class, character.Race);
            stats.Accumulate(statsBase);
            stats.BaseAgility = statsBase.Agility;
            stats.Mana *= 5.0f; // Resto specialisation mana bonus: Natural Insight
            stats.Mp5 *= 5.0f; // Resto specialisation mana bonus: Natural Insight

            // Get the gear/enchants/buffs stats loaded in
            stats.Accumulate(GetItemStats(character, additionalItem));
            stats.Accumulate(GetBuffsStats(character, opts));

            // Talented bonus multipliers
            Stats statsTalents = new Stats()
            {
                BonusIntellectMultiplier = (1 + (character.DruidTalents.HeartOfTheWild ? 0.06f : 0f)) * (Character.ValidateArmorSpecialization(character, ItemType.Leather) ? 1.05f : 1f) - 1f,
//                BonusManaMultiplier = 0.05f * character.DruidTalents.Furor,
//                ManaCostReductionMultiplier = character.DruidTalents.Moonglow * 0.03f,
//                SpellCrit = 0.02f * character.DruidTalents.NaturesMajesty
            };

            stats.Accumulate(statsTalents);

            FinalizeStats(stats, stats);

            // Derived stats: Health, mana pool, armor
            stats.Health = (float)Math.Round(stats.Health + StatConversion.GetHealthFromStamina(stats.Stamina));
            stats.Health = (float)Math.Floor(stats.Health * (1f + stats.BonusHealthMultiplier));
            stats.Mana = (float)Math.Round(stats.Mana + StatConversion.GetManaFromIntellect(stats.Intellect));
            stats.Mana = (float)Math.Floor(stats.Mana * (1f + stats.BonusManaMultiplier));

            // Armor
            stats.Armor = stats.Armor * (1f + stats.BaseArmorMultiplier);
            stats.BonusArmor = stats.BonusArmor * (1f + stats.BonusArmorMultiplier);
            stats.Armor += stats.BonusArmor;
            stats.Armor = (float)Math.Round(stats.Armor);

            int T13Count;
            character.SetBonusCount.TryGetValue("Deep Earth Vestments", out T13Count);

            int T14Count;
            character.SetBonusCount.TryGetValue("Vestments of the Eternal Blossom", out T14Count);

//            if (character.DruidTalents.NaturesGrace > 0)
//                stats.AddSpecialEffect(NaturesGrace[character.DruidTalents.NaturesGrace]);

            if (character.DruidTalents.Incarnation)
                stats.AddSpecialEffect(TreeOfLife);

            if (T13Count >= 2)
                stats.AddSpecialEffect(T13InnervateManaCostReduction);

            return stats;
        }

        public static void FinalizeStats(Stats stats, Stats statsMultipliers)
        {
            stats.Intellect += stats.HighestStat;
            stats.Intellect *= (1 + statsMultipliers.BonusIntellectMultiplier);
            stats.Agility *= (1 + statsMultipliers.BonusAgilityMultiplier);
            stats.Stamina *= (1 + statsMultipliers.BonusStaminaMultiplier);
            stats.Spirit *= (1 + statsMultipliers.BonusSpiritMultiplier);
        }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            // First things first, we need to ensure that we aren't using bad data
            CharacterCalculationsTree calc = new CharacterCalculationsTree();
            if (character == null)
                return calc;
            calc.BasicStats = (Stats)GetCharacterStats(character, additionalItem);
            new TreeSolver(character, calc);
            return calc;
        }

        #region Custom Charts

        private string[] customChartNames = new string[] { 
            "Haste Breakpoints (vs Crit, Mast, Spi, Int)",
            "Haste Breakpoints (vs Crit, Spi, Mast, Int)",
            "Haste Breakpoints (vs Mast, Crit, Spi, Int)",
            "Haste Breakpoints (vs Spi, Crit, Mask, Int)",
            "Haste Breakpoints (vs Mast, Spi, Crit, Int)",
            "Haste Breakpoints (vs Spi, Mast, Crit, Int)",
            "Haste Breakpoints (vs Int)"
        };

        public override string[] CustomChartNames
        {
            get
            {
                return customChartNames;
            }
        }

        public int[] GetHasteBreakpoints(Character character, int maxHaste)
        {
            Stats stats = GetCharacterStats(character, null);

            List<int> hasteRatingProcsList = new List<int>();
            List<float> spellHasteProcsList = new List<float>();

            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (CalculationsTree.RelevantTriggers.Contains(effect.Trigger))
                {
                    if (effect.Stats.HasteRating > 0)
                        hasteRatingProcsList.Add((int)effect.Stats.HasteRating);
                    if (effect.Stats.SpellHaste > 0)
                        spellHasteProcsList.Add(1 + effect.Stats.SpellHaste);
                }
            }

            int[] hasteRatingProcs = hasteRatingProcsList.ToArray();
            float[] spellHasteProcs = spellHasteProcsList.ToArray();

            List<int> baseHasteBreakpointsList = new List<int>();

            string[] spells = { "LB", "WG", "Rj", "Tq", "Rg" };
            int[] tickRates = { 1, 1, 3, 2, 2 };
            int[] durations = { 15, 7, 12, 8, 6 };   // TODO: Merge this with spell data at top of file

            int numHasteMults = 1 << spellHasteProcs.Length;
            double[] hasteMults = new double[numHasteMults];
            for (int i = 0; i < numHasteMults; ++i)
            {
                double hasteMult = 1 + stats.SpellHaste;
                for (int j = 0; j < spellHasteProcs.Length; ++j)
                {
                    if ((i & (1 << j)) != 0)
                        hasteMult *= spellHasteProcs[j];
                }

                hasteMults[i] = hasteMult;
            }

            int numHasteOffsets = 1 << hasteRatingProcs.Length;
            int[] hasteOffsets = new int[numHasteOffsets];
            for (int i = 0; i < numHasteOffsets; ++i)
            {
                int hasteOffset = 0;
                for (int j = 0; j < hasteRatingProcs.Length; ++j)
                {
                    if ((i & (1 << j)) != 0)
                        hasteOffset += hasteRatingProcs[j];
                }

                hasteOffsets[i] = hasteOffset;
            }

            int[] ticks = new int[numHasteMults * 4];
            int gotem = 0;
            bool[] hasteBreakpointsMask = new bool[maxHaste + 1];
            int maxh = maxHaste + hasteOffsets[hasteOffsets.Length - 1];
            for (int h = 0; h <= maxh; ++h)
            {
                double hasteRatingMult = 1 + StatConversion.GetSpellHasteFromRating(h);
                bool isBreakpoint = h == 0;
                int curGotem = (int)Math.Floor(4.0f * (1 + StatConversion.GetSpellHasteFromRating((float)h)) + 0.5f);
                if (curGotem > gotem)
                {
                    isBreakpoint = true;
                    gotem = curGotem;
                }
                for (int i = 0; i < numHasteMults * 4; ++i)
                {
                    double curTickRate = Math.Round(tickRates[i & 3] / (hasteMults[i >> 2] * hasteRatingMult), 3);
                    int curTicks = (int)Math.Ceiling(durations[i & 3] / curTickRate - 0.5);
                    if (curTicks > ticks[i])
                    {
                        ticks[i] = curTicks;
                        isBreakpoint = true;
                    }
                }
                if (isBreakpoint)
                {
                    for (int j = 0; j < numHasteOffsets; ++j)
                    {
                        int hr = h - hasteOffsets[j];
                        if (hr >= 0 && hr <= maxHaste)
                            hasteBreakpointsMask[hr] = true;
                    }
                }
            }

            List<int> hasteBreakpointsList = new List<int>();
            for (int h = 0; h <= maxHaste; ++h)
            {
                if (hasteBreakpointsMask[h])
                    hasteBreakpointsList.Add(h);
            }
            return hasteBreakpointsList.ToArray();
        }


        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            int i = customChartNames.FindIndex(x => x == chartName);
            if(i < 0)
                return new ComparisonCalculationBase[0];
            //if(i <= 4)
            {
                List<ComparisonCalculationBase> chart = new List<ComparisonCalculationBase>();
                CharacterCalculationsTree calcsBase = GetCharacterCalculations(character) as CharacterCalculationsTree;
                Character c2 = character.Clone();

                bool curIsBreakpoint = false;
                int[] hasteBreakpoints = GetHasteBreakpoints(character, 10000);   // Assume 10000 haste rating will be enough for this expansion
                foreach(int hb in hasteBreakpoints)
                {
                    float hasteDelta = hb - calcsBase.BasicStats.HasteRating;
                    float curHasteDelta;

                    Buff buff = new Buff();
                    buff.Name = "Haste adjustment to meet breakpoint";
                    buff.Stats.HasteRating += hasteDelta;
                    bool stop = false;

                    if(i == 6)
                    {
                        if (hasteDelta > calcsBase.BasicStats.Intellect)
                            stop = true;
                        buff.Stats.Intellect -= hasteDelta;
                    }
                    else
                    {
                        int[] ratings = new int[3];
                        int critPos = i >> 1;
                        ratings[critPos] = 0; // crit
                        ratings[critPos == 0 ? 1 : 0] = 1 + (i & 1);
                        ratings[critPos == 2 ? 1 : 2] = 2 - (i & 1);

                        for(int j = 0; j < 3; ++j)
                        {
                            switch(ratings[j])
                            {
                                case 0:
                                    curHasteDelta = Math.Min(hasteDelta, calcsBase.BasicStats.CritRating);
                                    buff.Stats.CritRating -= curHasteDelta;
                                    hasteDelta -= curHasteDelta;
                                    break;
                                case 1:
                                    curHasteDelta = Math.Min(hasteDelta, calcsBase.BasicStats.MasteryRating);
                                    buff.Stats.MasteryRating -= curHasteDelta;
                                    hasteDelta -= curHasteDelta;
                                    break;
                                case 2:
                                    curHasteDelta = Math.Min(hasteDelta, calcsBase.BasicStats.Spirit);
                                    buff.Stats.Spirit -= curHasteDelta;
                                    hasteDelta -= curHasteDelta;
                                    break;
                            }
                        }

                        if (hasteDelta > calcsBase.BasicStats.Intellect)
                            stop = true;
                        buff.Stats.Intellect -= hasteDelta;
                    }
                    if (stop)
                        break;
                    
                    c2.ActiveBuffs.Add(buff);
                    CharacterCalculationsTree calcsCur = GetCharacterCalculations(c2) as CharacterCalculationsTree;
                    c2.ActiveBuffs.Remove(buff);

                    ComparisonCalculationBase comp = Calculations.GetCharacterComparisonCalculations(calcsBase, calcsCur, String.Format("{0,4:F0}", hb), false, false);
                    if (hb == calcsBase.BasicStats.HasteRating)
                        comp.Description = "Current Gear (and on a breakpoint)";
                    else
                        comp.Description = buff.ToString();

                    chart.Add(comp);
                }

                if (!curIsBreakpoint)
                {
                    ComparisonCalculationBase comp = Calculations.GetCharacterComparisonCalculations(calcsBase, calcsBase, String.Format("{0,4:F0}" + (curIsBreakpoint ? "" : "(-)"), calcsBase.BasicStats.HasteRating), true, false);
                    comp.Description = "Current Gear (not on a breakpoint)";
                    chart.Add(comp);
                }

                return chart.ToArray();
            }
        }

        #endregion
    }
}
