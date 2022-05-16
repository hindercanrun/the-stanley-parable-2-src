using System;
using System.Collections;
using UnityEngine;

public class PhaseManager : MonoBehaviour
{
	private void Awake()
	{
		this.phaseArray = base.GetComponentsInChildren<Phase>();
	}

	private void Start()
	{
		if (this.AutoStart)
		{
			this.StartFirstPhase();
		}
	}

	public void Disable()
	{
		this.disabled = true;
	}

	public void StartFirstPhase()
	{
		if (this.disabled)
		{
			return;
		}
		this.currentPhase = this.phaseArray[0];
		if (this.currentSwitchRoutine != null)
		{
			base.StopCoroutine(this.currentSwitchRoutine);
		}
		this.currentSwitchRoutine = base.StartCoroutine(this.SwitchRoutine(this.currentPhase, true));
		this.currentPhaseIndex = 0;
	}

	public void AdvanceToNextPhase()
	{
		if (this.disabled)
		{
			return;
		}
		if (this.phaseArray.Length > this.currentPhaseIndex + 1)
		{
			this.currentPhaseIndex++;
			this.EnterPhase(this.phaseArray[this.currentPhaseIndex]);
		}
	}

	public void AdvanceToLastPhase()
	{
		if (this.disabled)
		{
			return;
		}
		if (this.currentPhaseIndex > 0)
		{
			this.currentPhaseIndex--;
			this.EnterPhase(this.phaseArray[this.currentPhaseIndex]);
		}
	}

	public void EnterPhase(Phase newPhase)
	{
		if (this.disabled)
		{
			return;
		}
		if (this.currentSwitchRoutine != null)
		{
			base.StopCoroutine(this.currentSwitchRoutine);
		}
		this.currentSwitchRoutine = base.StartCoroutine(this.SwitchRoutine(newPhase, false));
		this.currentPhaseIndex = newPhase.Index;
	}

	private IEnumerator SwitchRoutine(Phase newPhase, bool first = false)
	{
		if (this.currentPhaseRoutine != null)
		{
			base.StopCoroutine(this.currentPhaseRoutine);
		}
		if (this.disabled)
		{
			yield break;
		}
		if (!first && this.currentPhase != null)
		{
			this.currentPhaseRoutine = base.StartCoroutine(this.PhaseRoutine(this.currentPhase.LeaveItems));
			yield return this.currentPhaseRoutine;
		}
		this.currentPhase = newPhase;
		this.currentPhaseRoutine = base.StartCoroutine(this.PhaseRoutine(this.currentPhase.EnterItems));
		yield return this.currentPhaseRoutine;
		yield break;
	}

	private IEnumerator PhaseRoutine(PhaseItem[] itemArray)
	{
		while (Time.timeScale == 0f)
		{
			yield return null;
		}
		int num;
		for (int i = 0; i < itemArray.Length; i = num + 1)
		{
			if (itemArray[i].Mode != PhaseItem.PhaseMode.Disabled)
			{
				itemArray[i].PhaseEvent.Invoke();
				float itemDuration = itemArray[i].Duration;
				if (itemDuration > 0f)
				{
					float itemCompletion = 0f;
					while (itemCompletion < itemDuration)
					{
						itemCompletion += Time.deltaTime * PhaseManager.GameSpeed;
						yield return null;
					}
				}
			}
			num = i;
		}
		this.currentPhaseRoutine = null;
		yield break;
	}

	private void OnDestroy()
	{
		if (this.currentSwitchRoutine != null)
		{
			base.StopCoroutine(this.currentSwitchRoutine);
		}
		if (this.currentPhaseRoutine != null)
		{
			base.StopCoroutine(this.currentPhaseRoutine);
		}
	}

	public static float GameSpeed = 1f;

	private Phase[] phaseArray;

	private Phase currentPhase;

	private int currentPhaseIndex;

	public bool AutoStart = true;

	private bool disabled;

	private Coroutine currentPhaseRoutine;

	private Coroutine currentSwitchRoutine;
}
