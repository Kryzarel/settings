using System;
using UnityEngine;

namespace Kryz.Settings
{
	[Serializable]
	public class ResourcesPicker<T> : AssetPicker<string, T> where T : UnityEngine.Object
	{
		public override void OnBeforeSerialize()
		{
			if (Application.isEditor && asset != null && SharedFunctions.GetResourcesPath != null)
			{
				string path = SharedFunctions.GetResourcesPath(asset);

				if (string.IsNullOrEmpty(path))
				{
					Debug.LogError($"Asset {asset.name} is not in Resources.", asset);
					asset = null;
				}
			}

			base.OnBeforeSerialize();
		}

		protected override string GetIdentifier()
		{
			return SharedFunctions.GetResourcesPath?.Invoke(asset) ?? Id;
		}
	}
}