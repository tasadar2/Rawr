﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Rawr.Enhance
{
    public class CharacterCalculationsEnhance : CharacterCalculationsBase
    {
        #region Getter/Setter
        private float _overallPoints = 0f;
        public override float OverallPoints
        {
            get { return _overallPoints; }
            set { _overallPoints = value; }
        }

        private float[] _subPoints = new float[] { 0f, 0f };
        public override float[] SubPoints
        {
            get { return _subPoints; }
            set { _subPoints = value; }
        }

        public float DPS
        {
            get { return _subPoints[0]; }
            set { _subPoints[0] = value; }
        }

        public float Survivability
        {
            get { return _subPoints[1]; }
            set { _subPoints[1] = value; }
        }

        private StatsEnhance _basicStats;
        public StatsEnhance BasicStats
        {
            get { return _basicStats; }
            set { _basicStats = value; }
        }

        private StatsEnhance _enhsimStats;
        public StatsEnhance EnhSimStats
        {
            get { return _enhsimStats; }
            set { _enhsimStats = value; }
        }

        private Stats _buffStats;
        public Stats BuffStats
        {
            get { return _buffStats; }
            set { _buffStats = value; }
        }

        private float _attackPower;
        public float AttackPower
        {
            get { return _attackPower; }
            set { _attackPower = value; }
        }

        private float _spellPower;
        public float SpellPower
        {
            get { return _spellPower; }
            set { _spellPower = value; }
        }

        private float _totalExpertiseMH;
        public float TotalExpertiseMH
        {
            get { return _totalExpertiseMH; }
            set { _totalExpertiseMH = value; }
        }

        private float _totalExpertiseOH;
        public float TotalExpertiseOH
        {
            get { return _totalExpertiseOH; }
            set { _totalExpertiseOH = value; }
        }

        private float _avoidedAttacks;
        public float AvoidedAttacks
        {
            get { return _avoidedAttacks; }
            set { _avoidedAttacks = value; }
        }

        private float _dodgedAttacks;
        public float DodgedAttacks
        {
            get { return _dodgedAttacks; }
            set { _dodgedAttacks = value; }
        }

        private float _parriedAttacks;
        public float ParriedAttacks
        {
            get { return _parriedAttacks; }
            set { _parriedAttacks = value; }
        }

        private float _missedAttacks;
        public float MissedAttacks
        {
            get { return _missedAttacks; }
            set { _missedAttacks = value; }
        }

        private float _whiteCrit;
        public float MeleeCrit
        {
            get { return _whiteCrit; }
            set { _whiteCrit = value; }
        }

        private float _yellowCrit;
        public float YellowCrit
        {
            get { return _yellowCrit; }
            set { _yellowCrit = value; }
        }

        private float _spellCrit;
        public float SpellCrit
        {
            get { return _spellCrit; }
            set { _spellCrit = value; }
        }

        private float _whiteHit;
        public float WhiteHit
        {
            get { return _whiteHit; }
            set { _whiteHit = value; }
        }

        private float _yellowHit;
        public float YellowHit
        {
            get { return _yellowHit; }
            set { _yellowHit = value; }
        }

        private float _spellHit;
        public float SpellHit
        {
            get { return _spellHit; }
            set { _spellHit = value; }
        }

        private float _masteryRating;
        public float MasteryRating
        {
            get { return _masteryRating; }
            set { _masteryRating = value; }
        }

        private float _overSpellHitCap;
        public float OverSpellHitCap
        {
            get { return _overSpellHitCap; }
            set { _overSpellHitCap = value; }
        }

        private float _overMeleeCritCap;
        public float OverMeleeCritCap
        {
            get { return _overMeleeCritCap; }
            set { _overMeleeCritCap = value; }
        }

        private float _armorMitigation;
        public float ArmorMitigation
        {
            get { return _armorMitigation; }
            set { _armorMitigation = value; }
        }

        private float _edUptime;
        public float EDUptime
        {
            get { return _edUptime; }
            set { _edUptime = value; }
        }

        private float _edBonusCrit;
        public float EDBonusCrit
        {
            get { return _edBonusCrit; }
            set { _edBonusCrit = value; }
        }

        private float _flurryUptime;
        public float FlurryUptime
        {
            get { return _flurryUptime; }
            set { _flurryUptime = value; }
        }

        private float _secondsTo5Stack;
        private float _MWPPM;
        public float SecondsTo5Stack
        {
            get { return _secondsTo5Stack; }
            set { _secondsTo5Stack = value; _MWPPM = value == 0 ? 0 : 5f * 60f / value; }
        }

        private float _avMHSpeed;
        public float AvMHSpeed
        {
            get { return _avMHSpeed; }
            set { _avMHSpeed = value; }
        }

        private float _avOHSpeed;
        public float AvOHSpeed
        {
            get { return _avOHSpeed; }
            set { _avOHSpeed = value; }
        }

        private float _meleeDamage;
        public float MeleeDamage
        {
            get { return _meleeDamage; }
            set { _meleeDamage = value; }
        }

        private float _glancingBlows;
        public float GlancingBlows
        {
            get { return _glancingBlows; }
            set { _glancingBlows = value; }
        }

        private DPSAnalysis _swingDamage;
        public DPSAnalysis SwingDamage
        {
            get { return _swingDamage; }
            set { _swingDamage = value; }
        }

        private DPSAnalysis _windfuryAttack;
        public DPSAnalysis WindfuryAttack
        {
            get { return _windfuryAttack; }
            set { _windfuryAttack = value; }
        }

        private DPSAnalysis _flametongueAttack;
        public DPSAnalysis FlameTongueAttack
        {
            get { return _flametongueAttack; }
            set { _flametongueAttack = value; }
        }

        private DPSAnalysis _lightningBolt;
        public DPSAnalysis LightningBolt
        {
            get { return _lightningBolt; }
            set { _lightningBolt = value; }
        }

        private DPSAnalysis _chainLightning;
        public DPSAnalysis ChainLightning
        {
            get { return _chainLightning; }
            set { _chainLightning = value; }
        }

        private DPSAnalysis _earthShock;
        public DPSAnalysis EarthShock
        {
            get { return _earthShock; }
            set { _earthShock = value; }
        }

        private DPSAnalysis _flameShock;
        public DPSAnalysis FlameShock
        {
            get { return _flameShock; }
            set { _flameShock = value; }
        }

        private DPSAnalysis _searingMagma;
        public DPSAnalysis SearingMagma
        {
            get { return _searingMagma; }
            set { _searingMagma = value; }
        }

        private DPSAnalysis _fireNova;
        public DPSAnalysis FireNova
        {
            get { return _fireNova; }
            set { _fireNova = value; }
        }

        private FireElemental _fireElemental;
        public FireElemental FireElemental
        {
            get { return _fireElemental; }
            set { _fireElemental = value; }
        }

        private DPSAnalysis _stormstrike;
        public DPSAnalysis Stormstrike
        {
            get { return _stormstrike; }
            set { _stormstrike = value; }
        }

        private DPSAnalysis _spiritWolf;
        public DPSAnalysis SpiritWolf
        {
            get { return _spiritWolf; }
            set { _spiritWolf = value; }
        }

        private DPSAnalysis _lightningShield;
        public DPSAnalysis LightningShield
        {
            get { return _lightningShield; }
            set { _lightningShield = value; }
        }

        private DPSAnalysis _lavaLash;
        public DPSAnalysis LavaLash
        {
            get { return _lavaLash; }
            set { _lavaLash = value; }
        }

        private DPSAnalysis _unleashWind;
        public DPSAnalysis UnleashWind
        {
            get { return _unleashWind; }
            set { _unleashWind = value; }
        }

        private DPSAnalysis _unleashFlame;
        public DPSAnalysis UnleashFlame
        {
            get { return _unleashFlame; }
            set { _unleashFlame = value; }
        }

        private DPSAnalysis _magicOther;
        public DPSAnalysis MagicOther
        {
            get { return _magicOther; }
            set { _magicOther = value; }
        }

        private DPSAnalysis _physicalOther;
        public DPSAnalysis PhysicalOther
        {
            get { return _physicalOther; }
            set { _physicalOther = value; }
        }

        private string _version;
        public string Version
        {
            get { return _version; }
            set { _version = value; }
        }

        private float _mhEnchantUptime;
        public float MHEnchantUptime
        {
            get { return _mhEnchantUptime; }
            set { _mhEnchantUptime = value; }
        }

        private float _ohEnchantUptime;
        public float OHEnchantUptime
        {
            get { return _ohEnchantUptime; }
            set { _ohEnchantUptime = value; }
        }

        private float _trinket1Uptime;
        public float Trinket1Uptime
        {
            get { return _trinket1Uptime; }
            set { _trinket1Uptime = value; }
        }

        private float _trinket2Uptime;
        public float Trinket2Uptime
        {
            get { return _trinket2Uptime; }
            set { _trinket2Uptime = value; }
        }

        private float _fireTotemUptime;
        public float FireTotemUptime
        {
            get { return _fireTotemUptime; }
            set { _fireTotemUptime = value; }
        }

        private float _baseRegen;
        public float BaseRegen
        {
            get { return _baseRegen; }
            set { _baseRegen = value; }
        }

        private float _manaRegen;
        public float ManaRegen
        {
            get { return _manaRegen; }
            set { _manaRegen = value; }
        }

        private float _elemPrecMod;
        public float ElemPrecMod
        {
            get { return _elemPrecMod; }
            set { _elemPrecMod = value; }
        }

        private float _draeneiHitBonus;
        public float DraeneiHitBonus
        {
            get { return _draeneiHitBonus; }
            set { _draeneiHitBonus = value; }
        }

        private float _specializationHitBonus;
        public float SpecializationHitBonus
        {
            get { return _specializationHitBonus; }
            set { _specializationHitBonus = value; }
        }

        private float _t10_2Uptime;
        public float T10_2Uptime
        {
            get { return _t10_2Uptime; }
            set { _t10_2Uptime = value; }
        }

        private float _t10_4Uptime;
        public float T10_4Uptime
        {
            get { return _t10_4Uptime; }
            set { _t10_4Uptime = value; }
        }

        public List<Buff> ActiveBuffs { get; set; }
        #endregion

        public override Dictionary<string, string> GetCharacterDisplayCalculationValues()
        {
            Dictionary<string, string> dictValues = new Dictionary<string, string>();
            string displayFormat = "";
            int formIter = 1;
            #region Summary
            dictValues.Add("DPS Points", DPS.ToString("F2", CultureInfo.InvariantCulture));
            dictValues.Add("Survivability Points", Survivability.ToString("F2", CultureInfo.InvariantCulture));
            dictValues.Add("Overall Points", OverallPoints.ToString("F2", CultureInfo.InvariantCulture));
            #endregion
            #region Basic Stats
            #region Base Stats
            dictValues.Add("Health", BasicStats.Health.ToString("F0", CultureInfo.InvariantCulture));
            dictValues.Add("Mana", BasicStats.Mana.ToString("F0", CultureInfo.InvariantCulture));

            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nIncreases Attack Power by {1:0.#}";
            dictValues.Add("Strength", string.Format(displayFormat,
                BasicStats.Strength, BasicStats.Strength - 10f));

            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nIncreases Attack Power by {1:0.#}";
            displayFormat += "\r\nIncreases Critical Hit chance by {2:0.00%}";
            dictValues.Add("Agility", string.Format(displayFormat,
                BasicStats.Agility, ((BasicStats.Agility * 2f) - 20f), StatConversion.GetPhysicalCritFromAgility(BasicStats.Agility, CharacterClass.Shaman)));

            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nIncreases Health by {1:0.#}";
            dictValues.Add("Stamina", string.Format(displayFormat,
                BasicStats.Stamina, StatConversion.GetHealthFromStamina(BasicStats.Stamina)));

            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nIncreases Mana by {1:0.#}";
            displayFormat += "\r\nIncreases Spell Critical Hit chance by {2:0.00%}";
            dictValues.Add("Intellect", string.Format(displayFormat,
                BasicStats.Intellect, StatConversion.GetManaFromIntellect(BasicStats.Intellect), StatConversion.GetSpellCritFromIntellect(BasicStats.Intellect)));

            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nIncreases Mana regeneration by {1:0.#} every 5 seconds while not in combat";
            dictValues.Add("Spirit", string.Format(displayFormat,
                    BasicStats.Spirit, StatConversion.GetSpiritRegenSec(BasicStats.Spirit, BasicStats.Intellect) * 5f));

            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nMastery rating of {1:0.#} adds {2:0.0#} Mastery";
            displayFormat += "\r\nIncreases all Fire, Frost, and Nature Damage by {3:0.00%}.";
            dictValues.Add("Mastery", string.Format(displayFormat,
                    BasicStats.Mastery + StatConversion.GetMasteryFromRating(BasicStats.MasteryRating),
                    BasicStats.MasteryRating, StatConversion.GetMasteryFromRating(BasicStats.MasteryRating),
                    (BasicStats.Mastery + StatConversion.GetMasteryFromRating(BasicStats.MasteryRating)) * 0.025f));
            #endregion
            #region Melee
            //dictValues.Add("Damage"
            //dictValues.Add("DPS"

            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nIncreases damage with melee weapons by {1:0.0} dps";
            dictValues.Add("Attack Power", string.Format(displayFormat,
                BasicStats.AttackPower, BasicStats.AttackPower / 14f));

            //dictValues.Add("Speed"

            //displayFormat = "";
            //displayFormat += "{0:0.00%}*";
            //displayFormat += "Stats Pane shows Averaged Values*";
            //displayFormat += "\r\nHaste Rating of {1:0.#} adds {2:0.00%} Haste";
            dictValues.Add("Melee Haste", String.Format("{0}%*Haste Rating of {1} adds {2}% Haste",
                (StatConversion.GetHasteFromRating(BasicStats.HasteRating, CharacterClass.Shaman) * 100f).ToString("F2", CultureInfo.InvariantCulture),
                BasicStats.HasteRating.ToString("F0", CultureInfo.InvariantCulture),
                (StatConversion.GetPhysicalHasteFromRating(BasicStats.HasteRating, CharacterClass.Shaman) * 100f).ToString("F2", CultureInfo.InvariantCulture)));

            displayFormat = "";
            displayFormat += "{0:0.00%}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nHit Rating of {1:0.#} adds {2:0.00%} Hit chance";
            displayFormat += "\r\n= Breakdown =";
            displayFormat += "\r\n{3:0.00%} Draenei Hit Bonus";
            displayFormat += "\r\n{4:0.00%} Specialization Bonus";
            dictValues.Add("Melee Hit", string.Format(displayFormat,
                StatConversion.GetPhysicalHitFromRating(BasicStats.HitRating) + BasicStats.PhysicalHit,
                BasicStats.HitRating, StatConversion.GetPhysicalHitFromRating(BasicStats.HitRating),
                DraeneiHitBonus, SpecializationHitBonus));


            dictValues.Add("Melee Crit", String.Format("{0}%*Crit Rating of {1} adds {2}% Crit chance",
                ((StatConversion.GetCritFromRating(BasicStats.CritRating, CharacterClass.Shaman) * 100f) + (StatConversion.GetCritFromAgility(BasicStats.Agility, CharacterClass.Shaman) * 100f)).ToString("F2", CultureInfo.InvariantCulture),
                BasicStats.CritRating.ToString("F0", CultureInfo.InvariantCulture),
                (StatConversion.GetCritFromRating(BasicStats.CritRating, CharacterClass.Shaman) * 100f).ToString("F2", CultureInfo.InvariantCulture)));


            dictValues.Add("Expertise",getExpertiseString());// String.Format("{0} / {1}*Reduces chance to be dodged or parried by {2}% / {3}%\r\nExpertise Rating of {4} adds {5} Expertise",
            #endregion
            #region Spell
            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            dictValues.Add("Spell Power", string.Format(displayFormat,
                BasicStats.SpellPower));

            //displayFormat = "";
            //displayFormat += "{0:0.00%}*";
            //displayFormat += "Stats Pane shows Averaged Values*";
            //displayFormat += "\r\nHaste Rating of {1:0.#} adds {2:0.00%} Haste";
            dictValues.Add("Spell Haste", String.Format("{0}%*Haste Rating of {1} adds {2}% Haste",
                (StatConversion.GetSpellHasteFromRating(BasicStats.HasteRating, CharacterClass.Shaman) * 100f).ToString("F2", CultureInfo.InvariantCulture),
                BasicStats.HasteRating.ToString("F0", CultureInfo.InvariantCulture),
                (StatConversion.GetSpellHasteFromRating(BasicStats.HasteRating, CharacterClass.Shaman) * 100f).ToString("F2", CultureInfo.InvariantCulture)));

            displayFormat = "";
            displayFormat += "{0:0.00%}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\nHit Rating of {1:0.#} adds {2:0.00%} Hit chance";
            displayFormat += "\r\n= Breakdown =";
            displayFormat += "\r\n{3:0.00%} Draenei Hit Bonus";
            dictValues.Add("Spell Hit", string.Format(displayFormat,
                StatConversion.GetSpellHitFromRating(BasicStats.HitRating + ElemPrecMod) + DraeneiHitBonus,
                BasicStats.HitRating + ElemPrecMod, StatConversion.GetSpellHitFromRating(BasicStats.HitRating + ElemPrecMod),
                DraeneiHitBonus));


            dictValues.Add("Spell Crit", String.Format("{0}%*Crit Rating of {1} adds {2}% Crit chance",
                ((StatConversion.GetSpellCritFromRating(BasicStats.CritRating, CharacterClass.Shaman) * 100f) + (StatConversion.GetSpellCritFromIntellect(BasicStats.Intellect, CharacterClass.Shaman) * 100f)).ToString("F2", CultureInfo.InvariantCulture),
                BasicStats.CritRating.ToString("F0", CultureInfo.InvariantCulture),
                (StatConversion.GetSpellCritFromRating(BasicStats.CritRating, CharacterClass.Shaman) * 100f).ToString("F2", CultureInfo.InvariantCulture)));

            displayFormat = "";
            displayFormat += "{0:0.#}*";
            displayFormat += "Stats Pane shows Averaged Values*";
            displayFormat += "\r\n{0:0.#} mana regenerated every 5 seconds while in combat";
            dictValues.Add("Combat Regen", string.Format(displayFormat,
                BaseRegen));
            #endregion
            #endregion
            #region Complex Stats
            //dictValues.Add("Avg Agility", _attackPower.ToString("F0", CultureInfo.InvariantCulture));
            //dictValues.Add("Avg Intellect"
            //dictValues.Add("Avg Mastery"
            //dictValues.Add("Avg Attack Power"
            dictValues.Add("Avg Speed", String.Format("{0} / {1}", AvMHSpeed.ToString("F2", CultureInfo.InvariantCulture), AvOHSpeed.ToString("F2", CultureInfo.InvariantCulture)));
            //dictValues.Add("Avg Melee Haste"
            //dictValues.Add("Avg Melee Hit"
            //dictValues.Add("Avg Melee Crit"
            //dictValues.Add("Avg Expertise"
            //dictValues.Add("Avg Spell Power"
            //dictValues.Add("Avg Spell Haste"
            //dictValues.Add("Avg Spell Hit"
            //dictValues.Add("Avg Spell Crit"
            dictValues.Add("Avg Combat Regen", ManaRegen.ToString("F0", CultureInfo.InvariantCulture));

            /*dictValues.Add("White Hit", WhiteHit.ToString("F2", CultureInfo.InvariantCulture) + "%");
            if (YellowHit < 100f && TotalExpertiseMH < 26)
            {
                float ratingRequired = (float)Math.Ceiling(4f * StatConversion.GetRatingFromExpertise(100f - YellowHit));
                dictValues.Add("Yellow Hit", String.Format("{0}% (Under Cap)*You need {1} more expertise to cap specials (WF,SS)",
                    YellowHit.ToString("F2", CultureInfo.InvariantCulture),
                    ratingRequired.ToString("F0", CultureInfo.InvariantCulture)));
            }
            else
            {
                if (ParriedAttacks > 0)
                {
                    float ratingRequired = (float)Math.Ceiling(4f * StatConversion.GetRatingFromExpertise(100f - YellowHit));
                    dictValues.Add("Yellow Hit", String.Format("{0}%*Being in front of boss allows your attacks to be parried\r\nYou would need {1} more expertise to cap specials (WF,SS)",
                       YellowHit.ToString("F2", CultureInfo.InvariantCulture),
                       ratingRequired.ToString("F0", CultureInfo.InvariantCulture)));
                }
                else
                    dictValues.Add("Yellow Hit", YellowHit.ToString("F2", CultureInfo.InvariantCulture) + "%");
            }
            if (OverSpellHitCap > 0.38f) // only warn if more than .38% over cap (equivalent to 10 hit rating)
                dictValues.Add("Spell Hit", String.Format("{0}% (Over Cap)*Over Spell Hit Cap by {1}%",
                    SpellHit.ToString("F2", CultureInfo.InvariantCulture),
                    OverSpellHitCap.ToString("F2", CultureInfo.InvariantCulture)));
            else
            {
                if (SpellHit < 100f)
                {
                    float ratingRequired = (float)Math.Ceiling(StatConversion.GetRatingFromSpellHit(1f - SpellHit/100f));
                    dictValues.Add("Spell Hit", String.Format("{0}% (Under Cap)*You need {1} more hit rating to cap spells (ES, LB etc)", 
                        SpellHit.ToString("F2", CultureInfo.InvariantCulture),
                        ratingRequired.ToString("F0", CultureInfo.InvariantCulture)));
                }
                else
                    dictValues.Add("Spell Hit", SpellHit.ToString("F2", CultureInfo.InvariantCulture) + "%");
            }*/
            /*if (OverMeleeCritCap > 0.21f) // only warn if more than .21% over cap (equivalent to 10 crit rating)
                dictValues.Add("Melee Crit", String.Format("{0} (Over Cap)*Crit Rating {1} (+{2}% crit chance)\r\nOver Soft Cap by {3}%",
                    MeleeCrit.ToString("F2", CultureInfo.InvariantCulture) + "%",
                    BasicStats.CritRating.ToString("F0", CultureInfo.InvariantCulture),
                    (StatConversion.GetCritFromRating(BasicStats.CritRating) * 100f).ToString("F2", CultureInfo.InvariantCulture),
                    OverMeleeCritCap.ToString("F2", CultureInfo.InvariantCulture)));
            else
                dictValues.Add("Melee Crit", String.Format("{0}*Crit Rating {1} (+{2}% crit chance)",
                    MeleeCrit.ToString("F2", CultureInfo.InvariantCulture) + "%",
                    BasicStats.CritRating.ToString("F0", CultureInfo.InvariantCulture),
                    (StatConversion.GetCritFromRating(BasicStats.CritRating) * 100f).ToString("F2", CultureInfo.InvariantCulture)));

            dictValues.Add("Spell Crit", String.Format("{0}*Crit Rating {1} (+{2}% crit chance)",
                SpellCrit.ToString("F2", CultureInfo.InvariantCulture) + "%",
                BasicStats.CritRating.ToString("F0", CultureInfo.InvariantCulture),
                (StatConversion.GetSpellCritFromRating(BasicStats.CritRating) * 100f).ToString("F2", CultureInfo.InvariantCulture)));*/

            float spellMiss = 100 - SpellHit;
            dictValues.Add("Avoided Attacks", String.Format("{0}%*{1}% Boss Dodged\r\n{2}% Boss Parried\r\n{3}% Spell Misses\r\n{4}% White Misses",
                        AvoidedAttacks.ToString("F2", CultureInfo.InvariantCulture), 
                        DodgedAttacks.ToString("F2", CultureInfo.InvariantCulture),
                        ParriedAttacks.ToString("F2", CultureInfo.InvariantCulture),
                        spellMiss.ToString("F2", CultureInfo.InvariantCulture), 
                        MissedAttacks.ToString("F2", CultureInfo.InvariantCulture)));
            dictValues.Add("Armor Mitigation", ArmorMitigation.ToString("F2", CultureInfo.InvariantCulture) + "%*Amount of physical damage lost due to boss armor");
                                
            dictValues.Add("ED Uptime", String.Format("{0}%*{1}% ED Bonus Crit",
                EDUptime.ToString("F2", CultureInfo.InvariantCulture),
                EDBonusCrit.ToString("F2", CultureInfo.InvariantCulture)));
            dictValues.Add("Flurry Uptime", FlurryUptime.ToString("F2", CultureInfo.InvariantCulture) + "%");
            dictValues.Add("Avg Time to 5 Stack", String.Format("{0} sec*{1} PPM", 
                SecondsTo5Stack.ToString("F2", CultureInfo.InvariantCulture),
                _MWPPM.ToString("F2", CultureInfo.InvariantCulture)));
            dictValues.Add("MH Enchant Uptime", MHEnchantUptime.ToString("F2", CultureInfo.InvariantCulture) + "%");
            dictValues.Add("OH Enchant Uptime", OHEnchantUptime.ToString("F2", CultureInfo.InvariantCulture) + "%");
            dictValues.Add("Trinket 1 Uptime", Trinket1Uptime.ToString("F2", CultureInfo.InvariantCulture) + "%");
            dictValues.Add("Trinket 2 Uptime", Trinket2Uptime.ToString("F2", CultureInfo.InvariantCulture) + "%");
            dictValues.Add("Fire Totem Uptime", FireTotemUptime.ToString("F2", CultureInfo.InvariantCulture) + "%");
            #endregion
            #region Attacks Breakdown
            dictValues.Add("White Damage", dpsOutputFormat(SwingDamage, DPS, true));
            dictValues.Add("Windfury Attack", dpsOutputFormat(WindfuryAttack, DPS, true));
            dictValues.Add("Flametongue Attack", dpsOutputFormat(FlameTongueAttack, DPS, true));
            dictValues.Add("Stormstrike", dpsOutputFormat(Stormstrike, DPS, true));
            dictValues.Add("Lava Lash", dpsOutputFormat(LavaLash, DPS, true));
            dictValues.Add("Searing/Magma Totem", dpsOutputFormat(SearingMagma, DPS, false));
            dictValues.Add("Earth Shock", dpsOutputFormat(EarthShock, DPS, false));
            dictValues.Add("Flame Shock", dpsOutputFormat(FlameShock, DPS, false));
            dictValues.Add("Lightning Bolt", dpsOutputFormat(LightningBolt, DPS, false));
            dictValues.Add("Unleash Wind", dpsOutputFormat(UnleashWind, DPS, true));
            dictValues.Add("Unleash Flame", dpsOutputFormat(UnleashFlame, DPS, false));
            dictValues.Add("Lightning Shield", dpsOutputFormat(LightningShield, DPS, false));
            dictValues.Add("Chain Lightning", dpsOutputFormat(ChainLightning, DPS, false));
            dictValues.Add("Fire Nova", dpsOutputFormat(FireNova, DPS, false));
            dictValues.Add("Fire Elemental", FireElemental.getDPSOutput());
            dictValues.Add("Spirit Wolf", dpsOutputFormat(SpiritWolf, DPS, true));
            dictValues.Add("Magic Other", dpsOutputFormat(MagicOther, DPS, false));
            dictValues.Add("Physical Other", dpsOutputFormat(PhysicalOther, DPS, true));
            dictValues.Add("Total DPS", DPS.ToString("F2", CultureInfo.InvariantCulture));
            #endregion
            
            return dictValues;
        }

        private String dpsOutputFormat(DPSAnalysis dpsStat, float totaldps, bool AP)
        {
            float percent = dpsStat.dps / totaldps * 100f;
            string power = AP ? "Av.AP : " + _attackPower.ToString("F2", CultureInfo.InvariantCulture) :
                                "Av.SP : " + _spellPower.ToString("F2", CultureInfo.InvariantCulture);
            return string.Format("{0}\r\n{1}% of total dps\r\nPPM    : {2}\r\n{3}",
                dpsStat, percent.ToString("F2", CultureInfo.InvariantCulture), dpsStat.PPM.ToString("F2", CultureInfo.InvariantCulture), power);
        }
        
        private String dpsOutputFormat(float dps, float totaldps)
        {
            float percent = dps / totaldps * 100f;
            return string.Format("{0}*{1}% of total dps", 
                dps.ToString("F2", CultureInfo.InvariantCulture),
                percent.ToString("F2", CultureInfo.InvariantCulture));
        }

        private String getExpertiseString()
        {   // using 26.5 for display purposes as its pointless warning over cap if 26.1 Expertise for example.
            String caps = "";
            if (TotalExpertiseMH == TotalExpertiseOH)
            {
                if (TotalExpertiseMH > 26.5f)
                    caps = "{0} (Over Cap)*{1} Expertise\r\n{2} Expertise Rating\r\n{3}% Dodged\r\n{4}% Parried";
                else if (TotalExpertiseMH < 26)
                    caps = "{0} (Under Cap)*{1} Expertise\r\n{2} Expertise Rating\r\n{3}% Dodged\r\n{4}% Parried";
                else
                    caps = "{0}*{1} Expertise\r\n{2} Expertise Rating\r\n{3}% Dodged\r\n{4}% Parried";
                return String.Format(caps,
                    TotalExpertiseMH.ToString("F0", CultureInfo.InvariantCulture),
                    BasicStats.Expertise.ToString("F0", CultureInfo.InvariantCulture),
                    BasicStats.ExpertiseRating.ToString("F0", CultureInfo.InvariantCulture),
                    DodgedAttacks.ToString("F2", CultureInfo.InvariantCulture),
                    ParriedAttacks.ToString("F2", CultureInfo.InvariantCulture));
            }
            else
            {
                if (TotalExpertiseMH > 26.5f && TotalExpertiseOH > 26.5f)
                    caps = "{0}/{1} (Over Cap)*MH/OH\r\n{2} Expertise\r\n{3} Expertise Rating\r\n{4}% Dodged\r\n{5}% Parried";
                else if (TotalExpertiseMH > 26.5f)
                    caps = "{0}/{1} (MH Over Cap)*MH/OH\r\n{2} Expertise\r\n{3} Expertise Rating\r\n{4}% OH Dodged\r\n{5}% Parried";
                else if (TotalExpertiseOH > 26.5f)
                    caps = "{0}/{1} (OH Over Cap)*MH/OH\r\n{2} Expertise\r\n{3} Expertise Rating\r\n{4}% MH Dodged\r\n{5}% Parried";
                else if (TotalExpertiseMH < 26f)
                    caps = "{0}/{1} (MH Under Cap)*MH/OH\r\n{2} Expertise\r\n{3} Expertise Rating\r\n{4}% Dodged";
                else 
                    caps = "{0}/{1}*MH/OH\r\n{2} Expertise\r\n{3} Expertise Rating\r\n{4}% Dodged";
               return String.Format(caps,
                    TotalExpertiseMH.ToString("F0", CultureInfo.InvariantCulture),
                    TotalExpertiseOH.ToString("F0", CultureInfo.InvariantCulture), 
                    BasicStats.Expertise.ToString("F0", CultureInfo.InvariantCulture),
                    BasicStats.ExpertiseRating.ToString("F0", CultureInfo.InvariantCulture),
                    DodgedAttacks.ToString("F2", CultureInfo.InvariantCulture),
                    ParriedAttacks.ToString("F2", CultureInfo.InvariantCulture));
            }
        }

        public override float GetOptimizableCalculationValue(string calculation)
        {
            switch (calculation)
            {
                case "Health": return BasicStats.Health;
                case "DPS Points": return DPS;
                case "% Chance to Miss (White)": return (100 - WhiteHit);
                case "% Chance to Miss (Yellow)": return (100 - YellowHit);
                case "% Chance to Miss (Spell)": return (100 - SpellHit);
                case "% Chance to be Dodged": return DodgedAttacks;
            }
            return 0f;
        }
    }
}
