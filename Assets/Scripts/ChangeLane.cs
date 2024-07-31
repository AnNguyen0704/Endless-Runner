using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeLane : MonoBehaviour
{
    // Start is called before the first frame update
    public void PositionLane()
    {
        int randomLane = Random.Range(-1, 2);
        transform.position = new Vector3(randomLane, transform.position.y, transform.position.z);
    }

}
