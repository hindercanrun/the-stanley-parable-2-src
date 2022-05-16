using System;
using UnityEngine;

namespace Ferr
{
	public abstract class Singleton<T> : MonoBehaviour where T : Component
	{
		public static T Instance
		{
			get
			{
				if (Singleton<!0>._instance == null)
				{
					Singleton<!0>.EnsureInstantiated();
				}
				return Singleton<!0>._instance;
			}
		}

		public static bool Instantiated
		{
			get
			{
				return Singleton<!0>._instance != null;
			}
		}

		public static void EnsureInstantiated()
		{
			if (Singleton<!0>._instance == null)
			{
				Singleton<!0>._instance = Object.FindObjectOfType<T>();
				if (Singleton<!0>._instance == null)
				{
					GameObject gameObject = Resources.Load("Singletons/" + typeof(!0).Name) as GameObject;
					if (gameObject != null)
					{
						Singleton<!0>._instance = Object.Instantiate<GameObject>(gameObject).GetComponent<T>();
					}
				}
				if (Singleton<!0>._instance == null)
				{
					Singleton<!0>._instance = (new GameObject("_" + typeof(!0).Name).AddComponent(typeof(!0)) as !0);
				}
				if (Singleton<!0>._instance == null)
				{
					Debug.LogErrorFormat("Couldn't connect or create singleton {0}!", new object[]
					{
						typeof(!0).Name
					});
				}
			}
		}

		protected virtual void Awake()
		{
			if (Singleton<!0>._instance != null && Singleton<!0>._instance != this)
			{
				Debug.LogErrorFormat(base.gameObject, "Creating a new instance of a singleton [{0}] when one already exists!", new object[]
				{
					typeof(!0).Name
				});
				base.gameObject.SetActive(false);
				return;
			}
			Singleton<!0>._instance = base.GetComponent<T>();
		}

		protected virtual void OnDestroy()
		{
			Singleton<!0>._instance = default(!0);
		}

		private static T _instance;
	}
}
