using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour
{
    private Piece selectedPiece;
    private bool hasSelectedPiece = false;


    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !hasSelectedPiece)
        {
            Select();

        }
        else if(Input.GetMouseButtonDown(0))
        {
            MovePiece();
        }

        if(Input.GetMouseButtonDown(1))
        {
            Deselect();
        }
    }

    private void Deselect()
    {
        if (selectedPiece == null)
            return;

        hasSelectedPiece = false;

        //Reset the spaces to their original colour
        for(int i = 0; i < selectedPiece.avaliableSpaces.Length; i++)
        {
            selectedPiece.avaliableSpaces[i].ResetColour();
        }

        //Allow you to click on the pieces again since the space is closer to the screen but the pieces get rendered first
        BoardManager.ChangeSpaceCollider(false);
    }

    private void Select()
    {
        Collider2D collider = ClickOnObject();

        if (collider == null)
            return;


        Piece mousePiece = collider.GetComponent<Piece>();

        if (selectedPiece != mousePiece)
            Deselect();

        selectedPiece = mousePiece;

        hasSelectedPiece = true;
        //Highlight any space the piece can move
        selectedPiece.ShowMoveLocations();
        //Enable colliders for all of the spaces so you can click on them
        BoardManager.ChangeSpaceCollider(true);
    }

    private void MovePiece()
    {
        Collider2D collider = ClickOnObject();

        //If you click on anything other than the avalible spaces then stop showing where that piece can move
        if (collider == null)
        {
            Deselect();
            return;
        }

        //Get the space that the mouse clicked on
        Space mouseSpace = BoardManager.GetSpace(collider.transform.position.x, collider.transform.position.y);

        if (mouseSpace == selectedPiece.currentSpace)
            return;

        //Atempt to move the selected piece
        bool didMove = selectedPiece.Move(mouseSpace);

        //If you did not click an avalible spaces then stop showing where that piece can move
        if (!didMove)
        {
            Deselect();
            return;
        }

        //The piece did move so stop selecting it and swith which team can be selected
        Deselect();
        TurnManager.instance.NextTurn();
    }

    private Collider2D ClickOnObject()
    {
        //Get the screen position of the mouse
        Vector2 mousePosition = Input.mousePosition;

        //Get the world position of the mouse
        Vector2 worldMousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

        //Return any collider that was clicked on
        return Physics2D.OverlapCircle(worldMousePosition, 0.25f);
    }
}
