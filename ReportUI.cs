using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportUI : MonoBehaviour
{
	private void Awake()
	{
		Object.DontDestroyOnLoad(base.gameObject);
		if (!this.useHardcodedCredentials)
		{
			if (PlayerPrefs.HasKey(Application.productName + "_REPORTUSERNAME"))
			{
				this.inputUsername.text = PlayerPrefs.GetString(Application.productName + "_REPORTUSERNAME");
			}
			if (PlayerPrefs.HasKey(Application.productName + "_REPORTPASSWORD"))
			{
				this.inputPassword.text = PlayerPrefs.GetString(Application.productName + "_REPORTPASSWORD");
			}
			if (PlayerPrefs.HasKey(Application.productName + "_REPORTSMTP"))
			{
				this.inputSmpt.text = PlayerPrefs.GetString(Application.productName + "_REPORTSMTP");
			}
			if (PlayerPrefs.HasKey(Application.productName + "_REPORTRECEIVER"))
			{
				this.inputReceiver.text = PlayerPrefs.GetString(Application.productName + "_REPORTRECEIVER");
			}
		}
		this.audioSource = base.gameObject.AddComponent<AudioSource>();
		this.audioSource.clip = this.screenshotSFX;
		this.canvas.enabled = false;
		this.Tool.gameObject.SetActive(false);
	}

	private void Start()
	{
		Painter painter = this.painter;
		painter._OnDisable = (Action)Delegate.Combine(painter._OnDisable, new Action(this.UpdateUITexture));
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F9))
		{
			this.BroadcastOpenReportTool();
			this.Tool.SetActive(true);
			this.canvas.enabled = true;
		}
		if (Input.GetKeyDown(KeyCode.F8))
		{
			this.TakeScreenshotButton();
		}
	}

	public void ReportIssue()
	{
		List<RawImage> list = this.screenshotSlots.FindAll((RawImage ri) => ri.texture != null);
		List<Texture2D> list2 = new List<Texture2D>();
		foreach (RawImage rawImage in list)
		{
			list2.Add((Texture2D)rawImage.texture);
		}
		try
		{
			string str = "";
			if (this.reporter.SendReport(this.inputUsername.text, this.inputPassword.text, this.inputSmpt.text, this.inputReceiver.text, this.inputTitle.text, this.inputDescription.text, list2, out str))
			{
				this.inputTitle.text = "";
				this.inputDescription.text = "";
				this.RemoveScreenshot(0);
				this.RemoveScreenshot(1);
				this.RemoveScreenshot(2);
				if (!this.useHardcodedCredentials)
				{
					PlayerPrefs.SetString(Application.productName + "_REPORTUSERNAME", this.inputUsername.text);
					PlayerPrefs.SetString(Application.productName + "_REPORTPASSWORD", this.inputPassword.text);
					PlayerPrefs.SetString(Application.productName + "_REPORTRECEIVER", this.inputReceiver.text);
					PlayerPrefs.SetString(Application.productName + "_REPORTSMTP", this.inputSmpt.text);
				}
				this.canvas.enabled = false;
				this.Tool.gameObject.SetActive(false);
				base.StartCoroutine(this.ShowStatus("Report sent!"));
			}
			else
			{
				this.canvas.enabled = false;
				this.Tool.gameObject.SetActive(false);
				base.StartCoroutine(this.ShowStatus("Error: " + str));
			}
		}
		catch (Exception ex)
		{
			Debug.Log(ex.Message);
			base.StartCoroutine(this.ShowStatus("Error: " + ex.Message));
		}
	}

	public void TakeScreenshotButton()
	{
		base.StartCoroutine(this.TakeScreenshotRoutine());
	}

	private IEnumerator ShowStatus(string message)
	{
		this.StatusText.text = message;
		this.StatusText.enabled = true;
		yield return new WaitForSeconds(3.5f);
		this.StatusText.enabled = false;
		yield break;
	}

	public IEnumerator TakeScreenshotRoutine()
	{
		int index = this.screenshotSlots.FindIndex((RawImage ri) => ri.texture == null);
		if (index < 0)
		{
			yield break;
		}
		bool canvasActive = this.canvas.enabled;
		if (canvasActive)
		{
			this.canvas.enabled = false;
		}
		yield return new WaitForEndOfFrame();
		this.screenshotSlots[index].texture = ScreenshotTool.TakeScreenshot();
		this.screenshotSlots[index].color = Color.white;
		this.audioSource.Play();
		if (canvasActive)
		{
			this.canvas.enabled = true;
		}
		yield break;
	}

	public void OpenPainter(int screenshotIndex)
	{
		this._screenshotIndex = screenshotIndex;
		if (this.screenshotSlots[screenshotIndex].texture != null)
		{
			this.painter.enabled = true;
			this.painter.baseTex = (Texture2D)this.screenshotSlots[screenshotIndex].texture;
		}
	}

	public void UpdateUITexture()
	{
		this.screenshotSlots[this._screenshotIndex].texture = this.painter.baseTex;
	}

	public void BroadcastOpenReportTool()
	{
		if (ReportUI.OnOpenReportTool != null)
		{
			ReportUI.OnOpenReportTool();
		}
		ReportUI.REPORTUIACTIVE = true;
	}

	public void BroadcastCloseReportTool()
	{
		ReportUI.REPORTUIACTIVE = false;
	}

	public void RemoveScreenshot(int screenshotIndex)
	{
		this.screenshotSlots[screenshotIndex].texture = null;
		this.screenshotSlots[screenshotIndex].color = new Color(0.06082088f, 0.2481493f, 0.326f, 1f);
	}

	public static Action OnOpenReportTool;

	public static bool REPORTUIACTIVE;

	public bool pauseGameOnActive = true;

	[SerializeField]
	public bool useHardcodedCredentials;

	public Canvas canvas;

	public InputField inputUsername;

	public InputField inputPassword;

	public InputField inputTitle;

	public InputField inputDescription;

	public InputField inputSmpt;

	public InputField inputReceiver;

	public Painter painter;

	public List<RawImage> screenshotSlots;

	public Text StatusText;

	public GameObject Tool;

	[SerializeField]
	private AudioClip screenshotSFX;

	private AudioSource audioSource;

	[SerializeField]
	private EmailReporter reporter;

	private bool paused;

	private int _screenshotIndex;
}
