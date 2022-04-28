using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMove : MonoBehaviour
{
    private Transform trans;
    [SerializeField] private Vector3 firstPos;
    private float posY = 0;
    private int upDown = 1; //è„Ç™ÇÈÇ©â∫Ç™ÇÈÇ©ÇÃïÑçÜÇï\Ç∑
    [SerializeField] private float upDownSpeed;
    [SerializeField] private float maxPosY;
    private FPSController fpsC;
    private CharacterController playerCC;

    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();
        fpsC = GameObject.Find("player").GetComponent<FPSController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(fpsC.GetMoveFlg())
        {
            if(Mathf.Abs(posY) > Mathf.Abs(maxPosY))
            {
                upDown *= -1;
            }
            posY += upDownSpeed * upDown;
        }
        else
        {
            posY = 0;
            upDown = 1;
        }

        trans.position = firstPos + new Vector3(0.0f, posY, 0.0f) + playerCC.transform.position;
    }
}
