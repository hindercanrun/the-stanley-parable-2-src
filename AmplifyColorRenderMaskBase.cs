using System;
using AmplifyColor;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.XR;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("")]
public class AmplifyColorRenderMaskBase : MonoBehaviour
{
	private void OnEnable()
	{
		if (this.maskCamera == null)
		{
			GameObject gameObject = new GameObject("Mask Camera", new Type[]
			{
				typeof(Camera)
			})
			{
				hideFlags = HideFlags.HideAndDontSave
			};
			gameObject.transform.parent = base.gameObject.transform;
			this.maskCamera = gameObject.GetComponent<Camera>();
		}
		this.referenceCamera = base.GetComponent<Camera>();
		this.colorEffect = base.GetComponent<AmplifyColorBase>();
		this.colorMaskShader = Shader.Find("Hidden/RenderMask");
	}

	private void OnDisable()
	{
		this.DestroyCamera();
		this.DestroyRenderTextures();
	}

	private void DestroyCamera()
	{
		if (this.maskCamera != null)
		{
			Object.DestroyImmediate(this.maskCamera.gameObject);
			this.maskCamera = null;
		}
	}

	private void DestroyRenderTextures()
	{
		if (this.maskTexture != null)
		{
			RenderTexture.active = null;
			Object.DestroyImmediate(this.maskTexture);
			this.maskTexture = null;
		}
	}

	private void UpdateRenderTextures(bool singlePassStereo)
	{
		int num = this.referenceCamera.pixelWidth;
		int num2 = this.referenceCamera.pixelHeight;
		if (this.maskTexture == null || this.width != num || this.height != num2 || !this.maskTexture.IsCreated() || this.singlePassStereo != singlePassStereo)
		{
			this.width = num;
			this.height = num2;
			this.DestroyRenderTextures();
			if (XRSettings.enabled)
			{
				num = XRSettings.eyeTextureWidth * (singlePassStereo ? 2 : 1);
				num2 = XRSettings.eyeTextureHeight;
			}
			if (this.maskTexture == null)
			{
				this.maskTexture = new RenderTexture(num, num2, 24, RenderTextureFormat.Default, RenderTextureReadWrite.sRGB)
				{
					hideFlags = HideFlags.HideAndDontSave,
					name = "MaskTexture"
				};
				this.maskTexture.name = "AmplifyColorMaskTexture";
				bool allowMSAA = this.maskCamera.allowMSAA;
				this.maskTexture.antiAliasing = ((allowMSAA && QualitySettings.antiAliasing > 0) ? QualitySettings.antiAliasing : 1);
			}
			this.maskTexture.Create();
			this.singlePassStereo = singlePassStereo;
		}
		if (this.colorEffect != null)
		{
			this.colorEffect.MaskTexture = this.maskTexture;
		}
	}

	private void UpdateCameraProperties()
	{
		this.maskCamera.CopyFrom(this.referenceCamera);
		this.maskCamera.targetTexture = this.maskTexture;
		this.maskCamera.clearFlags = CameraClearFlags.Nothing;
		this.maskCamera.renderingPath = RenderingPath.VertexLit;
		this.maskCamera.pixelRect = new Rect(0f, 0f, (float)this.width, (float)this.height);
		this.maskCamera.depthTextureMode = DepthTextureMode.None;
		this.maskCamera.allowHDR = false;
		this.maskCamera.enabled = false;
	}

	private void OnPreRender()
	{
		if (this.maskCamera != null)
		{
			RenderBuffer activeColorBuffer = Graphics.activeColorBuffer;
			RenderBuffer activeDepthBuffer = Graphics.activeDepthBuffer;
			bool flag = false;
			if (this.referenceCamera.stereoEnabled)
			{
				flag = (XRSettings.eyeTextureDesc.vrUsage == VRTextureUsage.TwoEyes);
				this.maskCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Left, this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left));
				this.maskCamera.SetStereoViewMatrix(Camera.StereoscopicEye.Right, this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right));
				this.maskCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Left, this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left));
				this.maskCamera.SetStereoProjectionMatrix(Camera.StereoscopicEye.Right, this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right));
			}
			this.UpdateRenderTextures(flag);
			this.UpdateCameraProperties();
			Graphics.SetRenderTarget(this.maskTexture);
			GL.Clear(true, true, this.ClearColor);
			if (flag)
			{
				this.maskCamera.worldToCameraMatrix = this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
				this.maskCamera.projectionMatrix = this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
				this.maskCamera.rect = new Rect(0f, 0f, 0.5f, 1f);
			}
			foreach (RenderLayer renderLayer in this.RenderLayers)
			{
				Shader.SetGlobalColor("_COLORMASK_Color", renderLayer.color);
				this.maskCamera.cullingMask = renderLayer.mask;
				this.maskCamera.RenderWithShader(this.colorMaskShader, "RenderType");
			}
			if (flag)
			{
				this.maskCamera.worldToCameraMatrix = this.referenceCamera.GetStereoViewMatrix(Camera.StereoscopicEye.Right);
				this.maskCamera.projectionMatrix = this.referenceCamera.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
				this.maskCamera.rect = new Rect(0.5f, 0f, 0.5f, 1f);
				foreach (RenderLayer renderLayer2 in this.RenderLayers)
				{
					Shader.SetGlobalColor("_COLORMASK_Color", renderLayer2.color);
					this.maskCamera.cullingMask = renderLayer2.mask;
					this.maskCamera.RenderWithShader(this.colorMaskShader, "RenderType");
				}
			}
			Graphics.SetRenderTarget(activeColorBuffer, activeDepthBuffer);
		}
	}

	[FormerlySerializedAs("clearColor")]
	public Color ClearColor = Color.black;

	[FormerlySerializedAs("renderLayers")]
	public RenderLayer[] RenderLayers = new RenderLayer[0];

	[FormerlySerializedAs("debug")]
	public bool DebugMask;

	private Camera referenceCamera;

	private Camera maskCamera;

	private AmplifyColorBase colorEffect;

	private int width;

	private int height;

	private RenderTexture maskTexture;

	private Shader colorMaskShader;

	private bool singlePassStereo;
}
