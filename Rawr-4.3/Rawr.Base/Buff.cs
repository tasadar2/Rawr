using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Rawr
{
    public class BuffList : List<Buff>
    {
        public BuffList() : base() { }
        public BuffList(IEnumerable<Buff> collection) : base(collection) { }
    }

    /// <summary>
    /// Specify all the talents in every tree and their rank (threshold)
    /// before the Buff would conflict with it. Once conflicted, the Buff
    /// will remove itself.
    /// </summary>
    public class TalentsConflictingWithBuff
    {
        public TalentsConflictingWithBuff()
        {
            DeathKnightTalents = new DeathKnightTalents();
            DruidTalents = new DruidTalents();
            HunterTalents = new HunterTalents();
            MageTalents = new MageTalents();
            PaladinTalents = new PaladinTalents();
            PriestTalents = new PriestTalents();
            RogueTalents = new RogueTalents();
            ShamanTalents = new ShamanTalents();
            WarlockTalents = new WarlockTalents();
            WarriorTalents = new WarriorTalents();
        }
        public TalentsConflictingWithBuff Clone()
        {
            TalentsConflictingWithBuff clone = this.MemberwiseClone() as TalentsConflictingWithBuff;
            //
            clone.DeathKnightTalents = DeathKnightTalents.Clone() as DeathKnightTalents;
            clone.DruidTalents = DruidTalents.Clone() as DruidTalents;
            clone.HunterTalents = HunterTalents.Clone() as HunterTalents;
            clone.MageTalents = MageTalents.Clone() as MageTalents;
            clone.PaladinTalents = PaladinTalents.Clone() as PaladinTalents;
            clone.PriestTalents = PriestTalents.Clone() as PriestTalents;
            clone.RogueTalents = RogueTalents.Clone() as RogueTalents;
            clone.ShamanTalents = ShamanTalents.Clone() as ShamanTalents;
            clone.WarlockTalents = WarlockTalents.Clone() as WarlockTalents;
            clone.WarriorTalents = WarriorTalents.Clone() as WarriorTalents;
            //
            return clone;
        }
        #region Variables
        public DeathKnightTalents DeathKnightTalents { get; set; }
        public DruidTalents DruidTalents { get; set; }
        public HunterTalents HunterTalents { get; set; }
        public MageTalents MageTalents { get; set; }
        public PaladinTalents PaladinTalents { get; set; }
        public PriestTalents PriestTalents { get; set; }
        public RogueTalents RogueTalents { get; set; }
        public ShamanTalents ShamanTalents { get; set; }
        public WarlockTalents WarlockTalents { get; set; }
        public WarriorTalents WarriorTalents { get; set; }
        #endregion
        #region Functions
        private bool CheckForConflictingTalent(TalentsBase talents, TalentsBase charTalents)
        {
            for (int tree = 0; tree < 3; tree++)
            {
                // Check only trees with values in them
                if (talents.TreeCounts[tree] > 0)
                {
                    int pointsSeenSoFar = 0;
                    for (int talent = talents.TreeStartingIndexes[tree];
                        talent < (talents.TreeStartingIndexes[tree] + talents.TreeLengths[tree]);
                        talent++)
                    {
                        if (charTalents.Data[talent] >= talents.Data[talent]) { return true; }
                        pointsSeenSoFar += charTalents.Data[talent];
                        if (pointsSeenSoFar > charTalents.TreeCounts[tree]) { pointsSeenSoFar = 0; break; }
                    }
                }
            }
            for (int glyphTree = 0; glyphTree < 3; glyphTree++)
            {
                // Check only trees with values in them
                if (talents.GlyphTreeCounts[glyphTree] > 0)
                {
                    int glyphsSeenSoFar = 0;
                    for (int glyph = talents.GlyphTreeStartingIndexes[glyphTree];
                        glyph < (talents.GlyphTreeStartingIndexes[glyphTree] + talents.GlyphTreeLengths[glyphTree]);
                        glyph++)
                    {
                        if (talents.GlyphData[glyph] && charTalents.GlyphData[glyph]) { return true; }
                        if (charTalents.GlyphData[glyph]) { glyphsSeenSoFar++; }
                        if (glyphsSeenSoFar > charTalents.GlyphTreeCounts[glyphTree]) { glyphsSeenSoFar = 0; break; }
                    }
                }
            }
            return false;
        }
        public bool CheckForConflictingTalents(Character character)
        {
            switch (character.Class)
            {
                case CharacterClass.DeathKnight: { return CheckForConflictingTalent(DeathKnightTalents, character.DeathKnightTalents); }
                case CharacterClass.Druid: { return CheckForConflictingTalent(DruidTalents, character.DruidTalents); }
                case CharacterClass.Hunter: { return CheckForConflictingTalent(HunterTalents, character.HunterTalents); }
                case CharacterClass.Mage: { return CheckForConflictingTalent(MageTalents, character.MageTalents); }
                case CharacterClass.Paladin: { return CheckForConflictingTalent(PaladinTalents, character.PaladinTalents); }
                case CharacterClass.Priest: { return CheckForConflictingTalent(PriestTalents, character.PriestTalents); }
                case CharacterClass.Rogue: { return CheckForConflictingTalent(RogueTalents, character.RogueTalents); }
                case CharacterClass.Shaman: { return CheckForConflictingTalent(ShamanTalents, character.ShamanTalents); }
                case CharacterClass.Warlock: { return CheckForConflictingTalent(WarlockTalents, character.WarlockTalents); }
                case CharacterClass.Warrior: { return CheckForConflictingTalent(WarriorTalents, character.WarriorTalents); }
                default: { return false; }
            }
        }
        #endregion
        #region Statics
        #endregion
    }

    public class Buff
    {
        #region Variables
        public string Name;
        public string Group;
        public Stats Stats = new Stats();
        private List<CharacterClass> _allowedClasses = null;
        public List<CharacterClass> AllowedClasses
        {
            get
            {
                return _allowedClasses ??
                    (_allowedClasses = new List<CharacterClass>() {
                        CharacterClass.DeathKnight,
                        CharacterClass.Druid,
                        CharacterClass.Hunter,
                        CharacterClass.Mage,
                        CharacterClass.Paladin,
                        CharacterClass.Priest,
                        CharacterClass.Rogue,
                        CharacterClass.Shaman,
                        CharacterClass.Warlock,
                        CharacterClass.Warrior,
                    });
            }
            set { _allowedClasses = value; }
        }
        private List<Profession> _professions = null;
        public List<Profession> Professions
        {
            get
            {
                return _professions ??
                    (_professions = new List<Profession>() {
                        Profession.None,
                        Profession.Alchemy,
                        Profession.Blacksmithing,
                        Profession.Enchanting,
                        Profession.Engineering,
                        Profession.Herbalism,
                        Profession.Inscription,
                        Profession.Jewelcrafting,
                        Profession.Leatherworking,
                        Profession.Mining,
                        Profession.Skinning,
                        Profession.Tailoring,
                    });
            }
            set { _professions = value; }
        }
        public int SpellId = 0;
        public string SetName;
        public string Source;
        /// <summary>-1 = No class. Otherwise assign by CharacterClass enum</summary>
        public int SourceClass = -1;
        public int SetThreshold = 0;
        public List<Buff> Improvements = new List<Buff>();
        public bool IsTargetDebuff = false;
        public bool IsCustom = false;
        private List<string> _conflictingBuffs = null;
        public List<string> ConflictingBuffs
        {
            get { return _conflictingBuffs ?? (_conflictingBuffs = new List<string>() { Group }); }
            set { _conflictingBuffs = value; }
        }
        TalentsConflictingWithBuff TalentsConflictingWithBuff = null;
        public override string ToString() {
            string summary = Name + ": ";
            summary += Stats.ToString();
            summary = summary.TrimEnd(',', ' ', ':');
            return summary;
        }
        #endregion

        #region Events
        public static event EventHandler<EventArgs> BuffsLoaded;
        #endregion

        #region Static Functions
        public static void LoadDefaultBuffs(List<Buff> loadedBuffs, int level)
        {
            loadedBuffs = loadedBuffs ?? new List<Buff>();
            List<Buff> defaultBuffs = GetDefaultBuffs(level);
            Dictionary<string, Buff> allBuffs = new Dictionary<string, Buff>();
            foreach (Buff loadedBuff in loadedBuffs)
                if (loadedBuff.IsCustom)
                    allBuffs.Add(loadedBuff.Name, loadedBuff);
            foreach (Buff defaultBuff in defaultBuffs)
                if (!allBuffs.ContainsKey(defaultBuff.Name))
                    allBuffs.Add(defaultBuff.Name, defaultBuff);
            Buff[] allBuffArray = new Buff[allBuffs.Count];
            allBuffs.Values.CopyTo(allBuffArray, 0);
            _allBuffs = new BuffList(allBuffs.Values);
            _allBuffsByName = null;
            _allSetBonuses = null;
            _relevantBuffs = null;
            _relevantSetBonuses = null;
            CacheSetBonuses(); // cache it at the start because we don't like on demand caching with multithreading
            if (BuffsLoaded != null)
            {
                BuffsLoaded(null, EventArgs.Empty);
            }
        }

        public static Buff GetBuffByName(string name)
        {
#if RAWRSERVER
            if (_allBuffs == null)
            {
                LoadDefaultBuffs(null, 85);
            }
#endif
            Buff buff;
            AllBuffsByName.TryGetValue(name, out buff);
            return buff;
        }

        public static Buff GetBuffBySpellId(int Id)
        {
#if RAWRSERVER
            if (_allBuffs == null)
            {
                LoadDefaultBuffs(null, 85);
            }
#endif
            Buff buff;
            AllBuffsBySpellId.TryGetValue(Id, out buff);
            return buff;
        }

        #region Buff Relevancy to Character (Model, Professions)
        private static string _cachedModel = "";
        private static string _cachedPriProf = "";
        private static string _cachedSecProf = "";
        private static List<Buff> _relevantBuffs = new List<Buff>();
        public static CharacterClass cachedClass = CharacterClass.Druid;
        public static Profession cachedPriProf = Profession.None;
        public static Profession cachedSecProf = Profession.None;
        /// <summary>Returns relevant buffs, but not filtered for professions</summary>
        public static List<Buff> RelevantBuffs {
            get {
                if (Calculations.Instance == null
                    || (_cachedModel != Calculations.Instance.ToString()
                        || _cachedPriProf != cachedPriProf.ToString()
                        || _cachedSecProf != cachedSecProf.ToString())
                    || _relevantBuffs == null) {
                    if (Calculations.Instance != null) {
                        _cachedModel = Calculations.Instance.ToString();
                        _cachedPriProf = cachedPriProf.ToString();
                        _cachedSecProf = cachedSecProf.ToString();
                        _relevantBuffs = AllBuffs.FindAll(buff => Calculations.IsBuffRelevant(buff,
                            new Character() { Class = cachedClass, PrimaryProfession = cachedPriProf, SecondaryProfession = cachedSecProf, IsLoading = false }));
                    } else { _relevantBuffs = new List<Buff>(); }
                }
                return _relevantBuffs;
            }
        }
        #endregion

        public static void InvalidateBuffs() { _relevantBuffs = null; }

        #region Buffs that are Set Bonuses, which are handled separately from all other Buffs
        private static void CacheSetBonuses() {
            Dictionary<string, List<Buff>> listDict = new Dictionary<string, List<Buff>>();
            foreach (Buff buff in AllBuffs) 
            {
                string setName = buff.SetName;
                if (!string.IsNullOrEmpty(setName)) 
                {
                    List<Buff> setBonuses;
                    if (!listDict.TryGetValue(setName, out setBonuses))
                    {
                        setBonuses = new List<Buff>();
                        listDict[setName] = setBonuses;
                    }
                    setBonuses.Add(buff);
                }
            }
            foreach (var kvp in listDict)
            {
                setBonusesByName[kvp.Key] = kvp.Value.ToArray();
            }
        }

        private static List<Buff> _allSetBonuses = null;
        public static List<Buff> AllSetBonuses {
            get {
                if (_allSetBonuses == null) {
                    _allSetBonuses = AllBuffs.FindAll(buff => !string.IsNullOrEmpty(buff.SetName));
                }
                return _allSetBonuses;
            }
        }

        private static Dictionary<string, Buff[]> setBonusesByName = new Dictionary<string, Buff[]>();
        public static Buff[] GetSetBonuses(string setName) {
            Buff[] setBonuses;
            // if it's not cached we know we don't have any
            setBonusesByName.TryGetValue(setName, out setBonuses);
            return setBonuses;
        }

        private static List<Buff> _relevantSetBonuses = null;
        public static List<Buff> RelevantSetBonuses {
            get {
                if (Calculations.Instance == null || _cachedModel != Calculations.Instance.ToString() || _relevantSetBonuses == null) {
                    if (Calculations.Instance != null) {
                        _cachedModel = Calculations.Instance.ToString();
                        _relevantSetBonuses = AllBuffs.FindAll(buff =>
                            Calculations.HasRelevantStats(buff.GetTotalStats()) && !string.IsNullOrEmpty(buff.SetName));
                    } else { _relevantSetBonuses = new List<Buff>(); }
                }
                return _relevantSetBonuses;
            }
        }
        #endregion

        private static Dictionary<string, Buff> _allBuffsByName = null;
        private static Dictionary<string, Buff> AllBuffsByName {
            get {
                if (_allBuffsByName == null) {
                    _allBuffsByName = new Dictionary<string, Buff>();
                    //&UT&
                    // Chance for null
                    if (AllBuffs != null)
                    {
                        foreach (Buff buff in AllBuffs)
                        {
                            if (!_allBuffsByName.ContainsKey(buff.Name))
                            {
                                _allBuffsByName.Add(buff.Name, buff);
                                foreach (Buff improvement in buff.Improvements)
                                {
                                    if (!_allBuffsByName.ContainsKey(improvement.Name))
                                    {
                                        _allBuffsByName.Add(improvement.Name, improvement);
                                    }
                                }
                            }
                        }
                    }
                }
                return _allBuffsByName;
            }
        }

        private static Dictionary<int, Buff> _allBuffsBySpellId = null;
        private static Dictionary<int, Buff> AllBuffsBySpellId
        {
            get
            {
                if (_allBuffsBySpellId == null)
                {
                    _allBuffsBySpellId = new Dictionary<int, Buff>();
                    //&UT&
                    // Chance for null
                    if (AllBuffs != null)
                    {
                        foreach (Buff buff in AllBuffs)
                        {
                            if (!_allBuffsBySpellId.ContainsKey(buff.SpellId))
                            {
                                _allBuffsBySpellId.Add(buff.SpellId, buff);
                                int iter = 0;
                                foreach (Buff improvement in buff.Improvements)
                                {
                                    // TODO JOTHAY VERIFY THIS
                                    int impSpellId = improvement.SpellId != 0
                                        ? improvement.SpellId * 100
                                        : buff.SpellId * 100 + iter;
                                    if (!_allBuffsBySpellId.ContainsKey(impSpellId))
                                    {
                                        _allBuffsBySpellId.Add(impSpellId, improvement);
                                    }
                                    iter++;
                                }
                            }
                        }
                    }
                }
                return _allBuffsBySpellId;
            }
        }

        public Stats GetTotalStats()
        {
            Stats ret = this.Stats.Clone();
            foreach (Buff buff in Improvements)
                ret.Accumulate(buff.Stats);
            return ret;
        }

        private static BuffList _allBuffs = null;
        public static List<Buff> AllBuffs
        {
            get
            {
#if RAWRSERVER
                if (_allBuffs == null)
                {
                    LoadDefaultBuffs(null, 85);
                }
#endif
                return _allBuffs;
            }
        }
        #endregion

        #region This generates the actual Buff List
        private static List<Buff> GetDefaultBuffs() { return GetDefaultBuffs(StatConversion.DEFAULTPLAYERLEVEL); }
        private static List<Buff> GetDefaultBuffs(int level = StatConversion.DEFAULTPLAYERLEVEL) {
            List<Buff> defaultBuffs = new List<Buff>();
            Buff buff;

            // these values are for spells that have generic scaling (-1 group in gtSpellScaling in DBC, starting at 1100)
            float scalingValue;
            switch (level)
            {
                case 80: scalingValue = 125f; break;
                case 81: scalingValue = 305f; break;
                case 82: scalingValue = 338f; break;
                case 83: scalingValue = 375f; break;
                case 84: scalingValue = 407f; break;
                case 85: default: scalingValue = 443f; break;
            }

            #region Buffs

            #region Agility and Strength
            defaultBuffs.Add(new Buff
            {
                Name = "Strength of Earth Totem",
                Source = "Shaman",
                Group = "Agility and Strength",
                Stats = { Strength = (float)Math.Round(scalingValue * 1.24), Agility = (float)Math.Round(scalingValue * 1.24) },
                ConflictingBuffs = { "Agility", "Strength" },
                SpellId = 8075,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Horn of Winter",
                Source = "Death Knight",
                Group = "Agility and Strength",
                Stats = { Strength = (float)Math.Round(scalingValue * 1.24), Agility = (float)Math.Round(scalingValue * 1.24) },
                ConflictingBuffs = { "Agility", "Strength" },
                SpellId = 57330,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Battle Shout",
                Source = "Warrior",
                Group = "Agility and Strength",
                Stats = { Strength = (float)Math.Round(scalingValue * 1.24), Agility = (float)Math.Round(scalingValue * 1.24) },
                ConflictingBuffs = { "Agility", "Strength" },
                SpellId = 6673,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Roar of Courage",
                Source = "Hunter w/ Cat OR Beast Mastery Hunter w/ Spirit Beast",
                Group = "Agility and Strength",
                Stats = { Strength = (float)Math.Round(scalingValue * 1.24), Agility = (float)Math.Round(scalingValue * 1.24) },
                ConflictingBuffs = { "Agility", "Strength" },
                SpellId = 93435,
            });
            #endregion

            #region Armor
            defaultBuffs.Add(new Buff
            {
                Name = "Stoneskin Totem",
                Source = "Shaman",
                Group = "Armor",
                Stats = { BonusArmor = (float)Math.Round(scalingValue * 9.2) },
                SpellId = 8071,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Devotion Aura",
                Source = "Paladin",
                Group = "Armor",
                Stats = { BonusArmor = (float)Math.Round(scalingValue * 9.2) },
                SpellId = 465,
            });
            #endregion

            #region Damage Reduction (Major %)
            defaultBuffs.Add(new Buff
            {
                Name = "Ancestral Healing",
                Source = "Resto Shaman",
                Group = "Damage Reduction (Major %)",
                Stats = { DamageTakenReductionMultiplier = 0.10f },
                SpellId = 16235,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Inspiration",
                Source = "Holy Priest",
                Group = "Damage Reduction (Major %)",
                Stats = { DamageTakenReductionMultiplier = 0.10f },
                SpellId = 15362,
            });
            #endregion

            #region Attack Power (%)
            defaultBuffs.Add(new Buff
            {
                Name = "Trueshot Aura",
                Source = "MM Hunter",
                Group = "Attack Power (%)",
                Stats = { BonusAttackPowerMultiplier = 0.2f, BonusRangeAttackPowerMultiplier = 0.1f },
                SpellId = 19506,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Unleashed Rage",
                Source = "Enhance Shaman",
                Group = "Attack Power (%)",
                Stats = { BonusAttackPowerMultiplier = 0.2f, BonusRangeAttackPowerMultiplier = 0.1f },
                SpellId = 30808,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Abomination's Might",
                Source = "Blood Death Knight",
                Group = "Attack Power (%)",
                Stats = { BonusAttackPowerMultiplier = 0.2f, BonusRangeAttackPowerMultiplier = 0.1f },
                SpellId = 53138,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Blessing of Might (AP%)",
                Source = "Paladin",
                Group = "Attack Power (%)",
                Stats = { BonusAttackPowerMultiplier = 0.2f, BonusRangeAttackPowerMultiplier = 0.1f },
                SpellId = 19740, // This is the first Blessing of Might, the second will be 19740 * 100
            });
            #endregion

            #region Damage (%)
            defaultBuffs.Add(new Buff
            {
                Name = "Ferocious Inspiration",
                Source = "BM Hunter",
                Group = "Damage (%)",
                Stats = { BonusDamageMultiplier = 0.03f },
                SpellId = 75447,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Sanctified Retribution",
                Source = "Ret Paladin",
                Group = "Damage (%)",
                Stats = { BonusDamageMultiplier = 0.03f },
                //SpellId = 0000, // TODO: Cannot find this spell on Wowhead
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Arcane Tactics",
                Source = "Arcane Mage",
                Group = "Damage (%)",
                Stats = { BonusDamageMultiplier = 0.03f },
                SpellId = 82930,
            });
            #endregion

            #region Stamina
            defaultBuffs.Add(new Buff
            {
                Name = "Power Word: Fortitude",
                Source = "Priest",
                Group = "Stamina",
                Stats = { Stamina = (float)Math.Round(scalingValue * 1.32) },
                ConflictingBuffs = new List<string>(new string[] { "Stamina" }),
                SpellId = 21562,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Qiraji Fortitude",
                Source = "Beast Mastery Hunter w/ Silithid",
                Group = "Stamina",
                Stats = { Stamina = (float)Math.Round(scalingValue * 1.32) },
                ConflictingBuffs = new List<string>(new string[] { "Stamina" }),
                SpellId = 90364,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Blood Pact",
                Source = "Warlock w/ Imp",
                Group = "Stamina",
                Stats = { Stamina = (float)Math.Round(scalingValue * 1.32) },
                ConflictingBuffs = new List<string>(new string[] { "Stamina" }),
                SpellId = 6307,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Commanding Shout",
                Source = "Warrior",
                Group = "Stamina",
                Stats = { Stamina = (float)Math.Round(scalingValue * 1.32) },
                ConflictingBuffs = new List<string>(new string[] { "Stamina" }),
                SpellId = 469,
            });
            #endregion

            #region Mana
            defaultBuffs.Add(new Buff
            {
                Name = "Arcane Brilliance (Mana)",
                Source = "Mage",
                Group = "Mana",
                Stats = { Mana = (float)Math.Round(scalingValue * 4.8) },
                ConflictingBuffs = new List<string>(new string[] { "Mana" }),
                SpellId = 1459, // This is the first Arcane Brilliance, the second will be 1459 * 100
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Fel Intelligence (Mana)",
                Source = "Warlock",
                Group = "Mana",
                Stats = { Mana = (float)Math.Round(scalingValue * 4.8) },
                ConflictingBuffs = new List<string>(new string[] { "Mana" }),
                SpellId = 54424, // This is the first Fel Intelligence, the second will be 54424 * 100
            });
            #endregion

            #region Mana Regeneration
            defaultBuffs.Add(new Buff
            {
                Name = "Mana Spring Totem",
                Source = "Shaman",
                Group = "Mana Regeneration",
                Stats = { Mp5 = (float)Math.Round(scalingValue * 0.736) },
                SpellId = 5675,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Fel Intelligence (Mp5)",
                Source = "Warlock w/ Felhunter",
                Group = "Mana Regeneration",
                Stats = { Mp5 = (float)Math.Round(scalingValue * 0.736) },
                SpellId = 54424 * 100, // This is the second Fel Intelligence
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Blessing of Might (Mp5)",
                Source = "Paladin",
                Group = "Mana Regeneration",
                Stats = { Mp5 = (float)Math.Round(scalingValue * 0.736) },
                SpellId = 19740 * 100, // This is the second Blessing of Might
            });
            #endregion

            #region Spell Power
            defaultBuffs.Add(new Buff
            {
                Name = "Arcane Brilliance (SP%)",
                Source = "Mage",
                Group = "Spell Power",
                Stats = { BonusSpellPowerMultiplier = 0.06f },
                SpellId = 1459 * 100, // This is the second Arcane Brilliance
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Flametongue Totem",
                Source = "Shaman",
                Group = "Spell Power",
                Stats = { BonusSpellPowerMultiplier = 0.06f },
                SpellId = 8227,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Totemic Wrath",
                Source = "Elemental Shaman",
                Group = "Spell Power",
                Stats = { BonusSpellPowerMultiplier = 0.10f },
                SpellId = 77746,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Demonic Pact",
                Source = "Demonology Warlock",
                Group = "Spell Power",
                Stats = { BonusSpellPowerMultiplier = 0.10f, },
                SpellId = 47236,
            });
            #endregion

            #region Critical Strike Chance
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Leader of the Pack",
                Source = "Feral Druid",
                Group = "Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f, SpellCrit = 0.05f },
                SpellId = 17007,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Rampage",
                Source = "Fury Warrior",
                Group = "Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f, SpellCrit = 0.05f },
                SpellId = 29801,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Honor Among Thieves",
                Source = "Subtlety Rogue",
                Group = "Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f, SpellCrit = 0.05f },
                SpellId = 51701,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Elemental Oath",
                Source = "Elemental Shaman",
                Group = "Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f, SpellCrit = 0.05f },
                SpellId = 51470,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Furious Howl",
                Source = "Hunter w/ Wolf",
                Group = "Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f, SpellCrit = 0.05f },
                SpellId = 24604,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Terrifying Roar",
                Source = "Beast Mastery Hunter w/ Devilsaur",
                Group = "Critical Strike Chance",
                Stats = { PhysicalCrit = 0.05f, SpellCrit = 0.05f },
                SpellId = 90309,
            });
            #endregion

            #region Physical Haste
            defaultBuffs.Add(new Buff
            {
                Name = "Windfury Totem",
                Source = "Shaman",
                Group = "Physical Haste",
                Stats = { PhysicalHaste = 0.1f },
                SpellId = 8512,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Improved Icy Talons",
                Source = "Frost Death Knight",
                Group = "Physical Haste",
                Stats = { PhysicalHaste = 0.1f },
                SpellId = 55610,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Hunting Party",
                Source = "Survival Hunter",
                Group = "Physical Haste",
                Stats = { PhysicalHaste = 0.1f },
                SpellId = 53290,
            });
            #endregion

            #region Replenishment
            defaultBuffs.Add(new Buff
            {
                Name = "Revitalize",
                Source = "Restoration Druid",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.001f },
                SpellId = 48544,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Communion",
                Source = "Retribution Paladin",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.001f },
                SpellId = 31876,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Vampiric Touch",
                Source = "Shadow Priest",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.001f },
                SpellId = 34914,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Soul Leech",
                Source = "Dest Warlock",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.001f },
                SpellId = 30295,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Enduring Winter",
                Source = "Frost Mage",
                Group = "Replenishment",
                Stats = { ManaRestoreFromMaxManaPerSecond = 0.001f },
                SpellId = 86508,
            });
            #endregion

            #region Burst Mana Regeneration
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Hymn of Hope",
                Source = "Holy Priest",
                Group = "Burst Mana Regeneration",
                Stats = { },
                Improvements = {
                    new Buff {
                        Name = "Glyphed Hymn of Hope",
                        Source = "Holy Priest",
                        Stats = { },
                        SpellId = 64901 * 1000, // Improvements are id*1000
                    }
                },
                ConflictingBuffs = { }, // preventing this group from conflicting with itself
                SpellId = 64901,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { // 3% every 2 sec and 20% Bonus Mana for 8 sec
                    ManaRestoreFromMaxManaPerSecond = (0.03f / 2f),
                    BonusManaMultiplier = 0.20f
                },
                8f, 6 * 60)
            );
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { // Glyph adds 2 sec to the Duration
                    ManaRestoreFromMaxManaPerSecond = (0.03f / 2f),
                    BonusManaMultiplier = 0.20f
                },
                2f, 6 * 60)
            );

            defaultBuffs.Add(buff = new Buff {
                Name = "Mana Tide Totem",
                Source = "Resto Shaman",
                Group = "Burst Mana Regeneration",
                Stats = { },
                Improvements = { },
                ConflictingBuffs = { }, // preventing this group from conflicting with itself
                SpellId = 16190,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { // Spirit increased by 350% for 12 seconds.
                    BonusSpiritMultiplier = 3.5f,
                },
                12f, 3 * 60)
            );
            #endregion

            #region Focus Magic, Spell Critical Strike Chance
            defaultBuffs.Add(new Buff()
            {
                Name = "Focus Magic",
                Source = "Mage",
                Group = "Focus Magic, Spell Critical Strike Chance",
                Stats = { SpellCrit = 0.03f },
                SpellId = 54646,
            });
            #endregion

            #region Dark Intent
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Dark Intent",
                Source = "Warlock",
                Group = "Dark Intent",
                Stats = { SpellHaste = 0.03f },
                SpellId = 80398,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DarkIntentCriticalPeriodicDamageOrHealing,
                new Stats() { BonusPeriodicDamageMultiplier = 0.03f, BonusPeriodicHealingMultiplier = 0.03f },
                7f, 0f, 1f, 3));
            #endregion

            #region Spell Haste
            defaultBuffs.Add(new Buff
            {
                Name = "Wrath of Air Totem",
                Source = "Shaman",
                Group = "Spell Haste",
                Stats = { SpellHaste = 0.05f },
                SpellId = 3738,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Moonkin Form",
                Source = "Balance Druid",
                Group = "Spell Haste",
                Stats = { SpellHaste = 0.05f },
                SpellId = 24858,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Mind Quickening",
                Source = "Shadow Priest",
                Group = "Spell Haste",
                Stats = { SpellHaste = 0.05f },
                SpellId = 19339,
            });
            #endregion

            #region Stat Multiplier
            defaultBuffs.Add(new Buff
            {
                Name = "Blessing of Kings",
                Source = "Paladin",
                Group = "Stat Multiplier",
                Stats =
                {
                    BonusStrengthMultiplier = 0.05f,
                    BonusAgilityMultiplier = 0.05f,
                    BonusIntellectMultiplier = 0.05f,
                    BonusStaminaMultiplier = 0.05f,
                    NatureResistanceBuff = 97f, //(int)(level / 2f + (level - 70) / 2f * 5f + (level - 80) / 2f * 7f - 0.5f),
                    FireResistanceBuff = 97f, //(int)(level / 2f + (level - 70) / 2f * 5f + (level - 80) / 2f * 7f - 0.5f),
                    FrostResistanceBuff = 97f, //(int)(level / 2f + (level - 70) / 2f * 5f + (level - 80) / 2f * 7f - 0.5f),
                    ShadowResistanceBuff = 97f, //(int)(level / 2f + (level - 70) / 2f * 5f + (level - 80) / 2f * 7f - 0.5f),
                    ArcaneResistanceBuff = 97f, //(int)(level / 2f + (level - 70) / 2f * 5f + (level - 80) / 2f * 7f - 0.5f),
                },
                SpellId = 20217,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Mark of the Wild",
                Source = "Druid",
                Group = "Stat Multiplier",
                Stats =
                {
                    BonusStrengthMultiplier = 0.05f,
                    BonusAgilityMultiplier = 0.05f,
                    BonusIntellectMultiplier = 0.05f,
                    BonusStaminaMultiplier = 0.05f,
                    NatureResistanceBuff = 97f,
                    FireResistanceBuff = 97f,
                    FrostResistanceBuff = 97f,
                    ShadowResistanceBuff = 97f,
                    ArcaneResistanceBuff = 97f,
                },
                SpellId = 1126,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Embrace of the Shale Spider",
                Source = "Beast Mastery Hunter w/ Shale Spider",
                Group = "Stat Multiplier",
                Stats =
                {
                    BonusStrengthMultiplier = 0.05f,
                    BonusAgilityMultiplier = 0.05f,
                    BonusIntellectMultiplier = 0.05f,
                    BonusStaminaMultiplier = 0.05f,
                },
                SpellId = 90363,
            });
            #endregion

            #region Resistance
            defaultBuffs.Add(new Buff()
            {
                Name = "Resistance Aura",
                Source = "Paladin",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Shadow Resistance Buff", "Fire Resistance Buff", "Frost Resistance Buff" }),
                Stats =
                {
                    ShadowResistanceBuff = 195f,
                    FireResistanceBuff = 195f,
                    FrostResistanceBuff = 195f,
                },
                SpellId = 19891,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Shadow Protection",
                Source = "Priest",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Shadow Resistance Buff" }),
                Stats = { ShadowResistanceBuff = 195f },
                SpellId = 27683,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Aspect of the Wild",
                Source = "Hunter",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Nature Resistance Buff" }),
                Stats = { NatureResistanceBuff = 195f },
                SpellId = 20043,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elemental Resistance Totem",
                Source = "Shaman",
                Group = "Resistance",
                ConflictingBuffs = new List<string>(new string[] { "Fire Resistance Buff", "Frost Resistance Buff", "Nature Resistance Buff" }),
                Stats =
                {
                    NatureResistanceBuff = 195f,
                    FireResistanceBuff = 195f,
                    FrostResistanceBuff = 195f,
                },
                SpellId = 8184,
            });
            #endregion

            #region Pushback Protection
            defaultBuffs.Add(new Buff()
            {
                Name = "Concentration Aura",
                Source = "Paladin",
                Group = "Pushback Protection",
                Stats = { InterruptProtection = 0.35f },
                SpellId = 19746,
            });
            #endregion

            #region Class Buffs
            defaultBuffs.Add(new Buff()
            {
                Name = "Mage Armor",
                Group = "Class Buffs",
                Stats = { MageMageArmor = 1f },
                ConflictingBuffs = new List<string>(new string[] { "Mage Class Armor" }),
                AllowedClasses = new List<CharacterClass> { CharacterClass.Mage },
                SpellId = 6117,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Molten Armor",
                Group = "Class Buffs",
                Stats = { MageMoltenArmor = 1f },
                ConflictingBuffs = new List<string>(new string[] { "Mage Class Armor" }),
                AllowedClasses = new List<CharacterClass> { CharacterClass.Mage },
                SpellId = 30482,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Ice Armor",
                Group = "Class Buffs",
                Stats = { MageIceArmor = 1f },
                ConflictingBuffs = new List<string>(new string[] { "Mage Class Armor" }),
                AllowedClasses = new List<CharacterClass> { CharacterClass.Mage },
                SpellId = 7302,
            });
            #endregion

            #region Temp Power Boosts
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Tricks of the Trade",
                Source = "Rogue",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Tricks" },
                Stats = new Stats(),
                SpellId = 57934,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { BonusDamageMultiplier = 0.15f, }, 6f, 30f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Tricks of the Trade (Glyphed)",
                Source = "Rogue",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Tricks" },
                Stats = new Stats(),
                SpellId = 63256,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { BonusDamageMultiplier = 0.15f-0.05f, }, 6f, 30f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Heroism/Bloodlust",
                Source = "Shaman",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Heroism" },
                Stats = new Stats(),
                SpellId = 32182, // Bloodlust is 2825, but only searchable by Heroism
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { PhysicalHaste = 0.30f, RangedHaste = 0.30f, SpellHaste = 0.30f }, 40f, 10f * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Time Warp",
                Source = "Mage",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Heroism" },
                Stats = new Stats(),
                SpellId = 80353,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { PhysicalHaste = 0.30f, RangedHaste = 0.30f, SpellHaste = 0.30f }, 40f, 10f * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Ancient Hysteria",
                Source = "Beast Mastery Hunter w/ Core Hound",
                Group = "Temp Power Boost",
                ConflictingBuffs = new List<string>() { "Heroism" },
                Stats = new Stats(),
                SpellId = 90355,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { PhysicalHaste = 0.30f, RangedHaste = 0.30f, SpellHaste = 0.30f }, 40f, 10f * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Shattering Throw",
                Source = "Arms Warrior",
                Group = "Temp Power Boost",
                Stats = new Stats(),
                ConflictingBuffs = new List<string>() { "Shattering" },
                SpellId = 64382,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { TargetArmorReduction = 0.20f }, 10f, 5 * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Unholy Frenzy",
                Source = "Unholy Death Knight",
                Group = "Temp Power Boost",
                Stats = new Stats(),
                ConflictingBuffs = new List<string>() { "Unholy Frenzy" },
                SpellId = 49016,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { PhysicalHaste = 0.20f, HealthRestoreFromMaxHealth = -0.01f * 30f }, 30f, 3 * 60f));
            defaultBuffs.Add(buff = new Buff
            {
                Name = "Power Infusion",
                Source = "Disc Priest",
                Group = "Temp Power Boost",
                Stats = new Stats(),
                ConflictingBuffs = new List<string>() { "PowerInfusion" },
                SpellId = 10060,
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use,
                new Stats() { SpellHaste = 0.20f, ManaCostReductionMultiplier = 0.20f }, 15f, 2 * 60f));
            #endregion

            #endregion

            #region Debuffs

            #region Armor
            defaultBuffs.Add(new Buff
            {
                Name = "Faerie Fire",
                Source = "Druid",
                Group = "Armor Debuff",
                Stats = { TargetArmorReduction = 0.12f },
                IsTargetDebuff = true,
                SpellId = 770,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Expose Armor",
                Source = "Rogue",
                Group = "Armor Debuff",
                Stats = { TargetArmorReduction = 0.12f },
                IsTargetDebuff = true,
                SpellId = 8647,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Sunder Armor",
                Source = "Warrior",
                Group = "Armor Debuff",
                Stats = { TargetArmorReduction = 0.12f },
                IsTargetDebuff = true,
                SpellId = 7386,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Corrosive Spit",
                Source = "Hunter w/ Serpent",
                Group = "Armor Debuff",
                Stats = { TargetArmorReduction = 0.12f },
                IsTargetDebuff = true,
                SpellId = 35387,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Tear Armor",
                Source = "Hunter w/ Raptor",
                Group = "Armor Debuff",
                Stats = { TargetArmorReduction = 0.12f },
                IsTargetDebuff = true,
                SpellId = 50498,
            });
            #endregion

            #region Ranged Attack Power
            defaultBuffs.Add(new Buff()
            {
                Name = "Hunter's Mark",
                Source = "Hunter",
                Group = "Ranged Attack Power",
                Stats = { RangedAttackPower = 1772f },
                IsTargetDebuff = true,
                SpellId = 1130,
            });
            #endregion

            #region Target Physical Damage Reduction
            defaultBuffs.Add(new Buff()
            {
                Name = "Demoralizing Shout",
                Group = "Target Physical Damage Reduction",
                Source = "Warrior",
                Stats = { BossPhysicalDamageDealtReductionMultiplier = 0.10f },
                IsTargetDebuff = true,
                SpellId = 1160,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Demoralizing Roar",
                Group = "Target Physical Damage Reduction",
                Source = "Bear Druid OR Hunter w/ Bear",
                Stats = { BossPhysicalDamageDealtReductionMultiplier = 0.10f },
                IsTargetDebuff = true,
                SpellId = 99,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vindication",
                Group = "Target Physical Damage Reduction",
                Source = "Protection Paladin",
                Stats = { BossPhysicalDamageDealtReductionMultiplier = 0.10f },
                IsTargetDebuff = true,
                SpellId = 26016,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Curse of Weakness",
                Group = "Target Physical Damage Reduction",
                Source = "Warlock",
                Stats = { BossPhysicalDamageDealtReductionMultiplier = 0.10f },
                IsTargetDebuff = true,
                SpellId = 702,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scarlet Fever",
                Group = "Target Physical Damage Reduction",
                Source = "Blood Death Knight",
                Stats = { BossPhysicalDamageDealtReductionMultiplier = 0.10f },
                IsTargetDebuff = true,
                SpellId = 81132,
            });
            #endregion

            #region Bleed Damage
            TalentsConflictingWithBuff BleedDamageConfTalents = new TalentsConflictingWithBuff();
            BleedDamageConfTalents.WarriorTalents.BloodFrenzy = 1;
            BleedDamageConfTalents.RogueTalents.Hemorrhage = 1;
            defaultBuffs.Add(new Buff
            {
                Name = "Mangle",
                Source = "Feral Druid",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
                SpellId = 33876, // There is a bear form id too, but only doing the cat form
                TalentsConflictingWithBuff = BleedDamageConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Blood Frenzy (Bleed)",
                Source = "Arms Warrior",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
                SpellId = 29859, // This is the first Blood Frenzy, the second will be 29859 * 10
                TalentsConflictingWithBuff = BleedDamageConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Hemorrhage",
                Source = "Subtlety Rogue",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
                SpellId = 16511,
                TalentsConflictingWithBuff = BleedDamageConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Tendon Rip",
                Source = "Hunter w/ Hyena",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
                SpellId = 50271,
                TalentsConflictingWithBuff = BleedDamageConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Gore",
                Source = "Hunter w/ Boar",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
                SpellId = 35290,
                TalentsConflictingWithBuff = BleedDamageConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Stampede",
                Source = "Beast Mastery Hunter w/ Rhino",
                Group = "Bleed Damage",
                Stats = { BonusBleedDamageMultiplier = 0.3f },
                IsTargetDebuff = true,
                SpellId = 57386,
                TalentsConflictingWithBuff = BleedDamageConfTalents.Clone(),
            });
            #endregion

            #region Physical Vulnerability
            TalentsConflictingWithBuff PhysVulnConfTalents = new TalentsConflictingWithBuff();
            PhysVulnConfTalents.WarriorTalents.BloodFrenzy = 1;
            defaultBuffs.Add(new Buff
            {
                Name = "Blood Frenzy (Vuln)",
                Source = "Arms Warrior",
                Group = "Physical Vulnerability",
                Stats = { BonusPhysicalDamageMultiplier = 0.04f },
                IsTargetDebuff = true,
                SpellId = 29859 * 10, // This is the second Blood Frenzy
                TalentsConflictingWithBuff = PhysVulnConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Savage Combat",
                Source = "Combat Rogue",
                Group = "Physical Vulnerability",
                Stats = { BonusPhysicalDamageMultiplier = 0.04f },
                IsTargetDebuff = true,
                SpellId = 58413,
                TalentsConflictingWithBuff = PhysVulnConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Brittle Bones",
                Source = "Frost Death Knight",
                Group = "Physical Vulnerability",
                Stats = { BonusPhysicalDamageMultiplier = 0.04f },
                IsTargetDebuff = true,
                SpellId = 81328,
                TalentsConflictingWithBuff = PhysVulnConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Ravage",
                Source = "Hunter w/ Ravager",
                Group = "Physical Vulnerability",
                Stats = { BonusPhysicalDamageMultiplier = 0.04f },
                IsTargetDebuff = true,
                SpellId = 50518,
                TalentsConflictingWithBuff = PhysVulnConfTalents.Clone(),
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Acid Spit",
                Source = "Beast Mastery Hunter w/ Worm",
                Group = "Physical Vulnerability",
                Stats = { BonusPhysicalDamageMultiplier = 0.04f },
                IsTargetDebuff = true,
                SpellId = 55749,
                TalentsConflictingWithBuff = PhysVulnConfTalents.Clone(),
            });
            #endregion

            #region Spell Critical Strike Chance
            defaultBuffs.Add(new Buff
            {
                Name = "Critical Mass",
                Source = "Fire Mage",
                Group = "Spell Critical Strike Taken",
                Stats = { SpellCritOnTarget = 0.05f },
                IsTargetDebuff = true,
                SpellId = 12873,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Shadow and Flame",
                Source = "Warlock",
                Group = "Spell Critical Strike Taken",
                Stats = { SpellCritOnTarget = 0.05f },
                IsTargetDebuff = true,
                SpellId = 17801,
            });
            #endregion

            #region Spell Damage Taken
            defaultBuffs.Add(new Buff
            {
                Name = "Master Poisoner",
                Source = "Rogue",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.08f,
                    BonusFrostDamageMultiplier = 0.08f,
                    BonusArcaneDamageMultiplier = 0.08f,
                    BonusShadowDamageMultiplier = 0.08f,
                    BonusHolyDamageMultiplier = 0.08f,
                    BonusNatureDamageMultiplier = 0.08f
                },
                IsTargetDebuff = true,
                SpellId = 58410,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Curse of the Elements",
                Source = "Warlock",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.08f,
                    BonusFrostDamageMultiplier = 0.08f,
                    BonusArcaneDamageMultiplier = 0.08f,
                    BonusShadowDamageMultiplier = 0.08f,
                    BonusHolyDamageMultiplier = 0.08f,
                    BonusNatureDamageMultiplier = 0.08f
                },
                IsTargetDebuff = true,
                SpellId = 1490,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Earth and Moon",
                Source = "Moonkin Druid",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.08f,
                    BonusFrostDamageMultiplier = 0.08f,
                    BonusArcaneDamageMultiplier = 0.08f,
                    BonusShadowDamageMultiplier = 0.08f,
                    BonusHolyDamageMultiplier = 0.08f,
                    BonusNatureDamageMultiplier = 0.08f
                },
                IsTargetDebuff = true,
                SpellId = 48506,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Ebon Plaguebringer",
                Source = "Unholy Death Knight",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.08f,
                    BonusFrostDamageMultiplier = 0.08f,
                    BonusArcaneDamageMultiplier = 0.08f,
                    BonusShadowDamageMultiplier = 0.08f,
                    BonusHolyDamageMultiplier = 0.08f,
                    BonusNatureDamageMultiplier = 0.08f
                },
                IsTargetDebuff = true,
                SpellId = 51160,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Fire Breath",
                Source = "Hunter w/ Dragonhawk",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.08f,
                    BonusFrostDamageMultiplier = 0.08f,
                    BonusArcaneDamageMultiplier = 0.08f,
                    BonusShadowDamageMultiplier = 0.08f,
                    BonusHolyDamageMultiplier = 0.08f,
                    BonusNatureDamageMultiplier = 0.08f
                },
                IsTargetDebuff = true,
                SpellId = 34889,
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Lightning Breath",
                Source = "Hunter w/ Wind Serpent",
                Group = "Spell Damage Taken",
                Stats =
                {
                    BonusFireDamageMultiplier = 0.08f,
                    BonusFrostDamageMultiplier = 0.08f,
                    BonusArcaneDamageMultiplier = 0.08f,
                    BonusShadowDamageMultiplier = 0.08f,
                    BonusHolyDamageMultiplier = 0.08f,
                    BonusNatureDamageMultiplier = 0.08f
                },
                IsTargetDebuff = true,
                SpellId = 24844,
            });

            #endregion

            #region Boss Attack Speed Reduction
            defaultBuffs.Add(new Buff()
            {
                Name = "Judgements of the Just",
                Group = "Boss Attack Speed",
                Source = "Prot Paladin",
                Stats = { BossAttackSpeedReductionMultiplier = 0.20f },
                IsTargetDebuff = true,
                SpellId = 53696,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Infected Wounds",
                Group = "Boss Attack Speed",
                Source = "Feral Druid",
                Stats = { BossAttackSpeedReductionMultiplier = 0.20f },
                IsTargetDebuff = true,
                SpellId = 48484,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Frost Fever",
                Group = "Boss Attack Speed",
                Source = "Frost Death Knight",
                Stats = { BossAttackSpeedReductionMultiplier = 0.20f },
                IsTargetDebuff = true,
                SpellId = 59921,
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunder Clap",
                Group = "Boss Attack Speed",
                Source = "Protection/Arms Warrior",
                Stats = { BossAttackSpeedReductionMultiplier = 0.20f },
                IsTargetDebuff = true,
                SpellId = 6343,
            });
            #endregion

            #endregion

            #region Consumables
            #region Elixirs and Flasks
            #region Flasks
            defaultBuffs.Add(new Buff()
            {
                Name = "Lesser Flask of Resistance",
                Group = "Elixirs and Flasks",
                Stats =
                {
                    ArcaneResistance = 50,
                    FireResistance = 50,
                    FrostResistance = 50,
                    ShadowResistance = 50,
                    NatureResistance = 50
                },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Lesser Flask of Resistance (Mixology)",
                    Stats = { ArcaneResistance = 40, FireResistance = 40, FrostResistance = 40, ShadowResistance = 40, NatureResistance = 40 },
                    ConflictingBuffs = { "Flask Mixology" },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Titanic Strength",
                Group = "Elixirs and Flasks",
                Stats = { Strength = 300 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Titanic Strength (Mixology)",
                    Stats = { Strength = 80 },
                    ConflictingBuffs = { "Flask Mixology" },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of the Winds",
                Group = "Elixirs and Flasks",
                Stats = { Agility = 300 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of the Winds (Mixology)",
                    Stats = { Agility = 80 },
                    ConflictingBuffs = { "Flask Mixology" },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Steelskin",
                Group = "Elixirs and Flasks",
                Stats = { Stamina = 450 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Steelskin (Mixology)",
                    Stats = { Stamina = 120 },
                    ConflictingBuffs = { "Flask Mixology" },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of the Draconic Mind",
                Group = "Elixirs and Flasks",
                Stats = { Intellect = 300 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of the Draconic Mind (Mixology)",
                    Stats = { Intellect = 80 },
                    ConflictingBuffs = { "Flask Mixology" },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Flowing Water",
                Group = "Elixirs and Flasks",
                Stats = { Spirit = 300 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Flask of Water (Mixology)",
                    Stats = { Spirit = 80 },
                    ConflictingBuffs = { "Flask Mixology" },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Enhancement - Agility",
                Group = "Elixirs and Flasks",
                Source = "Alchemy",
                Stats = { Agility = 80 },
                Professions = new List<Profession>() { Profession.Alchemy },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Enhancement - Intellect",
                Group = "Elixirs and Flasks",
                Source = "Alchemy",
                Stats = { Intellect = 80 },
                Professions = new List<Profession>() { Profession.Alchemy },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flask of Enhancement - Strength",
                Group = "Elixirs and Flasks",
                Source = "Alchemy",
                Stats = { Strength = 80 },
                Professions = new List<Profession>() { Profession.Alchemy },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir", "Guardian Elixir" }),
            });
            #endregion

            #region Elixirs
            defaultBuffs.Add(new Buff()
            {
                Name = "Prismatic Elixir",
                Group = "Elixirs and Flasks",
                Stats =
                {
                    ArcaneResistance = 90,
                    FireResistance = 90,
                    FrostResistance = 90,
                    ShadowResistance = 90,
                    NatureResistance = 90
                },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Prismatic Elixir (Mixology)",
                    Stats = { ArcaneResistance = 105, FireResistance = 105, FrostResistance = 105, ShadowResistance = 105, NatureResistance = 105 },
                    ConflictingBuffs = { "Flask Mixology" },
                    Professions = new List<Profession>() { Profession.Alchemy } } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Major Agility",
                Group = "Elixirs and Flasks",
                Stats = { Agility = 30, CritRating = 12 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = {
                    new Buff {
                        Name = "Elixir of Major Agility (Mixology)",
                        Stats = { Agility = 10, CritRating = 4 },
                        Professions = new List<Profession>() { Profession.Alchemy },
                        ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }),
                    },
                },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Adept's Elixir",
                Group = "Elixirs and Flasks",
                Stats = { SpellPower = 24, CritRating = 24 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Adept's Elixir (Mixology)", Stats = { SpellPower = 9, CritRating = 9 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Healing Power",
                Group = "Elixirs and Flasks",
                Stats = { SpellPower = 24f, Spirit = 24 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Healing Power (Mixology)", Stats = { SpellPower = 9, Spirit = 9 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Accuracy",
                Group = "Elixirs and Flasks",
                Stats = { HitRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Accuracy (Mixology)", Stats = { HitRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Deadly Strikes",
                Group = "Elixirs and Flasks",
                Stats = { CritRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Deadly Strikes (Mixology)", Stats = { CritRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Expertise",
                Group = "Elixirs and Flasks",
                Stats = { ExpertiseRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Expertise (Mixology)", Stats = { ExpertiseRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Lightning Speed",
                Group = "Elixirs and Flasks",
                Stats = { HasteRating = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Lightning Speed (Mixology)", Stats = { HasteRating = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Agility",
                Group = "Elixirs and Flasks",
                Stats = { Agility = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Agility (Mixology)", Stats = { Agility = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Strength",
                Group = "Elixirs and Flasks",
                Stats = { Strength = 50 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Strength (Mixology)", Stats = { Strength = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Guru's Elixir",
                Group = "Elixirs and Flasks",
                Stats = { Stamina = 20, Intellect = 20, Spirit = 20, Strength = 20, Agility = 20 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Guru's Elixir (Mixology)", Stats = { 
                    Stamina = 8, Intellect = 8, Spirit = 8, Strength = 8, Agility = 8},
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Spellpower Elixir",
                Group = "Elixirs and Flasks",
                Stats = { SpellPower = 58 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Spellpower Elixir (Mixology)", Stats = { SpellPower = 19 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrath Elixir",
                Group = "Elixirs and Flasks",
                Stats = { AttackPower = 90 },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Wrath Elixir (Mixology)", Stats = { AttackPower = 32 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Ironskin",
                Group = "Elixirs and Flasks",
                Stats = { Resilience = 30 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Ironskin (Mixology)", Stats = { Resilience = 10 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Draenic Wisdom",
                Group = "Elixirs and Flasks",
                Stats = { Intellect = 30, Spirit = 30 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Draenic Wisdom (Mixology)", Stats = { Intellect = 8, Spirit = 8 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Defense",
                Group = "Elixirs and Flasks",
                Stats = { BonusArmor = 180 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Defense (Mixology)", Stats = { BonusArmor = 45 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Mageblood",
                Group = "Elixirs and Flasks",
                Stats = { Spirit = 60 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Mageblood (Mixology)", Stats = { Spirit = 24 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Protection",
                Group = "Elixirs and Flasks",
                Stats = { BonusArmor = 800 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Protection (Mixology)", Stats = { BonusArmor = 224 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Thoughts",
                Group = "Elixirs and Flasks",
                Stats = { Intellect = 45 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Thoughts (Mixology)", Stats = { Intellect = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Fortitude",
                Group = "Elixirs and Flasks",
                Stats = { Health = 350, Hp5 = 20 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Fortitude (Mixology)", Stats = { Health = 116, Hp5 = 6 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Spirit",
                Group = "Elixirs and Flasks",
                Stats = { Spirit = 50 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Spirit (Mixology)", Stats = { Spirit = 16 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Empowerment",
                Group = "Elixirs and Flasks",
                Stats = { SpellPenetration = 30 },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Empowerment (Mixology)", Stats = { SpellPenetration = 10 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            #region Cataclysm
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Deep Earth",
                Group = "Elixirs and Flasks",
                Stats = { BonusArmor = 900f },
                ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Deep Earth (Mixology)", Stats = { BonusArmor = 360 },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Guardian Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of the Master",
                Group = "Elixirs and Flasks",
                Stats = { MasteryRating = 225f },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of the Master (Mixology)", Stats = { MasteryRating = 40f },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Ghost Elixir",
                Group = "Elixirs and Flasks",
                Stats = { Spirit = 225f },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Ghost Elixir (Mixology)", Stats = { Spirit = 40f },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of the Naga",
                Group = "Elixirs and Flasks",
                Stats = { ExpertiseRating = 225f },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of the Naga (Mixology)", Stats = { ExpertiseRating = 40f },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of the Cobra",
                Group = "Elixirs and Flasks",
                Stats = { CritRating = 225f },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of the Cobra (Mixology)", Stats = { CritRating = 40f },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Mighty Speed",
                Group = "Elixirs and Flasks",
                Stats = { HasteRating = 225f },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Mighty Speed (Mixology)", Stats = { HasteRating = 40f },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elixir of Impossible Accuracy",
                Group = "Elixirs and Flasks",
                Stats = { HitRating = 225f },
                ConflictingBuffs = new List<string>(new string[] { "Battle Elixir" }),
                Improvements = { new Buff { Name = "Elixir of Impossible Accuracy (Mixology)", Stats = { HitRating = 40f },
                    Professions = new List<Profession>() { Profession.Alchemy }, ConflictingBuffs = new List<string>(new string[] { "Battle Elixir Mixology" }), } }
            });
            #endregion
            #endregion
            #endregion

            #region Potion //TODO
            // potions set to be 1 hr cooldown to ensure its treated as once per combat.
            // Jothay (old): Changed to 20 Minutes to give a higher value for the fight while
            // keeping it outside the chance of using it twice during same fight
            // Jothay+Kavan: Added a routine to SpecialEffect calcs to treat float.PositiveInfinity as "once per fight"
            #region WotLK
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Potion of Speed",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Potion of Speed (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = 500f }, 15f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { HasteRating = 500f }, 15f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Swiftness Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Swiftness Potion (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { MovementSpeed = 0.50f }, 15f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { MovementSpeed = 0.50f }, 15f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Potion of Wild Magic",
                Group = "Potion",
                Stats = new Stats(),
                SpellId = 53909,
                Improvements = { new Buff { Name = "Potion of Wild Magic (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { CritRating = 200f, SpellPower = 200f }, 15f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { CritRating = 200f, SpellPower = 200f }, 15f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Heroic Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Heroic Potion (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 70f, Health = 700f }, 15f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 70f, Health = 700f }, 15f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Insane Strength Potion",
                Group = "Potion",
                Stats = new Stats(),
                SpellId = 28494,
                Improvements = { new Buff { Name = "Insane Strength Potion (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 120f, DodgeRating = -75f }, 15f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 120f, DodgeRating = -75f }, 15f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Indestructible Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Indestructible Potion (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = 3500f }, 15f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = 3500f }, 15f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Mighty Rage Potion",
                Group = "Potion",
                Stats = new Stats() { BonusRageGen = (45f + 75) / 2f, },
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, CharacterClass.Druid, },
                Improvements = {
                    new Buff {
                        Name = "Mighty Rage Potion (Double Pot Trick)",
                        Stats = new Stats() { BonusRageGen = (45f + 75) / 2f, },
                        ConflictingBuffs = { "Double Pot Tricks" },
                        AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, CharacterClass.Druid, },
                    }
                }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 60f, }, 20f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 60f, }, 20f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Mana Potion",
                Group = "Potion",
                Stats = new Stats() { ManaRestore = (4200f + 4400f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Mana Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { ManaRestore = ((4200f + 4400f) / 2f) * 0.40f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Mana Injector",
                Group = "Potion",
                Stats = new Stats() { ManaRestore = (4200f + 4400f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Mana Injector (Engineer Bonus)",
                            Professions = new List<Profession>() { Profession.Engineering },
                            Stats = new Stats() { ManaRestore = ((4200f + 4400f) / 2f) * 0.25f, }
                        }
                    }
            });
             defaultBuffs.Add(new Buff()
             {
                Name = "Endless Mana Potion",
                Group = "Potion",
                Source = "Alchemy",
                Professions = new List<Profession>() { Profession.Alchemy },
                Stats = new Stats() { ManaRestore = (1800f + 3000f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Endless Mana Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { ManaRestore = ((1800f + 3000f) / 2f) * 0.40f, }
                        }
                    }
            });
             defaultBuffs.Add(new Buff()
             {
                Name = "Endless Healing Potion",
                Group = "Potion",
                Source = "Alchemy",
                Professions = new List<Profession>() { Profession.Alchemy },
                Stats = new Stats() { HealthRestore = (1500f + 2500f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Endless Healing Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { HealthRestore = ((1500f + 2500f) / 2f) * 0.40f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Healing Potion",
                Group = "Potion",
                Stats = new Stats() { HealthRestore = (2700f + 4500f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Healing Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() { HealthRestore = ((2700f + 4500f) / 2f) * 0.40f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Runic Healing Injector",
                Group = "Potion",
                Stats = new Stats() { HealthRestore = (2700f + 4500f) / 2f, },
                Improvements = {
                        new Buff {
                            Name = "Runic Healing Injector (Engineer Bonus)",
                            Professions = new List<Profession>() { Profession.Engineering },
                            Stats = new Stats() { HealthRestore = ((2700f + 4500f) / 2f) * 0.25f, }
                        }
                    }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Powerful Rejuvenation Potion",
                Group = "Potion",
                Stats = new Stats() {
                    ManaRestore   = (2475f + 4125f) / 2f,
                    HealthRestore = (2475f + 4125f) / 2f,
                },
                Improvements = {
                        new Buff {
                            Name = "Powerful Rejuvenation Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() {
                                ManaRestore   = ((2475f + 4125f) / 2f) * 0.40f,
                                HealthRestore = ((2475f + 4125f) / 2f) * 0.40f,
                            }
                        }
                    }
            });
            /* This potion has several effects that could individually randomly proc on use:
             * 
             * Wild Magic - Spell critical rating increased by 60 and spell power increased by 180. 15 seconds
             * Nightmare Slumber - like Dreamless Sleep potion (health and mana back over 6 seconds, ~5k both)
             * Healing Potion - like a healing potion
             * Nothing - Does nothing
             * Haste - Increased haste rating by 500 for 15 seconds
             * Nightmare slumber:
             *   Puts an undispellable debuff on you that regens
             *   2520 mana and health every second for 3 seconds (with the alchemist stone on)
             *   This thing always seems to happen when you are lower on health and mana then
             *   when on full health and mana.
             */
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Crazy Alchemist's Potion (proc not included)",
                Group = "Potion",
                Source = "Alchemy",
                Professions = new List<Profession>() { Profession.Alchemy },
                Stats = new Stats() {
                    ManaRestore   = (4200f + 4400f) / 2f,
                    HealthRestore = (3100f + 3500f) / 2f,
                },
                Improvements = {
                        new Buff {
                            Name = "Crazy Alchemist's Potion (Alch Stone Bonus)",
                            Professions = new List<Profession>() { Profession.Alchemy },
                            Stats = new Stats() {
                                ManaRestore   = ((4200f + 4400f) / 2f) * 0.40f,
                                HealthRestore = ((3100f + 3500f) / 2f) * 0.40f,
                            }
                        }
                    }
            });
            #endregion
            #region Cataclysm
            // Not sure the best way to support this pot...
            // http://www.wowhead.com/item=58142
            // Use: Fills your veins with deadly poison, causing you to sometimes
            // poison enemies that strike you in melee for 1900 to 2100 damage over 8 sec. (1 Min Cooldown)
            // Comments: Lasts 15 seconds, shares cd with other pots
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Deathblood Venom",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Deathblood Venom (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { NatureDamage = 2000f }, 15f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { NatureDamage = 2000f }, 15f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Earthen Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Earthen Potion (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = 4800f }, 25f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { BonusArmor = 4800f }, 25f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Golemblood Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Golemblood Potion (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 1200f }, 25f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Strength = 1200f }, 25f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Potion of the Tol'vir",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Potion of the Tol'vir (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Agility = 1200f }, 25f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { Agility = 1200f }, 25f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Volcanic Potion",
                Group = "Potion",
                Stats = new Stats(),
                Improvements = { new Buff { Name = "Volcanic Potion (Double Pot Trick)", Stats = new Stats(), ConflictingBuffs = { "Double Pot Tricks" }, } }
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellPower = 1200f }, 25f, float.PositiveInfinity));
            buff.Improvements[0].Stats.AddSpecialEffect(new SpecialEffect(Trigger.Use, new Stats() { SpellPower = 1200f }, 25f - 1f, float.PositiveInfinity));
            defaultBuffs.Add(new Buff()
            {
                Name = "Mighty Rejuvenation Potion",
                Group = "Potion",
                Stats = new Stats() { HealthRestore = 8000, ManaRestore = 8000 },
                Improvements = {
                    new Buff {
                        Name = "Mighty Rejuvenation Potion (Alch Stone Bonus)",
                        Professions = new List<Profession>() { Profession.Alchemy },
                        Stats = new Stats() {
                            ManaRestore   = 8000 * 0.40f,
                            HealthRestore = 8000 * 0.40f,
                        }
                    }
                }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Mysterious Potion",
                Group = "Potion",
                Stats = new Stats() { HealthRestore = 7500, ManaRestore = 7500 },
                Improvements = {
                    new Buff {
                        Name = "Mysterious Potion (Alch Stone Bonus)",
                        Professions = new List<Profession>() { Profession.Alchemy },
                        Stats = new Stats() {
                            ManaRestore   = 7500 * 0.40f,
                            HealthRestore = 7500 * 0.40f,
                        }
                    }
                }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Mythical Healing Potion",
                Group = "Potion",
                Stats = new Stats() { HealthRestore = 10000, },
                Improvements = {
                    new Buff {
                        Name = "Mythical HealingPotion (Alch Stone Bonus)",
                        Professions = new List<Profession>() { Profession.Alchemy },
                        Stats = new Stats() {
                            HealthRestore = 10000 * 0.40f,
                        }
                    }
                }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Mythical Mana Potion",
                Group = "Potion",
                Stats = new Stats() { ManaRestore = 10000 },
                Improvements = {
                    new Buff {
                        Name = "Mythical Mana Potion (Alch Stone Bonus)",
                        Professions = new List<Profession>() { Profession.Alchemy },
                        Stats = new Stats() {
                            HealthRestore = 10000 * 0.40f,
                        }
                    }
                }
            });
            #endregion
            #endregion

            #region Food
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Strength Food", Stats = { Strength = 90, Stamina = 90 } }); // Bear-Basted Crocolisk
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Agility Food", Stats = { Agility = 90, Stamina = 90 } }); // Skewered Eel
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Crit Food", Stats = { CritRating = 90, Stamina = 90 } }); // Baked Rockfish
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Mastery Food", Stats = { MasteryRating = 90, Stamina = 90 } }); // Lavascale Minestrone
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Hit Food", Stats = { HitRating = 90, Stamina = 90 } }); // Grilled Dragon
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Expertise Food", Stats = { ExpertiseRating = 90, Stamina = 90 } }); // Crocolisk Au Gratin
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Haste Food",  Stats = { HasteRating = 90, Stamina = 90 } }); // Basilisk Riverdog
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Spirit Food", Stats = { Stamina = 90, Spirit = 90 } }); // Delicious Sagefish Tail
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Intellect Food", Stats = { Intellect = 90, Stamina = 90 } }); // Severed Sagefish Head
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Dodge Food", Stats = { DodgeRating = 90, Stamina = 90 } }); // Mushroom Sauce Mudfish
            defaultBuffs.Add(new Buff() { Group = "Food", Name = "Parry Food", Stats = { ParryRating = 90, Stamina = 90 } }); // Blackbelly Sushi
            // The new feasts are 90 Stam + 90 [Other Useful Stat]. No reason to keep this like that and just make them do the other appropriate food instead
            //defaultBuffs.Add(new Buff() { Group = "Food", Name = "Fish Feast", Stats = { AttackPower = 80, SpellPower = 46, Stamina = 40 } });
            #endregion

            #region Pet Food
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Spiced Mammoth Treats",
                Group = "Pet Food",
                Stats = { PetStrength = 30, PetStamina = 30 },
                ConflictingBuffs = new List<string>(new string[] { "Pet Food" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Cataclysm Pet Food (Str)",
                Group = "Pet Food",
                Stats = { PetStrength = 75 },
                ConflictingBuffs = new List<string>(new string[] { "Pet Food" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Cataclysm Pet Food (Sta)",
                Group = "Pet Food",
                Stats = { PetStamina = 110 },
                ConflictingBuffs = new List<string>(new string[] { "Pet Food" }),
            });*/
            #endregion

            #region Scrolls
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Agility IX",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { /*"Scroll of Agility", "Agility",*/ "Battle Elixir", "Mixology" }),
                Stats = { Agility = 100 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Strength IX",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { /*"Scroll of Strength", "Strength",*/ "Battle Elixir", "Mixology" }),
                Stats = { Strength = 100 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Intellect IX",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { /*"Scroll of Intellect", "Intellect",*/ "Battle Elixir", "Mixology" }),
                Stats = { Intellect = 100 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Stamina IX",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { /*"Scroll of Stamina", "Stamina",*/ "Guardian Elixir", "Mixology" }),
                Stats = { Stamina = 150 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Spirit IX",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { /*"Scroll of Spirit", "Spirit",*/ "Battle Elixir", "Mixology" }),
                Stats = { Spirit = 100 }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scroll of Protection IX",
                Group = "Scrolls",
                ConflictingBuffs = new List<string>(new string[] { /*"Scroll of Protection", "StatArmor",*/ "Guardian Elixir", "Mixology" }),
                Stats = { BonusArmor = 400 }
            });
            #endregion
            #endregion

            #region Set Bonuses
            #region Death Knight
            #region WotLK
            #region Tier 10 | Scourgelord's
            #region Battlegear
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusDamageMultiplierObliterate = .1f
                },
                SetName = "Scourgelord's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });*/
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    BonusPhysicalDamageMultiplier = .03f,
                    BonusSpellPowerMultiplier = .03f
                },
                SetName = "Scourgelord's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });*/
            #endregion
            #region Plate
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Plate (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    TankDK_T10_2pc = .20f,
                },
                SetName = "Scourgelord's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgelord's Plate (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats =
                {
                    TankDK_T10_4pc = .12f,
                },
                SetName = "Scourgelord's Plate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });*/
            #endregion
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Magma Plated
            #region Battlearmor (Blood)
            defaultBuffs.Add(new Buff()
            {
                Name = "Magma Plated Battlearmor (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Magma Plated Battlearmor",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Magma Plated Battlearmor (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Magma Plated Battlearmor",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #region Battlegear (DPS)
            defaultBuffs.Add(new Buff()
            {
                Name = "Magma Plated Battlegear (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Magma Plated Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Magma Plated Battlegear (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Magma Plated Battlegear",
                Stats = new Stats(),
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion
            #region Tier 12 | Elementium Deathplate
            #region Battlearmor (Blood)
            defaultBuffs.Add(new Buff()
            {
                Name = "Elementium Deathplate Battlearmor (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Elementium Deathplate Battlearmor",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Elementium Deathplate Battlearmor (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Elementium Deathplate Battlearmor",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #region Battlegear (DPS)
            defaultBuffs.Add(new Buff()
            {
                Name = "Elementium Deathplate Battlegear (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Elementium Deathplate Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Elementium Deathplate Battlegear (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Elementium Deathplate Battlegear",
                Stats = new Stats(),
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion
            #region Tier 13 | Necrotic Boneplate
            #region Armor (Blood)
            defaultBuffs.Add(new Buff()
            {
                Name = "Necrotic Boneplate Armor (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Necrotic Boneplate Armor",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Necrotic Boneplate Armor (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Necrotic Boneplate Armor",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #region Battlegear (DPS)
            defaultBuffs.Add(new Buff()
            {
                Name = "Necrotic Boneplate Battlegear (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Necrotic Boneplate Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Necrotic Boneplate Battlegear (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Necrotic Boneplate Battlegear",
                Stats = new Stats(),
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Desecration (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Strength = 70, },
                SetName = "Gladiator's Desecration",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Desecration (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Strength = 90, },
                SetName = "Gladiator's Desecration",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.DeathKnight, },
            });
            #endregion
            #endregion

            #region Druid
            #region TBC
            /*
            #region Tier 4 | Malorne
            #region Harness
            defaultBuffs.Add(new Buff()
            {
                Name = "Malorne Harness 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BloodlustProc = 0.8f },
                SetName = "Malorne Harness",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malorne Harness 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusArmor = 1400, CatFormStrength = 30 },
                SetName = "Malorne Harness",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            #endregion
            #region Raiment
            defaultBuffs.Add( buff = new Buff()
            {
                Name = "Malorne Raiment 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ManaRestoreOnCast_5_15 = 120 },
                SetName = "Malorne Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.HealingSpellCast, new Stats() { ManaRestore = 120f }, 0f, 0f, 0.05f));
            #endregion
            #region Regalia
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Malorne Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Malorne Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast, new Stats() { Mana = 120f }, 0f, 0f, 0.1f, 1));
            defaultBuffs.Add(new Buff()
            {
                Name = "Malorne Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { InnervateCooldownReduction = 48.0f },
                SetName = "Malorne Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier 5 | Nordrassil
            #region Harness
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Harness 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusShredDamage = 75 }, //, BonusLacerateDamage = 15/5},
                SetName = "Nordrassil Harness",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            #endregion
            #region Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Raiment 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RegrowthExtraTicks = 2 },
                SetName = "Nordrassil Raiment",
                SetThreshold = 2
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Raiment 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { LifebloomFinalHealBonus = 150 },
                SetName = "Nordrassil Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Nordrassil Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { StarfireBonusWithDot = 0.1f },
                SetName = "Nordrassil Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier 6 | Thunderheart
            #region Harness
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Harness 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MangleCatCostReduction = 5, BonusMangleBearThreat = 0.15f },
                SetName = "Thunderheart Harness",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Harness 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRipDamageMultiplier = .15f, BonusFerociousBiteDamageMultiplier = .15f, BonusSwipeDamageMultiplier = .15f },
                SetName = "Thunderheart Harness",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid },
            });
            #endregion
            #region Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Raiment 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusHealingTouchMultiplier = 0.05f },
                SetName = "Thunderheart Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MoonfireExtension = 3.0f },
                SetName = "Thunderheart Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thunderheart Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { StarfireCritChance = 0.05f },
                SetName = "Thunderheart Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
 */
            #endregion
            #region WotLK
            #region Tier  7 | Dreamwalker
            /*            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRipDuration = 4f, BonusLacerateDamageMultiplier = 0.05f },
                SetName = "Dreamwalker Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { TigersFuryCooldownReduction = 3f, WeaponDamage = 1.778f /*Increased Barkskin Duration },
                SetName = "Dreamwalker Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Garb (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusInsectSwarmDamage = 0.1f },
                SetName = "Dreamwalker Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Garb (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusNukeCritChance = 0.05f },
                SetName = "Dreamwalker Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Regalia (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { LifebloomCostReduction = 0.05f },
                SetName = "Dreamwalker Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreamwalker Regalia (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { NourishBonusPerHoT = 0.05f },
                SetName = "Dreamwalker Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion */
            #endregion
            #region Tier  8 | Nightsong
            /*            #region Battlegear
            defaultBuffs.Add(new Buff() //TODO TODO TODO TODO
            {
                Name = "Nightsong Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ClearcastOnBleedChance = 0.02f },
                SetName = "Nightsong Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff() //TODO TODO TODO TODO
            {
                Name = "Nightsong Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusSavageRoarDuration = 8f },
                SetName = "Nightsong Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Nightsong Garb (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { EclipseBonus = 0.07f },
                SetName = "Nightsong Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Nightsong Garb (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Nightsong Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.InsectSwarmTick, new Stats() { StarfireProc = 1f }, 0f, 0f, 0.08f, 1));
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Nightsong Regalia (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SwiftmendBonus = 0.1f },
                SetName = "Nightsong Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });

            defaultBuffs.Add(new Buff()
            {
                Name = "Nightsong Regalia (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RejuvenationInstantTick = 0.5f },
                SetName = "Nightsong Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion */
            #endregion
            #region Tier  9 | Malfurion's
            /*            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Battlegear (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRakeDuration = 3f, BonusLacerateDamageMultiplier = 0.05f },
                SetName = "Malfurion's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Battlegear (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRipCrit = 0.05f, BonusFerociousBiteCrit = 0.05f },
                SetName = "Malfurion's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Regalia (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MoonfireDotCrit = 1f },
                SetName = "Malfurion's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Regalia (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusMoonkinNukeDamage = 0.04f },
                SetName = "Malfurion's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Garb (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { NourishCritBonus = 0.05f },
                SetName = "Malfurion's Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Malfurion's Garb (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RejuvenationCrit = 1.0f },
                SetName = "Malfurion's Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion */
            #endregion
            #region Tier 10 | Lasherweave
            #region Battlegear
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lasherweave Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RipCostReduction = 10f, BonusLacerateDamageMultiplier = 0.20f, BonusBearSwipeDamageMultiplier = 0.20f },
                SetName = "Lasherweave Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });*/
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Lasherweave Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusRakeDamageMultiplier = 0.10f, DamageTakenMultiplier = -0.12f },
                SetName = "Lasherweave Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });*/
            #endregion
            #region Regalia
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lasherweave Regalia (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Lasherweave Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DamageSpellCast, new Stats() { BonusArcaneDamageMultiplier = 0.15f, BonusNatureDamageMultiplier = 0.15f }, 6.0f, 0f, 0.06f, 1));
            defaultBuffs.Add(new Buff()
            {
                Name = "Lasherweave Regalia (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Lasherweave Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });*/
            #endregion
            #region Garb
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Lasherweave Garb (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { WildGrowthLessReduction = 0.3f },
                SetName = "Lasherweave Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Lasherweave Garb (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RejuvJumpChance = 0.02f },
                SetName = "Lasherweave Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });*/
            #endregion
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Stormrider's
            #region Regalia (Balance)
            defaultBuffs.Add(new Buff()
            {
                Name = "Stormrider's Regalia (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Stormrider's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Stormrider's Regalia (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Stormrider's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Battlegarb (Feral)
            defaultBuffs.Add(new Buff()
            {
                Name = "Stormrider's Battlegarb (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { /*BonusDamageMultiplierRakeTick = 0.1f, BonusDamageMultiplierLacerate = 0.1f*/ },
                SetName = "Stormrider's Battlegarb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Stormrider's Battlegarb (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { /*BonusAttackPowerMultiplier = 0.03f*/ }, // TODO: This set bonus has been updated to not just a flat 3%
                SetName = "Stormrider's Battlegarb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            //buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.MangleCatHit, new Stats() { BonusAttackPowerMultiplier = 0.01f, }, 30, 0, 1f, 3));
            #endregion
            #region Vestments (Restoration)
            defaultBuffs.Add(new Buff()
            {
                Name = "Stormrider's Vestments (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Stormrider's Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Stormrider's Vestments (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Stormrider's Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier 12 | Obsidian Arborweave
            #region Regalia (Balance)
            defaultBuffs.Add(new Buff()
            {
                Name = "Obsidian Arborweave Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Obsidian Arborweave Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Obsidian Arborweave Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Obsidian Arborweave Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Battlegarb (Feral)
            defaultBuffs.Add(new Buff()
            {
                Name = "Obsidian Arborweave Battlegarb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Obsidian Arborweave Battlegarb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Obsidian Arborweave Battlegarb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Obsidian Arborweave Battlegarb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Vestments (Restoration)
            defaultBuffs.Add(new Buff()
            {
                Name = "Obsidian Arborweave Vestments 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Obsidian Arborweave Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Obsidian Arborweave Vestments 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Obsidian Arborweave Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #region Tier 13 | Deep Earth
            #region Regalia (Balance)
            defaultBuffs.Add(new Buff()
            {
                Name = "Deep Earth Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Deep Earth Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Deep Earth Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Deep Earth Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Battlegarb (Feral)
            defaultBuffs.Add(new Buff()
            {
                Name = "Deep Earth Battlegarb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Deep Earth Battlegarb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Deep Earth Battlegarb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Deep Earth Battlegarb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Vestments (Restoration)
            defaultBuffs.Add(new Buff()
            {
                Name = "Deep Earth Vestments 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Deep Earth Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Deep Earth Vestments 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Deep Earth Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            #region Gladiator's Wildhide (Balance)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Wildhide (PvP) 2 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Intellect = 70 },
                SetName = "Gladiator's Wildhide",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Wildhide (PvP) 4 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90, },
                SetName = "Gladiator's Wildhide",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Gladiator's Sanctuary (Feral)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator Sanctuary (PvP) 2 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Agility = 70 },
                SetName = "Gladiator's Sanctuary",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator Sanctuary (PvP) 4 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Agility = 90, },
                SetName = "Gladiator's Sanctuary",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #region Gladiator's Refuge (Restoration)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Refuge (PvP) 2 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Intellect = 70 },
                SetName = "Gladiator's Refuge",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Refuge (PvP) 4 Piece Set Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90, },
                SetName = "Gladiator's Refuge",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Druid, },
            });
            #endregion
            #endregion
            #endregion

            #region Hunter
            #region WotLK
            #region Tier 7 | Cryptstalker
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "Cryptstalker Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cryptstalker Battlegear",
                Stats = { BonusPetDamageMultiplier = 0.05f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Cryptstalker Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cryptstalker Battlegear",
                Stats = { BonusHunter_T7_4P_ViperSpeed = 0.2f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            }); */
            #endregion
            #region Tier 8 | Scourgestalker
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "Scourgestalker Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Scourgestalker Battlegear",
                Stats = { BonusHunter_T8_2P_SerpDmg = 0.1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Scourgestalker Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Scourgestalker Battlegear",
                Stats = new Stats(), //{ BonusHunter_T8_4P_SteadyShotAPProc = 1 },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.SteadyShotHit,
                new Stats() { AttackPower = 600f, },
                15f, 45f, 0.10f)); */
            #endregion
            #region Tier 9 | Windrunner's
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "Windrunner's Battlegear (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Windrunner's Battlegear",
                Stats = { BonusHunter_T9_2P_SerpCanCrit = 1 },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Windrunner's Battlegear (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Windrunner's Battlegear",
                Stats = new Stats() { },// { BonusHunter_T9_4P_SteadyShotPetAPProc = 1 },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.RangedHit,
                new Stats() { PetAttackPower = 600f, },
                15f, 45f, 0.35f)); */
            #endregion
            #region Tier 10 | Ahn'Kahar Blood Hunter's
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Ahn'Kahar Blood Hunter's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Ahn'Kahar Blood Hunter's Battlegear",
                Stats = new Stats() { },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.HunterAutoShotHit,
                new Stats() { BonusDamageMultiplier = 0.15f },
                10f, 0f, 0.05f
            ));
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Ahn'Kahar Blood Hunter's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Ahn'Kahar Blood Hunter's Battlegear",
                Stats = new Stats() { },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.SerpentWyvernStingsDoDamage,
                new Stats() { BonusAttackPowerMultiplier = 0.20f },
                10f, 0f, 0.05f
            ));*/
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Lightning-Charged Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Lightning-Charged Battlegear (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Lightning-Charged Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Lightning-Charged Battlegear (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Lightning-Charged Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            #endregion
            #region Tier 12 | Flamewaker's Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Flamewaker's Battlegear (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Flamewaker's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Flamewaker's Battlegear (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Flamewaker's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            #endregion
            #region Tier 13 | Wrymstalker's Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrymstalker's Battlegear (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Wrymstalker's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrymstalker's Battlegear (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Wrymstalker's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Pursuit 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Agility = 70, },
                SetName = "Gladiator's Pursuit",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Pursuit 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Agility = 90 },
                SetName = "Gladiator's Pursuit",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Hunter, },
            });
            #endregion
            #endregion

            #region Mage
            #region TBC
            /*
            #region Tier 4 | Aldor
            defaultBuffs.Add(new Buff()
            {
                Name = "Aldor Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { AldorRegaliaInterruptProtection = 1 },
                SetName = "Aldor Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #region Tier 5 | Tirisfal
            defaultBuffs.Add(new Buff()
            {
                Name = "Tirisfal Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ArcaneBlastBonus = .05f },
                SetName = "Tirisfal Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Tirisfal Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats(),
                SetName = "Tirisfal Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.SpellCrit, new Stats() { SpellPower = 70f }, 6.0f, 0.0f));
            #endregion
            #region Tier 6 | Tempest
            defaultBuffs.Add(new Buff()
            {
                Name = "Tempest Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { EvocationExtension = 2f },
                SetName = "Tempest Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Tempest Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusMageNukeMultiplier = 0.05f },
                SetName = "Tempest Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Frostfire
            /*            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frostfire Garb 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusManaGem = 0.25f },
                SetName = "Frostfire Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.ManaGem, new Stats() { SpellPower = 225f }, 15f, 0f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Frostfire Garb 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { CritBonusDamage = 0.05f },
                SetName = "Frostfire Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            }); */
            #endregion
            #region Tier 8 | Kirin Tor
            /*            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Kirin Tor Garb 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Kirin Tor Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.MageNukeCast, new Stats() { SpellPower = 350f }, 15f, 45f, 0.25f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Kirin Tor Garb 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage4T8 = 1 },
                SetName = "Kirin Tor Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            }); */
            #endregion
            #region Tier 9 | Khadgar's Regalia
            /*            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Khadgar's Regalia 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage2T9 = 1 },
                SetName = "Khadgar's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Khadgar's Regalia 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage4T9 = 1 },
                SetName = "Khadgar's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            }); */
            #endregion
            #region Tier 10 | Bloodmage's Regalia
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Bloodmage's Regalia 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage2T10 = 1 },
                SetName = "Bloodmage's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Bloodmage's Regalia 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Mage4T10 = 1 },
                SetName = "Bloodmage's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });*/
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Firelord's Vestments
            defaultBuffs.Add(new Buff()
            {
                Name = "Firelord's Vestments (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Firelord's Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Firelord's Vestments (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Firelord's Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #region Tier 12 | Firehawk Robes of Conflagration
            defaultBuffs.Add(new Buff()
            {
                Name = "Firehawk Robes of Conflagration (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Firehawk Robes of Conflagration",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Firehawk Robes of Conflagration (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Firehawk Robes of Conflagration",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #region Tier 13 | Timelord's Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Time Lord's Regalia (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Time Lord's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Time Lord's Regalia (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Time Lord's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Gladiator's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Gladiator's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Mage, },
            });
            #endregion
            #endregion

            #region Paladin
            #region Cataclysm
            #region Tier 11 | Reinforced Sapphirium
            #region Regalia (Holy)
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Reinforced Sapphirium Regalia (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Reinforced Sapphirium Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Reinforced Sapphirium Regalia (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Reinforced Sapphirium Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            #endregion
            #region Battlearmor (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Reinforced Sapphirium Battlearmor (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Reinforced Sapphirium Battlearmor",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Reinforced Sapphirium Battlearmor (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Reinforced Sapphirium Battlearmor",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #region Battleplate (Retribution)
            defaultBuffs.Add(new Buff()
            {
                Name = "Reinforced Sapphirium Battleplate (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Reinforced Sapphirium Battleplate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Reinforced Sapphirium Battleplate (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Reinforced Sapphirium Battleplate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion
            #region Tier 12 | of Immolation
            #region Regalia (Holy)
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Regalia of Immolation (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of Immolation",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Regalia of Immolation (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of Immolation",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            #endregion
            #region Battlearmor (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Battlearmor of Immolation (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Battlearmor of Immolation",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Battlearmor of Immolation (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Battlearmor of Immolation",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #region Battleplate (Retribution)
            defaultBuffs.Add(new Buff()
            {
                Name = "Battleplate of Immolation (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Battleplate of Immolation",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Battleplate of Immolation (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Battleplate of Immolation",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion
            #region Tier 13 | of Radiant Glory
            #region Regalia (Holy)
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Regalia of Radiant Glory (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of Radiant Glory",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Regalia of Radiant Glory (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of Radiant Glory",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });

            #endregion
            #region Armor (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Armor of Radiant Glory (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Armor of Radiant Glory",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Armor of Radiant Glory (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Armor of Radiant Glory",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #region Battleplate (Retribution)
            defaultBuffs.Add(new Buff()
            {
                Name = "Battleplate of Radiant Glory (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Battleplate of Radiant Glory",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Battleplate of Radiant Glory (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Battleplate of Radiant Glory",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Redemption (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Intellect = 70, },
                SetName = "Gladiator's Redemption",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Redemption (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90, },
                SetName = "Gladiator's Redemption",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Vindication (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Gladiator's Vindication",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Vindication (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Gladiator's Vindication",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Paladin, },
            });
            #endregion
            #endregion

            #region Priest
            #region TBC
            /*
            #region Tier 4 |
            #endregion
            #region Tier 5 | Avatar Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Avatar Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ManaGainOnGreaterHealOverheal = 100f },
                SetName = "Avatar Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Avatar Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RenewDurationIncrease = 3f },
                SetName = "Avatar Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Tier 6 | Absolution
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of Absolution 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusPoHManaCostReductionMultiplier = 0.1f },
                SetName = "Vestments of Absolution",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of Absolution 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusGHHealingMultiplier = 0.05f },
                SetName = "Vestments of Absolution",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Absolution Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { SWPDurationIncrease = 3f },
                SetName = "Absolution Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Absolution Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusMindBlastMultiplier = 0.1f },
                SetName = "Absolution Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Garb of Faith
            /*            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of Faith 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PrayerOfMendingExtraJumps = 1 },
                SetName = "Regalia of Faith",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of Faith 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { GreaterHealCostReduction = 0.05f },
                SetName = "Regalia of Faith",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Garb of Faith 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MindBlastCostReduction = 0.1f },
                SetName = "Garb of Faith",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Garb of Faith 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ShadowWordDeathCritIncrease = 0.1f },
                SetName = "Garb of Faith",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion */
            #endregion
            #region Tier 8 | Sanctification
            /*            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PrayerOfHealingExtraCrit = 0.1f },
                SetName = "Sanctification Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PWSBonusSpellPowerProc = 250 },
                SetName = "Sanctification Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DevouringPlagueBonusDamage = 0.15f },
                SetName = "Sanctification Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Sanctification Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { MindBlastHasteProc = 240 },
                SetName = "Sanctification Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion */
            #endregion
            #region Tier 9 | Velen's
            /*            #region Raiment
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T9_2pc = 0.2f },
                SetName = "Velen's Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T9_4pc = 0.1f },
                SetName = "Velen's Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T9_2pc = 6 },
                SetName = "Velen's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Velen's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T9_4pc = 0.05f },
                SetName = "Velen's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion */
            #endregion
            #region Tier 10 | Crimson Acolyte's
            #region Raiment
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T10_2pc = 0.33f },
                SetName = "Crimson Acolyte's Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestHeal_T10_4pc = 0.05f },
                SetName = "Crimson Acolyte's Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });*/
            #endregion
            #region Regalia
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T10_2pc = 0.05f },
                SetName = "Crimson Acolyte's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Crimson Acolyte's Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { PriestDPS_T10_4pc = 0.51f },
                SetName = "Crimson Acolyte's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });*/
            #endregion
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Mercurial
            #region Vestments (Heals)
            defaultBuffs.Add(new Buff()
            {
                Name = "Mercurial Vestments (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Mercurial Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Mercurial Vestments (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Mercurial Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Regalia (Shadow)
            defaultBuffs.Add(new Buff()
            {
                Name = "Mercurial Regalia (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Mercurial Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Mercurial Regalia (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Mercurial Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion
            #region Tier 12 | of the Cleansing Flame
            #region Vestments (Heals)
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Cleansing Flame (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of the Cleansing Flame",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Cleansing Flame (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of the Cleansing Flame",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Regalia (Shadow)
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of the Cleansing Flame (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of the Cleansing Flame",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of the Cleansing Flame (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of the Cleansing Flame",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion
            #region Tier 13 | of Dying Light
            #region Vestments (Heals)
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of Dying Light (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of Dying Light",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of Dying Light (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of Dying Light",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Regalia (Shadow)
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of Dying Light (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of Dying Light",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of Dying Light (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of Dying Light",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            #region Gladiator's Investiture (Heals)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Investiture 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400f, Intellect = 70 },
                SetName = "Gladiator's Investiture",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Investiture 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90 },
                SetName = "Gladiator's Investiture",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #region Gladiator's Raiment (Shadow)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400f, Intellect = 70 },
                SetName = "Gladiator's Raiment",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90 },
                SetName = "Gladiator's Raiment",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Priest, },
            });
            #endregion
            #endregion
            #endregion

            #region Rogue
            #region TBC
            /*
            #region Tier 4 | Netherblade
            defaultBuffs.Add(new Buff()
            {
                Name = "Netherblade 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusSnDDuration = 3f },
                SetName = "Netherblade",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Netherblade 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { CPOnFinisher = .15f },
                SetName = "Netherblade",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 5 | Deathmantle
            defaultBuffs.Add(new Buff()
            {
                Name = "Deathmantle 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusEvisEnvenomDamage = 40f },
                SetName = "Deathmantle",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Deathmantle 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusFreeFinisher = 1f },
                SetName = "Deathmantle",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 6 | Slayer
            defaultBuffs.Add(new Buff()
            {
                Name = "Slayer's Armor 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusSnDHaste = .05f },
                SetName = "Slayer's Armor",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Slayer's Armor 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusCPGDamage = .06f },
                SetName = "Slayer's Armor",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Bonescythe
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "Bonescythe Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RuptureDamageBonus = 0.1f },
                SetName = "Bonescythe Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Bonescythe Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ComboMoveEnergyReduction = .05f },
                SetName = "Bonescythe Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            }); */
            #endregion
            #region Tier 8 | Terrorblade
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "Terrorblade Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusEnergyFromDP = 1f },
                SetName = "Terrorblade Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Terrorblade Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { RuptureCrit = 1f },
                SetName = "Terrorblade Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            }); */
            #endregion
            #region Tier 9 | VanCleef/Garona
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "VanCleef's Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ReduceEnergyCostFromRupture = 40f },
                SetName = "VanCleef's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "VanCleef's Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusCPGCritChance = .05f },
                SetName = "VanCleef's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Garona's Battlegear 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ReduceEnergyCostFromRupture = 40f },
                SetName = "Garona's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Garona's Battlegear 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusCPGCritChance = .05f },
                SetName = "Garona's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            }); */
            #endregion
            #region Tier 10 | Shadowblade's
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Shadowblade's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusToTTEnergy = 30 },
                SetName = "Shadowblade's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });*/
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Shadowblade's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { ChanceOn3CPOnFinisher = 0.13f },
                SetName = "Shadowblade's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });*/
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Wind Dancer's Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Wind Dancer's Regalia (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { Rogue_T11_2P = 1f },
                SetName = "Wind Dancer's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wind Dancer's Regalia (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { Rogue_T11_4P = 1f },
                SetName = "Wind Dancer's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 12 | Vestments of the Dark Phoenix
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Dark Phoenix (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Vestments of the Dark Phoenix",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Dark Phoenix (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Vestments of the Dark Phoenix",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #region Tier 13 | Blackfang Battleweave
            defaultBuffs.Add(new Buff()
            {
                Name = "Blackfang Battleweave (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Blackfang Battleweave",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Blackfang Battleweave (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { },
                SetName = "Blackfang Battleweave",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Vestments (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Agility = 70, },
                SetName = "Gladiator's Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Vestments (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Agility = 90, },
                SetName = "Gladiator's Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Rogue, },
            });
            #endregion
            #endregion

            #region Shaman
            #region TBC
            /*
            #region Tier 4 | Cyclone
            defaultBuffs.Add(new Buff()
            {
                Name = "Cyclone Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cyclone Raiment",
                Stats = { ManaSpringMp5Increase = 7.5f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Tier 5 | Cataclysm
            defaultBuffs.Add(new Buff()
            {
                Name = "Cataclysm Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Cataclysm Raiment",
                Stats = { LHWManaReduction = .05f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Tier 6 | Skyshatter
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Regalia 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Regalia",
                Stats = { Mp5 = 15f, CritRating = 35f, SpellPower = 45f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Regalia 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Regalia",
                Stats = { LightningBoltDamageModifier = 5f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Raiment 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Raiment",
                Stats = { CHManaReduction = .1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Skyshatter Raiment 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Skyshatter Raiment",
                Stats = { CHHealIncrease = .05f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
*/
            #endregion
            #region WotLK
            #region Tier 7 | Earthshatter
            /*            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Regalia (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Regalia",
                Stats = { WaterShieldIncrease = .1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Regalia (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Regalia",
                Stats = { CHHWHealIncrease = .05f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Battlegear",
                Stats = { Enhance2T7 = 0.1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Battlegear",
                Stats = { Enhance4T7 = 0.05f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Garb (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Garb",
                Stats = { LightningBoltCostReduction = 5f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthshatter Garb (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Earthshatter Garb",
                Stats = { BonusLavaBurstCritDamage = 10f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion */
            #endregion
            #region Tier 8 | Worldbreaker
            /*            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Regalia (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Regalia",
                Stats = { RTCDDecrease = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Regalia (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Regalia",
                Stats = { CHCTDecrease = .2f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Battlegear",
                Stats = { Enhance2T8 = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Battlegear",
                Stats = { Enhance4T8 = 1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Garb (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Garb",
                Stats = { BonusFlameShockDoTDamage = .2f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Worldbreaker Garb (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Worldbreaker Garb",
                Stats = { LightningBoltCritDamageModifier = 0.08f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion */
            #endregion
            #region Tier 9 | Nobundo's
            /*            #region Garb
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Garb (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Garb",
                Stats = { RestoSham2T9 = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Garb (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Garb",
                Stats = { RestoSham4T9 = 1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Regalia (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Regalia",
                Stats = { BonusFlameShockDuration = 9f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Thrall's/Nobundo's Regalia (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Nobundo's Regalia",
                Stats = { BonusLavaBurstDamageMultiplier = 0.1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Nobundo's Battlegear (T9) 2 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance2T9 = 1 },
                SetName = "Nobundo's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Nobundo's Battlegear (T9) 4 Piece",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance4T9 = 1 },
                SetName = "Nobundo's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion */
            #endregion
            #region Tier 10 | Frost Witch's
            #region Battlegear
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = {  },
                SetName = "Frost Witch's Battlegear",
                SetThreshold = 2, // obsoleted
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = {  }, // obsoleted
                SetName = "Frost Witch's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });*/
            #endregion
            #region Garb
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Frost Witch's Garb (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Frost Witch's Garb",
                Stats = { RestoSham2T10 = 1f },
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Frost Witch's Garb (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Frost Witch's Garb",
                Stats = { RestoSham4T10 = 1f },
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });*/
            #endregion
            #region Regalia
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Regalia (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Elemental_T10_2P = 1 },
                SetName = "Frost Witch's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Frost Witch's Regalia (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Elemental_T10_4P = 1 },
                SetName = "Frost Witch's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });*/
            #endregion
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | of the Raging Elements
            #region Battlegear (Enhancement)
            defaultBuffs.Add(new Buff()
            {
                Name = "Battlegear of the Raging Elements (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance_T11_2P = 1, },
                SetName = "Battlegear of the Raging Elements",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Battlegear of the Raging Elements (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Enhance_T11_4P = 1, },
                SetName = "Battlegear of the Raging Elements",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Vestments (Resto)
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Raging Elements (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of the Raging Elements",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Raging Elements (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of the Raging Elements",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Regalia (Elemental)
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of the Raging Elements (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of the Raging Elements",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Regalia of the Raging Elements (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Regalia of the Raging Elements",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #region Tier 12 | Volcanic
            #region Battlegear (Enhancement)
            defaultBuffs.Add(new Buff()
            {
                Name = "Volcanic Battlegear (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Volcanic Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Volcanic Battlegear (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Volcanic Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Vestments (Resto)
            defaultBuffs.Add(new Buff()
            {
                Name = "Volcanic Vestments (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Volcanic Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Volcanic Vestments (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Volcanic Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Regalia (Elemental)
            defaultBuffs.Add(new Buff()
            {
                Name = "Volcanic Regalia (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Volcanic Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Volcanic Regalia (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Volcanic Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #region Tier 13 | Spiritwalker�s
            #region Battlegear (Enhancement)
            defaultBuffs.Add(new Buff()
            {
                Name = "Spiritwalker's Battlegear (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Spiritwalker's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Spiritwalker's Battlegear (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { },
                SetName = "Spiritwalker's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Vestments (Resto)
            defaultBuffs.Add(new Buff()
            {
                Name = "Spiritwalker's Vestments (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Spiritwalker's Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Spiritwalker's Vestments (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Spiritwalker's Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Regalia (Elemental)
            defaultBuffs.Add(new Buff()
            {
                Name = "Spiritwalker's Regalia (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Spiritwalker's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Spiritwalker's Regalia (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Spiritwalker's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            #region Earthshaker (Enhancement)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Earthshaker (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Agility = 70, },
                SetName = "Gladiator's Earthshaker",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Earthshaker (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Agility = 90, },
                SetName = "Gladiator's Earthshaker",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Wartide (Resto)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Wartide (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Intellect = 70, },
                SetName = "Gladiator's Wartide",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Wartide (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90, },
                SetName = "Gladiator's Wartide",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #region Thunderfist (Elemental)
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Thunderfist (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Intellect = 70, },
                SetName = "Gladiator's Thunderfist",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Thunderfist (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90, },
                SetName = "Gladiator's Thunderfist",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Shaman, },
            });
            #endregion
            #endregion
            #endregion

            #region Warlock
            #region TBC
            /*
            #region Tier 4 |
            #endregion
            #region Tier 5 |
            #endregion
            #region Tier 6 |
            #endregion
*/
            #endregion
            #region WotLK
            #region Tier 7 | Plagueheart
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "Plagueheart Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock2T7 = 0.10f },
                SetName = "Plagueheart Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Plagueheart Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock4T7 = 300f },
                SetName = "Plagueheart Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            }); */
            #endregion
            #region Tier 8 | Deathbringer
            /*            defaultBuffs.Add(new Buff()
            {
                Name = "Deathbringer Garb 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock2T8 = 0.2f },
                SetName = "Deathbringer Garb",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Deathbringer Garb 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock4T8 = 0.05f },
                SetName = "Deathbringer Garb",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            }); */
            #endregion
            #region Tier 9 | Kel'Thuzad's Regalia
            //alliance [<3] set
            /*            defaultBuffs.Add(new Buff()
                        {
                            Name = "Kel'Thuzad's Regalia 2 Piece Bonus",
                            Group = "Set Bonuses",
                            ConflictingBuffs = new List<string>(new string[] { }),
                            Stats = { Warlock2T9 = 0.10f },
                            SetName = "Kel'Thuzad's Regalia",
                            SetThreshold = 2,
                            AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
                        });
                        defaultBuffs.Add(new Buff()
                        {
                            Name = "Kel'Thuzad's Regalia 4 Piece Bonus",
                            Group = "Set Bonuses",
                            ConflictingBuffs = new List<string>(new string[] { }),
                            Stats = { Warlock4T9 = 0.10f },
                            SetName = "Kel'Thuzad's Regalia",
                            SetThreshold = 4,
                            AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
                        }); */
            #endregion
            #region Tier 10 | Dark Coven's Regalia
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Dark Coven's Regalia (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = {
                    BonusCritChanceShadowbolt = 0.05f,
                    BonusCritChanceIncinerate = 0.05f,
                    BonusCritChanceSoulfire   = 0.05f,
                    BonusCritChanceCorruption = 0.05f,
                },
                SetName = "Dark Coven's Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dark Coven's Regalia (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock_T10_4P = 0.15f },
                SetName = "Dark Coven's Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });*/
            #endregion
            #region PvP
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Shadowflame Regalia
            defaultBuffs.Add(new Buff()
            {
                Name = "Shadowflame Regalia (T11) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock_T11_2P = 0.10f },
                SetName = "Shadowflame Regalia",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Shadowflame Regalia (T11) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Warlock_T11_4P = 0.02f },
                SetName = "Shadowflame Regalia",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #region Tier 12 | Balespider's Burning Vestments
            defaultBuffs.Add(new Buff()
            {
                Name = "Balespider's Burning Vestments (T12) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Balespider's Burning Vestments",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Balespider's Burning Vestments (T12) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Balespider's Burning Vestments",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #region Tier 13 | Vestments of the Faceless Shroud
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Faceless Shroud (T13) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of the Faceless Shroud",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Vestments of the Faceless Shroud (T13) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Vestments of the Faceless Shroud",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Felshroud (PvP) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Intellect = 70, },
                SetName = "Gladiator's Felshroud",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Felshroud (PvP) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Intellect = 90, },
                SetName = "Gladiator's Felshroud",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warlock, },
            });
            #endregion
            #endregion

            #region Warrior
            #region WotLK
            #region Tier  7 | Dreadnaught
            /*#region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreadnaught Plate (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusShieldSlamDamage = 0.1f },
                SetName = "Dreadnaught Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreadnaught Battlegear (T7) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T7_2P_SlamDamage = 0.1f },
                SetName = "Dreadnaught Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Dreadnaught Battlegear (T7) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T7_4P_RageProc = 5f },
                SetName = "Dreadnaught Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion */
            #endregion
            #region Tier  8 | Siegebreaker
            /*#region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Siegebreaker Plate (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { DevastateCritIncrease = 0.1f },
                SetName = "Siegebreaker Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Siegebreaker Battlegear (T8) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = new Stats() { /*BonusWarrior_T8_2P_HasteProc = 1,* },
                SetName = "Siegebreaker Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.HSorSLHit, new Stats() { HasteRating = 150f, }, 5f, 0f, 0.40f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Siegebreaker Battlegear (T8) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T8_4P_MSBTCritIncrease = 0.1f },
                SetName = "Siegebreaker Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion */
            #endregion
            #region Tier  9 | Wrynn's
            /* #region Plate
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Plate (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusDevastateDamage = 0.05f },
                SetName = "Wrynn's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battlegear
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Battlegear (T9) 2 Piece Fury Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T9_2P_Crit = 0.02f },
                SetName = "Wrynn's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Battlegear (T9) 2 Piece Arms Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T9_2P_ArP = 0.06f },
                SetName = "Wrynn's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Battlegear (T9) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusWarrior_T9_4P_SLHSCritIncrease = 0.05f },
                SetName = "Wrynn's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Wrynn's Plate (T9) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusDevastateDamage = 0.05f },
                SetName = "Wrynn's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion */
            #endregion
            #region Tier 10 | Ymirjar Lord's
            #region Plate
            /*defaultBuffs.Add(new Buff()
            {
                Name = "Ymirjar Lord's Plate 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                //Stats = { BonusShieldSlamDamage = 0.2f, BonusShockwaveDamage = 0.2f },
                SpellId = 70843,
                SetName = "Ymirjar Lord's Plate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });*/
            #endregion
            #region Battlegear
            /*defaultBuffs.Add(buff = new Buff()
            {
                Name = "Ymirjar Lord's Battlegear (T10) 2 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusPhysicalDamageMultiplier = 0.05f, },
                SetName = "Ymirjar Lord's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.DeepWoundsTick,
                new Stats() { BonusAttackPowerMultiplier = 0.16f, },
                10f, 0f, 0.03f));
            defaultBuffs.Add(new Buff()
            {
                Name = "Ymirjar Lord's Battlegear (T10) 4 Piece Bonus",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { BonusDamageMultiplier = 0.05f, },
                SetName = "Ymirjar Lord's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });*/
            #endregion
            #endregion
            #endregion
            #region Cataclysm
            #region Tier 11 | Earthen
            #region Warplate (DPS)
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthen Warplate (T11) 2P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                //Stats = new Stats() { BonusWarrior_T11_2P_BTMSDmgMult = 0.05f, },
                SpellId = 90293,
                SetName = "Earthen Warplate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                // Each time you use Overpower or Raging Blow you gain a 1% increase to attack power for 30 sec stacking up to 3 times.
                Name = "Earthen Warplate (T11) 4P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                //Stats = new Stats(), // This uses a proc
                SpellId = 90295,
                SetName = "Earthen Warplate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            //buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.OPorRBAttack, new Stats() { BonusAttackPowerMultiplier = 0.01f, }, 30, 0, 1f, 3));
            #endregion
            #region Battleplate (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthen Battleplate (T11) 2P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                //Stats = new Stats() { BonusShieldSlamDamage = 0.05f, },
                SpellId = 90296,
                SetName = "Earthen Battleplate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Earthen Battleplate (T11) 4P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                //Stats = new Stats() { BonusWarrior_T11_4P_ShieldWallDurMult = 0.05f, },
                SetName = "Earthen Battleplate",
                SpellId = 90297,
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #region Tier 12 | Molten Giant
            #region Warplate (DPS)
            defaultBuffs.Add(new Buff()
            {
                Name = "Molten Giant Warplate (T12) 2P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Molten Giant Warplate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Molten Giant Warplate (T12) 4P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Molten Giant Warplate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Battleplate (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Molten Giant Battleplate (T12) 2P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Molten Giant Battleplate",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Molten Giant Battleplate (T12) 4P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Molten Giant Battleplate",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #region Tier 13 | Colossal Dragonplate
            #region Battlegear (DPS)
            defaultBuffs.Add(new Buff()
            {
                Name = "Colossal Dragonplate Battlegear (T13) 2P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Colossal Dragonplate Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(buff = new Buff()
            {
                Name = "Colossal Dragonplate Battlegear (T13) 4P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Colossal Dragonplate Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #region Armor (Protection)
            defaultBuffs.Add(new Buff()
            {
                Name = "Colossal Dragonplate Armor (T13) 2P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Colossal Dragonplate Armor",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Colossal Dragonplate Armor (T13) 4P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                SetName = "Colossal Dragonplate Armor",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion
            #endregion
            #region PvP
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Battlegear (PvP) 2P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Resilience = 400, Strength = 70, },
                SetName = "Gladiator's Battlegear",
                SetThreshold = 2,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Gladiator's Battlegear (PvP) 4P",
                Group = "Set Bonuses",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { /*BonusWarrior_PvP_4P_InterceptCDReduc = 5,*/ Strength = 90, },
                SpellId = 22738,
                SetName = "Gladiator's Battlegear",
                SetThreshold = 4,
                AllowedClasses = new List<CharacterClass>() { CharacterClass.Warrior, },
            });
            #endregion
            #endregion

            #region Non-Class Set Bonuses
            #region T11 Weapon set Bonus
            {
                // Weapon set consists of Claws of Agony and Claws of Torment
                defaultBuffs.Add(buff = new Buff()
                {
                    Name = "Agony and Torment 2P",
                    Group = "Set Bonuses",
                    ConflictingBuffs = new List<string>(new string[] { }),
                    Stats = { },
                    SetName = "Agony and Torment",
                    SetThreshold = 2,
                    AllowedClasses = new List<CharacterClass> { CharacterClass.Druid, CharacterClass.Hunter, CharacterClass.Rogue, CharacterClass.Shaman, CharacterClass.Warrior }
                });
                // Your melee and ranged attacks have a chance to grant 1000 haste rating for 10 sec.
                buff.Stats.AddSpecialEffect(new SpecialEffect(Trigger.PhysicalAttack, new Stats() { HasteRating = 1000f }, 10f, 45f, 0.1f));
            }
            #endregion
            #endregion
            #endregion // Set Bonuses

            #region Profession Buffs
            defaultBuffs.Add(new Buff() {
                Name = "Toughness",
                Group = "Profession Buffs",
                Source = "Mining [525]",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { Stamina = 120f },
                Professions = new List<Profession>() { Profession.Mining },
            });
            defaultBuffs.Add(new Buff() {
                Name = "Master of Anatomy",
                Group = "Profession Buffs",
                Source = "Skinning [525]",
                ConflictingBuffs = new List<string>(new string[] { }),
                Stats = { CritRating = 80f },
                Professions = new List<Profession>() { Profession.Skinning },
            });
            #endregion

            #region Temporary Buffs
            defaultBuffs.Add(new Buff()
            {
                Name = "Dual Wielding Mob",
                Group = "Temporary Buffs",
                ConflictingBuffs = new List<string>(new string[] { "Dual Wielding Mob" }),
                Stats = { Miss = 0.2f }
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Luck of the Draw 5%",
                Group = "Temporary Buffs",
                Source = "Dungeon Finder Groups",
                Stats =
                {
                    BonusHealthMultiplier = .05f,
                    BonusDamageMultiplier = .05f,
                    BonusHealingDoneMultiplier = .05f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn", "Luck of the Draw" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Luck of the Draw 10%",
                Group = "Temporary Buffs",
                Source = "Dungeon Finder Groups",
                Stats =
                {
                    BonusHealthMultiplier = .10f,
                    BonusDamageMultiplier = .10f,
                    BonusHealingDoneMultiplier = .10f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn", "Luck of the Draw" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Luck of the Draw 15%",
                Group = "Temporary Buffs",
                Source = "Dungeon Finder Groups",
                Stats =
                {
                    BonusHealthMultiplier = .15f,
                    BonusDamageMultiplier = .15f,
                    BonusHealingDoneMultiplier = .15f,
                },
                ConflictingBuffs = new List<string>(new string[] { "Strength of Wrynn", "Luck of the Draw" }),
            });
            defaultBuffs.Add(new Buff()
            {
                Name = "Improved Lay On Hands",
                Group = "Temporary Buffs",
                Stats = { PhysicalDamageTakenReductionMultiplier = 0.20f },
                ConflictingBuffs = new List<string>(new string[] { })
            });
            defaultBuffs.Add(new Buff
            {
                Name = "Nozdormu's Presence",
                Source = "Nozdormu - Madness of Deathwing",
                Group = "Temporary Buffs",
                Stats = { SpellHaste = 0.2f, PhysicalHaste = 0.2f },
                ConflictingBuffs = new List<string>(new string[] { })
            });
            #endregion

            return defaultBuffs;
        }
        #endregion
    }
}
