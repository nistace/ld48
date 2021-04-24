using System.Collections.Generic;
using LD48.Game.Data.Blocks;
using Utils.Id;

namespace LD48.Constants {
	public static class LdMemory {
		public static IReadOnlyDictionary<int, BlockType> gameResourceTypes { get; } = DataId.FromResources<BlockType>("Data/BlockTypes");
	}
}