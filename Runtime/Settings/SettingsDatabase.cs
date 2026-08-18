using System.Collections.Generic;

namespace Kryz.Settings
{
	public class SettingsDatabase : ISettingsDatabase
	{
		private readonly Dictionary<ulong, SettingsAsset> settings = new();

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

		public void InitializeSettings(IEnumerable<SettingsAsset> settings)
		{
			this.settings.Clear();

			foreach (SettingsAsset item in settings)
			{
				this.settings[item.Id] = item;
			}
		}
	}
}