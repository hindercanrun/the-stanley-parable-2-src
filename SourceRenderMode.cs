using System;

[Serializable]
public enum SourceRenderMode
{
	Normal,
	Color,
	Texture,
	Glow,
	Solid,
	Additive,
	AdditiveFractionalFrame = 7,
	WorldSpaceGlow = 9,
	DontRender
}
