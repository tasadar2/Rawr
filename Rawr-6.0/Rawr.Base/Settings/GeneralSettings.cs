﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Reflection;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace Rawr.Properties
{
    public class GeneralSettings
    {
        static GeneralSettings()
        {
            _default = new GeneralSettings();

            _default.CountGemmingsShown = 3;
            _default.UseMultithreading = true;
            _default.Locale = "en";
            _default.DisplayBuffSource = false;
            _default.DisplayGemNames = false;
            //_default.DisplayExtraItemInfo = false;
            _default.ProcEffectMode = 3;
            _default.CombinationEffectMode = 1;
            _default.ItemNameWidthSetting = 0;
            _default.ShowRelativeToEquipped = false;
            _default.DisplayUnusedStats = false;

            _default.UIScale = 0.90;

            _default.WelcomeScreenSeen = false;

            _default.EnforceGemRequirements = true;
            _default.EnforceGemRequirements_Meta = true;
            _default.EnforceGemRequirements_JC = true;
            _default.EnforceGemRequirements_Unique = true;

            _default.DisplayInCompactMode = false;

            _default.PTRMode = false;

            _default.UseLargeViewItemBrowser = false;
            _default.UseRegexInItemBrowser = false;

            _default.HasSeenHelpWindow = false;

            _default.AlertMeToBetaReleases = true;

            _default.FilterSideBarWidth = 200;

            _default.LastVersionRun = getCurrentVersion;
        }

        private static GeneralSettings _default;
        public static GeneralSettings Default { get { return _default; } set { _default = value; } }

        #region Version Checking
        [XmlIgnore]
        public bool IsNewVersionRunning { get; set; }

        public static string getCurrentVersion
        {
            get {
                // Get the Version number
                Assembly asm = Assembly.GetExecutingAssembly();
                string version = "Debug";
                if (asm.FullName != null)
                {
                    string[] parts = asm.FullName.Split(',');
                    version = parts[1];
                    while (version.Contains("Version=")) { version = version.Replace("Version=", ""); }
                    while (version.Contains(" ")) { version = version.Replace(" ", ""); }
                }
                return version;
            }
        }
        /// <summary>True if we are running a new version</summary>
        public bool compareVersions {
            get {
                string lastversion = LastVersionRun;
                if (!string.IsNullOrEmpty(lastversion))
                {
                    string currentVersion = getCurrentVersion;// System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
                    Match currentMatch = Regex.Match(currentVersion, @"(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)\.?(?<revsn>\d+)?");
                    Match otherMatch = Regex.Match(lastversion, @"(?<major>\d+)\.(?<minor>\d+)\.(?<build>\d+)\.?(?<revsn>\d+)?");
                    //
                    bool NewRelAvail = false;

                    int valueMajorC = 0; int valueMajorO = 0;
                    int valueMinorC = 0; int valueMinorO = 0;
                    int valueBuildC = 0; int valueBuildO = 0;
                    int valueRevsnC = 0; int valueRevsnO = 0;                    

                    if (currentMatch.Groups["major"].Value != "" && otherMatch.Groups["major"].Value != "")
                    {
                        valueMajorC = int.Parse(currentMatch.Groups["major"].Value);
                        valueMajorO = int.Parse(otherMatch.Groups["major"].Value);
                        if (valueMajorO != valueMajorC) { NewRelAvail = true; }
                    }
                    if (currentMatch.Groups["minor"].Value != "" && otherMatch.Groups["minor"].Value != "")
                    {
                        valueMinorC = int.Parse(currentMatch.Groups["minor"].Value);
                        valueMinorO = int.Parse(otherMatch.Groups["minor"].Value);
                        if (valueMinorO != valueMinorC) { NewRelAvail = true; }
                    }
                    if (currentMatch.Groups["build"].Value != "" && otherMatch.Groups["build"].Value != "")
                    {
                        valueBuildC = int.Parse(currentMatch.Groups["build"].Value);
                        valueBuildO = int.Parse(otherMatch.Groups["build"].Value);
                        if (valueBuildO != valueBuildC) { NewRelAvail = true; }
                    }
                    if (currentMatch.Groups["revsn"].Value != "" && otherMatch.Groups["revsn"].Value != "")
                    {
                        valueRevsnC = int.Parse(currentMatch.Groups["revsn"].Value);
                        valueRevsnO = int.Parse(otherMatch.Groups["revsn"].Value);
                        if (valueRevsnO != valueRevsnC) { NewRelAvail = true; }
                    }

                    return IsNewVersionRunning = NewRelAvail;
                }
                return IsNewVersionRunning = true;
            }
        }

        private string _LastVersionRun = "4.0.0";
        public string LastVersionRun { get { return _LastVersionRun; } set { _LastVersionRun = value; bool runIt = compareVersions; } }
        #endregion

        public bool HasSeenHelpWindow { get; set; }

        public bool AlertMeToBetaReleases { get; set; }

        public double UIScale {
            get { if (_UIScale <= 0) { _UIScale = 0.90; } return _UIScale; }
            set { _UIScale = Math.Round(value, 2); if (_UIScale <= 0) { _UIScale = 0.90; } }
        }
        public double _UIScale = 0.90;

        public int CountGemmingsShown { get; set; }
        public bool UseMultithreading { get; set; }
        public string Locale { get; set; }
        public bool DisplayBuffSource { get; set; }
        public bool DisplayGemNames { get; set; }
        public int ProcEffectMode { get; set; }
        public bool DisplayUnusedStats { get; set; }
        public bool HideProfEnchants { get; set; }
        public bool EnforceGemRequirements { get; set; }
        public bool EnforceGemRequirements_Meta { get; set; }
        public bool EnforceGemRequirements_JC { get; set; }
        public bool EnforceGemRequirements_Unique { get; set; }
        public bool DisplayInCompactMode { get; set; }
        public int CombinationEffectMode { get; set; }
        public int ItemNameWidthSetting { get; set; } // 0 = Normal (142 px), 1 = Wide (162 px), 2 = Widest (182 px)
        public bool WelcomeScreenSeen { get; set; }
        public bool PTRMode { get; set; }
        public bool UseLargeViewItemBrowser { get; set; }
        public bool UseRegexInItemBrowser { get; set; }
        private double _FilterSideBarWidth = 200;
        public double FilterSideBarWidth {
            get { return _FilterSideBarWidth; }
            set {
                if (value < 75) { value = 75; }
                _FilterSideBarWidth = value;
            }
        }
        public bool ShowRelativeToEquipped { get; set; }
    }
}
