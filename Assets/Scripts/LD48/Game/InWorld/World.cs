using System.Collections.Generic;
using System.Linq;
using LD48.Constants;
using LD48.Game.Data.Blocks;
using UnityEngine;
using Utils.Extensions;

public class World : MonoBehaviour {
	[SerializeField] protected Transform _blockContainer;
	[SerializeField] protected int       _maxY;
	[SerializeField] protected int       _width       = 12;
	[SerializeField] protected int       _secureDepth = 10;
	[SerializeField] protected Block     _block;

	private List<Block[]> blocksPerDepth { get; } = new List<Block[]>();
	private Queue<Block>  pooledBlocks   { get; } = new Queue<Block>();

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
		var possibleTypes = LdMemory.gameResourceTypes.Values.Where(t => depth <= t.maxDepth && depth >= t.minDepth && t.frequency > 0).ToArray();
		while (blocksPerDepth.Count <= depth) blocksPerDepth.Add(new Block[_width]);
		for (var position = 0; position < _width; ++position) {
			var block = GetNewBlock();
			block.transform.position = new Vector3(position - (_width - 1) / 2f, _maxY - depth - 1, 0);
			var blockType = possibleTypes.Random(t => t.frequency);
			blocksPerDepth[depth][position] = block;
			blocksPerDepth[depth][position].Init(blockType, new Vector2Int(position, depth));
		}
	}

	private Block GetNewBlock() {
		if (pooledBlocks.Count == 0) return Instantiate(_block, _blockContainer);
		var block = pooledBlocks.Dequeue();
		block.gameObject.SetActive(true);
		return block;
	}

	[ContextMenu("Clear")] private void Clear() => _blockContainer.ClearChildren();
}