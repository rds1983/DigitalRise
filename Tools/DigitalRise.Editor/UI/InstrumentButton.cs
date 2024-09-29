﻿using System;
using System.Collections.Generic;
using Myra.Graphics2D.UI;

namespace DigitalRise.Studio.UI
{
	public class InstrumentButton : ImageTextButton
	{
		private readonly List<InstrumentButton> _allButtons;

		public override bool IsPressed
		{
			get => base.IsPressed;

			set
			{
				if (IsPressed)
				{
					// If this is last pressed button
					// Don't allow it to be unpressed
					var allow = false;
					foreach (var button in _allButtons)
					{
						if (button == this)
						{
							continue;
						}

						if (button.IsPressed)
						{
							allow = true;
							break;
						}
					}

					if (!allow)
					{
						return;
					}
				}

				base.IsPressed = value;
			}
		}

		public InstrumentButton(List<InstrumentButton> allButtons)
		{
			_allButtons = allButtons ?? throw new ArgumentNullException(nameof(allButtons));
			_allButtons.Add(this);
			Toggleable = true;
			LabelHorizontalAlignment = HorizontalAlignment.Center;
			HorizontalAlignment = HorizontalAlignment.Stretch;
			VerticalAlignment = VerticalAlignment.Stretch;
			ContentHorizontalAlignment = HorizontalAlignment.Center;
			ContentVerticalAlignment = VerticalAlignment.Center;
			TextPosition = TextPositionEnum.Top;
		}

		public override void OnPressedChanged()
		{
			base.OnPressedChanged();

			if (!IsPressed)
			{
				return;
			}

			// Release other pressed radio buttons
			foreach (var button in _allButtons)
			{
				if (button == this)
				{
					continue;
				}

				button.IsPressed = false;
			}
		}
	}
}
