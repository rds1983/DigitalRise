using System;
using DigitalRise.Animation;
using DigitalRise.Animation.Traits;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace Samples.Animation
{
  // A custom animation derived from Animation<T> base class. This animation animates
  // a Vector2 value and creates a circle movement.
  public class MyCircleAnimation : Animation<Vector2>
  {
    // Traits tell the animation system how to create/recycle/add/blend/etc. the animated
    // value type. Trait classes are usually singletons.
    public override IAnimationValueTraits<Vector2> Traits
    {
      get { return Vector2Traits.Instance; }
    }


    // This animation goes on forever.
    public override TimeSpan GetTotalDuration()
    {
      return TimeSpan.MaxValue;
    }


    // Compute the animation value for the given time and stores it in result. 
    // This animation does not need defaultSource and defaultTarget parameters.
    protected override void GetValueCore(TimeSpan time, ref Vector2 defaultSource, ref Vector2 defaultTarget, ref Vector2 result)
    {
      const float circlePeriod = 4.0f;
      float angle = (float)time.TotalSeconds / circlePeriod * ConstantsF.TwoPi;

      Matrix22F rotation = Matrix22F.CreateRotation(angle);
      Vector2 offset = new Vector2(200, 0);
      Vector2 rotatedOffset = rotation * offset;

      result.X = 500 + rotatedOffset.X;
      result.Y = 350 + rotatedOffset.Y;
    }
  }
}
