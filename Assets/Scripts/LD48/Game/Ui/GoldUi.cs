using LD48.Game;
using TMPro;
using UnityEngine;
using Utils.Extensions;

public class GoldUi : MonoBehaviour {
	[SerializeField] protected TMP_Text _goldText;

	private void Start() {
		Refresh(LdGame.gold);
		LdGame.onGoldChanged.AddListenerOnce(Refresh);
	}

	private void Refresh(int value) => _goldText.text = $"{value}";
}