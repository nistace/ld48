using System;
using LD48.Game;
using LD48.Input;
using TDDG.Scenes.Game.States;
using UnityEngine;
using Utils.Extensions;

public class GameController : MonoBehaviour {
	[SerializeField] protected LdGame _game;

	private void Start() {
		_game.Init();
		AbstractState.Init(this, DefaultState.instance);
	}

	private void OnEnable() {
		Inputs.controls.Game.MousePosition.SetEnabled(enabled);
	}
}