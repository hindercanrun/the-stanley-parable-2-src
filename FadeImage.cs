using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class FadeImage : MonoBehaviour
{
	private void Awake()
	{
		this.img = base.GetComponent<Image>();
	}

	private void Start()
	{
		if (this.selfStart)
		{
			this.StartFade();
		}
	}

	public void StartFade()
	{
		base.StartCoroutine(this.Fade());
	}

	private IEnumerator Fade()
	{
		float progress = 0f;
		yield return new WaitForSeconds(this.delay);
		while (progress < 1f)
		{
			progress += Time.deltaTime * (1f / this.duration);
			this.img.color = Color.Lerp(this.fromColor, this.toColor, progress);
			yield return null;
		}
		yield break;
	}

	private Image img;

	[SerializeField]
	private Color fromColor;

	[SerializeField]
	private Color toColor;

	[SerializeField]
	private bool selfStart;

	[SerializeField]
	private float duration = 2f;

	[SerializeField]
	private float delay = 1f;
}
