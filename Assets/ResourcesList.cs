using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourcesList : MonoBehaviour
{
    public Button resourceListButton;

    public TextMeshProUGUI nectarText;
    public TextMeshProUGUI pollenText;
    public TextMeshProUGUI waxText;
    public TextMeshProUGUI honeyText;
    public TextMeshProUGUI royalJellyText;
    // Start is called before the first frame update
    void Start() {
        nectarText.text = 0.ToString();
        pollenText.text = 0.ToString();
        waxText.text = 0.ToString();
        honeyText.text = 0.ToString();
        royalJellyText.text = 0.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateInterface(StorageManager storageManager) {
        
    }
}
