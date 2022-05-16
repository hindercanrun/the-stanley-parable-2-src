using System;
using UnityEngine;

namespace AmplifyBloom
{
	[ImageEffectAllowedInSceneView]
	[ExecuteInEditMode]
	[RequireComponent(typeof(Camera))]
	[AddComponentMenu("Image Effects/Amplify Bloom")]
	[Serializable]
	public sealed class AmplifyBloomEffect : AmplifyBloomBase
	{
	}
}
