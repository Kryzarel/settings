using System;
using System.IO;
using System.Text;
using Kryz.Settings;
using Kryz.Settings.Editor;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsEditorWindow : EditorWindow
{
	private static class Data
	{
		public static string DefaultExportPath;
		public static string ExportPathKey;

		public static string CreateMissingSettingsKey;

		public static string DefaultAssetCreationPath;
		public static string AssetCreationPathKey;

		private static SettingsCatalog _settingsCatalog;
		public static SettingsCatalog SettingsCatalog => _settingsCatalog = _settingsCatalog != null ? _settingsCatalog : SingletonScriptableObjectUtils.Get<SettingsCatalog>();

		static Data()
		{
			DefaultExportPath ??= Path.GetFullPath("Settings Export/Settings.txt").Replace('\\', '/');
			ExportPathKey ??= PlayerSettings.productGUID + ":SettingsExportPath";

			CreateMissingSettingsKey ??= PlayerSettings.productGUID + ":CreateMissingSettings";

			DefaultAssetCreationPath ??= "Settings";
			AssetCreationPathKey ??= PlayerSettings.productGUID + ":AssetCreationPath";
		}
	}

	[MenuItem("Window/Kryzarel/Settings/Settings Exporter")]
	public static void ShowWindow()
	{
		SettingsEditorWindow window = GetWindow<SettingsEditorWindow>();
		window.titleContent = new GUIContent("Kryz Settings Exporter");
	}

	public void CreateGUI()
	{
		VisualElement root = rootVisualElement;

		Label header = new("Settings Exporter");
		header.style.unityFontStyleAndWeight = FontStyle.Bold;
		header.style.fontSize = 20;
		SetAllMargins(header, 4);
		root.Add(header);

		Label exportLabel = new("Export");
		exportLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
		exportLabel.style.fontSize = 16;
		SetAllMargins(exportLabel, 4);
		root.Add(exportLabel);

		TextField exportPathField = new("Settings Export Path");
		exportPathField.labelElement.style.minWidth = 150;
		exportPathField.labelElement.style.maxWidth = 150;
		exportPathField.RegisterValueChangedCallback(evt =>
		{
			bool isDefault = evt.newValue.Equals(Data.DefaultExportPath, StringComparison.Ordinal);
			EditorPrefs.SetString(Data.ExportPathKey, isDefault ? null : evt.newValue);
		});
		exportPathField.value = EditorPrefs.GetString(Data.ExportPathKey, Data.DefaultExportPath);
		root.Add(exportPathField);

		root.Add(new Button(ExportToFile) { text = "Export To File" });
		root.Add(new Button(ExportToClipboard) { text = "Export To Clipboard" });
		root.Add(new Button(OpenExportFolder) { text = "Open Export Folder" });

		VisualElement space = new();
		space.style.minHeight = 10;
		space.style.maxHeight = 10;
		root.Add(space);

		Label settingsCatalogLabel = new("Settings Catalog");
		settingsCatalogLabel.style.unityFontStyleAndWeight = FontStyle.Bold;
		settingsCatalogLabel.style.fontSize = 16;
		SetAllMargins(settingsCatalogLabel, 4);
		root.Add(settingsCatalogLabel);

		ObjectField catalogField = new();
		catalogField.SetEnabled(false);
		catalogField.labelElement.style.minWidth = 150;
		catalogField.labelElement.style.maxWidth = 150;
		catalogField.value = Data.SettingsCatalog;
		catalogField.label = ObjectNames.NicifyVariableName(nameof(SettingsCatalog));
		root.Add(catalogField);

		InspectorElement inspectorElement = new(Data.SettingsCatalog);
		root.Add(inspectorElement);
	}

	private static void SetAllMargins(VisualElement element, float value)
	{
		element.style.marginLeft = value;
		element.style.marginRight = value;
		element.style.marginTop = value;
		element.style.marginBottom = value;
	}

	public static void ExportToFile()
	{
		try
		{
			string path = EditorPrefs.GetString(Data.ExportPathKey, Data.DefaultExportPath);
			string directory = Path.GetDirectoryName(path);
			if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

			using StreamWriter writer = new(path);
			SettingsSerializer.Serialize(Data.SettingsCatalog.Assets.Values, writer);

			Debug.Log("Settings exported to \"" + path + "\"");
		}
		catch (Exception e)
		{
			Debug.LogError(e.Message);
		}
	}

	public static void ExportToClipboard()
	{
		StringBuilder sb = new();

		using StringWriter writer = new(sb);
		SettingsSerializer.Serialize(Data.SettingsCatalog.Assets.Values, writer);

		string settings = sb.ToString();
		EditorGUIUtility.systemCopyBuffer = settings;

		Debug.Log("Settings exported to clipboard:\n" + settings);
	}

	public static void OpenExportFolder()
	{
		string path = EditorPrefs.GetString(Data.ExportPathKey, Data.DefaultExportPath);
		string directory = Path.GetDirectoryName(path);
		if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

		EditorUtility.OpenWithDefaultApp(directory);
	}
}