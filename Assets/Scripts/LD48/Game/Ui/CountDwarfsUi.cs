using LD48.Game;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils.Extensions;

public class CountDwarfsUi : MonoBehaviour {
	[SerializeField] protected Image    _progressImage;
	[SerializeField] protected TMP_Text _countText;

	private void Start() {
		Refresh(LdGame.countDiggingDwarfs);
		_progressImage.fillAmount = 0;
		LdGame.onCountDiggingDwarfsChanged.AddListenerOnce(Refresh);
	}

	private void Refresh(int value) => _countText.text = $"{value}";

	private void Update() => _progressImage.fillAmount = LdGame.progressToSpawnNextDwarf;
}