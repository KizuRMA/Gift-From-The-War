using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rantanMove : MonoBehaviour
{
    private Transform trans;
    private FPSController fpsC;
    private Transform camTrans;
    private int time = 0;
    private Quaternion firstQua;
    [SerializeField] private int timeMax = 900;
    private int upDown = 1;
    [SerializeField] private float movePower = 0.0001f;
    private float moveSum = 0;  //�ړ��ʂ̍��v
    [SerializeField] private float upRaito = 0; //��̌X���̕␳�{��

    // Start is called before the first frame update
    void Start()
    {
        trans = GetComponent<Transform>();
        camTrans = GameObject.Find("Main Camera").GetComponent<Transform>();
        fpsC = GameObject.Find("player").GetComponent<FPSController>();
        firstQua = trans.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        rotation();
        tremor();
    }

    void rotation()
    {
        Quaternion _ranQua = Quaternion.identity;

        //�J�����̃N�I�[�^�j�I���l���擾
        Quaternion _camQua = camTrans.rotation;

        _ranQua = trans.rotation;

        float _camEulerAngleX = _camQua.eulerAngles.x;

        //�p�x�𒲐�����
        if (_camEulerAngleX >= 300.0f)
        {
            _camEulerAngleX -= 360.0f;
            _camEulerAngleX *= upRaito;
        }

        _camEulerAngleX *= -1;

        _ranQua = Quaternion.AngleAxis(_camEulerAngleX, Vector3.right);
        trans.localRotation = _ranQua * firstQua;
    }

    void tremor()
    {
        if (fpsC.GetMoveFlg())
        {
            if (time > timeMax || time < 0)
            {
                upDown *= -1;
            }
            time += upDown;
            moveSum += movePower * upDown;
            trans.localPosition += trans.up * movePower * upDown;
        }
        else
        {
            trans.localPosition += trans.up * -moveSum;
            time = 0;
            upDown = 1;
            moveSum = 0;
        }
    }
}
