using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : MonoBehaviour
{
    public Vector2 MoveDir;
    void Update()
    {
        transform.position += (Vector3)MoveDir*Time.deltaTime;
    }
}
