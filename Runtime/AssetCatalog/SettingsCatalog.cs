using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public class SettingsCatalog : SingletonScriptableObject<SettingsCatalog>, ISerializationCallbackReceiver, ISettingsStore
	{
		[Serializable]
		private struct Data
		{
			public ulong Id;
			public SettingsAsset Asset;
		}

		[SerializeField, ReadOnly, HideInInspector] internal int version;
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

		public void Add(ulong id, SettingsAsset asset) => assets.Add(id, asset);
		public bool Remove(ulong id) => assets.Remove(id);
		public bool TryGetValue(ulong id, out SettingsAsset asset) => assets.TryGetValue(id, out asset);
	}
}