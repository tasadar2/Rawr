﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
    public partial class WelcomeWindow : ChildWindow
    {
        public WelcomeWindow()
        {
            InitializeComponent();
            //
            #if !SILVERLIGHT
                this.ResizeMode = System.Windows.ResizeMode.NoResize;
                this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
                this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
                this.WindowState = System.Windows.WindowState.Normal;
                this.ShowInTaskbar = false;
            #endif
            //
            SetUpTips();
            SetUpFAQ();
            SetUpPatchNotes();
            SetUpKI();
            //
            SetUpRecentCharsList();
        }

        #region Variables
        private Dictionary<string, string> TipStuff = new Dictionary<string, string>();
        private Dictionary<string, string> FAQStuff = new Dictionary<string, string>();
        private Dictionary<string, string> VNStuff = new Dictionary<string, string>();
        private Dictionary<string, string> KIStuff = new Dictionary<string, string>();
        #endregion

        // Set Up Information for display
        private void SetUpTips()
        {
TipStuff.Add(@"You can save talent builds by clicking the Save button, and then compare different builds in the Talent Specs chart.", "");
TipStuff.Add(@"You can use the All Buffs chart to find the best food, elixir, or flask for your character.", "");
TipStuff.Add(@"By marking the green diamond next to items, gems, and enchants, you indicate that you have them available to you. The Optimizer can use that information to find the best set of gear available to you, or to build a list of potential upgrades for you, and their upgrade values.", "");
TipStuff.Add(@"When using the Optimizer, you can use the Additional Constraints feature to enforce requirements such as being uncrittable, being hit capped, maintaining a certain level of survivability, a minimum haste %, and much much more.", "");
TipStuff.Add(@"You can use Batch Tools to figure out which spec of yours performs the best given your gear!", "");
TipStuff.Add(@"Rawr Batch Tools can help you find gear that's an upgrade for more than one of your specs!", "");
TipStuff.Add(@"The direct-upgrades chart will quickly show you where your current gear is lacking.", "");
TipStuff.Add(@"If you can never remember which class gives which buff, you can set an option to display the source of each buff in Tools : Options : General Settings.", "");
TipStuff.Add(@"You can select which language (English, French, German, Spanish, Russian) to view the item names in by selecting your language in Tools : Options : General Settings.", "");
TipStuff.Add(@"In addition to the gems you mark available, the Optimizer will also use any gems included in any enabled Gemming Template. You can disable that in Tools : Options : Optimizer Settings, if you prefer to manually choose what gems are available.", "");
TipStuff.Add(@"You can choose which gemming templates Rawr uses to gem items in the charts in Tools : Edit Gemming Templates.", "");
TipStuff.Add(@"Jewelcrafters can tell Rawr to use their Dragon's Eye gems by ticking the Jeweler gems section in Tools : Edit Gemming Templates.", "");
TipStuff.Add(@"You can force the Optimizer to use only a specific gemming of an item by CTRL-clicking the diamond in the gear list (it will turn blue).", "");
TipStuff.Add(@"You can force the Optimizer to use only a specific enchanting of an item by right-clicking the diamond and selecting the enchant (it will show a red dot).", "");
TipStuff.Add(@"Upgrade Lists provide a huge advantage over the simple comparison charts. The comparison charts will show you the values for individual items, while the Upgrade List will show you each item's upgrade including the optimal combination of items that you have available to go with that item. Use Tools : Optimizer : Build Upgrade List to get started!", "");
TipStuff.Add(@"If Rawr is valuing items for your caster character which focus on mana regen higher than you'd expect, you may be running out of mana. Ensure that you have all the appropriate buffs checked, especially for mana regen.", "");
TipStuff.Add(@"You can set an option in the Tools : Options menu to show or hide profession buffs that your character doesn't know.", "");
TipStuff.Add(@"You can view your character in 3D on Wowhead.com by using the menu off the Tools menu.", "");

            CB_Tips.Items.Add((String)"All");
            String[] arr = new String[TipStuff.Keys.Count];
            TipStuff.Keys.CopyTo(arr, 0);
            foreach (String a in arr) { CB_Tips.Items.Add(a); }
            CB_Tips.SelectedIndex = 0;
            CB_Tips_SelectedIndexChanged(null, null);
        }
        private void SetUpFAQ()
        {
FAQStuff.Add(
"Where can I set the Stat Weights in Rawr?",
@"Rawr absolutely, flat out, completely, in no way uses Stat Weights (the idea of assigning a Stat a value and using that to score gear, Eg- STR = 1.2 so an item with 20 STR would be value 24). Instead, each Stat is calculated into a modeling process that includes all of the class' abilities with respect to the character as a whole. The gear comparison then becomes the total character change that item would provide. DPS classes see DPS changes, Tanks see Survivability changes, and Healers see HPS changes. This is what sets Rawr apart from most other modeling programs."
);

FAQStuff.Add(
"Why isn't the Optimizer keeping my meta gem active?",
@"You need to set Enforce Gem Requirements to be enabled. See Gemmings for more details."
);

FAQStuff.Add(
"Enforce Gem Requirements is active, so why isn't Rawr forcing the gems to match the socket colors?",
@"Enforce Gem Requirements only does two things: ensure that the meta gem is activated, and ensure that any Unique or Unique-Equipped gems are, in fact, unique. The gemmings recommended may or may not match the socket bonuses, irregardless of this option's state. Rawr does not have any options to set to force socket bonuses to be activated, and instead recommends the best gemming possible, whether it's with the socket bonus, or ignoring it. See Gemmings for more details."
);

FAQStuff.Add(
"Why does the Optimizer try and equip two of my weapon when I only have one?",
@"The item is not Unique, so the Optimizer assumes that you simply have access to the item, as it does not know how many of a given item you have. To restrict the Optimizer to consider only one copy of a weapon, right-click the item, select Edit, and mark the item as Unique. This will prevent the Optimizer from putting the item in both MH and OH slots, as if you had two copies of the item. The same process can be used for rings and trinkets."
);

FAQStuff.Add(
"Why does a Troll's Berserking and an Orc's Bloodfury not show up in the Buffs list?",
@"Racial buffs are automatically added to the character stats, based on your race selection in the Character Definition area of the Stats pane."
);

FAQStuff.Add(
"Why does the Optimizer sometimes lower my DPS?",
@"The Optimizer operates on a Random Seed method, which is why it works at all. Sometimes it can't use that random seed to find a set that is better than what you are currently wearing. This is a known issue that we wish to hopefully find a solution for in the future. In the meantime, you can help the Optimizer to find the optimal setup by following these steps:
- Limit the number of potential items that you mark as available to the Optimizer. As the number of pieces of gear / gems / enchants rise, the number of potential solutions (results) the Optimizer can create increases exponentially. Periodically clean up your list of items that you know will perform as well as other options.
- Increase the Optimizer thoroughness by moving the slider on the Optimizer window to the right. While the Optimizer will take longer to run, it will be checking through more and more potential setups the higher the thoroughness is set.
- Make absolutely sure that everything on your current character is marked as available to the Optimizer. This includes gear, enchants, and gems. This should be mostly covered by a check that is run when you start the Optimizer, though we are currently refining the warning dialogue to be more descriptive (to tell you exactly what you are currently wearing is not available to the Optimizer)."
);

FAQStuff.Add(
"Why does the Optimizer sometimes just rearrange my Gems?",
@"In the more modern versions of Rawr, this issue should no longer exist."
);

FAQStuff.Add(
"Why is my Crit value so low, compared to the in-game character panel?",
@"Boss level relative to your own affects your actual chance to deal a critical strike. Targets that are three levels higher than your own (83, or Boss level) exhibit a 4.8 crit depression / conversion, which affects both white and yellow damage. Rawr calculates and displays your actual chance to crit, while the in-game character panel reflects your stats against a same-level target."
);

FAQStuff.Add(
"Why does X talent/glyph not show any value in the comparison lists?",
@"Many talents cannot be valued by DPS gain or by Survivability Gain. It's also possible that you do not have the Situation setting where the Talent/Glyph would have value. For example, If you are never Stunned, then a Warrior's Iron Will wouldn't have a value."
);

FAQStuff.Add(
"Why is it when I run the Optimizer I don't end up hit and/or expertise capped?",
@"The optimizer, when run without any requirements, will attempt to find the highest possible total DPS. In many cases, this does not include being hit/expertise capped, either by a small amount, or sometimes even by a larger amount. This is usually a correct recommendation in terms of optimizing you DPS / tanking performance. However, sometimes one may need to ensure that an interrupt or a Taunt does not miss. To ensure that an avoidance cap is enforced, add the '% Chance to be Missed <= 0' requirement before optimizing. Similar parameters are available (model-dependent) for Dodged, Parried, and Avoided, the latter taking into account all types of target avoidance."
);

FAQStuff.Add(
"Why do higher tier items sometimes show less value than lower tier items?",
@"Set Bonuses can have a serious impact on DPS; getting that 2nd or 4th piece can mean more or less for your character in specific gear-sets. It could impact your meta gem requirements, depending on your setup."
);

FAQStuff.Add(
"Why aren't items with Resilience considered relevant?",
@"Rawr is for PvE, not PvP. Modeling for PvP settings is an exercise in futility, due to the wide range of fight settings, high mobility, the extreme variability of damage intake and damage output requirements, and the sheer number of potential targets that are presented in PvP situations. Some models allow for comparison of PvP-sourced items, despite the wasted item budget, and some filter out those items with Resilience. Check the Options panel for your particular model for more information."
);

FAQStuff.Add(
"Why are the stats wrong for my X level (sub-80) character when I load from Armory?",
@"Rawr is for end-game PvE, meaning you should be level 80. Rawr does not factor things for leveling as there is no point, you will replace the item in a couple of levels anyway and all your raiding gear will end up requiring level 80 to wear."
);

FAQStuff.Add(
"Why can't I select X weapon type?",
@"Some weapon types are pointless to factor in depending on your model. For example, Fury Warriors and Enhance Shaman will never use staves, and Arms Warriors will never use one-handed weapons. Rawr intentionally does not consider such futile, sub-optimal situations."
);

FAQStuff.Add(
"Why can't I select X armor type?",
@"Each model typically only shows those items that are specifically designed for use with a certain class, but limiting the armor shown to that of the highest armor class that is wearable. However, there are certainly a number of situations in which downarmoring provides similar, only slightly lower, or sometimes even better performance for a given character. Some examples might be a Hunter considering Leather armor, or a Resto Shaman considering Cloth. To enable such considerations, simply pull up Tools > Refine Types of Items Listed, and enable / disable what items you would like to consider as applicable."
);


            CB_FAQ_Questions.Items.Add((String)"All");
            String[] arr = new String[FAQStuff.Keys.Count];
            FAQStuff.Keys.CopyTo(arr, 0);
            foreach (String a in arr) { CB_FAQ_Questions.Items.Add(a); }
            CB_FAQ_Questions.SelectedIndex = 0;
            CB_FAQ_Questions_SelectedIndexChanged(null, null);
        }
        private void SetUpPatchNotes()
        {
            #region Rawr 5.4.1 (September 24, 2013) [r73055]
            VNStuff.Add("Rawr 5.4.1 (September 24, 2013) [r73055]",
                @"Mists of Pandaria Release
Rawr.Base:
- Implemented 5.4 RPPM nerf for 5.2 trinkets/enchants.
- Fixed special effect editor to prevent extra 'modified by' strings.
- Removed some nonexistent items from the item cache and cleaned up previously edited special effects.

Rawr.Moonkin:
- Implemented haste boosting RPPM procs.
");
            #endregion
            #region Rawr 5.4.0 (September 23, 2013) [r73052]
            VNStuff.Add("Rawr 5.4.0 (September 23, 2013) [r73052]",
                @"Mists of Pandaria Release
Rawr.Base:
- Added new 5.4 randomly enchanted gear and bugfixes associated with randomly enchanted gear.
- Added new 5.4 gear from Siege of Orgrimmar and the Timeless Isle to the Item Cache.
- New caster DPS trinkets and legendary cloak have procs associated with them.
- Multiple bugfixes to handle issues with gear whose database IDs are over 99999.

Rawr.Moonkin:
- 5.4 live realm talent math changes.
- Tier 16 set bonuses.
- 5.4 trinket procs and legendary cloak.
- Assorted bugfixes related to talent math.
- Remove option for Matching Eclipse rotation.

Rawr.Mage:
- Combustion 60% nerf.
");
            #endregion
            #region Rawr 5.3.0 (August 18, 2013) [r72855]
            VNStuff.Add("Rawr 5.3.0 (August 18, 2013) [r72855]",
                @"Mists of Pandaria Release
Rawr.Base:
- Add new suffixes for newly added random items.

Rawr.Moonkin:
- 5.4 PTR stuff.

Rawr.RestoSham:
- Add healing spell triggers.
");
            #endregion
            #region Rawr 5.2.1 (June 9, 2013) [r72304]
            VNStuff.Add("Rawr 5.2.1 (June 9, 2013) [r72304]",
                @"Mists of Pandaria Release
Rawr.Base:
- Fix formula for upgraded items to be more accurate.
- Add new suffixes for newly added random items.

Rawr.Mage:
- 5.3: Living Bomb dot increased by 121% (2.21x) and explosion reduced by 78% (0.22x). No proper explosion scaling with ticks but assumes 5 effective ticks for now. AoE cap of 3
targets also removed.
");
            #endregion
            #region Rawr 5.2.0 (May 7, 2013) [r72048]
            VNStuff.Add("Rawr 5.2.0 (May 7, 2013) [r72048]",
                @"Mists of Pandaria Release
Rawr.Base:
- Update item cache to include 5.2 items.
- Enable loading upgraded items from Battle.Net.
- Miscellaneous code updates to enable new trinket procs and legendary metas.
- Implement Wowhead parsing/updating code to enable loading future item caches from Wowhead.

Rawr.Moonkin:
- Implement 5.2 trinket/meta procs.

Rawr.Mage:
- T15 4-piece bonus for Arcane and Frost.
- T15 2-piece bonus support.
- Fingers of Frost proc rate increased to 15%.
- Support for legendary metas and new trinket procs.
");
            #endregion
            #region Rawr 5.0.5 (April 21, 2013) [r71925]
            VNStuff.Add("Rawr 5.0.5 (April 26, 2013) [r71925]",
                @"Mists of Pandaria Release
Rawr.Base:
- Fix for error when loading an item set.
- Fix for error when loading a saved character.
- Add ability to select Real PPM from the Special Effect Editor.
- Performance increase in optimization and enchant listing.
- Wowhead query update.
- Enchant slot bugfix.

Rawr.Moonkin:
- Fix several stat display calculations.
- Update to 5.2 live mechanics.
- Implement T15 set bonuses.
- Fix a mistake in the Eclipse multiplier calculations.
- Add trigger for Relic of Yu'lon.

Rawr.ProtPaladin:
- Update mechanics and rotations to 5.1 standards.
- Implement many special effects triggers.

Rawr.Hunter:
- Optimize calculation speed.
- Reorder calculations for Marksmanship.
- Make calculations for Hunter more granular and closer to expected results.

Rawr.Mage:
- Arcane 5.2 updates.
- 5.2 hotfix: 40% buff to Mage Bombs.
- 5.2: 10% pyroblast direct damage reduction.
- 5.2 buff Frostbolt by 32%.
- Add placeholders for T15 set bonuses.
- T15 4-piece bonus for Fire.
");
            #endregion
            #region Rawr 5.0.4 (December 26, 2012) [r70656]
            VNStuff.Add("Rawr 5.0.4 (December 26, 2012) [r70656]",
            @"Mists of Pandaria Release
Note Rawr (Official) addon has been made to work with MoP, its release will happen shortly.

Rawr.Base:
- Base Stamina and in combat MP5
- Fixed an error that was causing misordered ItemSets, with missing helm slots on export to addon
- Removed discontinued tree druid glyphs.
- Corrected description of Glyph of Rejuvenation
- GetSpiritRegenSec formula changed to actually be per second, instead of Mp5
- Add 'Crystal of Insanity' to list of buffs.

Rawr.Mage:
- Take out Pyro dot contribution from Combustion. Ignite contribution untouched, to reflect hotfix applied shortly after 5.1.
- Arcane Charge damage buff updated to +25% per charge (up from +22%), and Arcane Blast mana cost increase updated to +75% per charge (down from +125%).
- Fire: Update Critical Mass multiplier to 1.3x, down from 1.5x.

Rawr.Hunter:
- This change should fix numerous reported bugs regarding the inability to import Marksmanship and BM spec.  Note, these specs are still not considered operational, but they will now at least import from Battle.net.

Rawr.Tree:
- Changed Glyphs to use new Glyph naming
- Implemented Glyph of Regrowth to be crit and not duration based (dropped UI field setting duration)
- Added support for Glyph of Bloom
- Got rid of a wrong basemana constant that was affecting spell costs
- Replaced Insect Swarm with Moonfire as a potential damage trigger
- Base regen now also correctly scales with mana pool scaling bonus of resto
- Added support for T14 Set Bonuses
- Fixed Tree of Life to be tied to Incarnation Talent
- Fixed Haste Breakpoint graphs and extended graphs to 10k haste rating
- Changed Tree status to partial

Rawr.Moonkin:
- Mana, MP5, Crit and Mastery so that information shown by Rawr matches the in-game values.

Rawr.Enhance:
- More work on setting up ability classes.
- Work on a CombatTable
");
            #endregion

            #region Rawr 5.0.3 (November 10, 2012) [r70237]
            VNStuff.Add("Rawr 5.0.3 (November 10, 2012) [r70237]",
            @"Mists of Pandaria Release
Rawr.Base:
- Fix for ring enchants.
- Only enable rare jewelcrafting gemming templates by default.
- Adjusted item level filtering to match live values.
- All Perfect gems are implemented
- Burning Primal has the 3% crit buff implemented
- All Inscription weapons have their correct values associated
- Fixed incorrect Engineering Tinker Gear stats
- Added Unique Equip filtering for 5.1 Rep Trinkets and Rings.
- Fix for crash related with druid glyphs.
- Fix for offhand enchant.
- Fixes for upgrades export.

Rawr.Mage:
- Fix for base stats.
- Support for T14 set bonuses.
- Support for frost spec.

Rawr.Guardian:
- Updated with inclusion of a PTR mode
- Updated Thrash to the PTR changes

Rawr.Feral:
- Updated with inclusion of a PTR mode
- Updated Thrash to the PTR changes
");
            #endregion
            #region Rawr 5.0.2 (October 25, 2012) [r70039]
            VNStuff.Add("Rawr 5.0.2 (October 25, 2012) [r70039]",
            @"Mists of Pandaria Release
Rawr.Base:
- Fix for ranged weapons and enchants.
- Fix for importing non-English server characters.
- Updated server name list.
- Added Pandaren to race dropdown.
- Updated some stat conversions.

Rawr.Guardian:
- Updated Vengence change from 2% of Unmitigated damage to 1.8%.

Rawr.ProtPaladin:
- Updated Vengence to proc from all sources (not just hits and blocks).
- Updated from 2% of Unmitigated damage to 1.8%.
");
            #endregion
            #region Rawr 5.0.1 (October 18, 2012) [r69915]
            VNStuff.Add("Rawr 5.0.1 (October 18, 2012) [r69915]",
            @"Mists of Pandaria Release
Rawr.Base:
- Spellthread enchants
- Include faction vendors when loading token costs
- Fix for Jeweler's Facet requirements
- Fix for raid buffs
- Updated Wowhead lookup to include new PvP Power
- Updated Dodge Tinker with hotfix
- Updated all JP items to their 458 ilvl values
- Updated all Honor gear to their hotfixed 458 ilvl values
- Updated Darkmoon Stamina Trinket with missing Stamina
- Support for Real PPM effects

Rawr.Hunter:
- Expertise/Dodge is now modeled for hunters
- Survival calculation method has been revised to be usable in realtime
- Some talent shots (Tier 6 and Dire Beast) are now implemented. Others are coming, still need to tie out damage values to other calculations to validate
- Survival Spec now includes approximate value for Pet DPS based solely on linear regression estimates

Rawr.Guardian:
- Added clarification on an option label
- Implemented the Thrash hotfix
- Implemented the recent hotfix to vengence that allows for stacking of Vengence even when avoiding attacks

Rawr.Mage:
- Support for crit effects and ppm effects
- Updated gemming templates
- Fix for off-hand enchant filter
- More special effect support
- Expertise to hit conversion
- Setting for Rune of Power refresh interval
- Fix for potion values
- Update for PVP Power and Resilience
- Updated default buffs
- Fire specialization now supported (Frost still in the works)

Rawr.Tree:
-Updates spells to new coefficients
-Removed Cataclysm talent checks

Rawr.Feral:
- Slight temporary adjustement to rotation
- Updated Damage values to updated hotfixes
");
            #endregion
            #region Rawr 5.0.0 (September 24, 2012) [r69617]
            VNStuff.Add("Rawr 5.0.0 (September 24, 2012) [r69617]",
            @"Mists of Pandaria Release

This is a beta release with only partial support for the following models:
Bear, Feral, Moonkin, DPSDK, TankDK, Hunter, Mage, Brewmaster, Windwalker, ProtPaladin

The rest of the models are not supported at the moment.
");
            #endregion
            #region Rawr 4.3.8 (February 20, 2012) [r65268]
            VNStuff.Add("Rawr 4.3.8 (February 20, 2012) [r65268]",
            @"Cataclysm Release
Rawr.Base:
- Workaround for item availability filtering.
- Change dwarf racial from armor multiplier to damage reduction multiplier.
- Fixes to printing.
- Added US-Gallywix server to the server list.

Rawr.ProtPaladin:
- Fix stat display calculations.
- Fix the issue where going over the block cap completely wrecked defensive stat values.
- Implement an EffectiveBlock property.
- Partial fix to relevancy methods to show relevant set bonuses.
- Implement 2T13 set bonus.
- Absorbs are applied after damage reduction multipliers, instead of getting multiplied through.
- Separate always-up special effects from proc special effects and apply them to the calculations.
- Fix display values to show static combat table values, including always-up special effects.
- Incorporate paladin tank cooldowns and Avenging Wrath in special effects.
- Fix 2T12 set bonus.

Rawr.Enhance:
- Small changes for the display
- First go at counting damage procs. Still needs some work

Rawr.Mage:
- Fix for int stacking effects in genetic solver.

Rawr.Moonkin:
- Comprehensive rebuild of moonkin simulator.
");
            #endregion
            #region Rawr 4.3.7 (January 29, 2012) [r64845]
            VNStuff.Add("Rawr 4.3.7 (January 29, 2012) [r64845]",
            @"Cataclysm Release
Rawr.Base.Items:
- All Dragon Soul Plate Tanking Socket Bonuses have been fixed
- Removed unnessasary Unique IDs from Fireland Rep trinkets
- Updated with missing Dodge, Parry, and Expertise socket bonus values

Rawr.Enhance:
- Fix for trinkets being badly valued (sans trinkets that proc damage)
- Minor fix for export to enhsim
- Tweak to Windfury calcs

Rawr.Mage:
- Updating Fireball/Pyroblast coefficients for 4.3.2.
- Fix for elixir and flask chart.
- Fix for aoe mode with cycle level procs.

Rawr.Moonkin:
- Major fixes to DoT calculations in the simulator:
- Slight change to the calculation of NG DoT tick rate in the simulator.
- DoTs implement proper refresh behavior, instead of clipping.
- Shooting Stars procs properly calculated.
- Use exact representation of DoT ticks when checking if a refresh is needed, instead of rounding to the nearest tick.
This doesn't completely solve the haste problem, but the distribution tables should be more accurate.
");
#endregion
#region Rawr 4.3.6 (January 22, 2012) [r64731]
VNStuff.Add("Rawr 4.3.6 (January 22, 2012) [r64731]",
@"Cataclysm Release
Rawr.Enhance:
- SP is now correctly set to 55% of AP
- Further tweaks to Export to EnhSim

Rawr.DPSDK:
- Fix for 21931 - Gargoyle damage was not actually related to Summon Gargoyle talent like it should be.
- Fix for 21932: Unholy Frenzy wasn't modeled.
- 21930: SS was not properly taking into account Bonus damage from mastery for it's shadow component.

Rawr.Mage:
- Improved fire cycle models.
- Combustion policy optimization tool (using idealized ignite model currently, still need to add the free tick model).

Rawr.ProtPaladin:
- Remove the behavior where a negative chance to be crit was reducing the average damage per hit/attack, which was causing mastery to be way overvalued even past the block cap.
- Fix the combat table so that entries below block are properly adjusted.
- Reduce Holy Shield static %BV to 6.67% from 10% to match the new Holy Shield mechanic.
- Change the %BV meta gem to be applied as a multiplier.
- Add explanatory tooltips to the character stats tab to help explain differences in display between Rawr and live.

Rawr.Moonkin:
- Add caster crit chance to trinket procs.
- Fix a bug with the Variable Pulse Lightning Capacitor that was double-counting its proc.
- Greatly increase the resolution of the simulator data.
- Correct the simulator's DoT casting behavior to match SimulationCraft.
- 4T12: Properly implement Shooting Stars in the simulator so that the buff can run out before Eclipse is reached.
- Glyph of Starfire: Increase the level of detail in the spell distribution tables to be able to properly calculate GoSF damage.
- 4T12: Set bonus now modifies the displayed average energy for Wrath and Starfire.
- Add additional precision to the displayed energy for nukes.
- Fix broken Glyph of Starfire calculations in the simulator.
- Minor adjustment to Glyph of Starfire implementation to improve accuracy.
");
#endregion
#region Rawr 4.3.5 (January 8, 2012) [r64477]
VNStuff.Add("Rawr 4.3.5 (January 8, 2012) [r64477]",
@"Cataclysm Release
Rawr.Base:
- Changed server name from Aggra to Aggra-Portugues to work with Blizzard's API.
- Adding Nozdormu's Presence temporary buff.

Rawr.Items:
- Updated all 3 Timepiece of the Bronze Flight's with a gem socket that Blizzard hot fixed
- Kiril, Fury of Beasts was hotfixed with undocumented Bonus Armor
- Corrected Bracers of Flowing Serenity to included the 20 Int gem socket that was hotfixed
- Added both Pit Fighter trinkets
- Support for special effect damage procs modified by spell power and attack power.
- Updated Item Cache with Cunning, Vial, and Bone-Link SP/AP Scaling
- Fixed Rathrak, the Poisonous Mind (LFR 390) not having a Mage Restriction associated with it.
- Gurthalak now procs 10 times over 12 secs; up from 8 times over 10 seconds.

Rawr.Enhance:
- Add export for T13 set bonus's
- Fix for incorrect AP/SP values

Rawr.Cat:
- Added support for AP Scaling

Rawr.Bear:
- Added support for AP Scaling

Rawr.Mage:
- Changing effect procs to 2x crit multiplier, support for spell power modified damage procs.

Rawr.Moonkin:
- Support proc effects that benefit from spell power scaling.
- 4.3.2 PTR: 4T13 set bonus now also increases Starsurge damage by 10%.
");
#endregion
#region Rawr 4.3.4 (December 26, 2011) [r64337]
VNStuff.Add("Rawr 4.3.4 (December 26, 2011) [r64337]",
@"Cataclysm Release
Rawr.Base:
- Added Azralon Server to the Brazilian realm listing
- Fix for marking gems available from optimizer checks.

Rawr.Items:
- Adding crit heal to Burning Shadowspirit Diamond.
- Corrected Mage T13 legs being associated with Tier 13 set
- Double checked Boomkin T13 shoulds as being associated with Tier 13 set

Rawr.BossHandler:
- More health adjustments to Normal and Heroic Bosses, including Heroic 25 Spine and Madness health values

Rawr.Cat
- Fixed for overvaluing of Agility Stacking trinkets
- Started work on getting Vial to be valued more consistantly against SimC and Mew

TankDK:
- Added trigger support for Indomitable Pride

Rawr.Mage:
- Fix for base stats.
");
#endregion
#region Rawr 4.3.3 (December 19, 2011) [r64234]
VNStuff.Add("Rawr 4.3.3 (December 19, 2011) [r64234]",
@"Cataclysm Release
Rawr.Items:
- All missing items should be in place
- Removed all weapons from LFR Dragon Soul except those that drop from Madness of Deathwing
- All Dragon Soul procs should have the correct proc information including damage and cooldowns
- Changed Souldrinker's Trigger to a Current Hand Hit Trigger so that it can proc from either main hand or off hand 

Rawr.Mage:
- Fix for T13 combustion.
- Don't show stacking effects in proc uptime graph.
- Cycle level haste procs enabled by default.
- Adding crit to optimization constraints.
- By spell breakdown improvements.
- Second order int proc effects on fire cycles.

Rawr.Enhance:
- Fix for mastery being completely devauled.

DPSDK:
- Beta version of MasterFrost rotation.
");
            #endregion
#region Rawr 4.3.2 (December 14, 2011) [r64166]
VNStuff.Add("Rawr 4.3.2 (December 14, 2011) [r64166]",
@"Cataclysm Release
DPSDK:
- Fix for Physical damage procs like the Bone-Link Fetish

Rawr.Mage:
- Fix for cycle level haste procs

Rawr.Enhance:
- 4.3 Flametongue change
");
#endregion
#region Rawr 4.3.1 (December 11, 2011) [r64147]
VNStuff.Add("Rawr 4.3.1 (December 11, 2011) [r64147]",
@"Cataclysm Release
Rawr.Base:
- Added the 4 new Portugues servers.

Rawr.BossHandler:
- Adjusted Enrage times and speed timers
- More work on health values. Values should be more accurate.

Rawr.Mage:
- Update talent presets.
- Fix for goblin haste.
- Support for T13 with advanced cycle level procs. Better T13/Combustion integration.

Rawr.DPSDK:
- Fixed a bug w/ Chill of the Grave & Rime procs.
- Fixed a bug w/ CritChance calcs for OB & FS
- Fix for Glance damage in White Swings.
- Change rotation report to provide Crit & Hit chance values instead of Threat.
- Default Presence is now Unholy.
- Added KMProc rate option. TODO: add to UI.
- Update KMProc in the rotation. Now very close to actual numbers, but very hacky.
- Update FS & OB to only crit from KM.

Rawr.Tree:
- Add Burning Shadowspirit Diamond, remove Revitalizing metas

Rawr.Retribution:
- T13 4P implemented (Currently increases all damage done)

Rawr.Healadin:
- Holy Radiance now has a 3.0-second cast time
- Seal of Insight, when Judged, no longer returns 15% base mana to the paladin. 
- Clarity of Purpose now also reduces the cast time of Holy Radiance.
- Illuminated Healing (mastery) now also applies to Holy Radiance.
- In addition to providing haste, the effect from Judgements of the Pure now increases mana regeneration from Spirit by 10/20/30% for 60 seconds. (assumed 100% uptime) 
- Light of Dawn now affects 6 targets (base effect), up from 5.
- Tower of Radiance, in addition to its current effects, now also causes Holy Radiance to always generate 1 charge of Holy Power at all times.
- Beacon of Light is triggered by Word of Glory, Holy Shock, Flash of Light, Divine Light and Light of Dawn at 50% transference and Holy Light at 100% transference. It does not transfer Holy Radiance, Protector of the Innocent or other sources of healing.
- Glyph of Light of Dawn now lowers the number of targets to 4, instead of increasing targets to 6, but increases healing by 25%.
");
#endregion
#region Rawr 4.3.0 (November 29, 2011) [r63968]
VNStuff.Add("Rawr 4.3.0 (November 29, 2011) [r63968]",
@"Cataclysm Release
Rawr.Base:
- Support for WoW 4.3
- Updated the Attack Power Multiplier Buff to 20%, up from 10%

Rawr.Items:
- Add SP scaling to Windward Heart proc
- corrected the haste buff Special Effect calculations on the Ti'tahk, the Steps of Time staff
- Most of the tier pieces have source info. Unfortunetly I ran out of time on completing the task, this will be corrected in the next release
- DK T13 tier set now properly is DK only
- Added T13 Tier tokens to the Item Source editor

Rawr.BossHandler:
- Adding T13 Bosses to Boss handler drop-down selector.

Rawr.Bear:
- Updated the Base Stamina multiplier to 20%, up from 10%
- A bit cleaner implementation of the Tier 13 set bonus.

Rawr.Mage:
- Support for Will of Unbinding.
- Fix for dot rounding.

Rawr.DPSDK:
Rawr.TankDK:
- Fix for 21825: Exception on setting TankDK buffs.
- Update to finish implementation of T13. In the process found a bug w/ Mastery Special effects that I fixed.
- Partial fix for 21833: Fix for Disease DPS and Pet DPS presented in the UI and calculated in total DPS. Still to look into White Damage numbers.
- Adding DSp5 to the DeathStrike tooltip.

Rawr.Hunter:
- Refactoring state table. It's a work in progress, but at least we're getting Survival numbers.

Rawr.Retribution:
- First work on T13
- 4.3 ability changes
");
#endregion
#region Rawr 4.2.7 (November 13, 2011) [r63784]
VNStuff.Add("Rawr 4.2.7 (November 13, 2011) [r63784]",
@"Cataclysm Release
Rawr.Base:
- Fix for progressive optimize.

Rawr.Items:
- Madness of Deathwing's weapon procs are now modeled
- Item update for 4.3

Rawr.Bear:
- Intellect gear should no longer show up in the item selection list
- Gear Set Bonus's should be more condensed.

Rawr.Mage:
- PTR mode, very rough support for T13.

Rawr.DPSDK:
- Defect : Fix the KM crit rate.
- Fix the RP math in the rotations.
- Update more 4.3 PTR values (Bone Shield count & Unholy Might)
- Wrap the 4.3 changes w/ the PTR Mode flag from general options incase we have another release before 4.3
- Update Blood Rotation for Outbreak change. (1 more DS since we have those runes free)
- Adding DND to Unholy rotation. It's not a complete fix, but just an experimental start to deal w/ UH numbers.
- Adding Damage object. Plan is to implement this as a global damage value that can be used as a core value esp for any ability that may have multiple damage sources from a single ability.

Rawr.TankDK:
- Update DS healing to not be reliant on hit.
- Refactor model to better deal with DS Heals and Blood Shield. 
- Fix for Defect 21634: Negative Burst value in stam.
- Fix for T12 set bonus evaluations
- Provide the option to roll burst back into the body of values (burst changes to survival will appear in survival, and burst changes to mitigation will appear in mitigation)
- Update defaults to go with the changes I implemented.

Rawr.Cat:
- No more Intellect gear showing up in the item select
- Set bonus list is more condensed
- Selecting Fully Buffed Cat should no longer crash the program
- More buffs are now selected when loading from Battle.Net
- Default glyphs are turned off so that when importing from Battle.Net, the imported glyphs are not overwritten by what the default glyphs
- Added several Hit and Expertise Optimizer options
- Added a few information mouseovers in the Info area
- Slightly adjusted the T12 4-piece.
- Gave some value to Physical Damage procs. Though it needs a better fix.

Rawr.Moonkin:
- Add epic gem templates.

Rawr.Hunter:
- MM is generating some numbers.

Rawr.Tree:
- Add epic gem templates
- Support trinkets activated by damage spells (e.g. Necromantic Focus, Insignia of the Corrupted Mind, Will of Unbinding) by automatically keeping up Insect Swarm if a damage-triggered effect is present; 3/3 Genesis is recommended for that since it increases the duration of DoTs
- Make triggering damage effects configurable and turned off by default, since it may confuse users otherwise
- Account for miss chance on DoT; currently assumes 0 hit instead of bothering to compute actual spell hit from gear (but we are an healer, so that's accurate in most cases)
");
            #endregion
#region Rawr 4.2.6 (October 16, 2011) [r63473]
VNStuff.Add("Rawr 4.2.6 (October 16, 2011) [r63473]",
@"Cataclysm Release
Rawr.Base:
- Disable nonoperational batch tools options in SL.
- Warn about tinkering availability.
- Show file name in window title for WPF.
- Don't replace dots in save to repo dialog.
- Fix for optimizer crash.
- Fix button size.
- Fix for export from upgrade list.
- High dpi support; wrapping character slots and cut off search boxes
- Fix for Issue 21084: [Charts] Top Graph Values Texts are Clipped
- Fix for Issue 21257: Add buffs by raid member adds incorrect buff
- Adding note to the Error Window's Copy to Clipboard for which Application has the issue SL vs WPF
- Modified Error Window's Copy to Clipboard to ask the user for the steps they have tried in a window and then putting that information into the clipboard text.
- Mark JC gemming template if JC
- Remember last model used when starting, persist settings between versions for WPF
- Context menu option to load upgrade set from upgrade list.

Rawr.Items:
- Added most T13 Set names to the buff tab
- Updated Wowhead patch 4.3 zone info
- Updated Item cache filtering for T13 normal and heroic ilvls
- Several Brimstone Rings and Fireland rep trinkets were added, but not implemented (updated uniqueness for all)
- Updated with a few zone names
- Added Season 11 PvP to the Item Set filtering
- Updated Filtering to include 4.3 Dungeons and T13 Dragon Soul Raid
- Updated iLvl filtering to include T13 ilvls
- Updated the Special Effects introduced in 4.3
- Implement new Bow/Gun Enchant Flintlocke's Woodchucker
- Corrected Skullpiercer Pauldrons socket bonus
- Refreshed Feral Druid T12 tier
- Double checked Flickering Cowl's Suffix Enchants

Rawr.Buffs:
- Fixed pet buffs from not applying
- Removed Improved and Glyphed Hunter Pets from the Buff By Raid Module
- All Hunters provide the Hunter's Mark from the Buff by Raid Module
- Hunters stings no longer provide a Raid buff, removed from Buff by Raid Module
- If a user selects Beast Mastery Hunter, show BM only pets as possible pets, else don't
- The Apply Buffs by Raid Members dialog now has a sub-dialog that allows you to import the Link to Composition from http://raidcomp.mmo-champion.com/
- Slight pet list naming change in the last release.

Rawr.BossHandler:
- Updated Basic Damage and health values for Fireland bosses. (25% nerf on normal, 15% nerf on heroic)
- Small update to T11 Bosses
- Did a first pass on adding in the Tier 13 Bosses to the Boss handler
- Guesswork on the damage numbers for Tier 13 white attacks
- Alizabal, Mistress of Hate the new Baradin Hold boss is initially implemented; the timing of abilities are guesses at this point
- Added support for a 5th type of Raid level in preparation for the upcoming Looking For Raid raid difficulty
- Updated timing of the new Baradin Hold Raid boss based on early videos (still a work in progress; but it should be close)
- Started going back and re-working/cleaning up T12 boss abilities starting with Beth'tilac and Lord Rhyo'lith. Will get T11 and T13 afterwards.
- Massive update to Alysrazor and Baleroc modules 
- Slight adjustments to the new Baradin Hold Boss in 4.3
- Shannox has been double checked and cleaned up
- Majordomo Staghelm has been implemented
- Fixed an issue where selecting average damage from a 25 man raid was not selecting the correct Raid size
- Most Ragnaros Normal Abilites are in place. 
- Just missing World of Flame, Seeds, Lava Scion melee attacks, and all of Phase 4
- Slightly adjusted Tier 13 damage numbers As well as Ragnaros' damage numbers
- Updated the first three boss's 10-man Health pools
- Deathwing needs to hit harder than the rest of his minions

Rawr.Bear:
- Implemented T13 set bonuses.

Rawr.Mage:
- Fix for solver stall.
- Take mana adept into account for by spell breakdown.

Rawr.DPSWarr:
- Recklessness modelling and tooltips updated
- Inner Rage modelling and tooltips updated
- Berserker Rage tooltip typo fixed
- Removed the change that made it state Fury Isn't Cata Ready on the pane and put it into the tooltip

Rawr.DPSDK:
- Trying to fix the odd pet numbers.
- Gut the solver to make room for the possibility of using a Markov process for priorities.
- Tweak the hit/crit code to see if yet another simple change may fix some of the odd values.
- Disable the BloodTap code because it was screwing with haste.

Rawr.TankDK:
- Fix for 21596: I forgot to update STR - parry conversion to 27%
- Fix for Issue: 21602: Mastery Rating was being added in twice. Once from the AccumulateTalents and once in AccumulateRatings.
- Fix for some inflated White Hit values due to hit rating providing benefits past cap.

Rawr.Cat:
- Added T13 2 and 4 piece bonuses
- updated Tier 13 4-piece to the latest PTR build (TF causes next Ravage to cost no energy, not stealthed)
- 4 Prime glyphs were marked as default, removed glyph of Tiger's Fury as a default glyph (though that could change in 4.3 with the T13 4-piece set bonus)

Rawr.Moonkin:
- New 4T13 set bonus.
- Fix bug where Starsurge cooldown was being decreased by Starsurge cast time in the simulator.
- Apparently, I never correctly buffed the spell coefficients for Starfire and Wrath. Fixed.
- Change solver to count only casts executed during a rotation. This corrects the issue with under-weighting Starfire.
- Shooting Stars procs do need to decrement the Starsurge cooldown by the global cooldown, because the CD starts when the nuke is launched.
- Shooting Stars procs were being based off partial ticks of dots.
- Fix a bug in the simulator where the dot tick calculator used the wrong tick rate for Insect Swarm.
- The Moonfire/Sunfire transition should not be clipped.
- When Starsurge is below the GCD, part of the cooldown should be used up during the transition.
- Fix an issue with not correctly determining whether Moonfire needs to be recast.
- Fixed double-counting of Starsurge damage in the rotation data, which was causing the 4T13 set bonus over-valuation.
- Display the actual average cast time of Starsurge, including Shooting Stars procs.
- Fix the DPET chart due to the fix to Starsurge cast time.
- Spell power multiplier multiplies actual reported spell power, not spell damage.
- Moonfury is additive with glyphs, Blessing of the Grove
- Fix a mistake that caused Int trinkets to be severely undervalued.
- Fix the rotation duration to match WrathCalcs. It excludes the duration of time spent on cooldown spells, but oh well.
- Re-implement Dragonwrath procs reducing rotation duration.
- Apply BonusSpellPowerMultiplier to the Heart of Ignacious.
- Intellect procs benefit from BonusIntellectMultiplier.
- Remove spell crit depression.
- Modify Eclipse uptimes by Starfall usage.
- Replace the None rotation on the options tab.
- Starfall damage benefits from the boosted Lunar uptime when cast on cooldown.
- Moonkin Form grants 10% spell damage increase, not 1%.
- Put the duration lengthening from Starfall and Treants back in. This brings the DPS value in line with WC and SimC.
- Add default glyphs.
- Add a buff set.

Rawr.Hunter:
- Big check in. Hacked up a bunch of stuff to start getting some numbers out that look kinda reasonable. Currently I am only modeling MM and there's still a bunch of stuff not properly hooked up.
- Have the Aimed Shot proc working, but I think Careful Aim is still broken. 
- Focus cost/deficit and rotation adjustments for that has not been implemented.

Rawr.Tree:
- Add T13 2P and 4P set bonuses
- Add support for PTR changes to Wild Growth and Glyph of Wild Growth
- Make the T12 4P heal respect effective healing settings properly
- Allow the T12 4P heal to crit
");
#endregion
#region Rawr 4.2.5 (September 25, 2011) [r63191]
            VNStuff.Add("Rawr 4.2.5 (September 25, 2011) [r63191]",
            @"Cataclysm Release
Rawr.Base:
- Add override reforge option in batch tools.
- Support for multiple of the same item (each diamond represents a separate item).

Rawr.Items:
- Fixed a small issue with the filtering system.
- Variable Pulse Lightning Capacitor was buffed by 260% but only stacks to 5 stacks, down from 10
- Feral Druid Tier 12 legs had wrong stats, fixed
- You cannot equip 2 different Signet of the Avengers (Fireland Rep) rings at the same time

Rawr.BossHandler:
- Updated Sinestra and Ragnaros with better Under 35% and 20% numbers.

Rawr.Bear:
- Added PvP set bonuses.
- added Damage Absorb and Healing trinket procs functionality
- Added Damaging Procs from trinket
- Movement enhancing buffs should be valued a lot higher than what they were before
- Added Leader of the Pack self-healing
- Implemented Survival Instincts, King of the Jungle, partially Frenzied Regen (the healing portion is not working as intended as of this moment), and Barkskin cooldowns
- Berserk is implemented but has not been added to the rotation calculator yet.

Rawr.Mage:
- Hyper regen cycle analysis.
- Solver for arcane dragonwrath cycles.
- Fix for cost multiplier 0.
- Fix for mana potion constraint in combinatorial/genetic solver.
- Cooldown offset option for genetic solver.

Rawr.Retribution:
- Censure hotfix applied

Rawr.DPSDK:
- Fix Crit rate of DS.

Rawr.TankDK:
- Stop filtering out ALL caster stats since it was filtering out things like Kings.

Rawr.Cat:
- Added PvP set bonuses.

Rawr.Moonkin:
- Dragonwrath proc modeling.
- Euphoria now correctly includes the 4T12 set bonus when it procs.
- Moonkin Solver engine version 3.0
- Add T13 set bonuses.

Rawr.Hunter:
- Update Base Stats.
- Update to GetStats methods. On MM hunter w/i 2% for RAP.
- Working on getting displayed values closer to in-game paper-doll.
- Adjust GCD to 1 Second.
");
#endregion
#region Rawr 4.2.4 (August 15, 2011) [r62567]
VNStuff.Add("Rawr 4.2.4 (August 15, 2011) [r62567]",
@"Cataclysm Release
Rawr.Base:
- Menu options for loading item costs.
- Improved filter manipulation performance.
- Updated missing Chinese (about 200 servers) and Tiawanese (about 20 servers) server names.
- Printing support for optimizer results.

Rawr.Items:
- Unheeded Warning's proc was hotfixed to proc 1926 Attack Power
- Incorrect Nature damaging proc from DMC:Hurricane
- Updated several items that were hotfixed recently
- Finalized Heroic trinket values that were hotfixed shortly after the release of 4.2 (values were guesstimates at the time)
- Corrected several 371 PvP leather off-pieces that have lower than anticipated Stamina

Rawr.BossHandler:
- Tier 11 was over exaggering on the surviability values (Basing all damage values around the two hardest hitting bosses of the content) Fixed so that it takes the weakest boss damage and then multiplies the damage on a per boss basis
- Updated Heroic 25 Beth'tilac's health value
- Fixed some Boss Damage numbers for Firelands Bosses

Rawr.Bear:
- Fixed so that the expected Survival amount needed from the Boss Handler properly updates the Survivability number in the Options Menu

Rawr.Mage:
- Combinatorial solver available in advanced options, only works in async mode currently. It's a good alternative for solving optimal cooldown stacking (not any faster than full advanced solver, but is guaranteed to generate perfect reconstructions), only works for arcane at the moment.
- Genetic solver, if you like the idea of combinatorial solver, but would like to see results today, then this is for you.
- Advanced Simple Stacking mode, based on combinatorial solver, but using a handcrafted stacking rule that is in general suboptimal. Only works for arcane at the moment and highly experimental. It can be slower than normal solver depending on how many item based use effects you're using.
- updated legendary proc rate (still doesn't model mechanics changes)

Rawr.Retribution:
- Propper CS CD

Rawr.Tree:
- Correct T12 4P to double-dip MSS and ToL according to EJ thread
- Fixed the Revitalizing meta to give 206% instead of 203% crit heals (according to in-game tests and TreeCalcs): this seems to make it BiS

Rawr.DPSDK:
- Update DS values based on hotfix.

Rawr.Cat:
- Fixed some missed 4.2 changes in the trigger section

Rawr.Moonkin:
- Remove hard-coded 8% magic damage debuff and calculate based on whether the buff is present.
- Dragonwrath: Added ability of proc to crit. Also made the special effect static to reduce potential performance issues.
- Dragonwrath has an 11% proc rate and also procs from Starfall.
- Dragonwrath proc: Inherits the moonkin's critical damage multiplier.

Rawr.Hunter:
- Most base calculations are back in place
");
#endregion
#region Rawr 4.2.3 (July 29, 2011) [r62241]
VNStuff.Add("Rawr 4.2.3 (July 29, 2011) [r62241]",
@"Cataclysm Release

Rawr.BossHandler:
- Updated with Heroic Baleroc and Domo health nerfs and updated Heroic Rag 25 health pool 

Rawr.Elemental:
- Updated hotfix to Lightning Bolt, Chain Lightning and Lava Burst's Spell Power bonus from Shamanism 

Rawr.Mage:
- Moonwell Chalice hotfix. 
- Arcane cycle solver support for delayed AM proc from nether vortex

Rawr.Retribution:
- Changed Goak to static SpecialEffect
- PVP Set Bonus adapted
- CrusaderStrike Cooldown decreases now with Spellhaste and not Physicalhaste 
- More work on the Rotation 

Rawr.ProtWarr:
- Combat Table Coverage Ranking mode

Rawr.TankDK:
- Adjustment for Vengeance calcs by player role. 
- Hanging on imports, so hopefully this resolves that issue. 

Rawr.Moonkin:
- Change usage of BonusNature/ArcangeDamageMultiplier to be specifically for the debuff on target, enabling more accurate representation of damage procs.
- Change damage proc effects to only benefit from magic damage multiplier on target, and not on self-buffs. 
");
#endregion
#region Rawr 4.2.2 (July 24, 2011) [r62174]
VNStuff.Add("Rawr 4.2.2 (July 24, 2011) [r62174]",
@"Cataclysm Release

Rawr.Base:
- Implemented Blizzard API and readded support for TW and CN realms.

Rawr.Items:
- Finally got around to finish updating the Tier token drop location info.

Rawr.Buffs:
- Removed Hunter pet food
- Fixed Hunter's mark Buffs 
- All Cata Elixirs provide 40 additional stats instead of 90 

Rawr.BossHandler:
- First pass to implement Initial 4 bosses in Firelands raid. 

Rawr.Optimizer:
- Reforging swap mutation. 
- Defect 21159: IndexOutOfRange because AllowedRandomSuffixes can have a count > than item.AvailabilityInformation.Length
- Defect 21065: Sometimes reforge ID is -1 when passed into CurrentStatValue(), but that condition was not being handled. 
- Display percent change in optimizer results. 
- When you cancel optimization you will now be presented with the best result so far with an option to cancel, continue or load current best. 
- When optimizer determines your currently equipped items are not marked available you will now have an option to automatically mark them available and continue with optimization, leave as it is and continue or cancel. (WPF only)

Rawr.Mage:
- Changing 4T12 to multiplicative mana reduction. 
- Fix for comparison calculations. 
- Performance improvements. 
- Nonsummon damage procs benefit from mana adept, Moonwell Chalice can stack with other item based cooldowns. 
- Adding hard cast Pyro to spell info. 
- Passive healing parameter for survivability model. 

Rawr.ProtWarr:
- Fixed missed Strength->Parry calc in ProtWarrior
- Fix for performance issues

Rawr.DPSWarr:
- Fix for performance problems

Rawr.ProtPaladin:
- New ranking mode based solely on combat table coverage. 

Rawr.DPSDK:
- Defect 21131: Reduce problems with stacking Razorice, Cinderglacier, and Fallen Crusader
- Reduce issue with creating a new character. 
- Partial fix for defect 21185: A bug where a possible negative crit chance could cause issues in Hit and therefore translate to Expertise being worth more than it should. 
- Fix for 21069: Switch SD procs from cheaper DCs to be like Rime, which means extra DCs. 
- Fix for 21173: DMC:Hurricane was not properly handling the proc. Now it does and it's scary for DWFrost! 
- Improve rotation calculations to include partial valued spells. This should smooth out some Haste issues, but not all.
- Little bit of cleanup.
- Switch SpecialEffect handling to use the new dictionary methods.
- Improve KM handling
- Improve Runic Corruption handling.

Rawr.TankDK:
- Update DPS from Boss values to ensure proper handling of non-physical dots as well as % of health values in the DPH numbers. 

Rawr.Tree:
- Fix Eye of Blazing Power to be affected by crit, Master Shapeshifter and Tree of Life 

Rawr.Moonkin:
- Remove treant melee crit depression, because my testing showed a flat 5% crit rate on a boss-level dummy.
- Modify 2T12 set bonus down to 3.5 casts/proc, and a 2.5% crit rate. 

Rawr.Enhance:
- Partially apply Patch 9898: Tweaks to EnhSim Export 
");
#endregion
#region Rawr 4.2.1 (July 10, 2011) [r61914]
VNStuff.Add("Rawr 4.2.1 (July 10, 2011) [r61914]",
@"Cataclysm Release

Rawr.Items:
- Added 4.2 Random Suffix information. 
- Added missing Molten Front items (I blame MMO-C for not mentioning the vender)
- Fixed Circuit Design Breastplate and Spiritshield Mask for not having a socket Bonus.
- Corrected all Fireland's Flickering Random enchants (Helm will not longer be OP)
- All 371 Vicious Gladiator gear is added
- All Bloodthirsty and all non-weapon 352 and 365Vicious gear has had it's Honor removed. I'll leave all of the depreciated gear in the database for the next month so that people can transition to the new 371 Vicious gear.
- Added loot that drops from all bosses in Firelands except from Rag. (7 weapons, both normal and heroic)
- Necromatic Focus, Matrix Restabalizer, Vessel of Acceleration, and Apparatus of Khazgoroth all are using proc amounts on live that are different than what is being posted on the tooltip. Corrected all with the correct normal amounts and estimated heroic amounts.
- So apparently the Stage 1 and Stage 2 versions of the Dragonwrath, Tarecgosa's Rest staff did make it to live dispite what Wowhead and MMO-C's databases say otherwise. Added both staffs to the databases. 

Rawr.BossHandler:
- Updated Normal health values for all T12 bosses 
- Alysrazor has a 15 minute enrage timer
- Baleroc has a 6 minute Berserk timer

Rawr.Mage:
- Advanced armor swapping mode, doesn't take into account gcd loss from swapping. 
- Fix for proc calculations.

Rawr.ProtWarr:
- Fixed an issue where T12 2-piece was not providing any value
- Updated T12 2-piece to be affected by Booming Voice. 

Rawr.Bear:
- Fix for 4T12 slowdown. 

Rawr.DPSDK:
- Defect 21071: Presence wasn't properly being handled in all cases. Even if UH spec should always be in UH presence, it's best to give th option to see the difference.
- Defect 21074: Orc racial too high.
- Defect 21076: Exception on Load/create new character.
- Update status to fully supported. 
- Defect 21069: Unholy Blight wasn't actually plugged into the rotation. 

Rawr.Tree:
- Rework and improve effective healing options 
- Add activity rate
- Wild Growth is now affected by Gift of Nature according to the EJ thread

Rawr.Moonkin:
- Wild Mushroom is affected by Moonfury, per EJ testing.
- Relabel stat graph options to correct names and remove irrelevant stats Expertise and Spell Pen. 
- Wild Mushroom not affected by the crit part of Moonfury. 
");
#endregion
#region Rawr 4.2.0 (June 26, 2011) [r61631]
VNStuff.Add("Rawr 4.2.0 (June 26, 2011) [r61631]",
@"Cataclysm Release

General:
- Most models updated for 4.2

Rawr.Items:
- Updated all Vicious (Season 9) PvP gear to be bought with Honor Points
- Added Reckless (Season 10) PvP items
- Added both Direbrew and Hallow's End items
- Updated descriptions on all Seasonal item's sources
- Updated a few trinket procs based on SimC testing
- Updated proc on Eye of Blazing Power (now heals for over 5 times the original value)

Rawr.Healadin:
- Holy Radiance default 50% of max heals (up from 40%).
- Burst vs Total healing changed to 30% burst (up from 25%).

Rawr.Mage:
- Fix for some proc calculations.
- Changing ignite munching to default 8%.
- Support for on use mastery effects.

Rawr.ProtWarr:
- Fixed a bug when trinkets (like Porcelain Crab) go over the effective mastery cap Rawr would appear to lock up.

Rawr.Retribution:
- Inq Multiplier fix.
- AvgTargets implemented.

Rawr.Enhance:
- Enable a set of default buffs/debuffs when loading. 

Rawr.DPSDK:
- Defect 20994: Fix problem with Expertise.

Rawr.Tree:
- Add limited cooldown usage decision support to the optimizer.

Rawr.Cat:
- Added support for Fire, Holy, Nature, and Physical Damaging Proc enchants.
");
#endregion
#region Rawr 4.1.06 (June 12, 2011) [r61314]
VNStuff.Add("Rawr 4.1.06 (June 12, 2011) [r61314]",
@"Cataclysm Release

General:
- A complete new model for Tree
- Fixes for Battle.Net loading
- Many models have implemented 4.2 PTR mode

Rawr.Base:
- Gear charts will now display versions of items for all enchants marked as available
- Fix for Paladin glyphs
- Support for optimizer pause/resume

Rawr.BossHandler:
- Added new fields for Spell/Creature Ids. They currently are not used by anything at this point, but they are in for completion reasons. 
- Finished documenting all of the T12 bosses
- GetImpedancePerc now only counts if affected by your character role

Rawr.Items:
- Added most items from 4.2 PTR
- Updated Voodoo Hexblade and Amani Scepter of Rites as Main Hand only instead of One Handed weapon as per May 11th Hotfix
- Almost all gear below ilvl 300 is now gone. Only thing left from Wrath is the ilvl 277 T10 gear. 
- Added missing Leatherworking Random Enchanted items and updated all with their Random Enchanted ids
- Removed all PvP that was datamind but never made it to live
- Added all 2200 Arena/BG rating items
- Updated all pvp gear sources (2200 Arena gear is temporarily set to vender until the extra PvP info in the UI can be implemented)

Rawr.Healadin:
- Added Holy Radiance. Worked on options tab. Started work on rotation. Removed Str and Agi items from gear list. 
- Lay on Hands added
- Enllightened Judgement self heals added
- Protector of the Innocent added
- Illuminated Healing added (Mastery)
- fixed crit % bug
- updated gemming template
- added Cleansing to rotation
- Glyph of Cleansing
- added Seal of Insight melee attack mana regen
- Speed of Light haste buff added
- Fixed bug that caused trinkets and other proc items to be calculated way wrong
- Infusion of Light talent- HS crit gives -0.75 sec per point from next DL/HL
- Fixed bug in Crusade talent calculations
- added User delay to options tab to account for average lag + user delay for each cast
- accounted for Divine Plea cast time
- adjusted options panel to make things more clear and compact
- Conviction talent added to model 

Rawr.Bear:
- Changing back to survival soft cap

Rawr.Mage:
- Fix for arcane solver crash
- Numerical stability improvements
- If you enable Boss Handler and Cooldown Segmentation you can take advantage of phase damage multiplier data, phases are also shown on sequence reconstruction chart
- Changed frost cycle to interpolation on simulation data
- Disabled Ignite from Flame Orb, fix for spell breakdown, incremental evocation restriction for arcane only. 
- Fix for advanced mana gem constraint
- Support for orc racials
- Improved cycle generator reporting
- Proc mode combustion model

Rawr.Moonkin:
- Remove the calculations for DoT spells generating Eclipse energy.
- When over hit cap, displays the amount of rating over cap.
- Fix issue with enabling reforging spirit to hit that was causing disappearing reforge options. 

Rawr.Retribution:
- HoW included in rotation
- some work on impedances
- New Rotation now calculates time lost through impedance of bossOptions 

Rawr.TankDK:
- Added Stats Graph
- Luck of the draw buff not factored in

Rawr.ProtPaladin:
- Potential Veng issue
- Added Vengeance Calc to ProtPaladin

Rawr.Enhance:
- Elemental Precision Talent not fully modelled
- Could not locate any ties to Elemental Precision for the frost/fire/nature damage bonus. Added
- Tweak to Lava Lash calcs
- T11 calc typo fixed 
- Agony and Torment EnhSim export fixed

Rawr.DPSDK:
- Update the new graphs
- Fix from EJ Frost Post: Frost Haste (Icy Talons) doesn't translate to faster runes. (Still can't confirm that UH is the preferred presence with my gear while DW.)
- Fix White Damage being so far, far off from tested values.
- Fixed stat display. Wasn't applying ratings to BasicStats before displaying them. 
- Double-Dipping for Buff/Talent Brittle Bones
- Replaced the GetBuffsStats framework in DPSDK to what DPSWarr uses to prevent conflicting buff/talents. Set up Brittle Bones as the first.
- Fixed the Set Bonuses implementation, was looking for > 2 not >= 2 as it should have been 
- Fix Haste affects on RuneCD. And display value for comparison w/ in-game values.
- Adjust rotation reporting slightly. 
- Fix display issue w/ DPS breakdown and Damage Per Use. Some of the structures were over-lapping in usage. Now it's discrete and values should make more sense.
- Haste fix 
- Significant work on Rotation and ability displays has me confident to upgrading Unholy to be on-par with Frost at Mostly Cata ready.
- Fixed Health and a few other displays.
- Completely rebuild present Unholy priority. It's not very performant, but it's much more accurate to testing.
- Fixed UH mastery.
- Unittests for a new static function in rotation. 
- Update UH numbers based on logs... Adjusting Per-hit numbers for SS that were > 5% off from what I was seeing in BWD last night. 
- Update UH rotation to not be so expensive. :( The dynamic version is much more accurate, but also much more expensive. 

Rawr.Tree:
- Complete redesign

Rawr.Warlock:
- Mastery and Improved Corruption are additive modifiers
- Shadow Embrace is +3/4/5%, not 9/12/15%
- Bane of Doom is not affected by Shadow Embrace or Haunt 
- Add some more pet calculation values to the display; relabel Bonus Damage as Spell Power.
- Revert erroneous Shadow Embrace change.
- Try to make displayed pet stats a little closer to in-game character sheet (for now, tested with Felhunter only).
- Update Shadow Bite for 4.1 changes.
- Include embedded effects when determining trinket relevancy (e.g. Heart of Ignacious). 
");
#endregion
#region Rawr 4.1.05 (May 08, 2011) [r60625]
VNStuff.Add("Rawr 4.1.05 (May 08, 2011) [r60625]",
@"Cataclysm Release

Aligned CalcOpts panes for all Tank models

Rawr.Base:
- Removed BossAttackPower stat as it no longer exists in game (Demo shout and others were changed to a % dmg mult)
- Efflorescence Talent no longer requires Living Seed Talent as a pre-req

Rawr.BossHandler:
- Avg Attack/TargetGroup and Impedance calcs now calc in the PhaseUptime to reduce oversizing
- Attack editor fixed for standard usability scenarios

Rawr.Buffs:
- Updated 4.1's Steelskin's flask bonus from 300 stam to 450 stam

Rawr.Items:
- Updated 4.1 sources with correct locations
- Fixed Shard of Woes' Mana reduction not showing
- Updated Mastery stat to 356 ilvl version of Manacles of the Sleeping Beast
- Initial working for automating Holiday item being treated as quest rewards (Holiday daily satchels; which the holiday items drop from; are considered quest rewards). Needs further work

Rawr.Optimizer:
- Fixed a bug with the Mixology checkbox

Rawr.Server:
- A change in battle.net caused professions to not get parsed correctly

Rawr.DPSWarr:
- Removed a bad reference from the CalcOpts xaml
- Fixed the positioning of the Advanced Tooltips on CalcOpts so they don't flicker
- Cleaned up GetRelevantStats, ensured it wasn't missing anything that HasRelevantStats had
- Changed HasRelevantStats (and similar overrides) to use the != 0 return true method for performance
- Survivability Score now functions like a Tank's would but divided by 1000. This means uses the same calc method and is soft-capped. Gets values from the Attacks that would affect a MeleeDPS role (mostly just Raid AoE). Also splits the score down for different types of damage like Fire vs Physical
- Fury's Precision now properly applies the 40% Bonnus White damage
- Removed some dead code
- Updated the Rage from Incoming Damage calculations to iterate through boss attacks using damage and uptime values. Still using the Cataclysm modifier of 2.5/453.3 though, need to determine the new modifier. This is a very small amount of change on the rage levels so it's not going to seriously throw off calculations for being wrong
- Fix for issue 20619: Fury Mastery value off. The Mastery value wasn't properly updated in last release, sorry :/
- Removed some unnecessary string actions that were hitting performance for no reason
- Special Damage Procs no longer attempts to process when you don't have a proc of that type. Saves performance
- Changed the Damage Taken Interval handling to new boss handler attacks with affects role filtering instead of just aoe attacks, regardless of target
- Special Effects handling now looks for intervals of Infinity and ignores them as 'intervals which dont actually occur'
- Determination of Fury stance which read talent counts in trees before now just uses the Talents class's new count methods which are more generic and automated for updates
- Fix for BloodCraze 3/3 health value
- Changed Rotation.InvalidateCache function to just provide the 3dim array's values at once instead of iterating through them to set them all to -1. Saves performance
- All Impedances now filter based on the new Roles check
- All Impedances are now affected by the new Phase Uptime methods
- Updated the AverageTargets calc for White Damage to new methods (Phase Uptime, caching the value, Roles filtering)

Rawr.Healadin:
- Applied Two Patches

Rawr.Mage:
- Fix for glyphs, Glyph of Arcane Missiles and Glyph of Arcane Barrage were swapped
- Fix for Issue 20682: Focus Magic on buff list by default when importing from battle.net - Removed the application of the buff in Mage's SetDefaults
- Updated frost cycle, I don't like it, but it's probably better than it was before

Rawr.Moonkin:
- PTR changes, round 1:
* Initial cut of T12 2-piece and 4-piece set bonuses
* Bugfix for duplicate/missing gem templates
* Bugfix for old mastery breakpoint tooltip still being displayed
- 4.2 PTR, round 2:
* A tentative implementation of DoT spells generating Eclipse energy
- Show true mastery percentage, rather than the floored percentage from pre-4.1

Rawr.ProtWarr:
- Fix for Issue 20600: Loading a character with the Avalanche enchant would crash - Needed to use a RelevantTriggers statement to filter out effects that it's not set up for
- Replaced Mitigation Scale with Survival Soft Cap/Hits to Survive scaler, similar to Bear
- Properly average the value of temporary Mastery effects (e.g. Trinkets) to cap out their effective value while active, rather than averaging as if the entire value was effective
- More accuratly calculate the special effect trigger chance of Block/Parry triggers by adding base Parry from Strength and Block from Mastery prior to calculating special effects
- Removed Resilience display/relevance as it does nothing in PvE now
- Fixed tooltip regarding Heroic Strike frequency
- Increased Devastate base damage from 336 to 854

Rawr.Tree:
- Removed Dead Code from CalcOpts
- Excludes weapons from off-hand
- Support for T11 Set bonuses
- Fixed Glyph of Rejuvenation not showing up

Rawr.Warlock:
- Fix calculation of hasted number of DOT ticks
- Fix Sorrowsong calculation
- Use consistent handling for trinket and non-trinket relevance (fixes, e.g. chest stats enchants)
");
#endregion
#region Rawr 4.1.04 (Apr 29, 2011) [r60390]
VNStuff.Add("Rawr 4.1.04 (Apr 29, 2011) [r60390]",
@"Cataclysm Release

General:
- Tank Models now have new methods in the SetDefaults function. They will dynamically adjust the starting values for bosses based on the avg ilevel worn. This is to make the model start with more appropriate values for it's level.

Rawr.Base:
- Fix for Issue 20418: 'T11_0' is not a valid value for TierLevels error on character file load - Had updated the other possible values but forgot T11
- Added a Rawr Web Help dialog to display individual help topics across Rawr directly in the program
- More Web Help work
- Updated TalentClassGenerator with some new code for getting tree counts
- Updated Talents to 4.1.0 (Build 13726) values
- Updated TalentClassGenerator with a couple fixes TNSe identified
- Character.GetXiLevel now ignores tabard, shirt, projectile and projectile bag slots
- Welcome Window in WPF now recalls last 5 loaded character files and lists them for you, newest on top
- Implemented Import Talent Spec from Wowhead feature. Still need glyphs data to be updated for most classes to work
- Glyphs audited, now fully standardized and updated to 4.1.0. Current positioning will allow import talent spec from wowhead to function correctly
- Fix for defect 20537: Exception thrown sorting by subpoint values because the array indexes were off
- Fix for Issue 20551: Save Character File... vs Save As to Character File... not working correctly
- Added line to ensure lastSavedPath gets updated
- Fixed a bug with Save Character vs Save Character As

Rawr.BossHandler:
- Updated lower melee attack damage values per WoL parsing
- Argaloth: Separated Meteor Slash into Group 1 and Group 2 with different affects roles and the attack speed halfed (meaning it would hit each group alternately)
- Argaloth: Meteor Slash now actually targets players
- Argaloth: Consuming Darkness updated to Ranged
- Argaloth: Consuming Darkness now actually targets players
- Argaloth: Consuming Darkness updated to AoE
- Argaloth: Fel Firestorm now actually targets players
- Argaloth: Fel Firestorm's movement is breakable because it's movement
- Argaloth: Fel Firestorm's movement now actually targets players
- Magmaw: Pillar of Flame's movement is now properly set up to be valid
- Magmaw: Parasites TargetGroup now affects roles
- Omnotron: Corrected Magmatron's attacks
- Maloriak: Added Red Vial phase attacks 
- More T11 work
- Overhauled modelling to allow Phase setups, which are much easier to set up and provide more information
- Argaloth, Magmaw, Maloriak and Atremedes are each about 90% modelled now 
- Upped Atremede's melee damage to 85% from 75% of normal
- Added a HasAProblem bool to BossOptions, to be used later
- Added Help button to the Pane
- Added Alert! section to the Pane, to be used later
- Mangle fixed to DamagePerTick instead of DamagePerHit
- Blazing Bone Constructs Melee is now marked as from an add
- Ignition fixed to DamagePerTick instead of DamagePerHit
- Caustic Slime Damage adjusted
- Caustic Slime Debuff Roles fixed
- Onyxia's Shadowflame Breath fixed to DamagePerTick instead of DamagePerHit
- Animated Bone Warrior Melee is now marked as from an add
- Nefarian's Shadowflame Breath fixed to DamagePerTick instead of DamagePerHit
- HasAProblem bool moved down to BossHandler from BossOptions
- HasAProblem changed to return a string stating the issue instead of bool
- Boss Summary Message now states ALERT! and the HasAProblem string. Also turns the text red
- Special Bosses and their TargetGroups and Impedances have had thier routines updated to the new method used by Attacks routines. Allows for far more dynamic and accurate assignment of averages
- Special Bosses Melee now distinguishes between Boss Melee and Adds Melee to prevent the low adds melee from improperly affecting EZ and Avg bosses
- Attacks, TargetGroups, BuffStates and Impedances now provide more information when they are invalid, telling the user exactly what's wrong
- Attacks Editor now properly adjusts the decimal points of Damage Per Hit when changing to and from Damage is Perc
- Some corrections and updates in other Boss Editors
- Updated some of the default values for a Boss
- Added a lot of Documentation to Maloriak
- Adjusted a few damage numbers especially for heroic abilities
- Updated phase lengths on a few bosses in Blackwing Descent
- Updated a few Heroic only info on said bosses

Rawr.Buffs:
- Fix for Issue 19984: Flask Mixology and Double Pot Trick mutually exclusive - Not actually mutually exclusive per the code set up. The problem was in the ValidateActiveBuffs code wasn't null-checking the strings in the ConflictingBuffs list, so it was allowing a null to match a null. 

Rawr.Enchants:
- Added flag to determine that the enchant is only applicable to Shields 

Rawr.Items:
- Updated all meta gems
- Updated all trinkets that use the PhysicalAttack trigger
- Removed Gift of the Godfather trinket (not in the game...YET)
- Updated filters so that all T11 bosses, Baradin Hold, and PvP is shown by default 
- Fix for Issue 20438: Shard of Woe missing the mana cost reduc - Fixed the Item
- Fixed an unrelated typo with another stat as well 
- Fix for Issue 20477: Razorshell items don't really exist - Removed 56511 and 56515
- Updated Razor Edge Cloak with missing Mastery stat
- Updated Item Cache and Item Filter with 4.1 items

Rawr.LoadCharacter:
- Fix for Issue 20433: Unable to save/load to/from Rawr4 Repository - Replace wasn't in the right spot
- Fix for Reload from Rawr Addon not properly marking items available

Rawr.Optimizer:
- Fix for Issue 20490: Optimizer crash when adding talent requirement and not selecting talent - Changed it so when you select talent requirement it auto-selects the first talent instead of staying blank. Will blank out when you select away from it, this way it's impossible to start an optimize with an empty box 

Rawr.Bear:
- Reordered some of the calculations for better grouping and placed some extra comments and better variable naming to reduce confusion for someone new looking at the code. No actual calculational changes occurred
- Fix for Issue 20425: Feral Tier 11 four-piece bonus for cat form being utilized in bear form in Rawr.Bear - Reimplemented Feral T11 Set Bonuses to new Set Bonus method and updated them to the current standings (verified PTR Wowhead said the same thing for forward compatibility)

Rawr.Cat:
- Partial Fix for Issue 20559: Razor-edged Cloak and Meta Gem - Meta Gem templates updated for Cat
- Fix for Issues 20586, 20587, 20588: Mangle Buff and T11 4P doesn't properly affect Rotation (also causes Glyph of Mangle to not have Value) - Changed logic of application of Mangle
- Updated Default Buffs

Rawr.DPSDK:
- Fix for Issue 20443: Crash on selecting Unholy Talent Spec - Dark Transformation wasn't passing in CombatState as part of the constructor. When calling GetTotalThreat later, would fail on null error.
- Additional Pet implementation
- Fix custom charts
- Update UH rotation to only call Dark Transform if spec'd for it

Rawr.DPSWarr:
- Fix for Issue 20451: Unable to do single-minded fury - Couple of checks weren't looking to see if the OH was empty first
- Fixed a bug with BossHandler modelling in DPSWarr
- Reviewed all PTR 4.1.0 changes and ensured up to date with latest changes
- Migrated Set Bonuses to new method
- PTR 4.1.0 changes made standard

Rawr.Enhance:
- More work on tying Enhance module to the Boss Options
- Armour Spec cleanup
- Removed Spell Resistances based on level differences
- Fix for Crit Display
- Elemental Precision shows properly in spell hit display
- Crit from agility/intellect was being counted but not displayed

Rawr.HealPriest:
- Checking in some stuff I had forgotten to check in. Nothing still works
- Continued work on HealPriest for 4.x
- Updated Priest stats
- Added GetDamageReductionFromResilience
- Wroom Wroom. Changes be made. Still not useable for anything but dolling
- Further work
- Even further work
- Work in Progress!
- Fix for ComparisonCalculationsHealPriest class being totally fubar
- Work goes onwardeth
- I need sleep now. v2
- Gnaw Gnaw Gnaw
- OMG, we may have a winner!

Rawr.Hunter:
- Fix for Issue 20434: Crash Loading Character - CalculateTriggers function wasn't using best practices, so the add function could cause breaks

Rawr.Mage:
- Switching to 4.1 default

Rawr.Moonkin:
- Heart of Ignacious
- Change the trigger rate of Heart of Ignacious. It procs on all damaging spell casts
- Migrated T11 Set bonuses to the new method
- Changed handling for T11 2P
- Mainline 4.1 PTR changes now that the patch has been released
- Converted T11 4P to use a cached single static instance of the special effect rather than generate a new one each calc

Rawr.Retribution:
- First draft of new rotation. See Optionpanel
- More Work on new Rotation
- Change to new SetBonus handling
- 4.1.0 Ready

Rawr.TankDK:
- Fixed the top two most expensive operations in calcs. Now runs at 1/6 original speed
- Allow users to optimize by Mastery
- Adding Stats Graph to help stat valuation research

Rawr.Tree:
- Efflorescence changed to 4.1 handling+fixed to now actually consider heals to multiple targets
- Lifebloom bloom nerf of 4.1

Rawr.Warlock: Some work on verifying basic numbers
- Update BaseStats for warlocks
- Fix T11 tooltips, move to new set bonus model
- Add a PTR mode checkbox under Options | Debug, tied to relevant values (shadow mastery, shadow bite, haunt)
- Fix mastery, spell power display to more closely match character sheet
- Fix bug with talented/inherent multipliers to dot damage
- Fix bug with Drain Soul damage calculation
- Update for 4.1; fix some rounding errors
");
#endregion
#region Rawr 4.1.03 (Apr 04, 2011) [r59318]
VNStuff.Add("Rawr 4.1.03 (Apr 04, 2011) [r59318]",
@"Cataclysm Release

Rawr.Base:
- Fix for Issue 20403: Crash on Reset All Caches - The new Set Bonuses framework has an issue where it may not have initialized yet at the time it's called for this process. Changed it so that it will recalc at any time of being called when it's still null 
- Fixed implementation of special effects that are melee or ranged attack but dont require landing
- Work for Task 20079: New Mana Cost Reduction Variables (Nature and Holy) - Implemented in Elemental, Healadin, Moonkin, Tree, RestoSham. Marked Relevant in Enhance, HealPriest

Rawr.LoadCharacter:
- Fix for Issue 20407: This was a backwards compatibility issue
- Fix for Issue 20387: Cannot upload to Rawr4 Repository - The Post command doesn't like periods in the save text. Changed the default pattern use spaces instead and added on the back end to replace periods with spaces if it finds them

Rawr.Cat:
- Fix for Issue 20408: Fluid Death/Tia's Grace uptime terrible - Effect wasn't being processed properly due to a typo. Fixed

Rawr.DPSDK:
- Fix for Neg DPS values in some cases
- Recent chance had importing values that were already in the local instance

Rawr.DPSWarr:
- Fix for Issue 20404: Crash on file load - The actual error was a Stack Overflow. The GetBuffsStats was set up incorrectly with Kavan's new Set Bonus implementation, it would loop inside itself instead of calling the Base GetBuffsStats like it was supposed to

Rawr.Mage:
- Disabled Pots from Buffs from showing up in Mage

Rawr.ProtWarr:
- Fix for Issue 20405: Survivability significantly higher in this release - The Damage Taken multipliers were not being applied properly due to previous bad handling in two places and only one of those places having been fixed in last release
- Minor clean-ups following issue 20405
- Fix for issue 20324 (stacking trinket procs are non-functional)

Rawr.RestoSham:
- Fix for Issue 20399: Crashing on any action - RelevantStats wasn't following proper standards

Rawr.ShadowPriest:
- Removed a duplicate assembly reference in xaml

Rawr.Warlock:
- Fix for Issue 20398: Crash when Chaos Bolt is selected - The Talent Values (from the Bane talent) were set up wrong. Fixed
");
#endregion
#region Rawr 4.1.02 (Apr 03, 2011) [r59297]
VNStuff.Add("Rawr 4.1.02 (Apr 03, 2011) [r59297]",
@"Cataclysm Release

Rawr.Addon:
- Addon update
- Fix for Issue 20341: Error when exporting upgrade list - Needed a null check on subpoints

Rawr.Base:
- Reset Cache fix for Retribution => new Character () will now assign the current models' class
- Bug with Copy to Clipboard on the error window
- Changed the method for saving CSV files that allows for better Silverlight compatibility
- Added an additional handler specifically for Security Exceptions when saving a file to a location where it doesn't have permissions to. The message warns the user instead of throwing the error dialog
- Stats Spring Cleaning. Many stats removed and several moved to the correct lists
- Partial fix for defect 20358
- Fixed a typo in the item cache
- Rest of fix for 20358
- Removed the process for caching Buffs, Enchants and Tinkerings in xml. The intention behind it isn't used by anyone
- Fixed several stats that should have been in the InverseMultiplicative list. Had to adjust it's usage across Rawr in all models

Rawr.BossHandler:
- Magmaw should be all in. A few of the attack speeds are slightly off but abilities are all in
* Significant Overhaul:
- T7-T10 Commented Out from Code, will no longer be updated with other Boss Handler Changes
- T11 modelling updated in some areas
- DoT class removed, merged with Attack class
- Content and Version have been combined to a single selection (T11 10N, T11 25N, ...)
- Some default values for Attacks updated
- Smarter Validate checks on Attacks
- Smarter ToString calls on Attacks
- Bear provided with a new MyModelSupportsThis setup which hides all the options it's not using
- Old Tier Content points in enums and arrays removed. Only T11 stands at present for simplicity
- Parry Haste Completely removed from Rawr, including Parry Haste not generated by the Boss Handler
- Basics removed from the Summary message, unnecessary
- Several Boss Defaults updated to use T11 content from T10
- The Hardest Boss renamed to An Impossible Boss
- Added Smarter Attack compilations for Easy, Average and Impossible Bosses
- Added Comments to the Special Bosses explaining what they are and when you should use them. These comments show in the Summary message
- UI rearranged:
- - Instead of Accordions, using Tabs, and fewer of them
- - Instead of the DynamicCompiler string showing on the Edit button for Offensive and Impedances. The button just says Edit
- - Added a label for each Offensive and Impedance to display the list of all of that type
- - Several points on the Basics tab compressed to take less space
- Simplified the MyModelSupportsThis hide commands
- Attacks Editor updated to use DoT points as well
- Added Mechanical to Mob_Type
- Updated all T11 Bosses with Mob Type
- Fixed Spelling of Omnotron's name (they are not OMNIpotent)
- corrected a few damage numbers for Magmaw 
- Impedances, Target Groups and Buff States classes now have the AffectsRole variable like Attacks do
- UI for Affects Role added to the related dialogs
- No data for these implemented yet 
- Updated StandardMeleePerHit values based on some WoL parses
- Fixed the naming of the attacks on The Average Boss and An Impossible Boss
- Put in a higher Maximum for DamagePerHit on the Attack Editor
- Nefarian is modeled
- Added PhaseStartTime and PhaseEndTime to BuffStates, TargetGroups, Attacks and Impedances

Rawr.Items:
- Fix for Issue 20351: Items with wild ability multipliers - Retribution stat changes in base have caused a couple of items to get bad multipliers on them. Removed the bad references
- Agony and Torment Set Bonus: Set allowed Classes
- Refreshed 'Leggings of the Burrowing Mole' to reflect changes

Rawr.ItemFilters:
- Fix for Issues 20370/20379: Unknown items that can still be identified as PVP will be listed in the PVP filters under '?' and no longer be listed in the normal Unkown filters

Rawr.LoadCharacter:
- Fix for Issue 20366: Addon Import affected by filters for available items - Forced it to ignore filters
- Partial Fix for Issue 20358: Loading a Character from the Addon may mark duplicated Enchants to off - Changed implementation to check if Enchant is already marked first before using toggle
- Added toggling for Tinkerings since it wasn't done yet

Rawr.Server:
- Additional handling for the Blizzard Server is Down for High Volume message

Rawr.DK:
- Fine Tuning base stats. More to do here
- Implement RE
- Fix white swing hit chance to properly use WhiteHit rather than Yellow Hit
- Cleanup VampiricBlood implmentation
- Add Rotation list null checks
- Fix OnUse Trigger evaluation
- Didn't fully update the welcome page for DK models
Rawr.DPSDK:
- Fix Presence Combobox. It actually changes presences now
- Fix Presence custom chart
Rawr.TankDK:
- Fix Rotation report
- Total refactor of the evalutation logic flow to provide more complete burst value
- Implement Death Pact
- Update Trigger handling in special effects for tanking related triggers
(And with this, TankDK is 'Fully' implemented. - I'm sure there are still bugs and such, but that can be dealt with.) 
- Revamped Options Pane for ease of use
- Tweaking some of the output values via code review
- Updated the order, naming and colors of the SubPoints
- Changed the Threat Slider to a combobox selector as seen in Bear
- Fixed a bug with SetDefaults
- Updated Values for several DK Abilities
- Fixed Threat Multiplier
- Fixed some other minor bugs
- Changed how the Scores are reported to the user, now use the same scaling system that Bear does + Burst

Rawr.Enhance:
- Base Stats adjustement
- Updated Gemming Templates to use correct meta
- Add default Talent Spec and Buff Set

Rawr.Hunter:
- Fix for Issue 20349: Rotation numbers way off - This was caused by a use of the wrong value in a multiplier

Rawr.Mage:
- Fix for Issue 20347
- Updated optimal arcane cycles based on PTR
- Fix for Issue 20357
- Set bonus cleanup

Rawr.Moonkin:
- 4.1 PTR: The Eclipse buff no longer rounds down to the nearest percent

Rawr.ProtPaladin:
- Updated the subpoints
- Changed the SpecialEffects stats processing to use recursive effects

Rawr.Retribution:
- Fix negative Crit value
- Exorcism Proc Chance fix
- Stats spring cleaning

Rawr.Rogue:
- MG can proc Combat Potency, uses MH damage and is displayed on the overview

Rawr.ShadowPriest:
- Initial Mind Spike & Mind Blast burst damage calculations. These calculations are not using any special effects, cooldowns, gem bonus right now. so the numbers are a little low
- Remove setting of OverallPoints value
- Minor updates to calculations
- Add Inner fire usage

Rawr.Warlock:
- Fix issue 20363
- Also update mana cost of Immolate and Corruption
- Add unmodeled Warlock Glyphs
- Add Glyph item IDs to all Warlock Glyphs
");
#endregion
#region Rawr 4.1.01 (Mar 27, 2011) [r59144]
VNStuff.Add("Rawr 4.1.01 (Mar 27, 2011) [r59144]",
@"Cataclysm Release

Rawr.Addon:
- Partial fix for 20265: Exporting random suffix items
- Rest of fix for 20265: Exporting random suffix items
- Fix for Issue 19980: Weapon Upgrades will be not imported into the Rawr Addon - If the Weapon was of type OneHand or TwoHand then it would come back as a slot id of 0 which would invalidate it. Added handling for those types to come out correctly. 

Rawr.Base:
- Fix for Issue 20240: When Ranged is Relic show no enchants - Change to IsEnchantRelevant to do a class check on Ranged enchants, if not a Warrior, Hunter or Rogue none of those will be listed
- Completed Feature 19788: Adding some extra save options - Silverlight is unable to accomodate this due to permissions issues - WPF will now remember the file as loaded so you can just save without having to Save As - WPF now has Save As (separate from just Save)
- If suffix ID is out of range, this will return "" rather than throwing an exception. (I'm seeing this with some T10 items)
- Fix for previous commit that hid enchants for relics items. The filter logic was flawed and would end up hiding nearly all enchant
- Fix for Issue 20249: Item Source Editor in WPF version crashes - WPF doesn't like the empty Strings from some of the lists. Implemented an IF statement for xaml syncs to hide it in WPF and show it in SL (which doesn't have the issue)
- Fix crash if DisplayUnusedStats is set to false
- Task 19758 Completed: When loading a new version of Rawr for the first time, you will be prompted to reset your item cache so you can recieve any updates made
- Fix for settings dialog
- Fix for Fury Swipes Talent icon
- Fix for Issue 19755: Performance w/o network connection is bad in WPF: Icons were trying to be loaded from wowhead. Icons are now cached locally when found so they don't need to be called again and again. Will provide a sample cache of icons with next release
- Forgot a couple !SILVERLIGHT statements
- Added a sample images.zip file which holds about 1450 icons downloaded for WPF
- Implemented Right Click menu for Paper Doll Item Buttons and their main Selector lists

Rawr.Buffs:
- Fixed the numbers for Tricks of the Trade (Glyphed)
- Updated a couple of Enchants and Buffs to use Spirit instead of Mp5

Rawr.BossHandler:
- When loading a character from the armory or addon some default boss settings will now be enforced for Ret and Tank models NOTE: Model devs can add more defaults to this
- Undid the Validate BossHandler settings function add
- Replaced with SetDefaults implementation
- Started back up on work on the Boss Handler. A few abilities were added to Magmaw and Chimaeron
- Changed the Targeting mechanic for Attacks to state AffectsRole[Role] instead of IgnoresRole
- The Sub-classes and Enums have been moved to a separate file
- Changed the Dictionary to SerializableDictionary for attack targeting
- Updated Argaloth and the number of targets affected by Consuming Darkness
- More work on Magmaw and Chimaeron abilities

Rawr.Charts:
- Fix for Issue 19783: Tooltip flickers in gear list - It wasn't accounting for the Wide or Widest settings for name plates
- Added flag to force slot name in with enchant/tinkering name
- Simplified GetEnchantsCalculation function
- Simplified GetTinkeringsCalculation function
- Added 'Gear|All (This is Slow to Calc)' Chart, shows all gear from all slots
- Added 'Enchants|All (This is Slow to Calc)' Chart, shows all enchants from all slots (with slot name in the label)
- Added 'Tinkerings|All (This is Slow to Calc)' Chart, shows all tinkerings from all slots (with slot name in the label)
- Moved Item Sets chart up one
- Added 'Available|All' Chart, shows all gear, enchants and tinkerings marked with Diamonds from all slots
- Completed 'Available|Gear' Chart, shows all gear marked with Diamonds from all slots
- Completed 'Available|Enchants' Chart, shows all enchants marked with Diamonds from all slots
- Added 'Available|Tinkerings' Chart, shows all tinkerings marked with Diamonds from all slots
- Completed 'Direct Upgades|Enchants' Chart, shows all enchants that would be a direct score increase from all slots (with slot name in the label)
- Added 'Direct Upgades|Tinkerings' Chart, shows all tinkerings that would be a direct score increase from all slots (with slot name in the label)
- Added 'Dark Intent' to the 'Buffs|Raid Buffs' chart
- Removed Projectile and Projectile Bags from visible slots on any chart
- Added Search function to the Live Filter box. Enter Text (or regex if regex checked) and press Shift+Enter or click the 'Adv Search' button to be taken to a new chart that will search the entire item database and enchants and tinkerings. This search includes items that would normally be filtered out (such as the malorial filter turned off, will still display maloriak items, also items not for the class, like plate for Bears)
- Fix for Issue 20314: Wrong Min/Max Steps - When the larger scale was less than zero, the step going nuts correction would overcorrect

Rawr.Enchants:
- Fix for Avalanche, it's Duration was set to 10 instead of its Cooldown
- Avalanche SpecialEffect fix 2: 5 PPM on Melee AND 20% with 10s ICD on Spell
- Updated a couple of Enchants and Buffs to use Spirit instead of Mp5

Rawr.Items:
- Fix for Items that have a Purchasable cost not having enough spaces in the fully built return description string
- Added Item Id Column to the Item Browser
- Added Source column to the Item Browser
- Added Regex Searching to the Name field (checkbox is next to it to turn it on). With Regex on, you can use the full power regex to find an item by Name, Id or Source. Example: 5 ID's '(50100|50101|50102|50103|50104)' or '5010[0-4]'
- Option to use Regex stored and recalled with window open and close
- Added Large View button to the Item Browser. When active it nearly doubles the size of the window for expanded viewing
- Option to use Large View stored and recalled with window open and close
- Fix for the Refresh button in the Item Browser
- Fixes for the PvP Vendor type in the Source Editor. Corrected max token counts and added Honor and Conquest points to each box
- Updated BuffCache
- Updated EnchantCache
- Updated Settings Cache
- Updated TinkeringCache
- Updated ItemCache - Fixes Issues 20201, 20202, 20203 and 20204 as well as some other source updates
- ItemBrowser Sorting by Name works now
- Fixes for ItemLocationLists
- Fixes for ItemLocation Types
- Added Binding column to the Item Browser and matching Filter list
- Removed Projectile and Proj. Bag from the Filter by Slot list as they have been removed from the game
- Added Cogwheel and Hydraulic to the Filter by Slot list
- Changed the PvP section of the Item Source Editor Child window
- Updated Item Cache with a lot of source updates/fixes and removed many items you can't get anymore
- Fix for Issue 20239: Recent checkin altered several items incorrectly - Applied suggested fixes
- Completed Feature Request 15554: Display Unhandled Stats and Procs on items more clearly - Added second stat list that is grayed out and shows unused stats
- Show Unused Stats is now tied to a setting which is defaulted off
- Work for Issue 20081: Need New Trigger for when 'Power' is below 20% - Trigger has been added and marked Relevant for Cat, Hunter and Rogue - Devs will still need to model the proc interval and chance for this trigger
- Fix for Unused Stats message not clearing
- Used Stats message now does not show when all stats are being used (prevents clutter)
- Fix for Issue 19979: Missing trinket effects with the <35% player HP trigger - Properly modelled all of the trinkets with this trigger and their stats. Verified they all refresh properly. - All Tank Models (except TankDK) are now modelling this trigger
- Updated Item Cache and filters
- Updated known items not giving correct sources
- Updated filtering to get Vortex Pinacle and Throne of the Four Winds to work correctly
- Task 20080 Completed: New Highest Secondary Stat variable - Added implementation to DPSDK, Rogue and Hunter - This completes all models that need to worry about it
- Task 20081 Completed: Put in approximations for the Trigger of when power is below 20% as 80% chance every 4 seconds. This was applied to Cat, Hunter and Rogue (the only three that use it)
- Small update to Darkmoon cards and Meteor Shard Dagger proc rate

Rawr.LoadCharacter:
- Fix for Issue 20245: Reload Character from Battle.Net uses HTML encoding for special characters - Changed it to read the name/realm off of the Character object instead of the UI
- Completed Feature 19591: Import from Addon doesn't flag gems as available - Added a checkbox to the dialog for this. Will mark gems in your currently equipped gear as available NOTE: We dont recommend doing this in practice
- Loading toon no longer refreshes every item on the toon, only items which are either not in the cache or are in the cache but haven't been loaded yet (aka the name says Downloading from wowhead)

Rawr.Optimizer:
- Fix for Issue 19667: Optimizer showing items that do not require modification - There actually were changes being made. The reforging Id was being brought down in iterations of 56. Placed a check in where reforge id's are loaded in to prevent this (will load them into the proper values first, so they wont be reassigned again later)
Rawr.Optimizer.Batch:
- Fix for batch tools
Rawr.Optimizer.UL:
- Completed Feature 18769: Evaluate Upgrade to show negative values - When the upgrade list returns as empty from Evaluate upgrades by slot or Build Upgrade List, it will add a dummy tag to tell you its empty and how you could try to fix that - When the upgrade list returns as empty from Evaluate Upgrade (single item) it will add the item in with it's negative value in comparison to what you are wearing

Rawr.Server:
- Updates to Rawr.Server post-migrations
- Fix for loading Glyphs that have an ' in the name. They were coming from the page as &#39; so it wouldn't recognize the glyph name
- Fix for parsing Tinkering Id from items

Rawr.Tinkerings:
- Fix for Quickflip Deflection plates having wrong id

Rawr.Bear:
- Fix for Issue 20269: Periodic Crits No Longer Apply Savage Defense - Set the Lacerate TicksPerSecond used in this calc to 0 from 1/3 so it no longer applies

Rawr.Cat:
- Work for HighestSecondaryStat

Rawr.DK:
- Implement DPS breakdown. It's not perfect due to some hax in other areas, but at least there's some infrastructure
- Tweak to Virulence tooltip

Rawr.DPSDK:
- Fix for Issue 20250: Virulence shows old tooltip - Updated the Talent info
- Work for T11 Set Bonuses
- Fix a problem with DCs
- Fix the T11 set bonuses
- Fix triggers so that we can trigger on Death Runes
- Broke the RP & DeathRune counters
- Implement Killing Machine
- Implement Sudden Doom
- Tweak Defaults for BossOptions
- Fix Rime implementation in Frost rotation so it's less hacky
- Fix BuildCosts() so it can be called repeatedly within a given solver progression
- Fix the options pane to use Accordion UI

Rawr.DPSWarr:
- Fixed a bug with the options pane
- Work for HighestSecondaryStat
- Support for Dark Intent
- PTR Mode for 4.1.0 work and is now active
Rawr.DPSWarr.Arms:
- Fix for Issue 20220: Arms model not working - Latency/Reaction times were getting stuck on default cached values, added another enforcement check
Rawr.DPSWarr.Fury:
- Slam offhand swing was not counted by flurry/etc procs calculations for SMF builds
- Fixed flurry procs by multitarget/multiswings abilities
- Fixed combat table OH attacks O20/U20 to use appropriate numbers instead of 'all' attacks
- Modified RageNeededOverDurU20/O20 'battle trance' rage cost reduction 

Rawr.Enhance:
- Updates to gemming templates
- Work for HighestSecondaryStat

Rawr.Healadin:
- Removed Mail,Cloth, and Leather from calculations to illustrate the Plate Mastery advantage. Will add them back in once Plate Mastery is completely modelled
- Refreshed Healadin Glyphs

Rawr.Mage:
- Support for Arcane AoE cycles
- Improved support for AoE
- Option to use boss handler instead of model settings (only using some basic settings for now)
- Started work on fire AoE cycles
- Updated Combustion model
- Updated to latest PTR

Rawr.Moonkin:
- Fix a mistake with Euphoria energy return
- Omen of Clarity proc chance lowered to 2% from 6%
- Fix a minor calculation error in the calculation of Glyph of Starfire/Nature's Grace interactions
- New charts:
* Rotation Selection: Shows the details of all rotations that Rawr.Moonkin generates
* PTR Buff/Nerf: Shows graphically the effect of any DPS-related changes on the current test realm
- Display actual average cast times for MF and IS instead of the GCD
- Fix the haste issue with calculating DoT ticks, as per the EJ Balance Druid thread
- Adjustments to movement DPS. Not finished yet
- Laid some behind the scenes groundwork for multiple target DPS

Rawr.Retribution:
- Combattable implementation
- Add more constants, Reorganized the calculations
- Fix BonusSpellPowerMultiplier
- Added Plate Specialization
- Usage of BossOptions for Combattable
- Tooltip Update
- Added 2 Options, Ability Hit / Crit /sec fix
- Tooltip update
- Removed double crit suppression
- Added some retribution specific Charts
- Trigger reorganization
- Itemfilter update
- Relevancy Methods update
- Base ActiveBuff update
- SealDot Damage fix
- PVP 4P Bonus 180 => 90
- Base stat correction
- SoC / SoR DPS corrected
- Exclude Cons from Spell trigger
- WhiteHit Trigger added
- Retribution Bonus corrected
- Partitial resists removed
- Statsgraph from DPSWarr added
- Sacred Shield Talentree update
- Talent spec added
- T11 P4 support
- Inq per HP Chart added
- JudgementsOfThePure support
- Talent check for some talents
- Multiplier correction
- Glyph of Exorcism support
- Weapondamage normalized added to the breakdown
- Combattable extended
- All skills reorganized
- use base ValidateArmorSpecialization
- Combatstats replaced
- Inq profits now from 8% Spelldamage 
- Multiplier.Others String Override
- Glyph of Consecration
- General Info added (Tooltip)
- Meteor functionality added
- SoR JoR Damage fix
- Start GoaK modeling

Rawr.Rogue:
- Fixed AP multiplier for Str and Agi procs
- Implemented T11 set bonuses
- Fix for glyphs in Asssasination templates
- Added Mastery to stats overview

Rawr.ShadowPriest:
- Update 'Shadow Power' multiplier
- Shadow Power has been changed to only a 15% bonus
- Add some whitespace to calculations
- Refactor relavant trigger checks
- Add SubPoints for burst dps
- Refactor some useful parts of calculations
- Fix for wrong Color class usage

Rawr.TankDK:
- Fix an issue where in certain situations, invalid Boss attack data can really screw up values
- Implement Burst Subpoint
- Some Code Clean up and moved to be better. More to do on this course
- Total Refactor the DeathStrike and Blood Shield code
- Fix for Bone Shield causing negative avoidance values
- Implement UI for Burst Weight
- Setup Framework for the Rotation Report
- Fix for TankDK getting stack overflow from adding a default attack when there isn't one. Applied same fix to Bear just in case

Rawr.Tree:
- Fix for Issue 20225: Nature's Majesty talent not being applied - Was being applied but on each spell rather than just a base bonus spell crit percentage. Moved the add to core
- Mana regen modelling improved, probably still an issue here (Combat regen doesn't match armory values)
- Changed SustainedPoints to be based on a fixed tank healing rotation for now:
* Maintain LifebloomStack and Rejuvenation on the tank
* Swiftmend on the tank on Cooldown
* 1 Healing Touch per 10 seconds on Tank (Refresh LB stack + Use Omen Procs here)
* Use remaining cast time/mana to cast Nourish on the tank. (If sufficient mana remain, should cast extra Healing Touches, but not modelled yet)
- Changed fight duration to be based on boss handler settings
- Labeled Tree to be partially cata ready
");
#endregion
#region Rawr 4.1.00 (Mar 10, 2011) [r58642]
VNStuff.Add("Rawr 4.1.00 (Mar 10, 2011) [r58642]",
@"Cataclysm Release

Rawr.Addon:
- Partial support for export to Rawr Addon from upgrade list

Rawr.Base:
- Turning off ElevatedPermissions requirement for OOB mode. This may fix automated updating for OOB installs
- Silverlight and WPF versions both have new update protocols to check in addition to what they already have. This should better alert users when new versions come out
- Fix for crash when loading a character that has items equipped which are not in the database
- Refactored BossHandler functions, ordering and adding of Default Values to reduce XML file clutter
- When generating a new BossOptions because it was null, will automatically add a Default Melee Attack (helps Tanking models with their default values, especially Bear)
- Refactored Character class, ordering and adding of Default Values to reduce XML File Clutter and added comments
- This commit has an overall reduction of Character files with minimal settings changes by approximately 25 lines
- Migrated Rawr.Base to Root
- Rawr2 unused file removals from Root
- WPF: Better version check handling
- Fix for random suffix items crash
- Fix for random suffix items crash in batch tools
- Fix for Item Cloning
- Organized the files into folders
- Migrated Rawr.UI to Root, since there was no Rawr2 version of this directory or its children, there was nothing to delete prior

Rawr.BossHandler:
- Added Mob Types variable to the Boss Handler (Defaults to Humanoid)
- Added Default setups for MyModelSupports this against MobType
- The three Special Bosses now determine what mob type they are by counting. If the highest count is same number as Humanoid, will revert to Humanoid

Rawr.Buffs:
- Fix for Issue 19984: Flask Mixology and Double Pot Trick mutually exclusive - Added Conflicting Buffs statements for Double Pot Tricks, I swear if I have to change this stuff again.

Rawr.Enchants:
- Fix for Issue 20067: Avalanche & Hurricane - Hurricane already done, Updated Avalanche to DamageDone trigger from MeleeHit

Rawr.Items:
- Updated Mercury-Coated Hood
- Added Hornet-Sting Band
- Added Throne of the Four Winds items in preparation for new suffix update
- Added Tier 12 ilvl uniqueness
- Added early support for new Tier 12 trinkets
- Added support for Nature and Holy Mana Cost reduction
- Added support for Highest Secondary Stat
- TODO: several new trinkets do not have any ICD or duration information at this time
- Task 19987 Completed: Alchemist Stones need to be mutually exclusive - Added exclusivity between 50400, 50386, 50399, 50378
- Fix for Issue 19810: Darkmoon Card: Hurricane proc is Incorrect - Changed the value to a 5% proc for now
- Fix for Issue 19390: Engineering cogwheels not working properly - Added another Cog selector to gemming templates. When the Templates in each model get set up correctly (or custom added by user) they will show up correctly
- Fix for Issue 19975: PVP Set parsing incorrectly - Added a Replace to kill any instance of the word Vicious from set names
- Fix for Issue 19530: Quest rewards not flagged as unique - By popular demand (aka bitching) Quest Reward items will be marked unique upon refresh
- Updated all raid item sources in BoT, BD, and Tot4W
- Updated all Tier token item locations
- Fixed Bell of Enraged Resonance proc
- Fixed all Cata Alchemy Stones
- Updated all 325 - 333 quest items (so that they are showing as Unique Equip)
- Updated Suffix information for all Random Enchant items found in Tot4W
- Updated Cata Jewelcrafting gems and armor with Source information and Binding Type
- Added Suffix information for JC rings and Necks
- Updated Source information and Binding Type for Cogwheels
- Updated source and binding type for all Blacksmithing crafted items

Rawr.ItemFilters:
- Fix for Issue 20034: Filter by Profession hides non-profession items - Shirts/Tabards are no longer profession filtered by that check. The lances don't matter and no real item would have this problem.

Rawr.Optimizer:
- Fix for crash with random suffix items
- Results should display score depending on what you optimize for
Rawr.Optimizer.UL:
- Fix for Issue 20110: Upgrade list not thorough enough, missing item(s) - Implemented 10x Ctrl Click for Upgrade List too

Rawr.Server:
- Fix for glyph parsing

Rawr.Tinkerings:
- Fix for Issue 20100: Incorrect duration for Synapse Springs - 4.0.6 Update for Synapse Springs

Rawr.Bear:
- Fix for Issue 20043: Armor Multiplier, Survival Soft Caps - Fixed Armor Multiplier
- Upped Soft Caps by 25% of current values
- Updated Gemming Templates to include proper Cogwheel templates

Rawr.Cat:
- Updated Gemming Templates to include proper Cogwheel templates

Rawr.DK:
- Fix for 20091: DamageModifier problem causing ability value inflation causing HUGE values in most abilities
- Removing old files for Rawr4 migration/reintegration back to trunk
- Update to new Cogs format

Rawr.DPSDK:
- XML optimization & adding in some defaults
- Migrated files to Root
- Fix a null reference problem on clean characters
- Fix for Issue 20007: Rune of Fallen Crusader wasn't working or being excluded properly
- Implement CinderGlacier
- Fix RazorIce
- Implement Rime (it's super hacky right now)

Rawr.TankDK:
- Adding an XMLIgnore in there to help shrink the XMLs a bit
- Fix for 19864: Default over-healing should not be 0%
- Migrated files to Root
- Fix issue 19851: Lack of Haste value in RSV because HasteRating wasn't being properly handled

Rawr.DPSWarr:
- Migrated DPSWarr files to Root
- Added several DefaultValue flags to Calcopts, reduces saved file size when defaults aren't changed
- Updated Gemming Templates to include proper Cogwheel templates

Rawr.Enhance:
- Fix for display issue on options panel
- Reimplemented Stats Graph
- Fix for Issue 20070: 1% spell hit difference between WoW and Rawr - Display issue only, added Draenei hit check to the tooltip and value
- Migrated to Root
- Further work on Export to EnhSim
- Remove an unused file

Rawr.Healadin:
- Task 19775 Completed: Option to not display spirit items should be removed - Removed
- Working Issue 20187: Spirit not a relevant stat - Added Spirit as a relevant stat

Rawr.HealPriest:
- Update the available glyphs's for priests

Rawr.Hunter:
- Updated basic Pet stats (Health, AP, Armor, Crit should be correct)
- Sgen for Hunter project
- Removed the PetBuffSeletor UI and back end. This was removed for Cata
- More updates to Pet stats
- More work on calculations
- Updated Gemming template with Cogwheel info

Rawr.Mage:
- Changing arcane light setting to default enabled
- Fix for by spell breakdown
- Improved quadratic solver support for int and mastery procs
- No dot ignites in ptr mode (you should manually lower ignite munching factor)
- Implemented PTR changes, arcane aoe cycle solver
- Fixing improved ae, changing gcd latency to 0.01 default

Rawr.Moonkin:
- Fix treant hit calculations, now that I've resolved the issue with my in-game testing
- Remove the Treant Hit display, as it is mostly irrelevant now
- Delete Rawr2/Rawr3 code in preparation for migration of Rawr4 code to root folder
- Migrate code to base
- Fix cogwheel gemming templates
- Don't need to add the cogwheel templates to every tier of the gemming templates
- Fix for Issue 20175: Fixed the treant hit mistake

Rawr.ProtPaladin:
- Mongoose should not be adding armor
- Fix for Issue 20039: Agility provides Armor - Agility was still increasing armor, removed. Also removed from HealPriest and ShadowPriest
- Fixed bug with Seal of Truth being way too high on threat

Rawr.Retribution:
- First Changeset, mostly adaption of new skills
- Gems copied from DPSWar, DPS Breakdown changes
- Old Code removed and attack table fix
- Removal of old code and some bugfixes
- Rearranged Additive / Multiplicative
- Added T11 support
- Damage values mostly done, some old code removed
- HighestStat support, Crusader Glyph and a bugfix of additive and multiplicative stats rearrangement from yesterday
- Cleared up the simulation for better performance, some damage fixes
- First raw draft of rotation
- Inqusition handling, reorganizing some code
- Removal of old code
- Migrated to Root
- Further removal of old code and some improvements
- Unused option removed
- Fixed Ret's options pane constructions back to the standard for all models, the DataContext stuff wasn't being assigned correctly
- More options removed
- Removed the Bloodlust checkbox from the Options Pane, tied the Ret back end to trigger this from a user actually selecting the buff (or it's equivalent)
- Bosshandler useage
- Updated Gemming Templates to include Cogs
- Forgot to add the spell crit buff as default
- Crit fix

Rawr.Rogue:
- To speed up optimizing all mobs are assumed to be poisonable. Assorted fixes for Assassination. First work on Combat
- Ported a fix Poison fix made in Ass to Combat and Subt. Forgot to remove a line added only for testing
- More work on Combat. Switched glyphs in Prime and Major glyph categories. Added glyphs of Blind and Vanish. Added saved talent specs for Assassination and Combat (glyphs don't load yet).
- Fixed glyph numbering. Glyphs now load when picking a saved talent spec
- Small updates for Ass / Combat and first pass at Subt. HoT is overvalued (assumed to proc on CD) and Ambush / Shadow Dance aren't (fully) in yet

Rawr.ShadowPriest:
- Update the available glyphs's for priests
- Verification of the Shadow Priest Model Stats Calculations
- Ensure basic stat calculations are correct for all priest races /w standard talent set/untalented and three item sets
- Cleanup
- Update gemming templates
- Remove unused files
- Migration to Root

Rawr.Tree:
- Modified spell durations, coefficients and mana costs to 4.0 values from Restocalcs.xls
- Some talents updated
- Something still looked weird on spell crit values
- Mana regen not yet touched
- Started work on a hopefully more elegant way to reduce copy+paste code in Solver.cs
- Most talents now implemented (Tree of Life doesn't model different spell behaviour)
- Fixed issue with crit percentages
- Mastery now implemented at the spell level and used in New custom graphs
- New custom graphs include effects of Revitalize (not replenishment) and lifebloom refresh from Emp Touch
- Top level simulation and mana regen still need work
- Fix for Issue 20104: Spellpower from Spirit - The model was no longer using it but the line was still there. Commented out the line to prevent confusion
- Commented it out from HealPriest as well

Rawr.Warlock:
- Fix for issue 20112: Mark the Rare gem templates as enabled by default
- Fix Cogwheel templates
");
#endregion
#region Rawr 4.0.20 (Feb 20, 2011) [r58200]
VNStuff.Add("Rawr 4.0.20 (Feb 20, 2011) [r58200]",
@"Cataclysm Release Beta

Rawr.Addon:
- Professions export with None instead of empty XML
- Mage talent bug not fixed in client 4.0.6 - export checks for 4.0.6 now

Rawr.Base:
- Replaced the caches with all same encodings
- Fix for LocationInfos being null
- Fix for Pawn string to export Mastery Rating
- Fix for Filter side-bar starting on Drop Rates instead of Sources
- Support for random suffix items. Still needs to be done: wowhead parsing, armory import, rawr addon import/export. Needs a lot of testing
- Fix for random suffix display in optimizer results
- Fixing some bugs introduced with random suffixes
- Fix for Issue 19978: Scaling on random charts too large - In some cases, the rounding method can cause the step scaling to go nuts. Added a loop to reduce the step and bring it back in line in the event that it does this
- Updated Base Stats per Chardev for all races/classes. There may be still some info that is a little off but we're far closer than we were
- Feature 18505 Implemented: Refresh Only Items Currently Worn - Added a function to do this to the Import menu
- Feature 15546 Implemented: Bars showing relative to equipped item - Added setting to the Options dialog to enable this - NOTE: It is NOT recommended to turn this on as it makes the charts harder to read
- Feature 15606 Implemented: Equip Option within Item Edit - Added Equip button to Item Editor
- Feature 9028 Implemented: Character Comparison - Added the ability to compare the currently loaded character to another xml file using the optimizer results window. Select this from the Tools menu (under item sets)
- Fix for Issue 19747: Tweaks for WPF popup placement
- UI Work for Talent Spec importing, but it's not done yet as it needs a back end
- Updated Wowhead url loading of PTR items (Rawr was still using the Beta's cata.wowhead, instead of ptr.wowhead)
- Handling of random suffix items not present in item cache, performance improvements

Rawr.BossHandler:
- Task 18222 Completed: Create a method for Editing Attacks, Impedances, etc - Selecting things in their respective lists will populate the UI so they can be changed. Use the Add/Edit button to push any changes to the list

Rawr.Buffs:
- Added Dark Intent

Rawr.Items:
- Fixed Incorrect stats for Feral Druid Tier shoulders
- Fix for parsing PvP token costs on item sources
- Fix for Zone names going bad on source parsing
- Updated Item Cache with a lot of fixed sources. Especially PvP, Justice Points and Relic Items
- Updated Alchemist Stones data
- Fix for Gladiator's Regalia 4 Piece Bonus

Rawr.ItemFilters:
- Implemented Filter by Bind Type UI
- Removed 'Disable by Bind Type' filters from the Tree
- Implemented Filter by Professions UI
- Removed 'Disable BoP Crafted' filters from the Tree
- Implemented Filter by Drop Rate UI
- Removed 'Disable by Drop Rate' filters from the Tree
- Fixes for Issue 20000: Filtering Less than Optimal - Multiple Source Data with valids + not founds [FIXED]. The Item Cache will automatically purge itself of these scenarios on load
- BoP Crafted filters with new UI method not working [FIXED]. It was checking for BoA instead of BoP... stupid bug
- Cleaned up filters of old currenies no long in game and made them less messy to work with current parsing info
- Fix for Gem filter not working as intended
- Combined 'Wildhammer Clan/Dragonmaw Clan' as they are faction specific vendors

Rawr.Optimizer:
- Added notice next to the Optimize button about Ctrl+Click for 10x thoroughness
- Better optimizer support for random suffixes, xaml sync

Rawr.UpgradeLists:
- Feature 17387 Implemented: Remove Item from Upgrade List Option - Added ability to do this from the context menu. Removing an item cannot be undone
- Feature 15396 Implemented: Build Upgrade List for a Slot - Added 'Evaluate Upgrades for Slot...' to the item context menu, verified it worked on processing only items in Head slot (checked other slots too)
- Feature 19524 Implemented: Export Upgrade List to CSV format - Added Export options to UL, CSV in clipboard and saving CSV to file (saving to file requires install offline mode)

Rawr.Bear:
- Fix for Issue 19976: Thick Hide in 4.0.6 changed - Updated Bonus Armor Multiplier values for Thick Hide to 4.0.6
- Implemented DamageAbsorbed stat, which only shows how crappy the stat is for Bear

Rawr.DPSDK:
- Fix for Issue 20005: Implement filters like DPSWarr for gear
- Associate for 19726: Some additional fleshing out of ghoul data
- Adjustments to the Unholy rotation. It's really screwy, but far better than it was

Rawr.DPSWarr:
- Updated Bloodthirst stats with 4.0.6 values
- Single-Minded Fury bonus has been increased to 20%, up from 15%
- Bloodsurge procs add 20% damage to instant slams
- Slam hits with both weapons for characters with SMF talent
- Don't use WW in rotation by default
- Raging blow deals 120% weapon damage up from 110%
- Battle Trance talent now affects fury calculations
- HS and CL don't use GCD as it was in old model
- Fixed execute phase calculations
- SMF-specced warrior calculations use dual-wield miss chances instead of one-handed miss chances
- Updated Raging Blow values for 4.0.6a patch
- Fixed base mastery value
- Fixed stances damage modifiers
- Fixed infinite rage and 'hit doesn't matter' problem - 'Rage Details' still shows that many rage is unused, but rotation uses other 'source' until common solution for arms and fury is implemented
- Code cleanup
- Added 18 Feb hotfix for arms(Two-Handed Weapon Specialization (Arms passive) now gives 20% bonus damage with two-handed weapons, up from 10%)

Rawr.Enhance:
- Gemming templates updated
- Fix for mana regen
- Initial Major Display revamp (many things don't have any values beside them, but the values that do exist are the ones that were there before)
- Clean up of display in-case next release is pushed before my next check-in. 
- Removing reference to a temp working file (woops) 

Rawr.Hunter:
- Updated Talents and Glyph information for hunters
- Numbers pass on all shots. Still need more work with integration using Focus 
- Tons of Refractoring
- Added gemming templates
- Mastery and Specialization have rudimentory settings set up
- Cleaned up several Basic stats (RAP and Health should be correct or within a few points of live)
- Continued work on shot information
- Started work on pet basic stats

Rawr.Mage:
- Applying arcane hotfix changes
- Shard of Woe tweak
- Support for Dark Intent

Rawr.Moonkin:
- Further refinement to the model of Glyph of SS/Glyph of SFall with Starfall Lunar Only rotations
- Change Wild Mushroom cast time to 3 * 1 second global cooldown
- Add information to display about Starfall and Wild Mushroom usage in the rotations
- Update the coefficient on Wild Mushroom
- I had the wrong damage multiplier for Moonfury in the first place. Changed to the hotfixed 4.0.6a value
- Fix another mistake where I was double-counting the baseline 8 points of Mastery
- Update the calculations for Moonfire and Nature's Grace to better match the spreadsheet
- I also believe I improved upon that last by providing an actual calculation for GoSF and NG, rather than just plumbing in a guess of 50%
- Performance minded change, should improve performance by ~25%. Unsure as to the full ramifications of the change
- Implemented Mastery flooring as per Elitist Jerks. Note that this totally messes up the Relative Stat Values chart
- Add a button to display the stats graph. This will show the haste breakpoints and the mastery stepping function

Rawr.ProtPaladin:
- Now modeling Holy Shield. Thought that was already in there...

Rawr.RestoSham:
- Removed level 80 option, fixed HW and GHW calcs to better reflect real world

Rawr.Rogue:
- Implemented Honor Among Thieves. First steps in splitting the solvers, the output is complete bogus now
- More work on splitting the solvers, still useless output
- Fix for Defect 19962: Errors when compiling the SL version
- Changes to Assassination solver, Expertise and Hit calculations, gemming templates and display tooltips
- Implemented Energy regen from Haste
- Assassination: Made some change (forgot what) but the stat values seem to be right now, the overall DPS is just low. Once that's fixed I'll go on to Combat
- Fixed damage reduction from boss armor
- Potent Poisons and Vile Poisons stack additively. Rupture dmg is shown on the overview. SnD increases melee speed
- To speed up the optimizer SnD use is only considered with 4 or 5 CP. Split off Venemous Wounds damage from Rupture damage
- Fix for Poisons double dipping Mastery

Rawr.Tree:
- Fix for Issue 19674: Intellect not increasing Spellpower - Applied it at 1:1

Rawr.Warlock:
- Updated gemming templates to use Burning Shadowspirit Diamond; removed Wrath templates
- Updated stat relevancy to include Power Torrent, Hurricane, and offhand intellect
- Updated mastery bonuses for Demo and Destr for patch 4.0.6
- Updated effect of Inferno talent
");
#endregion
#region Rawr 4.0.19 (Feb 06, 2011) [r57865]
VNStuff.Add("Rawr 4.0.19 (Feb 06, 2011) [r57865]",
@"Cataclysm Release Beta

Rawr.Base:
- Fix for Charts not starting on 0.0
- Fix for Boss Handler's Average Boss averaging very slow melee attacks with normal melee (skewing the results)
- Fixed base physical crit and spell crit for paladins 

Rawr.MultipleModels:
- Support for WoW 4.0.6

Rawr.WPF:
- Removed System.Web dependency

Rawr.Buffs:
- Added 10% and 15% options for Luck of the Draw buff

Rawr.Items:
- Added Love is in the Air 346 ilvl neck drops 
- Withered Dream Belt having haste instead of mastery

Rawr.LoadCharacter:
- Fixes for loading characters with Russian text
- Removed TW and CN regions from loading from Battle.Net as they don't work anyways and won't work for a while. Added a notice to the dialog to this effect
- UI for Force Refresh option on loading character from Battle.Net
- Added UI for saving character files to repository and recalling them

Rawr.Cat:
- Switched to using BossHandler
- Fix for Leather Specialization

Rawr.Bear:
- Switched to using BossHandler

Rawr.Enhance:
- The BonusWhiteDamageMultiplier stat application wasn't set correctly

Rawr.Mage:
- Changing Improved Mana Gem to 15 sec, fix for 4T11
- Added AB4ABar1234AM and AB3ABar123AM cycles, added a note for mana neutral mix showing a mix of what it is
- Cooldown restrictions editor
- Fix for sequence reconstruction chart in WPF
- Numerical stability improvements for advanced solver

Rawr.ProtWarr:
- Support for all patch changes to abilities and talents
- Fix for Improved Revenge not granting enough bonus damage
- Support for Shield Block uptime calculations (now displayed in Block/Total Avoidance tooltips)
- Support for Heavy Repercussions (if Shield Block is enabled)
- Lowered default threat weight to 10% and increased default avoidance weight value
- Proper fix for Hold the Line threat values
- Support for additional Mastery and Parry gemming templates 

Rawr.ProtPaladin:
- Touched by the Light and Plate specialization now modeled
- Fixed relevant glyphs
- Fixed mastery calculation, block should be reading right now
- Touched by the light fully modeled
- Fixed spell power calc 
- Added 0.25 stam to 1 parry conversion
- Fix to toughness and bonus armor mult calculations 
- Fixed attack power calculation 
- Fixed a couple issues with Hammer of the Righteous 

Rawr.Moonkin:
- Update math to latest WrathCalcs spreadsheet:
- Move Starfall, Treants, Wild Mushroom calculations into the Rotation calculations.
- Add rotations with Starfall cast mode and Wild Mushroom cast mode.
- Remove rotations with Starsurge cast mode, is always on cooldown now.
- Remove Once and Unused rotations from dot cast modes.
- Corrected relevant NG and Eclipse calculations 
- Supposed to fix the issue with Sorrowsong where the value isn't right sub 35%. 
- Fix to Wild Mushroom calculations
- Make the glyph of Starsurge do something when the selected or burst rotations are set to Starfall Lunar Only
- Better modeling of the effects of the Starfall/Starsurge glyphs on Starfall Lunar Only rotations.
- Fix Starsurge glyph on rotations that are not Starfall Lunar Only.
- Fix Glyph of Focus. 

Rawr.Tree:
- Updates to include mastery and spell power. Cleaning up some bad logic
");
#endregion
#region Rawr 4.0.18 (Feb 03, 2011) [r57766]
VNStuff.Add(
"Rawr 4.0.18 (Feb 03, 2011) [r57766]",
@"Cataclysm Release Beta

Rawr.Addon:
- Fix for issue 19861 - exporting Rawr addon data wasn't working if item had a location with quotes in it
- Update version number in export to latest commit

Rawr.Base:
- Fix for Issue 19849: Exception on using the Add Item button twice - The dialog being reused in Silverlight worked fine but for WPF it causes problems
- Removed the Reusage method and now it generates a new one instead
- Fix for resetting interpolation cache when editing special effects
- Support for Triggers that only proc at sub 35% target health. Models need to add the uptime modifiers themselves

Rawr.BossHandler:
- Updated heroic 25 Nefarian Health based on Paragon's video
- More updated Boss health pools
- Small update to Boss Handler

Rawr.Buffs:
- Fixed for overvaluing Resistance bonus from Mark and Kings
- Fixed working item 19865: Stealskin Mixology bonus being undervalued

Rawr.Items:
- Updated trinkets that use the 35% triggers
- Corrected Tanking trinkets incorrectly using the target's execute trigger

Rawr.LoadCharacter:
- Fix for Issue 19848: Error Loading 'character.xml' Files - Character files without the Name property set come in as null charcater name, so when it's loading in and hitting the new escaped characters check, it's throwing a null exception - Added a fix to ignore empty character Names and Realms
- Fix for Issue 19859: Can't load character from EU-Голдринн Added Голдринн to EU Server List
- Fix for Issue 19746: Striping out hyphens from the server names that have it when making a character request
- Fix for Issue 19739: Load from Battle.net/Rawr Addon doesn't mark Ring Enchants as available - The same problem that was happening before with having 2 of the same item was happening to ring enchants. Broke it out and overrode the check to allow it to mark them. Verified with the example character

Rawr.Optimizer:
- Fix for Issue 19831: Listing Items as Changed When They are Not - Changed the visibility options for the parts of the display, now everything is shown side by side so it's obvious what has changed and what hasn't

Rawr.MultipleModels:
- Task 19371 Completed: Melee Modules need support for BonusWhiteDamageMultiplier - Added Relevancy, Get and usage of BonusWhiteDamageMultiplier to Cat, Bear, ProtWarr, Retribution, ProtPaladin and Enhance

Rawr.Cat:
- Fix for Issue 19832: Unheeded Warning proc not working - Added WeaponDamage stat usage in CatAbilityBuilder constructor. Relevancy and Get were already in

Rawr.DK:
- Adjust some additional talent tweaks based on in-game values
- Implement Rage of Rivendare
- Fix for Defect 19598: validate White swing code

Rawr.DPSDK:
- Basic Ghoul code started. Not plugged in yet

Rawr.Enhance:
- Fix for Issue 19581
- Initial work at display revamp
- Fix for Issue 19890: Ignoring custom Boss Level for hit/expertise caps - Enhance was set up to replicate a lot of variables from BossOpts over to duplicate variables in CalcOpts. Removed (almost) all of that and replaced it with actual BossOpts calls. Fire/Nature Resist and Target Groups are still using the bad method and should be fixed

Rawr.Mage:
- Fix for hasted evocation
- Fix for Issue 19858: Arg_NullReferenceException when trying to run optimizer
- Support for execute phase special effect triggers
- Fix for numerical instability in arcane solver

Rawr.Moonkin:
- Should fix the issue where it was crashing during optimization. I don't know what possessed me to write it the way I did
- Add a user option to enable or disable reforging Spirit to Hit rating, now that Hit rating has a slight tangible advantage over Spirit
- Fix for issue where pots were showing 0 DPS value

Rawr.Rogue:
- Enable default gemming templates. Fix some casts
- Refactoring. Implemented Ambidexteriry, Executioner, Improved Ambush, Ambush damage, Initiative, Sanguinary Veins, Preparation and Serrated Blades. Prevented bleed debuff double dipping. Removed Evis resetting SnD when spec'd in Master Poisoner

Rawr.TankDK:
- Fix for Defects 19769 & 19812 - though there is still some stats normalization that I need to do. Dodge & Parry are now correct and validated
- Fix for Defect 19863: DRW was screwing up my RP math. Not perfect, but it's alot better. Refactored Blood rotation
");
#endregion
#region Rawr 4.0.17 (Jan 28, 2011) [r57608]
VNStuff.Add(
"Rawr 4.0.17 (Jan 28, 2011) [r57608]",
@"Cataclysm Release Beta

Rawr.Addon:
- Display character frame & paperdoll frame on typing /rawr import
- Add check to comparison values to ensure not nil
- Tested fixes for import
- Changed to have 3 ranks of sounds
- Fixed issue with checking item ids with nil itemstrings
- Looting mobs tested - bug fixes
- Added warning frame and warning frame options
- Export to Rawr Addon now includes location string so tooltip can show where to get the item
- Added item location info from Rawr requires Rawr 4.0.17
- Allow Changes button to show if data previously imported
- Fix for Issue 19819: Import does not mark all items available - The toggler that sets it as available turns it on, then the second instance of the item turns it back off. Wrote a check to see if the item id has already been processed and ignored it if it was

Rawr.Base:
- Added a generic Armor Specialization check to the Character Class
- Base Stats for draenei shaman updated
- 30% boost to melee haste for shamans removed
- A proper fix for calculations trying to get called when character is not ready for it
- Don't show Jewelcrafters gems on gemming lists if character isn't a Jewelcrafter
- Jewelcrafter gems only excluded if Enforce Professions selected
- Loading addon data now loads model defaults
- Changed default reforging options to include reforging from nonrelevant stats
- Fix for defect 19823: Hunter model status was not on home splash page
- Fix for Specials Character in names issues from Patch 8190 submitted by yury2808
- Fix for Issue 19729: Enchant icons not showing - The cached copies of all the enchants didn't have the Icon Sources set. Forced it to rebuild the cache with new data. - Updated the XML and DefaultDataFiles.zip with a new default cache

Rawr.BossHandler:
- Updated Lady Sinestra's Boss Handler Information. Most spells and frequency are in place. Damages are estimates based on Paragon's video. All info is based on that video. Phase two is estimated at lasting 1 minute
- Updated Maloriak's enrage timers

Rawr.Buffs:
- Fix for Issue 19685: Enabling a Scroll does not disable Mixology bonuses - The check that would turn it off was being skipped due to the professions enforcement. Fixed to support both
- Fix for Issue 19731: Can only select one Mixology when selecting Elixirs - Verified after fix that you can select a Battle Elixir with it's Mixology and a Guardian Elixir with it's Mixology all at the same time

Rawr.ItemFilters:
- Disable By Item iLevel filters removed from Filter Tree
- Activated the Filters by Item Level Accordion at the bottom
- Filled out the Accordion with two option sets: Checkboxes which when checked will show that ilevel group and a Custom Slider where you can manually set a range you want to see
- Added Reset and Uncheck All buttons to the Accordion
- These filter values save to the Character object so they will be consistent with each save file managed
- The Sidebar will now remember how wide it was from last program launch
- Updated the separation of WotLK top end Cata low end to 284 from 277

Rawr.Items:
- Add Lady Sinestra's Cache drop information to filter system
- Updated Lady Sinestra's loot with Cache information
- More Source updates
- Updated Earthen Handguard normal & Heroic stats
- Updated 11.5 Token drop locations
- Added support for 4.0.6 change to Unheeded Warning

Rawr.Optimizer:
- Fix for issue 19834: Cogwheels/Hydraulics not being used by Optimizer

Rawr.DK:
- Talents.XML: Add some default specs. Some are missing glyphs, but should provide some additional guidance

Rawr.DPSDK:
- Pulling out unused Files
- project update from earlier check in
- Fix for 19773: Nerves of Cold Steel was not implemented despite what my comments had suggested
- Null checking in the fix for NoCS

Rawr.DPSWarr:
- Updated Meta List to add the new Strength Meta to the templates. Will remove other now useless templates once 4.0.6 hits

Rawr.Enhance:
- Initial work at cleaning up the code
- Export to Enhsim is now working for WPF version. Numbers may be slightly off
- Removal of the 30% boost to hybrid haste
- Added glove tinkers to EnhSim Export
- Further work on getting Enhsim export accurate
- New Export to EnhSim for Silverlight is here!!

Rawr.Mage:
- Updated mana cost based on hot fix notes
- Fix for mirror images in averaged mode
- Fix for Combustion cooldown segmentation
- Changing Mirror Image default to averaged
- Setting for ignite munching, PTR mode (hopefully I got all the changes, could use some review)

Rawr.Moonkin:
- PTR changes:
* Remove armor modifier from Moonkin form
* Add 30% damage buff to Wild Mushroom calculations
* Also changed the Wild Mushroom calculations to 650-786 base damage per mushroom, rather than split across all 3 mushrooms. I have no idea if this is correct
- Switch the Wild Mushroom calculations to match the spreadsheet:
* Reduce coefficient to the spreadsheet value. (Needs confirmation)
* Re-reduce damage to 650-786 across all 3 mushroom
- DEFAULT_GEMMING_TIER changed to 1 - Array was zero based not 1 based so default of 2 was epic instead of rare. Epic gems of course don't exist yet in Cataclysm.
- Updates to treants:
* Treants do not benefit from raid-wide auras such as 10% AP, 5% crit, 10% haste
* Treants benefit from Heroism and 4% physical damage debuff
* Add a display value to show the amount of hit treants have, as well as hit rating to cap
- Correct display typo: StatConversion.YELLOW_MISS_CHANCE_CAP[PlayerLevel - TargetLevel] which always gives a negative and out of bounds exception should have been StatConversion.YELLOW_MISS_CHANCE_CAP[TargetLevel - PlayerLevel]

Rawr.RestoSham:
- Applied patch 8175 to fix non-beta model's GHW calcs

Rawr.Retribution:
- Split out active and passive abilities into separate classes
- removed conflicting abilities class 
- Fixed stats for Tauren, blood elf and human paladin
- Still need stam and spirit for dwarf and draenei
- Updated more Paladin Base Stats 

Rawr.Rogue:
- Rupt dmg updated. Energy costs updated. The optimizer will no longer use finishers with less than 3 CPs. Rupture and Recuperate are no longer assumed to be used with 5CP if used at all. Agi now gives 2 AP per point. Updated base AP and base Belf stats. Implemented Leather Specialization. OH dmg was penalized twice. Lots of talent/glyph updates
- More refactoring. Updated Assassination talents. Fixed base stat calculations
- Added Faerie Fire to the armor debuffs to consider when deciding about Expose Armor
- First pass at better DP calcs
- Removed an unused variable
- Fixed Rupt dmg

Rawr.TankDK:
- Implementation of some combat data metrics as requested in the discussion forums. This should help us figure out why the mastery numbers are so far off. Includes a tweak to the DS & BShield methodology. Which, while making things worse for Mastery valuation, is much more accurate. Looking at the avoidance numbers next
- Some Cleanup. Additional NoCS tweak
- Fixed Missing Dodge from Agi.
- Fix for Improved DeathStrike properly buffing Healing done
- Adjustment of Survival math to include all static damage reduction, not just armor
");
#endregion
#region Rawr 4.0.16 (Jan 16, 2011) [r57235]
VNStuff.Add(
"Rawr 4.0.16 (Jan 16, 2011) [r57235]",
@"Cataclysm Release Beta

Rawr.Addon:
- Drycoded adding options to play sounds on looting items that are upgrades. NOTE: NOT TESTED IN GAME YET!! ie: probably typos crashes etc ALPHA quality
- Update packaging to include LibSharedMedia-3.0
- Fix typos in packaging
- Add LibSharedMedia to embeds.xml
- Fix locales
- Add options to select sounds to play if an upgrade is seen
- Fix issue if slot in direct upgrades isn't loaded from cache yet
- Fixed shift clicking of Rawr slots or upgrade lists puts item links in chat
- Fixed ctrl clicking of Rawr slots or upgrade lists shows items in dressing room
- Added some default test sounds
- Tweaks to looting from looting window (dry coded)
- Force check to load media once initialized
- Changed to have 3 ranks of sounds
- Fixed issue with checking item ids with nil itemstrings
- Looting mobs tested - bug fixes
- Added warning frame and warning frame options
- Upgrade shows percentage of upgrade
- Tested Warning frame seems to all be working now
- Added various default sounds
- Added Loot Upgrade Check when Need/Greed roll window pops up
- Release as Version 0.62
- Added command line /rawr import as per website description

Rawr.Base:
- Updated EU and Korean server listings with missing server names (There are a few EU and several Korean server names that appear to be no longer listed on Blizzard's server status pages)
- Added new PTR enchants to the global PTR mode
- Added new Meta gem requirements for the new Agil, Strength, and Intellect metas
- Updated PTR information of Rune of Swordshattering and Rune of Swordbreaking with their reduction of 60% disarm effect
- Updated Welcome Window with Moonkin's Fully Ready status
- Fixed Tinkering showing up in Enchant listing
- Updated Racials with PTR info
- Added a generic Armor Specialization check to the Character Class
- Fixed the meta references for the three new metas, Hina had entered the JC spell to create them, not the metas themselves
- Updated the Armor Damage Reduction formula based on multiple sources. This appears correct though someone else should probably take a look at it to be sure
- Fixed Spirit proccing from Lightweave Embroidery (version 2)
- Updated Spell Crit Reduction from Boss level mobs from 2.1% to 1.8% based on EJ testing
- Adding some of the missing Racials

Rawr.BossHandler:
- Updated the Armor values list to what SimCraft is using
- Disabled the Armor value box and tied it's selected index to the Level value. This means if you select 87, it will auto-select 87's matching Armor
- Adjusted Dynamic Attack so that the attackspeed was the total attack interval rather than the interval between just 1 each attack on the list.

Rawr.Buffs:
- Fix for Issue 19643: Elixir of the Master (Mixology) missing - The improved was typo'd as 'Deep Earth' instead of 'the Master'
- Fix for Issue 19565: Inscription scrolls not correctly limiting - Changed the ConflictingBuffs lists again
- Fix for Issue 19656: Indestructible Potion Duration incorrect - Updated to Cata value
- Updated Synapse Springs, Darkglow Embroidery (Rank 2), and Weapon Chains with PTR information based on Blizzard's updated PTR notes
- Fix for Issue 19685: Enabling a Scroll does not disable Mixology bonuses - The check that would turn it off was being skipped due to the professions enforcement. Fixed to support both

Rawr.Items:
- Updated Left and Right Eyes of Rajh's chance to proc in PTR mode (50% chance to proc on Physical/Melee Crit)
- Added support for Mandala of Stirring Patterns's PTR On Equip effect
- Added PTR's PvP helm and shoulder enchants
- Small update to Profression created items
- Started work on adding abilities to Blackwing Descent Bosses for BossHandler

Rawr.ItemFilters:
- More adjustments
- Corrected a few errors with the use of the '(The\s|)' and the '.*' in '.*Vortex Pinnacle', just to keep it simple and added a few more filters. Combined Baradin's Wardens/Hellscream's Reach as they are just faction vendors with the same items just in different places. Also included a special curency tab for Tol Borad Commendation
- Corrected Error in The Bastion of Twilight filtering
- Feature 19675 Completed: Added Item Level filtering for Normal and Heroic Dungeons and Raids
- Corrected Error in 'Disable 346-358 Cata Dungeons (H)'
- Disable By Item iLevel filters removed from Filter Tree
- Activated the Filters by Item Level Accordion at the bottom
- Filled out the Accordion with two option sets: Checkboxes which when checked will show that ilevel group and a Custom Slider where you can manually set a range you want to see
- Added Reset and Uncheck All buttons to the Accordion
- These filter values save to the Character object so they will be consistent with each save file managed
- The Sidebar will now remember how wide it was from last program launch

Rawr.LoadCharacter:
- Fox for Issue 19657: Reload Character Crash - Was trying to remove items from a collection that was being iterated over

Rawr.Optimizer:
- Fix for optimizer

Rawr.Bear:
- Improvements to Vengeance calculations in Bear

Rawr.DPSDK:
- Fix for Issue 19648: XAML parse error on the options pane
- Implement BonusWhiteDamageMultiplier

Rawr.DPSWarr:
- Updated Meta List to add the new Strength Meta to the templates. Will remove other now useless templates once 4.0.6 hits

Rawr.Enhance:
- Daggers aren't relevant items

Rawr.HealPriest:
- Updated Priest Base Stats

Rawr.Mage:
- Fixes, support for Heart of Ignacius, support for mastery procs for fire and frost specialization

Rawr.Moonkin:
- Add a PTR mode switch and plumbing
- Add Moonfire/Sunfire mana cost reduction
- Add Eclipse Mastery buff
- I'm pretty sure I've implemented all the mechanics. Updating to Fully Ready

Rawr.ProtWarr:
- Some fixes against the StatsWarrior class and handling effect procs

Rawr.Retribution:
- Added a couple of exploritory classes for setting up a state manager
- Added a static constants class for a central location for coefficients, etc, for easy maintenance

Rawr.Rogue:
- Updated attack damage values to level 85. Removed Anesthetic Poison

Rawr.ShadowPriest:
- Updated Priest Base Stats

Rawr.TankDK:
- Fix for BoneShield & Vengeance based on dynamic attack info
- Implement BonusWhiteDamageMultiplier

Rawr.Warlock:
- Added Stat Graph to Info tab on Options pane
");
#endregion
#region Rawr 4.0.15 (Jan 09, 2011) [r57038]
VNStuff.Add(
"Rawr 4.0.15 (Jan 09, 2011) [r57038]",
@"Cataclysm Release Beta

Rawr.Addon
- Dry coded changes to import subpoints
- Fixed a missing comma on export
- Move bank items into savedvariable
- Reworked Gem exports to use gem ids
- Reworked export to Addon to include equipped score data 
- Import now shows DPS subpoints on tooltip on import paperdoll frame
- Fix color display of tooltips
- duCalcs are now passed to the export
- Prep work to use ItemSets for writing character & loaded character
- Prep work for exporting direct upgrades
- Refactored export to use ItemSets
- Changed Direct Upgrades button to show hide changes - doesn't hide at present
- Reworked Import to use new loaded/character import
- Release as Version 0.40
- Import now shows differences between what was loaded (from addon or Battle.net) and what was displayed when doing export - This means you can load up your character do some tweaks/optimisations load it back into Addon and see changes in game
- Tweak for dataloaded always being false on reloadUI
- Changed Tooltip to use custom tooltip
- Update minimum build & addon versions
- Refactored export to addon - introduced upgrades - needs subpoint values
- Added SlotId to Item - Needs testing with addon v0.41
- Added wait cursor to Export to Addon menu click.
- Only export upgrades for regular slots
- Fixes to DU data
- Added comparison tooltip - now shows difference between loaded and exported
- Fixed Bank Export
- Added Output on scanning bank
- Added GemId to enchantID routine - fixes display of gems IF user has seen gems in itemcache
- Added text to comparison tooltips to identify which is which
- Initial coding of upgrades frame - kinda right position does nothing at present - Have now got Icon buttons set and text objects reads, Layout looking a lot better too. Still no actual data populates the frame
- Added fix for professions being localised
- Update minimum addon version to import
- Implemented CheckBoxes on Import form for Direct Upgrades Processing
- Implemented CheckBoxes to select Display Upgrades Filter
- Implemented Select All/Clear All buttons
- Build Upgrade List now works on checked/unchecked items
- Direct Upgrades now scrolls and displays overall upgrade score.
- Icon textures not yet working though
- Added ItemIdToEnchantId.lua file should really be in Rawr4
- Now shows tooltip for Direct Upgrade items 
- Direct Upgrades now show tooltips and comparison tooltips
- Direct Upgrade scrolling can now also be done by mousewheel
- Release as Version 0.51
- Fix issue with first time use of addon
- Convert ItemIdToEnchantId.lua to a XML version for Rawr4.
- Added ItemIdToEnchantID files to TFS project
- Added GemIDtoEnchantId convertor changed Item.ToItemString to use EnchantId.
- Release as Version 0.52
- Changed Import to use GemEnchantId and not GemId
- Changed version on export to ensure 4.0.15 release works with addon.
- Added version check of Rawr data on import
- Release as Version 0.53
- Direct Upgrade values are rounded to two decimal places
- Tooltip values are rounded to two decimal places
- Added fix for Blizzard bug on Mage talents in patch 4.0.3
- Mage Talent bug is in spurious 15th talent in Arcane tree. 

Rawr.Base:
- Fix for issue 19503: Weapons capped at 2000 max damage - Was a type with Maximum Damage in the Item Browser. Set it to 10000 instead.
- Fix for Chaotic Skyflare requirements
- Fix for Issue 19418: Overwrite save set does not overwrite - Changed the way it checks to see if the set is in the list
- Adjusted the size of the Reforging, Enchanting, Tinkering and Blacksmith socket boxes to save some room on the UI for smaller screens.
- ItemSets are now saving/loading from character xmls, yay!
- Removing level-based partial resists
- Agi no longer provides armor
- Added a RetryCount check to the number of times it tries to load the caches. If it fails more than 4 times in a row, it will just throw an error and stop trying. The error will provide more instrucitons to the User about deleting their Silverlight Cache to try and get new ones.
- Fix for Issue 19542: EU-Malorne missing in Server List - Added
- Fix for Issue 19519: Tooltips with Large Amounts of Text Disappear - Swapped Tooltip from base to a Popup that stays until mouse moves away. Also added a Header usage as an option for a Bold line at the top
- Added GetItemSetByName to Character for Addon
- Added GetDirectUpgradesGearCalcs to GraphDisplay for Addon
- Calling GetItemSetByName('Current') will return the character's equipped items (null if they are null)
- Loading in a character from the Addon or Armory will automatically assign an ItemSet of the gear at that time - This can be used to compare all changes since last load and will be passed into the Rawr Addon to show comparisons
- Added Uldum to EU Server List
- Fix for Issue 19600: Wrong value for the Titanium Plating - Titanium Plating has only 26 Parry and it has 50% Disarm Dur Reduc. Updated
- Canceling a new, open or load character so you can save your file first should now result in the action actually being cancelled
- Fully removed several things from Rawr that are no longer in game:
- - Armor Penetration Rating and related value to rating and armor reduc from rating, etc functions. Armor Penetration as a Percentage still exists because of talents and abilities in some models, like Colossus Smash.
- - Defense and Defense Rating and related rating to value, etc functions. Defense has been completely removed from the game and all items and effects providing it have been converted to other stats like dodge and parry
- - Block Value, this stat no longer exists as all blocks result in a 30% value (modified by talents and abilities)
- - Crit Reduction from Resilience, this was removed from game
- - Armor from Agility, this was removed from game
- Stamina to Health conversion is now 14 from 10 per Cata changes
- Changed behavior of save file check before getting another file
- Fix for Issue 19370: Can't 'Open in Wowhead' when installed locally - This is a Silverlight issue, found a workaround and implemented. Tested fine
- Added a global PTR Mode to Options
- Tied 4.0.6 Meta Gem requirements to the PTR mode
- Corrected 4.0.3a Meta Gem Requirements for all Chaotic and Relentless Metas
- Fixed Tooltip Widths of several UI Elements on Options dialog by inserting character returns
- Added even more status info into Rawr to try and cut down on duplicate issues for inoperable models
- Description in Wowhead was incorrect with what Lightweave Embroidery procced. Procs 580 Intellect instead of Spell Power. Fixed enchant.
- Fixed an issue with array xml storage from a previous commit 
- Added a popup to the Install Offline button when you are already installed to alert the user to what they are doing and what they should be doing
- Enabled Reload Character from Battle.Net and Rawr Addon

Rawr.BossHandler:
- Task 18226 Completed: Add a flag for time periods where Boss takes Bonus Damage - Added BuffStates as a list on BH. Just like other Impedance and Attack lists it uses the Freq, Chance, Dur, system and also includes a Stats object to reflect bonuses or penalties to players. UI created as well

Rawr.Buffs:
- Added the new Cataclysm Elixirs
- Added Shamans to the Select Buffs By Raid Members Dialog
- Added Druids, Paladins, Rogues and Warlocks to the Select Buffs By Raid Members Dialog
- Buffs by Raid Comp should be fully functional now
- Fix for Issue 19565: Inscription scrolls not correctly limiting. Added the respective Guardian and Battle Elixir conflicts to Scroll Buffs
- Fix for Issue 19508: Values for Resistance Buffs are too high - Updated those buffs to 195 from 1105 (an old PTR value)
- Arcane Brilliance (SP) was incorrectly marked as coming from Shamans

Rawr.Items:
- Feature 15466 Implemented: Add Item Drop Rate to the Tooltip - Implemented Drop Rates for static drop items, will pull info on wowhead refresh or you can manually populate it with the Item Source Editor
- Fixed source parsing for Crafted Items. The skill type value was stored as an integer, not string so it wasn't converting properly
- Changed Crafted Items Desription function to not show the '(0)' if it's 0.
- Fix for Crafted Source UI editor, wasn't fixing nulls 

Rawr.ItemFilters
- Minor fixes
- You can now filter by Drop Rates. A default set of Disable filters was added but not sure if thats what we will decide to keep

Rawr.Bear:
- Added Avoided Interrupts % optimizable value

Rawr.Cat:
- Fix for Issue 19611: T11 2P Bonus wasn't relevant to Cat - Added BonusRakeTickDamageMultiplier to the HasRelevantStats function (was already in GetRelevantStats) 
- Added Avoided Interrupts % optimizable value

Rawr.DK:
- Tweak the GetSpec() function
- Add Mastery to Paperdoll output on TankDK
- Add SpellDamageTakenMultiplier so that Effluent Meta is properly evaluated
- Update Gem Template slightly
- Pull out dead function
- Adding in initial rough of DRW
- Adding in rough of T11 set bonuses
- Implmenting some of the 4.0.6 changes
- Fixed RuneTap implementation SE
Rawr.DPSDK:
- Work for Issue 19414: Models using old crit reductions - Updated DPSDK to pass BossOptions around so Target Level could be pulled from it and used for calcs.
Rawr.TankDK:
- Updating one of the BossHandler GetDPS functions to include Attack Speed adjustments.
- Adding Initial Impedence handling in the mitigation section. Things like Run Speed increases & Stun reductions should now have values when using a Boss that includes those kinds of impedences. You will see this in the tooltip of Mitigation values.
- Fixed/added some gemming templates

Rawr.DPSWarr:
- Stat Graph settings for Str and Agi checkboxes weren't assigning to the UI correctly, fixed
- Fixed the Heroic Strike and Cleave usage against Rage Cost variables, they needed to be inverted
- Implemented 4.0.6 patch changes for DPSWarr and tied them to the PTRMode check on the model Options Pane
- Both Warrior models are now using a StatsWarrior class instead of Stats. This reduces the overall size of the Base Stats class. Ours is fully implemented for all accumulate functions, etc. 
- Migrated several things to the new StatsWarrior object

Rawr.Enhance:
- Work for Issue 19414: Models using old crit reductions - Ensured no hardcoded level 85 or 80 numbers for character level were left, all now use the Character.Level at some point or another
- Fix for Issue 19511: Lava Lash CD appears to be too fast - Searched all references for Lava Lash and verified set cooldowns to 10s from 6s
- Work for Issue 19581: Elemental Precision not being factored. Added the necessary mod to the tooltip

Rawr.Hunter:
- HunterEnums Updated, new spells added, old spells removed. Update all references
- Refactor ManaCost -> FocusCost
- Update Base DMG / Cooldown of existing Spells
- Added Cobra 2 Shots
- Remove Some Mana Infos From Display
- Some refactoring MPS -> focus per secound

Rawr.Mage:
- New Arcane Light option - simplified arcane model using only mana neutral cycle mix and AB spam
- Fix for Flashburn
- Added T11 set bonuses
- Support for Gale of Shadows
- Updated base stats
- Fix for Pyroblast coefficient
- Updated Pyro dot uptime model
- Updated fire cycles
- More fixes and rework of Combustion (not finished yet)
- Heuristic adjustments to Pyro dot uptime and Combustion, switching to latest T3 HS formula

Rawr.Moonkin:
- Add an option to prefer either Hit or Spirit for reforging on gear
- Reducing the Wild Mushroom damage until I'm sure I know how hard the things hit
- Correct the math for Treants
- Redo the combat table to reflect new testing
- Redo swing speed, base AP, base weapon DPS
- Re-add Target Armor Reduction as a relevant stat and implement it. Still to do: Figure out what crit/haste buffs apply to the Treants and apply them
- Fix treant base DPS
- Correct the formula used to scale treant hit to treant expertise
- Add crit/haste raid buffs and Heroism to treants

Rawr.ProtPaladin:
- Work for Issue 19414: Models using old crit reductions - Ensured no hardcoded level 85 or 80 numbers for character level were left, all now use the Character.Level at some point or another

Rawr.ProtWarr:
- Fix for Work Item 19599: gemming templates now have the correct Austere meta gem and no longer default to JC gems turned on
- Both models are now using a StatsWarrior class instead of Stats
- This reduces the overall size of the Base Stats class
- Ours is fully implemented for all accumulate functions, etc

Rawr.RestoSham:
- Restored front panel look of resto sham stats now that beta model is disabled partially
- Mastery fix, wrong number used originally
- First fix for issue 19553
- First fix in place for 19409
- More work on the new model
- Fixed a bug with Water Shield in the current model
- Various fixes

Rawr.Retribution:
- The model still doesn't work but it at least doesn't crash on load character now

Rawr.Tree:
- Removed unnecessary fields to clean UI
- Traced variables and commented on usefulness in my tweaks of the solver
- Healing simulation updated

Rawr.Warlock:
- Fixed several array bugs and nullcheck errors
- Improve fix for issues 19488, 19391
- clean up item relevancy rules; fix bug causing GCDs < 1 sec
");
#endregion
#region Rawr 4.0.14 (Jan 01, 2011) [r56705]
VNStuff.Add(
"Rawr 4.0.14 (Jan 01, 2011) [r56705]",
@"Cataclysm Release Beta

Rawr.Addon:
- Added export of empty tinkered items until Blizzard adds API call for checking tinkers.
- Fix paperdoll display scaling issue
- Fix export of empty profession
- Lock frame to UIParent and use its native scaling

Rawr.Base:
- Added Tinkerings as a listing to the Equipped charts (under All and its own header).
- Fix for blue diamond toggle.
- Fix for default values of buffs.
- Updated support for single changes direct upgrades optimization method.
- Changed the Block% meta gem to only give a 1% bonus rather than the stated 5% bonus to match in-game testing
- Improved comparison charts for gem selection.
- Fix for gem selection when socket is empty.

Rawr.BossHandler:
- Adding some handling code adjusting what Jothay had put in. To deal w/ physical v. Magical attacks. And updated the one T11 boss implemented to match.

Rawr.Items:
- Major update to trinket proc modeling
- A few adjustments to a few melee enchants
- Updated Filter with several missing bosses
- Slight adjustment to Heart of Ignacious and Jar of Ancient Remedies procs

Rawr.LoadCharacter:
- Fix for dashes being allowed in the server name, and showing up as defaults instead of spaces.
- Restricted server names to the valid list of values, to prevent mistakes.

Rawr.Bear:
- Fix for broken Stamina multiplier from HotW.
- Fix for broken Health multiplier from Stamina.
- Improved attack power calculations.
- Fixed base attack power.
- Fixed Glyphs showing up, but only Mangle is modeled currently.

Rawr.Mage:
- Fix for quadratic solver.
- Fix for DotTick trigger proc.

Rawr.ProtWarr:
- Adjusted the base damage/threat and coefficients of a number of abilities
- White damage is no longer reduced by Heroic Strike usage
- Fixed Devastate having no value in the talent point view when using Sword and Board
- Added initial support for Vengeance--slider added to the options pane, defaulting at 60% stack
- Added support for Cataclysm gemming templates
- Adjusted yellow critical hits to use two-roll mechanics
- Added support for BonusBlockValueMultiplier stat
- Fixed Mastery base value double-dipping issue causing Block% to be too high

Rawr.DK:
- Fix issue w/ Base damage valuation for Spell v. Physical hit.
- Rotations: Provide pre-set rotations to help until solver is handled.
- Fix issue w/ rotation math coming up w/ weird values for rotation duration.
- Implement initial Scent of Blood work.
- Gem Templates for TankDK using new gems.
- Updated Relevant stats for DPSDK to exclude defensive stats.
- White damage wasn't properly included in the rotation outputs.
- Fixed base stats... they'll need some further tweaking.
- Fix for melee/spell special counts
- Use Pre-made blood rotation in DPSDK when in a Blood Spec.
- Update discription of TankDK DPS & Threat values to include max Vengeance.
- Fix for 16078: Display the rotation on the options tab and it's working w/o going crazy.
- Solver is now actually doing some work. The rotations don't always make alot of sense and it's way big on the DPS numbers, but it's now a working set.
- Setting TankDK as 'Mostly' since Survival and Mitigation values are looking reasonable. And threat is OK as long as it's using the pre-set rotation. DPSDK on the other hand still needs work. It's work will dial in the Threat on TankDK.
");
            #endregion
#region Rawr 4.0.13 (Dec 28, 2010) [r56600]
VNStuff.Add(
"Rawr 4.0.13 (Dec 28, 2010) [r56608]",
@"Cataclysm Release Beta

Rawr.Addon:
- Fix case of class name
- Release as v0.10
- Class Name for Death Knights should be DeathKnight capital K
- Fix for Night Elf & Blood Elf names needing no spaces and correct capitalisation
- Typo on elseif
- Fix for races
- add isValidVersion for more reliable version parsing in AddonLoadDialog
- Needs thorough testing for various invalids
- Import - build needs maxvalue to check if parse worked
- Added TODO comment about build number
- Add support files for Curseforge Localisations
- TFS project hadn't added locale files to project files
- Change warning on addon load text to indicate invalid data as a possible cause of bad import
- Release as Version 0.20
- Added button to Character Panel
- Added frame to display imported Rawr Data
- Update TFS to use Frame files
- Dry coded import button functionality needs testing
- Added import buttons (don't do anything yet)
- Added import.lua code file
- Changed icon to have lowercase lettering
- Export was missing trailing commas
- Change first line of export to named import function
- Tweak output format for import to addon
- Import now accepts data from Rawr
- Updated minimum version numbers on addon/Rawr interfacing with each other
- Now actually displays icons of stuff imported
- Missing comma on export
- Added UI for Rawr Export to AddOn
- Release as Version 0.21
- Added display of items on import frame with working tooltips
- Highlight item borders to show slot item rarity
- Selecting Equip Item from the context menu on the Direct Upgrades charts will now try to equip the item to the related slots, not just the Head slot.
- Fix crash if missing a profession
- Release as Version 0.22
- Fix crash if missing a profession
- Fix issue with non English locales exporting race name
- Moved buttons on Paper doll frame a bit
- Small tweak to button sizes

Rawr.Base:
- Added Item Set Comparisons!!! You can now use the Tools Menu to Add/Equip/Remove Saved Sets for comparison.
- Addon Importing now filters out items from your bags/bank that aren't relevant to the model. So if you have tank gear in your bags and you are wearing your healing gear, the model loads into the healing model and doesn't mark the tank gear available (as well as junk gear link Green Winter Hat)
- You can now save an Optimized GearSet to the ItemSets list. This does not save changes in the optimizer other than the items themselves (meaning no buff or talent changes)
- Started work on adding parsings for Cata trinkets. Currently have most 'On Use' effects in place.
- Cleaned up the 'On Use' special effects section so that procs are grouped up and duplicate lookups are removed.
- Added a BonusWhiteDamage stat so that Unheeded Warning can be modeled. Melee devs need to add this stat as a relevant stat.
- Updated all items that have been changed by Blizzard through their hotfixes
- Updated items and filter with Rare Mob drops information (MMO Champ missed a few rare mobs in their article)
- Started work on adding more import information from Wowhead.
- I stand corrected MMO Champ did have them all and I was the one who didn't have them all. Filters are updated
- Fix for Issue 19369: Unit Race doesn't seem to be working for Worgen - Worgen and Goblin didn't have the correct Race ID's set yet and the Race Selector on the Main Page wasn't set up right. Fixed both
- Item Editor no longer persists between usings, stops all issues with specific fields persisting
- Removed the duplicate 'basics2' reference from all the xaml's in Rawr.UI 
- Undid previous removal of basics2 reference, Kavan says it behaves differently for WPF 
- Updated all caches to clear out errors and post new settings and stuff 
- Fix for Issue 18787: Tooltips Clipped on Right - Added handling to move the tooltip to the left if its going off the right edge 
- Hid the option to Refresh Item from Armory since we aren't using that process
- Updated Welcome Window with FAQ entries
- ItemCache failover should now check to see if the ItemCache loaded was empty (this occurs when a previously bad item cache from an older version was accidentally saved as empty).
- Replaced the ErrorBox handling (again, sorry) to prevent the mishaps of things going to the wrong place on the resulting dialog.
- Fixed Character class wasn't copying CogwheelId & HydraulicId from gemming templates therefore wasn't using them at all
- Add a spell crit depression array. The only values I'm 100% sure of are Level+0 and Level+3 - the other two are just guesstimates. Could use some testing.
- Add a Rawr.Addon Save dialog in preparation for being able to export optimised character to Rawr.addon
- Save to addon implemented now exports data of equipped items to Addon from Rawr
- Fixed some level 83 references that should be 88 now across several models
- Added a StatConversion.GetRatingFromSpellHit funtion to complement StatConversion.GetRatingFromHit which was Physical Hit only
- Fix for issue 19447: float.Parse() does not work as expected for some culture settings - Implemented InvariantInfo setting on the version number check and added ',' as a possible delimiter in the RegEx so it will capture properly. 
- Added an option for people with low res monitors to hide the shirt & tabard icons and move the gloves to the left side, basically saves you a row of paper doll selectors
- Changed the Stats Pane to have the character basic info stuf moved into an accordion thats default collapsed. The info in the boxes is now tied to a label at the top. This saved you some space on the Stats pane
- Removed the usage of LocationFactory as a separate cache, there is now no need for ItemSource.xml
- Resorted the Wowhead.cs file so its easier to find stuff (had gotten rather haphazard)
- Working on getting Update Item Cache from Wowhead working. Initial code placed but throws constant InvalidCrossThreadAccess errors
- Moved several import/export dialogs to a new Character folder (instead of cluttering up the Dialogs folder)
- Polished up the Item Editor some more with Group Boxes
- Added Binds On selector to the Item Editor
- Added the ability to Compare an Item Set to your currently equipped items using the Optimizer Results window. You can also equip the comparing set from that window.
- The tooltips on the Item Sets chart entries now display as an Item List (like the bottom of the Build Upgrade List's tooltips) instead of a giant wall of text.
- Fix for Issue 19411: Error pop up on sorting and changing charts - Needed the special sort check in the new chart switch sorting
- Task 19069 Work: Add support for Engineering tinkers 
- Tinkerings for Waist slot fixed
- Selecting other Tinkerings works now, NOTE: This commit should mean full support of Tinkerings now
- Added better warning info to the model status in the status bar
- Colorized the model status label
- Fix for Non-ItemTooltips having a nullcheck crash
- Disabled Update Item Cache from Wowhead function in release builds as it doesn't work yet

Rawr.Buffs:
- The Buffs By raid Members dialog has been updated to Cataclysm for the four classes that I had previously set up (Priest, Warrior, Mage and Hunter). Other classes coming soon
- Added Death Knights to that dialog 
- Fix for Issue 16401: Buff.AllowedClasses wasn't being consistently enforced - Changed the implementation in Base to enforce allowed classes and professions. Charts, the Buff Control and the affected models should now all work properly

Rawr.BossHandler:
- Raid Boss armour confirmed at 11977 not 10643
- Issue 19328 Work: GetDPSbyType always uses avoidance stats - Added some if statements into the calc that would be for formulating against other attack types. They are non-functional at this time, just a placeholde

Rawr.ItemFilters:
- Vendor Item Source now supports multiple tokens, the second one can be typed in.
- Slight adjustments to the Rare mobs listing
- Ensured the ItemFilters were fixed and in the DefaultDataFiles.zip
- Basic cleanup of test data and un-needed references.
- More cleanup and errors fixed. Updated DefaultDataFiles.zip 

Rawr.Items:
- Change Darkmoon Card: Volcano to use two different special effects for the two different parts of the proc.
- Fix for Chimera gems uniqueness
- Fix for issue 19344: Anhuur's Hymnal proc not counted - Added parsing for its proc
- Fixed BonusWhiteDamageMultiplier with a DefaultValueAtribute(0)
- Added ToItemString function to ItemInstance for Rawr.Addon
- Fix ToItemString - Rawr uses ReforgeIds-56 needed to add that back to itemstring export to get in game to work
- A bunch of the Source parsing from Wowhead is now fixed. Still more work to do
- Parsing source from Wowhead is about 95% functional now. Only a few items won't get source data (not counting the ones that simply don't have source data on wowhead yet).
- More Parsing Fixes:
- - Now sets costs for Justice and Valor points if the item has them
- - Now sets both the purchase currency+cost and where the currency drops (namely the armor token drops) if its available

Rawr.LoadCharacter:
- Importing your toon from the Rawr AddOn or the Armory will now mark the items (and their enchants if equipped) with Green Diamonds

Rawr.Bear:
- Fix for Issue 19426: Proc AGI is still providing armor - Took off the Agility to Armor bonus from procs 
- Vastly improved default and preset values on the Options tab.
- Improvements to Vengeance calculations

Rawr.DPSWarr:
- Overhauled using warnings from Code Analysis
- Fix for Issue 19372: SMT with 1H Weapons doesn't work
- - This was Three separate things
- - You need to go to Filters by Item Type and turn on 1h Weapons then click Apply. DPSWarr by default doesn't show them but does leave you the option to turn them on
- - SMT wasn't handled correctly. I've fixed this
- - Having a non-1h in the OffHand is throwing a NaN on that chart, so it ends up not knowing what to do and crashes. Added a handler to check for NaN in that core file so no models can make that break anymore.
- Modelled BonusWhiteDamageMultiplier (some new trinkets use this)
- More overhauling
- Fixed a bug that would make all the Buff Selectors disappear
- More overhauling
- Added a notice that Fury doesnt work yet, invalidated its calcs
- Gemming Templates now generate for 4 metas instead of just Chaotic

Rawr.Enhance:
- Initial work at implementing T11
- Removed T7-T9
- Fix for issue 19428 - hit rating required was using physical not spell hit 

Rawr.Mage:
- Added average mode for mirror images, default to disabled
- Performance improvement for incremental optimization with advanced mana segment constraints. 

Rawr.Moonkin:
- Add Silences to the boss options handling code in Moonkin
- Removed a lot of else's that should enable handling of single SpecialEffects with multiple stats that fall into separate handler categories
- Enable trinket procs that apply DoT effects.
- Fix mana proc handling. In particular, Hymn of Hope / Mana Tide Totem should now give sustained damage benefit.
- Add cogwheel support to gemming templates
- Add sparkling cogwheel for spirit = hit so both hit & spirit cogwheels can be used together
- Implement spell crit depression
- Undo an old optimization change that turned out to break a lot of calculations for non-standard rotations. Woopsy
- Fix a mistake with calculating mastery procs. Theralion's Mirror should now show the proper bonus
- Fix a display issue where the hit cap is not displayed properly for non-raid boss targets
- Got the numbers backwards on the display function

Rawr.ProtPaladin:
- Fix for Issue 19434: Crash on Character Load - Consecrate Glyph wasn't playing well with number of ticks count. Fixed the array size and the starting point so it doesn't crash anymore

Rawr.ProtWarr:
- Fix for Issue 19365: Agility should no longer grant armor - Removed the calc that adds Armor from Agility
- Task 19364 Completed: Plate Specialization is missing - Added ValidatePlateSpec function from DPSWarr, applied as BonusStaminaMultiplier
- Fix for Issue 19366: [Enchant Shield - Blocking] missing - ProtWarr didn't have Block Rating relevant and wasn't pulling the stat. Fixed
- Fixed a cross thread access issue

Rawr.RestoSham:
- Fix for buffs, gemming templates, and changes to stats area to keep confusion down while Alpineman continues work on new model
- Disabled the beta model for now
- Beta model changes

Rawr.ShadowPriest:
- Fix for Issue 19425: Twisted Faith is giving hit on base spirit - Added a subtractor to make it ignore base spirit 

Rawr.Warlock:
- Fix for Issue 19391
");
#endregion
#region Rawr 4.0.12 (Dec 19, 2010) [r56280]
VNStuff.Add(
"Rawr 4.0.12 (Dec 19, 2010) [r56280]",
@"Cataclysm Release Beta

Rawr.Base:
- Monolithically huge performance fix!
- Catch for IsolatedStorageExceptions to alert the user they need to allow Rawr in Silverlight permissions instead of Deny it.
- Second Catch for failed Armory Imports to try and get better messaging in case it failed in a different spot
- Attempted to make a call that would download new caches automatically if they failed to load. Astrylian will investigate this further.
- Task 19071 Completed: Support Item Source from Achievements - You can now manually set item source to Achievements 
- Task 18891 Completed: Cat T10 4P Change - Updated the Set Bonus
- Fix for ItemCache failover reloading

Rawr.Moonkin:
- Fix broken null reference exception in the reforging code. 
");
#endregion
#region Rawr 4.0.11 (Dec 18, 2010) [r56269]
VNStuff.Add(
"Rawr 4.0.11 (Dec 18, 2010) [r56269]",
@"Cataclysm Release Beta

Rawr.Addon
- Version 0.07
- - Reworked the Character XML to be same as save from Rawr
- Version 0.08
- - Removed Equipped Block for equipped items in export
- - Removed alternate talent lists for other classes
- - Fixed extra line break at start of file
- - Added extra indent for available items for visual inspection
- - Refactor slot working
- - Dry coded blacksmithing socket check
- - Has sockets returns true for time being
- Version 0.09
- - Fixed some parsing issues to make it work
- - Export Class name as lowercase
- - Flag as v0.09 in TOC
- - Updated version check for Rawr AddOn to 0.09 and added version notes to the LUA
- - Added RawrBuild tag to export to allow Rawr to know what minimum build addon supports.

Rawr.Base:
- Created Import from Rawr AddOn process in program!
- - only Imports what you are wearing
- - Doesn't mark as available to the optimizer yet
- - Gems in sockets didn't work on test toon, need to investigate
- Fix for Issue 19269:  Enchant Cloak - Protection should give bonus armor - Changed to Bonus Armor
- Hid Add/Delete Custom Gemming menu options as they are non-functional and will be removed from program
- Changed ErrorBox to use a Silverlight ErrorWindow method instead of MessageBox so that we can keep it more internal and give a better window
- Armory Imports that fail with a not Found will state better messaging
- Enable Enchant Off-Hand - Superior Intellect.
- Potential fix for no saved file.
- Fix for bad merge.
- Fix for optimization without any available cogwheel/hydraulic.
- For the most part you should now be able to manually set sources for items. Please report bugs in the Issue Tracker on this.
- Fixed null checks
- More work for Item Sourcing, can now properly store and recall up to three sources per item and they show all of them on the Item Tooltips and should all be filterable against. If any one source is available from current filters it should show. Turn off all filters related to the sources to hide it.
- ilevel box on the Edit Item dialog was persisting between items. Changed handling so that it won't
- Same thing for Min/Max Weapon damage
- Added parsing for a trinket
- ItemFilters had a couple || in it, so those filters were matching everything and you couldn't properly hide a lot of items
- Fixed some bugs with Item Source editing
- More ItemSource fixes
- Fix for issue 19301: Copy Data to Clipboard on the Direct Upgrades->Gear chart only copies head-slot items - the list that Copy to Clipboard uses was only being Updated on item charts. Added checks to every chart type so they will all Export
- Fix for Issue 19292: Export to CSV generates FileSecurityState_OperationNotPermitted error - Tested that this cannot be performed in browser. When Installed Offline you can. Set a check to see if you were running Offline and if not will alert User that they need to install Offline before they can do this.
- Fix for Issue 19287: Editing/importing items seems to add them to the cache, but are unavailable to equip. or optimizer - I think a large part of this issue is users not switching to the right model or race beforehand so the Items are filtered out as irrelevant - Added a check to ensure the item shows in the chart if it is Relevant
- Fix for Issue 18697: Pawn export doesn't consider value of weapon DPS - Added Pawn Export function from Rawr2 - Added check for melee classes to add MeleeDPS1 to the string so that it gets a value
- Updated a few shaman races base stats
- New Rawr4 Welcome Window with a lot more noob friendliness
- More Welcome Window stuff
- Additional support for model based reforging restrictions
- Redid the Welcome Window some to give the Tabs more room (so you can read them more easily)
- Populated the Version History with Rawr4 stuff instead of Rawr2 from Nov 2009
- Fixed the Tips display
- Populated the Known Issues list with Open Issues that haven't already been resolved for .11
- Fixed the annoying scrollbar for no reason issue on the main page
- Fixed Windwalk's Enchant Id
- GetCritReductionFromResilience now always returns 0. Resilience has not reduced crit strike chance since 4.0.1
- Moved crit chance reduction via resilence removal to the right spot. I didn't realize at first that GetCritReductionFromResilience still calculates crit damage reduction.
- More ItemSource fixes
- Performance fixes

Rawr.Bear:
- Fix for Issue 19244: Ignoring Threat Rating Customization - Added ThreatScale to the ThreatPoints value

Rawr.DK:
- Fix for Defect 19241: Items not showing on list - This was due to bad gemming templates. So I shamelessly stole the template format from DPSWarr. It works great!
- Fix for possible rotation exception Found during new unit test.
- Fix for sorting issue in sorting by DPS/TPS
- Fix for BB weirdness.
- Fix bug w/ exception in rotation cost evaluation. Found during unittest.
- Fixed Tanking runes
- Fixed special effects handler
- Fixed Blood Parasite implementation
- Implemented basic RPP5 code
- Implemented combinedswing time for DW.
- Implemented Mitigation Subvalues like the survival subvalues.
- Fix for 19288: Hang when looking at trinkets.
- Unittests failing because of a config change.

Rawr.DPSWarr:
- Heroic Throw can no longer go nuts and take over your rotation
- Random work on stuff
- Worked on Heroic Strike/Cleave and how it interacts with the rotation. I'm liking the numbers I'm seeing and Rage is a lot smoother in interaction. This also greatly boosted Haste's relative value and it's not spikey in the test files like it was before.
- Changed the weighting of Normal v Exec phase so that they are based on percentage of time instead of direct average.

Rawr.Elemental:
- Activated the Gemming Templates so Items would show up
- Fixed a Null bug with Frost Shock
- It comes up without crashing now

Rawr.HealPriest:
- Fix for Issue 19280: Model not loading - Uncommented the RegisterModel command, now it loads

Rawr.Mage:
- Support for mastery procs (arcane only for now, probably slightly overestimates).
- Partial support for int procs (includes int=> spell power conversion, regen from max mana and negative impact on mana adept, does not include int=>crit conversion at the moment)
- Fix for int procs.
- Added Draenei hit bonus.

Rawr4.Moonkin:
* Fix double-counting base Druid crit.
* Fix for defect #19282: Items, talents, etc., have no value when no character is loaded.
- Fix for Issue 19317: SpellPower and Hit slightly off - User reported that base Intellect and Spirit don't factor into SpellPower and Hit fully. Implemented suggested fix
- Fix for issue where reforging was showing nonsensical combinations
- Add Intellect and Mastery trinket procs

Rawr.ProtPaladin:
- Fix for Issue 19281: Items not loading - No Gemming Templates were enabled by default (due to there being no Epic Templates this early in Cataclysm). Set Rare templates to enable instead of Epic
- Fixed crit vulnerability calculations
- Fixed display issue when crit immune
- Updated consecration probability calculations
- Added Mastery Rating to the item budget table
- Added custom rotation, fixed some miscellaneous bugs, started work on fixing the scales
- Bar colors now match other tank models 

Rawr.RestoSham:
- New gemming templates created and working, default set to use rares
- Fixed a parenthesis error

Rawr.ShadowPriest:
- Fix for Issue 19320: Mastery Rating doesn't show up for ShadowPriest Items - Added Mastery Rating as a Relevant Stat, though it will have no value

Rawr.TankDK:
- Fix for Runetap
- Fix for Mitigation values being Sky high - I was double adding the TotalDPS to PhysicalDPS from boss.
- Some other massaging
"
);
#endregion
#region Rawr 4.0.10 (Dec 06, 2010) [r56001]
VNStuff.Add(
"Rawr 4.0.10 (Dec 06, 2010) [r56001]",
@"Cataclysm Pre-Release Beta with first wave of Fixes

Rawr.Addon:
- Fixed drycoding errors

Rawr4.Base:
- Hid the Force Refresh box as we don't have caching on the server yet. 
- Fix for Issue 19240 - Tooltips for Cloaks were not showing their Additional Info strings (iLvl, Id, etc). - Cloaks used to be considered ItemType.Cloth but now they are ItemType.None. Needed to add a check to ensure the slot was still Back like we do for Necks, Rings and Trinkets
- Updated the Welcome Window with newer/better info and forced it to show always until we make a couple releases from this one.
- Fixed a null reference bug with Icons 

Rawr.Base.Items:
- Updated source information for all Reputation and Crafted Gear
- Added any Reputation/Crafted gear that were not in before
- Added Guild Reputation Heirloom heads and backs 
- Changed default on Enter ID form to Use Wowhead 
- Loading from BNet Armory now checks wowhead for items that are not in the database. This will be VERY slow when it occurs for a couple minutes, but then it will clear up when its done and when you close out those items are saved so you won't have to get them again.
- Removed the proxying process for communicating with wowhead. This should fix wowhead item loading for a lot of people 

Rawr.Mage:
- Make sure spell cost reduction can't make base spell cost negative."
);
#endregion
#region Rawr 4.0.09 (Dec 06, 2010) [r55987]
VNStuff.Add(
"Rawr 4.0.09 (Dec 06, 2010) [r55987]",
@"Cataclysm Pre-Release Beta"
);
#endregion

            CB_Version.Items.Add("All");
            String[] arr = new String[VNStuff.Keys.Count];
            VNStuff.Keys.CopyTo(arr, 0);
            foreach (String a in arr) { CB_Version.Items.Add(a); }
            CB_Version.SelectedIndex = 0;
            CB_Version_SelectedIndexChanged(null, null);
        }
        private void SetUpKI()
        {
KIStuff.Add(
"Many Models Don't work",
@"Due to the Cataclysm release, many models have not been fully coded as Cataclysm Ready.
Those that have been will most likely have several core bugs while we work out the specifics.
Note that some models are in fact ready, such as DPSWarr.Arms, Mage, Bear and Cat"
);

            CB_Issues.Items.Add((String)"All");
            String[] arr = new String[KIStuff.Keys.Count];
            KIStuff.Keys.CopyTo(arr, 0);
            foreach (String a in arr) { CB_Issues.Items.Add(a); }
            CB_Issues.SelectedIndex = 0;
            CB_Issues_SelectedIndexChanged(null, null);
        }
        private void SetUpRecentCharsList() {
#if !SILVERLIGHT
            if (Rawr.Properties.RecentSettings.Default.RecentFiles.Count > 0) {
                foreach (string s in Rawr.Properties.RecentSettings.Default.RecentFiles) {
                    List<string> sl = s.Split('\\').ToList();
                    Button b = new Button() {
                        Content = sl[sl.Count - 1].Replace(".xml", ""),
                        Margin = new Thickness(4, 0, 4, 0),
                        Tag = s,
                        Background = new SolidColorBrush(Colors.White),
                    };
                    b.Click += new RoutedEventHandler(b_Click);
                    SP_RecentChars.Children.Insert(1, b);
                }
            }
#endif
        }

#if !SILVERLIGHT
        void b_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            MainPage.Instance.OpenCharacter(b.Tag as string);
            this.DialogResult = true;
        }
#endif

        #region Info Operations
        private void CB_Tips_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            //try {
            string text = "";
            if ((String)CB_Tips.SelectedItem == "All")
            {
                int Iter = 1;
                text += "== CONTENTS ==" + "\n";
                foreach (string s in TipStuff.Keys)
                {
                    text += Iter.ToString("00") + ". " + s + "\n"; // Tip
                    Iter++;
                } Iter = 1;
                /*text += "\n";
                text += "== READ ON ==" + "\n";
                foreach (string s in TipStuff.Keys)
                {
                    text += Iter.ToString("00") + ". " + s + "\n"; // Tip
                    text += "\n" + "\n";
                    Iter++;
                } Iter = 1;*/
                RTB_Tips.Text = text.Trim();
            }
            else
            {
                string s = (String)CB_Tips.SelectedItem;
                string a = "invalid";
                bool ver = TipStuff.TryGetValue(s, out a);
                text += s + "\n";
                text += "\n";
                text += (ver ? a : "An error occurred calling the string");
                RTB_Tips.Text = text.Trim();
            }
            /*} catch(Exception ex){
                ErrorBox eb = new ErrorBoxDPSWarr("Error in setting the Tip Item",
                    ex, "CB_Tip_SelectedIndexChanged");
                eb.show();
            }*/
        }
        private void CB_FAQ_Questions_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            //try {
            string text = "";
            if ((String)CB_FAQ_Questions.SelectedItem == "All")
            {
                int Iter = 1;
                text += "== CONTENTS ==" + "\n";
                foreach (string s in FAQStuff.Keys)
                {
                    text += Iter.ToString("00") + "Q. " + s + "\n"; // Question
                    Iter++;
                } Iter = 1;
                text += "\n";
                text += "== READ ON ==" + "\n";
                foreach (string s in FAQStuff.Keys)
                {
                    string a = "invalid";
                    text += Iter.ToString("00") + "Q. " + s + "\n"; // Question
                    bool ver = FAQStuff.TryGetValue(s, out a);
                    text += Iter.ToString("00") + "A. " + (ver ? a : "An error occurred calling the string") + "\n"; // Answer
                    text += "\n" + "\n";
                    Iter++;
                } Iter = 1;
                RTB_FAQ.Text = text.Trim();
            }
            else
            {
                string s = (String)CB_FAQ_Questions.SelectedItem;
                string a = "invalid";
                bool ver = FAQStuff.TryGetValue(s, out a);
                text += s + "\n";
                text += "\n";
                text += (ver ? a : "An error occurred calling the string");
                RTB_FAQ.Text = text.Trim();
            }
            /*} catch(Exception ex){
                ErrorBox eb = new ErrorBoxDPSWarr("Error in setting the FAQ Item",
                    ex, "CB_FAQ_Questions_SelectedIndexChanged");
                eb.show();
            }*/
        }
        private void CB_Version_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            string text = "";
            if ((String)CB_Version.SelectedItem == "All")
            {
                int Iter = 1;
                text += "== CONTENTS ==" + "\r\n";
                foreach (string s in VNStuff.Keys)
                {
                    text += s + "\r\n";
                    Iter++;
                } Iter = 1;
                text += "\r\n";
                text += "== READ ON ==" + "\r\n";
                foreach (string s in VNStuff.Keys)
                {
                    string a = "invalid";
                    text += s + "\r\n";
                    bool ver = VNStuff.TryGetValue(s, out a);
                    text += (ver ? a : "An error occurred calling the string") + "\r\n";
                    text += "\r\n" + "\r\n";
                    Iter++;
                } Iter = 1;
                RTB_Version.Text = text.Trim();
            }
            else
            {
                string s = (String)CB_Version.SelectedItem;
                string a = "invalid";
                bool ver = VNStuff.TryGetValue(s, out a);
                text += s + "\r\n";
                text += "\r\n";
                text += (ver ? a : "An error occurred calling the string");
                RTB_Version.Text = text.Trim();
            }
        }
        private void CB_Issues_SelectedIndexChanged(object sender, SelectionChangedEventArgs e)
        {
            //try {
            string text = "";
            if ((String)CB_Issues.SelectedItem == "All")
            {
                int Iter = 1;
                text += "== CONTENTS ==" + "\n";
                foreach (string s in KIStuff.Keys)
                {
                    text += Iter.ToString("00") + "I. " + s + "\n"; // Issue
                    Iter++;
                } Iter = 1;
                text += "\n";
                text += "== READ ON ==" + "\n";
                foreach (string s in KIStuff.Keys)
                {
                    string a = "invalid";
                    text += Iter.ToString("00") + "I. " + s + "\n"; // Issue
                    bool ver = KIStuff.TryGetValue(s, out a);
                    text += Iter.ToString("00") + "W. " + (ver ? a : "An error occurred calling the string") + "\n"; // WorkAround
                    text += "\n" + "\n";
                    Iter++;
                } Iter = 1;
                RTB_Issues.Text = text.Trim();
            }
            else
            {
                string s = (String)CB_Issues.SelectedItem;
                string a = "invalid";
                bool ver = KIStuff.TryGetValue(s, out a);
                text += s + "\n";
                text += "\n";
                text += (ver ? a : "An error occurred calling the string");
                RTB_Issues.Text = text.Trim();
            }
            /*} catch(Exception ex){
                ErrorBox eb = new ErrorBoxDPSWarr("Error in setting the Known Issue Item",
                    ex, "CB_Issues_SelectedIndexChanged");
                eb.show();
            }*/
        }
        #endregion

        #region Character Operations
        private void BT_CreateNew_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Instance.NewCharacter(null, null);
            this.DialogResult = true;
        }
        private void BT_LoadBNet_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Instance.LoadFromBNet(null, null);
            this.DialogResult = true;
        }
        private void BT_LoadRawrAddOn_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Instance.LoadFromRawrAddon(null, null);
            this.DialogResult = true;
        }
        private void BT_LoadRawrRepo_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Instance.LoadFromRawr4Repository(null, null);
            this.DialogResult = true;
        }
        private void BT_OpenFile_Click(object sender, RoutedEventArgs e)
        {
            MainPage.Instance.OpenCharacter(null, null);
            this.DialogResult = true;
        }
        #endregion

        private void OKButton_Click(object sender, RoutedEventArgs e)
        {
            Rawr.Properties.GeneralSettings.Default.WelcomeScreenSeen = true;
            this.DialogResult = true;
        }

#if !SILVERLIGHT
        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
#endif
    }
}
