using System;
using System.Collections;
using UnityEngine;

namespace Ferr
{
	public class WebMessager : MonoBehaviour
	{
		public static WebMessager Instance
		{
			get
			{
				if (WebMessager.mInstance == null)
				{
					WebMessager.mInstance = new GameObject("_WebMessager").AddComponent<WebMessager>();
				}
				return WebMessager.mInstance;
			}
		}

		public event Action OnAllMessagesComplete;

		private void Start()
		{
			Object.DontDestroyOnLoad(base.gameObject);
		}

		public void Post(string aTo, byte[] aData, Action<WWW> aCallback, Action<WWW> aOnError)
		{
			WWWForm wwwform = new WWWForm();
			wwwform.AddBinaryData("body", aData);
			base.StartCoroutine(this.Send(aTo, wwwform, aCallback, aOnError));
		}

		public void Post(string aTo, string aData, Action<WWW> aCallback, Action<WWW> aOnError)
		{
			byte[] array = new byte[aData.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = (byte)aData[i];
			}
			this.Post(aTo, array, aCallback, aOnError);
		}

		public void PostForm(string aTo, WWWForm aForm, Action<WWW> aCallback, Action<WWW> aOnError)
		{
			base.StartCoroutine(this.Send(aTo, aForm, aCallback, aOnError));
		}

		public void GetText(string aTo, Action<string> aCallback, Action<WWW> aOnError)
		{
			base.StartCoroutine(this.Send(aTo, aCallback, aOnError));
		}

		public void GetRaw(string aTo, Action<WWW> aCallback, Action<WWW> aOnError)
		{
			base.StartCoroutine(this.Send(aTo, aCallback, aOnError));
		}

		public void GetImage(string aTo, Action<Texture> aCallback, Action<WWW> aOnError)
		{
			base.StartCoroutine(this.Send(aTo, aCallback, aOnError));
		}

		private IEnumerator Send(string aTo, Action<WWW> aCallback, Action<WWW> aOnError)
		{
			this.BeginMessage();
			WWW www = new WWW(aTo);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && aCallback != null)
			{
				aCallback(www);
			}
			else if (!string.IsNullOrEmpty(www.error) && aOnError != null)
			{
				aOnError(www);
			}
			this.FinishMessage();
			yield break;
		}

		private IEnumerator Send(string aTo, Action<string> aCallback, Action<WWW> aOnError)
		{
			this.BeginMessage();
			WWW www = new WWW(aTo);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && aCallback != null)
			{
				aCallback(www.text);
			}
			else if (!string.IsNullOrEmpty(www.error) && aOnError != null)
			{
				aOnError(www);
			}
			this.FinishMessage();
			yield break;
		}

		private IEnumerator Send(string aTo, Action<Texture> aCallback, Action<WWW> aOnError)
		{
			this.BeginMessage();
			WWW www = new WWW(aTo);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && aCallback != null)
			{
				aCallback(www.texture);
			}
			else if (!string.IsNullOrEmpty(www.error) && aOnError != null)
			{
				aOnError(www);
			}
			this.FinishMessage();
			yield break;
		}

		private IEnumerator Send(string aTo, WWWForm aForm, Action<WWW> aCallback, Action<WWW> aOnError)
		{
			this.BeginMessage();
			WWW www = new WWW(aTo, aForm);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && aCallback != null)
			{
				aCallback(www);
			}
			else if (!string.IsNullOrEmpty(www.error) && aOnError != null)
			{
				aOnError(www);
			}
			this.FinishMessage();
			yield break;
		}

		private IEnumerator Send(string aTo, byte[] aData, Action<WWW> aCallback, Action<WWW> aOnError)
		{
			this.BeginMessage();
			WWW www = new WWW(aTo, aData);
			yield return www;
			if (string.IsNullOrEmpty(www.error) && aCallback != null)
			{
				aCallback(www);
			}
			else if (!string.IsNullOrEmpty(www.error) && aOnError != null)
			{
				aOnError(www);
			}
			this.FinishMessage();
			yield break;
		}

		public int GetActive()
		{
			return this.activeMessages;
		}

		private void BeginMessage()
		{
			this.activeMessages++;
		}

		private void FinishMessage()
		{
			this.activeMessages--;
			if (this.activeMessages == 0 && this.OnAllMessagesComplete != null)
			{
				this.OnAllMessagesComplete();
			}
		}

		private static WebMessager mInstance;

		private int activeMessages;
	}
}
