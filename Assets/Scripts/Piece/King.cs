using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    public bool inCheck = false;

    private readonly List<Space> danagerSpaces = new List<Space>();

    private void Start()
    {

        SetUpPiece();

        storeMovementModify = movementModifier;

        isKing = true;

        PieceManager.SetKingPiece(this);
    }

    public bool CheckIfInCheckMate()
    {
        int spacesNotSafe = 0;
        int spaceWithSameColour = 0;
        for(int i = 0; i < movementModifier.Length; i++)
        {
            Space space = BoardManager.GetSpace(currentSpace.localPosition.x + movementModifier[i].x,
                currentSpace.localPosition.y + movementModifier[i].y);

            //if the king in by the edge of the board, then the spaces around it are still not
            //safe since it cannot move to a space that doesnt exist
            if (space == null)
            {
                spacesNotSafe++;
                spaceWithSameColour++;
                continue;
            }

            if (space.hasPieceOnIt || danagerSpaces.Contains(space))
                spacesNotSafe++;

            if (space.pieceColour == pieceColour)
                spaceWithSameColour++;

        }

        //if the king is surrounded by pieces on its team, its not in checkmate
        if (spaceWithSameColour >= 8)
            return false;

        //if there are no spaces that the king can go then its in checkmate
        return spacesNotSafe >= 8 ? true: false;
    }

    public override void ShowMoveLocations()
    {
        avaliableSpaces = BoardManager.ShowMovingSquares(GetSpaces(), pieceColour);

    }

    public override bool Move(Space moveToSpace)
    {
        Space lastCurrentSpace = currentSpace;
        //AddDangerSpace(currentSpace);

        bool didMove = base.Move(moveToSpace);
        //RemoveSpace(lastCurrentSpace);

        if (!didMove)
            return false;


        if (inCheck)
        {
            Debug.Log("feafea");
            ChangeColliderEnabled();
            PieceManager.ChangeColourCollider(pieceColour);
        }

        //The king has moved and so it is no longer in check
        inCheck = false;


        return didMove;
    }


    
    public override void AddKingDanagerSpaces()
    {
        
    }

    protected override void RemoveKingDanagerSpaces()
    {
        
    }



    public void AddDangerSpaces(Space[] spaces)
    {
        if (spaces == null)
            return;

        for (int i = 0; i < spaces.Length; i++)
        {
            if (spaces[i] == currentSpace)
            {
                inCheck = true;
            }
            danagerSpaces.Add(spaces[i]);


        }
    }


    //call this on a piece when they have moved
    //get all of the spaces that they can move since these are all danger space
    //do this by calling GetSpaceWithinRange() in piece
    public void AddDangerSpace(Space space)
    {
        if (space == currentSpace)
        {
            inCheck = true;

        }

        danagerSpaces.Add(space);



    }

    private Space[] GetSpaces()
    {
        List<Space> safeSpaces = new List<Space>();


        for (int i = 0; i < movementModifier.Length; i++)
        {
            Space space = BoardManager.GetSpace(currentSpace.localPosition.x + movementModifier[i].x,
                currentSpace.localPosition.y + movementModifier[i].y);

            if (space == null || danagerSpaces.Contains(space))
                continue;



            if (!space.hasPieceOnIt)
                safeSpaces.Add(space);

            if (space.hasPieceOnIt && space.pieceColour != pieceColour)
                safeSpaces.Add(space);
        }

        return safeSpaces.ToArray();
    }


    public void RemoveSpace(Space[] spaces)
    {
        if (danagerSpaces == null || spaces == null)
            return;

        for (int i = 0; i < spaces.Length; i++)
        {
            if (danagerSpaces.Contains(spaces[i]))
                danagerSpaces.Remove(spaces[i]);
        }
    }

    public void RemoveAllSpaces()
    {
        danagerSpaces.Clear();
    }
}
