using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;

public class WorkingManager : MonoBehaviour {
    public TimeManager timeManager;
    public HexGrid overworldGrid;
    public HexGrid hiveGrid;
    public List<JobOrder> jobQueue = new List<JobOrder>();
    public List<JobOrder> activeJobs = new List<JobOrder>();

    private void Start() {
        this.jobQueue = new List<JobOrder>();
    }

    public JobOrder AddJobOrder(BeeAction action, HexCell hexCell) {
        JobOrder jobOrder = new JobOrder();
        jobOrder.Action = action;
        jobOrder.AssignedBee = null;
        jobOrder.Finished = false;
        jobOrder.MapType = hexCell.MapType;
        jobOrder.Cell = hexCell;
        
        jobQueue.Add(jobOrder);
        hexCell.AssignJob(jobOrder);
        hexCell.ShowJobHighlight();
        return jobOrder;
    }

    private void Update() {
        AssignJobs();
        
    }

    private void AssignJobs() {
        for (int i = 0; i < jobQueue.Count; i++) {
            JobOrder jobOrder = jobQueue[i];
            Bee bee = FindBee(jobOrder);
            if (bee) {
                jobOrder.AssignedBee = bee;
                bee.AssignJob(jobOrder);
                this.activeJobs.Add(jobOrder);
                this.jobQueue.Remove(jobOrder);
                bee.Travel(jobOrder.Cell);
            }
        }
    }

    public bool CancelJob(JobOrder jobOrder) {
        Debug.Log(jobOrder.Cell != null);
        Bee bee = jobOrder.AssignedBee;
        HexCell cell = jobOrder.Cell;
        if (bee) {
            bee.AssignJob(null);
            jobOrder.AssignedBee = null;
        }
        cell.AssignJob(null);
        cell.DisableJobHighlight();
        jobOrder.Cell = null;
        this.jobQueue.Remove(jobOrder);
        return this.activeJobs.Remove(jobOrder);
    }
    
    public bool CancelJob(HexCell cell) {
        JobOrder order = FindJobOrder(cell);
        if (order != null) {
            return this.CancelJob(order);
        }
        return false;
    }

    public JobOrder FindJobOrder(HexCell cell) {
        foreach (JobOrder jobOrder in activeJobs) {
            if (jobOrder.Cell == cell) return jobOrder;
        }
        foreach (JobOrder jobOrder in jobQueue) {
            if (jobOrder.Cell == cell) return jobOrder;
        }

        return null;
    }

    public bool RequeueJob(JobOrder jobOrder) {
        if (!activeJobs.Contains((jobOrder))) return false;
        if (jobOrder.AssignedBee) jobOrder.AssignedBee = null;
        this.jobQueue.Add(jobOrder);
        return this.activeJobs.Remove(jobOrder);
    }

    public bool FinishJob(JobOrder jobOrder) {
        if (!activeJobs.Contains((jobOrder))) return false;
        jobOrder.AssignedBee.AssignJob(null);
        jobOrder.AssignedBee = null;
        jobOrder.Cell.AssignJob(null);
        jobOrder.Cell.DisableJobHighlight();
        jobOrder.Finished = true;
        return this.activeJobs.Remove(jobOrder);
        
    }

    private Bee FindBee(JobOrder jobOrder) {
        List<Bee> possibles = new List<Bee>();
        List<Bee> highestPriorities = new List<Bee>();

        for (int i = 0; i < overworldGrid.Units.Count; i++) {
            HexUnit unit = overworldGrid.Units[i];
            if (unit) {
                Bee bee = unit.GetComponent<Bee>();
                if (CheckIfBeeCan(bee, jobOrder)) {
                    possibles.Add(bee);
                }
            }
            
        }
        for (int i = 0; i < hiveGrid.Units.Count; i++) {
            HexUnit unit = hiveGrid.Units[i];
            if (unit) {
                Bee bee = unit.GetComponent<Bee>();
                if (CheckIfBeeCan(bee, jobOrder)) {
                    possibles.Add(bee);
                }
            }
        }

        PriorityValue highestPrio = PriorityValue.Wont;
        foreach (Bee bee in possibles) {
            PriorityValue value = bee.Priorities[jobOrder.Action];
            if (value == highestPrio) {
                highestPriorities.Add(bee);
            } 
            else if (value < highestPrio) {
                highestPrio = value;
                highestPriorities = new List<Bee>();
                highestPriorities.Add(bee);
            }
        }

        if (highestPriorities.Count < 1) return null;
        if (highestPriorities.Count == 1) return highestPriorities[0];

        return FindBeeWithSmallestDistance(highestPriorities);
    }

    private Bee FindBeeWithSmallestDistance(List<Bee> highestPriorities) {
        return highestPriorities[0];
    }

    private bool CheckIfBeeCan(Bee bee, JobOrder jobOrder) {
        if (bee.priorities.Count < 1) return false;
        PriorityValue value = bee.Priorities[jobOrder.Action];
        return (value != PriorityValue.Cant && value != PriorityValue.Wont) && bee.GetAssignedJob() == null && bee.CanWork();
    }
    
    
}

[System.Serializable]
public class JobOrder {
    private HexMapType mapType;
    private BeeAction action;
    private Bee assignedBee;
    private HexCell cell;
    private bool finished;
    private float progress;

    // public override string ToString() {
    //     return "";
    //     // return "Action: " + action + ", Assigned Bee: " + assignedBee.name + ", Cell: " + cell.coordinates;
    // }

    public BeeAction Action {
        get => action;
        set => action = value;
    }

    public Bee AssignedBee {
        get => assignedBee;
        set => assignedBee = value;
    }

    public bool Finished {
        get => finished;
        set => finished = value;
    }

    public HexMapType MapType {
        get => mapType;
        set => mapType = value;
    }

    public HexCell Cell {
        get => cell;
        set => cell = value;
    }

    public float Progress {
        get => progress;
        set => progress = value;
    }

    public int GetRequiredHours() {
        return GetRequiredHoursByAction(this.action);
    }

    private int GetRequiredHoursByAction(BeeAction action) {
        switch (action) {
            case BeeAction.Breed: return 3;
            case BeeAction.Cover: return 1;
            case BeeAction.Destroy: return 2;
            case BeeAction.Evaporate: return 1;
            case BeeAction.Evaporator: return 3;
            case BeeAction.Feed: return 1;
            case BeeAction.Fertilize: return 2;
            case BeeAction.Gather: return 10;
            case BeeAction.Lay: return 2;
            case BeeAction.Mix: return 1;
            case BeeAction.Mixer: return 3;
            case BeeAction.Pollinate: return 2;
            case BeeAction.Refine: return 1;
            case BeeAction.Refiner: return 3;
            case BeeAction.Royal: return 3;
            case BeeAction.Storage: return 3;
            default: return 10;
        }
    }
}

public enum PriorityValue {
    High,
    Medium,
    Low,
    Wont,
    Cant
    
}
