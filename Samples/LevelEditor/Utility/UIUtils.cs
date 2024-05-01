using Myra.Graphics2D.UI;
using System;
using System.Collections.Generic;

namespace DigitalRise.LevelEditor.Utility
{
	internal static class UIUtils
	{
		public static void BuildContextMenu(this Desktop desktop, IEnumerable<Tuple<string, Action>> items)
		{
			if (desktop.ContextMenu != null || desktop.TouchPosition == null)
			{
				// Dont show if it's already shown
				return;
			}

			var verticalMenu = new VerticalMenu();

			foreach (var item in items)
			{
				var menuItem = new MenuItem
				{
					Text = item.Item1
				};
				menuItem.Selected += (s, a) => item.Item2();
				verticalMenu.Items.Add(menuItem);
			}

			desktop.ShowContextMenu(verticalMenu, desktop.TouchPosition.Value);
		}
	}
}
