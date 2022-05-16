using System;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<!0>
{
	public static T Instance
	{
		get
		{
			if (Singleton<!0>._instance == null)
			{
				Singleton<!0>._instance = (!0)((object)Object.FindObjectOfType(typeof(!0)));
			}
			return Singleton<!0>._instance;
		}
	}

	protected virtual void Awake()
	{
		if (Singleton<!0>._instance == null)
		{
			Singleton<!0>._instance = (this as !0);
		}
		if (Singleton<!0>._instance != this as !0)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private static T _instance;
}
