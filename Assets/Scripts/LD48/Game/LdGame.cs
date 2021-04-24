using System;
using LD48.Game.Data.Blocks;
using UnityEngine;
using Utils.Events;
using Utils.Extensions;

namespace LD48.Game {
	[Serializable]
	public class LdGame {
		private static LdGame instance { get; set; }

		[SerializeField] protected int _dugDepth;
		[SerializeField] protected int _gold;
		[SerializeField] protected int _blocksCleared;

		public static int blocksCleared => instance?._blocksCleared ?? 0;
		public static int gold          => instance?._gold ?? 0;

		public static IntEvent onDugDepthChanged      { get; } = new IntEvent();
		public static IntEvent onGoldChanged          { get; } = new IntEvent();
		public static IntEvent onBlocksClearedChanged { get; } = new IntEvent();

		public void Init() {
			instance = this;
			_dugDepth = 0;
			_gold = 20;
			_blocksCleared = 0;
			LdCamera.SetMinY(-_dugDepth);
			Block.onBlockHealthChanged.AddListenerOnce(HandleBlockHealthChanged);
		}

		private void HandleBlockHealthChanged(Block block) {
			if (block.health > 0) return;
			_blocksCleared++;
			onBlocksClearedChanged.Invoke(_blocksCleared);
			if (block.type.goldValue > 0) {
				_gold++;
				onGoldChanged.Invoke(_gold);
			}
			if (_dugDepth >= block.coordinates.y) return;
			_dugDepth = block.coordinates.y;
			onDugDepthChanged.Invoke(_dugDepth);
			LdCamera.SetMinY(-_dugDepth);
		}

		public static void PayGold(int amount) {
			if (amount <= 0) return;
			instance._gold--;
			onGoldChanged.Invoke(instance._gold);
		}
	}
}