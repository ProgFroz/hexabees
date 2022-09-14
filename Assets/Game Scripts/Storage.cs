using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour {

    private StorageManager _storageManager;
    private HexCell cell;

    private List<GameObject> _storageObjects = new List<GameObject>();

    private int _hourCreated;
    
    [SerializeField]
    private int ageHours;

    private int ageDays;
    
    private TimeManager _timeManager;

    private Item _currentlyStoredItem = Item.None;
    private int _currentlyStoredAmount = 0;
    private int _maxStoredAmount = 100;

    void Start()
    {
        HourCreated = TimeManager.GetCurrentHoursSinceBegin();
        Initialize();
    }

    private void Initialize() {
        
    }

    // Update is called once per frame
    void Update() {
        ageHours = TimeManager.GetCurrentHoursSinceExistence(_hourCreated);
        ageDays = TimeManager.ConvertHoursToDays(ageHours);
    }

    public void Add(Item item, int amount) {
        if (CheckIfStorable(item, amount)) {
            CurrentlyStoredAmount += amount;
        }
    }

    public bool CheckIfStorable(Item item, int amount) {
        if (CurrentlyStoredItem == Item.None || CurrentlyStoredItem == item) {
            if (CurrentlyStoredAmount + amount <= MaxStoredAmount) {
                return true;
            }
        }

        return false;
    }
    


    public HexCell Cell {
        get => cell;
        set => cell = value;
    }

    public int HourCreated {
        get => _hourCreated;
        set => _hourCreated = value;
    }

    public TimeManager TimeManager {
        get => _timeManager;
        set => _timeManager = value;
    }

    public List<GameObject> StorageObjects {
        get => _storageObjects;
        set => _storageObjects = value;
    }

    public Item CurrentlyStoredItem {
        get => _currentlyStoredItem;
        set => _currentlyStoredItem = value;
    }

    public StorageManager StorageManager {
        get => _storageManager;
        set => _storageManager = value;
    }

    public int CurrentlyStoredAmount {
        get => _currentlyStoredAmount;
        set => _currentlyStoredAmount = value;
    }

    public int MaxStoredAmount {
        get => _maxStoredAmount;
        set => _maxStoredAmount = value;
    }
}
