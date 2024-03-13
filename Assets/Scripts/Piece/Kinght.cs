using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kinght : Piece
{
    // Start is called before the first frame update
    void Start()
    {
        if (pieceColour == PieceColour.Black)
        {
            for (int i = 0; i < movementModifier.Length; i++)
            {
                movementModifier[i] *= -1;
            }

        }

        SetUpPiece();
    }

    public override void ShowMoveLocations()
    {
        avaliableSpaces = BoardManager.ShowMovingSquares(movementModifier, currentSpace.worldPosition, pieceColour);
    }

    public override void AddKingDanagerSpaces()
    {
        oppositeColourKing.AddDangerSpaces(avaliableSpaces);
    }

}
