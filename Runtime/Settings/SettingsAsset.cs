using System.Diagnostics;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public abstract class SettingsAsset : ScriptableObject, ISerializationCallbackReceiver
	{
		[SerializeField, ReadOnly] ulong id;
		[SerializeField, ReadOnly] string settingName; // Used to serialize a human readable name.
		[SerializeField, ReadOnly] string type; // Type metadata for deserialization
		[SerializeField] bool enabled = true;

		public ulong Id => id;
		public string Name => settingName;
		public bool Enabled => enabled;

		public void OnBeforeSerialize()
		{
			type = GetType().AssemblyQualifiedName;
		}

		public void OnAfterDeserialize()
		{
		}

		[Conditional("UNITY_EDITOR")]
		internal void SetIdAndName(ulong id, string name)
		{
			this.id = id;
			settingName = name;
		}
	}
}