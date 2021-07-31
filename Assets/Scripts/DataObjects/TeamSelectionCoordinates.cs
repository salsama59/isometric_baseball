using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectionCoordinates
{
    private int row;
    private int collumn;

    public TeamSelectionCoordinates(int row, int collumn)
    {
        Row = row;
        Collumn = collumn;
    }

    public int Row { get => row; set => row = value; }
    public int Collumn { get => collumn; set => collumn = value; }
}
