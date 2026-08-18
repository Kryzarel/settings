using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Kryz.Settings.Editor
{
	public class EditorSettingsManager
	{
		private const BindingFlags bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
		private static readonly Type settingsType = typeof(SettingsAsset);

		private static readonly Dictionary<uint, SettingsAsset> settings = new();

		public static readonly IReadOnlyDictionary<uint, SettingsAsset> Settings;

		static EditorSettingsManager()
		{
			Settings = new ReadOnlyDictionary<uint, SettingsAsset>(settings);

			Refresh();
			EditorApplication.projectChanged -= Refresh;
			EditorApplication.projectChanged += Refresh;
		}

		private static void Refresh()
		{
			GUID[] guids = AssetDatabase.FindAssetGUIDs("t:" + settingsType.Name);

			settings.Clear();
			settings.EnsureCapacity(guids.Length);

			foreach (GUID guid in guids)
			{
				uint id = (uint)guid.GetHashCode();
				SettingsAsset setting = AssetDatabase.LoadAssetByGUID<SettingsAsset>(guid);

				if (setting.Id != id || setting.Name != setting.name)
				{
					setting.SetIdAndName(id, setting.name);
					EditorUtility.SetDirty(setting);
				}

				if (!settings.TryAdd(id, setting))
				{
					Debug.LogError($"Asset ID collision between {setting.name} and {settings[id].name}. Please regenerate one of the GUIDs.", setting);
				}
			}

			AssetDatabase.SaveAssets();
		}
	}
}