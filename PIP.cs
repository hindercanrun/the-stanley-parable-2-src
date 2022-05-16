using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PIP : MonoBehaviour
{
	private float aspectRatio
	{
		get
		{
			return this._aspectRatio;
		}
		set
		{
			if (value != this._aspectRatio)
			{
				if ((double)this._aspectRatio >= 1.5 && (double)value < 1.5)
				{
					this.SetAspect(PIP.Aspect.Narrow);
				}
				else if ((double)this._aspectRatio < 1.5 && (double)value >= 1.5)
				{
					this.SetAspect(PIP.Aspect.Wide);
				}
				this._aspectRatio = value;
			}
		}
	}

	private void Awake()
	{
		Transform[] componentsInChildren = this.level_0.GetComponentsInChildren<Transform>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (componentsInChildren[i].name == "Main-version")
			{
				componentsInChildren[i].gameObject.SetActive(true);
			}
			if (componentsInChildren[i].name == "Pause-version")
			{
				componentsInChildren[i].gameObject.SetActive(false);
			}
		}
		this.CreatePIPLevels(this.level_0);
		this.PIPBasePos = this.PIPRT.anchoredPosition3D;
		this.PIPBaseScale = this.PIPRT.localScale;
		this.canvasBaseScale = this.canvasRT.localScale;
		this.canvasBaseRect = new Rect(this.BGRT.rect);
		this.BGRT.GetComponent<RawImage>().material.SetFloat("_Strength", 0f);
		this.backgroundImages.Add(this.BGRT.GetComponent<RawImage>());
	}

	private void Update()
	{
		this.BGRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, this.canvasRT.rect.width);
		this.BGRT.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, this.canvasRT.rect.height);
		Vector3 anchoredPosition3D = new Vector3(this.PIPBasePos.x * (this.canvasRT.rect.width / this.canvasBaseRect.width), this.PIPBasePos.y * (this.canvasRT.rect.height / this.canvasBaseRect.height), 0f);
		this.PIPRT.anchoredPosition3D = anchoredPosition3D;
		this.PIPRT.localScale = new Vector3(this.PIPBaseScale.x * (this.canvasRT.rect.width / this.canvasBaseRect.width), this.PIPBaseScale.y * (this.canvasRT.rect.height / this.canvasBaseRect.height), 1f);
		this.aspectRatio = (float)Screen.width / (float)Screen.height;
	}

	private void OnGUI()
	{
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.finalTex);
	}

	private void SetAspect(PIP.Aspect aspect)
	{
		Texture2D texture = this.widescreenTex;
		if (aspect == PIP.Aspect.Narrow)
		{
			texture = this.squarescreenTex;
		}
		for (int i = 0; i < this.backgroundImages.Count; i++)
		{
			this.backgroundImages[i].texture = texture;
		}
		for (int j = 0; j < this.PIPsWide.Count; j++)
		{
			this.PIPsWide[j].SetActive(aspect == PIP.Aspect.Wide);
		}
		for (int k = 0; k < this.PIPsNarrow.Count; k++)
		{
			this.PIPsNarrow[k].SetActive(aspect == PIP.Aspect.Narrow);
		}
	}

	private void CreatePIPLevels(GameObject orig)
	{
		GameObject gameObject = orig;
		float num = 3f;
		int num2 = 0;
		while ((float)num2 < num)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(gameObject);
			gameObject2.name = "Menu_Level_" + (num2 + 1);
			gameObject2.transform.position += Vector3.right * 300f;
			int num3 = (int)Mathf.Pow((float)(num2 + 2), 2f);
			RenderTexture renderTexture = new RenderTexture(2048 / num3, 1024 / num3, 0);
			gameObject2.GetComponentInChildren<CanvasScaler>().scaleFactor = 1f / (float)num3;
			RawImage[] componentsInChildren = gameObject.transform.GetComponentsInChildren<RawImage>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i].name == "PIP-WIDE")
				{
					componentsInChildren[i].texture = renderTexture;
					if (!this.PIPsWide.Contains(componentsInChildren[i].gameObject))
					{
						this.PIPsWide.Add(componentsInChildren[i].gameObject);
					}
				}
				else if (componentsInChildren[i].name == "PIP-NARROW")
				{
					componentsInChildren[i].texture = renderTexture;
					if (!this.PIPsNarrow.Contains(componentsInChildren[i].gameObject))
					{
						this.PIPsNarrow.Add(componentsInChildren[i].gameObject);
					}
				}
			}
			gameObject2.transform.GetComponentInChildren<Camera>().targetTexture = renderTexture;
			RawImage[] componentsInChildren2 = gameObject2.transform.GetComponentsInChildren<RawImage>();
			for (int j = 0; j < componentsInChildren2.Length; j++)
			{
				if (componentsInChildren2[j].name == "Office-BG")
				{
					this.backgroundImages.Add(componentsInChildren2[j]);
				}
				else if ((float)num2 == num - 1f && componentsInChildren2[j].name == "PIP-WIDE")
				{
					this.backgroundImages.Add(componentsInChildren2[j]);
					if (!this.PIPsWide.Contains(componentsInChildren2[j].gameObject))
					{
						this.PIPsWide.Add(componentsInChildren2[j].gameObject);
					}
				}
				else if ((float)num2 == num - 1f && componentsInChildren2[j].name == "PIP-NARROW")
				{
					this.backgroundImages.Add(componentsInChildren2[j]);
					if (!this.PIPsNarrow.Contains(componentsInChildren2[j].gameObject))
					{
						this.PIPsNarrow.Add(componentsInChildren2[j].gameObject);
					}
				}
			}
			MenuButton[] componentsInChildren3 = gameObject2.transform.GetComponentsInChildren<MenuButton>();
			for (int k = 0; k < componentsInChildren3.Length; k++)
			{
				componentsInChildren3[k].original = false;
			}
			gameObject = gameObject2;
			num2++;
		}
	}

	public GameObject level_0;

	[Space(10f)]
	public Texture2D widescreenTex;

	public Texture2D squarescreenTex;

	private List<RawImage> backgroundImages = new List<RawImage>();

	private List<GameObject> PIPsWide = new List<GameObject>();

	private List<GameObject> PIPsNarrow = new List<GameObject>();

	[Space(10f)]
	public RectTransform BGRT;

	public RectTransform canvasRT;

	public RectTransform PIPRT;

	private Vector3 PIPBaseScale;

	private Vector3 PIPBasePos;

	private Vector3 canvasBaseScale;

	private Rect canvasBaseRect;

	public Texture finalTex;

	private float _aspectRatio;

	private enum Aspect
	{
		Wide,
		Narrow
	}
}
