using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighlightImageHolder : MonoBehaviour {
    public UIManager uiManager;
    public GameObject imageHolder;
    public RawImage image;
    public HexCell cell;
    
    // Start is called before the first frame update
    void Start()
    {
        this.SetActive(false);
        this.uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetImage(BeeAction action) {
        SetTexture(uiManager.GetAccordingTexture(action), action);
    }
    
    public BeeAction SetHoverImage() {
        SetTexture(uiManager.GetAccordingTexture(uiManager.currentAction), uiManager.currentAction);
        return uiManager.currentAction;
    }

    public void SetActive(bool isActive) {
        imageHolder.SetActive(isActive);
    }

    private void SetTexture(Texture texture, BeeAction action) {
        image.texture = texture;

        switch (action) {
            case BeeAction.Cancel:
            case BeeAction.Destroy:
            case BeeAction.Refiner:
            case BeeAction.Evaporator:
            case BeeAction.Mixer:
                image.color = uiManager.GetBlankTexturesColor();
                break;
            default:
                image.color = Color.white;
                break;
        }
    }
}
