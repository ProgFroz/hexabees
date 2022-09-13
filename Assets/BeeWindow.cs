using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker.Actions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeeWindow : MonoBehaviour {
    [SerializeField] private UIManager uiManager;
    [SerializeField] private bool hidden;
    
    [SerializeField] private TextMeshProUGUI nameText;
    
    [SerializeField] private TraitHolder traitHolder1;
    [SerializeField] private TraitHolder traitHolder2;
    [SerializeField] private TraitHolder traitHolder3;
    
    [SerializeField] private TextMeshProUGUI casteText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI jobText;
    
    [SerializeField] private TextMeshProUGUI nectarText;
    [SerializeField] private TextMeshProUGUI pollenText;
    [SerializeField] private TextMeshProUGUI waxText;
    [SerializeField] private TextMeshProUGUI honeyText;
    [SerializeField] private TextMeshProUGUI royalJellyText;
    
    [SerializeField] private TextMeshProUGUI taskText;
    
    [SerializeField] private Canvas jobCanvas;
    [SerializeField] private Canvas semenCanvas;
    
    [SerializeField] private Slider foodSlider;
    [SerializeField] private Slider waterSlider;
    [SerializeField] private Slider semenSlider;

    private bool _transitioning;
    void Start()
    {
        transform.localScale = Vector3.zero;
        Hidden = true;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateInterface(uiManager.SelectedBee);
    }

    public void UpdateInterface(Bee bee) {
        if (bee == null) {
            Hide();
            return;
        }
        if (bee != null && hidden) Show();
        
        semenCanvas.enabled = bee.Caste == Caste.Queen && bee.Metamorphosis == Metamorphosis.Adult;
        jobCanvas.enabled = bee.Caste == Caste.Worker && bee.Metamorphosis == Metamorphosis.Adult;
        
        this.nameText.text = bee.DisplayName;
        traitHolder1.UpdateTrait(bee.Trait1);
        traitHolder2.UpdateTrait(bee.Trait2);
        traitHolder3.UpdateTrait(bee.Trait3);

        casteText.text = bee.GetCasteAsString();
        ageText.text = bee.GetAgeAsString();
        jobText.text = bee.GetJobAsString();

        nectarText.text = bee.Inventory[Item.Nectar].ToString();
        pollenText.text = bee.Inventory[Item.Pollen].ToString();
        waxText.text = bee.Inventory[Item.Wax].ToString();
        honeyText.text = bee.Inventory[Item.Honey].ToString();
        royalJellyText.text = bee.Inventory[Item.RoyalJelly].ToString();

        taskText.text = bee.GetTaskAsString();

        foodSlider.minValue = 0;
        semenSlider.minValue = 0;
        waterSlider.minValue = 0;

        foodSlider.maxValue = bee.MaxFoodLevel;
        semenSlider.maxValue = bee.MaxSemenLevel;
        waterSlider.maxValue = bee.MaxWaterLevel;

        foodSlider.value = bee.CurrentFoodLevel;
        semenSlider.value = bee.Caste == Caste.Queen ? bee.CurrentSemenLevel : 0;
        waterSlider.value = bee.CurrentWaterLevel;
    }

    public void Hide() {
        if (!_transitioning) {
            _transitioning = true;
            LeanTween.scale(gameObject, Vector3.zero, 0.2f).setEaseInOutCubic().setOnComplete(() => {
                _transitioning = false;
            });
            Hidden = true;
        }
    }

    public void Show() {
        if (!_transitioning) {
            _transitioning = true;
            LeanTween.scale(gameObject, Vector3.one, 0.2f).setEaseInOutCubic().setOnComplete(() => {
                _transitioning = false;
            });
            Hidden = false;
        }
    }

    public UIManager UIManager {
        get => uiManager;
        set => uiManager = value;
    }

    public TextMeshProUGUI NameText {
        get => nameText;
        set => nameText = value;
    }

    public TraitHolder TraitHolder1 {
        get => traitHolder1;
        set => traitHolder1 = value;
    }

    public TraitHolder TraitHolder2 {
        get => traitHolder2;
        set => traitHolder2 = value;
    }

    public TraitHolder TraitHolder3 {
        get => traitHolder3;
        set => traitHolder3 = value;
    }

    public bool Hidden {
        get => hidden;
        set => hidden = value;
    }
}
