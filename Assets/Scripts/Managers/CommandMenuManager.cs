using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class CommandMenuManager : MonoBehaviour
{
    public GameObject commandMenuModel;
    public GameObject buttonModel;
    private GameObject menuGameObject;
    private PlayersTurnManager playersTurnManager;
    public static bool isMenuDisplayEnabled = false;
    private bool isFirstDisplay = true;
    private List<GameObject> subMenuButtons;
    private List<GameObject> menuButtons;

    private void Start()
    {
        PlayersTurnManager = GameUtils.FetchPlayersTurnManager();
    }

    private void Update()
    {
        isMenuDisplayEnabled =  (MenuGameObject != null && !MenuGameObject.activeInHierarchy || isFirstDisplay)
            && PlayersTurnManager.IsCommandPhase && playersTurnManager.turnState != TurnStateEnum.STANDBY;
    }

    public void GenerateCommandMenu(PlayerAbilities playerAbilities)
    {

        GameObject playerGameObject = playerAbilities.gameObject;
        List<PlayerAbility> abilityList = playerAbilities.PlayerAbilityList;
        GameObject panelGameObject = null;

        if (MenuGameObject != null)
        {
            panelGameObject = GetMenuPanelGameObject();
            MenuGameObject.SetActive(true);
            Button[] menuButtons = panelGameObject.GetComponentsInChildren<Button>();
            foreach (Button button in menuButtons)
            {
                Destroy(button.gameObject);
            }
        }
        else
        {
            MenuGameObject = Instantiate(commandMenuModel, Vector3.zero, Quaternion.identity);
        }

        this.SetMenuPosition(playerGameObject);
        panelGameObject = GetMenuPanelGameObject();

        MenuButtons = new List<GameObject>();
        SubMenuButtons = new List<GameObject>();
        GameObject specialButtonGameObject = null;

        if (playerAbilities.HasSpecialAbilities)
        {
            specialButtonGameObject = Instantiate(buttonModel, panelGameObject.transform.position, panelGameObject.transform.rotation);
        }

        bool isSpecialButonInitialized = false;

        foreach (PlayerAbility ability in abilityList)
        {
            GameObject buttonGameObject = Instantiate(buttonModel, panelGameObject.transform.position, panelGameObject.transform.rotation);
            TextMeshProUGUI buttonText = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
            Button buttonComponent = buttonGameObject.GetComponent<Button>();
            PlayerAbility.AbilityDelegate playerActions;

            buttonText.text = ability.AbilityName;
            
            switch (ability.AbilityType)
            {
                case AbilityTypeEnum.BASIC:
                    playerActions = ability.AbilityAction;
                    playerActions += DisableMenu;
                    buttonComponent.onClick.AddListener(() => playerActions());
                    MenuButtons.Add(buttonComponent.gameObject);
                    break;
                case AbilityTypeEnum.SPECIAL:

                    if(ability.AbilityCategory == AbilityCategoryEnum.NORMAL)
                    {
                        if (!isSpecialButonInitialized)
                        {
                            TextMeshProUGUI specialButtonText = specialButtonGameObject.GetComponentInChildren<TextMeshProUGUI>();
                            Button specialButtonComponent = specialButtonGameObject.GetComponent<Button>();
                            specialButtonComponent.onClick.AddListener(() => DisplaySubMenu());
                            specialButtonText.text = "Special Ability";
                            specialButtonGameObject.transform.SetParent(panelGameObject.transform);
                            MenuButtons.Add(specialButtonComponent.gameObject);
                            isSpecialButonInitialized = true;
                        }

                        playerActions = ability.AbilityAction;
                        playerActions += DisableMenu;

                        buttonComponent.onClick.AddListener(() => playerActions());

                        buttonGameObject.SetActive(false);
                        SubMenuButtons.Add(buttonGameObject);
                    }
                    else if(ability.AbilityCategory == AbilityCategoryEnum.UI)
                    {
                        playerActions = HideSubMenu;
                        buttonComponent.onClick.AddListener(() => playerActions());
                        buttonComponent.gameObject.SetActive(false);
                        SubMenuButtons.Add(buttonComponent.gameObject);
                    }

                    break;
                default:
                    break;
            }

            buttonGameObject.transform.SetParent(panelGameObject.transform);

        }

        isFirstDisplay = false;
    }


    private void DisplaySubMenu()
    {
        foreach(GameObject menuButtonGameObject in MenuButtons)
        {
            menuButtonGameObject.SetActive(false);
        }

        foreach (GameObject subMenuButtonGameObject in SubMenuButtons)
        {
            subMenuButtonGameObject.SetActive(true);
        }
    }

    private void HideSubMenu()
    {
        foreach (GameObject subMenuButtonGameObject in SubMenuButtons)
        {
            subMenuButtonGameObject.SetActive(false);
        }

        foreach (GameObject menuButtonGameObject in MenuButtons)
        {
            menuButtonGameObject.SetActive(true);
        }
    }

    private void SetMenuPosition(GameObject playerGameObject)
    {
        RectTransform canvasRectransform = MenuGameObject.transform.GetComponentInChildren<RectTransform>();
        canvasRectransform.anchoredPosition = Camera.main.WorldToScreenPoint(playerGameObject.transform.position);
    }

    private GameObject GetMenuPanelGameObject()
    {
        GameObject canvasGameObject = MenuGameObject.transform.GetComponentInChildren<RectTransform>().gameObject;
        GameObject panelGameObject = canvasGameObject.GetComponentsInChildren<RectTransform>()
            .Where(rectTransform => rectTransform != canvasGameObject.transform)
            .First().gameObject;
        return panelGameObject;
    }

    public void DisableMenu()
    {
        GameObject menu = GameObject.FindGameObjectWithTag(TagsConstants.COMMAND_MENU_TAG);
        menu.SetActive(false);
        PlayersTurnManager.IsCommandPhase = false;
    }

    public GameObject MenuGameObject { get => menuGameObject; set => menuGameObject = value; }
    public PlayersTurnManager PlayersTurnManager { get => playersTurnManager; set => playersTurnManager = value; }
    public List<GameObject> SubMenuButtons { get => subMenuButtons; set => subMenuButtons = value; }
    public List<GameObject> MenuButtons { get => menuButtons; set => menuButtons = value; }
}
