using System;
using System.Collections.Generic;
using Kryz.UnityUtils.Editor;
using UnityEditor;

namespace Kryz.Settings.Editor
{
	public static class ResourcesCatalogManager
	{
		private class PostProcessor : AssetPostprocessor
		{
			private const int version = 3;

			public override uint GetVersion() => version;

			public static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
			{
				bool setDirty = false;
				ResourcesCatalog catalog = SingletonScriptableObjectUtils.Get<ResourcesCatalog>();

				if (catalog.version != version)
				{
					catalog.version = version;
					catalog.assets.Clear();
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

		private static bool UpdateByGUID(ResourcesCatalog catalog, GUID[] guids)
		{
			bool setDirty = false;

			foreach (GUID guid in guids)
			{
				setDirty |= UpdateEntry(catalog.assets, guid: guid, delete: false);
			}

			return setDirty;
		}

		private static bool UpdateByPath(ResourcesCatalog catalog, string[] paths, bool delete)
		{
			bool setDirty = false;

			foreach (string path in paths)
			{
				setDirty |= UpdateEntry(catalog.assets, path: path, delete: delete);
			}

			return setDirty;
		}

		private static bool UpdateEntry(Dictionary<ulong, string> catalog, GUID guid = default, string path = "", string resourcesPath = "", bool delete = false)
		{
			if (guid == default) guid = AssetDatabase.GUIDFromAssetPath(path);
			if (string.IsNullOrEmpty(path)) path = AssetDatabase.GUIDToAssetPath(guid);

			if (AssetDatabase.IsValidFolder(path))
				return false;

			if (!delete && string.IsNullOrEmpty(resourcesPath)) resourcesPath = AssetDatabaseUtilities.GetPathRelativeToResources(path);

			ulong id = guid.GetAssetId64();

			if (delete || string.IsNullOrEmpty(resourcesPath))
			{
				return catalog.Remove(id);
			}
			else if (catalog.TryGetValue(id, out string current) && current.Equals(resourcesPath, StringComparison.Ordinal))
			{
				return false;
			}

			catalog[id] = resourcesPath;
			return true;
		}
	}
}