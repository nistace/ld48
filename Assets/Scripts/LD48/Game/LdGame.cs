using System;
using System.Collections;
using System.Collections.Generic;
using LD48.Constants;
using LD48.Game.Data.Blocks;
using LD48.Game.Data.Dwarfs;
using UnityEngine;
using Utils.Events;
using Utils.Extensions;
using Utils.Types;

namespace LD48.Game {
	[Serializable]
	public class LdGame {
		private static LdGame instance { get; set; }

		[SerializeField] protected int         _dugDepth;
		[SerializeField] protected int         _gold;
		[SerializeField] protected int         _blocksCleared;
		[SerializeField] protected List<Dwarf> _diggingDwarfs = new List<Dwarf>();
		[SerializeField] protected int         _countDeadDwarfs;
		[SerializeField] protected Ratio       _progressToSpawnNextDwarf;
		[SerializeField] protected float       _spawnDwarfsSpeed = .1f;
		[SerializeField] protected Dwarf       _dwarfPrefab;

		public static int blocksCleared => instance?._blocksCleared ?? 0;
		public static int gold          => instance?._gold ?? 0;

		public static IntEvent onDugDepthChanged           { get; } = new IntEvent();
		public static IntEvent onGoldChanged               { get; } = new IntEvent();
		public static IntEvent onBlocksClearedChanged      { get; } = new IntEvent();
		public static IntEvent onCountDeadDwarfsChanged    { get; } = new IntEvent();
		public static IntEvent onCountDiggingDwarfsChanged { get; } = new IntEvent();

		public void Init(MonoBehaviour routineRunner) {
			instance = this;
			SetDugDepth(0);
			SetGold(20);
			SetBlocksCleared(0);
			SetCountDeadDwarfs(0);
			Block.onBlockHealthChanged.AddListenerOnce(HandleBlockHealthChanged);
			Dwarf.onDamaged.AddListenerOnce(HandleDwarfDamaged);
			Dwarf.onStartDigging.AddListener(HandleDwarfStartedDigging);
			routineRunner.StartCoroutine(ManageDwarfs());
		}

		private void HandleDwarfStartedDigging(Dwarf dwarf) {
			if (_diggingDwarfs.Contains(dwarf)) return;
			_diggingDwarfs.Add(dwarf);
			onCountDiggingDwarfsChanged.Invoke(_diggingDwarfs.Count);
		}

		private void HandleDwarfDamaged(Dwarf dwarf) {
			if (dwarf.health > 0) return;
			if (!_diggingDwarfs.Contains(dwarf)) return;
			SetCountDeadDwarfs(_countDeadDwarfs + 1);
			_diggingDwarfs.Remove(dwarf);
			onCountDiggingDwarfsChanged.Invoke(_diggingDwarfs.Count);
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

		private void SetCountDeadDwarfs(int amount) {
			if (_countDeadDwarfs == amount) return;
			_countDeadDwarfs = amount;
			onCountDeadDwarfsChanged.Invoke(amount);
		}

		private void SetBlocksCleared(int amount) {
			if (_blocksCleared == amount) return;
			_blocksCleared = amount;
			onBlocksClearedChanged.Invoke(amount);
		}

		private static void SpawnDwarf() {
			var spawn = World.RandomSpawn().position;
			var destination = World.GetClosestPositionToSignFromSpawn(spawn);
			var dwarf = UnityEngine.Object.Instantiate(instance._dwarfPrefab, spawn, Quaternion.identity, null);
			dwarf.Init(LdMemory.dwarfTypes.Values.Random());
			dwarf.WalkUpToTheSign(spawn, destination);
		}

		private IEnumerator ManageDwarfs() {
			do {
				SpawnDwarf();
				_progressToSpawnNextDwarf = 0;
				while (_progressToSpawnNextDwarf < 1) {
					_progressToSpawnNextDwarf += _spawnDwarfsSpeed * Time.deltaTime;
					yield return null;
				}
			} while (_dugDepth < 9999);
		}
	}
}