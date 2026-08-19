using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Kryz.Settings
{
	public static class SettingsSerializer
	{
		public struct Header
		{
			public ulong id;
			public bool enabled;
		}

		public static void Serialize<T>(T settings, TextWriter writer) where T : IEnumerable<SettingsAsset>
		{
			Dictionary<Type, List<SettingsAsset>> settingsByType = new();

			foreach (SettingsAsset setting in settings)
			{
				Type type = setting.GetType();

				if (!settingsByType.TryGetValue(type, out List<SettingsAsset> list))
				{
					settingsByType[type] = list = new List<SettingsAsset>();
				}

				list.Add(setting);
			}

			foreach (KeyValuePair<Type, List<SettingsAsset>> item in settingsByType)
			{
				writer.WriteLine(item.Key.AssemblyQualifiedName);

				foreach (SettingsAsset setting in item.Value)
				{
					writer.WriteLine(JsonUtility.ToJson(setting));
				}
			}
		}

		public static void Deserialize(ISettingsStore settings, TextReader reader)
		{
			Type type = null;

			for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
			{
				if (string.IsNullOrWhiteSpace(line))
					continue;

				if (!line.StartsWith('{'))
				{
					type = Type.GetType(line);
					continue;
				}

				Header header = JsonUtility.FromJson<Header>(line);

				if (!header.enabled)
				{
					settings.Remove(header.id);
					continue;
				}

				if (!settings.TryGetValue(header.id, out SettingsAsset asset))
				{
					asset = (SettingsAsset)ScriptableObject.CreateInstance(type);
					settings.Add(header.id, asset);
				}

				JsonUtility.FromJsonOverwrite(line, asset);
			}
		}
	}
}