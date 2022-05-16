using System;
using UnityEngine;

public class Painter : MonoBehaviour
{
	private void OnGUI()
	{
		GUI.skin = this.gskin;
		GUILayout.BeginArea(new Rect((float)Screen.width * this.uIOffsets.mainPainterBox.x, (float)Screen.height * this.uIOffsets.mainPainterBox.y, (float)Screen.width * 0.9f, (float)Screen.height * 0.9f), "", "PurpleBox");
		GUILayout.BeginArea(new Rect((float)Screen.width * this.uIOffsets.toolsbox.x, (float)Screen.height * this.uIOffsets.toolsbox.y, (float)Screen.width * 0.15f, (float)Screen.height));
		GUILayout.Space(10f);
		GUILayout.Label("Drawing Options", Array.Empty<GUILayoutOption>());
		this.tool = (Painter.Tool)GUILayout.Toolbar((int)this.tool, this.toolIcons, "Tool", Array.Empty<GUILayoutOption>());
		GUILayout.Space(10f);
		Painter.Tool tool = this.tool;
		if (tool != Painter.Tool.Brush)
		{
			if (tool == Painter.Tool.Eraser)
			{
				GUILayout.Label("Size " + Mathf.Round(this.eraser.width * 10f) / 10f, Array.Empty<GUILayoutOption>());
				this.eraser.width = GUILayout.HorizontalSlider(this.eraser.width, 0f, 50f, Array.Empty<GUILayoutOption>());
				GUILayout.Space(50f);
			}
		}
		else
		{
			GUILayout.Label("Size " + Mathf.Round(this.brush.width * 10f) / 10f, Array.Empty<GUILayoutOption>());
			this.brush.width = GUILayout.HorizontalSlider(this.brush.width, 0f, 40f, Array.Empty<GUILayoutOption>());
			GUILayout.Space(10f);
			this.brushColor = GUIControls.RGBCircle(this.brushColor, "Color Picker", this.colorCircleTex);
			GUILayout.Space(10f);
		}
		if (GUILayout.Button(this.clearDwatingIcon, "ClearDrawing", Array.Empty<GUILayoutOption>()))
		{
			this.OnEnable();
		}
		GUILayout.Space(10f);
		if (GUILayout.Button("Save", "Save", Array.Empty<GUILayoutOption>()))
		{
			Drawing.MergeTextures(ref this.baseTex, ref this.drawingTex, this.drawingTextureDownScalingRatio);
			base.enabled = false;
		}
		GUILayout.EndArea();
		GUI.DrawTexture(new Rect((float)Screen.width * this.uIOffsets.canvas.x, (float)Screen.height * this.uIOffsets.canvas.y, (float)Screen.width * this.canvasScaleRatio, (float)Screen.height * this.canvasScaleRatio), this.baseTex);
		GUI.DrawTexture(new Rect((float)Screen.width * this.uIOffsets.canvas.x, (float)Screen.height * this.uIOffsets.canvas.y, (float)Screen.width * this.canvasScaleRatio, (float)Screen.height * this.canvasScaleRatio), this.drawingTex);
		GUILayout.BeginArea(new Rect((float)Screen.width * (1f - this.uIOffsets.toolsbox.x) - (float)Screen.width * 0.15f, (float)Screen.height * this.uIOffsets.toolsbox.y, (float)Screen.width * 0.15f, (float)Screen.height));
		if (GUILayout.Button("", "Close", Array.Empty<GUILayoutOption>()))
		{
			base.enabled = false;
		}
		GUILayout.EndArea();
		GUILayout.EndArea();
	}

	private void Update()
	{
		this.imgRect = new Rect((float)Screen.width * (this.uIOffsets.canvas.x + this.uIOffsets.mainPainterBox.x), (float)Screen.height * (this.uIOffsets.canvas.y + this.uIOffsets.mainPainterBox.y), (float)Screen.width * this.canvasScaleRatio, (float)Screen.height * this.canvasScaleRatio);
		Vector2 vector = Input.mousePosition;
		vector.y = (float)Screen.height - vector.y;
		if (Input.GetKeyDown("mouse 0"))
		{
			if (this.imgRect.Contains(vector))
			{
				this.dragStart = vector - new Vector2(this.imgRect.x, this.imgRect.y);
				this.dragStart.y = this.imgRect.height - this.dragStart.y;
				this.dragStart.x = Mathf.Round(this.dragStart.x * (this.canvasScaleRatio * (float)this.drawingTextureDownScalingRatio));
				this.dragStart.y = Mathf.Round(this.dragStart.y * (this.canvasScaleRatio * (float)this.drawingTextureDownScalingRatio));
				this.ClampCursor(vector, this.imgRect);
			}
			else
			{
				this.dragStart = Vector3.zero;
			}
		}
		if (Input.GetKey("mouse 0"))
		{
			if (this.dragStart == Vector2.zero)
			{
				return;
			}
			this.ClampCursor(vector, this.imgRect);
			if (this.tool == Painter.Tool.Brush)
			{
				this.Brush(this.dragEnd, this.preDrag);
			}
			if (this.tool == Painter.Tool.Eraser)
			{
				this.Eraser(this.dragEnd, this.preDrag);
			}
		}
		if (Input.GetKeyUp("mouse 0") && this.dragStart != Vector2.zero)
		{
			this.dragStart = Vector2.zero;
			this.dragEnd = Vector2.zero;
		}
		this.preDrag = this.dragEnd;
	}

	private void ClampCursor(Vector2 mouse, Rect imgRect)
	{
		this.dragEnd = mouse - new Vector2(imgRect.x, imgRect.y);
		this.dragEnd.x = Mathf.Clamp(this.dragEnd.x, 0f, imgRect.width);
		this.dragEnd.y = imgRect.height - Mathf.Clamp(this.dragEnd.y, 0f, imgRect.height * 2f);
		this.dragEnd.x = Mathf.Round(this.dragEnd.x / (this.canvasScaleRatio * (float)this.drawingTextureDownScalingRatio));
		this.dragEnd.y = Mathf.Round(this.dragEnd.y / (this.canvasScaleRatio * (float)this.drawingTextureDownScalingRatio));
	}

	private void Brush(Vector2 p1, Vector2 p2)
	{
		if (p2 == Vector2.zero)
		{
			p2 = p1;
		}
		this.stroke.Set(p1, p2, this.brush.width, this.brush.hardness, this.brushColor);
		Drawing.PaintLine(this.stroke, ref this.drawingTex);
	}

	private void Eraser(Vector2 p1, Vector2 p2)
	{
		if (p2 == Vector2.zero)
		{
			p2 = p1;
		}
		this.stroke.Set(p1, p2, this.eraser.width, this.eraser.hardness, Color.clear);
		Drawing.PaintLine(this.stroke, ref this.drawingTex);
	}

	public void OnDisable()
	{
		if (this._OnDisable != null)
		{
			this._OnDisable();
		}
	}

	public void OnEnable()
	{
		if (this.drawingTex != null)
		{
			Object.Destroy(this.drawingTex);
		}
		this.drawingTex = new Texture2D(Screen.width / this.drawingTextureDownScalingRatio, Screen.height / this.drawingTextureDownScalingRatio, TextureFormat.RGBA32, false, true);
		Color[] colors = new Color[Screen.width * Screen.height / this.drawingTextureDownScalingRatio];
		this.drawingTex.SetPixels(colors, 0);
		this.drawingTex.Apply(false);
	}

	[Header("Drawing Toolset")]
	public Painter.Tool tool;

	public Texture[] toolIcons;

	public Texture2D colorCircleTex;

	public Texture2D clearDwatingIcon;

	public Color brushColor = Color.white;

	[Header("Canvas")]
	[Tooltip("A value of 2 would mean that the transparent texture to be drawn on top off is going to be half of the resolution of the screenshot taken, this saves a lot of perfomance but makes drawing look a bit more pixelated")]
	public int drawingTextureDownScalingRatio = 2;

	public GUISkin gskin;

	[Header("Organization")]
	public Painter.UIOffsets uIOffsets;

	[Tooltip("The scale of the canvas in proportion to the size of the screen")]
	public float canvasScaleRatio = 0.675f;

	public Action _OnDisable;

	[HideInInspector]
	public Texture2D baseTex;

	private Texture2D drawingTex;

	private Painter.BrushTool brush = new Painter.BrushTool();

	private Painter.EraserTool eraser = new Painter.EraserTool();

	private Vector2 dragStart;

	private Vector2 dragEnd;

	private Vector2 preDrag;

	private Rect imgRect;

	private Drawing.Stroke stroke = new Drawing.Stroke();

	public enum Tool
	{
		Brush,
		Eraser,
		None
	}

	public class EraserTool
	{
		public float width = 2f;

		public float hardness = 50f;
	}

	public class BrushTool
	{
		public float width = 1f;

		public float hardness = 50f;

		public float spacing = 10f;
	}

	[Serializable]
	public class UIOffsets
	{
		[HideInInspector]
		public Vector2 canvas
		{
			get
			{
				this._canvas.Set(this.canvasBox, this.canvasBox * 0.5f);
				return this._canvas;
			}
			set
			{
			}
		}

		public Vector2 mainPainterBox = new Vector2(0.05f, 0.05f);

		public Vector2 toolsbox = new Vector2(0.025f, 0.025f);

		public float canvasBox = 0.2f;

		private Vector2 _canvas;
	}
}
