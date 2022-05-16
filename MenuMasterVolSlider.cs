using System;

public class MenuMasterVolSlider : MenuSlider
{
	private void OnEnable()
	{
		base.position = Singleton<GameMaster>.Instance.masterVolume;
	}

	protected override void Changed()
	{
		base.Changed();
	}

	public override void SaveChange()
	{
		base.SaveChange();
		if (this.original)
		{
			Singleton<GameMaster>.Instance.SetMasterVolume(base.position);
		}
	}
}
