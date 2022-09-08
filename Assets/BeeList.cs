using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeeList : MonoBehaviour {
    public UIManager uiManager;

    public Button beeListButton;

    public TextMeshProUGUI workerAmountText;
    public TextMeshProUGUI workerBreedAmountText;
    public TextMeshProUGUI droneAmountText;
    public TextMeshProUGUI droneBreedAmountText;
    public TextMeshProUGUI queenAmountText;
    public TextMeshProUGUI queenBreedAmountText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateAmount(int amount, CasteAmountType type) {
        switch (type) {
            case CasteAmountType.Worker: this.workerAmountText.text = amount.ToString();
                break;
            case CasteAmountType.WorkerBreed: this.workerBreedAmountText.text = amount.ToString();
                break;
            case CasteAmountType.Drone: this.droneAmountText.text = amount.ToString();
                break;
            case CasteAmountType.DroneBreed: this.droneBreedAmountText.text = amount.ToString();
                break;
            case CasteAmountType.Queen: this.queenAmountText.text = amount.ToString();
                break;
            case CasteAmountType.QueenBreed: this.queenBreedAmountText.text = amount.ToString();
                break;
        }
    }
}

public enum CasteAmountType {
    Worker,
    WorkerBreed,
    Drone,
    DroneBreed,
    Queen,
    QueenBreed
}

