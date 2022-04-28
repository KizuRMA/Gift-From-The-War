using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatController : MonoBehaviour
{
    BaseState state;

    // Start is called before the first frame update
    void Start()
    {
        //ステートを切り替える
        ChangeState(GetComponent<batMove>());
    }

    // Update is called once per frame
    void Update()
    {
        state.Update();
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
