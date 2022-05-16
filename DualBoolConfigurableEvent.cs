using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DualBoolConfigurableEvent : MonoBehaviour
{
	private IEnumerable<BooleanConfigurable> Configurables
	{
		get
		{
			yield return this.configurableA;
			yield return this.configurableB;
			yield break;
		}
	}

	private void Awake()
	{
		foreach (BooleanConfigurable booleanConfigurable in this.Configurables)
		{
			if (booleanConfigurable != null)
			{
				BooleanConfigurable booleanConfigurable2 = booleanConfigurable;
				booleanConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Combine(booleanConfigurable2.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
				booleanConfigurable.Init();
			}
		}
	}

	private void Start()
	{
		foreach (BooleanConfigurable booleanConfigurable in this.Configurables)
		{
			if (booleanConfigurable != null)
			{
				booleanConfigurable.Init();
			}
		}
		if (this.invokeOnStart)
		{
			this.Invoke();
		}
	}

	private void OnDestroy()
	{
		foreach (BooleanConfigurable booleanConfigurable in this.Configurables)
		{
			if (booleanConfigurable != null)
			{
				BooleanConfigurable booleanConfigurable2 = booleanConfigurable;
				booleanConfigurable2.OnValueChanged = (Action<LiveData>)Delegate.Remove(booleanConfigurable2.OnValueChanged, new Action<LiveData>(this.OnValueChanged));
			}
		}
	}

	private void OnValueChanged(LiveData data)
	{
		if (base.enabled && this.invokeOnValueChange)
		{
			this.Invoke();
		}
	}

	public void Invoke()
	{
		bool flag = this.configurableA != null && this.configurableA.GetBooleanValue();
		bool flag2 = this.configurableB != null && this.configurableB.GetBooleanValue();
		bool flag3;
		switch (this.operation)
		{
		case DualBoolConfigurableEvent.Operator.AND:
			flag3 = (flag && flag2);
			break;
		case DualBoolConfigurableEvent.Operator.OR:
			flag3 = (flag || flag2);
			break;
		case DualBoolConfigurableEvent.Operator.NAND:
			flag3 = (!flag || !flag2);
			break;
		case DualBoolConfigurableEvent.Operator.NOR:
			flag3 = (!flag && !flag2);
			break;
		case DualBoolConfigurableEvent.Operator.XOR:
			flag3 = (flag ^ flag2);
			break;
		case DualBoolConfigurableEvent.Operator.NXOR_EQUALITY:
			flag3 = (flag == flag2);
			break;
		case DualBoolConfigurableEvent.Operator.ALWAYS_TRUE:
			flag3 = true;
			break;
		case DualBoolConfigurableEvent.Operator.ALWAYS_FALSE:
			flag3 = true;
			break;
		case DualBoolConfigurableEvent.Operator.ONLY_A:
			flag3 = flag;
			break;
		case DualBoolConfigurableEvent.Operator.ONLY_B:
			flag3 = flag2;
			break;
		case DualBoolConfigurableEvent.Operator.ONLY_A_NEGATED:
			flag3 = !flag;
			break;
		case DualBoolConfigurableEvent.Operator.ONLY_B_NEGATED:
			flag3 = !flag2;
			break;
		case DualBoolConfigurableEvent.Operator.A__AND__B_NEG:
			flag3 = (flag & !flag2);
			break;
		case DualBoolConfigurableEvent.Operator.A_NEG__AND__B:
			flag3 = (!flag && flag2);
			break;
		case DualBoolConfigurableEvent.Operator.A__OR__B_NEG:
			flag3 = (flag | !flag2);
			break;
		case DualBoolConfigurableEvent.Operator.A_NEG__OR__B:
			flag3 = (!flag || flag2);
			break;
		default:
			return;
		}
		DualBoolConfigurableEvent.BooleanUnityEvent booleanUnityEvent = this.onEvaluate;
		if (booleanUnityEvent != null)
		{
			booleanUnityEvent.Invoke(flag3);
		}
		DualBoolConfigurableEvent.BooleanUnityEvent booleanUnityEvent2 = this.onEvaluateNegated;
		if (booleanUnityEvent2 != null)
		{
			booleanUnityEvent2.Invoke(!flag3);
		}
		if (flag3)
		{
			UnityEvent unityEvent = this.onConditionMet;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
			return;
		}
		else
		{
			UnityEvent unityEvent2 = this.onConditionNotMet;
			if (unityEvent2 == null)
			{
				return;
			}
			unityEvent2.Invoke();
			return;
		}
	}

	[SerializeField]
	private BooleanConfigurable configurableA;

	[SerializeField]
	private DualBoolConfigurableEvent.Operator operation;

	[SerializeField]
	private BooleanConfigurable configurableB;

	[SerializeField]
	private bool invokeOnValueChange;

	[SerializeField]
	private bool invokeOnStart;

	[SerializeField]
	private DualBoolConfigurableEvent.BooleanUnityEvent onEvaluate;

	[SerializeField]
	private DualBoolConfigurableEvent.BooleanUnityEvent onEvaluateNegated;

	[SerializeField]
	private UnityEvent onConditionMet;

	[SerializeField]
	private UnityEvent onConditionNotMet;

	[Serializable]
	public class BooleanUnityEvent : UnityEvent<bool>
	{
	}

	public enum Operator
	{
		AND,
		OR,
		NAND,
		NOR,
		XOR,
		NXOR_EQUALITY,
		ALWAYS_TRUE,
		ALWAYS_FALSE,
		ONLY_A,
		ONLY_B,
		ONLY_A_NEGATED,
		ONLY_B_NEGATED,
		A__AND__B_NEG,
		A_NEG__AND__B,
		A__OR__B_NEG,
		A_NEG__OR__B
	}
}
