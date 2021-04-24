using System;
using LD48.Game.Data.Blocks;
using UnityEngine;
using Utils.Events;
using Utils.Extensions;

namespace LD48.Game {
	[Serializable]
	public class LdGame {
		[SerializeField] protected int _dugDepth;
		[SerializeField] protected int _gold;

		public static IntEvent onDugDepthChanged { get; } = new IntEvent();
		public static IntEvent onGoldChanged     { get; } = new IntEvent();

		public void Init() {
			_dugDepth = 0;
			LdCamera.SetMinY(-_dugDepth);
			Block.onBlockHealthChanged.AddListenerOnce(HandleBlockHealthChanged);
		}

		private void HandleBlockHealthChanged(Block block) {
			if (block.health > 0) return;
			if (_dugDepth >= block.coordinates.y) return;
			_dugDepth = block.coordinates.y;
			onDugDepthChanged.Invoke(_dugDepth);
			LdCamera.SetMinY(-_dugDepth);
		}
	}
}