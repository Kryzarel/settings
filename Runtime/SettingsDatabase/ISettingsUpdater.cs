namespace Kryz.Settings
{
	public interface ISettingsUpdater
	{
		void UpdateSettings(ISettingsStore settings);
	}
}