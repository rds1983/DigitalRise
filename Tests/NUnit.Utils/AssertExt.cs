using DigitalRise.Animation.Character;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace NUnit.Utils
{
	public static class AssertExt
	{
		public static void ReportEqualFailure(object expected, object actual, object epsilon)
		{
			Assert.Fail($"Expected: {expected}\nActual: {actual}\nEpsilon: {epsilon}");
		}

		public static void AreNumericallyEqual(float expected, float actual, float epsilon)
		{
			if (Numeric.AreEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(float expected, float actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(double expected, double actual, double epsilon)
		{
			if (Numeric.AreEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(double expected, double actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonD);

		public static void AreNumericallyEqual(Vector2 expected, Vector2 actual, float epsilon)
		{
			if (MathHelper.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Vector2 expected, Vector2 actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(Vector3 expected, Vector3 actual, float epsilon)
		{
			if (MathHelper.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Vector3 expected, Vector3 actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(Vector4 expected, Vector4 actual, float epsilon)
		{
			if (MathHelper.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Vector4 expected, Vector4 actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(Quaternion expected, Quaternion actual, float epsilon)
		{
			if (MathHelper.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Quaternion expected, Quaternion actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(Matrix22F expected, Matrix22F actual, float epsilon)
		{
			if (Matrix22F.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Matrix22F expected, Matrix22F actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(Matrix33F expected, Matrix33F actual, float epsilon)
		{
			if (Matrix33F.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Matrix33F expected, Matrix33F actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(Matrix44F expected, Matrix44F actual, float epsilon)
		{
			if (Matrix44F.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Matrix44F expected, Matrix44F actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(SrtTransform expected, SrtTransform actual, float epsilon)
		{
			if (SrtTransform.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(SrtTransform expected, SrtTransform actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(Aabb expected, Aabb actual, float epsilon)
		{
			if (Aabb.AreNumericallyEqual(expected, actual, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(Aabb expected, Aabb actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);

		public static void AreNumericallyEqual(TimeSpan expected, TimeSpan actual, float epsilon)
		{
			if (Numeric.AreEqual((float)expected.TotalSeconds, (float)actual.TotalSeconds, epsilon))
			{
				return;
			}

			ReportEqualFailure(expected, actual, epsilon);
		}

		public static void AreNumericallyEqual(TimeSpan expected, TimeSpan actual) =>
			AreNumericallyEqual(expected, actual, Numeric.EpsilonF);
	}
}
