using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    BaseState state;

    // Start is called before the first frame update
    void Start()
    {
        state = new batMove();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeState(BaseState _state)
    {
        
    }
}
