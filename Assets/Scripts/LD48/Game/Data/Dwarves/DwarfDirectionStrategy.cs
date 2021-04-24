using System;
using LD48.Game.Data.Blocks;
using LD48.Game.Data.Constructions;
using UnityEngine;
using Utils.Extensions;

namespace LD48.Game.Data.Dwarfs {
	public class DwarfDirectionStrategy {
		public enum Direction {
			Up    = 0,
			Left  = 1,
			Right = 2,
			Down  = 3,
		}

		public enum Action {
			Nothing = 0,
			Mine    = 1,
			Move    = 2,
			Climb   = 3
		}

		public Direction direction       { get; }
		public  Action    preferredAction { get; private set; }
		public  float     motivation      { get; private set; }

		public Block   block       { get; private set; }
		public Vector3 destination { get; private set; }

		public DwarfDirectionStrategy(Direction direction) => this.direction = direction;

		public void Refresh(Vector3 currentPosition) {
			var currentCoordinates = World.WorldPointToCoordinates(currentPosition + Vector3.up / 2);
			var destinationCoordinates = GetDestinationCoordinates(currentCoordinates);
			if (TestNotInGrid(destinationCoordinates)) return;
			if (TestBlockToMine(destinationCoordinates)) return;
			if (direction == Direction.Up) {
				if (TestClimbUp(currentCoordinates, destinationCoordinates)) return;
				ChooseToDoNothing();
				return;
			}
			if (direction == Direction.Down && World.TryGetConstruction(destinationCoordinates, out var construction)) {
				if (TestClimbDown(construction, destinationCoordinates)) return;
				if (construction.type.canStandOver) {
					ChooseToDoNothing();
					return;
				}
			}
			preferredAction = Action.Move;
			destination = World.CoordinatesToWorldPoint(destinationCoordinates);
			motivation = 1;
		}

		private bool TestClimbDown(Construction construction, Vector2Int destinationCoordinates) {
			if (construction.type.canClimb) {
				preferredAction = Action.Climb;
				destination = World.CoordinatesToWorldPoint(destinationCoordinates);
				motivation = 1;
				return true;
			}
			return false;
		}

		private void ChooseToDoNothing() {
			preferredAction = Action.Nothing;
			motivation = 0;
		}

		private bool TestClimbUp(Vector2Int currentCoordinates, Vector2Int destinationCoordinates) {
			if (!World.TryGetConstruction(currentCoordinates, out var construction) || !construction.type.canClimb) return false;
			preferredAction = Action.Climb;
			destination = World.CoordinatesToWorldPoint(destinationCoordinates);
			motivation = 1;
			return true;
		}

		private bool TestBlockToMine(Vector2Int destinationCoordinates) {
			if (!World.TryGetBlock(destinationCoordinates, out var blockAtDestination)) return false;
			preferredAction = Action.Mine;
			block = blockAtDestination;
			motivation = 2 + block.type.goldValue;
			return true;
		}

		private bool TestNotInGrid(Vector2Int destinationCoordinates) {
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
				default: throw new ArgumentOutOfRangeException();
			}
		}
	}
}