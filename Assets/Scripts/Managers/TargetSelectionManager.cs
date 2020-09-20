using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionManager : MonoBehaviour
{

    public GameObject highLightUiModel = null;
    private GameObject highLightUi;
    private bool isActivated;
    private List<GameObject> targetList;
    private int currentTargetSelectionIndex;
    private CameraController cameraController;
    private Action<GameObject, GameObject> actionSelection;
    private GameObject actionUser;
    private readonly int DEFAULT_SELECTION_INDEX = 0;

    // Start is called before the first frame update
    void Start()
    {
        CameraController = CameraUtils.FetchCameraController();
        HighLightUi = Instantiate(highLightUiModel, this.transform.position, this.transform.rotation);
        HighLightUi.SetActive(false);
        currentTargetSelectionIndex = DEFAULT_SELECTION_INDEX;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActivated)
        {
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {

                int nextSelectionIndex = currentTargetSelectionIndex - 1;

                if (this.HasReachedBeginingOfTargetList(nextSelectionIndex))
                {
                    nextSelectionIndex = TargetList.Count - 1;
                }

                this.SelectTarget(nextSelectionIndex);
            }
            else if (Input.GetKeyUp(KeyCode.RightArrow))
            {

                int nextSelectionIndex = currentTargetSelectionIndex + 1;

                if (this.HasReachedEndOfTargetList(nextSelectionIndex))
                {
                    nextSelectionIndex = DEFAULT_SELECTION_INDEX;
                }

                this.SelectTarget(nextSelectionIndex);
            }
            else if (Input.GetKeyUp(KeyCode.Return))
            {
                GameObject targetSelection = TargetList[currentTargetSelectionIndex];
                this.ExecuteSelectionAction(this.actionUser, targetSelection);
                this.DisableSelection();
                PlayersTurnManager.IsCommandPhase = false;
            }
            else if (Input.GetKeyUp(KeyCode.Escape))
            {
                this.DisableSelection();
            }
        }
    }

    public void EnableSelection(Vector3 initialPosition, List<GameObject> targetSelectionList, Action<GameObject, GameObject> actionForSelection = null, GameObject actionUser = null)
    {
        HighLightUi.transform.position = initialPosition;
        TargetList = targetSelectionList;
        HighLightUi.SetActive(true);
        GameObject targetSelection = TargetList[currentTargetSelectionIndex];
        CameraController.FocusOnPlayer(targetSelection.transform);
        IsActivated = true;

        if(actionForSelection != null)
        {
            this.actionSelection = actionForSelection;
        }

        if(actionUser != null)
        {
            this.actionUser = actionUser;
        }
    }

    public void DisableSelection()
    {
        HighLightUi.SetActive(false);
        IsActivated = false;
        currentTargetSelectionIndex = DEFAULT_SELECTION_INDEX;
    }

    private void SelectTarget(int nextSelectionIndex)
    {
        GameObject targetSelection = TargetList[nextSelectionIndex];

        HighLightUi.transform.position = targetSelection.transform.position;
        CameraController.FocusOnPlayer(targetSelection.transform);
        currentTargetSelectionIndex = nextSelectionIndex;
    }

    private void ExecuteSelectionAction(GameObject user, GameObject target)
    {
        if(this.actionSelection != null && user != null)
        {
            this.actionSelection(user, target);
        }
    }

    private bool HasReachedEndOfTargetList(int nextSelectionIndex)
    {
        return nextSelectionIndex >= TargetList.Count;
    }

    private bool HasReachedBeginingOfTargetList(int nextSelectionIndex)
    {
        return nextSelectionIndex < DEFAULT_SELECTION_INDEX;
    }

    public GameObject HighLightUi { get => highLightUi; set => highLightUi = value; }
    public bool IsActivated { get => isActivated; set => isActivated = value; }
    public List<GameObject> TargetList { get => targetList; set => targetList = value; }
    public CameraController CameraController { get => cameraController; set => cameraController = value; }
}
