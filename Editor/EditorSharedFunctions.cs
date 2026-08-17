using System.IO;
using Kryz.UnityUtils.Editor;
using UnityEditor;
using UnityEngine;

namespace Kryz.Settings.Editor
{
	[InitializeOnLoad]
	public static class EditorSharedFunctions
	{
		static EditorSharedFunctions()
		{
			SharedFunctions.GetSettingsId = GetSettingsId;
			SharedFunctions.GetResourcesPath = GetResourcesPath;
			SharedFunctions.GetAssetName = GetAssetName;
		}

		internal static uint GetSettingsId(SettingsAsset asset)
		{
			if (asset == null)
			{
				return 0;
			}
			string path = AssetDatabase.GetAssetPath(asset);
			GUID guid = AssetDatabase.GUIDFromAssetPath(path);
			return (uint)guid.GetHashCode();
		}

		internal static string GetResourcesPath(Object asset)
		{
			if (asset == null)
			{
				return string.Empty;
			}
			return AssetDatabaseUtilities.GetPathRelativeToResources(asset);
		}

		internal static string GetAssetName(Object asset)
		{
			if (asset == null)
			{
				return string.Empty;
			}
			return Path.GetFileNameWithoutExtension(AssetDatabase.GetAssetPath(asset));
		}
	}
}