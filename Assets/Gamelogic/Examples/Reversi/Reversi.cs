using System.Collections.Generic;
using System.Linq;
using Gamelogic.AbstractStrategy.AI;
using Gamelogic.Grids;
using Gamelogic.AbstractStrategy.Grids;
using UnityEngine.UI;

namespace Gamelogic.AbstractStrategy.Examples
{
	public class Reversi : GLMonoBehaviour
	{
		#region Constants
		public const string Player0ID = "0";
		public const string Player1ID = "1";
		#endregion

		#region Public Variables
		public GridGameManager gameManager;
		public bool doAI;
		public Text messageText;
		public BehaviourTree behaviourTree;
		#endregion

		#region Private Fieldss
		private Player<RectPoint, GridGamePieceSettings> player0;
		private Player<RectPoint, GridGamePieceSettings> player1;
		#endregion

		#region Unity Messages
		public void Start()
		{
			messageText.gameObject.SetActive(false);

			player0 = new GridHumanPlayer(Player0ID);

			if (doAI)
			{
				player1 = new GridAIPlayer(Player1ID, behaviourTree);
			}
			else
			{
				player1 = new GridHumanPlayer(Player1ID);
			}

			gameManager.RegisterPlayer(player0);
			gameManager.RegisterPlayer(player1);

			gameManager.TurnManager.onTurnStateChange += (oldState, newState) =>
			{
				if (newState != PlayerTurnState.Running) return;

				var moves = AddReversiPieceRule.GetValidPositions(gameManager.State, gameManager.CurrentPlayer);

				if (!moves.Any())
				{
					gameManager.TurnManager.AdvanceTurnState();
				}
			};

			gameManager.StartGame();
			gameManager.OnGameOver += OnGameOver;
		}
		#endregion

		#region Grids messages
		public void OnClick(RectPoint point)
		{
			var currentPlayer = gameManager.CurrentPlayer;

			if (currentPlayer is GridHumanPlayer &&
				gameManager.MoveManager.Done)
			{
				var state = gameManager.State;
				var rules = gameManager.Rules;

				// Create a piece for the current player and commit the move
				var newMove = state.CreateAddPieceMove(
					rules.CreatePieceProperties(rules.GetPieceSettings(Player0ID, currentPlayer.PlayerID), currentPlayer),
					point);

				var positionsToReplace = GetPositionsToReplace(gameManager, point);

				bool validMove = gameManager.CommitMove(newMove);

				if (validMove) //if a valid active move was made
				{
					//replace all the enemy cells between current player pieces
					//and the newly added piece
					foreach (var rectPoint in positionsToReplace)
					{
						var newMove2 = state.CreateReplacePieceMove(gameManager.GameGrid[rectPoint][0],
							rules.CreatePieceProperties(rules.GetPieceSettings(Player0ID, currentPlayer.PlayerID), currentPlayer),
							rectPoint);

						gameManager.CommitMove(newMove2, false);
					}
				}
			}
		}
		#endregion

		#region Events
		private void OnGameOver()
		{
			var grid = gameManager.GameGrid;

			int player0Count = 0;
			int player1Count = 0;

			foreach (var point in grid)
			{
				var pieces = grid[point];

				player0Count += pieces.Count(p => p.pieceID == "0");
				player1Count += pieces.Count(p => p.pieceID == "1");
			}

			if (player0Count == player1Count)
			{
				messageText.text = "The game is a draw";
			}
			else if (player0Count > player1Count)
			{
				messageText.text = "Player 0 wins";
			}
			else
			{
				messageText.text = "Player 1 wins";
			}

			messageText.gameObject.SetActive(true);
		}
		#endregion

		#region Methods
		public static IEnumerable<RectPoint> GetPositionsToReplace(GridGameManager gameManager, RectPoint position)
		{
			var grid = gameManager.GameGrid;
			var currentPlayer = gameManager.CurrentPlayer;
			var positionsToReplace = new List<RectPoint>();

			foreach (var direction in RectPoint.MainDirections)
			{
				var potentialPositionsToReplace = new List<RectPoint>();

				foreach (var point in grid.LineIterator(position, direction).Skip(1))
				{
					if (grid[point].Any())
					{
						if (grid[point][0].playerID != currentPlayer.PlayerID)
						{
							potentialPositionsToReplace.Add(point);
						}
						else
						{
							positionsToReplace.AddRange(potentialPositionsToReplace);
							break;
						}
					}
					else
					{
						break;
					}
				}
			}

			return positionsToReplace;
		}
		#endregion
	}
}