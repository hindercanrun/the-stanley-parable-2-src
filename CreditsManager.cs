using System;
using UnityEngine;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
	public void ShowMenuCredits()
	{
		this.creditsAnimator.playbackTime = 0f;
		this.creditsAnimator.Rebind();
		this.creditsCanvasGroup.alpha = 1f;
		this.creditsAudioSource.Play();
	}

	[SerializeField]
	private Animator creditsAnimator;

	[SerializeField]
	private AudioSource creditsAudioSource;

	[SerializeField]
	private Image creditsBlackBackgrounds;

	[SerializeField]
	private CanvasGroup creditsCanvasGroup;
}
