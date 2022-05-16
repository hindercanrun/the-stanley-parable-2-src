using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MovingScreenshot : MonoBehaviour
{
	private void Awake()
	{
		this.cachedRectTransform = base.GetComponent<RectTransform>();
		this.finalPosition = this.cachedRectTransform.anchoredPosition;
		this.cachedImageMat = base.GetComponent<RawImage>().material;
	}

	public void StartMoveScale()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.moveRoutine != null)
		{
			base.StopCoroutine(this.moveRoutine);
		}
		this.moveRoutine = base.StartCoroutine(this.MoveAndScaleIntoPosition());
	}

	private IEnumerator MoveAndScaleIntoPosition()
	{
		float timer = 0f;
		float width = (float)Screen.currentResolution.width;
		float height = (float)Screen.currentResolution.height;
		while (timer < 1f)
		{
			this.SetSize(Mathf.Lerp(width, width / this.reductionFactor, timer / 1f), Mathf.Lerp(height, height / this.reductionFactor, timer / 1f));
			this.cachedRectTransform.anchoredPosition = Vector2.Lerp(Vector2.zero, this.finalPosition, timer / 1f);
			this.cachedImageMat.SetFloat("_Intensity", Mathf.Lerp(0f, 1f, timer / 1f));
			timer += Time.unscaledDeltaTime * 0.666f;
			yield return null;
		}
		this.moveRoutine = null;
		yield break;
	}

	private void SetSize(float width, float height)
	{
		base.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		base.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
	}

	[SerializeField]
	private float reductionFactor = 2.5f;

	private RectTransform cachedRectTransform;

	private Vector2 finalPosition;

	private Coroutine moveRoutine;

	private Material cachedImageMat;
}
