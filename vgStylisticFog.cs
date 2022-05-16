using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera))]
internal class vgStylisticFog : vgCommandBufferBase
{
	public override void VerifyResources()
	{
		base.VerifyResources();
		if (this.fogMaterial == null)
		{
			this.fogMaterial = new Material(this.fogShader);
			this.fogMaterial.hideFlags = HideFlags.DontSave;
		}
		if (this.mParamIdsList.Count == 0)
		{
			for (int i = 0; i < this.kSHADER_PASSES.Length; i++)
			{
				string prefix = this.kSHADER_PASSES[i];
				vgStylisticFog.ShaderParamIds shaderParamIds = new vgStylisticFog.ShaderParamIds();
				shaderParamIds.Link(prefix);
				this.mParamIdsList.Add(shaderParamIds);
			}
		}
	}

	protected override CameraEvent GetPassCameraEvent()
	{
		return this.cameraQueueEvent;
	}

	protected override string GetPassCommandBufferName()
	{
		return this.commandBufferName;
	}

	protected override int GetPassSortingIndex()
	{
		return this.orderingIndex;
	}

	private bool CheckFogData()
	{
		return true;
	}

	[ContextMenu("Clear Command Buffers")]
	public void ClearCommandBuffers()
	{
	}

	private void SetFogShaderParameters(vgStylisticFogData data, int pass)
	{
		if (data == null)
		{
			Debug.LogError("vgStylisticFog:: Data being set from is null, no parameters have been setup.", this);
			return;
		}
		vgStylisticFog.ShaderParamIds shaderParamIds = this.mParamIdsList[pass];
		Vector3 v = default(Vector3);
		float value = Mathf.Clamp01(data.intensityScale + this.intensityOffset);
		float num = data.startDistance + this.startPointOffset;
		float num2 = data.endDistance + this.endPointOffset;
		float z = (num - this.cameraNear) / (this.cameraFar - this.cameraNear);
		float z2 = (num2 - this.cameraNear) / (this.cameraFar - this.cameraNear);
		if (data.transformObjectB != null && data.transformObjectA != null)
		{
			v = data.transformObjectB.position - data.transformObjectA.position;
			v.Normalize();
		}
		else
		{
			v = new Vector3(0f, 0f, 1f);
		}
		data.offsetFromAToB = Mathf.Clamp(data.offsetFromAToB, -1f, 1f);
		this.fogMaterial.SetVector(shaderParamIds.StartDistance, new Vector4(1f / num, this.cameraScale - num, z, 0f));
		this.fogMaterial.SetVector(shaderParamIds.EndDistance, new Vector4(1f / num2, this.cameraScale - num2, z2, 0f));
		this.fogMaterial.SetTexture(shaderParamIds.FogColorTextureFromAToB, data.fogColorTextureFromAToB);
		this.fogMaterial.SetTexture(shaderParamIds.FogColorTextureFromBToA, data.fogColorTextureFromBToA);
		this.fogMaterial.SetVector(shaderParamIds.FromAToBNormal, v);
		this.fogMaterial.SetFloat(shaderParamIds.FromAToBOffset, data.offsetFromAToB);
		this.fogMaterial.SetFloat(shaderParamIds.IntensityScale, value);
	}

	private void PrepareFogParameters(Camera cam)
	{
		this.VerifyResources();
		this.cameraNear = cam.nearClipPlane;
		this.cameraFar = cam.farClipPlane;
		this.cameraFov = cam.fieldOfView;
		this.cameraAspectRatio = cam.aspect;
		Matrix4x4 identity = Matrix4x4.identity;
		float num = this.cameraFov * 0.5f;
		Vector3 b = cam.transform.right * this.cameraNear * Mathf.Tan(num * 0.017453292f) * this.cameraAspectRatio;
		Vector3 b2 = cam.transform.up * this.cameraNear * Mathf.Tan(num * 0.017453292f);
		Vector3 vector = cam.transform.forward * this.cameraNear - b + b2;
		this.cameraScale = vector.magnitude * this.cameraFar / this.cameraNear;
		vector.Normalize();
		vector *= this.cameraScale;
		Vector3 vector2 = cam.transform.forward * this.cameraNear + b + b2;
		vector2.Normalize();
		vector2 *= this.cameraScale;
		Vector3 vector3 = cam.transform.forward * this.cameraNear + b - b2;
		vector3.Normalize();
		vector3 *= this.cameraScale;
		Vector3 vector4 = cam.transform.forward * this.cameraNear - b - b2;
		vector4.Normalize();
		vector4 *= this.cameraScale;
		identity.SetRow(0, vector);
		identity.SetRow(1, vector2);
		identity.SetRow(2, vector3);
		identity.SetRow(3, vector4);
		vgStylisticFog.ShaderParamIds shaderParamIds = this.mParamIdsList[0];
		this.fogMaterial.SetMatrix(shaderParamIds.FrustumCornersWS, identity);
		this.fogMaterial.SetVector(shaderParamIds.CameraWS, cam.transform.position);
		this.fogMaterial.SetFloat(shaderParamIds.OneOverFarMinusNearPlane, 1f / (cam.farClipPlane - cam.nearClipPlane));
		this.SetFogShaderParameters(this._customFogData, 0);
	}

	protected override void RefreshCommandBufferInfo(CommandBuffer buf, Camera cam)
	{
		this.VerifyResources();
		this.PrepareFogParameters(cam);
		buf.SetRenderTarget(BuiltinRenderTextureType.CurrentActive);
		buf.DrawMesh(vgCommandBufferBase.fullScreenQuadMesh, Matrix4x4.identity, this.fogMaterial, 0, 0);
	}

	public Shader fogShader;

	private Material fogMaterial;

	private float cameraNear = 0.5f;

	private float cameraFar = 50f;

	private float cameraFov = 60f;

	private float cameraAspectRatio = 1.333333f;

	private bool interpolateBetweenTwoFogs;

	private float dualFogInterpolationValue;

	private static string shaderDualFogFeatureDefineOn = "INTERPOLATE_DUAL_FOG";

	private static string shaderDualFogFeatureDefineOff = "DONT_INTERPOLATE_DUAL_FOG";

	private float cameraScale;

	private string commandBufferName = "Stylistic Fog Effect";

	private CameraEvent cameraQueueEvent = CameraEvent.BeforeSkybox;

	[Tooltip("Change this to render this before or after other command buffers - doesn't change in real time")]
	public int orderingIndex = 1;

	public float intensityOffset;

	public float startPointOffset;

	public float endPointOffset;

	private string[] kSHADER_PASSES = new string[]
	{
		"_",
		"_Second"
	};

	protected List<vgStylisticFog.ShaderParamIds> mParamIdsList = new List<vgStylisticFog.ShaderParamIds>();

	public vgStylisticFogData _customFogData;

	[Serializable]
	protected class ShaderParamIds
	{
		public bool valid
		{
			get
			{
				return this._valid;
			}
		}

		public void Link(string prefix)
		{
			this.StartDistance = Shader.PropertyToID(prefix + "StartDistance");
			this.EndDistance = Shader.PropertyToID(prefix + "EndDistance");
			this.FogColorTextureFromAToB = Shader.PropertyToID(prefix + "FogColorTextureFromAToB");
			this.FogColorTextureFromBToA = Shader.PropertyToID(prefix + "FogColorTextureFromBToA");
			this.FromAToBNormal = Shader.PropertyToID(prefix + "FromAToBNormal");
			this.FromAToBOffset = Shader.PropertyToID(prefix + "FromAToBOffset");
			this.IntensityScale = Shader.PropertyToID(prefix + "IntensityScale");
			this.MainTex_TexelSize = Shader.PropertyToID("_MainTex_TexelSize");
			this.DualFogInterpolationValue = Shader.PropertyToID("_DualFogInterpolationValue");
			this.FrustumCornersWS = Shader.PropertyToID("_FrustumCornersWS");
			this.OneOverFarMinusNearPlane = Shader.PropertyToID("_OneOverFarMinusNearPlane");
			this.CameraWS = Shader.PropertyToID("_CameraWS");
			this._valid = true;
		}

		public int StartDistance = -1;

		public int EndDistance = -1;

		public int FogColorTextureFromAToB = -1;

		public int FogColorTextureFromBToA = -1;

		public int FromAToBNormal = -1;

		public int FromAToBOffset = -1;

		public int IntensityScale = -1;

		public int MainTex_TexelSize;

		public int DualFogInterpolationValue;

		public int FrustumCornersWS;

		public int OneOverFarMinusNearPlane;

		public int CameraWS;

		private bool _valid;
	}
}
