using System;
using Nest.Util;
using UnityEngine;
using UnityEngine.Events;

namespace Nest.Addons
{
	public class ScenePostbox : MonoBehaviour
	{
		public void RecieveEvent(string message)
		{
			UnityEvent unityEvent;
			if (this.Events.TryGetValue(message, out unityEvent))
			{
				unityEvent.Invoke();
			}
		}

		private void OnEnable()
		{
			Singleton<SceneController>.Instance.RegisterPostbox(this);
		}

		public ScenePostbox.EventDictionary Events;

		[Serializable]
		public class EventDictionary : SerializableDictionaryBase<string, UnityEvent>
		{
		}
	}
}
