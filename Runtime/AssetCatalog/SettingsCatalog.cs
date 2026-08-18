using System.Collections.Generic;
using UnityEngine;

namespace Kryz.Settings
{
	public class SettingsCatalog : SingletonScriptableObject<SettingsCatalog>
	{
		[SerializeField] internal List<SettingsAsset> assets;

		private readonly Dictionary<uint, SettingsAsset> dict;

		public IReadOnlyDictionary<uint, SettingsAsset> Assets => dict;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			dict.Clear();

			foreach (SettingsAsset item in assets)
			{
				dict[item.Id] = item;
			}
		}
	}
}