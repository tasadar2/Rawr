﻿using System;
using System.Collections.Generic;
using System.Text;
#if RAWR3 || RAWR4
using System.Windows.Media;
#else
using System.Drawing;
#endif

namespace Rawr.Mage
{
    #region Helper Classes
    public class Segment
    {
        public int Index { get; set; }
        public double Duration { get; set; }
        public double TimeStart { get; set; }
        public double TimeEnd { get { return TimeStart + Duration; } }
        //public int FirstSpellColumn { get; set; }

        public override string ToString()
        {
            return string.Format("{0} ({1} - {2})", Index, CalculationsMage.TimeFormat(TimeStart), CalculationsMage.TimeFormat(TimeEnd));
        }
    }

    public struct SegmentConstraint
    {
        public int Row;
        public int MinSegment;
        public int MaxSegment;
    }

    public struct ManaSegmentConstraint
    {
        public int Row;
        public int ManaSegment;
    }

    public struct StackingConstraint
    {
        public int Row;
        public EffectCooldown Effect1;
        public EffectCooldown Effect2;
        public double MaximumStackingDuration;
    }

    public struct CombinatorialConstraint
    {
        public int Row;
        public int MinSegment;
        public int MaxSegment;
        public double MinTime;
        public double MaxTime;
    }

    public class EffectCooldown
    {
        public int Mask { get; set; }
        public bool ItemBased { get; set; }
        public StandardEffect StandardEffect { get; set; }
        public SpecialEffect SpecialEffect { get; set; }
        public float Cooldown { get; set; }
        public float Duration { get; set; }
        public bool HasteEffect { get; set; }
        public int Row { get; set; }
        public List<SegmentConstraint> SegmentConstraints { get; set; }
        public string Name { get; set; }
        public float MaximumDuration { get; set; }
        public bool AutomaticConstraints { get; set; }
        public bool AutomaticStackingConstraints { get; set; }
        public Color Color { get; set; }

        public void Clear()
        {
            // Row is always initialized in ConstructRows
            // MaximumDuration is initialized in InitializeEffectCooldowns or in ConstructRows if AutomaticConstraints is true
            if (SegmentConstraints != null)
            {
                SegmentConstraints.Clear();
            }
        }

        public static implicit operator int(EffectCooldown cooldown)
        {
            return cooldown.Mask;
        }

        public EffectCooldown Clone()
        {
            EffectCooldown clone = (EffectCooldown)MemberwiseClone();
            clone.SegmentConstraints = null;
            return clone;
        }
    }

    public enum Specialization
    {
        None,
        Arcane,
        Fire,
        Frost
    }
    #endregion

    public sealed partial class Solver
    {
        #region Variables
        // initialized in constructor
        public Character Character { get; set; }
        public MageTalents MageTalents { get; set; }
        public CalculationOptionsMage CalculationOptions { get; set; }
        private bool segmentCooldowns;
        private bool segmentMana;
        private int advancedConstraintsLevel;
        private bool integralMana;
        private string armor;
        public bool UseIncrementalOptimizations { get; private set; }
        private bool useGlobalOptimizations;
        public bool NeedsDisplayCalculations { get; private set; }
        public bool SolveCycles { get; private set; }
        private bool requiresMIP;
        private bool needsSolutionVariables;
        private bool cancellationPending;
        private bool needsQuadratic;
        public bool CombinatorialSolver { get; private set; }
        public bool SimpleStacking { get; private set; }

        public ArraySet ArraySet { get; set; }

        // initialized in CalculateCycles

        //public float FrBDFFFBIL_KFrB;
        //public float FrBDFFFBIL_KFFB;
        //public float FrBDFFFBIL_KFFBS;
        //public float FrBDFFFBIL_KILS;
        //public float FrBDFFFBIL_KDFS;

        // initialized in Initialize
        public Stats BaseStats { get; set; }
        public List<Buff> ActiveBuffs;
        private List<Buff> autoActivatedBuffs;
        private TargetDebuffStats targetDebuffs;
        //private bool restrictThreat;
        private int baseArmorMask;

        public bool Mage2T10 { get; set; }
        public bool Mage4T10 { get; set; }
        public bool Mage2T11 { get; set; }
        public bool Mage4T11 { get; set; }
        public bool Mage2T12 { get; set; }
        public bool Mage4T12 { get; set; }
        public bool Mage2PVP { get; set; }
        public bool Mage4PVP { get; set; }
        public bool Mage2T13 { get; set; }
        public bool Mage4T13 { get; set; }
        public bool Mage2T14 { get; set; }
        public bool Mage4T14 { get; set; }
        public bool Mage2T15 { get; set; }
        public bool Mage4T15 { get; set; }
        public bool Mage2T16 { get; set; }
        public bool Mage4T16 { get; set; }

        public static readonly SpecialEffect SpecialEffect2T12 = new SpecialEffect(Trigger.MageNukeCast, new Stats() { FireSummonedDamage = 4 * 7016.5f / 15f }, 15f, 45f, 0.2f, 1);
        public static readonly SpecialEffect SpecialEffectCombustion = new SpecialEffect(Trigger.DamageSpellCrit, new Stats() { }, 10f, 45f, 1f);
        public static readonly SpecialEffect SpecialEffectCombustion4T13 = new SpecialEffect(Trigger.DamageSpellCrit, new Stats() { }, 10f, 45f, 1f); // no idea how it works with new combustion
        public static readonly SpecialEffect SpecialEffectCombustion4T14 = new SpecialEffect(Trigger.DamageSpellCrit, new Stats() { }, 10f, 36f, 1f);
        public static readonly SpecialEffect SpecialEffect2T13 = new SpecialEffect(Trigger.MageNukeCast2, new Stats() { HasteRating = 50f }, 30f, 0f, 1f, 10);

        public Specialization Specialization { get; set; }

        public float ManaGemValue;
        public float ManaPotionValue;
        public float MaxManaGemValue;
        public float MaxManaPotionValue;

        private bool heroismAvailable;
        private bool arcanePowerAvailable;
        private bool icyVeinsAvailable;
        private bool combustionAvailable;
        //private bool coldsnapAvailable;
        private bool volcanicPotionAvailable;
        private bool effectPotionAvailable;
        private bool berserkingAvailable;
        private bool mage2T15EffectAvailable;
        private bool waterElementalAvailable;
        private bool mirrorImageAvailable;
        private bool manaGemEffectAvailable;
        private bool powerInfusionAvailable;
        private bool evocationAvailable;
        private bool manaPotionAvailable;
        private bool bloodFuryAvailable;
        private bool mageArmorAvailable;
        private bool moltenArmorAvailable;
        private bool frostArmorAvailable;

        // initialized in InitializeEffectCooldowns
        private const int standardEffectCount = 18; // can't just compute from enum, because that counts the combined masks also
        public List<EffectCooldown> CooldownList { get; set; }
        public Dictionary<int, EffectCooldown> EffectCooldown { get; set; }
        private int[] effectExclusionList;
        private int cooldownCount;
        public float ManaGemEffectDuration;
        private int availableCooldownMask;

        public const float CombustionDuration = 10.0f;
        public const float PowerInfusionDuration = 15.0f;
        public const float PowerInfusionCooldown = 120.0f;
        public const float MirrorImageDuration = 30.0f;
        public const float MirrorImageCooldown = 180.0f;
        public const float ImprovedManaGemDuration = 15.0f;
        public const float ImprovedManaGemCooldown = 120.0f;
        public float IcyVeinsCooldown;
        public float ColdsnapCooldown;
        public float ArcanePowerCooldown;
        public float ArcanePowerDuration;
        public float CombustionCooldown;
        //public float WaterElementalCooldown;
        //public float WaterElementalDuration;
        public float EvocationCooldown;

        public EffectCooldown[] ItemBasedEffectCooldowns { get; set; }
        public int ItemBasedEffectCooldownsCount;
        public EffectCooldown[] StackingHasteEffectCooldowns { get; set; }
        public int StackingHasteEffectCooldownsCount;
        public EffectCooldown[] StackingNonHasteEffectCooldowns { get; set; }
        public int StackingNonHasteEffectCooldownsCount;

        // initialized in InitializeProcEffects
        public SpecialEffect[] SpellPowerEffects { get; set; }
        public int SpellPowerEffectsCount;
        public SpecialEffect[] IntellectEffects { get; set; }
        public int IntellectEffectsCount;
        public SpecialEffect[] StackingIntellectEffects { get; set; }
        public int StackingIntellectEffectsCount;
        public SpecialEffect[] HasteRatingEffects { get; set; }
        public int HasteRatingEffectsCount;
        public SpecialEffect[] DamageProcEffects { get; set; }
        public int DamageProcEffectsCount;
        public SpecialEffect[] ResetStackingEffects { get; set; }
        public int ResetStackingEffectsCount;
        public SpecialEffect[] ManaRestoreEffects { get; set; }
        public int ManaRestoreEffectsCount;
        public SpecialEffect[] Mp5Effects { get; set; }
        public int Mp5EffectsCount;
        public SpecialEffect[] MasteryRatingEffects { get; set; }
        public int MasteryRatingEffectsCount;
        public SpecialEffect[] CritRatingEffects { get; set; }
        public int CritRatingEffectsCount;

        // initialized in CalculateBaseStateStats

        #region Base State Stats
        public float BaseSpellHit { get; set; }
        public float RawArcaneHitRate { get; set; }
        public float RawFireHitRate { get; set; }
        public float RawFrostHitRate { get; set; }
        public float BaseArcaneHitRate { get; set; }
        public float BaseFireHitRate { get; set; }
        public float BaseFrostHitRate { get; set; }
        public float BaseNatureHitRate { get; set; }
        public float BaseShadowHitRate { get; set; }
        public float BaseFrostFireHitRate { get; set; }
        public float BaseHolyHitRate { get; set; }

        public float ArcaneThreatMultiplier { get; set; }
        public float FireThreatMultiplier { get; set; }
        public float FrostThreatMultiplier { get; set; }
        public float NatureThreatMultiplier { get; set; }
        public float ShadowThreatMultiplier { get; set; }
        public float FrostFireThreatMultiplier { get; set; }
        public float HolyThreatMultiplier { get; set; }

        public float BaseSpellModifier { get; set; }
        public float BaseArcaneSpellModifier { get; set; }
        public float BaseFireSpellModifier { get; set; }
        public float BaseFrostSpellModifier { get; set; }
        public float BaseNatureSpellModifier { get; set; }
        public float BaseShadowSpellModifier { get; set; }
        public float BaseFrostFireSpellModifier { get; set; }
        public float BaseHolySpellModifier { get; set; }

        public float BaseAdditiveSpellModifier { get; set; }
        public float BaseArcaneAdditiveSpellModifier { get; set; }
        public float BaseFireAdditiveSpellModifier { get; set; }
        public float BaseFrostAdditiveSpellModifier { get; set; }
        public float BaseNatureAdditiveSpellModifier { get; set; }
        public float BaseShadowAdditiveSpellModifier { get; set; }
        public float BaseFrostFireAdditiveSpellModifier { get; set; }
        public float BaseHolyAdditiveSpellModifier { get; set; }

        public float SpellCritPerInt { get; set; }
        public float BaseCritRate { get; set; }
        public float BaseArcaneCritRate { get; set; }
        public float BaseFireCritRate { get; set; }
        public float BaseFrostCritRate { get; set; }
        public float BaseNatureCritRate { get; set; }
        public float BaseShadowCritRate { get; set; }
        public float BaseFrostFireCritRate { get; set; }
        public float BaseHolyCritRate { get; set; }

        public float BaseArcaneCritBonus { get; set; }
        public float BaseFireCritBonus { get; set; }
        public float BaseFrostCritBonus { get; set; }
        public float BaseNatureCritBonus { get; set; }
        public float BaseShadowCritBonus { get; set; }
        public float BaseFrostFireCritBonus { get; set; }
        public float BaseHolyCritBonus { get; set; }

        public float BaseArcaneSpellPower { get; set; }
        public float BaseFireSpellPower { get; set; }
        public float BaseFrostSpellPower { get; set; }
        public float BaseNatureSpellPower { get; set; }
        public float BaseShadowSpellPower { get; set; }
        public float BaseHolySpellPower { get; set; }

        public float SpiritRegen { get; set; }
        public float FlatManaRegen { get; set; }
        public float BaseManaRegen { get; set; }
        //public float BaseManaRegen5SR { get; set; }
        public float FlatManaRegenDrinking { get; set; }
        public float HealthRegen { get; set; }
        public float HealthRegenCombat { get; set; }
        public float HealthRegenEating { get; set; }
        public float MeleeMitigation { get; set; }
        //public float Defense { get; set; }
        public float PhysicalCritReduction { get; set; }
        public float SpellCritReduction { get; set; }
        public float CritDamageReduction { get; set; }
        public float DamageTakenReduction { get; set; }
        public float Dodge { get; set; }

        public float BaseCastingSpeed { get; set; }
        public float CastingSpeedMultiplier { get; set; }
        public float BaseGlobalCooldown { get; set; }

        public float IncomingDamageAmpMelee { get; set; }
        public float IncomingDamageAmpPhysical { get; set; }
        public float IncomingDamageAmpArcane { get; set; }
        public float IncomingDamageAmpFire { get; set; }
        public float IncomingDamageAmpFrost { get; set; }
        public float IncomingDamageAmpNature { get; set; }
        public float IncomingDamageAmpShadow { get; set; }
        public float IncomingDamageAmpHoly { get; set; }

        public float IncomingDamageDpsMelee { get; set; }
        public float IncomingDamageDpsPhysical { get; set; }
        public float IncomingDamageDpsArcane { get; set; }
        public float IncomingDamageDpsFire { get; set; }
        public float IncomingDamageDpsFrost { get; set; }
        public float IncomingDamageDpsNature { get; set; }
        public float IncomingDamageDpsShadow { get; set; }
        public float IncomingDamageDpsHoly { get; set; }

        public float IncomingDamageDps { get; set; }
        public float IncomingDamageDpsRaw { get; set; }

        public float Mastery { get; set; }
        public float ManaAdeptBonus { get; set; }
        public float ManaAdeptMultiplier { get; set; }
        public float IgniteBonus { get; set; }
        public float IgniteMultiplier { get; set; }
        public float FrostburnBonus { get; set; }
        public float FrostburnMultiplier { get; set; }
        #endregion

        // initialized in InitializeSpellTemplates

        #region Spell Templates
        private WaterboltTemplate _WaterboltTemplate;
        public WaterboltTemplate WaterboltTemplate
        {
            get
            {
                if (_WaterboltTemplate == null)
                {
                    _WaterboltTemplate = new WaterboltTemplate();
                }
                if (_WaterboltTemplate.Dirty)
                {
                    _WaterboltTemplate.Initialize(this);
                }
                return _WaterboltTemplate;
            }
        }

        private MirrorImageTemplate _MirrorImageTemplate;
        public MirrorImageTemplate MirrorImageTemplate
        {
            get
            {
                if (_MirrorImageTemplate == null)
                {
                    _MirrorImageTemplate = new MirrorImageTemplate();
                }
                if (_MirrorImageTemplate.Dirty)
                {
                    _MirrorImageTemplate.Initialize(this);
                }
                return _MirrorImageTemplate;
            }
        }

        private FireBlastTemplate _FireBlastTemplate;
        public FireBlastTemplate FireBlastTemplate
        {
            get
            {
                if (_FireBlastTemplate == null)
                {
                    _FireBlastTemplate = new FireBlastTemplate();
                }
                if (_FireBlastTemplate.Dirty)
                {
                    _FireBlastTemplate.Initialize(this);
                }
                return _FireBlastTemplate;
            }
        }

        private InfernoBlastTemplate _InfernoBlastTemplate;
        public InfernoBlastTemplate InfernoBlastTemplate
        {
            get
            {
                if (_InfernoBlastTemplate == null)
                {
                    _InfernoBlastTemplate = new InfernoBlastTemplate();
                }
                if (_InfernoBlastTemplate.Dirty)
                {
                    _InfernoBlastTemplate.Initialize(this);
                }
                return _InfernoBlastTemplate;
            }
        }

        private FrostboltTemplate _FrostboltTemplate;
        public FrostboltTemplate FrostboltTemplate
        {
            get
            {
                if (_FrostboltTemplate == null)
                {
                    _FrostboltTemplate = new FrostboltTemplate();
                }
                if (_FrostboltTemplate.Dirty)
                {
                    _FrostboltTemplate.Initialize(this);
                }
                return _FrostboltTemplate;
            }
        }

        private CombustionTemplate _CombustionTemplate;
        public CombustionTemplate CombustionTemplate
        {
            get
            {
                if (_CombustionTemplate == null)
                {
                    _CombustionTemplate = new CombustionTemplate();
                }
                if (_CombustionTemplate.Dirty)
                {
                    _CombustionTemplate.Initialize(this);
                }
                return _CombustionTemplate;
            }
        }

        private FrostfireBoltTemplate _FrostfireBoltTemplate;
        public FrostfireBoltTemplate FrostfireBoltTemplate
        {
            get
            {
                if (_FrostfireBoltTemplate == null)
                {
                    _FrostfireBoltTemplate = new FrostfireBoltTemplate();
                }
                if (_FrostfireBoltTemplate.Dirty)
                {
                    _FrostfireBoltTemplate.Initialize(this);
                }
                return _FrostfireBoltTemplate;
            }
        }

        private ArcaneMissilesTemplate _ArcaneMissilesTemplate;
        public ArcaneMissilesTemplate ArcaneMissilesTemplate
        {
            get
            {
                if (_ArcaneMissilesTemplate == null)
                {
                    _ArcaneMissilesTemplate = new ArcaneMissilesTemplate();
                }
                if (_ArcaneMissilesTemplate.Dirty)
                {
                    _ArcaneMissilesTemplate.Initialize(this);
                }
                return _ArcaneMissilesTemplate;
            }
        }

        private NetherTempestTemplate _NetherTempestTemplate;
        public NetherTempestTemplate NetherTempestTemplate
        {
            get
            {
                if (_NetherTempestTemplate == null)
                {
                    _NetherTempestTemplate = new NetherTempestTemplate();
                }
                if (_NetherTempestTemplate.Dirty)
                {
                    _NetherTempestTemplate.Initialize(this);
                }
                return _NetherTempestTemplate;
            }
        }

        private FireballTemplate _FireballTemplate;
        public FireballTemplate FireballTemplate
        {
            get
            {
                if (_FireballTemplate == null)
                {
                    _FireballTemplate = new FireballTemplate();
                }
                if (_FireballTemplate.Dirty)
                {
                    _FireballTemplate.Initialize(this);
                }
                return _FireballTemplate;
            }
        }

        private FrozenOrbTemplate _FrozenOrbTemplate;
        public FrozenOrbTemplate FrozenOrbTemplate
        {
            get
            {
                if (_FrozenOrbTemplate == null)
                {
                    _FrozenOrbTemplate = new FrozenOrbTemplate();
                }
                if (_FrozenOrbTemplate.Dirty)
                {
                    _FrozenOrbTemplate.Initialize(this);
                }
                return _FrozenOrbTemplate;
            }
        }

        private PyroblastTemplate _PyroblastTemplate;
        public PyroblastTemplate PyroblastTemplate
        {
            get
            {
                if (_PyroblastTemplate == null)
                {
                    _PyroblastTemplate = new PyroblastTemplate();
                }
                if (_PyroblastTemplate.Dirty)
                {
                    _PyroblastTemplate.Initialize(this);
                }
                return _PyroblastTemplate;
            }
        }

        private PyroblastHardCastTemplate _PyroblastHardCastTemplate;
        public PyroblastHardCastTemplate PyroblastHardCastTemplate
        {
            get
            {
                if (_PyroblastHardCastTemplate == null)
                {
                    _PyroblastHardCastTemplate = new PyroblastHardCastTemplate();
                }
                if (_PyroblastHardCastTemplate.Dirty)
                {
                    _PyroblastHardCastTemplate.Initialize(this);
                }
                return _PyroblastHardCastTemplate;
            }
        }

        private ScorchTemplate _ScorchTemplate;
        public ScorchTemplate ScorchTemplate
        {
            get
            {
                if (_ScorchTemplate == null)
                {
                    _ScorchTemplate = new ScorchTemplate();
                }
                if (_ScorchTemplate.Dirty)
                {
                    _ScorchTemplate.Initialize(this);
                }
                return _ScorchTemplate;
            }
        }

        private ArcaneBarrageTemplate _ArcaneBarrageTemplate;
        public ArcaneBarrageTemplate ArcaneBarrageTemplate
        {
            get
            {
                if (_ArcaneBarrageTemplate == null)
                {
                    _ArcaneBarrageTemplate = new ArcaneBarrageTemplate();
                }
                if (_ArcaneBarrageTemplate.Dirty)
                {
                    _ArcaneBarrageTemplate.Initialize(this);
                }
                return _ArcaneBarrageTemplate;
            }
        }

        private DeepFreezeTemplate _DeepFreezeTemplate;
        public DeepFreezeTemplate DeepFreezeTemplate
        {
            get
            {
                if (_DeepFreezeTemplate == null)
                {
                    _DeepFreezeTemplate = new DeepFreezeTemplate();
                }
                if (_DeepFreezeTemplate.Dirty)
                {
                    _DeepFreezeTemplate.Initialize(this);
                }
                return _DeepFreezeTemplate;
            }
        }

        private ArcaneBlastTemplate _ArcaneBlastTemplate;
        public ArcaneBlastTemplate ArcaneBlastTemplate
        {
            get
            {
                if (_ArcaneBlastTemplate == null)
                {
                    _ArcaneBlastTemplate = new ArcaneBlastTemplate();
                }
                if (_ArcaneBlastTemplate.Dirty)
                {
                    _ArcaneBlastTemplate.Initialize(this);
                }
                return _ArcaneBlastTemplate;
            }
        }

        private IceLanceTemplate _IceLanceTemplate;
        public IceLanceTemplate IceLanceTemplate
        {
            get
            {
                if (_IceLanceTemplate == null)
                {
                    _IceLanceTemplate = new IceLanceTemplate();
                }
                if (_IceLanceTemplate.Dirty)
                {
                    _IceLanceTemplate.Initialize(this);
                }
                return _IceLanceTemplate;
            }
        }

        private ArcaneExplosionTemplate _ArcaneExplosionTemplate;
        public ArcaneExplosionTemplate ArcaneExplosionTemplate
        {
            get
            {
                if (_ArcaneExplosionTemplate == null)
                {
                    _ArcaneExplosionTemplate = new ArcaneExplosionTemplate();
                }
                if (_ArcaneExplosionTemplate.Dirty)
                {
                    _ArcaneExplosionTemplate.Initialize(this);
                }
                return _ArcaneExplosionTemplate;
            }
        }

        private FlamestrikeTemplate _FlamestrikeTemplate;
        public FlamestrikeTemplate FlamestrikeTemplate
        {
            get
            {
                if (_FlamestrikeTemplate == null)
                {
                    _FlamestrikeTemplate = new FlamestrikeTemplate();
                }
                if (_FlamestrikeTemplate.Dirty)
                {
                    _FlamestrikeTemplate.Initialize(this);
                }
                return _FlamestrikeTemplate;
            }
        }

        private BlizzardTemplate _BlizzardTemplate;
        public BlizzardTemplate BlizzardTemplate
        {
            get
            {
                if (_BlizzardTemplate == null)
                {
                    _BlizzardTemplate = new BlizzardTemplate();
                }
                if (_BlizzardTemplate.Dirty)
                {
                    _BlizzardTemplate.Initialize(this);
                }
                return _BlizzardTemplate;
            }
        }

        private BlastWaveTemplate _BlastWaveTemplate;
        public BlastWaveTemplate BlastWaveTemplate
        {
            get
            {
                if (_BlastWaveTemplate == null)
                {
                    _BlastWaveTemplate = new BlastWaveTemplate();
                }
                if (_BlastWaveTemplate.Dirty)
                {
                    _BlastWaveTemplate.Initialize(this);
                }
                return _BlastWaveTemplate;
            }
        }

        private DragonsBreathTemplate _DragonsBreathTemplate;
        public DragonsBreathTemplate DragonsBreathTemplate
        {
            get
            {
                if (_DragonsBreathTemplate == null)
                {
                    _DragonsBreathTemplate = new DragonsBreathTemplate();
                }
                if (_DragonsBreathTemplate.Dirty)
                {
                    _DragonsBreathTemplate.Initialize(this);
                }
                return _DragonsBreathTemplate;
            }
        }

        private ConeOfColdTemplate _ConeOfColdTemplate;
        public ConeOfColdTemplate ConeOfColdTemplate
        {
            get
            {
                if (_ConeOfColdTemplate == null)
                {
                    _ConeOfColdTemplate = new ConeOfColdTemplate();
                }
                if (_ConeOfColdTemplate.Dirty)
                {
                    _ConeOfColdTemplate.Initialize(this);
                }
                return _ConeOfColdTemplate;
            }
        }

        private SlowTemplate _SlowTemplate;
        public SlowTemplate SlowTemplate
        {
            get
            {
                if (_SlowTemplate == null)
                {
                    _SlowTemplate = new SlowTemplate();
                }
                if (_SlowTemplate.Dirty)
                {
                    _SlowTemplate.Initialize(this);
                }
                return _SlowTemplate;
            }
        }

        private LivingBombTemplate _LivingBombTemplate;
        public LivingBombTemplate LivingBombTemplate
        {
            get
            {
                if (_LivingBombTemplate == null)
                {
                    _LivingBombTemplate = new LivingBombTemplate();
                }
                if (_LivingBombTemplate.Dirty)
                {
                    _LivingBombTemplate.Initialize(this);
                }
                return _LivingBombTemplate;
            }
        }

        private FrostBombTemplate _FrostBombTemplate;
        public FrostBombTemplate FrostBombTemplate
        {
            get
            {
                if (_FrostBombTemplate == null)
                {
                    _FrostBombTemplate = new FrostBombTemplate();
                }
                if (_FrostBombTemplate.Dirty)
                {
                    _FrostBombTemplate.Initialize(this);
                }
                return _FrostBombTemplate;
            }
        }

        private IncantersWardTemplate _IncantersWardTemplate;
        public IncantersWardTemplate IncantersWardTemplate
        {
            get
            {
                if (_IncantersWardTemplate == null)
                {
                    _IncantersWardTemplate = new IncantersWardTemplate();
                }
                if (_IncantersWardTemplate.Dirty)
                {
                    _IncantersWardTemplate.Initialize(this);
                }
                return _IncantersWardTemplate;
            }
        }

        private ConjureManaGemTemplate _ConjureManaGemTemplate;
        public ConjureManaGemTemplate ConjureManaGemTemplate
        {
            get
            {
                if (_ConjureManaGemTemplate == null)
                {
                    _ConjureManaGemTemplate = new ConjureManaGemTemplate();
                }
                if (_ConjureManaGemTemplate.Dirty)
                {
                    _ConjureManaGemTemplate.Initialize(this);
                }
                return _ConjureManaGemTemplate;
            }
        }

        private ArcaneDamageTemplate _ArcaneDamageTemplate;
        public ArcaneDamageTemplate ArcaneDamageTemplate
        {
            get
            {
                if (_ArcaneDamageTemplate == null)
                {
                    _ArcaneDamageTemplate = new ArcaneDamageTemplate();
                }
                if (_ArcaneDamageTemplate.Dirty)
                {
                    _ArcaneDamageTemplate.Initialize(this);
                }
                return _ArcaneDamageTemplate;
            }
        }

        private FireDamageTemplate _FireDamageTemplate;
        public FireDamageTemplate FireDamageTemplate
        {
            get
            {
                if (_FireDamageTemplate == null)
                {
                    _FireDamageTemplate = new FireDamageTemplate();
                }
                if (_FireDamageTemplate.Dirty)
                {
                    _FireDamageTemplate.Initialize(this);
                }
                return _FireDamageTemplate;
            }
        }

        private FrostDamageTemplate _FrostDamageTemplate;
        public FrostDamageTemplate FrostDamageTemplate
        {
            get
            {
                if (_FrostDamageTemplate == null)
                {
                    _FrostDamageTemplate = new FrostDamageTemplate();
                }
                if (_FrostDamageTemplate.Dirty)
                {
                    _FrostDamageTemplate.Initialize(this);
                }
                return _FrostDamageTemplate;
            }
        }

        private ShadowDamageTemplate _ShadowDamageTemplate;
        public ShadowDamageTemplate ShadowDamageTemplate
        {
            get
            {
                if (_ShadowDamageTemplate == null)
                {
                    _ShadowDamageTemplate = new ShadowDamageTemplate();
                }
                if (_ShadowDamageTemplate.Dirty)
                {
                    _ShadowDamageTemplate.Initialize(this);
                }
                return _ShadowDamageTemplate;
            }
        }

        private NatureDamageTemplate _NatureDamageTemplate;
        public NatureDamageTemplate NatureDamageTemplate
        {
            get
            {
                if (_NatureDamageTemplate == null)
                {
                    _NatureDamageTemplate = new NatureDamageTemplate();
                }
                if (_NatureDamageTemplate.Dirty)
                {
                    _NatureDamageTemplate.Initialize(this);
                }
                return _NatureDamageTemplate;
            }
        }

        private HolyDamageTemplate _HolyDamageTemplate;
        public HolyDamageTemplate HolyDamageTemplate
        {
            get
            {
                if (_HolyDamageTemplate == null)
                {
                    _HolyDamageTemplate = new HolyDamageTemplate();
                }
                if (_HolyDamageTemplate.Dirty)
                {
                    _HolyDamageTemplate.Initialize(this);
                }
                return _HolyDamageTemplate;
            }
        }

        private HolySummonedDamageTemplate _HolySummonedDamageTemplate;
        public HolySummonedDamageTemplate HolySummonedDamageTemplate
        {
            get
            {
                if (_HolySummonedDamageTemplate == null)
                {
                    _HolySummonedDamageTemplate = new HolySummonedDamageTemplate();
                }
                if (_HolySummonedDamageTemplate.Dirty)
                {
                    _HolySummonedDamageTemplate.Initialize(this);
                }
                return _HolySummonedDamageTemplate;
            }
        }

        private FireSummonedDamageTemplate _FireSummonedDamageTemplate;
        public FireSummonedDamageTemplate FireSummonedDamageTemplate
        {
            get
            {
                if (_FireSummonedDamageTemplate == null)
                {
                    _FireSummonedDamageTemplate = new FireSummonedDamageTemplate();
                }
                if (_FireSummonedDamageTemplate.Dirty)
                {
                    _FireSummonedDamageTemplate.Initialize(this);
                }
                return _FireSummonedDamageTemplate;
            }
        }

        private WandTemplate _WandTemplate;
        public WandTemplate WandTemplate
        {
            get
            {
                if (_WandTemplate == null)
                {
                    _WandTemplate = new WandTemplate();
                }
                return _WandTemplate;
            }
        }

        #endregion

        // initialized in GenerateStateList
        private List<CastingState> stateList;
        private List<CastingState> scratchStateList = new List<CastingState>();
        public CastingState BaseState { get; set; }

        // initialized in GenerateSpellList
        private List<CycleId> spellList;

        // initialized in ConstructProblem
        private bool segmentNonCooldowns;
        private bool minimizeTime;
        private bool restrictManaUse;
        private bool needsTimeExtension;
        private bool conjureManaGem;
        private float dpsTime;
        public int manaSegments;

        private SolverLP lp;
        private int[] segmentColumn;
        public List<SolutionVariable> SolutionVariable { get; set; }

        private const double ManaRegenLPScaling = 0.001;
        public float StartingMana { get; set; }
        public double MaxDrinkingTime { get; set; }
        public bool DrinkingEnabled { get; set; }

        public Cycle ConjureManaGem { get; set; }
        public int MaxConjureManaGem { get; set; }

        public float MaxEvocation;
        public float EvocationDuration;
        public float EvocationRegen;
        public float EvocationDurationIV;
        public float EvocationRegenIV;
        public float EvocationDurationHero;
        public float EvocationRegenHero;
        public float EvocationDurationIVHero;
        public float EvocationRegenIVHero;

        public int MaxManaGem;
        public float ManaGemTps;
        public float ManaPotionTps;        

        // initialized in ConstructSegments
        public List<Segment> SegmentList { get; set; }

        // initialized in ConstructRows
        private StackingConstraint[] rowStackingConstraint;
        private int rowStackingConstraintCount;

        private CombinatorialConstraint[] rowCombinatorialConstraint;
        private int rowCombinatorialConstraintCount;

        #region LP rows
        private int rowManaRegen;
        private int rowFightDuration;
        private int rowEvocation;
        private int rowEvocationIV;
        private int rowEvocationHero;
        private int rowEvocationIVHero;
        private int rowPotion;
        private int rowManaPotion;
        private int rowConjureManaGem;
        private int rowManaGem;
        private int rowManaGemMax;
        private int rowHeroism;
        private int rowArcanePower;
        private int rowIcyVeins;
        //private int rowWaterElemental;
        private int rowMirrorImage;
        //private int rowMoltenFury;
        //private int rowMoltenFuryIcyVeins;
        private int rowManaGemEffect;
        private int rowManaGemEffectActivation;
        private int rowDpsTime;
        private int rowAoe;
        //private int rowFlamestrike;
        //private int rowConeOfCold;
        //private int rowBlastWave;
        //private int rowDragonsBreath;
        private int rowCombustion;
        private int rowPowerInfusion;
        //private int rowFlameOrb;
        //private int rowMoltenFuryCombustion;
        //private int rowHeroismCombustion;
        private int rowHeroismIcyVeins;
        //private int rowSummonWaterElemental;
        //private int rowSummonWaterElementalCount;
        private int rowSummonMirrorImage;
        private int rowSummonMirrorImageCount;
        //private int rowThreat;
        private int rowBerserking;
        private int rowBloodFury;
        private int rowTimeExtension;
        private int rowAfterFightRegenMana;
        private int rowTargetDamage;
        private int rowSegment;
        private int rowSegmentManaOverflow;
        private int rowSegmentManaUnderflow;
        //private int rowSegmentThreat;
        private List<SegmentConstraint> rowSegmentArcanePower;
        private List<SegmentConstraint> rowSegmentPowerInfusion;
        private List<SegmentConstraint> rowSegmentIcyVeins;
        //private List<SegmentConstraint> rowSegmentWaterElemental;
        //private List<SegmentConstraint> rowSegmentSummonWaterElemental;
        private List<SegmentConstraint> rowSegmentMirrorImage;
        private List<SegmentConstraint> rowSegmentSummonMirrorImage;
        private List<SegmentConstraint> rowSegmentCombustion;
        private List<SegmentConstraint> rowSegmentBerserking;
        private List<SegmentConstraint> rowSegmentBloodFury;
        private List<SegmentConstraint> rowSegmentManaGem;
        private List<SegmentConstraint> rowSegmentManaGemEffect;
        private List<SegmentConstraint> rowSegmentEvocation;
        private List<ManaSegmentConstraint> rowManaSegment;
        private bool needsManaSegmentConstraints;
        #endregion

        // initialized in RestrictSolution
        private double[] solution;
        private double lowerBound;
        private double upperBound;

        // initialized in EvaluateSurvivability
        public float ChanceToDie { get; set; }
        public float MeanIncomingDps { get; set; }
        #endregion

        #region Public Methods and Properties
        public TargetDebuffStats TargetDebuffs
        {
            get
            {
                if (targetDebuffs == null)
                {
                    targetDebuffs = new TargetDebuffStats();
                    foreach (Buff buff in ActiveBuffs)
                    {
                        if (buff.IsTargetDebuff)
                        {
                            targetDebuffs.Accumulate(buff.Stats);
                        }
                    }
                }
                return targetDebuffs;
            }
        }

        internal bool CancellationPending
        {
            get
            {
                return cancellationPending;
            }
        }

        public void CancelAsync()
        {
            cancellationPending = true;
            if (stackingOptimizer != null)
            {
                stackingOptimizer.CancelAsync();
            }
        }

        public List<EffectCooldown> GetEffectList(int effects)
        {
            return CooldownList.FindAll(effect => (effects & effect.Mask) == effect.Mask);
        }

        public string EffectsDescription(int effects)
        {
            List<string> buffList = new List<string>();
            List<EffectCooldown> cooldownList = CooldownList;
            for (int i = 0; i < cooldownList.Count; i++)
            {
                EffectCooldown effect = cooldownList[i];
                if ((effects & effect.Mask) == effect.Mask)
                {
                    buffList.Add(effect.Name);
                }
            }
            return string.Join("+", buffList.ToArray());
        }

        private static bool IsItemActivatable(ItemInstance item)
        {
            if (item == null || item.Item == null) return false;
            return (item.Item.Stats.ContainsSpecialEffect(effect => effect.Trigger == Trigger.Use));
        }
        #endregion

        public Solver(Character character, CalculationOptionsMage calculationOptions, bool segmentCooldowns, bool segmentMana, bool integralMana, int advancedConstraintsLevel, string armor, bool useIncrementalOptimizations, bool useGlobalOptimizations, bool needsDisplayCalculations, bool needsSolutionVariables, bool solveCycles, bool combinatorialSolver, bool simpleStacking)
        {
            Construct(character, calculationOptions, segmentCooldowns, segmentMana, integralMana, advancedConstraintsLevel, armor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables, solveCycles, combinatorialSolver, simpleStacking);
        }

        private void Construct(Character character, CalculationOptionsMage calculationOptions, bool segmentCooldowns, bool segmentMana, bool integralMana, int advancedConstraintsLevel, string armor, bool useIncrementalOptimizations, bool useGlobalOptimizations, bool needsDisplayCalculations, bool needsSolutionVariables, bool solveCycles, bool combinatorialSolver, bool simpleStacking)
        {
            this.Character = character;
            this.MageTalents = character.MageTalents;
            this.CalculationOptions = calculationOptions;
            this.segmentCooldowns = segmentCooldowns;
            this.segmentMana = segmentMana;
            this.advancedConstraintsLevel = advancedConstraintsLevel;
            this.integralMana = integralMana;
            this.armor = armor;
            this.UseIncrementalOptimizations = useIncrementalOptimizations;
            this.useGlobalOptimizations = useGlobalOptimizations;
            this.NeedsDisplayCalculations = needsDisplayCalculations;
            this.SolveCycles = solveCycles;
            this.requiresMIP = segmentCooldowns || integralMana || (segmentMana && advancedConstraintsLevel > 0);
            if (needsDisplayCalculations || requiresMIP) needsSolutionVariables = true;
            this.needsSolutionVariables = needsSolutionVariables;
            this.needsQuadratic = false;
            this.needsManaSegmentConstraints = segmentMana && !segmentCooldowns && advancedConstraintsLevel >= 1 && useIncrementalOptimizations;
            this.CombinatorialSolver = combinatorialSolver;
            this.SimpleStacking = simpleStacking;
            if (combinatorialSolver)
            {
                this.requiresMIP = false;
                this.UseIncrementalOptimizations = false;
                this.needsManaSegmentConstraints = false;
            }
            cancellationPending = false;
        }

        [ThreadStatic]
        private static Solver threadSolver;

        public static CharacterCalculationsMage GetCharacterCalculations(Character character, Item additionalItem, CalculationOptionsMage calculationOptions, CalculationsMage calculations, string armor, bool segmentCooldowns, bool segmentMana, bool integralMana, int advancedConstraintsLevel, bool useIncrementalOptimizations, bool useGlobalOptimizations, bool needsDisplayCalculations, bool needsSolutionVariables, bool solveCycles, bool combinatorialSolver, bool simpleStacking)
        {
            if (needsDisplayCalculations)
            {
                // if we need display calculations then solver data has to remain clean because calls from display calculations
                // that generate spell/cycle tooltips use that data (for example otherwise mage armor solver gets overwritten with molten armor solver data and we get bad data)
                var displaySolver = new Solver(character, calculationOptions, segmentCooldowns, segmentMana, integralMana, advancedConstraintsLevel, armor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables, solveCycles, combinatorialSolver, simpleStacking);
                return displaySolver.GetCharacterCalculations(additionalItem);
            }
            if (threadSolver == null)
            {
                threadSolver = new Solver(character, calculationOptions, segmentCooldowns, segmentMana, integralMana, advancedConstraintsLevel, armor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables, solveCycles, combinatorialSolver, simpleStacking);
            }
            else
            {
                threadSolver.Construct(character, calculationOptions, segmentCooldowns, segmentMana, integralMana, advancedConstraintsLevel, armor, useIncrementalOptimizations, useGlobalOptimizations, needsDisplayCalculations, needsSolutionVariables, solveCycles, combinatorialSolver, simpleStacking);
            }
            return threadSolver.GetCharacterCalculations(additionalItem);
        }

        public CharacterCalculationsMage GetCharacterCalculations(Item additionalItem)
        {
            ArraySet = ArrayPool.RequestArraySet(!UseIncrementalOptimizations && NeedsDisplayCalculations && segmentCooldowns);

            Initialize(additionalItem);

            GenerateSpellList();
            GenerateStateList();

            CalculateCycles();

            SetCalculationReuseReferences();

            CalculateStartingMana();

            CharacterCalculationsMage ret;

            if (CombinatorialSolver)
            {
                if (SimpleStacking)
                {
                    ret = SolveSimpleStackingProblem();
                }
                else if (!string.IsNullOrEmpty(CalculationOptions.CombinatorialFixedOrdering))
                {
                    ret = SolveCombinatorialFixedProblem();
                }
                else if (CalculationOptions.GeneticSolver)
                {
                    ret = SolveGeneticCombinatorialProblem();
                }
                else
                {
                    ret = SolveCombinatorialProblem();
                }
            }
            else
            {
                SolveBasicProblem();
                ret = GetCalculationsResult();
            }

            ArrayPool.ReleaseArraySet(ArraySet);
            ArraySet = null;

            return ret;
        }

        private class CombList : List<CombItem>
        {
            public int[] Generator;

            public CombList(List<EffectCooldown> effects, List<CastingState> states, Solver solver)
            {
                this.effects = effects;
                this.states = new Dictionary<int, CastingState>();
                foreach (var s in states)
                {
                    this.states[s.Effects] = s;
                }
                this.solver = solver;
                foreach (EffectCooldown cooldown in effects)
                {
                    if (cooldown.Cooldown > 0 && cooldown.Duration > 0)
                    {
                        cooldown.MaximumDuration = (float)MaximizeEffectDuration(solver.CalculationOptions.FightDuration, cooldown.Duration, cooldown.Cooldown);
                    }
                }
            }

            internal List<EffectCooldown> effects;
            internal Dictionary<int, CastingState> states;
            internal Solver solver;

            public bool IsActive(int cooldown)
            {
                return IsActive(cooldown, Count);
            }

            public bool IsActive(int cooldown, int index)
            {
                /*bool active = false;
                for (int i = 0; i < index; i++)
                {
                    if (this[i].Cooldown == cooldown)
                    {
                        active = this[i].Activation;
                    }
                }
                return active;*/
                if (index == 0) return false;
                return (this[index - 1].Effects & effects[cooldown].Mask) != 0;
            }

            public void UpdateCastingStates()
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i].UpdateCastingState(this);
                }
            }

            public void UpdateEffectsAndCastingStates()
            {
                for (int i = 0; i < Count; i++)
                {
                    UpdateEffects(i);                    
                    this[i].UpdateCastingState(this);
                    this[i].UpdateDuration(this);
                }
            }

            public void UpdateMinTime(int index)
            {
                int cooldown = this[index].Cooldown;
                bool act = this[index].Activation;
                double min = 0;
                for (int i = 0; i < index; i++)
                {
                    if (this[i].Cooldown == cooldown)
                    {
                        if (this[i].Activation && act)
                        {
                            min = Math.Max(min, this[i].MinTime + effects[cooldown].Cooldown);
                        }
                        else if (this[i].Activation && !act)
                        {                          
                            min = Math.Max(min, this[i].MinTime + this[i].Duration);
                        }
                        else
                        {
                            min = Math.Max(min, this[i].MinTime);
                        }
                    }
                    else
                    {
                        min = Math.Max(min, this[i].MinTime);
                    }
                }
                this[index].MinTime = min;
            }

            public bool UpdateEffects(int index)
            {
                int effect = 0;                
                /*for (int i = 0; i <= index; i++)
                {
                    if (this[i].Activation)
                    {
                        effect = effect | effects[this[i].Cooldown].Mask;
                    }
                    else
                    {
                        effect = effect & ~effects[this[i].Cooldown].Mask;
                    }
                }*/
                if (index > 0)
                {
                    effect = this[index - 1].Effects;
                }
                var cur = this[index];
                if (!(effects[cur.Cooldown].StandardEffect == StandardEffect.ManaGemEffect && !solver.manaGemEffectAvailable)) // special casing for mana gem handling
                {
                    if (cur.Activation)
                    {
                        // during evocation we can't activate anything
                        if ((effect & (int)StandardEffect.Evocation) != 0)
                        {
                            cur.Effects = -1;
                            return false;
                        }
                        effect = effect | effects[cur.Cooldown].Mask;
                    }
                    else
                    {
                        effect = effect & ~effects[cur.Cooldown].Mask;
                    }
                }
                

                bool valid = true;
                foreach (int exclusionMask in solver.effectExclusionList)
                {
                    if (solver.BitCount2(effect & exclusionMask))
                    {
                        valid = false;
                        break;
                    }
                }

                cur.CastingState = null;
                if (valid)
                {
                    /*CastingState s;
                    if (!states.TryGetValue(effect, out s))
                    {
                        s = CastingState.New(solver, effect, false, 0);
                        states[effect] = s;
                    }*/
                    cur.Effects = effect;
                    return true;
                }
                else
                {
                    cur.Effects = -1;
                    return false;
                }
            }

            public bool IsFeasible(int[] activationCount)
            {
                // make sure we use up all abilities
                for (int index = 0; index < effects.Count; index++)
                {
                    EffectCooldown cooldown = effects[index];
                    if (cooldown.Cooldown > 0 && cooldown.Duration > 0)
                    {
                        double max;
                        if (float.IsPositiveInfinity(cooldown.Cooldown))
                        {
                            max = 1;
                        }
                        else
                        {
                            max = Math.Ceiling(solver.CalculationOptions.FightDuration / cooldown.Cooldown);
                        }
                        if (activationCount[index] < max)
                        {
                            return false;
                        }
                    }
                }
                return true;
            }

            public bool IsPartialFeasible(int[] lastActivation, int[] activationCount)
            {
                // make sure that from last activation till last event there's no more than double cooldown duration
                // if it is we could have filled one more activation in without affecting anything
                // each activation must have a deactivation before an entry with higher min time
                for (int index = 0; index < effects.Count; index++)
                {
                    EffectCooldown cooldown = effects[index];
                    if (cooldown.Cooldown > 0 && cooldown.Duration > 0)
                    {
                        if (lastActivation[index] == -1)
                        {
                            if (this[Count - 1].MinTime >= cooldown.Cooldown)
                            {
                                return false;
                            }
                        }
                        else
                        {            
                            // make sure we can reach feasible
                            double act = this[lastActivation[index]].MinTime;
                            double delta = this[Count - 1].MinTime - act;
                            if (!float.IsPositiveInfinity(cooldown.Cooldown))
                            {
                                double max = Math.Ceiling(solver.CalculationOptions.FightDuration / cooldown.Cooldown);
                                double nextact = Math.Max(this[Count - 1].MinTime, act + cooldown.Cooldown);
                                double remaining = Math.Ceiling((solver.CalculationOptions.FightDuration - nextact) / cooldown.Cooldown);
                                if (activationCount[index] + remaining < max)
                                {
                                    return false;
                                }
                            }

                            // only need to check for matching deactivation if we just passed the duration, if we were past it already there's no need to check again
                            if (delta >= cooldown.Duration)
                            {
                                if (Count < 2 || this[Count - 2].MinTime - act < cooldown.Duration)
                                {
                                    // find deactivation
                                    bool found = false;
                                    for (int j = lastActivation[index] + 1; j < Count; j++)
                                    {
                                        if (this[j].Cooldown == index && !this[j].Activation)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                }
                return true;
            }

            public override string ToString()
            {
                return string.Join(",", this);
            }
        }

        private class CombItem
        {
            public int Cooldown;
            public bool Activation;
            public double MinTime;
            public bool Generated;
            public CastingState CastingState;
            public double Duration;
            public int LastActivation;
            public int Effects;
            public bool ZeroDuration;
            public int StateIndex;

            public void UpdateDuration(CombList list)
            {
                int cooldown = Cooldown;
                EffectCooldown effect = list.effects[cooldown];
                if (effect.StandardEffect == StandardEffect.Evocation)
                {
                    UpdateCastingState(list);
                    Duration = 6.0 / CastingState.CastingSpeed;
                    if (CastingState.MageTalents.Invocation > 0)
                    {
                        Duration /= 2;
                    }
                }
                else
                {
                    Duration = effect.Duration;
                }
            }

            public void UpdateCastingState(CombList list)
            {
                if (CastingState == null)
                {
                    CastingState s;
                    if (!list.states.TryGetValue(Effects, out s))
                    {
                        s = CastingState.New(list.solver, Effects, false, 0);
                        list.states[Effects] = s;
                    }
                    CastingState = s;
                }
            }

            public override string ToString()
            {
                return Cooldown.ToString();
            }
        }

        public class CombState
        {
            public CastingState CastingState;
            public int OverflowConstraint;
            public int UnderflowConstraint;
            public int Segment;
            public int ManaSegment;
            public bool ManaGemActivation;
        }

        private CombList combList;
        private List<CombState> combStateList;

        private void ConstructCombStateList()
        {
            combStateList = new List<CombState>();
            combStateList.Add(new CombState() { CastingState = BaseState, Segment = 0, ManaSegment = 0 });
            int start = 0;
            int ms = 0;
            for (int i = 0; i < combList.Count; i++)
            {
                if (!combList[i].ZeroDuration)
                {
                    combStateList.Add(new CombState() { CastingState = combList[i].CastingState, Segment = 0, ManaSegment = ++ms });
                    for (int j = start; j <= i; j++)
                    {
                        combList[j].StateIndex = ms;
                        if (combList[j].Activation && combList.effects[combList[j].Cooldown].StandardEffect == StandardEffect.ManaGemEffect)
                        {
                            combStateList[ms].ManaGemActivation = true;
                        }
                    }
                    start = i + 1;
                }
            }
        }

        private class CooldownOptimizer : Optimizer.OptimizerBase<int, CombList, CharacterCalculationsMage>
        {
            private Solver solver;
            private int[] maxSlots;

            public CooldownOptimizer(Solver solver)
            {
                this.solver = solver;
                ThreadPoolValuation = false;
                slotCount = 0;
                _thoroughness = solver.CalculationOptions.GeneticThoroughness;
                validators = new List<Optimizer.OptimizerRangeValidatorBase<int>>();

                maxSlots = new int[solver.CooldownList.Count];
                for (int i = 0; i < maxSlots.Length; i++)
                {
                    int max = (int)Math.Ceiling(solver.CalculationOptions.FightDuration / solver.CooldownList[i].Cooldown);
                    if (float.IsPositiveInfinity(solver.CooldownList[i].Cooldown))
                    {
                        max = 1;
                    }
                    maxSlots[i] = 2 * max;
                    slotCount += maxSlots[i];
                }
                slotCount++; // ending marker
            }

            protected override CombList BuildRandomIndividual(CombList recycledIndividual)
            {
                int[] countSlots = new int[maxSlots.Length];
                int[] gen = new int[slotCount];
                int end = Rnd.Next((int)(0.8 * slotCount), slotCount);
                for (int i = 0; i < gen.Length; i++)
                {
                    if (i == end)
                    {
                        gen[i] = -1;
                    }
                    else
                    {
                        int slot;
                        do
                        {
                            slot = Rnd.Next(countSlots.Length);
                        } while (countSlots[slot] >= maxSlots[slot]);
                        gen[i] = slot;
                        countSlots[slot]++;
                    }
                }

                return GenerateIndividual(gen, true, recycledIndividual);
            }

            protected override int GetRandomItem(int slot, int[] items)
            {
                return Rnd.Next(solver.CooldownList.Count);
            }

            protected override CombList BuildMutantIndividual(CombList parent, CombList recycledIndividual)
            {
                List<int> gen = new List<int>(GetItems(parent));

                if (gen.Count > 0)
                {
                    int max = Rnd.Next(5);
                    for (int i = 0; i < max; i++)
                    {
                        int index = Rnd.Next(gen.Count);
                        int v = gen[index];
                        gen.RemoveAt(index);
                        index += (Rnd.Next(5) - 2);
                        if (index < 0) index = 0;
                        if (index > gen.Count) index = gen.Count;
                        gen.Insert(index, v);
                    }
                }

                return GenerateIndividual(gen.ToArray(), true, recycledIndividual);
            }

            protected override CombList BuildChildIndividual(CombList father, CombList mother, CombList recycledIndividual)
            {
                int[] f = GetItems(father);
                int[] m = GetItems(mother);

                if (Rnd.Next(2) == 1)
                {
                    var tmp = f;
                    f = m;
                    m = tmp;
                }

                // select a section from first parent, eliminate those from second
                // what remains is things in front, a scattering remaining in between, and those at the end
                // pick randomly a spot in the scattering, this gives the final placement in offspring

                int i1 = Rnd.Next(f.Length);
                int i2 = Rnd.Next(f.Length);

                if (i2 < i1)
                {
                    var tmp = i1;
                    i1 = i2;
                    i2 = tmp;
                }

                bool[] used = new bool[slotCount];
                int[] count = new int[solver.CooldownList.Count];

                for (int i = 0; i < i1; i++)
                {
                    if (f[i] >= 0)
                    {
                        count[f[i]]++;
                    }
                }

                // potentially make an index adjustment so we can do time based transplants
                //if (Rnd.NextDouble() < 0.2)
                {
                    int[] count2 = new int[solver.CooldownList.Count];
                    for (int i = 0; i <= i2; i++)
                    {
                        if (f[i] >= 0)
                        {
                            count2[f[i]]++;
                        }
                    }

                    // pick a random cooldown
                    int c = Rnd.Next(solver.CooldownList.Count);

                    if (!float.IsPositiveInfinity(solver.CooldownList[c].Cooldown))
                    {
                        // choose a shift

                        int min = -(count[c] / 2);
                        int max = (maxSlots[c] - count2[c]) / 2;
                        int shift = Rnd.Next(min, max + 1);
                        double tshift = shift * solver.CooldownList[c].Cooldown;

                        // shift
                        for (int i = 0; i < solver.CooldownList.Count; i++)
                        {
                            if (i == c)
                            {
                                count[c] += 2 * shift;
                            }
                            else
                            {
                                if (!float.IsPositiveInfinity(solver.CooldownList[i].Cooldown))
                                {
                                    double d = tshift / solver.CooldownList[i].Cooldown;
                                    int dd;
                                    if (Rnd.Next(2) == 0)
                                    {
                                        dd = (int)Math.Floor(d);
                                    }
                                    else
                                    {
                                        dd = (int)Math.Ceiling(d);
                                    }
                                    min = -(count[i] / 2);
                                    max = (maxSlots[i] - count2[i]) / 2;
                                    if (dd < min) dd = min;
                                    if (dd > max) dd = max;
                                    count[i] += 2 * dd;
                                }
                            }
                        }
                    }
                }

                // start the markings
                for (int i = i1; i <= i2; i++)
                {
                    int c = f[i];
                    if (c >= 0)
                    {
                        int d = count[c];
                        // find the right match
                        for (int j = 0; j < m.Length; j++)
                        {
                            if (m[j] == c)
                            {
                                if (d == 0)
                                {
                                    used[j] = true;
                                    break;
                                }
                                else
                                {
                                    d--;
                                }
                            }
                        }
                        count[c]++;
                    }
                    else
                    {
                        for (int j = 0; j < m.Length; j++)
                        {
                            if (m[j] == c)
                            {
                                used[j] = true;
                                break;
                            }
                        }
                    }
                }

                int j1 = Array.IndexOf(used, true);
                int j2 = Array.LastIndexOf(used, true);

                int s = Rnd.Next(j1, j2);

                // splice

                int[] gen = new int[slotCount];
                int index = 0;
                for (int j = 0; j < s; j++)
                {
                    if (!used[j])
                    {
                        gen[index++] = m[j];
                    }
                }
                for (int i = i1; i <= i2; i++)
                {
                    gen[index++] = f[i];
                }
                for (int j = s; j < slotCount; j++)
                {
                    if (!used[j])
                    {
                        gen[index++] = m[j];
                    }
                }

                return GenerateIndividual(gen, true, recycledIndividual);
            }
            
            protected override void ReportProgress(int progressPercentage, float bestValue)
            {
                CalculationsMage.ClearLog(solver, progressPercentage + ": " + bestValue);
            }

            protected override void LookForDirectItemUpgrades()
            {
                // no direct upgrades
            }            

            protected override CharacterCalculationsMage GetValuation(CombList individual)
            {
                individual.UpdateCastingStates();
                solver.combList = individual;
                solver.ConstructCombStateList();
                solver.SolveBasicProblem();
                return solver.GetCalculationsResult();
            }

            protected override float GetOptimizationValue(CombList individual, CharacterCalculationsMage valuation)
            {
                return valuation.OverallPoints;
            }

            protected override int GetItem(CombList individual, int slot)
            {
                if (slot < individual.Count)
                {
                    return individual[slot].Cooldown;
                }
                else
                {
                    return GetRandomItem(slot, null);
                }
            }

            protected override int[] GetItems(CombList individual)
            {
                if (individual.Generator != null)
                {
                    return individual.Generator;
                }
                int[] arr = new int[individual.Count];
                for (int i = 0; i < individual.Count; i++)
                {
                    arr[i] = individual[i].Cooldown;
                }
                return arr;
            }

            public CombList GenerateIndividual(string code)
            {
                var codes = code.Split(',');
                int[] items = codes.ConvertAll(c => int.Parse(c)).ToArray();
                return GenerateIndividual(items, true, null);
            }

            protected override CombList GenerateIndividual(int[] items, bool canUseArray, CombList recycledIndividual)
            {
                /*int[] lastActivation = new int[solver.CooldownList.Count];
                int[] activationCount = new int[solver.CooldownList.Count];
                bool[] used = new bool[items.Length];
                for (int i = 0; i < lastActivation.Length; i++)
                {
                    lastActivation[i] = -1;
                }*/

                CombList list = new CombList(solver.CooldownList, solver.stateList, solver);
                for (int index = 0; index < items.Length; index++)
                {
                    if (items[index] == -1) break;
                    //if (used[index]) continue;
                    var cur = new CombItem() { Cooldown = items[index], Activation = !list.IsActive(items[index]) };
                //TRY_AGAIN:
                    list.Add(cur);

                    int cooldown = cur.Cooldown;
                    /*double mt;
                    if (list.Count >= 2)
                    {
                        mt = list[list.Count - 2].MinTime;
                    }
                    else
                    {
                        mt = 0;
                    }
                    if (lastActivation[cooldown] >= 0)
                    {
                        var last = list[lastActivation[cooldown]];
                        double t = last.MinTime;
                        if (cur.Activation)
                        {
                            t += solver.CooldownList[cooldown].Cooldown;
                        }
                        else
                        {
                            t += last.Duration;
                        }
                        if (t > mt)
                        {
                            mt = t;
                        }
                    }
                    for (int i = 0; i < solver.CooldownList.Count; i++)
                    {
                        EffectCooldown coold = solver.CooldownList[i];
                        if (coold.Cooldown > 0 && coold.Duration > 0)
                        {
                            if (lastActivation[i] >= 0)
                            {
                                double act = list[lastActivation[i]].MinTime;
                                double delta = mt - act;

                                if (delta > coold.Duration)
                                {
                                    // find deactivation
                                    bool found = false;
                                    for (int j = lastActivation[i] + 1; j < list.Count; j++)
                                    {
                                        if (list[j].Cooldown == i && !list[j].Activation)
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (!found)
                                    {
                                        // insert missing closer
                                        cur = new CombItem() { Cooldown = i, Activation = false };
                                        list.RemoveAt(list.Count - 1);
                                        index--;
                                        // find the slot and mark it used (it was positioned too late)
                                        for (int j = index + 1; j < items.Length; j++)
                                        {
                                            if (items[j] == i)
                                            {
                                                used[j] = true;
                                                break;
                                            }
                                        }
                                        goto TRY_AGAIN;
                                    }
                                }
                            }
                        }
                    }

                    cur.MinTime = mt;*/
                    if (list.UpdateEffects(list.Count - 1))
                    {
                        cur.UpdateDuration(list);
                        /*if (cur.MinTime >= solver.CalculationOptions.FightDuration)
                        {
                            list.RemoveAt(list.Count - 1);
                            break;
                        }
                        else
                        {
                            if (cur.Activation)
                            {
                                cur.LastActivation = lastActivation[cooldown];
                                lastActivation[cooldown] = list.Count - 1;
                                activationCount[cooldown]++;
                            }
                        }*/
                    }
                    else
                    {
                        // it creates an invalid combo, we have to skip this
                        list.RemoveAt(list.Count - 1);
                    }
                }

                if (canUseArray)
                {
                    list.Generator = items;
                }
                else
                {
                    list.Generator = (int[])items.Clone();
                }

                return list;
            }

            public CharacterCalculationsMage Optimize()
            {
                float bestValue;
                bool inj;
                CharacterCalculationsMage ret;
                base.Optimize(null, 0, out bestValue, out ret, out inj);
                return ret;
            }
        }

        private CooldownOptimizer stackingOptimizer;

        private CharacterCalculationsMage SolveGeneticCombinatorialProblem()
        {
            stackingOptimizer = new CooldownOptimizer(this);
            var ret = stackingOptimizer.Optimize();
            stackingOptimizer = null;
            return ret;
        }

        private CharacterCalculationsMage SolveCombinatorialFixedProblem()
        {
            CooldownOptimizer optimizer = new CooldownOptimizer(this);
            combList = optimizer.GenerateIndividual(CalculationOptions.CombinatorialFixedOrdering);
            combList.UpdateCastingStates();
            ConstructCombStateList();
            SolveBasicProblem();
            return GetCalculationsResult();
        }

        private class ComparisonComparer<T> : IComparer<T>
        {
            private readonly Comparison<T> _comparison;

            public ComparisonComparer(Comparison<T> comparison)
            {
                _comparison = comparison;
            }

            public int Compare(T x, T y)
            {
                return _comparison(x, y);
            }
        }

        private int GetCombItemStage(CombItem item)
        {
            if (CooldownList[item.Cooldown].StandardEffect == StandardEffect.ManaGemEffect && !manaGemEffectAvailable)
            {
                if (item.Activation)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
            else if (CooldownList[item.Cooldown].StandardEffect == StandardEffect.Evocation)
            {
                if (item.Activation)
                {
                    return 4;
                }
                else
                {
                    return 5;
                }
            }
            else
            {
                if (item.Activation)
                {
                    return 3;
                }
                else
                {
                    return 0;
                }
            }
        }

        private CharacterCalculationsMage SolveSimpleStackingProblem()
        {
            double[] bestSolution = null;
            CharacterCalculationsMage bestResult = null;

            var sorter = new ComparisonComparer<CombItem>((a, b) => 
                {
                    int c = a.MinTime.CompareTo(b.MinTime);
                    if (c == 0)
                    {
                        int astage = GetCombItemStage(a);
                        int bstage = GetCombItemStage(b);
                        return astage.CompareTo(bstage);
                    }
                    return c;
                });
            // stack on 2 min burns, search through all item based cooldown permutations
            int[] P = new int[ItemBasedEffectCooldownsCount];
            int[] Q = new int[P.Length];
            int evo = CooldownList.FindIndex(c => c.StandardEffect == StandardEffect.Evocation);
            int invo = CooldownList.FindIndex(c => c.StandardEffect == StandardEffect.Invocation);
            double[] lastActivation = new double[CooldownList.Count];
            for (int i = 0; i < CooldownList.Count; i++)
            {
                lastActivation[i] = -CooldownList[i].Cooldown;
            }
            for (int i = 0; i < P.Length; i++)
            {
                P[i] = i;
                Q[i] = CooldownList.IndexOf(ItemBasedEffectCooldowns[i]);
            }
            int k;
            do
            {
                // create a basic stacking sequence based on current item based order
                combList = new CombList(CooldownList, stateList, this);

                if (MageTalents.Invocation > 0 && evo >= 0)
                {
                    // one time cooldowns on last burn
                    // others on burn unless you can fit multiple inside a 2 min period
                    double t = 0;
                    combList.Add(new CombItem() { Cooldown = invo, Activation = true, MinTime = t });
                    bool onetime = true;
                    while (t < CalculationOptions.FightDuration)
                    {
                        int count = combList.Count;
                        if (onetime)
                        {
                            // pop one time cooldowns
                            for (int i = 0; i < CooldownList.Count; i++)
                            {
                                if (float.IsPositiveInfinity(CooldownList[i].Cooldown))
                                {
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                                }
                            }
                            onetime = false;
                        }
                        // start burn, activate effects that are off cooldown
                        // pop all nonitem based cooldowns
                        for (int i = 0; i < CooldownList.Count; i++)
                        {
                            if (CooldownList[i].StandardEffect != StandardEffect.Evocation && !CooldownList[i].ItemBased && !float.IsPositiveInfinity(CooldownList[i].Cooldown))
                            {
                                if (CooldownList[i].StandardEffect == StandardEffect.ManaGemEffect && !manaGemEffectAvailable)
                                {
                                    // offset mana gem if it's only used for mana
                                    double offset = 10;
                                    if (lastActivation[i] + CooldownList[i].Cooldown <= t + offset + 0.0001)
                                    {
                                        combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t + offset });
                                        combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + offset + CooldownList[i].Duration });
                                        lastActivation[i] = t + offset;
                                    }
                                }
                                else if (CooldownList[i].StandardEffect != StandardEffect.Invocation)
                                {
                                    if (lastActivation[i] + CooldownList[i].Cooldown <= t + 0.0001)
                                    {
                                        combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                                        combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                                        lastActivation[i] = t;
                                    }
                                }
                            }
                        }
                        double tt = 0;
                        // start popping item cooldowns and close off the others as they go away
                        for (int i = 0; i < P.Length; i++)
                        {
                            if (lastActivation[Q[P[i]]] + CooldownList[Q[P[i]]].Cooldown <= t + tt + 0.0001)
                            {
                                combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = true, MinTime = t + tt });
                                combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = false, MinTime = t + tt + CooldownList[Q[P[i]]].Duration });
                                tt += CooldownList[Q[P[i]]].Duration;
                                lastActivation[Q[P[i]]] = t + tt;
                            }
                        }
                        // sprinkle in gcd evos
                        /*if (Specialization == Specialization.Arcane)
                        {
                            for (int i = 0; i < 6; i++)
                            {
                                combList.Add(new CombItem() { Cooldown = evo, Activation = true, MinTime = t + 5 * (i + 1) });
                                combList.Add(new CombItem() { Cooldown = evo, Activation = false, MinTime = t + 5 * (i + 1) });
                            }
                        }*/
                        // throw out anything above fight length
                        combList.RemoveAll(ci => ci.MinTime > CalculationOptions.FightDuration);
                        // sort by time
                        combList.Sort(count, combList.Count - count, sorter);
                        // everything that has same time stamp should be considered simultaneous and zero duration in between (except evocation)
                        for (int i = count; i < combList.Count - 1; i++)
                        {
                            if (combList[i].MinTime == combList[i + 1].MinTime && combList[i + 1].Cooldown != evo)
                            {
                                combList[i].ZeroDuration = true;
                            }
                        }
                        //if (t == 0)
                        {
                            combList.Add(new CombItem() { Cooldown = invo, Activation = false });
                        }
                        // pop evocation                        
                        if (CalculationOptions.FightDuration > ((Specialization == Specialization.Arcane) ? 25 : 60))
                        {
                            combList.Add(new CombItem() { Cooldown = evo, Activation = true });
                            combList.Add(new CombItem() { Cooldown = evo, Activation = false, ZeroDuration = true });
                            combList.Add(new CombItem() { Cooldown = invo, Activation = true });
                        }
                        // move to next burn
                        t += ((Specialization == Specialization.Arcane) ? 25 : 70);
                    }
                }
                else if (MageTalents.RuneOfPower > 0 || evo == -1)
                {
                    // no evocation so just place everything as it comes
                    double t = 0;

                    // pop one time cooldowns
                    for (int i = 0; i < CooldownList.Count; i++)
                    {
                        if (float.IsPositiveInfinity(CooldownList[i].Cooldown))
                        {
                            combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                            combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                        }
                    }

                    // pop all nonitem based cooldowns
                    for (int i = 0; i < CooldownList.Count; i++)
                    {
                        t = 0;
                        while (t < CalculationOptions.FightDuration)
                        {
                            if (CooldownList[i].StandardEffect != StandardEffect.Evocation && !CooldownList[i].ItemBased && !float.IsPositiveInfinity(CooldownList[i].Cooldown))
                            {
                                if (CooldownList[i].StandardEffect == StandardEffect.ManaGemEffect && !manaGemEffectAvailable)
                                {
                                    // offset mana gem if it's only used for mana
                                    double offset = 10;
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t + offset });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + offset + CooldownList[i].Duration });
                                    lastActivation[i] = t + offset;
                                }
                                else
                                {
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                                    lastActivation[i] = t;
                                }
                            }
                            t += CooldownList[i].Cooldown;
                        }
                    }

                    double tt = 0;
                    // start popping item cooldowns and close off the others as they go away
                    for (int i = 0; i < P.Length; i++)
                    {
                        t = tt;
                        while (t < CalculationOptions.FightDuration)
                        {
                            combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = true, MinTime = t });
                            combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = false, MinTime = t + CooldownList[Q[P[i]]].Duration });
                            lastActivation[Q[P[i]]] = t;
                            t += CooldownList[Q[P[i]]].Cooldown;
                        }
                        tt += CooldownList[Q[P[i]]].Duration;
                    }

                    // throw out anything above fight length
                    combList.RemoveAll(ci => ci.MinTime > CalculationOptions.FightDuration);
                    // sort by time
                    combList.Sort(0, combList.Count, sorter);
                    // everything that has same time stamp should be considered simultaneous and zero duration in between
                    for (int i = 0; i < combList.Count - 1; i++)
                    {
                        if (combList[i].MinTime == combList[i + 1].MinTime)
                        {
                            combList[i].ZeroDuration = true;
                        }
                    }
                }
                else if (MageTalents.IncantersWard > 0 && evo >= 0)
                {
                    // one time cooldowns on last burn
                    // others on burn unless you can fit multiple inside a 2 min period
                    double t = 0;
                    while (t < CalculationOptions.FightDuration)
                    {
                        int count = combList.Count;
                        if (CalculationOptions.FightDuration - t < EvocationCooldown)
                        {
                            // pop one time cooldowns
                            for (int i = 0; i < CooldownList.Count; i++)
                            {
                                if (float.IsPositiveInfinity(CooldownList[i].Cooldown))
                                {
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                                }
                            }
                        }
                        // pop all nonitem based cooldowns
                        for (int i = 0; i < CooldownList.Count; i++)
                        {
                            if (CooldownList[i].StandardEffect != StandardEffect.Evocation && !CooldownList[i].ItemBased && !float.IsPositiveInfinity(CooldownList[i].Cooldown))
                            {
                                if (CooldownList[i].StandardEffect == StandardEffect.ManaGemEffect && !manaGemEffectAvailable)
                                {
                                    // offset mana gem if it's only used for mana
                                    double offset = 10;
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t + offset });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + offset + CooldownList[i].Duration });
                                    lastActivation[i] = t + offset;
                                }
                                else
                                {
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                                    lastActivation[i] = t;
                                }
                            }
                        }
                        double tt = 0;
                        // start popping item cooldowns and close off the others as they go away
                        for (int i = 0; i < P.Length; i++)
                        {
                            combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = true, MinTime = t + tt });
                            combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = false, MinTime = t + tt + CooldownList[Q[P[i]]].Duration });
                            tt += CooldownList[Q[P[i]]].Duration;
                            lastActivation[Q[P[i]]] = t + tt;
                        }
                        // throw out anything above fight length
                        combList.RemoveAll(ci => ci.MinTime > CalculationOptions.FightDuration);
                        // sort by time
                        combList.Sort(count, combList.Count - count, sorter);
                        // everything that has same time stamp should be considered simultaneous and zero duration in between
                        for (int i = count; i < combList.Count - 1; i++)
                        {
                            if (combList[i].MinTime == combList[i + 1].MinTime)
                            {
                                combList[i].ZeroDuration = true;
                            }
                        }
                        // pop evocation
                        combList.Add(new CombItem() { Cooldown = evo, Activation = true });
                        combList.Add(new CombItem() { Cooldown = evo, Activation = false });
                        // do cooldowns that can be popped during neutral phase
                        count = combList.Count;
                        for (int i = 0; i < CooldownList.Count; i++)
                        {
                            if (CooldownList[i].StandardEffect != StandardEffect.Evocation && !float.IsPositiveInfinity(CooldownList[i].Cooldown))
                            {
                                if (CooldownList[i].Cooldown <= 60)
                                {
                                    int num = (int)(120 / CooldownList[i].Cooldown);
                                    for (int j = 1; j < num; j++)
                                    {
                                        combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = lastActivation[i] + j * CooldownList[i].Cooldown });
                                        combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = lastActivation[i] + j * CooldownList[i].Cooldown + CooldownList[i].Duration });
                                    }
                                }
                            }
                        }
                        // throw out anything above fight length
                        combList.RemoveAll(ci => ci.MinTime > CalculationOptions.FightDuration);
                        // sort by time
                        combList.Sort(count, combList.Count - count, sorter);
                        // everything that has same time stamp should be considered simultaneous and zero duration in between
                        for (int i = count; i < combList.Count - 1; i++)
                        {
                            if (combList[i].MinTime == combList[i + 1].MinTime)
                            {
                                combList[i].ZeroDuration = true;
                            }
                        }
                        // move to next burn                    
                        t += EvocationCooldown;
                    }
                }
                else
                {
                    // one time cooldowns on last burn
                    // others on burn unless you can fit multiple inside a 2 min period
                    double t = 0;
                    while (t < CalculationOptions.FightDuration)
                    {
                        int count = combList.Count;
                        if (CalculationOptions.FightDuration - t < EvocationCooldown)
                        {
                            // pop one time cooldowns
                            for (int i = 0; i < CooldownList.Count; i++)
                            {
                                if (float.IsPositiveInfinity(CooldownList[i].Cooldown))
                                {
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                                }
                            }
                        }
                        // pop all nonitem based cooldowns
                        for (int i = 0; i < CooldownList.Count; i++)
                        {
                            if (CooldownList[i].StandardEffect != StandardEffect.Evocation && !CooldownList[i].ItemBased && !float.IsPositiveInfinity(CooldownList[i].Cooldown))
                            {
                                if (CooldownList[i].StandardEffect == StandardEffect.ManaGemEffect && !manaGemEffectAvailable)
                                {
                                    // offset mana gem if it's only used for mana
                                    double offset = 10;
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t + offset });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + offset + CooldownList[i].Duration });
                                    lastActivation[i] = t + offset;
                                }
                                else
                                {
                                    combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = t });
                                    combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = t + CooldownList[i].Duration });
                                    lastActivation[i] = t;
                                }
                            }
                        }
                        double tt = 0;
                        // start popping item cooldowns and close off the others as they go away
                        for (int i = 0; i < P.Length; i++)
                        {
                            combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = true, MinTime = t + tt });
                            combList.Add(new CombItem() { Cooldown = Q[P[i]], Activation = false, MinTime = t + tt + CooldownList[Q[P[i]]].Duration });
                            tt += CooldownList[Q[P[i]]].Duration;
                            lastActivation[Q[P[i]]] = t + tt;
                        }
                        // throw out anything above fight length
                        combList.RemoveAll(ci => ci.MinTime > CalculationOptions.FightDuration);
                        // sort by time
                        combList.Sort(count, combList.Count - count, sorter);
                        // everything that has same time stamp should be considered simultaneous and zero duration in between
                        for (int i = count; i < combList.Count - 1; i++)
                        {
                            if (combList[i].MinTime == combList[i + 1].MinTime)
                            {
                                combList[i].ZeroDuration = true;
                            }
                        }
                        // pop evocation
                        combList.Add(new CombItem() { Cooldown = evo, Activation = true });
                        combList.Add(new CombItem() { Cooldown = evo, Activation = false });
                        // do cooldowns that can be popped during neutral phase
                        count = combList.Count;
                        for (int i = 0; i < CooldownList.Count; i++)
                        {
                            if (CooldownList[i].StandardEffect != StandardEffect.Evocation && !float.IsPositiveInfinity(CooldownList[i].Cooldown))
                            {
                                if (CooldownList[i].Cooldown <= 60)
                                {
                                    int num = (int)(120 / CooldownList[i].Cooldown);
                                    for (int j = 1; j < num; j++)
                                    {
                                        combList.Add(new CombItem() { Cooldown = i, Activation = true, MinTime = lastActivation[i] + j * CooldownList[i].Cooldown });
                                        combList.Add(new CombItem() { Cooldown = i, Activation = false, MinTime = lastActivation[i] + j * CooldownList[i].Cooldown + CooldownList[i].Duration });
                                    }
                                }
                            }
                        }
                        // throw out anything above fight length
                        combList.RemoveAll(ci => ci.MinTime > CalculationOptions.FightDuration);
                        // sort by time
                        combList.Sort(count, combList.Count - count, sorter);
                        // everything that has same time stamp should be considered simultaneous and zero duration in between
                        for (int i = count; i < combList.Count - 1; i++)
                        {
                            if (combList[i].MinTime == combList[i + 1].MinTime)
                            {
                                combList[i].ZeroDuration = true;
                            }
                        }
                        // move to next burn                    
                        t += EvocationCooldown;
                    }
                }

                combList.UpdateEffectsAndCastingStates();
                ConstructCombStateList();

                for (int i = 0; i < combStateList.Count; i++)
                {
                    combStateList[i].OverflowConstraint = (Specialization == Specialization.Arcane && ((combStateList[i].CastingState.Evocation && (i == combStateList.Count - 1 || !combStateList[i + 1].CastingState.Evocation)) || combStateList[i].ManaGemActivation || !CalculationOptions.DisableManaRegenCycles)) ? 0 : -1;
                    combStateList[i].UnderflowConstraint = (!combStateList[i].CastingState.Evocation && i < combStateList.Count - 1 && combStateList[i + 1].CastingState.Evocation) ? 0 : -1;
                }

                SolveBasicProblem();
                if (bestSolution == null || solution[solution.Length - 1] > bestSolution[bestSolution.Length - 1])
                {
                    bestSolution = solution;
                    bestResult = GetCalculationsResult();
                }

                // move to next permutation
                for (k = P.Length - 2; k >= 0; k--)
                {
                    if (P[k] < P[k + 1])
                    {
                        for (int l = P.Length - 1; l > k; l--)
                        {
                            if (P[k] < P[l])
                            {
                                int tmp = P[k];
                                P[k] = P[l];
                                P[l] = tmp;

                                int j = P.Length - 1;
                                k++;
                                for (; k < j; k++, j--)
                                {
                                    tmp = P[k];
                                    P[k] = P[j];
                                    P[j] = tmp;
                                }
                                break;
                            }
                        }
                        break;
                    }
                }
            } while (k >= 0);
            solution = bestSolution;
            return bestResult;
        }

        private CharacterCalculationsMage SolveCombinatorialProblem()
        {
            // we're examining combinatorial solutions to cooldown stacking
            // for each feasible ordering of cooldown activations/deactivations we solve a separate linear/quadratic problem
            // the best solution is the ultimate result

            double[] bestSolution = null;
            CharacterCalculationsMage bestResult = null;

            int[] lastActivation = new int[CooldownList.Count];
            int[] activationCount = new int[CooldownList.Count];
            for (int i = 0; i < lastActivation.Length; i++)
            {
                lastActivation[i] = -1;
            }

            // generate all combinatorial options
            combList = new CombList(CooldownList, stateList, this);
            int index = 0;
            int count = 0;
            CombItem cur;
            do
            {
                count++;
                if (count % 100000 == 0)
                {
                    CalculationsMage.Log(this, count + ": " + combList.ToString());
                }
                if (index == combList.Count)
                {
                    // adding new entry
                    cur = new CombItem() { Cooldown = 0, Activation = !combList.IsActive(0) };
                    combList.Add(cur);
                }
                else
                {
                    // increment option
                    cur = combList[index];
                    cur.Cooldown++;
                    if (cur.Cooldown >= CooldownList.Count)
                    {
                        // we exhausted all options, move to earlier time event
                        index--;
                        cur = combList[index];
                        // if this option did not generate a solution yet then create it now
                        // since all future events are beyond fight horizon
                        if (!cur.Generated)
                        {
                            combList.RemoveRange(index + 1, combList.Count - index - 1);
                            if (combList.IsFeasible(activationCount))
                            {
                                for (int i = 0; i < index; i++)
                                {
                                    combList[i].Generated = true;
                                }
                                combList.UpdateCastingStates();
                                ConstructCombStateList();
                                SolveBasicProblem();
                                if (bestSolution == null || solution[solution.Length - 1] > bestSolution[bestSolution.Length - 1])
                                {
                                    bestSolution = solution;
                                    bestResult = GetCalculationsResult();
                                    CalculationsMage.Log(this, count + ": " + bestSolution[bestSolution.Length - 1]);
                                }
                            }
                        }
                        if (cur.Activation)
                        {
                            lastActivation[cur.Cooldown] = cur.LastActivation;
                            activationCount[cur.Cooldown]--;
                        }
                        continue;
                    }
                    cur.Activation = !combList.IsActive(cur.Cooldown, index);
                    combList.RemoveRange(index + 1, combList.Count - index - 1);
                }
                //combList.UpdateMinTime(index);
                int cooldown = cur.Cooldown;
                double mt;
                if (index > 0)
                {
                    mt = combList[index - 1].MinTime;
                }
                else
                {
                    mt = 0;
                }
                if (lastActivation[cooldown] >= 0)
                {
                    var last = combList[lastActivation[cooldown]];
                    double t = last.MinTime;
                    if (cur.Activation)
                    {
                        t += CooldownList[cooldown].Cooldown;
                    }
                    else
                    {
                        t += last.Duration;
                    }
                    if (t > mt)
                    {
                        mt = t;
                    }
                }
                cur.MinTime = mt;
                if (combList.UpdateEffects(index))
                {
                    cur.UpdateDuration(combList);
                    cur.Generated = false;
                    if (cur.MinTime >= CalculationOptions.FightDuration || !combList.IsPartialFeasible(lastActivation, activationCount))
                    {
                        // we went beyond the fight horizon, move to next option
                        // we skipped some deactivations, cancel and move on
                        // or it won't be optimal
                    }
                    else
                    {
                        // move to future events
                        if (cur.Activation)
                        {
                            cur.LastActivation = lastActivation[cooldown];
                            lastActivation[cooldown] = index;
                            activationCount[cooldown]++;
                        }
                        index++;
                    }
                }
            } while (index >= 0 && !cancellationPending);
            solution = bestSolution;
            return bestResult;
        }

        private void SolveBasicProblem()
        {
            ConstructProblem();

            //TestScaling();

            if (needsQuadratic)
            {
                SolveQuadratic();
            }

            if (requiresMIP)
            {
                RestrictSolution();
            }

            solution = lp.Solve();
        }

        private void CalculateCycles()
        {
            //if (SolveCycles)
            //{
            //    FrBDFFFBIL.SolveCycle(BaseState, out FrBDFFFBIL_KFrB, out FrBDFFFBIL_KFFB, out FrBDFFFBIL_KFFBS, out FrBDFFFBIL_KILS, out FrBDFFFBIL_KDFS);
            //}
            //else
            //{
            //    FrBDFFFBIL_KDFS = CalculationOptions.FrBDFFFBIL_KDFS;
            //    FrBDFFFBIL_KFFB = CalculationOptions.FrBDFFFBIL_KFFB;
            //    FrBDFFFBIL_KFFBS = CalculationOptions.FrBDFFFBIL_KFFBS;
            //    FrBDFFFBIL_KFrB = CalculationOptions.FrBDFFFBIL_KFrB;
            //    FrBDFFFBIL_KILS = CalculationOptions.FrBDFFFBIL_KILS;
            //}
        }

        private void TestScaling()
        {
            // column scaling
            for (int i = 0; i < lp.Columns; i++)
            {
                double maxMagnitude = 0;
                for (int j = 0; j < lp.Rows; j++)
                {
                    double v = Math.Abs(ArraySet.SparseMatrixData[i * lp.Rows + j]);
                    if (v > maxMagnitude)
                    {
                        maxMagnitude = v;
                    }
                }
                if (maxMagnitude < 0.1 || maxMagnitude > 10)
                {
                    maxMagnitude = 1;
                }
            }

            // row scaling
            for (int j = 0; j < lp.Rows; j++)
            {
                double maxMagnitude = 0;
                for (int i = 0; i < lp.Columns; i++)
                {
                    double v = Math.Abs(ArraySet.SparseMatrixData[i * lp.Rows + j]);
                    if (v > maxMagnitude)
                    {
                        maxMagnitude = v;
                    }
                }
                if (maxMagnitude < 0.1 || maxMagnitude > 10)
                {
                    maxMagnitude = 1;
                }
            }
        }

        #region Effect Maximization
        private double MaximizeColdsnapDuration(double fightDuration, double coldsnapCooldown, double effectDuration, double effectCooldown, out int coldsnapCount)
        {
            int bestColdsnap = 0;
            double bestEffect = 0.0;
            List<int> coldsnap = new List<int>();
            List<double> startTime = new List<double>();
            List<double> coldsnapTime = new List<double>();
            int index = 0;
            coldsnap.Add(2);
            startTime.Add(0.0);
            coldsnapTime.Add(0.0);
            do
            {
                if (index > 0 && startTime[index - 1] + effectDuration >= fightDuration)
                {
                    double effect = (index - 1) * effectDuration + Math.Max(fightDuration - startTime[index - 1], 0.0);
                    if (effect > bestEffect)
                    {
                        bestEffect = effect;
                        bestColdsnap = 0;
                        for (int i = 0; i < index; i++)
                        {
                            if (startTime[i] < fightDuration - 20.0) bestColdsnap += coldsnap[i]; // if it is a coldsnap for a very short elemental, don't count it for IV
                        }
                    }
                    index--;
                }
                coldsnap[index]--;
                if (coldsnap[index] < 0)
                {
                    index--;
                }
                else
                {
                    double time = 0.0;
                    if (index > 0)
                    {
                        time = startTime[index - 1] + effectDuration;
                        int lastColdsnap = -1;
                        for (int j = 0; j < index; j++)
                        {
                            if (coldsnap[j] == 1) lastColdsnap = j;
                        }
                        if (coldsnap[index] == 1)
                        {
                            // use coldsnap
                            double normalTime = Math.Max(time, startTime[index - 1] + effectCooldown);
                            double coldsnapReady = 0.0;
                            if (lastColdsnap >= 0) coldsnapReady = coldsnapTime[lastColdsnap] + coldsnapCooldown;
                            if (coldsnapReady >= normalTime)
                            {
                                // coldsnap won't be ready until effect will be back anyway, so we don't actually need it
                                coldsnap[index] = 0;
                                time = normalTime;
                            }
                            else
                            {
                                // go now or when coldsnap is ready
                                time = Math.Max(coldsnapReady, time);
                                coldsnapTime[index] = Math.Max(coldsnapReady, startTime[index - 1]);
                            }
                        }
                        else
                        {
                            // we are not allowed to use coldsnap even if we could
                            // make sure to adjust by coldsnap constraints
                            time = Math.Max(time, startTime[index - 1] + effectCooldown);
                        }
                    }
                    else
                    {
                        coldsnap[index] = 0;
                    }
                    startTime[index] = time;
                    index++;
                    if (index >= coldsnap.Count)
                    {
                        coldsnap.Add(0);
                        coldsnapTime.Add(0.0);
                        startTime.Add(0.0);
                    }
                    coldsnap[index] = 2;
                }
            } while (index >= 0);
            coldsnapCount = bestColdsnap;
            return bestEffect;
        }

        public static double MaximizeEffectDuration(double fightDuration, double effectDuration, double effectCooldown)
        {
            if (effectCooldown < effectDuration) return fightDuration;
            if (fightDuration < effectDuration) return fightDuration;
            double total = effectDuration;
            fightDuration -= effectDuration;
            int count = (int)(fightDuration / effectCooldown);
            total += effectDuration * count;
            fightDuration -= effectCooldown * count;
            fightDuration -= effectCooldown - effectDuration;
            if (fightDuration > 0) total += fightDuration;
            return total;
        }

        private double MaximizeStackingDuration(double fightDuration, double effect1Duration, double effect1Cooldown, double effect2Duration, double effect2Cooldown)
        {
            // clean up in case of bad data
            if (effect1Cooldown < effect1Duration)
            {
                effect1Cooldown = effect1Duration;
            }
            if (effect2Cooldown < effect2Duration)
            {
                effect2Cooldown = effect2Duration;
            }
            /*if (double.IsPositiveInfinity(effect1Cooldown) || double.IsPositiveInfinity(effect2Cooldown))
            {
                return MaximizeStackingDuration(fightDuration, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, 0, 0);
            }
            else*/
            {
                var cache = CalculationOptions.CooldownStackingCache;
                lock (cache)
                {
                    /*for (int i = 0; i < cache.Count; i++)
                    {
                        var entry = cache[i];
                        if ((entry.Effect1Cooldown == effect1Cooldown && entry.Effect2Cooldown == effect2Cooldown && entry.Effect1Duration == effect1Duration && entry.Effect2Duration == effect2Duration) ||
                            (entry.Effect1Cooldown == effect2Cooldown && entry.Effect2Cooldown == effect1Cooldown && entry.Effect1Duration == effect2Duration && entry.Effect2Duration == effect1Duration))
                        {
                            return entry.MaximumStackingDuration;
                        }
                    }*/
                    // do binary search
                    int num = 0;
                    int num2 = cache.Count - 1;
                    while (num <= num2)
                    {
                        int num3 = num + ((num2 - num) >> 1);
                        var entry = cache[num3];
                        double key = entry.Effect1Duration;
                        if (key < effect1Duration)
                        {
                            num = num3 + 1;
                        }
                        else if (key > effect1Duration)
                        {
                            num2 = num3 - 1;
                        }
                        else
                        {
                            key = entry.Effect1Cooldown;
                            if (key < effect1Cooldown)
                            {
                                num = num3 + 1;
                            }
                            else if (key > effect1Cooldown)
                            {
                                num2 = num3 - 1;
                            }
                            else
                            {
                                key = entry.Effect2Duration;
                                if (key < effect2Duration)
                                {
                                    num = num3 + 1;
                                }
                                else if (key > effect2Duration)
                                {
                                    num2 = num3 - 1;
                                }
                                else
                                {
                                    key = entry.Effect2Cooldown;
                                    if (key < effect2Cooldown)
                                    {
                                        num = num3 + 1;
                                    }
                                    else if (key > effect2Cooldown)
                                    {
                                        num2 = num3 - 1;
                                    }
                                    else
                                    {
                                        return entry.MaximumStackingDuration;
                                    }
                                }
                            }
                        }
                    }

                    double value;
                    //System.Diagnostics.Stopwatch clock = new System.Diagnostics.Stopwatch();
                    //clock.Reset();
                    if (double.IsPositiveInfinity(effect1Cooldown) || double.IsPositiveInfinity(effect2Cooldown))
                    {
                        //clock.Start();
                        value = MaximizeStackingDuration(fightDuration, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, 0, 0);
                        //clock.Stop();
                        //System.Diagnostics.Trace.WriteLine("noncache = " + clock.ElapsedTicks);
                        //clock.Reset();
                    }
                    else
                    {
                        //clock.Start();
                        memoizationCache = new List<StackingMemoizationEntry>[(int)fightDuration + 1];
                        value = MaximizeStackingDuration2((int)fightDuration, (int)effect1Duration, (int)effect1Cooldown, (int)effect2Duration, (int)effect2Cooldown, 0, 0, 0);
                        memoizationCache = null;
                        //clock.Stop();
                        //System.Diagnostics.Trace.WriteLine("cache = " + clock.ElapsedTicks);
                    }
                    cache.Insert(num, new CooldownStackingCacheEntry()
                    {
                        Effect1Duration = effect1Duration,
                        Effect1Cooldown = effect1Cooldown,
                        Effect2Duration = effect2Duration,
                        Effect2Cooldown = effect2Cooldown,
                        MaximumStackingDuration = value
                    });
                    return value;
                }
            }
        }

        private double MaximizeStackingDuration(double fightDuration, double effect1Duration, double effect1Cooldown, double effect2Duration, double effect2Cooldown, double effect2ActiveDuration, double effect2ActiveCooldown)
        {
            if (fightDuration <= 0) return 0;
            if (double.IsPositiveInfinity(effect2ActiveCooldown) && effect2ActiveDuration == 0) return 0;
            effect2ActiveDuration = Math.Min(effect2ActiveDuration, fightDuration);

            double slack = 0;
            double f = fightDuration;

            if (f < effect1Duration)
            {
                slack = 0;
            }
            else
            {
                f -= effect1Duration;
                int count = (int)(f / effect1Cooldown);
                if (count > 0)
                {
                    f -= effect1Cooldown * count;
                }
                if (f - effect1Cooldown + effect1Duration > 0)
                {
                    slack = 0;
                }
                else
                {
                    slack = f;
                }
            }
            if (!CalculationOptions.MaxUseAssumption)
            {
                slack = effect2ActiveCooldown;
            }


            // ####........|
            double best = 0;
            double value = 0;
            double min = 0;

            if (effect2ActiveCooldown > 0)
            {
                // if optimal placement of effect1 is stacked with effect2 activation
                // and it doesn't overlap two different activations
                // or if optimal placement has effect1 activated and finished before effect2 gets off cooldown
                // then we'll get as good or better stacking if we move effect1 all the way to the start
                if (effect1Cooldown < effect2ActiveCooldown)
                {
                    // effect1 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration(fightDuration - effect1Cooldown, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2ActiveDuration - effect1Cooldown), Math.Max(0, effect2ActiveCooldown - effect1Cooldown));
                }
                else
                {
                    // effect2 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration(fightDuration - effect2ActiveCooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - effect2ActiveCooldown), Math.Max(0, effect1Cooldown - effect2ActiveCooldown));
                }
                if (value > best)
                {
                    best = value;
                }
            }
            // the next case is if effect1 activation crosses over effect2 cooldown
            // in this case it's just as good if effect2 starts right on cooldown
            // now in this case moving effect1 earlier has negative effect so we can't do that
            // we want however to push it as late as possible without affecting the optimum
            // but just as long as it ends before effect2 ends
            // ####........|#####
            // ..........#######
            // 0 <= offset <= effect2ActiveCooldown
            // offset <= effect1Duration
            // this can potentially still be optimized, I doubt we need to look at every option
            // but it seems in practice it works well enough so don't waste time unless profiling shows need
            // TODO ok it works well enough, but still costs a significant amount, time to improve this
            if (!double.IsPositiveInfinity(effect2ActiveCooldown))
            {
                int minOffset = Math.Max(0, (int)(effect2ActiveCooldown - slack));
                int maxOffset = (int)Math.Min(effect1Duration, effect2ActiveCooldown);
                if (effect2Cooldown >= effect1Duration)
                {
                    int endAlignedOffset = (int)(effect1Duration - effect2Duration);
                    if (endAlignedOffset > minOffset)
                    {
                        minOffset = Math.Min(endAlignedOffset, maxOffset);
                    }
                }
                for (int offset = minOffset; offset <= maxOffset; offset++)
                {
                    // is there any stacking left from current effect2 activation?
                    double leftover = Math.Max(0, effect2ActiveDuration - (effect2ActiveCooldown - offset));
                    min = Math.Min(effect1Duration - offset, effect2Duration);
                    if (effect1Cooldown - offset < effect2Cooldown)
                    {
                        // effect1 will be off cooldown first
                        value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration(fightDuration - effect2ActiveCooldown - effect1Cooldown + offset, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2Duration - effect1Cooldown + offset), Math.Max(0, effect2Cooldown - effect1Cooldown + offset));
                    }
                    else
                    {
                        // effect2 will be off cooldown first
                        value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration(fightDuration - effect2ActiveCooldown - effect2Cooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - offset - effect2Cooldown), Math.Max(0, effect1Cooldown - offset - effect2Cooldown));
                    }
                    if (value > best)
                    {
                        best = value;
                    }
                }
            }
            return best;
        }

        private struct StackingMemoizationEntry
        {
            public int Order;
            public int Effect2ActiveDuration;
            public int Effect2ActiveCooldown;
            public int MaximizeStackingDuration;
        }

        private List<StackingMemoizationEntry>[] memoizationCache;

        private int MaximizeStackingDuration2(int fightDuration, int effect1Duration, int effect1Cooldown, int effect2Duration, int effect2Cooldown, int effect2ActiveDuration, int effect2ActiveCooldown, int order)
        {
            if (fightDuration <= 0) return 0;
            effect2ActiveDuration = Math.Min(effect2ActiveDuration, fightDuration);

            List<StackingMemoizationEntry> list = memoizationCache[fightDuration];
            if (list == null)
            {
                list = new List<StackingMemoizationEntry>();
                memoizationCache[fightDuration] = list;
            }

            for (int i = 0; i < list.Count; i++)
            {
                StackingMemoizationEntry entry = list[i];
                if (entry.Effect2ActiveCooldown == effect2ActiveCooldown && entry.Order == order && entry.Effect2ActiveDuration == effect2ActiveDuration)
                {
                    return entry.MaximizeStackingDuration;
                }
            }

            int slack = 0;
            int f = fightDuration;

            if (f < effect1Duration)
            {
                slack = 0;
            }
            else
            {
                f -= effect1Duration;
                int count = f / effect1Cooldown;
                if (count > 0)
                {
                    f -= effect1Cooldown * count;
                }
                if (f - effect1Cooldown + effect1Duration > 0)
                {
                    slack = 0;
                }
                else
                {
                    slack = f;
                }
            }
            if (!CalculationOptions.MaxUseAssumption)
            {
                slack = effect2ActiveCooldown;
            }


            // ####........|
            int best = 0;
            int value = 0;
            int min = 0;

            if (effect2ActiveCooldown > 0)
            {
                // if optimal placement of effect1 is stacked with effect2 activation
                // and it doesn't overlap two different activations
                // or if optimal placement has effect1 activated and finished before effect2 gets off cooldown
                // then we'll get as good or better stacking if we move effect1 all the way to the start
                if (effect1Cooldown < effect2ActiveCooldown)
                {
                    // effect1 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration2(fightDuration - effect1Cooldown, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2ActiveDuration - effect1Cooldown), Math.Max(0, effect2ActiveCooldown - effect1Cooldown), order);
                }
                else
                {
                    // effect2 will be off cooldown first
                    value = Math.Min(effect1Duration, effect2ActiveDuration) + MaximizeStackingDuration2(fightDuration - effect2ActiveCooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - effect2ActiveCooldown), Math.Max(0, effect1Cooldown - effect2ActiveCooldown), 1 - order);
                }
                if (value > best)
                {
                    best = value;
                }
            }
            // the next case is if effect1 activation crosses over effect2 cooldown
            // in this case it's just as good if effect2 starts right on cooldown
            // now in this case moving effect1 earlier has negative effect so we can't do that
            // we want however to push it as late as possible without affecting the optimum
            // but just as long as it ends before effect2 ends
            // ####........|#####
            // ..........#######
            // 0 <= offset <= effect2ActiveCooldown
            // offset <= effect1Duration
            // this can potentially still be optimized, I doubt we need to look at every option
            // but it seems in practice it works well enough so don't waste time unless profiling shows need
            // TODO ok it works well enough, but still costs a significant amount, time to improve this
            int minOffset = Math.Max(0, effect2ActiveCooldown - slack);
            int maxOffset = Math.Min(effect1Duration, effect2ActiveCooldown);
            if (effect2Cooldown >= effect1Duration)
            {
                int endAlignedOffset = effect1Duration - effect2Duration;
                if (endAlignedOffset > minOffset)
                {
                    minOffset = Math.Min(endAlignedOffset, maxOffset);
                }
            }
            for (int offset = minOffset; offset <= maxOffset; offset++)
            {
                // is there any stacking left from current effect2 activation?
                int leftover = Math.Max(0, effect2ActiveDuration - (effect2ActiveCooldown - offset));
                min = Math.Min(effect1Duration - offset, effect2Duration);
                if (effect1Cooldown - offset < effect2Cooldown)
                {
                    // effect1 will be off cooldown first
                    value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration2(fightDuration - effect2ActiveCooldown - effect1Cooldown + offset, effect1Duration, effect1Cooldown, effect2Duration, effect2Cooldown, Math.Max(0, effect2Duration - effect1Cooldown + offset), Math.Max(0, effect2Cooldown - effect1Cooldown + offset), order);
                }
                else
                {
                    // effect2 will be off cooldown first
                    value = leftover + Math.Min(min, fightDuration - effect2ActiveCooldown) + MaximizeStackingDuration2(fightDuration - effect2ActiveCooldown - effect2Cooldown, effect2Duration, effect2Cooldown, effect1Duration, effect1Cooldown, Math.Max(0, effect1Duration - offset - effect2Cooldown), Math.Max(0, effect1Cooldown - offset - effect2Cooldown), 1 - order);
                }
                if (value > best)
                {
                    best = value;
                }
            }

            list.Add(new StackingMemoizationEntry()
            {
                Order = order,
                Effect2ActiveDuration = effect2ActiveDuration,
                Effect2ActiveCooldown = effect2ActiveCooldown,
                MaximizeStackingDuration = best
            });

            return best;
        }
        #endregion

        #region Initialize
        public void Initialize(Item additionalItem)
        {
            Stats rawStats;
            if (NeedsDisplayCalculations || ArraySet == null)
            {
                rawStats = new Stats();
            }
            else
            {
                rawStats = ArraySet.accumulator;
                if (rawStats == null)
                {
                    rawStats = new Stats();
                    ArraySet.accumulator = rawStats;
                }
                else
                {
                    rawStats.Clear();
                }
            }

            if (armor == null)
            {
                if (Character.ActiveBuffs.Contains(CalculationsMage.MoltenArmorBuff)) armor = "Molten Armor";
                else if (Character.ActiveBuffs.Contains(CalculationsMage.MageArmorBuff)) armor = "Mage Armor";
                else if (Character.ActiveBuffs.Contains(CalculationsMage.FrostArmorBuff)) armor = "Frost Armor";
            }

            CalculationsMage calculations = CalculationsMage.Instance;
            targetDebuffs = null;
            calculations.AccumulateRawStats(rawStats, Character, additionalItem, CalculationOptions, out autoActivatedBuffs, armor, out ActiveBuffs);

            // apply set bonuses
            int setCount;
            Character.SetBonusCount.TryGetValue("Bloodmage's Regalia", out setCount);
            Mage2T10 = (setCount >= 2);
            Mage4T10 = (setCount >= 4);
            Character.SetBonusCount.TryGetValue("Firelord's Vestments", out setCount);
            Mage2T11 = (setCount >= 2);
            Mage4T11 = (setCount >= 4);
            Character.SetBonusCount.TryGetValue("Firehawk Robes of Conflagration", out setCount);
            Mage2T12 = (setCount >= 2);
            Mage4T12 = (setCount >= 4);
            Character.SetBonusCount.TryGetValue("Gladiator's Regalia", out setCount);
            Mage2PVP = (setCount >= 2);
            Mage4PVP = (setCount >= 4);
            Character.SetBonusCount.TryGetValue("Time Lord's Regalia", out setCount);
            Mage2T13 = (setCount >= 2);
            Mage4T13 = (setCount >= 4);
            Character.SetBonusCount.TryGetValue("Regalia of the Burning Scroll", out setCount);
            Mage2T14 = (setCount >= 2);
            Mage4T14 = (setCount >= 4);
            Character.SetBonusCount.TryGetValue("Regalia of the Chromatic Hydra", out setCount);
            Mage2T15 = (setCount >= 2);
            Mage4T15 = (setCount >= 4);
            Character.SetBonusCount.TryGetValue("Chronomancer Regalia", out setCount);
            Mage2T16 = (setCount >= 2);
            Mage4T16 = (setCount >= 4);

            if (Mage2PVP)
            {
                rawStats.PvPPower += 500;
            }
            if (Mage4PVP)
            {
                rawStats.PvPResilience += 1000;
            }
            if (Mage2T12)
            {
                rawStats.AddSpecialEffect(SpecialEffect2T12);
            }

            // armor            
            if (CalculationOptions.ArmorSwapping)
            {
                baseArmorMask = (int)StandardEffect.MoltenArmor;
                moltenArmorAvailable = true;
                mageArmorAvailable = true;
                frostArmorAvailable = false;
            }
            else
            {
                baseArmorMask = 0;
                mageArmorAvailable = false;
                moltenArmorAvailable = false;
                frostArmorAvailable = false;
            }

            BaseStats = calculations.GetCharacterStats(Character, additionalItem, rawStats, CalculationOptions, !CalculationOptions.ArmorSwapping);

            Specialization = (Specialization)(MageTalents.Specialization + 1);

            evocationAvailable = CalculationOptions.EvocationEnabled && !CalculationOptions.EffectDisableManaSources && MageTalents.RuneOfPower == 0;
            manaPotionAvailable = CalculationOptions.ManaPotionEnabled && !CalculationOptions.EffectDisableManaSources;
            //restrictThreat = segmentCooldowns && CalculationOptions.TpsLimit > 0f;
            powerInfusionAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.PowerInfusionAvailable;
            heroismAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.HeroismAvailable;
            arcanePowerAvailable = !CalculationOptions.DisableCooldowns && (Specialization == Mage.Specialization.Arcane);
            icyVeinsAvailable = !CalculationOptions.DisableCooldowns && (Specialization == Mage.Specialization.Frost);
            combustionAvailable = !CalculationOptions.DisableCooldowns && (Specialization == Mage.Specialization.Fire) && !CalculationOptions.ProcCombustion;
            //coldsnapAvailable = !CalculationOptions.DisableCooldowns && (MageTalents.ColdSnap == 1);
            volcanicPotionAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.VolcanicPotion;
            effectPotionAvailable = volcanicPotionAvailable;
            berserkingAvailable = !CalculationOptions.DisableCooldowns && Character.Race == CharacterRace.Troll;
            mage2T15EffectAvailable = !CalculationOptions.DisableCooldowns && Mage2T15;
            waterElementalAvailable = !CalculationOptions.DisableCooldowns && Specialization == Mage.Specialization.Frost;
            mirrorImageAvailable = !CalculationOptions.DisableCooldowns && CalculationOptions.MirrorImage == 2;
            manaGemEffectAvailable = CalculationOptions.ManaGemEnabled && false;
            bloodFuryAvailable = !CalculationOptions.DisableCooldowns && Character.Race == CharacterRace.Orc;

            // if we're using incremental optimizations it's possible we know some effects won't be used
            // in that case we can skip them and possible save some constraints
            if (UseIncrementalOptimizations)
            {
                int[] sortedStates = CalculationOptions.IncrementalSetSortedStates;
                bool usesVolcanicPotion = false;
                for (int incrementalSortedIndex = 0; incrementalSortedIndex < sortedStates.Length; incrementalSortedIndex++)
                {
                    // incremental index is filtered by non-item based cooldowns
                    int incrementalSetIndex = sortedStates[incrementalSortedIndex];
                    if ((incrementalSetIndex & (int)StandardEffect.VolcanicPotion) != 0)
                    {
                        usesVolcanicPotion = true;
                    }
                }
                if (!usesVolcanicPotion)
                {
                    volcanicPotionAvailable = false;
                }
            }

            if (!CalculationOptions.EffectDisableManaSources)
            {
                ManaGemValue = 0.15f * BaseCombatRating.MageBaseMana(CalculationOptions.PlayerLevel);
                MaxManaGemValue = ManaGemValue * 1.05f;
                if (CalculationOptions.PlayerLevel <= 70)
                {
                    ManaPotionValue = 2400.0f;
                    MaxManaPotionValue = 3000.0f;
                }
                else if (CalculationOptions.PlayerLevel <= 80)
                {
                    ManaPotionValue = 4300.0f;
                    MaxManaPotionValue = 4400.0f;
                }
                else if (CalculationOptions.PlayerLevel <= 84)
                {
                    ManaPotionValue = 10000.0f;
                    MaxManaPotionValue = 10750.0f;
                }
                else
                {
                    ManaPotionValue = 30000.0f;
                    MaxManaPotionValue = 31500.0f;
                }
            }

            CalculateIncomingDamage();

            CalculateBaseStateStats(); // BaseCritRate used below

            // Dynamic proc modifiers
            foreach (SpecialEffect effect in BaseStats.SpecialEffects())
            {
                switch (effect.ModifiedBy)
                {
                    case "Spec/Class":  // Sinister Primal Diamond
                        if (Specialization == Mage.Specialization.Frost)
                            effect.ChanceModifier = 1.387f; 
                        if (Specialization == Mage.Specialization.Arcane)
                            effect.ChanceModifier = 0.761f;
                        if (Specialization == Mage.Specialization.Fire)
                            effect.ChanceModifier = 0.705f;
                        break;
                        
                    case "Spell Crit":  // Cha-Ye's Essence of Brilliance
                        effect.ChanceModifier = 1 + BaseCritRate;
                        break;
                        
                    default:
                        break; 
                }
            }
            
            InitializeEffectCooldowns();
            InitializeProcEffects();

            InitializeSpellTemplates();
        }

        private void InitializeProcEffects()
        {
            Stats baseStats = BaseStats;
            int N = baseStats._rawSpecialEffectDataSize + 1;
            HasteRatingEffectsCount = 0;
            if (HasteRatingEffects == null || HasteRatingEffects.Length < N)
            {
                HasteRatingEffects = new SpecialEffect[N];
            }
            SpellPowerEffectsCount = 0;
            if (SpellPowerEffects == null || SpellPowerEffects.Length < N)
            {
                SpellPowerEffects = new SpecialEffect[N];
            }
            ResetStackingEffectsCount = 0;
            if (ResetStackingEffects == null || ResetStackingEffects.Length < N)
            {
                ResetStackingEffects = new SpecialEffect[N];
            }
            IntellectEffectsCount = 0;
            if (IntellectEffects == null || IntellectEffects.Length < N)
            {
                IntellectEffects = new SpecialEffect[N];
            }
            StackingIntellectEffectsCount = 0;
            if (StackingIntellectEffects == null || StackingIntellectEffects.Length < N)
            {
                StackingIntellectEffects = new SpecialEffect[N];
            }
            MasteryRatingEffectsCount = 0;
            if (MasteryRatingEffects == null || MasteryRatingEffects.Length < N)
            {
                MasteryRatingEffects = new SpecialEffect[N];
            }
            CritRatingEffectsCount = 0;
            if (CritRatingEffects == null || CritRatingEffects.Length < N)
            {
                CritRatingEffects = new SpecialEffect[N];
            }
            DamageProcEffectsCount = 0;
            if (DamageProcEffects == null || DamageProcEffects.Length < N)
            {
                DamageProcEffects = new SpecialEffect[N];
            }
            ManaRestoreEffectsCount = 0;
            if (ManaRestoreEffects == null || ManaRestoreEffects.Length < N)
            {
                ManaRestoreEffects = new SpecialEffect[N];
            }
            Mp5EffectsCount = 0;
            if (Mp5Effects == null || Mp5Effects.Length < N)
            {
                Mp5Effects = new SpecialEffect[N];
            }
            for (int i = 0; i < baseStats._rawSpecialEffectDataSize; i++)
            {
                SpecialEffect effect = baseStats._rawSpecialEffectData[i];
                if (CalculationsMage.IsSupportedHasteProc(effect))
                {
                    HasteRatingEffects[HasteRatingEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedSpellPowerProc(effect))
                {
                    SpellPowerEffects[SpellPowerEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedIntellectProc(effect))
                {
                    IntellectEffects[IntellectEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedStackingIntellectProc(effect))
                {
                    StackingIntellectEffects[StackingIntellectEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedMasteryProc(effect))
                {
                    MasteryRatingEffects[MasteryRatingEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedCritProc(effect))
                {
                    CritRatingEffects[CritRatingEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedDamageProc(effect))
                {
                    DamageProcEffects[DamageProcEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedManaRestoreProc(effect))
                {
                    ManaRestoreEffects[ManaRestoreEffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedMp5Proc(effect))
                {
                    Mp5Effects[Mp5EffectsCount++] = effect;
                }
                if (CalculationsMage.IsSupportedResetStackingEffect(effect))
                {
                    ResetStackingEffects[ResetStackingEffectsCount++] = effect;
                }
            }
            if (CalculationOptions.AdvancedHasteProcs && Mage2T13)
            {
                HasteRatingEffects[HasteRatingEffectsCount++] = SpecialEffect2T13;
            }
        }

        private static readonly Color[] itemColors = new Color[] {
                Color.FromArgb(0xFF, 0x00, 0xFF, 0xFF), //Aqua
                Color.FromArgb(255, 0, 0, 255),
                Color.FromArgb(0xFF, 0xFF, 0x7F, 0x50), //Coral
                Color.FromArgb(0xFF, 0xBD, 0xB7, 0x6B), //DarkKhaki
                Color.FromArgb(0xFF, 0x2F, 0x4F, 0x4F), //DarkSlateGray
                Color.FromArgb(0xFF, 0xB2, 0x22, 0x22), //Firebrick
                Color.FromArgb(0xFF, 0xFF, 0xD7, 0x00), //Gold
                Color.FromArgb(0xFF, 0xFF, 0xFF, 0xF0), //Ivory
            };

        private EffectCooldown NewEffectCooldown()
        {
            if (NeedsDisplayCalculations || ArraySet == null)
            {
                return new EffectCooldown();
            }
            else
            {
                EffectCooldown effect = ArraySet.NewEffectCooldown();
                effect.Clear();
                return effect;
            }
        }

        private EffectCooldown NewStandardEffectCooldown(EffectCooldown cachedEffect)
        {
            if (NeedsDisplayCalculations || ArraySet == null)
            {
                return cachedEffect.Clone();
            }
            else
            {
                cachedEffect.Clear();
                return cachedEffect;
            }
        }

        EffectCooldown cachedEffectEvocation = new EffectCooldown()
        {
            Mask = (int)StandardEffect.Evocation,
            Name = "Evocation",
            StandardEffect = StandardEffect.Evocation,
            Color = Color.FromArgb(0xFF, 0x7F, 0xFF, 0xD4) //Aquamarine
        };
        EffectCooldown cachedEffectPowerInfusion = new EffectCooldown()
        {
            Cooldown = PowerInfusionCooldown,
            Duration = PowerInfusionDuration,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.PowerInfusion,
            Name = "Power Infusion",
            StandardEffect = StandardEffect.PowerInfusion,
            Color = Color.FromArgb(255, 255, 255, 0),
        };
        EffectCooldown cachedEffectVolcanicPotion = new EffectCooldown()
        {
            Cooldown = float.PositiveInfinity,
            Duration = 25.0f,
            MaximumDuration = 25.0f,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.VolcanicPotion,
            Name = "Potion of the Jade Serpent",
            StandardEffect = StandardEffect.VolcanicPotion,
            Color = Color.FromArgb(0xFF, 0xFF, 0xFA, 0xCD) //LemonChiffon
        };
        EffectCooldown cachedEffectArcanePower = new EffectCooldown()
        {
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.ArcanePower,
            Name = "Arcane Power",
            StandardEffect = StandardEffect.ArcanePower,
            Color = Color.FromArgb(0xFF, 0xF0, 0xFF, 0xFF) //Azure
        };
        EffectCooldown cachedEffectCombustion = new EffectCooldown()
        {
            Duration = CombustionDuration,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.Combustion,
            Name = "Combustion",
            StandardEffect = StandardEffect.Combustion,
            Color = Color.FromArgb(255, 255, 69, 0),
        };
        EffectCooldown cachedEffectBerserking = new EffectCooldown()
        {
            Cooldown = 180.0f,
            Duration = 10.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.Berserking,
            Name = "Berserking",
            StandardEffect = StandardEffect.Berserking,
            Color = Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A) //Brown
        };
        EffectCooldown cachedEffectMage2T15Effect = new EffectCooldown()
        {
            Cooldown = 180.0f,
            Duration = 30.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.Mage2T15Effect,
            Name = "2T15",
            StandardEffect = StandardEffect.Mage2T15Effect,
            Color = Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A) //Brown
        };
        EffectCooldown cachedEffectHeroism = new EffectCooldown()
        {
            Cooldown = float.PositiveInfinity,
            Duration = 40.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.Heroism,
            Name = "Heroism",
            StandardEffect = StandardEffect.Heroism,
            Color = Color.FromArgb(0xFF, 0x80, 0x80, 0x00) //Olive
        };
        EffectCooldown cachedEffectIcyVeins = new EffectCooldown()
        {
            Duration = 20.0f,
            Mask = (int)StandardEffect.IcyVeins,
            Name = "Icy Veins",
            StandardEffect = StandardEffect.IcyVeins,
            Color = Color.FromArgb(0xFF, 0x00, 0x00, 0x8B) //DarkBlue
        };
        EffectCooldown cachedEffectMirrorImage = new EffectCooldown()
        {
            Cooldown = MirrorImageCooldown,
            Duration = MirrorImageDuration,
            Mask = (int)StandardEffect.MirrorImage,
            Name = "Mirror Image",
            StandardEffect = StandardEffect.MirrorImage,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Color = Color.FromArgb(0xFF, 0xFF, 0xA0, 0x7A), //LightSalmon
        };
        EffectCooldown cachedEffectBloodFury = new EffectCooldown()
        {
            Cooldown = 120.0f,
            Duration = 15.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.BloodFury,
            Name = "Blood Fury",
            StandardEffect = StandardEffect.BloodFury,
            Color = Color.FromArgb(0xFF, 0xA5, 0x2A, 0x2A) //Brown
        };
        EffectCooldown cachedEffectMageArmor = new EffectCooldown()
        {
            Mask = (int)StandardEffect.MageArmor,
            Name = "Mage Armor",
            StandardEffect = StandardEffect.MageArmor,
            Color = Color.FromArgb(0xFF, 0x0, 0xFF, 0xFF)
        };
        EffectCooldown cachedEffectMoltenArmor = new EffectCooldown()
        {
            Mask = (int)StandardEffect.MoltenArmor,
            Name = "Molten Armor",
            StandardEffect = StandardEffect.MoltenArmor,
            Color = Color.FromArgb(0xFF, 0x80, 0x0, 0x0)
        };
        EffectCooldown cachedEffectFrostArmor = new EffectCooldown()
        {
            Mask = (int)StandardEffect.FrostArmor,
            Name = "Frost Armor",
            StandardEffect = StandardEffect.FrostArmor,
            Color = Color.FromArgb(0xFF, 0x0, 0x0, 0xFF)
        };
        EffectCooldown cachedEffectInvocation = new EffectCooldown()
        {
            Cooldown = 0.0f,
            Duration = 60.0f,
            AutomaticConstraints = false,
            AutomaticStackingConstraints = false,
            Mask = (int)StandardEffect.Invocation,
            Name = "Invocation",
            StandardEffect = StandardEffect.Invocation,
            Color = Color.FromArgb(0xFF, 0x0, 0x0, 0xFF)
        };
        EffectCooldown cachedEffectIncantersWard = new EffectCooldown()
        {
            Cooldown = 25.0f,
            Duration = 15.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.IncantersWard,
            Name = "Incanter's Ward",
            StandardEffect = StandardEffect.IncantersWard,
            Color = Color.FromArgb(0xFF, 0x0, 0x0, 0xFF)
        };
        EffectCooldown cachedEffectIncantersWardCooldown = new EffectCooldown()
        {
            Cooldown = 25.0f,
            Duration = 25.0f,
            AutomaticConstraints = true,
            AutomaticStackingConstraints = true,
            Mask = (int)StandardEffect.IncantersWardCooldown,
            Name = "Incanter's Ward Cooldown",
            StandardEffect = StandardEffect.IncantersWardCooldown,
            Color = Color.FromArgb(0xFF, 0x0, 0x0, 0xFF)
        };


        private void InitializeEffectCooldowns()
        {
            if (CooldownList == null)
            {
                CooldownList = new List<EffectCooldown>();
            }
            else
            {
                CooldownList.Clear();
            }

            EvocationCooldown = (MageTalents.Invocation > 0) ? 0.0f : 120.0f;
            ColdsnapCooldown = (8 * 60);
            ArcanePowerCooldown = 90.0f;
            if (Mage4T13)
            {
                ArcanePowerCooldown = 40f;
            }
            ArcanePowerDuration = 15.0f;
            IcyVeinsCooldown = 180.0f;
            CombustionCooldown = 45f;
            if (Mage4T13)
            {
                // for IV very rough right now, optimum seems to be roughly 8 stacks
                IcyVeinsCooldown = 60.0f;
                CombustionCooldown = 45f; // ????
            }
            if (Mage4T14)
            {
                IcyVeinsCooldown = 90f;
                CombustionCooldown = 36f;
            }
            /*WaterElementalCooldown = (180.0f - (MageTalents.GlyphOfWaterElemental ? 30.0f : 0.0f));
            if (MageTalents.GlyphOfEternalWater)
            {
                WaterElementalDuration = float.PositiveInfinity;
            }
            else
            {
                WaterElementalDuration = 45.0f + 5.0f * MageTalents.EnduringWinter;
            }*/

            if (evocationAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectEvocation);
                cooldown.Cooldown = EvocationCooldown;
                CooldownList.Add(cooldown);
            }
            if (powerInfusionAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectPowerInfusion);
                CooldownList.Add(cooldown);
            }
            if (volcanicPotionAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectVolcanicPotion);
                CooldownList.Add(cooldown);
            }
            if (arcanePowerAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectArcanePower);
                cooldown.Cooldown = ArcanePowerCooldown;
                cooldown.Duration = ArcanePowerDuration;
                CooldownList.Add(cooldown);
            }
            if (combustionAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectCombustion);
                cooldown.Cooldown = CombustionCooldown;
                CooldownList.Add(cooldown);
            }
            if (berserkingAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectBerserking);
                CooldownList.Add(cooldown);
            }
            if (mage2T15EffectAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectMage2T15Effect);
                CooldownList.Add(cooldown);
            }
            if (heroismAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectHeroism);
                CooldownList.Add(cooldown);
            }
            if (icyVeinsAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectIcyVeins);
                cooldown.Cooldown = IcyVeinsCooldown;
                cooldown.AutomaticStackingConstraints = cooldown.AutomaticConstraints = (MageTalents.ColdSnap == 0);
                CooldownList.Add(cooldown);
            }
            if (mirrorImageAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectMirrorImage);
                CooldownList.Add(cooldown);
            }
            if (bloodFuryAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectBloodFury);
                CooldownList.Add(cooldown);
            }
            if (mageArmorAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectMageArmor);
                CooldownList.Add(cooldown);
            }
            if (moltenArmorAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectMoltenArmor);
                CooldownList.Add(cooldown);
            }
            if (frostArmorAvailable)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectFrostArmor);
                CooldownList.Add(cooldown);
            }
            if (MageTalents.Invocation > 0)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectInvocation);
                CooldownList.Add(cooldown);
            }
            if (MageTalents.IncantersWard > 0 && IncomingDamageDpsRaw > 0)
            {
                EffectCooldown cooldown = NewStandardEffectCooldown(cachedEffectIncantersWard);
                CooldownList.Add(cooldown);
                cooldown = NewStandardEffectCooldown(cachedEffectIncantersWardCooldown);
                CooldownList.Add(cooldown);
            }

            cooldownCount = standardEffectCount;
            int mask = 1 << standardEffectCount;

            int N = BaseStats._rawSpecialEffectDataSize;
            ItemBasedEffectCooldownsCount = 0;
            if (ItemBasedEffectCooldowns == null || ItemBasedEffectCooldowns.Length < N)
            {
                ItemBasedEffectCooldowns = new EffectCooldown[N];
            }
            StackingHasteEffectCooldownsCount = 0;
            if (StackingHasteEffectCooldowns == null || StackingHasteEffectCooldowns.Length < N)
            {
                StackingHasteEffectCooldowns = new EffectCooldown[N];
            }
            StackingNonHasteEffectCooldownsCount = 0;
            if (StackingNonHasteEffectCooldowns == null || StackingNonHasteEffectCooldowns.Length < N)
            {
                StackingNonHasteEffectCooldowns = new EffectCooldown[N];
            }
            int itemBasedMask = 0;
            bool hasteEffect, stackingEffect;

            int colorIndex = 0;

            if (!CalculationOptions.DisableCooldowns)
            {
                for (CharacterSlot i = 0; i < (CharacterSlot)Character.OptimizableSlotCount; i++)
                {
                    ItemInstance itemInstance = Character[i];
                    if ((object)itemInstance != null)
                    {
                        Item item = itemInstance.Item;
                        if (item != null)
                        {
                            Stats stats = item.Stats;
                            for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                            {
                                SpecialEffect effect = stats._rawSpecialEffectData[j];
                                if (CalculationsMage.IsSupportedUseEffect(effect, out hasteEffect, out stackingEffect))
                                {
                                    EffectCooldown cooldown = NewEffectCooldown();
                                    cooldown.StandardEffect = 0;
                                    cooldown.SpecialEffect = effect;
                                    cooldown.HasteEffect = hasteEffect;
                                    cooldown.Mask = mask;
                                    itemBasedMask |= mask;
                                    mask <<= 1;
                                    cooldownCount++;
                                    cooldown.ItemBased = true;
                                    cooldown.Name = item.Name;
                                    cooldown.Cooldown = effect.Cooldown;
                                    cooldown.Duration = effect.Duration;
                                    cooldown.AutomaticConstraints = true;
                                    cooldown.AutomaticStackingConstraints = true;
                                    cooldown.Color = itemColors[Math.Min(itemColors.Length - 1, colorIndex++)];
                                    CooldownList.Add(cooldown);
                                    ItemBasedEffectCooldowns[ItemBasedEffectCooldownsCount++] = cooldown;
                                    if (stackingEffect)
                                    {
                                        if (hasteEffect)
                                        {
                                            StackingHasteEffectCooldowns[StackingHasteEffectCooldownsCount++] = cooldown;
                                        }
                                        else
                                        {
                                            StackingNonHasteEffectCooldowns[StackingNonHasteEffectCooldownsCount++] = cooldown;
                                        }
                                    }
                                }
                            }
                        }
                        Enchant enchant = itemInstance.Enchant;
                        if (enchant != null)
                        {
                            Stats stats = enchant.Stats;
                            for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                            {
                                SpecialEffect effect = stats._rawSpecialEffectData[j];
                                if (CalculationsMage.IsSupportedUseEffect(effect, out hasteEffect, out stackingEffect))
                                {
                                    EffectCooldown cooldown = NewEffectCooldown();
                                    cooldown.StandardEffect = 0;
                                    cooldown.SpecialEffect = effect;
                                    cooldown.Mask = mask;
                                    cooldown.HasteEffect = hasteEffect;
                                    itemBasedMask |= mask;
                                    mask <<= 1;
                                    cooldownCount++;
                                    cooldown.ItemBased = true;
                                    cooldown.Name = enchant.Name;
                                    cooldown.Cooldown = effect.Cooldown;
                                    cooldown.Duration = effect.Duration;
                                    cooldown.AutomaticConstraints = true;
                                    cooldown.AutomaticStackingConstraints = true;
                                    cooldown.Color = itemColors[Math.Min(itemColors.Length - 1, colorIndex++)];
                                    CooldownList.Add(cooldown);
                                    ItemBasedEffectCooldowns[ItemBasedEffectCooldownsCount++] = cooldown;
                                    if (stackingEffect)
                                    {
                                        if (hasteEffect)
                                        {
                                            StackingHasteEffectCooldowns[StackingHasteEffectCooldownsCount++] = cooldown;
                                        }
                                        else
                                        {
                                            StackingNonHasteEffectCooldowns[StackingNonHasteEffectCooldownsCount++] = cooldown;
                                        }
                                    }
                                }
                            }
                        }
                        Tinkering tinkering = itemInstance.Tinkering;
                        if (tinkering != null)
                        {
                            Stats stats = tinkering.Stats;
                            for (int j = 0; j < stats._rawSpecialEffectDataSize; j++)
                            {
                                SpecialEffect effect = stats._rawSpecialEffectData[j];
                                if (CalculationsMage.IsSupportedUseEffect(effect, out hasteEffect, out stackingEffect))
                                {
                                    EffectCooldown cooldown = NewEffectCooldown();
                                    cooldown.StandardEffect = 0;
                                    cooldown.SpecialEffect = effect;
                                    cooldown.Mask = mask;
                                    cooldown.HasteEffect = hasteEffect;
                                    itemBasedMask |= mask;
                                    mask <<= 1;
                                    cooldownCount++;
                                    cooldown.ItemBased = true;
                                    cooldown.Name = tinkering.Name;
                                    cooldown.Cooldown = effect.Cooldown;
                                    cooldown.Duration = effect.Duration;
                                    cooldown.AutomaticConstraints = true;
                                    cooldown.AutomaticStackingConstraints = true;
                                    cooldown.Color = itemColors[Math.Min(itemColors.Length - 1, colorIndex++)];
                                    CooldownList.Add(cooldown);
                                    ItemBasedEffectCooldowns[ItemBasedEffectCooldownsCount++] = cooldown;
                                    if (stackingEffect)
                                    {
                                        if (hasteEffect)
                                        {
                                            StackingHasteEffectCooldowns[StackingHasteEffectCooldownsCount++] = cooldown;
                                        }
                                        else
                                        {
                                            StackingNonHasteEffectCooldowns[StackingNonHasteEffectCooldownsCount++] = cooldown;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (manaGemEffectAvailable)
            {
                EffectCooldown cooldown = NewEffectCooldown();
                cooldown.StandardEffect = StandardEffect.ManaGemEffect;
                cooldown.SpecialEffect = null;
                cooldown.Mask = (int)StandardEffect.ManaGemEffect;
                cooldown.HasteEffect = false;
                cooldown.ItemBased = false;
                cooldown.Name = "Improved Mana Gem";
                cooldown.Cooldown = ImprovedManaGemCooldown;
                cooldown.Duration = ImprovedManaGemDuration;
                cooldown.MaximumDuration = (float)MaximizeEffectDuration(CalculationOptions.FightDuration, ImprovedManaGemDuration, ImprovedManaGemCooldown);
                cooldown.AutomaticConstraints = false;
                cooldown.AutomaticStackingConstraints = true;
                cooldown.Color = Color.FromArgb(0xFF, 0x00, 0x64, 0x00); //DarkGreen
                CooldownList.Add(cooldown);
                ManaGemEffectDuration = ImprovedManaGemDuration;
            }
            else
            {
                if (CombinatorialSolver && CalculationOptions.ManaGemEnabled)
                {
                    EffectCooldown cooldown = NewEffectCooldown();
                    cooldown.StandardEffect = StandardEffect.ManaGemEffect;
                    cooldown.SpecialEffect = null;
                    cooldown.Mask = (int)StandardEffect.ManaGemEffect;
                    cooldown.HasteEffect = false;
                    cooldown.ItemBased = false;
                    cooldown.Name = "Improved Mana Gem";
                    cooldown.Cooldown = ImprovedManaGemCooldown;
                    cooldown.Duration = 0;
                    cooldown.MaximumDuration = 0;
                    cooldown.AutomaticConstraints = false;
                    cooldown.AutomaticStackingConstraints = false;
                    cooldown.Color = Color.FromArgb(0xFF, 0x00, 0x64, 0x00); //DarkGreen
                    CooldownList.Add(cooldown);
                }
                ManaGemEffectDuration = 0;
            }

            if (EffectCooldown == null)
            {
                EffectCooldown = new Dictionary<int, EffectCooldown>(CooldownList.Count);
            }
            else
            {
                EffectCooldown.Clear();
            }
            availableCooldownMask = 0;
            foreach (EffectCooldown cooldown in CooldownList)
            {
                EffectCooldown[cooldown.Mask] = cooldown;
                if (cooldown.StandardEffect != StandardEffect.Evocation && cooldown.Duration > 0)
                {
                    availableCooldownMask |= cooldown.Mask;
                }
            }

            if (effectExclusionList == null)
            {
                effectExclusionList = new int[]
                {
                    (int)(StandardEffect.ArcanePower | StandardEffect.PowerInfusion),
                    //(int)(StandardEffect.PotionOfSpeed | StandardEffect.PotionOfWildMagic),
                    itemBasedMask
                };
            }
            else
            {
                effectExclusionList[1] = itemBasedMask;
            }
        }

        private void CalculateBaseStateStats()
        {
            Stats baseStats = BaseStats;            
            BaseSpellHit = (baseStats.HitRating + baseStats.ExpertiseRating) * 0.01f / CalculationOptions.HitRatingMultiplier + baseStats.SpellHit + baseStats.Expertise;

            int targetLevel = CalculationOptions.TargetLevel;
            int playerLevel = CalculationOptions.PlayerLevel;

            float hitRate = ((targetLevel >= playerLevel - 2) ? (0.94f - (targetLevel - playerLevel) * 0.03f) : 1.0f) + BaseSpellHit;
            RawArcaneHitRate = hitRate;
            RawFireHitRate = hitRate;
            RawFrostHitRate = hitRate;
            hitRate = Math.Min(Spell.MaxHitRate, hitRate);

            BaseArcaneHitRate = hitRate;
            BaseFireHitRate = hitRate;
            BaseFrostHitRate = hitRate;
            BaseNatureHitRate = hitRate;
            BaseShadowHitRate = hitRate;
            BaseFrostFireHitRate = hitRate;
            BaseHolyHitRate = hitRate;

            float threatFactor = (1 + baseStats.ThreatIncreaseMultiplier) * (1 - baseStats.ThreatReductionMultiplier);

            ArcaneThreatMultiplier = threatFactor;
            FireThreatMultiplier = threatFactor;
            FrostThreatMultiplier = threatFactor;
            FrostFireThreatMultiplier = threatFactor;
            NatureThreatMultiplier = threatFactor;
            ShadowThreatMultiplier = threatFactor;
            HolyThreatMultiplier = threatFactor;

            float baseSpellModifier = (1 + baseStats.BonusDamageMultiplier) * CalculationOptions.EffectDamageMultiplier;
            if (CalculationOptions.PVP)
            {
                baseSpellModifier *= (1 + baseStats.PvPPower * 0.01f / CalculationOptions.PvPPowerMultiplier);
            }
            if (MageTalents.RuneOfPower > 0)
            {
                baseSpellModifier *= 1.15f;
            }
            float baseAdditiveSpellModifier = 1.0f;
            BaseSpellModifier = baseSpellModifier;
            BaseAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseArcaneSpellModifier = baseSpellModifier * (1 + baseStats.BonusArcaneDamageMultiplier);
            BaseArcaneAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseFireSpellModifier = baseSpellModifier * (1 + baseStats.BonusFireDamageMultiplier);
            BaseFireAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseFrostSpellModifier = baseSpellModifier * (1 + baseStats.BonusFrostDamageMultiplier);
            BaseFrostAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseNatureSpellModifier = baseSpellModifier * (1 + baseStats.BonusNatureDamageMultiplier);
            BaseNatureAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseShadowSpellModifier = baseSpellModifier * (1 + baseStats.BonusShadowDamageMultiplier);
            BaseShadowAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseHolySpellModifier = baseSpellModifier * (1 + baseStats.BonusHolyDamageMultiplier);
            BaseHolyAdditiveSpellModifier = baseAdditiveSpellModifier;
            BaseFrostFireSpellModifier = baseSpellModifier * Math.Max(1 + baseStats.BonusFireDamageMultiplier, 1 + baseStats.BonusFrostDamageMultiplier);
            BaseFrostFireAdditiveSpellModifier = baseAdditiveSpellModifier;

            float spellCritBase = 0.9100000374019f;
            SpellCritPerInt = 1 / BaseCombatRating.MageChanceToSpellCrit(playerLevel);

            float spellCrit = 0.01f * (baseStats.Intellect * SpellCritPerInt + spellCritBase) + baseStats.CritRating * 0.01f / CalculationOptions.CritRatingMultiplier + baseStats.SpellCrit + baseStats.SpellCritOnTarget;
            if (targetLevel > playerLevel)
            {
                spellCrit -= (targetLevel - playerLevel) * 0.01f;
            }

            BaseCritRate = spellCrit;
            BaseArcaneCritRate = spellCrit;
            BaseFireCritRate = spellCrit;
            BaseFrostFireCritRate = spellCrit;
            BaseFrostCritRate = spellCrit;
            BaseNatureCritRate = spellCrit;
            BaseShadowCritRate = spellCrit;
            BaseHolyCritRate = spellCrit;

            Mastery = 8 + baseStats.Mastery + baseStats.MasteryRating / CalculationOptions.MasteryRatingMultiplier;
            ManaAdeptBonus = 0.0f;
            IgniteBonus = 0.0f;
            FrostburnBonus = 0.0f;
            if (Specialization == Specialization.Arcane)
            {
                ManaAdeptMultiplier = 0.02f;
                ManaAdeptBonus = ManaAdeptMultiplier * Mastery;
                needsQuadratic = true;
                needsSolutionVariables = true;
            }
            else if (Specialization == Specialization.Fire)
            {
                IgniteMultiplier = 0.015f;
                IgniteBonus = IgniteMultiplier * Mastery;
            }
            else if (Specialization == Specialization.Frost)
            {
                FrostburnMultiplier = 0.02f;
                FrostburnBonus = FrostburnMultiplier * Mastery;
            }

            float mult = (1.5f * 1.33f * (1 + baseStats.BonusSpellCritDamageMultiplier) - 1);
            float baseAddMult = (1 + baseStats.CritBonusDamage);
            BaseArcaneCritBonus = (1 + mult * baseAddMult);
            BaseFireCritBonus = (1 + mult * baseAddMult);
            BaseFrostCritBonus = (1 + mult * baseAddMult);
            BaseFrostFireCritBonus = (1 + mult * baseAddMult);
            BaseNatureCritBonus = 
            BaseShadowCritBonus =
            BaseHolyCritBonus = (1 + mult * baseAddMult); // unknown if affected by burnout

            CastingSpeedMultiplier = (1f + baseStats.SpellHaste) * CalculationOptions.EffectHasteMultiplier;
            BaseCastingSpeed = (1 + baseStats.HasteRating * 0.01f / CalculationOptions.HasteRatingMultiplier) * CastingSpeedMultiplier;
            BaseGlobalCooldown = Math.Max(Spell.GlobalCooldownLimit, 1.5f / BaseCastingSpeed);

            float baseRegen = 0f;
            float regenMultiplier = 0.01f;
            if (playerLevel < 90)
            {
                regenMultiplier = Math.Min(0.02f, 0.01f + (90 - playerLevel) * 0.02f);
            }
            float baseCombatRegen = CalculationOptions.BaseMana * regenMultiplier;
            baseRegen = BaseCombatRating.MageManaPerSpirit(playerLevel);

            if (!CalculationOptions.EffectDisableManaSources)
            {
                SpiritRegen = (0.001f + 5 * baseStats.Spirit * BaseCombatRating.MageManaPerSpirit(playerLevel)) * CalculationOptions.EffectRegenMultiplier;
                BaseManaRegen = baseCombatRegen;
                if (MageTalents.Invocation > 0)
                {
                    //SpiritRegen *= 0.5f;
                    //BaseManaRegen *= 0.5f;
                }
                else if (MageTalents.RuneOfPower > 0)
                {
                    SpiritRegen *= 1.75f;
                    BaseManaRegen *= 1.75f;
                }
                FlatManaRegen = baseStats.Mp5 / 5f + 15732 * CalculationOptions.Innervate / CalculationOptions.FightDuration + baseStats.ManaRestoreFromMaxManaPerSecond * baseStats.Mana;
            }
            else
            {
                SpiritRegen = 0;
                BaseManaRegen = 0;
                FlatManaRegen = 0;
            }
            HealthRegen = 0.0312f * baseStats.Spirit + baseStats.Hp5 / 5f;
            HealthRegenCombat = baseStats.Hp5 / 5f;
            if (playerLevel < 75)
            {
                FlatManaRegenDrinking = FlatManaRegen + 240f;
                HealthRegenEating = HealthRegen + 250f;
            }
            else if (playerLevel < 80)
            {
                FlatManaRegenDrinking = FlatManaRegen + 306f;
                HealthRegenEating = HealthRegen + 440f;
            }
            else
            {
                FlatManaRegenDrinking = FlatManaRegen + 640f;
                HealthRegenEating = HealthRegen + 750f;
            }

            BaseArcaneSpellPower = baseStats.SpellArcaneDamageRating + baseStats.SpellPower;
            BaseFireSpellPower = baseStats.SpellFireDamageRating + baseStats.SpellPower;
            BaseFrostSpellPower = baseStats.SpellFrostDamageRating + baseStats.SpellPower;
            BaseNatureSpellPower = baseStats.SpellNatureDamageRating + baseStats.SpellPower;
            BaseShadowSpellPower = baseStats.SpellShadowDamageRating + baseStats.SpellPower;
            BaseHolySpellPower = /* baseStats.SpellHolyDamageRating + */ baseStats.SpellPower;
        }

        private void CalculateIncomingDamage()
        {
            Stats baseStats = BaseStats;
            int targetLevel = CalculationOptions.TargetLevel;
            int playerLevel = CalculationOptions.PlayerLevel;

            //MeleeMitigation = (1 - 1 / (1 + 0.1f * baseStats.Armor / (8.5f * (targetLevel + 4.5f * (targetLevel - 59)) + 40)));
            MeleeMitigation = baseStats.Armor / (baseStats.Armor + 2167.5f * targetLevel - 158167.5f);
            //Defense = 5 * playerLevel + baseStats.DefenseRating / 4.918498039f; // this is for level 80 only
            int molten = (armor == "Molten Armor") ? 1 : 0;
            PhysicalCritReduction = (/*0.04f * (Defense - 5 * CalculationOptions.PlayerLevel) / 100 +*/ /*baseStats.Resilience / resilienceFactor * levelScalingFactor + */molten * 0.06f);
            //SpellCritReduction = (baseStats.Resilience / resilienceFactor * levelScalingFactor + molten * 0.05f);
            //CritDamageReduction = (baseStats.Resilience / resilienceFactor * 2.2f * levelScalingFactor);
            SpellCritReduction = 0;
            CritDamageReduction = 0;
            if (CalculationOptions.PVP)
            {
                DamageTakenReduction = (float)(1 - 0.6 * 37.827 / (37.827 + baseStats.PvPResilience / CalculationOptions.PvPResilienceMultiplier));
            }
            else
            {
                DamageTakenReduction = 0;
            }
            Dodge = 0.036174132823944f + 0.01f / (0.006650f + 0.9830f / (/*(0.04f * (Defense - 5 * playerLevel)) / 100f +*/ baseStats.DodgeRating * 0.01f / BaseCombatRating.DodgeRatingMultiplier(CalculationOptions.PlayerLevel) + (baseStats.Agility - 46f) * 0.003331530f));


            IncomingDamageAmpMelee = (1 - MeleeMitigation) * (1 - Dodge) * (1 - DamageTakenReduction);
            IncomingDamageAmpPhysical = (1 - MeleeMitigation) * (1 - DamageTakenReduction);
            IncomingDamageAmpArcane = (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.ArcaneResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpFire = (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.FireResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpFrost = (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.FrostResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpNature = (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.NatureResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpShadow = (1 - StatConversion.GetAverageResistance(targetLevel, playerLevel, baseStats.ShadowResistance, 0)) * (1 - DamageTakenReduction);
            IncomingDamageAmpHoly = (1 - DamageTakenReduction);

            IncomingDamageDpsMelee = IncomingDamageAmpMelee * (CalculationOptions.MeleeDps * (1 + Math.Max(0, CalculationOptions.MeleeCrit / 100.0f - PhysicalCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.MeleeDot);
            IncomingDamageDpsPhysical = IncomingDamageAmpPhysical * (CalculationOptions.PhysicalDps * (1 + Math.Max(0, CalculationOptions.PhysicalCrit / 100.0f - PhysicalCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.PhysicalDot);
            IncomingDamageDpsArcane = IncomingDamageAmpArcane * (CalculationOptions.ArcaneDps * (1 + Math.Max(0, CalculationOptions.ArcaneCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.ArcaneDot);
            IncomingDamageDpsFire = IncomingDamageAmpFire * (CalculationOptions.FireDps * (1 + Math.Max(0, CalculationOptions.FireCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.FireDot);
            IncomingDamageDpsFrost = IncomingDamageAmpFrost * (CalculationOptions.FrostDps * (1 + Math.Max(0, CalculationOptions.FrostCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.FrostDot);
            IncomingDamageDpsNature = IncomingDamageAmpNature * (CalculationOptions.NatureDps * (1 + Math.Max(0, CalculationOptions.NatureCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.NatureDot);
            IncomingDamageDpsShadow = IncomingDamageAmpShadow * (CalculationOptions.ShadowDps * (1 + Math.Max(0, CalculationOptions.ShadowCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.ShadowDot);
            IncomingDamageDpsHoly = IncomingDamageAmpHoly * (CalculationOptions.HolyDps * (1 + Math.Max(0, CalculationOptions.HolyCrit / 100.0f - SpellCritReduction) * (2 * (1 - CritDamageReduction) - 1)) + CalculationOptions.HolyDot);

            IncomingDamageDps = IncomingDamageDpsMelee + IncomingDamageDpsPhysical + IncomingDamageDpsArcane + IncomingDamageDpsFire + IncomingDamageDpsFrost + IncomingDamageDpsShadow + IncomingDamageDpsNature + IncomingDamageDpsHoly;
            IncomingDamageDpsRaw = IncomingDamageDps;
            //float incanterSpellPower = Math.Min((float)Math.Min(calculationOptions.AbsorptionPerSecond, calculationResult.IncomingDamageDps) * 0.05f * talents.IncantersAbsorption * 10, 0.05f * baseStats.Health);
            if (CalculationOptions.AbsorptionPerSecond > IncomingDamageDps)
            {
                IncomingDamageDps = 0.0f;
            }
            else
            {
                IncomingDamageDps -= CalculationOptions.AbsorptionPerSecond;
            }
        }

        private void InitializeSpellTemplates()
        {
            if (_WaterboltTemplate != null) _WaterboltTemplate.Dirty = true;
            if (_MirrorImageTemplate != null) _MirrorImageTemplate.Dirty = true;
            if (_FireBlastTemplate != null) _FireBlastTemplate.Dirty = true;
            if (_InfernoBlastTemplate != null) _InfernoBlastTemplate.Dirty = true;
            if (_FrozenOrbTemplate != null) _FrozenOrbTemplate.Dirty = true;
            if (_FrostboltTemplate != null) _FrostboltTemplate.Dirty = true;
            if (_FrostfireBoltTemplate != null) _FrostfireBoltTemplate.Dirty = true;
            if (_ArcaneMissilesTemplate != null) _ArcaneMissilesTemplate.Dirty = true;
            if (_FireballTemplate != null) _FireballTemplate.Dirty = true;
            if (_PyroblastTemplate != null) _PyroblastTemplate.Dirty = true;
            if (_PyroblastHardCastTemplate != null) _PyroblastHardCastTemplate.Dirty = true;
            if (_ScorchTemplate != null) _ScorchTemplate.Dirty = true;
            if (_CombustionTemplate != null) _CombustionTemplate.Dirty = true;
            if (_ArcaneBarrageTemplate != null) _ArcaneBarrageTemplate.Dirty = true;
            if (_DeepFreezeTemplate != null) _DeepFreezeTemplate.Dirty = true;
            if (_ArcaneBlastTemplate != null) _ArcaneBlastTemplate.Dirty = true;
            if (_IceLanceTemplate != null) _IceLanceTemplate.Dirty = true;
            if (_ArcaneExplosionTemplate != null) _ArcaneExplosionTemplate.Dirty = true;
            if (_FlamestrikeTemplate != null) _FlamestrikeTemplate.Dirty = true;
            if (_BlizzardTemplate != null) _BlizzardTemplate.Dirty = true;
            if (_BlastWaveTemplate != null) _BlastWaveTemplate.Dirty = true;
            if (_DragonsBreathTemplate != null) _DragonsBreathTemplate.Dirty = true;
            if (_ConeOfColdTemplate != null) _ConeOfColdTemplate.Dirty = true;
            if (_SlowTemplate != null) _SlowTemplate.Dirty = true;
            if (_LivingBombTemplate != null) _LivingBombTemplate.Dirty = true;
            if (_FrostBombTemplate != null) _FrostBombTemplate.Dirty = true;
            if (_IncantersWardTemplate != null) _IncantersWardTemplate.Dirty = true;
            if (_ConjureManaGemTemplate != null) _ConjureManaGemTemplate.Dirty = true;
            if (_ArcaneDamageTemplate != null) _ArcaneDamageTemplate.Dirty = true;
            if (_FireDamageTemplate != null) _FireDamageTemplate.Dirty = true;
            if (_FrostDamageTemplate != null) _FrostDamageTemplate.Dirty = true;
            if (_ShadowDamageTemplate != null) _ShadowDamageTemplate.Dirty = true;
            if (_NatureDamageTemplate != null) _NatureDamageTemplate.Dirty = true;
            if (_HolyDamageTemplate != null) _HolyDamageTemplate.Dirty = true;
            if (_HolySummonedDamageTemplate != null) _HolySummonedDamageTemplate.Dirty = true;
            if (_FireSummonedDamageTemplate != null) _FireSummonedDamageTemplate.Dirty = true;
            if (_NetherTempestTemplate != null) _NetherTempestTemplate.Dirty = true;
        }
        #endregion

        #region Construct Problem
        private void AddSegmentTicks(List<double> ticks, double cooldownDuration)
        {
            for (int i = 0; i * 0.5 * cooldownDuration < CalculationOptions.FightDuration; i++)
            {
                ticks.Add(i * 0.5 * cooldownDuration);
            }
        }

        private void AddEffectTicks(List<double> ticks, double cooldownDuration, double effectDuration)
        {
            for (int i = 0; i * cooldownDuration + effectDuration < CalculationOptions.FightDuration; i++)
            {
                ticks.Add(i * cooldownDuration + effectDuration);
                if (i * cooldownDuration + effectDuration > CalculationOptions.FightDuration - effectDuration)
                {
                    ticks.Add(CalculationOptions.FightDuration - effectDuration);
                }
            }
        }

#if SILVERLIGHT
        private void ConstructProblem()
#else
        private unsafe void ConstructProblem()
#endif
        {
            Stats baseStats = BaseStats;

            ConstructSegments();

            //segments = (segmentCooldowns) ? (int)Math.Ceiling(calculationOptions.FightDuration / segmentDuration) : 1;
            if (requiresMIP)
            {
                segmentColumn = new int[SegmentList.Count + 1];
            }            

            needsTimeExtension = false;
            bool afterFightRegen = CalculationOptions.FarmingMode;
            conjureManaGem = CalculationOptions.ManaGemEnabled && CalculationOptions.FightDuration > 500.0f;
            //wardsAvailable = calculationResult.IncomingDamageDpsFire + calculationResult.IncomingDamageDpsFrost > 0.0 && talents.FrostWarding > 0;

            minimizeTime = false;
            if (CalculationOptions.TargetDamage > 0)
            {
                if (!needsQuadratic)
                {
                    minimizeTime = true;
                }
                needsTimeExtension = true;
            }

            restrictManaUse = false;
            if (segmentCooldowns || segmentMana) restrictManaUse = true;
            if (CalculationOptions.UnlimitedMana)
            {
                restrictManaUse = false;
                integralMana = false;
                segmentMana = false;
            }
            segmentNonCooldowns = false;
            if (segmentCooldowns)
            {
                if (restrictManaUse) segmentNonCooldowns = true;
                //if (restrictThreat) segmentNonCooldowns = true;
            }

            dpsTime = CalculationOptions.DpsTime;
            float silenceTime = CalculationOptions.EffectShadowSilenceFrequency * CalculationOptions.EffectShadowSilenceDuration * Math.Max(1 - baseStats.ShadowResistance / CalculationOptions.TargetLevel * 0.15f, 0.25f);
            if (1 - silenceTime < dpsTime) dpsTime = 1 - silenceTime;
            if (CalculationOptions.MovementFrequency > 0)
            {
                float movementShare = CalculationOptions.MovementDuration / CalculationOptions.MovementFrequency / (1 + baseStats.MovementSpeed);
                dpsTime -= movementShare;
            }

            if (CombinatorialSolver)
            {
                manaSegments = combStateList.Count;
            }
            else if (segmentMana)
            {
                if (segmentCooldowns)
                {
                    manaSegments = 2;
                }
                else
                {
                    manaSegments = (int)Math.Ceiling(CalculationOptions.FightDuration / EvocationCooldown) + 1;
                }
            }
            else
            {
                manaSegments = 1;
            }

            int rowCount = ConstructRows(minimizeTime, DrinkingEnabled, needsTimeExtension, afterFightRegen);

            if (lp == null)
            {
                lp = new SolverLP();
            }
            lp.Initialize(ArraySet, rowCount, 9 + (12 + (CalculationOptions.EnableHastedEvocation ? 6 : 0) + spellList.Count * stateList.Count) * manaSegments * SegmentList.Count, this, SegmentList.Count);

            if (needsSolutionVariables)
            {
                SolutionVariable = new List<SolutionVariable>();
            }

#if !SILVERLIGHT
            fixed (double* pRowScale = lp.ArraySet.rowScale, pColumnScale = lp.ArraySet.columnScale, pCost = lp.ArraySet._cost, pData = lp.ArraySet.SparseMatrixData, pValue = lp.ArraySet.SparseMatrixValue)
            fixed (int* pRow = lp.ArraySet.SparseMatrixRow, pCol = lp.ArraySet.SparseMatrixCol)
#endif
            {
#if SILVERLIGHT
                lp.BeginSafe(lp.ArraySet.rowScale, lp.ArraySet.columnScale, lp.ArraySet._cost, lp.ArraySet.SparseMatrixData, lp.ArraySet.SparseMatrixValue, lp.ArraySet.SparseMatrixRow, lp.ArraySet.SparseMatrixCol);
#else
                lp.BeginUnsafe(pRowScale, pColumnScale, pCost, pData, pValue, pRow, pCol);
#endif

                #region Set LP Scaling
                lp.SetRowScaleUnsafe(rowManaRegen, ManaRegenLPScaling);
                //lp.SetRowScaleUnsafe(rowManaGem, 40.0);
                //lp.SetRowScaleUnsafe(rowPotion, 40.0);
                //lp.SetRowScaleUnsafe(rowManaGemMax, 40.0);
                //lp.SetRowScaleUnsafe(rowManaPotion, 40.0);
                //lp.SetRowScaleUnsafe(rowCombustion, 10.0);
                //lp.SetRowScaleUnsafe(rowHeroismCombustion, 10.0);
                //lp.SetRowScaleUnsafe(rowMoltenFuryCombustion, 10.0);
                //lp.SetRowScaleUnsafe(rowThreat, 0.001);
                lp.SetRowScaleUnsafe(rowCount, 0.05);
                if (restrictManaUse)
                {
                    if (CombinatorialSolver)
                    {
                        for (int i = 0; i < combStateList.Count; i++)
                        {
                            lp.SetRowScaleUnsafe(combStateList[i].UnderflowConstraint, ManaRegenLPScaling);
                            lp.SetRowScaleUnsafe(combStateList[i].OverflowConstraint, ManaRegenLPScaling);
                        }
                    }
                    else
                    {
                        for (int ss = 0; ss < manaSegments * SegmentList.Count - 1; ss++)
                        {
                            lp.SetRowScaleUnsafe(rowSegmentManaUnderflow + ss, ManaRegenLPScaling);
                        }
                        for (int ss = 0; ss < manaSegments * SegmentList.Count; ss++)
                        {
                            lp.SetRowScaleUnsafe(rowSegmentManaOverflow + ss, ManaRegenLPScaling);
                        }
                    }
                }
                /*if (restrictThreat)
                {
                    for (int ss = 0; ss < SegmentList.Count - 1; ss++)
                    {
                        lp.SetRowScaleUnsafe(rowSegmentThreat + ss, 0.001);
                    }
                }*/
                #endregion

                float threatFactor = (1 + baseStats.ThreatIncreaseMultiplier) * (1 - baseStats.ThreatReductionMultiplier);

                ConstructIdleRegen();
                //ConstructWand();
                ConstructEvocation(baseStats, threatFactor);
                ConstructManaPotion(baseStats, threatFactor);
                ConstructManaGem(baseStats, threatFactor);
                //ConstructSummonWaterElemental();
                ConstructSummonMirrorImage();
                ConstructDrinking();
                ConstructTimeExtension();
                ConstructAfterFightRegen(afterFightRegen);
                ConstructManaOverflow();
                ConstructConjureManaGem();
                ConstructSpells();

                lp.EndColumnConstruction();
                SetProblemRHS();

                lp.EndUnsafe();
            }
        }

        private void CalculateStartingMana()
        {
            StartingMana = Math.Min(BaseStats.Mana, BaseState.ManaRegenDrinking * CalculationOptions.DrinkingTime);
            MaxDrinkingTime = Math.Min(30, (BaseStats.Mana - StartingMana) / BaseState.ManaRegenDrinking);
            DrinkingEnabled = (MaxDrinkingTime > 0.000001);
        }

        private void ConstructSpells()
        {
            int column = 0;
            if (UseIncrementalOptimizations)
            {
                int lastSegment = -1;
                for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                {
                    if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.Spell)
                    {
                        for (int buffset = 0; buffset < stateList.Count; buffset++)
                        {
                            CastingState state = stateList[buffset];
                            if ((state.Effects & (int)StandardEffect.NonItemBasedMask) == CalculationOptions.IncrementalSetStateIndexes[index])
                            {
                                if (CalculationOptions.CooldownRestrictionsValid(SegmentList[CalculationOptions.IncrementalSetSegments[index]], state))
                                {
                                    float mult = segmentCooldowns ? CalculationOptions.GetDamageMultiplier(SegmentList[CalculationOptions.IncrementalSetSegments[index]]) : 1.0f;
                                    if (mult > 0)
                                    {
                                        Cycle c = state.GetCycle(CalculationOptions.IncrementalSetSpells[index]);
                                        if (c.CycleId == CycleId.ArcaneManaNeutral)
                                        {
                                            c.FixManaNeutral();
                                        }
                                        int seg = CalculationOptions.IncrementalSetSegments[index];
                                        int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                                        column = lp.AddColumnUnsafe();
                                        if (requiresMIP)
                                        {
                                            if (seg != lastSegment)
                                            {
                                                for (; lastSegment < seg; )
                                                {
                                                    segmentColumn[++lastSegment] = column;
                                                }
                                            }
                                        }
                                        if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { State = state, Cycle = c, Segment = seg, ManaSegment = manaSegment, Type = VariableType.Spell, Dps = c.DamagePerSecond, Mps = c.ManaPerSecond });
                                        SetSpellColumn(minimizeTime, seg, manaSegment, state, column, c, mult);
                                    }
                                }
                            }
                        }
                    }
                }
                if (requiresMIP)
                {
                    for (; lastSegment < SegmentList.Count; )
                    {
                        segmentColumn[++lastSegment] = column + 1;
                    }
                }
            }
            else if (CombinatorialSolver)
            {
                List<Cycle> placed = new List<Cycle>();
                for (int manaSegment = manaSegments - 1; manaSegment >= 0; manaSegment--)
                {
                    CastingState state = combStateList[manaSegment].CastingState;
                    if (!state.Evocation)
                    {
                        placed.Clear();
                        for (int spell = 0; spell < spellList.Count; spell++)
                        {
                            Cycle c = state.GetCycle(spellList[spell]);
                            if (c.CycleId == CycleId.ArcaneManaNeutral)
                            {
                                c.FixManaNeutral();
                            }
                            bool skip = false;
                            foreach (Cycle s2 in placed)
                            {
                                // TODO verify it this is ok, it assumes that spells placed under same casting state are independent except for aoe spells
                                // assuming there are no constraints that depend on properties of particular spell cycle instead of properties of casting state
                                if (!c.AreaEffect && s2.DamagePerSecond >= c.DamagePerSecond - 0.00001 && s2.ManaPerSecond <= c.ManaPerSecond + 0.00001)
                                {
                                    skip = true;
                                    break;
                                }
                            }
                            if ((c.ManaPerSecond < -0.001 && c.CycleId != CycleId.ArcaneManaNeutral) && (CalculationOptions.DisableManaRegenCycles && Specialization == Mage.Specialization.Arcane))
                            {
                                skip = true;
                            }
                            if (!skip)
                            {
                                placed.Add(c);
                                //for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                                column = lp.AddColumnUnsafe();
                                if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { State = state, Cycle = c, Segment = 0, ManaSegment = manaSegment, Type = VariableType.Spell, Dps = c.DamagePerSecond, Mps = c.ManaPerSecond });
                                SetSpellColumn(minimizeTime, 0, manaSegment, state, column, c, 1);
                            }
                        }
                    }
                }
            }
            else
            {
                float mfMin = CalculationOptions.FightDuration * (1.0f - CalculationOptions.MoltenFuryPercentage) + 0.00001f;
                float heroMin = Math.Min(mfMin, CalculationOptions.FightDuration - 40.0f + 0.00001f);
                int firstHeroismSegment = SegmentList.FindIndex(s => s.TimeEnd > heroMin);

                List<Cycle> placed = new List<Cycle>();
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    if (requiresMIP)
                    {
                        segmentColumn[seg] = lp.Columns;
                    }
                    for (int buffset = 0; buffset < stateList.Count; buffset++)
                    {
                        CastingState state = stateList[buffset];
                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[seg], state))
                        {
                            float mult = segmentCooldowns ? CalculationOptions.GetDamageMultiplier(SegmentList[seg]) : 1.0f;
                            if (mult > 0)
                            {
                                placed.Clear();
                                for (int spell = 0; spell < spellList.Count; spell++)
                                {
                                    if (!segmentNonCooldowns && state == BaseState && seg != 0) continue;
                                    if (segmentCooldowns && CalculationOptions.HeroismControl == 3 && state.Heroism && seg < firstHeroismSegment) continue;
                                    Cycle c = state.GetCycle(spellList[spell]);
                                    if (c.CycleId == CycleId.ArcaneManaNeutral)
                                    {
                                        c.FixManaNeutral();
                                    }
                                    bool skip = false;
                                    foreach (Cycle s2 in placed)
                                    {
                                        // TODO verify it this is ok, it assumes that spells placed under same casting state are independent except for aoe spells
                                        // assuming there are no constraints that depend on properties of particular spell cycle instead of properties of casting state
                                        if (!c.AreaEffect && s2.DamagePerSecond >= c.DamagePerSecond - 0.00001 && s2.ManaPerSecond <= c.ManaPerSecond + 0.00001)
                                        {
                                            skip = true;
                                            break;
                                        }
                                    }
                                    if ((c.ManaPerSecond < -0.001 && c.CycleId != CycleId.ArcaneManaNeutral) && (CalculationOptions.DisableManaRegenCycles && Specialization == Mage.Specialization.Arcane))
                                    {
                                        skip = true;
                                    }
                                    if (!skip)
                                    {
                                        placed.Add(c);
                                        //for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                                        for (int manaSegment = manaSegments - 1; manaSegment >= 0; manaSegment--)
                                        {
                                            column = lp.AddColumnUnsafe();
                                            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { State = state, Cycle = c, Segment = seg, ManaSegment = manaSegment, Type = VariableType.Spell, Dps = c.DamagePerSecond, Mps = c.ManaPerSecond });
                                            SetSpellColumn(minimizeTime, seg, manaSegment, state, column, c, mult);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (requiresMIP)
                {
                    segmentColumn[SegmentList.Count] = column + 1;
                }
            }
        }

        private void ConstructConjureManaGem()
        {
            if (conjureManaGem)
            {
                int conjureSegments = (restrictManaUse) ? SegmentList.Count : 1;
                Cycle spell = ConjureManaGemTemplate.GetSpell(BaseState);
                ConjureManaGem = spell;
                MaxConjureManaGem = (int)((CalculationOptions.FightDuration - 300.0f) / 360.0f) + 1;
                double mps = spell.ManaPerSecond;
                double dps = 0.0;
                //double tps = spell.ThreatPerSecond;
                for (int segment = 0; segment < conjureSegments; segment++)
                {
                    for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                    {
                        int column = lp.AddColumnUnsafe();
                        lp.SetColumnUpperBound(column, spell.CastTime * ((conjureSegments > 1) ? 1 : MaxConjureManaGem));
                        if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ConjureManaGem, Cycle = spell, Segment = segment, ManaSegment = manaSegment, State = BaseState, Dps = dps, Mps = mps });
                        lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
                        lp.SetElementUnsafe(rowManaRegen, column, mps);
                        lp.SetElementUnsafe(rowConjureManaGem, column, 1.0);
                        lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                        lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                        //lp.SetElementUnsafe(rowThreat, column, tps = spell.ThreatPerSecond);
                        lp.SetElementUnsafe(rowTargetDamage, column, -spell.DamagePerSecond);
                        lp.SetCostUnsafe(column, minimizeTime ? -1 : spell.DamagePerSecond);
                        lp.SetElementUnsafe(rowManaGem, column, -3.0 / spell.CastTime); // one cast time gives 3 new gem uses
                        if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
                        if (restrictManaUse)
                        {
                            SetManaConstraint(mps, segment, manaSegment, column, false);
                        }
                        /*if (restrictThreat)
                        {
                            for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                            {
                                lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                            }
                        }*/
                        if (needsManaSegmentConstraints)
                        {
                            SetManaSegmentConstraint(manaSegment, column);
                        }
                    }
                }
            }
            else
            {
                ConjureManaGem = null;
                MaxConjureManaGem = 0;
            }
        }

        private void SetCombinatorialConstraint(int manaSegment, int column)
        {
            for (int i = 0; i < rowCombinatorialConstraintCount; i++)
            {
                if (manaSegment >= rowCombinatorialConstraint[i].MinSegment && manaSegment <= rowCombinatorialConstraint[i].MaxSegment)
                {
                    lp.SetElementUnsafe(rowCombinatorialConstraint[i].Row, column, 1.0);
                }
            }
        }


        private void SetManaSegmentConstraint(int manaSegment, int column)
        {
            for (int ms = 0; ms < rowManaSegment.Count; ms++)
            {
                if (rowManaSegment[ms].ManaSegment == manaSegment)
                {
                    lp.SetElementUnsafe(rowManaSegment[ms].Row, column, 1.0);
                }
            }
        }

        private void SetManaConstraint(double mps, int segment, int manaSegment, int column, bool overflow)
        {
            if (CombinatorialSolver)
            {
                for (int i = 0; i < combStateList.Count; i++)
                {
                    if (manaSegment <= i)
                    {
                        lp.SetElementUnsafe(combStateList[i].UnderflowConstraint, column, mps);
                    }
                    if (manaSegment < i || (manaSegment == i && (mps < 0 || overflow)))
                    {
                        lp.SetElementUnsafe(combStateList[i].OverflowConstraint, column, -mps);
                    }
                }
            }
            else
            {
                for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, mps);
                }
                if (mps < 0)
                {
                    lp.SetElementUnsafe(rowSegmentManaOverflow + segment * manaSegments + manaSegment, column, -mps);
                }
                for (int ss = segment * manaSegments + manaSegment + 1; ss < SegmentList.Count * manaSegments; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -mps);
                }
            }
        }

        private void ConstructManaOverflow()
        {
            if (restrictManaUse)
            {
                // TODO reevaluate how much we need this, if we don't have this then mana regen effects can get negative value
                /*if (useIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.ManaOverflow)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            SetManaOverflowColumn(segment, manaSegment);
                        }
                    }
                }
                else*/
                {
                    if (!SimpleStacking)
                    {
                        for (int segment = 0; segment < SegmentList.Count; segment++)
                        {
                            for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                            {
                                SetManaOverflowColumn(segment, manaSegment);
                            }
                        }
                    }
                    else if (CombinatorialSolver && (MageTalents.Invocation > 0))
                    {
                        // need to handle evocation overflowing since we're forcing it to max duration
                        for (int i = 0; i < combList.Count; i++)
                        {
                            if (!combList[i].Activation && CooldownList[combList[i].Cooldown].StandardEffect == StandardEffect.Evocation)
                            {
                                SetManaOverflowColumn(0, combList[i].StateIndex - 1);
                            }
                        }
                    }
                }
            }
        }

        private void SetManaOverflowColumn(int segment, int manaSegment)
        {
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ManaOverflow, Segment = segment, ManaSegment = manaSegment });
            int column = lp.AddColumnUnsafe();
            lp.SetColumnScaleUnsafe(column, 100);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, 1.0);
            lp.SetElementUnsafe(rowManaRegen, column, 1.0);
            /*for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments - 1; ss++)
            {
                lp.SetElementUnsafe(rowSegmentManaUnderflow + ss, column, 1.0);
            }
            for (int ss = segment * manaSegments + manaSegment; ss < SegmentList.Count * manaSegments; ss++)
            {
                lp.SetElementUnsafe(rowSegmentManaOverflow + ss, column, -1.0);
            }*/
            SetManaConstraint(1.0, segment, manaSegment, column, true);
        }

        private void ConstructAfterFightRegen(bool afterFightRegen)
        {
            if (afterFightRegen)
            {
                if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.AfterFightRegen });
                int column = lp.AddColumnUnsafe();
                lp.SetColumnUpperBound(column, CalculationOptions.FightDuration);
                lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                lp.SetElementUnsafe(rowAfterFightRegenMana, column, -BaseState.ManaRegenDrinking);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : 0);
            }
        }

        private void ConstructTimeExtension()
        {
            if (needsTimeExtension)
            {
                if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.TimeExtension });
                int column = lp.AddColumnUnsafe();
                lp.SetColumnUpperBound(column, CalculationOptions.FightDuration);
                if (!needsQuadratic)
                {
                    lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                }
                lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                lp.SetElementUnsafe(rowEvocation, column, EvocationDuration / EvocationCooldown);
                //lp.SetElementUnsafe(rowPotion, column, 1.0 / 120.0);
                lp.SetElementUnsafe(rowManaGem, column, 1.0 / 120.0);
                lp.SetElementUnsafe(rowPowerInfusion, column, PowerInfusionDuration / PowerInfusionCooldown);
                lp.SetElementUnsafe(rowArcanePower, column, ArcanePowerDuration / ArcanePowerCooldown);
                lp.SetElementUnsafe(rowIcyVeins, column, 20.0 / IcyVeinsCooldown);
                for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
                {
                    EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                    lp.SetElementUnsafe(cooldown.Row, column, cooldown.Duration / cooldown.Cooldown);
                }
                lp.SetElementUnsafe(rowManaGemEffect, column, ManaGemEffectDuration / 120f);
                lp.SetElementUnsafe(rowDpsTime, column, -(1 - dpsTime));
                lp.SetElementUnsafe(rowAoe, column, CalculationOptions.AoeDuration);
                lp.SetElementUnsafe(rowCombustion, column, CombustionDuration / CombustionCooldown);
                lp.SetElementUnsafe(rowBerserking, column, 10.0 / 180.0);
                lp.SetElementUnsafe(rowBloodFury, column, 15.0 / 120.0);
            }
        }

        private void ConstructDrinking()
        {
            if (DrinkingEnabled)
            {
                double mps = -BaseState.ManaRegenDrinking;
                double dps = 0.0f;
                //double tps = 0.0f;
                if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.Drinking, Dps = dps, Mps = mps });
                int column = lp.AddColumnUnsafe();
                lp.SetColumnUpperBound(column, MaxDrinkingTime);
                lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
                lp.SetElementUnsafe(rowManaRegen, column, mps);
                lp.SetElementUnsafe(rowFightDuration, column, 1.0);
                lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : 0);
                if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + 0, column, 1.0);
                if (restrictManaUse)
                {
                    SetManaConstraint(mps, 0, 0, column, false);
                }
            }
        }

        private void ConstructSummonMirrorImage()
        {
            if (mirrorImageAvailable)
            {
                int mirrorImageSegments = SegmentList.Count; // always segment, we need it to guarantee each block has activation
                double mps = (int)(0.10 * CalculationOptions.BaseMana) / BaseGlobalCooldown - BaseState.ManaRegen5SR;
                scratchStateList.Clear();
                bool found = false;
                for (int i = 0; i < stateList.Count; i++)
                {
                    if (stateList[i].Effects == (int)StandardEffect.MirrorImage)
                    {
                        scratchStateList.Add(stateList[i]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    scratchStateList.Add(CastingState.New(this, (int)StandardEffect.MirrorImage, false, 0));
                }
                if (UseIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.SummonMirrorImage)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonMirrorImageColumn(mirrorImageSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < mirrorImageSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonMirrorImageColumn(mirrorImageSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
            }
        }

        private void SetSummonMirrorImageColumn(int mirrorImageSegments, double mps, int segment, int manaSegment, CastingState state)
        {
            Spell mirrorImage = state.GetSpell(SpellId.MirrorImage);
            double dps = mirrorImage.DamagePerSecond;
            //double tps = 0.0;
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.SummonMirrorImage, Segment = segment, ManaSegment = manaSegment, State = state, Dps = dps, Mps = mps });
            int column = lp.AddColumnUnsafe();
            if (mirrorImageSegments > 1) lp.SetColumnUpperBound(column, BaseGlobalCooldown);
            //if (segment == 0 && state == states[0]) calculationResult.ColumnSummonWaterElemental = column;
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            lp.SetElementUnsafe(rowSummonMirrorImage, column, -1 / BaseGlobalCooldown);
            lp.SetElementUnsafe(rowSummonMirrorImageCount, column, 1.0);
            lp.SetElementUnsafe(rowMirrorImage, column, 1.0);
            lp.SetCostUnsafe(column, minimizeTime ? -1 : mirrorImage.DamagePerSecond);
            lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                SetManaConstraint(mps, segment, manaSegment, column, false);
            }
            if (segmentCooldowns)
            {
                foreach (SegmentConstraint constraint in rowSegmentMirrorImage)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
                foreach (SegmentConstraint constraint in rowSegmentSummonMirrorImage)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (needsManaSegmentConstraints)
            {
                SetManaSegmentConstraint(manaSegment, column);
            }
            if (CombinatorialSolver)
            {
                SetCombinatorialConstraint(manaSegment, column);
            }
        }

        /*private void ConstructSummonWaterElemental()
        {
            if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
            {
                int waterElementalSegments = SegmentList.Count; // always segment, we need it to guarantee each block has activation
                double mps = (int)(0.16 * SpellTemplate.BaseMana[CalculationOptions.PlayerLevel]) / BaseGlobalCooldown - BaseState.ManaRegen5SR;
                scratchStateList.Clear();
                bool found = false;
                // WE = 0x100
                for (int i = 0; i < stateList.Count; i++)
                {
                    if (stateList[i].Effects == (int)StandardEffect.WaterElemental)
                    {
                        scratchStateList.Add(stateList[i]);
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    scratchStateList.Add(CastingState.New(this, (int)StandardEffect.WaterElemental, false, 0));
                }
                if (useIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.SummonWaterElemental)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonWaterElementalColumn(waterElementalSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < waterElementalSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            foreach (CastingState state in scratchStateList)
                            {
                                SetSummonWaterElementalColumn(waterElementalSegments, mps, segment, manaSegment, state);
                            }
                        }
                    }
                }
            }
        }*/

        private void SetSummonWaterElementalColumn(int waterElementalSegments, double mps, int segment, int manaSegment, CastingState state)
        {
            Spell waterbolt = state.GetSpell(SpellId.Waterbolt);
            double dps = waterbolt.DamagePerSecond;
            //double tps = 0.0;
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.SummonWaterElemental, Segment = segment, ManaSegment = manaSegment, State = state, Dps = dps, Mps = mps });
            int column = lp.AddColumnUnsafe();
            if (waterElementalSegments > 1) lp.SetColumnUpperBound(column, BaseGlobalCooldown);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            /*if (!MageTalents.GlyphOfEternalWater)
            {
                lp.SetElementUnsafe(rowSummonWaterElemental, column, -1 / BaseGlobalCooldown);
                lp.SetElementUnsafe(rowSummonWaterElementalCount, column, 1.0);
                lp.SetElementUnsafe(rowWaterElemental, column, 1.0);
            }*/
            lp.SetCostUnsafe(column, minimizeTime ? -1 : waterbolt.DamagePerSecond);
            lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                SetManaConstraint(mps, segment, manaSegment, column, false);
            }
            /*if (segmentCooldowns)
            {
                foreach (SegmentConstraint constraint in rowSegmentWaterElemental)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
                foreach (SegmentConstraint constraint in rowSegmentSummonWaterElemental)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }*/
            if (needsManaSegmentConstraints)
            {
                SetManaSegmentConstraint(manaSegment, column);
            }
            if (CombinatorialSolver)
            {
                SetCombinatorialConstraint(manaSegment, column);
            }
        }

        private void ConstructManaGem(Stats baseStats, float threatFactor)
        {
            if (CalculationOptions.ManaGemEnabled)
            {
                int manaGemSegments = (segmentCooldowns && restrictManaUse) ? SegmentList.Count : 1;
                if (segmentCooldowns && advancedConstraintsLevel >= 3)
                {
                    MaxManaGem = 1 + (int)((CalculationOptions.FightDuration - 1f) / 120f);
                }
                else
                {
                    MaxManaGem = 1 + (int)((CalculationOptions.FightDuration - 30f) / 120f);
                }
                double mps = -(1 + baseStats.BonusManaGem) * ManaGemValue;
                double tps = -mps * 0.5f * threatFactor;
                double dps = 0.0f;
                double upperBound;
                if (manaGemSegments > 1)
                {
                    upperBound = 1.0;
                }
                else if (CombinatorialSolver)
                {
                    upperBound = 1.0;
                }
                else
                {
                    if (needsTimeExtension || conjureManaGem)
                    {
                        upperBound = MaxManaGem;
                    }
                    else
                    {
                        upperBound = Math.Min(3, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / 120.0 : MaxManaGem);
                    }
                }
                if (UseIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.ManaGem)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            SetManaGemColumn(mps * BaseCastingSpeed, tps * BaseCastingSpeed, dps, upperBound, segment, manaSegment);
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < manaGemSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            if (!CombinatorialSolver || combStateList[manaSegment].ManaGemActivation)
                            {
                                float castingSpeed;
                                if (CombinatorialSolver)
                                {
                                    castingSpeed = combStateList[manaSegment].CastingState.CastingSpeed;
                                }
                                else
                                {
                                    castingSpeed = BaseCastingSpeed;
                                }
                                SetManaGemColumn(mps * castingSpeed, tps * castingSpeed, dps, upperBound, segment, manaSegment);
                            }
                        }
                    }
                }
            }
            else
            {
                MaxManaGem = 0;
                ManaGemTps = 0;
            }
        }

        private void SetManaGemColumn(double mps, double tps, double dps, double upperBound, int segment, int manaSegment)
        {
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ManaGem, Segment = segment, ManaSegment = manaSegment, Dps = dps, Mps = mps });
            int column = lp.AddColumnUnsafe();
            //lp.SetColumnScaleUnsafe(column, 1.0 / 40.0);
            lp.SetColumnUpperBound(column, upperBound);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowManaGem, column, 1.0);
            lp.SetElementUnsafe(rowManaGemMax, column, 1.0);
            //lp.SetElementUnsafe(rowFlameCap, column, 1.0);
            lp.SetElementUnsafe(rowManaGemEffectActivation, column, -1.0);
            //lp.SetElementUnsafe(rowThreat, column, tps);
            ManaGemTps = (float)tps;
            //lp.SetElementUnsafe(rowManaPotionManaGem, column, 40.0);
            lp.SetCostUnsafe(column, 0.0);
            if (segmentCooldowns)
            {
                foreach (SegmentConstraint constraint in rowSegmentManaGem)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (restrictManaUse)
            {
                SetManaConstraint(mps, segment, manaSegment, column, false);
            }
            /*if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }*/
        }

        private void ConstructManaPotion(Stats baseStats, float threatFactor)
        {
            if (manaPotionAvailable)
            {
                int manaPotionSegments = (segmentCooldowns && (volcanicPotionAvailable || restrictManaUse)) ? SegmentList.Count : 1;
                double mps = -(1 + baseStats.BonusManaPotionEffectMultiplier) * ManaPotionValue;
                double dps = 0;
                double tps = (1 + baseStats.BonusManaPotionEffectMultiplier) * ManaPotionValue * 0.5f * threatFactor;
                if (UseIncrementalOptimizations)
                {
                    for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                    {
                        if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.ManaPotion)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            SetManaPotionColumn(mps, dps, tps, segment, manaSegment);
                        }
                    }
                }
                else
                {
                    for (int segment = 0; segment < manaPotionSegments; segment++)
                    {
                        for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                        {
                            SetManaPotionColumn(mps, dps, tps, segment, manaSegment);
                        }
                    }
                }
            }
            else
            {
                ManaPotionTps = 0;
            }
        }

        private void SetManaPotionColumn(double mps, double dps, double tps, int segment, int manaSegment)
        {
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.ManaPotion, Segment = segment, ManaSegment = manaSegment, Dps = dps, Mps = mps });
            int column = lp.AddColumnUnsafe();
            //lp.SetColumnScaleUnsafe(column, 1.0 / 40.0);
            lp.SetColumnUpperBound(column, 1.0);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowPotion, column, 1.0);
            lp.SetElementUnsafe(rowManaPotion, column, 1.0);
            //lp.SetElementUnsafe(rowThreat, column, tps);
            ManaPotionTps = (float)tps;
            //lp.SetElementUnsafe(rowManaPotionManaGem, column, 40.0);
            lp.SetCostUnsafe(column, 0.0);
            /*if (segmentCooldowns && effectPotionAvailable)
            {
                for (int ss = 0; ss < segments; ss++)
                {
                    double cool = 120;
                    int maxs = (int)Math.Floor(ss + cool / segmentDuration) - 1;
                    if (ss * segmentDuration + cool >= calculationOptions.FightDuration) maxs = segments - 1;
                    if (segment >= ss && segment <= maxs) lp.SetElementUnsafe(rowSegmentPotion + ss, column, 15.0);
                    if (ss * segmentDuration + cool >= calculationOptions.FightDuration) break;
                }
            }*/
            if (restrictManaUse)
            {
                SetManaConstraint(mps, segment, manaSegment, column, false);
            }
            /*if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }*/
        }

        private void ConstructEvocation(Stats baseStats, float threatFactor)
        {
            if (evocationAvailable)
            {
                CastingState evoBaseState = BaseState;
                int evocationSegments = (restrictManaUse) ? SegmentList.Count : 1;
                float evocationDuration = 6f / evoBaseState.CastingSpeed;
                if (MageTalents.Invocation > 0)
                {
                    evocationDuration /= 2;
                }
                EvocationDuration = evocationDuration;
                EvocationDurationIV = evocationDuration / 1.2f;
                EvocationDurationHero = evocationDuration / 1.3f;
                EvocationDurationIVHero = evocationDuration / 1.2f / 1.3f;
                /*if (CalculationOptions.Beta)
                {
                    EvocationDuration = (float)Math.Floor(8 / (EvocationDuration / 4)) * EvocationDuration / 4;
                    EvocationDurationIV = (float)Math.Floor(8 / (EvocationDurationIV / 4)) * EvocationDurationIV / 4;
                    EvocationDurationHero = (float)Math.Floor(8 / (EvocationDurationHero / 4)) * EvocationDurationHero / 4;
                    EvocationDurationIVHero = (float)Math.Floor(8 / (EvocationDurationIVHero / 4)) * EvocationDurationIVHero / 4;
                }*/
                float evocationMana = baseStats.Mana;
                float evocationPercent = 0.6f;
                if (Specialization == Specialization.Arcane)
                {
                    evocationPercent = 0.8f; // assume 4 charge evo always
                    if (Mage4T15)
                    {
                        evocationPercent = 0.88f; // 0.4 + 0.4 * 1.2
                    }
                }
                EvocationRegen = BaseState.ManaRegen5SR + evocationPercent * evocationMana / evocationDuration;
                EvocationRegenIV = BaseState.ManaRegen5SR + evocationPercent * evocationMana / evocationDuration * 1.2f;
                EvocationRegenHero = BaseState.ManaRegen5SR + evocationPercent * evocationMana / evocationDuration * 1.3f;
                EvocationRegenIVHero = BaseState.ManaRegen5SR + evocationPercent * evocationMana / evocationDuration * 1.2f * 1.3f;
                if (EvocationRegen * evocationDuration > baseStats.Mana)
                {
                    evocationDuration = baseStats.Mana / EvocationRegen;
                    EvocationDuration = evocationDuration;
                    EvocationDurationIV = baseStats.Mana / EvocationRegenIV;
                    EvocationDurationHero = baseStats.Mana / EvocationRegenHero;
                    EvocationDurationIVHero = baseStats.Mana / EvocationRegenIVHero;
                }
                if (CalculationOptions.AverageCooldowns)
                {
                    MaxEvocation = CalculationOptions.FightDuration / EvocationCooldown;
                }
                else if (segmentCooldowns && advancedConstraintsLevel >= 3)
                {
                    MaxEvocation = Math.Max(1, 1 + (float)Math.Floor((CalculationOptions.FightDuration - evocationDuration) / EvocationCooldown));
                }
                else
                {
                    MaxEvocation = Math.Max(1, 1 + (float)Math.Floor((CalculationOptions.FightDuration - 90f) / EvocationCooldown));
                }
                if (CombinatorialSolver)
                {
                    for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                    {
                        CastingState state = combStateList[manaSegment].CastingState;
                        if (state.Evocation)
                        {
                            SetEvocationColumn(threatFactor, evocationSegments, baseStats.Mana + state.StateEffectMaxMana, state, 0, manaSegment, VariableType.Evocation);
                        }
                    }
                }
                else
                {
                    int mask = 0;
                    CastingState evoState = null;
                    CastingState evoStateIV = null;
                    CastingState evoStateHero = null;
                    CastingState evoStateIVHero = null;
                    if (waterElementalAvailable)
                    {
                        evoState = CastingState.New(this, (int)StandardEffect.Evocation | mask, false, 0);
                        if (CalculationOptions.EnableHastedEvocation)
                        {
                            evoStateIV = CastingState.New(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | mask, false, 0);
                            evoStateHero = CastingState.New(this, (int)StandardEffect.Evocation | (int)StandardEffect.Heroism | mask, false, 0);
                            evoStateIVHero = CastingState.New(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism | mask, false, 0);
                        }
                    }
                    else
                    {
                        evoState = CastingState.NewRaw(this, (int)StandardEffect.Evocation | mask);
                        if (CalculationOptions.EnableHastedEvocation)
                        {
                            evoStateIV = CastingState.NewRaw(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | mask);
                            evoStateHero = CastingState.NewRaw(this, (int)StandardEffect.Evocation | (int)StandardEffect.Heroism | mask);
                            evoStateIVHero = CastingState.NewRaw(this, (int)StandardEffect.Evocation | (int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism | mask);
                        }
                    }
                    if (UseIncrementalOptimizations && segmentMana && Specialization == Specialization.Arcane)
                    {
                        for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                        {
                            int segment = CalculationOptions.IncrementalSetSegments[index];
                            int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                            int state = CalculationOptions.IncrementalSetStateIndexes[index];
                            switch (CalculationOptions.IncrementalSetVariableType[index])
                            {
                                case VariableType.Evocation:
                                    if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                    {
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.Evocation, false);
                                    }
                                    break;
                                case VariableType.EvocationHero:
                                    if (state == (evoStateHero.Effects & (int)StandardEffect.NonItemBasedMask) && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateHero))
                                    {
                                        // last tick of heroism
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateHero, segment, manaSegment, VariableType.EvocationHero, true);
                                    }
                                    if (state == (evoState.Effects & (int)StandardEffect.NonItemBasedMask) && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                    {
                                        // remainder
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationHero, false);
                                    }
                                    break;
                                case VariableType.EvocationIV:
                                    if (state == (evoStateIV.Effects & (int)StandardEffect.NonItemBasedMask) && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIV))
                                    {
                                        // last tick of icy veins
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIV, segment, manaSegment, VariableType.EvocationIV, true);
                                    }
                                    else if (state == (evoState.Effects & (int)StandardEffect.NonItemBasedMask) && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                    {
                                        // remainder
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIV, false);
                                    }
                                    break;
                                case VariableType.EvocationIVHero:
                                    if (state == (evoStateIVHero.Effects & (int)StandardEffect.NonItemBasedMask) && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIVHero))
                                    {
                                        // last tick of icy veins+heroism
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIVHero, segment, manaSegment, VariableType.EvocationIVHero, true);
                                    }
                                    if (state == (evoState.Effects & (int)StandardEffect.NonItemBasedMask) && CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                    {
                                        // remainder
                                        SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIVHero, false);
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        int minManaSegment = segmentMana ? 1 : 0;
                        for (int segment = 0; segment < evocationSegments; segment++)
                        {
                            for (int manaSegment = minManaSegment; manaSegment < manaSegments; manaSegment++)
                            {
                                // base evocation
                                if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                {
                                    SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.Evocation, false);
                                }
                                if (CalculationOptions.EnableHastedEvocation)
                                {
                                    if (icyVeinsAvailable)
                                    {
                                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIV))
                                        {
                                            // last tick of icy veins
                                            SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIV, segment, manaSegment, VariableType.EvocationIV, true);
                                        }
                                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                        {
                                            // remainder
                                            SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIV, false);
                                        }
                                    }
                                    if (heroismAvailable)
                                    {
                                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateHero))
                                        {
                                            // last tick of heroism
                                            SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateHero, segment, manaSegment, VariableType.EvocationHero, true);
                                        }
                                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                        {
                                            // remainder
                                            SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationHero, false);
                                        }
                                    }
                                    if (icyVeinsAvailable && heroismAvailable)
                                    {
                                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoStateIVHero))
                                        {
                                            // last tick of icy veins+heroism
                                            SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoStateIVHero, segment, manaSegment, VariableType.EvocationIVHero, true);
                                        }
                                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], evoState))
                                        {
                                            // remainder
                                            SetEvocationColumn(threatFactor, evocationSegments, evocationMana, evoState, segment, manaSegment, VariableType.EvocationIVHero, false);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                MaxEvocation = 0;
                EvocationDuration = 0;
                EvocationDurationIV = 0;
                EvocationDurationHero = 0;
                EvocationDurationIVHero = 0;
                EvocationRegen = 0;
                EvocationRegenIV = 0;
                EvocationRegenHero = 0;
                EvocationRegenIVHero = 0;
            }
        }

        private void SetEvocationColumn(float threatFactor, int evocationSegments, float evocationMana, CastingState evoState, int segment, int manaSegment, VariableType evocationType, bool lastTick)
        {
            double dps = 0.0f;
            if (waterElementalAvailable)
            {
                dps = evoState.GetSpell(SpellId.Waterbolt).DamagePerSecond;
            }
            double tps;            
            double mps;
            float evocationDuration;
            int column = lp.AddColumnUnsafe();
            double evoFactor;
            switch (evocationType)
            {
                case VariableType.Evocation:
                    mps = -EvocationRegen;
                    evocationDuration = EvocationDuration;
                    tps = 0.6f * evocationMana / evocationDuration * 0.5f * threatFactor;
                    evoFactor = 1.0;

                    lp.SetElementUnsafe(rowEvocation, column, 1.0);

                    break;
                case VariableType.EvocationHero:
                    mps = -EvocationRegenHero;
                    evocationDuration = EvocationDurationHero;
                    tps = 0.6f * evocationMana / evocationDuration * 0.5f * threatFactor;
                    evoFactor = 1.3;

                    if (lastTick)
                    {
                        lp.SetElementUnsafe(rowHeroism, column, 1.0);
                    }
                    lp.SetElementUnsafe(rowEvocation, column, 1.3);
                    lp.SetElementUnsafe(rowEvocationHero, column, 1.0);

                    break;
                case VariableType.EvocationIV:
                    mps = -EvocationRegenIV;
                    evocationDuration = EvocationDurationIV;
                    tps = 0.6f * evocationMana / evocationDuration * 0.5f * threatFactor;
                    evoFactor = 1.2;

                    if (lastTick)
                    {
                        lp.SetElementUnsafe(rowIcyVeins, column, 1.0);
                        if (segmentCooldowns)
                        {
                            foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                            {
                                if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                            }
                        }
                    }
                    lp.SetElementUnsafe(rowEvocation, column, 1.2);
                    lp.SetElementUnsafe(rowEvocationIV, column, 1.0);

                    break;
                case VariableType.EvocationIVHero:
                default:
                    mps = -EvocationRegenIVHero;
                    evocationDuration = EvocationDurationIVHero;
                    tps = 0.6f * evocationMana / evocationDuration * 0.5f * threatFactor;
                    evoFactor = 1.2 * 1.3;

                    if (lastTick)
                    {
                        lp.SetElementUnsafe(rowHeroism, column, 1.0);
                        lp.SetElementUnsafe(rowIcyVeins, column, 1.0);
                        lp.SetElementUnsafe(rowHeroismIcyVeins, column, 1.0);
                        if (segmentCooldowns)
                        {
                            foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                            {
                                if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                            }
                        }
                    }
                    lp.SetElementUnsafe(rowEvocation, column, 1.2 * 1.3);
                    lp.SetElementUnsafe(rowEvocationHero, column, 1.2);
                    lp.SetElementUnsafe(rowEvocationIVHero, column, 1.0);

                    break;
            }
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = evocationType, Segment = segment, ManaSegment = manaSegment, State = evoState, Dps = dps, Mps = mps });
            lp.SetColumnUpperBound(column, (evocationSegments > 1 || manaSegments > 1) ? evocationDuration : evocationDuration * MaxEvocation);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            //lp.SetElementUnsafe(rowThreat, column, tps); // should split among all targets if more than one, assume one only
            lp.SetCostUnsafe(column, minimizeTime ? -1 : dps);
            if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                SetManaConstraint(mps, segment, manaSegment, column, false);
                if (segmentCooldowns)
                {
                    foreach (SegmentConstraint constraint in rowSegmentEvocation)
                    {
                        if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, evoFactor);
                    }
                }
            }
            /*if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }*/
            if (needsManaSegmentConstraints)
            {
                SetManaSegmentConstraint(manaSegment, column);
            }
            if (CombinatorialSolver)
            {
                SetCombinatorialConstraint(manaSegment, column);
            }
        }

        private void SetEvocationColumn(float threatFactor, int evocationSegments, float evocationMana, CastingState evoState, int segment, int manaSegment, VariableType evocationType)
        {
            double dps = 0.0f;
            if (waterElementalAvailable)
            {
                dps = evoState.GetSpell(SpellId.Waterbolt).DamagePerSecond;
            }
            //double tps;
            double mps;
            float evocationDuration;
            int column = lp.AddColumnUnsafe();
            double evoFactor;

            var duration = 6f / evoState.CastingSpeed;
            if (MageTalents.Invocation > 0)
            {
                duration /= 2;
            }
            var regen = evoState.ManaRegen5SR + 0.6f * evocationMana / duration * evoState.CastingSpeed;

            mps = -regen;
            evocationDuration = duration;
            //tps = 0.6f * evocationMana / evocationDuration * 0.5f * threatFactor;
            evoFactor = 1.0;

            lp.SetElementUnsafe(rowEvocation, column, 1.0);

            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = evocationType, Segment = segment, ManaSegment = manaSegment, State = evoState, Dps = dps, Mps = mps });
            lp.SetColumnUpperBound(column, (evocationSegments > 1 || manaSegments > 1) ? evocationDuration : evocationDuration * MaxEvocation);
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            //lp.SetElementUnsafe(rowThreat, column, tps); // should split among all targets if more than one, assume one only
            lp.SetCostUnsafe(column, minimizeTime ? -1 : dps);
            if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                SetManaConstraint(mps, segment, manaSegment, column, false);
                if (segmentCooldowns)
                {
                    foreach (SegmentConstraint constraint in rowSegmentEvocation)
                    {
                        if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, evoFactor);
                    }
                }
            }
            /*if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, tps);
                }
            }*/
            if (needsManaSegmentConstraints)
            {
                SetManaSegmentConstraint(manaSegment, column);
            }
            if (CombinatorialSolver)
            {
                SetCombinatorialConstraint(manaSegment, column);
            }
        }

        private void ConstructIdleRegen()
        {
            int idleRegenSegments = (restrictManaUse) ? SegmentList.Count : 1;
            double dps = 0.0f;
            double tps = 0.0f;
            double mps = -(BaseState.ManaRegen * (1 - CalculationOptions.Fragmentation) + BaseState.ManaRegen5SR * CalculationOptions.Fragmentation);
            if (UseIncrementalOptimizations && Specialization == Specialization.Arcane) // fire and frost can get in trouble satisfying constraints if they don't have any regen options
            {
                for (int index = 0; index < CalculationOptions.IncrementalSetStateIndexes.Length; index++)
                {
                    if (CalculationOptions.IncrementalSetVariableType[index] == VariableType.IdleRegen)
                    {
                        int segment = CalculationOptions.IncrementalSetSegments[index];
                        int manaSegment = CalculationOptions.IncrementalSetManaSegment[index];
                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], BaseState))
                        {
                            SetIdleRegenColumn(idleRegenSegments, dps, tps, mps, segment, manaSegment);
                        }
                    }
                }
            }
            else
            {
                if (!CalculationOptions.DisableManaRegenCycles || CalculationOptions.DpsTime < 1)
                {
                    for (int segment = 0; segment < idleRegenSegments; segment++)
                    {
                        if (CalculationOptions.CooldownRestrictionsValid(SegmentList[segment], BaseState))
                        {
                            for (int manaSegment = 0; manaSegment < manaSegments; manaSegment++)
                            {
                                if (!CombinatorialSolver || combStateList[manaSegment].CastingState.Effects == 0)
                                {
                                    SetIdleRegenColumn(idleRegenSegments, dps, tps, mps, segment, manaSegment);
                                }
                            }
                        }
                    }
                }
            }
        }

        private void SetIdleRegenColumn(int idleRegenSegments, double dps, double tps, double mps, int segment, int manaSegment)
        {
            int column = lp.AddColumnUnsafe();
            lp.SetColumnUpperBound(column, (idleRegenSegments > 1) ? SegmentList[segment].Duration : CalculationOptions.FightDuration);
            if (idleRegenSegments == 1 && manaSegments == 1 && !needsTimeExtension)
            {
                lp.SetColumnLowerBound(column, (1 - dpsTime) * CalculationOptions.FightDuration);
            }
            if (needsSolutionVariables) SolutionVariable.Add(new SolutionVariable() { Type = VariableType.IdleRegen, Segment = segment, ManaSegment = manaSegment, State = BaseState, Dps = dps, Mps = mps });
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, mps);
            lp.SetElementUnsafe(rowManaRegen, column, mps);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            lp.SetElementUnsafe(rowDpsTime, column, -1.0);
            lp.SetCostUnsafe(column, minimizeTime ? -1 : 0);
            if (segmentNonCooldowns) lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            if (restrictManaUse)
            {
                SetManaConstraint(mps, segment, manaSegment, column, false);
            }
            if (needsManaSegmentConstraints)
            {
                SetManaSegmentConstraint(manaSegment, column);
            }
            if (CombinatorialSolver)
            {
                SetCombinatorialConstraint(manaSegment, column);
            }
        }

        private void SetCalculationReuseReferences()
        {
            // determine which effects only cause a change in haste, thus allowing calculation reuse (only recalculating cast time)
            int recalcCastTime = (int)StandardEffect.IcyVeins | (int)StandardEffect.Heroism | (int)StandardEffect.Berserking | (int)StandardEffect.PowerInfusion;
            for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
            {
                EffectCooldown effect = ItemBasedEffectCooldowns[i];
                if (effect.HasteEffect)
                {
                    recalcCastTime |= effect.Mask;
                }
            }
            if (Mage4T10)
            {
                recalcCastTime |= (int)StandardEffect.MirrorImage; // for this it's actually identical, potential for further optimization
            }
            // states will be calculated in forward manner, see if some can reuse previous states
            for (int i = 0; i < stateList.Count; i++)
            {
                CastingState si = stateList[i];
                for (int j = 0; j < i; j++)
                {
                    CastingState sj = stateList[j];
                    // check the difference
                    int diff = si.Effects ^ sj.Effects;
                    if ((diff & ~recalcCastTime) == 0)
                    {
                        // the only difference is in haste effects
                        si.ReferenceCastingState = sj;
                        break;
                    }
                }
            }
        }

        private void ConstructSegments()
        {
            if (SegmentList == null)
            {
                SegmentList = new List<Segment>();
            }
            else
            {
                SegmentList.Clear();
            }
            if (segmentCooldowns)
            {
                List<double> ticks = new List<double>();
                if (CalculationOptions.VariableSegmentDuration)
                {
                    // variable segment durations to get a better grasp on varied cooldown durations
                    // create ticks in intervals of half cooldown duration
                    if (volcanicPotionAvailable || manaPotionAvailable)
                    {
                        AddSegmentTicks(ticks, 120.0);
                    }
                    if (evocationAvailable)
                    {
                        AddSegmentTicks(ticks, EvocationCooldown);
                    }
                    if (arcanePowerAvailable)
                    {
                        AddSegmentTicks(ticks, ArcanePowerCooldown);
                        //AddEffectTicks(ticks, ArcanePowerCooldown, ArcanePowerDuration);
                    }
                    if (combustionAvailable) AddSegmentTicks(ticks, 300.0);
                    if (berserkingAvailable) AddSegmentTicks(ticks, 180.0);
                    if (bloodFuryAvailable) AddSegmentTicks(ticks, 120.0);
                    if (CalculationOptions.ManaGemEnabled || manaGemEffectAvailable)
                    {
                        ticks.Add(15.0); // get a better grasp on mana overflow
                        AddSegmentTicks(ticks, 60.0);
                    }
                    if (icyVeinsAvailable)
                    {
                        AddSegmentTicks(ticks, IcyVeinsCooldown);
                        //if (!coldsnapAvailable) AddEffectTicks(ticks, calculationResult.IcyVeinsCooldown, 20.0);
                    }
                    //if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater) AddSegmentTicks(ticks, WaterElementalCooldown);
                    for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
                    {
                        EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                        AddSegmentTicks(ticks, cooldown.Cooldown);
                    }
                }
                else
                {
                    for (int i = 0; CalculationOptions.FixedSegmentDuration * i < CalculationOptions.FightDuration - 0.00001; i++)
                    {
                        //segmentList.Add(new Segment() { TimeStart = calculationOptions.FixedSegmentDuration * i, Duration = Math.Min(calculationOptions.FixedSegmentDuration, calculationOptions.FightDuration - calculationOptions.FixedSegmentDuration * i) });
                        ticks.Add(CalculationOptions.FixedSegmentDuration * i);
                    }
                }
                if (!string.IsNullOrEmpty(CalculationOptions.AdditionalSegmentSplits))
                {
                    string[] splits = CalculationOptions.AdditionalSegmentSplits.Split(',');
                    foreach (string split in splits)
                    {
                        double tick;
                        if (double.TryParse(split.Trim(), out tick))
                        {
                            ticks.Add(tick);
                        }
                    }
                }
                if (CalculationOptions.HeroismControl == 3)
                {
                    ticks.Add(Math.Max(0.0, Math.Min(CalculationOptions.FightDuration - CalculationOptions.MoltenFuryPercentage * CalculationOptions.FightDuration, CalculationOptions.FightDuration - 40.0)));
                }
                if (!string.IsNullOrEmpty(CalculationOptions.CooldownRestrictions) && CalculationOptions.CooldownRestrictionList == null)
                {
                    StateDescription.Scanner scanner = new StateDescription.Scanner();
                    StateDescription.Parser parser = new StateDescription.Parser(scanner);
                    CalculationOptions.CooldownRestrictionList = new List<CooldownRestriction>();
                    string[] lines = CalculationOptions.CooldownRestrictions.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string line in lines)
                    {
#if SILVERLIGHT
                        string[] tokens = line.Split(new char[] { '-', ':' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < tokens.Length; i++)
                        {
                            tokens[i] = tokens[i].Trim();
                        }
#else
                        string[] tokens = line.Split(new char[] { '-', ':', ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
#endif
                        if (tokens.Length == 3)
                        {
                            CooldownRestriction restriction = new CooldownRestriction();
                            double value;
                            if (!double.TryParse(tokens[0], out value)) continue;
                            restriction.TimeStart = value;
                            if (!double.TryParse(tokens[1], out value)) continue;
                            restriction.TimeEnd = value;
                            StateDescription.ParseTree parseTree = parser.Parse(tokens[2], this);
                            if (parseTree != null && parseTree.Errors.Count == 0)
                            {
                                try
                                {
                                    restriction.IsMatch = parseTree.Compile();
                                    CalculationOptions.CooldownRestrictionList.Add(restriction);
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
                if (CalculationOptions.CooldownRestrictionList != null)
                {
                    foreach (CooldownRestriction restriction in CalculationOptions.CooldownRestrictionList)
                    {
                        ticks.Add(restriction.TimeStart);
                        ticks.Add(restriction.TimeEnd);
                    }
                }
                if (CalculationOptions.BossHandler)
                {
                    // we have to go through phases of the encounter and place markers where phases change
                    foreach (var buffState in Character.BossOptions.BuffStates)
                    {
                        // for now only handle deterministic damage multipliers and average over the whole state
                        if (buffState.Chance > 0 && buffState.Stats.BonusDamageMultiplier > 0)
                        {
                            foreach (var phase in buffState.PhaseTimes)
                            {
                                ticks.Add(phase.Value[0]);
                                ticks.Add(phase.Value[1]);
                            }
                        }
                    }
                }
                ticks.Sort();
                for (int i = 0; i < ticks.Count; i++)
                {
                    if ((i == 0 || ticks[i] > ticks[i - 1] + 0.00001) && ticks[i] < CalculationOptions.FightDuration - 0.00001)
                    {
                        if (SegmentList.Count > 0)
                        {
                            SegmentList[SegmentList.Count - 1].Duration = ticks[i] - ticks[i - 1];
                        }
                        SegmentList.Add(new Segment() { TimeStart = ticks[i] });
                    }
                }
                SegmentList[SegmentList.Count - 1].Duration = CalculationOptions.FightDuration - SegmentList[SegmentList.Count - 1].TimeStart;
            }
            else
            {
                SegmentList.Add(new Segment() { TimeStart = 0, Duration = CalculationOptions.FightDuration });
            }
            for (int i = 0; i < SegmentList.Count; i++)
            {
                SegmentList[i].Index = i;
            }
        }

        private void SetProblemRHS()
        {
            double ivlength = 0.0;
            double effectiveDuration = CalculationOptions.FightDuration;
            if (waterElementalAvailable) effectiveDuration -= BaseGlobalCooldown; // EXPERIMENTAL
            ivlength = MaximizeEffectDuration(effectiveDuration, 20.0, IcyVeinsCooldown);

            //double mflength = CalculationOptions.MoltenFuryPercentage * CalculationOptions.FightDuration;

            lp.SetRHSUnsafe(rowManaRegen, StartingMana);
            lp.SetRHSUnsafe(rowFightDuration, CalculationOptions.FightDuration);
            lp.SetRHSUnsafe(rowTimeExtension, -CalculationOptions.FightDuration);
            lp.SetRHSUnsafe(rowEvocation, EvocationDuration * MaxEvocation);
            if (icyVeinsAvailable) lp.SetRHSUnsafe(rowEvocationIV, EvocationDurationIV * MaxEvocation);
            if (heroismAvailable) lp.SetRHSUnsafe(rowEvocationHero, EvocationDurationHero);
            if (icyVeinsAvailable && heroismAvailable) lp.SetRHSUnsafe(rowEvocationIVHero, EvocationDurationIVHero);
            lp.SetRHSUnsafe(rowPotion, 1.0);
            lp.SetRHSUnsafe(rowManaPotion, 1.0);
            lp.SetRHSUnsafe(rowManaGem, Math.Min(3, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / 120.0 : MaxManaGem));
            lp.SetRHSUnsafe(rowManaGemMax, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / 120.0 : MaxManaGem);
            if (conjureManaGem) lp.SetRHSUnsafe(rowConjureManaGem, MaxConjureManaGem * ConjureManaGem.CastTime);
            //if (wardsAvailable) lp.SetRHSUnsafe(rowWard, calculationResult.MaxWards * calculationResult.Ward.CastTime);

            if (!CombinatorialSolver)
            {
                foreach (EffectCooldown cooldown in CooldownList)
                {
                    if (cooldown.AutomaticConstraints)
                    {
                        lp.SetRHSUnsafe(cooldown.Row, (CalculationOptions.AverageCooldowns && !float.IsPositiveInfinity(cooldown.Cooldown)) ? CalculationOptions.FightDuration * cooldown.Duration / cooldown.Cooldown : cooldown.MaximumDuration);
                    }
                }
            }

            if (heroismAvailable)
            {
                double minDuration = Math.Min(0.99 * CalculationOptions.FightDuration * dpsTime, 40.0);
                lp.SetLHSUnsafe(rowHeroism, minDuration); // if heroism is marked as available then this implies that it has to be used, not only that it can be used
            }
            //if (powerInfusionAvailable) lp.SetRHSUnsafe(rowPowerInfusion, calculationOptions.AverageCooldowns ? calculationResult.PowerInfusionDuration / calculationResult.PowerInfusionCooldown * calculationOptions.FightDuration : pilength);
            //if (arcanePowerAvailable) lp.SetRHSUnsafe(rowArcanePower, calculationOptions.AverageCooldowns ? calculationResult.ArcanePowerDuration / calculationResult.ArcanePowerCooldown * calculationOptions.FightDuration : aplength);
            //if (heroismAvailable && arcanePowerAvailable) lp.SetRHSUnsafe(rowHeroismArcanePower, calculationResult.ArcanePowerDuration);
            //if (heroismAvailable && manaGemEffectAvailable) lp.SetRHSUnsafe(rowHeroismManaGemEffect, calculationResult.ManaGemEffectDuration);
            //if (moltenFuryAvailable) lp.SetRHSUnsafe(rowMoltenFuryDestructionPotion, 15);
            //if (moltenFuryAvailable && manaGemEffectAvailable) lp.SetRHSUnsafe(rowMoltenFuryManaGemEffect, manaGemEffectDuration);
            //if (heroismAvailable) lp.SetRHSUnsafe(rowHeroismDestructionPotion, 15);
            //if (icyVeinsAvailable) lp.SetRHSUnsafe(rowIcyVeinsDestructionPotion, dpivlength);
            //if (flameCapAvailable)
            //{
            //    lp.SetRHSUnsafe(rowFlameCap, calculationOptions.AverageCooldowns ? calculationOptions.FightDuration / 120.0 : ((int)(calculationOptions.FightDuration / 180.0 + 2.0 / 3.0)) * 3.0 / 2.0);
            //}
            //if (moltenFuryAvailable) lp.SetRHSUnsafe(rowMoltenFuryFlameCap, 60);
            //lp.SetRHSUnsafe(rowFlameCapDestructionPotion, dpflamelength);
            if (manaGemEffectAvailable) lp.SetRHSUnsafe(rowManaGemEffect, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration * ManaGemEffectDuration / 120f : MaximizeEffectDuration(CalculationOptions.FightDuration, ManaGemEffectDuration, 120.0));
            lp.SetRHSUnsafe(rowDpsTime, -(1 - dpsTime) * CalculationOptions.FightDuration);
            lp.SetRHSUnsafe(rowAoe, CalculationOptions.AoeDuration * CalculationOptions.FightDuration);
            //lp.SetRHSUnsafe(rowCombustion, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration / CombustionCooldown : combustionCount);
            //lp.SetRHSUnsafe(rowMoltenFuryCombustion, 1);
            //lp.SetRHSUnsafe(rowHeroismCombustion, 1);
            //lp.SetRHSUnsafe(rowMoltenFuryBerserking, 10);
            //lp.SetRHSUnsafe(rowHeroismBerserking, 10);
            //lp.SetRHSUnsafe(rowIcyVeinsDrumsOfBattle, drumsivlength);
            //lp.SetRHSUnsafe(rowArcanePowerDrumsOfBattle, drumsaplength);
            //lp.SetRHSUnsafe(rowThreat, CalculationOptions.TpsLimit * CalculationOptions.FightDuration);
            double manaConsum;
            /*if (integralMana)
            {
                manaConsum = Math.Ceiling((calculationOptions.FightDuration - 7800 / manaBurn) / 60f + 2);
            }
            else
            {
                manaConsum = ((calculationOptions.FightDuration - 7800 / manaBurn) / 60f + 2);
            }
            if (manaGemEffectAvailable && manaConsum < calculationResult.MaxManaGem)*/
            manaConsum = MaxManaGem;
            //lp.SetRHSUnsafe(rowManaPotionManaGem, manaConsum * 40.0);
            lp.SetRHSUnsafe(rowBerserking, CalculationOptions.AverageCooldowns ? CalculationOptions.FightDuration * 10.0 / 180.0 : 10.0 * (1 + (int)((CalculationOptions.FightDuration - 10) / 180)));
            /*if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
            {
                double duration = CalculationOptions.AverageCooldowns ? (WaterElementalDuration / WaterElementalCooldown + (coldsnapAvailable ? WaterElementalDuration / ColdsnapCooldown : 0.0)) * CalculationOptions.FightDuration : weDuration;
                lp.SetRHSUnsafe(rowWaterElemental, duration);
                lp.SetRHSUnsafe(rowSummonWaterElementalCount, BaseGlobalCooldown * Math.Ceiling(duration / WaterElementalDuration));
            }*/
            if (mirrorImageAvailable)
            {
                double duration = EffectCooldown[(int)StandardEffect.MirrorImage].MaximumDuration;
                lp.SetRHSUnsafe(rowSummonMirrorImageCount, BaseGlobalCooldown * Math.Ceiling(duration / MirrorImageDuration));
            }
            lp.SetRHSUnsafe(rowTargetDamage, -CalculationOptions.TargetDamage);

            for (int i = 0; i < rowStackingConstraintCount; i++)
            {
                lp.SetRHSUnsafe(rowStackingConstraint[i].Row, rowStackingConstraint[i].MaximumStackingDuration);
            }

            if (segmentCooldowns)
            {
                // heroism
                // ap
                if (arcanePowerAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentArcanePower)
                    {
                        lp.SetRHSUnsafe(constraint.Row, ArcanePowerDuration);
                    }
                }
                // pi
                if (powerInfusionAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentPowerInfusion)
                    {
                        lp.SetRHSUnsafe(constraint.Row, PowerInfusionDuration);
                    }
                }
                // iv
                if (icyVeinsAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 20);
                    }
                }
                // combustion
                if (combustionAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentCombustion)
                    {
                        lp.SetRHSUnsafe(constraint.Row, CombustionDuration);
                    }
                }
                // berserking
                if (berserkingAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentBerserking)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 10.0);
                    }
                }
                // blood furt
                if (bloodFuryAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentBloodFury)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 10.0);
                    }
                }
                // water elemental
                /*if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
                {
                    foreach (SegmentConstraint constraint in rowSegmentWaterElemental)
                    {
                        lp.SetRHSUnsafe(constraint.Row, WaterElementalDuration + (coldsnapAvailable ? WaterElementalDuration : 0.0));
                    }
                    foreach (SegmentConstraint constraint in rowSegmentSummonWaterElemental)
                    {
                        lp.SetRHSUnsafe(constraint.Row, BaseGlobalCooldown + (coldsnapAvailable ? BaseGlobalCooldown : 0.0));
                    }
                }*/
                // mirror image
                if (mirrorImageAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentMirrorImage)
                    {
                        lp.SetRHSUnsafe(constraint.Row, MirrorImageDuration);
                    }
                    foreach (SegmentConstraint constraint in rowSegmentSummonMirrorImage)
                    {
                        lp.SetRHSUnsafe(constraint.Row, BaseGlobalCooldown);
                    }
                }
                if (CalculationOptions.ManaGemEnabled)
                {
                    foreach (SegmentConstraint constraint in rowSegmentManaGem)
                    {
                        lp.SetRHSUnsafe(constraint.Row, 1.0);
                    }
                }
                // effect potion
                /*if (effectPotionAvailable)
                {
                    for (int seg = 0; seg < segments; seg++)
                    {
                        lp.SetRHSUnsafe(rowSegmentPotion + seg, 15.0);
                        double cool = 120;
                        if (seg * segmentDuration + cool >= calculationOptions.FightDuration) break;
                    }
                }*/
                for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
                {
                    EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                    foreach (SegmentConstraint constraint in cooldown.SegmentConstraints)
                    {
                        lp.SetRHSUnsafe(constraint.Row, cooldown.Duration);
                    }
                }
                if (manaGemEffectAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentManaGemEffect)
                    {
                        lp.SetRHSUnsafe(constraint.Row, ManaGemEffectDuration);
                    }
                }
                if (evocationAvailable)
                {
                    foreach (SegmentConstraint constraint in rowSegmentEvocation)
                    {
                        lp.SetRHSUnsafe(constraint.Row, EvocationDuration);
                    }
                }
                // timing
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    lp.SetRHSUnsafe(rowSegment + seg, SegmentList[seg].Duration);
                    //lp.SetLHSUnsafe(rowSegment + seg, SegmentList[seg].Duration); // good idea but it's causing numerical problems for some reason
                }
            }
            if (restrictManaUse)
            {
                if (CombinatorialSolver)
                {
                    for (int i = 0; i < combStateList.Count; i++)
                    {
                        lp.SetRHSUnsafe(combStateList[i].UnderflowConstraint, StartingMana);
                        lp.SetRHSUnsafe(combStateList[i].OverflowConstraint, BaseStats.Mana - StartingMana);
                    }
                }
                else
                {
                    for (int ss = 0; ss < SegmentList.Count * manaSegments - 1; ss++)
                    {
                        lp.SetRHSUnsafe(rowSegmentManaUnderflow + ss, StartingMana);
                    }
                    for (int ss = 0; ss < SegmentList.Count * manaSegments; ss++)
                    {
                        lp.SetRHSUnsafe(rowSegmentManaOverflow + ss, BaseStats.Mana - StartingMana);
                    }
                }
            }
            /*if (restrictThreat)
            {
                for (int ss = 0; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetRHSUnsafe(rowSegmentThreat + ss, CalculationOptions.TpsLimit * SegmentList[ss].TimeEnd);
                }
            }*/
            if (needsManaSegmentConstraints)
            {
                for (int ms = 0; ms < rowManaSegment.Count; ms++)
                {
                    lp.SetRHSUnsafe(rowManaSegment[ms].Row, double.PositiveInfinity);
                    lp.SetLHSUnsafe(rowManaSegment[ms].Row, EvocationCooldown);
                }
            }
            if (CombinatorialSolver)
            {
                for (int i = 0; i < rowCombinatorialConstraintCount; i++)
                {
                    lp.SetRHSUnsafe(rowCombinatorialConstraint[i].Row, rowCombinatorialConstraint[i].MaxTime);
                    lp.SetLHSUnsafe(rowCombinatorialConstraint[i].Row, rowCombinatorialConstraint[i].MinTime);
                }
            }
        }

        private int ConstructRows(bool minimizeTime, bool drinkingEnabled, bool needsTimeExtension, bool afterFightRegen)
        {
            #region Reset Rows
            rowManaRegen = -1;
            rowFightDuration = -1;
            rowEvocation = -1;
            rowEvocationIV = -1;
            rowEvocationHero = -1;
            rowEvocationIVHero = -1;
            rowPotion = -1;
            rowManaPotion = -1;
            rowConjureManaGem = -1;
            rowManaGem = -1;
            rowManaGemMax = -1;
            rowHeroism = -1;
            rowArcanePower = -1;
            rowIcyVeins = -1;
            rowMirrorImage = -1;
            rowManaGemEffect = -1;
            rowManaGemEffectActivation = -1;
            rowDpsTime = -1;
            rowAoe = -1;
            rowCombustion = -1;
            rowPowerInfusion = -1;
            rowHeroismIcyVeins = -1;
            rowSummonMirrorImage = -1;
            rowSummonMirrorImageCount = -1;
            //rowThreat = -1;
            rowBerserking = -1;
            rowBloodFury = -1;
            rowTimeExtension = -1;
            rowAfterFightRegenMana = -1;
            rowTargetDamage = -1;
            rowSegment = -1;
            rowSegmentManaOverflow = -1;
            rowSegmentManaUnderflow = -1;
            //rowSegmentThreat = -1;
            rowSegmentArcanePower = null;
            rowSegmentPowerInfusion = null;
            rowSegmentIcyVeins = null;
            rowSegmentMirrorImage = null;
            rowSegmentSummonMirrorImage = null;
            rowSegmentCombustion = null;
            rowSegmentBerserking = null;
            rowSegmentManaGem = null;
            rowSegmentManaGemEffect = null;
            rowSegmentEvocation = null;
            rowManaSegment = null;
            #endregion

            int rowCount = 0;

            if (!CalculationOptions.UnlimitedMana) rowManaRegen = rowCount++;
            rowFightDuration = rowCount++;
            if (evocationAvailable && (needsTimeExtension || restrictManaUse || integralMana || CalculationOptions.EnableHastedEvocation) && !CombinatorialSolver) rowEvocation = rowCount++;
            if (CalculationOptions.EnableHastedEvocation && !CombinatorialSolver)
            {
                if (evocationAvailable && icyVeinsAvailable)
                {
                    if (needsTimeExtension || restrictManaUse || integralMana) rowEvocationIV = rowCount++;
                    //rowEvocationIVActivation = rowCount++;
                }
                if (evocationAvailable && heroismAvailable)
                {
                    if (needsTimeExtension || restrictManaUse || integralMana) rowEvocationHero = rowCount++;
                    //rowEvocationHeroActivation = rowCount++;
                }
                if (evocationAvailable && icyVeinsAvailable && heroismAvailable)
                {
                    if (needsTimeExtension || restrictManaUse || integralMana) rowEvocationIVHero = rowCount++;
                    //rowEvocationIVHeroActivation = rowCount++;
                }
            }
            if (manaPotionAvailable || effectPotionAvailable) rowPotion = rowCount++;
            if (manaPotionAvailable && integralMana && !CombinatorialSolver) rowManaPotion = rowCount++;
            if (CalculationOptions.ManaGemEnabled)
            {
                if (segmentCooldowns || conjureManaGem || needsTimeExtension || segmentMana)
                {
                    rowManaGem = rowCount++;
                }
                if (requiresMIP || conjureManaGem)
                {
                    rowManaGemMax = rowCount++;
                }
                if (conjureManaGem)
                {
                    rowConjureManaGem = rowCount++;
                }
            }
            if (!CombinatorialSolver)
            {
                foreach (EffectCooldown cooldown in CooldownList)
                {
                    if (cooldown.AutomaticConstraints)
                    {
                        cooldown.Row = rowCount++;
                        cooldown.MaximumDuration = (float)MaximizeEffectDuration(CalculationOptions.FightDuration, cooldown.Duration, cooldown.Cooldown);
                    }
                }
                if (manaGemEffectAvailable) rowManaGemEffectActivation = rowCount++;
            }
            if (CalculationOptions.AoeDuration > 0)
            {
                rowAoe = rowCount++;
            }
            //if (CalculationOptions.TpsLimit > 0f) rowThreat = rowCount++;
            if (needsTimeExtension) rowTimeExtension = rowCount++;
            if (afterFightRegen) rowAfterFightRegenMana = rowCount++;
            if (minimizeTime) rowTargetDamage = rowCount++;
            if (mirrorImageAvailable && !CombinatorialSolver)
            {
                rowSummonMirrorImage = rowCount++;
                if (requiresMIP)
                {
                    rowSummonMirrorImageCount = rowCount++;
                }
            }
            if (dpsTime < 1 && (needsTimeExtension || segmentCooldowns || segmentMana))
            {
                rowDpsTime = rowCount++;
            }

            rowStackingConstraintCount = 0;
            if (!CombinatorialSolver)
            {
                if (rowStackingConstraint == null)
                {
                    rowStackingConstraint = new StackingConstraint[8];
                }
                for (int i = 0; i < CooldownList.Count; i++)
                {
                    EffectCooldown cooli = CooldownList[i];
                    if (cooli.AutomaticStackingConstraints)
                    {
                        for (int j = i + 1; j < CooldownList.Count; j++)
                        {
                            EffectCooldown coolj = CooldownList[j];
                            if (coolj.AutomaticStackingConstraints)
                            {
                                bool valid = true;
                                foreach (int exclusionMask in effectExclusionList)
                                {
                                    if (BitCount2((cooli.Mask | coolj.Mask) & exclusionMask))
                                    {
                                        valid = false;
                                        break;
                                    }
                                }
                                if (valid)
                                {
                                    // if we're using incremental optimizations and both are non-item based then we can
                                    // remove the constraint if they won't be used together
                                    if (UseIncrementalOptimizations && cooli.StandardEffect != StandardEffect.None && coolj.StandardEffect != StandardEffect.None)
                                    {
                                        int mask = (cooli.Mask | coolj.Mask);
                                        int[] sortedStates = CalculationOptions.IncrementalSetSortedStates;
                                        bool usedTogether = false;
                                        for (int incrementalSortedIndex = 0; incrementalSortedIndex < sortedStates.Length; incrementalSortedIndex++)
                                        {
                                            // incremental index is filtered by non-item based cooldowns
                                            int incrementalSetIndex = sortedStates[incrementalSortedIndex];
                                            if ((incrementalSetIndex & mask) == mask)
                                            {
                                                usedTogether = true;
                                                break;
                                            }
                                        }
                                        if (!usedTogether)
                                        {
                                            valid = false;
                                        }
                                    }
                                }
                                if (valid)
                                {
                                    double maxDuration = MaximizeStackingDuration(CalculationOptions.FightDuration, cooli.Duration, cooli.Cooldown, coolj.Duration, coolj.Cooldown);
                                    if (maxDuration < cooli.MaximumDuration && maxDuration < coolj.MaximumDuration)
                                    {
                                        int scCount = rowStackingConstraintCount;
                                        if (scCount >= rowStackingConstraint.Length)
                                        {
                                            StackingConstraint[] newArr = new StackingConstraint[rowStackingConstraint.Length * 2];
                                            Array.Copy(rowStackingConstraint, 0, newArr, 0, scCount);
                                            rowStackingConstraint = newArr;
                                        }
                                        // Heroism is before IcyVeins in cooldown list in cooldown list (initialized in InitializeEffectCooldowns)
                                        if (cooli.StandardEffect == StandardEffect.Heroism && coolj.StandardEffect == StandardEffect.IcyVeins)
                                        {
                                            rowHeroismIcyVeins = rowCount;
                                        }
                                        StackingConstraint[] arr = rowStackingConstraint;
                                        arr[scCount].Row = rowCount++;
                                        arr[scCount].MaximumStackingDuration = maxDuration;
                                        arr[scCount].Effect1 = cooli;
                                        arr[scCount].Effect2 = coolj;
                                        rowStackingConstraintCount = scCount + 1;
                                    }
                                }
                            }
                        }
                    }
                }

                if (icyVeinsAvailable)
                {
                    rowIcyVeins = EffectCooldown[(int)StandardEffect.IcyVeins].Row;
                }
                if (heroismAvailable) rowHeroism = EffectCooldown[(int)StandardEffect.Heroism].Row;
                if (arcanePowerAvailable) rowArcanePower = EffectCooldown[(int)StandardEffect.ArcanePower].Row;
                if (combustionAvailable) rowCombustion = EffectCooldown[(int)StandardEffect.Combustion].Row;
                if (powerInfusionAvailable) rowPowerInfusion = EffectCooldown[(int)StandardEffect.PowerInfusion].Row;
                if (berserkingAvailable) rowBerserking = EffectCooldown[(int)StandardEffect.Berserking].Row;
                if (bloodFuryAvailable) rowBloodFury = EffectCooldown[(int)StandardEffect.BloodFury].Row;
                if (mirrorImageAvailable) rowMirrorImage = EffectCooldown[(int)StandardEffect.MirrorImage].Row;
            }

            rowCombinatorialConstraintCount = 0;
            if (CombinatorialSolver)
            {
                if (rowCombinatorialConstraint == null)
                {
                    rowCombinatorialConstraint = new CombinatorialConstraint[16];
                }

                for (int i = 0; i < combList.Count; i++)
                {
                    if (combList[i].Activation)
                    {
                        // constraint on duration
                        int maxj = -1;
                        int maxSeg = combStateList.Count - 1;
                        for (int j = i + 1; j < combList.Count; j++)
                        {
                            if (combList[j].Cooldown == combList[i].Cooldown && !combList[j].Activation)
                            {
                                maxSeg = combList[j].StateIndex - 1;
                                maxj = j;
                                break;
                            }
                        }
                        int ccCount = rowCombinatorialConstraintCount;
                        if (ccCount >= rowCombinatorialConstraint.Length)
                        {
                            CombinatorialConstraint[] newArr = new CombinatorialConstraint[rowCombinatorialConstraint.Length * 2];
                            Array.Copy(rowCombinatorialConstraint, 0, newArr, 0, ccCount);
                            rowCombinatorialConstraint = newArr;
                        }
                        CombinatorialConstraint[] arr = rowCombinatorialConstraint;
                        int minSeg = combList[i].StateIndex;
                        int ccRow;
                        for (ccRow = 0; ccRow < ccCount; ccRow++)
                        {
                            if (arr[ccRow].MinSegment == minSeg && arr[ccRow].MaxSegment == maxSeg)
                            {
                                break;
                            }
                        }
                        if (maxSeg >= minSeg)
                        {
                            if (ccRow == ccCount)
                            {
                                if (!(CooldownList[combList[i].Cooldown].StandardEffect == StandardEffect.Evocation && minSeg == maxSeg && MageTalents.Invocation == 0))
                                {
                                    arr[ccCount].Row = rowCount++;
                                    arr[ccCount].MinSegment = minSeg;
                                    arr[ccCount].MaxSegment = maxSeg;
                                    if (MageTalents.Invocation > 0 && CooldownList[combList[i].Cooldown].StandardEffect == StandardEffect.Evocation)
                                    {
                                        if (maxj >= 0 && maxj + 1 < combList.Count && combList[maxj + 1].Activation && CooldownList[combList[maxj + 1].Cooldown].StandardEffect == StandardEffect.Invocation)
                                        {
                                            arr[ccCount].MinTime = combList[i].Duration - 0.001;
                                        }
                                        else
                                        {
                                            arr[ccCount].MinTime = Math.Min(Math.Max(combList[i].Duration / 3, 1), combList[i].Duration - 0.001);
                                        }
                                    }
                                    else if (CooldownList[combList[i].Cooldown].StandardEffect == StandardEffect.IncantersWardCooldown && maxSeg < combStateList.Count - 1)
                                    {
                                        arr[ccCount].MinTime = combList[i].Duration - 0.001;
                                    }
                                    else
                                    {
                                        arr[ccCount].MinTime = 0;
                                    }
                                    arr[ccCount].MaxTime = combList[i].Duration;
                                    rowCombinatorialConstraintCount = ccCount + 1;
                                }
                            }
                            else
                            {
                                if (MageTalents.Invocation > 0 && CooldownList[combList[i].Cooldown].StandardEffect == StandardEffect.Evocation)
                                {
                                    if (maxj >= 0 && maxj + 1 < combList.Count && combList[maxj + 1].Activation && CooldownList[combList[maxj + 1].Cooldown].StandardEffect == StandardEffect.Invocation)
                                    {
                                        arr[ccRow].MinTime = Math.Max(arr[ccRow].MinTime, combList[i].Duration - 0.001);
                                    }
                                    else
                                    {
                                        arr[ccRow].MinTime = Math.Max(arr[ccRow].MinTime, Math.Min(Math.Max(combList[i].Duration / 3, 1), combList[i].Duration - 0.001));
                                    }
                                }
                                else if (CooldownList[combList[i].Cooldown].StandardEffect == StandardEffect.IncantersWardCooldown && maxSeg < combStateList.Count - 1)
                                {
                                    arr[ccRow].MinTime = Math.Max(arr[ccRow].MinTime, combList[i].Duration - 0.001);
                                }
                                else
                                {
                                    arr[ccRow].MinTime = Math.Max(arr[ccRow].MinTime, 0);
                                }
                                arr[ccRow].MaxTime = Math.Min(arr[ccRow].MaxTime, combList[i].Duration);
                            }
                        }

                        // constraint on cooldown
                        if (CooldownList[combList[i].Cooldown].StandardEffect != StandardEffect.Invocation)
                        {
                            maxSeg = -1;
                            for (int j = i + 1; j < combList.Count; j++)
                            {
                                if (combList[j].Cooldown == combList[i].Cooldown && combList[j].Activation)
                                {
                                    maxSeg = combList[j].StateIndex - 1;
                                    break;
                                }
                            }
                            if (maxSeg >= 0)
                            {
                                ccCount = rowCombinatorialConstraintCount;
                                if (ccCount >= rowCombinatorialConstraint.Length)
                                {
                                    CombinatorialConstraint[] newArr = new CombinatorialConstraint[rowCombinatorialConstraint.Length * 2];
                                    Array.Copy(rowCombinatorialConstraint, 0, newArr, 0, ccCount);
                                    rowCombinatorialConstraint = newArr;
                                }
                                arr = rowCombinatorialConstraint;
                                minSeg = combList[i].StateIndex;
                                for (ccRow = 0; ccRow < ccCount; ccRow++)
                                {
                                    if (arr[ccRow].MinSegment == minSeg && arr[ccRow].MaxSegment == maxSeg)
                                    {
                                        break;
                                    }
                                }
                                if (ccRow == ccCount)
                                {
                                    arr[ccCount].Row = rowCount++;
                                    arr[ccCount].MinSegment = minSeg;
                                    arr[ccCount].MaxSegment = maxSeg;
                                    arr[ccCount].MinTime = CooldownList[combList[i].Cooldown].Cooldown;
                                    arr[ccCount].MaxTime = CalculationOptions.FightDuration;
                                    rowCombinatorialConstraintCount = ccCount + 1;
                                }
                                else
                                {
                                    arr[ccRow].MinTime = Math.Max(arr[ccRow].MinTime, CooldownList[combList[i].Cooldown].Cooldown);
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(CalculationOptions.CooldownOffset))
                {
                    if (CalculationOptions.CooldownOffsetList == null)
                    {
                        List<CooldownOffset> list = new List<CooldownOffset>();
                        var split = CalculationOptions.CooldownOffset.Split(',');
                        foreach (var e in split)
                        {
                            var tokens = e.Split('=');
                            if (tokens.Length == 2)
                            {
                                list.Add(new CooldownOffset() { Name = tokens[0].Trim(), Offset = double.Parse(tokens[1].Trim(), System.Globalization.CultureInfo.InvariantCulture) });
                            }
                        }
                        CalculationOptions.CooldownOffsetList = list;
                    }

                    foreach (var offset in CalculationOptions.CooldownOffsetList)
                    {
                        int activation = -1;
                        if (offset.Effect != StandardEffect.None)
                        {
                            for (int i = 0; i < combList.Count; i++)
                            {
                                if (combList[i].Activation && CooldownList[combList[i].Cooldown].StandardEffect == offset.Effect)
                                {
                                    activation = i;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            for (int i = 0; i < combList.Count; i++)
                            {
                                var c = CooldownList[combList[i].Cooldown];
                                if (combList[i].Activation && c.Name == offset.Name)
                                {
                                    activation = i;
                                    if (c.StandardEffect != StandardEffect.None)
                                    {
                                        offset.Effect = c.StandardEffect;
                                    }
                                    break;
                                }
                            }
                        }
                        if (activation >= 0)
                        {
                            int minSeg = 0;
                            int maxSeg = combList[activation].StateIndex - 1;
                            int ccCount = rowCombinatorialConstraintCount;
                            if (ccCount >= rowCombinatorialConstraint.Length)
                            {
                                CombinatorialConstraint[] newArr = new CombinatorialConstraint[rowCombinatorialConstraint.Length * 2];
                                Array.Copy(rowCombinatorialConstraint, 0, newArr, 0, ccCount);
                                rowCombinatorialConstraint = newArr;
                            }
                            CombinatorialConstraint[] arr = rowCombinatorialConstraint;
                            int ccRow;
                            for (ccRow = 0; ccRow < ccCount; ccRow++)
                            {
                                if (arr[ccRow].MinSegment == minSeg && arr[ccRow].MaxSegment == maxSeg)
                                {
                                    break;
                                }
                            }
                            if (ccRow == ccCount)
                            {
                                arr[ccCount].Row = rowCount++;
                                arr[ccCount].MinSegment = minSeg;
                                arr[ccCount].MaxSegment = maxSeg;
                                arr[ccCount].MinTime = offset.Offset;
                                arr[ccCount].MaxTime = CalculationOptions.FightDuration;
                                rowCombinatorialConstraintCount = ccCount + 1;
                            }
                            else
                            {
                                arr[ccRow].MinTime = Math.Max(arr[ccRow].MinTime, offset.Offset);
                                arr[ccRow].MaxTime = Math.Min(arr[ccRow].MaxTime, CalculationOptions.FightDuration);
                            }
                        }
                    }
                }
            }

            //rowManaPotionManaGem = rowCount++;
            if (segmentCooldowns)
            {
                rowCount = ConstructSegmentationRows(rowCount);
            }
            if (segmentCooldowns || segmentMana)
            {
                if (restrictManaUse)
                {
                    if (CombinatorialSolver)
                    {
                        rowSegmentManaOverflow = rowCount;
                        for (int i = 0; i < combStateList.Count; i++)
                        {
                            if (combStateList[i].OverflowConstraint >= 0)
                            {
                                combStateList[i].OverflowConstraint = rowCount++;
                            }
                        }
                        rowSegmentManaUnderflow = rowCount;
                        for (int i = 0; i < combStateList.Count; i++)
                        {
                            if (combStateList[i].UnderflowConstraint >= 0)
                            {
                                combStateList[i].UnderflowConstraint = rowCount++;
                            }
                        }
                    }
                    else
                    {
                        int segments = segmentCooldowns ? SegmentList.Count : 1;
                        rowSegmentManaOverflow = rowCount;
                        rowCount += segments * manaSegments;
                        rowSegmentManaUnderflow = rowCount;
                        rowCount += segments * manaSegments - 1;
                    }
                }
            }
            if (needsManaSegmentConstraints)
            {
                rowManaSegment = new List<ManaSegmentConstraint>();
                for (int i = 0; i < CalculationOptions.IncrementalSetVariableType.Length; i++)
                {
                    var t = CalculationOptions.IncrementalSetVariableType[i];
                    if (t == VariableType.Evocation || t == VariableType.EvocationHero || t == VariableType.EvocationIV || t == VariableType.EvocationIVHero)
                    {
                        if (rowManaSegment.Count == 0 || rowManaSegment[rowManaSegment.Count - 1].ManaSegment != CalculationOptions.IncrementalSetManaSegment[i])
                        {
                            rowManaSegment.Add(new ManaSegmentConstraint() { ManaSegment = CalculationOptions.IncrementalSetManaSegment[i], Row = rowCount++ });
                        }
                    }
                }
                if (rowManaSegment.Count > 0)
                {
                    rowManaSegment.RemoveAt(rowManaSegment.Count - 1);
                    rowCount--;
                }
            }
            return rowCount;
        }

        private int ConstructSegmentationRows(int rowCount)
        {
            // mf, heroism, ap, iv, combustion, drums, flamecap, destruction, t1, t2
            // mf
            // heroism
            // ap
            if (arcanePowerAvailable)
            {
                List<SegmentConstraint> list = rowSegmentArcanePower = new List<SegmentConstraint>();
                double cool = ArcanePowerCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // pi
            if (powerInfusionAvailable)
            {
                List<SegmentConstraint> list = rowSegmentPowerInfusion = new List<SegmentConstraint>();
                double cool = PowerInfusionCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // iv
            if (icyVeinsAvailable)
            {
                List<SegmentConstraint> list = rowSegmentIcyVeins = new List<SegmentConstraint>();
                double cool = IcyVeinsCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // combustion
            if (combustionAvailable)
            {
                List<SegmentConstraint> list = rowSegmentCombustion = new List<SegmentConstraint>();
                double cool = CombustionCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // berserking
            if (berserkingAvailable)
            {
                List<SegmentConstraint> list = rowSegmentBerserking = new List<SegmentConstraint>();
                double cool = 120;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // blood fury
            if (bloodFuryAvailable)
            {
                List<SegmentConstraint> list = rowSegmentBloodFury = new List<SegmentConstraint>();
                double cool = 120;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            /*if (waterElementalAvailable && !MageTalents.GlyphOfEternalWater)
            {
                List<SegmentConstraint> list = rowSegmentWaterElemental = new List<SegmentConstraint>();
                double cool = WaterElementalCooldown + (coldsnapAvailable ? WaterElementalDuration : 0.0);
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
                list = rowSegmentSummonWaterElemental = new List<SegmentConstraint>();
                cool = WaterElementalCooldown + (coldsnapAvailable ? WaterElementalDuration : 0.0);
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }*/
            if (mirrorImageAvailable)
            {
                List<SegmentConstraint> list = rowSegmentMirrorImage = new List<SegmentConstraint>();
                double cool = MirrorImageCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
                list = rowSegmentSummonMirrorImage = new List<SegmentConstraint>();
                cool = MirrorImageCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            if (CalculationOptions.ManaGemEnabled)
            {
                List<SegmentConstraint> list = rowSegmentManaGem = new List<SegmentConstraint>();
                double cool = 120;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // effect potion
            /*if (effectPotionAvailable)
            {
                rowSegmentPotion = rowCount;
                for (int seg = 0; seg < segments; seg++)
                {
                    rowCount++;
                    double cool = 120;
                    if (seg * segmentDuration + cool >= calculationOptions.FightDuration) break;
                }
            }*/
            for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
            {
                EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                if (cooldown.SegmentConstraints == null) // if there's existing one we guarantee that it is cleared
                {
                    cooldown.SegmentConstraints = new List<SegmentConstraint>();
                }
                List<SegmentConstraint> list = cooldown.SegmentConstraints;
                double cool = cooldown.Cooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // mana gem effect
            if (manaGemEffectAvailable)
            {
                List<SegmentConstraint> list = rowSegmentManaGemEffect = new List<SegmentConstraint>();
                double cool = 120.0;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            if (evocationAvailable)
            {
                List<SegmentConstraint> list = rowSegmentEvocation = new List<SegmentConstraint>();
                double cool = EvocationCooldown;
                for (int seg = 0; seg < SegmentList.Count; seg++)
                {
                    int maxs = SegmentList.FindIndex(s => s.TimeEnd > SegmentList[seg].TimeStart + cool + 0.00001) - 1;
                    if (maxs == -2) maxs = SegmentList.Count - 1;
                    if (list.Count == 0 || maxs > list[list.Count - 1].MaxSegment)
                    {
                        list.Add(new SegmentConstraint() { Row = rowCount++, MinSegment = seg, MaxSegment = maxs });
                    }
                }
            }
            // max segment time
            rowSegment = rowCount;
            rowCount += SegmentList.Count;
            // mana overflow & underflow (don't need over all segments, that is already verified)
            /*if (restrictThreat)
            {
                rowSegmentThreat = rowCount;
                rowCount += SegmentList.Count - 1;
            }*/
            return rowCount;
        }

        private void SetSpellColumn(bool minimizeTime, int segment, int manaSegment, CastingState state, int column, Cycle cycle, float multiplier)
        {
            double bound = CalculationOptions.FightDuration;
            double manaRegen = cycle.ManaPerSecond;
            lp.SetElementUnsafe(rowAfterFightRegenMana, column, manaRegen);
            lp.SetElementUnsafe(rowManaRegen, column, manaRegen);
            lp.SetElementUnsafe(rowFightDuration, column, 1.0);
            lp.SetElementUnsafe(rowTimeExtension, column, -1.0);
            if (state.VolcanicPotion)
            {
                lp.SetElementUnsafe(rowPotion, column, 1.0 / 25.0);
            }
            /*if (state.WaterElemental && !MageTalents.GlyphOfEternalWater)
            {
                lp.SetElementUnsafe(rowWaterElemental, column, 1.0);
                lp.SetElementUnsafe(rowSummonWaterElemental, column, 1 / (WaterElementalDuration - BaseGlobalCooldown));
            }*/
            if (state.MirrorImage)
            {
                lp.SetElementUnsafe(rowMirrorImage, column, 1.0);
                lp.SetElementUnsafe(rowSummonMirrorImage, column, 1 / (MirrorImageDuration - BaseGlobalCooldown));
            }
            if (state.Heroism) lp.SetElementUnsafe(rowHeroism, column, 1.0);
            if (state.ArcanePower) lp.SetElementUnsafe(rowArcanePower, column, 1.0);
            if (state.PowerInfusion) lp.SetElementUnsafe(rowPowerInfusion, column, 1.0);
            //if (state.Heroism && state.ArcanePower) lp.SetElementUnsafe(rowHeroismArcanePower, column, 1.0);
            //if (state.Heroism && state.ManaGemEffect) lp.SetElementUnsafe(rowHeroismManaGemEffect, column, 1.0);
            if (state.IcyVeins)
            {
                lp.SetElementUnsafe(rowIcyVeins, column, 1.0);
            }
            //if (state.MoltenFury && state.PotionOfWildMagic) lp.SetElementUnsafe(rowMoltenFuryDestructionPotion, column, 1.0);
            //if (state.MoltenFury && state.ManaGemEffect) lp.SetElementUnsafe(rowMoltenFuryManaGemEffect, column, 1.0);
            //if (state.PotionOfWildMagic && state.Heroism) lp.SetElementUnsafe(rowHeroismDestructionPotion, column, 1.0);
            //if (state.PotionOfWildMagic && state.IcyVeins) lp.SetElementUnsafe(rowIcyVeinsDestructionPotion, column, 1.0);
            //if (state.MoltenFury && state.FlameCap) lp.SetElementUnsafe(rowMoltenFuryFlameCap, column, 1.0);
            //if (state.PotionOfWildMagic && state.FlameCap) lp.SetElementUnsafe(rowFlameCapDestructionPotion, column, 1.0);
            if (!CombinatorialSolver)
            {
                for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
                {
                    EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                    if (state.EffectsActive(cooldown.Mask))
                    {
                        lp.SetElementUnsafe(cooldown.Row, column, 1.0);
                    }
                }
            }
            if (state.ManaGemEffect) lp.SetElementUnsafe(rowManaGemEffectActivation, column, 1 / ManaGemEffectDuration);
            if (cycle.AreaEffect)
            {
                lp.SetElementUnsafe(rowAoe, column, 1.0);
            }
            if (state.Combustion)
            {
                lp.SetElementUnsafe(rowCombustion, column, 1.0);
                //if (state.MoltenFury) lp.SetElementUnsafe(rowMoltenFuryCombustion, column, (1 / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs)));
                //if (state.Heroism) lp.SetElementUnsafe(rowHeroismCombustion, column, (1 / (state.CombustionDuration * cycle.CastTime / cycle.CastProcs)));
            }
            if (state.Berserking) lp.SetElementUnsafe(rowBerserking, column, 1.0);
            //if (state.Berserking && state.MoltenFury) lp.SetElementUnsafe(rowMoltenFuryBerserking, column, 1.0);
            //if (state.Berserking && state.Heroism) lp.SetElementUnsafe(rowHeroismBerserking, column, 1.0);
            //if (state.Berserking && state.IcyVeins) lp.SetElementUnsafe(rowIcyVeinsBerserking, column, 1.0);
            //if (state.Berserking && state.ArcanePower) lp.SetElementUnsafe(rowArcanePowerBerserking, column, 1.0);
            if (state.BloodFury) lp.SetElementUnsafe(rowBloodFury, column, 1.0);
            //lp.SetElementUnsafe(rowThreat, column, cycle.ThreatPerSecond);
            //lp[rowManaPotionManaGem, index] = (statsList[buffset].FlameCap ? 1 : 0) + (statsList[buffset].DestructionPotion ? 40.0 / 15.0 : 0);
            if (needsQuadratic)
            {
                double dps = cycle.GetDamagePerSecond(state.ManaAdeptBonus, StartingMana / BaseStats.Mana);
                lp.SetElementUnsafe(rowTargetDamage, column, -dps * multiplier);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : dps * multiplier);
                lp.SetSpellDpsUnsafe(column, cycle.GetQuadraticSpellDPS() * multiplier);
            }
            else
            {
                lp.SetElementUnsafe(rowTargetDamage, column, -cycle.DamagePerSecond * multiplier);
                lp.SetCostUnsafe(column, minimizeTime ? -1 : cycle.DamagePerSecond * multiplier);
            }

            for (int i = 0; i < rowStackingConstraintCount; i++)
            {
                //if (state.EffectsActive(rowStackingConstraint[i].Effect1.Mask | rowStackingConstraint[i].Effect2.Mask))
                int effects = rowStackingConstraint[i].Effect1.Mask | rowStackingConstraint[i].Effect2.Mask;
                if ((effects & state.Effects) == effects)
                {
                    lp.SetElementUnsafe(rowStackingConstraint[i].Row, column, 1.0);
                }
            }

            if (segmentCooldowns)
            {
                bound = SetSpellColumnSegment(segment, manaSegment, state, column, cycle, bound, manaRegen);
            }
            if (restrictManaUse)
            {
                SetManaConstraint(manaRegen, segment, manaSegment, column, false);
            }
            if (needsManaSegmentConstraints)
            {
                SetManaSegmentConstraint(manaSegment, column);
            }
            if (CombinatorialSolver)
            {
                SetCombinatorialConstraint(manaSegment, column);
            }
            lp.SetColumnUpperBound(column, bound);
        }

        private double SetSpellColumnSegment(int segment, int manaSegment, CastingState state, int column, Cycle cycle, double bound, double manaRegen)
        {
            // mf, heroism, ap, iv, combustion, drums, flamecap, destro, t1, t2
            //lp[rowOffset + 1 * segments + seg, index] = 1;
            if (state.ArcanePower)
            {
                bound = Math.Min(bound, ArcanePowerDuration);
                foreach (SegmentConstraint constraint in rowSegmentArcanePower)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.PowerInfusion)
            {
                bound = Math.Min(bound, PowerInfusionDuration);
                foreach (SegmentConstraint constraint in rowSegmentPowerInfusion)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.IcyVeins)
            {
                bound = Math.Min(bound, 20.0);
                foreach (SegmentConstraint constraint in rowSegmentIcyVeins)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            /*if (state.WaterElemental)
            {
                foreach (SegmentConstraint constraint in rowSegmentWaterElemental)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }*/
            if (state.MirrorImage)
            {
                foreach (SegmentConstraint constraint in rowSegmentMirrorImage)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.Combustion)
            {
                foreach (SegmentConstraint constraint in rowSegmentCombustion)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.Berserking)
            {
                foreach (SegmentConstraint constraint in rowSegmentBerserking)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (state.VolcanicPotion)
            {
                bound = Math.Min(bound, 25.0);
            }
            for (int i = 0; i < ItemBasedEffectCooldownsCount; i++)
            {
                EffectCooldown cooldown = ItemBasedEffectCooldowns[i];
                if (state.EffectsActive(cooldown.Mask))
                {
                    bound = Math.Min(bound, cooldown.Duration);
                    foreach (SegmentConstraint constraint in cooldown.SegmentConstraints)
                    {
                        if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                    }
                }
            }
            if (state.ManaGemEffect)
            {
                bound = Math.Min(bound, ManaGemEffectDuration);
                foreach (SegmentConstraint constraint in rowSegmentManaGemEffect)
                {
                    if (segment >= constraint.MinSegment && segment <= constraint.MaxSegment) lp.SetElementUnsafe(constraint.Row, column, 1.0);
                }
            }
            if (segmentNonCooldowns || state != BaseState)
            {
                bound = Math.Min(bound, SegmentList[segment].Duration);
                lp.SetElementUnsafe(rowSegment + segment, column, 1.0);
            }
            /*if (restrictThreat)
            {
                for (int ss = segment; ss < SegmentList.Count - 1; ss++)
                {
                    lp.SetElementUnsafe(rowSegmentThreat + ss, column, cycle.ThreatPerSecond);
                }
            }*/
            return bound;
        }

        private void GenerateSpellList()
        {
            if (spellList == null)
            {
                spellList = new List<CycleId>();
            }
            else
            {
                spellList.Clear();
            }

            if (CalculationOptions.CustomSpellMixEnabled || CalculationOptions.CustomSpellMixOnly)
            {
                spellList.Add(CycleId.CustomSpellMix);
            }
            if (!CalculationOptions.CustomSpellMixOnly)
            {
                switch (Specialization)
                {
                    case Specialization.Arcane:
                        if (CalculationOptions.ArcaneLight)
                        {
                            spellList.Add(CycleId.AB4AM);
                            spellList.Add(CycleId.ArcaneManaNeutral);
                        }
                        else
                        {
                            spellList.Add(CycleId.AB4AM);
                            spellList.Add(CycleId.AB4ABar4AM);
                            spellList.Add(CycleId.AB4ABar34AM);
                            spellList.Add(CycleId.AB34ABar34AM);
                            spellList.Add(CycleId.AB24ABar34AM);
                            spellList.Add(CycleId.AB234ABar34AM);
                            spellList.Add(CycleId.AB2ABar0AM);
                            if (CalculationOptions.IncludeManaNeutralCycleMix)
                            {
                                spellList.Add(CycleId.ArcaneManaNeutral);
                            }
                        }
                        break;
                    case Specialization.Fire:
                        spellList.Add(CycleId.FBIBPyro);
                        spellList.Add(CycleId.ScIBPyro);
                        spellList.Add(CycleId.FFBIBPyro);
                        break;
                    case Specialization.Frost:
                        spellList.Add(CycleId.FFBILFrOFrB);
                        break;
                    case Specialization.None:
                        break;
                }
            }
            if (CalculationOptions.AoeDuration > 0)
            {
                switch (Specialization)
                {
                    case Specialization.Arcane:
                        spellList.Add(CycleId.AE4AB); // TODO aoe rotation
                        spellList.Add(CycleId.AERampAB);
                        break;
                    case Specialization.Fire:
                        spellList.Add(CycleId.Blizzard); // TODO aoe rotation
                        break;
                    case Specialization.Frost:
                        spellList.Add(CycleId.Blizzard);
                        break;
                    case Specialization.None:
                        spellList.Add(CycleId.Blizzard);
                        break;
                }
            }                      
        }

        // http://tekpool.wordpress.com/category/bit-count/
        int BitCount(int i)
        {
            uint u = (uint)i;
            int uCount = 0;

            for (; u != 0; u &= (u - 1))
                uCount++;

            return uCount;
        }

        bool BitCount2(int i)
        {
            uint u = (uint)i;

            if (u == 0) return false;
            u &= (u - 1);
            return u != 0;
        }

        private void GenerateStateList()
        {
            BaseState = CastingState.New(this, baseArmorMask, false, 0);

            if (stateList == null)
            {
                stateList = new List<CastingState>(64);
            }
            else
            {
                stateList.Clear();
            }
            if (UseIncrementalOptimizations)
            {
                int[] sortedStates = CalculationOptions.IncrementalSetSortedStates;
                for (int incrementalSortedIndex = 0; incrementalSortedIndex < sortedStates.Length; incrementalSortedIndex++)
                {
                    // incremental index is filtered by non-item based cooldowns
                    int incrementalSetIndex = sortedStates[incrementalSortedIndex];
                    bool heroism = (incrementalSetIndex & (int)StandardEffect.Heroism) != 0;
                    int itemBasedMax = 1 << ItemBasedEffectCooldownsCount;
                    for (int index = 0; index < itemBasedMax; index++)
                    {
                        int combinedIndex = incrementalSetIndex | (index << standardEffectCount);
                        if ((combinedIndex & availableCooldownMask) == combinedIndex) // make sure all are available
                        {
                            bool valid = true;
                            foreach (int exclusionMask in effectExclusionList)
                            {
                                if (BitCount2(combinedIndex & exclusionMask))
                                {
                                    valid = false;
                                    break;
                                }
                            }
                            if (valid)
                            {
                                if ((CalculationOptions.HeroismControl != 1 || !heroism || true) && (CalculationOptions.HeroismControl != 2 || !heroism || (combinedIndex == (int)StandardEffect.Heroism && index == 0)))
                                {
                                    if (combinedIndex == BaseState.Effects)
                                    {
                                        stateList.Add(BaseState);
                                    }
                                    else
                                    {
                                        stateList.Add(CastingState.New(this, combinedIndex, false, 0));
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                for (int incrementalSetIndex = 0; incrementalSetIndex <= availableCooldownMask; incrementalSetIndex++)
                {
                    if (((incrementalSetIndex) & availableCooldownMask) == (incrementalSetIndex)) // make sure all are available
                    {
                        bool valid = true;
                        foreach (int exclusionMask in effectExclusionList)
                        {
                            if (BitCount2(incrementalSetIndex & exclusionMask))
                            {
                                valid = false;
                                break;
                            }
                        }
                        // make sure only one armor is active, but at least one if one is available
                        int armorCountActive = 0;
                        if ((incrementalSetIndex & (int)StandardEffect.MageArmor) != 0) armorCountActive++;
                        if ((incrementalSetIndex & (int)StandardEffect.MoltenArmor) != 0) armorCountActive++;
                        if ((incrementalSetIndex & (int)StandardEffect.FrostArmor) != 0) armorCountActive++;
                        if (armorCountActive > 1)
                        {
                            valid = false;
                        }
                        else if (armorCountActive == 0 && (mageArmorAvailable || moltenArmorAvailable || frostArmorAvailable))
                        {
                            valid = false;
                        }
                        if (valid)
                        {
                            bool heroism = (incrementalSetIndex & (int)StandardEffect.Heroism) != 0;
                            if ((CalculationOptions.HeroismControl != 1 || !heroism || true) && (CalculationOptions.HeroismControl != 2 || !heroism || (incrementalSetIndex == (int)StandardEffect.Heroism)))
                            {
                                if (incrementalSetIndex == BaseState.Effects)
                                {
                                    stateList.Add(BaseState);
                                }
                                else
                                {
                                    stateList.Add(CastingState.New(this, incrementalSetIndex, false, 0));
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region Quadratic Solver
        internal static int GetQuadraticIndex(SolutionVariable v)
        {
            if (v.IsZeroTime)
            {
                if (v.Mps >= -0.001)
                {
                    return 2;
                }
                else
                {
                    return 0;
                }
            }
            if (v.Mps >= -0.001 || (v.Cycle != null && v.Cycle.CycleId == CycleId.ArcaneManaNeutral))
            {
                return 3;
            }
            return 1;
        }

        private void SolveQuadratic()
        {
            // for now requires solution variables to work
            int[] sort = new int[lp.Columns];
            for (int j = 0; j < lp.Columns; j++)
            {
                sort[j] = j;
            }
            Array.Sort(sort, (x, y) =>
            {
                SolutionVariable vx = SolutionVariable[x];
                SolutionVariable vy = SolutionVariable[y];
                int comp = vx.Segment.CompareTo(vy.Segment);
                if (comp != 0) return comp;
                comp = vx.ManaSegment.CompareTo(vy.ManaSegment);
                if (comp != 0) return comp;
                // first instant mana gain, then negative mps
                // then mana overflow, then positive mps
                comp = GetQuadraticIndex(vx).CompareTo(GetQuadraticIndex(vy));
                if (comp != 0) return comp;
                return vx.Mps.CompareTo(vy.Mps);
            });

            lp.SolvePrimalQuadratic(rowManaRegen, sort, 1 / (BaseStats.Mana * ManaRegenLPScaling), UseIncrementalOptimizations || SimpleStacking, CalculationOptions.TargetDamage > 0 ? lp.Columns + rowFightDuration : -1, CalculationOptions.TargetDamage);
        }
        #endregion

        #region Calculation Result
        private DisplayCalculations GetDisplayCalculations(CharacterCalculationsMage baseCalculations)
        {
            DisplayCalculations displayCalculations = new DisplayCalculations();

            displayCalculations.Specialization = Specialization;
            displayCalculations.BaseStats = BaseStats;
            displayCalculations.BaseState = BaseState;
            displayCalculations.Character = Character;
            displayCalculations.CalculationOptions = CalculationOptions;
            displayCalculations.SolutionVariable = SolutionVariable;
            displayCalculations.MageTalents = MageTalents;
            displayCalculations.Solution = solution;
            displayCalculations.BaseCalculations = baseCalculations;

            displayCalculations.RawArcaneHitRate = RawArcaneHitRate;
            displayCalculations.RawFireHitRate = RawFireHitRate;
            displayCalculations.RawFrostHitRate = RawFrostHitRate;

            displayCalculations.CooldownList = new List<EffectCooldown>(CooldownList);
            displayCalculations.EffectCooldown = new Dictionary<int,EffectCooldown>(EffectCooldown);
            displayCalculations.ItemBasedEffectCooldowns = new EffectCooldown[ItemBasedEffectCooldownsCount];
            Array.Copy(ItemBasedEffectCooldowns, 0, displayCalculations.ItemBasedEffectCooldowns, 0, ItemBasedEffectCooldownsCount);

            displayCalculations.SegmentList = new List<Segment>(SegmentList);

            displayCalculations.SpellPowerEffects = new SpecialEffect[SpellPowerEffectsCount];
            Array.Copy(SpellPowerEffects, 0, displayCalculations.SpellPowerEffects, 0, SpellPowerEffectsCount);
            displayCalculations.IntellectEffects = new SpecialEffect[IntellectEffectsCount];
            Array.Copy(IntellectEffects, 0, displayCalculations.IntellectEffects, 0, IntellectEffectsCount);
            displayCalculations.MasteryRatingEffects = new SpecialEffect[MasteryRatingEffectsCount];
            Array.Copy(MasteryRatingEffects, 0, displayCalculations.MasteryRatingEffects, 0, MasteryRatingEffectsCount);
            displayCalculations.CritRatingEffects = new SpecialEffect[CritRatingEffectsCount];
            Array.Copy(CritRatingEffects, 0, displayCalculations.CritRatingEffects, 0, CritRatingEffectsCount);
            displayCalculations.HasteRatingEffects = new SpecialEffect[HasteRatingEffectsCount];
            Array.Copy(HasteRatingEffects, 0, displayCalculations.HasteRatingEffects, 0, HasteRatingEffectsCount);

            displayCalculations.BaseGlobalCooldown = BaseGlobalCooldown;
            displayCalculations.ManaAdeptBonus = ManaAdeptBonus;
            displayCalculations.IgniteBonus = IgniteBonus;
            displayCalculations.FrostburnBonus = FrostburnBonus;

            displayCalculations.EvocationDuration = EvocationDuration;
            displayCalculations.EvocationRegen = EvocationRegen;
            displayCalculations.EvocationDurationIV = EvocationDurationIV;
            displayCalculations.EvocationRegenIV = EvocationRegenIV;
            displayCalculations.EvocationDurationHero = EvocationDurationHero;
            displayCalculations.EvocationRegenHero = EvocationRegenHero;
            displayCalculations.EvocationDurationIVHero = EvocationDurationIVHero;
            displayCalculations.EvocationRegenIVHero = EvocationRegenIVHero;

            displayCalculations.MaxManaGem = MaxManaGem;
            displayCalculations.MaxEvocation = MaxEvocation;
            displayCalculations.ManaGemTps = ManaGemTps;
            displayCalculations.ManaPotionTps = ManaPotionTps;
            displayCalculations.ManaGemValue = ManaGemValue;
            displayCalculations.MaxManaGemValue = MaxManaGemValue;
            displayCalculations.ManaPotionValue = ManaPotionValue;
            displayCalculations.MaxManaPotionValue = MaxManaPotionValue;

            displayCalculations.CombustionCooldown = CombustionCooldown;
            displayCalculations.PowerInfusionDuration = PowerInfusionDuration;
            displayCalculations.PowerInfusionCooldown = PowerInfusionCooldown;
            displayCalculations.MirrorImageDuration = MirrorImageDuration;
            displayCalculations.MirrorImageCooldown = MirrorImageCooldown;
            displayCalculations.IcyVeinsCooldown = IcyVeinsCooldown;
            displayCalculations.ColdsnapCooldown = ColdsnapCooldown;
            displayCalculations.ArcanePowerCooldown = ArcanePowerCooldown;
            displayCalculations.ArcanePowerDuration = ArcanePowerDuration;
            //displayCalculations.WaterElementalCooldown = WaterElementalCooldown;
            //displayCalculations.WaterElementalDuration = WaterElementalDuration;
            displayCalculations.EvocationCooldown = EvocationCooldown;
            displayCalculations.ManaGemEffectDuration = ManaGemEffectDuration;

            displayCalculations.StartingMana = StartingMana;
            displayCalculations.ConjureManaGem = ConjureManaGem;
            displayCalculations.MaxConjureManaGem = MaxConjureManaGem;
            displayCalculations.ManaGemEffect = manaGemEffectAvailable;
            displayCalculations.ChanceToDie = ChanceToDie;
            displayCalculations.MeanIncomingDps = MeanIncomingDps;
            displayCalculations.MageArmor = armor;
            displayCalculations.DamageTakenReduction = DamageTakenReduction;

            //displayCalculations.FrBDFFFBIL_KDFS = FrBDFFFBIL_KDFS;
            //displayCalculations.FrBDFFFBIL_KFFB = FrBDFFFBIL_KFFB;
            //displayCalculations.FrBDFFFBIL_KFFBS = FrBDFFFBIL_KFFBS;
            //displayCalculations.FrBDFFFBIL_KFrB = FrBDFFFBIL_KFrB;
            //displayCalculations.FrBDFFFBIL_KILS = FrBDFFFBIL_KILS;

            if (!requiresMIP)
            {
                displayCalculations.UpperBound = lp.Value;
                displayCalculations.LowerBound = 0.0;
            }
            else
            {
                displayCalculations.UpperBound = upperBound;
                if (integralMana && segmentCooldowns && advancedConstraintsLevel >= 5) displayCalculations.LowerBound = lowerBound;
            }

            /*float threat = 0;
            for (int i = 0; i < SolutionVariable.Count; i++)
            {
                threat += (float)(SolutionVariable[i].Tps * solution[i]);
            }
            displayCalculations.Tps = threat / CalculationOptions.FightDuration;*/

            return displayCalculations;
        }

        private OptimizableCalculations GetOptimizableCalculations()
        {
            OptimizableCalculations optimizableCalculations = new OptimizableCalculations();

            optimizableCalculations.ArcaneResistance = BaseStats.ArcaneResistance;
            optimizableCalculations.ChanceToDie = ChanceToDie;
            optimizableCalculations.FireResistance = BaseStats.FireResistance;
            optimizableCalculations.FrostResistance = BaseStats.FrostResistance;
            optimizableCalculations.HasteRating = BaseStats.HasteRating;
            optimizableCalculations.CritRating = BaseStats.CritRating;
            optimizableCalculations.Health = BaseStats.Health;
            optimizableCalculations.HitRating = BaseStats.HitRating;
            optimizableCalculations.MovementSpeed = BaseStats.MovementSpeed;
            optimizableCalculations.NatureResistance = BaseStats.NatureResistance;
            optimizableCalculations.PVPTrinket = BaseStats.PVPTrinket;
            optimizableCalculations.PvPResilience = BaseStats.PvPResilience;
            optimizableCalculations.ShadowResistance = BaseStats.ShadowResistance;

            return optimizableCalculations;
        }

        private CharacterCalculationsMage GetCalculationsResult()
        {
            CharacterCalculationsMage calculationResult = new CharacterCalculationsMage();

            if (CalculationOptions.TargetDamage > 0)
            {
                calculationResult.SubPoints[0] = -(float)(CalculationOptions.TargetDamage / solution[solution.Length - 1]);
            }
            else
            {
                calculationResult.SubPoints[0] = ((float)solution[solution.Length - 1] /*+ calculationResult.WaterElementalDamage*/) / CalculationOptions.FightDuration;
            }
            calculationResult.SubPoints[1] = EvaluateSurvivability();
            calculationResult.OverallPoints = calculationResult.SubPoints[0] + calculationResult.SubPoints[1];

            if (NeedsDisplayCalculations)
            {
                calculationResult.DisplayCalculations = GetDisplayCalculations(calculationResult);
            }
            calculationResult.OptimizableCalculations = GetOptimizableCalculations();

            if (autoActivatedBuffs != null)
            {
                calculationResult.AutoActivatedBuffs.AddRange(autoActivatedBuffs);
            }

            SolutionVariable = null;

            return calculationResult;
        }
        #endregion

        #region Survivability
        public void CalculateChanceToDie()
        {
            double ampMelee = IncomingDamageAmpMelee;
            double ampPhysical = IncomingDamageAmpPhysical;
            double ampArcane = IncomingDamageAmpArcane;
            double ampFire = IncomingDamageAmpFire;
            double ampFrost = IncomingDamageAmpFrost;
            double ampNature = IncomingDamageAmpNature;
            double ampShadow = IncomingDamageAmpShadow;
            double ampHoly = IncomingDamageAmpHoly;

            double melee = IncomingDamageDpsMelee;
            double physical = IncomingDamageDpsPhysical;
            double arcane = IncomingDamageDpsArcane;
            double fire = IncomingDamageDpsFire;
            double frost = IncomingDamageDpsFrost;
            double nature = IncomingDamageDpsNature;
            double shadow = IncomingDamageDpsShadow;
            double holy = IncomingDamageDpsHoly;

            double burstWindow = CalculationOptions.BurstWindow;
            double burstImpacts = CalculationOptions.BurstImpacts;

            // B(n, p) ~ N(np, np(1-p))
            // n = burstImpacts
            // Xi ~ ampi * (dpsi * (1 + B(n, criti) / n * critMulti) + doti)
            //    ~ ampi * (dpsi * (1 + N(n * criti, n * criti * (1 - criti)) / n * critMulti) + doti)
            //    ~ N(ampi * (doti + dpsi * (1 + critMulti * criti)), ampi^2 * dpsi^2 * critMulti^2 / n * criti * (1 - criti))
            // X = sum Xi ~ N(sum ampi * (doti + dpsi * (1 + critMulti * criti)), sum ampi^2 * dpsi^2 * critMulti^2 / n * criti * (1 - criti))
            // H = Health + hp5 / 5 * burstWindow
            // P(burstWindow * sum Xi >= H) = 1 - P(burstWindow * sum Xi <= H) = 1 / 2 * (1 - Erf((H - mu) / (sigma * sqrt(2)))) =
            //                = 1 / 2 * (1 - Erf((H / burstWindow - [sum ampi * (doti + dpsi * (1 + critMulti * criti))]) / sqrt(2 * [sum ampi^2 * dpsi^2 * critMulti^2 / n * criti * (1 - criti)])))

            double meleeVar = Math.Pow(ampMelee * CalculationOptions.MeleeDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.MeleeCrit / 100.0 - BaseState.PhysicalCritReduction) * (1 - Math.Max(0, CalculationOptions.MeleeCrit / 100.0 - BaseState.PhysicalCritReduction));
            double physicalVar = Math.Pow(ampPhysical * CalculationOptions.PhysicalDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.PhysicalCrit / 100.0 - BaseState.PhysicalCritReduction) * (1 - Math.Max(0, CalculationOptions.PhysicalCrit / 100.0 - BaseState.PhysicalCritReduction));
            double arcaneVar = Math.Pow(ampArcane * CalculationOptions.ArcaneDps * (1.75 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.ArcaneCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.ArcaneCrit / 100.0 - BaseState.SpellCritReduction));
            double fireVar = Math.Pow(ampFire * CalculationOptions.FireDps * (2.1 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.FireCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.FireCrit / 100.0 - BaseState.SpellCritReduction));
            double frostVar = Math.Pow(ampFrost * CalculationOptions.FrostDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.FrostCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.FrostCrit / 100.0 - BaseState.SpellCritReduction));
            double holyVar = Math.Pow(ampHoly * CalculationOptions.HolyDps * (1.5 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.HolyCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.HolyCrit / 100.0 - BaseState.SpellCritReduction));
            double natureVar = Math.Pow(ampNature * CalculationOptions.NatureDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.NatureCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.NatureCrit / 100.0 - BaseState.SpellCritReduction));
            double shadowVar = Math.Pow(ampShadow * CalculationOptions.ShadowDps * (2 * (1 - BaseState.CritDamageReduction) - 1), 2) / burstImpacts * Math.Max(0, CalculationOptions.ShadowCrit / 100.0 - BaseState.SpellCritReduction) * (1 - Math.Max(0, CalculationOptions.ShadowCrit / 100.0 - BaseState.SpellCritReduction));

            double Xmean = melee + physical + arcane + fire + frost + holy + nature + shadow - CalculationOptions.PassiveHealing;
            double Xvar = meleeVar + physicalVar + arcaneVar + fireVar + frostVar + holyVar + natureVar + shadowVar;

            // T = healing response time ~ N(Tmean, Tvar)
            // T * X ~ N(Tmean * Xmean, Tvar * Xvar + Tmean^2 * Xvar + Xmean^2 * Tvar)   // approximation reasonable for high Tmean / sqrt(Tvar)
            // P(T * X >= H) = 1 / 2 * (1 - Erf((H - mean) / (sigma * sqrt(2)))) =
            //               = 1 / 2 * (1 - Erf((H - mean) / sqrt(2 * var)))
            //               = 1 / 2 * (1 - Erf((H - Tmean * Xmean) / sqrt(2 * (Tvar * Xvar + Tmean^2 * Xvar + Xmean^2 * Tvar))))

            // Tvar := Tk * Tmean^2,   Tk <<< 1

            // P(T * X >= H) = 1 / 2 * (1 - Erf((H / Tmean - Xmean) / sqrt(2 * (Xvar * (Tk + 1) + Xmean^2 * Tk))))

            double Tk = 0.01;

            ChanceToDie = (float)(0.5f * (1f - SpecialFunction.Erf((BaseStats.Health / burstWindow + BaseStats.Hp5 / 5 - Xmean) / Math.Sqrt(2 * (Xvar * (1 + Tk) + Xmean * Xmean * Tk)))));
            MeanIncomingDps = (float)Xmean;
        }

        private float EvaluateSurvivability()
        {
            float ret = BaseStats.Health * CalculationOptions.SurvivabilityRating;
            if (CalculationOptions.ChanceToLiveScore > 0 || NeedsDisplayCalculations)
            {
                CalculateChanceToDie();

                //double maxTimeToDie = 1.0 / (1 - calculationOptions.ChanceToLiveLimit / 100.0) - 1;
                //double timeToDie = Math.Min(1.0 / calculatedStats.ChanceToDie - 1, maxTimeToDie);

                //calculatedStats.SubPoints[1] = calculatedStats.BasicStats.Health * calculationOptions.SurvivabilityRating + (float)(calculationOptions.ChanceToLiveScore * timeToDie / maxTimeToDie);
                ret += (float)(CalculationOptions.ChanceToLiveScore * Math.Pow(1 - ChanceToDie, CalculationOptions.ChanceToLiveAttenuation));
                if (float.IsNaN(ret)) ret = 0f;
            }
            else
            {
                ChanceToDie = 0f;
                MeanIncomingDps = 0f;
            }
            return ret;
        }
        #endregion
    }
}
