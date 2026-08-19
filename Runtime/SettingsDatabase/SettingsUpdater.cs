using System.IO;

namespace Kryz.Settings
{
	public class SettingsUpdater : ISettingsUpdater
	{
		private TextReader reader;

		public void UpdateFromFile(ISettingsDatabase settingsDatabase, string path)
		{
			reader = new StreamReader(path);
			settingsDatabase.UpdateSettings(this);
			reader.Close();
			reader = null;
		}

		public void UpdateFromString(ISettingsDatabase settingsDatabase, string contents)
		{
			reader = new StringReader(contents);
			settingsDatabase.UpdateSettings(this);
			reader.Close();
			reader = null;
		}

		public void UpdateSettings(ISettingsStore settings)
		{
			if (reader == null || reader.Peek() < 0)
				return;

			SettingsSerializer.Deserialize(settings, reader);
		}
	}
}