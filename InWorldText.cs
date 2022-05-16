using System;
using System.Collections;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class InWorldText : MonoBehaviour
{
	public void InitTextLabel(InWorldTextualObject referencedObject)
	{
		string text = LocalizationManager.GetTranslation(referencedObject.localizationTerm, true, 0, true, false, null, null);
		if (text == null)
		{
			text = referencedObject.localizationTerm + "\n<size=14><localization not found></size>";
		}
		this.textMeshPro.text = text;
		this.textMeshPro.color = referencedObject.labelColor;
		this.canvasGroup.alpha = 0f;
		UnityEvent onUpdateText = this.OnUpdateText;
		if (onUpdateText == null)
		{
			return;
		}
		onUpdateText.Invoke();
	}

	public void ForceUpdateText()
	{
		base.StartCoroutine(this.WaitOneFrameAndUpdate());
	}

	private IEnumerator WaitOneFrameAndUpdate()
	{
		yield return null;
		UnityEvent onUpdateText = this.OnUpdateText;
		if (onUpdateText != null)
		{
			onUpdateText.Invoke();
		}
		yield break;
	}

	public void UpdateTextLabel(InWorldTextualObject referencedObject, float alpha, float fadeTime)
	{
		this.textMeshPro.horizontalAlignment = referencedObject.horizontalAlignment;
		this.canvasGroup.alpha = Mathf.MoveTowards(this.canvasGroup.alpha, alpha, Time.smoothDeltaTime / fadeTime);
	}

	public TextMeshProUGUI textMeshPro;

	public CanvasGroup canvasGroup;

	public UnityEvent OnUpdateText;
}
