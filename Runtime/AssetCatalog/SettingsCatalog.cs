using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public class SettingsCatalog : SingletonScriptableObject<SettingsCatalog>, ISerializationCallbackReceiver
	{
		[Serializable]
		private struct Data
		{
			public ulong Id;
			public SettingsAsset Asset;
		}

		[SerializeField, ReadOnly] internal int version;
		[SerializeField, ReadOnly] List<Data> assetList = new();

		internal readonly Dictionary<ulong, SettingsAsset> assets = new();

		private ReadOnlyDictionary<ulong, SettingsAsset> readOnlyAssets;

		public ReadOnlyDictionary<ulong, SettingsAsset> Assets => readOnlyAssets ??= new(assets);

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			assetList.Clear();

			foreach (KeyValuePair<ulong, SettingsAsset> item in assets)
			{
				assetList.Add(new Data { Id = item.Key, Asset = item.Value });
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			assets.Clear();

			foreach (Data item in assetList)
			{
				assets[item.Id] = item.Asset;
			}
		}
	}
}