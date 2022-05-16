using System;

public class LogicBranch : HammerEntity
{
	public int value { get; private set; }

	private void Awake()
	{
		this.value = this.initialValue;
	}

	public void Input_SetValue(float val)
	{
		this.value = (int)val;
	}

	public void Input_SetValueTest(float val)
	{
		this.Input_SetValue(val);
		this.Input_Test();
	}

	public void Input_Toggle()
	{
		if (this.value == 1)
		{
			this.value = 0;
			return;
		}
		this.value = 1;
	}

	public void Input_ToggleTest()
	{
		this.Input_Toggle();
		this.Input_Test();
	}

	public void Input_Test()
	{
		if (this.value == 1)
		{
			base.FireOutput(Outputs.OnTrue);
			return;
		}
		base.FireOutput(Outputs.OnFalse);
	}

	public int initialValue;
}
