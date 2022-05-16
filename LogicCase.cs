using System;
using System.Collections.Generic;
using UnityEngine;

public class LogicCase : HammerEntity
{
	private void Awake()
	{
		for (int i = 0; i < this.expandedConnections.Count; i++)
		{
			int j = 0;
			while (j < this.outCases.Length)
			{
				if (this.outCases[j] == this.expandedConnections[i].output)
				{
					if (!this.firableRandomCases.Contains(this.expandedConnections[i].output))
					{
						this.firableRandomCases.Add(this.expandedConnections[i].output);
						break;
					}
					break;
				}
				else
				{
					j++;
				}
			}
		}
		if (this.debugPickRandomOverrideConfigurable != null)
		{
			this.debugPickRandomOverrideConfigurable.SetNewMaxValue(this.firableRandomCases.Count);
		}
	}

	public void Input_InValue(string value)
	{
		bool flag = true;
		for (int i = 0; i < this.cases.Length; i++)
		{
			if (this.cases[i] == value)
			{
				base.FireOutput(this.outCases[i]);
			}
			flag = false;
		}
		if (flag)
		{
			base.FireOutput(Outputs.OnDefault);
		}
	}

	public int GetRandomFirableCaseIndex()
	{
		LogicCaseCustomWeights component = base.GetComponent<LogicCaseCustomWeights>();
		if (component == null)
		{
			return Random.Range(0, this.firableRandomCases.Count);
		}
		Dictionary<Outputs, float> dictionary = new Dictionary<Outputs, float>();
		foreach (Outputs key in this.firableRandomCases)
		{
			dictionary[key] = 1f;
		}
		foreach (LogicCaseCustomWeights.CaseWeight caseWeight in component.caseWeightOverrides)
		{
			if (dictionary.ContainsKey(caseWeight.caseNumber))
			{
				dictionary[caseWeight.caseNumber] = caseWeight.weight;
			}
		}
		float num = 0f;
		foreach (Outputs key2 in this.firableRandomCases)
		{
			num += dictionary[key2];
		}
		float num2 = Random.Range(0f, num);
		for (int i = 0; i < this.firableRandomCases.Count; i++)
		{
			Outputs key3 = this.firableRandomCases[i];
			num2 -= dictionary[key3];
			if (num2 <= 0f)
			{
				return i;
			}
		}
		Debug.LogError("Problem in weighted random case picker" + base.name, this);
		return 0;
	}

	public void Input_PickRandom()
	{
		int index = this.GetRandomFirableCaseIndex();
		if (this.debugPickRandomOverrideConfigurable != null && this.debugPickRandomOverrideConfigurable.GetIntValue() != 0)
		{
			index = this.debugPickRandomOverrideConfigurable.GetIntValue() - 1;
		}
		base.FireOutput(this.firableRandomCases[index]);
	}

	public void Input_PickRandomShuffle()
	{
		if (this.shuffleIndex == this.shuffledCases.Count)
		{
			this.shuffledCases = new List<Outputs>();
			List<Outputs> list = new List<Outputs>(this.firableRandomCases);
			while (list.Count > 0)
			{
				int index = Random.Range(0, list.Count);
				this.shuffledCases.Add(list[index]);
				list.RemoveAt(index);
			}
			this.shuffleIndex = 0;
		}
		base.FireOutput(this.shuffledCases[this.shuffleIndex]);
		this.shuffleIndex++;
	}

	public string[] cases = new string[16];

	[Header("Overrides case (0 = pick random as normal, 1-16 is OnCase01-16")]
	public IntConfigurable debugPickRandomOverrideConfigurable;

	private Outputs[] outCases = new Outputs[]
	{
		Outputs.OnCase01,
		Outputs.OnCase02,
		Outputs.OnCase03,
		Outputs.OnCase04,
		Outputs.OnCase05,
		Outputs.OnCase06,
		Outputs.OnCase07,
		Outputs.OnCase08,
		Outputs.OnCase09,
		Outputs.OnCase10,
		Outputs.OnCase11,
		Outputs.OnCase12,
		Outputs.OnCase13,
		Outputs.OnCase14,
		Outputs.OnCase15,
		Outputs.OnCase16
	};

	private List<Outputs> firableRandomCases = new List<Outputs>();

	private List<Outputs> shuffledCases = new List<Outputs>();

	private int shuffleIndex;
}
