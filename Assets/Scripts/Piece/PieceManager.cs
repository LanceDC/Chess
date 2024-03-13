using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This class deals with anything having to interact with more than one piece at any time
public static class PieceManager
{
    public static King WhiteKing { get; private set; }
    public static King BlackKing { get; private set; }

    public static readonly Color[] teamColour = { Color.white, Color.black };

    private readonly static List<Piece> pieces = new List<Piece>();

    //maybe create a private class to store the king piece, in check so it would be easier to check if its in check mate
    public static void SetKingPiece(King piece)
    {
        if (piece.pieceColour == PieceColour.Black)
            BlackKing = piece;
        else
            WhiteKing = piece;
    }

    private static King GetKing(PieceColour colour)
    {
        King king;

        if (colour == PieceColour.Black)
            king = BlackKing;
        else
            king = WhiteKing;

        return king;
    }

    public static void AddPiece(Piece piece)
    {
        pieces.Add(piece);
    }

    public static void RemovePiece(Piece piece)
    {
        if (pieces.Contains(piece))
            pieces.Remove(piece);
    }

    public static void ChangeColourCollider(PieceColour colour)
    {
        for(int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].pieceColour != colour)
                continue;

            pieces[i].ChangeColliderEnabled();
        }
    }

    public static void ChangeAllPieceColliders()
    {
        for(int i = 0; i < pieces.Count; i++)
        {
            pieces[i].ChangeColliderEnabled();
        }
    }


    public static void RemoveDangerSpaces(PieceColour colour)
    {
        King king = GetKing(colour);
        king.RemoveAllSpaces();
    }

    public static void AddDangerSpaces(PieceColour colour)
    {
        for(int i = 0; i < pieces.Count; i++)
        {
            if (pieces[i].pieceColour != colour)
                continue;

            pieces[i].AddKingDanagerSpaces();
        }

    }

    public static void KingInCheck(PieceColour colour)
    {
        //set the only collider to move being the king or any piece on that colour that can move into a space to save the king
        if (WhiteKing.pieceColour == colour)
            WhiteKing.inCheck = true;
        else
            BlackKing.inCheck = true;
    }
}

public enum PieceColour
{
    White,
    Black,
    None
}

