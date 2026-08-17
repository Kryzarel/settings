namespace Kryz.Settings
{
	public interface ISettingsDatabase
	{
		public T Get<T>(uint id) where T : SettingsAsset;
	}
}