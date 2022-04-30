using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class batMove : BaseState
{
    private CharacterController playerCC;
    private CharacterController batCC;
    private NavMeshAgent _agent;
    Transform defaltTransform;
    [SerializeField] bool moveFlg;
    [SerializeField] float playerFromInterval;

    // Start is called before the first frame update
    public override void Start()
    {
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();
        batCC = GetComponent<CharacterController>();
        myController = GetComponent<BatController>();
        _agent = GetComponent<NavMeshAgent>();
        defaltTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    public override void Update()
    {
        //体を前に傾ける
        Vector3 _localAngle = transform.localEulerAngles;
        _localAngle.x = myController.forwardAngle;
        transform.localEulerAngles = _localAngle;

        //移動する場合
        if (moveFlg)
        {
            Vector3 _playerPos = playerCC.transform.position;
            Vector3 _myPos = transform.position;

            //プレイヤーに近づきすぎない処理
            float dis = Vector3.Distance(_myPos, _playerPos);

            //プレイヤーとの距離を調べる
            if (dis <= playerFromInterval)
            {
                //近づきすぎている場合
                _agent.destination = _myPos;
                BatController batCon = gameObject.GetComponent<BatController>();

                batCon.ChangeState(GetComponent<WingFoldState>());
                //早期リターン
                return;
            }
            else
            {
                //離れている場合
                _agent.destination = _playerPos;
            }

        }

        transform.position = new Vector3(transform.position.x, myController.hight, transform.position.z);
    }
}
