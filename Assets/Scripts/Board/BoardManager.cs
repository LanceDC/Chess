using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoardManager : MonoBehaviour
{
    public GameObject boardSquare;
    public Piece[] boardPieces = new Piece[6];
    public static Space[,] spaces = new Space[8, 8];

    private static readonly GameObject[,] squareArray = new GameObject[8, 8]; //[row, column]

    private static BoardManager instance;

    private void Start()
    {
        instance = this;
    }
    
    public static void CreatAllBoard()
    {
        instance.CreateBoard();

        //place king first to set the opposite king
        King whiteKing = instance.PlaceKings(0, PieceColour.White);
        King blackKing = instance.PlaceKings(7, PieceColour.Black);

        //Pawn
        instance.PlacePieces(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 6, 0, PieceColour.Black, whiteKing);
        instance.PlacePieces(new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }, 1, 0, PieceColour.White, blackKing);


        //Queen
        instance.PlacePieces(new int[] { 4 }, 7, 1, PieceColour.Black, whiteKing);
        instance.PlacePieces(new int[] { 3 }, 0, 1, PieceColour.White, blackKing);

        //Bishop
        instance.PlacePieces(new int[] { 2, 5 }, 7, 2, PieceColour.Black, whiteKing);
        instance.PlacePieces(new int[] { 2, 5 }, 0, 2, PieceColour.White, blackKing);

        //Knight
        instance.PlacePieces(new int[] { 1, 6 }, 7, 4, PieceColour.Black, whiteKing);
        instance.PlacePieces(new int[] { 1, 6 }, 0, 4, PieceColour.White, blackKing);

        //Rook
        instance.PlacePieces(new int[] { 0, 7 }, 7, 5, PieceColour.Black, whiteKing);
        instance.PlacePieces(new int[] { 0, 7 }, 0, 5, PieceColour.White, blackKing);

    }

    private void CreateBoard()
    {
        for (int y = 0; y < squareArray.GetLength(0); y++)
        {
            for (int x = 0; x < squareArray.GetLength(1); x++)
            {
                Vector3 squarePosition = new Vector3(x, y, 0);
                squareArray[x, y] = Instantiate(boardSquare, squarePosition + transform.position, Quaternion.identity, transform);

                //Alternate the colour of the board
                Color squareColour = (x + y) % 2 == 0 ? Color.white : Color.blue;

                SpriteRenderer sr = squareArray[x, y].GetComponent<SpriteRenderer>();
                sr.color = squareColour;
                sr.sortingOrder = -1;

                squareArray[x, y].GetComponent<Collider2D>().enabled = false;

                //Add the new space to the array
                spaces[x, y] = new Space(new Vector3(x, y, 0), sr);
            }
        }
    }

    private void PlacePieces(int[] row, int column, int piece, PieceColour colour, King opposingKing)
    {
        boardPieces[piece].pieceColour = colour;
        for (int i = 0; i < row.Length; i++)
        {
            Piece newPiece = Instantiate(boardPieces[piece], GetSquarePosition(row[i], column), Quaternion.identity);
            newPiece.currentSpace = spaces[row[i], column];
            spaces[row[i], column].SetNewPiece(newPiece);
            newPiece.SetOpposingKing(opposingKing);
            PieceManager.AddPiece(newPiece);
        }
    }

    private King PlaceKings(int coloumn, PieceColour colour)
    {
        int row = colour == PieceColour.Black ? 3 : 4;

        King newKing = Instantiate(boardPieces[3], GetSquarePosition(row, coloumn), Quaternion.identity) as King;
        newKing.currentSpace = spaces[row, coloumn];
        newKing.pieceColour = colour;
        newKing.pieceColour = colour;
        PieceManager.AddPiece(newKing);

        spaces[row, coloumn].SetNewPiece(newKing);


        return newKing;
    }

    public Vector3 GetSquarePosition(int row, int column)
    {
        return squareArray[row, column].transform.position;
    }

    public Vector3 GetSquarePosition(Space space)
    {
        return squareArray[(int)space.worldPosition.x, (int)space.worldPosition.y].transform.position;
    }


    public static Space GetSpace(float row, float column)
    {
        //Changing the local location of the space into the world location of the space
        row += 3.5f;
        column += 3.5f;

        int iRow = (int)row;
        int iColumn = (int)column;

        //if the location values are greater than 7 or less than 0 the space does not exist
        if (iColumn > 7 || iRow > 7 || iColumn < 0 || iRow < 0)
            return null;

        return spaces[iRow, iColumn];

    }


    public static void ChangeSpaceCollider(bool isActive)
    {
        foreach (GameObject squareObject in squareArray)
        {
            squareObject.GetComponent<Collider2D>().enabled = isActive;
        }
    }

    public static Space[] ShowMovingSquares(Space[] spaces, PieceColour colour)
    {
        List<Space> spaceList = new List<Space>();

        for (int i = 0; i < spaces.Length; i++)
        {
            //if the piece has the same colour on it
            if (spaces[i].hasPieceOnIt && spaces[i].pieceColour == colour)
            {
                continue;
            }

            //if the space is empty
            if (spaces[i].hasPieceOnIt && spaces[i].pieceColour != colour)
            {
                spaces[i].SetColour(Space.arrayOfSpaceColour[2]);
            }

            //if the space has an opposing colour on it
            else
            {
                spaces[i].SetColour(Space.arrayOfSpaceColour[3]);
            }

            spaceList.Add(spaces[i]);
        }

        return spaceList.ToArray();
    }

    public static Space[] ShowMovingSquares(Vector2[] movingModifier, Vector2 currentPosition, PieceColour colour)
    {
        int currentRow = (int)currentPosition.x;
        int currentColumn = (int)currentPosition.y;

        List<Space> spaceList = new List<Space>();

        for (int i = 0; i < movingModifier.Length; i++)
        {
            int row = (int)movingModifier[i].x;
            int column = (int)movingModifier[i].y;


            //if it goes out of bounds
            if (currentRow + row > spaces.GetLength(0) - 1 || currentColumn + column > spaces.GetLength(1) - 1
                || currentRow + row < 0 || currentColumn + column < 0)
                continue;

            //if the piece has the same colour on it
            if (spaces[currentRow + row, currentColumn + column].hasPieceOnIt &&
                spaces[currentRow + row, currentColumn + column].pieceColour == colour)
            {
                continue;
            }

            //if the space is empty
            if (spaces[currentRow + row, currentColumn + column].hasPieceOnIt &&
                spaces[currentRow + row, currentColumn + column].pieceColour != colour)
            {
                spaces[currentRow + row, currentColumn + column].SetColour(Space.arrayOfSpaceColour[2]);


            }
            //if the space has an opposing colour on it
            else
            {
                spaces[currentRow + row, currentColumn + column].SetColour(Space.arrayOfSpaceColour[3]);
            }

            spaceList.Add(spaces[currentRow + row, currentColumn + column]);

        }

        //return an array of avalible spaces
        return spaceList.ToArray();
    }

    //Creates a new queen when a pawn reaches the other side of the board
    public static void CreatePiece(int index, Piece pawn, King king)
    {
        Space space = pawn.currentSpace;
        //Create a queen piece
        Piece newPiece = Instantiate(instance.boardPieces[index], instance.GetSquarePosition(space), Quaternion.identity);

        //Remove the pawn from the game so it cannot be called anymore since its now a queen
        pawn.DestroyPiece();

        //Set up the new piece
        space.SetNewPiece(newPiece);
        newPiece.SetOpposingKing(king);
        newPiece.currentSpace = space;
        newPiece.pieceColour = pawn.pieceColour;
        PieceManager.AddPiece(newPiece);
    }
}


 
