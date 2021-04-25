using System.Collections;
using LD48.Game.Scenario;
using TMPro;
using UnityEngine;
using Utils.Coroutines;
using Utils.Ui;

namespace LD48.Game.Ui {
	public class ScenarioTextUi : MonoBehaviourUi {
		[SerializeField] protected TMP_Text _text;

		private SingleCoroutine singleCoroutine { get; set; }

		public void Show(ScenarioScript script) {
			if (singleCoroutine == null) singleCoroutine = new SingleCoroutine(this);
			_text.text = script.text;
			gameObject.SetActive(true);
			singleCoroutine.Start(HideAfter(Time.time + script.displayTime));
		}

		private IEnumerator HideAfter(float atTime) {
			while (Time.time < atTime) yield return null;
			gameObject.SetActive(false);
		}
	}
}