using System;
using UnityEngine;

namespace Kryz.Settings
{
	[Serializable]
	public abstract class AssetPicker<T> where T : UnityEngine.Object
	{
		[SerializeField] ulong id;

		public ulong Id => id;
	}
}