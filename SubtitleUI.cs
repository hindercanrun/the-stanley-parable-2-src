using System;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SubtitleUI : MonoBehaviour
{
	private void Awake()
	{
		this.subtitlesAlpha = 0f;
		this.SubtitleFadeGroup.alpha = 0f;
		IntConfigurable intConfigurable = this.subtitleSizeIndex;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Combine(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnSubtitleSizeChange));
	}

	private void OnDestroy()
	{
		IntConfigurable intConfigurable = this.subtitleSizeIndex;
		intConfigurable.OnValueChanged = (Action<LiveData>)Delegate.Remove(intConfigurable.OnValueChanged, new Action<LiveData>(this.OnSubtitleSizeChange));
	}

	private void OnSubtitleSizeChange(LiveData data)
	{
		Vector2 a = new Vector2(1.7777778f, 1f);
		this.subtitleCanvasScaler.referenceResolution = a * this.subtitleSizeProfiles.sizeProfiles[data.IntValue].uiReferenceHeight;
	}

	private void Update()
	{
		if (this.fadingOut)
		{
			this.subtitlesAlpha -= Singleton<GameMaster>.Instance.GameDeltaTime / this.subtitleMicroFadeTime;
		}
		if (this.fadingOut && this.subtitlesAlpha <= 0f)
		{
			this.DEBUG_OUTPUT("alpha set to 0, ending fade out");
			this.subtitlesAlpha = 0f;
			this.fadingOut = false;
		}
		this.SubtitleFadeGroup.alpha = this.subtitlesAlpha;
		this.SubtitleFadeGroup.alpha *= (float)(this.subtitleToggle.GetBooleanValue() ? 1 : 0);
	}

	private string GetWordWrappedText(string text, TMP_Text tmp)
	{
		return text;
	}

	private IEnumerator SubSubtitleRunner(string splittableString, float clipLength, string originalKeyword)
	{
		this.DEBUG_OUTPUT("SubSubtitleRunner " + splittableString);
		if (splittableString.StartsWith("<split="))
		{
			splittableString = " " + splittableString;
		}
		string[] subSubtitles = splittableString.Split(new string[]
		{
			"<split="
		}, StringSplitOptions.None);
		List<float> subSplits = new List<float>();
		for (int j = 1; j < subSubtitles.Length; j++)
		{
			int num = subSubtitles[j].IndexOf('>');
			subSplits.Add(float.Parse(subSubtitles[j].Substring(0, num)));
			subSubtitles[j] = subSubtitles[j].Substring(num + 1);
		}
		subSplits.Add(clipLength);
		this.totalTimeElapsed = 0f;
		int num2;
		for (int i = 0; i < subSubtitles.Length; i = num2 + 1)
		{
			this.DEBUG_OUTPUT(string.Format("SubSubtitleRunner::SubtitleRenderer.text {0}/{1} = {2}", i, subSubtitles.Length, subSubtitles[i]));
			float timeToWait = subSplits[i] - this.totalTimeElapsed;
			while (this.fadingOut)
			{
				yield return null;
				timeToWait -= Time.deltaTime * ChoreoMaster.GameSpeed;
			}
			this.SubtitleRenderer.text = this.GetWordWrappedText(subSubtitles[i], this.SubtitleRenderer);
			if (subSubtitles[i].Trim() != "" && this.subtitleDebugInfo.GetBooleanValue())
			{
				this.SubtitleRenderer.text = string.Format("{0}\n{1} ({2:0.00}/{3:0.00})", new object[]
				{
					this.SubtitleRenderer.text,
					originalKeyword,
					subSplits[i],
					clipLength
				});
			}
			float fadeInStartTime = 0f;
			float fadeInEndTime = this.subtitleMicroFadeTime;
			float fadeOutStartTime = timeToWait - this.subtitleBlinkTime - this.subtitleMicroFadeTime;
			float fadeOutEndTime = timeToWait - this.subtitleBlinkTime;
			this.endTime = timeToWait;
			float time = Time.time;
			this.subtitlesAlpha = 0f;
			this.timeElapsed = 0f;
			if (string.IsNullOrEmpty(this.SubtitleRenderer.text.Trim()))
			{
				this.subtitlesAlpha = 0f;
				while (this.timeElapsed < this.endTime)
				{
					yield return null;
					this.timeElapsed += Time.deltaTime * ChoreoMaster.GameSpeed;
				}
			}
			else
			{
				while (this.timeElapsed < this.endTime)
				{
					if (fadeInStartTime <= this.timeElapsed && this.timeElapsed < fadeInEndTime)
					{
						this.subtitlesAlpha = Mathf.InverseLerp(fadeInStartTime, fadeInEndTime, this.timeElapsed);
					}
					if (fadeInEndTime <= this.timeElapsed && this.timeElapsed < fadeOutStartTime)
					{
						this.subtitlesAlpha = 1f;
					}
					if (fadeOutStartTime <= this.timeElapsed && this.timeElapsed < fadeOutEndTime)
					{
						this.subtitlesAlpha = 1f - Mathf.InverseLerp(fadeOutStartTime, fadeOutEndTime, this.timeElapsed);
					}
					if (fadeOutEndTime <= this.timeElapsed && this.timeElapsed <= this.endTime)
					{
						this.subtitlesAlpha = 0f;
					}
					yield return null;
					this.timeElapsed += Time.deltaTime * ChoreoMaster.GameSpeed;
				}
			}
			this.totalTimeElapsed += timeToWait;
			num2 = i;
		}
		this.SubSubTitleRoutinePlaying_DEBUG = false;
		this.SubSubTitleRoutine = null;
		yield break;
	}

	private void DEBUG_OUTPUT(string log)
	{
		if (this.DEBUG_OUTPUT_ON)
		{
			Debug.Log("SubtitleUI::" + log);
		}
	}

	private void FadeOutAndStartSubtitle(string subtitle, float clipLength, string originalKeyword)
	{
		this.HideSubtitlesWithFade();
		this.StartSubSubtitleRunner(subtitle, clipLength, originalKeyword);
	}

	private void StartSubSubtitleRunner(string subtitle, float clipLength, string originalKeyword)
	{
		this.StopSubSubtitleRunner();
		this.SubSubTitleRoutinePlaying_DEBUG = true;
		this.SubSubTitleRoutine = base.StartCoroutine(this.SubSubtitleRunner(subtitle, clipLength, originalKeyword));
	}

	private void StopSubSubtitleRunner()
	{
		if (this.SubSubTitleRoutine != null)
		{
			this.SubSubTitleRoutinePlaying_DEBUG = false;
			base.StopCoroutine(this.SubSubTitleRoutine);
			this.SubSubTitleRoutine = null;
		}
	}

	public void NewSubtitle(string keyword, float clipLength)
	{
		string empty = string.Empty;
		bool flag = LocalizationManager.TryGetTranslation(keyword, out empty, true, 0, true, false, null, null);
		if (keyword == "silence")
		{
			flag = false;
		}
		if (flag)
		{
			this.FadeOutAndStartSubtitle(empty, clipLength, keyword);
			return;
		}
		this.HideSubtitlesWithFade();
	}

	public void HideSubtitlesWithFade()
	{
		if (this.fadingOut && this.subtitlesAlpha > 0f)
		{
			return;
		}
		this.fadingOut = true;
		this.StopSubSubtitleRunner();
	}

	public void HideSubtitlesImmediate()
	{
		this.SubtitleFadeGroup.alpha = 0f;
		this.subtitlesAlpha = 0f;
		this.fadingOut = false;
		this.StopSubSubtitleRunner();
	}

	public CanvasGroup SubtitleFadeGroup;

	public TextMeshProUGUI SubtitleRenderer;

	private bool fadingOut;

	private float subtitlesAlpha = 1f;

	public float subtitleBlinkTime = 0.08f;

	public float subtitleMicroFadeTime = 0.02f;

	public BooleanConfigurable subtitleDebugInfo;

	public BooleanConfigurable subtitleToggle;

	public IntConfigurable subtitleLanguageIndex;

	public IntConfigurable subtitleSizeIndex;

	public CanvasScaler subtitleCanvasScaler;

	public SubtitleSizeProfileData subtitleSizeProfiles;

	private Coroutine fadeOutRoutine;

	private float totalTimeElapsed;

	private float endTime;

	private float timeElapsed;

	private bool DEBUG_OUTPUT_ON;

	public bool SubSubTitleRoutinePlaying_DEBUG;

	private Coroutine SubSubTitleRoutine;
}
