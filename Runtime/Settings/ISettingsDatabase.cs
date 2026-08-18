namespace Kryz.Settings
{
	public interface ISettingsDatabase
	{
		public T Get<T>(ulong id) where T : SettingsAsset;
	}
}