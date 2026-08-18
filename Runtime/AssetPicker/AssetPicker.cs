using System;
using UnityEngine;

namespace Kryz.Settings
{
	[Serializable]
	public abstract class AssetPicker<T> where T : UnityEngine.Object
	{
		[SerializeField] uint id;

		public uint Id => id;
	}
}