using System;
using UnityEngine;

namespace Nest.Addons
{
	public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
	{
		public static T Instance
		{
			get
			{
				if (!Singleton<!0>.Instantiated)
				{
					Singleton<!0>.CreateInstance();
				}
				return Singleton<!0>._instance;
			}
		}

		public static bool Instantiated { get; private set; }

		public static bool Destroyed { get; private set; }

		private static void CreateInstance()
		{
			if (Singleton<!0>.Destroyed)
			{
				return;
			}
			Type typeFromHandle = typeof(!0);
			T[] array = Object.FindObjectsOfType<T>();
			if (array.Length != 0)
			{
				if (array.Length > 1)
				{
					Debug.LogWarning("There is more than one instance of Singleton of type \"" + typeFromHandle + "\". Keeping the first one. Destroying the others.");
					for (int i = 1; i < array.Length; i++)
					{
						Object.Destroy(array[i].gameObject);
					}
				}
				Singleton<!0>._instance = array[0];
				Singleton<!0>._instance.gameObject.SetActive(true);
				Singleton<!0>.Instantiated = true;
				Singleton<!0>.Destroyed = false;
				return;
			}
			Singleton<!0>._instance = new GameObject
			{
				name = typeFromHandle.ToString()
			}.AddComponent<T>();
			Singleton<!0>.Instantiated = true;
			Singleton<!0>.Destroyed = false;
		}

		protected virtual void Awake()
		{
			if (!(Singleton<!0>._instance == null))
			{
				if (this.Persistent)
				{
					Object.DontDestroyOnLoad(base.gameObject);
				}
				if (base.GetInstanceID() != Singleton<!0>._instance.GetInstanceID())
				{
					Object.Destroy(base.gameObject);
				}
				return;
			}
			if (!this.Persistent)
			{
				return;
			}
			Singleton<!0>.CreateInstance();
			Object.DontDestroyOnLoad(base.gameObject);
		}

		private void OnDestroy()
		{
			Singleton<!0>.Destroyed = true;
			Singleton<!0>.Instantiated = false;
		}

		public void Touch()
		{
		}

		private static T _instance;

		public bool Persistent;
	}
}
