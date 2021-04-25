using System;
using LD48.Game.Data.Blocks;
using LD48.Game.Data.Constructions;
using UnityEngine;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	public class DwarfDirectionStrategy {
		public enum Action {
			Nothing = 0,
			Mine    = 1,
			Move    = 2,
			Climb   = 3,
			Use     = 4
		}

		public  Direction        direction                { get; }
		public  Action           preferredAction          { get; private set; }
		public  float            motivation               { get; private set; }
		public  Vector2Int       destinationCoordinates   { get; private set; }
		public  Block            block                    { get; private set; }
		public  RestorationPlace restorationPlaceToUse    { get; private set; }
		public  Vector3          destination              { get; private set; }
		private int              directionPreferenceScore { get; set; }

		public DwarfDirectionStrategy(Direction direction) => this.direction = direction;

		public void Refresh(Vector3 currentPosition, IReadDwarfNeeds needs, Vector2Int dislikePosition) {
			var currentCoordinates = World.WorldPointToCoordinates(currentPosition + Vector3.up / 2);
			destinationCoordinates = GetDestinationCoordinates(currentCoordinates);
			if (destinationCoordinates == dislikePosition) {
				ChooseToDoNothing();
				return;
			}
			directionPreferenceScore = needs.GetAdditionalMotivationToGo(direction);
			if (TestNotInGrid()) return;
			if (TestBlockToMine(currentCoordinates, needs)) return;
			if (TestUse(needs)) return;
			if (direction == Direction.Up) {
				if (TestClimbUp(currentCoordinates)) return;
				ChooseToDoNothing();
				return;
			}
			if (direction == Direction.Down && World.TryGetConstruction(destinationCoordinates, out var construction)) {
				if (TestClimbDown(construction)) return;
				if (construction.type.canStandOver) {
					ChooseToDoNothing();
					return;
				}
			}
			if (TestMove(needs)) return;
			ChooseToDoNothing();
		}

		private bool TestMove(IReadDwarfNeeds needs) {
			if (direction == Direction.Self) return false;
			if (World.TryGetBlock(destinationCoordinates, out _)) return false;
			if (World.TryGetConstruction(destinationCoordinates, out var construction) && construction.type.preventsMovement) return false;
			var constructionScore = construction && construction.TryGetComponent<RestorationPlace>(out var restorationPlace) ? needs.GetAdditionalMotivationToUse(restorationPlace.need) : 0;
			preferredAction = Action.Move;
			destination = World.CoordinatesToWorldPoint(destinationCoordinates);
			motivation = 1 + directionPreferenceScore + constructionScore;
			return true;
		}

		private bool TestUse(IReadDwarfNeeds needs) {
			if (direction != Direction.Self) return false;
			if (!World.TryGetConstruction(destinationCoordinates, out var construction) || !construction.TryGetComponent<RestorationPlace>(out var restorationPlace)) return false;
			preferredAction = Action.Use;
			motivation = 2 * needs.GetAdditionalMotivationToUse(restorationPlace.need);
			restorationPlaceToUse = restorationPlace;
			return true;
		}

		private bool TestClimbDown(Construction construction) {
			if (!construction.type.canClimb) return false;
			preferredAction = Action.Climb;
			destination = World.CoordinatesToWorldPoint(destinationCoordinates);
			motivation = 1 + directionPreferenceScore;
			return true;
		}

		private void ChooseToDoNothing() {
			preferredAction = Action.Nothing;
			motivation = 0;
		}

		private bool TestClimbUp(Vector2Int currentCoordinates) {
			if (!World.TryGetConstruction(currentCoordinates, out var construction) || !construction.type.canClimb) return false;
			preferredAction = Action.Climb;
			destination = World.CoordinatesToWorldPoint(destinationCoordinates);
			motivation = directionPreferenceScore + 1;
			return true;
		}

		private bool TestBlockToMine(Vector2Int currentCoordinates, IReadDwarfNeeds needs) {
			if (!World.TryGetBlock(destinationCoordinates, out var blockAtDestination)) return false;
			if (World.TryGetConstruction(destinationCoordinates, out var construction) && construction.type.preventsFromMining) return false;
			preferredAction = Action.Mine;
			block = blockAtDestination;
			motivation = (needs[DwarfNeed.Sleep] < .25f ? -15 : 0) + (currentCoordinates.y < 0 ? 5 : 0) + (block.type.goldValue > 0 ? 8 : 3) + block.type.goldValue +
							Mathf.Min(0, directionPreferenceScore);
			return true;
		}

		private bool TestNotInGrid() {
			if (World.InGrid(destinationCoordinates, true)) return false;
			preferredAction = Action.Nothing;
			motivation = 0;
			return true;
		}

		private Vector2Int GetDestinationCoordinates(Vector2Int currentCoordinates) {
			switch (direction) {
				case Direction.Up: return currentCoordinates.With(yFunc: t => t - 1);
				case Direction.Left: return currentCoordinates.With(t => t - 1);
				case Direction.Right: return currentCoordinates.With(t => t + 1);
				case Direction.Down: return currentCoordinates.With(yFunc: t => t + 1);
				case Direction.Self: return currentCoordinates;
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}
}