using System;

public class FullscreenModeConfigurator : Configurator
{
	public override void ApplyData()
	{
	}

	public override void PrintValue(Configurable _configurable)
	{
		int intValue = this.configurable.GetIntValue();
		string arg = "";
		switch (intValue)
		{
		case 0:
			arg = "Menu_Fullscreen_Mode_Fullscreen";
			break;
		case 1:
			arg = "Menu_Fullscreen_Mode_Borderless";
			break;
		case 2:
			arg = "Menu_Fullscreen_Mode_Windowed";
			break;
		}
		this.OnPrintValue.Invoke(arg);
	}

	public void FunctionBecauesTheFuckingThingWontFuckingCallAndIDontKnowWhy(string yeah_the_fucking_string)
	{
	}
}
