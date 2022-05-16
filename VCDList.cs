using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "VCD List")]
public class VCDList : ScriptableObject
{
	public VCDList()
	{
	}

	public VCDList(VCDList original)
	{
		this.scripts = original.scripts;
		this.text = original.text;
	}

	public List<string> scripts = new List<string>();

	public List<string> text = new List<string>();
}
