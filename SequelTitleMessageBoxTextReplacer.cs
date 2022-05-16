using System;
using UnityEngine;

public class SequelTitleMessageBoxTextReplacer : MonoBehaviour, IMessageBoxKeyReplacer
{
	public string DoReplaceStep(string originalText)
	{
		return SequelTools.DoStandardSequelReplacementStep(originalText, this.sequelCountConfigurable, this.prefixIndexConfigurable, this.postfixIndexConfigurable);
	}

	public IntConfigurable sequelCountConfigurable;

	public IntConfigurable prefixIndexConfigurable;

	public IntConfigurable postfixIndexConfigurable;
}
