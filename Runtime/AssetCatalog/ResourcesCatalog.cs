using System;
using System.Collections.Generic;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public class ResourcesCatalog : SingletonScriptableObject<ResourcesCatalog>, ISerializationCallbackReceiver
	{
		[Serializable]
		internal struct Data : IEquatable<Data>
		{
			public uint Id;
			public string Path;

			public readonly bool Equals(Data other) => Id == other.Id && Path.Equals(other.Path, StringComparison.Ordinal);
		}

		[SerializeField, ReadOnly] internal int version;
		[SerializeField, ReadOnly] internal List<Data> assets;

		private readonly Dictionary<uint, string> dict = new();

		public IReadOnlyDictionary<uint, string> Assets => dict;

		public void OnBeforeSerialize()
		{
		}

		public void OnAfterDeserialize()
		{
			dict.Clear();

			foreach (Data item in assets)
			{
				dict[item.Id] = item.Path;
			}
		}
	}
}