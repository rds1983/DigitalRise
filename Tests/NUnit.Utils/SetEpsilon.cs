using DigitalRise.Mathematics;
using System;

namespace NUnit.Utils
{
	public class SetEpsilon: IDisposable
	{
		private readonly float _oldEpsilonF;
		private readonly double _oldEpsilonD;

		public SetEpsilon(float newEpsilon)
		{
			_oldEpsilonF = Numeric.EpsilonF;
			_oldEpsilonD = Numeric.EpsilonD;
			Numeric.EpsilonF = newEpsilon;
			Numeric.EpsilonD = newEpsilon;
		}

		public void Dispose()
		{
			Numeric.EpsilonF = _oldEpsilonF;
			Numeric.EpsilonD = _oldEpsilonD;
		}
	}
}
