using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Linq;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

	public Text[] textToShow;
	public Image _logo;

	// Use this for initialization
	void Start () {
		StartCoroutine (IntroThing ());
	}

	private IEnumerator IntroThing() {

		textToShow.ToList ().ForEach((t) => t.color = new Color (1,1,1,0));

		Tween tween = _logo.DOColor (Color.white, 4f);

		yield return tween.WaitForCompletion ();

		yield return new WaitForSeconds (2f);

		tween = _logo.DOColor (Color.clear, 1f);

		yield return tween.WaitForCompletion ();


		for (int i = 0; i < textToShow.Length; i++) {
			var text = textToShow [i];

			tween = text.DOColor (Color.black, 4f);

			yield return tween.WaitForCompletion ();

			yield return new WaitForSeconds (2f);

			tween = text.DOColor (new Color (1,1,1,0), 1f);

			yield return tween.WaitForCompletion ();

		}
			
		SceneManager.LoadScene("Main");
	}

}
