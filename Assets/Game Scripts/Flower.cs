using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flower : MonoBehaviour {

    private HexCell cell;
    

    private List<GameObject> _flowerObjects = new List<GameObject>();
    [SerializeField]
    private int maxPollen;
    [SerializeField]
    private int currentPollen;
    [SerializeField]
    private int maxNectar;
    [SerializeField]
    private int currentNectar;
    
    private int _hourCreated;
    
    [SerializeField]
    private int ageHours;

    private int ageDays;
    
    private TimeManager _timeManager;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("FillUp", 0.0f, 1f);
        HourCreated = TimeManager.GetCurrentHoursSinceBegin();
        
        Initialize();
        UpdateSize();
        // UpdateSizeByPollenAndNectar();
    }

    private void Initialize() {
        this.maxPollen = 30;
        this.maxNectar = 30;
    }

    // Update is called once per frame
    void Update() {
        ageHours = TimeManager.GetCurrentHoursSinceExistence(_hourCreated);
        int prevAgeDays = ageDays;
        ageDays = TimeManager.ConvertHoursToDays(ageHours);
        if (prevAgeDays != ageDays && ageDays < 4) {
            UpdateSize();
        }
        // UpdateSizeByPollenAndNectar();
    }

    private void FillUp() {
        if (this.currentPollen < this.maxPollen) this.currentPollen++;
        if (this.currentNectar < this.maxNectar) this.currentNectar++;
    }

    public int DrainNectar(int desired) {
        int current = CurrentNectar;
        CurrentNectar = (CurrentNectar - desired < 0) ? 0 : (CurrentNectar - desired);
        return (current - desired) < 0 ? current : desired;
    }

    public void UpdateSize() {
        float factor = 0;
        switch (ageDays) {
            case 0: factor = 0.2f;
                break;
            case 1: factor = 0.5f;
                break;
            case 2: factor = 0.7f;
                break;
            default: factor = 1f;
                break;
        }

        // var seq = LeanTween.sequence();
        // foreach (GameObject gameObject in FlowerObjects) {
        //     if (gameObject != null && seq != null) {
        //         LTDescr descr = LeanTween.scale(gameObject, new Vector3(factor, factor, factor), 1f)
        //             .setEaseInOutCubic();
        //         seq.append(descr);
        //     }
        // }

        foreach (GameObject gameObject in FlowerObjects) {
            LTDescr descr = LeanTween.scale(gameObject, new Vector3(factor, factor, factor), 1f)
                .setEaseInOutCubic();
        }

    }
    
    // public void UpdateSizeByPollenAndNectar() {
    //     float factor = ((float)CurrentNectar + (float)CurrentPollen) / ((float)MaxNectar + (float)MaxPollen);
    //     Debug.Log(factor);
    //     // var seq = LeanTween.sequence();
    //     // foreach (GameObject gameObject in FlowerObjects) {
    //     //     if (gameObject != null && seq != null) {
    //     //         LTDescr descr = LeanTween.scale(gameObject, new Vector3(factor, factor, factor), 1f)
    //     //             .setEaseInOutCubic();
    //     //         seq.append(descr);
    //     //     }
    //     // }
    //
    //     foreach (GameObject gameObject in FlowerObjects) {
    //         gameObject.transform.localScale = new Vector3(factor, factor, factor);
    //     }
    //
    // }
    
    public int DrainPollen(int desired) {
        int current = CurrentPollen;
        CurrentPollen = (CurrentPollen - desired < 0) ? 0 : (CurrentPollen - desired);
        return (current - desired) < 0 ? current : desired;
    }

    public HexCell Cell {
        get => cell;
        set => cell = value;
    }

    public List<GameObject> FlowerObjects {
        get => _flowerObjects;
        set => _flowerObjects = value;
    }

    public int MaxPollen {
        get => maxPollen;
        set => maxPollen = value;
    }

    public int CurrentPollen {
        get => currentPollen;
        set => currentPollen = value;
    }

    public int MaxNectar {
        get => maxNectar;
        set => maxNectar = value;
    }

    public int CurrentNectar {
        get => currentNectar;
        set => currentNectar = value;
    }

    public int HourCreated {
        get => _hourCreated;
        set => _hourCreated = value;
    }

    public TimeManager TimeManager {
        get => _timeManager;
        set => _timeManager = value;
    }
}
