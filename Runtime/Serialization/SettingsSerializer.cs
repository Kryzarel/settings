using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Kryz.Settings
{
	public class SettingsSerializer
	{
		private static readonly string[] lineSeparators = { "\r\n", "\r", "\n" };

		public struct Header
		{
			public ulong id;
			public string type;
			public bool enabled;
		}

		public static string Serialize(IEnumerable<SettingsAsset> assets)
		{
			StringBuilder sb = new();
			foreach (SettingsAsset item in assets)
			{
				sb.AppendLine(JsonUtility.ToJson(item));
			}
			return sb.ToString();
		}

		public static List<SettingsAsset> Deserialize(string json)
		{
			List<SettingsAsset> settings = new();
			string[] lines = json.Split(lineSeparators, StringSplitOptions.None);
			foreach (string line in lines)
			{
				if (string.IsNullOrEmpty(line)) continue;
				SettingsAsset asset = null;
				OverwriteOrCreateSetting(line, ref asset);
				settings.Add(asset);
			}
			return settings;
		}

		public static void SerializeToFile(IEnumerable<SettingsAsset> assets, string filePath)
		{
			using StreamWriter writer = new(filePath);

			foreach (SettingsAsset item in assets)
			{
				writer.WriteLine(JsonUtility.ToJson(item));
			}
		}

		public static List<SettingsAsset> DeserializeFromFile(string filePath)
		{
			List<SettingsAsset> settings = new();
			using StreamReader reader = new(filePath);

			while (!reader.EndOfStream)
			{
				string line = reader.ReadLine();
				SettingsAsset asset = null;
				OverwriteOrCreateSetting(line, ref asset);
				settings.Add(asset);
			}

			return settings;
		}

		public static void OverwriteOrCreateSetting(string json, ref SettingsAsset asset)
		{
			if (asset == null)
			{
				Header header = JsonUtility.FromJson<Header>(json);
				Type type = Type.GetType(header.type);
				asset = (SettingsAsset)ScriptableObject.CreateInstance(type);
			}

			JsonUtility.FromJsonOverwrite(json, asset);
		}
	}
}