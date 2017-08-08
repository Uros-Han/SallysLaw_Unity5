using UnityEngine;
using System.Collections;

public class DoorMiniBtnPositionAdjuster : MonoBehaviour {

	Transform m_TargetTransform;

	void OnEnable()
	{
		StartCoroutine(PositionAdjustByDoorSize ());
	}

	IEnumerator PositionAdjustByDoorSize()
	{
		while (m_TargetTransform == null) {
			m_TargetTransform = GetComponent<UIFollowTarget> ().target;
			yield return null;
		}

		if (gameObject.name == "SwitchBtn(Blue)(Clone)") {
			transform.GetChild (0).localPosition = new Vector2 (35, m_TargetTransform.localScale.y * -30f);
			transform.GetChild (1).localPosition = new Vector2 (-35, m_TargetTransform.localScale.y * -30f);
		} else {
			transform.GetChild (0).localPosition = new Vector2 (35, m_TargetTransform.GetChild(0).localScale.y * -30f);
			transform.GetChild (1).localPosition = new Vector2 (-35, m_TargetTransform.GetChild(0).localScale.y * -30f);
		}

		GetComponent<UISprite> ().color = Color.white;
	}
}
