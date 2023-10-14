using DigitalRise.Mathematics;
using System;

namespace NUnit.Utils
{
	public class SetEpsilonD : IDisposable
	{
		private readonly double _oldEpsilonD;

		public SetEpsilonD(double newEpsilon)
		{
			_oldEpsilonD = Numeric.EpsilonD;
			Numeric.EpsilonD = (double)newEpsilon;
		}

		public void Dispose()
		{
			Numeric.EpsilonD = _oldEpsilonD;
		}
	}
}
