using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(SkeletonRenderer))]
public class SkeletonGhost : MonoBehaviour {

	public float spawnRate = 0.05f;
	public Color32 color = new Color32(0xFF, 0xFF, 0xFF, 0xFF);
	public int maximumGhosts = 10;
	public float fadeSpeed = 10;

	float nextSpawnTime;
	SkeletonGhostRenderer[] pool;
	int poolIndex = 0;
	SkeletonRenderer skeletonRenderer;

	void Start()
	{
		skeletonRenderer = GetComponent<SkeletonRenderer>();
		nextSpawnTime = Time.time + spawnRate;
		pool = new SkeletonGhostRenderer[maximumGhosts];
		for (int i = 0; i < maximumGhosts; i++)
		{
			GameObject go = new GameObject(gameObject.name + " Ghost", typeof(SkeletonGhostRenderer));
			pool[i] = go.GetComponent<SkeletonGhostRenderer>();
			go.SetActive(false);
			go.hideFlags = HideFlags.HideInHierarchy;
		}
	}

	void Update()
	{
		if (Time.time >= nextSpawnTime)
		{
			GameObject go = pool[poolIndex].gameObject;
			pool[poolIndex].Initialize(skeletonRenderer, color, fadeSpeed);
			go.transform.parent = transform;

			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.transform.localScale = Vector3.one;

			go.transform.parent = null;

			poolIndex++;

			if (poolIndex == pool.Length)
				poolIndex = 0;

			nextSpawnTime = Time.time + spawnRate;
		}
	}

	void OnDestroy()
	{
		for (int i = 0; i < maximumGhosts; i++)
		{
			if(pool[i] != null)
				pool[i].Cleanup();
		}
		
	}

	
}
