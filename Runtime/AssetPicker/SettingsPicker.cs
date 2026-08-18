using System;

namespace Kryz.Settings
{
	[Serializable]
	public class SettingsPicker<T> : AssetPicker<T> where T : SettingsAsset
	{
	}
}