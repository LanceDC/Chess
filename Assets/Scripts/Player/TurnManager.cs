using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TurnManager : MonoBehaviour
{
    public static TurnManager instance;

    [SerializeField] private TMP_Text currentTurn;
    [SerializeField] private TMP_Text whiteName;
    [SerializeField] private TMP_Text blackName;
    [SerializeField] private TMP_Text winningText;
    [SerializeField] private Button playButton;
    [SerializeField] private GameObject startCanvas;
    [SerializeField] private GameObject gameCanvas;
    [SerializeField] private GameObject endCanvas;


    private PlayerTeam whitePlayer;
    private PlayerTeam blackPlayer;
    private PlayerTeam currentPlayer;
    private PlayerTeam lastPlayer;
    private readonly string currentTurnText = "Current Turn : ";

    // Start is called before the first frame update
    void Start()
    {
        instance = this;

        playButton.onClick.AddListener(() =>
        {
            BoardManager.CreatAllBoard();
            //make it so you cannot click on the black pieces
            PieceManager.ChangeColourCollider(PieceColour.Black);

            whitePlayer = new PlayerTeam(whiteName.text, PieceColour.White);
            blackPlayer = new PlayerTeam(blackName.text, PieceColour.Black);

            //set the starting player as white
            currentPlayer = whitePlayer;
            currentTurn.text = currentTurnText + currentPlayer.GetPlayerName();
            
            //PieceManager.ChangeColourCollider(currentPlayer.GetPieceColour());

            gameCanvas.SetActive(true);
            startCanvas.SetActive(false);

        });
    }

    public void NextTurn()
    {
        whitePlayer.GetKing(PieceManager.WhiteKing);
        blackPlayer.GetKing(PieceManager.BlackKing);




        lastPlayer = currentPlayer;

        //if the current player is white, make black the current player
        currentPlayer = currentPlayer == whitePlayer ? blackPlayer : whitePlayer;
        currentTurn.text = currentTurnText + currentPlayer.GetPlayerName();

        //Remove all danger spaces from the king
        PieceManager.RemoveDangerSpaces(lastPlayer.GetPieceColour());

        //Add updated danger spaces for the currnet player king
        PieceManager.AddDangerSpaces(lastPlayer.GetPieceColour());

        if (currentPlayer.GetKing().inCheck)
        {

            if (currentPlayer.GetKing().CheckIfInCheckMate())
            {
                ShowWinningScreen();
                return;
            }

            
            PieceManager.ChangeColourCollider(lastPlayer.GetPieceColour());
            
            //if the king is only in check then only enable its collider since the king needs to move
            currentPlayer.GetKing().ChangeColliderEnabled();


        }
        else
        {
            PieceManager.ChangeAllPieceColliders();
        }
    }


    private void ShowWinningScreen()
    {
        gameCanvas.SetActive(false);

        PieceManager.ChangeColourCollider(lastPlayer.GetPieceColour());

        winningText.text = "AND THE WINNER IS \n" + lastPlayer.GetPlayerName();

        endCanvas.SetActive(true);

    }

}
