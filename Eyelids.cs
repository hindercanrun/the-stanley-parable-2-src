using System;
using System.Collections;
using UnityEngine;

public class Eyelids : MonoBehaviour
{
	private void Start()
	{
		this.canvasRect = this.canvas.GetComponent<RectTransform>();
		this.ScaleToScreen();
		this.topLid.anchoredPosition = new Vector2(0f, this.height);
		this.bottomLid.anchoredPosition = new Vector2(0f, -this.height);
		this.canvas.enabled = false;
		GameMaster.OnResume += this.ScaleToScreen;
	}

	private void OnDestroy()
	{
		GameMaster.OnResume -= this.ScaleToScreen;
	}

	public void StartClose()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.Animate(EyelidDir.Close));
	}

	public void StartOpen()
	{
		base.StopAllCoroutines();
		base.StartCoroutine(this.Animate(EyelidDir.Open));
	}

	private void ScaleToScreen()
	{
		this.height = this.canvasRect.rect.size.y;
		this.width = this.canvasRect.rect.size.x;
		this.height += 1f;
		this.width += 150f;
		this.topLid.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.width);
		this.topLid.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.height);
		this.bottomLid.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.width);
		this.bottomLid.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.height);
		this.speed = this.height / this.duration;
	}

	private IEnumerator Animate(EyelidDir dir)
	{
		this.ScaleToScreen();
		float target = 0f;
		if (dir == EyelidDir.Open)
		{
			target = this.height;
		}
		else
		{
			this.canvas.enabled = base.enabled;
		}
		while (this.topLid.anchoredPosition.y != target)
		{
			float num = Mathf.MoveTowards(this.topLid.anchoredPosition.y, target, this.speed * Singleton<GameMaster>.Instance.GameDeltaTime);
			this.topLid.anchoredPosition = new Vector2(0f, num);
			this.bottomLid.anchoredPosition = new Vector2(0f, -num);
			yield return null;
		}
		if (dir == EyelidDir.Open)
		{
			this.canvas.enabled = false;
		}
		yield break;
	}

	public void Reset()
	{
		base.StopAllCoroutines();
		this.topLid.anchoredPosition = new Vector2(0f, this.height);
		this.bottomLid.anchoredPosition = new Vector2(0f, -this.height);
	}

	public Canvas canvas;

	private RectTransform canvasRect;

	public RectTransform bottomLid;

	public RectTransform topLid;

	private float duration = 4f;

	private float speed = 200f;

	private float height = 400f;

	private float width = 800f;
}
