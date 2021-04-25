﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LD48.Constants;
using LD48.Game.Data.Blocks;
using LD48.Game.Data.Constructions;
using LD48.Game.Data.Dwarfs;
using UnityEngine;
using UnityEngine.Events;
using Utils.Audio;
using Utils.Events;
using Utils.Extensions;
using Utils.Types;
using Object = UnityEngine.Object;

namespace LD48.Game {
	[Serializable]
	public class LdGame {
		private static LdGame instance { get; set; }

		[SerializeField] protected int                   _dugDepth;
		[SerializeField] protected int                   _gold;
		[SerializeField] protected int                   _blocksCleared;
		[SerializeField] protected List<Dwarf>           _diggingDwarfs = new List<Dwarf>();
		[SerializeField] protected int                   _countDeadDwarfs;
		[SerializeField] protected int                   _maxDeadDwarfs = 10;
		[SerializeField] protected Ratio                 _progressToSpawnNextDwarf;
		[SerializeField] protected float                 _spawnDwarfsSpeed = .1f;
		[SerializeField] protected Dwarf                 _dwarfPrefab;
		[SerializeField] protected RestorationOnGround[] _restorationPlacesOnGround;

		public static int   blocksCleared            => instance?._blocksCleared ?? 0;
		public static int   dugDepth                 => instance?._dugDepth ?? 0;
		public static int   gold                     => instance?._gold ?? 0;
		public static int   countDiggingDwarfs       => instance?._diggingDwarfs.Count ?? 0;
		public static int   countDeadDwarfs          => instance?._countDeadDwarfs ?? 0;
		public static int   maxDeadDwarfs            => instance?._maxDeadDwarfs ?? 0;
		public static Ratio progressToSpawnNextDwarf => instance?._progressToSpawnNextDwarf ?? 0;

		public static IntEvent   onDugDepthChanged           { get; } = new IntEvent();
		public static IntEvent   onGoldChanged               { get; } = new IntEvent();
		public static IntEvent   onBlocksClearedChanged      { get; } = new IntEvent();
		public static IntEvent   onCountDeadDwarfsChanged    { get; } = new IntEvent();
		public static IntEvent   onCountDiggingDwarfsChanged { get; } = new IntEvent();
		public static UnityEvent onGameOver                  { get; } = new UnityEvent();

		public void Init(MonoBehaviour routineRunner) {
			instance = this;
			SetDugDepth(0);
			SetGold(20);
			SetBlocksCleared(0);
			SetCountDeadDwarfs(0);
			_restorationPlacesOnGround.ForEach(t => t.Hide());
			EnumUtils.Values<DwarfNeed>().ForEach(t => DwarfNeeds.SetNeedActive(t, false));
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
			_restorationPlacesOnGround.Where(t => !t.appeared && t.appearsAfterBlocks <= blocksCleared).ForEach(t => t.Appear());
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
			if (_gold < amount) AudioManager.Sfx.PlayRandom("gold");
			_gold = amount;
			onGoldChanged.Invoke(amount);
		}

		private void SetCountDeadDwarfs(int amount) {
			if (_countDeadDwarfs == amount) return;
			_countDeadDwarfs = amount;
			onCountDeadDwarfsChanged.Invoke(amount);
			if (_countDeadDwarfs >= _maxDeadDwarfs) EndGame();
		}

		private static void EndGame() {
			instance._spawnDwarfsSpeed = 0;
			onGameOver.Invoke();
			instance._diggingDwarfs.ForEach(t => Object.Destroy(t.gameObject));
			instance._diggingDwarfs.Clear();
		}

		private void SetBlocksCleared(int amount) {
			if (_blocksCleared == amount) return;
			_blocksCleared = amount;
			onBlocksClearedChanged.Invoke(amount);
		}

		private static void SpawnDwarf() {
			var spawn = World.RandomSpawn().position;
			var destination = World.GetClosestPositionToSignFromSpawn(spawn);
			var dwarf = Object.Instantiate(instance._dwarfPrefab, spawn, Quaternion.identity, null);
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