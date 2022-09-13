using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour {
    
    private HexCell cell;

    private List<GameObject> _treeObjects = new List<GameObject>();

    private int _hourCreated;
    
    [SerializeField]
    private int ageHours;

    private int ageDays;
    
    private TimeManager _timeManager;
    // Start is called before the first frame update

    public Material springMaterial;
    public Material summerMaterial;
    public Material fallMaterial;
    public Material winterMaterial;

    private int _prevSeason = -1;
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

        if (_prevSeason != _timeManager.season) {
            foreach (GameObject gameObject in _treeObjects) {
                switch (_timeManager.season) {
                    case 0: gameObject.GetComponent<MeshRenderer>().material = springMaterial;
                        break;
                    case 1: gameObject.GetComponent<MeshRenderer>().material = summerMaterial;
                        break;
                    case 2: gameObject.GetComponent<MeshRenderer>().material = fallMaterial;
                        break;
                    case 3: gameObject.GetComponent<MeshRenderer>().material = winterMaterial;
                        break;
                    default: break;
                }
            }
        }

        _prevSeason = _timeManager.season;
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

    public List<GameObject> TreeObjects {
        get => _treeObjects;
        set => _treeObjects = value;
    }
}
