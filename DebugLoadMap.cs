using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class DebugLoadMap : MonoBehaviour
{
	public void TryLoadMap()
	{
		if (Singleton<GameMaster>.Instance.ChangeLevel(this.levelNameInput.text.Trim(new char[]
		{
			'​'
		}), true))
		{
			this.levelNameInput.color = Color.black;
			this.OnChangeLevel.Invoke();
			Singleton<GameMaster>.Instance.ClosePauseMenu(false);
			return;
		}
		this.levelNameInput.color = Color.red;
	}

	[SerializeField]
	private TextMeshProUGUI levelNameInput;

	[SerializeField]
	private UnityEvent OnChangeLevel;
}
