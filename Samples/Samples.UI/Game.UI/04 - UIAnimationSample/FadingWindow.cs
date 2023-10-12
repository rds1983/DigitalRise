using System;
using DigitalRise.Animation;
using DigitalRise.UI.Controls;
using Microsoft.Xna.Framework;

namespace Samples.UI
{
  // A window that animates the Opacity when the windows is loaded/closed.
  public class FadingWindow : AnimatedWindow
  {
    public FadingWindow(IServiceProvider services) 
      : base(services)
    {
      Title = "FadingWindow";

      Width = 200;
      Height = 100;

      Content = new TextBlock
      {
        Margin = new Vector4(8),
        Text = "The 'Opacity' of this window is animated.",
        WrapText = true,
      };

      LoadingAnimation = new SingleFromToByAnimation
      {
        TargetProperty = "Opacity",           // Transition the property UIControl.Opacity 
        From = 0,                             // from 0 to its actual value
        Duration = TimeSpan.FromSeconds(0.3), // over a duration of 0.3 seconds.
      };

      ClosingAnimation = new SingleFromToByAnimation
      {
        TargetProperty = "Opacity",           // Transition the property UIControl.Opacity
        To = 0,                               // from its current value to 0
        Duration = TimeSpan.FromSeconds(0.3), // over a duration 0.3 seconds.
      };
    }
  }
}
