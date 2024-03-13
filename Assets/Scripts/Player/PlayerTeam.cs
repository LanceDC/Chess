using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerTeam
{
    private readonly string name;
    private Color playerColour;
    private readonly PieceColour teamColour;
    private King king;

    public PlayerTeam(string setName, PieceColour setColour)
    {
        name = setName;
        teamColour = setColour;
        playerColour = PieceManager.teamColour[(int)setColour];
    }

    //When getting the player name, make the colour of the name the same colour as its team
    public string GetPlayerName()
    {
        return "<color=#"+ColorUtility.ToHtmlStringRGB(playerColour) +">" + name +" </color>";
    }

    public PieceColour GetPieceColour()
    {
        return teamColour;
    }

    public King GetKing()
    {
        return king;
    }

    public void GetKing(King setKing)
    {
        king = setKing;
    }
}
