using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardFlipAnimation : MonoBehaviour, IPointerClickHandler {
	Transform cardFront;
	Transform cardBack;
	float flip_duration = 0.2f;
	// Use this for initialization

	public bool isFront = false;
	public bool isOver = false;

	void Start () {
		cardFront = transform.Find ("Image_front");
		cardBack = transform.Find ("Image_back");
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		if (isOver) return;
		if (isFront) {
			StartCoroutine (FlipCardToBack());
		} else {
			StartCoroutine (FlipCardToFront());
		}
	}

	IEnumerator FlipCardToFront()
	{
		cardFront.gameObject.SetActive (false);
		cardBack.gameObject.SetActive (true);
		cardBack.rotation = Quaternion.identity;
		while (cardBack.rotation.eulerAngles.y < 90) {
			cardBack.rotation *= Quaternion.Euler (0, Time.deltaTime*90*(1f / flip_duration), 0);
			if (cardBack.rotation.eulerAngles.y > 90) {
				cardBack.rotation = Quaternion.Euler (0, 90, 0);
			}
			yield return new WaitForFixedUpdate ();
		}

		cardFront.gameObject.SetActive (true);
		cardBack.gameObject.SetActive (false);
		cardFront.rotation = Quaternion.Euler (0, 90, 0);
		while (cardFront.rotation.eulerAngles.y > 0) {
			cardFront.rotation *= Quaternion.Euler (0, -Time.deltaTime*90*(1f / flip_duration), 0);
			if (cardFront.rotation.eulerAngles.y > 90) {
				cardFront.rotation = Quaternion.Euler (0, 0, 0);
			}
			yield return new WaitForFixedUpdate ();
		}

		isFront = true;

		Camera.main.gameObject.GetComponent<GameMain> ().checkGameOver ();
	}

	IEnumerator FlipCardToBack()
	{
		cardFront.gameObject.SetActive (true);
		cardBack.gameObject.SetActive (false);
		cardFront.rotation = Quaternion.identity;
		while (cardFront.rotation.eulerAngles.y < 90) {
			cardFront.rotation *= Quaternion.Euler (0, Time.deltaTime*90*(1f / flip_duration), 0);
			if (cardFront.rotation.eulerAngles.y > 90) {
				cardFront.rotation = Quaternion.Euler (0, 90, 0);
			}
			yield return new WaitForFixedUpdate ();
		}

		cardFront.gameObject.SetActive (false);
		cardBack.gameObject.SetActive (true);
		cardBack.rotation = Quaternion.Euler (0, 90, 0);
		while (cardBack.rotation.eulerAngles.y > 0) {
			cardBack.rotation *= Quaternion.Euler (0, -Time.deltaTime*90*(1f / flip_duration), 0);
			if (cardBack.rotation.eulerAngles.y > 90) {
				cardBack.rotation = Quaternion.Euler (0, 0, 0);
			}
			yield return new WaitForFixedUpdate ();
		}

		isFront = false;
	}

	public string GetCardImageName()
	{
		return cardFront.GetComponent<Image> ().sprite.name;
	}

	public void MatchSuccess()
	{
		isOver = true;
		cardFront.gameObject.SetActive (false);
		cardBack.gameObject.SetActive (false);
	}
	public void MatchFailed(){
		StartCoroutine (FlipCardToBack ());
	}

	public void SetDefaultState()
	{
		isFront = false;
		isOver = false;
		if (cardFront != null) {
			cardFront.gameObject.SetActive (false);
			cardFront.rotation = Quaternion.identity;
		}
		if (cardBack != null) {
			cardBack.gameObject.SetActive (true);
			cardBack.rotation = Quaternion.identity;
		}
	}
}
