using System.Collections.Generic;

namespace Kryz.Settings
{
	public class SettingsDatabase : ISettingsDatabase
	{
		private readonly Dictionary<ulong, SettingsAsset> settings;

		public SettingsDatabase()
		{
			settings = new();
		}

		public T Get<T>(ulong id) where T : SettingsAsset => (T)settings[id];

		public bool TryGet<T>(ulong id, out T asset) where T : SettingsAsset
		{
			if (settings.TryGetValue(id, out SettingsAsset baseAsset) && baseAsset is T derivedAsset)
			{
				asset = derivedAsset;
				return true;
			}
			asset = default;
			return false;
		}

		public bool TryGetSingleton<T>(out T asset) where T : SettingsAsset
		{
			asset = null;

			foreach (SettingsAsset item in settings.Values)
			{
				if (item is T derived)
				{
					if (asset != null)
					{
						return false;
					}
					asset = derived;
				}
			}
			return asset != null;
		}

		public void GetAllSettingsOfType<T>(IList<T> values) where T : SettingsAsset
		{
			foreach (SettingsAsset item in settings.Values)
			{
				if (item is T derived)
				{
					values.Add(derived);
				}
			}
		}

		public IEnumerable<T> GetAllSettingsOfType<T>() where T : SettingsAsset
		{
			foreach (SettingsAsset item in settings.Values)
			{
				if (item is T derived)
				{
					yield return derived;
				}
			}
		}
	}
}