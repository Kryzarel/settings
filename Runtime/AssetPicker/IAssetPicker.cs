namespace Kryz.Settings
{
	public interface IAssetPicker<T> where T : UnityEngine.Object
	{
		public ulong Id { get; }
	}
}