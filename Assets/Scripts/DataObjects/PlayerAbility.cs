using UnityEngine;

public class PlayerAbility
{
    private string abilityName;
    private AbilityTypeEnum abilityType;
    public delegate void AbilityDelegate(GameObject actionUser);
    private AbilityDelegate abilityAction;
    private AbilityCategoryEnum abilityCategory;
    private GameObject playerReference;
    private bool isCommandPhaseMandatory;

    public PlayerAbility(string abilityName, AbilityTypeEnum abilityType, AbilityCategoryEnum abilityCategory, AbilityDelegate abilityAction, GameObject playerReference, bool isCommandPhaseMandatory = false)
    {
        AbilityName = abilityName;
        AbilityType = abilityType;
        AbilityAction = abilityAction;
        AbilityCategory = abilityCategory;
        PlayerReference = playerReference;
        IsCommandPhaseMandatory = isCommandPhaseMandatory;
    }

    public string AbilityName { get => abilityName; set => abilityName = value; }
    public AbilityTypeEnum AbilityType { get => abilityType; set => abilityType = value; }
    public AbilityDelegate AbilityAction { get => abilityAction; set => abilityAction = value; }
    public AbilityCategoryEnum AbilityCategory { get => abilityCategory; set => abilityCategory = value; }
    public GameObject PlayerReference { get => playerReference; set => playerReference = value; }
    public bool IsCommandPhaseMandatory { get => isCommandPhaseMandatory; set => isCommandPhaseMandatory = value; }
}
