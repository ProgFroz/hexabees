using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PriorityManager : MonoBehaviour {
    [SerializeField] private Bee bee;
    [SerializeField] private PriorityButton gatherPriorityButton;
    [SerializeField] private PriorityButton pollinatePriorityButton;
    [SerializeField] private PriorityButton fertilizePriorityButton;
    
    [SerializeField] private PriorityButton destroyPriorityButton;
    
    [SerializeField] private PriorityButton buildPriorityButton;
    
    [SerializeField] private PriorityButton feedPriorityButton;
    [SerializeField] private PriorityButton coverPriorityButton;
    
    [SerializeField] private PriorityButton convertPriorityButton;
    // Start is called before the first frame update
    void Start()
    {
         gatherPriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Gather));
         pollinatePriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Pollinate));
         fertilizePriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Fertilize));
         destroyPriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Destroy));
         buildPriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Evaporator));
         feedPriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Feed));
         coverPriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Cover));
         convertPriorityButton.Button.onClick.AddListener(() => UpdateBeePriorities(bee, BeeAction.Evaporate));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdatePriorities(Bee bee) {
        this.bee = bee;
        gatherPriorityButton.UpdateImages(bee.Priorities[BeeAction.Gather]);
        pollinatePriorityButton.UpdateImages(bee.Priorities[BeeAction.Pollinate]);
        fertilizePriorityButton.UpdateImages(bee.Priorities[BeeAction.Fertilize]);
        destroyPriorityButton.UpdateImages(bee.Priorities[BeeAction.Destroy]);
        buildPriorityButton.UpdateImages(bee.Priorities[BeeAction.Evaporator]);
        feedPriorityButton.UpdateImages(bee.Priorities[BeeAction.Feed]);
        coverPriorityButton.UpdateImages(bee.Priorities[BeeAction.Cover]);
        convertPriorityButton.UpdateImages(bee.Priorities[BeeAction.Evaporate]);
    }

    private void UpdateBeePriorities(Bee bee, BeeAction beeAction) {
        if (!bee) return;
        PriorityValue next = NextPriority(bee.Priorities[beeAction]);
        bee.UpdatePriorities(beeAction, next);
        this.UpdatePriorities(bee);
    }

    private PriorityValue NextPriority(PriorityValue current) {
        switch (current) {
            case PriorityValue.Cant: return PriorityValue.Cant;
            case PriorityValue.Wont: return PriorityValue.Low;
            case PriorityValue.Low: return PriorityValue.Medium;
            case PriorityValue.Medium: return PriorityValue.High;
            case PriorityValue.High: return PriorityValue.Wont;
            default: return PriorityValue.Cant;
        }
    }
}
