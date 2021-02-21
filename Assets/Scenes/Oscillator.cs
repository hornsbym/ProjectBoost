using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class Oscillator : MonoBehaviour{
    [SerializeField] Vector3 movementVector = new Vector3(10f, 10f, 10f);
    [SerializeField] float period = 2f;

    [Range(0,1)]
    [SerializeField]
    float movementFactor;

    Vector3 startingPos;
    
    // Pulled this out of Update to reduce redundant calculations
    private const float tau = Mathf.PI * 2;

    
    // Start is called before the first frame update
    void Start(){
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update(){
        // Prevents our period from being 0
        if (period <= Mathf.Epsilon) { return; }

        float cycles = Time.time / period;

        float rawSin = Mathf.Sin(cycles * tau);

        movementFactor = rawSin / 2f + .05f;
        
        Vector3 offset = movementVector * movementFactor;
        transform.position = startingPos + offset;
    }
}
