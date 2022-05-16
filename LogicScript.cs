using System;
using System.Collections.Generic;
using UnityEngine;

public class LogicScript : HammerEntity
{
	private void OnValidate()
	{
		this.groups = new List<LogicScript.GroupList>();
		HammerEntity[] array = Object.FindObjectsOfType<HammerEntity>();
		for (int i = 0; i < this.entityGroups.Length; i++)
		{
			this.groups.Add(new LogicScript.GroupList());
			for (int j = 0; j < array.Length; j++)
			{
				if (array[j].name == this.entityGroups[i])
				{
					this.groups[i].group.Add(array[j]);
				}
			}
		}
	}

	public void Input_EnableScript()
	{
		this.script.enabled = true;
	}

	public void Input_RunScriptCode(string command)
	{
		this.script.ParseCommand(command);
	}

	public string[] entityGroups;

	public LogicScriptBase script;

	public List<LogicScript.GroupList> groups = new List<LogicScript.GroupList>();

	[Serializable]
	public class GroupList
	{
		public List<HammerEntity> group = new List<HammerEntity>();
	}
}
