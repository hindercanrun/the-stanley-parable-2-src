using System;

public class TriggerChangelevel : TriggerMultiple
{
	public void Input_ChangeLevel()
	{
		if (this.isEnabled)
		{
			Singleton<GameMaster>.Instance.ChangeLevel(this.destination, true);
		}
	}

	public void Input_ChangeDestination(string newDestination)
	{
		this.destination = newDestination;
	}

	protected override void StartTouch()
	{
		if (!this.isEnabled)
		{
			return;
		}
		base.StartTouch();
		Singleton<GameMaster>.Instance.ChangeLevel(this.destination, true);
	}

	public string destination = "";
}
