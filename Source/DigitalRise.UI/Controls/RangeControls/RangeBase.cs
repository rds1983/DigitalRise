// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System.ComponentModel;
using DigitalRise.GameBase;
using DigitalRise.Mathematics;


namespace DigitalRise.UI.Controls
{
	/// <summary>
	/// Represents an element that has a value within a specific range, such as the 
	/// <see cref="ProgressBar"/>, <see cref="ScrollBar"/>, and <see cref="Slider"/> controls. 
	/// </summary>
	public class RangeBase : UIControl
	{
		// Note:
		// Derived classes should be able to handle cases where Minimum and Maximum are 
		// swapped and negative values.

		//--------------------------------------------------------------
		#region Fields
		//--------------------------------------------------------------
		#endregion


		//--------------------------------------------------------------
		#region Properties & Events
		//--------------------------------------------------------------
		#endregion


		//--------------------------------------------------------------
		#region Game Object Properties & Events
		//--------------------------------------------------------------

		/// <summary> 
		/// The game object property for <see cref="Minimum"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> MinimumProperty = CreateProperty(
			typeof(RangeBase), "Minimum", GamePropertyCategories.Common, null, 0.0f,
			UIPropertyOptions.AffectsArrange);

		/// <summary>
		/// Gets or sets the minimum possible <see cref="Value"/> of the range element. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The minimum possible <see cref="Value"/> of the range element. The default is 0.
		/// </value>
		public float Minimum
		{
			get => MinimumProperty.GetValue(this);
			set => MinimumProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The game object property for <see cref="Maximum"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> MaximumProperty = CreateProperty(
			typeof(RangeBase), "Maximum", GamePropertyCategories.Common, null, float.MaxValue,
			UIPropertyOptions.AffectsArrange);

		/// <summary>
		/// Gets or sets the maximum possible <see cref="Value"/> of the range element. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The maximum possible <see cref="Value"/> of the range element. The default is 100.
		/// </value>
		public float Maximum
		{
			get => MaximumProperty.GetValue(this);
			set => MaximumProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The game object property for <see cref="Value"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> ValueProperty = CreateProperty(
			typeof(RangeBase), "Value", GamePropertyCategories.Common, null, 0.0f,
			UIPropertyOptions.AffectsArrange);

		/// <summary>
		/// Gets or sets the current value of the range element. 
		/// This is a game object property.
		/// </summary>
		/// <value>
		/// The current value of the range element. The default is 0.
		/// </value>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1721:PropertyNamesShouldNotMatchGetMethods")]
		public float Value
		{
			get => ValueProperty.GetValue(this);
			set => ValueProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The game object property for <see cref="SmallChange"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> SmallChangeProperty = CreateProperty(
			typeof(RangeBase), "SmallChange", GamePropertyCategories.Common, null, 1.0f,
			UIPropertyOptions.None);

		/// <summary>
		/// Gets or sets a value to be added to or subtracted from the <see cref="Value"/> of a 
		/// <see cref="RangeBase"/> control. This is a game object property.
		/// </summary>
		/// <value>
		/// A value to be added to or subtracted from the <see cref="Value"/> of a 
		/// <see cref="RangeBase"/> control. The default is 1.
		/// </value>
		/// <remarks>
		/// Classes derived from <see cref="RangeBase"/> determine how this property is used.
		/// </remarks>
		public float SmallChange
		{
			get => SmallChangeProperty.GetValue(this);
			set => SmallChangeProperty.SetValue(this, value);
		}


		/// <summary> 
		/// The game object property for <see cref="LargeChange"/>
		/// </summary>
		[Browsable(false)]
		public static readonly GamePropertyInfo<float> LargeChangeProperty = CreateProperty(
			typeof(RangeBase), "LargeChange", GamePropertyCategories.Common, null, 10.0f,
			UIPropertyOptions.None);

		/// <summary>
		/// Gets or sets a value to be added to or subtracted from the <see cref="Value"/> of a 
		/// <see cref="RangeBase"/> control. This is a game object property.
		/// </summary>
		/// <value>
		/// A value to be added to or subtracted from the <see cref="Value"/> of a 
		/// <see cref="RangeBase"/> control. The default is 10.
		/// </value>
		/// <remarks>
		/// Classes derived from <see cref="RangeBase"/> determine how this property is used.
		/// </remarks>
		public float LargeChange
		{
			get => LargeChangeProperty.GetValue(this);
			set => LargeChangeProperty.SetValue(this, value);
		}
		#endregion


		//--------------------------------------------------------------
		#region Creation & Cleanup
		//--------------------------------------------------------------

		/// <summary>
		/// Initializes static members of the <see cref="RangeBase"/> class.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline")]
		static RangeBase()
		{
			OverrideDefaultValue(typeof(RangeBase), MaximumProperty.Id, 100.0f);
			OverrideDefaultValue(typeof(RangeBase), SmallChangeProperty.Id, 1.0f);
			OverrideDefaultValue(typeof(RangeBase), LargeChangeProperty.Id, 10.0f);
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="RangeBase"/> class.
		/// </summary>
		public RangeBase()
		{
			Style = "RangeBase";

			// When value changes: Coerce it to allowed range.
			var value = ValueProperty.Get(this);
			value.Changing += (s, e) => e.CoercedValue = MathHelper.Clamp(e.CoercedValue, Minimum, Maximum);
		}
		#endregion


		//--------------------------------------------------------------
		#region Methods
		//--------------------------------------------------------------
		#endregion
	}
}
