using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Kryz.Settings.Editor
{
	[CustomPropertyDrawer(typeof(AssetPicker), useForChildren: true)]
	public class AssetPickerDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new();

			ObjectField objectField = new(property.displayName);
			objectField.AddToClassList("unity-base-field__aligned");
			objectField.AddToClassList("unity-base-field__inspector-field");
			root.Add(objectField);

			AssetPicker picker = property.boxedValue as AssetPicker;
			objectField.objectType = GetAssetType(picker.GetType());
			objectField.allowSceneObjects = false;

			SerializedProperty idProperty = property.FindPropertyRelative("id");

			HelpBox warningBox = new($"<b>Warning:</b> The assigned object is not a {nameof(SettingsAsset)} or is not in Resources.", HelpBoxMessageType.Warning);
			root.Add(warningBox);

			objectField.value = GetAssetFromId(idProperty);
			warningBox.style.display = !IsValidAsset(objectField.value) ? DisplayStyle.Flex : DisplayStyle.None;

			objectField.RegisterValueChangedCallback(c =>
			{
				SetIdFromAsset(idProperty, c.newValue);
				warningBox.style.display = !IsValidAsset(c.newValue) ? DisplayStyle.Flex : DisplayStyle.None;
			});
			return root;
		}

		private static bool IsValidId(uint id)
		{
			return ResourcesCatalog.Instance.Assets.ContainsKey(id) || EditorSettingsManager.Settings.ContainsKey(id);
		}

		private static bool IsValidAsset(Object asset)
		{
			if (asset == null) return true;
			string path = AssetDatabase.GetAssetPath(asset);
			GUID guid = AssetDatabase.GUIDFromAssetPath(path);
			uint id = (uint)guid.GetHashCode();
			return IsValidId(id);
		}

		private static Object GetAssetFromId(SerializedProperty idProperty)
		{
			uint id = (uint)idProperty.intValue;

			if (ResourcesCatalog.Instance.Assets.TryGetValue(id, out string resourcesPath))
				return Resources.Load(resourcesPath);

			if (EditorSettingsManager.Settings.TryGetValue(id, out SettingsAsset asset))
				return asset;

			return null;
		}

		private static void SetIdFromAsset(SerializedProperty idProperty, Object asset)
		{
			string path = AssetDatabase.GetAssetPath(asset);
			GUID guid = AssetDatabase.GUIDFromAssetPath(path);
			uint id = (uint)guid.GetHashCode();

			if (!IsValidId(id))
			{
				id = 0;
			}

			if (idProperty.uintValue != id)
			{
				idProperty.uintValue = id;
				idProperty.serializedObject.ApplyModifiedProperties();
			}
		}

		private static Type GetAssetType(Type type)
		{
			for (Type current = type; current != null; current = current.BaseType)
			{
				if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(AssetPicker<>))
				{
					return current.GenericTypeArguments[0];
				}
			}
			return default;
		}
	}
}