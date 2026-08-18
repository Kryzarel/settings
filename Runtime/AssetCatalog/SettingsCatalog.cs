using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public class SettingsCatalog : SingletonScriptableObject<SettingsCatalog>, ISerializationCallbackReceiver
	{
		[SerializeField, ReadOnly] internal int version;
		[SerializeField, ReadOnly] List<SettingsAsset> assetList = new();

		internal readonly Dictionary<ulong, SettingsAsset> assets = new();

		private ReadOnlyDictionary<ulong, SettingsAsset> readOnlyAssets;

		public IReadOnlyDictionary<ulong, SettingsAsset> Assets => readOnlyAssets ??= new(assets);

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			assetList.Clear();

			foreach (SettingsAsset item in assets.Values)
			{
				assetList.Add(item);
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			assets.Clear();

			foreach (SettingsAsset item in assetList)
			{
				assets[item.Id] = item;
			}
		}
	}
}