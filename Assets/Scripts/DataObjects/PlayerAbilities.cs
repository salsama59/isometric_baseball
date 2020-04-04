using System.Collections.Generic;
using UnityEngine;

public class PlayerAbilities : MonoBehaviour
{
    List<PlayerAbility> playerAbilityList = new List<PlayerAbility>();
    private bool hasSpecialAbilities = false;

    public void AddAbility(PlayerAbility playerAbility)
    {
        PlayerAbilityList.Add(playerAbility);
    }

    public List<PlayerAbility> PlayerAbilityList { get => playerAbilityList; set => playerAbilityList = value; }
    public bool HasSpecialAbilities { get => hasSpecialAbilities; set => hasSpecialAbilities = value; }
}
