using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    private Transform targetTransform;
    [SerializeField]
    private float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(TargetTransform != null)
        {
            Vector3 finalTargetPosition = new Vector3(TargetTransform.position.x, TargetTransform.position.y, this.transform.position.z);
            this.transform.position = Vector3.Lerp(this.transform.position, finalTargetPosition, Speed);
        }
        
    }

    public Transform TargetTransform { get => targetTransform; set => targetTransform = value; }
    public float Speed { get => speed; set => speed = value; }
}
