using System;
using System.Collections.Generic;
using UnityEngine;

public class LogicBranchListener : HammerEntity
{
	private void OnValidate()
	{
		this.branches = new List<LogicBranch>();
		this.values = new List<int>();
		for (int i = 0; i < this.branchStrings.Length; i++)
		{
			GameObject gameObject = GameObject.Find(this.branchStrings[i]);
			if (gameObject)
			{
				LogicBranch component = gameObject.GetComponent<LogicBranch>();
				if (component)
				{
					this.branches.Add(component);
					this.values.Add(component.initialValue);
				}
			}
		}
	}

	private void Update()
	{
		bool flag = false;
		int num = 0;
		for (int i = 0; i < this.branches.Count; i++)
		{
			if (this.branches[i] != null)
			{
				if (this.branches[i].value != this.values[i])
				{
					this.values[i] = this.branches[i].value;
					flag = true;
				}
				num += this.values[i];
			}
		}
		if (flag)
		{
			if (num == this.values.Count)
			{
				base.FireOutput(Outputs.OnAllTrue);
				return;
			}
			if (num == 0)
			{
				base.FireOutput(Outputs.OnAllFalse);
				return;
			}
			base.FireOutput(Outputs.OnMixed);
		}
	}

	public string[] branchStrings;

	public List<LogicBranch> branches;

	[SerializeField]
	private List<int> values;
}
