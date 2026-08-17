using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Kryz.Settings.Editor
{
	public class EditorSettingsManager
	{
		private static readonly Type settingsType = typeof(SettingsAsset);
		private static readonly Dictionary<uint, SettingsAsset> settings = new();

		static EditorSettingsManager()
		{
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
				SettingsAsset setting = AssetDatabase.LoadAssetByGUID<SettingsAsset>(guid);

				if (!settings.TryAdd(setting.Id, setting))
				{
					Debug.LogError($"Asset ID collision between {setting.name} and {settings[setting.Id].name}. Please regenerate one of the GUIDs.", setting);
				}
			}
		}
	}
}