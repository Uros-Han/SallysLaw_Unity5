using UnityEngine;
using System.Collections;

public class BoxDeco : MonoBehaviour {

	public int m_iTopDecoIdx;
	public int m_iMidDecoIdx;

	public int m_iTopDecoCreateRatio;

	// Use this for initialization
	void Start () {

		if (StageLoader.getInstance.m_bMaptool) {
			GameObject decoNum = Instantiate(Resources.Load("Prefabs/Objects/Boxes/DecoNum") as GameObject) as GameObject;
			decoNum.transform.parent = transform;
			decoNum.transform.localPosition = Vector2.zero;
			decoNum.GetComponent<TextMesh>().text = m_iTopDecoIdx.ToString();

			return;
		}

		if (SceneStatus.getInstance.m_bMemoryStage)
			GetComponent<Background> ().enabled = false;

//		m_iTopDecoIdx = Random.Range (0, SceneObjectPool.getInstance.m_sally_listTopDeco.Count);
		m_iMidDecoIdx = Random.Range (0, SceneObjectPool.getInstance.m_sally_listMidDeco.Count);

		m_iTopDecoCreateRatio = 20;

//		StartCoroutine(waitOneFrame());

		if (!StageLoader.getInstance.m_bMaptool) {
			if (gameObject.name.Contains("TopDeco"))
			{
//				if(!TextInRange())
//					RandTopDeco ();
//				else
//					gameObject.SetActive(false);

				SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();
				Sprite[] TopDeco = SceneObjectPool.getInstance.m_sally_listTopDeco.ToArray();


				if(m_iTopDecoIdx < TopDeco.Length)
					spriteRenderer.sprite = TopDeco [m_iTopDecoIdx];

				if(spriteRenderer.sprite == null)
					return;
				
				transform.position = new Vector3(transform.position.x, transform.position.y + (spriteRenderer.sprite.bounds.size.y/2f) - 0.25f);
			}else if (gameObject.name.Equals("MidDeco"))
				RandMidDeco ();
		}

	}

	IEnumerator waitOneFrame()
	{
		yield return new WaitForEndOfFrame ();


	}

	bool TextInRange()
	{
		Transform textPosTrans = GameObject.Find ("TextFloat_Pos").transform;

		for(int i = 0 ; i < textPosTrans.childCount; ++i)
		{
			if(Vector3.Distance(textPosTrans.GetChild(i).position, transform.position) < 2.5f)
				return true;
		}

		return false;
	}


	void RandTopDeco()
	{
		Sprite[] TopDeco = SceneObjectPool.getInstance.m_sally_listTopDeco.ToArray();
		SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer> ();

		string boxSpriteName = transform.parent.GetComponent<SpriteRenderer> ().sprite.name;

		//최상단 박스 아닌애들은 생성안하도록
		if (boxSpriteName.Contains ("_4") ||
		    boxSpriteName.Contains ("_5") ||
		    boxSpriteName.Contains ("_6") ||
		    boxSpriteName.Contains ("_7")) {
			gameObject.SetActive(false);
			return;
		}
		
		if (Random.Range (0, m_iTopDecoCreateRatio).Equals(0)) {
			spriteRenderer.sprite = TopDeco [m_iTopDecoIdx];

			transform.localPosition = new Vector3(Random.Range(-0.15f, 0.15f), spriteRenderer.sprite.bounds.size.y/2f + 0.25f);


		} else {
			gameObject.SetActive(false);
		}
	}

	void RandMidDeco()
	{
		Sprite[] MidDeco = SceneObjectPool.getInstance.m_sally_listMidDeco.ToArray();

		if (Random.Range (0, 3).Equals(0)) {
			GetComponent<SpriteRenderer>().sprite = MidDeco[m_iMidDecoIdx];


		} else {
			gameObject.SetActive(false);
		}
	}
}
