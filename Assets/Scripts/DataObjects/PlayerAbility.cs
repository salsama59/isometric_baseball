public class PlayerAbility
{
    private string abilityName;
    private AbilityTypeEnum abilityType;
    public delegate void AbilityDelegate();
    private AbilityDelegate abilityAction;
    private AbilityCategoryEnum abilityCategory;

    public PlayerAbility(string abilityName, AbilityTypeEnum abilityType, AbilityCategoryEnum abilityCategory, AbilityDelegate abilityAction)
    {
        AbilityName = abilityName;
        AbilityType = abilityType;
        AbilityAction = abilityAction;
        AbilityCategory = abilityCategory;
    }

    public string AbilityName { get => abilityName; set => abilityName = value; }
    public AbilityTypeEnum AbilityType { get => abilityType; set => abilityType = value; }
    public AbilityDelegate AbilityAction { get => abilityAction; set => abilityAction = value; }
    public AbilityCategoryEnum AbilityCategory { get => abilityCategory; set => abilityCategory = value; }
}
