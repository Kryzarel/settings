using System.IO;
using Kryz.UnityUtils.Editor;
using UnityEditor;
using UnityEngine;

namespace Kryz.Settings.Editor
{
	public class SingletonScriptableObjectUtils
	{
		public static T GetSingleton<T>() where T : SingletonScriptableObject<T>
		{
			T[] objects = AssetDatabaseUtilities.FindAssetsOfType<T>();

			if (objects.Length > 1)
			{
				Debug.LogError($"More than one {nameof(SingletonScriptableObject<T>)} found. This is not supported.", objects[1]);
			}
			else if (objects.Length == 0)
			{
				T value = ScriptableObject.CreateInstance<T>();
				string path = Path.Combine("Assets", "Resources", $"{typeof(T).Name}.asset");
				AssetDatabase.CreateAsset(value, path);
				Debug.Log($"Couldn't find {typeof(T).Name}. Creating one. Path: {path}", value);
				return value;
			}

			string resourcesPath = AssetDatabaseUtilities.GetPathRelativeToResources(objects[0]);
			if (string.IsNullOrEmpty(resourcesPath))
			{
				Debug.LogError($"{typeof(T).Name} is not in Resources. This is not supported.", objects[0]);
			}
			return objects[0];
		}
	}
}