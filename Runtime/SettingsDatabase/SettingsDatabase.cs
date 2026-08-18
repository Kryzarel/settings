using System.Collections.Generic;

namespace Kryz.Settings
{
	public class SettingsDatabase : ISettingsDatabase
	{
		private readonly Dictionary<uint, SettingsAsset> settings = new();

		public T Get<T>(uint id) where T : SettingsAsset => settings[id] as T;
	}
}