using System;
using Nest.Components;

public class LogicRelay : HammerEntity
{
	private void Start()
	{
		if (this.auto)
		{
			base.FireOutput(Outputs.OnMapSpawn);
		}
	}

	public override void Input_Kill()
	{
		this.killed = true;
		base.Input_Kill();
	}

	public void Input_Trigger()
	{
		if (this.isEnabled && !this.killed)
		{
			base.FireOutput(Outputs.OnTrigger);
			for (int i = 0; i < this.manualOutputs.Length; i++)
			{
				if (this.manualOutputs[i] != null)
				{
					this.manualOutputs[i].Invoke();
				}
			}
		}
	}

	public bool auto;

	public bool killed;

	public NestInput[] manualOutputs;
}
