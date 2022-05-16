using System;
using System.Collections.Generic;
using UnityEngine;

public class SequelNameSelectionRandomizer : MonoBehaviour
{
	[ContextMenu("Randomize")]
	public void Randomize()
	{
		this.RandomizeGroup(this.prefixButtons, this.prefixIndex);
		this.RandomizeGroup(this.postFixButtons, this.postfixIndex);
	}

	private void RandomizeGroup(SequelToggleButton[] buttons, IntConfigurable pfindex)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < pfindex.MaxValue; i++)
		{
			list.Add(i);
		}
		int[] array = new int[buttons.Length];
		for (int j = 0; j < buttons.Length; j++)
		{
			int index = Random.Range(0, list.Count);
			array[j] = list[index];
			list.RemoveAt(index);
		}
		for (int k = 0; k < buttons.Length; k++)
		{
			buttons[k].SetIndex(array[k]);
		}
	}

	public SequelToggleButton[] prefixButtons;

	public IntConfigurable prefixIndex;

	public SequelToggleButton[] postFixButtons;

	public IntConfigurable postfixIndex;
}
