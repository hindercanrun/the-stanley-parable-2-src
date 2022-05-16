using System;
using UnityEngine;

public class KeyframeRope : HammerEntity
{
	private void Awake()
	{
		base.transform.rotation = Quaternion.identity;
		this.InitNodes();
		this.InitMesh();
	}

	private void Start()
	{
		this.UpdateRope();
	}

	private void InitNodes()
	{
		if (this.nextKeyObj == null)
		{
			return;
		}
		int num = 10 + (int)Mathf.Pow((float)this.subdivisions, 2f);
		this.nodes = new KeyframeRope.node[num];
		this.nodes[0].position = base.transform.position;
		this.nodes[this.nodes.Length - 1].position = this.nextKeyObj.transform.position;
		for (int i = 1; i < num - 1; i++)
		{
			this.nodes[i].position = Vector3.Lerp(this.nodes[0].position, this.nodes[this.nodes.Length - 1].position, (float)i / (float)(num - 1));
		}
		this.nodeColors = new Color[num];
		for (int j = 0; j < num; j++)
		{
			this.nodeColors[j] = Color.Lerp(Color.green, Color.red, (float)j / (float)(num - 1));
		}
		this.length = Vector3.Distance(this.nodes[0].position, this.nodes[this.nodes.Length - 1].position);
		this.slackLength = this.length * Mathf.Pow(1f + this.slack, this.slackPower);
	}

	private void InitMesh()
	{
		if (this.nextKeyObj == null)
		{
			return;
		}
		this.render = base.GetComponent<MeshRenderer>();
		if (this.render == null)
		{
			this.render = base.gameObject.AddComponent<MeshRenderer>();
		}
		this.meshFilter = base.GetComponent<MeshFilter>();
		if (this.meshFilter == null)
		{
			this.meshFilter = base.gameObject.AddComponent<MeshFilter>();
		}
		this.mesh = this.meshFilter.mesh;
		this.verts = new Vector3[this.nodes.Length * this.segments];
		this.tris = new int[this.nodes.Length * this.segments * 6];
		for (int i = 0; i < this.nodes.Length; i++)
		{
			for (int j = 0; j < this.segments; j++)
			{
				if (i < this.nodes.Length - 1)
				{
					int num = 0;
					if (j == this.segments - 1)
					{
						num = -this.segments;
					}
					this.tris[j * this.segments + j + i * this.segments * 6] = i * this.segments + j;
					this.tris[1 + j * this.segments + j + i * this.segments * 6] = i * this.segments + j + 1 + num;
					this.tris[2 + j * this.segments + j + i * this.segments * 6] = i * this.segments + j + this.segments;
					this.tris[3 + j * this.segments + j + i * this.segments * 6] = i * this.segments + j + 1 + num;
					this.tris[4 + j * this.segments + j + i * this.segments * 6] = i * this.segments + j + 1 + num + this.segments;
					this.tris[5 + j * this.segments + j + i * this.segments * 6] = i * this.segments + j + this.segments;
				}
			}
		}
		this.mesh.vertices = this.verts;
		this.mesh.triangles = this.tris;
	}

	private void OnBecameVisible()
	{
		this.visible = true;
	}

	private void OnBecameInvisible()
	{
		this.visible = false;
	}

	private void Update()
	{
		if (!this.visible)
		{
			return;
		}
		this.UpdateRope();
	}

	private void UpdateRope()
	{
		if (this.nextKeyObj == null)
		{
			return;
		}
		this.slackLength = this.length * Mathf.Pow(1f + this.slack, this.slackPower);
		if (!this.precalculated)
		{
			this.precalculated = true;
			for (int i = 0; i < 10000; i++)
			{
				this.UpdateNodes();
			}
		}
		this.UpdateNodes();
		this.UpdateMesh();
	}

	private void UpdateNodes()
	{
		this.nodes[0].position = base.transform.position;
		this.nodes[this.nodes.Length - 1].position = this.nextKeyObj.transform.position;
		for (int i = 1; i < this.nodes.Length - 1; i++)
		{
			this.nodes[i].cachedPosition = this.nodes[i].position;
			this.nodes[i].velocity = Vector3.ClampMagnitude(this.nodes[i].velocity, this.velocityLimit);
			KeyframeRope.node[] array = this.nodes;
			int num = i;
			array[num].velocity = array[num].velocity * 0.9f;
		}
		float gameDeltaTime = Singleton<GameMaster>.Instance.GameDeltaTime;
		this.steps = Mathf.CeilToInt(Mathf.Log(80f * gameDeltaTime, 2f));
		this.steps = Mathf.Clamp(this.steps, 1, 3);
		for (int j = 0; j < this.steps; j++)
		{
			this.TimeStep(gameDeltaTime * (1f / (float)this.steps));
		}
	}

	private void TimeStep(float delta)
	{
		if (delta > 0.1f)
		{
			delta = 0.1f;
		}
		this.lengthPerNode = this.slackLength / ((float)this.nodes.Length * 4f);
		for (int i = 1; i < this.nodes.Length - 1; i++)
		{
			Vector3 a = Vector3.zero;
			int num = 1;
			for (int j = -num; j <= num; j++)
			{
				if (i + j >= 0 && i + j < this.nodes.Length && j != 0)
				{
					Vector3 vector = this.nodes[i + j].position - this.nodes[i].position;
					Vector3 vector2 = -(this.lengthPerNode * (float)Mathf.Abs(j) - vector.magnitude) * vector.normalized * 1500f * this.springForceMultiplier / (float)Mathf.Abs(j);
					if (this.useSpringForceLimit)
					{
						vector2 = Vector3.ClampMagnitude(vector2, this.springForceLimit);
					}
					a += vector2;
					if (i + j == 0 || i + j == this.nodes.Length - 1)
					{
						a += vector2;
					}
				}
			}
			if (this.gravityToggle)
			{
				a += Vector3.down * 9.8f * this.nodeWeight;
			}
			KeyframeRope.node[] array = this.nodes;
			int num2 = i;
			array[num2].velocity = array[num2].velocity + a * delta * (this.useOldCalcs ? 1f : delta);
			if (this.showDebug)
			{
				int num3 = this.nodes.Length / 2;
			}
		}
		for (int k = 1; k < this.nodes.Length - 1; k++)
		{
			if (this.nodes[k].velocity.sqrMagnitude < 1E-07f)
			{
				this.nodes[k].velocity = Vector3.zero;
			}
			else
			{
				this.nodes[k].velocity = Vector3.ClampMagnitude(this.nodes[k].velocity, this.velocityLimit);
				KeyframeRope.node[] array2 = this.nodes;
				int num4 = k;
				array2[num4].position = array2[num4].position + this.nodes[k].velocity * delta;
			}
		}
	}

	private void UpdateMesh()
	{
		Vector3 position = base.transform.position;
		for (int i = 0; i < this.nodes.Length; i++)
		{
			bool flag = false;
			Vector3 vector;
			if (i == this.nodes.Length - 1)
			{
				vector = this.nodes[i].position - this.nodes[i - 1].position;
				if (this.nodes[i].position == this.nodes[i].cachedPosition && this.nodes[i - 1].position == this.nodes[i - 1].cachedPosition)
				{
					flag = true;
				}
			}
			else if (i == 0)
			{
				vector = this.nodes[i + 1].position - this.nodes[i].position;
				if (this.nodes[i].position == this.nodes[i].cachedPosition && this.nodes[i + 1].position == this.nodes[i + 1].cachedPosition)
				{
					flag = true;
				}
			}
			else
			{
				vector = this.nodes[i + 1].position - this.nodes[i].position + (this.nodes[i].position - this.nodes[i - 1].position);
				vector *= 0.5f;
				if (this.nodes[i].position == this.nodes[i].cachedPosition && this.nodes[i - 1].position == this.nodes[i - 1].cachedPosition && this.nodes[i + 1].position == this.nodes[i + 1].cachedPosition)
				{
					flag = true;
				}
			}
			if (!flag || this.firstMeshPass)
			{
				Vector3 normalized = Vector3.Cross(vector, Vector3.up).normalized;
				for (int j = 0; j < this.segments; j++)
				{
					Vector3 a = Quaternion.AngleAxis((float)(j * (360 / this.segments)), vector) * normalized;
					this.verts[i * this.segments + j] = this.nodes[i].position - position + a * this.width * 0.5f;
				}
			}
		}
		this.mesh.vertices = this.verts;
		this.mesh.RecalculateBounds();
		this.mesh.RecalculateNormals();
		this.firstMeshPass = false;
	}

	public void OnDrawGizmos()
	{
		if (this.nodes != null)
		{
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				Gizmos.color = this.nodeColors[i];
				Gizmos.DrawLine(this.nodes[i].position, this.nodes[i + 1].position);
			}
		}
	}

	public void DrawDebugRope()
	{
		if (this.nodes != null)
		{
			for (int i = 0; i < this.nodes.Length - 1; i++)
			{
				Debug.DrawLine(this.nodes[i].position, this.nodes[i + 1].position, this.nodeColors[i]);
				Debug.DrawLine(this.nodes[i].position, this.nodes[i].position + Vector3.up * 0.2f, this.nodeColors[i]);
			}
		}
	}

	public bool start;

	public string nextKeyName = "";

	public GameObject nextKeyObj;

	public float slack = 25f;

	public float springForceMultiplier = 10f;

	public int subdivisions = 2;

	public float width = 0.02f;

	public float nodeWeight = 5f;

	private int steps = 1;

	[SerializeField]
	private KeyframeRope.node[] nodes;

	[SerializeField]
	[HideInInspector]
	private Color[] nodeColors;

	private float length;

	private float actualLength;

	private float slackLength;

	private float slackPower = 1.75f;

	private float lengthPerNode;

	public bool gravityToggle = true;

	private float spring = 20f;

	private bool precalculated;

	[SerializeField]
	[HideInInspector]
	private MeshRenderer render;

	[SerializeField]
	[HideInInspector]
	private MeshFilter meshFilter;

	[SerializeField]
	[HideInInspector]
	private Mesh mesh;

	[SerializeField]
	[HideInInspector]
	private Vector3[] verts;

	[SerializeField]
	[HideInInspector]
	private int[] tris;

	private int segments = 5;

	private float velocityLimit = 5f;

	public bool useOldCalcs;

	public bool useSpringForceLimit;

	public float springForceLimit = 10000f;

	public bool showDebug;

	private bool firstMeshPass = true;

	private bool visible;

	[Serializable]
	private struct node
	{
		public Vector3 position;

		public Vector3 velocity;

		public Vector3 cachedPosition;
	}
}
