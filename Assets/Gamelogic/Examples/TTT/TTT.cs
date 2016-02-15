using Gamelogic.AbstractStrategy.AI;
using Gamelogic.AbstractStrategy.Grids;
using UnityEngine;
using UnityEngine.UI;
using Gamelogic.Grids;

namespace Gamelogic.AbstractStrategy.Examples
{
	public class TTT : MonoBehaviour
	{
		#region Fields
		public GridGameManager manager;
		public BehaviourTree aiTreeX;
		public BehaviourTree aiTreeO;
		public bool doAI;
		public Text messageText;

		private Player<RectPoint, GridGamePieceSettings> XPlayer, OPlayer;
		#endregion

		// Use this for initialization
		void Start () 
		{
			messageText.gameObject.SetActive(false);

			if (doAI && aiTreeX != null)
			{
				XPlayer = new GridAIPlayer("X", aiTreeX);
			}
			else
			{
				XPlayer = new GridHumanPlayer("X");
			}
			if (doAI && aiTreeO != null)
			{
				OPlayer = new GridAIPlayer("O", aiTreeO);
			}
			else
			{
				OPlayer = new GridHumanPlayer("O");
			}

			manager.RegisterPlayer(XPlayer);
			manager.RegisterPlayer(OPlayer);

			manager.StartGame();

			manager.OnGameOver += () => ShowMessage("Game Over! Press Space to restart");
		}

		private void ShowMessage(string message)
		{
			messageText.gameObject.SetActive(true);
			messageText.text = message;
		}

		// Update is called once per frame
		void Update ()
		{
			// Reset
			if (Input.GetKeyDown(KeyCode.Space))
			{
				messageText.gameObject.SetActive(false);
				manager.StartGame();
			}
		}

		// Event fired when grid element is clicked
		public void OnClick(RectPoint point)
		{
			var currentPlayer = manager.CurrentPlayer;
			if (currentPlayer is GridHumanPlayer &&
			    manager.MoveManager.Done)
			{
				var state = manager.State;
				var rules = manager.Rules;

				// Create a piece for the current player and commit the move
				var newMove = state.CreateAddPieceMove(
					rules.CreatePieceProperties(rules.GetPieceSettings("piece", currentPlayer.PlayerID), currentPlayer),
					point);

				manager.CommitMove(newMove);
			}
		}
	}
}