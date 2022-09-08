using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.TestTools;

[RequireComponent(typeof(HexUnit))]
public class Bee : MonoBehaviour {
    
    [SerializeField]
    private HexUnit hexUnit;
    
    public bool hasJob;

    public WorkingManager workingManager;

    public TimeManager timeManager;
    
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
    
    float _time;
    private float _interval = 1f;
    public bool doWork = false;

    [SerializeField] private bool canWork = false;

    private GameObject activeModel;
    [SerializeField] private GameObject workerModel;
    [SerializeField] private GameObject queenModel;
    [SerializeField] private GameObject droneModel;
    [SerializeField] private GameObject eggModel;

    public Dictionary<BeeAction, PriorityValue> Priorities {
        get => priorities;
        set => priorities = value;
    }

    void Start() {
        _time = 0f;
        
        GenerateStats();
        caste = Caste.Worker;
        job = Job.Collector;
        UpdateMetamorphosis(Metamorphosis.Egg);
        UpdatePriorities();
        WorkingManager = hexUnit.Grid.workingManager;
        hexGameUI = hexUnit.Grid.hexGameUI;
        timeManager = WorkingManager.timeManager;
        this._hourBorn = timeManager.GetCurrentHoursSinceBegin();
        InvokeRepeating("DrainStats", 0.0f, 1f);
    }

    private void UpdatePriorities() {
        priorities.Add(BeeAction.Breed, (this.caste == Caste.Queen ? GetRecentPriority(BeeAction.Breed) : PriorityValue.Cant));
        priorities.Add(BeeAction.Destroy, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Destroy) : PriorityValue.Cant));
        priorities.Add(BeeAction.Evaporator, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Evaporator) : PriorityValue.Cant));
        priorities.Add(BeeAction.Fertilize, (this.caste == Caste.Drone ? GetRecentPriority(BeeAction.Fertilize) : PriorityValue.Cant));
        priorities.Add(BeeAction.Gather, (this.job == Job.Collector ? GetRecentPriority(BeeAction.Gather) : PriorityValue.Cant));
        priorities.Add(BeeAction.Lay, (this.caste == Caste.Queen ? GetRecentPriority(BeeAction.Lay) : PriorityValue.Cant));
        priorities.Add(BeeAction.Mixer, (this.job == Job.Builder ? GetRecentPriority(BeeAction.Mixer) : PriorityValue.Cant));
        priorities.Add(BeeAction.Pollinate, (this.job == Job.Collector ? GetRecentPriority(BeeAction.Pollinate) : PriorityValue.Cant));
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
        CurrentFoodLevel = MaxFoodLevel;
        CurrentWaterLevel = MaxWaterLevel;
    }

    public void AssignJob(JobOrder jobOrder) {
        _assignedJob = jobOrder;
    }

    // Update is called once per frame
    void Update() {
        ageHours = timeManager.GetCurrentHoursSinceBegin() - this._hourBorn;
        ageDays = (int) Math.Floor((decimal) (ageHours / 24));
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
    }

    public void UpdatePriorities(BeeAction action, PriorityValue value) {
        
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

    public void Die() {
        if (this._assignedJob != null) {
            WorkingManager.RequeueJob(this._assignedJob);
        }
    }

    public void Travel(HexCell destination) {
        this.hexGameUI.DoPathfinding(this.hexUnit, destination);
        this.hexGameUI.DoMove(this.hexUnit);
    }

    public JobOrder GetAssignedJob() {
        return this._assignedJob;
    }

    public void FinishJob() {
        this.WorkingManager.FinishJob(this._assignedJob);
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
            case Metamorphosis.Pupa:
                activeModel = eggModel;
                break;
        }

        if (activeModel != prevModel) {
            LeanTween.scale(gameObject, Vector3.zero, 1f).setOnComplete(() => {
                prevModel.SetActive(false);
                activeModel.SetActive(true);
                LeanTween.scale(gameObject, Vector3.one, 1f).setOnComplete(() => {
                    canWork = true;
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
                    case Caste.Drone: return 12;
                    case Caste.Worker: return 2;
                    case Caste.Queen: return 8;
                    default: return 3;
                }
            case Metamorphosis.Adult:
                switch (caste) {
                    case Caste.Drone: return 2;
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

    public Metamorphosis Metamorphosis {
        get => metamorphosis;
        set => metamorphosis = value;
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
    Collector
}
public enum Metamorphosis {
    Egg,
    Larva,
    Pupa,
    Adult
}