using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherAnimationScript : MonoBehaviour
{
    [SerializeField] GameObject arrowObj;
    public void ShootArrow()
    {
        Debug.Log("Arrow");
        arrowObj.SetActive(true);
    }
}
