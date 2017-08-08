using UnityEngine;
using System.Collections;

public class SkeletonGhostRenderer : MonoBehaviour {

	public float fadeSpeed = 10;

	Color32[] colors;
	Color32 black = new Color32(0, 0, 0, 0);
	MeshFilter meshFilter;
	
	public void Initialize(SkeletonRenderer skeletonRenderer, Color32 color, float speed)
	{
		StopAllCoroutines();

		gameObject.SetActive(true);

		if (gameObject.GetComponent<Renderer>() == null)
		{
			gameObject.AddComponent<MeshRenderer>();
			meshFilter = gameObject.AddComponent<MeshFilter>();
		}
		

		GetComponent<Renderer>().sharedMaterials = skeletonRenderer.GetComponent<Renderer>().sharedMaterials;
		GetComponent<Renderer>().sortingOrder = skeletonRenderer.GetComponent<Renderer>().sortingOrder - 1;

		meshFilter.sharedMesh = (Mesh)Instantiate(skeletonRenderer.GetComponent<MeshFilter>().sharedMesh);

		colors = meshFilter.sharedMesh.colors32;


		if ((color.a + color.r + color.g + color.b) > 0)
		{
			for (int i = 0; i < colors.Length; i++)
			{
				colors[i] = color;
			}
		}

		fadeSpeed = speed;

		StartCoroutine(Fade());
	}

	IEnumerator Fade()
	{
		Color32 c;

		for (int t = 0; t < 500; t++)
		{
			bool breakout = true;
			for (int i = 0; i < colors.Length; i++)
			{
				c = colors[i];
				if (c.a > 0)
					breakout = false;
				colors[i] = Color32.Lerp(c, black, Time.deltaTime * fadeSpeed);
			}

			meshFilter.sharedMesh.colors32 = colors;

			if (breakout)
				break;
			yield return null;
		}

		Destroy(meshFilter.sharedMesh);

		gameObject.SetActive(false);
	}

	public void Cleanup()
	{
		if(meshFilter != null && meshFilter.sharedMesh != null)
			Destroy(meshFilter.sharedMesh);

		Destroy(gameObject);
	}
}
