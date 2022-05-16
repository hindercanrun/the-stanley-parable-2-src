using System;
using TMPro;
using UnityEngine;

public class PlayerPrefsTest : MonoBehaviour
{
	private void Start()
	{
		if (PlatformPlayerPrefs.HasKey(this.textKey))
		{
			this.text = PlatformPlayerPrefs.GetString(this.textKey);
			this.testText.text = this.text;
		}
	}

	public void ChangeTextUp()
	{
		this.text = "Pressed up";
		PlatformPlayerPrefs.SetString(this.textKey, this.text);
		this.testText.text = this.text;
	}

	public void ChangeTextDown()
	{
		this.text = "Pressed down";
		PlatformPlayerPrefs.SetString(this.textKey, this.text);
		this.testText.text = this.text;
	}

	public void DeleteTextPref()
	{
		this.text = "Text was deleted";
		PlatformPlayerPrefs.DeleteKey(this.textKey);
		this.testText.text = this.text;
	}

	public void Update()
	{
		if (Singleton<GameMaster>.Instance.stanleyActions.MoveForward.WasPressed)
		{
			this.ChangeTextUp();
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.MoveBackward.WasPressed)
		{
			this.ChangeTextDown();
		}
		if (Singleton<GameMaster>.Instance.stanleyActions.MoveRight.WasPressed)
		{
			this.DeleteTextPref();
		}
	}

	private string textKey = "text";

	private string text;

	[SerializeField]
	private TextMeshProUGUI testText;
}
