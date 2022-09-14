using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour {

    [SerializeField] private UIManager uiManager;
    private List<Storage> _storages = new List<Storage>();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStorage(Storage storage) {
        
    }

    public Storage FindClosestStorage(Bee bee, Item item, int amount) {
        List<Storage> canStore = new List<Storage>();
        foreach (Storage storage in _storages) {
            if (storage.CheckIfStorable(item, amount)) {
                canStore.Add(storage);
            }
        }

        if (canStore.Count > 0) {
            return canStore[0];
        }
        else {
            return null;
        }
    }

    public List<Storage> Storages {
        get => _storages;
        set => _storages = value;
    }
}
