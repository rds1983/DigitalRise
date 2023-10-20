using DigitalRise.GameBase;

namespace DigitalRise.UI
{
	public class GamePropertyInfo<T>
	{
		private readonly int _id;

		public int Id => _id;

		internal GamePropertyInfo(int propertyId)
		{
			_id = propertyId;
		}

		public GameProperty<T> Get(GameObject owner)
		{
			return owner.Properties.Get<T>(_id);
		}

		public T GetValue(GameObject owner)
		{
			var property = Get(owner);
			return property.Value;
		}

		public void SetValue(GameObject owner, T value)
		{
			var property = Get(owner);
			property.Value = value;
		}
	}
}
