
using System;
using UnityEngine;

[Serializable]
public class PlayerStatusData
{
    [SerializeField]
    private int statusId;
    [SerializeField]
    private int playerId;
    [SerializeField]
    private string profileName;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float stamina;
    [SerializeField]
    private float catchEfficiency;
    [SerializeField]
    private float pitchEfficiency;
    [SerializeField]
    private int passEfficiency;
    [SerializeField]
    private float battingEfficiency;
    [SerializeField]
    private int battingPower;
    [SerializeField]
    private int pitchingPower;
    [SerializeField]
    private int pitchingEffect;

    public PlayerStatusData(int statusId, int playerId, string profileName, float speed, float stamina, float catchEfficiency, float pitchEfficiency, int passEfficiency, float battingEfficiency, int battingPower, int pitchingPower, int pitchingEffect)
    {
        StatusId = statusId;
        PlayerId = playerId;
        ProfileName = profileName;
        Speed = speed;
        Stamina = stamina;
        CatchEfficiency = catchEfficiency;
        PitchEfficiency = pitchEfficiency;
        PassEfficiency = passEfficiency;
        BattingEfficiency = battingEfficiency;
        BattingPower = battingPower;
        PitchingPower = pitchingPower;
        PitchingEffect = pitchingEffect;
    }

    public int StatusId { get => statusId; set => statusId = value; }
    public int PlayerId { get => playerId; set => playerId = value; }
    public string ProfileName { get => profileName; set => profileName = value; }
    public float Speed { get => speed; set => speed = value; }
    public float Stamina { get => stamina; set => stamina = value; }
    public float CatchEfficiency { get => catchEfficiency; set => catchEfficiency = value; }
    public float PitchEfficiency { get => pitchEfficiency; set => pitchEfficiency = value; }
    public int PassEfficiency { get => passEfficiency; set => passEfficiency = value; }
    public float BattingEfficiency { get => battingEfficiency; set => battingEfficiency = value; }
    public int BattingPower { get => battingPower; set => battingPower = value; }
    public int PitchingPower { get => pitchingPower; set => pitchingPower = value; }
    public int PitchingEffect { get => pitchingEffect; set => pitchingEffect = value; }
}
