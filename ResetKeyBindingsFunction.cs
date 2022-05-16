using System;
using UnityEngine;

public class ResetKeyBindingsFunction : MonoBehaviour
{
	public void ResetKeyBindings()
	{
		Singleton<GameMaster>.Instance.stanleyActions.ResetKeyBindings(this.keybindingString);
	}

	public void LoadKeyBindings()
	{
		Singleton<GameMaster>.Instance.stanleyActions.LoadCustomKeyBindings(this.keybindingString);
	}

	public StringConfigurable keybindingString;
}
