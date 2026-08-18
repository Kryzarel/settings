using UnityEditor;

namespace Kryz.Settings.Editor
{
	public static class SharedFunctions
	{
		public static uint GetAssetId32(this GUID guid)
		{
			return FNVHash.UInt32.Hash(guid);
		}

		public static ulong GetAssetId64(this GUID guid)
		{
			return FNVHash.UInt64.Hash(guid);
		}
	}
}