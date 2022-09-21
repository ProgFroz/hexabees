using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BeeListElement : MonoBehaviour {
    private UIManager _uiManager;
    [SerializeField] private Button button;
    [SerializeField] private RawImage casteImage;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private PriorityManager priorityManager;
    [SerializeField] private Bee bee;
    
    // Start is called before the first frame update
    void Start() {
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        button.onClick.AddListener(() => _uiManager.UpdateSelectedGameObject(bee.gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateElements(Bee bee) {
        nameText.text = bee.DisplayName;
        priorityManager.UpdatePriorities(bee);
        this.bee = bee;
    }
}
