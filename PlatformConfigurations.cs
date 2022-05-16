using System;
using UnityEngine;

[CreateAssetMenu(fileName = "New Platform Configuration", menuName = "Platform Configuration")]
public class PlatformConfigurations : ScriptableObject
{
	public bool OverrideExists(RuntimePlatform platform, ReleaseBundle bundle, out BundleConfiguration overrideConfiguration)
	{
		for (int i = 0; i < this.overrides.Length; i++)
		{
			DefaultOverride defaultOverride = this.overrides[i];
			if (defaultOverride.MatchesPlatform(platform))
			{
				return defaultOverride.OverrideExistsForBundle(bundle, out overrideConfiguration);
			}
		}
		overrideConfiguration = null;
		return false;
	}

	public bool LoadIntoMemory(RuntimePlatform platform)
	{
		for (int i = 0; i < this.overrides.Length; i++)
		{
			DefaultOverride defaultOverride = this.overrides[i];
			if (defaultOverride.MatchesPlatform(platform))
			{
				return defaultOverride.loadBundlesIntoMemory;
			}
		}
		return false;
	}

	[SerializeField]
	private DefaultOverride[] overrides;
}
