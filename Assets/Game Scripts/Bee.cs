using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.TestTools;

[RequireComponent(typeof(HexUnit))]
public class Bee : MonoBehaviour {
    
    [SerializeField]
    private HexUnit hexUnit;
    
    public bool hasJob;

    public WorkingManager _workingManager;
    
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
    private int age;
    
    [SerializeField]
    private int maxSemenLevel;
    [SerializeField]
    private int currentSemenLevel;
    
    [SerializeField]
    private Caste caste = Caste.None;
    [SerializeField]
    private Job job = Job.None;

    private JobOrder _assignedJob = null;

    public Dictionary<BeeAction, PriorityValue> priorities = new Dictionary<BeeAction, PriorityValue>();
    
    float _time;
    private float _interval = 3f;
    public bool doWork = false;

    public Dictionary<BeeAction, PriorityValue> Priorities {
        get => priorities;
        set => priorities = value;
    }

    void Start() {
        _time = 0f;
        GenerateStats();
        caste = Caste.Worker;
        job = Job.Collector;
        UpdatePriorities();
        WorkingManager = hexUnit.Grid.workingManager;
        hexGameUI = hexUnit.Grid.hexGameUI;
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
        this.hasJob = this._assignedJob != null;
        
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
        get => age;
        set => age = value;
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
        get => _workingManager;
        set => _workingManager = value;
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
        if (_assignedJob.Progress >= 1) {
            FinishJob();
        }
        _assignedJob.Progress += _interval / _assignedJob.GetRequiredHours();
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