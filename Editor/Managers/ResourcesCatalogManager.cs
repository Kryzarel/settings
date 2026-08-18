using Kryz.UnityUtils.Editor;
using UnityEditor;
using UnityEngine;

namespace Kryz.Settings.Editor
{
	public class ResourcesCatalogManager : SingletonScriptableObjectManager<ResourcesCatalog>
	{
		private const int version = 1;

		public override uint GetVersion() => version;

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
		{
			bool setDirty = false;
			ResourcesCatalog catalog = GetSingleton();

			if (catalog.version != version)
			{
				catalog.version = version;
				FullRefresh(catalog);
				setDirty = true;
			}
			else
			{
				setDirty |= HandleImport(catalog, importedAssets);
				setDirty |= HandleDelete(catalog, deletedAssets);
				setDirty |= HandleMove(catalog, movedAssets, movedFromAssetPaths);
			}

			if (setDirty)
			{
				EditorUtility.SetDirty(catalog);
				AssetDatabase.SaveAssetIfDirty(catalog);
			}
		}

		private static void FullRefresh(ResourcesCatalog catalog)
		{
			catalog.assets.Clear();

			foreach (GUID guid in AssetDatabase.FindAssetGUIDs(""))
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				if (AssetDatabase.IsValidFolder(path))
					continue;

				string resourcesPath = AssetDatabaseUtilities.GetPathRelativeToResources(path);
				if (string.IsNullOrEmpty(resourcesPath))
					continue;

				catalog.assets.Add(new ResourcesCatalog.Data { Id = (uint)guid.GetHashCode(), Path = resourcesPath });
			}
		}

		private static bool HandleImport(ResourcesCatalog catalog, string[] importedAssets)
		{
			bool setDirty = false;

			foreach (string path in importedAssets)
			{
				if (AssetDatabase.IsValidFolder(path))
					continue;

				string resourcesPath = AssetDatabaseUtilities.GetPathRelativeToResources(path);
				if (string.IsNullOrEmpty(resourcesPath))
					continue;

				GUID guid = AssetDatabase.GUIDFromAssetPath(path);

				ResourcesCatalog.Data data = new() { Id = (uint)guid.GetHashCode(), Path = resourcesPath };

				int index = catalog.assets.IndexOf(data);
				if (index < 0)
				{
					catalog.assets.Add(data);
					setDirty = true;
				}
			}

			return setDirty;
		}

		private static bool HandleDelete(ResourcesCatalog catalog, string[] deletedAssets)
		{
			bool setDirty = false;

			foreach (string path in deletedAssets)
			{
				if (AssetDatabase.IsValidFolder(path))
					continue;

				GUID guid = AssetDatabase.GUIDFromAssetPath(path);
				uint id = (uint)guid.GetHashCode();

				int index = catalog.assets.FindIndex(e => e.Id == id);

				if (index >= 0)
				{
					catalog.assets.RemoveAt(index);
					Debug.Log($"Deleting {id} {path}");
					setDirty = true;
				}
			}

			return setDirty;
		}

		private static bool HandleMove(ResourcesCatalog catalog, string[] movedAssets, string[] movedFromAssetPaths)
		{
			bool setDirty = false;

			for (int i = 0; i < movedAssets.Length; i++)
			{
				string path = movedAssets[i];
				if (AssetDatabase.IsValidFolder(path))
					continue;

				string resourcesPath = AssetDatabaseUtilities.GetPathRelativeToResources(path);
				GUID guid = AssetDatabase.GUIDFromAssetPath(path);
				uint id = (uint)guid.GetHashCode();

				int index = catalog.assets.FindIndex(e => e.Id == id);

				// Moved INTO resources, add to catalog
				if (index < 0 && !string.IsNullOrEmpty(resourcesPath))
				{
					catalog.assets.Add(new ResourcesCatalog.Data { Id = id, Path = resourcesPath });
					setDirty = true;
				}
				// Moved OUT OF resources, remove from catalog
				else if (index >= 0 && string.IsNullOrEmpty(resourcesPath))
				{
					catalog.assets.RemoveAt(index);
					setDirty = true;
				}
				// Moved to a differente resources path, update existing entry
				else if(index >= 0 && !string.IsNullOrEmpty(resourcesPath))
				{
					ResourcesCatalog.Data data = catalog.assets[i];
					data.Path = resourcesPath;
					catalog.assets[i] = data;
					setDirty = true;
				}
			}

			return setDirty;
		}
	}
}