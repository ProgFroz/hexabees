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
        image.texture = uiManager.GetAccordingTexture(action);
    }
    
    public BeeAction SetHoverImage() {
        image.texture = uiManager.GetAccordingTexture(uiManager.currentAction);
        return uiManager.currentAction;
    }

    public void SetActive(bool isActive) {
        imageHolder.SetActive(isActive);
    }
}
