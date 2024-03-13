using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Piece : MonoBehaviour
{
    public string pieceName;
    public Sprite pieceSprite;
    public PieceColour pieceColour;
    public Vector2[] movementModifier;
    public Space currentSpace;
    public bool tester = false;
    [HideInInspector] public bool isKing = false;
    public Space[] avaliableSpaces;

    private SpriteRenderer spriteRenderer;
    [SerializeField] private bool moveVerticallyAndHorizontal = false;
    [SerializeField] private bool moveDiagonally = false;


    [SerializeField] protected new CircleCollider2D collider;
    protected Vector2[] storeMovementModify;

    protected King oppositeColourKing;


    private void Start()
    {
        SetUpPiece();

        AddKingDanagerSpaces();
    }

    protected void SetUpPiece()
    {

        collider = GetComponent<CircleCollider2D>();

        spriteRenderer = GetComponent<SpriteRenderer>();
        //Setting the colour of the piece based upon which team it is on
        spriteRenderer.color = PieceManager.teamColour[(int)pieceColour];
        spriteRenderer.sprite = pieceSprite;

    }

    public void SetOpposingKing(King king)
    {
        oppositeColourKing = king;
    }

    public virtual bool Move(Space moveToSpace)
    {
        bool isAvaliable = false;
        for (int i = 0; i < avaliableSpaces.Length; i++)
        {
            //See if the space clicked on is a space that the piece can move to
            if (moveToSpace == avaliableSpaces[i])
            {
                //The piece moved
                isAvaliable = true;
                break;
            }
        }
        //if the space is not and avaliable space then return false so you don't change turn
        if (!isAvaliable)
            return false;

        //Store this piece as the piece to being on the space it is going to move to
        moveToSpace.SetNewPiece(this);
        
        //Actually move the piece
        transform.position = moveToSpace.localPosition;
        currentSpace.RemoveCurrentPiece();

        //Store the space that the piece is on
        currentSpace = moveToSpace;

        //return that the piece did move
        return true;
    }

    public virtual void ShowMoveLocations()
    {
        avaliableSpaces = BoardManager.ShowMovingSquares(GetSpacesWithinRange(), pieceColour);
    }

    public virtual void AddKingDanagerSpaces()
    {
        //Set all of the spaces that this piece could move as a space the king on the other team could not move to,
        //so it would not move to a space that would put it in check
        oppositeColourKing.AddDangerSpaces(GetSpacesWithinRange());
    }

    protected virtual void RemoveKingDanagerSpaces()
    {
        oppositeColourKing.RemoveSpace(avaliableSpaces);
    }



    public void DestroyPiece()
    {
        PieceManager.RemovePiece(this);
        RemoveKingDanagerSpaces();

        Destroy(gameObject);
    }

    public void ChangeColliderEnabled()
    {
        collider.enabled = !collider.enabled;
    }

    public Space[] GetSpacesWithinRange()
    {

        List<Space> actualSpaces = new List<Space>();

        /*
            Visual representaion of the parameters for GetSpace()
            (-1, 1)(0, 1)(1, 1)
            (-1, 0)(0, 0)(1, 0)
            (-1,-1)(0,-1)(1,-1)
         */

        //Get avaliable spaces on the horizontal and vertical axis of this space
        if (moveVerticallyAndHorizontal)
        {
            actualSpaces.AddRange(GetSpaces(-1, 0));
            actualSpaces.AddRange(GetSpaces(1, 0));
            actualSpaces.AddRange(GetSpaces(0, 1));
            actualSpaces.AddRange(GetSpaces(0, -1));
        }

        //Get avaliable spaces on the diagonally axis of this space
        if (moveDiagonally)
        {
            actualSpaces.AddRange(GetSpaces(-1, -1));
            actualSpaces.AddRange(GetSpaces(1, 1));
            actualSpaces.AddRange(GetSpaces(-1, 1));
            actualSpaces.AddRange(GetSpaces(1, -1));
        }

        return actualSpaces.ToArray();
    }

    public Space[] GetSpaces(int row, int column)
    {
        List<Space> actualSpaces = new List<Space>();
        //8 since that is the most spaces in a row that a piece can move
        for (int i = 0; i < 8; i++)
        {
            Space space = BoardManager.GetSpace(currentSpace.localPosition.x + (i * row), currentSpace.localPosition.y + (i * column));

            if (space == null || space == currentSpace)
                continue;


            if (!space.hasPieceOnIt)
            {
                actualSpaces.Add(space);
                continue;
            }

            if (space.pieceColour == pieceColour)
                break;
            else
            {
                actualSpaces.Add(space);
                if (space.IsKing())
                {
                    PieceManager.KingInCheck(space.pieceColour);
                    oppositeColourKing.inCheck = true;
                    //if its the king, 
                    //set that king to being in check
                    //continue getting spaces so that if this piece is the rook and puts the king in check,
                    //they wont be able to move in that row or column since that would still put the king in check
                    continue;
                }
                break;
            }

        }
        return actualSpaces.ToArray();
    }
}