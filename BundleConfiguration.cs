using System;
using UnityEngine;

[Serializable]
public class BundleConfiguration
{
	public string GetCustomVariant()
	{
		return this.customVariant;
	}

	public bool MatchesReferences(ReleaseBundle bundleToCheck)
	{
		return this.bundleReference == bundleToCheck;
	}

	[SerializeField]
	private ReleaseBundle bundleReference;

	[SerializeField]
	private string customVariant;

	public BundleConfiguration.IncludeOptions IncludeOption;

	public enum IncludeOptions
	{
		UseDefault,
		UseVariant,
		DoNotInclude
	}
}
