using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private PlayerFieldPositionEnum playerFieldPosition;
    private string playerName;
    private bool isAllowedToMove = false;
    private float speed;
    private float catchEfficiency;
    private float pitchEfficiency;
    private float passEfficiency;
    private float battingEfficiency;
    private int battingPower;
    private int pitchingPower;
    private float pitchingEffect;
    private int stamina;

    public PlayerFieldPositionEnum PlayerFieldPosition { get => playerFieldPosition; set => playerFieldPosition = value; }
    public string PlayerName { get => playerName; set => playerName = value; }
    public bool IsAllowedToMove { get => isAllowedToMove; set => isAllowedToMove = value; }
    public float Speed { get => speed; set => speed = value; }
    public float CatchEfficiency { get => catchEfficiency; set => catchEfficiency = value; }
    public float PitchEfficiency { get => pitchEfficiency; set => pitchEfficiency = value; }
    public float PassEfficiency { get => passEfficiency; set => passEfficiency = value; }
    public float BattingEfficiency { get => battingEfficiency; set => battingEfficiency = value; }
    public int BattingPower { get => battingPower; set => battingPower = value; }
    public int PitchingPower { get => pitchingPower; set => pitchingPower = value; }
    public float PitchingEffect { get => pitchingEffect; set => pitchingEffect = value; }
    public int Stamina { get => stamina; set => stamina = value; }
}
