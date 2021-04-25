namespace LD48.Game.Data.Dwarfs {
	public interface IReadDwarfNeeds {
		float this[DwarfNeed need] { get; }
		int GetAdditionalMotivationToGo(Direction direction);
		int GetAdditionalMotivationToUse(DwarfNeed restorationNeed);
	}
}