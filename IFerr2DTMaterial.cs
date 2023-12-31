﻿using System;
using UnityEngine;

public interface IFerr2DTMaterial
{
	string name { get; }

	Material fillMaterial { get; set; }

	Material edgeMaterial { get; set; }

	Ferr2DT_SegmentDescription GetDescriptor(Ferr2DT_TerrainDirection aDirection);

	bool Has(Ferr2DT_TerrainDirection aDirection);

	void Set(Ferr2DT_TerrainDirection aDirection, bool aActive);

	Rect GetBody(Ferr2DT_TerrainDirection aDirection, int aBodyID);

	Rect ToUV(Rect aNativeRect);

	Rect ToScreen(Rect aNativeRect);

	Rect ToNative(Rect aPixelRect);

	Rect ToPixels(Rect aNativeRect);
}
