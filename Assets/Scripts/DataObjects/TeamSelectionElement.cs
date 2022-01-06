using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamSelectionElement
{
    private TeamData team;
    private GameObject userInterfaceElement;

    public TeamData Team { get => team; set => team = value; }
    public GameObject UserInterfaceElement { get => userInterfaceElement; set => userInterfaceElement = value; }
}
