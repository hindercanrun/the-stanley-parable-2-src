using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LogicCase))]
public class LogicCaseCustomWeights : MonoBehaviour
{
	public List<LogicCaseCustomWeights.CaseWeight> caseWeightOverrides;

	[Serializable]
	public class CaseWeight
	{
		public Outputs caseNumber = Outputs.OnCase01;

		public float weight = 1f;
	}
}
