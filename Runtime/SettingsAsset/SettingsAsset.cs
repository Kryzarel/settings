using System.Diagnostics;
using Kryz.UnityUtils;
using UnityEngine;

namespace Kryz.Settings
{
	public abstract class SettingsAsset : ScriptableObject
	{
		[SerializeField, ReadOnly] ulong id;
		[SerializeField, ReadOnly] string settingName; // Used to serialize a human readable name.
		[SerializeField] bool enabled = true;

		public ulong Id => id;
		public string Name => settingName;
		public bool Enabled => enabled;

		[Conditional("UNITY_EDITOR")]
		internal void SetIdAndName(ulong id, string name)
		{
			this.id = id;
			settingName = name;
		}
	}
}