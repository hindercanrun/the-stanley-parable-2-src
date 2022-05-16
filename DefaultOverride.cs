using System;
using UnityEngine;

[Serializable]
public class DefaultOverride
{
	public bool MatchesPlatform(RuntimePlatform platformToCheck)
	{
		return this.platform == platformToCheck;
	}

	public bool OverrideExistsForBundle(ReleaseBundle bundleToCheck, out BundleConfiguration overrideConfiguration)
	{
		for (int i = 0; i < this.CustomBundleConfigurations.Length; i++)
		{
			BundleConfiguration bundleConfiguration = this.CustomBundleConfigurations[i];
			if (bundleConfiguration.MatchesReferences(bundleToCheck))
			{
				overrideConfiguration = bundleConfiguration;
				return true;
			}
		}
		overrideConfiguration = null;
		return false;
	}

	[SerializeField]
	private RuntimePlatform platform;

	[SerializeField]
	public bool loadBundlesIntoMemory;

	[SerializeField]
	private BundleConfiguration[] CustomBundleConfigurations;
}
