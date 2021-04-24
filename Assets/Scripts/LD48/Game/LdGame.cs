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
			SetDugDepth(0);
			SetGold(20);
			SetBlocksCleared(0);
			Block.onBlockHealthChanged.AddListenerOnce(HandleBlockHealthChanged);
		}

		private void HandleBlockHealthChanged(Block block) {
			if (block.health > 0) return;
			SetBlocksCleared(blocksCleared + 1);
			if (block.type.goldValue > 0) SetGold(_gold + block.type.goldValue);
			if (block.coordinates.y > _dugDepth) SetDugDepth(block.coordinates.y);
		}

		private void SetDugDepth(int depth) {
			if (_dugDepth == depth) return;
			_dugDepth = depth;
			onDugDepthChanged.Invoke(_dugDepth);
			LdCamera.SetMinY(-_dugDepth);
		}

		public static void PayGold(int amount) {
			if (amount <= 0) return;
			instance.SetGold(instance._gold - amount);
		}

		private void SetGold(int amount) {
			if (_gold == amount) return;
			_gold = amount;
			onGoldChanged.Invoke(amount);
		}

		private void SetBlocksCleared(int amount) {
			if (_blocksCleared == amount) return;
			_blocksCleared = amount;
			onBlocksClearedChanged.Invoke(amount);
		}
	}
}