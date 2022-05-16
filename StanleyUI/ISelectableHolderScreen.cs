using System;
using UnityEngine.UI;

namespace StanleyUI
{
	public interface ISelectableHolderScreen
	{
		Selectable DefaultSelectable { get; }

		Selectable LastSelectable { get; set; }
	}
}
