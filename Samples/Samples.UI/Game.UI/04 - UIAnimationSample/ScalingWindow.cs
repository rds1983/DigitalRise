using System;
using DigitalRise.Animation;
using DigitalRise.Animation.Easing;
using DigitalRise.UI.Controls;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace Samples.UI
{
  // A window that animates the RenderScale when the windows is loaded/closed.
  public class ScalingWindow : AnimatedWindow
  {
    public ScalingWindow(IServiceProvider services) 
      : base(services)
    {
      Title = "ScalingWindow";

      Width = 200;
      Height = 100;

      Content = new TextBlock
      {
        Margin = new Vector4(8),
        Text = "The 'RenderScale' of this window is animated.",
        WrapText = true,
      };

      // Set the center of the scale transformation to the center of the window. 
      RenderTransformOrigin = new Vector2(0.5f, 0.5f);

      LoadingAnimation = new Vector2FromToByAnimation
      {
        TargetProperty = "RenderScale",       // Animate the property UIControl.RenderScale 
        From = new Vector2(0, 0),            // from (0, 0) to its actual value (1, 1)
        Duration = TimeSpan.FromSeconds(0.3), // over 0.3 seconds.
        EasingFunction = new ElasticEase { Mode = EasingMode.EaseOut, Oscillations = 1 },
      };

      ClosingAnimation = new Vector2FromToByAnimation
      {
        TargetProperty = "RenderScale",       // Animate the property UIControl.RenderScale
        To = new Vector2(0, 0),              // from its current value to (0, 0)
        Duration = TimeSpan.FromSeconds(0.3), // over 0.3 seconds.
        EasingFunction = new HermiteEase { Mode = EasingMode.EaseIn },
      };
    }
  }
}
