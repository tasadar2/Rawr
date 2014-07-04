﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace Rawr.Tree
{
    public class CalculationOptionsTree : ICalculationOptionBase, INotifyPropertyChanged
    {
        public bool Notify = true;

        private float tankRatio = 0.05f;
        [DefaultValue(0.05f)]
        public float TankRatio
        {
            get { return tankRatio; }
            set { tankRatio = value; OnPropertyChanged("TankRatio"); }
        }

        private float raidBurstRatio = 0.05f;
        [DefaultValue(0.05f)]
        public float RaidBurstRatio
        {
            get { return raidBurstRatio; }
            set { raidBurstRatio = value; OnPropertyChanged("RaidBurstRatio"); }
        }

        private float tankBurstRatio = 0.20f;
        [DefaultValue(0.20f)]
        public float TankBurstRatio
        {
            get { return tankBurstRatio; }
            set { tankBurstRatio = value; OnPropertyChanged("TankBurstRatio"); }
        }

        private bool restoration = true;
        [DefaultValue(true)]
        public bool Restoration
        {
            get { return restoration; }
            set { restoration = value; OnPropertyChanged("Restoration"); }
        }

        private bool timedInnervates = true;
        [DefaultValue(true)]
        public bool TimedInnervates
        {
            get { return timedInnervates; }
            set { timedInnervates = value; OnPropertyChanged("TimedInnervates"); }
        }

        private bool boostIntellectBeforeInnervate = true;
        [DefaultValue(true)]
        public bool BoostIntellectBeforeInnervate
        {
            get { return boostIntellectBeforeInnervate; }
            set { boostIntellectBeforeInnervate = value; OnPropertyChanged("BoostIntellectBeforeInnervate"); }
        }

        private bool innervateOther = false;
        [DefaultValue(false)]
        public bool InnervateOther
        {
            get { return innervateOther; }
            set { innervateOther = value; OnPropertyChanged("InnervateOther"); }
        }

        private int externalInnervates = 0;
        [DefaultValue(0)]
        public int ExternalInnervates
        {
            get { return externalInnervates; }
            set { externalInnervates = value; OnPropertyChanged("ExternalInnervates"); }
        }

        private bool separateHasteEffects = false;
        [DefaultValue(false)]
        public bool SeparateHasteEffects
        {
            get { return separateHasteEffects; }
            set { separateHasteEffects = value; OnPropertyChanged("SeparateHasteEffects"); }
        }

        private float procTriggerInterval = 0.3f;
        [DefaultValue(0.3f)]
        public float ProcTriggerInterval
        {
            get { return procTriggerInterval; }
            set { procTriggerInterval = value; OnPropertyChanged("ProcTriggerInterval"); }
        }

        private float procPeriodicTriggerInterval = 0.3f;
        [DefaultValue(0.3f)]
        public float ProcPeriodicTriggerInterval
        {
            get { return procPeriodicTriggerInterval; }
            set { procPeriodicTriggerInterval = value; OnPropertyChanged("ProcPeriodicTriggerInterval"); }
        }

        private int procTriggerIterations = 1;
        [DefaultValue(1)]
        public int ProcTriggerIterations
        {
            get { return procTriggerIterations; }
            set { procTriggerIterations = value; OnPropertyChanged("ProcTriggerIterations"); }
        }

        private float lifebloomWastedDuration = 0.5f;
        [DefaultValue(0.5f)]
        public float LifebloomWastedDuration
        {
            get { return lifebloomWastedDuration; }
            set { lifebloomWastedDuration = value; OnPropertyChanged("LifebloomWastedDuration"); }
        }

        private float wildGrowthCastDelay = 1.0f;
        [DefaultValue(1.0f)]
        public float WildGrowthCastDelay
        {
            get { return wildGrowthCastDelay; }
            set { wildGrowthCastDelay = value; OnPropertyChanged("WildGrowthCastDelay"); }
        }

        private float swiftmendCastDelay = 1.0f;
        [DefaultValue(1.0f)]
        public float SwiftmendCastDelay
        {
            get { return swiftmendCastDelay; }
            set { swiftmendCastDelay = value; OnPropertyChanged("SwiftmendCastDelay"); }
        }
    
        private float naturesSwiftnessCastDelay = 5.0f;
        [DefaultValue(5.0f)]
        public float NaturesSwiftnessCastDelay
        {
            get { return naturesSwiftnessCastDelay; }
            set { naturesSwiftnessCastDelay = value; OnPropertyChanged("NaturesSwiftnessCastDelay"); }
        }

        private float tranquilityCastDelay = 5.0f;
        [DefaultValue(5.0f)]
        public float TranquilityCastDelay
        {
            get { return tranquilityCastDelay; }
            set { tranquilityCastDelay = value; OnPropertyChanged("TranquilityCastDelay"); }
        }

        private float glyphOfRegrowthExtraDuration = 2.0f;
        [DefaultValue(2.0f)]
        public float GlyphOfRegrowthExtraDuration
        {
            get { return glyphOfRegrowthExtraDuration; }
            set { glyphOfRegrowthExtraDuration = value; OnPropertyChanged("GlyphOfRegrowthExtraDuration"); }
        }

        private bool tankSwiftmend = true;
        [DefaultValue(true)]
        public bool TankSwiftmend
        {
            get { return tankSwiftmend; }
            set { tankSwiftmend = value; OnPropertyChanged("TankSwiftmend"); }
        }

        private bool tankWildGrowth = false;
        [DefaultValue(false)]
        public bool TankWildGrowth
        {
            get { return tankWildGrowth; }
            set { tankWildGrowth = value; OnPropertyChanged("TankWildGrowth"); }
        }

        private bool refreshLifebloomWithDirectHeals = false;
        [DefaultValue(false)]
        public bool RefreshLifebloomWithDirectHeals
        {
            get { return refreshLifebloomWithDirectHeals; }
            set { refreshLifebloomWithDirectHeals = value; OnPropertyChanged("RefreshLifebloomWithDirectHeals"); }
        }

        private bool rejuvenationTankDuringRaid = false;
        [DefaultValue(false)]
        public bool RejuvenationTankDuringRaid
        {
            get { return rejuvenationTankDuringRaid; }
            set { rejuvenationTankDuringRaid = value; OnPropertyChanged("RejuvenationTankDuringRaid"); }
        }

        private float harmonyPeriodicRate = 1.0f;
        [DefaultValue(1.0f)]
        public float HarmonyPeriodicRate
        {
            get { return harmonyPeriodicRate; }
            set { harmonyPeriodicRate = value; OnPropertyChanged("HarmonyPeriodicRate"); }
        }

        private float nourishHoTRate = 0.6f;
        [DefaultValue(0.6f)]
        public float NourishHoTRate
        {
            get { return nourishHoTRate; }
            set { nourishHoTRate = value; OnPropertyChanged("NourishHoTRate"); }
        }

        private float wildGrowthEH = 0.8f;
        [DefaultValue(0.8f)]
        public float WildGrowthEH
        {
            get { return wildGrowthEH; }
            set { wildGrowthEH = value; OnPropertyChanged("WildGrowthEH"); }
        }

        private float tranquilityEH = 1.0f;
        [DefaultValue(1.0f)]
        public float TranquilityEH
        {
            get { return tranquilityEH; }
            set { tranquilityEH = value; OnPropertyChanged("TranquilityEH"); }
        }

        private float swiftmendEH = 1.0f;
        [DefaultValue(1.0f)]
        public float SwiftmendEH
        {
            get { return swiftmendEH; }
            set { swiftmendEH = value; OnPropertyChanged("SwiftmendEH"); }
        }

        private float swiftmendExtraHealEH = 1.0f;
        [DefaultValue(1.0f)]
        public float SwiftmendExtraHealEH
        {
            get { return swiftmendExtraHealEH; }
            set { swiftmendExtraHealEH = value; OnPropertyChanged("SwiftmendExtraHealEH"); }
        }

        private float efflorescenceEH = 0.5f;
        [DefaultValue(0.5f)]
        public float EfflorescenceEH
        {
            get { return efflorescenceEH; }
            set { efflorescenceEH = value; OnPropertyChanged("EfflorescenceEH"); }
        }

        private float livingSeedEH = 0.2f;
        [DefaultValue(0.2f)]
        public float LivingSeedEH
        {
            get { return livingSeedEH; }
            set { livingSeedEH = value; OnPropertyChanged("LivingSeedEH"); }
        }

        private float tankLifebloomEH = 0.7f;
        [DefaultValue(0.7f)]
        public float TankLifebloomEH
        {
            get { return tankLifebloomEH; }
            set { tankLifebloomEH = value; OnPropertyChanged("TankLifebloomEH"); }
        }

        private float toLLifebloomEH = 0.7f;
        [DefaultValue(0.7f)]
        public float ToLLifebloomEH
        {
            get { return toLLifebloomEH; }
            set { toLLifebloomEH = value; OnPropertyChanged("ToLLifebloomEH"); }
        }

        private float rejuvenationEH = 0.7f;
        [DefaultValue(0.7f)]
        public float RejuvenationEH
        {
            get { return rejuvenationEH; }
            set { rejuvenationEH = value; OnPropertyChanged("RejuvenationEH"); }
        }

        private float healingTouchEH = 0.85f;
        [DefaultValue(0.85f)]
        public float HealingTouchEH
        {
            get { return healingTouchEH; }
            set { healingTouchEH = value; OnPropertyChanged("HealingTouchEH"); }
        }

        private float nourishEH = 1.0f;
        [DefaultValue(1.0f)]
        public float NourishEH
        {
            get { return nourishEH; }
            set { nourishEH = value; OnPropertyChanged("NourishEH"); }
        }

        private float raidUnevenlyAllocatedFillerMana = 1.0f;
        [DefaultValue(1.0f)]
        public float RaidUnevenlyAllocatedFillerMana
        {
            get { return raidUnevenlyAllocatedFillerMana; }
            set { raidUnevenlyAllocatedFillerMana = value; OnPropertyChanged("RaidUnevenlyAllocatedFillerMana"); }
        }

        private float tankUnevenlyAllocatedFillerMana = 1.0f;
        [DefaultValue(1.0f)]
        public float TankUnevenlyAllocatedFillerMana
        {
            get { return tankUnevenlyAllocatedFillerMana; }
            set { tankUnevenlyAllocatedFillerMana = value; OnPropertyChanged("TankUnevenlyAllocatedFillerMana"); }
        }

        private float tankRaidHealingWeight = 0.0f;
        [DefaultValue(0.0f)]
        public float TankRaidHealingWeight
        {
            get { return tankRaidHealingWeight; }
            set { tankRaidHealingWeight = value; OnPropertyChanged("TankRaidHealingWeight"); }
        }

        private float activityRate = 1.0f;
        [DefaultValue(1.0f)]
        public float ActivityRate
        {
            get { return activityRate; }
            set { activityRate = value; OnPropertyChanged("ActivityRate"); }
        }

        private bool triggerDamageEffects = false;
        [DefaultValue(false)]
        public bool TriggerDamageEffects
        {
            get { return triggerDamageEffects; }
            set { triggerDamageEffects = value; OnPropertyChanged("TriggerDamageEffects"); }
        }

        #region Stat Graph
        [DefaultValue(new bool[] { true, true, true, true, true, true })]
        public bool[] StatsList { get { return _statsList; } set { _statsList = value; OnPropertyChanged("StatsList"); } }
        private bool[] _statsList = new bool[] { true, true, true, true, true, /**/ true, true, true, true, };
        [DefaultValue(100)]
        public int StatsIncrement { get { return _StatsIncrement; } set { _StatsIncrement = value; OnPropertyChanged("StatsIncrement"); } }
        private int _StatsIncrement = 100;
        [DefaultValue("Overall Rating")]
        public string CalculationToGraph { get { return _calculationToGraph; } set { _calculationToGraph = value; OnPropertyChanged("CalculationToGraph"); } }
        private string _calculationToGraph = "Overall Rating";
        [XmlIgnore]
        public bool SG_Int { get { return StatsList[0]; } set { StatsList[0] = value; OnPropertyChanged("SG_Int"); } }
        [XmlIgnore]
        public bool SG_Spi { get { return StatsList[1]; } set { StatsList[1] = value; OnPropertyChanged("SG_Spi"); } }
        [XmlIgnore]
        public bool SG_SP { get { return StatsList[2]; } set { StatsList[2] = value; OnPropertyChanged("SG_SP"); } }
        [XmlIgnore]
        public bool SG_Crit { get { return StatsList[3]; } set { StatsList[3] = value; OnPropertyChanged("SG_Crit"); } }
        [XmlIgnore]
        public bool SG_Haste { get { return StatsList[4]; } set { StatsList[4] = value; OnPropertyChanged("SG_Haste"); } }
        [XmlIgnore]
        public bool SG_Mstr { get { return StatsList[5]; } set { StatsList[5] = value; OnPropertyChanged("SG_Mstr"); } }
        #endregion

        #region ICalculationOptionBase Overrides
        public string GetXml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(CalculationOptionsTree));
            StringBuilder xml = new StringBuilder();
            StringWriter writer = new StringWriter(xml);
            serializer.Serialize(writer, this);
            return xml.ToString();
        }
        #endregion
        #region INotifyPropertyChanged Members
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
