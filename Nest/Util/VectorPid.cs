using System;
using UnityEngine;

namespace Nest.Util
{
	public class VectorPid
	{
		public VectorPid(float pFactor, float factor, float dFactor)
		{
			this.PFactor = pFactor;
			this.IFactor = factor;
			this.DFactor = dFactor;
		}

		public Vector3 Update(Vector3 currentError, float timeFrame)
		{
			this._integral += currentError * timeFrame;
			Vector3 a = (currentError - this._lastError) / timeFrame;
			this._lastError = currentError;
			return currentError * this.PFactor + this._integral * this.IFactor + a * this.DFactor;
		}

		public float PFactor;

		public float IFactor;

		public float DFactor;

		private Vector3 _integral;

		private Vector3 _lastError;
	}
}
