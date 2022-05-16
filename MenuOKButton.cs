using System;
using UnityEngine;

public class MenuOKButton : MenuToggleButton
{
	public override void OnClick(Vector3 point = default(Vector3))
	{
		base.OnClick(point);
		if (this.original)
		{
			for (int i = 0; i < this.toSave.Length; i++)
			{
				this.toSave[i].SaveChange();
			}
			Singleton<GameMaster>.Instance.WriteAllPrefs();
		}
	}

	public MenuButton[] toSave;
}
