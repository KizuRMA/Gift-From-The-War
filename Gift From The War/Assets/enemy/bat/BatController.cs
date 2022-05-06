using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    BaseState state;
    public float hight { get; set; }
    public float forwardAngle { get; set; }
    [SerializeField] public float defaltHight;
    [SerializeField] public float defaltForwardAngle;


    // Start is called before the first frame update
    void Start()
    {
        hight = defaltHight;
        forwardAngle = defaltForwardAngle;
        //ステートを切り替える
        ChangeState(GetComponent<batMove>());
    }

    // Update is called once per frame
    void Update()
    {
        state.Update();
        Debug.Log(hight);
    }

    public void ChangeState(BaseState _state)
    {
        //実体を削除
        state = null;
        //新しい実体のアドレスを入れる
        state = _state;
        state.Start();
    }
}
