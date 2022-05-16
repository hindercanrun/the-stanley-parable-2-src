using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TSP3BackgroundElementSet", menuName = "TSP3 Background ElementSet")]
public class TSP3BackgroundRandomSet : ScriptableObject
{
	public T RandomItem<T>(List<T> list, int seed = 14574572)
	{
		if (list == null || list.Count == 0)
		{
			return default(!!0);
		}
		Random.State state = Random.state;
		Random.InitState(seed);
		T result = list[Random.Range(0, list.Count)];
		Random.state = state;
		return result;
	}

	public Texture2D GetLUTImage(int seed)
	{
		return this.RandomItem<Texture2D>(this.LUT, seed);
	}

	public Texture2D GetBackgroundImage(int seed)
	{
		return this.RandomItem<Texture2D>(this.backgroundImage, seed);
	}

	public IEnumerable<GameObject> GetAllToInstantiate(int seed)
	{
		yield return this.RandomItem<GameObject>(this.toInstantiate1, seed);
		yield return this.RandomItem<GameObject>(this.toInstantiate2, seed);
		yield return this.RandomItem<GameObject>(this.toInstantiate3, seed);
		yield break;
	}

	[Header("Will pick **ONE** randomly from each list")]
	[SerializeField]
	private List<Texture2D> LUT;

	[SerializeField]
	private List<Texture2D> backgroundImage;

	[SerializeField]
	private List<GameObject> toInstantiate1;

	[SerializeField]
	private List<GameObject> toInstantiate2;

	[SerializeField]
	private List<GameObject> toInstantiate3;
}
