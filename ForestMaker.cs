using System;
using UnityEngine;

public class ForestMaker : MonoBehaviour
{
	private void Start()
	{
		if (this.m_treePrefab == null)
		{
			return;
		}
		this.m_ground.transform.localScale = new Vector3((float)(this.m_amount * 10), 1f, (float)(this.m_amount * 5) * 1.866f);
		for (int i = -this.m_amount / 2; i <= this.m_amount / 2; i++)
		{
			for (int j = -this.m_amount / 2; j <= this.m_amount / 2; j++)
			{
				if (Random.Range(0f, 1f) <= 0.5f)
				{
					GameObject gameObject = Object.Instantiate<GameObject>(this.m_treePrefab);
					Vector3 zero = Vector3.zero;
					zero.x = ((float)i + (float)j * 0.5f - (float)((int)(((j < 0) ? ((float)j - 1f) : ((float)j)) / 2f))) * 2f * this.m_radiusDistance;
					zero.z = (float)j * 1.866f * this.m_radiusDistance;
					gameObject.transform.position = zero;
					float d = Random.Range(1f, 1.5f);
					gameObject.transform.localScale = Vector3.one * d;
					gameObject.transform.Rotate(Random.Range(-10f, 10f), Random.Range(-180f, 180f), Random.Range(-10f, 10f));
				}
			}
		}
	}

	public GameObject m_treePrefab;

	public int m_amount;

	public GameObject m_ground;

	public float m_radiusDistance;
}
