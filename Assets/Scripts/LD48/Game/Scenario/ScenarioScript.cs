using System;
using UnityEngine;

namespace LD48.Game.Scenario {
	[Serializable]
	public class ScenarioScript {
		[SerializeField] protected bool      _hasBeenPlayed;
		[SerializeField] protected string    _text;
		[SerializeField] protected AudioClip _audioClip;
		[SerializeField] protected int       _afterCountBlock;
		[SerializeField] protected float     _displayTime;

		public bool hasBeenPlayed {
			get => _hasBeenPlayed;
			set => _hasBeenPlayed = value;
		}

		public string    text            => _text;
		public int       afterCountBlock => _afterCountBlock;
		public AudioClip audioClip       => _audioClip;
		public float     displayTime     => audioClip ? audioClip.length : _displayTime;
	}
}