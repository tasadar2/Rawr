﻿using System;
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
    public partial class GemmingTemplateItem : UserControl
    {
        public bool isCustom;

        public GemmingTemplate template;
        private GemmingTemplates parent;

        public GemmingTemplateItem(GemmingTemplates parent, GemmingTemplate template)
        {
            this.parent = parent;
            this.template = template;
            DataContext = template;
            isCustom = template.Group == "Custom";
            InitializeComponent();

            template.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(template_PropertyChanged);

            if (isCustom) DeleteButton.Visibility = Visibility.Visible;
            else DeleteButton.Visibility = Visibility.Collapsed;
        }

        private void template_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Enabled")
            {
                parent.UpdateGroupChecked(template.Group);
            }
        }

        private void MouseEnterButton(object sender, MouseEventArgs e)
        {
            Button itemButton = sender as Button;
            if (itemButton != null)
            {
                MainPage.Tooltip.Item = itemButton.Tag as Item;
                MainPage.Tooltip.Show(itemButton, itemButton.ActualWidth + 4, 0);
            }
        }

        private void MouseLeaveButton(object sender, MouseEventArgs e)
        {
            Button itemButton = sender as Button;
            if (itemButton != null)
            {
                MainPage.Tooltip.Hide();
            }
        }

        private void RedButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.GemButtonClick(template.RedGem, sender as Button, RedGemChanged);
        }
        private void RedGemChanged(Item newGem)
        {
            if (isCustom) template.RedGem = newGem;
        }

        private void YellowButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.GemButtonClick(template.YellowGem, sender as Button, YellowGemChanged);
        }
        private void YellowGemChanged(Item newGem)
        {
            if (isCustom) template.YellowGem = newGem;
        }

        private void BlueButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.GemButtonClick(template.BlueGem, sender as Button, BlueGemChanged);
        }
        private void BlueGemChanged(Item newGem)
        {
            if (isCustom) template.BlueGem = newGem;
        }

        private void PrismaticButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.GemButtonClick(template.PrismaticGem, sender as Button, PrismaticGemChanged);
        }
        private void PrismaticGemChanged(Item newGem)
        {
            if (isCustom) template.PrismaticGem = newGem;
        }

        private void MetaButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.MetaButtonClick(template.MetaGem, sender as Button, MetaGemChanged);
        }
        private void MetaGemChanged(Item newGem)
        {
            if (isCustom) template.MetaGem = newGem;
        }

        private void CogwheelButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.CogwheelButtonClick(template.Cogwheel, sender as Button, CogwheelChanged);
        }
        private void CogwheelChanged(Item newCog)
        {
            if (isCustom) template.Cogwheel = newCog;
        }

        private void Cogwheel2ButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.CogwheelButtonClick(template.Cogwheel2, sender as Button, Cogwheel2Changed);
        }
        private void Cogwheel2Changed(Item newCog)
        {
            if (isCustom) template.Cogwheel2 = newCog;
        }

        private void HydraulicButtonClick(object sender, RoutedEventArgs e)
        {
            if (isCustom) parent.HydraulicButtonClick(template.Hydraulic, sender as Button, HydraulicChanged);
        }
        private void HydraulicChanged(Item newHyd)
        {
            if (isCustom) template.Hydraulic = newHyd;
        }

        private void DeleteTemplate(object sender, RoutedEventArgs e)
        {
            parent.RemoveTemplate(this);
        }

    }
}