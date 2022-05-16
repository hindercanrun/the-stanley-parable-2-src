using System;
using TMPro;
using UnityEngine;

public class ResolutionConfigurator : Configurator
{
	public new void Start()
	{
		this.PrintValue(this.configurable);
		this.TMProForceMeshUpdate();
		SimpleEvent simpleEvent = this.onResolutionChange;
		simpleEvent.OnCall = (Action)Delegate.Combine(simpleEvent.OnCall, new Action(this.OnResolutionChangedEvent));
		base.Start();
	}

	private void OnDestroy()
	{
		SimpleEvent simpleEvent = this.onResolutionChange;
		simpleEvent.OnCall = (Action)Delegate.Combine(simpleEvent.OnCall, new Action(this.OnResolutionChangedEvent));
	}

	private void OnResolutionChangedEvent()
	{
		this.TMProForceMeshUpdate();
	}

	public void TMProForceMeshUpdate()
	{
		TextMeshProUGUI[] componentsInChildren = Singleton<GameMaster>.Instance.MenuManager.GetComponent<Canvas>().GetComponentsInChildren<TextMeshProUGUI>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ForceMeshUpdate(true, true);
		}
	}

	public override void ApplyData()
	{
	}

	private static int GCD(int a, int b)
	{
		if (a == 0)
		{
			return b;
		}
		if (b == 0)
		{
			return a;
		}
		if (a == b)
		{
			return a;
		}
		if (a > b)
		{
			return ResolutionConfigurator.GCD(a - b, b);
		}
		return ResolutionConfigurator.GCD(a, b - a);
	}

	private static string GetAspectRatioString(int w, int h)
	{
		int num = ResolutionConfigurator.GCD(w, h);
		int num2 = w / num;
		int num3 = h / num;
		if (num2 == 8 && num3 == 5)
		{
			num2 *= 2;
			num3 *= 2;
		}
		return num2 + ":" + num3;
	}

	public override void PrintValue(Configurable _configurable)
	{
		int intValue = this.configurable.GetIntValue();
		Resolution b = ResolutionController.Instance.availableResolutions[intValue];
		string text = ResolutionController.CompareResolutions(Screen.currentResolution, b) ? "" : "*";
		string aspectRatioString = ResolutionConfigurator.GetAspectRatioString(b.width, b.height);
		this.OnPrintValue.Invoke(string.Format("{0} x {1} @ {2}Hz{3} ({4})", new object[]
		{
			b.width,
			b.height,
			b.refreshRate,
			text,
			aspectRatioString
		}));
	}

	public void FunctionBecauesTheFuckingThingWontFuckingCallAndIDontKnowWhy(string yeah_the_fucking_string)
	{
	}

	[SerializeField]
	private TextMeshProUGUI theLabelThatWontWorkBcItsBeingStupid;

	[SerializeField]
	private SimpleEvent onResolutionChange;
}
