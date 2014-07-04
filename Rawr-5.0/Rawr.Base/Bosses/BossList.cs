using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Rawr.Bosses;

namespace Rawr {
    public class BossList {
        // Constructors
        public BossList()
        {
            #region Instance Declarations
            #region ==== Tier 7 Content ====
            // Naxxramas
            //AnubRekhan anubrekhan = new AnubRekhan();
            //GrandWidowFaerlina grandwidow = new GrandWidowFaerlina();
            //Maexxna maexxna = new Maexxna();
            //NoththePlaguebringer noth = new NoththePlaguebringer();
            //HeigantheUnclean heigantheUnclean = new HeigantheUnclean();
            //Loatheb loatheb = new Loatheb();
            //InstructorRazuvious instructorRazuvious = new InstructorRazuvious();
            //GothiktheHarvester gothiktheHarvester = new GothiktheHarvester();
            //FourHorsemen fourHorsemen = new FourHorsemen();
            //Patchwerk patchwerk = new Patchwerk();
            //Grobbulus grobbulus = new Grobbulus();
            //Gluth gluth = new Gluth();
            //Thaddius thaddius = new Thaddius();
            //Sapphiron sapphiron = new Sapphiron();
            //KelThuzad kelThuzad = new KelThuzad();
            // The Obsidian Sanctum
            //Shadron shadron = new Shadron();
            //Tenebron tenebron = new Tenebron();
            //Vesperon vesperon = new Vesperon();
            //Sartharion sartharion = new Sartharion();
            // The Vault of Archavon
            //ArchavonTheStoneWatcher archavon = new ArchavonTheStoneWatcher();
            // The Eye of Eternity
            //Malygos malygos = new Malygos();
            #endregion
            #region ==== Tier 8 Content ====
            // Ulduar
            //IgnistheFurnaceMaster IgnistheFurnaceMaster = new IgnistheFurnaceMaster();
            //Razorscale Razorscale = new Razorscale();
            //XT002Deconstructor XT002Deconstructor = new XT002Deconstructor();
            //AssemblyofIron AssemblyofIron = new AssemblyofIron();
            //Kologarn Kologarn = new Kologarn();
            //Auriaya Auriaya = new Auriaya();
            //Mimiron Mimiron = new Mimiron();
            //Freya Freya = new Freya();
            //Thorim Thorim = new Thorim();
            //Hodir Hodir = new Hodir();
            //GeneralVezax GeneralVezax = new GeneralVezax();
            //YoggSaron YoggSaron = new YoggSaron();
            //AlgalontheObserver AlgalontheObserver = new AlgalontheObserver();
            // Vault of Archavon
            //EmalonTheStormWatcher EmalonTheStormWatcher = new EmalonTheStormWatcher();
            #endregion
            #region ==== Tier 9 Content ====
            // The Vault of Archavon
            //KoralonTheFlameWatcher KoralonTheFlameWatcher = new KoralonTheFlameWatcher();
            // Trial of the Crusader
            //NorthrendBeasts NorthrendBeasts = new NorthrendBeasts();
            //LordJaraxxus LordJaraxxus = new LordJaraxxus();
            //FactionChampions FactionChampions = new FactionChampions();
            //TwinValkyr TwinValkyr = new TwinValkyr();
            //Anubarak Anubarak = new Anubarak();
            #endregion
            #region ==== Tier 10 Content ====
            // The Vault of Archavon
            //ToravonTheIceWatcher ToravonTheIceWatcher = new ToravonTheIceWatcher();
            // Icecrown Citadel
            //LordMarrowgar LordMarrowgar = new LordMarrowgar();
            //LadyDeathwhisper LadyDeathwhisper = new LadyDeathwhisper();
            //DeathbringerSaurfang DeathbringerSaurfang = new DeathbringerSaurfang();
            //Festergut Festergut = new Festergut();
            //Rotface Rotface = new Rotface();
            //ProfessorPutricide ProfessorPutricide = new ProfessorPutricide();
            //BloodPrinceCouncil BloodPrinceCouncil = new BloodPrinceCouncil();
            //BloodQueenLanathel BloodQueenLanathel = new BloodQueenLanathel();
            //ValathriaDreamwalker ValathriaDreamwalker = new ValathriaDreamwalker();
            //Sindragosa Sindragosa = new Sindragosa();
            //TheLichKing TheLichKing = new TheLichKing();
            // Ruby Sanctum
            //Baltharus Baltharus = new Baltharus();
            //SavianaRagefire SavianaRagefire = new SavianaRagefire();
            //GeneralZarithrian GeneralZarithrian = new GeneralZarithrian();
            //Halion Halion = new Halion();
            #endregion
            #region ==== Tier 11 Content ====
            // Baradin Hold
            Argaloth Argaloth = new Argaloth();
            // Blackwing Descent
            Magmaw Magmaw = new Magmaw();
            OmnotronDefenseSystem OmnotronDefenseSystem = new OmnotronDefenseSystem();
            Maloriak Maloriak = new Maloriak();
            Atramedes Atramedes = new Atramedes();
            Chimaron Chimaron = new Chimaron();
            Nefarian Nefarian = new Nefarian();
            // The Bastion of Twilight
            ValionaAndTheralion ValionaAndTheralion = new ValionaAndTheralion();
            HalfusWyrmbreaker HalfusWyrmbreaker = new HalfusWyrmbreaker();
            TwilightAscendantCouncil TwilightAscendantCouncil = new TwilightAscendantCouncil();
            Chogall Chogall = new Chogall();
            LadySinestra LadySinestra = new LadySinestra();
            // Throne of the Four Winds
            ConclaveOfWind ConclaveOfWind = new ConclaveOfWind();
            AlAkir AlAkir = new AlAkir();
            #endregion
            #region ==== Tier 12 Content ====
            // Baradin Hold
            Occuthar Occuthar = new Occuthar();
            // Fireland
            Bethtilac Bethtilac = new Bethtilac();
            LordRhyolith LordRhyolith = new LordRhyolith();
            Alysrazor Alysrazor = new Alysrazor();
            Shannox Shannox = new Shannox();
            Baleroc Baleroc = new Baleroc();
            MajordomoStaghelm MajordomoStaghelm = new MajordomoStaghelm();
            Ragnaros Ragnaros = new Ragnaros();
            #endregion
            #region ==== Tier 13 Content ====
            // Baradin Hold
            Alizabal Alizabal = new Alizabal();
            // Dragon Soul
            Morchok Morchok = new Morchok();
            WarlordZonozz WarlordZonozz = new WarlordZonozz();
            Yorsahj Yorsahj = new Yorsahj();
            Hagara Hagara = new Hagara();
            Ultraxion Ultraxion = new Ultraxion();
            WarmasterBlackhorn WarmasterBlackhorn = new WarmasterBlackhorn();
            SpineofDeathwing SpineofDeathwing = new SpineofDeathwing();
            MadnessofDeathwing MadnessofDeathwing = new MadnessofDeathwing();
            #endregion
            #region ==== Tier 14 Content ====
            // World Bosses
            ShaofAnger ShaofAnger = new ShaofAnger();
            SalyissWarband SalyissWarband = new SalyissWarband();
            // Mogu'shan Vaults
            TheStoneGuard TheStoneGuard = new TheStoneGuard();
            FengtheAccursed FengtheAccursed = new FengtheAccursed();
            GarajaltheSpiritbinder GarajaltheSpiritbinder = new GarajaltheSpiritbinder();
            TheSpiritKings TheSpiritKings = new TheSpiritKings();
            Elegon Elegon = new Elegon();
            WillOfTheEmperor WillOfTheEmperor = new WillOfTheEmperor();
            // Heart of Fear
            ImperialVizierZorlok ImperialVizierZorlok = new ImperialVizierZorlok();
            BladeLordTayak BladeLordTayak = new BladeLordTayak();
            Garalon Garalon = new Garalon();
            WindLordMeljarak WindLordMeljarak = new WindLordMeljarak();
            AmberShaperUnsok AmberShaperUnsok = new AmberShaperUnsok();
            GrandEmpressShekzeer GrandEmpressShekzeer = new GrandEmpressShekzeer();
            // Terrace of Endless Spring
            ProtectorsOfTheEndless ProtectorsOfTheEndless = new ProtectorsOfTheEndless();
            Tsulong Tsulong = new Tsulong();
            LeiShi LeiShi = new LeiShi();
            ShaOfFear ShaOfFear = new ShaOfFear();
            #endregion
            #endregion
            list = new BossHandler[] {
                #region ==== Tier 7 Content ====
                // Naxxramas
                //anubrekhan[0],anubrekhan[1],
                //grandwidow[0],grandwidow[1],
                //maexxna[0],maexxna[1],
                //noth[0],noth[1],
                //heigantheUnclean[0],heigantheUnclean[1],
                //loatheb[0],loatheb[1],
                //instructorRazuvious[0],instructorRazuvious[1],
                //gothiktheHarvester[0],gothiktheHarvester[1],
                //fourHorsemen[0],fourHorsemen[1],
                //patchwerk[0],patchwerk[1],
                //grobbulus[0],grobbulus[1],
                //gluth[0],gluth[1],
                //thaddius[0],thaddius[1],
                //sapphiron[0],sapphiron[1],
                //kelThuzad[0],kelThuzad[1],
                // The Obsidian Sanctum
                //shadron[0],shadron[1],
                //tenebron[0],tenebron[1],
                //vesperon[0],vesperon[1],
                //sartharion[0],sartharion[1],
                // Vault of Archavon
                //archavon[0],archavon[1],
                // The Eye of Eternity
                //malygos[0],malygos[1],
                #endregion
                #region ==== Tier 8 Content ====
                // Ulduar
                //IgnistheFurnaceMaster[0],IgnistheFurnaceMaster[1],
                //Razorscale[0],Razorscale[1],
                //XT002Deconstructor[0],XT002Deconstructor[1],
                //AssemblyofIron[0],AssemblyofIron[1],
                //Kologarn[0],Kologarn[1],
                //Auriaya[0],Auriaya[1],
                //Mimiron[0],Mimiron[1],
                //Freya[0],Freya[1],
                //Thorim[0],Thorim[1],
                //Hodir[0],Hodir[1],
                //GeneralVezax[0],GeneralVezax[1],
                //YoggSaron[0],YoggSaron[1],
                //AlgalontheObserver[0],AlgalontheObserver[1],
                // Vault of Archavon
                //EmalonTheStormWatcher[0], EmalonTheStormWatcher[1],
                #endregion
                #region ==== Tier 9 Content ====
                // Vault of Archavon
                //KoralonTheFlameWatcher[0],KoralonTheFlameWatcher[1],
                // Trial of the Crusader
                //NorthrendBeasts[0],NorthrendBeasts[1],NorthrendBeasts[2],NorthrendBeasts[3],
                //LordJaraxxus[0],LordJaraxxus[1],LordJaraxxus[2],LordJaraxxus[3],
                //FactionChampions[0],FactionChampions[1],FactionChampions[2],FactionChampions[3],
                //TwinValkyr[0],TwinValkyr[1],TwinValkyr[2],TwinValkyr[3],
                //Anubarak[0],Anubarak[1],Anubarak[2],Anubarak[3],
                #endregion
                #region ==== Tier 10 Content ====
                // Vault of Archavon
                //ToravonTheIceWatcher[0],ToravonTheIceWatcher[1],
                // Icecrown Citadel
                //LordMarrowgar[0],LordMarrowgar[1],LordMarrowgar[2],LordMarrowgar[3],
                //LadyDeathwhisper[0],LadyDeathwhisper[1],LadyDeathwhisper[2],LadyDeathwhisper[3],
                //DeathbringerSaurfang[0], DeathbringerSaurfang[1], DeathbringerSaurfang[2], DeathbringerSaurfang[3],
                //Festergut[0], Festergut[1], Festergut[2], Festergut[3],
                //Rotface[0], Rotface[1], Rotface[2], Rotface[3],
                //ProfessorPutricide[0], ProfessorPutricide[1], ProfessorPutricide[2], ProfessorPutricide[3],
                //BloodPrinceCouncil[0], BloodPrinceCouncil[1], BloodPrinceCouncil[2], BloodPrinceCouncil[3],
                //BloodQueenLanathel[0], BloodQueenLanathel[1], BloodQueenLanathel[2], BloodQueenLanathel[3],
                //ValathriaDreamwalker[0], ValathriaDreamwalker[1], ValathriaDreamwalker[2], ValathriaDreamwalker[3],
                //Sindragosa[0], Sindragosa[1], Sindragosa[2], Sindragosa[3],
                //TheLichKing[0], TheLichKing[1], TheLichKing[2], TheLichKing[3],
                // Ruby Sanctum
                //Baltharus[0],Baltharus[1],Baltharus[2],Baltharus[3],
                //SavianaRagefire[0],SavianaRagefire[1],SavianaRagefire[2],SavianaRagefire[3],
                //GeneralZarithrian[0],GeneralZarithrian[1],GeneralZarithrian[2],GeneralZarithrian[3],
                //Halion[0],Halion[1],Halion[2],Halion[3],
                #endregion
                #region ==== Tier 11 Content ====
                // Baradin Hold
                Argaloth[0], Argaloth[1],
                // Blackwing Descent
                Magmaw[0],Magmaw[1],Magmaw[2],Magmaw[3],
                OmnotronDefenseSystem[0],OmnotronDefenseSystem[1],OmnotronDefenseSystem[2],OmnotronDefenseSystem[3],
                Maloriak[0], Maloriak[1], Maloriak[2], Maloriak[3],
                Atramedes[0], Atramedes[1], Atramedes[2], Atramedes[3],
                Chimaron[0], Chimaron[1], Chimaron[2], Chimaron[3],
                Nefarian[0], Nefarian[1], Nefarian[2], Nefarian[3],
                // The Bastion of Twilight
                ValionaAndTheralion[0], ValionaAndTheralion[1], ValionaAndTheralion[2], ValionaAndTheralion[3],
                HalfusWyrmbreaker[0], HalfusWyrmbreaker[1], HalfusWyrmbreaker[2], HalfusWyrmbreaker[3],
                TwilightAscendantCouncil[0], TwilightAscendantCouncil[1], TwilightAscendantCouncil[2], TwilightAscendantCouncil[3],
                Chogall[0], Chogall[1], Chogall[2], Chogall[3],
                LadySinestra[0], LadySinestra[1],
                // Throne of the Four Winds
                ConclaveOfWind[0],ConclaveOfWind[1],ConclaveOfWind[2],ConclaveOfWind[3],
                AlAkir[0],AlAkir[1],AlAkir[2],AlAkir[3],
                #endregion
                #region ==== Tier 12 Content ====
                // Baradin Hold
                Occuthar[0], Occuthar[1],
                // Fireland
                Bethtilac[0],Bethtilac[1],Bethtilac[2],Bethtilac[3],
                LordRhyolith[0],LordRhyolith[1],LordRhyolith[2],LordRhyolith[3],
                Alysrazor[0],Alysrazor[1],Alysrazor[2],Alysrazor[3],
                Shannox[0],Shannox[1],Shannox[2],Shannox[3],
                Baleroc[0],Baleroc[1],Baleroc[2],Baleroc[3],
                MajordomoStaghelm[0],MajordomoStaghelm[1],MajordomoStaghelm[2],MajordomoStaghelm[3],
                Ragnaros[0],Ragnaros[1],Ragnaros[2],Ragnaros[3],
                #endregion
                #region ==== Tier 13 Content ====
                // Baradin Hold
                Alizabal[0], Alizabal[1],
                // Dragon Soul
                Morchok[0],Morchok[1],Morchok[2],Morchok[3],Morchok[4],
                WarlordZonozz[0],WarlordZonozz[1],WarlordZonozz[2],WarlordZonozz[3],WarlordZonozz[4],
                Yorsahj[0],Yorsahj[1],Yorsahj[2],Yorsahj[3],Yorsahj[4],
                Hagara[0],Hagara[1],Hagara[2],Hagara[3],Hagara[4],
                Ultraxion[0],Ultraxion[1],Ultraxion[2],Ultraxion[3],Ultraxion[4],
                WarmasterBlackhorn[0],WarmasterBlackhorn[1],WarmasterBlackhorn[2],WarmasterBlackhorn[3],WarmasterBlackhorn[4],
                SpineofDeathwing[0],SpineofDeathwing[1],SpineofDeathwing[2],SpineofDeathwing[3],SpineofDeathwing[4],
                MadnessofDeathwing[0],MadnessofDeathwing[1],MadnessofDeathwing[2],MadnessofDeathwing[3],MadnessofDeathwing[4],
                #endregion
                #region ==== Tier 14 Content ====
                // World Bosses
                ShaofAnger[0], 
                SalyissWarband[0],
                // Mogu'shan Vaults
                TheStoneGuard[0],TheStoneGuard[1],TheStoneGuard[2],TheStoneGuard[3],TheStoneGuard[4],
                FengtheAccursed[0],FengtheAccursed[1],FengtheAccursed[2],FengtheAccursed[3],FengtheAccursed[4],
                GarajaltheSpiritbinder[0],GarajaltheSpiritbinder[1],GarajaltheSpiritbinder[2],GarajaltheSpiritbinder[3],GarajaltheSpiritbinder[4],
                TheSpiritKings[0],TheSpiritKings[1],TheSpiritKings[2],TheSpiritKings[3],TheSpiritKings[4],
                Elegon[0],Elegon[1],Elegon[2],Elegon[3],Elegon[4],
                WillOfTheEmperor[0],WillOfTheEmperor[1],WillOfTheEmperor[2],WillOfTheEmperor[3],WillOfTheEmperor[4],
                // Heart of Fear
                ImperialVizierZorlok[0],ImperialVizierZorlok[1],ImperialVizierZorlok[2],ImperialVizierZorlok[3],ImperialVizierZorlok[4],
                BladeLordTayak[0],BladeLordTayak[1],BladeLordTayak[2],BladeLordTayak[3],BladeLordTayak[4],
                Garalon[0],Garalon[1],Garalon[2],Garalon[3],Garalon[4],
                WindLordMeljarak[0],WindLordMeljarak[1],WindLordMeljarak[2],WindLordMeljarak[3],WindLordMeljarak[4],
                AmberShaperUnsok[0],AmberShaperUnsok[1],AmberShaperUnsok[2],AmberShaperUnsok[3],AmberShaperUnsok[4],
                GrandEmpressShekzeer[0],GrandEmpressShekzeer[1],GrandEmpressShekzeer[2],GrandEmpressShekzeer[3],GrandEmpressShekzeer[4],
                // Terrace of Endless Spring
                ProtectorsOfTheEndless[0],ProtectorsOfTheEndless[1],ProtectorsOfTheEndless[2],ProtectorsOfTheEndless[3],ProtectorsOfTheEndless[4],
                Tsulong[0],Tsulong[1],Tsulong[2],Tsulong[3],Tsulong[4],
                LeiShi[0],LeiShi[1],LeiShi[2],LeiShi[3],LeiShi[4],
                ShaOfFear[0],ShaOfFear[1],ShaOfFear[2],ShaOfFear[3],ShaOfFear[4],
                #endregion

            };
            TheEZModeBoss  = GenTheEZModeBoss(list);
            TheAvgBoss     = GenTheAvgBoss(list);
            TheHardestBoss = GenTheHardestBoss(list);
            // This one is for filtered lists, defaults to the full list above
            calledList = GenCalledList(FilterType.Content,"All");
        }
        #region Variables
        /// <summary>Global setting, the Character's level should always be 80 until the next expansion</summary>
        public const int NormCharLevel = 80;
        /// <summary>The primary list, this hold all bosses regardless of filters</summary>
        public BossHandler[] list;
        /// <summary>This is what modules actually see and is based upon current filters</summary>
        public BossHandler[] calledList;
        /// <summary>Variable for storing Damage Type (Physical, Nature, Holy, etc)</summary>
        private ItemDamageType[] dts = null;
        private ItemDamageType[] DamageTypes {
            get
            {
                return dts ?? (new ItemDamageType[] { 
                    ItemDamageType.Physical,
                    ItemDamageType.Frost,
                    ItemDamageType.Fire,
                    ItemDamageType.Nature,
                    ItemDamageType.Arcane,
                    ItemDamageType.Shadow,
                    ItemDamageType.Holy,
                });
            }
        }
        /// <summary>Checks all the bosses to find the easiest of each stat and combines them to a single boss. Does NOT pick An Easy Boss in the list but MAKES a new one. This IS NOT affected by filters.</summary>
        public BossHandler TheEZModeBoss;
        /// <summary>Checks all the bosses to total up stats and average them out and combines them to a single boss, this is what most users should base their characters against. This IS NOT affected by filters.</summary>
        public BossHandler TheAvgBoss;
        /// <summary>Checks all the bosses to find the worst of each stat and combines them to a single boss. Does NOT pick An Impossible Boss in the list but MAKES a new one. This IS NOT affected by filters.</summary>
        public BossHandler TheHardestBoss;
        /// <summary>Checks all the bosses to find the easiest of each stat and combines them to a single boss. Does NOT pick An Easy Boss in the list but MAKES a new one. This IS affected by filters.</summary>
        public BossHandler TheEZModeBoss_Called;
        /// <summary>Checks all the bosses to total up stats and average them out and combines them to a single boss, this is what most users should base their characters against. This IS affected by filters.</summary>
        public BossHandler TheAvgBoss_Called;
        /// <summary>Checks all the bosses to find the worst of each stat and combines them to a single boss. Does NOT pick An Impossible Boss in the list but MAKES a new one. This IS affected by filters.</summary>
        public BossHandler TheHardestBoss_Called;
        #endregion
        #region Functions
        // Called List Generation and Interaction
        public enum FilterType { Content=0, Instance, Name }
        public BossHandler[] GenCalledList(FilterType ftype, string Filter) {
            if (Filter.Equals("All",StringComparison.OrdinalIgnoreCase)) {
                // Resets the calledList to the full, unfiltered List
                TheEZModeBoss_Called  = TheEZModeBoss;
                TheAvgBoss_Called     = TheAvgBoss;
                TheHardestBoss_Called = TheHardestBoss;
                return calledList = list;
            }
            // Generate a list based upon the specialized Filter, only 1 thing can be compared at a time though
            List<BossHandler> retList = new List<BossHandler>();
            foreach (BossHandler boss in list) {
                switch (ftype) {
                    case FilterType.Content:  { if (boss.ContentString.Equals( Filter,StringComparison.OrdinalIgnoreCase)) { retList.Add(boss); } break; }
                    case FilterType.Instance: { if (boss.Instance.Equals(Filter,StringComparison.OrdinalIgnoreCase)) { retList.Add(boss); } break; }
                    case FilterType.Name:     { if (boss.Name.Equals(    Filter,StringComparison.OrdinalIgnoreCase)) { retList.Add(boss); } break; }
                    default: { /*Invalid type, do nothing*/ break; }
                }
            }
            BossHandler[] retList2 = retList.ToArray();
            // Gen the special bosses based upon this filtered list
            TheEZModeBoss_Called  = GenTheEZModeBoss(retList2);
            TheAvgBoss_Called     = GenTheAvgBoss(retList2);
            TheHardestBoss_Called = GenTheHardestBoss(retList2);
            return calledList = retList2;
        }
        #region Name Calling
        public List<string> GetBossNames() {
            List<string> names = new List<string>() { };
            names.Add(TheEZModeBoss.Name);
            names.Add(TheAvgBoss.Name);
            names.Add(TheHardestBoss.Name);
            foreach (BossHandler boss in calledList) {
                names.Add(boss.Name);
            }
            return names;
        }
        public string[] GetBossNamesAsArray() { return GetBossNames().ToArray(); }
        public List<string> GetBetterBossNames() {
            List<string> names = new List<string>() { };
            names.Add(TheEZModeBoss.Name);
            names.Add(TheAvgBoss.Name);
            names.Add(TheHardestBoss.Name);
            foreach (BossHandler boss in calledList) {
                string name = boss.ContentString + " : " + boss.Instance + " : " + boss.Name;
                names.Add(name);
            }
            return names;
        }
        public string[] GetBetterBossNamesAsArray() { return GetBetterBossNames().ToArray(); }
        public BossHandler GetBossFromName(string name) {
            BossHandler retBoss = new BossHandler();
            if      (TheEZModeBoss_Called.Name  == name) { retBoss = TheEZModeBoss_Called;  }
            else if (TheAvgBoss_Called.Name     == name) { retBoss = TheAvgBoss_Called;     }
            else if (TheHardestBoss_Called.Name == name) { retBoss = TheHardestBoss_Called; }
            else {
                foreach (BossHandler boss in calledList) {
                    if(boss.Name == name){
                        retBoss = boss;
                        break;
                    }
                }
            }
            return retBoss;
        }
        public BossHandler GetBossFromBetterName(string name) {
            BossHandler retBoss = new BossHandler();
            if      (TheEZModeBoss_Called.Name  == name) { retBoss = TheEZModeBoss_Called;  }
            else if (TheAvgBoss_Called.Name     == name) { retBoss = TheAvgBoss_Called;     }
            else if (TheHardestBoss_Called.Name == name) { retBoss = TheHardestBoss_Called; }
            else {
                foreach (BossHandler boss in calledList) {
                    string checkName = boss.ContentString + " : " + boss.Instance + " : " + boss.Name;
                    if(checkName == name){
                        retBoss = boss;
                        break;
                    }
                }
            }
            return retBoss;
        }
        public List<string> GetFilterList(FilterType ftype) {
            List<string> names = new List<string>() { };
            switch (ftype) {
                case FilterType.Content: {
                    if (!names.Contains(TheEZModeBoss_Called.ContentString)) { names.Add(TheEZModeBoss_Called.ContentString); }
                    if (!names.Contains(TheAvgBoss_Called.ContentString)) { names.Add(TheAvgBoss_Called.ContentString); }
                    if (!names.Contains(TheHardestBoss_Called.ContentString)) { names.Add(TheHardestBoss_Called.ContentString); }
                    foreach (BossHandler boss in calledList) { if (!names.Contains(boss.ContentString)) { names.Add(boss.ContentString); } }
                    break;
                }
                case FilterType.Instance: {
                    if (!names.Contains(TheEZModeBoss_Called.Instance)) { names.Add(TheEZModeBoss_Called.Instance); }
                    if (!names.Contains(TheAvgBoss_Called.Instance)) { names.Add(TheAvgBoss_Called.Instance); }
                    if (!names.Contains(TheHardestBoss_Called.Instance)) { names.Add(TheHardestBoss_Called.Instance); }
                    foreach (BossHandler boss in calledList) { if (!names.Contains(boss.Instance)) { names.Add(boss.Instance); } }
                    break;
                }
                case FilterType.Name: {
                    if (!names.Contains(TheEZModeBoss_Called.Name)) { names.Add(TheEZModeBoss_Called.Name); }
                    if (!names.Contains(TheAvgBoss_Called.Name)) { names.Add(TheAvgBoss_Called.Name); }
                    if (!names.Contains(TheHardestBoss_Called.Name)) { names.Add(TheHardestBoss_Called.Name); }
                    foreach (BossHandler boss in calledList) { if (!names.Contains(boss.Name)) { names.Add(boss.Name); } }
                    break;
                }
                default: { /*Invalid type, do nothing*/ break; }
            }
            return names;
        }
        public string[] GetFilterListAsArray(FilterType ftype) { return GetFilterList(ftype).ToArray(); }
        /// <summary>
        /// Generates a Fight Info description listing the stats of the fight as well as any comments listed for the boss
        /// </summary>
        /// <param name="bossName">The full name of the boss, can use BetterBossName</param>
        /// <param name="useBettername">Set to True if you are passing the BetterBossName version of the Boss Name for proper verification</param>
        /// <returns>The generated string</returns>
        public string GenInfoString(string bossName, bool useBettername) {
            BossHandler boss = useBettername ? GetBossFromBetterName(bossName) : GetBossFromName(bossName);
            string retVal = boss.GenInfoString();
            return retVal;
        }
        #endregion
        // The Special Bosses
        #region Convert Lists
        bool useGoodBoyAvg = true;
        private bool IsContent25Man(BossHandler.TierLevels tier) {
            if (tier == BossHandler.TierLevels.T11_25
                || tier == BossHandler.TierLevels.T11_25H
                || tier == BossHandler.TierLevels.T12_25
                || tier == BossHandler.TierLevels.T12_25H
                || tier == BossHandler.TierLevels.T13_LFR
                || tier == BossHandler.TierLevels.T13_25
                || tier == BossHandler.TierLevels.T13_25H)
            {
                return true;
            }
            return false;
        }
        public void ConvertList_Move(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> moves = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Move;
                if (s.Frequency != -1f) {
                    moves.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a move that doesn't actually occur
                    moves.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Moves = moves;
            s = dummy.DynamicCompiler_Move;
            if (s.Frequency != -1) { retboss.Moves.Add(s); }
        }
        public void ConvertList_Stun(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> stuns = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Stun;
                if (s.Frequency != -1f) {
                    stuns.Add(s);
                } else if(useGoodBoyAvg) {
                    // Adds a stun that doesn't actually occur
                    stuns.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Stuns = stuns;
            s = dummy.DynamicCompiler_Stun;
            if (s.Frequency != -1) { retboss.Stuns.Add(s); }
        }
        public void ConvertList_Fear(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> fears = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Fear;
                if (s.Frequency != -1f) {
                    fears.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a fear that doesn't actually occur
                    fears.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Fears = fears;
            s = dummy.DynamicCompiler_Fear;
            if (s.Frequency != -1) { retboss.Fears.Add(s); }
        }
        public void ConvertList_Root(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> roots = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Root;
                if (s.Frequency != -1f) {
                    roots.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a fear that doesn't actually occur
                    roots.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Roots = roots;
            s = dummy.DynamicCompiler_Root;
            if (s.Frequency != -1) { retboss.Roots.Add(s); }
        }
        public void ConvertList_Dsrm(BossHandler[] passedList, BossHandler retboss) {
            BossHandler dummy = new BossHandler(); dummy.BerserkTimer = retboss.BerserkTimer;
            Impedance s;
            List<Impedance> disarms = new List<Impedance>();
            //
            foreach (BossHandler boss in passedList)
            {
                s = boss.DynamicCompiler_Dsrm;
                if (s.Frequency != -1f) {
                    disarms.Add(s);
                } else if (useGoodBoyAvg) {
                    // Adds a fear that doesn't actually occur
                    disarms.Add(new Impedance()
                    {
                        Frequency = retboss.BerserkTimer,
                        Duration = 0f, //retboss.BerserkTimer,
                        Chance = 1f,
                        Breakable = true,
                    });
                }
            }
            dummy.Disarms = disarms;
            s = dummy.DynamicCompiler_Dsrm;
            if (s.Frequency != -1) { retboss.Disarms.Add(s); }
        }
        #endregion

        private void CalculateEZAttack(List<Attack> attacks, int maxPlayers, string name, bool defaultmelee, out Attack toAdd) {
            bool isDot = attacks[0].IsDoT;
            float perhit = BossHandler.StandardMeleePerHit[(int)BossHandler.TierLevels.T14_25H] * 2, // start at normal attack, some bosses could be less and it will pick that up
                pertick = BossHandler.StandardMeleePerHit[(int)BossHandler.TierLevels.T14_25H] * 2,
                duration = 20*60,
                //phasestarttime = 0,
                //phaseendtime = 20*60,
                tickinterval = 0,
                numtrg = maxPlayers,
                atkspd = 0f;
            Dictionary<ATTACK_TYPES, int> ATCounts = new Dictionary<ATTACK_TYPES, int>() {
                { ATTACK_TYPES.AT_MELEE, 0 },
                { ATTACK_TYPES.AT_RANGED, 0 },
                { ATTACK_TYPES.AT_AOE, 0 },
                { ATTACK_TYPES.AT_DOT, 0 },
            };
            Dictionary<ItemDamageType, int> DTCounts = new Dictionary<ItemDamageType, int>() {
                { ItemDamageType.Physical, 0 },
                { ItemDamageType.Arcane, 0 },
                { ItemDamageType.Fire, 0 },
                { ItemDamageType.Frost, 0 },
                { ItemDamageType.Holy, 0 },
                { ItemDamageType.Nature, 0 },
                { ItemDamageType.Shadow, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            foreach (Attack a in attacks) {
                perhit = Math.Min(perhit, a.DamagePerHit);
                if (isDot) {
                    pertick = Math.Min(pertick, a.DamagePerTick);
                    duration = Math.Min(duration, a.Duration);
                    tickinterval = Math.Max(tickinterval, a.TickInterval);
                }
                //phasestarttime = Math.Max(phasestarttime, a.PhaseStartTime); // this isn't actually gonna work
                //phaseendtime = Math.Min(phaseendtime, a.PhaseEndTime);
                numtrg = Math.Min(numtrg, a.MaxNumTargets);
                atkspd = Math.Max(atkspd, a.AttackSpeed);
                ATCounts[a.AttackType]++;
                DTCounts[a.DamageType]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++)
                {
                    if (a.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            ATTACK_TYPES at = ATTACK_TYPES.AT_MELEE;
            foreach(ATTACK_TYPES t in ATCounts.Keys){
                if (ATCounts[t] > ATCounts[at]) { at = t; }
                if (ATCounts[at] == ATCounts[ATTACK_TYPES.AT_MELEE]) { at = ATTACK_TYPES.AT_MELEE; }
            }
            ItemDamageType dt = ItemDamageType.Physical;
            foreach (ItemDamageType t in DTCounts.Keys) {
                if (DTCounts[t] > DTCounts[dt]) { dt = t; }
                if (DTCounts[dt] == DTCounts[ItemDamageType.Physical]) { dt = ItemDamageType.Physical; }
            }
            toAdd = new Attack {
                Name = name,
                IsDoT = isDot,
                AttackType = at,
                DamageType = dt,
                DamagePerHit = perhit,
                DamagePerTick = isDot ? pertick : 0,
                Duration = isDot ? duration : 0,
                TickInterval = isDot ? tickinterval : 0,
                MaxNumTargets = numtrg,
                AttackSpeed = atkspd,
                IsTheDefaultMelee = defaultmelee,
                //PhaseStartTime = phasestarttime,
                //PhaseEndTime = phaseendtime,
            };
            foreach (PLAYER_ROLES pr in ARCounts.Keys) {
                toAdd.AffectsRole[pr] = ARCounts[pr] > 0;
            }
        }
        private void CalculateEZTargetGroup(List<TargetGroup> groups, int maxPlayers, string name, out TargetGroup toAdd) {
            float duration = 20 * 60 * 1000,
                freq = 20 * 60,
                chance = 1f,
                numtrg = 10;
            Dictionary<bool, int> NBCounts = new Dictionary<bool, int>() {
                { false, 0 },
                { true, 0 },
            };
            Dictionary<int, int> LvlCounts = new Dictionary<int, int>() {
                { (int)POSSIBLE_LEVELS.LVLP0, 0 },
                { (int)POSSIBLE_LEVELS.LVLP1, 0 },
                { (int)POSSIBLE_LEVELS.LVLP2, 0 },
                { (int)POSSIBLE_LEVELS.LVLP3, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            //
            foreach (TargetGroup t in groups) {
                duration = Math.Min(duration, t.Duration);
                freq = Math.Min(freq, t.Frequency);
                chance = Math.Min(chance, t.Chance);
                numtrg = Math.Min(numtrg, t.NumTargs);
                NBCounts[t.NearBoss]++;
                LvlCounts[t.LevelOfTargets]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++) {
                    if (t.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            //
            bool snb = false;
            foreach(bool nb in NBCounts.Keys){
                if (NBCounts[nb] > NBCounts[snb]) { snb = nb; }
                if (NBCounts[snb] == NBCounts[false]) { snb = false; }
            }
            int slvl = (int)POSSIBLE_LEVELS.LVLP3;
            foreach(int lvl in LvlCounts.Keys){
                if (LvlCounts[lvl] > LvlCounts[slvl]) { slvl = lvl; }
                if (LvlCounts[slvl] == LvlCounts[(int)POSSIBLE_LEVELS.LVLP3]) { slvl = (int)POSSIBLE_LEVELS.LVLP3; }
            }
            toAdd = new TargetGroup {
                Name = name,
                Duration = duration,
                Frequency = freq,
                Chance = chance,
                NumTargs = numtrg,
                LevelOfTargets = slvl,
                NearBoss = snb,
            };
            foreach (PLAYER_ROLES pr in ARCounts.Keys) {
                toAdd.AffectsRole[pr] = ((float)ARCounts[pr] / (float)groups.Count) > 0.25f; // at least 25% of the affected roles matched
            }
        }
        private void CalculateEZImpedance(List<Impedance> imps, int maxPlayers, string name, out Impedance toAdd)
        {
            float duration = 20 * 60 * 1000,
                freq = 20 * 60,
                chance = 1f;
            Dictionary<bool, int> BreakCounts = new Dictionary<bool, int>() {
                { false, 0 },
                { true, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            //
            foreach (Impedance t in imps)
            {
                duration = Math.Min(duration, t.Duration);
                freq = Math.Min(freq, t.Frequency);
                chance = Math.Min(chance, t.Chance);
                BreakCounts[t.Breakable]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++)
                {
                    if (t.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            //
            bool snb = false;
            foreach (bool nb in BreakCounts.Keys)
            {
                if (BreakCounts[nb] > BreakCounts[snb]) { snb = nb; }
                if (BreakCounts[snb] == BreakCounts[false]) { snb = false; }
            }
            toAdd = new Impedance
            {
                Name = name,
                Duration = duration,
                Frequency = freq,
                Chance = chance,
                Breakable = snb,
            };
            foreach (PLAYER_ROLES pr in ARCounts.Keys)
            {
                toAdd.AffectsRole[pr] = ((float)ARCounts[pr] / (float)imps.Count) > 0.25f; // at least 25% of the affected roles matched
            }
        }
        private void CalculateAvgAttack(List<Attack> attacks, float fightDur, int maxPlayers, string name, bool defaultmelee, out Attack toAdd)
        {
            bool isDot = attacks[0].IsDoT;
            float perhit = 0,
                pertick = 0,
                duration = 0,
                tickinterval = 0,
                numtrg = 0,
                atkspd = 0f,
                phaseUptime = 0;
            Dictionary<ATTACK_TYPES, int> ATCounts = new Dictionary<ATTACK_TYPES, int>() {
                { ATTACK_TYPES.AT_MELEE, 0 },
                { ATTACK_TYPES.AT_RANGED, 0 },
                { ATTACK_TYPES.AT_AOE, 0 },
                { ATTACK_TYPES.AT_DOT, 0 },
            };
            Dictionary<ItemDamageType, int> DTCounts = new Dictionary<ItemDamageType, int>() {
                { ItemDamageType.Physical, 0 },
                { ItemDamageType.Arcane, 0 },
                { ItemDamageType.Fire, 0 },
                { ItemDamageType.Frost, 0 },
                { ItemDamageType.Holy, 0 },
                { ItemDamageType.Nature, 0 },
                { ItemDamageType.Shadow, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            foreach (Attack a in attacks) {
                perhit += a.DamagePerHit;
                if (isDot) {
                    pertick += a.DamagePerTick;
                    duration += a.Duration;
                    tickinterval += a.TickInterval;
                }
                numtrg += a.MaxNumTargets;
                atkspd += a.AttackSpeed;
                phaseUptime += a.FightUptimePercent;
                ATCounts[a.AttackType]++;
                DTCounts[a.DamageType]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++)
                {
                    if (a.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            perhit /= (float)attacks.Count;
            if (isDot) {
                pertick /= (float)attacks.Count;
                duration /= (float)attacks.Count;
                tickinterval /= (float)attacks.Count;
            }
            numtrg /= (float)attacks.Count;
            atkspd /= (float)attacks.Count;
            phaseUptime /= (float)attacks.Count;
            ATTACK_TYPES at = ATTACK_TYPES.AT_MELEE;
            foreach(ATTACK_TYPES t in ATCounts.Keys){
                if (ATCounts[t] > ATCounts[at]) { at = t; }
                if (ATCounts[at] == ATCounts[ATTACK_TYPES.AT_MELEE]) { at = ATTACK_TYPES.AT_MELEE; }
            }
            ItemDamageType dt = ItemDamageType.Physical;
            foreach (ItemDamageType t in DTCounts.Keys) {
                if (DTCounts[t] > DTCounts[dt]) { dt = t; }
                if (DTCounts[dt] == DTCounts[ItemDamageType.Physical]) { dt = ItemDamageType.Physical; }
            }
            toAdd = new Attack {
                Name = name,
                IsDoT = isDot,
                AttackType = at,
                DamageType = dt,
                DamagePerHit = perhit,
                DamagePerTick = isDot ? pertick : 0,
                Duration = isDot ? duration : 0,
                TickInterval = isDot ? tickinterval : 0,
                MaxNumTargets = numtrg,
                AttackSpeed = atkspd,
                IsTheDefaultMelee = defaultmelee,
                FightDuration = fightDur,
            };
            toAdd.PhaseTimes[1] = new float[] { 0f, fightDur * phaseUptime };
            foreach (PLAYER_ROLES pr in ARCounts.Keys) {
                toAdd.AffectsRole[pr] = ((float)ARCounts[pr] / (float)attacks.Count) > 0.25f; // at least 25% of the affected roles matched
            }
        }
        private void CalculateAvgTargetGroup(List<TargetGroup> groups, float fightDur, int maxPlayers, string name, out TargetGroup toAdd) {
            float duration = 0,
                freq = 0,
                chance = 0,
                numtrg = 0,
                phaseUptime = 0;
            Dictionary<bool, int> NBCounts = new Dictionary<bool, int>() {
                { false, 0 },
                { true, 0 },
            };
            Dictionary<int, int> LvlCounts = new Dictionary<int, int>() {
                { (int)POSSIBLE_LEVELS.LVLP0, 0 },
                { (int)POSSIBLE_LEVELS.LVLP1, 0 },
                { (int)POSSIBLE_LEVELS.LVLP2, 0 },
                { (int)POSSIBLE_LEVELS.LVLP3, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            //
            foreach (TargetGroup t in groups) {
                duration += t.Duration;
                freq += t.Frequency;
                chance += t.Chance;
                numtrg += t.NumTargs;
                phaseUptime += t.FightUptimePercent;
                NBCounts[t.NearBoss]++;
                LvlCounts[t.LevelOfTargets]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++) {
                    if (t.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            //
            duration /= (float)groups.Count;
            freq /= (float)groups.Count;
            chance /= (float)groups.Count;
            numtrg /= (float)groups.Count;
            phaseUptime /= (float)groups.Count;
            //
            bool snb = false;
            foreach(bool nb in NBCounts.Keys){
                if (NBCounts[nb] > NBCounts[snb]) { snb = nb; }
                if (NBCounts[snb] == NBCounts[false]) { snb = false; }
            }
            int slvl = (int)POSSIBLE_LEVELS.LVLP3;
            foreach(int lvl in LvlCounts.Keys){
                if (LvlCounts[lvl] > LvlCounts[slvl]) { slvl = lvl; }
                if (LvlCounts[slvl] == LvlCounts[(int)POSSIBLE_LEVELS.LVLP3]) { slvl = (int)POSSIBLE_LEVELS.LVLP3; }
            }
            toAdd = new TargetGroup {
                Name = name,
                Duration = duration,
                Frequency = freq,
                Chance = chance,
                NumTargs = numtrg,
                LevelOfTargets = slvl,
                NearBoss = snb,
                FightDuration = fightDur,
            };
            toAdd.PhaseTimes[1] = new float[] { 0f, fightDur * phaseUptime };
            foreach (PLAYER_ROLES pr in ARCounts.Keys) {
                toAdd.AffectsRole[pr] = ((float)ARCounts[pr] / (float)groups.Count) > 0.25f; // at least 25% of the affected roles matched
            }
        }
        private void CalculateAvgImpedance(List<Impedance> imps, float fightDur, int maxPlayers, string name, out Impedance toAdd)
        {
            float duration = 0,
                freq = 0,
                chance = 0,
                phaseUptime = 0;
            Dictionary<bool, int> BreakCounts = new Dictionary<bool, int>() {
                { false, 0 },
                { true, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            //
            foreach (Impedance t in imps)
            {
                duration += t.Duration;
                freq += t.Frequency;
                chance += t.Chance;
                phaseUptime += t.FightUptimePercent;
                BreakCounts[t.Breakable]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++)
                {
                    if (t.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            //
            duration /= (float)imps.Count;
            freq /= (float)imps.Count;
            chance /= (float)imps.Count;
            phaseUptime /= (float)imps.Count;
            //
            bool snb = false;
            foreach (bool nb in BreakCounts.Keys)
            {
                if (BreakCounts[nb] > BreakCounts[snb]) { snb = nb; }
                if (BreakCounts[snb] == BreakCounts[false]) { snb = false; }
            }
            toAdd = new Impedance
            {
                Name = name,
                Duration = duration,
                Frequency = freq,
                Chance = chance,
                Breakable = snb,
                FightDuration = fightDur,
            };
            toAdd.PhaseTimes[1] = new float[] { 0, fightDur * phaseUptime };
            foreach (PLAYER_ROLES pr in ARCounts.Keys)
            {
                toAdd.AffectsRole[pr] = ((float)ARCounts[pr] / (float)imps.Count) > 0.25f; // at least 25% of the affected roles matched
            }
        }
        private void CalculateHardAttack(List<Attack> attacks, int maxPlayers, string name, bool defaultmelee, out Attack toAdd)
        {
            bool isDot = attacks[0].IsDoT;
            float perhit = 0,
                pertick = 0,
                duration = 0,
                tickinterval = 12*60,
                numtrg = 0,
                atkspd = 12*60;
            Dictionary<ATTACK_TYPES, int> ATCounts = new Dictionary<ATTACK_TYPES, int>() {
                { ATTACK_TYPES.AT_MELEE, 0 },
                { ATTACK_TYPES.AT_RANGED, 0 },
                { ATTACK_TYPES.AT_AOE, 0 },
                { ATTACK_TYPES.AT_DOT, 0 },
            };
            Dictionary<ItemDamageType, int> DTCounts = new Dictionary<ItemDamageType, int>() {
                { ItemDamageType.Physical, 0 },
                { ItemDamageType.Arcane, 0 },
                { ItemDamageType.Fire, 0 },
                { ItemDamageType.Frost, 0 },
                { ItemDamageType.Holy, 0 },
                { ItemDamageType.Nature, 0 },
                { ItemDamageType.Shadow, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            foreach (Attack a in attacks) {
                perhit = Math.Max(perhit, a.DamagePerHit);
                if (isDot) {
                    pertick = Math.Max(pertick, a.DamagePerTick);
                    duration = Math.Max(duration, a.Duration);
                    tickinterval = Math.Min(tickinterval, a.TickInterval);
                }
                numtrg = Math.Max(numtrg, a.MaxNumTargets);
                atkspd = Math.Min(atkspd, a.AttackSpeed);
                ATCounts[a.AttackType]++;
                DTCounts[a.DamageType]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++)
                {
                    if (a.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            ATTACK_TYPES at = ATTACK_TYPES.AT_MELEE;
            foreach(ATTACK_TYPES t in ATCounts.Keys){
                if (ATCounts[t] > ATCounts[at]) { at = t; }
                if (ATCounts[at] == ATCounts[ATTACK_TYPES.AT_MELEE]) { at = ATTACK_TYPES.AT_MELEE; }
            }
            ItemDamageType dt = ItemDamageType.Physical;
            foreach (ItemDamageType t in DTCounts.Keys) {
                if (DTCounts[t] > DTCounts[dt]) { dt = t; }
                if (DTCounts[dt] == DTCounts[ItemDamageType.Physical]) { dt = ItemDamageType.Physical; }
            }
            toAdd = new Attack {
                Name = name,
                IsDoT = isDot,
                AttackType = at,
                DamageType = dt,
                DamagePerHit = perhit,
                DamagePerTick = isDot ? pertick : 0,
                Duration = isDot ? duration : 0,
                TickInterval = isDot ? tickinterval : 0,
                MaxNumTargets = numtrg,
                AttackSpeed = atkspd,
                IsTheDefaultMelee = defaultmelee,
            };
            foreach (PLAYER_ROLES pr in ARCounts.Keys) {
                toAdd.AffectsRole[pr] = ARCounts[pr] > 0;
            }
        }
        private void CalculateHardTargetGroup(List<TargetGroup> groups, int maxPlayers, string name, out TargetGroup toAdd) {
            float duration = 0,
                freq = 0,
                chance = 0,
                numtrg = 0;
            Dictionary<bool, int> NBCounts = new Dictionary<bool, int>() {
                { false, 0 },
                { true, 0 },
            };
            Dictionary<int, int> LvlCounts = new Dictionary<int, int>() {
                { (int)POSSIBLE_LEVELS.LVLP0, 0 },
                { (int)POSSIBLE_LEVELS.LVLP1, 0 },
                { (int)POSSIBLE_LEVELS.LVLP2, 0 },
                { (int)POSSIBLE_LEVELS.LVLP3, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            //
            foreach (TargetGroup t in groups) {
                duration = Math.Max(duration, t.Duration);
                freq = Math.Max(freq, t.Frequency);
                chance = Math.Max(chance, t.Chance);
                numtrg = Math.Max(numtrg, t.NumTargs);
                NBCounts[t.NearBoss]++;
                LvlCounts[t.LevelOfTargets]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++) {
                    if (t.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            //
            bool snb = false;
            foreach(bool nb in NBCounts.Keys){
                if (NBCounts[nb] > NBCounts[snb]) { snb = nb; }
                if (NBCounts[snb] == NBCounts[false]) { snb = false; }
            }
            int slvl = (int)POSSIBLE_LEVELS.LVLP3;
            foreach(int lvl in LvlCounts.Keys){
                if (LvlCounts[lvl] > LvlCounts[slvl]) { slvl = lvl; }
                if (LvlCounts[slvl] == LvlCounts[(int)POSSIBLE_LEVELS.LVLP3]) { slvl = (int)POSSIBLE_LEVELS.LVLP3; }
            }
            toAdd = new TargetGroup {
                Name = name,
                Duration = duration,
                Frequency = freq,
                Chance = chance,
                NumTargs = numtrg,
                LevelOfTargets = slvl,
                NearBoss = snb,
            };
            foreach (PLAYER_ROLES pr in ARCounts.Keys) {
                toAdd.AffectsRole[pr] = ((float)ARCounts[pr] / (float)groups.Count) > 0.25f; // at least 25% of the affected roles matched
            }
        }
        private void CalculateHardImpedance(List<Impedance> imps, int maxPlayers, string name, out Impedance toAdd)
        {
            float duration = 0,
                freq = 0,
                chance = 0;
            Dictionary<bool, int> BreakCounts = new Dictionary<bool, int>() {
                { false, 0 },
                { true, 0 },
            };
            Dictionary<PLAYER_ROLES, int> ARCounts = new Dictionary<PLAYER_ROLES, int>() {
                { PLAYER_ROLES.MainTank, 0 },
                { PLAYER_ROLES.OffTank, 0 },
                { PLAYER_ROLES.TertiaryTank, 0 },
                { PLAYER_ROLES.MeleeDPS, 0 },
                { PLAYER_ROLES.RangedDPS, 0 },
                { PLAYER_ROLES.RaidHealer, 0 },
                { PLAYER_ROLES.MainTankHealer, 0 },
                { PLAYER_ROLES.OffAndTertTankHealer, 0 },
            };
            //
            foreach (Impedance t in imps)
            {
                duration = Math.Max(duration, t.Duration);
                freq = Math.Max(freq, t.Frequency);
                chance = Math.Max(chance, t.Chance);
                BreakCounts[t.Breakable]++;
                for (int i = 0; i < (int)PLAYER_ROLES.RaidHealer; i++)
                {
                    if (t.AffectsRole[(PLAYER_ROLES)i]) { ARCounts[(PLAYER_ROLES)i]++; }
                }
            }
            //
            bool snb = false;
            foreach (bool nb in BreakCounts.Keys)
            {
                if (BreakCounts[nb] > BreakCounts[snb]) { snb = nb; }
                if (BreakCounts[snb] == BreakCounts[false]) { snb = false; }
            }
            toAdd = new Impedance
            {
                Name = name,
                Duration = duration,
                Frequency = freq,
                Chance = chance,
                Breakable = snb,
            };
            foreach (PLAYER_ROLES pr in ARCounts.Keys)
            {
                toAdd.AffectsRole[pr] = ((float)ARCounts[pr] / (float)imps.Count) > 0.25f; // at least 25% of the affected roles matched
            }
        }
        private void calcImp(BossHandler[] passedList, ref BossHandler retboss, int type, int level) {
            List<Impedance> imps = new List<Impedance>();
            foreach (BossHandler boss in passedList)
            {
                switch (type)
                {
                    case 1: { imps.AddRange(boss.Moves.FindAll(a => a.Validate)); break; }
                    case 2: { imps.AddRange(boss.Stuns.FindAll(a => a.Validate)); break; }
                    case 3: { imps.AddRange(boss.Fears.FindAll(a => a.Validate)); break; }
                    case 4: { imps.AddRange(boss.Roots.FindAll(a => a.Validate)); break; }
                    case 5: { imps.AddRange(boss.Silences.FindAll(a => a.Validate)); break; }
                    case 6: { imps.AddRange(boss.Disarms.FindAll(a => a.Validate)); break; }
                    default: { break; }
                }
            }
            if (imps.Count > 0)
            {
                Impedance toAdd;
                switch (level)
                {
                    case 1: { CalculateEZImpedance(imps, retboss.Max_Players, "Easy ", out toAdd); break; }
                    case 2: { CalculateAvgImpedance(imps, retboss.BerserkTimer, retboss.Max_Players, "Avg ", out toAdd); break; }
                    case 3:
                    default: { CalculateHardImpedance(imps, retboss.Max_Players, "Impossible ", out toAdd); break; }
                }
                switch (type)
                {
                    case 1: { toAdd.Name += "Movement"; retboss.Moves.Add(toAdd); break; }
                    case 2: { toAdd.Name += "Stuns"; retboss.Stuns.Add(toAdd); break; }
                    case 3: { toAdd.Name += "Fears"; retboss.Fears.Add(toAdd); break; }
                    case 4: { toAdd.Name += "Roots"; retboss.Roots.Add(toAdd); break; }
                    case 5: { toAdd.Name += "Silences"; retboss.Silences.Add(toAdd); break; }
                    case 6: { toAdd.Name += "Disarms"; retboss.Disarms.Add(toAdd); break; }
                    default: { break; }
                }
            }
        }
        private BossHandler GenTheEZModeBoss(BossHandler[] passedList)
        {
            useGoodBoyAvg = true;
            BossHandler retboss = new BossHandler();
            if (passedList.Length < 1) { return retboss; }
            float value = 0f;
            #region Info
            retboss.Name = "An Easy Boss";
            value = (int)BossHandler.TierLevels.T14_LFR; foreach (BossHandler boss in passedList) { value = Math.Min(value, (int)boss.Content); } retboss.Content = (BossHandler.TierLevels)Math.Floor(value);
            // Instance Skipped
            retboss.Comment = "An Easy Boss is a compilation of every Boss in your current filter. "
                            + "It takes the lightest values from every point of reference such as Health, "
                            + "Attack Damage, Movement and brings them into a single target.\r\nThe "
                            + "Primary intention of An Easy Boss is to help fresh 90's with getting ready "
                            + "to raid. Once you have the appropriate ilevels for raiding and can surpass the "
                            + "requirements listed above, you should move on to The Average Boss.";
            #endregion
            #region Basics
            value = passedList[0].Level; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Level); } retboss.Level = (int)value;
            value = passedList[0].Armor; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Armor); } retboss.Armor = (int)value;
            value = passedList[0].Health; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Health); } retboss.Health = (int)value;

            {
                value = passedList[0].MobType;
                int[] counts = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                foreach (BossHandler boss in passedList) {
                    counts[boss.MobType]++;
                }
                for (int m = 0; m < 9; m++)
                {
                    if (m != (int)value && counts[m] > counts[(int)value]) { value = m; }
                }
                if (counts[(int)value] == counts[(int)MOB_TYPES.HUMANOID]) { value = (int)MOB_TYPES.HUMANOID; }
                retboss.MobType = (int)value;
            }

            value = 1f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.BerserkTimer); } retboss.BerserkTimer = (int)Math.Ceiling(value);
            value = 1f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.SpeedKillTimer); } retboss.SpeedKillTimer = (int)Math.Ceiling(value);
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, (float)boss.InBackPerc_Melee); } retboss.InBackPerc_Melee = value;
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, (float)boss.InBackPerc_Ranged); } retboss.InBackPerc_Ranged = value;
            value = 25f; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Min_Tanks); } retboss.Min_Tanks = (int)Math.Ceiling(value);
            value = 25f; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.Min_Healers); } retboss.Min_Healers = (int)Math.Ceiling(value);
            #endregion
            #region Offensive
            #region Multi-Targets
            {
                // Target Groups
                List<TargetGroup> groups = new List<TargetGroup>();
                foreach (BossHandler boss in passedList)
                {
                    groups.AddRange(boss.Targets.FindAll(a => a.Validate));
                }
                if (groups.Count > 0)
                {
                    TargetGroup toAdd;
                    CalculateEZTargetGroup(groups, retboss.Max_Players, "Easy Target Groups", out toAdd);
                    retboss.Targets.Add(toAdd);
                }
            }
            #endregion
            #region Attacks
            {
                {
                    // Regular Melee
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && !a.IsFromAnAdd && a.AttackSpeed < 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0) {
                        Attack toAdd;
                        CalculateEZAttack(attacks, retboss.Max_Players, "Easy Melee", true, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Special Melee
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && a.AttackSpeed >= 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0) {
                        Attack toAdd;
                        CalculateEZAttack(attacks, retboss.Max_Players, "Easy Special Melee", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Melee from Adds
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && a.IsFromAnAdd && a.AttackSpeed < 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0) {
                        Attack toAdd;
                        CalculateEZAttack(attacks, retboss.Max_Players, "Easy Melee from Adds", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Ranged
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_RANGED) && a.Validate && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0) {
                        Attack toAdd;
                        CalculateEZAttack(attacks, retboss.Max_Players, "Easy Ranged", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // AoE
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_AOE) && a.Validate && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0) {
                        Attack toAdd;
                        CalculateEZAttack(attacks, retboss.Max_Players, "Easy AoE", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // DoTs
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList) {
                        attacks.AddRange(boss.Attacks.FindAll(a => a.IsDoT && a.Validate && !a.DamageIsPerc));
                    }
                    if (attacks.Count > 0) {
                        Attack toAdd;
                        CalculateEZAttack(attacks, retboss.Max_Players, "Easy DoT", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
            }
            #endregion
            #endregion
            #region Defensive
            foreach (ItemDamageType t in DamageTypes) {
                value = (float)passedList[0].Resistance(t);
                foreach (BossHandler boss in passedList) {
                    value = (float)Math.Min(value, boss.Resistance(t));
                }
                retboss.Resistance(t, value);
            }
            #endregion
            #region Impedances
            calcImp(passedList, ref retboss, 1, 1);
            calcImp(passedList, ref retboss, 2, 1);
            calcImp(passedList, ref retboss, 3, 1);
            calcImp(passedList, ref retboss, 4, 1);
            calcImp(passedList, ref retboss, 5, 1);
            calcImp(passedList, ref retboss, 6, 1);
            #endregion
            //
            return retboss;
        }
        private BossHandler GenTheAvgBoss(BossHandler[] passedList) {
            useGoodBoyAvg = true;
            BossHandler retboss = new BossHandler();
            if (passedList.Length < 1) { return retboss; }
            float value = 0f;
            #region Info
            retboss.Name = "The Average Boss";
            value = 0; foreach (BossHandler boss in passedList) { value += (int)boss.Content; } retboss.Content = (BossHandler.TierLevels)Math.Floor(value / (float)passedList.Length);
            // Instance Skipped
            retboss.Comment = "The Average Boss is a compilation of every Boss in your current filter. "
                            + "It takes averaged values from every point of reference such as Health, "
                            + "Attack Damage, Movement and brings them into a single target.\r\nMost "
                            + "users should perform calculations against this specialized Boss to get a "
                            + "solid view of your current preparedness for any given fight.";
            #endregion
            #region Basics
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Level; } value /= passedList.Length; retboss.Level = (int)value;
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Armor; } value /= passedList.Length; retboss.Armor = (int)value;
            {
                value = passedList[0].MobType;
                int[] counts = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                foreach (BossHandler boss in passedList)
                {
                    counts[boss.MobType]++;
                }
                for (int m = 0; m < 9; m++)
                {
                    if (m != (int)value && counts[m] > counts[(int)value]) { value = m; }
                }
                if (counts[(int)value] == counts[(int)MOB_TYPES.HUMANOID]) { value = (int)MOB_TYPES.HUMANOID; }
                retboss.MobType = (int)value;
            }
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.BerserkTimer; } value /= passedList.Length; retboss.BerserkTimer = (int)Math.Floor(value);
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.SpeedKillTimer; } value /= passedList.Length; retboss.SpeedKillTimer = (int)Math.Floor(value);
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Health; } value /= passedList.Length; retboss.Health = value;
            value = 0f; foreach (BossHandler boss in passedList) { value += (float)boss.InBackPerc_Melee; } value /= passedList.Length; retboss.InBackPerc_Melee = value;
            value = 0f; foreach (BossHandler boss in passedList) { value += (float)boss.InBackPerc_Ranged; } value /= passedList.Length; retboss.InBackPerc_Ranged = value;
            bool is25 = false;
            foreach (BossHandler boss in passedList) { if (IsContent25Man(boss.Content)) { is25 = true; break; } }
            retboss.Max_Players = is25 ? 25 : 10;
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Min_Tanks; } value /= passedList.Length; retboss.Min_Tanks = (int)Math.Floor(value);
            value = 0f; foreach (BossHandler boss in passedList) { value += boss.Min_Healers; } value /= passedList.Length; retboss.Min_Healers = (int)Math.Floor(value);
            #endregion
            #region Offensive
            #region MultiTargs
            {
                // Target Groups
                List<TargetGroup> groups = new List<TargetGroup>();
                foreach (BossHandler boss in passedList)
                {
                    groups.AddRange(boss.Targets.FindAll(a => a.Validate));
                }
                if (groups.Count > 0)
                {
                    TargetGroup toAdd;
                    CalculateAvgTargetGroup(groups, retboss.BerserkTimer, retboss.Max_Players, "Avg Target Groups", out toAdd);
                    retboss.Targets.Add(toAdd);
                }
            }
            #endregion
            #region Attacks
            {
                {
                    // Regular Melee
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && !a.IsFromAnAdd && a.AttackSpeed < 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateAvgAttack(attacks, retboss.BerserkTimer, retboss.Max_Players, "Avg Melee", true, out toAdd);
                        toAdd.PhaseTimes[1] = new float[] { 0f, retboss.BerserkTimer }; // Since this is the default melee, force it back to 100% uptime
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Special Melee
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && a.AttackSpeed >= 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateAvgAttack(attacks, retboss.BerserkTimer, retboss.Max_Players, "Avg Special Melee", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Melee from Adds
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && a.IsFromAnAdd && a.AttackSpeed < 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateAvgAttack(attacks, retboss.BerserkTimer, retboss.Max_Players, "Avg Melee from Adds", true, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Ranged
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_RANGED) && a.Validate && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateAvgAttack(attacks, retboss.BerserkTimer, retboss.Max_Players, "Avg Ranged", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // AoE
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_AOE) && a.Validate && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateAvgAttack(attacks, retboss.BerserkTimer, retboss.Max_Players, "Avg AoE", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // DoTs
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => a.IsDoT && a.Validate && !a.DamageIsPerc));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateAvgAttack(attacks, retboss.BerserkTimer, retboss.Max_Players, "Avg DoT", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
            }
            #endregion
            #endregion
            #region Defensive
            foreach (ItemDamageType t in DamageTypes) {
                value = 0f;
                foreach (BossHandler boss in passedList) {
                    value += (float)boss.Resistance(t);
                }
                value /= passedList.Length;
                retboss.Resistance(t, value);
            }
            #endregion
            #region Impedances
            calcImp(passedList, ref retboss, 1, 2);
            calcImp(passedList, ref retboss, 2, 2);
            calcImp(passedList, ref retboss, 3, 2);
            calcImp(passedList, ref retboss, 4, 2);
            calcImp(passedList, ref retboss, 5, 2);
            calcImp(passedList, ref retboss, 6, 2);
            #endregion
            //
            return retboss;
        }
        private BossHandler GenTheHardestBoss(BossHandler[] passedList) {
            useGoodBoyAvg = false;
            BossHandler retboss = new BossHandler();
            if (passedList.Length < 1) { return retboss; }
            float value = 0f;
            #region Info
            retboss.Name = "An Impossible Boss";
            value = 0; foreach (BossHandler boss in passedList) { value = Math.Max(value, (int)boss.Content); } retboss.Content = (BossHandler.TierLevels)Math.Ceiling(value);
            // Instance Skipped
            retboss.Comment = "An Impossible Boss is a compilation of every Boss in your current filter. "
                            + "It takes the hardest values from every point of reference such as Health, "
                            + "Attack Damage, Movement and brings them into a single target.\r\nThe "
                            + "Primary intention of An Impossible Boss is really just for kicks, you are not "
                            + "expected to ever surpass these values and no boss in the game would ever be "
                            + "as difficult to beat.";
            #endregion
            #region Basics
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Level); } retboss.Level = (int)value;
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Armor); } retboss.Armor = (int)value;
            {
                value = passedList[0].MobType;
                int[] counts = new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                foreach (BossHandler boss in passedList)
                {
                    counts[boss.MobType]++;
                }
                for (int m = 0; m < 9; m++)
                {
                    if (m != (int)value && counts[m] > counts[(int)value]) { value = m; }
                }
                if (counts[(int)value] == counts[(int)MOB_TYPES.HUMANOID]) { value = (int)MOB_TYPES.HUMANOID; }
                retboss.MobType = (int)value;
            }
            value = 0f; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Health); } retboss.Health = value;
            value = passedList[0].BerserkTimer; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.BerserkTimer); } retboss.BerserkTimer = (int)Math.Floor(value);
            value = passedList[0].SpeedKillTimer; foreach (BossHandler boss in passedList) { value = Math.Min(value, boss.SpeedKillTimer); } retboss.SpeedKillTimer = (int)Math.Floor(value);
            value = 1.00f;      foreach (BossHandler boss in passedList) { value = (float)Math.Min(value, boss.InBackPerc_Melee   ); } retboss.InBackPerc_Melee  = value;
            value = 1.00f;      foreach (BossHandler boss in passedList) { value = (float)Math.Min(value, boss.InBackPerc_Ranged  ); } retboss.InBackPerc_Ranged = value;
            bool is25 = false;
            foreach (BossHandler boss in passedList) { if (IsContent25Man(boss.Content)) { is25 = true; break; } }
            retboss.Max_Players = is25 ? 25 : 10;
            value = 0; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Min_Tanks); } retboss.Min_Tanks = (int)value;
            value = 0; foreach (BossHandler boss in passedList) { value = Math.Max(value, boss.Min_Healers); } retboss.Min_Healers = (int)value;
            #endregion
            #region Offensive
            #region MultiTargs
            {
                // Target Groups
                List<TargetGroup> groups = new List<TargetGroup>();
                foreach (BossHandler boss in passedList)
                {
                    groups.AddRange(boss.Targets.FindAll(a => a.Validate));
                }
                if (groups.Count > 0)
                {
                    TargetGroup toAdd;
                    CalculateHardTargetGroup(groups, retboss.Max_Players, "Hard Target Groups", out toAdd);
                    retboss.Targets.Add(toAdd);
                }
            }
            #endregion
            #region Attacks
            {
                {
                    // Regular Melee
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && !a.IsFromAnAdd && a.AttackSpeed < 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateHardAttack(attacks, retboss.Max_Players, "Impossible Melee", true, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Special Melee
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && a.AttackSpeed >= 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateHardAttack(attacks, retboss.Max_Players, "Impossible Special Melee", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Melee from Adds
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_MELEE) && a.Validate && a.IsFromAnAdd && a.AttackSpeed < 5 && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateHardAttack(attacks, retboss.Max_Players, "Impossible Melee from Adds", true, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // Ranged
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_RANGED) && a.Validate && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateHardAttack(attacks, retboss.Max_Players, "Impossible Ranged", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // AoE
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => (a.AttackType == ATTACK_TYPES.AT_AOE) && a.Validate && !a.DamageIsPerc && !a.IsDoT));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateHardAttack(attacks, retboss.Max_Players, "Impossible AoE", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
                {
                    // DoTs
                    List<Attack> attacks = new List<Attack>();
                    foreach (BossHandler boss in passedList)
                    {
                        attacks.AddRange(boss.Attacks.FindAll(a => a.IsDoT && a.Validate && !a.DamageIsPerc));
                    }
                    if (attacks.Count > 0)
                    {
                        Attack toAdd;
                        CalculateHardAttack(attacks, retboss.Max_Players, "Impossible DoT", false, out toAdd);
                        retboss.Attacks.Add(toAdd);
                    }
                }
            }
            #endregion
            #endregion
            #region Defensive
            foreach (ItemDamageType t in DamageTypes) { value = 0f; foreach (BossHandler boss in passedList) { value = (float)Math.Max(value, boss.Resistance(t)); } retboss.Resistance(t, value); }
            #endregion
            #region Impedances
            calcImp(passedList, ref retboss, 1, 3);
            calcImp(passedList, ref retboss, 2, 3);
            calcImp(passedList, ref retboss, 3, 3);
            calcImp(passedList, ref retboss, 4, 3);
            calcImp(passedList, ref retboss, 5, 3);
            calcImp(passedList, ref retboss, 6, 3);
            #endregion
            //
            return retboss;
        }
        #endregion
    }
}
