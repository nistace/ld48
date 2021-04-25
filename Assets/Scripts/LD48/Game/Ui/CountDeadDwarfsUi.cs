using LD48.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

public class CountDeadDwarfsUi : MonoBehaviour {
	[SerializeField] protected Image    _progressImage;
	[SerializeField] protected TMP_Text _countText;

	private void Start() {
		Refresh(LdGame.countDeadDwarfs);
		LdGame.onCountDeadDwarfsChanged.AddListenerOnce(Refresh);
	}

	private void Refresh(int value) {
		_countText.text = $"{value}";
		_progressImage.fillAmount = (float) value / LdGame.maxDeadDwarfs;
	}
}