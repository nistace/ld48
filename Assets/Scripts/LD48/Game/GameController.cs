using LD48.Game;
using LD48.Game.Ui;
using LD48.Input;
using TDDG.Scenes.Game.States;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils.Audio;
using Utils.Extensions;

public class GameController : MonoBehaviour {
	[SerializeField] protected LdGame     _game;
	[SerializeField] protected GameOverUi _gameOverUi;

	private void Start() {
		_game.Init(this);
		AbstractState.Init(this, DefaultState.instance);
	}

	private void OnEnable() => SetListenersEnabled(true);
	private void OnDisable() => SetListenersEnabled(false);

	private void SetListenersEnabled(bool enabled) {
		Inputs.controls.Game.MousePosition.SetEnabled(enabled);
		LdGame.onGameOver.SetListenerActive(HandleGameOver, enabled);
		_gameOverUi.onEndButtonClicked.SetListenerActive(GoToMenu, enabled);
	}

	private static void GoToMenu() => SceneManager.LoadScene("Menu");

	private void HandleGameOver() {
		AudioManager.Sfx.Play("gameOver");
		_gameOverUi.Show(LdGame.dugDepth, LdGame.blocksCleared);
	}
}