using System.IO;

namespace Kryz.Settings
{
	public readonly struct SettingsUpdater : ISettingsUpdater
	{
		private readonly TextReader reader;

		public SettingsUpdater(TextReader reader) => this.reader = reader;

		public void UpdateSettings(ISettingsStore settings)
		{
			SettingsSerializer.Deserialize(settings, reader);
		}

		public static void UpdateFromFile(ISettingsDatabase settingsDatabase, string path)
		{
			using StreamReader reader = new(path);
			settingsDatabase.UpdateSettings(new SettingsUpdater(reader));
		}

		public static void UpdateFromString(ISettingsDatabase settingsDatabase, string contents)
		{
			using StringReader reader = new(contents);
			settingsDatabase.UpdateSettings(new SettingsUpdater(reader));
		}
	}
}