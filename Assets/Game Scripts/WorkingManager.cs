using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Jobs.LowLevel.Unsafe;
using UnityEditorInternal;
using Random = UnityEngine.Random;

public class WorkingManager : MonoBehaviour {
    public TimeManager timeManager;
    public StorageManager storageManager;
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
    
    public JobOrder AddJobOrder(Bee bee, BeeAction action, HexCell hexCell) {
        JobOrder jobOrder = new JobOrder();
        jobOrder.Action = action;
        jobOrder.AssignedBee = bee;
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
            Bee bee = jobOrder.AssignedBee == null ? FindBee(jobOrder) : jobOrder.AssignedBee;
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

    public bool FinishJob(Bee bee, JobOrder jobOrder) {
        if (!activeJobs.Contains((jobOrder))) return false;
        jobOrder.AssignedBee.AssignJob(null);
        jobOrder.AssignedBee = null;
        jobOrder.Cell.AssignJob(null);
        jobOrder.Cell.DisableJobHighlight();
        jobOrder.Finished = true;

        switch (jobOrder.Action) {
            case BeeAction.Gather: FinishGather(bee, jobOrder.Cell);
                break;
            case BeeAction.Pollinate: FinishPollinate(bee, jobOrder.Cell);
                break;
            case BeeAction.Storage: FinishStorage(bee, jobOrder.Cell);
                break;
            case BeeAction.Lay: FinishLay(bee, jobOrder.Cell);
                break;
            default: break;
        }
        
        
        return this.activeJobs.Remove(jobOrder);
        
    }

    private void FinishLay(Bee bee, HexCell cell) {
        cell.hexGrid.AddUnit(Instantiate(HexUnit.unitPrefab), cell, Random.Range(0f, 360f));
        bee.CurrentSemenLevel -= 1;
    }

    private void FinishStorage(Bee bee, HexCell cell) {
        // cell.SpecialIndex = 1;
        cell.FarmLevel = Bee.GenerateInt(2, 4);
    }

    private void FinishPollinate(Bee bee, HexCell cell) {
        bee.RemoveItemsFromInventory(Item.Pollen, Bee.GetRequiredPollenToPollinate());
        cell.PlantLevel = 3;
    }

    private void FinishGather(Bee bee, HexCell cell) {
        if (bee) {
            Flower flower = cell.Flower;
            int leftSpaceNectar = bee.GetSpaceForItem(Item.Nectar);
            int leftSpacePollen = bee.GetSpaceForItem(Item.Pollen);

            int receivedNectar = flower.DrainNectar(leftSpaceNectar);
            int receivedPollen = flower.DrainPollen(leftSpacePollen);

            bee.AddItemsToInventory(Item.Nectar, receivedNectar);
            bee.AddItemsToInventory(Item.Pollen, receivedPollen);

            if (bee.CheckIfInventoryFull(Item.Nectar)) {
                AddStoreTask(bee, Item.Nectar, bee.Inventory[Item.Nectar]);
            }
            if (bee.CheckIfInventoryFull(Item.Pollen)) {
                AddStoreTask(bee, Item.Pollen, bee.Inventory[Item.Pollen]);
            }
        }
    }

    private Bee FindBee(JobOrder jobOrder) {
        List<Bee> possibles = new List<Bee>();
        List<Bee> highestPriorities = new List<Bee>();
        List<Bee> mostSuitableInventory = new List<Bee>();

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

        

        switch (jobOrder.Action) {
            case BeeAction.Pollinate: mostSuitableInventory = FindBeesWithInventory(highestPriorities, Item.Pollen);
                break;
            case BeeAction.Lay: mostSuitableInventory = FindQueensWithSemen(highestPriorities);
                break;
            default:
                mostSuitableInventory = highestPriorities;
                break;
        }

        if (mostSuitableInventory.Count < 1) return null;
        if (mostSuitableInventory.Count == 1) return mostSuitableInventory[0];
        return FindBeeWithSmallestDistance(mostSuitableInventory);
    }

    private List<Bee> FindQueensWithSemen(List<Bee> highestPriorities) {
        List<Bee> queensWithSemen = new List<Bee>();
        foreach (Bee bee in highestPriorities) {
            if (bee.Caste == Caste.Queen && bee.CurrentSemenLevel > 0) {
                queensWithSemen.Add(bee);
            }
        }

        return queensWithSemen;
    }

    private List<Bee> FindBeesWithInventory(List<Bee> bees, Item item) {
        List<Bee> suitable = new List<Bee>();
        switch (item) {
            case Item.Pollen: suitable = FindBeesWithPollen(bees);
                break;
            default: break;
        }

        return suitable;
    }

    private List<Bee> FindBeesWithPollen(List<Bee> bees) {
        List<Bee> suitable = new List<Bee>();
        foreach (Bee bee in bees) {
            if (bee.HasEnoughPollen()) {
                suitable.Add(bee);
            }
        }

        return suitable;
    }

    private Bee FindBeeWithSmallestDistance(List<Bee> list) {
        return list.Count > 0 ? list[0] : null;
    }

    private bool CheckIfBeeCan(Bee bee, JobOrder jobOrder) {
        if (bee.priorities.Count < 1) return false;
        PriorityValue value = bee.Priorities[jobOrder.Action];
        return (value != PriorityValue.Cant && value != PriorityValue.Wont) && bee.GetAssignedJob() == null && bee.CanWork();
    }

    public void AddStoreTask(Bee bee, Item item, int amount) {
        Storage storage = storageManager.FindClosestStorage(bee, item, amount);
        if (storage) {
            AddJobOrder(bee, BeeAction.Store, storage.Cell);
        }
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
