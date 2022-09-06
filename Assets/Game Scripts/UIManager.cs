using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public TimeManager timeManager;
    public WorkingManager workingManager;

    public Color pauseButtonInactive;
    public Color pauseButtonActive;

    public Color timeButtonInactive;
    public Color timeButtonActive;

    public Button pauseButton;
    public Button onceButton;
    public Button twiceButton;

    public TextMeshProUGUI dateTimeText;

    public List<Button> allActionButtons;
    public ActionButton selectedActionButton;
    public BeeAction currentAction = BeeAction.None;

    public ChangeMapButton changeMapButton;
    private HexMapCamera currentHexMapCamera;
    public HexMapCamera overworldHexMapCamera;
    public HexMapCamera hiveHexMapCamera;

    public HexCell clickedCell;
    private void Start() {
        this.currentHexMapCamera = overworldHexMapCamera.gameObject.activeSelf ? overworldHexMapCamera : hiveHexMapCamera;
        SetPauseButtonInactive();
        pauseButton.onClick.AddListener(() => timeManager.TogglePause());
        onceButton.onClick.AddListener(() => timeManager.SetNormalSpeed());
        twiceButton.onClick.AddListener(() => timeManager.SetHigherSpeed());
        changeMapButton.GetComponent<Button>().onClick.AddListener( () => SwitchWorlds(currentHexMapCamera.type));
        
        foreach (Button actionButton in allActionButtons)
        {
            ActionButton beeActionButton = actionButton.GetComponent<ActionButton>();
            actionButton.onClick.AddListener(() => ButtonSelected(beeActionButton));
            actionButton.transform.localScale = Vector3.zero;
            
            Outline outline = actionButton.GetComponent<Outline>();
            outline.useGraphicAlpha = false;
            outline.enabled = false;
        }
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            HexCell cell;
            if (currentHexMapCamera.type == HexMapType.Hive) {
                cell = hiveHexMapCamera.grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
            }
            else {
                cell = overworldHexMapCamera.grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
            }

            if (currentAction != BeeAction.None && cell) {
                if (currentAction != BeeAction.Cancel) {
                    workingManager.AddJobOrder(currentAction, cell);
                }
                else {
                    Debug.Log("Cancel1");
                    workingManager.CancelJob(cell);
                }
            }
            
        }
    }

    private void SwitchWorlds(HexMapType type) {
        SwitchCameras();
        SwitchButtonImages(type);
    }

    private void SwitchButtonImages(HexMapType type) {
        this.changeMapButton.SwitchImages(type);
    }

    private void SwitchCameras() {
        if (currentHexMapCamera == null) throw new ArgumentNullException();
        HexMapCamera next = this.currentHexMapCamera.type == HexMapType.Hive ? overworldHexMapCamera : hiveHexMapCamera;
        currentHexMapCamera.gameObject.SetActive(false);
        next.gameObject.SetActive(true);
        this.currentHexMapCamera = next;
    }

    public void ButtonSelected(ActionButton beeActionButton)
    {
        if (this.selectedActionButton != null)
        {
            UnityEngine.UI.Outline prevOutline = selectedActionButton.GetComponent<UnityEngine.UI.Outline>();
            prevOutline.enabled = false;
        }
        if (this.selectedActionButton != beeActionButton)
        {
            UnityEngine.UI.Outline outline = beeActionButton.GetComponent<UnityEngine.UI.Outline>();
            outline.enabled = true;
            this.selectedActionButton = beeActionButton;
            currentAction = beeActionButton.beeAction;
        }
        else
        {
            this.selectedActionButton = null;
            currentAction = BeeAction.None;
        }
        
    }

    public void MenuClosed() {
        if (selectedActionButton != null) {
            UnityEngine.UI.Outline prevOutline = selectedActionButton.GetComponent<UnityEngine.UI.Outline>();
            prevOutline.enabled = false;
            this.selectedActionButton = null;
            currentAction = BeeAction.None;
        }
    }

    public void SetPauseButtonActive() {
        pauseButton.GetComponent<RawImage>().color = pauseButtonActive;
    }

    public void SetPauseButtonInactive() {
        pauseButton.GetComponent<RawImage>().color = pauseButtonInactive;
    }

    public void SetNormalSpeedButtonActive() {
        this.onceButton.GetComponent<RawImage>().color = this.timeButtonActive;
        this.twiceButton.GetComponent<RawImage>().color = this.timeButtonInactive;
    }

    public void SetDoubleSpeedButtonActive() {
        this.onceButton.GetComponent<RawImage>().color = this.timeButtonInactive;
        this.twiceButton.GetComponent<RawImage>().color = this.timeButtonActive;
    }

    public void UpdateDateTimeUI(int hour, int day, int season, int year) {
        dateTimeText.text = "Year " + (year + 1) + ", Day " + (day + 1) + " of " + ConvertSeasonIntToString(season) + ", " + hour + "h ";
    }

    private string ConvertSeasonIntToString(int season) {
        switch (season) {
            case 0: return "Spring";
            case 1: return "Summer";
            case 2: return "Fall";
            case 3: return "Winter";
            default: return "NA";
        }
    }

    public TimeManager TimeManager {
        get => timeManager;
        set => timeManager = value;
    }
}
