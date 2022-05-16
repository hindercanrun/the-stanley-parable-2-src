using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ConfigurableCountTest : MonoBehaviour
{
	public void TestConfigurableWeightSum()
	{
		int num = 0;
		foreach (ConfigurableCountTest.ConfigurableIntergerPair configurableIntergerPair in this.weightedConfigurables)
		{
			bool booleanValue = configurableIntergerPair.configurable.GetBooleanValue();
			configurableIntergerPair.LastWeightedValue = booleanValue;
			if (booleanValue)
			{
				num += configurableIntergerPair.weighting;
			}
		}
		this.DEBUG_LastTestedWeight = num;
		int num2 = this.hasPlayedTSPBeforeConfigurable.GetBooleanValue() ? this.thresholdIfPlayedTSPBefore : this.thresholdIfNotPlayedTSPBefore;
		if (num >= num2)
		{
			if (this.OnSumAboveOrEqualThreshold != null)
			{
				this.OnSumAboveOrEqualThreshold.Invoke();
				return;
			}
			if (this.OnSumBelowThreshold != null)
			{
				this.OnSumBelowThreshold.Invoke();
			}
		}
	}

	private void OnValidate()
	{
		while (this.configurableToAdd != null && this.configurableToAdd.Count > 0)
		{
			this.weightedConfigurables.Add(new ConfigurableCountTest.ConfigurableIntergerPair
			{
				configurable = this.configurableToAdd[0]
			});
			this.configurableToAdd.RemoveAt(0);
		}
	}

	[SerializeField]
	private List<ConfigurableCountTest.ConfigurableIntergerPair> weightedConfigurables = new List<ConfigurableCountTest.ConfigurableIntergerPair>();

	[InspectorButton("TestConfigurableWeightSum", "Perform manual Test")]
	[SerializeField]
	private Configurable hasPlayedTSPBeforeConfigurable;

	[SerializeField]
	private int thresholdIfPlayedTSPBefore = 10;

	[SerializeField]
	private int thresholdIfNotPlayedTSPBefore = 8;

	[SerializeField]
	private UnityEvent OnSumAboveOrEqualThreshold;

	[SerializeField]
	private UnityEvent OnSumBelowThreshold;

	[Header("Easy Add Multiple Configurables")]
	[SerializeField]
	private List<Configurable> configurableToAdd = new List<Configurable>();

	[Header("DEBUG DO NOT EDIT")]
	public int DEBUG_LastTestedWeight;

	[Serializable]
	private class ConfigurableIntergerPair
	{
		public bool LastWeightedValue { get; set; }

		public Configurable configurable;

		public int weighting = 1;
	}
}
