using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Kryz.Settings.Editor
{
	[CustomPropertyDrawer(typeof(IAssetPicker<>), useForChildren: true)]
	public class AssetPickerDrawer : PropertyDrawer
	{
		private const string warning = "<b>Warning:</b> The assigned object is not in " + nameof(ResourcesCatalog) + " nor " + nameof(SettingsCatalog) + ".";

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

			HelpBox warningBox = new(warning, HelpBoxMessageType.Warning);
			warningBox.style.display = DisplayStyle.None;
			root.Add(warningBox);

			SerializedProperty idProperty = property.FindPropertyRelative("id");

			void Refresh(SerializedProperty idProperty)
			{
				ulong id = idProperty.ulongValue;
				Object asset = GetAssetFromId(id, objectField.objectType);
				objectField.SetValueWithoutNotify(asset);
				warningBox.style.display = id == 0 || IsValidId(id) ? DisplayStyle.None : DisplayStyle.Flex;
			}

			Refresh(idProperty);

			objectField.TrackPropertyValue(idProperty, Refresh);

			objectField.RegisterValueChangedCallback(evt =>
			{
				idProperty.serializedObject.Update();
				idProperty.ulongValue = GetIdFromAsset(evt.newValue);
				idProperty.serializedObject.ApplyModifiedProperties();
			});

			ContextualMenuManipulator contextualMenuManipulator = new(evt => ShowContextualMenu(evt, idProperty));
			root.AddManipulator(contextualMenuManipulator);
			return root;
		}

		private void ShowContextualMenu(ContextualMenuPopulateEvent evt, SerializedProperty property)
		{
			evt.menu.AppendAction("Copy", _ => CopyProperty(property));
			evt.menu.AppendAction("Paste", _ => PasteProperty(property), CanPasteProperty);
		}

		private void CopyProperty(SerializedProperty property)
		{
			EditorGUIUtility.systemCopyBuffer = property.ulongValue.ToString();
		}

		private void PasteProperty(SerializedProperty property)
		{
			if (ulong.TryParse(EditorGUIUtility.systemCopyBuffer, out ulong value) && IsValidId(value))
			{
				property.ulongValue = value;
				property.serializedObject.ApplyModifiedProperties();
			}
		}

		private DropdownMenuAction.Status CanPasteProperty(DropdownMenuAction action)
		{
			if (ulong.TryParse(EditorGUIUtility.systemCopyBuffer, out ulong value) && IsValidId(value))
			{
				return DropdownMenuAction.Status.Normal;
			}
			return DropdownMenuAction.Status.Disabled;
		}

		private static bool IsValidId(ulong id)
		{
			return ResourcesCatalog.Instance.Assets.ContainsKey(id) || SettingsCatalog.Instance.Assets.ContainsKey(id);
		}

		private static Object GetAssetFromId(ulong id, Type type)
		{
			if (ResourcesCatalog.Instance.Assets.TryGetValue(id, out string resourcesPath))
				return Resources.Load(resourcesPath);

			if (SettingsCatalog.Instance.Assets.TryGetValue(id, out SettingsAsset asset))
				return asset;

			foreach (GUID guid in AssetDatabase.FindAssetGUIDs("t:" + type.Name))
			{
				if (id == guid.GetAssetId64())
				{
					return AssetDatabase.LoadAssetByGUID(guid, type);
				}
			}
			return null;
		}

		private static ulong GetIdFromAsset(Object asset)
		{
			if (asset == null)
				return 0;

			string path = AssetDatabase.GetAssetPath(asset);
			GUID guid = AssetDatabase.GUIDFromAssetPath(path);
			ulong id = guid.GetAssetId64();
			return id;
		}

		private static Type GetAssetType(Type type)
		{
			for (Type current = type; current != null; current = current.BaseType)
			{
				if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(IAssetPicker<>))
				{
					return current.GenericTypeArguments[0];
				}

				foreach (Type interfaceType in type.GetInterfaces())
				{
					if (interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IAssetPicker<>))
					{
						return interfaceType.GenericTypeArguments[0];
					}
				}
			}
			return default;
		}
	}
}