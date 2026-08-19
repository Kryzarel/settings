namespace Kryz.Settings
{
	public interface ISettingsStore
	{
		void Add(ulong id, SettingsAsset asset);
		bool Remove(ulong id);
		bool TryGetValue(ulong id, out SettingsAsset asset);
	}
}