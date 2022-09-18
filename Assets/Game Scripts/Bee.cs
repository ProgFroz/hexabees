using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using RandomGenerator.Scripts.ModernNameGenerators;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
using Random = System.Random;

[RequireComponent(typeof(HexUnit))]
public class Bee : MonoBehaviour {
    
    [SerializeField]
    private HexUnit hexUnit;
    
    public bool hasJob;

    public WorkingManager workingManager;
    public TimeManager timeManager;
    public UIManager uiManager;
    
    public HexGameUI hexGameUI;
    [SerializeField]
    private int maxFoodLevel;
    [SerializeField]
    private int currentFoodLevel;
    
    [SerializeField]
    private int maxWaterLevel;
    [SerializeField]
    private int currentWaterLevel;
    
    [SerializeField]
    private int ageHours;
    [SerializeField]
    private int ageDays;
    private int _hourBorn;
    
    [SerializeField]
    private int maxSemenLevel;
    [SerializeField]
    private int currentSemenLevel;
    
    [SerializeField]
    private Caste caste = Caste.None;
    [SerializeField]
    private Job job = Job.None;
    [SerializeField]
    private Metamorphosis metamorphosis = Metamorphosis.Egg;

    private JobOrder _assignedJob = null;

    public Dictionary<BeeAction, PriorityValue> priorities = new Dictionary<BeeAction, PriorityValue>();

    [SerializeField]
    private Dictionary<Item, int> _inventory = new Dictionary<Item, int>();

    public int pollen;

    float _time;
    private float _interval = 1f;
    public bool doWork = false;

    [SerializeField] private bool canWork = false;

    private GameObject activeModel;
    [SerializeField] private GameObject workerModel;
    [SerializeField] private GameObject queenModel;
    [SerializeField] private GameObject droneModel;
    [SerializeField] private GameObject eggModel;
    [SerializeField] private GameObject larvaModel;
    [SerializeField] private GameObject pupaModel;

    [SerializeField] private int lifespan;

    public Dictionary<BeeAction, PriorityValue> Priorities {
        get => priorities;
        set => priorities = value;
    }

    [SerializeField] private string displayName;
    [SerializeField] private Trait trait1;
    [SerializeField] private Trait trait2;
    [SerializeField] private Trait trait3;
    
    private SimplePersonNameGenerator _generator = new SimplePersonNameGenerator();

    private Dictionary<Trait, float> chancesByTrait = new Dictionary<Trait, float>();

    private List<PriorityValue> _priorityOrder = new List<PriorityValue>();

    void Start() {
        _time = 0f;
        GenerateStats();
        GenerateCaste();
        UpdateMetamorphosis(Metamorphosis.Egg);
        InitializePriorities();
        WorkingManager = hexUnit.Grid.workingManager;
        
        hexGameUI = hexUnit.Grid.hexGameUI;
        timeManager = WorkingManager.timeManager;
        uiManager = timeManager.uiManager;
        this._hourBorn = timeManager.GetCurrentHoursSinceBegin();
        InitializeInventory();
        GiveName();
        GenerateTraitChances();
        GenerateTraits();
        GenerateLifespan();
        CheckInventory();
        

        InvokeRepeating("DrainStats", 0.0f, 1f);
    }

    private void CheckInventory() {
        CheckIfInventoryFull();
    }

    private void GenerateCaste() {
        List<Caste> castes = new List<Caste>();
        castes.Add(Caste.Drone);
        castes.Add(Caste.Queen);
        castes.Add(Caste.Worker);

        List<Job> jobs = new List<Job>();
        jobs.Add(Job.Builder);
        jobs.Add(Job.Nurse);
        jobs.Add(Job.Gatherer);

            // Caste = castes[GenerateInt(0, castes.Count)];
        Caste = Caste.Worker;
        if (Caste == Caste.Worker) this.job = jobs[GenerateInt(0, jobs.Count)];
    }

    private void GenerateLifespan() {
        switch (Caste) {
            case Caste.Drone: this.lifespan = GenerateInt(14, 28);
                break;
            case Caste.Queen: this.lifespan = GenerateInt(93, 155);
                break;
            case Caste.Worker:
                if (this.timeManager.season == 2) {
                    this.lifespan = GenerateInt(50, 62);
                }
                else {
                    this.lifespan = GenerateInt(31, 51);
                }
                break;
            default: this.lifespan = 1;
                break;
        }
    }

    private void GenerateTraitChances() {
        chancesByTrait.Add(Trait.Immunity, 0.02f);
        chancesByTrait.Add(Trait.Shiny, 0.02f);
        
        chancesByTrait.Add(Trait.BreedNeg, 0.06f);
        chancesByTrait.Add(Trait.BreedPos, 0.06f);
        
        chancesByTrait.Add(Trait.CreateNeg, 0.06f);
        chancesByTrait.Add(Trait.CreatePos, 0.06f);
        
        chancesByTrait.Add(Trait.FeedNeg, 0.06f);
        chancesByTrait.Add(Trait.FeedPos, 0.06f);

        chancesByTrait.Add(Trait.GatherNeg, 0.06f);
        chancesByTrait.Add(Trait.GatherPos, 0.06f);
        
        chancesByTrait.Add(Trait.LifeNeg, 0.06f);
        chancesByTrait.Add(Trait.LifePos, 0.06f);
        
        chancesByTrait.Add(Trait.WaterNeg, 0.06f);
        chancesByTrait.Add(Trait.WaterPos, 0.06f);
        
        chancesByTrait.Add(Trait.WorkNeg, 0.06f);
        chancesByTrait.Add(Trait.WorkPos, 0.06f);
        
        chancesByTrait.Add(Trait.FoodNeg, 0.06f);
        chancesByTrait.Add(Trait.FoodPos, 0.06f);
    }

    void Update() {
        pollen = Inventory[Item.Pollen];
        ageHours = timeManager.GetCurrentHoursSinceExistence(_hourBorn);
        ageDays = TimeManager.ConvertHoursToDays(ageHours);
        this.hasJob = this._assignedJob != null;
        this.doWork = this._assignedJob != null && this._assignedJob.Cell == this.hexUnit.Location;
        
        
        _time += Time.deltaTime;
        while(_time >= _interval) {
            Work();
            _time -= _interval;
        }

        int daysLeft = GetDaysUntilNextMetamorphosis();
        if (daysLeft == 0) {
            this.UpdateMetamorphosis(GetUpcomingMetamorphosis(metamorphosis));
        }

        CheckMaxLifespan();
    }

    private void CheckMaxLifespan() {
        if (this.ageDays > lifespan) Die();
    }

    private void InitializeInventory() {
        Inventory.Add(Item.Honey, 0);
        Inventory.Add(Item.Nectar, 0);
        Inventory.Add(Item.Wax, 0);
        Inventory.Add(Item.Pollen, 0);
        Inventory.Add(Item.RoyalJelly, 0);
    }

    public bool CheckIfInventoryFull(Item item) {
        return Inventory[item] == GetMaximumItemsPerItem(item);
    }
    
    public void CheckIfInventoryFull() {
        // foreach (KeyValuePair<Item, int> kv in Inventory) {
        //     if (kv.Value == GetMaximumItemsPerItem(kv.Key)) {
        //         workingManager.AddStoreTask(this, kv.Key, kv.Value);
        //     }
        // }
    }

    private void InitializePriorities() {
        priorities.Add(BeeAction.Breed, (this.caste == Caste.Queen ? GetRecentPriority(BeeAction.Breed) : PriorityValue.Cant));
        priorities.Add(BeeAction.Destroy, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Destroy) : PriorityValue.Cant));
        priorities.Add(BeeAction.Evaporator, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Evaporator) : PriorityValue.Cant));
        priorities.Add(BeeAction.Fertilize, (this.caste == Caste.Drone ? GetRecentPriority(BeeAction.Fertilize) : PriorityValue.Cant));
        priorities.Add(BeeAction.Gather, (this.job == Job.Gatherer ? GetRecentPriority(BeeAction.Gather) : PriorityValue.Cant));
        priorities.Add(BeeAction.Lay, (this.caste == Caste.Queen ? GetRecentPriority(BeeAction.Lay) : PriorityValue.Cant));
        priorities.Add(BeeAction.Mixer, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Mixer) : PriorityValue.Cant));
        priorities.Add(BeeAction.Pollinate, (this.job == Job.Gatherer ? GetRecentPriority(BeeAction.Pollinate) : PriorityValue.Cant));
        priorities.Add(BeeAction.Refiner, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Refiner) : PriorityValue.Cant));
        priorities.Add(BeeAction.Royal, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Royal) : PriorityValue.Cant));
        priorities.Add(BeeAction.Storage, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Storage) : PriorityValue.Cant));
        priorities.Add(BeeAction.Cover, (this.job == Job.Nurse ? GetRecentPriority(BeeAction.Cover) : PriorityValue.Cant));
        priorities.Add(BeeAction.Evaporate, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Evaporate) : PriorityValue.Cant));
        priorities.Add(BeeAction.Refine, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Refine) : PriorityValue.Cant));
        priorities.Add(BeeAction.Mix, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Mix) : PriorityValue.Cant));
        priorities.Add(BeeAction.Feed, (this.job == Job.Nurse ? GetRecentPriority(BeeAction.Feed) : PriorityValue.Cant));
    }



    public void UpdateMetamorphosis(Metamorphosis metamorphosis) {
        this.metamorphosis = metamorphosis;
        
        this.UpdateModel(metamorphosis);
    }
    private PriorityValue GetRecentPriority(BeeAction action) {
        if (priorities.ContainsKey(action)) {
            PriorityValue value = priorities[action];
            if (value == PriorityValue.Cant) return PriorityValue.Medium;
            return value;
        }
        else {
            return PriorityValue.Medium;
        }
        
    }

    private void GenerateStats() {
        MaxFoodLevel = 100;
        MaxWaterLevel = 300;
        MaxSemenLevel = 100;
        CurrentFoodLevel = MaxFoodLevel;
        CurrentWaterLevel = MaxWaterLevel;
        CurrentSemenLevel = MaxSemenLevel;
    }

    public void AssignJob(JobOrder jobOrder) {
        _assignedJob = jobOrder;
    }

    public void UpdatePriorities(BeeAction action, PriorityValue value) {
        Priorities[action] = value;
    }
    
    public void DrainStats() {
        CurrentFoodLevel -= 1;
        CurrentWaterLevel -= 1;

        if (CurrentFoodLevel < 0 || CurrentWaterLevel < 0) {
            this.hexUnit.Die();
        }
    }

    public int MaxFoodLevel {
        get => maxFoodLevel;
        set => maxFoodLevel = value;
    }

    public int CurrentFoodLevel {
        get => currentFoodLevel;
        set => currentFoodLevel = value;
    }

    public int MaxWaterLevel {
        get => maxWaterLevel;
        set => maxWaterLevel = value;
    }

    public int CurrentWaterLevel {
        get => currentWaterLevel;
        set => currentWaterLevel = value;
    }

    public int Age {
        get => ageHours;
        set => ageHours = value;
    }

    public int MaxSemenLevel {
        get => maxSemenLevel;
        set => maxSemenLevel = value;
    }

    public int CurrentSemenLevel {
        get => currentSemenLevel;
        set => currentSemenLevel = value;
    }

    public Caste Caste {
        get => caste;
        set => caste = value;
    }

    public Job Job {
        get => job;
        set => job = value;
    }

    public WorkingManager WorkingManager {
        get => workingManager;
        set => workingManager = value;
    }

    public string PrioritiesToString() {
        string str = "";
        foreach (KeyValuePair<BeeAction, PriorityValue> kv in Priorities) {
            str += kv.Key + " - " + kv.Value;
            str += "\n";
        }

        return str;
    }

    public LTSeq Die() {
        LTSeq seq = LeanTween.sequence();
        if (this._assignedJob != null) {
            WorkingManager.RequeueJob(this._assignedJob);
        }

        Animator animator = activeModel.GetComponent<Animator>();
        if (animator) {
            animator.enabled = false;
            seq.append(LeanTween.rotateZ(activeModel, 30f, 0.1f).setEaseInOutCubic());
            seq.append(LeanTween.rotateZ(activeModel, -30f, 0.1f).setEaseInOutCubic());
            seq.append(LeanTween.moveY(activeModel, hexUnit ? hexUnit.Location.transform.position.y : (transform.position.y - 9f), 0.5f).setEaseInOutExpo());
            seq.append(LeanTween.scale(activeModel, Vector3.zero, 0.5f).setEaseInOutCubic());
        }
       
        uiManager.RefreshPriorityList();
        return seq;
    }

    public void Travel(HexCell destination) {
        this.hexGameUI.DoPathfinding(this.hexUnit, destination);
        this.hexGameUI.DoMove(this.hexUnit);
    }

    public JobOrder GetAssignedJob() {
        return this._assignedJob;
    }

    public void FinishJob() {
        this.WorkingManager.FinishJob(this, this._assignedJob);
    }
    
    private void Work() {
        if (_assignedJob != null && doWork) {
            _assignedJob.Progress += (100 / _assignedJob.GetRequiredHours());
            if (_assignedJob.Progress >= 100) {
                FinishJob();
            }
        }
        
    }

    public void UpdateModel(Metamorphosis metamorphosis) {
        if (this.activeModel == null) this.activeModel = eggModel;

        GameObject prevModel = this.activeModel;

        switch (metamorphosis) {
            case Metamorphosis.Adult: 
                switch (Caste) {
                    case Caste.Drone:
                        activeModel = droneModel;
                        break;
                    case Caste.Queen:
                        activeModel = queenModel;
                        break;
                    case Caste.Worker:
                        activeModel = workerModel;
                        break;
                    default:
                        break;
                }
                break;
            case Metamorphosis.Egg:
                activeModel = eggModel;
                break;
            case Metamorphosis.Larva:
                activeModel = larvaModel;
                break;
            case Metamorphosis.Pupa:
                activeModel = pupaModel;
                break;
        }

        if (activeModel != prevModel) {
            LeanTween.scale(gameObject, Vector3.zero, 1f).setOnComplete(() => {
                prevModel.SetActive(false);

                activeModel.SetActive(true);

                LeanTween.scale(gameObject, Vector3.one, 1f).setOnComplete(() => {
                    if (metamorphosis == Metamorphosis.Adult) {
                        canWork = true;
                        uiManager.RefreshPriorityList();
                    }
                }).setEaseInOutCubic();
            }).setEaseInOutCubic();
        }
        
        
    }

    public static int GetRequiredDaysForMetamorphosis(Caste caste, Metamorphosis metamorphosis) {
        switch (metamorphosis) {
            case Metamorphosis.Egg: return 0;
            case Metamorphosis.Larva:
                switch (caste) {
                    case Caste.Drone:
                    case Caste.Queen:
                    case Caste.Worker:
                    case Caste.None:
                    default: 
                        return 3;
                }
            case Metamorphosis.Pupa:
                switch (caste) {
                    case Caste.Drone: return 9;
                    case Caste.Worker: return 7;
                    case Caste.Queen: return 5;
                    default: return 3;
                }
            case Metamorphosis.Adult:
                switch (caste) {
                    case Caste.Drone: return 12;
                    case Caste.Queen: return 8;
                    case Caste.Worker: return 10;
                    default: return 3;
                }
            default: return -1;
        }
    }

    public static int GetTotalDaysForMetamorphosis(Caste caste, Metamorphosis metamorphosis) {
        switch (metamorphosis) {
            case Metamorphosis.Egg: return 0;
            case Metamorphosis.Larva:
                switch (caste) {
                    case Caste.Drone:
                    case Caste.Queen:
                    case Caste.Worker:
                    case Caste.None:
                    default: 
                        return 1;
                }
            case Metamorphosis.Pupa:
                switch (caste) {
                    case Caste.Drone: return 2;
                    case Caste.Worker: return 2;
                    case Caste.Queen: return 2;
                    default: return 3;
                }
            case Metamorphosis.Adult:
                switch (caste) {
                    case Caste.Drone: return 3;
                    case Caste.Queen: return 3;
                    case Caste.Worker: return 3;
                    default: return 3;
                }
            default: return -1;
        }
    }

    public static Metamorphosis GetUpcomingMetamorphosis(Metamorphosis metamorphosis) {
        switch (metamorphosis) {
            case Metamorphosis.Egg: return Metamorphosis.Larva;
            case Metamorphosis.Larva: return Metamorphosis.Pupa;
            case Metamorphosis.Pupa: return Metamorphosis.Adult;
            default: return Metamorphosis.Egg;
        }
    }

    public int GetDaysUntilNextMetamorphosis() {
        if (GetUpcomingMetamorphosis(metamorphosis) == Metamorphosis.Egg) return -1;
        return GetTotalDaysForMetamorphosis(caste, GetUpcomingMetamorphosis(metamorphosis)) - ageDays;
    }

    public bool CanWork() {
        return this.canWork;
    }

    public static int GetRequiredPollenToPollinate() {
        return 30;
    }
    public bool HasEnoughPollen() {
        return Inventory[Item.Pollen] >= GetRequiredPollenToPollinate();
    }

    public Metamorphosis Metamorphosis {
        get => metamorphosis;
        set => metamorphosis = value;
    }

    public Dictionary<Item, int> Inventory {
        get => _inventory;
        set => _inventory = value;
    }

    public static int GetMaximumItemsPerItem(Item item) {
        switch (item) {
            case Item.Honey: return 10;
            case Item.Wax: return 10;
            case Item.Nectar: return 10;
            case Item.Pollen: return 30;
            case Item.RoyalJelly: return 30;
            default: return -1;
        }
    }

    public int AddItemsToInventory(Item item, int amount) {
        int max = GetMaximumItemsPerItem(item);
        int current = Inventory[item];
        int leftSpace = max - current;
        int leftover = amount - leftSpace;
        leftover = leftover < 0 ? 0 : leftover;

        Inventory[item] += ((current + amount) > max) ? (amount - leftover) : amount;
        return leftover;
    }

    public int RemoveItemsFromInventory(Item item, int desired) {
        int current = Inventory[item];
        int newAmount = (current - desired < 0) ? 0 : current - desired;
        Inventory[item] = newAmount;
        return current - newAmount;
    }

    public int GetSpaceForItem(Item item) {
        return GetMaximumItemsPerItem(item) - Inventory[item];
    }

    public void GiveName() {
        Random random = new System.Random();
        string n = _generator.Generate(random);
        DisplayName = ToCapsAllWords(n);
    }

    public static string ToCapsAllWords(string name)
    {
        TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
        return textInfo.ToTitleCase(name);
    }
    
    public string DisplayName {
        get => displayName;
        set => displayName = value;
    }

    public Trait Trait1 {
        get => trait1;
        set => trait1 = value;
    }

    public Trait Trait2 {
        get => trait2;
        set => trait2 = value;
    }

    public Trait Trait3 {
        get => trait3;
        set => trait3 = value;
    }

    private void OnMouseDown() {
        timeManager.uiManager.UpdateSelectedGameObject(gameObject);
    }

    public string GetCasteAsString() {
        switch (Caste) {
            case Caste.Drone: return "Drone";
            case Caste.Queen: return "Queen";
            case Caste.Worker: return "Worker";
            default: return "None";
        }
    }

    public string GetAgeAsString() {
        return ageDays + " Days";
    }

    public string GetJobAsString() {
        switch (Job) {
            case Job.Builder: return "Builder";
            case Job.Nurse: return "Nurse";
            case Job.Gatherer: return "Collector";
            default: return "None";
        }
    }

    public string GetTaskAsString() {
        if (GetAssignedJob() == null) return "None";
        switch (GetAssignedJob().Action) {
            case BeeAction.Gather: return "Gathering..";
            case BeeAction.Pollinate: return "Pollinating..";
            default: return "Doing Something!";
        }
    }

    public float GetTraitChance(Trait trait) {
        return chancesByTrait[trait];
    }

    private void GenerateTraits() {
        List<Trait> traitPool = chancesByTrait.Keys.ToList();
        
        trait1 = traitPool[GenerateInt(0, traitPool.Count)];
        traitPool.Remove(trait1);
        
        trait2 = traitPool[GenerateInt(0, traitPool.Count)];
        traitPool.Remove(trait1);
        
        trait3 = traitPool[GenerateInt(0, traitPool.Count)];
        traitPool.Remove(trait1);
    }

    public static bool GenerateBool(float chance) {
        if (chance > 1) return true;
        return GenerateInt(0, 100) < (chance * 100);
    }
    
    public static float GenerateFloat(int min, int max)
    {
        return UnityEngine.Random.Range(min, max);
    }
    
    public static int GenerateInt(int min, int max)
    {
        return (int)GenerateFloat(min, max);
    }

}

public enum Caste {
    None,
    Queen,
    Drone,
    Worker
}
public enum Job {
    None,
    Nurse,
    Builder,
    Gatherer
}
public enum Metamorphosis {
    Egg,
    Larva,
    Pupa,
    Adult
}

public enum Item {
    Nectar,
    Honey,
    Pollen,
    Wax,
    RoyalJelly,
    None
}

public enum Trait {
    WorkPos,
    WorkNeg,
    
    GatherPos,
    GatherNeg,
    
    CreatePos,
    CreateNeg,
    
    FeedPos,
    FeedNeg,
    
    BreedPos,
    BreedNeg,
    
    LifePos,
    LifeNeg,
    
    WaterPos,
    WaterNeg,
    
    FoodPos,
    FoodNeg,
    
    Immunity,
    Shiny
}

