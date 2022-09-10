using System;
using System.Collections.Generic;
using System.Data;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public TimeManager timeManager;
    public WorkingManager workingManager;
    public BeeList beeList;

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
    
    public Texture gatherTexture;
    public Texture pollinateTexture;
    public Texture fertilizeTexture;
    public Texture layTexture;
    public Texture cancelTexture;
    public Texture destroyTexture;
    
    public Texture evaporatorTexture;
    public Texture mixerTexture;
    public Texture refinerTexture;
    public Texture storageTexture;
    public Texture breedTexture;
    public Texture royalTexture;
    
    public Texture feedTexture;
    public Texture coverTexture;
    
    public Texture eatTexture;
    public Texture drinkTexture;
    
    public Texture mixTexture;
    public Texture refineTexture;
    public Texture evaporateTexture;

    public Color blankTexturesColor;
    
    private void Start() {
        LeanTween.init(99999);
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

    HexCell prevCell;
    HexCell cell;
    private void Update() {
        
        if (currentHexMapCamera.type == HexMapType.Hive) {
            cell = hiveHexMapCamera.grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        }
        else {
            cell = overworldHexMapCamera.grid.GetCell(Camera.main.ScreenPointToRay(Input.mousePosition));
        }

        if (cell) {
            cell.isHovered = !EventSystem.current.IsPointerOverGameObject();
            
            if (prevCell != null && prevCell != cell ) {
                prevCell.isHovered = false;
            }
            prevCell = cell;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (currentAction != BeeAction.None && cell) {
                if (currentAction != BeeAction.Cancel) {
                    if (cell.CanPerformJob(currentAction)) {
                        workingManager.AddJobOrder(currentAction, cell);
                    }
                }
                else { 
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

        HexGrid g1 = workingManager.overworldGrid;
        HexGrid g2 = workingManager.hiveGrid;
        int amountWorkers = g1.GetBees(Caste.Worker).Count + g2.GetBees(Caste.Worker).Count;
        int amountAdultWorkers = g1.GetBees(Caste.Worker, Metamorphosis.Adult).Count + g2.GetBees(Caste.Worker, Metamorphosis.Adult).Count;
        
        int amountQueens = g1.GetBees(Caste.Queen).Count + g2.GetBees(Caste.Queen).Count;
        int amountAdultQueens = g1.GetBees(Caste.Queen, Metamorphosis.Adult).Count + g2.GetBees(Caste.Queen, Metamorphosis.Adult).Count;
        
        int amountDrones = g1.GetBees(Caste.Drone).Count + g2.GetBees(Caste.Drone).Count;
        int amountAdultDrones = g1.GetBees(Caste.Drone, Metamorphosis.Adult).Count + g2.GetBees(Caste.Drone, Metamorphosis.Adult).Count;
        
        beeList.UpdateAmount(amountAdultWorkers, CasteAmountType.Worker);
        beeList.UpdateAmount(amountWorkers - amountAdultWorkers, CasteAmountType.WorkerBreed);
        beeList.UpdateAmount(amountAdultQueens, CasteAmountType.Queen);
        beeList.UpdateAmount(amountQueens - amountAdultQueens, CasteAmountType.QueenBreed);
        beeList.UpdateAmount(amountAdultDrones, CasteAmountType.Drone);
        beeList.UpdateAmount(amountDrones - amountAdultDrones, CasteAmountType.DroneBreed);
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

    public Texture GetAccordingTexture(BeeAction action) {
        Texture texture;
        switch (action) {
            case BeeAction.Gather: texture = gatherTexture; break;
            case BeeAction.Pollinate: texture = pollinateTexture; break;
            case BeeAction.Fertilize: texture = fertilizeTexture; break;
            case BeeAction.Lay: texture = layTexture; break;
            case BeeAction.Cancel: texture = cancelTexture; break;
            case BeeAction.Destroy: texture = destroyTexture; break;
            case BeeAction.Evaporator: texture = evaporatorTexture; break;
            case BeeAction.Mixer: texture = mixerTexture; break;
            case BeeAction.Refiner: texture = refinerTexture; break;
            case BeeAction.Storage: texture = storageTexture; break;
            case BeeAction.Breed: texture = breedTexture; break;
            case BeeAction.Royal: texture = royalTexture; break;
            case BeeAction.Feed: texture = feedTexture; break;
            case BeeAction.Cover: texture = coverTexture; break;
            case BeeAction.Eat: texture = eatTexture; break;
            case BeeAction.Drink: texture = drinkTexture; break;
            case BeeAction.Mix: texture = mixTexture; break;
            case BeeAction.Refine: texture = refineTexture; break;
            case BeeAction.Evaporate: texture = evaporateTexture; break;
            default: texture = gatherTexture;
                break;
        }

        return texture;
    }

    public Color GetBlankTexturesColor() {
        return blankTexturesColor;
    }
}
