using System;
using System.Collections.Generic;
using UnityEditor;

namespace Kryz.Settings.Editor
{
	public static class SettingsCatalogManager
	{
		private class PostProcessor : AssetPostprocessor
		{
			private const int version = 1;

			public override uint GetVersion() => version;

			public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
			{
				bool setDirty = false;
				SettingsCatalog catalog = SingletonScriptableObjectUtils.GetSingleton<SettingsCatalog>();

				if (catalog.version != version)
				{
					catalog.version = version;
					setDirty = true;
					UpdateByGUID(catalog, AssetDatabase.FindAssetGUIDs(""));
				}
				else
				{
					setDirty |= UpdateByPath(catalog, importedAssets, delete: false);
					setDirty |= UpdateByPath(catalog, deletedAssets, delete: true);
					setDirty |= UpdateByPath(catalog, movedAssets, delete: false);
				}

				if (setDirty)
				{
					EditorUtility.SetDirty(catalog);
					AssetDatabase.SaveAssetIfDirty(catalog);
				}
			}
		}

		private class ModificationProcessor : AssetModificationProcessor
		{
			public static string[] OnWillSaveAssets(string[] paths)
			{
				foreach (string path in paths)
				{
					SettingsAsset settingsAsset = AssetDatabase.LoadAssetAtPath<SettingsAsset>(path);
					if (settingsAsset == null)
						continue;

					GUID guid = AssetDatabase.GUIDFromAssetPath(path);
					uint id = (uint)guid.GetHashCode();
					UpdateSettingsAssetData(settingsAsset, id);
				}
				return paths;
			}
		}

		private static void UpdateSettingsAssetData(SettingsAsset settingsAsset, uint id)
		{
			string name = settingsAsset.name;
			if (settingsAsset.Id != id || !settingsAsset.Name.Equals(name, StringComparison.Ordinal))
			{
				settingsAsset.SetIdAndName(id, name);
				EditorUtility.SetDirty(settingsAsset);
			}
		}

		private static bool UpdateByGUID(SettingsCatalog catalog, GUID[] guids)
		{
			bool setDirty = false;

			foreach (GUID guid in guids)
			{
				setDirty |= UpdateEntry(catalog.assets, guid: guid, delete: false);
			}

			return setDirty;
		}

		private static bool UpdateByPath(SettingsCatalog catalog, string[] paths, bool delete)
		{
			bool setDirty = false;

			foreach (string path in paths)
			{
				setDirty |= UpdateEntry(catalog.assets, path: path, delete: delete);
			}

			return setDirty;
		}

		private static bool UpdateEntry(Dictionary<uint, SettingsAsset> catalog, GUID guid = default, string path = "", bool delete = false)
		{
			if (guid == default) guid = AssetDatabase.GUIDFromAssetPath(path);
			if (string.IsNullOrEmpty(path)) path = AssetDatabase.GUIDToAssetPath(guid);

			if (AssetDatabase.IsValidFolder(path))
				return false;

			SettingsAsset settingsAsset = AssetDatabase.LoadAssetByGUID<SettingsAsset>(guid);
			uint id = (uint)guid.GetHashCode();

			if (delete || settingsAsset == null || !settingsAsset.Enabled)
			{
				return catalog.Remove(id);
			}

			UpdateSettingsAssetData(settingsAsset, id);

			if (catalog.TryGetValue(id, out SettingsAsset current) && current == settingsAsset)
			{
				return false;
			}

			return catalog.TryAdd(id, settingsAsset);
		}
	}
}