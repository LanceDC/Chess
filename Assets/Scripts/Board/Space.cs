using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Space
{
    //position in the board (-3.5 on the x and y in world space)
    public Vector3 localPosition;
    public Vector3 worldPosition;
    public bool hasPieceOnIt;
    public PieceColour pieceColour;
    public Color spaceColour;

    //All of the colours the space can be,
    //Black and white being the team colour
    //Green if a piece can move to this space
    //Red for if piece can move to this space and it has a piece on it that is of the other team
    public static readonly Color[] arrayOfSpaceColour = { Color.white, Color.black, Color.red, Color.green };

    private SpriteRenderer spriteRenderer;
    //The colour this space is when it is not being highlighted
    private readonly Color deafultColour;
    private Piece pieceOnSpace;


    public Space(Vector3 setPosition, SpriteRenderer renderer)
    {
        SetNewPosition(setPosition);
        hasPieceOnIt = false;
        pieceColour = PieceColour.None;
        spriteRenderer = renderer;
        deafultColour = renderer.color;
    }

    public void SetColour(Color colour)
    {
        spriteRenderer.color = colour;
    }

    public void SetNewPiece(Piece piece)
    {
        pieceColour = piece.pieceColour;
        hasPieceOnIt = true;
        spaceColour = deafultColour;

        if (pieceOnSpace != piece && pieceOnSpace != null)
            pieceOnSpace.DestroyPiece();

        pieceOnSpace = piece;
    }

    public void RemoveCurrentPiece()
    {
        hasPieceOnIt = false;
        pieceColour = PieceColour.None;
        spaceColour = deafultColour;
        pieceOnSpace = null;
    }

    public void ResetColour()
    {
        spriteRenderer.color = deafultColour;
    }

    public void SetNewPosition(Vector3 newPos)
    {
        localPosition = newPos - new Vector3(3.5f, 3.5f);
        worldPosition = newPos;
    }

    public bool IsKing()
    {
        return pieceOnSpace.isKing;
    }
}
