using UnityEngine;
using System.Collections;

public class CountDown : MonoBehaviour {

	void OnEnable()
	{
//		transform.GetChild (2).GetComponent<TweenScale> ().ResetToBeginning ();
//		transform.GetChild (2).GetComponent<TweenScale> ().Play (true);
//		transform.GetChild (2).GetComponent<TweenScale> ().RemoveOnFinished (new EventDelegate (DeactiveThis));
		StartCoroutine (Counter ());
	}

	IEnumerator Counter()
	{
//		yield return new WaitForSeconds (0.75f);
//		transform.GetChild (2).GetComponent<TweenScale> ().ResetToBeginning ();
//		transform.GetChild (2).GetComponent<UILabel> ().text = "2";
//		transform.GetChild (2).GetComponent<TweenScale> ().Play (true);
//		yield return new WaitForSeconds (0.75f);
//		transform.GetChild (2).GetComponent<TweenScale> ().ResetToBeginning ();
//		transform.GetChild (2).GetComponent<UILabel> ().text = "1";
//		transform.GetChild (2).GetComponent<TweenScale> ().Play (true);
//		yield return new WaitForSeconds (0.75f);
//		transform.GetChild (2).GetComponent<TweenScale> ().ResetToBeginning ();
//		transform.GetChild (2).GetComponent<UILabel> ().text = "GO";
//		transform.GetChild (2).GetComponent<TweenScale> ().AddOnFinished (new EventDelegate (DeactiveThis));
//		transform.GetChild (2).GetComponent<TweenScale> ().Play (true);


		bool bExit = false;

		do {
			if((Input.GetMouseButtonUp(0)&& UICamera.selectedObject != null && UICamera.selectedObject.name.Contains("UI Root")) || Input.GetKeyUp(KeyCode.Space))
			{
				bExit = true;
			}

			yield return null;
		} while(!bExit);

		DeactiveThis ();
	}

	void DeactiveThis()
	{
		gameObject.SetActive (false);
		TimeMgr.Play ();
	}
}
