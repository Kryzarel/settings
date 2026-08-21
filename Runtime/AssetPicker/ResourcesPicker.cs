using System;
using UnityEngine;

namespace Kryz.Settings
{
	[Serializable]
	public struct ResourcesPicker<T> : IAssetPicker<T> where T : UnityEngine.Object
	{
		[SerializeField] ulong id;

		public readonly ulong Id => id;
	}
}