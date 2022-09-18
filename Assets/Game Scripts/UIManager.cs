using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {
    public TimeManager timeManager;
    public WorkingManager workingManager;
    public BeeList beeList;
    public ResourcesList resourceList;
    public PriorityList priorityList;
    public AlertWindow alertWindow;

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

    public Texture workTraitTexture;
    public Texture createTraitTexture;
    public Texture gatherTraitTexture;
    public Texture feedTraitTexture;
    public Texture breedTraitTexture;
    public Texture lifeTraitTexture;
    public Texture waterTraitTexture;
    public Texture foodTraitTexture;
    public Texture immunityTraitTexture;
    public Texture shinyTraitTexture;

    public Color positiveColor;
    public Color negativeColor;
    public Color neutralColor;

    [SerializeField] private BeeWindow beeWindow;

    [SerializeField] private GameObject selectedGameObject;
    [SerializeField] private Bee selectedBee;
    
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
            else {
                if (cell && cell.Unit == null) {
                    UpdateSelectedGameObject(cell.gameObject);
                }

                if (cell == null) {
                    UpdateSelectedGameObject(null);
                }
            }

            // if (currentAction == BeeAction.None) {
            //     GameObject gameObject = EventSystem.current.currentSelectedGameObject;
            //     if (gameObject) {
            //         Bee bee = gameObject.GetComponent<Bee>();
            //         if (bee) {
            //             Debug.Log(bee.DisplayName);
            //         }
            //     }
            // }
            
        }

        if (Input.GetKeyDown(KeyCode.Alpha9)) {
            alertWindow.UpdateAlert(AlertType.Food);
        }
    }

    public void RefreshPriorityList() {
        HexGrid g1 = workingManager.overworldGrid;
        HexGrid g2 = workingManager.hiveGrid;
        List<Bee> bees = g1.GetBees(Metamorphosis.Adult);
        bees.AddRange(g2.GetBees(Metamorphosis.Adult));
        List<Bee> final = bees.OrderBy(x => x.DisplayName).ToList();
        Debug.Log(final.Count);
        this.priorityList.UpdateList(final);
    }
    public void UpdateSelectedGameObject(GameObject gameObject) {
        this.selectedGameObject = gameObject;
        if (gameObject) {
            Bee bee = gameObject.GetComponent<Bee>();
            this.UpdateSelectedBee(bee ? bee : null);
        }
    }
    private void UpdateSelectedBee(Bee bee) {
        this.selectedBee = bee;
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
        
        resourceList.UpdateInterface(workingManager.storageManager);
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
    
    public Texture GetAccordingTexture(Trait trait) {
        switch (trait) {
            case Trait.WorkPos:
            case Trait.WorkNeg:
                return workTraitTexture;
            case Trait.GatherPos:
            case Trait.GatherNeg:
                return gatherTraitTexture;
            case Trait.CreatePos:
            case Trait.CreateNeg:
                return createTraitTexture;
            case Trait.FeedPos:
            case Trait.FeedNeg:
                return feedTraitTexture;
            case Trait.BreedNeg:
            case Trait.BreedPos:
                return breedTraitTexture;
            case Trait.LifePos:
            case Trait.LifeNeg:
                return lifeTraitTexture;
            case Trait.WaterPos:
            case Trait.WaterNeg:
                return waterTraitTexture;
            case Trait.FoodNeg:
            case Trait.FoodPos:
                return foodTraitTexture;
            case Trait.Immunity:
                return immunityTraitTexture;
            case Trait.Shiny:
                return shinyTraitTexture;
            
            default: return gatherTexture;
        }
    }
    
    public Color GetAccordingColorPositiveNegative(Trait trait) {
        switch (trait) {
            case Trait.WorkPos: 
            case Trait.GatherPos: 
            case Trait.CreatePos: 
            case Trait.FeedPos: 
            case Trait.BreedPos: 
            case Trait.LifePos: 
            case Trait.WaterPos: 
            case Trait.FoodPos:
                return positiveColor;
            case Trait.WorkNeg:
            case Trait.GatherNeg:
            case Trait.CreateNeg:
            case Trait.FeedNeg:
            case Trait.BreedNeg:
            case Trait.LifeNeg:
            case Trait.WaterNeg:
            case Trait.FoodNeg:
                return negativeColor;
            default:
                return neutralColor;
        }
    }

    public BeeWindow BeeWindow {
        get => beeWindow;
        set => beeWindow = value;
    }

    public Bee SelectedBee {
        get => selectedBee;
        set => selectedBee = value;
    }

    public Color GetBlankTexturesColor() {
        return blankTexturesColor;
    }

    public GameObject SelectedGameObject {
        get => selectedGameObject;
        set => selectedGameObject = value;
    }
}
