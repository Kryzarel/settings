using System.Collections.Generic;

namespace Kryz.Settings
{
	public class SettingsDatabase : ISettingsDatabase
	{
		private readonly Dictionary<ulong, SettingsAsset> settings;

		private readonly SettingsStore settingsStore;

		private class SettingsStore : ISettingsStore
		{
			private readonly Dictionary<ulong, SettingsAsset> settings;

			public SettingsStore(Dictionary<ulong, SettingsAsset> settings) => this.settings = settings;

			public void Add(ulong id, SettingsAsset asset) => settings.Add(id, asset);
			public bool Remove(ulong id) => settings.Remove(id);
			public bool TryGetValue(ulong id, out SettingsAsset asset) => settings.TryGetValue(id, out asset);
		}

		public SettingsDatabase()
		{
			settings = new Dictionary<ulong, SettingsAsset>();
			settingsStore = new SettingsStore(settings);
		}

		public SettingsDatabase(IEnumerable<KeyValuePair<ulong, SettingsAsset>> settings)
		{
			this.settings = new Dictionary<ulong, SettingsAsset>(settings);
			settingsStore = new SettingsStore(this.settings);
		}

		public SettingsAsset Get(ulong id)
		{
			return settings[id];
		}

		public bool TryGet(ulong id, out SettingsAsset asset)
		{
			return settings.TryGetValue(id, out asset);
		}

		public T Get<T>(ulong id) where T : SettingsAsset
		{
			return (T)settings[id];
		}

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
						asset = null;
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

		public void UpdateSettings<T>(T updater) where T : ISettingsUpdater
		{
			updater.UpdateSettings(settingsStore);
		}
	}
}