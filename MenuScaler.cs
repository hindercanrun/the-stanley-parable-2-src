using System;
using UnityEngine;

public class MenuScaler : MonoBehaviour
{
	private void Start()
	{
		this.RT = base.GetComponent<RectTransform>();
		this.anchoredPos = this.RT.anchoredPosition3D;
		this.widthHeight = new Vector2(this.RT.rect.width, this.RT.rect.height);
		this.scale = this.RT.localScale;
		this.canvasRT = base.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
	}

	private void Update()
	{
		this.RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.widthHeight.x * (this.canvasRT.rect.width / 512f));
		this.RT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.widthHeight.y * (this.canvasRT.rect.width / 256f));
		Vector3 anchoredPosition3D = new Vector3(this.anchoredPos.x * (this.canvasRT.rect.width / 512f), this.anchoredPos.y * (this.canvasRT.rect.height / 256f), 0f);
		this.RT.anchoredPosition3D = anchoredPosition3D;
		this.RT.localScale = new Vector3(this.scale.x * (this.canvasRT.rect.width / 512f), this.scale.y * (this.canvasRT.rect.height / 256f), 1f);
	}

	private RectTransform RT;

	private Vector3 anchoredPos;

	private Vector2 widthHeight;

	private Vector3 scale;

	private RectTransform canvasRT;
}
