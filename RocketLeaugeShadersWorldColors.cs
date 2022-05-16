using System;
using UnityEngine;

[ExecuteInEditMode]
public class RocketLeaugeShadersWorldColors : MonoBehaviour
{
	private void Start()
	{
		this.SetColours();
	}

	[ContextMenu("Set Colours")]
	private void SetColours()
	{
		Shader.SetGlobalColor("TeamBlueColour", this.blueTeamColor);
		Shader.SetGlobalColor("TeamOrangeColour", this.orangeTeamColor);
	}

	public Color blueTeamColor;

	public Color orangeTeamColor;
}
