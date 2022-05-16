using System;
using System.Collections;
using Nest.Integrations;
using UnityEngine;

[Serializable]
public class Phase : BaseIntegration
{
	private void Awake()
	{
		this.Index = base.transform.GetSiblingIndex();
	}

	public IEnumerator PhaseRoutine(PhaseItem[] itemArray)
	{
		int num;
		for (int i = 0; i < itemArray.Length; i = num + 1)
		{
			itemArray[i].PhaseEvent.Invoke();
			float duration = itemArray[i].Duration;
			if (duration > 0f)
			{
				yield return new WaitForSeconds(duration);
			}
			num = i;
		}
		yield break;
	}

	public PhaseItem[] EnterItems;

	public PhaseItem[] LeaveItems;

	public int Index;
}
