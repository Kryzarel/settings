using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Kryz.Settings.Editor
{
	[CustomPropertyDrawer(typeof(AssetPicker<>), useForChildren: true)]
	public class AssetPickerDrawer : PropertyDrawer
	{
		private const string resourcesWarning = "<b>Warning:</b> The assigned object is not in " + nameof(ResourcesCatalog) + " nor " + nameof(SettingsCatalog) + ".";

		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			VisualElement root = new();

			Type pickerType = property.boxedValue.GetType();

			ObjectField objectField = new(property.displayName)
			{
				objectType = GetAssetType(pickerType),
				allowSceneObjects = false
			};
			objectField.AddToClassList("unity-base-field__aligned");
			root.Add(objectField);

			HelpBox warningBox = new(resourcesWarning, HelpBoxMessageType.Warning);
			warningBox.style.display = DisplayStyle.None;
			root.Add(warningBox);

			SerializedProperty idProperty = property.FindPropertyRelative("id");

			void Refresh(SerializedProperty idProperty)
			{
				uint id = idProperty.uintValue;
				Object asset = GetAssetFromId(id, objectField.objectType);
				objectField.SetValueWithoutNotify(asset);
				warningBox.style.display = IsValidId(id) ? DisplayStyle.None : DisplayStyle.Flex;
			}

			Refresh(idProperty);

			objectField.TrackPropertyValue(idProperty, Refresh);

			objectField.RegisterValueChangedCallback(evt =>
			{
				idProperty.serializedObject.Update();
				idProperty.uintValue = GetIdFromAsset(evt.newValue);
				idProperty.serializedObject.ApplyModifiedProperties();
			});
			return root;
		}

		private static bool IsValidId(uint id)
		{
			return ResourcesCatalog.Instance.Assets.ContainsKey(id) || SettingsCatalog.Instance.Assets.ContainsKey(id);
		}

		private static Object GetAssetFromId(uint id, Type type)
		{
			if (ResourcesCatalog.Instance.Assets.TryGetValue(id, out string resourcesPath))
				return Resources.Load(resourcesPath);

			if (EditorSettingsManager.Settings.TryGetValue(id, out SettingsAsset asset))
				return asset;

			GUID[] guids = AssetDatabase.FindAssetGUIDs("t:" + type.Name);
			foreach (GUID guid in guids)
			{
				if (id == (uint)guid.GetHashCode())
				{
					return AssetDatabase.LoadAssetByGUID(guid, type);
				}
			}

			return null;
		}

		private static uint GetIdFromAsset(Object asset)
		{
			if (asset == null)
				return 0;

			string path = AssetDatabase.GetAssetPath(asset);
			GUID guid = AssetDatabase.GUIDFromAssetPath(path);
			uint id = (uint)guid.GetHashCode();
			return id;
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