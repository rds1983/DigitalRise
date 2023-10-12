using System;
using System.ComponentModel.Design;

namespace DigitalRise
{
	public static class ServiceLocatorExtensions
	{
		public static T GetInstance<T>(this IServiceProvider servies) => (T)servies.GetService(typeof(T));
	}
}
