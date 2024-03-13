using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pawn : Piece
{
    private bool isFirstMove = true;

    private Vector2[] AttackMovementModify = new Vector2[2]
    {
        new Vector2(1,1),
        new Vector2(-1,1)
    };

    private int oppositeSideY = 7;

    private void Start()
    {
        storeMovementModify = movementModifier;

        

        if (isFirstMove)
            movementModifier = new Vector2[] { new Vector2(0, 1), new Vector2(0, 2) };


        if (pieceColour == PieceColour.Black)
        {
            oppositeSideY = 0;

            for (int i = 0; i < movementModifier.Length; i++)
            {
                movementModifier[i] *= -1;
            }

            for (int i = 0; i < storeMovementModify.Length; i++)
            {
                storeMovementModify[i] *= -1;
            }

            for (int i = 0; i < AttackMovementModify.Length; i++)
            {
                AttackMovementModify[i] *= -1;
            }
        }

        SetUpPiece();

        AddKingDanagerSpaces();
    }

    public override void ShowMoveLocations()
    {
        avaliableSpaces = BoardManager.ShowMovingSquares(CheckSpace(), pieceColour);
    }

    public override bool Move(Space moveToSpace)
    {
        bool didMove = base.Move(moveToSpace);

        if (!didMove)
            return false;


        //When the pawn reaches the other side of the board, change into a queen
        if (moveToSpace.worldPosition.y == oppositeSideY)
        {
            BoardManager.CreatePiece(1, this, oppositeColourKing);
        }

        isFirstMove = false;
        movementModifier = storeMovementModify;
        //PieceManager.AddDangerSpaces(oppositeColourKing.pieceColour, CheckAttackSpaces(true));
        return didMove;
    }

    public override void AddKingDanagerSpaces()
    {
        //after piece has moved, get all of the spaces this piece can attack
        //and tell the king of the opposite colour that it cannot move into these spaces
        //since it would put it into check
        oppositeColourKing.AddDangerSpaces(CheckAttackSpaces(true));
    }

    protected override void RemoveKingDanagerSpaces()
    {
        base.RemoveKingDanagerSpaces();

        oppositeColourKing.RemoveSpace(CheckAttackSpaces(true));
    }

    private Space[] CheckSpace()
    {
        List<Space> actualSpaces = new List<Space>();

        collider = GetComponent<CircleCollider2D>();
        for(int i = 0; i < movementModifier.Length; i++)
        {
            Space space = BoardManager.GetSpace(currentSpace.localPosition.x + movementModifier[i].x,
                currentSpace.localPosition.y + movementModifier[i].y);

            //if there is a space infront of the pawn then break since the pawn cannot move
            if (space.hasPieceOnIt)
                break;

            actualSpaces.Add(space);

        }

        actualSpaces.AddRange(CheckAttackSpaces());
        return actualSpaces.ToArray();
    }

    //Only return a space if the spaces in the front diagonal spaces have a piece on the opposite team
    private Space[] CheckAttackSpaces(bool kingCheck = false)
    {
        List<Space> attackSpaces = new List<Space>();

        for (int i = 0; i < AttackMovementModify.Length; i++)
        {
            Space space = BoardManager.GetSpace(currentSpace.localPosition.x + AttackMovementModify[i].x,
                currentSpace.localPosition.y + AttackMovementModify[i].y);

            if (space == null|| !space.hasPieceOnIt && !kingCheck)
                continue;

            if (space.pieceColour != pieceColour)
                attackSpaces.Add(space);

        }

        return attackSpaces.ToArray();
    }


}
