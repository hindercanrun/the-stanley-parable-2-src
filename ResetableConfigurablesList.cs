using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResetableConfigurablesList.asset", menuName = "Data/Resetable Configurables List")]
public class ResetableConfigurablesList : ScriptableObject
{
	public void ResetAllConfigurables()
	{
		foreach (Configurable configurable in this.allConfigurables)
		{
			configurable.Init();
			configurable.SetNewConfiguredValue(configurable.CreateDefaultLiveData());
			configurable.SaveToDiskAll();
		}
	}

	public void ResetAll()
	{
		this.ResetAllConfigurables();
		this.DestroyAllGameObjects();
	}

	private void DestroyAllGameObjects()
	{
		Singleton<ChoreoMaster>.Instance.DropAll();
		StanleyController.Instance.gameObject.SetActive(false);
		Singleton<GameMaster>.Instance.ClosePauseMenu(true);
		Singleton<GameMaster>.Instance.ReInit();
		Singleton<GameMaster>.Instance.ChangeLevel("Settings_UD_MASTER", true);
	}

	[Header("Right-Click and Reimport any configurables that do not show up on the list to allow to find them")]
	[InspectorButton("FindAllConfigurables", null)]
	public bool findAll;

	public List<Configurable> allConfigurables;

	public List<Configurable> excludeTheseConfigurablesInFind;
}
