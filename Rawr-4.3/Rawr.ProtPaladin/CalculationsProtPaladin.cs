﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Media;
using System.Xml.Serialization;
using Rawr.Base;

namespace Rawr.ProtPaladin
{
    [Rawr.Calculations.RawrModelInfo("ProtPaladin", "Ability_Paladin_JudgementsOfTheJust", CharacterClass.Paladin)]
    public class CalculationsProtPaladin : CalculationsBase
    {
        #region Variables and Properties
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                // Relevant Gem IDs for ProtPaladin
                // Red
                int[] bold = { 52081, 52176, 52206, 52255 };        // Strength
                //int[] delicate = { 52082, 52175, 52212, 52258 };    // Agility
                int[] flashing = { 52083, 52174, 52216, 52259 };    // Parry
                int[] precise = { 52085, 52172, 52230, 52260 };     // Expertise

                // Purple
                int[] accurate = { 52105, 52152, 52203, 52203 };    // Expertise + Hit
                int[] defender = { 52097, 52160, 52210, 52210 };    // Parry + Stamina
                int[] etched = { 52101, 52156, 52213, 52213 };      // Strength + Hit
                //int[] glinting = { 52102, 52155, 52220, 52220 };    // Agility + Hit
                int[] guardian = { 52099, 52158, 52221, 52221 };    // Expertise + Stamina
                int[] retaliating = { 52103, 52154, 52234, 52234 }; // Parry + Hit
                //int[] shifting = { 52096, 52161, 52238, 52238 };    // Agility + Stamina
                int[] sovereign = { 52095, 52162, 52243, 52243 };   // Strength + Stamina

                // Blue
                int[] rigid = { 52089, 52168, 52235, 52264 };       // Hit
                int[] solid = { 52086, 52171, 52242, 52261 };       // Stamina

                // Green
                int[] nimble = { 52120, 52137, 52227, 52227 };      // Dodge + Hit
                int[] puissant = { 52126, 52131, 52231, 52231 };    // Mastery + Stamina
                int[] regal = { 52119, 52138, 52233, 52233 };       // Dodge + Stamina
                int[] sensei = { 52128, 52129, 52237, 52237 };      // Mastery + Hit

                // Yellow
                int[] fractured = { 52094, 52163, 52219, 52269 };   // Mastery
                int[] subtle = { 52090, 52167, 52247, 52265 };      // Dodge

                // Orange
                //int[] adept = { 52115, 52142, 52204, 52204 };      // Agility + Mastery
                int[] fine = { 52116, 52141, 52215, 52215 };       // Parry + Mastery
                int[] keen = { 52118, 52139, 52224, 52224 };       // Expertise + Mastery
                //int[] polished = { 52106, 52151, 52229, 52229 };   // Agility + Dodge
                int[] resolute = { 52107, 52150, 52249, 52249 };   // Expertise + Dodge
                int[] skillful = { 52114, 52143, 52240, 52240 };   // Strength + Mastery

                // Meta
                int austere = 52294;                               // Stamina + Increased Armor Value
                int eternal = 52293;                               // Stamina + Shield Block Value
                int fleet = 52289;                                 // Mastery + Minor Run Speed

                // Cogwheel
                //int cog_flashing = 59491;                          // Parry
                //int cog_fractured = 59480;                         // Mastery
                //int cog_precise = 59489;                           // Expertise
                //int cog_rigid = 59493;                             // Hit
                //int cog_subtle = 59477;                            // Dodge

                string[] qualityGroupNames = new string[] { "Uncommon", "Perfect Uncommon", "Rare", "Jewelcrafter" };
                string[] typeGroupNames = new string[] { "Survivability", "Mitigation (Dodge)", "Mitigation (Parry)", "Mitigation (Mastery)", "Threat" };
                
                int[] metaTemplates = new int[] { austere, eternal, fleet };

                //    Red           Yellow      Blue        Prismatic
                int[,][] survivabilityTemplates = new int[,][]
                { // Survivability
                    { solid,        solid,      solid,      solid },
                };

                /*int[,][] agilityTemplates = new int[,][]
                { // Mitigation (Agility)
                    { delicate,     delicate,   delicate,   delicate },
                    { delicate,     polished,   shifting,   delicate },
                    { polished,     subtle,     regal,      delicate },
                    { shifting,     regal,      solid,      delicate },
                };*/

                int[,][] dodgeTemplates = new int[,][]
                { // Mitigation (Dodge)
                    { subtle,       subtle,     subtle,     subtle },
                    { flashing,     resolute,   defender,   subtle },
                    { resolute,     subtle,     regal,      subtle },
                    { defender,     regal,      solid,      subtle },
                };

                int[,][] parryTemplates = new int[,][]
                { // Mitigation (Parry)
                    { flashing,     flashing,   flashing,   flashing },
                    { flashing,     fine,       defender,   flashing },
                    { fine,         subtle,     regal,      flashing },
                    { defender,     regal,      solid,      flashing },
                };

                int[,][] masteryTemplates = new int[,][]
                { // Mitigation (Mastery)
                    { fractured,    fractured,  fractured,  fractured },
                    //{ flashing,     fine,       defender,   fractured },
                    { fine,         fractured,  puissant,   fractured },
                    { defender,     fractured,   puissant,      solid },
                    { fine,     fractured,   puissant,      solid },
                };

                int[,][] threatTemplates = new int[,][]
                { // Threat
                    { bold,         bold,       bold,       bold },
                    { bold,         skillful,   sovereign,  bold },
                    { skillful,     subtle,     regal,      bold },
                    { sovereign,    regal,      solid,      bold },
                };

                int[][,][] gemmingTemplates = new int[][,][]
                //{ survivabilityTemplates, agilityTemplates, dodgeTemplates, parryTemplates, masteryTemplates, threatTemplates };
                { survivabilityTemplates, dodgeTemplates, parryTemplates, masteryTemplates, threatTemplates };

                // Generate List of Gemming Templates
                List<GemmingTemplate> gemmingTemplate = new List<GemmingTemplate>();
                for (int qualityId = 0; qualityId <= qualityGroupNames.GetUpperBound(0); qualityId++)
                {
                    for (int typeId = 0; typeId <= typeGroupNames.GetUpperBound(0); typeId++)
                    {
                        for (int templateId = 0; templateId <= gemmingTemplates[typeId].GetUpperBound(0); templateId++)
                        {
                            for (int metaId = 0; metaId <= metaTemplates.GetUpperBound(0); metaId++)
                            {
                                gemmingTemplate.Add(new GemmingTemplate()
                                {
                                    Model       = "ProtPaladin",
                                    Group       = string.Format("{0} - {1}", qualityGroupNames[qualityId], typeGroupNames[typeId]),
                                    RedId       = gemmingTemplates[typeId][templateId, 0][qualityId],
                                    YellowId    = gemmingTemplates[typeId][templateId, 1][qualityId],
                                    BlueId      = gemmingTemplates[typeId][templateId, 2][qualityId],
                                    PrismaticId = gemmingTemplates[typeId][templateId, 3][qualityId],
                                    MetaId      = metaTemplates[metaId],
                                    Enabled     = qualityGroupNames[qualityId] == "Rare" && typeGroupNames[typeId] != "Threat",
                                });
                            }
                        }
                    }
                }
                return gemmingTemplate;
            }
        }

        private ICalculationOptionsPanel _calculationOptionsPanel = null;
        public override ICalculationOptionsPanel CalculationOptionsPanel
        {
            get {
                if (_calculationOptionsPanel == null) {
                    _calculationOptionsPanel = new CalculationOptionsPanelProtPaladin();
                }
                return _calculationOptionsPanel;
            }
        }

        private string[] _characterDisplayCalculationLabels = null;
        public override string[] CharacterDisplayCalculationLabels {
            get {
                if (_characterDisplayCalculationLabels == null)
                    _characterDisplayCalculationLabels = new string[] {
                    "Base Stats:Health",
                    "Base Stats:Mana",
                    "Base Stats:Strength",
                    "Base Stats:Agility",
                    "Base Stats:Stamina",
                    "Base Stats:Intellect",

                    "Defensive Stats:Armor",
                    "Defensive Stats:Miss",
                    "Defensive Stats:Dodge",
                    "Defensive Stats:Parry",
                    "Defensive Stats:Block",
                    "Defensive Stats:Mastery",
                    "Defensive Stats:Chance to be Crit",
                    "Defensive Stats:Resilience",
                    "Defensive Stats:Block Value",                      
                    "Defensive Stats:Avoidance",
                    "Defensive Stats:Avoidance + Block",
                    "Defensive Stats:Guaranteed Reduction",
                    "Defensive Stats:Total Mitigation",
                    "Defensive Stats:Attacker Speed",
                    "Defensive Stats:Damage Taken",

                    "Offensive Stats:Weapon Speed",
                    "Offensive Stats:Attack Power",
                    "Offensive Stats:Spell Power",
                    "Offensive Stats:Hit",
                    "Offensive Stats:Spell Hit",
                    "Offensive Stats:Physical Haste",
                    "Offensive Stats:Effective Target Armor",
                    "Offensive Stats:Effective Armor Penetration",
                    "Offensive Stats:Crit",
                    "Offensive Stats:Spell Crit",
                    "Offensive Stats:Expertise",
                    "Offensive Stats:Weapon Damage",
                    "Offensive Stats:Missed Attacks",
                    "Offensive Stats:Glancing Attacks",
                    "Offensive Stats:Total Damage/sec",
                    "Offensive Stats:Threat/sec",

                    "Resistances:Nature Resist",
                    "Resistances:Fire Resist",
                    "Resistances:Frost Resist",
                    "Resistances:Shadow Resist",
                    "Resistances:Arcane Resist",
                    "Complex Stats:Ranking Mode*The currently selected ranking mode. Ranking modes can be changed in the Options tab.",
                    @"Complex Stats:Overall Points*Overall Points are a sum of Mitigation, Threat and Survival Points. 
Overall is typically, but not always, the best way to rate gear. 
For specific encounters, closer attention to Mitigation and 
Survival Points individually may be important.
Remember to set your threat scale as needed.",
                    @"Complex Stats:Mitigation Points*Mitigation Points represent the amount of damage you mitigate, 
on average, through armor mitigation and avoidance. It is directly 
relational to your Damage Taken. Ideally, you want to maximize 
Mitigation Points, while maintaining 'enough' Survival Points 
(see Survival Points). If you find yourself dying due to healers 
running OOM, or being too busy healing you and letting other 
raid members die, then focus on Mitigation Points.",
                    @"Complex Stats:Survival Points*Survival Points represents the total raw physical damage 
(pre-avoidance/block) you can take before dying. Unlike 
Mitigation Points, you should not attempt to maximize this, 
but rather get 'enough' of it, and then focus on Mitigation. 
'Enough' can vary greatly by fight and by your healers.
If you find that you are being killed by burst damage,
focus on Survival Points.",
                    @"Complex Stats:Threat Points*Threat Points represents the average threat per second accumulated scaled by the threat scale.",
                    "Complex Stats:Nature Survival",
                    "Complex Stats:Fire Survival",
                    "Complex Stats:Frost Survival",
                    "Complex Stats:Shadow Survival",
                    "Complex Stats:Arcane Survival",
                    };
                return _characterDisplayCalculationLabels;
            }
        }

        private string[] _optimizableCalculationLabels = null;
        public override string[] OptimizableCalculationLabels {
            get {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
                    "Health",
                    "Threat Per Second",
                    "% Total Mitigation",
                    "% Guaranteed Reduction",
                    "Avoidance Points",
                    "% Avoid + Block Attacks",
                    "% Chance to be Crit",
                    "Block Value",
                    "% Block Chance",
                    "Burst Time", 
                    "TankPoints", 
                    "Nature Survival",
                    "Fire Survival",
                    "Frost Survival",
                    "Shadow Survival",
                    "Arcane Survival",
                    "% Spell Hit Chance",
                    "% Chance to be Avoided",
                    "% Chance to be Missed",
                    "% Chance to be Dodged",
                    "% Chance to be Parried",
                    };
                return _optimizableCalculationLabels;
            }
        }

        private string[] _customChartNames = null;
        public override string[] CustomChartNames {
            get {
                if (_customChartNames == null)
                    _customChartNames = new string[] {
                    "Ability Damage",
                    "Ability Threat",
                    "Combat Table: Defensive Stats",
                    "Combat Table: Offensive Stats",
                    "Combat Table: Spell Resistance",
                    "Item Budget",
                    };
                return _customChartNames;
            }
        }

        private Dictionary<string, Color> _subPointNameColors = null;
        public override Dictionary<string, Color> SubPointNameColors {
            get {
                if (_subPointNameColors == null) {
                    _subPointNameColors = new Dictionary<string, Color>();
                    _subPointNameColors.Add("Survivability", Colors.Blue);
                    _subPointNameColors.Add("Mitigation", Colors.Red);
                    //_subPointNameColors.Add("Burst", Colors.Purple); // NYI
                    _subPointNameColors.Add("Threat", Colors.Green);
                }
                return _subPointNameColors;
            }
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Paladin; } }
        #endregion

        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationProtPaladin(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsProtPaladin(); }

        public override ICalculationOptionBase DeserializeDataObject(string xml) {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsProtPaladin));
            StringReader reader = new StringReader(xml);
            CalculationOptionsProtPaladin calcOpts = serializer.Deserialize(reader) as CalculationOptionsProtPaladin;
            return calcOpts;
        }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            // First things first, we need to ensure that we aren't using bad data
            CharacterCalculationsProtPaladin calc = new CharacterCalculationsProtPaladin();
            if (character == null) { return calc; }
            CalculationOptionsProtPaladin calcOpts = character.CalculationOptions as CalculationOptionsProtPaladin;
            if (calcOpts == null) { return calc; }
            BossOptions bossOpts = character.BossOptions;
            // Make sure there is at least one attack in the list.
            // If there's not, add a Default Melee Attack for processing
            if (bossOpts.Attacks.Count < 1) {
                character.IsLoading = true;
                bossOpts.DamagingTargs = true;
                bossOpts.Attacks.Add(BossHandler.ADefaultMeleeAttack);
                character.IsLoading = false;
            }
            // Make sure there is a default melee attack
            // If the above processed, there will be one so this won't have to process
            // If the above didn't process and there isn't one, add one now
            if (bossOpts.DefaultMeleeAttack == null) {
                character.IsLoading = true;
                bossOpts.DamagingTargs = true;
                bossOpts.Attacks.Add(BossHandler.ADefaultMeleeAttack);
                character.IsLoading = false;
            }

            Attack bossAttack = bossOpts.DefaultMeleeAttack;

            StatsProtPaladin stats = GetCharacterStats(character, additionalItem, calcOpts, bossOpts);

            DefendModel dmDisplay = new DefendModel(character, stats, calcOpts, bossOpts);

            // Defensive stats
            calc.Mastery = 8f + StatConversion.GetMasteryFromRating(stats.MasteryRating, CharacterClass.Paladin);
            calc.Miss = dmDisplay.DefendTable.Miss;
            calc.Dodge = dmDisplay.DefendTable.Dodge;
            calc.Parry = dmDisplay.DefendTable.Parry;
            calc.Block = dmDisplay.DefendTable.Block;

            calc.DodgePlusMissPlusParry = calc.Dodge + calc.Miss + calc.Parry;
            calc.DodgePlusMissPlusParryPlusBlock = calc.Dodge + calc.Miss + calc.Parry + calc.Block;
            calc.CritReduction = character.PaladinTalents.Sanctuary * 0.02f;
            calc.CritVulnerability = dmDisplay.DefendTable.Critical;

            stats.Accumulate(GetSpecialEffectStats(character, stats, calcOpts, bossOpts, false));

            DefendModel dm = new DefendModel(character, stats, calcOpts, bossOpts);

            int levelDifference = bossOpts.Level - character.Level;
            if (levelDifference > 3) { levelDifference = 3; }
            else if (levelDifference < 0) { levelDifference = 0; }
            float levelDifferenceAvoidance = levelDifference * 0.002f;

            #region Vengeance
            { // Vengeance Calc from Bear
                // == Evaluate damage taken once ahead of time for vengeance ==
                //Out of 100 attacks, you'll take...
                float critsVeng = Math.Min(Math.Max(0f, 1f - dm.DefendTable.AnyMiss /*calc.AvoidancePostDR*/), (0.05f + levelDifferenceAvoidance) - calc.CappedCritReduction);
                //float crushes = targetLevel == 73 ? Math.Max(0f, Math.Min(15f, 100f - (crits + calculatedStats.AvoidancePreDR)) - stats.CritChanceReduction) : 0f;
                float hitsVeng = Math.Max(0f, 1f - (critsVeng + dm.DefendTable.AnyMiss /*calc.AvoidancePostDR*/));
                //Apply armor and multipliers for each attack type...
                critsVeng *= (1f - calc.GuaranteedReduction) * 2f;
                //crushes *= (100f - calculatedStats.Mitigation) * .015f;
                hitsVeng *= (1f - calc.GuaranteedReduction);
                float damageTakenPercent = (hitsVeng + critsVeng) * (1f - stats.BossAttackSpeedReductionMultiplier);
                float damageTakenPerHit = bossAttack.DamagePerHit * damageTakenPercent - stats.DamageAbsorbed;
                float damageTakenPerSecond = damageTakenPerHit / bossAttack.AttackSpeed;
                float damageTakenPerVengeanceTick = damageTakenPerSecond * 2f;
                float vengeanceCap = stats.Stamina + BaseStats.GetBaseStats(character).Health * 0.1f;
                float vengeanceAPPreAvoidance = Math.Min(vengeanceCap, damageTakenPerVengeanceTick);

                double chanceHit = 1f - dm.DefendTable.AnyMiss /*calc.AvoidancePostDR*/;
                double vengeanceMultiplierFromAvoidance = //Best-fit of results from simulation of avoidance effects on vengeance
                    -46.288470839554d * Math.Pow(chanceHit, 6)
                    + 143.12528411194400d * Math.Pow(chanceHit, 5)
                    - 159.9833254324610000d * Math.Pow(chanceHit, 4)
                    + 74.0451030489808d * Math.Pow(chanceHit, 3)
                    - 10.8422088672455d * Math.Pow(chanceHit, 2)
                    + 0.935157126508557d * chanceHit;

                float vengeanceMultiplierFromSwingSpeed = bossAttack.AttackSpeed <= 2f ? 1f :
                    (1f - 0.1f * (1f - 2f / bossAttack.AttackSpeed)); //A percentage of the ticks will be guaranteed decays for attack speeds longer than 2sec, due to no swings occuring between the current and last tick

                float vengeanceAP = (float)(vengeanceAPPreAvoidance * vengeanceMultiplierFromAvoidance * vengeanceMultiplierFromSwingSpeed);

                calc.AverageVengeanceAP = (float)Math.Floor(vengeanceAP * (1 + stats.BonusAttackPowerMultiplier));
                stats.AverageVengeanceAP += calc.AverageVengeanceAP;
            }//*/
            #endregion

            AttackModel am = new AttackModel(character, stats, calcOpts, bossOpts);

            // Do the 2T13 set bonus
            if (stats.JudgementAbsorbShield > 0)
            {
                Ability judgement = calcOpts.SealChoice == "Seal of Truth" ? Ability.JudgementOfTruth : Ability.JudgementOfRighteousness;
                stats.DamageAbsorbed += stats.JudgementAbsorbShield * am.Abilities[judgement].Damage / 9.0f;
                dm = new DefendModel(character, stats, calcOpts, bossOpts);
            }

            calc.BasicStats = stats;

            // Target Info
            calc.TargetLevel = bossOpts.Level;
            calc.TargetArmor = bossOpts.Armor;
            calc.EffectiveTargetArmor = bossOpts.Armor * (1 - stats.TargetArmorReduction);
            calc.TargetArmorDamageReduction = StatConversion.GetArmorDamageReduction(character.Level, bossOpts.Level, bossOpts.Armor, 0, 0);
            calc.EffectiveTargetArmorDamageReduction = StatConversion.GetArmorDamageReduction(character.Level, bossOpts.Level, bossOpts.Armor, stats.TargetArmorReduction, 0);
            
            calc.ActiveBuffs = new List<Buff>(character.ActiveBuffs);
            calc.Abilities = am.Abilities;

            calc.ArmorReduction = Lookup.ArmorReduction(stats.Armor, calc.TargetLevel, character.Level);
            calc.GuaranteedReduction = dm.GuaranteedReduction;
            calc.TotalMitigation = dm.Mitigation;
            calc.AttackerSpeed = bossOpts.DefaultMeleeAttack.AttackSpeed / (1 - stats.BossAttackSpeedReductionMultiplier);
            calc.DamageTaken = dm.DamageTaken;
            calc.DPSTaken = dm.DamagePerSecond;
            calc.DamageTakenPerHit = dm.DamagePerHit;
            calc.DamageTakenPerBlock = dm.DamagePerBlock;
            calc.DamageTakenPerCrit = dm.DamagePerCrit;
            calc.CappedCritReduction = Math.Min(0.05f + levelDifferenceAvoidance, calc.CritReduction);

            calc.ArcaneReduction = Lookup.MagicReduction(stats, DamageType.Arcane, calc.TargetLevel);
            calc.FireReduction   = Lookup.MagicReduction(stats, DamageType.Fire, calc.TargetLevel);
            calc.FrostReduction  = Lookup.MagicReduction(stats, DamageType.Frost, calc.TargetLevel);
            calc.NatureReduction = Lookup.MagicReduction(stats, DamageType.Nature, calc.TargetLevel);
            calc.ShadowReduction = Lookup.MagicReduction(stats, DamageType.Shadow, calc.TargetLevel);
            calc.ArcaneSurvivalPoints = calc.ArcaneReduction == 0 ? 0 : stats.Health * (calc.ArcaneReduction);
            calc.FireSurvivalPoints = calc.FireReduction == 0 ? 0 : stats.Health * (calc.FireReduction);
            calc.FrostSurvivalPoints = calc.FrostReduction == 0 ? 0 : stats.Health * (calc.FrostReduction);
            calc.NatureSurvivalPoints = calc.NatureReduction == 0 ? 0 : stats.Health * (calc.NatureReduction);
            calc.ShadowSurvivalPoints = calc.ShadowReduction == 0 ? 0 : stats.Health * (calc.ShadowReduction);

            // Offensive Stats
            calc.Hit = Lookup.HitChance(stats, calc.TargetLevel, character.Level);
            calc.SpellHit = Lookup.SpellHitChance(character.Level, stats, calc.TargetLevel);
            calc.Crit = Lookup.CritChance(stats, calc.TargetLevel, character.Level);
            calc.SpellCrit = Lookup.SpellCritChance(character.Level, stats, calc.TargetLevel);
            calc.Expertise = Lookup.BonusExpertisePercentage(stats);
            calc.PhysicalHaste = Lookup.BonusPhysicalHastePercentage(stats);
            calc.SpellHaste = Lookup.BonusSpellHastePercentage(stats);
            calc.AvoidedAttacks = am.Abilities[Ability.MeleeSwing].AttackTable.AnyMiss;
            calc.MissedAttacks = am.Abilities[Ability.MeleeSwing].AttackTable.Miss;
            calc.DodgedAttacks = am.Abilities[Ability.MeleeSwing].AttackTable.Dodge;
            calc.ParriedAttacks = am.Abilities[Ability.MeleeSwing].AttackTable.Parry;
            calc.GlancingAttacks = am.Abilities[Ability.MeleeSwing].AttackTable.Glance;
            calc.GlancingReduction = Lookup.GlancingReduction(character.Level, calc.TargetLevel);
            calc.BlockedAttacks = am.Abilities[Ability.MeleeSwing].AttackTable.Block;
            calc.WeaponSpeed = Lookup.WeaponSpeed(character, stats);
            calc.TotalDamagePerSecond = am.DamagePerSecond;

            // Ranking Points
            //calculatedStats.UnlimitedThreat = am.ThreatPerSecond;
            //am.RageModelMode = RageModelMode.Limited;
            calc.ThreatPerSecond = am.ThreatPerSecond;
            calc.ThreatModel = am.Name + "\n" + am.Description;

            calc.TankPoints = dm.TankPoints;
            calc.BurstTime = dm.BurstTime;
            calc.RankingMode = calcOpts.RankingMode;
            calc.ThreatPoints = calcOpts.ThreatScale * calc.ThreatPerSecond;
            
            //float scale = 0.0f;

            float VALUE_CAP = 1000000000f;

            switch (calcOpts.RankingMode)
            {
                #region Alternative Ranking Modes
                case 1:
                    // Burst Time Mode
                    float threatScale = Convert.ToSingle(Math.Pow(Convert.ToDouble(bossOpts.DefaultMeleeAttack.DamagePerHit) / 25000.0d, 4));
                    calc.SurvivabilityPoints = Math.Min(dm.BurstTime * 100.0f, VALUE_CAP);
                    calc.MitigationPoints = 0.0f;
                    calc.ThreatPoints = 0.0f; // Math.Min((calc.ThreatPoints / threatScale) * 2.0f, VALUE_CAP);
                    break;
                case 2:
                    // Combat Table Coverage Mode
                    calc.SurvivabilityPoints = 0.0f;
                    calc.MitigationPoints = 0.0f;
                    calc.ThreatPoints = 0.0f;
                    //calc.CTCPoints = StatConversion.MitigationScaler / (1f - dm.CTCCovered);
                    calc.CTCPoints = dm.CTCovered * 10000f;
                    calc.MitigationPoints = calc.CTCPoints;
                    calc.OverallPoints = calc.CTCPoints;
                    break;
                case 3:
                    // Damage Output Mode
                    calc.SurvivabilityPoints = 0.0f;
                    calc.MitigationPoints = 0.0f;
                    calc.ThreatPoints = Math.Min(calc.TotalDamagePerSecond, VALUE_CAP);
                    break;
                #endregion
                case 0:
                default:
                    // Mitigation Scale Mode
                    //calc.SurvivalPoints = Math.Min(CapSurvival(dm.EffectiveHealth, calcOpts), VALUE_CAP);
                    //calc.MitigationPoints = Math.Min(calcOpts.MitigationScale / dm.DamageTaken, VALUE_CAP);
                    //calc.ThreatPoints = Math.Min(calc.ThreatPoints, VALUE_CAP);
                    calc.SurvivabilityPoints = CapSurvival(dm.EffectiveHealth, calcOpts, bossOpts);
                    calc.MitigationPoints = StatConversion.MitigationScaler / (1f - dm.Mitigation);
                    //calc.MitigationPoints = Math.Min(dm.Mitigation * bossOpts.DefaultMeleeAttack.DamagePerHit /** calcOpts.MitigationScale*/ * 10.0f, VALUE_CAP);
                    calc.ThreatPoints = Math.Min(calc.ThreatPoints / 10.0f, VALUE_CAP);
                    break;
            }
            calc.OverallPoints = calc.MitigationPoints + calc.SurvivabilityPoints + calc.ThreatPoints;

            return calc;
        }

        // Original code from CalculationsBear, thanks Astrylian!
        private float CapSurvival(float survivalScore, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts)
        {
            double survivalCap = (double)(bossOpts.DefaultMeleeAttack.DamagePerHit * calcOpts.HitsToSurvive) / 1000d;
            double survivalRaw = (double)survivalScore / 1000d;

            //Implement Survival Soft Cap
            if (survivalRaw <= survivalCap) {
                return survivalScore;
            } else {
                double x = survivalRaw;
                double cap = survivalCap;
                double fourToTheNegativeFourThirds = Math.Pow(4d, -4d / 3d);
                double topLeft = Math.Pow(((x - cap) / cap) + fourToTheNegativeFourThirds, 1d / 4d);
                double topRight = Math.Pow(fourToTheNegativeFourThirds, 1d / 4d);
                double fracTop = topLeft - topRight;
                double fraction = fracTop / 2d;
                double y = (cap * fraction + cap);
                return 1000f * (float)y;
            }
        }

        public override Stats GetItemStats(Character character, Item additionalItem)
        {
            CalculationOptionsProtPaladin calcOpts = character.CalculationOptions as CalculationOptionsProtPaladin;
            return GetItemStats(character, additionalItem, calcOpts);
        }

        public Stats GetItemStats(Character character, Item additionalItem, CalculationOptionsProtPaladin options)
        {
            Stats statsItems = base.GetItemStats(character, additionalItem);
            return statsItems;
        }

        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsProtPaladin calcOpts = character.CalculationOptions as CalculationOptionsProtPaladin;
            if (calcOpts == null) calcOpts = new CalculationOptionsProtPaladin();

            BossOptions bossOpts = character.BossOptions;
            if (bossOpts == null) bossOpts = new BossOptions();

            // Make sure there is at least one attack in the list.
            // If there's not, add a Default Melee Attack for processing
            if (bossOpts.Attacks.Count < 1)
            {
                character.IsLoading = true;
                bossOpts.DamagingTargs = true;
                bossOpts.Attacks.Add(BossHandler.ADefaultMeleeAttack);
                character.IsLoading = false;
            }
            // Make sure there is a default melee attack
            // If the above processed, there will be one so this won't have to process
            // If the above didn't process and there isn't one, add one now
            if (bossOpts.DefaultMeleeAttack == null)
            {
                character.IsLoading = true;
                bossOpts.DamagingTargs = true;
                bossOpts.Attacks.Add(BossHandler.ADefaultMeleeAttack);
                character.IsLoading = false;
            }

            return GetCharacterStats(character, additionalItem, calcOpts, bossOpts);
        }

        public StatsProtPaladin GetCharacterStats(Character character, Item additionalItem, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts)
        {
            PaladinTalents talents = character.PaladinTalents;

            Stats statsBase = BaseStats.GetBaseStats(character.Level, CharacterClass.Paladin, character.Race);
            statsBase.Expertise += BaseStats.GetRacialExpertise(character, ItemSlot.MainHand);

            SpecialEffect DivineProtection = new SpecialEffect(Trigger.Use, new Stats { DamageTakenReductionMultiplier = 0.2f }, 10, 60);
            SpecialEffect ArdentDefender = new SpecialEffect(Trigger.Use, new Stats { DamageTakenReductionMultiplier = 0.2f }, 10, 180);
            SpecialEffect GuardianOfAncientKings = new SpecialEffect(Trigger.Use, new Stats { DamageTakenReductionMultiplier = 0.5f }, 12f, 300f);
            SpecialEffect AvengingWrath = new SpecialEffect(Trigger.Use, new Stats { BonusDamageMultiplier = 0.2f }, 20, 180f);

            Stats statsBuffs = GetBuffsStats(character, calcOpts);
            Stats statsItems = GetItemStats(character, additionalItem, calcOpts);
            Stats statsTalents = new Stats()
            {
                BaseArmorMultiplier = talents.Toughness * 0.1f / 3f,
                BonusStaminaMultiplier = 0.15f, // Touched by the Light
                SpellHit = 0.08f,
                DamageTakenReductionMultiplier = talents.Sanctuary * 0.1f / 3f
            };
            StatsProtPaladin statsTotal = new StatsProtPaladin();

            int T11count;
            character.SetBonusCount.TryGetValue("Reinforced Sapphirium Battlearmor", out T11count);
            if (T11count >= 2) { statsTotal.BonusDamageMultiplierCrusaderStrike = 0.05f; }
            if (T11count >= 4) { GuardianOfAncientKings.Duration *= 1.5f; }
            int T12count;
            character.SetBonusCount.TryGetValue("Battlearmor of Immolation", out T12count);
            if (T12count >= 2) { statsTotal.BonusDamageShieldofRighteous = 0.20f; }
            if (T12count >= 4) {
                statsBuffs.AddSpecialEffect(new SpecialEffect(Trigger.DivineProtection,
                    new Stats() { Parry = 0.12f, },
                    10f, 60f));
            }
            int T13count;
            character.SetBonusCount.TryGetValue("Armor of Radiant Glory", out T13count);
            if (T13count >= 2) { statsTotal.JudgementAbsorbShield = 0.25f; }
            // 4-piece affects Divine Guardian, which does not affect the paladin

            GuardianOfAncientKings.Cooldown -= 40f * talents.ShieldOfTheTemplar;
            AvengingWrath.Cooldown -= 20f * talents.ShieldOfTheTemplar;

            // Add paladin-specific tank cooldowns
            statsBase.AddSpecialEffect(GuardianOfAncientKings);
            statsBase.AddSpecialEffect(DivineProtection);
            if (talents.ArdentDefender > 0) statsTalents.AddSpecialEffect(ArdentDefender);
            statsBase.AddSpecialEffect(AvengingWrath);

            statsTotal.Accumulate(statsBase);
            statsTotal.Accumulate(statsItems);
            statsTotal.Accumulate(statsBuffs);
            statsTotal.Accumulate(statsTalents);

            // Base stats: Intellect, stamina, strength, agility
            statsTotal.Intellect = (float)Math.Floor(statsTotal.Intellect * (1.0f + statsTotal.BonusIntellectMultiplier));

            statsTotal.BaseAgility = statsBase.Agility + statsTalents.Agility;

            statsTotal.Stamina = (float)Math.Floor(statsTotal.Stamina
                                    * (1.0f + statsTotal.BonusStaminaMultiplier)
                                    * (Character.ValidateArmorSpecialization(character, ItemType.Plate) ? 1.05f : 1f)); // Plate specialization

            statsTotal.Strength = (float)Math.Floor(statsTotal.Strength * (1.0f + statsTotal.BonusStrengthMultiplier));

            statsTotal.Agility = (float)Math.Floor(statsTotal.Agility * (1.0f + statsTotal.BonusAgilityMultiplier));

            // Armor
            statsTotal.Armor = statsTotal.Armor * (1f + statsTotal.BaseArmorMultiplier);
            statsTotal.BonusArmor = statsTotal.BonusArmor * (1f + statsTotal.BonusArmorMultiplier);
            statsTotal.Armor += statsTotal.BonusArmor;
            statsTotal.Armor = (float)Math.Round(statsTotal.Armor);

            /// Derived stats: Health, mana, spell power, attack power
            statsTotal.Health = (float)Math.Round(statsTotal.Health + StatConversion.GetHealthFromStamina(statsTotal.Stamina));
            statsTotal.Health = (float)Math.Floor(statsTotal.Health * (1f + statsTotal.BonusHealthMultiplier));
            statsTotal.Mana = (float)Math.Round(statsTotal.Mana + StatConversion.GetManaFromIntellect(statsTotal.Intellect));
            statsTotal.Mana = (float)Math.Floor(statsTotal.Mana * (1f + statsTotal.BonusManaMultiplier));
            statsTotal.MaxHealthDamageProc = statsTotal.Health * statsTotal.MaxHealthDamageProc;

            statsTotal.SpellPower += (float)Math.Floor(statsTotal.Strength * 0.60f) + statsTotal.Intellect - 10f; // Touched by the Light
            statsTotal.SpellPower = (float)Math.Floor(statsTotal.SpellPower * (1 + statsTotal.BonusSpellPowerMultiplier));
            statsTotal.AttackPower += statsTotal.Strength * 2f;
            statsTotal.AttackPower = (float)Math.Floor(statsTotal.AttackPower * (1f + statsTotal.BonusAttackPowerMultiplier));
            statsTotal.NatureResistance += statsTotal.NatureResistanceBuff;
            statsTotal.FireResistance += statsTotal.FireResistanceBuff;
            statsTotal.FrostResistance += statsTotal.FrostResistanceBuff;
            statsTotal.ShadowResistance += statsTotal.ShadowResistanceBuff;
            statsTotal.ArcaneResistance += statsTotal.ArcaneResistanceBuff;

            // Ratings stats: Hit, expertise, crit, haste, parry, dodge, mastery
            if (talents.GlyphOfSealOfTruth && calcOpts.SealChoice == "Seal of Truth")
            {
                statsTotal.Expertise += 10.0f;
            }
            statsTotal.ParryRating += (float)Math.Floor((statsTotal.Strength - statsBase.Strength) * 0.27f);
            statsTotal.WeaponDamage += Lookup.WeaponDamage(character, statsTotal.AttackPower, false);

            // Calculate Always-up Procs and Special Effects
            statsTotal.Accumulate(GetSpecialEffectStats(character, statsTotal, calcOpts, bossOpts, true));

            return statsTotal;
        }

        private void CreateTriggers(AttackModel am, Character character, Stats stats, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts, out Dictionary<Trigger, float> triggerIntervals, out Dictionary<Trigger, float> triggerChances)
        {
            triggerIntervals = new Dictionary<Trigger, float>();
            triggerChances = new Dictionary<Trigger, float>();

            float intervalRotation = 9.0f;

            triggerIntervals[Trigger.MeleeAttack] =
            triggerIntervals[Trigger.PhysicalAttack] =
            triggerIntervals[Trigger.MeleeHit] = 
            triggerIntervals[Trigger.PhysicalHit]                        = 
            triggerIntervals[Trigger.MainHandHit]                        =
            triggerIntervals[Trigger.CurrentHandHit]                     =
            triggerIntervals[Trigger.MeleeCrit]                          = 
            triggerIntervals[Trigger.PhysicalCrit]                       = Lookup.WeaponSpeed(character, stats); // + calcOptsTargetsHotR / intervalHotR;
            // 939 has 1 direct damage spell cast in 9 seconds.
            triggerIntervals[Trigger.DamageSpellCast]                    =
            triggerIntervals[Trigger.DamageSpellHit]                     =
            triggerIntervals[Trigger.DamageSpellCrit]                    = 1f / intervalRotation;
            triggerIntervals[Trigger.DoTTick]                            = 1f;
            triggerIntervals[Trigger.SpellCast]                          = 1.5f;
            // 939 assumes casting a spell every gcd. Changing auras, and casting a blessing is disregarded.
            triggerIntervals[Trigger.DamageOrHealingDone]                =
            triggerIntervals[Trigger.DamageDone]                         = 1f / (1f / triggerIntervals[Trigger.MeleeHit] + 1f / triggerIntervals[Trigger.SpellCast]);
            triggerIntervals[Trigger.JudgementHit]                       = 9.0f;
            triggerIntervals[Trigger.DamageTakenPhysical]                =
            triggerIntervals[Trigger.DamageTakenPutsMeBelow35PercHealth] = 
            triggerIntervals[Trigger.DamageTaken]                        = (1.0f / am.AttackerHitsPerSecond);
            triggerIntervals[Trigger.DivineProtection] = 60f;
            triggerIntervals[Trigger.Use] = 0f;

            // temporary combat table, used for the implementation of special effects.
            float hitBonusPhysical = StatConversion.GetPhysicalHitFromRating(stats.HitRating, CharacterClass.Paladin) + stats.PhysicalHit;
            float hitBonusSpell = StatConversion.GetSpellHitFromRating(stats.HitRating, CharacterClass.Paladin) + stats.SpellHit;
            float expertiseBonus = StatConversion.GetDodgeParryReducFromExpertise(StatConversion.GetExpertiseFromRating(stats.ExpertiseRating, CharacterClass.Paladin) + stats.Expertise, CharacterClass.Paladin);
            float chanceMissSpell = Math.Max(0f, StatConversion.GetSpellMiss(character.Level - bossOpts.Level, false) - hitBonusSpell);
            float chanceMissPhysical = Math.Max(0f, StatConversion.WHITE_MISS_CHANCE_CAP[bossOpts.Level - character.Level] - hitBonusPhysical);
            float chanceMissDodge = Math.Max(0f, StatConversion.WHITE_DODGE_CHANCE_CAP[bossOpts.Level - character.Level] - expertiseBonus);
            float chanceMissParry = Math.Max(0f, StatConversion.WHITE_PARRY_CHANCE_CAP[bossOpts.Level - character.Level] - expertiseBonus);
            float chanceMissPhysicalAny = chanceMissPhysical + chanceMissDodge + chanceMissParry;

            triggerChances[Trigger.MeleeAttack] =
            triggerChances[Trigger.MainHandHit] =
            triggerChances[Trigger.CurrentHandHit] =
            triggerChances[Trigger.PhysicalAttack] = 1.0f;
            triggerChances[Trigger.MeleeHit] = 
            triggerChances[Trigger.PhysicalHit] = 1.0f - chanceMissPhysicalAny;
            triggerChances[Trigger.MeleeCrit] = 
            triggerChances[Trigger.PhysicalCrit] = Math.Min(1f, Math.Max(0, StatConversion.GetPhysicalCritFromRating(stats.CritRating, CharacterClass.Paladin)
                                                 + StatConversion.GetPhysicalCritFromAgility(stats.Agility, CharacterClass.Paladin)
                                                 + stats.PhysicalCrit
                                                 + StatConversion.NPC_LEVEL_CRIT_MOD[bossOpts.Level - character.Level]));
            triggerChances[Trigger.DamageSpellCrit] = StatConversion.GetSpellCritFromRating(stats.CritRating, CharacterClass.Paladin)
                                       + StatConversion.GetSpellCritFromIntellect(stats.Intellect, CharacterClass.Paladin)
                                       + stats.SpellCrit + stats.SpellCritOnTarget
                                       - (0.006f * (bossOpts.Level - character.Level) + (bossOpts.Level == 88 ? 0.03f : 0.0f));
            triggerChances[Trigger.DamageSpellHit] = 1.0f - chanceMissSpell;
            triggerChances[Trigger.DoTTick] = triggerChances[Trigger.DamageSpellHit] * (character.PaladinTalents.GlyphOfConsecration ? 1.0f : 16.0f / 18.0f); // 16 ticks in 18 seconds of 9696 rotation. cba with cons. glyph atm.
            triggerChances[Trigger.DamageOrHealingDone] =
            triggerChances[Trigger.DamageDone] = (triggerIntervals[Trigger.MeleeHit] * triggerChances[Trigger.PhysicalHit] + triggerIntervals[Trigger.SpellCast] * triggerChances[Trigger.DamageSpellHit])
                                               / (triggerIntervals[Trigger.MeleeHit] + triggerIntervals[Trigger.SpellCast]);
            triggerChances[Trigger.JudgementHit] =
            triggerChances[Trigger.SpellCast] =
            triggerChances[Trigger.DamageSpellCast] = 
            triggerChances[Trigger.DamageTaken] =
            triggerChances[Trigger.DamageTakenPhysical] = 1f;
            triggerChances[Trigger.DamageTakenPutsMeBelow35PercHealth] = 0.35f;
            triggerChances[Trigger.DivineProtection] = 1f;
            triggerChances[Trigger.Use] = 1f;
        }

        private Stats GetSpecialEffectStats(Character character, StatsProtPaladin stats, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts, bool getConstant)
        {
            StatsProtPaladin statsSpecialEffects = new StatsProtPaladin();

            float weaponSpeed = 1.0f;
            if (character.MainHand != null)
                weaponSpeed = character.MainHand.Speed;

            AttackModel am = new AttackModel(character, stats, calcOpts, bossOpts);

            Dictionary<Trigger, float> triggerIntervals;
            Dictionary<Trigger, float> triggerChances;
            CreateTriggers(am, character, stats, calcOpts, bossOpts, out triggerIntervals, out triggerChances);

            GetSpecialEffectsStats_Child(triggerIntervals, triggerChances, weaponSpeed, calcOpts, bossOpts, stats, getConstant, ref statsSpecialEffects);

            // Darkmoon card greatness & Paragon procs
            // These should always increase strength, which is going to be the Paladin's top stat outside of stamina
            statsSpecialEffects.Strength += statsSpecialEffects.HighestStat + statsSpecialEffects.Paragon;
            statsSpecialEffects.HighestStat = 0;
            statsSpecialEffects.Paragon = 0;
            if (statsSpecialEffects.HighestSecondaryStat > 0) {
                float paragon = statsSpecialEffects.HighestSecondaryStat;
                statsSpecialEffects.HighestSecondaryStat = 0;
                if ((statsSpecialEffects.CritRating > statsSpecialEffects.HasteRating) && (statsSpecialEffects.CritRating > statsSpecialEffects.MasteryRating))
                    statsSpecialEffects.CritRating += paragon;
                else if ((statsSpecialEffects.HasteRating > statsSpecialEffects.CritRating) && (statsSpecialEffects.HasteRating > statsSpecialEffects.MasteryRating))
                    statsSpecialEffects.HasteRating += paragon;
                else
                    statsSpecialEffects.MasteryRating += paragon;
            }

            // Base Stats
            statsSpecialEffects.Stamina = (float)Math.Floor(statsSpecialEffects.Stamina * (1.0f + stats.BonusStaminaMultiplier));
            statsSpecialEffects.Strength = (float)Math.Floor(statsSpecialEffects.Strength * (1.0f + stats.BonusStrengthMultiplier));
            statsSpecialEffects.Agility = (float)Math.Floor(statsSpecialEffects.Agility * (1.0f + stats.BonusAgilityMultiplier));
            statsSpecialEffects.Health += (float)Math.Floor(statsSpecialEffects.Stamina * 10.0f) + (float)Math.Floor(statsSpecialEffects.BattlemasterHealthProc);

            // Defensive Stats
            statsSpecialEffects.Armor = (float)Math.Floor(statsSpecialEffects.Armor * (1f + stats.BaseArmorMultiplier + statsSpecialEffects.BaseArmorMultiplier));
            statsSpecialEffects.BonusArmor = (float)Math.Floor(statsSpecialEffects.BonusArmor * (1.0f + stats.BonusArmorMultiplier + statsSpecialEffects.BonusArmorMultiplier));
            statsSpecialEffects.Armor += statsSpecialEffects.BonusArmor;
 
            // Offensive Stats
            statsSpecialEffects.AttackPower += statsSpecialEffects.Strength * 2.0f;
            statsSpecialEffects.AttackPower = (float)Math.Floor(statsSpecialEffects.AttackPower * (1.0f + stats.BonusAttackPowerMultiplier + statsSpecialEffects.BonusAttackPowerMultiplier));
            statsSpecialEffects.MaxHealthDamageProc = statsSpecialEffects.Health * statsSpecialEffects.MaxHealthDamageProc;

            return statsSpecialEffects;
        }
        private void GetSpecialEffectsStats_Child(Dictionary<Trigger, float> triggerIntervals, Dictionary<Trigger, float> triggerChances,
            float weaponSpeed, CalculationOptionsProtPaladin calcOpts, BossOptions bossOpts, StatsProtPaladin stats, bool getConstant, ref StatsProtPaladin statsSpecialEffects)
        {
            StatsProtPaladin effectsToAdd = new StatsProtPaladin();
            float uptime = 0f;
            foreach (SpecialEffect effect in stats.SpecialEffects()) {
                switch (effect.Trigger) {
                    case Trigger.Use:
                    case Trigger.MeleeAttack:
                    case Trigger.MainHandHit:
                    case Trigger.CurrentHandHit:
                    case Trigger.PhysicalAttack:
                    case Trigger.MeleeHit:
                    case Trigger.PhysicalHit:
                    case Trigger.MeleeCrit:
                    case Trigger.PhysicalCrit:
                    case Trigger.DamageSpellCrit:
                    case Trigger.DamageSpellHit:
                    case Trigger.DoTTick:
                    case Trigger.DamageOrHealingDone: // Need to add Self-Heals
                    case Trigger.DamageDone:
                    case Trigger.JudgementHit:
                    case Trigger.SpellCast:
                    case Trigger.DamageSpellCast:
                    case Trigger.DamageTaken:
                    case Trigger.DamageTakenPhysical:
                    case Trigger.DamageTakenPutsMeBelow35PercHealth:
                    case Trigger.DivineProtection:
                        if (effect.MaxStack != 1) {
                            uptime = effect.GetAverageStackSize(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], weaponSpeed, bossOpts.BerserkTimer);
                        } else {
                            uptime = effect.GetAverageUptime(triggerIntervals[effect.Trigger], triggerChances[effect.Trigger], weaponSpeed, bossOpts.BerserkTimer);
                        }
                        if ((getConstant && uptime >= 1) || (!getConstant && uptime < 1))
                            statsSpecialEffects.Accumulate(effect.Stats, uptime);
                        break;
                }
            }
        }

        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            CharacterCalculationsProtPaladin calculations = GetCharacterCalculations(character) as CharacterCalculationsProtPaladin;

            switch (chartName)
            {
                #region Ability Damage/Threat
                case "Ability Damage":
                case "Ability Threat":
                    {
                        ComparisonCalculationBase[] comparisons = new ComparisonCalculationBase[calculations.Abilities.Count];
                        int j = 0;
                        foreach (var abilities in calculations.Abilities)
                        {
                            AbilityModel ability = (AbilityModel)abilities.Value;
                            ComparisonCalculationProtPaladin comparison = new ComparisonCalculationProtPaladin();

                            comparison.Name = ability.Name;
                            if (chartName == "Ability Damage")
                                comparison.MitigationPoints = ability.Damage;
                            if (chartName == "Ability Threat")
                                comparison.ThreatPoints = ability.Threat;

                            comparison.OverallPoints = comparison.SurvivalPoints + comparison.ThreatPoints + comparison.MitigationPoints;
                            comparisons[j] = comparison;
                            j++;
                        }
                        return comparisons;
                    }
                #endregion
                #region Combat Table: Defensive Stats
                case "Combat Table: Defensive Stats":
                    {
                        ComparisonCalculationProtPaladin calcMiss = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcDodge = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcParry = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcBlock = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcCrit = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcCrush = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcHit = new ComparisonCalculationProtPaladin();
                        if (calculations != null)
                        {
                            calcMiss.Name  = "1 Miss";
                            calcDodge.Name = "2 Dodge";
                            calcParry.Name = "3 Parry";
                            calcBlock.Name = "4 Block";
                            calcCrit.Name  = "5 Crit";
                            calcCrush.Name = "6 Crush";
                            calcHit.Name   = "7 Hit";

                            calcMiss.OverallPoints = calcMiss.MitigationPoints = calculations.Miss * 100.0f;
                            calcDodge.OverallPoints = calcDodge.MitigationPoints = calculations.Dodge * 100.0f;
                            calcParry.OverallPoints = calcParry.MitigationPoints = calculations.Parry * 100.0f;
                            calcBlock.OverallPoints = calcBlock.MitigationPoints = calculations.Block * 100.0f;
                            calcCrit.OverallPoints = calcCrit.SurvivalPoints = calculations.CritVulnerability * 100.0f;
                            calcHit.OverallPoints = calcHit.SurvivalPoints = (1.0f - (calculations.DodgePlusMissPlusParryPlusBlock + calculations.CritVulnerability)) * 100.0f;
                        }
                        return new ComparisonCalculationBase[] { calcMiss, calcDodge, calcParry, calcBlock, calcCrit, calcCrush, calcHit };
                    }
                #endregion
                #region Combat Table: Offensive Stats
                case "Combat Table: Offensive Stats":
                    {
                        ComparisonCalculationProtPaladin calcMiss = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcDodge = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcParry = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcGlance = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcBlock = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcCrit = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcHit = new ComparisonCalculationProtPaladin();
                        if (calculations != null)
                        {
                            calcMiss.Name   = "1 Miss";
                            calcDodge.Name  = "2 Dodge";
                            calcParry.Name  = "3 Parry";
                            calcGlance.Name = "4 Glancing";
                            calcBlock.Name  = "5 Block";
                            calcCrit.Name   = "6 Crit";
                            calcHit.Name    = "7 Hit";

                            calcMiss.OverallPoints = calcMiss.MitigationPoints = calculations.MissedAttacks * 100.0f;
                            calcDodge.OverallPoints = calcDodge.MitigationPoints = calculations.DodgedAttacks * 100.0f;
                            calcParry.OverallPoints = calcParry.MitigationPoints = calculations.ParriedAttacks * 100.0f;
                            calcGlance.OverallPoints = calcGlance.MitigationPoints = calculations.GlancingAttacks * 100.0f;
                            calcBlock.OverallPoints = calcBlock.MitigationPoints = calculations.BlockedAttacks * 100.0f;
                            calcCrit.OverallPoints = calcCrit.SurvivalPoints = calculations.Crit * 100.0f;
                            calcHit.OverallPoints = calcHit.SurvivalPoints = (1.0f - (calculations.AvoidedAttacks + calculations.GlancingAttacks + calculations.BlockedAttacks + calculations.Crit)) * 100.0f;
                        }
                        return new ComparisonCalculationBase[] { calcMiss, calcDodge, calcParry, calcGlance, calcBlock, calcCrit, calcHit };
                    }
                #endregion
                #region Item Budget
                case "Item Budget":
                    CharacterCalculationsProtPaladin calcBaseValue = GetCharacterCalculations(character) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcDodgeValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { DodgeRating = 10f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcParryValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ParryRating = 10f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcBlockValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { BlockRating = 10f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcHasteValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HasteRating = 10f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcExpertiseValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { ExpertiseRating = 10f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcHitValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { HitRating = 10f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcHealthValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Health = (10f * 10f) / 0.667f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcResilValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Resilience = 10f } }) as CharacterCalculationsProtPaladin;
                    CharacterCalculationsProtPaladin calcMasteryValue = GetCharacterCalculations(character, new Item() { Stats = new Stats() { MasteryRating = 10f } }) as CharacterCalculationsProtPaladin;

                    //Differential Calculations for Agi
                    CharacterCalculationsProtPaladin calcAtAdd = calcBaseValue;
                    float agiToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && agiToAdd < 2)
                    {
                        agiToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Agility = agiToAdd } }) as CharacterCalculationsProtPaladin;
                    }

                    CharacterCalculationsProtPaladin calcAtSubtract = calcBaseValue;
                    float agiToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && agiToSubtract > -2)
                    {
                        agiToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Agility = agiToSubtract } }) as CharacterCalculationsProtPaladin;
                    }
                    agiToSubtract += 0.01f;

                    ComparisonCalculationProtPaladin comparisonAgi = new ComparisonCalculationProtPaladin()
                    {
                        Name = "10 Agility",
                        OverallPoints = 10 * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (agiToAdd - agiToSubtract),
                        MitigationPoints = 10 * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (agiToAdd - agiToSubtract),
                        SurvivalPoints = 10 * (calcAtAdd.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints) / (agiToAdd - agiToSubtract),
                        ThreatPoints = 10 * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (agiToAdd - agiToSubtract)
                    };

                    //Differential Calculations for Str
                    calcAtAdd = calcBaseValue;
                    float strToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && strToAdd <= 22)
                    {
                        strToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Strength = strToAdd } }) as CharacterCalculationsProtPaladin;
                    }

                    calcAtSubtract = calcBaseValue;
                    float strToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && strToSubtract >= -22)
                    {
                        strToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Strength = strToSubtract } }) as CharacterCalculationsProtPaladin;
                    }
                    strToSubtract += 0.01f;

                    ComparisonCalculationProtPaladin comparisonStr = new ComparisonCalculationProtPaladin()
                    {
                        Name = "10 Strength",
                        OverallPoints = 10 * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (strToAdd - strToSubtract),
                        MitigationPoints = 10 * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (strToAdd - strToSubtract),
                        SurvivalPoints = 10 * (calcAtAdd.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints) / (strToAdd - strToSubtract),
                        ThreatPoints = 10 * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (strToAdd - strToSubtract)
                    };


                    //Differential Calculations for AC
                    calcAtAdd = calcBaseValue;
                    float acToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && acToAdd < 2)
                    {
                        acToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Armor = acToAdd } }) as CharacterCalculationsProtPaladin;
                    }

                    calcAtSubtract = calcBaseValue;
                    float acToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && acToSubtract > -2)
                    {
                        acToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Armor = acToSubtract } }) as CharacterCalculationsProtPaladin;
                    }
                    acToSubtract += 0.01f;

                    ComparisonCalculationProtPaladin comparisonAC = new ComparisonCalculationProtPaladin()
                    {
                        Name = "100 Armor",
                        OverallPoints = 100f * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (acToAdd - acToSubtract),
                        MitigationPoints = 100f * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (acToAdd - acToSubtract),
                        SurvivalPoints = 100f * (calcAtAdd.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints) / (acToAdd - acToSubtract),
                        ThreatPoints = 100f * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (acToAdd - acToSubtract),
                    };


                    //Differential Calculations for Sta
                    calcAtAdd = calcBaseValue;
                    float staToAdd = 0f;
                    while (calcBaseValue.OverallPoints == calcAtAdd.OverallPoints && staToAdd < 2)
                    {
                        staToAdd += 0.01f;
                        calcAtAdd = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = staToAdd } }) as CharacterCalculationsProtPaladin;
                    }

                    calcAtSubtract = calcBaseValue;
                    float staToSubtract = 0f;
                    while (calcBaseValue.OverallPoints == calcAtSubtract.OverallPoints && staToSubtract > -2)
                    {
                        staToSubtract -= 0.01f;
                        calcAtSubtract = GetCharacterCalculations(character, new Item() { Stats = new Stats() { Stamina = staToSubtract } }) as CharacterCalculationsProtPaladin;
                    }
                    staToSubtract += 0.01f;

                    ComparisonCalculationProtPaladin comparisonSta = new ComparisonCalculationProtPaladin()
                    {
                        Name = "15 Stamina",
                        OverallPoints = (10f / 0.667f) * (calcAtAdd.OverallPoints - calcBaseValue.OverallPoints) / (staToAdd - staToSubtract),
                        MitigationPoints = (10f / 0.667f) * (calcAtAdd.MitigationPoints - calcBaseValue.MitigationPoints) / (staToAdd - staToSubtract),
                        SurvivalPoints = (10f / 0.667f) * (calcAtAdd.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints) / (staToAdd - staToSubtract),
                        ThreatPoints = (10f / 0.667f) * (calcAtAdd.ThreatPoints - calcBaseValue.ThreatPoints) / (staToAdd - staToSubtract)
                    };

                    return new ComparisonCalculationBase[] { 
                        comparisonStr,
                        comparisonAgi,
                        comparisonAC,
                        comparisonSta,
                        new ComparisonCalculationProtPaladin() { Name = "10 Dodge Rating",
                            OverallPoints = (calcDodgeValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcDodgeValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcDodgeValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcDodgeValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "10 Parry Rating",
                            OverallPoints = (calcParryValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcParryValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcParryValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcParryValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "10 Block Rating",
                            OverallPoints = (calcBlockValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcBlockValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcBlockValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcBlockValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "10 Haste Rating",
                            OverallPoints = (calcHasteValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcHasteValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcHasteValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcHasteValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "10 Expertise Rating",
                            OverallPoints = (calcExpertiseValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcExpertiseValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcExpertiseValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcExpertiseValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "10 Hit Rating",
                            OverallPoints = (calcHitValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcHitValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcHitValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcHitValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "150 Health",
                            OverallPoints = (calcHealthValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcHealthValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcHealthValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcHealthValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "10 Resilience",
                            OverallPoints = (calcResilValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcResilValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcResilValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcResilValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                        new ComparisonCalculationProtPaladin() { Name = "10 Mastery Rating",
                            OverallPoints = (calcMasteryValue.OverallPoints - calcBaseValue.OverallPoints), 
                            MitigationPoints = (calcMasteryValue.MitigationPoints - calcBaseValue.MitigationPoints),
                            SurvivalPoints = (calcMasteryValue.SurvivabilityPoints - calcBaseValue.SurvivabilityPoints),
                            ThreatPoints = (calcMasteryValue.ThreatPoints - calcBaseValue.ThreatPoints)},
                    };
                #endregion 
                #region Spell Resistance
                case "Combat Table: Spell Resistance":
                    {
                        ComparisonCalculationProtPaladin calcSpellRes0   = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes10  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes20  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes30  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes40  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes50  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes60  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes70  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes80  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes90  = new ComparisonCalculationProtPaladin();
                        ComparisonCalculationProtPaladin calcSpellRes100 = new ComparisonCalculationProtPaladin();
                        
                        if (calculations != null)
                        {
                            calcSpellRes0.Name   = "0.00";
                            calcSpellRes10.Name  = "0.10";
                            calcSpellRes20.Name  = "0.20";
                            calcSpellRes30.Name  = "0.30";
                            calcSpellRes40.Name  = "0.40";
                            calcSpellRes50.Name  = "0.50";
                            calcSpellRes60.Name  = "0.60";
                            calcSpellRes70.Name  = "0.70";
                            calcSpellRes80.Name  = "0.80";
                            calcSpellRes90.Name  = "0.90";
                            calcSpellRes100.Name = "1.00";

                            calcSpellRes0.OverallPoints   = calcSpellRes0.MitigationPoints = calculations.ResistanceTable[0] * 100.0f;
                            calcSpellRes10.OverallPoints  = calcSpellRes10.MitigationPoints = calculations.ResistanceTable[1] * 100.0f;
                            calcSpellRes20.OverallPoints  = calcSpellRes20.MitigationPoints = calculations.ResistanceTable[2] * 100.0f;
                            calcSpellRes30.OverallPoints  = calcSpellRes30.MitigationPoints = calculations.ResistanceTable[3] * 100.0f;
                            calcSpellRes40.OverallPoints  = calcSpellRes40.MitigationPoints = calculations.ResistanceTable[4] * 100.0f;
                            calcSpellRes50.OverallPoints  = calcSpellRes50.MitigationPoints = calculations.ResistanceTable[5] * 100.0f;
                            calcSpellRes60.OverallPoints  = calcSpellRes60.MitigationPoints = calculations.ResistanceTable[6] * 100.0f;
                            calcSpellRes70.OverallPoints  = calcSpellRes70.MitigationPoints = calculations.ResistanceTable[7] * 100.0f;
                            calcSpellRes80.OverallPoints  = calcSpellRes80.MitigationPoints = calculations.ResistanceTable[8] * 100.0f;
                            calcSpellRes90.OverallPoints  = calcSpellRes90.MitigationPoints = calculations.ResistanceTable[9] * 100.0f;
                            calcSpellRes100.OverallPoints = calcSpellRes100.MitigationPoints = calculations.ResistanceTable[10] * 100.0f;
                        }
                        return new ComparisonCalculationBase[] { calcSpellRes0, calcSpellRes10, calcSpellRes20, calcSpellRes30, calcSpellRes40, calcSpellRes50, calcSpellRes60, calcSpellRes70, calcSpellRes80, calcSpellRes90, calcSpellRes100 };
                    }
                #endregion
                default:
                    return new ComparisonCalculationBase[0];
            }
        }

        #region Relevancy Methods
        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes {
            get {
                if (_relevantItemTypes == null) {
                    _relevantItemTypes = new List<ItemType>(new ItemType[] {
                        ItemType.Plate,
                        ItemType.None,
                        ItemType.Shield,
                        ItemType.Libram,
                        ItemType.Relic,
                        ItemType.OneHandAxe,
                        ItemType.OneHandMace,
                        ItemType.OneHandSword,
                    });
                }
                return _relevantItemTypes;
            }
        }

        private static List<string> _relevantGlyphs;
        public override List<string> GetRelevantGlyphs() {
            if (_relevantGlyphs == null) {
                _relevantGlyphs = new List<string>();
                _relevantGlyphs.Add("Glyph of Crusader Strike");
                _relevantGlyphs.Add("Glyph of Hammer of the Righteous");
                _relevantGlyphs.Add("Glyph of Judgement");
                _relevantGlyphs.Add("Glyph of Seal of Truth");
                _relevantGlyphs.Add("Glyph of Shield of the Righteous");

                _relevantGlyphs.Add("Glyph of Consecration");
                _relevantGlyphs.Add("Glyph of Focused Shield");
            }
            return _relevantGlyphs;
        }

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot) {
            // Filters out Non-Shield Offhand Enchants and Ranged Enchants
            if ((slot == ItemSlot.OffHand && enchant.Slot != ItemSlot.OffHand) || slot == ItemSlot.Ranged) return false;
            // Filters out Death Knight and Two-Hander Enchants
            if (enchant.Name.StartsWith("Rune of the") || enchant.Slot == ItemSlot.TwoHand) return false;

            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique) {
            if (slot == CharacterSlot.OffHand && item.Type != ItemType.Shield) return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }

        private bool IsTriggerRelevant(Trigger trigger)
        {
            return (
                    trigger == Trigger.Use                   || trigger == Trigger.MeleeCrit            ||
                    trigger == Trigger.MeleeHit              || trigger == Trigger.PhysicalCrit         ||
                    trigger == Trigger.PhysicalAttack        || trigger == Trigger.MeleeAttack          ||
                    trigger == Trigger.PhysicalHit           || trigger == Trigger.DoTTick              ||
                    trigger == Trigger.DamageDone            || trigger == Trigger.DamageOrHealingDone  ||
                    trigger == Trigger.JudgementHit          || trigger == Trigger.DamageParried        ||
                    trigger == Trigger.SpellCast             || trigger == Trigger.SpellHit             ||
                    trigger == Trigger.DamageSpellHit        || trigger == Trigger.DamageTaken          ||
                    trigger == Trigger.DamageTakenPhysical   || trigger == Trigger.DivineProtection     ||
                    trigger == Trigger.DamageTakenPutsMeBelow35PercHealth || trigger == Trigger.DamageTakenPutsMeBelow50PercHealth ||
                    trigger == Trigger.MainHandHit           || trigger == Trigger.CurrentHandHit
            );
        }

        public override Stats GetRelevantStats(Stats stats) {
            Stats s = new Stats() {
                Armor = stats.Armor,
                BonusArmor = stats.BonusArmor,
                Stamina = stats.Stamina,
                Agility = stats.Agility,
                DodgeRating = stats.DodgeRating,
                ParryRating = stats.ParryRating,
                BlockRating = stats.BlockRating,
                MasteryRating = stats.MasteryRating,
                Resilience = stats.Resilience,
                BonusAgilityMultiplier = stats.BonusAgilityMultiplier,
                BonusStrengthMultiplier = stats.BonusStrengthMultiplier,
                BonusAttackPowerMultiplier = stats.BonusAttackPowerMultiplier,
                BaseArmorMultiplier = stats.BaseArmorMultiplier,
                BonusArmorMultiplier = stats.BonusArmorMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                Health = stats.Health,
                BattlemasterHealthProc = stats.BattlemasterHealthProc,
                BonusHealthMultiplier = stats.BonusHealthMultiplier,
                DamageTakenReductionMultiplier = stats.DamageTakenReductionMultiplier,
                Miss = stats.Miss,
                ArcaneResistance = stats.ArcaneResistance,
                NatureResistance = stats.NatureResistance,
                FireResistance = stats.FireResistance,
                FrostResistance = stats.FrostResistance,
                ShadowResistance = stats.ShadowResistance,
                ArcaneResistanceBuff = stats.ArcaneResistanceBuff,
                NatureResistanceBuff = stats.NatureResistanceBuff,
                FireResistanceBuff = stats.FireResistanceBuff,
                FrostResistanceBuff = stats.FrostResistanceBuff,
                ShadowResistanceBuff = stats.ShadowResistanceBuff,
                HighestStat = stats.HighestStat,
                HighestSecondaryStat = stats.HighestSecondaryStat,
                Paragon = stats.Paragon,
                MaxHealthDamageProc = stats.MaxHealthDamageProc,

                Strength = stats.Strength,
                AttackPower = stats.AttackPower,
                SpellPower = stats.SpellPower,
                CritRating = stats.CritRating,
                PhysicalCrit = stats.PhysicalCrit,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                HitRating = stats.HitRating,
                PhysicalHit = stats.PhysicalHit,
                SpellHit = stats.SpellHit,
                HasteRating = stats.HasteRating,
                PhysicalHaste = stats.PhysicalHaste,
                ExpertiseRating = stats.ExpertiseRating,
                ArmorPenetration = stats.ArmorPenetration,
                WeaponDamage = stats.WeaponDamage,
                BonusCritDamageMultiplier = stats.BonusCritDamageMultiplier,
                ThreatIncreaseMultiplier = stats.ThreatIncreaseMultiplier,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                BonusWhiteDamageMultiplier = stats.BonusWhiteDamageMultiplier,
                BonusBlockValueMultiplier = stats.BonusBlockValueMultiplier,
                BossPhysicalDamageDealtReductionMultiplier = stats.BossPhysicalDamageDealtReductionMultiplier,
                BossAttackSpeedReductionMultiplier = stats.BossAttackSpeedReductionMultiplier,
                BonusPhysicalDamageMultiplier = stats.BonusPhysicalDamageMultiplier,
                HealthRestoreFromMaxHealth = stats.HealthRestoreFromMaxHealth,

                SnareRootDurReduc = stats.SnareRootDurReduc,
                FearDurReduc = stats.FearDurReduc,
                StunDurReduc = stats.StunDurReduc,
                MovementSpeed = stats.MovementSpeed,
            };
            foreach (SpecialEffect effect in stats.SpecialEffects()) {
                if (IsTriggerRelevant(effect.Trigger) && HasRelevantStats(effect.Stats)) {
                    s.AddSpecialEffect(effect);
                }
            }
            return s;
        }

        public override bool IsItemRelevant(Item item) {
            return base.IsItemRelevant(item);
        }

        public override bool HasRelevantStats(Stats stats) {
            bool relevant = (
                // Basic Stats
                stats.Stamina +
                stats.Health +
                stats.BattlemasterHealthProc + 
                stats.Strength +
                stats.Agility +

                // Tanking Stats
                stats.Armor +
                stats.BonusArmor +
                stats.Block +
                stats.BlockRating +
                stats.MasteryRating +
                stats.Dodge +
                stats.DodgeRating +
                stats.Miss +
                stats.Parry +
                stats.ParryRating +
                stats.Resilience +

                stats.BaseArmorMultiplier +
                stats.BonusArmorMultiplier +
                stats.BonusBlockValueMultiplier +

                // Threat Stats
                stats.AttackPower +
                stats.SpellPower +
                stats.CritRating +
                stats.PhysicalCrit +
                stats.SpellCrit +
                stats.SpellCritOnTarget +
                stats.HasteRating +
                stats.PhysicalHaste +
                stats.SpellHaste +
                stats.HitRating +
                stats.PhysicalHit +
                stats.SpellHit +
                stats.Expertise +
                stats.ExpertiseRating +

                // Special Stats
                stats.HighestStat +
                stats.HighestSecondaryStat +
                stats.Paragon +
                stats.ArcaneDamage +
                stats.ShadowDamage +
                stats.Healed +
                stats.HealthRestoreFromMaxHealth +
                stats.MaxHealthDamageProc +

                // Multiplier Buffs/Debuffs
                stats.BonusStrengthMultiplier +
                stats.BonusAgilityMultiplier +
                stats.BonusStaminaMultiplier +
                stats.BonusHealthMultiplier +
                stats.BossAttackSpeedReductionMultiplier +
                stats.ThreatIncreaseMultiplier +
                stats.BaseArmorMultiplier +
                stats.BonusArmorMultiplier +
                stats.BonusAttackPowerMultiplier +
                stats.BonusDamageMultiplier +
                stats.BonusWhiteDamageMultiplier +
                stats.BonusPhysicalDamageMultiplier +
                stats.BonusHolyDamageMultiplier +
                stats.DamageTakenReductionMultiplier +
                stats.BossPhysicalDamageDealtReductionMultiplier +

                // Resistances
                stats.ArcaneResistance + 
                stats.NatureResistance + 
                stats.FireResistance +
                stats.FrostResistance + 
                stats.ShadowResistance + 
                stats.ArcaneResistanceBuff +
                stats.NatureResistanceBuff + 
                stats.FireResistanceBuff +
                stats.FrostResistanceBuff + 
                stats.ShadowResistanceBuff +

                // BossHandler
                stats.SnareRootDurReduc +
                stats.FearDurReduc +
                stats.StunDurReduc +
                stats.MovementSpeed
                ) != 0;

            foreach (SpecialEffect effect in stats.SpecialEffects()) {
                if (IsTriggerRelevant(effect.Trigger)) {
                    relevant |= HasRelevantStats(effect.Stats);
                }
            }
            return relevant;
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            // Any "Armor" set bonus that applies to Paladins is the protection set bonus
            // Some are "Battlearmor", but the T13 is "Armor", hence the call to ToLower().Contains() instead of BeginsWith()
            if (!String.IsNullOrEmpty(buff.SetName) && buff.AllowedClasses.Contains(character.Class) && buff.SetName.ToLower().Contains("armor"))
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
            }
            return HasRelevantStats(buff.Stats);
        }

        public Stats GetBuffsStats(Character character, CalculationOptionsProtPaladin calcOpts) {
            List<Buff> removedBuffs = new List<Buff>();
            List<Buff> addedBuffs = new List<Buff>();

            //float hasRelevantBuff;

            #region Passive Ability Auto-Fixing
            // NOTE: THIS CODE IS FROM DPSWARR, PROTPALADIN MAY MAKE USE OF IT EVENTUALLY TO HANDLE CONFLICTS LIKE CONCENTRATION AURA
            // Removes the Trueshot Aura Buff and it's equivalents Unleashed Rage and Abomination's Might if you are
            // maintaining it yourself. We are now calculating this internally for better accuracy and to provide
            // value to relevant talents
            /*{
                hasRelevantBuff = character.HunterTalents.TrueshotAura;
                Buff a = Buff.GetBuffByName("Trueshot Aura");
                Buff b = Buff.GetBuffByName("Unleashed Rage");
                Buff c = Buff.GetBuffByName("Abomination's Might");
                if (hasRelevantBuff > 0)
                {
                    if (character.ActiveBuffs.Contains(a)) { character.ActiveBuffs.Remove(a); removedBuffs.Add(a); }
                    if (character.ActiveBuffs.Contains(b)) { character.ActiveBuffs.Remove(b); removedBuffs.Add(b); }
                    if (character.ActiveBuffs.Contains(c)) { character.ActiveBuffs.Remove(c); removedBuffs.Add(c); }
                }
            }
            // Removes the Hunter's Mark Buff and it's Children 'Glyphed', 'Improved' and 'Both' if you are
            // maintaining it yourself. We are now calculating this internally for better accuracy and to provide
            // value to relevant talents
            {
                hasRelevantBuff = character.HunterTalents.ImprovedHuntersMark
                                + (character.HunterTalents.GlyphOfHuntersMark ? 1 : 0);
                Buff a = Buff.GetBuffByName("Hunter's Mark");
                Buff b = Buff.GetBuffByName("Glyphed Hunter's Mark");
                Buff c = Buff.GetBuffByName("Improved Hunter's Mark");
                Buff d = Buff.GetBuffByName("Improved and Glyphed Hunter's Mark");
                // Since we are doing base Hunter's mark ourselves, we still don't want to double-dip
                if (character.ActiveBuffs.Contains(a)) { character.ActiveBuffs.Remove(a); /*removedBuffs.Add(a);*//* }
                // If we have an enhanced Hunter's Mark, kill the Buff
                if (hasRelevantBuff > 0)
                {
                    if (character.ActiveBuffs.Contains(b)) { character.ActiveBuffs.Remove(b); /*removedBuffs.Add(b);*//* }
                    if (character.ActiveBuffs.Contains(c)) { character.ActiveBuffs.Remove(c); /*removedBuffs.Add(c);*//* }
                    if (character.ActiveBuffs.Contains(d)) { character.ActiveBuffs.Remove(d); /*removedBuffs.Add(c);*//* }
                }
            }*/
            #endregion

            Stats statsBuffs = base.GetBuffsStats(character.ActiveBuffs, character.SetBonusCount);

            foreach (Buff b in removedBuffs) {
                character.ActiveBuffsAdd(b);
            }
            foreach (Buff b in addedBuffs)
            {
                character.ActiveBuffs.Remove(b);
            }

            return statsBuffs;
        }
        public override void SetDefaults(Character character) {
            #region Boss Options
            // Never in back of the Boss
            character.BossOptions.InBack = false;

            int avg = character.AvgWornItemLevel;
            int[] points = new int[] { 350, 358, 365 };
            #region Need a Boss Attack
            character.BossOptions.DamagingTargs = true;
            if (character.BossOptions.DefaultMeleeAttack == null) {
                character.BossOptions.Attacks.Add(BossHandler.ADefaultMeleeAttack);
            }
            if        (avg <= points[0]) {
                character.BossOptions.Health = 20000000;
                character.BossOptions.DefaultMeleeAttack.DamagePerHit = BossHandler.StandardMeleePerHit[(int)BossHandler.TierLevels.T11_10];
            } else if (avg <= points[1]) {
                character.BossOptions.Health = 35000000;
                character.BossOptions.DefaultMeleeAttack.DamagePerHit = BossHandler.StandardMeleePerHit[(int)BossHandler.TierLevels.T11_25];
            } else if (avg <= points[2]) {
                character.BossOptions.Health = 50000000;
                character.BossOptions.DefaultMeleeAttack.DamagePerHit = BossHandler.StandardMeleePerHit[(int)BossHandler.TierLevels.T11_10H];
            } else if (avg >  points[2]) {
                character.BossOptions.Health = 65000000;
                character.BossOptions.DefaultMeleeAttack.DamagePerHit = BossHandler.StandardMeleePerHit[(int)BossHandler.TierLevels.T11_25H];
            }
            #endregion
            #endregion
        }
        #endregion
    }
}
