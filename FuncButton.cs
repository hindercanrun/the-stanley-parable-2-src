using System;
using System.Collections;

public class FuncButton : HammerEntity
{
	private void Awake()
	{
		if (this.startLocked)
		{
			this.locked = true;
		}
	}

	public void Input_Lock()
	{
		this.locked = true;
	}

	public void Input_Unlock()
	{
		this.locked = false;
	}

	public override void Use()
	{
		if (this.depressed)
		{
			return;
		}
		if (!this.locked)
		{
			base.FireOutput(Outputs.OnPressed);
			if (this.delayBeforeReset != 0f)
			{
				this.depressed = true;
				if (this.delayBeforeReset > 0f)
				{
					base.StartCoroutine(this.Depressed());
				}
			}
		}
	}

	private IEnumerator Depressed()
	{
		yield return new WaitForGameSeconds(this.delayBeforeReset);
		this.depressed = false;
		yield break;
	}

	public bool startLocked;

	public bool locked;

	public float delayBeforeReset = -1f;

	private bool depressed;
}
