using System.Collections.Generic;
using LD48.Game.Data.Blocks;
using LD48.Game.Data.Dwarfs;
using Utils.Id;

namespace LD48.Constants {
	public static class LdMemory {
		public static IReadOnlyDictionary<int, BlockType> blockTypes { get; } = DataId.FromResources<BlockType>("Data/BlockTypes");
		public static IReadOnlyDictionary<int, DwarfType> dwarfTypes { get; } = DataId.FromResources<DwarfType>("Data/DwarfTypes");
	}
}