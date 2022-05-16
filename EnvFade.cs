using System;
using UnityEngine;

public class EnvFade : HammerEntity
{
	public void Input_Fade()
	{
		Singleton<GameMaster>.Instance.BeginFade(this.renderColor, this.inDuration, this.holdDuration, this.fadeFrom, this.stayOut);
		this.LogFade();
	}

	public void Input_FadeReverse()
	{
		Singleton<GameMaster>.Instance.BeginFade(this.renderColor, this.outDuration, this.holdDuration, true, false);
		this.LogFade();
	}

	private void LogFade()
	{
		string text = "";
		if (!this.fadeFrom)
		{
			text += "then ";
			if (this.stayOut)
			{
				text += "hold for all eternity";
				return;
			}
			if (this.holdDuration > 0f)
			{
				text = string.Concat(new object[]
				{
					text,
					"hold for",
					this.holdDuration,
					" seconds"
				});
				return;
			}
			text += "snap back to normal";
		}
	}

	public Color renderColor = Color.black;

	public float inDuration = 2f;

	public float outDuration = 2f;

	public float holdDuration;

	public bool fadeFrom;

	public bool stayOut;
}
