﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr
{
    public static class StatConversion
    {   // Class only works for Level 80 Characters (Level 85 if you are in the Rawr4 projects)
        // Numbers reverse engineered by Whitetooth (hotdogee [at] gmail [dot] com)

        #region Character Constants
        public const int DEFAULTPLAYERLEVEL = 85;

        /*  Here are the values data-mined from build 12942 and imported into SimulationCraft:

            Base Chance to Melee Crit%: 2.9219999909401%
            Base Chance to Spell Crit%: 2.2010000422597%

            Spell Scaling Multiplier: 1004.489990234375000 (level 80 value is 900.120300292968750. i.e. Level 85 base damage of abilities is 1004.49/900.12 or *1.12 more than at level 80 and so is the damage range.)
            Agi to +Melee Crit%: 0.0030780898669%
            Int to +Spell Crit%: 0.0015410500055%
            Int/Spirit regen coefficient: 0.003345000091940

            Ratings:

            Dodge: 176.718994140625000
            Parry: 176.718994140625000
            Block: 88.359397888183594
            Melee Hit: 120.109001159667969
            Ranged Hit: 120.109001159667969
            Spell Hit: 102.445999145507812
            Melee Crit: 179.279998779296875
            Ranged Crit: 179.279998779296875
            Spell Crit: 179.279998779296875
            Melee Haste: 128.057006835937500
            Ranged Haste: 128.057006835937500
            Spell Haste: 128.057006835937500
            Expertise: 30.027200698852539
            Mastery: 179.279998779296875

            The following is from in-game testing:
            Chance to Dodge Base%: 1.67%
            
         */

        // These are set based on the values above
        public const float RATING_PER_DODGE         = 17671.8994140625000f;
        public const float RATING_PER_PARRY         = 17671.8994140625000f;
        public const float RATING_PER_BLOCK         =  8835.9397888183594f;
        public const float RATING_PER_PHYSICALHIT   = 12010.9001159667969f;
        public const float RATING_PER_SPELLHIT      = 10244.5999145507812f;
        public const float RATING_PER_PHYSICALCRIT  = 17927.9998779296875f; 
        public const float RATING_PER_SPELLCRIT     = 17927.9998779296875f; 
        public const float RATING_PER_PHYSICALHASTE = 12805.7006835937500f; 
        public const float RATING_PER_SPELLHASTE    = 12805.7006835937500f; 
        public const float RATING_PER_EXPERTISE     =  30.027200698852539f; // Not a Perc, so decimal over
        public const float RATING_PER_MASTERY       = 179.279998779296875f; // Not a Perc, so decimal over
        // These shouldn't be changing
        public const float RATING_PER_HEALTH        = 14.00f; //14 Health per 1 STA;
        public const float RATING_PER_MANA          = 15.00f; //15 Mana per 1 INT;
        public const float BLOCKVALUE_PER_STR       =  2.00f;
        // These have not been provided Cata values yet, some could be removed as no longer valid
        //public const float LEVEL_85_COMBATRATING_MODIFIER      = 3.2789987789987789987789987789988f;
        public const float RATING_PER_RESILIENCE =      9520.6611570247933884297520661157f;
        public const float RATING_PER_DODGEPARRYREDUC          = 0.0025f; //4 Exp per 1% Dodge/Parry Reduction;
        public const float LEVEL_AVOIDANCE_MULTIPLIER          = 0.20f;

        // Attack Table for players attacking mobs                                           85       86        87      88
        public static readonly float[] WHITE_MISS_CHANCE_CAP                = new float[] { 0.0500f, 0.0520f, 0.0540f, 0.0800f };
        // MoP change // public static readonly float[] WHITE_MISS_CHANCE_CAP               = new float[] { 0.0300f, 0.0450f, 0.0600f, 0.0750f };
        public static readonly float[] WHITE_MISS_CHANCE_CAP_DW             = new float[] { 0.2400f, 0.2420f, 0.2440f, 0.2700f }; //  WHITE_MISS_CHANCE_CAP + 19%
        // MoP change (guess) // public static readonly float[] WHITE_MISS_CHANCE_CAP_DW          = new float[] { 0.2200f, 0.2350f, 0.2500f, 0.2650f }; //  WHITE_MISS_CHANCE_CAP + 19%
        
        //public static readonly float[] WHITE_MISS_CHANCE_CAP_BEHIND       = WHITE_MISS_CHANCE_CAP;
        //public static readonly float[] WHITE_MISS_CHANCE_CAP_DW_BEHIND    = WHITE_MISS_CHANCE_CAP_DW;
        public static readonly float[] YELLOW_MISS_CHANCE_CAP               = WHITE_MISS_CHANCE_CAP;
        //public static readonly float[] YELLOW_MISS_CHANCE_CAP_BEHIND      = WHITE_MISS_CHANCE_CAP_BEHIND;

        public static readonly float[] WHITE_DODGE_CHANCE_CAP               = new float[] { 0.0500f, 0.0550f, 0.0600f, 0.0650f }; //  6.5%
        // MoP change // public static readonly float[] WHITE_DODGE_CHANCE_CAP              = new float[] { 0.0300f, 0.0450f, 0.0600f, 0.0750f };
        //public static readonly float[] WHITE_DODGE_CHANCE_CAP_BEHIND      = WHITE_DODGE_CHANCE_CAP; // 6.5% Attacks from behind *can* be dodged
        public static readonly float[] YELLOW_DODGE_CHANCE_CAP              = WHITE_DODGE_CHANCE_CAP;
        //public static readonly float[] YELLOW_DODGE_CHANCE_CAP_BEHIND     = WHITE_DODGE_CHANCE_CAP_BEHIND;

        public static readonly float[] WHITE_PARRY_CHANCE_CAP               = new float[] { 0.0500f, 0.0550f, 0.0600f, 0.14f }; // 14%
        // MoP change // public static readonly float[] WHITE_PARRY_CHANCE_CAP              = new float[] { 0.0300f, 0.0450f, 0.0600f, 0.0750f };
        //public static readonly float[] WHITE_PARRY_CHANCE_CAP_BEHIND      = new float[] { 0.0000f, 0.0000f, 0.0000f, 0.0000f }; //  0% Attacks from behind can't be parried
        public static readonly float[] YELLOW_PARRY_CHANCE_CAP              = WHITE_PARRY_CHANCE_CAP;
        //public static readonly float[] YELLOW_PARRY_CHANCE_CAP_BEHIND     = WHITE_PARRY_CHANCE_CAP_BEHIND;

        public static readonly float[] WHITE_GLANCE_CHANCE_CAP              = new float[] { 0.1000f, 0.1500f, 0.2000f, 0.2400f }; // 25%
        //public static readonly float[] WHITE_GLANCE_CHANCE_CAP_BEHIND     = WHITE_GLANCE_CHANCE_CAP;
        public static readonly float[] YELLOW_GLANCE_CHANCE_CAP             = new float[] { 0.0000f, 0.0000f, 0.0000f, 0.0000f }; //  0% Yellows don't glance
        //public static readonly float[] YELLOW_GLANCE_CHANCE_CAP_BEHIND    = YELLOW_GLANCE_CHANCE_CAP;

        public static readonly float[] WHITE_BLOCK_CHANCE_CAP               = new float[] { 0.0500f, 0.0520f, 0.0540f, 0.0650f }; //  6.5%
        // MoP change // public static readonly float[] WHITE_BLOCK_CHANCE_CAP              = new float[] { 0.0300f, 0.0450f, 0.0600f, 0.0750f };
        //public static readonly float[] WHITE_BLOCK_CHANCE_CAP_BEHIND      = new float[] { 0.0000f, 0.0000f, 0.0000f, 0.0000f }; //  0% Attacks from behind can't be blocked
        public static readonly float[] YELLOW_BLOCK_CHANCE_CAP              = WHITE_BLOCK_CHANCE_CAP;
        //public static readonly float[] YELLOW_BLOCK_CHANCE_CAP_BEHIND     = WHITE_BLOCK_CHANCE_CAP_BEHIND;

        public static readonly float[] SPELL_MISS_CHANCE_CAP                = new float[] { 0.0400f, 0.0500f, 0.0600f, 0.1700f };
        // MoP change // public static readonly float[] SPELL_MISS_CHANCE_CAP                = new float[] { 0.0600f, 0.0900f, 0.1200f, 0.1500f };

        /// <summary>
        /// You need to *add* this to your current crit value as it's a negative number.
        /// <para>[85: 0, 86: -0.006, 87: -0.012, 88: -0.048]</para>
        /// </summary>
        public static readonly float[] NPC_LEVEL_CRIT_MOD                   = new float[] { -0.0000f, -0.0060f, -0.0120f, -0.0480f }; //  -4.8%

        // http://elitistjerks.com/f75/t110187-cataclysm_mage_simulators_formulators/p4/#post1834778
        // Level+3 has now been confirmed as being -1.8% (-0.0180)
        /// <summary>
        /// You need to *add* this to your current crit value as it's a negative number.
        /// <para>[85: 0, 86: -0.002625, 87: -0.00525, 88: -0.0180]</para>
        /// <para>Note: Level+1 and Level+2 values are just guesstimates based on trends
        /// from NPC_LEVEL_CRIT_MOD. We don't currently have solid values for these.</para>
        /// </summary>
        public static readonly float[] NPC_LEVEL_SPELL_CRIT_MOD             = new float[] { -0.0000f, -0.002625f, -0.00525f, -0.0180f }; //  -1.8%

        //source: http://code.google.com/p/simulationcraft/source/browse/branches/cataclysm/engine/sc_target.cpp
        public static readonly float[] NPC_ARMOR                            = new float[] { 11161f, 11441f, 11682f, 11977f };

        // Same for all classes
        public const float INT_PER_SPELLCRIT = 648.91f;
        public const float REGEN_CONSTANT = 0.003345f;

        /// <summary>
        /// Source: http://elitistjerks.com/f15/t29453-combat_ratings_level_85_cataclysm/
        /// </summary>
        public static readonly float[] AGI_PER_PHYSICALCRIT = { 0.0f, // CharacterClass starts at 1
            243.60f, //3.905f * 62.500f,  // Warrior 1
            203.08f, //3.905f * 52.083f,  // Paladin 2
            324.85f, //3.905f * 83.333f,  // Hunter 3
            324.72f, //3.905f * 83.333f,  // Rogue 4
            203.46f, //3.905f * 52.083f,  // Priest 5
            243.70f, //3.905f * 62.500f,  // Death Knight 6
            324.88f, //3.905f * 83.333f,  // Shaman 7
            199.87f, //3.905f * 51.0204f, // Mage 8
            197.08f, //3.905f * 50.505f,  // Warlock 9
              0.00f, //3.905f *  0.000f,  // Empty 10
            324.85f, //3.905f * 83.333f,  // Druid 11
        };

        /// <summary>
        /// Source: http://elitistjerks.com/f15/t29453-combat_ratings_level_85_cataclysm/
        /// </summary>
        public static readonly float[] AGI_PER_DODGE = { 0.0f, // Starts at 0
            0, // Patch 4.2 removed Agility to Dodge for Warriors //5.309f * 84.74576271f, // Warrior 1
            0, // Patch 4.2 removed Agility to Dodge for Paladins //5.309f * 59.88023952f, // Paladin 2
            439.99947200f, //5.309f * 86.20689655f, // Hunter 3
            243.51637648f, //5.309f * 47.84688995f, // Rogue 4
            304.00034048f, //5.309f * 59.88023952f, // Priest 5
            0, // Patch 4.2 removed Agility to Dodge for DKs //5.309f * 84.74576271f, // Death Knight 6
            304.00034048f, //5.309f * 59.88023952f, // Shaman 7
            300.16238785f, //5.309f * 58.82352941f, // Mage 8
            304.35470715f, //5.309f * 59.88023952f, // Warlock 9
              0.00000000f, //5.309f *  0.0f,        // Empty 10
            243.58281085f, //5.309f * 47.84688995f, // Druid 11
        };

        public static readonly float[] DR_COEFFIENT = { 0.0f, // Starts at 0
            0.9560f, // Warrior 1
            0.9560f, // Paladin 2
            0.9880f, // Hunter 3
            0.9880f, // Rogue 4
            0.9530f, // Priest 5
            0.9560f, // Death Knight 6
            0.9880f, // Shaman 7
            0.9530f, // Mage 8
            0.9530f, // Warlock 9
            0.0f,    // Empty 10
            0.9720f, // Druid 11
        };

        // This is the cap value for DODGE PERCENTAGE.
        public static readonly float[] CAP_DODGE = { 0.0f, // Starts at 0
             65.631440f, // Warrior 1
             65.631440f, // Paladin 2
            145.560408f, // Hunter 3
            145.560408f, // Rogue 4
            150.375940f, // Priest 5
             65.631440f, // Death Knight 6
            145.560408f, // Shaman 7
            150.375940f, // Mage 8
            150.375940f, // Warlock 9
              0f,        // Empty 10
            116.890707f, // Druid 11
        };

        /// <summary>
        /// This is the 1/CAP_DODGE to cut down the ammount of math going on.
        /// </summary>
        public static readonly float[] CAP_DODGE_INV = { 0.0f, // Starts at 0
            0.01523660f, // Warrior 1
            0.01523660f, // Paladin 2
            0.00687000f, // Hunter 3
            0.00687000f, // Rogue 4
            0.00665000f, // Priest 5
            0.01523660f, // Death Knight 6
            0.00687000f, // Shaman 7
            0.00665000f, // Mage 8
            0.00665000f, // Warlock 9
            0f,          // Empty 10
            0.00855500f, // Druid 11
        };

        // This is the cap value for PARRY PERCENTAGE.
        public static readonly float[] CAP_PARRY = { 0.0f, // Starts at 0
             65.631440f, // Warrior 1
             65.631440f, // Paladin 2
            145.560408f, // Hunter 3
            145.560408f, // Rogue 4
              0f,        // Priest 5
             65.631440f, // Death Knight 6
            145.560408f, // Shaman 7
              0f,        // Mage 8
              0f,        // Warlock 9
              0f,        // Empty 10
              0f,        // Druid 11
        };

        /// <summary>
        /// This is the 1/CAP_PARRY to cut down the amount of math going on.
        /// And prevent divide by 0 errors.
        /// </summary>
        public static readonly float[] CAP_PARRY_INV = { 0.0f, // Starts at 0
            0.01523660f, // Warrior 1
            0.01523660f, // Paladin 2
            0.00687000f, // Hunter 3
            0.00687000f, // Rogue 4
            0f,          // Priest 5
            0.01523660f, // Death Knight 6
            0.00687000f, // Shaman 7
            0f,          // Mage 8
            0f,          // Warlock 9
            0f,          // Empty 10
            0f,          // Druid 11
        };

        /// <summary>This is the cap value for MISS PERCENTAGE on NPC attacks against a Player</summary>
        public static readonly float[] CAP_MISSED = { 0.0f, // Starts at 0
            16f, // Warrior 1
            16f, // Paladin 2
             0f, // Hunter 3
             0f, // Rogue 4
             0f, // Priest 5
            16f, // Death Knight 6
             0f, // Shaman 7
             0f, // Mage 8
             0f, // Warlock 9
             0f, // Empty 10
             0f, // Druid 11
        };

        #endregion

        #region Functions for Plain Rating Conversions

        public static float GetHealthFromStamina(float Rating, CharacterClass Class) { return GetHealthFromStamina(Rating); }
        /// <summary>
        /// Returns a Value (1000 = 1000 extra Health)
        /// </summary>
        /// <param name="Rating">Stamina</param>
        /// <returns>A Value (1000 = 1000 extra Health)</returns>
        public static float GetHealthFromStamina(float Rating) {
            return Rating <= 20 ? Rating : (Rating - 20) * RATING_PER_HEALTH + 20; // first 20 stamina is 1 health
        }

        public static float GetManaFromIntellect(float Rating, CharacterClass Class) { return GetManaFromIntellect(Rating); }
        /// <summary>
        /// Returns a Value (1000 = 1000 extra Mana)
        /// </summary>
        /// <param name="Rating">Intellect</param>
        /// <returns>A Value (1000 = 1000 extra Mana)</returns>
        public static float GetManaFromIntellect(float Rating)
        {
            return Rating <= 20 ? Rating : (Rating - 20) * RATING_PER_MANA + 20; // first 20 intellect is 1 mana
        }

        public static float GetBlockFromRating(float Rating, CharacterClass Class) { return GetBlockFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% added chance to Block)
        /// </summary>
        /// <param name="Rating">Block Rating</param>
        /// <returns>A Percentage (0.05 = 5% added chance to Block)</returns>
        public static float GetBlockFromRating(float Rating)
        {
            return Rating / RATING_PER_BLOCK;
        }

        public static float GetBlockValueFromStrength(float str, CharacterClass Class) { return GetBlockValueFromStrength(str); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% added chance to Block)
        /// </summary>
        /// <param name="Rating">Block Rating</param>
        /// <returns>A Percentage (0.05 = 5% added chance to Block)</returns>
        public static float GetBlockValueFromStrength(float str) { return str / BLOCKVALUE_PER_STR; }

        public static float GetDodgeFromRating(float Rating, CharacterClass Class) { return GetDodgeFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Dodge)
        /// </summary>
        /// <param name="Rating">Dodge Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Dodge)</returns>
        public static float GetDodgeFromRating(float Rating)
        {
            return Rating / RATING_PER_DODGE;
        }

        public static float GetMasteryFromRating(float Rating, CharacterClass Class) { return GetMasteryFromRating(Rating); }
        /// <summary>
        /// Returns a Value (6.34 = 6.34 extra Mastery)
        /// </summary>
        /// <param name="Rating">Mastery Rating</param>
        /// <returns>A Value (6.34 = 6.34 extra Mastery)</returns>
        public static float GetMasteryFromRating(float Rating) { return Rating / RATING_PER_MASTERY; }
        
        public static float GetExpertiseFromRating(float Rating, CharacterClass Class) { return GetExpertiseFromRating(Rating); }
        /// <summary>
        /// Returns a Value (6.34 = 6.34 extra Expertise)
        /// </summary>
        /// <param name="Rating">Expertise Rating</param>
        /// <returns>A Value (6.34 = 6.34 extra Expertise)</returns>
        public static float GetExpertiseFromRating(float Rating) { return Rating / RATING_PER_EXPERTISE; }
        public static float GetRatingFromExpertise(float value, CharacterClass Class) { return GetRatingFromExpertise(value); }
        /// <summary>
        /// Returns a Value (6.34 = 6.34 extra Expertise)
        /// </summary>
        /// <param name="Rating">Expertise Rating</param>
        /// <returns>A Value (6.34 = 6.34 extra Expertise)</returns>
        public static float GetRatingFromExpertise(float value) { return value * RATING_PER_EXPERTISE; }

        public static float GetDodgeParryReducFromExpertise(float Rating, CharacterClass Class) { return GetDodgeParryReducFromExpertise(Rating); }
        /// <summary>
        /// Returns a Percentage (1.00 = 1% extra Dodge/Parry Reduction)
        /// </summary>
        /// <param name="Rating">Expertise</param>
        /// <returns>A Percentage (1.00 = 1% extra Dodge/Parry Reduction)</returns>
        public static float GetDodgeParryReducFromExpertise(float Rating) { return Rating * RATING_PER_DODGEPARRYREDUC; }
        public static float GetExpertiseFromDodgeParryReduc(float value, CharacterClass Class) { return GetExpertiseFromDodgeParryReduc(value); }
        /// <summary>
        /// Returns a Value (1 = 1 extra Expertise)
        /// </summary>
        /// <param name="Rating">DodgeParryReduc %</param>
        /// <returns>A Value (1 = 1 extra Expertise)</returns>
        public static float GetExpertiseFromDodgeParryReduc(float value) { return value / RATING_PER_DODGEPARRYREDUC; }

        public static float GetParryFromRating(float Rating, CharacterClass Class) { return GetParryFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Parry)
        /// </summary>
        /// <param name="Rating">Parry Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Parry)</returns>
        public static float GetParryFromRating(float Rating) { return Rating / RATING_PER_PARRY; }

        public static float GetCritFromRating(float Rating, CharacterClass Class) { return GetPhysicalCritFromRating(Rating); }
        public static float GetCritFromRating(float Rating) { return GetPhysicalCritFromRating(Rating); }
        public static float GetPhysicalCritFromRating(float Rating, CharacterClass Class) { return GetPhysicalCritFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Rating">Crit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetPhysicalCritFromRating(float Rating) { return Rating / RATING_PER_PHYSICALCRIT; }

        public static float GetRatingFromCrit(float Rating, CharacterClass Class) { return GetRatingFromPhysicalCrit(Rating); }
        public static float GetRatingFromCrit(float Rating) { return GetRatingFromPhysicalCrit(Rating); }
        public static float GetRatingFromPhysicalCrit(float Rating, CharacterClass Class) { return GetRatingFromPhysicalCrit(Rating); }
        /// <summary>
        /// Returns a Value (1 = 1 Crit Rating)
        /// </summary>
        /// <param name="Rating">Crit Percent</param>
        /// <returns>A Value (1 = 1 Crit Rating)</returns>
        public static float GetRatingFromPhysicalCrit(float Rating) { return Rating * RATING_PER_PHYSICALCRIT; }

        // Returns a Percentage
        public static float GetHasteFromRating(float Rating, CharacterClass Class) { return GetPhysicalHasteFromRating(Rating, Class); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Haste)
        /// </summary>
        /// <param name="Rating">Haste Rating</param>
        /// <param name="Class">CharacterClass</param>
        /// <returns>A Percentage (0.05 = 5% extra Haste)</returns>
        public static float GetPhysicalHasteFromRating(float Rating, CharacterClass Class) {
            //Removed in Cata (Patch 4.0.3)
            /*if (Class == CharacterClass.DeathKnight
                || Class == CharacterClass.Druid
                || Class == CharacterClass.Paladin
                || Class == CharacterClass.Shaman)
                return Rating / RATING_PER_PHYSICALHASTE * 1.3f;*/    // Patch 3.1: Hybrids gain 30% more Physical Haste from Haste Rating.
            return Rating / RATING_PER_PHYSICALHASTE;
        }

        public static float GetHitFromRating(float Rating, CharacterClass Class) { return GetPhysicalHitFromRating(Rating); }
        public static float GetHitFromRating(float Rating) { return GetPhysicalHitFromRating(Rating); }
        public static float GetPhysicalHitFromRating(float Rating, CharacterClass Class) { return GetPhysicalHitFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Hit)</returns>
        public static float GetPhysicalHitFromRating(float Rating){return Rating / RATING_PER_PHYSICALHIT;}
        public static float GetRatingFromHit(float value, CharacterClass Class) { return GetRatingFromHit(value); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Hit)</returns>
        public static float GetRatingFromHit(float value) { return value * RATING_PER_PHYSICALHIT; }

        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Hit)</returns>
        public static float GetRatingFromSpellHit(float value, CharacterClass Class) { return GetRatingFromSpellHit(value); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Hit)</returns>
        public static float GetRatingFromSpellHit(float value) { return value * RATING_PER_SPELLHIT; }
        
        public static float GetSpellCritFromRating(float Rating, CharacterClass Class) { return GetSpellCritFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Rating">Crit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetSpellCritFromRating(float Rating)
        {
            return Rating / RATING_PER_SPELLCRIT;
        }

        public static float GetSpellHasteFromRating(float Rating, CharacterClass Class) { return GetSpellHasteFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Haste)
        /// </summary>
        /// <param name="Rating">Haste Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra Haste)</returns>
        public static float GetSpellHasteFromRating(float Rating)
        {
            return Rating / RATING_PER_SPELLHASTE;
        }

        public static float GetSpellHitFromRating(float Rating, CharacterClass Class) { return GetSpellHitFromRating(Rating); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Hit)
        /// </summary>
        /// <param name="Rating">Hit Rating</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Hit)</returns>
        public static float GetSpellHitFromRating(float Rating)
        {
            return Rating / RATING_PER_SPELLHIT;
        }

        public static float GetSpellCritFromIntellect(float Intellect, CharacterClass Class) { return GetSpellCritFromIntellect(Intellect); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Intellect">Intellect</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetSpellCritFromIntellect(float Intellect)
        {
            return Intellect / INT_PER_SPELLCRIT * 0.01f;
        }

        public static float GetCritFromAgility(float Agility, CharacterClass Class) { return GetPhysicalCritFromAgility(Agility, Class); }
        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra chance to Crit)
        /// </summary>
        /// <param name="Agility">Agility</param>
        /// <param name="Class">CharacterClass</param>
        /// <returns>A Percentage (0.05 = 5% extra chance to Crit)</returns>
        public static float GetPhysicalCritFromAgility(float Agility, CharacterClass Class)
        {
            return Agility / AGI_PER_PHYSICALCRIT[(int)Class] * 0.01f;
        }

        /// <summary>
        /// Returns a Percentage (0.05 = 5% extra Dodge)
        /// </summary>
        /// <param name="Agility">Agility</param>
        /// <param name="Class">CharacterClass</param>
        /// <returns>A Percentage (0.05 = 5% extra Dodge)</returns>
        public static float GetDodgeFromAgility(float Agility, CharacterClass Class)
        {
            if (AGI_PER_DODGE[(int)Class] > 0) 
                return Agility / AGI_PER_DODGE[(int)Class] * 0.01f;
            return 0;
        }

        public static float GetSpiritRegenSec(float Spirit, float Intellect, CharacterClass Class) { return GetSpiritRegenSec(Spirit, Intellect); }
        /// <summary>
        /// Returns a Number, How much mana is gained each Second. (Multiply by 5 to get MP5)
        /// </summary>
        /// <param name="Spirit">Spirit</param>
        /// <param name="Intellect">Intellect</param>
        /// <returns>A Number, How much mana is gained each Second. (Multiply by 5 to get MP5)</returns>
        public static float GetSpiritRegenSec(float Spirit, float Intellect)
        {
            return 0.001f + Spirit * REGEN_CONSTANT * (float)Math.Sqrt(Intellect);
        }

        /// <summary>
        /// Returns the % Damage reduction based on amount of Resilience
        /// </summary>
        /// <param name="resilience">Amount of Resilience</param>
        /// <returns>% Damage reduction on attacks from other players</returns>
        public static float GetDamageReductionFromResilience(float resilience)
        {
            return resilience / RATING_PER_RESILIENCE;
        }


        public static float ApplyMultiplier(float baseValue, float multiplier)
        {
            return (baseValue * (1f + multiplier));
        }
        public static float ApplyInverseMultiplier(float baseValue, float multiplier)
        {
            return (baseValue * (1f - multiplier));
        }
        #endregion

        #region Functions for More complex things.

        /// <summary>Originally from Bear. This should be updated once per expansion</summary>
        public const float MitigationScaler         = 78591f;
        public const double SurvivalScalerBase      = 0.1574901d; // fourToTheNegativeFourThirds = Math.Pow(4d, -4d / 3d)
        public const double SurvivalScalerTopRight  = 0.6299605d; // topRight = Math.Pow(fourToTheNegativeFourThirds, 1d / 4d)

        /// <summary>
        /// Returns how much physical damage is reduced from Armor. (0.095 = 9.5% reduction)
        /// </summary>
        /// <remarks>
        /// * http://forums.worldofwarcraft.com/thread.html?topicId=16473618356&sid=1&pageNo=4 post 77.<br/>
        /// * Ghostcrawler vs theorycraft.<br/>
        /// * http://elitistjerks.com/f15/t29453-combat_ratings_level_85_cataclysm/p24/#post1841717<br/>
        /// </remarks>
        /// <param name="AttackerLevel">Level of Attacker</param>
        /// <param name="TargetArmor">Armor of Target</param>
        /// <param name="ArmorIgnoreDebuffs">Armor reduction on target as result of Debuffs (Sunder/Fearie Fire) These are Multiplied.</param>
        /// <param name="ArmorIgnoreBuffs">Armor reduction buffs on player (Mace Spec, Battle Stance, etc) These are Added.</param>
        /// <returns>How much physical damage is reduced from Armor. (0.095 = 9.5% reduction)</returns>
        public static float GetArmorDamageReduction(int AttackerLevel, float TargetArmor, float ArmorIgnoreDebuffs, float ArmorIgnoreBuffs)
        {
            return GetArmorDamageReduction(AttackerLevel, (int)POSSIBLE_LEVELS.LVLP3, TargetArmor, ArmorIgnoreDebuffs, ArmorIgnoreBuffs);
        }

        /// <summary>
        /// Returns how much physical damage is reduced from Armor. (0.095 = 9.5% reduction)
        /// <para>This function used to take in ArP Rating but that was removed from the game in Cata</para>
        /// </summary>
        /// <remarks>
        /// * http://forums.worldofwarcraft.com/thread.html?topicId=16473618356&sid=1&pageNo=4 post 77.<br/>
        /// * Ghostcrawler vs theorycraft.<br/>
        /// * http://elitistjerks.com/f15/t29453-combat_ratings_level_85_cataclysm/p24/#post1841717<br/>
        /// </remarks>
        /// <param name="AttackerLevel">Level of Attacker</param>
        /// <param name="TargetLevel">Level of Target</param>
        /// <param name="TargetArmor">Armor of Target</param>
        /// <param name="ArmorIgnoreDebuffs">Armor reduction on target as result of Debuffs (Sunder/Fearie Fire) These are Multiplied.</param>
        /// <param name="ArmorIgnoreBuffs">Armor reduction buffs on player (Mace Spec, Battle Stance, etc) These are Added.</param>
        /// <returns>How much physical damage is reduced from Armor. (0.095 = 9.5% reduction)</returns>
        public static float GetArmorDamageReduction(int AttackerLevel, int TargetLevel, float TargetArmor, float ArmorIgnoreDebuffs, float ArmorIgnoreBuffs)
        {
            #region Rawr WotLK
            /* This is what we were using before. This formula is now out of date
            float ArmorConstant = 400 + 85 * TargetLevel + 4.5f * 85 * (TargetLevel - 59);
            TargetArmor *= (1f - ArmorIgnoreDebuffs);
            float ArPCap = Math.Min((TargetArmor + ArmorConstant) / 3f, TargetArmor);
            TargetArmor -= ArPCap * Math.Min(1f, ArmorIgnoreBuffs);

            return 1f - ArmorConstant / (ArmorConstant + TargetArmor);
            */
            #endregion
            #region SimCraft Cata Trunk
            /*
            *************************
            * The Armor Function
            *************************
            double action_t::armor() SC_CONST
            {
                target_t* t = target;

                double adjusted_armor =  t -> base_armor(); // this calls to the NPC target level armor list, at 88 it's 11977 armor, we are using the same values in Rawr
                double armor_reduction = std::max( t -> debuffs.sunder_armor -> stack() * 0.04,
                                         std::max( t -> debuffs.faerie_fire  -> stack() * t -> debuffs.faerie_fire -> value(),
                                                   t -> debuffs.expose_armor -> value() ) );
                // TO-DO: Also need to add the Hunter Pets Raptor and Serpent

                armor_reduction += t -> debuffs.shattering_throw -> stack() * 0.20;

                adjusted_armor *= 1.0 - armor_reduction;

                return adjusted_armor;
            }
            *************************
            * The Restance Table
            *************************
            else if ( school == SCHOOL_PHYSICAL )
            {
                double temp_armor = armor();
                resist = temp_armor / ( temp_armor + player -> armor_coeff );

                if ( resist < 0.0 )
                    resist = 0.0;
                else if ( resist > 0.75 )
                    resist = 0.75;

            */
            #endregion
            #region LibStatLogic-1.2.lua file in Rating Buster
            /*
            local levelModifier = attackerLevel
            if ( levelModifier > 80 ) then
                levelModifier = levelModifier + (4.5 * (levelModifier - 59)) + (20 * (levelModifier - 80));
            elseif ( levelModifier > 59 ) then
                levelModifier = levelModifier + (4.5 * (levelModifier - 59))
            end
            local temp = armor / (85 * levelModifier + 400)
            local armorReduction = temp / (1 + temp)
            -- caps at 0.75
            if armorReduction > 0.75 then
                armorReduction = 0.75
            end
             */
            #endregion
            #region Rawr Cata
            float ArmorConstant = AttackerLevel + (4.5f * (AttackerLevel - 59f)) + (20f * (AttackerLevel - 80f));
            TargetArmor *= (1f - ArmorIgnoreDebuffs);
            float ArPCap = Math.Min((TargetArmor + ArmorConstant) / 3f, TargetArmor);
            TargetArmor -= ArPCap * Math.Min(1f, ArmorIgnoreBuffs);
            float temp = TargetArmor / (85 * ArmorConstant + 400);
            float armorReduction = temp / (1f + temp);
            if (armorReduction > 0.75f) { armorReduction = 0.75f; }
            return armorReduction;
            #endregion
        }

        /// <summary>
        /// Gets the amount of physical damage reduction provided by armor, from an attacker.
        /// In the case of modeling a tank's damage reduction, Attacker would be the boss, Target would be the tank.
        /// </summary>
        /// <param name="AttackerLevel">The level of the creature making the attack.</param>
        /// <param name="TargetArmor">The armor of the creature being attacked.</param>
        /// <returns>How much physical damage is reduced from Armor. (0.095 = 9.5% reduction)</returns>
        public static float GetDamageReductionFromArmor(int AttackerLevel, float TargetArmor)
        {
            return Math.Min(0.75f, (TargetArmor) / (TargetArmor + 2167.5f * AttackerLevel - 158167.5f));
        }

        /// <summary>
        /// Returns the chance to miss (0.09 = 9% chance to miss)<br/>
        /// http://www.wow-dark-destiny.com/images/spellhit.png
        /// </summary>
        /// <param name="LevelDelta">Attacker Level - Defender Level</param>
        /// <param name="bPvP">Set to True if Player vs Player combat</param>
        /// <returns>The chance to miss (0.09 = 9% chance to miss)</returns>
        public static float GetSpellMiss(int LevelDelta, bool bPvP)
        {
            if (-LevelDelta <= 2)
                return (float)Math.Min(0.0f, (-LevelDelta + 4) * 0.01f);

            if (-LevelDelta > 2)
                if (bPvP)
                    return (float)Math.Min(0.62f, (-LevelDelta * 7 - 8) * 0.01f);

            return (float)Math.Min(0.94f, (-LevelDelta * 11 - 16) * 0.01f);
        }

        private static float AttackerResistancePenalty(int LevelDelta)
        {
            if (LevelDelta == 1)
                return 0f;
            else if (LevelDelta == 2)
                return 0f;
            else if (LevelDelta == 3)
                return 95f;
            return 0f;
        }

        /// <summary>
        /// Returns a Percent giving Average Magical Damage Resisted (0.16 = 16% Resisted)
        /// </summary>
        /// <param name="AttackerLevel">Level of the Attacker</param>
        /// <param name="TargetLevel">Level of the Target</param>
        /// <param name="TargetResistance">Targets Resistance</param>
        /// <param name="AttackerSpellPenetration">Attackers Spell Penetration</param>
        /// <returns>A Percent giving Average Magical Damage Resisted (0.16 = 16% Resisted)</returns>
        public static float GetAverageResistance(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {
            float ActualResistance = (float)Math.Max(0f, TargetResistance - AttackerSpellPenetration);
            return ActualResistance / (AttackerLevel * 5f + AttackerResistancePenalty(AttackerLevel - TargetLevel) + ActualResistance)
                   /*+ 0.02f * (float)Math.Max(0, TargetLevel - AttackerLevel)*/; // apparently level-based partial resists were removed in Cataclysm
        }

        /// <summary>
        /// Returns a Table giving the chance to fall within a resistance slice cutoff.
        /// The table is float[11] Table, where Table[0] is 0% resisted, and Table[10] is 100% resisted.
        /// Each Table entry gives how much chance to roll into that slice.
        /// So if Table[1] contains 0.165, that means you have a 16.5% chance to resist 10% damage.
        /// </summary>
        /// <param name="AttackerLevel">Level of the Attacker</param>
        /// <param name="TargetLevel">Level of the Target</param>
        /// <param name="TargetResistance">Targets Resistance</param>
        /// <param name="AttackerSpellPenetration">Attackers Spell Penetration</param>
        /// <returns>A Table giving the chance to fall within a resistance slice cutoff.</returns>
        public static float[] GetResistanceTable(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {                      //   00% 10% 20% 30% 40% 50% 60% 70% 80% 90% 100%
            float[] ResistTable = { 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f };
            float AverageResistance = GetAverageResistance(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);

            for (int x = -1; x < 11; x++)
            {   // Build Table
                float ResistSlice = (float)Math.Max(0f, 0.5f - 2.5f * (float)Math.Abs(0.1f * x - AverageResistance));
                if (x == -1)
                {   // Adjust 0% and 10% for "negative" resists.
                    ResistTable[0] += 2f * ResistSlice;
                    ResistTable[1] -= 1f * ResistSlice;
                }
                else
                    ResistTable[x] += ResistSlice;
            }
            return ResistTable;
        }

        /// <summary>
        /// Returns a String version of the Table giving the chance to fall within a resistance slice cutoff.
        /// Useful as part of a tooltip
        /// </summary>
        /// <param name="AttackerLevel">Level of the Attacker</param>
        /// <param name="TargetLevel">Level of the Target</param>
        /// <param name="TargetResistance">Targets Resistance</param>
        /// <param name="AttackerSpellPenetration">Attackers Spell Penetration</param>
        /// <returns>A string version of a Table giving the chance to fall within a resistance slice cutoff.</returns>
        public static string GetResistanceTableString(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {
            int count;
            string tipResist = string.Empty;
            tipResist = Math.Round(StatConversion.GetAverageResistance(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration) * 100.0f, 2).ToString() + "% average resistance \n";
            tipResist += "% Resisted     Occurance";

            float[] ResistTable = StatConversion.GetResistanceTable(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);
            for (count = 0; count < 10; count++)
            {
                if (ResistTable[count] > 0)
                {
                    tipResist += "\n" + Math.Round(count * 10.0f, 1) + " % resisted   " + Math.Round(ResistTable[count] * 100.0f, 2) + "%";
                }
            }

            return tipResist;
        }

        /// <summary>
        /// Returns the Minimum amount of Spell Damage will be resisted. (0.2 = Anything below 20% is always resisted)
        /// If this returns 0.0, that means you will always have a chance to take full damage from spells.
        /// If this returns 0.1, that means you will never take full damage from spells, and minumum you will take is 10% reduction.
        /// </summary>
        /// <param name="AttackerLevel">Level of Attacker</param>
        /// <param name="TargetLevel">Level of Target</param>
        /// <param name="TargetResistance">Target Resistance</param>
        /// <param name="AttackerSpellPenetration">Attacker Spell Penetration</param>
        /// <returns>The Minimum amount of Spell Damage will be resisted. (0.2 = Anything below 20% is always resisted)</returns>
        public static float GetMinimumResistance(int AttackerLevel, int TargetLevel,
            float TargetResistance, float AttackerSpellPenetration)
        {
            float[] ResistTable = GetResistanceTable(AttackerLevel, TargetLevel, TargetResistance, AttackerSpellPenetration);

            for (int x = 0; x < 11; x++)
                if (ResistTable[x] > 0f)
                    return 0.1f * x;
            return 0f;
        }

        // Initial function taken from the ProtWarrior Module.
        // Then using table found on EJ:
        // http://elitistjerks.com/f31/t29453-combat_ratings_level_80_a/
        // creating updated Avoidance Chance w/ DR build in formula.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="character">Character in question.</param>
        /// <param name="stats">Stats object... total stats of the character.</param>
        /// <param name="avoidanceType">What type of hit is the target doing on the character?</param>
        /// <param name="TargetLevel">Level of the target being fought</param>
        /// <returns>A % value where .50 == 50%</returns>
        public static float GetDRAvoidanceChance(Character character, Stats stats, HitResult avoidanceType, int TargetLevel) { return GetDRAvoidanceChance(character, stats, avoidanceType, (uint)TargetLevel); }
        public static float GetDRAvoidanceChance(Character character, Stats stats, HitResult avoidanceType, uint TargetLevel)
        {
            /*
            float defSkill = stats.Defense;
            // Let's make sure we don't run off the bottom w/ a negative defense rating.
            stats.DefenseRating = Math.Max(stats.DefenseRating, 0f);
            float defSkillMod = (GetDefenseFromRating(stats.DefenseRating, character.Class) * DEFENSE_RATING_AVOIDANCE_MULTIPLIER);
            float baseAvoid = (defSkill - (TargetLevel * 5)) * DEFENSE_RATING_AVOIDANCE_MULTIPLIER;
            float modifiedAvoid = defSkillMod;
            float finalAvoid = 0f; // I know it breaks my lack of redundancy rule, but it helps w/ readability.
            int iClass = (int)character.Class;
            */

            float baseAvoid = (character.Level - TargetLevel) * LEVEL_AVOIDANCE_MULTIPLIER;
            float modifiedAvoid = 0.0f;
            float finalAvoid    = 0.0f;
            int iClass          = (int)character.Class;

            switch (avoidanceType)
            {
                case HitResult.Dodge:
                    if ((character.Class == CharacterClass.DeathKnight) || (character.Class == CharacterClass.Paladin) || (character.Class == CharacterClass.Warrior))
                        baseAvoid += (stats.Dodge * 100f);
                    else
                        baseAvoid += ((stats.Dodge + GetDodgeFromAgility(stats.BaseAgility, character.Class)) * 100f);
                    // Assuring we don't run off the bottom w/ negative dodge rating.
                    stats.DodgeRating = Math.Max(stats.DodgeRating, 0f);
                    modifiedAvoid += ((GetDodgeFromAgility((stats.Agility - stats.BaseAgility), character.Class) +
                                    GetDodgeFromRating(stats.DodgeRating)) * 100f);
                    modifiedAvoid = DRMath(CAP_DODGE_INV[iClass], DR_COEFFIENT[iClass], modifiedAvoid);
                    // Don't run off the bottom if we have negative dodge
                    finalAvoid = Math.Max(baseAvoid + modifiedAvoid, 0);
                    finalAvoid = Math.Min(finalAvoid, CAP_DODGE[iClass]);
                    break;
                case HitResult.Parry:
                    baseAvoid += stats.Parry * 100f;
                    // Assuring we don't run off the bottom w/ negative parry rating.
                    stats.ParryRating = Math.Max(stats.ParryRating, 0f);
                    if (character.Class == CharacterClass.DeathKnight) stats.ParryRating += (stats.Strength - BaseStats.GetBaseStats(character).Strength) * 0.25f;
                    modifiedAvoid += (GetParryFromRating(stats.ParryRating) * 100f);
                    modifiedAvoid = DRMath(CAP_PARRY_INV[iClass], DR_COEFFIENT[iClass], modifiedAvoid);
                    finalAvoid = Math.Max(baseAvoid + modifiedAvoid, 0);
                    finalAvoid = Math.Min(finalAvoid, CAP_PARRY[iClass]);
                    break;
                case HitResult.Miss:
                    // Base Miss rate according is 5%
                    // However, this can be talented up (e.g. Frigid Dreadplate, NE racial, etc.) 
                    baseAvoid += stats.Miss * 100f;
                    modifiedAvoid = DRMath( (1f/CAP_MISSED[iClass]), DR_COEFFIENT[iClass], modifiedAvoid );
                    // Factoring in the Miss Cap. 
                    modifiedAvoid = Math.Min(CAP_MISSED[iClass], modifiedAvoid);
                    finalAvoid = Math.Max(baseAvoid + modifiedAvoid, 0);
                    finalAvoid = Math.Min(finalAvoid, CAP_MISSED[iClass]);
                    break;
                case HitResult.Block:
                    // Base Block is 5%
                    baseAvoid += stats.Block * 100f;
                    // Assuring we don't run off the bottom w/ negative block rating.
                    stats.BlockRating = Math.Max(stats.BlockRating, 0f);
                    modifiedAvoid += (GetBlockFromRating(stats.BlockRating) * 100f);
                    finalAvoid = Math.Max(baseAvoid + modifiedAvoid, 0);
                    break;
                case HitResult.Crit:
                    // Resilience doesn't change crit chance anymore.
                    //modifiedAvoid -= (GetCritReductionFromResilience(stats.Resilience) * 100f);
                    finalAvoid = baseAvoid + modifiedAvoid;
                    break;
            }

            // Many of the base values are whole numbers, so need to get it back to decimal. 
            // May want to look at making this more consistant in the future.
            finalAvoid = finalAvoid / 100.0f;
            return finalAvoid;
        }
        /// <summary>
        /// StatPostDR =  1/(CAP_STAT_INV + COEF/StatPreDR)
        /// </summary>
        /// <param name="inv_cap">One of the CAP_STAT_INV values, appropriate for the class.</param>
        /// <param name="coefficient">One of the DR_COEF values, appropriate for the class.</param>
        /// <param name="valuePreDR">The value of the stat before DR are factored in.</param>
        /// <returns></returns>
        private static float DRMath(float inv_cap, float coefficient, float valuePreDR)
        {
            float DRValue = 0f;
            DRValue = 1f / (inv_cap + coefficient / valuePreDR);
            return DRValue;
        }
        #endregion
    }
}
