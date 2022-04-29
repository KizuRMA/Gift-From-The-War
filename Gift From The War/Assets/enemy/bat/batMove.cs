using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class batMove : BaseState
{
    private CharacterController playerCC;
    private CharacterController batCC;
    private NavMeshAgent _agent;
    Transform rantanTransform;
    [SerializeField] bool moveFlg;
    [SerializeField] float hight;
    [SerializeField] float forwardAngle;
    [SerializeField] float playerFromInterval;

    // Start is called before the first frame update
    public override void Start()
    {
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();
        batCC = GetComponent<CharacterController>();
        _agent = GetComponent<NavMeshAgent>();
        rantanTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    public override void Update()
    {
        //体を前に傾ける
        Transform _myTrans = this.transform;

        Vector3 _localAngle = _myTrans.localEulerAngles;

        _localAngle.x = forwardAngle;

        _myTrans.eulerAngles = _localAngle;

        //移動する場合
        if (moveFlg)
        {
            Vector3 _playerPos = playerCC.transform.position;
            Vector3 _myPos = this.transform.position;

            //プレイヤーに近づきすぎない処理
            float dis = Vector3.Distance(_myPos, _playerPos);

            //プレイヤーとの距離を調べる
            if (dis <= playerFromInterval)
            {
                //近づきすぎている場合
                _agent.destination = _myPos;
                BatController batCon = gameObject.GetComponent<BatController>();

                BaseState state = GetComponent<WingFoldState>();

                batCon.ChangeState(state);
                //早期リターン
                return;
            }
            else
            {
                //離れている場合
                _agent.destination = _playerPos;
            }

        }

        rantanTransform.position += new Vector3(0, hight, 0);
    }
}
