﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Rawr.UI
{
    public partial class DG_BossTargetGroups : ChildWindow
    {
        static DG_BossTargetGroups() { }

        public DG_BossTargetGroups(List<TargetGroup> list)
        {
            InitializeComponent();

#if !SILVERLIGHT
            this.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
            this.WindowState = System.Windows.WindowState.Normal;
            this.WindowStyle = System.Windows.WindowStyle.ToolWindow;
            this.SizeToContent = SizeToContent.Height;
#endif

            TheList = list;
            SetListBox();
        }

        public List<TargetGroup> TheList { get { return _TheList ?? (_TheList = new List<TargetGroup>()); } set { _TheList = value; } }
        protected List<TargetGroup> _TheList = null;

        private void SetListBox()
        {
            LB_TheList.Items.Clear();
            foreach (TargetGroup s in TheList)
            {
                LB_TheList.Items.Add(s);
            }
        }

        private void BT_Add_Click(object sender, RoutedEventArgs e)
        {
            TargetGroup s = new TargetGroup()
            {
                Name = TB_Name.Text,
                Frequency = (float)NUD_Freq.Value,
                Duration = (float)NUD_Dur.Value,
                Chance = ((float)NUD_Chance.Value) / 100f,
                NearBoss = (bool)CK_NearBoss.IsChecked,
                NumTargs = (float)NUD_NumTargs.Value,
                LevelOfTargets = (int)CB_LevelOfTargets.SelectedItem,
                // Phase Info
                //PhaseStartTime = (float)NUD_PhaseStartTime.Value,
                //PhaseEndTime = (float)NUD_PhaseEndTime.Value,
            };
            // Targeting Includes
            s.AffectsRole[PLAYER_ROLES.MainTank]             = CK_AffectsMTank.IsChecked.GetValueOrDefault(false);
            s.AffectsRole[PLAYER_ROLES.OffTank]              = CK_AffectsOTank.IsChecked.GetValueOrDefault(false);
            s.AffectsRole[PLAYER_ROLES.TertiaryTank]         = CK_AffectsTTank.IsChecked.GetValueOrDefault(false);
            s.AffectsRole[PLAYER_ROLES.MeleeDPS]             = CK_AffectsMeleeDPS.IsChecked.GetValueOrDefault(false);
            s.AffectsRole[PLAYER_ROLES.RangedDPS]            = CK_AffectsRangedDPS.IsChecked.GetValueOrDefault(false);
            s.AffectsRole[PLAYER_ROLES.MainTankHealer]       = CK_AffectsMainTankHealer.IsChecked.GetValueOrDefault(false);
            s.AffectsRole[PLAYER_ROLES.OffAndTertTankHealer] = CK_AffectsOffTankHealer.IsChecked.GetValueOrDefault(false);
            s.AffectsRole[PLAYER_ROLES.RaidHealer]           = CK_AffectsRaidHealer.IsChecked.GetValueOrDefault(false);

            if (isEditing) {
                // Affect your changes to the currently selected one
                isEditing = false;
                int index = LB_TheList.SelectedIndex;
                TheList.RemoveAt(LB_TheList.SelectedIndex);
                TheList.Insert(index, s);
            } else { TheList.Add(s); }
            SetListBox();
        }

        private void BT_Delete_Click(object sender, RoutedEventArgs e)
        {
            int index = LB_TheList.SelectedIndex;
            if (index == -1) { return; }
            TheList.RemoveAt(index);
            SetListBox();
        }

        bool isEditing = false;
        private void LB_TheList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LB_TheList.SelectedIndex != -1) {
                TargetGroup selected = LB_TheList.SelectedItem as TargetGroup;
                //
                TB_Name.Text = selected.Name;
                NUD_Freq.Value                  = selected.Frequency;
                NUD_Dur.Value                   = selected.Duration;
                NUD_Chance.Value                = selected.Chance * 100f;
                CK_NearBoss.IsChecked           = selected.NearBoss;
                NUD_NumTargs.Value              = selected.NumTargs;
                CB_LevelOfTargets.SelectedItem  = selected.LevelOfTargets;
                // Phase Info
                //NUD_PhaseStartTime.Value = selected.PhaseStartTime;
                //NUD_PhaseEndTime.Value = selected.PhaseEndTime;
                // Targeting Includes
                CK_AffectsMTank.IsChecked          = selected.AffectsRole[PLAYER_ROLES.MainTank];
                CK_AffectsOTank.IsChecked          = selected.AffectsRole[PLAYER_ROLES.OffTank];
                CK_AffectsTTank.IsChecked          = selected.AffectsRole[PLAYER_ROLES.TertiaryTank];
                CK_AffectsMeleeDPS.IsChecked       = selected.AffectsRole[PLAYER_ROLES.MeleeDPS];
                CK_AffectsRangedDPS.IsChecked      = selected.AffectsRole[PLAYER_ROLES.RangedDPS];
                CK_AffectsMainTankHealer.IsChecked = selected.AffectsRole[PLAYER_ROLES.MainTankHealer];
                CK_AffectsOffTankHealer.IsChecked  = selected.AffectsRole[PLAYER_ROLES.OffAndTertTankHealer];
                CK_AffectsRaidHealer.IsChecked     = selected.AffectsRole[PLAYER_ROLES.RaidHealer];
                //
                isEditing = true;
            } else {
                // Reset the UI to a blank target group
                TB_Name.Text = "Unnamed";
                NUD_Freq.Value                  = 45;
                NUD_Dur.Value                   = 10*1000;
                NUD_Chance.Value                = 100f;
                CK_NearBoss.IsChecked           = false;
                NUD_NumTargs.Value              = 2;
                CB_LevelOfTargets.SelectedItem  = 87;
                // Phase Info
                NUD_PhaseStartTime.Value = 0;
                NUD_PhaseEndTime.Value = 20 * 60;
                // Targeting Includes
                CK_AffectsMTank.IsChecked          = false;
                CK_AffectsOTank.IsChecked          = true;
                CK_AffectsTTank.IsChecked          = true;
                CK_AffectsMeleeDPS.IsChecked       = true;
                CK_AffectsRangedDPS.IsChecked      = true;
                CK_AffectsMainTankHealer.IsChecked = true;
                CK_AffectsOffTankHealer.IsChecked  = true;
                CK_AffectsRaidHealer.IsChecked     = true;
                //
                isEditing = false;
            }
        }

        private void OKButton_Click(object sender, RoutedEventArgs e) { this.DialogResult = true; }
        private void CancelButton_Click(object sender, RoutedEventArgs e) { this.DialogResult = false; }
    }
}

