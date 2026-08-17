using System;

namespace Kryz.Settings
{
	[Serializable]
	public class SettingsPicker : SettingsPicker<SettingsAsset> { }

	[Serializable]
	public class SettingsPicker<T> : AssetPicker<uint, T> where T : SettingsAsset
	{
		protected override uint GetIdentifier()
		{
			return SharedFunctions.GetSettingsId?.Invoke(asset) ?? Id;
		}

		internal void SetAssetReference(ISettingsDatabase settingsDatabase)
		{
			asset = settingsDatabase.Get<T>(Id);
		}
	}
}