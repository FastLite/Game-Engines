using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class CarSelector : MonoBehaviour
{
    public List<Sprite> carImages;
    public Image carPlate;
    public List<carParamSO> carInfo;
    public Text properties, carName;
    private int id;

    

    public void ShowNextPlate()
    {
        carPlate.sprite = carImages[id];
        
        UpdateText(id);
        PlayerPrefs.SetInt("carId", id);
        id++;
        if (id>=carImages.Count)
        {
            id = 0;
        }
    }
    private void UpdateText(int carId)
    {
        properties.text = "Speed [" + carInfo[carId].speed.z + "]  |  Handling [" + carInfo[carId].handeling.y +"]";
        carName.text = carInfo[carId].name;
    }
}
