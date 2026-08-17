using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public abstract class SettingsAsset : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField, HideInInspector] string assetName; // Used to serialize a human readable name.
		[SerializeField, ReadOnly] uint id;
		[SerializeField] bool enabled = true;

		public uint Id => id;
		public bool Enabled => enabled;

		public virtual void OnBeforeSerialize()
		{
			if (Application.isEditor)
			{
				assetName = SharedFunctions.GetAssetName?.Invoke(this) ?? assetName;
				id = SharedFunctions.GetSettingsId?.Invoke(this) ?? id;
			}
		}

		public virtual void OnAfterDeserialize() { }
	}
}