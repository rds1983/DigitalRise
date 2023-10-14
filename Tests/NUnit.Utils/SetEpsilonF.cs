using DigitalRise.Mathematics;
using System;

namespace NUnit.Utils
{
	public class SetEpsilonF: IDisposable
	{
		private readonly float _oldEpsilonF;

		public SetEpsilonF(float newEpsilon)
		{
			_oldEpsilonF = Numeric.EpsilonF;
			Numeric.EpsilonF = newEpsilon;
		}

		public void Dispose()
		{
			Numeric.EpsilonF = _oldEpsilonF;
		}
	}
}
