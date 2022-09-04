using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(HexUnit))]
public class Bee : MonoBehaviour {

    [SerializeField]
    private HexUnit hexUnit;
    
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
    
    void Start() {
        GenerateStats();
        InvokeRepeating("DrainStats", 0.0f, 1f);
    }

    private void GenerateStats() {
        MaxFoodLevel = 100;
        MaxWaterLevel = 300;
        CurrentFoodLevel = 10;
        CurrentWaterLevel = MaxWaterLevel;
    }

    // Update is called once per frame
    void Update()
    {
        
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