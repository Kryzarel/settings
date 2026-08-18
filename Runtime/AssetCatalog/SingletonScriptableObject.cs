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
					instance = Resources.LoadAll<T>("")?[0];

					if (instance == null)
					{
						Debug.LogError($"{typeof(T).Name} not found in Resources.");
					}
				}
				return instance;
			}
		}
	}
}