// DigitalRune Engine - Copyright (C) DigitalRune GmbH
// This file is subject to the terms and conditions defined in
// file 'LICENSE.TXT', which is part of this source code package.

using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics.CodeAnalysis;

namespace DigitalRise.Mathematics
{
	/// <summary>
	/// Provides useful mathematical algorithms and functions.
	/// </summary>
	public static class MathHelper
	{
		/// <summary>
		/// Clamps the specified value.
		/// </summary>
		/// <typeparam name="T">The type of the value.</typeparam>
		/// <param name="value">The value which should be clamped.</param>
		/// <param name="min">The min limit.</param>
		/// <param name="max">The max limit.</param>
		/// <returns>
		/// <paramref name="value"/> clamped to the interval
		/// [<paramref name="min"/>, <paramref name="max"/>].
		/// </returns>
		/// <remarks>
		/// Values within the limits are not changed. Values exceeding the limits are cut off.
		/// </remarks>
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			if (min.CompareTo(max) > 0)
			{
				// min and max are swapped.
				var dummy = max;
				max = min;
				min = dummy;
			}

			if (value.CompareTo(min) < 0)
				value = min;
			else if (value.CompareTo(max) > 0)
				value = max;

			return value;
		}


		/// <overloads>
		/// <summary>
		/// Computes Sqrt(a*a + b*b) without underflow/overflow.
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Computes Sqrt(a*a + b*b) without underflow/overflow (single-precision).
		/// </summary>
		/// <param name="cathetusA">Cathetus a.</param>
		/// <param name="cathetusB">Cathetus b.</param>
		/// <returns>The hypotenuse c, which is Sqrt(a*a + b*b).</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public static float Hypotenuse(float cathetusA, float cathetusB)
		{
			float h = 0;
			if (Math.Abs(cathetusA) > Math.Abs(cathetusB))
			{
				h = cathetusB / cathetusA;
				h = (float)(Math.Abs(cathetusA) * Math.Sqrt(1 + h * h));
			}
			else if (cathetusB != 0)
			{
				h = cathetusA / cathetusB;
				h = (float)(Math.Abs(cathetusB) * Math.Sqrt(1 + h * h));
			}

			return h;
		}


		/// <summary>
		/// Computes Sqrt(a*a + b*b) without underflow/overflow (double-precision).
		/// </summary>
		/// <param name="cathetusA">Cathetus a.</param>
		/// <param name="cathetusB">Cathetus b.</param>
		/// <returns>The hypotenuse c, which is Sqrt(a*a + b*b).</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public static double Hypotenuse(double cathetusA, double cathetusB)
		{
			double h = 0;
			if (Math.Abs(cathetusA) > Math.Abs(cathetusB))
			{
				h = cathetusB / cathetusA;
				h = Math.Abs(cathetusA) * Math.Sqrt(1 + h * h);
			}
			else if (cathetusB != 0)
			{
				h = cathetusA / cathetusB;
				h = Math.Abs(cathetusB) * Math.Sqrt(1 + h * h);
			}

			return h;
		}


		/// <summary>
		/// Swaps the content of two variables.
		/// </summary>
		/// <typeparam name="T">The type of the objects.</typeparam>
		/// <param name="obj1">First variable.</param>
		/// <param name="obj2">Second variable.</param>
		[SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference")]
		public static void Swap<T>(ref T obj1, ref T obj2)
		{
			T temp = obj1;
			obj1 = obj2;
			obj2 = temp;
		}


		/// <overloads>
		/// <summary>
		/// Converts an angle value from degrees to radians.
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Converts an angle value from degrees to radians (single-precision).
		/// </summary>
		/// <param name="degree">The angle in degrees.</param>
		/// <returns>The angle in radians.</returns>
		public static float ToRadians(float degree)
		{
			return degree * ConstantsF.Pi / 180;
		}


		/// <overloads>
		/// <summary>
		/// Converts an angle value from radians to degrees.
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Converts an angle value from radians to degrees (single-precision).
		/// </summary>
		/// <param name="radians">The angle in radians.</param>
		/// <returns>The angle in degrees.</returns>
		public static float ToDegrees(float radians)
		{
			return radians * 180 * ConstantsF.OneOverPi;
		}


		/// <summary>
		/// Returns the largest non-negative integer x such that 2<sup>x</sup> ≤ <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The largest non-negative integer x such that 2<sup>x</sup> ≤ <paramref name="value"/>.
		/// Exception: If <paramref name="value"/> is 0 then 0 is returned.
		/// </returns>
		[SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly")]
		public static uint Log2LessOrEqual(uint value)
		{
			// See Game Programming Gems 3, "Fast Base-2 Functions for Logarithms and Random Number Generation.

			uint testValue; // The value against which we test in the if condition.
			uint x;         // The value we are looking for.

			if (value >= 0x10000)
			{
				x = 16;
				testValue = 0x1000000;
			}
			else
			{
				x = 0;
				testValue = 0x100;
			}

			if (value >= testValue)
			{
				x += 8;
				testValue <<= 4;
			}
			else
			{
				testValue >>= 4;
			}

			if (value >= testValue)
			{
				x += 4;
				testValue <<= 2;
			}
			else
			{
				testValue >>= 2;
			}

			if (value >= testValue)
			{
				x += 2;
				testValue <<= 1;
			}
			else
			{
				testValue >>= 1;
			}

			if (value >= testValue)
			{
				x += 1;
			}

			return x;
		}


		/// <summary>
		/// Returns the smallest non-negative integer x such that 2<sup>x</sup> ≥ <paramref name="value"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The smallest non-negative integer x such that 2<sup>x</sup> ≥ <paramref name="value"/>.
		/// Exception: If <paramref name="value"/> is 0, 0 is returned.
		/// </returns>
		public static uint Log2GreaterOrEqual(uint value)
		{
			// See Game Programming Gems 3, "Fast Base-2 Functions for Logarithms and Random Number Generation.
			if (value > 0x80000000)
				return 32;

			uint testValue; // The value against which we test in the if condition.
			uint x;         // The value we are looking for.

			if (value > 0x8000)
			{
				x = 16;
				testValue = 0x800000;
			}
			else
			{
				x = 0;
				testValue = 0x80;
			}

			if (value > testValue)
			{
				x += 8;
				testValue <<= 4;
			}
			else
			{
				testValue >>= 4;
			}

			if (value > testValue)
			{
				x += 4;
				testValue <<= 2;
			}
			else
			{
				testValue >>= 2;
			}

			if (value > testValue)
			{
				x += 2;
				testValue <<= 1;
			}
			else
			{
				testValue >>= 1;
			}

			if (value > testValue)
			{
				x += 1;
			}

			return x;
		}


		/// <summary>
		/// Creates the smallest bitmask that is greater than or equal to the given value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// A bitmask where the left bits are 0 and the right bits are 1. The value of the bitmask
		/// is ≥ <paramref name="value"/>.
		/// </returns>
		/// <remarks>
		/// <para>
		/// This result can also be interpreted as finding the smallest x such that 2<sup>x</sup> &gt; 
		/// <paramref name="value"/> and returning 2<sup>x</sup> - 1.
		/// </para>
		/// <para>
		/// Another useful application: Bitmask(x) + 1 returns the next power of 2 that is greater than 
		/// x.
		/// </para>
		/// </remarks>
		public static uint Bitmask(uint value)
		{
			// Example:                 value = 10000000 00000000 00000000 00000000
			value |= (value >> 1);   // value = 11000000 00000000 00000000 00000000
			value |= (value >> 2);   // value = 11110000 00000000 00000000 00000000
			value |= (value >> 4);   // value = 11111111 00000000 00000000 00000000
			value |= (value >> 8);   // value = 11111111 11111111 00000000 00000000
			value |= (value >> 16);  // value = 11111111 11111111 11111111 11111111
			return value;
		}


		/// <summary>
		/// Determines whether the specified value is a power of two.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// <see langword="true"/> if <paramref name="value"/> is a power of two; otherwise, 
		/// <see langword="false"/>.
		/// </returns>
		[SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", MessageId = "value-1")]
		public static bool IsPowerOf2(int value)
		{
			// See http://stackoverflow.com/questions/600293/how-to-check-if-a-number-is-a-power-of-2
			return (value != 0) && (value & (value - 1)) == 0;
		}


		/// <summary>
		/// Returns the smallest power of two that is greater than the given value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>
		/// The smallest power of two (2<sup>x</sup>) that is greater than <paramref name="value"/>.
		/// </returns>
		/// <remarks>
		/// For example, <c>NextPowerOf2(7)</c> is <c>8</c> and <c>NextPowerOf2(8)</c> is <c>16</c>.
		/// </remarks>
		public static uint NextPowerOf2(uint value)
		{
			return Bitmask(value) + 1;
		}


		/// <overloads>
		/// <summary>
		/// Computes the Gaussian function y = k * e^( -(x-μ)<sup>2</sup>/(2σ<sup>2</sup>).
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Computes the Gaussian function y = k * e^( -(x-μ)<sup>2</sup>/(2σ<sup>2</sup>) 
		/// (single precision).
		/// </summary>
		/// <param name="x">The argument x.</param>
		/// <param name="coefficient">The coefficient k.</param>
		/// <param name="expectedValue">The expected value μ.</param>
		/// <param name="standardDeviation">The standard deviation σ.</param>
		/// <returns>The height of the Gaussian bell curve at x.</returns>
		/// <remarks>
		/// This method computes the Gaussian bell curve.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public static float Gaussian(float x, float coefficient, float expectedValue, float standardDeviation)
		{
			float xMinusExpected = x - expectedValue;
			return coefficient * (float)Math.Exp(-xMinusExpected * xMinusExpected
																					 / (2 * standardDeviation * standardDeviation));
		}


		/// <summary>
		/// <summary>
		/// Computes the Gaussian function y = k * e^( -(x-μ)<sup>2</sup>/(2σ<sup>2</sup>) 
		/// (double-precision).
		/// </summary>
		/// </summary>
		/// <param name="x">The argument x.</param>
		/// <param name="coefficient">The coefficient k.</param>
		/// <param name="expectedValue">The expected value μ.</param>
		/// <param name="standardDeviation">The standard deviation σ.</param>
		/// <returns>The height of the Gaussian bell curve at x.</returns>
		/// <remarks>
		/// This method computes the Gaussian bell curve.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public static double Gaussian(double x, double coefficient, double expectedValue, double standardDeviation)
		{
			double xMinusExpected = x - expectedValue;
			return coefficient * Math.Exp(-xMinusExpected * xMinusExpected
																		/ (2 * standardDeviation * standardDeviation));
		}


		/// <summary>
		/// Computes the binomial coefficient of (n, k), also read as "n choose k".
		/// </summary>
		/// <param name="n">n, must be a value equal to or greater than 0.</param>
		/// <param name="k">k, a value in the range [0, <paramref name="n"/>].</param>
		/// <returns>
		/// The binomial coefficient.
		/// </returns>
		/// <remarks>
		/// This method returns a binomial coefficient. The result is the k'th element in the n'th row
		/// of Pascal's triangle (using zero-based indices for k and n). This method returns 0 for
		/// negative <paramref name="n"/>.
		/// </remarks>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public static long BinomialCoefficient(int n, int k)
		{
			// See http://blog.plover.com/math/choose.html.

			if (k < 0 || k > n)
				return 0;

			long r = 1;
			long d;
			for (d = 1; d <= k; d++)
			{
				r *= n--;
				r /= d;
			}
			return r;
		}


		/// <overloads>
		/// <summary>
		/// Calculates the fractional part of a specified floating-point number.
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Calculates the fractional part of a specified single-precision floating-point number.
		/// </summary>
		/// <param name="f">The number.</param>
		/// <returns>The fractional part of <paramref name="f"/>.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public static float Frac(float f)
		{
			return f - (float)Math.Floor(f);
		}


		/// <summary>
		/// Calculates the fractional part of a specified double-precision floating-point number.
		/// </summary>
		/// <param name="d">The number.</param>
		/// <returns>The fractional part of <paramref name="d"/>.</returns>
		[SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly")]
		public static double Frac(double d)
		{
			return d - Math.Floor(d);
		}

		/// <overloads>
		/// <summary>
		/// Determines whether two vectors are equal (regarding a given tolerance).
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Determines whether two vectors are equal (regarding the tolerance 
		/// <see cref="Numeric.EpsilonF"/>).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector.</param>
		/// <returns>
		/// <see langword="true"/> if the vectors are equal (within the tolerance 
		/// <see cref="Numeric.EpsilonF"/>); otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// The two vectors are compared component-wise. If the differences of the components are less
		/// than <see cref="Numeric.EpsilonF"/> the vectors are considered as being equal.
		/// </remarks>
		public static bool AreNumericallyEqual(Vector2 vector1, Vector2 vector2)
		{
			return Numeric.AreEqual(vector1.X, vector2.X)
					&& Numeric.AreEqual(vector1.Y, vector2.Y);
		}

		/// <summary>
		/// Determines whether two vectors are equal (regarding a specific tolerance).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector.</param>
		/// <param name="epsilon">The tolerance value.</param>
		/// <returns>
		/// <see langword="true"/> if the vectors are equal (within the tolerance 
		/// <paramref name="epsilon"/>); otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// The two vectors are compared component-wise. If the differences of the components are less
		/// than <paramref name="epsilon"/> the vectors are considered as being equal.
		/// </remarks>
		public static bool AreNumericallyEqual(Vector2 vector1, Vector2 vector2, float epsilon)
		{
			return Numeric.AreEqual(vector1.X, vector2.X, epsilon)
					&& Numeric.AreEqual(vector1.Y, vector2.Y, epsilon);
		}


		public static float GetComponentByIndex(this Vector2 a, int index)
		{
			switch (index)
			{
				case 0: return a.X;
				case 1: return a.Y;
			}

			throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0 and 1.");
		}

		public static void SetComponentByIndex(this ref Vector2 a, int index, float value)
		{
			switch (index)
			{
				case 0: a.X = value; break;
				case 1: a.Y = value; break;
				default: throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0 and 1.");
			}
		}


		public static bool IsLessThen(this Vector2 a, Vector2 b) => a.X < b.X && a.Y < b.Y;
		public static bool IsLessOrEqual(this Vector2 a, Vector2 b) => a.X <= b.X && a.Y <= b.Y;
		public static bool IsGreaterThen(this Vector2 a, Vector2 b) => a.X > b.X && a.Y > b.Y;
		public static bool IsGreaterOrEqual(this Vector2 a, Vector2 b) => a.X >= b.X && a.Y >= b.Y;


		/// <summary>
		/// Gets a value indicating whether a component of the vector is <see cref="float.NaN"/>.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if a component of the vector is <see cref="float.NaN"/>; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		public static bool IsNaN(this Vector2 a) => Numeric.IsNaN(a.X) || Numeric.IsNaN(a.Y);


		/// <summary>
		/// Tries to normalize the vector.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the vector was normalized; otherwise, <see langword="false"/> if 
		/// the vector could not be normalized. (The length is numerically zero.)
		/// </returns>
		public static bool TryNormalize(this ref Vector2 a)
		{
			float lengthSquared = a.LengthSquared();
			if (Numeric.IsZero(lengthSquared, Numeric.EpsilonFSquared))
				return false;

			float length = (float)Math.Sqrt(lengthSquared);

			float scale = 1.0f / length;
			a.X *= scale;
			a.Y *= scale;

			return true;
		}

		/// <summary>
		/// Returns the normalized vector.
		/// </summary>
		/// <value>The normalized vector.</value>
		/// <remarks>
		/// The property does not change this instance. To normalize this instance you need to call 
		/// <see cref="Normalize"/>.
		/// </remarks>
		/// <exception cref="DivideByZeroException">
		/// The length of the vector is zero. The quaternion cannot be normalized.
		/// </exception>
		public static Vector2 Normalized(this Vector2 a)
		{
			Vector2 v = a;
			v.Normalize();
			return v;
		}

		/// <summary>
		/// Returns a vector with the absolute values of the elements of the given vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>A vector with the absolute values of the elements of the given vector.</returns>
		public static Vector2 Absolute(Vector2 vector)
		{
			return new Vector2(Math.Abs(vector.X), Math.Abs(vector.Y));
		}

		/// <summary>
		/// Returns a vector with the vector components clamped to the range [min, max].
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="min">The min limit.</param>
		/// <param name="max">The max limit.</param>
		/// <returns>A vector with clamped components.</returns>
		/// <remarks>
		/// This operation is carried out per component. Component values less than 
		/// <paramref name="min"/> are set to <paramref name="min"/>. Component values greater than 
		/// <paramref name="max"/> are set to <paramref name="max"/>.
		/// </remarks>
		public static Vector2 Clamp(Vector2 vector, float min, float max)
		{
			return new Vector2(MathHelper.Clamp(vector.X, min, max),
													MathHelper.Clamp(vector.Y, min, max));
		}

		/// <summary>
		/// Returns a value indicating whether this vector has zero size (the length is numerically
		/// equal to 0).
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this vector is numerically zero; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		/// <remarks>
		/// The length of this vector is compared to 0 using the default tolerance value (see 
		/// <see cref="Numeric.EpsilonD"/>).
		/// </remarks>
		public static bool IsNumericallyZero(this Vector3 a) => Numeric.IsZero(a.LengthSquared(), Numeric.EpsilonFSquared);

		/// <summary>
		/// Projects a vector onto an axis given by the target vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <param name="target">The target vector.</param>
		/// <returns>
		/// The projection of <paramref name="vector"/> onto <paramref name="target"/>.
		/// </returns>
		public static Vector3 ProjectTo(Vector3 vector, Vector3 target)
		{
			return Vector3.Dot(vector, target) / target.LengthSquared() * target;
		}

		/// <summary>
		/// Tries to normalize the vector.
		/// </summary>
		/// <returns>
		/// <see langword="true"/> if the vector was normalized; otherwise, <see langword="false"/> if 
		/// the vector could not be normalized. (The length is numerically zero.)
		/// </returns>
		public static bool TryNormalize(this ref Vector3 a)
		{
			float lengthSquared = a.LengthSquared();
			if (Numeric.IsZero(lengthSquared, Numeric.EpsilonDSquared))
				return false;

			float length = MathF.Sqrt(lengthSquared);

			float scale = 1.0f / length;
			a.X *= scale;
			a.Y *= scale;
			a.Z *= scale;

			return true;
		}

		/// <summary>
		/// Returns the normalized vector.
		/// </summary>
		/// <value>The normalized vector.</value>
		/// <remarks>
		/// The property does not change this instance. To normalize this instance you need to call 
		/// <see cref="Normalize"/>.
		/// </remarks>
		/// <exception cref="DivideByZeroException">
		/// The length of the vector is zero. The quaternion cannot be normalized.
		/// </exception>
		public static Vector3 Normalized(this Vector3 a)
		{
			Vector3 v = a;
			v.Normalize();
			return v;
		}

		/// <overloads>
		/// <summary>
		/// Determines whether two vectors are equal (regarding a given tolerance).
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Determines whether two vectors are equal (regarding the tolerance 
		/// <see cref="Numeric.EpsilonD"/>).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector.</param>
		/// <returns>
		/// <see langword="true"/> if the vectors are equal (within the tolerance 
		/// <see cref="Numeric.EpsilonD"/>); otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// The two vectors are compared component-wise. If the differences of the components are less
		/// than <see cref="Numeric.EpsilonD"/> the vectors are considered as being equal.
		/// </remarks>
		public static bool AreNumericallyEqual(Vector3 vector1, Vector3 vector2)
		{
			return Numeric.AreEqual(vector1.X, vector2.X)
					&& Numeric.AreEqual(vector1.Y, vector2.Y)
					&& Numeric.AreEqual(vector1.Z, vector2.Z);
		}

		/// <summary>
		/// Determines whether two vectors are equal (regarding a specific tolerance).
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector.</param>
		/// <param name="epsilon">The tolerance value.</param>
		/// <returns>
		/// <see langword="true"/> if the vectors are equal (within the tolerance 
		/// <paramref name="epsilon"/>); otherwise, <see langword="false"/>.
		/// </returns>
		/// <remarks>
		/// The two vectors are compared component-wise. If the differences of the components are less
		/// than <paramref name="epsilon"/> the vectors are considered as being equal.
		/// </remarks>
		public static bool AreNumericallyEqual(Vector3 vector1, Vector3 vector2, float epsilon)
		{
			return Numeric.AreEqual(vector1.X, vector2.X, epsilon)
					&& Numeric.AreEqual(vector1.Y, vector2.Y, epsilon)
					&& Numeric.AreEqual(vector1.Z, vector2.Z, epsilon);
		}

		/// <summary>
		/// Returns an arbitrary normalized <see cref="Vector3F"/> that is orthogonal to this vector.
		/// </summary>
		/// <value>An arbitrary normalized orthogonal <see cref="Vector3F"/>.</value>
		public static Vector3 Orthonormal1(this Vector3 a)
		{
			// Note: Other options to create normal vectors are discussed here:
			// http://blog.selfshadow.com/2011/10/17/perp-vectors/,
			// http://box2d.org/2014/02/computing-a-basis/
			// and here
			// "Building an Orthonormal Basis from a 3D Unit Vector Without Normalization"
			// http://orbit.dtu.dk/fedora/objects/orbit:113874/datastreams/file_75b66578-222e-4c7d-abdf-f7e255100209/content
			// This method is implemented in DigitalRune.Graphics/Misc.fxh/GetOrthonormals().

			Vector3 v;
			if (Numeric.IsZero(a.Z) == false)
			{
				// Orthonormal = (1, 0, 0) x (X, Y, Z)
				v.X = 0f;
				v.Y = -a.Z;
				v.Z = a.Y;
			}
			else
			{
				// Orthonormal = (0, 0, 1) x (X, Y, Z)
				v.X = -a.Y;
				v.Y = a.X;
				v.Z = 0f;
			}
			v.Normalize();
			return v;
		}

		/// <summary>
		/// Gets a normalized orthogonal <see cref="Vector3F"/> that is orthogonal to this 
		/// <see cref="Vector3F"/> and to <see cref="Orthonormal1"/>.
		/// </summary>
		/// <value>
		/// A normalized orthogonal <see cref="Vector3F"/> which is orthogonal to this 
		/// <see cref="Vector3F"/> and to <see cref="Orthonormal1"/>.
		/// </value>
		public static Vector3 Orthonormal2(this Vector3 a)
		{
			Vector3 v = Vector3.Cross(a, Orthonormal1(a));
			v.Normalize();
			return v;
		}

		/// <summary>
		/// Returns a vector with the absolute values of the elements of the given vector.
		/// </summary>
		/// <param name="vector">The vector.</param>
		/// <returns>A vector with the absolute values of the elements of the given vector.</returns>
		public static Vector3 Absolute(Vector3 vector)
		{
			return new Vector3(Math.Abs(vector.X), Math.Abs(vector.Y), Math.Abs(vector.Z));
		}

		/// <summary>
		/// Returns a value indicating whether this vector is normalized (the length is numerically
		/// equal to 1).
		/// </summary>
		/// <value>
		/// <see langword="true"/> if this <see cref="Vector3F"/> is normalized; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		/// <remarks>
		/// <see cref="IsNumericallyNormalized"/> compares the length of this vector against 1.0 using
		/// the default tolerance value (see <see cref="Numeric.EpsilonF"/>).
		/// </remarks>
		public static bool IsNumericallyNormalized(this Vector3 a)
		{
			return Numeric.AreEqual(a.LengthSquared(), 1.0f);
		}

		/// <summary>
		/// Converts this <see cref="Vector3"/> to <see cref="VectorF"/>.
		/// </summary>
		/// <returns>The result of the conversion.</returns>
		public static VectorF ToVectorF(this Vector3 a)
		{
			VectorF result = new VectorF(3);
			result[0] = a.X; result[1] = a.Y; result[2] = a.Z;
			return result;
		}

		public static float GetComponentByIndex(this Vector3 a, int index)
		{
			switch (index)
			{
				case 0: return a.X;
				case 1: return a.Y;
				case 2: return a.Z;
			}

			throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0, 1, or 2.");
		}

		public static void SetComponentByIndex(this ref Vector3 a, int index, float value)
		{
			switch (index)
			{
				case 0: a.X = value; break;
				case 1: a.Y = value; break;
				case 2: a.Z = value; break;
				default: throw new ArgumentOutOfRangeException("index", "The index is out of range. Allowed values are 0, 1, or 2.");
			}
		}

		public static bool IsLessThen(this Vector3 a, Vector3 b) => a.X < b.X && a.Y < b.Y && a.Z < b.Z;
		public static bool IsLessOrEqual(this Vector3 a, Vector3 b) => a.X <= b.X && a.Y <= b.Y && a.Z <= b.Z;
		public static bool IsGreaterThen(this Vector3 a, Vector3 b) => a.X > b.X && a.Y > b.Y && a.Z > b.Z;
		public static bool IsGreaterOrEqual(this Vector3 a, Vector3 b) => a.X >= b.X && a.Y >= b.Y && a.Z >= b.Z;

		/// <summary>
		/// Gets the index (zero-based) of the largest component.
		/// </summary>
		/// <value>The index (zero-based) of the largest component.</value>
		/// <remarks>
		/// <para>
		/// This method returns the index of the component (X, Y or Z) which has the largest value. The 
		/// index is zero-based, i.e. the index of X is 0. 
		/// </para>
		/// <para>
		/// If there are several components with equally large values, the smallest index of these is 
		/// returned.
		/// </para>
		/// </remarks>
		public static int IndexOfLargestComponent(this Vector3 a)
		{
			if (a.X >= a.Y && a.X >= a.Z)
				return 0;

			if (a.Y >= a.Z)
				return 1;

			return 2;
		}

		/// <summary>
		/// Gets the value of the largest component.
		/// </summary>
		/// <value>The value of the largest component.</value>
		public static float LargestComponent(this Vector3 a)
		{
				if (a.X >= a.Y && a.X >= a.Z)
					return a.X;

				if (a.Y >= a.Z)
					return a.Y;

				return a.Z;
		}

		/// <summary>
		/// Gets the value of the smallest component.
		/// </summary>
		/// <value>The value of the smallest component.</value>
		public static float SmallestComponent(this Vector3 a)
		{
			if (a.X <= a.Y && a.X <= a.Z)
				return a.X;

			if (a.Y <= a.Z)
				return a.Y;

			return a.Z;
		}


		/// <summary>
		/// Gets the index (zero-based) of the largest component.
		/// </summary>
		/// <value>The index (zero-based) of the largest component.</value>
		/// <remarks>
		/// <para>
		/// This method returns the index of the component (X, Y or Z) which has the smallest value. The 
		/// index is zero-based, i.e. the index of X is 0. 
		/// </para>
		/// <para>
		/// If there are several components with equally small values, the smallest index of these is 
		/// returned.
		/// </para>
		/// </remarks>
		public static int IndexOfSmallestComponent(this Vector3 a)
		{
			if (a.X <= a.Y && a.X <= a.Z)
				return 0;

			if (a.Y <= a.Z)
				return 1;

			return 2;
		}

		/// <summary>
		/// Gets a value indicating whether a component of the vector is <see cref="float.NaN"/>.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if a component of the vector is <see cref="float.NaN"/>; otherwise, 
		/// <see langword="false"/>.
		/// </value>
		public static bool IsNaN(this Vector3 a)
		{
			return Numeric.IsNaN(a.X) || Numeric.IsNaN(a.Y) || Numeric.IsNaN(a.Z);
		}

		/// <summary>
		/// Returns the cross product matrix (skew matrix) of this vector.
		/// </summary>
		/// <returns>The cross product matrix of this vector.</returns>
		/// <remarks>
		/// <c>Vector3F.Cross(v, w)</c> is the same as <c>v.ToCrossProductMatrix() * w</c>.
		/// </remarks>
		public static Matrix33F ToCrossProductMatrix(this Vector3 a)
		{
			return new Matrix33F(0, -a.Z, a.Y,
													 a.Z, 0, -a.X,
													 -a.Y, a.X, 0);
		}

		public static void SetLength(this ref Vector3 a, float value)
		{
			float length = a.Length();
			if (Numeric.IsZero(length))
				throw new MathematicsException("Cannot change length of a vector with length 0.");

			float scale = value / length;
			a.X *= scale;
			a.Y *= scale;
			a.Z *= scale;
		}

		/// <overloads>
		/// <summary>
		/// Clamps the vector components to the range [min, max].
		/// </summary>
		/// </overloads>
		/// 
		/// <summary>
		/// Clamps the vector components to the range [min, max].
		/// </summary>
		/// <param name="min">The min limit.</param>
		/// <param name="max">The max limit.</param>
		/// <remarks>
		/// This operation is carried out per component. Component values less than 
		/// <paramref name="min"/> are set to <paramref name="min"/>. Component values greater than 
		/// <paramref name="max"/> are set to <paramref name="max"/>.
		/// </remarks>
		public static Vector3 Clamp(Vector3 a, float min, float max)
		{
			a.X = MathHelper.Clamp(a.X, min, max);
			a.Y = MathHelper.Clamp(a.Y, min, max);
			a.Z = MathHelper.Clamp(a.Z, min, max);

			return a;
		}

		/// <summary>
		/// Calculates the angle between two vectors.
		/// </summary>
		/// <param name="vector1">The first vector.</param>
		/// <param name="vector2">The second vector.</param>
		/// <returns>The angle between the given vectors, such that 0 ≤ angle ≤ π.</returns>
		/// <exception cref="ArgumentException">
		/// <paramref name="vector1"/> or <paramref name="vector2"/> has a length of 0.
		/// </exception>
		public static float GetAngle(Vector3 vector1, Vector3 vector2)
		{
			if (!vector1.TryNormalize() || !vector2.TryNormalize())
				throw new ArgumentException("vector1 and vector2 must not have 0 length.");

			float α = Vector3.Dot(vector1, vector2);

			// Inaccuracy in the floating-point operations can cause
			// the result be outside of the valid range.
			// Ensure that the dot product α lies in the interval [-1, 1].
			// Math.Acos() returns Double.NaN if the argument lies outside
			// of this interval.
			α = MathHelper.Clamp(α, -1.0f, 1.0f);

			return (float)Math.Acos(α);
		}

		/// <summary>
		/// Converts this vector to an array of 3 <see langword="float"/> values.
		/// </summary>
		/// <returns>
		/// The array with 3 <see langword="float"/> values. The order of the elements is: x, y, z
		/// </returns>
		public static float[] ToArray(this Vector3 a)
		{
			return new[] { a.X, a.Y, a.Z };
		}
	}
}
