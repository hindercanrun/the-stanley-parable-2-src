using System;
using UnityEngine;

public class FlockScare : MonoBehaviour
{
	private void CheckProximityToLandingSpots()
	{
		this.IterateLandingSpots();
		if (this.currentController._activeLandingSpots > 0 && this.CheckDistanceToLandingSpot(this.landingSpotControllers[this.lsc]))
		{
			this.landingSpotControllers[this.lsc].ScareAll();
		}
		base.Invoke("CheckProximityToLandingSpots", this.scareInterval);
	}

	private void IterateLandingSpots()
	{
		this.ls += this.checkEveryNthLandingSpot;
		this.currentController = this.landingSpotControllers[this.lsc];
		int childCount = this.currentController.transform.childCount;
		if (this.ls > childCount - 1)
		{
			this.ls -= childCount;
			if (this.lsc < this.landingSpotControllers.Length - 1)
			{
				this.lsc++;
				return;
			}
			this.lsc = 0;
		}
	}

	private bool CheckDistanceToLandingSpot(LandingSpotController lc)
	{
		Transform child = lc.transform.GetChild(this.ls);
		return child.GetComponent<LandingSpot>().landingChild != null && (child.position - base.transform.position).sqrMagnitude < this.distanceToScare * this.distanceToScare;
	}

	private void Invoker()
	{
		for (int i = 0; i < this.InvokeAmounts; i++)
		{
			float num = this.scareInterval / (float)this.InvokeAmounts * (float)i;
			base.Invoke("CheckProximityToLandingSpots", this.scareInterval + num);
		}
	}

	private void OnEnable()
	{
		base.CancelInvoke("CheckProximityToLandingSpots");
		if (this.landingSpotControllers.Length != 0)
		{
			this.Invoker();
		}
	}

	private void OnDisable()
	{
		base.CancelInvoke("CheckProximityToLandingSpots");
	}

	public LandingSpotController[] landingSpotControllers;

	public float scareInterval = 0.1f;

	public float distanceToScare = 2f;

	public int checkEveryNthLandingSpot = 1;

	public int InvokeAmounts = 1;

	private int lsc;

	private int ls;

	private LandingSpotController currentController;
}
