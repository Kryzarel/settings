using System;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	[Serializable]
	public abstract class AssetPicker<TIdentifier, TAsset> : ISerializationCallbackReceiver where TAsset : UnityEngine.Object
	{
		[SerializeField] protected TAsset asset;
		[SerializeField, ReadOnly] TIdentifier id;

		public TIdentifier Id => id;

		public virtual void OnBeforeSerialize()
		{
			if (Application.isEditor)
			{
				id = GetIdentifier();
			}
		}

		public virtual void OnAfterDeserialize()
		{
			if (!Application.isEditor && Application.isPlaying)
			{
				asset = null;
			}
		}

		protected abstract TIdentifier GetIdentifier();
	}
}