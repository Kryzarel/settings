using UnityEngine;

namespace Kryz.Settings
{
	public abstract class SingletonScriptableObject<T> : ScriptableObject where T : SingletonScriptableObject<T>
	{
		private static T instance;

		public static T Instance
		{
			get
			{
				if (instance == null)
				{
					instance = Resources.Load<T>("Kryz.Singleton/" + typeof(T).Name);

					if (instance == null)
					{
						Debug.LogError($"{"Kryz.Singleton/" + typeof(T).Name} not found in Resources.");
					}
				}
				return instance;
			}
		}

		public static void LoadAsync()
		{
			Resources.LoadAsync<T>("Kryz.Singleton/" + typeof(T).Name);
		}
	}
}