﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rawr.Hunter.Skills;
using Rawr.Base.Algorithms;

namespace Rawr.Hunter
{
    public class HunterState : State<AbilWrapper>
    {
        public HunterTalents talents;
        private CDStates_Flags _flags_CDStates;
        public CDStates_Flags Flags_CDStates { get { return _flags_CDStates; } set { _flags_CDStates = value; } }

        #region Focus Related
        /// <summary>
        /// Focus > 50?
        /// </summary>
        public bool FocusGT50
        {
            get { return _flags_CDStates.HasFlag(CDStates_Flags.FOCUS_LT50); }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.FOCUS_LT50; }
                else { _flags_CDStates ^= CDStates_Flags.FOCUS_LT50; }
            }
        }

        #endregion

        #region Buffs/Debuffs
        public bool bReadiness
        {
            get { return (_flags_CDStates.HasFlag(CDStates_Flags.CD_READINESS)); }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.CD_READINESS; }
                else { _flags_CDStates ^= CDStates_Flags.CD_READINESS; }
            }
        }
        public bool bRapidFire
        {
            get { return (_flags_CDStates.HasFlag(CDStates_Flags.CD_RAPIDFIRE)); }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.CD_RAPIDFIRE; }
                else { _flags_CDStates ^= CDStates_Flags.CD_RAPIDFIRE; }
            }
        }
        
        /// <summary>
        /// Kill Command/ISS/Explosive Shot
        /// </summary>
        public bool bKC
        {
            get { return (_flags_CDStates & CDStates_Flags.CD_KILLCOMMAND) == CDStates_Flags.CD_KILLCOMMAND; }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.CD_KILLCOMMAND; }
                else { _flags_CDStates ^= CDStates_Flags.CD_KILLCOMMAND; }
            }
        }
        public bool bISS { get { return _flags_CDStates.HasFlag(CDStates_Flags.UP_IMPSTEADYSHOT); }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.UP_IMPSTEADYSHOT; }
                else { _flags_CDStates ^= CDStates_Flags.UP_IMPSTEADYSHOT; }
            }
        }
        public bool bES
        {
            get { return (_flags_CDStates & CDStates_Flags.EXPLOSIVESHOT) == CDStates_Flags.EXPLOSIVESHOT; }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.EXPLOSIVESHOT; }
                else { _flags_CDStates ^= CDStates_Flags.EXPLOSIVESHOT; }
            }
        }
        /// <summary>
        /// BuffProc: FF/MMM/LNL
        /// </summary>
        public bool bFF
        {
            get { return (_flags_CDStates & CDStates_Flags.FOCUSFIRE) == CDStates_Flags.FOCUSFIRE; }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.FOCUSFIRE; }
                else { _flags_CDStates ^= CDStates_Flags.FOCUSFIRE; }
            }
        }
        public bool bMMM
        {
            get { return _flags_CDStates.HasFlag(CDStates_Flags.PROC_MMM); }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.PROC_MMM; }
                else { _flags_CDStates ^= CDStates_Flags.PROC_MMM; }
            }
        }
        public bool bLNL
        {
            get { return (_flags_CDStates & CDStates_Flags.PROC_LOCKNLOAD) == CDStates_Flags.PROC_LOCKNLOAD; }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.PROC_LOCKNLOAD; }
                else { _flags_CDStates ^= CDStates_Flags.PROC_LOCKNLOAD; }
            }
        }
        #endregion
        /// <summary>
        /// ??/Chimera/BlackArrow Available?
        /// </summary>
        public bool bUnknown
        {
            get { return (_flags_CDStates & CDStates_Flags.BM_UNKNOWN) == CDStates_Flags.BM_UNKNOWN; }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.BM_UNKNOWN; }
                else { _flags_CDStates ^= CDStates_Flags.BM_UNKNOWN; }
            }
        }
        public bool bChimeraShot
        {
            get { return _flags_CDStates.HasFlag(CDStates_Flags.CD_CHIMERASHOT); }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.CD_CHIMERASHOT; }
                else { _flags_CDStates ^= CDStates_Flags.CD_CHIMERASHOT; }
            }
        }
        public bool bBlackArrow
        {
            get { return _flags_CDStates.HasFlag(CDStates_Flags.CD_BLACKARROW); }
            set
            {
                if (value == true) { _flags_CDStates |= CDStates_Flags.CD_BLACKARROW; }
                else { _flags_CDStates ^= CDStates_Flags.CD_BLACKARROW; }
            }
        }
        /// <summary>
        /// KS Available?
        /// </summary>
        public bool bKS
        {
            get { return (_flags_CDStates.HasFlag(CDStates_Flags.CD_KILLSHOT) /* && _flags_CDStates.HasFlag(CDStates_Flags.KILLSHOT_HEALTH) */); }
            set
            {
                if (value == true)
                {
                    _flags_CDStates |= CDStates_Flags.CD_KILLSHOT;
                    //                    _flags_CDStates |= CDStates_Flags.KILLSHOT_HEALTH; 
                }
                else 
                {
                    _flags_CDStates ^= CDStates_Flags.CD_KILLSHOT;
                    //                    _flags_CDStates ^= CDStates_Flags.KILLSHOT_HEALTH;
                }
            }
        }
        
        /// <summary>
        /// Assuming initial state with Talents.
        /// </summary>
        public HunterState(HunterTalents ht)
        {
            _flags_CDStates = 0;
            Name = "";
            talents = ht;
            FocusGT50 = true;
            this.Index = 0;
            if (ht != null)
            {
                switch ((Specialization)ht.HighestTree)
                {
                    case Specialization.BeastMastery:
                        {
                            Name += "BM";
                            break;
                        }
                    case Specialization.Marksmanship:
                        {
                            Name += "MM";
                            bChimeraShot = (ht.ChimeraShot > 0);
                            bReadiness = (ht.Readiness > 0);
                            break;
                        }
                    case Specialization.Survival:
                        {
                            Name += "SV";
                            bBlackArrow = (ht.BlackArrow > 0);
                            break;
                        }
                }
            }
        }

        public override string ToString()
        {
            string name = "";
            switch ((Specialization)talents.HighestTree)
            {
                case Specialization.BeastMastery:
                    {
                        string szName = "BM";
                        name = string.Format("{0}:Foc{1}FF{2}KC{3}??{4}KS{5}RF{6}", szName,
                            (FocusGT50 ? "+" : "-"),
                            (bFF ? "+" : "-"),
                            (bKC ? "+" : "-"),
                            (bUnknown ? "+" : "-"),
                            (bKS ? "+" : "-"),
                            (bRapidFire ? "+" : "-")
                            );
                        break;
                    }
                case Specialization.Marksmanship:
                    {
                        string szName = "MM";
                        name = string.Format("{0}:F50{1}MMM{2}ISS{3}CS{4}KS{5}RF{6}RD{7}", szName,
                            (FocusGT50 ? "+" : "-"),
                            (bMMM ? "+" : "-"),
                            (bISS ? "+" : "-"),
                            (bChimeraShot ? "+" : "-"),
                            (bKS ? "+" : "-"),
                            (bRapidFire ? "+" : "-"),
                            (bReadiness ? "+" : "-")
                            );
                        break;
                    }
                case Specialization.Survival:
                    {
                        string szName = "SV";
                        name = string.Format("{0}:Foc{1}LNL{2}ISS{3}BA{4:0}KS{5:0}RF{6}", szName,
                            (FocusGT50 ? "+" : "-"),
                            (bLNL ? "+" : "-"),
                            (bES ? "+" : "-"),
                            (bBlackArrow ? "+" : "-"),
                            (bKS ? "+" : "-"),
                            (bRapidFire ? "+" : "-")
                            );
                        break;
                    }
            }
            return name;
        }     

        public HunterState clone()
        {
            HunterState s = new HunterState(this.talents);
            s = this.MemberwiseClone() as HunterState;
            return s;
        }
    }

    class HunterStateSpaceGenerator : StateSpaceGenerator<AbilWrapper>
    {
        private HunterTalents talents;
        private FightTime ft;

        private double[][] TransitionTable;

        #region Global state parameters
        public float PhysicalHaste { get; set; }
        public int FocusMax 
        {
            get
            {
                if (talents == null
                    || talents.KindredSpirits == 0)
                    return 100;
                else
                    return 100 + (talents.KindredSpirits * 5);
            } 
        }
        public int CSCD 
        {
            get 
            {
                if (talents == null
                    || talents.GlyphOfChimeraShot == false)
                    return 10;
                else
                    return 9;
            } 
        }
        public int KSCD
        {
            get
            {
                if (talents == null
                    || talents.GlyphOfKillShot == false)
                    return 10;
                else
                    return 6;
            }
        }
        public int BACD
        {
            get
            {
                if (talents == null
                    || talents.Resourcefulness == 0)
                    return 30;
                else
                    return 30 - (talents.Resourcefulness * 2);
            }
        }
        public int ESCD
        {
            get
            {
                    return 6;
            }
        }
        #endregion

        public HunterStateSpaceGenerator(HunterTalents HT, FightTime FT)
        {
            talents = HT;
            ft = FT;

            BuildTransitionTable();
        }

        private void BuildTransitionTable()
        {
            if (AbilityList == null) return;

            int iShotCount = EnumHelper.GetCount(typeof(Shots));
            int iFlagCount = EnumHelper.GetCount(typeof(CDStates_Enum));
            #region Build Table Structure
            TransitionTable = new double[EnumHelper.GetCount(typeof(Shots))][];
            for (int i = 0; i < iShotCount; i++)
            {
                TransitionTable[i] = new double[iFlagCount];
            }
            #endregion
            foreach (KeyValuePair<Type, AbilWrapper> kvp in AbilityList)
            {
                BuildTable(kvp.Value);
            }
        }

        private void BuildTable(AbilWrapper fAS)
        {
            const double TRIGGER = (double)TriggerState.TRIGGER;
            const double NOEFFECT = (double)TriggerState.NOEFFECT;
            if (!fAS.ability.Validated)
            {
                return;
            }

            if (fAS.ability.eShot != Shots.KillShot)
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD(fAS.ability.UseTime, KSCD) * .2f;
            else
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_KILLSHOT] = TRIGGER;
            if (fAS.ability.eShot != Shots.RapidFire)
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_RAPIDFIRE] = SpendTimeForCD(fAS.ability.UseTime, 3 * 60);
            else
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_RAPIDFIRE] = TRIGGER;
            #region BM
            #endregion
            #region MM
            if (fAS.ability.eShot != Shots.ChimeraShot)
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_CHIMERASHOT] = SpendTimeForCD(fAS.ability.UseTime, CSCD);
            else
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_CHIMERASHOT] = TRIGGER;
            if (fAS.ability.eShot != Shots.SteadyShot)
            {
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.UP_IMPSTEADYSHOT] = SpendTimeForCD(fAS.ability.UseTime, 5f);
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(fAS.ability.FocusCost, fAS.ability.UseTime);
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.PROC_MMM] = NOEFFECT;
            }
            else
            {
                // Steady Shot
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.UP_IMPSTEADYSHOT] = .5;
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.FOCUS_LT50] = 1 - SpendFocus(fAS.ability.FocusCost, fAS.ability.UseTime);
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.PROC_MMM] = talents.MasterMarksman * .2 / 5;
            }
            if (talents.Readiness > 0)
            {
                if (fAS.ability.eShot != Shots.Readiness)
                    TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_READINESS] = SpendTimeForCD(fAS.ability.UseTime, 3 * 60);
                else
                {
                    // Readiness Resets all CDs.
                    TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_READINESS] = TRIGGER;
                    TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_CHIMERASHOT] = 1;
                    TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_KILLSHOT] = .2; // Should have a member for BossOptions Health < 20%
                    TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.CD_RAPIDFIRE] = 1;
                }
            }
            if (fAS.ability.eShot != Shots.AimedShot_MMM)
                TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.PROC_MMM] = NOEFFECT;
            else
            {
                if (talents.MasterMarksman > 0)
                {
                    TransitionTable[(int)fAS.ability.eShot][(int)CDStates_Enum.PROC_MMM] = TRIGGER;
                }
            }
            #endregion
            #region SV
            #endregion
        }

        #region StateChange Chances
        /// <summary>
        /// Get the chance that something will happen based on an incremental change.
        /// </summary>
        /// <param name="fCost">Value change</param>
        /// <param name="CrossOver">Cross Over point</param>
        /// <param name="iMaxRange">Max quantity</param>
        /// <returns>Percentage value that the crossover will happen.</returns>
        private double GetChanceBasedOnRange(float fCost, float fCrossOver, float fMaxRange)
        {
            double output = 1;
            // Ensure that fCost & fCrossover is not > fMaxRange
            if (fCost > fMaxRange
                || fCrossOver > fMaxRange)
            {
                throw new ArgumentOutOfRangeException(string.Format("Cost ({0:0.0}) or Cutoff ({1:0.0}) is outside of Range ({2:0.0}).", fCost, fCrossOver, fMaxRange));
            }
            double fCrossoverPerc = (fCrossOver / fMaxRange);
            double fCostRangePerc = (fCost / fMaxRange);
            output = (fCostRangePerc + fCrossoverPerc);
            output = Math.Max(Math.Min(output, 1), 0);
            return output;
        }

        /// <summary>
        /// Percent chance of changing from Above 50 focus to Below
        /// </summary>
        /// <param name="fCost">Normal cost of ability after glyphs/talents but not counting focus regen</param>
        /// <param name="fCastTime">How long to use the ability.</param>
        /// <returns></returns>
        private double SpendFocus(float fCost, float fCastTime)
        {
            float TimeofFR = Math.Max(fCastTime, 1);
            float FRpS = 4 * (1 + PhysicalHaste);
            float FR = FRpS * TimeofFR;
            return GetChanceBasedOnRange(Math.Abs(fCost - FR), 50f, FocusMax);
        }

        /// <summary>
        /// Percent chance of changing a CD
        /// </summary>
        /// <param name="fCastTime">How long to use the ability.</param>
        /// <returns>Percentage chance that duration will switch that CD to true.</returns>
        private double SpendTimeForCD(float fCastTime, float fCDTime)
        {
            float Time = Math.Max(fCastTime, 1);
            return GetChanceBasedOnRange(Time, 0, fCDTime);
        }
        #endregion

        protected override State<AbilWrapper> GetInitialState()
        {
            HunterState state = new HunterState(talents);
            string name = state.ToString();
            stateDictionary[name] = state;
            return state;
        }

        private Dictionary<string, HunterState> stateDictionary = new Dictionary<string, HunterState>();

        public Dictionary<Type, AbilWrapper> AbilityList;

        public HunterState GetState(CDStates_Flags cdStates)
        {
            string name = "";
            string szName = "";
            HunterState state = new HunterState(talents);
            bool bKS = (cdStates & CDStates_Flags.CD_KILLSHOT) == CDStates_Flags.CD_KILLSHOT;
            bool bFocus = (cdStates & CDStates_Flags.FOCUS_LT50) == CDStates_Flags.FOCUS_LT50;

            switch ((Specialization)talents.HighestTree)
            {
                case Specialization.BeastMastery:
                    {
                        szName = "BM";
                        name = string.Format("{0}:Foc{1}FF{2}KC{3}??{4}KS{5}RF{6}", szName,
                            (bFocus ? "+" : "-"),
                            (cdStates.HasFlag(CDStates_Flags.FOCUSFIRE) ? "+" : "-"),
                            (cdStates.HasFlag(CDStates_Flags.CD_KILLCOMMAND) ? "+" : "-"),
                            (cdStates.HasFlag(CDStates_Flags.BM_UNKNOWN) ? "+" : "-"), 
                            (bKS ? "+" : "-"),
                            (cdStates.HasFlag(CDStates_Flags.CD_RAPIDFIRE) ? "+" : "-")
                            );
                        break;
                    }
                case Specialization.Marksmanship:
                    {
                        szName = "MM";
                        name = string.Format("{0}:F50{1}MMM{2}ISS{3}CS{4}KS{5}RF{6}RD{7}", szName, 
                            (bFocus ? "+" : "-"),
                            ((cdStates & CDStates_Flags.PROC_MMM) == CDStates_Flags.PROC_MMM ? "+" : "-"),
                            ((cdStates & CDStates_Flags.UP_IMPSTEADYSHOT) == CDStates_Flags.UP_IMPSTEADYSHOT ? "+" : "-"),
                            ((cdStates & CDStates_Flags.CD_CHIMERASHOT) == CDStates_Flags.CD_CHIMERASHOT ? "+" : "-"), 
                            (bKS ? "+" : "-"),
                            (cdStates.HasFlag(CDStates_Flags.CD_RAPIDFIRE) ? "+" : "-"),
                            (cdStates.HasFlag(CDStates_Flags.CD_READINESS) ? "+" : "-")
                            );
                        break;
                    }
                case Specialization.Survival:
                    {
                        szName = "SV";
                        name = string.Format("{0}:Foc{1}LNL{2}ISS{3}BA{4:0}KS{5:0}RF{6}", szName, 
                            (bFocus ? "+" : "-"),
                            ((cdStates & CDStates_Flags.PROC_LOCKNLOAD) == CDStates_Flags.PROC_LOCKNLOAD ? "+" : "-"),
                            ((cdStates & CDStates_Flags.EXPLOSIVESHOT) == CDStates_Flags.EXPLOSIVESHOT ? "+" : "-"),
                            ((cdStates & CDStates_Flags.CD_BLACKARROW) == CDStates_Flags.CD_BLACKARROW ? "+" : "-"),
                            (bKS ? "+" : "-"),
                            (cdStates.HasFlag(CDStates_Flags.CD_RAPIDFIRE) ? "+" : "-")
                            );
                        break;
                    }
            }
            if (!stateDictionary.TryGetValue(name, out state))
            {
                state = new HunterState(talents) { Name = szName, Flags_CDStates = cdStates};
                stateDictionary[name] = state;
            }
            return state;
        }

#if false
        public HunterState GetState(string szName, bool bBuff, bool biss, bool bSpecial, bool bFocus, bool bKS, FightStage FS)
        {
            string name = "";
            HunterState state = new HunterState(ht);
            switch ((Specialization)ht.HighestTree)
            {
                case Specialization.BeastMastery:
                    {
                        name = string.Format("{0}:F50{1}MMM{2}ISS{3}CS{4:0}KS{5:0}_{6}", szName, (bFocus ? "+" : "-"), (bBuff ? "+" : "-"), (biss ? "+" : "-"), (bSpecial ? "+" : "-"), (bKS ? "+" : "-"), FS);
                        break;
                    }
                case Specialization.Marksmanship:
                    {
                        name = string.Format("{0}:F50{1}MMM{2}ISS{3}CS{4:0}KS{5:0}_{6}", szName, (bFocus ? "+" : "-"), (bBuff ? "+" : "-"), (biss ? "+" : "-"), (bSpecial ? "+" : "-"), (bKS ? "+" : "-"), FS);
                        break;
                    }
                case Specialization.Survival:
                    {
                        name = string.Format("{0}:F50{1}LNL{2}ISS{3}BA{4:0}KS{5:0}_{6}", szName, (bFocus ? "+" : "-"), (bBuff ? "+" : "-"), (biss ? "+" : "-"), (bSpecial ? "+" : "-"), (bKS ? "+" : "-"), FS);
                        break;
                    }
            }
            if (!stateDictionary.TryGetValue(name, out state))
            {
                state = new HunterState(ht) { Name = szName, bMMM = bBuff, bISS = biss, bChimeraShot = bSpecial, FocusGT50 = bFocus, bKS = bKS };
                stateDictionary[name] = state;
            }
            return state;
        }
        
#endif
        protected override List<StateTransition<AbilWrapper>> GetStateTransitions(State<AbilWrapper> state)
        {
            List<StateTransition<AbilWrapper>> output = new List<StateTransition<AbilWrapper>>();
            if (talents != null)
            {
                switch ((Specialization)talents.HighestTree)
                {
                    case Specialization.BeastMastery:
                        {
                            output = GetStateTransitionsBM(state);
                            break;
                        }
                    case Specialization.Marksmanship:
                        {
                            output = GetStateTransitionsMM(state);
                            break;
                        }
                    case Specialization.Survival:
                        {
                            output = GetStateTransitionsSV(state);
                            break;
                        }
                }
            }
            else
            {
                // output = GetStateTransitionsBlank(state);
            }
            return output;
        }

        /// <summary>
        /// BeastMaster cycle.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private List<StateTransition<AbilWrapper>> GetStateTransitionsBM(State<AbilWrapper> state)
        {
            // This roation assumes Serpent Sting is always up.
            HunterState s = state as HunterState;
            List<StateTransition<AbilWrapper>> output = new List<StateTransition<AbilWrapper>>();

            #region Setup Abilities to use.
//            AbilWrapper FF = AbilityList[typeof(FocusFire)];
            AbilWrapper KS = AbilityList[typeof(KillShot)];
//            AbilWrapper KC = AbilityList[typeof(KillCommand)];
            AbilWrapper ArcS = AbilityList[typeof(ArcaneShot)];
            AbilWrapper CS = AbilityList[typeof(CobraShot)];
            #endregion

            HunterState s2 = s.clone();

            if (s.talents == null)
                throw new ArgumentNullException();

            #region Shot priority.
            #endregion

            return output;
        }

        /// <summary>
        /// Marksman cycle.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private List<StateTransition<AbilWrapper>> GetStateTransitionsMM(State<AbilWrapper> state)
        {
            // This roation assumes Serpent Sting is always up.
            HunterState s = state as HunterState;
            List<StateTransition<AbilWrapper>> output = new List<StateTransition<AbilWrapper>>();

            AbilWrapper fAS = AbilityList[typeof(MMMAimedShot)]; 
            AbilWrapper CAAS = AbilityList[typeof(CAAimedShot)];
            AbilWrapper AS = AbilityList[typeof(AimedShot)];
            AbilWrapper SS = AbilityList[typeof(SteadyShot)];
            AbilWrapper CS = AbilityList[typeof(ChimeraShot)];
            AbilWrapper ArcS = AbilityList[typeof(ArcaneShot)];
            AbilWrapper KS = AbilityList[typeof(KillShot)];

            if (s.talents == null)
                throw new ArgumentNullException();
            float fTransitionMod = (s.bISS ? 1.15f : 1);
            float ssCastTime = SS.CastTime / fTransitionMod;
            const double GCD = 1;

            HunterState s2 = s.clone();

            #region Free Aimed Shot
            // Master Marksman Proc always has top billing.
            if (s.talents.MasterMarksman > 0
                && s.bMMM ) // MMM Procs
            {
                TriggerMMM(s, output, fAS, s2);
            } 
            #endregion
#if false // Careful Aim
            #region Careful Aim (Boss Health 90%+)
            // Careful Aim 
            if (s.talents.CarefulAim > 0
                && s.fightStage == FightStage.Opening)
            {
                if (s.Focus >= 50)
                {
                    output.Add(new StateTransition<AbilWrapper>()
                    {
                        Ability = CAAS,
                        TargetState = GetState(s.Name, s.IMMCount, s.bISS, (s.CSCD - (AS.CastTime / fTransitionMod)), (s.Focus - AS.FocusCost + (FR * AS.CastTime / fTransitionMod)), (s.KSCD - (AS.CastTime / fTransitionMod)), s.fightTime.AddSegment(AS.CastTime / fTransitionMod)),
                        TransitionDuration = AS.CastTime / fTransitionMod,
                        TransitionProbability = 1,
                    });
                }
                else
                {
                    // Increase Stacks
                    // 1st SS that doesn't proc the ISS.
                    output.Add(new StateTransition<AbilWrapper>()
                    {
                        Ability = SS,
                        TargetState = GetState(s.Name, ++s.IMMCount, false, (s.CSCD - ssCastTime), (s.Focus - SS.FocusCost + (ssCastTime * FR)), (s.KSCD - ssCastTime), s.fightTime.AddSegment(ssCastTime)),
                        TransitionDuration = ssCastTime,
                        TransitionProbability = (.5 * ProbToIncrementIMM),
                    });
                    // 2nd SS that does.
                    output.Add(new StateTransition<AbilWrapper>()
                    {
                        Ability = SS,
                        TargetState = GetState(s.Name, ++s.IMMCount, true, (s.CSCD - ssCastTime), (s.Focus - SS.FocusCost + (ssCastTime * FR)), (s.KSCD - ssCastTime), s.fightTime.AddSegment(ssCastTime)),
                        TransitionDuration = ssCastTime,
                        TransitionProbability = (.5 * ProbToIncrementIMM),
                    });
                    // Fail to increase stacks or doesn't have the MMM talent.
                    // 1st SS that doesn't proc the ISS.
                    output.Add(new StateTransition<AbilWrapper>()
                    {
                        Ability = SS,
                        TargetState = GetState(s.Name, s.IMMCount, false, (s.CSCD - ssCastTime), (s.Focus - SS.FocusCost + (ssCastTime * FR)), (s.KSCD - ssCastTime), s.fightTime.AddSegment(ssCastTime)),
                        TransitionDuration = ssCastTime,
                        TransitionProbability = (.5 * (1 - ProbToIncrementIMM)),
                    });
                    // 2nd SS that does.
                    output.Add(new StateTransition<AbilWrapper>()
                    {
                        Ability = SS,
                        TargetState = GetState(s.Name, s.IMMCount, true, (s.CSCD - ssCastTime), (s.Focus - SS.FocusCost + (ssCastTime * FR)), (s.KSCD - ssCastTime), s.fightTime.AddSegment(ssCastTime)),
                        TransitionDuration = ssCastTime,
                        TransitionProbability = (.5 * (1 - ProbToIncrementIMM)),
                    });
                }
            }
            #endregion  
#endif
            #region Kill Shot
            else if (s.bKS)
            {
                TriggerKillShot(s, output, KS, s2);
            } 
            #endregion
            #region Chimera Shot
            else if (s.talents.ChimeraShot > 0
                        && s.bChimeraShot
                        && s.FocusGT50)
            {
                TriggerChimera(s, output, CS, s2);
            } 
                #endregion
            #region Get ISS back Or Low Focus
            else if (s.bISS == false 
                    || s.FocusGT50 == false)
            {
                TriggerSteady(s, output, SS, s2);
            } 
            #endregion
            #region ISS == True && Focus > 50 
            else 
            {
                #region Arcane Shot
                TriggerArcaneShot(s, output, ArcS, s2);
                #endregion
                #region Hard Cast Aimed Shot // disabled - Bring in Fight Options for Movement percentages.
#if false

            double FocAS = SpendFocus(AS.FocusCost, (float)AS.CastTime / fTransitionMod);
            double AS_KSCD = SpendTimeForCD((float)AS.CastTime / fTransitionMod, (float)KSCD) * .2;
            double AS_CSCD = SpendTimeForCD((float)AS.CastTime / fTransitionMod, (float)CSCD);
            // No Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = AS,
                TargetState = GetState(s2.Flags_CDStates,
                    s2.fightTime.AddSegment(1)),
                TargetState = GetState(s.Name, s.bMMM, s.bISS, s.bChimeraShot, s.FocusGT50, s.bKS, s.fightTime.AddSegment(ssCastTime)),
                TransitionDuration = AS.CastTime / fTransitionMod,
                TransitionProbability = (.5 - (AS_KSCD + AS_CSCD)) * (1 - FocAS),
            });
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = AS,
                TargetState = GetState(s2.Flags_CDStates,
                    s2.fightTime.AddSegment(1)),
                TargetState = GetState(s.Name, s.bMMM, s.bISS, true, s.FocusGT50, s.bKS, s.fightTime.AddSegment(ssCastTime)),
                TransitionDuration = AS.CastTime / fTransitionMod,
                TransitionProbability = AS_CSCD * (1 - FocAS),
            });
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = AS,
                TargetState = GetState(s2.Flags_CDStates,
                    s2.fightTime.AddSegment(1)),
                TargetState = GetState(s.Name, s.bMMM, s.bISS, true, s.FocusGT50, s.bKS, s.fightTime.AddSegment(ssCastTime)),
                TransitionDuration = AS.CastTime / fTransitionMod,
                TransitionProbability = AS_KSCD * (1 - FocAS),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = AS,
                TargetState = GetState(s2.Flags_CDStates,
                    s2.fightTime.AddSegment(1)),
                TargetState = GetState(s.Name, s.bMMM, s.bISS, s.bChimeraShot, false, s.bKS, s.fightTime.AddSegment(ssCastTime)),
                TransitionDuration = AS.CastTime / fTransitionMod,
                TransitionProbability = (.5 - (AS_KSCD + AS_CSCD)) * FocAS,
            });
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = AS,
                TargetState = GetState(s2.Flags_CDStates,
                    s2.fightTime.AddSegment(1)),
                TargetState = GetState(s.Name, s.bMMM, s.bISS, true, false, s.bKS, s.fightTime.AddSegment(ssCastTime)),
                TransitionDuration = AS.CastTime / fTransitionMod,
                TransitionProbability = AS_CSCD * FocAS,
            });
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = AS,
                TargetState = GetState(s2.Flags_CDStates,
                    s2.fightTime.AddSegment(1)),
                TargetState = GetState(s.Name, s.bMMM, s.bISS, true, false, s.bKS, s.fightTime.AddSegment(ssCastTime)),
                TransitionDuration = AS.CastTime / fTransitionMod,
                TransitionProbability = AS_KSCD * FocAS,
            });  
#endif
                #endregion
            } 
            #endregion
            return output;
        }

        #region MM Ability Triggers.
        private void TriggerMMM(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper fAS, HunterState s2)
        {
            const double GCD = 1;
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD((float)GCD, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT] = SpendTimeForCD((float)GCD, CSCD);
            ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT] = SpendTimeForCD((float)GCD, 5f);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(0, fAS.ability.UseTime);

            s2.bMMM = false;
            #region KS No Change
            s2.bKS = s.bKS;
            #region CS No Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion
            #region KS Change
            s2.bKS = true;
            #region CS No Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = fAS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion        

        }

        private void TriggerSteady(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper SS, HunterState s2)
        {
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD(SS.ability.UseTime, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT] = SpendTimeForCD(SS.ability.UseTime, CSCD);
            ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT] = .5;
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(SS.ability.FocusCost, SS.ability.UseTime);
            ProbMatrix[(int)CDStates_Enum.PROC_MMM] = s.talents.MasterMarksman * .2 / 5;

            #region KS No Change
            s2.bKS = s.bKS;
            #region CS No Change
            s2.bChimeraShot = s.bChimeraShot;
            #region ISS No Change
            s2.bISS = s.bISS;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #region ISS Change
            s2.bISS = true;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #endregion
            #region CS Change
            s2.bChimeraShot = true;
            #region ISS No Change
            s2.bISS = s.bISS;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #region ISS Change
            s2.bISS = true;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #endregion            
            #endregion
            #region KS No Change
            s2.bKS = s.bKS;
            #region CS No Change
            s2.bChimeraShot = s.bChimeraShot;
            #region ISS No Change
            s2.bISS = s.bISS;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #region ISS Change
            s2.bISS = true;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #endregion
            #region CS Change
            s2.bChimeraShot = true;
            #region ISS No Change
            s2.bISS = s.bISS;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #region ISS Change
            s2.bISS = true;
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            // MMM No Change
            s2.bMMM = s.bMMM;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            // MMM Change
            s2.bMMM = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = SS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = SS.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_MMM]),
            });
            #endregion
            #endregion
            #endregion
            #endregion

        }

        private void TriggerArcaneShot(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper ArcS, HunterState s2)
        {
            const double GCD = 1;
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD((float)GCD, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT] = SpendTimeForCD((float)GCD, CSCD);
            ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT] = SpendTimeForCD((float)GCD, 5f);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(ArcS.FocusCost, ArcS.ability.UseTime);

            #region KS No Change
            s2.bKS = s.bKS;
            #region CS No Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion        
            #region KS Change
            s2.bKS = true;
            #region CS No Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            #region ISS No Change
            s2.bISS = s.bISS;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion        

        }

        private void TriggerChimera(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper CS, HunterState s2)
        {
            const double GCD = 1;
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD((float)GCD, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT] = SpendTimeForCD((float)GCD, 5f);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(CS.ability.FocusCost, (float)GCD);

            double GCD_ISS = SpendTimeForCD((float)GCD, 5f);
            s2.bChimeraShot = false;
            // But there's a chance that there is 100+ focus.
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bISS = s.bISS;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            });
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            });
            #endregion            
            #region KS Change
            s2.bKS = true;
            s2.bISS = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            });
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            });
            #endregion
            #endregion
            #region FocusChange
            s2.FocusGT50 = false;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bISS = s.bISS;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            });
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            });
            #endregion            
            #region KS Change
            s2.bKS = true;
            s2.bISS = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            });
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = CS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT]),
            }); 
            #endregion
            #endregion
        }

        private void TriggerKillShot(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper KS, HunterState s2)
        {
            const double GCD = 1;

            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT] = SpendTimeForCD((float)GCD, CSCD);
            ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT] = SpendTimeForCD((float)GCD, 5f);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(0, (float)GCD);

            s2.bKS = false;
            #region Focus NoChange
            s2.FocusGT50 = s.FocusGT50;
            #region CS NoChange
            s2.bChimeraShot = s.bChimeraShot;
            #region ISS NoChange
            s2.bISS = s.bISS;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            s2.bChimeraShot = true;
            #region ISS NoChange
            s2.bISS = s.bISS;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT]
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT]
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            #region CS NoChange
            s2.bChimeraShot = s.bChimeraShot;
            #region ISS NoChange
            s2.bISS = s.bISS;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            s2.bChimeraShot = true;
            #region ISS NoChange
            s2.bISS = s.bISS;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT]
                    * (1 - ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bISS = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT]
                    * (ProbMatrix[(int)CDStates_Enum.UP_IMPSTEADYSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion

        }
        #endregion

        /// <summary>
        /// Survival cycle.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private List<StateTransition<AbilWrapper>> GetStateTransitionsSV(State<AbilWrapper> state)
        {
            // This roation assumes Serpent Sting is always up.
            HunterState s = state as HunterState;
            List<StateTransition<AbilWrapper>> output = new List<StateTransition<AbilWrapper>>();

            #region Setup Abilities to use.
            AbilWrapper LNL = AbilityList[typeof(ExplosiveShotLNL)];
            AbilWrapper ES = AbilityList[typeof(ExplosiveShot)];
            AbilWrapper KS = AbilityList[typeof(KillShot)];
            AbilWrapper BA = AbilityList[typeof(BlackArrow)];
            AbilWrapper ArcS = AbilityList[typeof(ArcaneShot)];
            AbilWrapper Cobra = AbilityList[typeof(CobraShot)];
            #endregion

            if (s.talents == null)
                throw new ArgumentNullException();

            HunterState s2 = s.clone();

            #region Shot priority.
            #region Exposive Shot
            if (s.bLNL)
            {
                TriggerLNL(s, output, LNL, s2);
            }
            else if (s.bES && s.FocusGT50) // Explosive shot comes w/ the spec.
            {
                TriggerExplosive(s, output, ES, s2);
            }
            #endregion
            #region Kill Shot
            else if (s.bKS)
            {
                TriggerKillShotSV(s, output, KS, s2);
            }
            #endregion
            #region Black Arrow
            else if (s.bBlackArrow)
            {
                TriggerBlackArrow(s, output, BA, s2);
            }
            #endregion
            #region Focus Dump = Arcane Shot
            else if (s.FocusGT50)
            {
                TriggerArcaneShotSV(s, output, ArcS, s2);
            }
            #endregion
            #region Cobra Shot
            {
                TriggerCobra(s, output, Cobra, s2);
            }
            #endregion
            #endregion

            return output;
        }

        private void TriggerKillShotSV(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper KS, HunterState s2)
        {
            const double GCD = 1;

            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT] = SpendTimeForCD((float)GCD, ESCD);
            ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW] = SpendTimeForCD((float)GCD, BACD);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(0, (float)GCD);

            s2.bKS = false;
            #region Focus NoChange
            s2.FocusGT50 = s.FocusGT50;
            #region CS NoChange
            s2.bChimeraShot = s.bChimeraShot;
            #region BA NoChange
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            s2.bChimeraShot = true;
            #region ISS NoChange
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT]
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT]
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion
            #region Focus Change
            s2.FocusGT50 = true;
            #region CS NoChange
            s2.bChimeraShot = s.bChimeraShot;
            #region ISS NoChange
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            s2.bChimeraShot = true;
            #region ISS NoChange
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT]
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = KS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT]
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion

        }

        private void TriggerLNL(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper ES, HunterState s2)
        {
            // LNL Triggers 2 free ES's but these don't want to stack.
            // So really this should be LNL, Cobra, LNL
            const double GCD = 1;
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT] = SpendTimeForCD((float)GCD, ESCD);
            ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD] = .5f; 
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD((float)GCD, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW] = SpendTimeForCD((float)GCD, BACD);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(ES.ability.FocusCost, (float)GCD);

            #region ES No Change
            s2.bES = s.bES;
            #region LNL No Change
            s2.bLNL = s.bLNL;
            // But there's a chance that there is 100+ focus.
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bBlackArrow = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #region FocusChange
            s2.FocusGT50 = false;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bLNL = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #endregion
            #region LNL Change
            s2.bLNL = false;
            // But there's a chance that there is 100+ focus.
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bBlackArrow = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #region FocusChange
            s2.FocusGT50 = false;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bLNL = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #endregion 
            #endregion
            #region ES Change
            s2.bES = true;
            #region LNL No Change
            s2.bLNL = s.bLNL;
            // But there's a chance that there is 100+ focus.
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bBlackArrow = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #region FocusChange
            s2.FocusGT50 = false;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bLNL = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #endregion
            #region LNL Change
            s2.bLNL = false;
            // But there's a chance that there is 100+ focus.
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bBlackArrow = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #region FocusChange
            s2.FocusGT50 = false;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bLNL = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #endregion
            #endregion
        }

        private void TriggerExplosive(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper ES, HunterState s2)
        {
            const double GCD = 1;
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD((float)GCD, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW] = SpendTimeForCD((float)GCD, BACD);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(ES.ability.FocusCost, (float)GCD);

            s2.bES = false;
            // But there's a chance that there is 100+ focus.
            #region Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bBlackArrow = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
            #region FocusChange
            s2.FocusGT50 = false;
            #region KS No Change
            s2.bKS = s.bKS;
            s2.bBlackArrow = s.bBlackArrow;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            s2.bBlackArrow = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            s2.bBlackArrow = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ES,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW]),
            });
            #endregion
            #endregion
        }

        private void TriggerArcaneShotSV(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper ArcS, HunterState s2)
        {
            const double GCD = 1;
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD((float)GCD, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT] = SpendTimeForCD((float)GCD, ESCD);
            ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW] = SpendTimeForCD((float)GCD, BACD);
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(ArcS.FocusCost, ArcS.ability.UseTime);

            #region KS No Change
            s2.bKS = s.bKS;
            #region ES No Change
            s2.bES = s.bES;
            #region BA No Change
            s2.bBlackArrow = s.bBlackArrow;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region BA Change
            s2.bBlackArrow = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region ES Change
            s2.bES = true;
            #region ISS No Change
            s2.bBlackArrow = s.bBlackArrow;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bBlackArrow = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_CHIMERASHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion
            #region KS Change
            s2.bKS = true;
            #region CS No Change
            #region ISS No Change
            s2.bBlackArrow = s.bBlackArrow;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bBlackArrow = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region CS Change
            #region ISS No Change
            s2.bBlackArrow = s.bBlackArrow;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region ISS Change
            s2.bBlackArrow = false;
            // No Focus state change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = false;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = ArcS,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = GCD,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion

        }

        private void TriggerCobra(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper Cobra, HunterState s2)
        {
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT] = SpendTimeForCD(Cobra.ability.UseTime, CSCD);
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD(Cobra.ability.UseTime, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW] = SpendTimeForCD(Cobra.ability.UseTime, BACD) * .2f;
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(Cobra.ability.FocusCost, Cobra.ability.UseTime);

            #region ES No Change
            s2.bES = s.bES;
            #region KS No Change
            s2.bKS = s.bKS;
            #region BA No Change
            s2.bBlackArrow = s.bBlackArrow;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region BA Change
            s2.bBlackArrow = true;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region KS Change
            s2.bKS = true;
            #region BA No Change
            s2.bBlackArrow = s.bBlackArrow;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region BA Change
            s2.bBlackArrow = true;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion 
            #endregion

            #region ES Change
            s2.bES = true;
            #region KS No Change
            s2.bKS = s.bKS;
            #region BA No Change
            s2.bBlackArrow = s.bBlackArrow;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region BA Change
            s2.bBlackArrow = true;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region KS Change
            s2.bKS = true;
            #region BA No Change
            s2.bBlackArrow = s.bBlackArrow;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region BA Change
            s2.bBlackArrow = true;
            // Focus No Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = Cobra,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = Cobra.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_BLACKARROW])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion
        }

        private void TriggerBlackArrow(HunterState s, List<StateTransition<AbilWrapper>> output, AbilWrapper BA, HunterState s2)
        {
            double[] ProbMatrix = new double[EnumHelper.GetCount(typeof(CDStates_Enum))];
            ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD] = .06 * talents.TNT;
            ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT] = SpendTimeForCD(BA.ability.UseTime, ESCD);
            ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT] = SpendTimeForCD(BA.ability.UseTime, KSCD) * .2f;
            ProbMatrix[(int)CDStates_Enum.FOCUS_LT50] = SpendFocus(BA.ability.FocusCost, BA.ability.UseTime);

            #region LNL No Change
            s2.bLNL = s.bLNL;
            #region ES No Change
            s2.bES = s.bES;
            #region KS No Change
            s2.bKS = s.bKS;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region ES Change
            s2.bES = true;
            #region KS No Change
            s2.bKS = s.bKS;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (1 - ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion
            #region LNL Change
            s2.bLNL = true;
            #region ES No Change
            s2.bES = s.bES;
            #region KS No Change
            s2.bKS = s.bKS;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (1 - ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #region ES Change
            s2.bES = true;
            #region KS No Change
            s2.bKS = s.bKS;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #region KS Change
            s2.bKS = true;
            // Focus No Change
            s2.FocusGT50 = s.FocusGT50;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (1 - ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            // Focus Change
            s2.FocusGT50 = true;
            output.Add(new StateTransition<AbilWrapper>()
            {
                Ability = BA,
                TargetState = GetState(s2.Flags_CDStates),
                TransitionDuration = BA.ability.UseTime,
                TransitionProbability = (ProbMatrix[(int)CDStates_Enum.PROC_LOCKNLOAD])
                    * (ProbMatrix[(int)CDStates_Enum.EXPLOSIVESHOT])
                    * (ProbMatrix[(int)CDStates_Enum.CD_KILLSHOT])
                    * (ProbMatrix[(int)CDStates_Enum.FOCUS_LT50]),
            });
            #endregion
            #endregion
            #endregion
        }
    }
}
