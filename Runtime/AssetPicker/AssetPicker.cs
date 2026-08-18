using System;
using UnityEngine;

namespace Kryz.Settings
{
	[Serializable]
	public class AssetPicker
	{
		[SerializeField] uint id;

		public uint Id => id;
	}

	[Serializable]
	public class AssetPicker<T> : AssetPicker where T : UnityEngine.Object
	{
	}
}