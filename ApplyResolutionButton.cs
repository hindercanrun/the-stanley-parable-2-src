using System;
using System.Collections;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ApplyResolutionButton : MonoBehaviour
{
	private void Awake()
	{
		this.textLocalization = this.text.GetComponent<Localize>();
		this.textLocalization2 = this.text2.GetComponent<Localize>();
	}

	private void OnEnable()
	{
		this.ConfirmResolution();
	}

	private void OnDisable()
	{
		base.StopAllCoroutines();
		this.inConfirmStage = false;
	}

	public void PressedButton()
	{
		if (this.inConfirmStage)
		{
			this.ConfirmResolution();
			return;
		}
		this.ApplySelectedResolutionAndFullScreenMode();
	}

	private void ApplySelectedResolutionAndFullScreenMode()
	{
		FullScreenMode selectedFullScreenMode = ResolutionController.Instance.SelectedFullScreenMode;
		FullScreenMode currentFullScreenMode = ResolutionController.Instance.CurrentFullScreenMode;
		int selectedResolutionIndex = ResolutionController.Instance.SelectedResolutionIndex;
		int currentResolutionIndex = ResolutionController.Instance.CurrentResolutionIndex;
		ResolutionController.Instance.ApplyResolutionAtIndex(selectedResolutionIndex, selectedFullScreenMode);
		this.oldResolutionIndexDEBUG = currentResolutionIndex;
		this.oldFullscreenModeDEBUG = currentFullScreenMode;
		base.StartCoroutine(this.StartCountdown(currentResolutionIndex, currentFullScreenMode));
	}

	private IEnumerator StartCountdown(int oldResolutionIndex, FullScreenMode oldFullscreenModeIndex)
	{
		this.textLocalization.enabled = false;
		this.textLocalization2.enabled = false;
		this.inConfirmStage = true;
		int num;
		for (int i = 15; i >= 0; i = num - 1)
		{
			this.text.text = LocalizationManager.GetTranslation(this.keepChangesLocalizationTerm, true, 0, true, false, null, null).Replace("%!C!%", string.Format("{0}", i));
			this.text2.text = this.text.text;
			base.StartCoroutine(this.RefreshCanvasesAndText());
			yield return new WaitForSecondsRealtime(1f);
			num = i;
		}
		this.inConfirmStage = false;
		this.RevertResolutionAndFSMode(oldResolutionIndex, oldFullscreenModeIndex);
		yield break;
	}

	private void RevertResolutionAndFSMode(int oldResolutionIndex, FullScreenMode oldFullscreenModeIndex)
	{
		ResolutionController.Instance.ApplyResolutionAtIndex(oldResolutionIndex, oldFullscreenModeIndex);
		this.ResetLabel();
	}

	private void ConfirmResolution()
	{
		base.StopAllCoroutines();
		this.inConfirmStage = false;
		this.ResetLabel();
	}

	private void ResetLabel()
	{
		if (this.textLocalization != null)
		{
			this.textLocalization.enabled = true;
			this.textLocalization.OnLocalize(true);
			this.textLocalization2.enabled = true;
			this.textLocalization2.OnLocalize(true);
			base.StartCoroutine(this.RefreshCanvasesAndText());
		}
	}

	private IEnumerator RefreshCanvasesAndText()
	{
		Canvas.ForceUpdateCanvases();
		this.text.GetComponent<ContentSizeFitter>().enabled = false;
		this.text.GetComponent<ContentSizeFitter>().enabled = true;
		this.text2.GetComponent<ContentSizeFitter>().enabled = false;
		this.text2.GetComponent<ContentSizeFitter>().enabled = true;
		yield return null;
		Canvas.ForceUpdateCanvases();
		this.text.GetComponent<ContentSizeFitter>().enabled = false;
		this.text.GetComponent<ContentSizeFitter>().enabled = true;
		this.text2.GetComponent<ContentSizeFitter>().enabled = false;
		this.text2.GetComponent<ContentSizeFitter>().enabled = true;
		yield break;
	}

	[SerializeField]
	private ResolutionConfigurator resolutionConfigurator;

	[SerializeField]
	private FullscreenModeConfigurator fullscreenConfigurator;

	[SerializeField]
	private TextMeshProUGUI text;

	[SerializeField]
	private TextMeshProUGUI text2;

	private Localize textLocalization;

	private Localize textLocalization2;

	[SerializeField]
	private string keepChangesLocalizationTerm = "Menu_KeepResolution";

	private int oldResolutionIndexDEBUG;

	private FullScreenMode oldFullscreenModeDEBUG;

	private bool inConfirmStage;
}
