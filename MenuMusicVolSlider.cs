using System;

public class MenuMusicVolSlider : MenuSlider
{
	private void OnEnable()
	{
		base.position = Singleton<GameMaster>.Instance.musicVolume;
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
			Singleton<GameMaster>.Instance.SetMusicVolume(base.position);
		}
	}
}
