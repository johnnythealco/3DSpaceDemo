using Gamelogic.AbstractStrategy.AI;
using Gamelogic.AbstractStrategy.Grids;
using UnityEngine;
using UnityEngine.UI;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Examples
{
	public class Draughts : MonoBehaviour
	{
		private Player<RectPoint, GridGamePieceSettings> RedPlayer, BlackPlayer;
		public GridGameManager manager;

		public Text messageText;

		private IPieceProperties selectedPiece;

		// Use this for initialization
		void Start()
		{
			messageText.gameObject.SetActive(false);

			RedPlayer = new GridHumanPlayer("red");
			BlackPlayer = new GridHumanPlayer("black");

			manager.RegisterPlayer(RedPlayer);
			manager.RegisterPlayer(BlackPlayer);

			manager.StartGame();
			manager.OnGameOver += () => ShowMessage("Game Over! Press Space to restart");
		}

		private void ShowMessage(string message)
		{
			messageText.gameObject.SetActive(true);
			messageText.text = message;
		}

		// Update is called once per frame
		void Update()
		{
			// Reset
			if (Input.GetKeyDown(KeyCode.Space))
			{
				messageText.gameObject.SetActive(false);
				manager.StartGame();
				DeselectPiece();
			}
		}


		private void DeselectPiece()
		{
			if (selectedPiece != null)
			{
				var visualPiece = manager.GetExistingVisualPiece(selectedPiece);
				var sprite = visualPiece.GetComponentInChildren<SpriteRenderer>();
				if (sprite != null)
				{
					var colour = sprite.color;
					colour.a = 1.0f;
					sprite.color = colour;
				}
			}

			selectedPiece = null;
		}


		private void SelectPiece(IPieceProperties piece)
		{
			if (selectedPiece != null)
			{
				DeselectPiece();
			}

			selectedPiece = piece;
			var visualPiece = manager.GetExistingVisualPiece(selectedPiece);
			var sprite = visualPiece.GetComponentInChildren<SpriteRenderer>();
			if (sprite != null)
			{
				var colour = sprite.color;
				colour.a = 0.8f;
				sprite.color = colour;
			}
		}


		public void OnClick(RectPoint point)
		{
			var currentPlayer = manager.CurrentPlayer;
			if (currentPlayer is GridHumanPlayer &&
				manager.MoveManager.Done)
			{
				// If a block is occupied, try to select
				var state = manager.State;

				var piece = state.TestPosition(point);
				if (piece != null)
				{
					if (piece.playerID == currentPlayer.PlayerID)
					{
						SelectPiece(piece);
					}
				}
				else if (selectedPiece != null)
				{
					// Try to commit move
					var selectedPos = state.FindPiece(selectedPiece);
					var move = state.CreateMovePieceMove(selectedPiece, selectedPos, point);
					if (manager.CommitMove(move))
					{
						DeselectPiece();
					}
					else
					{
						// Capture?
						// Can be naive about this since the rules will validate

						// If we're trying to move to any tile 4 manhattan distance away, try and capture a piece in the middle
						var moveDist = selectedPos.DistanceFrom(point);
						if (moveDist == 4)
						{
							var centerPos = (point + selectedPos) / 2;
							var capturePiece = state.TestPosition(centerPos);
							if (capturePiece != null)
							{
								move = state.CreateCapturePieceMove(selectedPiece, capturePiece, selectedPos, point, centerPos);
								if (manager.CommitMove(move))
								{
									DeselectPiece();
								}
							}
						}
					}
				}
			}
		}
	}
}