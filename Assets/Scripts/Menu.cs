using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenuAttribute(fileName = "New Food", menuName = "New Food/food")]
public class Menu : ScriptableObject
{
    public string foodName;
    public GameObject foodObj;
    public Food food;
    public bool isActive;
    public float cookingTime;
    public enum Food
    {
        Bread,
        Coke,
    }
}
