﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Media;

namespace Rawr.Mage
{
    [Rawr.Calculations.RawrModelInfo("Mage", "Spell_Holy_MagicalSentry", CharacterClass.Mage)]
    public sealed class CalculationsMage : CalculationsBase
    {
        private List<GemmingTemplate> _defaultGemmingTemplates = null;
        public override List<GemmingTemplate> DefaultGemmingTemplates
        {
            get
            {
                if (_defaultGemmingTemplates == null)
                {
                    _defaultGemmingTemplates = new List<GemmingTemplate>();
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Rare", true, 52207, 52239, 52208, 52205, 52217, 68780);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Rare (Jewelcrafter)", false, 52257, 52239, 52208, 52205, 52217, 68780);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Epic", false, 71881, 71842, 71850, 71854, 71864, 68780);
                    AddGemmingTemplateGroup(_defaultGemmingTemplates, "Epic (Jewelcrafter)", false, 52257, 71842, 71850, 71854, 71864, 68780);
                    // cogwheels
                    int[] cog = new int[] { 59480, 59479, 59493, 59478 };
                    for (int i = 0; i < cog.Length; i++)
                    {
                        for (int j = i + 1; j < cog.Length; j++)
                        {
                            _defaultGemmingTemplates.Add(new GemmingTemplate() { Model = "Mage", Group = "Engineer", CogwheelId = cog[i], Cogwheel2Id = cog[j], MetaId = 68780, Enabled = false });
                        }
                    }
                }
                return _defaultGemmingTemplates;
            }
        }

        private void AddGemmingTemplateGroup(List<GemmingTemplate> list, string name, bool enabled, int brilliant, int potent, int reckless, int artful, int blue, int meta)
        {
            list.Add(new GemmingTemplate() { Model = "Mage", Group = name, RedId = brilliant, YellowId = brilliant, BlueId = brilliant, PrismaticId = brilliant, MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Mage", Group = name, RedId = brilliant, YellowId = potent, BlueId = blue, PrismaticId = brilliant, MetaId = meta, Enabled = enabled });
            list.Add(new GemmingTemplate() { Model = "Mage", Group = name, RedId = brilliant, YellowId = reckless, BlueId = blue, PrismaticId = brilliant, MetaId = meta, Enabled = enabled });
            if (artful != 0)
            {
                list.Add(new GemmingTemplate() { Model = "Mage", Group = name, RedId = brilliant, YellowId = artful, BlueId = blue, PrismaticId = brilliant, MetaId = meta, Enabled = enabled });
            }
        }

        private static CalculationsMage instance;
        public static CalculationsMage Instance { get { return instance; } }

        public CalculationsMage()
        {
            _subPointNameColorsRating = new Dictionary<string, Color>();
            _subPointNameColorsRating.Add("DPS", Color.FromArgb(255, 0, 128, 255));
            _subPointNameColorsRating.Add("Survivability", Color.FromArgb(255, 64, 128, 32));

            _subPointNameColorsMana = new Dictionary<string, Color>();
            _subPointNameColorsMana.Add("Mana", Color.FromArgb(255, 0, 0, 255));

            _subPointNameColors = _subPointNameColorsRating;

            instance = this;
        }

        private Dictionary<string, Color> _subPointNameColors = null;
        private Dictionary<string, Color> _subPointNameColorsRating = null;
        private Dictionary<string, Color> _subPointNameColorsMana = null;
        public override Dictionary<string, Color> SubPointNameColors
        {
            get
            {
                Dictionary<string, Color> ret = _subPointNameColors;
                _subPointNameColors = _subPointNameColorsRating;
                return ret;
            }
        }

        public override ICalculationOptionBase DeserializeDataObject(string xml)
        {
            xml = xml.Replace("ArcaneBlast00", "ArcaneBlast0");
            xml = xml.Replace("ArcaneBlast11", "ArcaneBlast1");
            xml = xml.Replace("ArcaneBlast22", "ArcaneBlast2");
            xml = xml.Replace("ArcaneBlast33", "ArcaneBlast3");
            System.Xml.Serialization.XmlSerializer serializer =
                new System.Xml.Serialization.XmlSerializer(typeof(CalculationOptionsMage));
            System.IO.StringReader reader = new System.IO.StringReader(xml);
            CalculationOptionsMage calcOpts = serializer.Deserialize(reader) as CalculationOptionsMage;
            return calcOpts;
        }

        private string[] _characterDisplayCalculationLabels = null;
        public override string[] CharacterDisplayCalculationLabels
        {
            get
            {
                if (_characterDisplayCalculationLabels == null)
                    _characterDisplayCalculationLabels = new string[] {
                    "Basic Stats:Stamina",
                    "Basic Stats:Intellect",
                    "Basic Stats:Health",
                    "Basic Stats:Mana",
                    "Spell Stats:Spell Power",
                    "Spell Stats:Haste",
                    "Spell Stats:Hit Chance",
                    "Spell Stats:Penetration",
                    "Spell Stats:Mana Regen",
                    "Spell Stats:Combat Regen",
                    "Spell Stats:Crit Chance",
                    "Spell Stats:Mastery",
                    "Solution:Total Damage",
                    "Solution:Score",
                    "Solution:Dps",
                    "Solution:Tps*Threat per second",
                    "Solution:Spell Cycles",
                    "Solution:By Spell",
                    "Solution:Sequence*Cycle sequence reconstruction based on optimum cycles",
                    "Solution:Minimum Range",
                    "Solution:Threat Reduction",
                    "Spell Info:Wand",
                    "Spell Info:Arcane Missiles",
                    "Spell Info:Arcane Blast(4)*Full debuff stack",
                    "Spell Info:Arcane Blast(0)*Non-debuffed",
                    "Spell Info:Arcane Barrage",
                    "Spell Info:Scorch",
                    "Spell Info:Fire Blast",
                    "Spell Info:Pyroblast*Spammed with refreshed dot (not accounting for Pyroblast! procs)",
                    "Spell Info:Pyroblast!*Including full dot duration",
                    "Spell Info:Fireball",
                    "Spell Info:Living Bomb",
                    "Spell Info:Frostfire Bolt",
                    "Spell Info:Flame Orb",
                    "Spell Info:Combustion",
                    "Spell Info:FBPyro*Pyroblast on Hot Streak",
                    "Spell Info:FBLBPyro*Pyroblast on Hot Streak, maintain Living Bomb dot",
                    "Spell Info:ScLBPyro*Pyroblast on Hot Streak, maintain Living Bomb dot",
                    //"Spell Info:FBScPyro*Maintain Scorch and Pyroblast on Hot Streak",
                    //"Spell Info:FBScLBPyro*Maintain Scorch, maintain Living Bomb dot and Pyroblast on Hot Streak",
                    //"Spell Info:FFBPyro*Pyroblast on Hot Streak",
                    "Spell Info:FFBLBPyro*Pyroblast on Hot Streak, maintain Living Bomb dot",
                    //"Spell Info:FFBScPyro*Maintain Scorch and Pyroblast on Hot Streak",
                    //"Spell Info:FFBScLBPyro*Maintain Scorch, maintain Living Bomb dot and Pyroblast on Hot Streak",
                    "Spell Info:Frostbolt",
                    "Spell Info:Deep Freeze*Displayed values include Fingers of Frost benefit",
                    "Spell Info:Ice Lance",
                    //"Spell Info:FrBFB*Fireball on Brain Freeze",
                    //"Spell Info:FrBIL*Ice Lance on shatter combo",
                    //"Spell Info:FrBDFFBIL*Fireball on non-FOF Brain Freeze, on shatter combo Deep Freeze > BF Fireball > Ice Lance",
                    //"Spell Info:FrBFBIL*Fireball on Brain Freeze, Ice Lance on shatter combo, use Brain Freeze on shatter combo when available",
                    //"Spell Info:FrBILFB*Fireball on Brain Freeze, always Ice Lance on shatter combo",
                    //"Spell Info:FrBDFFFB*Frostfire Bolt on non-FOF Brain Freeze, on shatter combo Deep Freeze > BF Frostfire Bolt",
                    "Spell Info:FrBDFFFBIL*Freeze when FOF off, on FOF Deep Freeze > BF Frostfire Bolt > Ice Lance (IL on 2 charges only or if Freeze coming off cooldown)",
                    "Spell Info:ArcaneManaNeutral*Mana neutral mix of arcane cycles",
                    "Spell Info:ABABar1AM*AB-ABar, wait on ABar cooldown if needed, AM on 1 stack",
                    "Spell Info:AB2ABar12AMABABar*AB-AB-ABar, wait on ABar cooldown if needed, AM-AB-ABar on 1 or 2 stack",
                    "Spell Info:AB2ABar02AMABABar*AB-AB-ABar, wait on ABar cooldown if needed, AM-AB-ABar on 0 or 2 stack",
                    "Spell Info:AB23ABar023AM*AB-AB-(AB-)-ABar, ABar on 2 stack if off cooldown, AM on 0, 2 or 3 stack",
                    "Spell Info:AB3ABar023AM*AB-AB-AB-ABar, AM on 0, 2 or 3 stack",
                    "Spell Info:AB2ABar2AMABar0AMABABar*AB-AB-ABar, AM-ABar if proc at 2 stack, AM-AB-ABar if proc at 0 stack",
                    "Spell Info:ABSpam0234AM[AB]ABar*Spam AB, AM-[AB]-ABar on 0, 2, 3 or 4 stack, AB inserted if ABar cooldown is not ready after AM, ABar only if not on cooldown",
                    "Spell Info:ABSpam0234AMABar*Spam AB, AM-ABar on 0, 2, 3 or 4 stack, ABar only if not on cooldown",
                    "Spell Info:AB3ABar123AM*AB-AB-AB-ABar, AM on 1, 2 or 3 stack",
                    "Spell Info:AB4ABar1234AM*AB-AB-AB-AB-ABar, AM on 1, 2, 3 or 4 stack",
                    "Spell Info:AB4ABar34AM*AB-AB-AB-AB-ABar, AM on 3 or 4 stack",
                    "Spell Info:AB4ABar4AM*AB-AB-AB-AB-ABar, AM on 4 stack",
                    "Spell Info:ABSpam234AM*Spam AB, AM on 2, 3 or 4 stack",
                    "Spell Info:ABSpam34AM*Spam AB, AM on 3 or 4 stack",
                    "Spell Info:ABSpam4AM*Spam AB, AM on 4 stack",
                    "Spell Info:Arcane Explosion",
                    "Spell Info:AERampAB*Cast AB, then AE until you need to refresh, finally at max stack AE until AB debuff runs out (i.e. AB-AEx4-AB-AEx4-AB-AEx4-AB-AEx6)",
                    "Spell Info:AE4AB*Cast AE with 4 stacks of AB, keep refreshing AB (i.e. AEx4-AB)",
                    "Spell Info:Blizzard",
                    "Spell Info:Cone of Cold",
                    "Spell Info:Living Bomb AOE",
                    "Spell Info:FBLB3Pyro*Pyroblast on Hot Streak, maintain Living Bomb on up to 3 targets",
                    "Spell Info:FFBLB3Pyro*Pyroblast on Hot Streak, maintain Living Bomb on up to 3 targets",
                    "Spell Info:Flamestrike*With full dot",
                    "Spell Info:FlamestrikeSpam",
                    "Spell Info:Blast Wave*Requires talent points",
                    "Spell Info:Dragon's Breath*Requires talent points",
                    "Spell Info:Mage Ward*Set incoming fire/frost/arcane damage under survivability settings",
                    "Survivability:Armor",
                    "Survivability:Arcane Resist",
                    "Survivability:Fire Resist",
                    "Survivability:Nature Resist",
                    "Survivability:Frost Resist",
                    "Survivability:Shadow Resist",
                    "Survivability:Physical Mitigation",
                    "Survivability:Resilience",
                    "Survivability:Defense",
                    "Survivability:Crit Reduction",
                    "Survivability:Dodge",
                    "Survivability:Mean Incoming Dps",
                    "Survivability:Chance to Die",
                };
                return _characterDisplayCalculationLabels;
            }
        }

        private AdditiveStat[] _reforgeFrom = new AdditiveStat[] { AdditiveStat.CritRating, AdditiveStat.HasteRating, AdditiveStat.Spirit, AdditiveStat.HitRating, AdditiveStat.MasteryRating };
        private AdditiveStat[] _reforgeTo = new AdditiveStat[] { AdditiveStat.CritRating, AdditiveStat.HasteRating, AdditiveStat.HitRating, AdditiveStat.MasteryRating };

        public override AdditiveStat[] GetStatsToReforgeFrom()
        {
            return _reforgeFrom;
        }

        public override AdditiveStat[] GetStatsToReforgeTo()
        {
            return _reforgeTo;
        }

        private string[] _customChartNames = null;
        public override string[] CustomChartNames
        {
            get
            {
                if (_customChartNames == null)
                    _customChartNames = new string[] { "Item Budget", "Mana Sources", "Mana Usage", "Sequence Reconstruction", "Proc Uptime", "Stats Graph", "Scaling vs Spell Power", "Scaling vs Crit Rating", "Scaling vs Haste Rating", "Scaling vs Intellect", "Scaling vs Mastery Rating" };
                return _customChartNames;
            }
        }

        public override System.Windows.Controls.Control GetCustomChartControl(string chartName)
        {
            switch (chartName)
            {
                case "Stats Graph":
                case "Scaling vs Spell Power":
                case "Scaling vs Crit Rating":
                case "Scaling vs Haste Rating":
                case "Scaling vs Intellect":
                case "Scaling vs Mastery Rating":
                    return Graph.Instance;
                case "Sequence Reconstruction":
                    return Graphs.SequenceReconstructionControl.Instance;
                case "Proc Uptime":
                    return Graphs.ProcUptimeControl.Instance;
                default:
                    return null;
            }
        }

        public override void UpdateCustomChartData(Character character, string chartName, System.Windows.Controls.Control control)
        {
            CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;

            Color[] statColors = new Color[] { Color.FromArgb(255, 255, 0, 0), Color.FromArgb(255, 255, 165, 0), Color.FromArgb(0xFF, 0x80, 0x80, 0x00), Color.FromArgb(255, 154, 205, 50), Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF), Color.FromArgb(255, 0, 0, 255) };

            DisplayCalculations calculations = calculationOptions.Calculations;

            List<float> X = new List<float>();
            List<ComparisonCalculationBase[]> Y = new List<ComparisonCalculationBase[]>();

            Stats[] statsList = new Stats[] {
                        new Stats() { SpellPower = 1.17f },
                        new Stats() { CritRating = 1 },
                        new Stats() { HasteRating = 1 },
                        new Stats() { HitRating = 1 },
                        new Stats() { Intellect = 1 },
                        new Stats() { MasteryRating = 1 },
                    };

            switch (chartName)
            {
                case "Sequence Reconstruction":
                    Graphs.SequenceReconstructionControl.Instance.UpdateGraph(calculationOptions);
                    break;
                case "Proc Uptime":
                    Graphs.ProcUptimeControl.Instance.UpdateGraph(calculations);
                    break;
                case "Stats Graph":
                    Graph.Instance.UpdateStatsGraph(character, statsList, statColors, 100, "", "DPS");
                    break;
                case "Scaling vs Spell Power":
                    Graph.Instance.UpdateScalingGraph(character, statsList, new Stats() { SpellPower = 5 }, true, statColors, 100, "", "DPS");
                    break;
                case "Scaling vs Crit Rating":
                    Graph.Instance.UpdateScalingGraph(character, statsList, new Stats() { CritRating = 5 }, true, statColors, 100, "", "DPS");
                    break;
                case "Scaling vs Haste Rating":
                    Graph.Instance.UpdateScalingGraph(character, statsList, new Stats() { HasteRating = 5 }, true, statColors, 100, "", "DPS");
                    break;
                case "Scaling vs Intellect":
                    Graph.Instance.UpdateScalingGraph(character, statsList, new Stats() { Intellect = 5 }, true, statColors, 100, "", "DPS");
                    break;
                case "Scaling vs Mastery Rating":
                    Graph.Instance.UpdateScalingGraph(character, statsList, new Stats() { MasteryRating = 5 }, true, statColors, 100, "", "DPS");
                    break;
            }
        }

        private CalculationOptionsPanelMage _calculationOptionsPanel = null;
        public override ICalculationOptionsPanel CalculationOptionsPanel { get { return _calculationOptionsPanel ?? (_calculationOptionsPanel = new CalculationOptionsPanelMage()); } }

        private List<ItemType> _relevantItemTypes = null;
        public override List<ItemType> RelevantItemTypes
        {
            get
            {
                if (_relevantItemTypes == null)
                {
                    _relevantItemTypes = new List<ItemType>(new ItemType[]
                    {
                        ItemType.None,
                        ItemType.Cloth,
                        ItemType.Dagger,
                        ItemType.OneHandSword,
                        ItemType.Staff,
                        ItemType.Wand,
                    });
                }
                return _relevantItemTypes;
            }
        }

        public override void SetDefaults(Character character)
        {
            character.ActiveBuffsAdd("Arcane Tactics");
            character.ActiveBuffsAdd("Power Word: Fortitude");
            character.ActiveBuffsAdd("Swift Retribution");
            character.ActiveBuffsAdd("Arcane Brilliance (Mana)");
            character.ActiveBuffsAdd("Blessing of Might (Mp5)");
            character.ActiveBuffsAdd("Totemic Wrath");
            character.ActiveBuffsAdd("Elemental Oath");
            character.ActiveBuffsAdd("Vampiric Touch");
            //if (character.MageTalents.FocusMagic == 1) { character.ActiveBuffsAdd("Focus Magic"); }
            character.ActiveBuffsAdd("Wrath of Air Totem");
            character.ActiveBuffsAdd("Blessing of Kings");
            character.ActiveBuffsAdd("Concentration Aura");
            character.ActiveBuffsAdd("Ebon Plaguebringer");
            character.ActiveBuffsAdd("Critical Mass");
            character.ActiveBuffsAdd("Flask of the Draconic Mind");
            character.ActiveBuffsAdd("Intellect Food");
        }

        public override string GetCharacterStatsString(Character character)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Character:\t\t{0}@{1}-{2}\r\nRace:\t\t{3}",
                character.Name, character.Region, character.Realm, character.Race);

            CalculationOptionsMage CalculationOptions = (CalculationOptionsMage)character.CalculationOptions;

            if (CalculationOptions.Calculations == null || CalculationOptions.Calculations.DisplayCalculationValues == null)
            {
                return base.GetCharacterStatsString(character);
            }

            Dictionary<string, string> dict = CalculationOptions.Calculations.DisplayCalculationValues;
            foreach (KeyValuePair<string, string> kvp in dict)
            {
                string[] value = kvp.Value.Split('*');
                if (value.Length == 2) {
                    sb.AppendFormat("\r\n{0}: {1}\r\n{2}\r\n", kvp.Key, value[0], value[1]);
                } else {
                    sb.AppendFormat("\r\n{0}: {1}", kvp.Key, value[0]);
                }
            }

            return sb.ToString();
        }

        private static Solver advancedSolver;
        private static StringBuilder advancedSolverLog = new StringBuilder();

        public static event EventHandler AdvancedSolverChanged;
        public static event EventHandler AdvancedSolverLogUpdated;

        public static string AdvancedSolverLog
        {
            get
            {
                return advancedSolverLog.ToString();
            }
        }

        public static void CancelAsync()
        {
            Solver solver = advancedSolver;
            if (solver != null)
            {
                solver.CancelAsync();
            }
        }

        public static void ClearLog(Solver solver, string line)
        {
            if (solver == advancedSolver)
            {
                advancedSolverLog.Length = 0;
                advancedSolverLog.AppendLine(line);
                if (AdvancedSolverLogUpdated != null)
                {
                    AdvancedSolverLogUpdated(null, EventArgs.Empty);
                }
            }
        }

        public static void Log(Solver solver, string line)
        {
            if (solver == advancedSolver)
            {
                advancedSolverLog.AppendLine(line);
                if (AdvancedSolverLogUpdated != null)
                {
                    AdvancedSolverLogUpdated(null, EventArgs.Empty);
                }
            }
        }

        public static bool IsSolverEnabled(Solver solver)
        {
            return solver == advancedSolver;
        }

        public static void EnableSolver(Solver solver)
        {
            advancedSolver = solver;
            advancedSolverLog.Length = 0;
            if (AdvancedSolverChanged != null)
            {
                AdvancedSolverChanged(null, EventArgs.Empty);
            }
            if (AdvancedSolverLogUpdated != null)
            {
                AdvancedSolverLogUpdated(null, EventArgs.Empty);
            }
        }

        public static void DisableSolver(Solver solver)
        {
            Interlocked.CompareExchange(ref advancedSolver, null, solver);
            if (AdvancedSolverChanged != null)
            {
                AdvancedSolverChanged(null, EventArgs.Empty);
            }
        }

        public override int MaxDegreeOfParallelism
        {
            get
            {
                return ArrayPool.MaximumPoolSize;
            }
        }

        public override CharacterClass TargetClass { get { return CharacterClass.Mage; } }
        public override ComparisonCalculationBase CreateNewComparisonCalculation() { return new ComparisonCalculationMage(); }
        public override CharacterCalculationsBase CreateNewCharacterCalculations() { return new CharacterCalculationsMage(); }

        public override CharacterCalculationsBase GetCharacterCalculations(Character character, Item additionalItem, bool referenceCalculation, bool significantChange, bool needsDisplayCalculations)
        {
            // First things first, we need to ensure that we aren't using bad data
            CharacterCalculationsMage calcs = new CharacterCalculationsMage();
            if (character == null) { return calcs; }
            CalculationOptionsMage calcOpts = character.CalculationOptions as CalculationOptionsMage;
            if (calcOpts == null) { return calcs; }
            //
            bool computeIncrementalSet = referenceCalculation && calcOpts.IncrementalOptimizations;
            bool useGlobalOptimizations = calcOpts.SmartOptimization && !significantChange;
            bool useIncrementalOptimizations = calcOpts.IncrementalOptimizations && (!significantChange || calcOpts.ForceIncrementalOptimizations);
            bool solveCycles = referenceCalculation || significantChange;
            if (useIncrementalOptimizations && calcOpts.IncrementalSetStateIndexes == null) computeIncrementalSet = true;
            if (computeIncrementalSet)
            {
                useIncrementalOptimizations = false;
                needsDisplayCalculations = true;
            }
            if (useIncrementalOptimizations && !character.DisableBuffAutoActivation)
            {
                return GetCharacterCalculations(character, additionalItem, calcOpts, calcOpts.IncrementalSetArmor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, computeIncrementalSet, solveCycles);
            }
            else if (calcOpts.AutomaticArmor && !calcOpts.ArmorSwapping && !character.DisableBuffAutoActivation)
            {
                CharacterCalculationsMage mage = GetCharacterCalculations(character, additionalItem, calcOpts, "Mage Armor", useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, computeIncrementalSet, solveCycles);
                CharacterCalculationsMage molten = GetCharacterCalculations(character, additionalItem, calcOpts, "Molten Armor", useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, computeIncrementalSet, solveCycles);
                CharacterCalculationsMage calc = (mage.OverallPoints > molten.OverallPoints) ? mage : molten;
                if (calcOpts.MeleeDps + calcOpts.MeleeDot + calcOpts.PhysicalDps + calcOpts.PhysicalDot > 0)
                {
                    CharacterCalculationsMage ice = GetCharacterCalculations(character, additionalItem, calcOpts, "Ice Armor", useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, computeIncrementalSet, solveCycles);
                    if (ice.OverallPoints > calc.OverallPoints) calc = ice;
                }
                if (computeIncrementalSet) StoreIncrementalSet(character, calc.DisplayCalculations);
                if (referenceCalculation) StoreCycleSolutions(character, calc.DisplayCalculations);
                return calc;
            }
            else
            {
                CharacterCalculationsMage calc = GetCharacterCalculations(character, additionalItem, calcOpts, null, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, computeIncrementalSet, solveCycles);
                if (computeIncrementalSet) StoreIncrementalSet(character, calc.DisplayCalculations);
                if (referenceCalculation) StoreCycleSolutions(character, calc.DisplayCalculations);
                return calc;
            }
        }

        private void StoreCycleSolutions(Character character, DisplayCalculations calculations)
        {
            //CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;

            //calculationOptions.FrBDFFFBIL_KDFS = calculations.FrBDFFFBIL_KDFS;
            //calculationOptions.FrBDFFFBIL_KFFB = calculations.FrBDFFFBIL_KFFB;
            //calculationOptions.FrBDFFFBIL_KFFBS = calculations.FrBDFFFBIL_KFFBS;
            //calculationOptions.FrBDFFFBIL_KFrB = calculations.FrBDFFFBIL_KFrB;
            //calculationOptions.FrBDFFFBIL_KILS = calculations.FrBDFFFBIL_KILS;
        }

        private void StoreIncrementalSet(Character character, DisplayCalculations calculations)
        {
            CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;
            List<int> cooldownList = new List<int>();
            List<CycleId> spellList = new List<CycleId>();
            List<int> segmentList = new List<int>();
            List<int> manaSegmentList = new List<int>();
            List<VariableType> variableTypeList = new List<VariableType>();
            List<CycleId> manaNeutralMixList = new List<CycleId>();
            for (int i = 0; i < calculations.SolutionVariable.Count; i++)
            {
                if (calculations.Solution[i] > 0)
                {
                    VariableType type = calculations.SolutionVariable[i].Type;
                    int cooldown;
                    if (calculations.SolutionVariable[i].State != null)
                    {
                        cooldown = calculations.SolutionVariable[i].State.Effects & (int)StandardEffect.NonItemBasedMask;
                    }
                    else
                    {
                        cooldown = 0;
                    }
                    CycleId spellId;
                    if (calculations.SolutionVariable[i].Type == VariableType.Spell)
                    {
                        spellId = calculations.SolutionVariable[i].Cycle.CycleId;
                    }
                    else
                    {
                        spellId = CycleId.None;
                    }
                    if (spellId == CycleId.ArcaneManaNeutral)
                    {
                        if (calculations.SolutionVariable[i].Cycle.Mix1 != CycleId.None && !manaNeutralMixList.Contains(calculations.SolutionVariable[i].Cycle.Mix1))
                        {
                            manaNeutralMixList.Add(calculations.SolutionVariable[i].Cycle.Mix1);
                        }
                        if (calculations.SolutionVariable[i].Cycle.Mix2 != CycleId.None && !manaNeutralMixList.Contains(calculations.SolutionVariable[i].Cycle.Mix2))
                        {
                            manaNeutralMixList.Add(calculations.SolutionVariable[i].Cycle.Mix2);
                        }
                    }
                    int segment = calculations.SolutionVariable[i].Segment;
                    int manaSegment = calculations.SolutionVariable[i].ManaSegment;
                    bool found = false;
                    for (int j = 0; j < cooldownList.Count; j++)
                    {
                        if (cooldownList[j] == cooldown && spellList[j] == spellId && segmentList[j] == segment && manaSegmentList[j] == manaSegment && variableTypeList[j] == type)
                        {
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        cooldownList.Add(cooldown);
                        spellList.Add(spellId);
                        segmentList.Add(segment);
                        manaSegmentList.Add(manaSegment);
                        variableTypeList.Add(type);
                    }
                }
            }
            calculationOptions.IncrementalSetStateIndexes = cooldownList.ToArray();
            calculationOptions.IncrementalSetSpells = spellList.ToArray();
            calculationOptions.IncrementalSetSegments = segmentList.ToArray();
            calculationOptions.IncrementalSetVariableType = variableTypeList.ToArray();
            calculationOptions.IncrementalSetManaSegment = manaSegmentList.ToArray();
            calculationOptions.IncrementalSetManaNeutralMix = manaNeutralMixList.ToArray();
            if (calculationOptions.AutomaticArmor)
            {
                calculationOptions.IncrementalSetArmor = calculations.MageArmor;
            }
            else
            {
                calculationOptions.IncrementalSetArmor = null;
            }

            List<int> filteredCooldowns = ListUtils.RemoveDuplicates(cooldownList);
            filteredCooldowns.Sort();
            calculationOptions.IncrementalSetSortedStates = filteredCooldowns.ToArray();
        }

        public CharacterCalculationsMage GetCharacterCalculations(Character character, Item additionalItem, CalculationOptionsMage calculationOptions, string armor, bool useIncrementalOptimizations, bool useGlobalOptimizations, bool needsDisplayCalculations, bool needsSolutionVariables, bool solveCycles)
        {
            return Solver.GetCharacterCalculations(character, additionalItem, calculationOptions, this, armor, calculationOptions.ComparisonSegmentCooldowns, calculationOptions.ComparisonSegmentMana, calculationOptions.ComparisonIntegralMana, calculationOptions.ComparisonAdvancedConstraintsLevel, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables, solveCycles, calculationOptions.SimpleStacking, calculationOptions.SimpleStacking);
        }

        //public static readonly Buff CriticalMassBuff = Buff.GetBuffByName("Critical Mass");
        //public static readonly Buff ArcaneTacticsBuff = Buff.GetBuffByName("Arcane Tactics");
        public static readonly Buff MoltenArmorBuff = Buff.GetBuffByName("Molten Armor");
        public static readonly Buff MageArmorBuff = Buff.GetBuffByName("Mage Armor");
        public static readonly Buff IceArmorBuff = Buff.GetBuffByName("Ice Armor");

        public void AccumulateRawStats(Stats stats, Character character, Item additionalItem, CalculationOptionsMage calculationOptions, out List<Buff> autoActivatedBuffs, string armor, out List<Buff> activeBuffs)
        {
            AccumulateItemStats(stats, character, additionalItem);

            bool activeBuffsCloned = false;
            activeBuffs = character.ActiveBuffs;
            autoActivatedBuffs = null;

            if (!character.DisableBuffAutoActivation)
            {
                MageTalents talents = character.MageTalents;
                /*if (talents.CriticalMass > 0)
                {
                    if (!character.ActiveBuffs.Contains(ImprovedScorchBuff) && !character.ActiveBuffs.Contains(WintersChillBuff))
                    {
                        if (!activeBuffsCloned)
                        {
                            activeBuffs = new List<Buff>(character.ActiveBuffs);
                            autoActivatedBuffs = new List<Buff>();
                            activeBuffsCloned = true;
                        }
                        activeBuffs.Add(ImprovedScorchBuff);
                        autoActivatedBuffs.Add(ImprovedScorchBuff);
                        RemoveConflictingBuffs(activeBuffs, ImprovedScorchBuff);
                    }
                }*/
                if (armor != null)
                {
                    Buff armorBuff = null;
                    switch (armor)
                    {
                        case "Molten Armor":
                            armorBuff = MoltenArmorBuff;
                            break;
                        case "Mage Armor":
                            armorBuff = MageArmorBuff;
                            break;
                        case "Ice Armor":
                            armorBuff = IceArmorBuff;
                            break;
                    }
                    if (!character.ActiveBuffs.Contains(armorBuff))
                    {
                        if (!activeBuffsCloned)
                        {
                            activeBuffs = new List<Buff>(character.ActiveBuffs);
                            autoActivatedBuffs = new List<Buff>();
                            activeBuffsCloned = true;
                        }
                        activeBuffs.Add(armorBuff);
                        autoActivatedBuffs.Add(armorBuff);
                        RemoveConflictingBuffs(activeBuffs, armorBuff);
                    }
                }
            }

            AccumulateBuffsStats(stats, activeBuffs);

            // moving away to handling these as normal procs, reevaluate if needed
            /*for (int i = 0; i < stats._rawSpecialEffectDataSize; i++)
            {
                SpecialEffect effect = stats._rawSpecialEffectData[i];
                if (effect.MaxStack > 1 && effect.Chance == 1f && effect.Cooldown == 0f && (effect.Trigger == Trigger.DamageSpellCast || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellHit))
                {
                    effect.Stats.GenerateSparseData();
                    stats.Accumulate(effect.Stats, effect.MaxStack);
                }                
            }*/
        }

        // required by base class, but never used
        public override Stats GetCharacterStats(Character character, Item additionalItem)
        {
            CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;
            List<Buff> ignore;
            List<Buff> ignore2;
            Stats stats = new Stats();
            AccumulateRawStats(stats, character, additionalItem, calculationOptions, out ignore2, null, out ignore);
            return GetCharacterStats(character, additionalItem, stats, calculationOptions, true);
        }

        public Stats GetCharacterStats(Character character, Item additionalItem, Stats rawStats, CalculationOptionsMage calculationOptions, bool includeArmor)
        {
            float statsRaceHealth;
            float statsRaceMana;
            float statsRaceStrength;
            float statsRaceAgility;
            float statsRaceStamina;
            float statsRaceIntellect;
            float statsRaceSpirit;
            switch (calculationOptions.PlayerLevel)
            {
                case 70:
                    statsRaceHealth = 3213f;
                    statsRaceMana = 1961f;
                    switch (character.Race)
                    {
                        case CharacterRace.BloodElf:
                            statsRaceStrength = 28f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 49f;
                            statsRaceIntellect = 149f;
                            statsRaceSpirit = 144;
                            break;
                        case CharacterRace.Draenei:
                            statsRaceStrength = 28f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 50f;
                            statsRaceIntellect = 152f;
                            statsRaceSpirit = 147;
                            break;
                        case CharacterRace.Human:
                            statsRaceStrength = 28f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 51f;
                            statsRaceIntellect = 151f;
                            statsRaceSpirit = 145;
                            break;
                        case CharacterRace.Troll:
                            statsRaceStrength = 28f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 52f;
                            statsRaceIntellect = 147f;
                            statsRaceSpirit = 146;
                            break;
                        case CharacterRace.Undead:
                            statsRaceStrength = 28f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 52f;
                            statsRaceIntellect = 149f;
                            statsRaceSpirit = 150;
                            break;
                        case CharacterRace.Gnome:
                        default:
                            statsRaceStrength = 28f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 50f;
                            statsRaceIntellect = 154f;
                            statsRaceSpirit = 145;
                            break;
                    }
                    break;
                case 71:
                    statsRaceHealth = 3308f;
                    statsRaceMana = 2063f;
                    statsRaceStrength = 28f;
                    statsRaceAgility = 42f;
                    statsRaceStamina = 51f;
                    statsRaceIntellect = 157f;
                    statsRaceSpirit = 148f;
                    break;
                case 72:
                    statsRaceHealth = 3406f;
                    statsRaceMana = 2166f;
                    statsRaceStrength = 28f;
                    statsRaceAgility = 43f;
                    statsRaceStamina = 52f;
                    statsRaceIntellect = 160f;
                    statsRaceSpirit = 151f;
                    break;
                case 73:
                    statsRaceHealth = 3505f;
                    statsRaceMana = 2269f;
                    statsRaceStrength = 28f;
                    statsRaceAgility = 43f;
                    statsRaceStamina = 53f;
                    statsRaceIntellect = 163f;
                    statsRaceSpirit = 154f;
                    break;
                case 74:
                    statsRaceHealth = 3623f;
                    statsRaceMana = 2372f;
                    statsRaceStrength = 29f;
                    statsRaceAgility = 44f;
                    statsRaceStamina = 53f;
                    statsRaceIntellect = 166f;
                    statsRaceSpirit = 156f;
                    break;
                case 75:
                    statsRaceHealth = 3726f;
                    statsRaceMana = 2474f;
                    statsRaceStrength = 29f;
                    statsRaceAgility = 44f;
                    statsRaceStamina = 54f;
                    statsRaceIntellect = 169f;
                    statsRaceSpirit = 159f;
                    break;
                case 76:
                    statsRaceHealth = 3830f;
                    statsRaceMana = 2577f;
                    statsRaceStrength = 29f;
                    statsRaceAgility = 44f;
                    statsRaceStamina = 55f;
                    statsRaceIntellect = 172f;
                    statsRaceSpirit = 162f;
                    break;
                case 77:
                    statsRaceHealth = 3937f;
                    statsRaceMana = 2680f;
                    statsRaceStrength = 30f;
                    statsRaceAgility = 45f;
                    statsRaceStamina = 56f;
                    statsRaceIntellect = 175f;
                    statsRaceSpirit = 165f;
                    break;
                case 78:
                    statsRaceHealth = 4063f;
                    statsRaceMana = 2783f;
                    statsRaceStrength = 30f;
                    statsRaceAgility = 45f;
                    statsRaceStamina = 56f;
                    statsRaceIntellect = 178f;
                    statsRaceSpirit = 168f;
                    break;
                case 79:
                    statsRaceHealth = 4172f;
                    statsRaceMana = 2885f;
                    statsRaceStrength = 30f;
                    statsRaceAgility = 46f;
                    statsRaceStamina = 57f;
                    statsRaceIntellect = 181f;
                    statsRaceSpirit = 171f;
                    break;
                case 80:
                default:
                    statsRaceHealth = 6783f;
                    statsRaceMana = 2988f;
                    switch (character.Race)
                    {
                        case CharacterRace.BloodElf:
                            statsRaceStrength = 33f;
                            statsRaceAgility = 45f;
                            statsRaceStamina = 57f;
                            statsRaceIntellect = 185f;
                            statsRaceSpirit = 173f;
                            break;
                        case CharacterRace.Draenei:
                            statsRaceStrength = 37f;
                            statsRaceAgility = 40f;
                            statsRaceStamina = 58f;
                            statsRaceIntellect = 182f;
                            statsRaceSpirit = 176f;
                            break;
                        case CharacterRace.Human:
                            statsRaceStrength = 36f;
                            statsRaceAgility = 43f;
                            statsRaceStamina = 59f;
                            statsRaceIntellect = 181f;
                            statsRaceSpirit = 174f;
                            break;
                        case CharacterRace.Troll:
                            statsRaceStrength = 37f;
                            statsRaceAgility = 45f;
                            statsRaceStamina = 60f;
                            statsRaceIntellect = 177f;
                            statsRaceSpirit = 175f;
                            break;
                        case CharacterRace.Undead:
                            statsRaceStrength = 35f;
                            statsRaceAgility = 41f;
                            statsRaceStamina = 60f;
                            statsRaceIntellect = 179f;
                            statsRaceSpirit = 179f;
                            break;
                        case CharacterRace.Gnome:
                        default:
                            statsRaceStrength = 31f;
                            statsRaceAgility = 45f;
                            statsRaceStamina = 59f;
                            statsRaceIntellect = 184f;
                            statsRaceSpirit = 174f;
                            break;
                    }
                    break;
                case 81:
                    statsRaceHealth = 9551f;
                    statsRaceMana = 4287f;
                    statsRaceStrength = 31f;
                    statsRaceAgility = 46f;
                    statsRaceStamina = 60f;
                    statsRaceIntellect = 188f;
                    statsRaceSpirit = 177f;
                    break;
                case 82:
                    statsRaceHealth = 13418f;
                    statsRaceMana = 6102f;
                    statsRaceStrength = 31f;
                    statsRaceAgility = 46f;
                    statsRaceStamina = 60f;
                    statsRaceIntellect = 191f;
                    statsRaceSpirit = 180f;
                    break;
                case 83:
                    statsRaceHealth = 13418f;
                    statsRaceMana = 6102f;
                    statsRaceStrength = 31f;
                    statsRaceAgility = 46f;
                    statsRaceStamina = 60f;
                    statsRaceIntellect = 191f;
                    statsRaceSpirit = 180f;
                    break;
                case 84:
                    statsRaceHealth = 13418f;
                    statsRaceMana = 6102f;
                    statsRaceStrength = 31f;
                    statsRaceAgility = 46f;
                    statsRaceStamina = 60f;
                    statsRaceIntellect = 191f;
                    statsRaceSpirit = 180f;
                    break;
                case 85:
                    statsRaceHealth = 36853f;
                    statsRaceMana = 17138f;
                    if (calculationOptions.ModeMOP)
                    {
                        statsRaceMana = 100000f;
                    }
                    switch (character.Race)
                    {
                        case CharacterRace.NightElf:
                            statsRaceStrength = 33f;
                            statsRaceAgility = 50f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 198f;
                            statsRaceSpirit = 190f;
                            break;
                        case CharacterRace.BloodElf:
                            statsRaceStrength = 34f;
                            statsRaceAgility = 48f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 201f;
                            statsRaceSpirit = 188f;
                            break;
                        case CharacterRace.Draenei:
                            statsRaceStrength = 38f;
                            statsRaceAgility = 43f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 198f;
                            statsRaceSpirit = 192f;
                            break;
                        case CharacterRace.Dwarf:
                            statsRaceStrength = 42f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 64f;
                            statsRaceIntellect = 197f;
                            statsRaceSpirit = 189f;
                            break;
                        case CharacterRace.Goblin:
                            statsRaceStrength = 34f;
                            statsRaceAgility = 48f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 201f;
                            statsRaceSpirit = 188f;
                            break;
                        case CharacterRace.Human:
                            statsRaceStrength = 37f;
                            statsRaceAgility = 46f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 198f;
                            statsRaceSpirit = 190f;
                            break;
                        case CharacterRace.Orc:
                            statsRaceStrength = 40f;
                            statsRaceAgility = 43f;
                            statsRaceStamina = 64f;
                            statsRaceIntellect = 195f;
                            statsRaceSpirit = 192f;
                            break;
                        case CharacterRace.Tauren:
                            statsRaceStrength = 42f;
                            statsRaceAgility = 42f;
                            statsRaceStamina = 64f;
                            statsRaceIntellect = 194f;
                            statsRaceSpirit = 192f;
                            break;
                        case CharacterRace.Troll:
                            statsRaceStrength = 38f;
                            statsRaceAgility = 48f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 194f;
                            statsRaceSpirit = 191f;
                            break;
                        case CharacterRace.Undead:
                            statsRaceStrength = 36f;
                            statsRaceAgility = 44f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 196f;
                            statsRaceSpirit = 195f;
                            break;
                        case CharacterRace.Worgen:
                            statsRaceStrength = 40f;
                            statsRaceAgility = 48f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 194f;
                            statsRaceSpirit = 189f;
                            break;
                        case CharacterRace.Gnome:
                        default:
                            statsRaceStrength = 32f;
                            statsRaceAgility = 48f;
                            statsRaceStamina = 63f;
                            statsRaceIntellect = 201f;
                            statsRaceSpirit = 190f;
                            break;
                    }
                    break;
            }
            MageTalents talents = character.MageTalents;

            float statsRaceBonusIntellectMultiplier = 0.0f;
            float statsRaceBonusManaMultiplier = 1.0f + rawStats.BonusManaMultiplier;
            if (character.Race == CharacterRace.Gnome)
            {
                statsRaceBonusManaMultiplier *= 1.05f;
            }
            float statsTalentBonusIntellectMultiplier = 0f;
            float statsRaceBonusSpiritMultiplier = 0.0f;
            if (character.Race == CharacterRace.Human)
            {
                statsRaceBonusSpiritMultiplier = 0.03f;
            }

            float statsWizardryBonusIntellectMultiplier = 0.0f;
            if (calculationOptions.PlayerLevel >= 50)
            {
                statsWizardryBonusIntellectMultiplier = 0.05f;
            }

            Stats statsTotal = rawStats;

            float statsTalentBonusSpiritMultiplier = 0.0f;
            if (calculationOptions.EffectSpiritMultiplier != 1.0f)
            {
                statsTotal.BonusSpiritMultiplier = (1 + statsTotal.BonusSpiritMultiplier) * calculationOptions.EffectSpiritMultiplier - 1;
            }
            statsTotal.Strength = (float)Math.Round((statsRaceStrength + statsTotal.Strength) * (1 + statsTotal.BonusStrengthMultiplier) - 0.00001);
            statsTotal.Agility = (float)Math.Round((statsRaceAgility + statsTotal.Agility) * (1 + statsTotal.BonusAgilityMultiplier) - 0.00001);
            statsTotal.Intellect = (float)Math.Round((Math.Floor(0.00001 + statsRaceIntellect * (1 + statsRaceBonusIntellectMultiplier) * (1 + statsWizardryBonusIntellectMultiplier) * (1 + statsTalentBonusIntellectMultiplier)) + Math.Floor(0.00001 + statsTotal.Intellect * (1 + statsRaceBonusIntellectMultiplier) * (1 + statsWizardryBonusIntellectMultiplier) * (1 + statsTalentBonusIntellectMultiplier))) * (1 + statsTotal.BonusIntellectMultiplier) - 0.00001);
            statsTotal.Stamina = (float)Math.Round((statsRaceStamina + statsTotal.Stamina) * (1 + statsTotal.BonusStaminaMultiplier) - 0.00001);
            statsTotal.Spirit = (float)Math.Round((Math.Floor(0.00001 + statsRaceSpirit * (1 + statsRaceBonusSpiritMultiplier) * (1 + statsTalentBonusSpiritMultiplier)) + Math.Floor(0.00001 + statsTotal.Spirit * (1 + statsRaceBonusSpiritMultiplier) * (1 + statsTalentBonusSpiritMultiplier))) * (1 + statsTotal.BonusSpiritMultiplier) - 0.00001);

            statsTotal.Health = (float)Math.Round((statsTotal.Health + statsRaceHealth + (statsTotal.Stamina * 14f)) * (character.Race == CharacterRace.Tauren ? 1.05f : 1f) * (1 + statsTotal.BonusHealthMultiplier));
            if (calculationOptions.ModeMOP)
            {
                statsTotal.Mana = (float)Math.Round((statsTotal.Mana) * statsRaceBonusManaMultiplier);
            }
            else
            {
                statsTotal.Mana = (float)Math.Round((statsTotal.Mana + statsRaceMana + 15f * statsTotal.Intellect) * statsRaceBonusManaMultiplier);
            }
            statsTotal.Armor = (float)Math.Round(statsTotal.Armor);

            statsTotal.BonusIntellectMultiplier = (1 + statsRaceBonusIntellectMultiplier) * (1 + statsWizardryBonusIntellectMultiplier) * (1 + statsTalentBonusIntellectMultiplier) * (1 + statsTotal.BonusIntellectMultiplier) - 1;
            statsTotal.BonusManaMultiplier = statsRaceBonusManaMultiplier - 1;

            if (character.Race == CharacterRace.BloodElf)
            {
                statsTotal.Mp5 += 5 * 0.06f * statsTotal.Mana / 120;
            }
            if (character.Race == CharacterRace.Draenei)
            {
                statsTotal.SpellHit += 0.01f;
            }

            float allResist = 0;
            if (includeArmor)
            {
                if (statsTotal.MageMageArmor > 0)
                {
                    statsTotal.ManaRestoreFromMaxManaPerSecond += 0.006f * (talents.GlyphOfMageArmor ? 1.2f : 1.0f);
                    allResist += calculationOptions.GetSpellValueRound(0.048f);
                }
                if (statsTotal.MageMoltenArmor > 0)
                {
                    statsTotal.SpellCrit += 0.03f + (talents.GlyphOfMoltenArmor ? 0.02f : 0.0f);
                }
            }
            if (calculationOptions.EffectCritBonus > 0)
            {
                statsTotal.SpellCrit += calculationOptions.EffectCritBonus;
            }
            if (character.Race == CharacterRace.Worgen)
            {
                statsTotal.SpellCrit += 0.01f;
            }
            if (character.Race == CharacterRace.Goblin)
            {
                statsTotal.SpellHaste = (1f + statsTotal.SpellHaste) * 1.01f - 1f;
            }
            /*if (talents.GlyphOfManaGem)
            {
                statsTotal.BonusManaGem += 0.4f;
            }*/

            //statsTotal.Mp5 += calculationOptions.ShadowPriest;

            float spellDamageFromIntellectPercentage = 0f;

            statsTotal.SpellPower += (float)Math.Floor(spellDamageFromIntellectPercentage * statsTotal.Intellect);
            statsTotal.SpellPower += statsTotal.Intellect - 10;
            //statsTotal.SpellPower += spellDamageFromSpiritPercentage * statsTotal.Spirit;

            statsTotal.CritBonusDamage += calculationOptions.EffectCritDamageBonus;

            statsTotal.ArcaneResistance += allResist + statsTotal.ArcaneResistanceBuff;
            statsTotal.FireResistance += allResist + statsTotal.FireResistanceBuff;
            statsTotal.FrostResistance += allResist + statsTotal.FrostResistanceBuff;
            statsTotal.NatureResistance += allResist + statsTotal.NatureResistanceBuff;
            statsTotal.ShadowResistance += allResist + statsTotal.ShadowResistanceBuff;

            int playerLevel = calculationOptions.PlayerLevel;
            float maxHitRate = 1.0f;
            float bossHitRate = Math.Min(maxHitRate, ((playerLevel <= calculationOptions.TargetLevel + 2) ? (0.96f - (playerLevel - calculationOptions.TargetLevel) * 0.01f) : (0.94f - (playerLevel - calculationOptions.TargetLevel - 2) * 0.11f)));
            statsTotal.Mp5 -= 5 * calculationOptions.EffectShadowManaDrain * calculationOptions.EffectShadowManaDrainFrequency * bossHitRate * (1 - StatConversion.GetAverageResistance(calculationOptions.TargetLevel, calculationOptions.PlayerLevel, statsTotal.ShadowResistance, 0));

            float fullResistRate = calculationOptions.EffectArcaneOtherBinary * (1 - bossHitRate * (1 - StatConversion.GetAverageResistance(calculationOptions.TargetLevel, calculationOptions.PlayerLevel, statsTotal.ArcaneResistance, 0)));
            fullResistRate += calculationOptions.EffectFireOtherBinary * (1 - bossHitRate * (1 - StatConversion.GetAverageResistance(calculationOptions.TargetLevel, calculationOptions.PlayerLevel, statsTotal.FireResistance, 0)));
            fullResistRate += calculationOptions.EffectFrostOtherBinary * (1 - bossHitRate * (1 - StatConversion.GetAverageResistance(calculationOptions.TargetLevel, calculationOptions.PlayerLevel, statsTotal.FrostResistance, 0)));
            fullResistRate += calculationOptions.EffectShadowOtherBinary * (1 - bossHitRate * (1 - StatConversion.GetAverageResistance(calculationOptions.TargetLevel, calculationOptions.PlayerLevel, statsTotal.ShadowResistance, 0)));
            fullResistRate += calculationOptions.EffectNatureOtherBinary * (1 - bossHitRate * (1 - StatConversion.GetAverageResistance(calculationOptions.TargetLevel, calculationOptions.PlayerLevel, statsTotal.NatureResistance, 0)));
            fullResistRate += calculationOptions.EffectHolyOtherBinary * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectArcaneOther * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectFireOther * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectFrostOther * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectShadowOther * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectNatureOther * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectHolyOther * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectShadowManaDrainFrequency * (1 - bossHitRate);
            fullResistRate += calculationOptions.EffectShadowSilenceFrequency * (1 - bossHitRate * StatConversion.GetAverageResistance(calculationOptions.TargetLevel, calculationOptions.PlayerLevel, statsTotal.ShadowResistance, 0));

            return statsTotal;
        }
         
        public override ComparisonCalculationBase[] GetCustomChartData(Character character, string chartName)
        {
            List<ComparisonCalculationBase> comparisonList = new List<ComparisonCalculationBase>();
            ComparisonCalculationBase comparison;
            float[] subPoints;

            CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;
            MageTalents talents = character.MageTalents;
            DisplayCalculations calculationResult = calculationOptions.Calculations;

            switch (chartName)
            {
                case "Item Budget":
                    Stats[] statsList = new Stats[] {
                        new Stats() { SpellPower = 1.17f },
                        new Stats() { CritRating = 1 },
                        new Stats() { HasteRating = 1 },
                        new Stats() { HitRating = 1 },
                        new Stats() { Intellect = 1 },
                        new Stats() { MasteryRating = 1 },
                    };

                    return EvaluateItemBudget(character, statsList);
                case "Mana Sources":
                    _subPointNameColors = _subPointNameColorsMana;
                    foreach (KeyValuePair<string, float> kvp in calculationResult.ManaSources)
                    {
                        if (kvp.Value > 0)
                        {
                            comparison = CreateNewComparisonCalculation();
                            comparison.Name = kvp.Key;
                            comparison.Equipped = false;
                            comparison.OverallPoints = kvp.Value;
                            subPoints = new float[] { kvp.Value };
                            comparison.SubPoints = subPoints;

                            comparisonList.Add(comparison);
                        }
                    }
                    return comparisonList.ToArray();
                case "Mana Usage":
                    _subPointNameColors = _subPointNameColorsMana;
                    foreach (KeyValuePair<string, float> kvp in calculationResult.ManaUsage)
                    {
                        if (kvp.Value > 0)
                        {
                            comparison = CreateNewComparisonCalculation();
                            comparison.Name = kvp.Key;
                            comparison.Equipped = false;
                            comparison.OverallPoints = kvp.Value;
                            subPoints = new float[] { kvp.Value };
                            comparison.SubPoints = subPoints;

                            comparisonList.Add(comparison);
                        }
                    }
                    return comparisonList.ToArray();
                default:
                    return new ComparisonCalculationBase[0];
            }
        }

        private ComparisonCalculationBase[] EvaluateItemBudget(Character character, Stats[] statsList)
        {
            KeyValuePair<PropertyInfo, float>[] properties = new KeyValuePair<PropertyInfo, float>[statsList.Length];
            for (int index = 0; index < statsList.Length; index++)
            {
                var p = statsList[index].Values(x => x > 0);
                foreach (var kvp in p)
                {
                    properties[index] = kvp;
                }
            }
            Item item = new Item() { Stats = new Stats() };
            ComparisonCalculationBase[] result = new ComparisonCalculationBase[statsList.Length];
            for (int index = 0; index < statsList.Length; index++)
            {
                result[index] = CalculationsBase.GetRelativeStatValue(character, properties[index].Key, item, properties[index].Value);
            }

            return result;
        }

        public static string TimeFormat(double time)
        {
            TimeSpan span = new TimeSpan((long)(Math.Round(time, 2) / 0.0000001));
            return string.Format("{0:0}:{1:00}", span.Minutes, span.Seconds);
        }

#if !RAWR3 && !RAWR4
        public override void RenderCustomChart(Character character, string chartName, System.Drawing.Graphics g, int width, int height)
        {
            Rectangle rectSubPoint;
            System.Drawing.Drawing2D.LinearGradientBrush brushSubPointFill;
            System.Drawing.Drawing2D.ColorBlend blendSubPoint;

            CalculationOptionsMage calculationOptions = character.CalculationOptions as CalculationOptionsMage;

            Font fontLegend = new Font("Verdana", 10f, GraphicsUnit.Pixel);
            int legendY;

            Brush[] brushSubPoints;
            Color[] colorSubPointsA;
            Color[] colorSubPointsB;

            float graphStart;
            float graphWidth;
            float graphTop;
            float graphBottom;
            float graphHeight;
            float maxScale;
            float graphEnd;
            float[] ticks;
            Pen black200 = new Pen(Color.FromArgb(200, 0, 0, 0));
            Pen black150 = new Pen(Color.FromArgb(150, 0, 0, 0));
            Pen black75 = new Pen(Color.FromArgb(75, 0, 0, 0));
            Pen black50 = new Pen(Color.FromArgb(50, 0, 0, 0));
            Pen black25 = new Pen(Color.FromArgb(25, 0, 0, 0));
            StringFormat formatTick = new StringFormat();
            formatTick.LineAlignment = StringAlignment.Far;
            formatTick.Alignment = StringAlignment.Center;
            Brush black200brush = new SolidBrush(Color.FromArgb(200, 0, 0, 0));
            Brush black150brush = new SolidBrush(Color.FromArgb(150, 0, 0, 0));
            Brush black75brush = new SolidBrush(Color.FromArgb(75, 0, 0, 0));
            Brush black50brush = new SolidBrush(Color.FromArgb(50, 0, 0, 0));
            Brush black25brush = new SolidBrush(Color.FromArgb(25, 0, 0, 0));

            string[] statNames = new string[] { "11.7 Spell Power", "4 Mana per 5 sec", "10 Crit Rating", "10 Haste Rating", "10 Hit Rating", "10 Intellect", "10 Spirit" };
            Color[] statColors = new Color[] { Color.FromArgb(255, 255, 0, 0), Color.DarkBlue, Color.FromArgb(255, 255, 165, 0), Color.Olive, Color.FromArgb(255, 154, 205, 50), Color.Aqua, Color.FromArgb(255, 0, 0, 255) };

            DisplayCalculations calculations = calculationOptions.Calculations;

            List<float> X = new List<float>();
            List<ComparisonCalculationBase[]> Y = new List<ComparisonCalculationBase[]>();

            height -= 2;

            Stats[] statsList = new Stats[] {
                        new Stats() { SpellPower = 1.17f },
                        new Stats() { Mp5 = 0.4f },
                        new Stats() { CritRating = 1 },
                        new Stats() { HasteRating = 1 },
                        new Stats() { HitRating = 1 },
                        new Stats() { Intellect = 1 },
                        new Stats() { Spirit = 1 },
                    };

            switch (chartName)
            {
                case "Stats Graph":
                    Base.Graph.RenderStatsGraph(g, width, height, character, statsList, statColors, 100, "", "Dps Rating", Base.Graph.Style.Mage);
                    break;
                case "Proc Uptime":
                    List<SpecialEffect> effectList = new List<SpecialEffect>();
                    effectList.AddRange(calculations.SpellPowerEffects);
                    effectList.AddRange(calculations.HasteRatingEffects);

                    #region Legend
                    legendY = 2;

                    Color[] colors = new Color[] {
                        Color.FromArgb(255,202,180,96), 
                        Color.FromArgb(255,101,225,240),
                        Color.FromArgb(255,0,4,3), 
                        Color.FromArgb(255,238,238,30),
                        Color.FromArgb(255,45,112,63), 
                        Color.FromArgb(255,121,72,210), 
                        Color.FromArgb(255,217,100,54), 
                        Color.FromArgb(255,210,72,195), 
                        Color.FromArgb(255,206,189,191), 
                        Color.FromArgb(255,255,0,0), 
                        Color.FromArgb(255,0,255,0), 
                        Color.FromArgb(255,0,0,255), 
                    };

                    brushSubPoints = new Brush[effectList.Count];
                    colorSubPointsA = new Color[effectList.Count];
                    colorSubPointsB = new Color[effectList.Count];
                    for (int i = 0; i < effectList.Count; i++)
                    {
                        Color baseColor = colors[i];
                        brushSubPoints[i] = new SolidBrush(Color.FromArgb(baseColor.R / 2, baseColor.G / 2, baseColor.B / 2));
                        colorSubPointsA[i] = Color.FromArgb(baseColor.A / 2, baseColor.R / 2, baseColor.G / 2, baseColor.B / 2);
                        colorSubPointsB[i] = Color.FromArgb(baseColor.A / 2, baseColor);
                    }

                    for (int i = 0; i < effectList.Count; i++)
                    {
                        g.DrawLine(new Pen(colors[i]), new Point(20, legendY + 7), new Point(50, legendY + 7));
                        //g.DrawString(effectList[i].ToString(), fontLegend, Brushes.Black, new Point(60, legendY));

                        legendY += 16;
                    }

                    #endregion

                    #region Graph Ticks
                    graphStart = 30f;
                    graphWidth = width - 50f;
                    graphTop = legendY;
                    graphBottom = height - 4 - 4 * effectList.Count;
                    graphHeight = graphBottom - graphTop - 40;
                    maxScale = calculationOptions.FightDuration;
                    graphEnd = graphStart + graphWidth;
                    ticks = new float[] {(float)Math.Round(graphStart + graphWidth * 0.5f),
                            (float)Math.Round(graphStart + graphWidth * 0.75f),
                            (float)Math.Round(graphStart + graphWidth * 0.25f),
                            (float)Math.Round(graphStart + graphWidth * 0.125f),
                            (float)Math.Round(graphStart + graphWidth * 0.375f),
                            (float)Math.Round(graphStart + graphWidth * 0.625f),
                            (float)Math.Round(graphStart + graphWidth * 0.875f)};

                    for (int i = 0; i <= 10; i++)
                    {
                        float h = (float)Math.Round(graphBottom - graphHeight * i / 10.0);
                        g.DrawLine(black25, graphStart - 4, h, graphEnd, h);
                        //g.DrawLine(black200, graphStart - 4, h, graphStart, h);

                        g.DrawString((i / 10.0).ToString("F1"), fontLegend, black200brush, graphStart - 15, h + 6, formatTick);
                    }

                    g.DrawLine(black150, ticks[1], graphTop + 36, ticks[1], graphTop + 39);
                    g.DrawLine(black150, ticks[2], graphTop + 36, ticks[2], graphTop + 39);
                    g.DrawLine(black75, ticks[3], graphTop + 36, ticks[3], graphTop + 39);
                    g.DrawLine(black75, ticks[4], graphTop + 36, ticks[4], graphTop + 39);
                    g.DrawLine(black75, ticks[5], graphTop + 36, ticks[5], graphTop + 39);
                    g.DrawLine(black75, ticks[6], graphTop + 36, ticks[6], graphTop + 39);
                    g.DrawLine(black75, graphEnd, graphTop + 41, graphEnd, height - 4);
                    g.DrawLine(black75, ticks[0], graphTop + 41, ticks[0], height - 4);
                    g.DrawLine(black50, ticks[1], graphTop + 41, ticks[1], height - 4);
                    g.DrawLine(black50, ticks[2], graphTop + 41, ticks[2], height - 4);
                    g.DrawLine(black25, ticks[3], graphTop + 41, ticks[3], height - 4);
                    g.DrawLine(black25, ticks[4], graphTop + 41, ticks[4], height - 4);
                    g.DrawLine(black25, ticks[5], graphTop + 41, ticks[5], height - 4);
                    g.DrawLine(black25, ticks[6], graphTop + 41, ticks[6], height - 4);
                    g.DrawLine(black200, graphStart - 4, graphTop + 40, graphEnd + 4, graphTop + 40);
                    g.DrawLine(black200, graphStart, graphTop + 36, graphStart, height - 4);
                    g.DrawLine(black200, graphEnd, graphTop + 36, graphEnd, height - 4);
                    g.DrawLine(black200, graphStart - 4, graphBottom, graphEnd + 4, graphBottom);

                    g.DrawString(TimeFormat(0f), fontLegend, black200brush, graphStart, graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale), fontLegend, black200brush, graphEnd, graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale * 0.5f), fontLegend, black200brush, ticks[0], graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale * 0.75f), fontLegend, black150brush, ticks[1], graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale * 0.25f), fontLegend, black150brush, ticks[2], graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale * 0.125f), fontLegend, black75brush, ticks[3], graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale * 0.375f), fontLegend, black75brush, ticks[4], graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale * 0.625f), fontLegend, black75brush, ticks[5], graphTop + 36, formatTick);
                    g.DrawString(TimeFormat(maxScale * 0.875f), fontLegend, black75brush, ticks[6], graphTop + 36, formatTick);
                    #endregion

                    for (int i = 0; i < effectList.Count; i++)
                    {
                        float procs = 0.0f;
                        float triggers = 0.0f;
                        for (int j = 0; j < calculations.SolutionVariable.Count; j++)
                        {
                            if (calculations.Solution[j] > 0)
                            {
                                Cycle c = calculations.SolutionVariable[j].Cycle;
                                if (c != null)
                                {
                                    switch (effectList[i].Trigger)
                                    {
                                        case Trigger.DamageSpellCrit:
                                        case Trigger.SpellCrit:
                                            triggers += (float)calculations.Solution[j] * c.Ticks / c.CastTime;
                                            procs += (float)calculations.Solution[j] * c.CritProcs / c.CastTime;
                                            break;
                                        case Trigger.DamageSpellHit:
                                        case Trigger.SpellHit:
                                            triggers += (float)calculations.Solution[j] * c.Ticks / c.CastTime;
                                            procs += (float)calculations.Solution[j] * c.HitProcs / c.CastTime;
                                            break;
                                        case Trigger.SpellMiss:
                                            triggers += (float)calculations.Solution[j] * c.Ticks / c.CastTime;
                                            procs += (float)calculations.Solution[j] * (1 - c.HitProcs) / c.CastTime;
                                            break;
                                        case Trigger.DamageSpellCast:
                                        case Trigger.SpellCast:
                                            if (effectList[i].Stats.ValkyrDamage > 0)
                                            {
                                                triggers += (float)calculations.Solution[j] * c.CastProcs2 / c.CastTime;
                                                procs += (float)calculations.Solution[j] * c.CastProcs2 / c.CastTime;
                                            }
                                            else
                                            {
                                                triggers += (float)calculations.Solution[j] * c.CastProcs / c.CastTime;
                                                procs += (float)calculations.Solution[j] * c.CastProcs / c.CastTime;
                                            }
                                            break;
                                        case Trigger.MageNukeCast:
                                            triggers += (float)calculations.Solution[j] * c.NukeProcs / c.CastTime;
                                            procs += (float)calculations.Solution[j] * c.NukeProcs / c.CastTime;
                                            break;
                                        case Trigger.DamageDone:
                                        case Trigger.DamageOrHealingDone:
                                            triggers += (float)calculations.Solution[j] * c.DamageProcs / c.CastTime;
                                            procs += (float)calculations.Solution[j] * c.DamageProcs / c.CastTime;
                                            break;
                                        case Trigger.DoTTick:
                                            triggers += (float)calculations.Solution[j] * c.DotProcs / c.CastTime;
                                            procs += (float)calculations.Solution[j] * c.DotProcs / c.CastTime;
                                            break;
                                    }
                                }
                            }
                        }
                        float triggerInterval = calculations.CalculationOptions.FightDuration / triggers;
                        float triggerChance = Math.Min(1.0f, procs / triggers);

                        int steps = 200;
                        PointF[] plot = new PointF[steps + 1];
                        for (int tick = 0; tick <= steps; tick++)
                        {
                            float time = tick / (float)steps * calculations.CalculationOptions.FightDuration;
                            plot[tick] = new PointF(graphStart + time / maxScale * graphWidth, graphBottom - graphHeight * effectList[i].GetUptimePlot(triggerInterval, triggerChance, 3.0f, time));
                        }

                        g.DrawLines(new Pen(colors[i]), plot);

                        g.DrawString(string.Format("{0} (average uptime {1:F}%)", effectList[i], effectList[i].GetAverageUptime(triggerInterval, triggerChance, 3.0f, calculations.CalculationOptions.FightDuration) * 100), fontLegend, Brushes.Black, new Point(60, 2 + i * 16));
                    }
                    break;
                case "Sequence Reconstruction":

                    if (calculationOptions.SequenceReconstruction == null)
                    {
                        g.DrawString("Sequence reconstruction data is not available.", fontLegend, Brushes.Black, 2, 2);
                    }
                    else
                    {
        #region Legend
                        legendY = 2;

                        List<EffectCooldown> cooldownList = calculationOptions.Calculations.CooldownList;

                        brushSubPoints = new Brush[cooldownList.Count];
                        colorSubPointsA = new Color[cooldownList.Count];
                        colorSubPointsB = new Color[cooldownList.Count];
                        for (int i = 0; i < cooldownList.Count; i++)
                        {
                            Color baseColor = cooldownList[i].Color;
                            brushSubPoints[i] = new SolidBrush(Color.FromArgb(baseColor.R / 2, baseColor.G / 2, baseColor.B / 2));
                            colorSubPointsA[i] = Color.FromArgb(baseColor.A / 2, baseColor.R / 2, baseColor.G / 2, baseColor.B / 2);
                            colorSubPointsB[i] = Color.FromArgb(baseColor.A / 2, baseColor);
                        }
                        StringFormat formatSubPoint = new StringFormat();
                        formatSubPoint.Alignment = StringAlignment.Center;
                        formatSubPoint.LineAlignment = StringAlignment.Center;

                        int maxWidth = 1;
                        for (int i = 0; i < cooldownList.Count; i++)
                        {
                            string subPointName = cooldownList[i].Name;
                            int widthSubPoint = (int)Math.Ceiling(g.MeasureString(subPointName, fontLegend).Width + 2f);
                            if (widthSubPoint > maxWidth) maxWidth = widthSubPoint;
                        }
                        for (int i = 0; i < cooldownList.Count; i++)
                        {
                            string cooldownName = cooldownList[i].Name;
                            rectSubPoint = new Rectangle(2, legendY, maxWidth, 16);
                            blendSubPoint = new System.Drawing.Drawing2D.ColorBlend(3);
                            blendSubPoint.Colors = new Color[] { colorSubPointsA[i], colorSubPointsB[i], colorSubPointsA[i] };
                            blendSubPoint.Positions = new float[] { 0f, 0.5f, 1f };
                            brushSubPointFill = new System.Drawing.Drawing2D.LinearGradientBrush(rectSubPoint, colorSubPointsA[i], colorSubPointsB[i], 67f);
                            brushSubPointFill.InterpolationColors = blendSubPoint;

                            g.FillRectangle(brushSubPointFill, rectSubPoint);
                            g.DrawRectangle(new Pen(brushSubPointFill), rectSubPoint);
                            g.DrawRectangle(new Pen(brushSubPointFill), rectSubPoint);
                            g.DrawRectangle(new Pen(brushSubPointFill), rectSubPoint);

                            g.DrawString(cooldownName, fontLegend, brushSubPoints[i], rectSubPoint, formatSubPoint);
                            legendY += 16;
                        }

                        if (calculationOptions.AdviseAdvancedSolver)
                        {
                            g.DrawString("Sequence Reconstruction was not fully successful, it is recommended that you enable more options in advanced solver (segment cooldowns, integral mana consumables, advanced constraints options)!", fontLegend, Brushes.Black, new RectangleF(5 + maxWidth, 40, width - maxWidth - 10, 100));
                        }

                        g.DrawLine(Pens.Aqua, new Point(maxWidth + 40, 10), new Point(maxWidth + 80, 10));
                        g.DrawString("Mana", fontLegend, Brushes.Black, new Point(maxWidth + 90, 2));
                        g.DrawLine(Pens.Red, new Point(maxWidth + 40, 26), new Point(maxWidth + 80, 26));
                        g.DrawString("Dps", fontLegend, Brushes.Black, new Point(maxWidth + 90, 18));
                        #endregion

        #region Graph Ticks
                        graphStart = 20f;
                        graphWidth = width - 40f;
                        graphTop = legendY;
                        graphBottom = height - 4 - 4 * cooldownList.Count;
                        graphHeight = graphBottom - graphTop - 40;
                        maxScale = calculationOptions.FightDuration;
                        graphEnd = graphStart + graphWidth;
                        ticks = new float[] {(float)Math.Round(graphStart + graphWidth * 0.5f),
                            (float)Math.Round(graphStart + graphWidth * 0.75f),
                            (float)Math.Round(graphStart + graphWidth * 0.25f),
                            (float)Math.Round(graphStart + graphWidth * 0.125f),
                            (float)Math.Round(graphStart + graphWidth * 0.375f),
                            (float)Math.Round(graphStart + graphWidth * 0.625f),
                            (float)Math.Round(graphStart + graphWidth * 0.875f)};

                        g.DrawLine(black150, ticks[1], graphTop + 36, ticks[1], graphTop + 39);
                        g.DrawLine(black150, ticks[2], graphTop + 36, ticks[2], graphTop + 39);
                        g.DrawLine(black75, ticks[3], graphTop + 36, ticks[3], graphTop + 39);
                        g.DrawLine(black75, ticks[4], graphTop + 36, ticks[4], graphTop + 39);
                        g.DrawLine(black75, ticks[5], graphTop + 36, ticks[5], graphTop + 39);
                        g.DrawLine(black75, ticks[6], graphTop + 36, ticks[6], graphTop + 39);
                        g.DrawLine(black75, graphEnd, graphTop + 41, graphEnd, height - 4);
                        g.DrawLine(black75, ticks[0], graphTop + 41, ticks[0], height - 4);
                        g.DrawLine(black50, ticks[1], graphTop + 41, ticks[1], height - 4);
                        g.DrawLine(black50, ticks[2], graphTop + 41, ticks[2], height - 4);
                        g.DrawLine(black25, ticks[3], graphTop + 41, ticks[3], height - 4);
                        g.DrawLine(black25, ticks[4], graphTop + 41, ticks[4], height - 4);
                        g.DrawLine(black25, ticks[5], graphTop + 41, ticks[5], height - 4);
                        g.DrawLine(black25, ticks[6], graphTop + 41, ticks[6], height - 4);
                        g.DrawLine(black200, graphStart - 4, graphTop + 40, graphEnd + 4, graphTop + 40);
                        g.DrawLine(black200, graphStart, graphTop + 36, graphStart, height - 4);
                        g.DrawLine(black200, graphEnd, graphTop + 36, graphEnd, height - 4);
                        g.DrawLine(black200, graphStart - 4, graphBottom, graphEnd + 4, graphBottom);

                        g.DrawString(TimeFormat(0f), fontLegend, black200brush, graphStart, graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale), fontLegend, black200brush, graphEnd, graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale * 0.5f), fontLegend, black200brush, ticks[0], graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale * 0.75f), fontLegend, black150brush, ticks[1], graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale * 0.25f), fontLegend, black150brush, ticks[2], graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale * 0.125f), fontLegend, black75brush, ticks[3], graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale * 0.375f), fontLegend, black75brush, ticks[4], graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale * 0.625f), fontLegend, black75brush, ticks[5], graphTop + 36, formatTick);
                        g.DrawString(TimeFormat(maxScale * 0.875f), fontLegend, black75brush, ticks[6], graphTop + 36, formatTick);
                        #endregion

                        List<SequenceReconstruction.SequenceItem> sequence = calculationOptions.SequenceReconstruction.sequence;

                        float mana = calculations.StartingMana;
                        int gemCount = 0;
                        float time = 0;
                        Color manaFill = Color.FromArgb(50, Color.FromArgb(255, 0, 0, 255));
                        float lastMana = mana;
                        float maxMana = calculations.BaseStats.Mana;
                        float maxDps = 100;
                        for (int i = 0; i < sequence.Count; i++)
                        {
                            int index = sequence[i].Index;
                            VariableType type = sequence[i].VariableType;
                            float duration = (float)sequence[i].Duration;
                            Cycle cycle = sequence[i].Cycle;
                            if (cycle != null && cycle.DamagePerSecond > maxDps) maxDps = cycle.DamagePerSecond;
                            CastingState state = sequence[i].CastingState;
                            float mps = (float)sequence[i].Mps;
                            if (sequence[i].IsManaPotionOrGem)
                            {
                                duration = 0;
                                if (sequence[i].VariableType == VariableType.ManaGem)
                                {
                                    mana += (float)((1 + calculations.BaseStats.BonusManaGem) * calculations.ManaGemValue);
                                    gemCount++;
                                }
                                else if (sequence[i].VariableType == VariableType.ManaPotion)
                                {
                                    mana += (float)((1 + calculations.BaseStats.BonusManaPotion) * calculations.ManaPotionValue);
                                }
                                if (mana < 0) mana = 0;
                                if (mana > maxMana)
                                {
                                    mana = maxMana;
                                }
                                g.DrawLine(Pens.Aqua, graphStart + time / maxScale * graphWidth, graphBottom - graphHeight * lastMana / maxMana, graphStart + time / maxScale * graphWidth, height - 44 - graphHeight * mana / maxMana);
                            }
                            else
                            {
                                if (sequence[i].IsEvocation)
                                {
                                    switch (sequence[i].VariableType)
                                    {
                                        case VariableType.Evocation:
                                            mps = -(float)calculationOptions.Calculations.EvocationRegen;
                                            break;
                                        case VariableType.EvocationIV:
                                            mps = -(float)calculationOptions.Calculations.EvocationRegenIV;
                                            break;
                                        case VariableType.EvocationHero:
                                            mps = -(float)calculationOptions.Calculations.EvocationRegenHero;
                                            break;
                                        case VariableType.EvocationIVHero:
                                            mps = -(float)calculationOptions.Calculations.EvocationRegenIVHero;
                                            break;
                                    }
                                }
                                float partTime = duration;
                                if (mana - mps * duration < 0) partTime = mana / mps;
                                else if (mana - mps * duration > maxMana) partTime = (mana - maxMana) / mps;
                                mana -= mps * duration;
                                if (mana < 0) mana = 0;
                                if (mana > maxMana)
                                {
                                    mana = maxMana;
                                }
                                float x1 = graphStart + time / maxScale * graphWidth;
                                float x2 = graphStart + (time + partTime) / maxScale * graphWidth;
                                float x3 = graphStart + (time + duration) / maxScale * graphWidth;
                                float y1 = graphBottom - graphHeight * lastMana / maxMana;
                                float y2 = graphBottom - graphHeight * mana / maxMana;
                                float y3 = graphBottom;
                                g.FillPolygon(new SolidBrush(manaFill), new PointF[] { new PointF(x1, y1), new PointF(x2, y2), new PointF(x3, y2), new PointF(x3, y3), new PointF(x1, y3) });
                                g.DrawLine(Pens.Aqua, x1, y1, x2, y2);
                                g.DrawLine(Pens.Aqua, x2, y2, x3, y2);
                            }
                            lastMana = mana;
                            time += duration;
                        }

                        maxDps *= 1.1f;
                        List<PointF> list = new List<PointF>();
                        time = 0.0f;
                        for (int i = 0; i < sequence.Count; i++)
                        {
                            int index = sequence[i].Index;
                            VariableType type = sequence[i].VariableType;
                            float duration = (float)sequence[i].Duration;
                            Cycle cycle = sequence[i].Cycle;
                            CastingState state = sequence[i].CastingState;
                            float mps = (float)sequence[i].Mps;
                            if (sequence[i].IsManaPotionOrGem) duration = 0;
                            float dps = 0;
                            if (cycle != null)
                            {
                                dps = cycle.DamagePerSecond;
                            }
                            if (duration > 0)
                            {
                                list.Add(new PointF(graphStart + (time + 0.1f * duration) / maxScale * graphWidth, graphBottom - graphHeight * dps / maxDps));
                                list.Add(new PointF(graphStart + (time + 0.9f * duration) / maxScale * graphWidth, graphBottom - graphHeight * dps / maxDps));
                            }
                            time += duration;
                        }
                        if (list.Count > 0) g.DrawLines(Pens.Red, list.ToArray());

                        for (int cooldown = 0; cooldown < cooldownList.Count; cooldown++)
                        {
                            blendSubPoint = new System.Drawing.Drawing2D.ColorBlend(3);
                            blendSubPoint.Colors = new Color[] { colorSubPointsA[cooldown], colorSubPointsB[cooldown], colorSubPointsA[cooldown] };
                            blendSubPoint.Positions = new float[] { 0f, 0.5f, 1f };
                            bool on = false;
                            float timeOn = 0.0f;
                            time = 0;
                            for (int i = 0; i < sequence.Count; i++)
                            {
                                float duration = (float)sequence[i].Duration;
                                if (sequence[i].IsManaPotionOrGem) duration = 0;
                                if (on && !sequence[i].CastingState.EffectsActive(cooldownList[cooldown]) && !sequence[i].IsManaPotionOrGem)
                                {
                                    on = false;
                                    if (time > timeOn)
                                    {
                                        RectangleF rect = new RectangleF(graphStart + graphWidth * timeOn / maxScale, graphBottom + cooldown * 4, graphWidth * (time - timeOn) / maxScale, 4);
                                        brushSubPointFill = new System.Drawing.Drawing2D.LinearGradientBrush(rect, colorSubPointsA[cooldown], colorSubPointsB[cooldown], 67f);
                                        brushSubPointFill.InterpolationColors = blendSubPoint;

                                        g.FillRectangle(brushSubPointFill, rect);
                                        g.DrawRectangle(new Pen(brushSubPointFill), rect.X, rect.Y, rect.Width, rect.Height);
                                        g.DrawRectangle(new Pen(brushSubPointFill), rect.X, rect.Y, rect.Width, rect.Height);
                                        g.DrawRectangle(new Pen(brushSubPointFill), rect.X, rect.Y, rect.Width, rect.Height);
                                    }
                                }
                                else if (!on && sequence[i].CastingState.EffectsActive(cooldownList[cooldown]))
                                {
                                    on = true;
                                    timeOn = time;
                                }
                                time += duration;
                            }
                            if (on)
                            {
                                if (time - timeOn > 0)
                                {
                                    RectangleF rect = new RectangleF(graphStart + graphWidth * timeOn / maxScale, graphBottom + cooldown * 4, graphWidth * (time - timeOn) / maxScale, 4);
                                    brushSubPointFill = new System.Drawing.Drawing2D.LinearGradientBrush(rect, colorSubPointsA[cooldown], colorSubPointsB[cooldown], 67f);
                                    brushSubPointFill.InterpolationColors = blendSubPoint;

                                    g.FillRectangle(brushSubPointFill, rect);
                                    g.DrawRectangle(new Pen(brushSubPointFill), rect.X, rect.Y, rect.Width, rect.Height);
                                    g.DrawRectangle(new Pen(brushSubPointFill), rect.X, rect.Y, rect.Width, rect.Height);
                                    g.DrawRectangle(new Pen(brushSubPointFill), rect.X, rect.Y, rect.Width, rect.Height);
                                }
                            }
                        }
                    }
                    break;
                case "Scaling vs Spell Power":
                    Base.Graph.RenderScalingGraph(g, width, height, character, statsList, new Stats() { SpellPower = 5 }, true, statColors, 100, "", "Dps Rating", Base.Graph.Style.Mage);
                    break;
                case "Scaling vs Crit Rating":
                    Base.Graph.RenderScalingGraph(g, width, height, character, statsList, new Stats() { CritRating = 5 }, true, statColors, 100, "", "Dps Rating", Base.Graph.Style.Mage);
                    break;
                case "Scaling vs Haste Rating":
                    Base.Graph.RenderScalingGraph(g, width, height, character, statsList, new Stats() { HasteRating = 5 }, true, statColors, 100, "", "Dps Rating", Base.Graph.Style.Mage);
                    break;
                case "Scaling vs Intellect":
                    Base.Graph.RenderScalingGraph(g, width, height, character, statsList, new Stats() { Intellect = 5 }, true, statColors, 100, "", "Dps Rating", Base.Graph.Style.Mage);
                    break;
                case "Scaling vs Spirit":
                    Base.Graph.RenderScalingGraph(g, width, height, character, statsList, new Stats() { Spirit = 5 }, true, statColors, 100, "", "Dps Rating", Base.Graph.Style.Mage);
                    break;
            }
        }
#endif

        private string[] _optimizableCalculationLabels = null;
        public override string[] OptimizableCalculationLabels
        {
            get
            {
                if (_optimizableCalculationLabels == null)
                    _optimizableCalculationLabels = new string[] {
                    "Health",
                    "Nature Resistance",
                    "Fire Resistance",
                    "Frost Resistance",
                    "Shadow Resistance",
                    "Arcane Resistance",
                    "Resilience",
                    "Chance to Live",
                    "Hit Rating",
                    "Haste Rating",
                    "Crit Rating",
                    "PVP Trinket",
                    "Movement Speed",
                    };
                return _optimizableCalculationLabels;
            }
        }

        public static bool IsSupportedProc(Trigger trigger)
        {
            switch (trigger)
            { 
                case Trigger.DamageDone:
                case Trigger.DamageOrHealingDone:
                case Trigger.DamageSpellCast:
                case Trigger.DamageSpellCrit:
                case Trigger.DamageSpellHit:
                case Trigger.DoTTick:
                case Trigger.MageNukeCast:
                case Trigger.SpellCast:
                case Trigger.SpellCrit:
                case Trigger.SpellHit:
                case Trigger.SpellMiss:
                    return true;
            }
            return false;
        }

        public static bool IsSupportedUseEffect(SpecialEffect effect)
        {
            bool hasteEffect;
            bool stackingEffect;
            return IsSupportedUseEffect(effect, out hasteEffect, out stackingEffect);
        }

        public static bool IsSupportedUseEffect(SpecialEffect effect, out bool hasteEffect, out bool stackingEffect)
        {
            stackingEffect = false;
            hasteEffect = false;
            if (effect.MaxStack == 1 && effect.Trigger == Trigger.Use)
            {
                // check if it is a stacking use effect
                Stats effectStats = effect.Stats;
                for (int i = 0; i < effectStats._rawSpecialEffectDataSize; i++)
                {
                    SpecialEffect e = effectStats._rawSpecialEffectData[i];
                    if (e.Chance == 1f && e.Cooldown == 0f && (e.Trigger == Trigger.DamageSpellCast || e.Trigger == Trigger.DamageSpellHit || e.Trigger == Trigger.SpellCast || e.Trigger == Trigger.SpellHit))
                    {
                        if (e.Stats.HasteRating > 0)
                        {
                            hasteEffect = true;
                            stackingEffect = true;
                            break;
                        }
                    }
                    if (e.Chance == 1f && e.Cooldown == 0f && (e.Trigger == Trigger.DamageSpellCrit || e.Trigger == Trigger.SpellCrit))
                    {
                        if (e.Stats.CritRating < 0 && effect.Stats.CritRating > 0)
                        {
                            stackingEffect = true;
                            break;
                        }
                    }
                }
                if (stackingEffect)
                {
                    return true;
                }
                if (effect.Stats.HasteRating > 0)
                {
                    hasteEffect = true;
                }
                return effect.Stats.SpellPower + effect.Stats.HasteRating + effect.Stats.Intellect + effect.Stats.HighestStat + effect.Stats.MasteryRating > 0;
            }
            return false;
        }

        public static bool IsSupportedSpellPowerProc(SpecialEffect effect)
        {
            return ((effect.MaxStack == 1 || effect.Cooldown == 0f) && effect.Stats.SpellPower > 0 && IsSupportedProc(effect.Trigger));
        }

        public static bool IsSupportedMasteryProc(SpecialEffect effect)
        {
            return ((effect.MaxStack == 1 || effect.Cooldown == 0f) && effect.Stats.MasteryRating > 0 && IsSupportedProc(effect.Trigger));
        }

        public static bool IsSupportedIntellectProc(SpecialEffect effect)
        {
            return (effect.MaxStack == 1 && (effect.Stats.Intellect + effect.Stats.HighestStat) > 0 && IsSupportedProc(effect.Trigger));
        }

        public static bool IsSupportedStackingIntellectProc(SpecialEffect effect)
        {
            return (effect.Chance == 1f && effect.Cooldown == 0f && effect.MaxStack > 1 && (effect.Stats.Intellect + effect.Stats.HighestStat) > 0 && (effect.Trigger == Trigger.DamageSpellCast || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellHit));
        }

        public static bool IsSupportedDamageProc(SpecialEffect effect)
        {
            return (effect.MaxStack == 1 && (effect.Stats.ArcaneDamage + effect.Stats.FireDamage + effect.Stats.FrostDamage + effect.Stats.NatureDamage + effect.Stats.ShadowDamage + effect.Stats.HolyDamage + effect.Stats.HolySummonedDamage + effect.Stats.FireSummonedDamage > 0) && (IsSupportedProc(effect.Trigger) || effect.Trigger == Trigger.Use));
        }

        public static bool IsSupportedHasteProc(SpecialEffect effect)
        {
            if (effect.MaxStack == 1 && effect.Stats.HasteRating > 0)
            {
                if (effect.Cooldown >= effect.Duration && (effect.Trigger == Trigger.DamageSpellCrit || effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.DamageSpellCast))
                {
                    return true;
                }
                if (effect.Cooldown == 0 && (effect.Trigger == Trigger.SpellCrit || effect.Trigger == Trigger.DamageSpellCrit))
                {
                    return true;
                }
            }
            return false;
        }

        public static bool IsSupportedManaRestoreProc(SpecialEffect effect)
        {
            return ((effect.MaxStack == 1 || effect.Cooldown == 0f) && effect.Stats.ManaRestore > 0 && (IsSupportedProc(effect.Trigger) || effect.Trigger == Trigger.Use));
        }

        public static bool IsSupportedMp5Proc(SpecialEffect effect)
        {
            return ((effect.MaxStack == 1 || effect.Cooldown == 0f) && effect.Stats.Mp5 > 0 && (IsSupportedProc(effect.Trigger) || effect.Trigger == Trigger.Use));
        }

        public static bool IsSupportedManaGemProc(SpecialEffect effect)
        {
            return (effect.MaxStack == 1 && effect.Trigger == Trigger.ManaGem);
        }

        public static bool IsSupportedResetStackingEffect(SpecialEffect effect)
        {
            if (effect.MaxStack == 1 && effect.Chance == 1 && (effect.Trigger == Trigger.DamageSpellCast || effect.Trigger == Trigger.DamageSpellHit || effect.Trigger == Trigger.SpellCast || effect.Trigger == Trigger.SpellHit))
            {
                Stats effectStats = effect.Stats;
                for (int i = 0; i < effectStats._rawSpecialEffectDataSize; i++)
                {
                    SpecialEffect e = effectStats._rawSpecialEffectData[i];
                    if (e.Chance == 1f && e.Cooldown == 0f && e.MaxStack > 1 && (e.Trigger == Trigger.DamageSpellCast || e.Trigger == Trigger.DamageSpellHit || e.Trigger == Trigger.SpellCast || e.Trigger == Trigger.SpellHit))
                    {
                        if (e.Stats.SpellPower > 0)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public static bool IsDarkIntentEffect(SpecialEffect effect)
        {
            if (effect.MaxStack > 1 && effect.Chance == 1 && effect.Trigger == Trigger.DarkIntentCriticalPeriodicDamageOrHealing && effect.Cooldown == 0)
            {
                return true;
            }
            return false;
        }

        public static bool IsSupportedEffect(SpecialEffect effect)
        {
            return IsSupportedUseEffect(effect) ||
                IsSupportedSpellPowerProc(effect) ||
                IsSupportedMasteryProc(effect) ||
                IsSupportedIntellectProc(effect) ||
                IsSupportedStackingIntellectProc(effect) ||
                IsSupportedDamageProc(effect) ||
                IsSupportedHasteProc(effect) ||
                IsSupportedManaRestoreProc(effect) ||
                IsSupportedMp5Proc(effect) ||
                IsSupportedManaGemProc(effect) ||
                IsSupportedResetStackingEffect(effect) ||
                IsDarkIntentEffect(effect);
        }

        public override Stats GetRelevantStats(Stats stats)
        {
            Stats s = new Stats()
            {
                ArcaneResistance = stats.ArcaneResistance,
                FireResistance = stats.FireResistance,
                FrostResistance = stats.FrostResistance,
                NatureResistance = stats.NatureResistance,
                ShadowResistance = stats.ShadowResistance,
                Stamina = stats.Stamina,
                Intellect = stats.Intellect,
                Spirit = stats.Spirit,
                DodgeRating = stats.DodgeRating,
                Health = stats.Health,
                Mp5 = stats.Mp5,
                Resilience = stats.Resilience,
                CritRating = stats.CritRating,
                SpellPower = stats.SpellPower,
                SpellFireDamageRating = stats.SpellFireDamageRating,
                HasteRating = stats.HasteRating,
                HitRating = stats.HitRating,
                BonusIntellectMultiplier = stats.BonusIntellectMultiplier,
                BonusSpellCritDamageMultiplier = stats.BonusSpellCritDamageMultiplier,
                BonusStaminaMultiplier = stats.BonusStaminaMultiplier,
                BonusSpiritMultiplier = stats.BonusSpiritMultiplier,
                Mana = stats.Mana,
                SpellArcaneDamageRating = stats.SpellArcaneDamageRating,
                SpellPenetration = stats.SpellPenetration,
                SpellFrostDamageRating = stats.SpellFrostDamageRating,
                Armor = stats.Armor,
                Hp5 = stats.Hp5,
                SpellCombatManaRegeneration = stats.SpellCombatManaRegeneration,
                BonusArcaneDamageMultiplier = stats.BonusArcaneDamageMultiplier,
                BonusFireDamageMultiplier = stats.BonusFireDamageMultiplier,
                BonusFrostDamageMultiplier = stats.BonusFrostDamageMultiplier,
                //SpellPowerFor6SecOnCrit = stats.SpellPowerFor6SecOnCrit,
                //EvocationExtension = stats.EvocationExtension,
                //BonusMageNukeMultiplier = stats.BonusMageNukeMultiplier,
                //LightningCapacitorProc = stats.LightningCapacitorProc,
                //SpellPowerFor20SecOnUse2Min = stats.SpellPowerFor20SecOnUse2Min,
                //HasteRatingFor20SecOnUse2Min = stats.HasteRatingFor20SecOnUse2Min,
                //ManaRestoreFromBaseManaPPM = stats.ManaRestoreFromBaseManaPPM,
                BonusManaGem = stats.BonusManaGem,
                //SpellPowerFor10SecOnHit_10_45 = stats.SpellPowerFor10SecOnHit_10_45,
                //SpellPowerFor10SecOnResist = stats.SpellPowerFor10SecOnResist,
                //SpellPowerFor15SecOnCrit_20_45 = stats.SpellPowerFor15SecOnCrit_20_45,
                //SpellPowerFor15SecOnUse90Sec = stats.SpellPowerFor15SecOnUse90Sec,
                //SpellHasteFor5SecOnCrit_50 = stats.SpellHasteFor5SecOnCrit_50,
                //SpellHasteFor6SecOnCast_15_45 = stats.SpellHasteFor6SecOnCast_15_45,
                //SpellDamageFor10SecOnHit_5 = stats.SpellDamageFor10SecOnHit_5,
                //SpellHasteFor6SecOnHit_10_45 = stats.SpellHasteFor6SecOnHit_10_45,
                //SpellPowerFor10SecOnCrit_20_45 = stats.SpellPowerFor10SecOnCrit_20_45,
                //SpellPowerFor10SecOnCast_10_45 = stats.SpellPowerFor10SecOnCast_10_45,
                BonusManaPotionEffectMultiplier = stats.BonusManaPotionEffectMultiplier,
                ThreatIncreaseMultiplier = stats.ThreatIncreaseMultiplier,
                ThreatReductionMultiplier = stats.ThreatReductionMultiplier,
                //HasteRatingFor20SecOnUse5Min = stats.HasteRatingFor20SecOnUse5Min,
                //SpellPowerFor15SecOnUse2Min = stats.SpellPowerFor15SecOnUse2Min,
                //ManaRestoreOnCast_5_15 = stats.ManaRestoreOnCast_5_15,
                InterruptProtection = stats.InterruptProtection,
                ArcaneResistanceBuff = stats.ArcaneResistanceBuff,
                FireResistanceBuff = stats.FireResistanceBuff,
                FrostResistanceBuff = stats.FrostResistanceBuff,
                ShadowResistanceBuff = stats.ShadowResistanceBuff,
                NatureResistanceBuff = stats.NatureResistanceBuff,
                PVPTrinket = stats.PVPTrinket,
                MovementSpeed = stats.MovementSpeed,
                MageIceArmor = stats.MageIceArmor,
                MageMageArmor = stats.MageMageArmor,
                MageMoltenArmor = stats.MageMoltenArmor,
                ManaRestoreFromMaxManaPerSecond = stats.ManaRestoreFromMaxManaPerSecond,
                SpellHit = stats.SpellHit,
                SpellCrit = stats.SpellCrit,
                SpellCritOnTarget = stats.SpellCritOnTarget,
                SpellHaste = stats.SpellHaste,
                //SpellPowerFor10SecOnCast_15_45 = stats.SpellPowerFor10SecOnCast_15_45,
                //ManaRestoreOnCast_10_45 = stats.ManaRestoreOnCast_10_45,
                //SpellHasteFor10SecOnCast_10_45 = stats.SpellHasteFor10SecOnCast_10_45,
                //ManaRestoreOnCrit_25_45 = stats.ManaRestoreOnCrit_25_45,
                //PendulumOfTelluricCurrentsProc = stats.PendulumOfTelluricCurrentsProc,
                //ThunderCapacitorProc = stats.ThunderCapacitorProc,
                //SpellPowerFor20SecOnUse5Min = stats.SpellPowerFor20SecOnUse5Min,
                CritBonusDamage = stats.CritBonusDamage,
                BonusDamageMultiplier = stats.BonusDamageMultiplier,
                //SpellPowerFor15SecOnCast_50_45 = stats.SpellPowerFor15SecOnCast_50_45,
                SpellsManaCostReduction = stats.SpellsManaCostReduction,
                BonusHealthMultiplier = stats.BonusHealthMultiplier,
                MasteryRating = stats.MasteryRating,
                BonusSpellPowerMultiplier = stats.BonusSpellPowerMultiplier,
                BonusManaMultiplier = stats.BonusManaMultiplier,
                DragonwrathProc = stats.DragonwrathProc,
            };
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (IsSupportedEffect(effect))
                {
                    s.AddSpecialEffect(effect);
                }
            }
            return s;
        }

        private static bool HasEffectStats(Stats stats)
        {
            float commonStats = stats.CritRating + stats.HasteRating + stats.HitRating;
            return HasMageStats(stats) || (commonStats > 0);
        }

        private static bool HasMageStats(Stats stats)
        {
            float mageStats = stats.Intellect + stats.Mp5 + stats.SpellPower + stats.SpellFireDamageRating + stats.BonusIntellectMultiplier + stats.BonusSpellCritDamageMultiplier + stats.BonusSpiritMultiplier + stats.SpellFrostDamageRating + stats.SpellArcaneDamageRating + stats.SpellPenetration + stats.Mana + stats.SpellCombatManaRegeneration + stats.BonusArcaneDamageMultiplier + stats.BonusFireDamageMultiplier + stats.BonusFrostDamageMultiplier + /*stats.EvocationExtension + stats.BonusMageNukeMultiplier + stats.LightningCapacitorProc + stats.ManaRestoreFromBaseManaPPM +*/ stats.BonusManaGem + stats.BonusManaPotionEffectMultiplier + stats.ThreatReductionMultiplier + stats.ArcaneResistance + stats.FireResistance + stats.FrostResistance + stats.NatureResistance + stats.ShadowResistance + stats.InterruptProtection + stats.ArcaneResistanceBuff + stats.FrostResistanceBuff + stats.FireResistanceBuff + stats.NatureResistanceBuff + stats.ShadowResistanceBuff + stats.MageIceArmor + stats.MageMageArmor + stats.MageMoltenArmor + stats.ManaRestoreFromMaxManaPerSecond + stats.SpellCrit + stats.SpellCritOnTarget + stats.SpellHit + stats.SpellHaste + /*stats.PendulumOfTelluricCurrentsProc + stats.ThunderCapacitorProc + */stats.CritBonusDamage + stats.BonusDamageMultiplier + stats.SpellsManaCostReduction + stats.BonusSpellPowerMultiplier + stats.BonusManaMultiplier + stats.DragonwrathProc;
            return mageStats > 0;
        }

        public override bool HasRelevantStats(Stats stats)
        {
            bool mageStats = HasMageStats(stats);
            float commonStats = stats.CritRating + stats.HasteRating + stats.HitRating + stats.Health + stats.Stamina + stats.Armor + stats.PVPTrinket + stats.MovementSpeed + stats.SnareRootDurReduc + stats.FearDurReduc + stats.StunDurReduc + stats.Resilience + stats.BonusHealthMultiplier + stats.MasteryRating;
            float ignoreStats = stats.Agility + stats.Strength + stats.AttackPower + stats.Dodge + stats.Parry + stats.DodgeRating + stats.ParryRating + stats.ExpertiseRating + stats.Block + stats.BlockRating + stats.SpellShadowDamageRating + stats.SpellNatureDamageRating + stats.ArmorPenetration + stats.TargetArmorReduction;
            foreach (SpecialEffect effect in stats.SpecialEffects())
            {
                if (IsSupportedEffect(effect))
                {
                    return true;
                }
                ignoreStats += effect.Stats.Agility + effect.Stats.Strength + effect.Stats.AttackPower + effect.Stats.Dodge + effect.Stats.Parry + effect.Stats.DodgeRating + effect.Stats.ParryRating + effect.Stats.ExpertiseRating + effect.Stats.Block + effect.Stats.BlockRating + effect.Stats.SpellShadowDamageRating + effect.Stats.SpellNatureDamageRating + effect.Stats.ArmorPenetration + effect.Stats.TargetArmorReduction;
            }
            return (mageStats || (commonStats > 0 && ignoreStats == 0.0f));
        }

        public override bool IsBuffRelevant(Buff buff, Character character)
        {
            if (buff != null && buff.Group != null && buff.Group.Contains("Pot")) 
            {
                return false;
            }
            return base.IsBuffRelevant(buff, character);
        }

        public override bool EnchantFitsInSlot(Enchant enchant, Character character, ItemSlot slot)
        {
            if (slot == ItemSlot.Ranged) return false;
            if (slot == ItemSlot.OffHand) return (enchant.Id == 4091);
            return base.EnchantFitsInSlot(enchant, character, slot);
        }

        public override bool ItemFitsInSlot(Item item, Character character, CharacterSlot slot, bool ignoreUnique)
        {
            if (slot == CharacterSlot.OffHand && item.Slot == ItemSlot.OneHand) return false;
            return base.ItemFitsInSlot(item, character, slot, ignoreUnique);
        }
    }
}
