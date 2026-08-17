using System;
using Object = UnityEngine.Object;

namespace Kryz.Settings
{
	internal class SharedFunctions
	{
		internal static Func<SettingsAsset, uint> GetSettingsId;
		internal static Func<Object, string> GetResourcesPath;
		internal static Func<Object, string> GetAssetName;
	}
}