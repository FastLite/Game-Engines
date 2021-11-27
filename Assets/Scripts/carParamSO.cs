using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "NewCar", menuName = "ScriptableObjects/cars", order = 1)]

public class carParamSO : ScriptableObject
{
    public int carId;
    public string carName;
    public Vector3 speed;
    public Vector3 handeling;
}
