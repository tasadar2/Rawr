﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Imaging;

namespace Rawr.UI
{
    public partial class TalentItem : UserControl
    {

        public TalentTree TalentTree { get { return (TalentTree)(((Grid)Parent).Parent); } }

        private TalentDataAttribute talentData;
        public TalentDataAttribute TalentData
        {
            get { return talentData; }
            set
            {
                talentData = value;
                current =  talentData == null ? 0 : (int)Math.Min(TalentTree.Talents.Data[talentData.Index] ? 1 : 0, 1);
            }
        }

        public int current;
        public int Current
        {
            get { return current; }
            set
            {
                if (talentData != null && value >= 0 && value <= 1 && CanPutPoints())
                {
                    current = value;
                    TalentTree.Talents.Data[TalentData.Index] = current == 1;
                    TalentTree.RankChanged();
                }
            }
        }

        public bool IsMaxRank() { return current == 1; }

        public bool CanPutPoints()
        {
            return TalentTree.PointsBelowRow(talentData.Row) >= (talentData.Row - 1);
        }

        public void Update()
        {
#if SILVERLIGHT
            string iconPrefix = "..";
#else
            string iconPrefix = "/Rawr.UI.WPF;component";
#endif
            if (talentData != null)
            {
                Brush b;
                if (Current == 1)
                {
                    b = new SolidColorBrush(Colors.Yellow);
                    OverlayImage.Source = Icons.NewBitmapImage(new Uri(iconPrefix + "/Images/icon-over-yellow.png", UriKind.Relative));
                }
                else
                {
                    if (CanPutPoints()) b = new SolidColorBrush(Color.FromArgb(255, 0, 255, 0));
                    else b = new SolidColorBrush(Colors.White);

                    if (Current > 0) OverlayImage.Source = Icons.NewBitmapImage(new Uri(iconPrefix + "/Images/icon-over-green.png", UriKind.Relative));
                    else OverlayImage.Source = Icons.NewBitmapImage(new Uri(iconPrefix + "/Images/icon-over-grey.png", UriKind.Relative));
                }

                RankLabel.Text = string.Format("{0}/{1}", Current, 1);
                RankLabel.Foreground = b;

                //TalentImage.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(TalentImage_ImageFailed);
                //TalentImage.Source = Icons.TalentIcon(TalentTree.Class, TalentTree.TreeName, talentData.Name, Current > 0);
                TalentImage.Source = Icons.AnIcon(talentData.Icon);

                ToolTipService.SetToolTip(this, GetTooltipString());
                this.Visibility = Visibility.Visible;
            }
            else
            {
                this.Visibility = Visibility.Collapsed;
            }
        }

        /*public void TalentImage_ImageFailed(object o, ExceptionRoutedEventArgs e) {
            if (talentData == null) { return; }
            TalentImage.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(TalentImage_ImageFailed);
#if DEBUG
            //TalentImage_ImageFailed2(o, e); // Tell me what happened
            //TalentImage.ImageFailed += new EventHandler<ExceptionRoutedEventArgs>(TalentImage_ImageFailed2);
#endif
            // Getting the Image from the Armory failed, lets try another source
            TalentImage.Source = Icons.AnIcon(talentData.Icon);
        }

        public void TalentImage_ImageFailed2(object o, ExceptionRoutedEventArgs e)
        {
            if (talentData == null) { return; }
            TalentImage.ImageFailed -= new EventHandler<ExceptionRoutedEventArgs>(TalentImage_ImageFailed2);
            // Getting the Image from the Armory & Wowhead failed, tell me why
            string infoString = string.Format("Talent Name: {0}\r\nClass: {1}\r\nTree Name: {2}\r\nTalent Icon: {3}\r\nSource String: {4}",
                talentData.Name, TalentTree.Class, TalentTree.TreeName, talentData.Icon, (TalentImage.Source as BitmapImage).UriSource);
            Base.ErrorBox eb = new Base.ErrorBox("Error getting the talent image", e.ErrorException, "Talent Image Update()", infoString);
            eb.Show();
        }*/

        private string wrapText(string toWrap)
        {
            int wrapWidth = 63;
            if (toWrap.Length <= wrapWidth) { return toWrap; } // Don't bother wrapping

            string retVal = toWrap;
            bool eos = false;
            bool foundspace = false;
            int i = wrapWidth;

            while (!eos)
            {
                while (!foundspace && i >= 0)
                {
                    if (retVal[i] == ' ') { foundspace = true; break; }
                    i--; // didn't find a space so backtrack a char
                }
                if (foundspace)
                {
                    retVal = retVal.Insert(i + 1, "\r\n"); // +1 because we want it after the space
                    i++; foundspace = false;
                }
                // Continue to next part of string unless we're at or close to the end
                if (i + wrapWidth >= retVal.Length - 1) { eos = true; } else { i += wrapWidth; }
            }

            return retVal;
        }

        public string GetTooltipString()
        {
            string n = talentData.Name + "\r\n";
            return string.Format(n + "\n{0}", wrapText(talentData.Description));
        }

        public TalentItem()
        {
            // Required to initialize variables
            InitializeComponent();
            //
            // None of these work? wtf?
            //MouseRightButtonUp += new MouseButtonEventHandler(TalentUnClicked);
            //TalentImage.MouseRightButtonUp += new MouseButtonEventHandler(TalentUnClicked);
            //OverlayImage.MouseRightButtonUp += new MouseButtonEventHandler(TalentUnClicked);
        }

        void Image_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            //Handling this event makes it not throw the exception up to the user. This occurs when a talent image cannot be found, which is fine to ignore.
        }

        private void TalentClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) Current--;
            else Current++;
        }
        public void TalentUnClicked(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) Current++;
            else Current--;
        }
    }
}
