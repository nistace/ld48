using LD48.Game;
using UnityEngine;

public class GameController : MonoBehaviour {
	[SerializeField] protected LdGame _game;

	private void Start() {
		_game.Init();
	}
}