using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetSelectionManager : MonoBehaviour
{
    private GameObject highLightUi;
    private GameObject actionPlayerhighLightUi;
    private bool isActivated;
    private List<GameObject> targetList;
    private int currentTargetSelectionIndex;
    private CameraController cameraController;
    private Action<GameObject, GameObject> actionSelection;
    private GameObject actionUser;
    private readonly int DEFAULT_SELECTION_INDEX = 0;
    private PathFindingManager isoPathFinder;
    public GameObject fieldHighligthModel;
    private List<GameObject> activePathHighlights;

    // Start is called before the first frame update
    void Start()
    {
        isoPathFinder = GameUtils.FetchPathFindingManager();
        CameraController = CameraUtils.FetchCameraController();

        ActionPlayerhighLightUi = Instantiate(fieldHighligthModel, this.transform.position, this.transform.rotation);
        ActionPlayerhighLightUi.GetComponent<SpriteRenderer>().color = Color.blue;
        ActionPlayerhighLightUi.SetActive(false);

        HighLightUi = Instantiate(fieldHighligthModel, this.transform.position, this.transform.rotation);
        HighLightUi.GetComponent<SpriteRenderer>().color = Color.red;
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

    private void DisplayHighlightPath(Vector2Int start, Vector2Int end)
    {
        List<Vector3Int> mapPath = isoPathFinder.GetCalculatedMapPath(start, end);
        List<Vector3> tilePositions = new List<Vector3>();
        this.activePathHighlights = new List<GameObject>();
        if (mapPath != null)
        {
            foreach (Vector3Int pos in mapPath)
            {
                tilePositions.Add(FieldUtils.GetTileCenterPositionInGameWorld(new Vector2Int(pos.x, pos.y)));
            }
            for (int i = 0; i < tilePositions.Count; i++)
            {
                this.activePathHighlights.Add(Instantiate(fieldHighligthModel));
                this.activePathHighlights[i].SetActive(true);
                this.activePathHighlights[i].GetComponent<SpriteRenderer>().color = Color.magenta;
                //activeHighlights[i].GetComponent<SpriteRenderer>().color = Color.Lerp(startColour, endColour, (float)i / (float)tilePositions.Count);
                this.activePathHighlights[i].transform.position = tilePositions[i];
            }
        }
    }

    private void RemoveHighlightPath()
    {
        if(this.activePathHighlights != null)
        {
            for (int i = 0; i < this.activePathHighlights.Count; i++)
            {
                Destroy(this.activePathHighlights[i]);
            }

            this.activePathHighlights.Clear();
        }
    }

    public void EnableSelection(Vector3 initialPosition, List<GameObject> targetSelectionList, Action<GameObject, GameObject> actionForSelection = null, GameObject actionUser = null)
    {
        //this.RemoveHighlightPath();
        //this.DisplayHighlightPath(FieldUtils.GetGameObjectTilePositionOnField(actionUser), FieldUtils.GetGameObjectTilePositionOnField(targetSelectionList[currentTargetSelectionIndex]));

        ActionPlayerhighLightUi.transform.position = actionUser.transform.position;
        HighLightUi.transform.position = initialPosition;
        TargetList = targetSelectionList;
        ActionPlayerhighLightUi.SetActive(true);
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
        ActionPlayerhighLightUi.SetActive(false);
        HighLightUi.SetActive(false);
        IsActivated = false;
        currentTargetSelectionIndex = DEFAULT_SELECTION_INDEX;
    }

    private void SelectTarget(int nextSelectionIndex)
    {
        //this.RemoveHighlightPath();
        //this.DisplayHighlightPath(FieldUtils.GetGameObjectTilePositionOnField(this.actionUser), FieldUtils.GetGameObjectTilePositionOnField(TargetList[nextSelectionIndex]));
        GameObject targetSelection = TargetList[nextSelectionIndex];
        ActionPlayerhighLightUi.transform.position = this.actionUser.transform.position;
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
    public GameObject ActionPlayerhighLightUi { get => actionPlayerhighLightUi; set => actionPlayerhighLightUi = value; }
}
