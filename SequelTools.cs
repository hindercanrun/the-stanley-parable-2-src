using System;
using I2.Loc;

public static class SequelTools
{
	public static string PrefixTerm(int i)
	{
		return string.Format("{0}_{1:00}", SequelTools.PrefixTermBase, i);
	}

	public static string PostfixTerm(int i)
	{
		return string.Format("{0}_{1:00}", SequelTools.PostfixTermBase, i);
	}

	public static string PrefixLocalizedText(int i)
	{
		return LocalizationManager.GetTranslation(SequelTools.PrefixTerm(i), true, 0, true, false, null, null);
	}

	public static string PostfixLocalizedText(int i)
	{
		return LocalizationManager.GetTranslation(SequelTools.PostfixTerm(i), true, 0, true, false, null, null);
	}

	public static string DoStandardSequelReplacementStep(string originalText, IntConfigurable sequelCountConfigurable, IntConfigurable prefixIndexConfigurable, IntConfigurable postfixIndexConfigurable)
	{
		int intValue = prefixIndexConfigurable.GetIntValue();
		string newValue = "";
		if (intValue != -1)
		{
			newValue = SequelTools.PrefixLocalizedText(intValue);
		}
		int intValue2 = postfixIndexConfigurable.GetIntValue();
		string newValue2 = "";
		if (intValue2 != -1)
		{
			newValue2 = SequelTools.PostfixLocalizedText(intValue2);
		}
		return originalText.Replace("\\n", "\n").Replace("%!N!%", sequelCountConfigurable.GetIntValue().ToString()).Replace("%!P!%", newValue).Replace("%!S!%", newValue2);
	}

	public static string PrefixTermBase = "General_Title_Gag_prefix";

	public static string PostfixTermBase = "General_Title_Gag_postfix";
}
