using System;
using UnityEngine;

public class Ferr2DT_Material : ScriptableObject, IFerr2DTMaterial
{
	public Material fillMaterial
	{
		get
		{
			return this._fillMaterial;
		}
		set
		{
			this._fillMaterial = value;
		}
	}

	public Material edgeMaterial
	{
		get
		{
			return this._edgeMaterial;
		}
		set
		{
			this._edgeMaterial = value;
		}
	}

	public Ferr2DT_Material()
	{
		for (int i = 0; i < this._descriptors.Length; i++)
		{
			this._descriptors[i] = new Ferr2DT_SegmentDescription();
		}
	}

	public Ferr2DT_SegmentDescription GetDescriptor(Ferr2DT_TerrainDirection aDirection)
	{
		this.ConvertToPercentage();
		for (int i = 0; i < this._descriptors.Length; i++)
		{
			if (this._descriptors[i].applyTo == aDirection)
			{
				return this._descriptors[i];
			}
		}
		if (this._descriptors.Length != 0)
		{
			return this._descriptors[0];
		}
		return new Ferr2DT_SegmentDescription();
	}

	public bool Has(Ferr2DT_TerrainDirection aDirection)
	{
		for (int i = 0; i < this._descriptors.Length; i++)
		{
			if (this._descriptors[i].applyTo == aDirection)
			{
				return true;
			}
		}
		return false;
	}

	public void Set(Ferr2DT_TerrainDirection aDirection, bool aActive)
	{
		if (aActive)
		{
			if (this._descriptors[(int)aDirection].applyTo != aDirection)
			{
				this._descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
				this._descriptors[(int)aDirection].applyTo = aDirection;
				return;
			}
		}
		else if (this._descriptors[(int)aDirection].applyTo != Ferr2DT_TerrainDirection.Top)
		{
			this._descriptors[(int)aDirection] = new Ferr2DT_SegmentDescription();
		}
	}

	public Rect ToUV(Rect aNativeRect)
	{
		if (this.edgeMaterial == null)
		{
			return aNativeRect;
		}
		return new Rect(aNativeRect.x, 1f - aNativeRect.height - aNativeRect.y, aNativeRect.width, aNativeRect.height);
	}

	public Rect ToScreen(Rect aNativeRect)
	{
		this.edgeMaterial == null;
		return aNativeRect;
	}

	public Rect GetBody(Ferr2DT_TerrainDirection aDirection, int aBodyID)
	{
		return this.GetDescriptor(aDirection).body[aBodyID];
	}

	private void ConvertToPercentage()
	{
		if (this.isPixel)
		{
			for (int i = 0; i < this._descriptors.Length; i++)
			{
				for (int j = 0; j < this._descriptors[i].body.Length; j++)
				{
					this._descriptors[i].body[j] = this.ToNative(this._descriptors[i].body[j]);
				}
				this._descriptors[i].leftCap = this.ToNative(this._descriptors[i].leftCap);
				this._descriptors[i].rightCap = this.ToNative(this._descriptors[i].rightCap);
			}
			this.isPixel = false;
		}
	}

	public Rect ToNative(Rect aPixelRect)
	{
		if (this.edgeMaterial == null)
		{
			return aPixelRect;
		}
		int num = (this.edgeMaterial.mainTexture == null) ? 1 : this.edgeMaterial.mainTexture.width;
		int num2 = (this.edgeMaterial.mainTexture == null) ? 1 : this.edgeMaterial.mainTexture.height;
		return new Rect(aPixelRect.x / (float)num, aPixelRect.y / (float)num2, aPixelRect.width / (float)num, aPixelRect.height / (float)num2);
	}

	public Rect ToPixels(Rect aNativeRect)
	{
		if (this.edgeMaterial == null)
		{
			return aNativeRect;
		}
		int num = (this.edgeMaterial.mainTexture == null) ? 1 : this.edgeMaterial.mainTexture.width;
		int num2 = (this.edgeMaterial.mainTexture == null) ? 1 : this.edgeMaterial.mainTexture.height;
		return new Rect(aNativeRect.x * (float)num, aNativeRect.y * (float)num2, aNativeRect.width * (float)num, aNativeRect.height * (float)num2);
	}

	string IFerr2DTMaterial.get_name()
	{
		return base.name;
	}

	[SerializeField]
	private Material _fillMaterial;

	[SerializeField]
	private Material _edgeMaterial;

	[SerializeField]
	private Ferr2DT_SegmentDescription[] _descriptors = new Ferr2DT_SegmentDescription[4];

	[SerializeField]
	private bool isPixel = true;
}
