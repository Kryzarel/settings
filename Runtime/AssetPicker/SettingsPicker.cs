using System;
using UnityEngine;

namespace Kryz.Settings
{
	[Serializable]
	public struct SettingsPicker<T> : IAssetPicker<T> where T : SettingsAsset
	{
		[SerializeField] ulong id;

		public readonly ulong Id => id;
	}
}