﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rawr.Retribution
{
    [Rawr.Calculations.RawrModelInfo("Retribution", "Spell_Holy_CrusaderStrike", CharacterClass.Paladin)]
    public class CalculationsRetribution : CalculationsBase
    {
        #region Model Properties
        #region Gemming Templates
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                // Relevant Gem IDs for Retribution
                //               common uncommon rare  jewel |  fills in gaps if it can
                int[] null_arr = { 0, 0, 0, 0 }; fixArray(null_arr);
                // Red slots
                int[] red_str = { 52081, 52206, 71883, 52255 }; fixArray(red_str);
                // Blue slots
                int[] blu_hit = { 52089, 52235, 71817, 52264 }; fixArray(blu_hit);
                // Yellow slots
                int[] ylw_mst = { 52094, 52219, 71877, 52269 }; fixArray(ylw_mst);
                int[] ylw_crt = { 52091, 52241, 71874, 52266 }; fixArray(ylw_crt);
                int[] ylw_has = { 52093, 52232, 71876, 52268 }; fixArray(ylw_has);
                // Orange slots
                int[] org_mst = { 52114, 52240, 71856, 00000 }; fixArray(org_mst);
                int[] org_crt = { 52108, 52222, 71843, 00000 }; fixArray(org_crt);
                int[] org_has = { 52111, 52214, 71851, 00000 }; fixArray(org_has);
                // Green slots
                int[] grn_hit = { 52128, 52237, 71825, 00000 }; fixArray(grn_hit);
                int[] grn_mst = { 52126, 52231, 71825, 00000 }; fixArray(grn_mst);
                int[] grn_crt = { 52121, 52223, 71823, 00000 }; fixArray(grn_crt);
                int[] grn_has = { 52124, 52218, 71824, 00000 }; fixArray(grn_has);
                // Purple slots
                int[] ppl_hit = { 52101, 52213, 71866, 00000 }; fixArray(ppl_hit);
                // Cogwheels
                int[] cog_exp = { 59489, 59489, 59489, 59489 }; fixArray(cog_exp);
                int[] cog_hit = { 59493, 59493, 59493, 59493 }; fixArray(cog_hit);
                int[] cog_mst = { 59480, 59480, 59480, 59480 }; fixArray(cog_mst);
                int[] cog_crt = { 59478, 59478, 59478, 59478 }; fixArray(cog_crt);
                int[] cog_has = { 59479, 59479, 59479, 59479 }; fixArray(cog_has);

                string group;
                List<GemmingTemplate> templates = new List<GemmingTemplate>();

                #region Strength
                group = "Strength";
                // Straight
                AddTemplates(templates,
                    red_str, red_str, red_str,
                    red_str, red_str, red_str,
                    group, true);
                // Socket Bonus
                AddTemplates(templates,
                    red_str, null_arr, null_arr,
                    null_arr, ppl_hit, grn_mst,
                    group, true);
                #endregion
                return templates;
            }
        }
        private static void fixArray(int[] thearray)
        {
            if (thearray[0] == 0) return; // Nothing to do, they are all 0
            if (thearray[1] == 0) thearray[1] = thearray[0]; // There was a Green, but no Blue
            if (thearray[2] == 0) thearray[2] = thearray[1]; // There was a Blue (or Green as set above), but no Purple
            if (thearray[3] == 0) thearray[3] = thearray[2]; // There was a Purple (or Blue/Green as set above), but no Jewel
        }
        private static void AddTemplates(List<GemmingTemplate> templates, int[] red, int[] ylw, int[] blu, int[] org, int[] prp, int[] grn, string group, bool enabled)
        {
            const int chaotic = 68779; // Meta
            const string groupFormat = "{0} {1}";
            int[] Cogs = new int[2] {59480, 59478};
            string[] quality = new string[] { "Uncommon", "Rare", "Epic", "Jewelcrafter" };
            for (int j = 0; j < 4; j++)
            {
                // Check to make sure we're not adding the same gem template twice due to repeating JC gems
                if (j != 3 || !(red[j] == red[j - 1] && blu[j] == blu[j - 1] && ylw[j] == ylw[j - 1]))
                {
                    string groupStr = String.Format(groupFormat, quality[j], group);
                    templates.Add(new GemmingTemplate()
                    {
                        Model = "Retribution",
                        Group = groupStr,
                        RedId = red[j] != 0 ? red[j] : org[j] != 0 ? org[j] : prp[j],
                        YellowId = ylw[j] != 0 ? ylw[j] : org[j] != 0 ? org[j] : grn[j],
                        BlueId = blu[j] != 0 ? blu[j] : prp[j] != 0 ? prp[j] : grn[j],
                        PrismaticId = red[j] != 0 ? red[j] : ylw[j] != 0 ? ylw[j] : blu[j],
                        MetaId = chaotic,
                        CogwheelId = Cogs[0],
                        Cogwheel2Id = Cogs[1],
                        Enabled = (enabled && j == 2)
                    });
                }
            }
        }
        #endregion

        /// <summary>
        /// Buffs that will be enabled by default in the given character object
        /// </summary>
        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd("Horn of Winter");
            character.ActiveBuffsAdd("Blessing of Might (AP%)");
            character.ActiveBuffsAdd("Elemental Oath");
            character.ActiveBuffsAdd("Arcane Tactics");
            character.ActiveBuffsAdd("Improved Icy Talons");
            character.ActiveBuffsAdd("Power Word: Fortitude");
            character.ActiveBuffsAdd("Totemic Wrath");
            character.ActiveBuffsAdd("Arcane Brilliance (Mana)");
            character.ActiveBuffsAdd("Critical Mass");
            character.ActiveBuffsAdd("Wrath of Air Totem");
            character.ActiveBuffsAdd("Blessing of Kings");
            character.ActiveBuffsAdd("Sunder Armor");
            character.ActiveBuffsAdd("Blood Frenzy");
            character.ActiveBuffsAdd("Shadow and Flame");
            character.ActiveBuffsAdd("Curse of the Elements");
            character.ActiveBuffsAdd("Strength Food");
            character.ActiveBuffsAdd("Flask of Titanic Strength");

            if (character.PrimaryProfession == Profession.Alchemy || character.SecondaryProfession == Profession.Alchemy)
                character.ActiveBuffsAdd("Flask of Titanic Strength (Mixology)");

            // Need to be behind boss
            character.BossOptions.InBack = true;
            character.BossOptions.InBackPerc_Melee = 1.00d;
        }

        private static List<string> _relevantGlyphs;
        /// <summary>
        /// List of glyphs that will be available in the Glyph subtab of the Talents tab.
        /// </summary>
        public override List<string> GetRelevantGlyphs()
        {
            return _relevantGlyphs ?? (_relevantGlyphs = new List<string>
                                                             {
                                                                 "Glyph of Crusader Strike",
                                                                 "Glyph of Exorcism",
                                                                 "Glyph of Judgement",
                                                                 "Glyph of Seal of Truth",
                                                                 "Glyph of Templar's Verdict",
                                                                 "Glyph of Consecration",
                                                                 "Glyph of Hammer of Wrath",
                                                                 "Glyph of Rebuke",
                                                                 "Glyph of Ascetic Crusader"
                                                             });
        }

        private string[] _optimizableCalculationLabels;
        /// <summary>
        /// Labels of the stats available to the Optimizer.
        /// </summary>
        /// The list of labels listed here needs to match with the list in GetOptimizableCalculationValue override in CharacterCalculationsRetribution.cs
        public override string[] OptimizableCalculationLabels
        {
            get
            {
                return _optimizableCalculationLabels ?? (_optimizableCalculationLabels = new string[] {
                                                                     "Health",
                                                                     "% Chance to Miss (Melee)",
                                                                     "% Chance to Miss (Spells)",
                                                                     "% Chance to be Dodged",
                                                                     "% Chance to be Parried",
                                                                     "% Chance to be Avoided (Melee/Dodge)"
                                                                  });
            }
        }

        /// <summary>
        /// Dictionary<string, Color> that includes the names of each rating which your model will use,
        /// and a color for each. These colors will be used in the charts.
        /// 
        /// EXAMPLE: 
        /// subPointNameColors = new Dictionary<string, System.Drawing.Color>();
        /// subPointNameColors.Add("Mitigation", System.Drawing.Colors.Red);
        /// subPointNameColors.Add("Survival", System.Drawing.Colors.Blue);
        /// </summary>
        private Dictionary<string, System.Windows.Media.Color> _subPointNameColors;
        public override Dictionary<string, System.Windows.Media.Color> SubPointNameColors
        {
            get
            {
                if (_subPointNameColors == null)
                {
                    _subPointNameColors = new Dictionary<string, System.Windows.Media.Color>();
                    _subPointNameColors.Add("DPS", System.Windows.Media.Colors.Red);
                }
                return _subPointNameColors;
            }
        }

        /// <summary>
        /// Creates the CalculationOptionPanel
        /// </summary>
        private ICalculationOptionsPanel _calculationOptionsPanel;
        public override ICalculationOptionsPanel CalculationOptionsPanel
        {
            get { return _calculationOptionsPanel ?? (_calculationOptionsPanel = new CalculationOptionsPanelRetribution()); }
        }

        private string[] _characterDisplayCalculationLabels;
        /// <summary>
        /// An array of strings which will be used to build the calculation display.
        /// Each string must be in the format of "Heading:Label". Heading will be used as the
        /// text of the group box containing all labels that have the same Heading.
        /// Label will be the label of that calculation, and may be appended with '*' followed by
        /// a description of that calculation which will be displayed in a tooltip for that label.
        /// Label (without the tooltip string) must be unique.
        /// 
        /// EXAMPLE:
        /// characterDisplayCalculationLabels = new string[]
        /// {
        ///		"Basic Stats:Health",
        ///		"Basic Stats:Armor",
        ///		"Advanced Stats:Dodge",
        ///		"Advanced Stats:Miss*Chance to be missed"
        /// };
        /// 
        /// Values used here need to be defined via the GetCharacterDisplayCalculationValues() member
        /// in CharacterCalculationsRetribution.cs
        /// </summary>
        public override string[] CharacterDisplayCalculationLabels
        {
            get
            {
                if (_characterDisplayCalculationLabels == null)
                {
                    List<string> labels = new List<string>(new string[]
                    {
                        "Basic Stats:Health",
                        "Basic Stats:Mana",
                        "Basic Stats:Strength",
                        "Basic Stats:Agility",
                        "Basic Stats:Attack Power",
                        "Basic Stats:Melee Crit",
                        "Basic Stats:Melee Haste",
                        "Basic Stats:Chance to Dodge",
                        "Basic Stats:Mastery",
                        "Basic Stats:Miss Chance",
                        "Basic Stats:Spell Power",
                        "Basic Stats:Spell Crit",
                        "Basic Stats:Spell Haste",
                        "Basic Stats:Weapon Damage",
                        "Basic Stats:Weapon Damage @3.3",
                        "Basic Stats:Attack Speed",
                        "DPS Breakdown:Total DPS",
                        "DPS Breakdown:White",
                        "DPS Breakdown:Seal",
                        "DPS Breakdown:Seal (Dot)",
                        "DPS Breakdown:Seal of Command",
                        "DPS Breakdown:Crusader Strike",
                        "DPS Breakdown:Templars Verdict",
                        "DPS Breakdown:Exorcism",
                        "DPS Breakdown:Hammer of Wrath",
                        "DPS Breakdown:Holy Wrath",
                        "DPS Breakdown:Judgement",
                        "DPS Breakdown:Consecration",
                        "DPS Breakdown:GoaK",
                        "DPS Breakdown:Other*From trinket procs",
                        "Rotation Info:Inqusition Uptime",
                        "Rotation Info:White Usage",
                        "Rotation Info:Seal Usage",
                        "Rotation Info:Seal (Dot) Usage",
                        "Rotation Info:Seal of Command Usage",
                        "Rotation Info:Crusader Strike Usage",
                        "Rotation Info:Templar's Verdict Usage",
                        "Rotation Info:Exorcism Usage",
                        "Rotation Info:Hammer of Wrath Usage",
                        "Rotation Info:Holy Wrath Usage",
                        "Rotation Info:Judgement Usage",
                        "Rotation Info:Consecration Usage",
                        "Rotation Info:GoaK Usage"
                    });
                    _characterDisplayCalculationLabels = labels.ToArray();
                }
                return _characterDisplayCalculationLabels;
            }
        }
        #endregion

        public override CharacterClass TargetClass { get { return CharacterClass.Paladin; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation()
        {
            return new ComparisonCalculationRetribution();
        }

        public override CharacterCalculationsBase CreateNewCharacterCalculations()
        {
            return new CharacterCalculationsRetribution();
        }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            System.Xml.Serialization.XmlSerializer serializer =
            new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsRetribution));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsRetribution calcOpts = serializer.Deserialize(reader) as CalculationOptionsRetribution;
            return calcOpts;
        }

        /// <summary>
        /// GetCharacterCalculations is the primary method of each model, where a majority of the calculations
        /// and formulae will be used. GetCharacterCalculations should call GetCharacterStats(), and based on
        /// those total stats for the character, and any calculationoptions on the character, perform all the 
        /// calculations required to come up with the final calculations defined in 
        /// CharacterDisplayCalculationLabels, including an Overall rating, and all Sub ratings defined in 
        /// SubPointNameColors.
        /// </summary>
        /// <param name="character">The character to perform calculations for.</param>
        /// <param name="additionalItem">An additional item to treat the character as wearing.
        /// This is used for gems, which don't have a slot on the character to fit in, so are just
        /// added onto the character, in order to get gem calculations.</param>
        /// <returns>A custom CharacterCalculations object which inherits from CharacterCalculationsBase,
        /// containing all of the final calculations defined in CharacterDisplayCalculationLabels. See
        /// CharacterCalculationsBase comments for more details.</returns>
        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            return GetCharacterCalculations(character, additionalItem, GetCharacterRotation(character, additionalItem));
        }

        public CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, RotationCalculation rot)
        {
            // First things first, we need to ensure that we aren't using bad data
            CharacterCalculationsRetribution calc = new CharacterCalculationsRetribution();
            if (character == null) { return calc; }
            if (rot == null) { return calc; }

            calc.Rotation = rot;
            calc.BasicStats = GetCharacterStats(character, additionalItem, false);
            //Damage procs are modeled as DPS
            calc.OtherDPS = new MagicDamage("", character, rot.Stats, DamageType.Arcane).AverageDamage
                          + new MagicDamage("", character, rot.Stats, DamageType.Fire).AverageDamage
                          + new MagicDamage("", character, rot.Stats, DamageType.Shadow).AverageDamage
                          + new MagicDamage("", character, rot.Stats, DamageType.Frost).AverageDamage
                          + new MagicDamage("", character, rot.Stats, DamageType.Nature).AverageDamage
                          + new MagicDamage("", character, rot.Stats, DamageType.Holy).AverageDamage;
            rot.SetDPS(calc);
            calc.OverallPoints = calc.DPSPoints;

            return calc;
        }

        public RotationCalculation GetCharacterRotation(Character character, Item additionalItem)
        {
            // First things first, we need to ensure that we aren't using bad data
            if (character == null) { return null; }
            CalculationOptionsRetribution calcOpts = character.CalculationOptions as CalculationOptionsRetribution;
            if (calcOpts == null) { return null; }
            //
            return CreateRotation(character, GetCharacterStats(character, additionalItem, true));
        }

        /// <summary>
        /// GetCharacterStats is the 2nd-most calculation intensive method in a model. Here the model will
        /// combine all of the information about the character, including race, gear, enchants, buffs,
        /// calculationoptions, etc., to form a single combined Stats object. Three of the methods below
        /// can be called from this method to help total up stats: GetItemStats(character, additionalItem),
        /// GetEnchantsStats(character), and GetBuffsStats(character.ActiveBuffs).
        /// </summary>
        /// <param name="character">The character whose stats should be totaled.</param>
        /// <param name="additionalItem">An additional item to treat the character as wearing.
        /// This is used for gems, which don't have a slot on the character to fit in, so are just
        /// added onto the character, in order to get gem calculations.</param>
        /// <returns>A Stats object containing the final totaled values of all character stats.</returns>
        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            return GetCharacterRotation(character, additionalItem).Stats;
        }

        public RotationCalculation CreateRotation(Character character, StatsRetri stats)
        {
            return new RotationCalculation(character, stats);
        }

        #region Stats conversion / calculation
        private static readonly SpecialEffect _GoakStrengthSE = new SpecialEffect(Trigger.DamageOrHealingDone, new Stats() { BonusStrengthMultiplier = PaladinConstants.GOAK_STRENGTH }, PaladinConstants.GOAK_DURATION, 0f, 1f, 20);
        private static readonly SpecialEffect _GoakSE = new SpecialEffect(Trigger.Use, new Stats(_GoakStrengthSE), PaladinConstants.GOAK_DURATION, PaladinConstants.GOAK_COOLDOWN);
        
        public StatsRetri GetCharacterStats(Character character, Item additionalItem, bool computeAverageStats)
        {
            PaladinTalents talents = character.PaladinTalents;
            CalculationOptionsRetribution calcOpts = character.CalculationOptions as CalculationOptionsRetribution;

            StatsRetri stats = new StatsRetri();
            stats.Accumulate(BaseStats.GetBaseStats(character.Level, CharacterClass.Paladin, character.Race)); //Race
            stats.Accumulate(GetItemStats(character, additionalItem));                                         //Items 
            stats.Accumulate(GetBuffsStats(character, calcOpts));                                              //Buffs 

            // Adjust expertise for racial passive
            stats.Expertise += BaseStats.GetRacialExpertise(character, ItemSlot.MainHand);
            // Judgements of the pure (Flat because it has nearly always a 100% chance.
            stats.PhysicalHaste += PaladinConstants.JUDGEMENTS_OF_THE_PURE * talents.JudgementsOfThePure;
            stats.SpellHaste += PaladinConstants.JUDGEMENTS_OF_THE_PURE * talents.JudgementsOfThePure;
            
            //Sets
            stats.SetSets(character);
                        
            // If wanted, Average out any Proc and OnUse effects into the stats
            if (computeAverageStats)
            {
                StatsRetri statsTmp = stats.Clone();
                ConvertRatings(statsTmp, talents, character);// Convert ratings so we have right value for haste, weaponspeed and talents etc.
                RotationCalculation rot = CreateRotation(character, statsTmp);

                Dictionary<Trigger, float> triggerIntervals = new Dictionary<Trigger, float>();
                Dictionary<Trigger, float> triggerChances = new Dictionary<Trigger, float>();
                CalculateTriggers(triggerIntervals, triggerChances, rot);

                //Talent special effects
                //GoaK Strength
                stats.AddSpecialEffect(_GoakSE);
                
                // Average out proc effects, and add to global stats.
                Stats statsAverage = new Stats();
                foreach (SpecialEffect effect in stats.SpecialEffects())
                    statsAverage.Accumulate(effect.GetAverageStats(triggerIntervals, triggerChances, AbilityHelper.BaseWeaponSpeed(character), character.BossOptions.BerserkTimer));
                stats.Accumulate(statsAverage);
            }

            // No negative values (from possible charts)
            if (stats.Strength < 0)
                stats.Strength = 0;
            if (stats.Agility < 0)
                stats.Agility = 0;
            if (stats.AttackPower < 0)
                stats.AttackPower = 0;
            if (stats.ExpertiseRating < 0)
                stats.ExpertiseRating = 0;
            if (stats.HitRating < 0)
                stats.HitRating = 0;
            if (stats.CritRating < 0)
                stats.CritRating = 0;
            if (stats.HasteRating < 0)
                stats.HasteRating = 0;
            if (stats.SpellPower < 0)
                stats.SpellPower = 0;
            if (stats.MasteryRating < 0)
                stats.MasteryRating = 0;
        
            // ConvertRatings needs to be done AFTER accounting for the averaged stats, since stat multipliers 
            // should affect the averaged stats also.
            ConvertRatings(stats, talents, character);

            return stats;
        }

        
        private static void CalculateTriggers(Dictionary<Trigger, float> triggerIntervals, Dictionary<Trigger, float> triggerChances, RotationCalculation rot)
        {
            triggerChances[Trigger.MeleeCrit] = triggerChances[Trigger.MeleeHit] = triggerChances[Trigger.MeleeAttack] = triggerChances[Trigger.PhysicalCrit] = triggerChances[Trigger.PhysicalHit] = triggerChances[Trigger.PhysicalAttack] =
                triggerChances[Trigger.DamageDone] = triggerChances[Trigger.SpellHit] = triggerChances[Trigger.DamageSpellHit] = triggerChances[Trigger.SpellCrit] = triggerChances[Trigger.DamageSpellCrit] =
                triggerChances[Trigger.DamageOrHealingDone] = triggerChances[Trigger.DoTTick] = triggerChances[Trigger.Use] = 1f;

            triggerIntervals[Trigger.Use] = 0f;
            triggerIntervals[Trigger.MeleeCrit] = (float)(1f / rot.GetMeleeCritsPerSec());
            triggerIntervals[Trigger.MeleeHit] = triggerIntervals[Trigger.MeleeAttack] = (float)(1f / rot.MeleeAttacksPerSec);
            triggerIntervals[Trigger.PhysicalCrit] = (float)(1f / rot.GetPhysicalCritsPerSec());
            triggerIntervals[Trigger.PhysicalHit] = triggerIntervals[Trigger.PhysicalAttack] = (float)(1f / rot.GetPhysicalAttacksPerSec());
            triggerIntervals[Trigger.DamageDone] = triggerIntervals[Trigger.DamageOrHealingDone] = (float)(1f / rot.GetAttacksPerSec());
            triggerIntervals[Trigger.SpellHit] = triggerIntervals[Trigger.DamageSpellHit] = (float)(1f / rot.SpellAttacksPerSec);
            triggerIntervals[Trigger.SpellCrit] = triggerIntervals[Trigger.DamageSpellCrit] = (float)(1f / rot.GetSpellCritsPerSec());
            triggerIntervals[Trigger.DoTTick] = (float)(1f / rot.GetAbilityHitsPerSecond(DamageAbility.SealDot));

            triggerIntervals[Trigger.WhiteHit] = (float)(1f / rot.GetAbilityHitsPerSecond(DamageAbility.White));
            triggerChances[Trigger.WhiteHit] = rot.White.CT.ChanceToLand;
            triggerIntervals[Trigger.CrusaderStrikeHit] = (float) (1f / rot.GetAbilityHitsPerSecond(DamageAbility.CrusaderStrike));
            triggerChances[Trigger.CrusaderStrikeHit] = rot.CS.CT.ChanceToLand;
            triggerIntervals[Trigger.JudgementHit] = (float) (1f / rot.GetAbilityHitsPerSecond(DamageAbility.Judgement));
            triggerChances[Trigger.JudgementHit] = rot.Judge.CT.ChanceToLand;
        }

        // Combine talents and buffs into primary and secondary stats.
        // Convert ratings into their percentage values.
        private void ConvertRatings(Stats stats, PaladinTalents talents, Character character)
        {
            // Primary stats
            stats.Strength += stats.HighestStat;
            stats.Strength *= (1 + stats.BonusStrengthMultiplier + (Character.ValidateArmorSpecialization(character, ItemType.Plate) ? .05f : 0f));
            stats.Agility *= (1 + stats.BonusAgilityMultiplier);
            stats.Stamina *= (1 + stats.BonusStaminaMultiplier);
            stats.Intellect *= (1 + stats.BonusIntellectMultiplier);

            // Secondary stats
            // GetManaFromIntellect/GetHealthFromStamina account for the fact 
            // that the first 20 Int/Sta only give 1 Mana/Health each.
            stats.Mana += StatConversion.GetManaFromIntellect(stats.Intellect, CharacterClass.Paladin);
            stats.Mana *= (1f + stats.BonusManaMultiplier);
            stats.Health += StatConversion.GetHealthFromStamina(stats.Stamina, CharacterClass.Paladin);
            stats.Health *= (1f + stats.BonusHealthMultiplier);
            stats.AttackPower += stats.Strength * 2;
            stats.AttackPower *= (1f + stats.BonusAttackPowerMultiplier);

            // Combat ratings
            if (stats.HighestSecondaryStat > 0)
            {
                if (stats.CritRating > stats.MasteryRating)
                    if (stats.HasteRating > stats.CritRating)
                        stats.HasteRating += stats.HighestSecondaryStat;
                    else
                        stats.CritRating += stats.HighestSecondaryStat;
                else
                    if (stats.HasteRating > stats.MasteryRating)
                        stats.HasteRating += stats.HighestSecondaryStat;
                    else
                        stats.MasteryRating += stats.HighestSecondaryStat;
            }
            stats.Expertise += (talents.GlyphOfSealOfTruth ? PaladinConstants.GLYPH_OF_SEAL_OF_TRUTH : 0) + StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Paladin);
            stats.PhysicalHit += StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Paladin);
            stats.SpellHit += StatConversion.GetSpellHitFromRating(stats.HitRating, CharacterClass.Paladin) + PaladinConstants.SHEATH_SPHIT_COEFF;

            stats.PhysicalCrit +=
                StatConversion.GetPhysicalCritFromRating(stats.CritRating, CharacterClass.Paladin) +
                StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.Paladin);
            stats.SpellCrit += stats.SpellCritOnTarget +
                StatConversion.GetSpellCritFromRating(stats.CritRating, CharacterClass.Paladin) +
                StatConversion.GetSpellCritFromIntellect(stats.Intellect, CharacterClass.Paladin);

            stats.PhysicalHaste = (1f + stats.PhysicalHaste) * (1f + StatConversion.GetPhysicalHasteFromRating(stats.HasteRating, CharacterClass.Paladin)) - 1f;
            stats.SpellHaste = (1f + stats.SpellHaste) * (1f + StatConversion.GetSpellHasteFromRating(stats.HasteRating, CharacterClass.Paladin)) - 1f;

            stats.BonusDamageMultiplier += talents.Communion * PaladinConstants.COMMUNION;

            stats.SpellPower += stats.AttackPower * PaladinConstants.SHEATH_AP_COEFF;
            stats.SpellPower *= (1f + stats.BonusSpellPowerMultiplier);
        }
        
        public Stats GetBuffsStats(Character character, CalculationOptionsRetribution calcOpts) 
        {
            // Variables
            List<Buff> removedBuffs = new List<Buff>();
            List<Buff> addedBuffs = new List<Buff>();
            List<Buff> buffGroup = new List<Buff>();
            // Removes the Stats from the Heroism and Equivalent buffs, but keeps the relevancy
            buffGroup.Clear();
            buffGroup.Add(Buff.GetBuffByName("Heroism/Bloodlust"));
            buffGroup.Add(Buff.GetBuffByName("Time Warp"));
            buffGroup.Add(Buff.GetBuffByName("Ancient Hysteria"));
            MaintBuffHelper(buffGroup, character, removedBuffs);
            // Pull the stats from the modified active buffs list
            Stats statsBuffs = GetBuffsStats(character.ActiveBuffs, character.SetBonusCount);
            // Put things back the way they were for the UI
            foreach (Buff b in removedBuffs) { character.ActiveBuffsAdd(b); }
            foreach (Buff b in addedBuffs) { character.ActiveBuffs.Remove(b); }
            // Return the result
            return statsBuffs;
        }
        private static void MaintBuffHelper(List<Buff> buffGroup, Character character, List<Buff> removedBuffs)
        {
            foreach (Buff b in buffGroup)
            {
                if (character.ActiveBuffs.Remove(b)) { removedBuffs.Add(b); }
            }
        }
        #endregion

        #region Relevancy Methods
        /// <summary>
        /// List of itemtypes that are relevant for retribution
        /// </summary>
        private List<ItemType> _relevantItemTypes;
        public override List<ItemType> RelevantItemTypes
        {
            get
            {
                return _relevantItemTypes ?? (_relevantItemTypes = new List<ItemType>(new ItemType[]
                {
                    ItemType.None,
                    ItemType.Plate,
                    ItemType.Relic,
                    ItemType.TwoHandAxe,
                    ItemType.TwoHandMace,
                    ItemType.TwoHandSword
                }));
            }
        }

        /// <summary>
        /// List of SpecialEffect Triggers that are relevant for retribution model
        /// Ever trigger listed here needs an implementation in ProcessSpecialEffects()
        /// A trigger not listed here should not appear in ProcessSpecialEffects()
        /// </summary>
        internal static List<Trigger> _RelevantTriggers;
        internal static List<Trigger> RelevantTriggers
        {
            get
            {
                return _RelevantTriggers ?? (_RelevantTriggers = new List<Trigger>() {
                            Trigger.Use,
                            Trigger.SpellCrit,
                            Trigger.SpellHit,
                            Trigger.DamageSpellCrit,
                            Trigger.DamageSpellHit,
                            Trigger.PhysicalCrit,
                            Trigger.PhysicalHit,
                            Trigger.PhysicalAttack,
                            Trigger.MeleeCrit,
                            Trigger.MeleeHit,
                            Trigger.MeleeAttack,
                            Trigger.WhiteHit,
                            Trigger.DamageDone,
                            Trigger.DamageOrHealingDone,
                            Trigger.DoTTick,
                            Trigger.JudgementHit,
                            Trigger.CrusaderStrikeHit,
                        });
            }
        }

        public override bool IsItemRelevant(Item item)
        {
            // First we let normal rules (profession, class, relevant stats) decide
            bool relevant = base.IsItemRelevant(item);

            // Next we use our special stat relevancy filtering.
            if (relevant)
                relevant = HasPrimaryStats(item.Stats) || (HasSecondaryStats(item.Stats) && !HasUnwantedStats(item.Stats));

            return relevant;
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            // First we let normal rules (profession, class, relevant stats) decide
            bool relevant = base.IsBuffRelevant(buff, character);

            // Next we use our special stat relevancy filtering on consumables. (party buffs only need filtering on relevant stats)
            if (relevant && (buff.Group == "Elixirs and Flasks" || buff.Group == "Potion" || buff.Group == "Food" || buff.Group == "Scrolls" || buff.Group == "Temporary Buffs" || buff.Group == "Dark Intent"))
                relevant = HasPrimaryStats(buff.Stats) || (HasSecondaryStats(buff.Stats) && !HasUnwantedStats(buff.Stats));
            
            return relevant;
        }

        public override bool IsEnchantRelevant(Enchant enchant, Character character)
        {
            // First we let the normal rules (profession, class, relevant stats) decide
            bool relevant = base.IsEnchantRelevant(enchant, character);

            // Next we use our special stat relevancy filtering.
            if (relevant)
                relevant = HasPrimaryStats(enchant.Stats) || (HasSecondaryStats(enchant.Stats) && !HasUnwantedStats(enchant.Stats));

            return relevant;
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                Health = stats.Health,
                Mana = stats.Mana,
                Strength = stats.Strength,
                HighestStat = stats.HighestStat,
                Agility = stats.Agility,
                Intellect = stats.Intellect,
                Stamina = stats.Stamina,
                AttackPower = stats.AttackPower,
                HitRating = stats.HitRating,
                CritRating = stats.CritRating,
                ArmorPenetration = stats.ArmorPenetration,
                TargetArmorReduction = stats.TargetArmorReduction,
                HasteRating = stats.HasteRating,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                PhysicalCrit = stats.PhysicalCrit,
                PhysicalHaste = stats.PhysicalHaste,
                PhysicalHit = stats.PhysicalHit,
                SpellHit = stats.SpellHit,
                SpellHaste = stats.SpellHaste, 
                Expertise = stats.Expertise,
                ExpertiseRating = stats.ExpertiseRating,
                MasteryRating = stats.MasteryRating,
                HighestSecondaryStat = stats.HighestSecondaryStat,
                SpellPower = stats.SpellPower,
                CritBonusDamage = stats.CritBonusDamage,
                BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
                BonusCritDamageMultiplier = stats.BonusCritDamageMultiplier,
                BonusSpellCritDamageMultiplier = stats.BonusSpellCritDamageMultiplier,
                BonusPhysicalDamageMultiplier = stats.BonusPhysicalDamageMultiplier,
                BonusHolyDamageMultiplier = stats.BonusHolyDamageMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                BonusWhiteDamageMultiplier = stats.BonusWhiteDamageMultiplier,
                BonusSpellPowerMultiplier = stats.BonusSpellPowerMultiplier,
                JudgementCDReduction = stats.JudgementCDReduction,
                FireDamage = stats.FireDamage,
                FrostDamage = stats.FrostDamage,
                ArcaneDamage = stats.ArcaneDamage,
                ShadowDamage = stats.ShadowDamage,
                NatureDamage = stats.NatureDamage,
                HolyDamage = stats.HolyDamage,
                MovementSpeed = stats.MovementSpeed,
                SnareRootDurReduc = stats.SnareRootDurReduc,
                FearDurReduc = stats.FearDurReduc,
                StunDurReduc = stats.StunDurReduc,
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
            return HasPrimaryStats(stats) || HasSecondaryStats(stats) || HasExtraStats(stats);
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
            bool PrimaryStats = // Base stats
                                stats.Strength != 0 ||
                                stats.AttackPower != 0 ||
                                stats.Expertise != 0 ||
                                // Combat ratings
                                stats.ExpertiseRating != 0 ||
                                stats.PhysicalHit != 0 ||
                                stats.PhysicalCrit != 0 ||
                                stats.PhysicalHaste != 0 ||
                                // Stat and damage multipliers
                                stats.BonusStrengthMultiplier != 0 ||
                                stats.BonusAttackPowerMultiplier != 0 ||
                                stats.BonusPhysicalDamageMultiplier != 0 ||
                                stats.BonusWhiteDamageMultiplier != 0 ||
                                stats.BonusHolyDamageMultiplier != 0 ||
                                stats.BonusDamageMultiplier != 0 || 
                                stats.BonusCritChance != 0 ||
                                stats.BonusCritDamageMultiplier != 0 ||
                                // Probably unused
                                stats.JudgementCDReduction != 0 ||
                                stats.ArmorPenetration != 0 ||
                                stats.TargetArmorReduction != 0;
            // Item proc effects
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
            bool SecondaryStats = // Generic DPS stats, useful for casters and melee.
                                  stats.HitRating != 0 ||
                                  stats.CritRating != 0 ||
                                  stats.HasteRating != 0 ||
                                  stats.HighestStat != 0 ||
                                  stats.HighestSecondaryStat != 0 ||
                                  stats.MasteryRating != 0 ||
                                  // Melee stats
                                  stats.Agility != 0 ||
                                  stats.BonusAgilityMultiplier != 0 ||
                                  // Caster stats
                                  stats.Intellect != 0 ||                // Intellect increases spellcrit, so it contributes to DPS.
                                  stats.SpellCrit != 0 ||                // Exorcism can crit
                                  stats.SpellCritOnTarget != 0 ||        // Exorcism
                                  stats.SpellHit != 0 ||                 // Exorcism & Consecration (1st tick)
                                  stats.SpellPower != 0 ||               // All holy damage effects benefit from spellpower
                                  stats.SpellHaste != 0 ||               // GCD
                                  stats.BonusIntellectMultiplier != 0 || // See intellect
                                  stats.BonusSpellCritDamageMultiplier != 0 || // See spellcrit
                                  stats.BonusSpellPowerMultiplier != 0 || // see spellcrit
                                  // Damage procs
                                  stats.FireDamage != 0 ||
                                  stats.FrostDamage != 0 ||
                                  stats.ArcaneDamage != 0 ||
                                  stats.ShadowDamage != 0 ||
                                  stats.NatureDamage != 0 ||
                                  stats.HolyDamage != 0 ||
                                  // Special (unmodelled)
                                  stats.MovementSpeed != 0 ||
                                  stats.SnareRootDurReduc != 0 ||
                                  stats.FearDurReduc != 0 ||
                                  stats.StunDurReduc != 0;
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
            bool ExtraStats =   stats.Health != 0 ||
                                stats.Mana != 0 ||
                                stats.Stamina != 0 ||
                                stats.BonusStaminaMultiplier != 0 ||
                                stats.BonusManaMultiplier != 0 ||
                                stats.BattlemasterHealthProc != 0;
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
            // List of stats that will filter out some buffs (Flasks, Elixirs & Scrolls), Enchants and Items.
            bool UnwantedStats = stats.SpellPower != 0 ||
                                 stats.Spirit != 0 ||
                                 stats.Mp5 != 0 ||
                                 stats.BonusArmor != 0 ||
                                 stats.ParryRating != 0 ||
                                 stats.DodgeRating != 0 ||
                                 stats.BlockRating != 0;
            if (!UnwantedStats)
            {
                foreach (SpecialEffect effect in stats.SpecialEffects())
                {
                    if (!RelevantTriggers.Contains(effect.Trigger) || (_RelevantTriggers.Contains(effect.Trigger) && HasUnwantedStats(effect.Stats)))    // An unwanted stat could be behind a trigger we don't model.
                    {
                        UnwantedStats = true;
                        break;
                    }
                }
            }
            return UnwantedStats;
        }
        #endregion

        #region Custom Charts
        // Custom charts are extra charts which can be defined per model.
        // The charts are available via the "Slot" menu of the righthand Rawr chart panel.

        private string[] _customChartNames;
        public override string[] CustomChartNames
        {
            get
            {
                return _customChartNames ?? (_customChartNames = new string[] {
                                                                         "Seals",
                                                                         "Weapon Speed",
                                                                         "Inquisition Refresh below",
                                                                         "Holy Power per Inq",
                                                                         "Wait for Crusader"
                                                                     });
            }
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            if (chartName == "Seals")
            {
                CalculationOptionsRetribution initOpts = character.CalculationOptions as CalculationOptionsRetribution;

                Character baseChar = character.Clone();
                CalculationOptionsRetribution baseOpts = initOpts.Clone();
                baseChar.CalculationOptions = baseOpts;
                baseOpts.Seal = SealOf.None;
                CharacterCalculationsBase baseCalc = Calculations.GetCharacterCalculations(baseChar);

                Character deltaChar = character.Clone();
                CalculationOptionsRetribution deltaOpts = initOpts.Clone();
                deltaChar.CalculationOptions = deltaOpts;

                deltaOpts.Seal = SealOf.Righteousness;
                ComparisonCalculationBase Righteousness = Calculations.GetCharacterComparisonCalculations(baseCalc, deltaChar, "Seal of Righteousness", initOpts.Seal == SealOf.Righteousness);
                Righteousness.Item = null;

                deltaOpts.Seal = SealOf.Truth;
                ComparisonCalculationBase Truth = Calculations.GetCharacterComparisonCalculations(baseCalc, deltaChar, "Seal of Truth", initOpts.Seal == SealOf.Truth);
                Truth.Item = null;

                return new ComparisonCalculationBase[] { Righteousness, Truth };
            }
            if (chartName == "Weapon Speed")
            {
                if (character.MainHand == null)
                    return new ComparisonCalculationBase[] { };

                return new ComparisonCalculationBase[] 
                { 
                    GetWeaponSpeedComparison(character, 3.3f),
                    GetWeaponSpeedComparison(character, 3.4f),
                    GetWeaponSpeedComparison(character, 3.5f),
                    GetWeaponSpeedComparison(character, 3.6f),
                    GetWeaponSpeedComparison(character, 3.7f),
                    GetWeaponSpeedComparison(character, 3.8f)
                };
            }
            if (chartName == "Inquisition Refresh below")
            {
                return new ComparisonCalculationBase[] { 
                    GetInqRefreshComparison(character, 0f),
                    GetInqRefreshComparison(character, 1f),
                    GetInqRefreshComparison(character, 2f),
                    GetInqRefreshComparison(character, 3f),
                    GetInqRefreshComparison(character, 4f),
                    GetInqRefreshComparison(character, 5f),
                    GetInqRefreshComparison(character, 6f),
                    GetInqRefreshComparison(character, 7f),
                    GetInqRefreshComparison(character, 8f)
                };
            }
            if (chartName == "Holy Power per Inq")
            {
                return new ComparisonCalculationBase[] { 
                    GetInqHPComparison(character, 1),
                    GetInqHPComparison(character, 2),
                    GetInqHPComparison(character, 3)
                };
            }
            if (chartName == "Wait for Crusader")
            {
                return new ComparisonCalculationBase[] { 
                    GetWaitforCSComparison(character, .1f),
                    GetWaitforCSComparison(character, .2f),
                    GetWaitforCSComparison(character, .3f),
                    GetWaitforCSComparison(character, .4f),
                    GetWaitforCSComparison(character, .5f),
                    GetWaitforCSComparison(character, .6f),
                    GetWaitforCSComparison(character, .7f)
                };
            }
            return new ComparisonCalculationBase[0];
        }
        private ComparisonCalculationBase GetWaitforCSComparison(Character character, float waitforCS)
        {
            CalculationOptionsRetribution initOpts = character.CalculationOptions as CalculationOptionsRetribution;
            CalculationOptionsRetribution deltaOpts = initOpts.Clone();
            Character adjustedCharacter = character.Clone();
            adjustedCharacter.CalculationOptions = deltaOpts;

            ((CalculationOptionsRetribution)adjustedCharacter.CalculationOptions).SkipToCrusader = waitforCS;
            return Calculations.GetCharacterComparisonCalculations(Calculations.GetCharacterCalculations(character),
                                                                   adjustedCharacter,
                                                                   string.Format("Wait for CS < {0:0.0} CD", waitforCS),
                                                                   ((CalculationOptionsRetribution)character.CalculationOptions).SkipToCrusader == waitforCS);
        }

        private ComparisonCalculationBase GetInqRefreshComparison(Character character, float inqRefresh)
        {
            CalculationOptionsRetribution initOpts = character.CalculationOptions as CalculationOptionsRetribution;
            CalculationOptionsRetribution deltaOpts = initOpts.Clone();
            Character adjustedCharacter = character.Clone();
            adjustedCharacter.CalculationOptions = deltaOpts;

            ((CalculationOptionsRetribution)adjustedCharacter.CalculationOptions).InqRefresh = inqRefresh;
            return Calculations.GetCharacterComparisonCalculations(Calculations.GetCharacterCalculations(character),
                                                                   adjustedCharacter,
                                                                   string.Format("Refresh below {0:0} sec", inqRefresh),
                                                                   ((CalculationOptionsRetribution)character.CalculationOptions).InqRefresh == inqRefresh);
        }

        private ComparisonCalculationBase GetInqHPComparison(Character character, int inqHP)
        {
            CalculationOptionsRetribution initOpts = character.CalculationOptions as CalculationOptionsRetribution;
            CalculationOptionsRetribution deltaOpts = initOpts.Clone();
            Character adjustedCharacter = character.Clone();
            adjustedCharacter.CalculationOptions = deltaOpts;

            ((CalculationOptionsRetribution)adjustedCharacter.CalculationOptions).HPperInq = inqHP;
            return Calculations.GetCharacterComparisonCalculations(Calculations.GetCharacterCalculations(character),
                                                                   adjustedCharacter,
                                                                   string.Format("Refresh at {0:0} HP", inqHP),
                                                                   ((CalculationOptionsRetribution)character.CalculationOptions).HPperInq == inqHP);
        }

        private ComparisonCalculationBase GetWeaponSpeedComparison(Character character, float speed)
        {
            Character adjustedCharacter = character.Clone();
            adjustedCharacter.IsLoading = true;
            adjustedCharacter.MainHand = new ItemInstance(
                AdjustWeaponSpeed(character.MainHand.Item, speed),
                character.MainHand.RandomSuffixId,
                character.MainHand.Gem1, 
                character.MainHand.Gem2, 
                character.MainHand.Gem3, 
                character.MainHand.Enchant,
                character.MainHand.Reforging,
                character.MainHand.Tinkering);

            var comparison = Calculations.GetCharacterComparisonCalculations(
                Calculations.GetCharacterCalculations(character), 
                adjustedCharacter, 
                string.Format(
                    CultureInfo.InvariantCulture,
                    "{0:0.0} Speed", 
                    speed),
                character.MainHand.Item.Speed == speed);
            comparison.Item = null;

            return comparison;
        }

        private Item AdjustWeaponSpeed(Item weapon, float speed)
        {
            Item adjustedWeapon = weapon.Clone();

            adjustedWeapon.MinDamage = (int)Math.Round(weapon.MinDamage / weapon.Speed * speed);
            adjustedWeapon.MaxDamage = (int)Math.Round(weapon.MaxDamage / weapon.Speed * speed);
            adjustedWeapon.Speed = speed;

            return adjustedWeapon;
        }

        #endregion
    }
}
