using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMapButton : MonoBehaviour {
    public GameObject overworldImage;
    public GameObject hiveImage;

    public void SwitchImages(HexMapType current) {
        if (overworldImage == null || hiveImage == null) throw new NullReferenceException();
        if (current == HexMapType.Overworld && !overworldImage.activeSelf) {
            this.overworldImage.SetActive(true);
            this.hiveImage.SetActive(false);
        }
        else if (current == HexMapType.Hive && !hiveImage.activeSelf) {
            this.hiveImage.SetActive(true);
            this.overworldImage.SetActive(false);
        }
        else {
            Debug.Log(current + " " + overworldImage.activeSelf + " " + hiveImage.activeSelf);
            throw new ArgumentException("State of Maps and Change Map Button is mismatched.");
        }
    }
    
}
