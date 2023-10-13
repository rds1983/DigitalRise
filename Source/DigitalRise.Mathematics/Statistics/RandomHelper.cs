// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using System;
using System.Diagnostics.CodeAnalysis;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;

namespace DigitalRise.Mathematics.Statistics
{
  /// <summary>
  /// A class to generate random values.
  /// </summary>
  public static class RandomHelper
  {
    /// <summary>
    /// Gets or sets the default random number generator.
    /// </summary>
    /// <value>
    /// The default random number generator. 
    /// </value>
    /// <remarks>
    /// This is a global random number generator. Per default, this property is initialized with a 
    /// new instance of <see cref="System.Random"/> with a time-dependent default seed.
    /// </remarks>
    /// <exception cref="ArgumentNullException">
    /// The property is set <see langword="null"/>.
    /// </exception>
    public static Random Random
    {
      get { return _random; }
      set 
      { 
        if (value == null)
          throw new ArgumentNullException("value");

        _random = value; 
      }
    }
    private static Random _random = new Random();


    /// <summary>
    /// Gets a random boolean value.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <returns>A random boolean value.</returns>
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
    public static bool NextBool(this Random random)
    {
      if (random == null)
        random = Random;

      return random.Next(2) == 0;
    }


    /// <summary>
    /// Gets a random byte value.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <returns>A random byte value.</returns>
    public static int NextByte(this Random random)
    {
      if (random == null)
        random = Random;

      return random.Next(255);
    }


    /// <summary>
    /// Gets a random unit <see cref="Quaternion"/>.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <returns>A random unit <see cref="Quaternion"/>.</returns>
    public static Quaternion NextQuaternion(this Random random)
    {
      if (random == null)
        random = Random;

      return MathHelper.CreateRotation(NextVector3(random, -1, 1), NextFloat(random, 0, ConstantsF.TwoPi));
    }


    /// <summary>
    /// Gets a random <see langword="float"/> value that lies in the interval 
    /// [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value.</param>
    /// <param name="max">The maximal allowed value.</param>
    /// <returns>A random <see langword="float"/> value within the bounds [min, max].</returns>
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
    public static float NextFloat(this Random random, float min, float max)
    {
      if (random == null)
        random = Random;

      return (float)random.NextDouble() * (max - min) + min;
    }


    /// <summary>
    /// Gets a random <see langword="double"/> value that lies in the interval 
    /// [<paramref name="min"/>, <paramref name="max"/>].
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global
    /// random number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value.</param>
    /// <param name="max">The maximal allowed value.</param>
    /// <returns>A random <see langword="double"/> value within the bounds [min, max].</returns>
    public static double NextDouble(this Random random, double min, double max)
    {
      if (random == null)
        random = Random;

      return random.NextDouble() * (max - min) + min;
    }


    /// <summary>
    /// Gets a random integer value that lies in the interval [<paramref name="min"/>, 
    /// <paramref name="max"/>].
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value.</param>
    /// <param name="max">
    /// The maximal allowed value. (Must be less than <see cref="int.MaxValue"/>.)
    /// </param>
    /// <returns>A random integer value within the bounds [min, max].</returns>
    [SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames")]
    [SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "max+1")]
    public static int NextInteger(this Random random, int min, int max)
    {
      if (random == null)
        random = Random;

      return random.Next(min, max + 1);
    }


    /// <summary>
    /// Gets a random <see cref="Vector2"/>.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value for a vector element.</param>
    /// <param name="max">The maximal allowed value for a vector element.</param>
    /// <returns>A random <see cref="Vector2"/>.</returns>
    public static Vector2 NextVector2(this Random random, float min, float max)
    {
      if (random == null)
        random = Random;

      return new Vector2(NextFloat(random, min, max),
                          NextFloat(random, min, max));
    }


   
    /// <summary>
    /// Gets a random <see cref="Vector3"/>.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value for a vector element.</param>
    /// <param name="max">The maximal allowed value for a vector element.</param>
    /// <returns>A random <see cref="Vector3"/>.</returns>
    public static Vector3 NextVector3(this Random random, float min, float max)
    {
      if (random == null)
        random = Random;

      return new Vector3(NextFloat(random, min, max),
                          NextFloat(random, min, max),
                          NextFloat(random, min, max));
    }


    /// <summary>
    /// Gets a random <see cref="Vector4"/>.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value for a vector element.</param>
    /// <param name="max">The maximal allowed value for a vector element.</param>
    /// <returns>A random <see cref="Vector4"/>.</returns>
    public static Vector4 NextVector4(this Random random, float min, float max)
    {
      if (random == null)
        random = Random;

      return new Vector4(NextFloat(random, min, max),
                          NextFloat(random, min, max),
                          NextFloat(random, min, max),
                          NextFloat(random, min, max));
    }


    /// <summary>
    /// Gets a random <see cref="Matrix22F"/>.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value for a matrix element.</param>
    /// <param name="max">The maximal allowed value for a matrix element.</param>
    /// <returns>A random <see cref="Matrix22F"/>.</returns>
    public static Matrix22F NextMatrix22F(this Random random, float min, float max)
    {
      if (random == null)
        random = Random;

      return new Matrix22F(NextFloat(random, min, max), NextFloat(random, min, max),
                           NextFloat(random, min, max), NextFloat(random, min, max));
    }


    /// <summary>
    /// Gets a random <see cref="Matrix33F"/>.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value for a matrix element.</param>
    /// <param name="max">The maximal allowed value for a matrix element.</param>
    /// <returns>A random <see cref="Matrix33F"/>.</returns>
    public static Matrix33F NextMatrix33F(this Random random, float min, float max)
    {
      if (random == null)
        random = Random;

      return new Matrix33F(NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max),
                           NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max),
                           NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max));
    }


    /// <summary>
    /// Gets a random <see cref="Matrix44F"/>.
    /// </summary>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="min">The minimal allowed value for a matrix element.</param>
    /// <param name="max">The maximal allowed value for a matrix element.</param>
    /// <returns>A random <see cref="Matrix44F"/>.</returns>
    public static Matrix44F NextMatrix44F(this Random random, float min, float max)
    {
      if (random == null)
        random = Random;

      return new Matrix44F(NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max),
                           NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max),
                           NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max),
                           NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max), NextFloat(random, min, max));
    }


    /// <summary>
    /// Gets a new random value for the specified probability distribution.
    /// </summary>
    /// <typeparam name="T">The type of the random value.</typeparam>
    /// <param name="random">
    /// The random number generator. If this parameter is <see langword="null"/>, the global random
    /// number generator (see <see cref="RandomHelper.Random"/>) is used.
    /// </param>
    /// <param name="distribution">The probability distribution.</param>
    /// <returns>A random value.</returns>
    [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", Justification = "Performance")]
    public static T Next<T>(this Random random, Distribution<T> distribution)
    {
      if (random == null)
        random = Random;

      return distribution.Next(random);
    }
  }
}
