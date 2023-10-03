using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnBeatRotate : MonoBehaviour
{
    
    public Vector3 startScale = Vector3.one;
    public Vector3 endScale = new Vector3(1.1f, 1.1f, 1.1f);

    public float startDuration = 0.025f;
    private float startElapsedTime = 0f;

    public float endDuration = 0.05f;
    private float endElapsedTime = 0f;
    
    // Start is called before the first frame update
    void Start()
    {
        //onBeatRotate()
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
   
}
