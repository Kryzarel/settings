using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public class ResourcesCatalog : SingletonScriptableObject<ResourcesCatalog>, ISerializationCallbackReceiver
	{
		[Serializable]
		private struct Data
		{
			public ulong Id;
			public string Path;
		}

		[SerializeField, ReadOnly] internal int version;
		[SerializeField, ReadOnly] List<Data> assetList = new();

		internal readonly Dictionary<ulong, string> assets = new();

		private ReadOnlyDictionary<ulong, string> readOnlyAssets;

		public ReadOnlyDictionary<ulong, string> Assets => readOnlyAssets ??= new(assets);

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			assetList.Clear();

			foreach (KeyValuePair<ulong, string> item in assets)
			{
				assetList.Add(new Data { Id = item.Key, Path = item.Value });
			}
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			assets.Clear();

			foreach (Data item in assetList)
			{
				assets[item.Id] = item.Path;
			}
		}
	}
}