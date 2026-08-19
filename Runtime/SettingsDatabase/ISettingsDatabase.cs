using System.Collections.Generic;

namespace Kryz.Settings
{
	public interface ISettingsDatabase
	{
		SettingsAsset Get(ulong id);
		bool TryGet(ulong id, out SettingsAsset asset);

		T Get<T>(ulong id) where T : SettingsAsset;
		bool TryGet<T>(ulong id, out T asset) where T : SettingsAsset;

		bool TryGetSingleton<T>(out T asset) where T : SettingsAsset;

		void GetAllSettingsOfType<T>(IList<T> values) where T : SettingsAsset;
		IEnumerable<T> GetAllSettingsOfType<T>() where T : SettingsAsset;

		void UpdateSettings(ISettingsUpdater settingsUpdater);
	}
}