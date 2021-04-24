﻿using System.Collections.Generic;
using System.Linq;
using LD48.Constants;
using LD48.Game.Data.Blocks;
using LD48.Game.Data.Constructions;
using UnityEngine;
using Utils.Extensions;

public class World : MonoBehaviour {
	private static World instance { get; set; }

	[SerializeField] protected Transform _blockContainer;
	[SerializeField] protected Transform _constructionsContainer;
	[SerializeField] protected int       _maxY;
	[SerializeField] protected int       _width       = 12;
	[SerializeField] protected int       _secureDepth = 10;
	[SerializeField] protected Block     _block;

	private List<Block[]>                        blocksPerDepth { get; } = new List<Block[]>();
	private Dictionary<Vector2Int, Construction> constructions  { get; } = new Dictionary<Vector2Int, Construction>();
	private Queue<Block>                         pooledBlocks   { get; } = new Queue<Block>();

	private void Awake() => instance = this;

	private void Start() => Build();
	private void OnEnable() => SetListenersEnabled(true);
	private void OnDisable() => SetListenersEnabled(false);

	private void SetListenersEnabled(bool enabled) {
		Block.onBlockHealthChanged.SetListenerActive(HandleBlockHealthChanged, enabled);
	}

	private void HandleBlockHealthChanged(Block block) {
		if (block.health > 0) return;
		for (var i = blocksPerDepth.Count; i < block.coordinates.y + _secureDepth; ++i) CreateLine(i);
		block.gameObject.SetActive(false);
		blocksPerDepth[block.coordinates.y][block.coordinates.x] = null;
		pooledBlocks.Enqueue(block);
	}

	private void Build() {
		_blockContainer.ClearChildren();
		for (var depth = 0; depth < _secureDepth; ++depth) CreateLine(depth);
	}

	private void CreateLine(int depth) {
		var possibleTypes = LdMemory.blockTypes.Values.Where(t => depth >= t.minDepth && t.frequency > 0).ToArray();
		while (blocksPerDepth.Count <= depth) blocksPerDepth.Add(new Block[_width]);
		for (var position = 0; position < _width; ++position) {
			var block = GetNewBlock();
			var coordinates = new Vector2Int(position, depth);
			block.transform.position = CoordinatesToWorldPoint(coordinates);
			var blockType = possibleTypes.Random(t => t.frequency);
			blocksPerDepth[depth][position] = block;
			blocksPerDepth[depth][position].Init(blockType, coordinates);
		}
	}

	private Block GetNewBlock() {
		if (pooledBlocks.Count == 0) return Instantiate(_block, _blockContainer);
		var block = pooledBlocks.Dequeue();
		block.gameObject.SetActive(true);
		return block;
	}

	public static void CreateConstruction(ConstructionType type, Vector2Int position) {
		if (instance.constructions.ContainsKey(position)) Destroy(instance.constructions[position]);
		var newConstruction = Instantiate(type.construction, CoordinatesToWorldPoint(position), Quaternion.identity, instance._constructionsContainer);
		newConstruction.Init(type);
		instance.constructions.Set(position, newConstruction);
	}

	public static Vector2Int WorldPointToCoordinates(Vector3 worldPosition) => new Vector2Int((worldPosition.x.Floor() + instance._width / 2f).Floor(), instance._maxY - worldPosition.y.Ceiling());
	public static Vector3 CoordinatesToWorldPoint(Vector2Int coordinates) => new Vector3(coordinates.x - (instance._width - 1) / 2f, instance._maxY - coordinates.y - 1, 0);

	public static bool IsThereAnythingAt(Vector2Int position, out Block block, out Construction construction) {
		block = instance.blocksPerDepth[position.y][position.x];
		construction = instance.constructions.Of(position);
		return block || construction;
	}

	public static bool TryGetBlock(Vector2Int position, out Block block) => block = InGrid(position, false) ? instance.blocksPerDepth[position.y][position.x] : null;

	public static bool InGrid(Vector2Int position, bool orOnGround) =>
		position.x >= 0 && position.x < instance._width && position.y >= (orOnGround ? -1 : 0) && position.y < instance.blocksPerDepth.Count;

	public static bool InGrid(Vector3 worldPosition, bool orOnGround) => InGrid(WorldPointToCoordinates(worldPosition), orOnGround);
}