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

namespace Rawr.Bear
{
	/// <summary>
	/// Data container class for the results of a comparison of two CharacterCalculationsBear objects
	/// </summary>
	public class ComparisonCalculationBear : ComparisonCalculationBase
	{
		private string _name = string.Empty;
		public override string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		private string _desc = string.Empty;
		public override string Description
		{
			get { return _desc; }
			set { _desc = value; }
		}

		private float _overallPoints = 0f;
		public override float OverallPoints
		{
			get { return _overallPoints; }
			set { _overallPoints = value; }
		}

		private float[] _subPoints = new float[] { 0f, 0f, 0f, 0f };
		public override float[] SubPoints
		{
			get { return _subPoints; }
			set { _subPoints = value; }
		}

		public float MitigationPoints
		{
			get { return _subPoints[0]; }
			set { _subPoints[0] = value; }
		}

		public float SurvivalPoints
		{
			get { return _subPoints[1]; }
			set { _subPoints[1] = value; }
		}
        public float RecoveryPoints
        {
            get { return _subPoints[2]; }
            set { _subPoints[2] = value; }
        }
        public float ThreatPoints
		{
			get { return _subPoints[3]; }
			set { _subPoints[3] = value; }
		}

		private Item _item = null;
		public override Item Item
		{
			get { return _item; }
			set { _item = value; }
		}

		private ItemInstance _itemInstance = null;
		public override ItemInstance ItemInstance
		{
			get { return _itemInstance; }
			set { _itemInstance = value; }
		}

		private bool _equipped = false;
		public override bool Equipped
		{
			get { return _equipped; }
			set { _equipped = value; }
		}
		public override bool PartEquipped { get; set; }

		public override string ToString()
		{
			return string.Format("{0}: ({1}O {2}M {3}S {4}R {5}T)", Name, Math.Round(OverallPoints), Math.Round(MitigationPoints), Math.Round(SurvivalPoints), Math.Round(RecoveryPoints), Math.Round(ThreatPoints));
		}
	}
}
