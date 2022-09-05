using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;

public class WorkingManager : MonoBehaviour {
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
        return jobOrder;
    }

    private void Update() {
        AssignJobs();
    }

    private void AssignJobs() {
        for (int i = 0; i < jobQueue.Count; i++) {
            JobOrder jobOrder = jobQueue[i];
            Bee bee = FindBee(jobOrder);
            Debug.Log(bee);
            if (bee) {
                jobOrder.AssignedBee = bee;
                bee.AssignJob(jobOrder);
                this.activeJobs.Add(jobOrder);
                this.jobQueue.Remove(jobOrder);
            }
        }
    }

    public bool CancelJob(JobOrder jobOrder) {
        Bee bee = jobOrder.AssignedBee;
        bee.AssignJob(null);
        jobOrder.AssignedBee = null;
        return this.activeJobs.Remove(jobOrder);
    }

    public bool RequeueJob(JobOrder jobOrder) {
        if (!activeJobs.Contains((jobOrder))) return false;
        if (jobOrder.AssignedBee) jobOrder.AssignedBee = null;
        this.jobQueue.Add(jobOrder);
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
        
        Debug.Log(possibles.Count);
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
        PriorityValue value = bee.Priorities[jobOrder.Action];
        Debug.Log(bee.assignedJob + " & " + (value != PriorityValue.Cant && value != PriorityValue.Wont));
        return (value != PriorityValue.Cant && value != PriorityValue.Wont);
    }
    
    
}

[Serializable]
public class JobOrder {
    private HexMapType mapType;
    private BeeAction action;
    private Bee assignedBee;
    private HexCell cell;
    private bool finished;

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
}

public enum PriorityValue {
    High,
    Medium,
    Low,
    Wont,
    Cant
    
}
