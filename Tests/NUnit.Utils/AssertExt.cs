using DigitalRise.Animation.Character;
using DigitalRise.Geometry.Shapes;
using DigitalRise.Mathematics;
using DigitalRise.Mathematics.Algebra;
using Microsoft.Xna.Framework;
using System;
using MathHelper = DigitalRise.Mathematics.MathHelper;

namespace NUnit.Utils
{
	public static class AssertExt
	{
		public static void ReportEqualFailure(object expected, object actual, float epsilon)
		{
			throw new Exception($"Expected: {expected}, Actual: {actual}, Epsilon: {epsilon}");

		}

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
	}
}
