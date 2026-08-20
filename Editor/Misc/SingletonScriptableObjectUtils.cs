using System;
using System.IO;
using Kryz.UnityUtils.Editor;
using UnityEditor;
using UnityEngine;

namespace Kryz.Settings.Editor
{
	public class SingletonScriptableObjectUtils
	{
		public static T Get<T>() where T : SingletonScriptableObject<T>
		{
			T[] objects = AssetDatabaseUtilities.FindAssetsOfType<T>();

			if (objects.Length > 1)
			{
				Debug.LogError($"More than one {nameof(SingletonScriptableObject<T>)} found. This is not supported.", objects[1]);
			}
			else if (objects.Length == 0)
			{
				T value = ScriptableObject.CreateInstance<T>();

				string path = Path.Combine("Assets", "Resources", "Kryz.Singleton", $"{typeof(T).Name}.asset");
				string directory = Path.GetDirectoryName(path);
				if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

				AssetDatabase.CreateAsset(value, path);
				Debug.Log($"Couldn't find {typeof(T).Name}. Creating one. Path: {path}", value);
				return value;
			}

			string resourcesPath = AssetDatabaseUtilities.GetPathRelativeToResources(objects[0]);
			if (!resourcesPath.Equals("Kryz.Singleton/" + typeof(T).Name, StringComparison.Ordinal))
			{
				Debug.LogError($"{typeof(T).Name} is not in \"Resources/Kryz.Singleton/{typeof(T).Name}\". This is not supported.", objects[0]);
			}
			return objects[0];
		}
	}
}