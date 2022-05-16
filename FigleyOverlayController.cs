using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class FigleyOverlayController : MonoBehaviour
{
	private bool CameraAndCanvasEnabled
	{
		set
		{
			this.figleyCamera.enabled = value;
			this.figleyCanvas.enabled = value;
		}
	}

	private bool UseBlurredBackground
	{
		get
		{
			return false;
		}
	}

	public static FigleyOverlayController Instance { get; private set; }

	public int FiglysFound
	{
		get
		{
			int num = 0;
			IntConfigurable[] array = this.figleyCountCounfigurable;
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].GetIntValue() != -1)
				{
					num++;
				}
			}
			return num;
		}
	}

	public int PostFiglysFound
	{
		get
		{
			int num = 0;
			StringConfigurable[] array = this.figleyPostCollectionCountConfigurables;
			for (int i = 0; i < array.Length; i++)
			{
				char[] postCollectableCharArray = FigleyCollectable.GetPostCollectableCharArray(array[i], 0);
				for (int j = 0; j < postCollectableCharArray.Length; j++)
				{
					if (postCollectableCharArray[j] == FigleyCollectable.CollectedChar)
					{
						num++;
					}
				}
			}
			return num;
		}
	}

	private void Awake()
	{
		FigleyOverlayController.Instance = this;
		this.CameraAndCanvasEnabled = false;
		IntConfigurable[] array = this.figleyCountCounfigurable;
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Init();
		}
		this.figleyInitiatorFoundCounfigurable.Init();
		StringConfigurable[] array2 = this.figleyPostCollectionCountConfigurables;
		for (int i = 0; i < array2.Length; i++)
		{
			array2[i].Init();
		}
	}

	private void Start()
	{
		GameMaster.OnPrepareLoadingLevel += this.ForceStop;
	}

	private void ForceStop()
	{
		base.StopAllCoroutines();
		this.FigleyHideFinished();
	}

	public void StartFigleyCollectionRoutine()
	{
		this.simpleRotate.enabled = true;
		this.blurredBackgroundImage.enabled = false;
		this.count = this.FiglysFound;
		this.count %= 6;
		this.ShowCount(this.count);
		this.collectionNarrations[this.count].Input_Start();
		StanleyController.Instance.viewFrozen = true;
		StanleyController.Instance.motionFrozen = true;
		StanleyController.Instance.ResetVelocity();
		base.StartCoroutine(this.SetAndBlurBackground(this.UseBlurredBackground));
	}

	public void StartFigleyPostCollection()
	{
		this.simpleRotate.ResetToStartRotation();
		this.CameraAndCanvasEnabled = true;
		this.blurredBackgroundImage.enabled = false;
		this.count = 6 + this.PostFiglysFound;
		this.ShowCount(this.count);
		this.pickupSFX.Play();
		this.figleyAnimator.SetTrigger("Instant Reveal");
	}

	private IEnumerator SetAndBlurBackground(bool useBlurredBackground)
	{
		yield return null;
		this.figleyRotation.ResetToStartRotation();
		this.figleyAnimator.SetTrigger("Reveal");
		if (useBlurredBackground)
		{
			yield return new WaitForEndOfFrame();
			this.renderTexture = new RenderTexture(Screen.width, Screen.height, 0);
			ScreenCapture.CaptureScreenshotIntoRenderTexture(this.renderTexture);
			AsyncGPUReadback.Request(this.renderTexture, 0, TextureFormat.RGB24, new Action<AsyncGPUReadbackRequest>(this.ReadbackCompleted));
		}
		else
		{
			this.CameraAndCanvasEnabled = true;
			this.blurredBackgroundImage.enabled = true;
		}
		yield break;
	}

	private void ReadbackCompleted(AsyncGPUReadbackRequest request)
	{
		this.bgMaterial.mainTexture = this.renderTexture;
		base.StartCoroutine(this.BlurBackground());
	}

	private IEnumerator BlurBackground()
	{
		this.CameraAndCanvasEnabled = true;
		this.blurredBackgroundImage.enabled = true;
		float startTime = Time.realtimeSinceStartup;
		float endTime = startTime + 2.5f;
		while (Time.realtimeSinceStartup < endTime)
		{
			float num = Mathf.InverseLerp(startTime, endTime, Time.realtimeSinceStartup);
			num += 0.2f;
			num /= 1.2f;
			this.bgMaterial.SetFloat("_Strength", num * 10f);
			yield return null;
		}
		yield break;
	}

	[ContextMenu("Complete Narration")]
	public void NarrationComplete()
	{
		this.figleyAnimator.SetTrigger("Hide");
	}

	private void ShowCount(int c)
	{
		this.countUI.text = c + "/6";
	}

	public void FigleyPop()
	{
		this.ShowCount(this.count + 1);
	}

	public void FigleySFX()
	{
		this.obnoxiousSFX.Play();
	}

	public void ImpactSFX()
	{
		this.impactSFX.Play();
	}

	public void SwishSFX()
	{
		this.swishSFX.Play();
	}

	public void PickupSmallSFX()
	{
		this.pickupSFX.Play();
	}

	public void FigleyDisableCamera()
	{
		this.CameraAndCanvasEnabled = false;
	}

	public void FigleyHideFinished()
	{
		StanleyController.Instance.viewFrozen = false;
		StanleyController.Instance.motionFrozen = false;
		this.CameraAndCanvasEnabled = false;
	}

	public void FigleyInstantHideFinished()
	{
		this.CameraAndCanvasEnabled = false;
	}

	[InspectorButton("StartFigleyCollectionRoutine", null)]
	[SerializeField]
	private Animator figleyAnimator;

	[SerializeField]
	private RawImage blurredBackgroundImage;

	[InspectorButton("ForceStop", null)]
	[SerializeField]
	private Camera figleyCamera;

	[SerializeField]
	private TextMeshProUGUI countUI;

	[SerializeField]
	private ChoreographedScene[] collectionNarrations;

	[SerializeField]
	private IntConfigurable[] figleyCountCounfigurable;

	[SerializeField]
	private StringConfigurable[] figleyPostCollectionCountConfigurables;

	[InspectorButton("StartFigleyPostCollection", null)]
	[SerializeField]
	private BooleanConfigurable figleyInitiatorFoundCounfigurable;

	[SerializeField]
	private AudioSource pickupSFX;

	[SerializeField]
	private AudioSource obnoxiousSFX;

	[SerializeField]
	private AudioSource impactSFX;

	[SerializeField]
	private AudioSource swishSFX;

	[SerializeField]
	private SimpleRotate simpleRotate;

	private RenderTexture renderTexture;

	[SerializeField]
	private Material bgMaterial;

	[SerializeField]
	private SimpleRotate figleyRotation;

	[SerializeField]
	private Canvas figleyCanvas;

	private int count;
}
