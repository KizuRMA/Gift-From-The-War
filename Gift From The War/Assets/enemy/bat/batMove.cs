using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class batMove : BaseState
{
    private CharacterController playerCC;
    private CharacterController batCC;
    private NavMeshAgent agent;
    private UltraSoundBeam ultrasound;
    Transform defaltTransform;
    [SerializeField] bool moveFlg;
    [SerializeField] float playerFromInterval;
    [SerializeField] float ultrasoundCoolTime;
    private float untilLaunch;

    // Start is called before the first frame update
    public override void Start()
    {
        ultrasound = null;
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();
        batCC = GetComponent<CharacterController>();
        myController = GetComponent<BatController>();
        agent = GetComponent<NavMeshAgent>();
        defaltTransform = GetComponent<Transform>();

        //超音波を初期化
        ultrasound = GetComponent<UltraSoundBeam>();
        ultrasound.Start();

        untilLaunch = 0;

        agent.isStopped = false;
        agent.updateUpAxis = true;
        agent.updateRotation = true;
        agent.updatePosition = true;
    }

    // Update is called once per frame
    public override void Update()
    {
        //体を前に傾ける
        Vector3 _localAngle = transform.localEulerAngles;
        _localAngle.x = myController.forwardAngle;
        transform.localEulerAngles = _localAngle;

        //高さを調整する
        Ray _ray = new Ray(transform.position, Vector3.up);
        RaycastHit _raycastHit;
        bool _hit = Physics.Raycast(_ray, out _raycastHit);

        //ステージの立幅を記録
        float _hight = _raycastHit.distance;

        //ステージの縦幅の４割の位置にいるようにする
        _hight *= 0.4f;
        //コウモリの飛行上限を設定する
        if (_hight > 0.8f)
        {
            _hight = 0.8f;
        }

        //現在のコウモリを高さを含んだ座標
        Vector3 nowPos = new Vector3(transform.position.x, transform.position.y + myController.hight, transform.position.z);
        //本来いてほしい座標
        Vector3 nextPos = new Vector3(transform.position.x, transform.position.y + _hight, transform.position.z);

        //ナビメッシュのスピードを用いてコウモリの高さを調整する
        transform.position = Vector3.MoveTowards(nowPos, nextPos, agent.speed);

        //次のフレームでは現在のY軸が保存されないため、記録しておく。
        myController.hight = transform.position.y;

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
                agent.destination = _myPos;
                BatController batCon = gameObject.GetComponent<BatController>();

                batCon.ChangeState(GetComponent<WingFoldState>());
                //早期リターン
                return;
            }
            else
            {
                //離れている場合
                agent.destination = _playerPos;
            }

            //超音波処理
            if (ultrasound != null && untilLaunch + ultrasoundCoolTime <= Time.time)
            {
                ultrasound.Update();
            }

            //超音波を出し切った場合
            if (ultrasound.IsAlive() == false)
            {
                //超音波処理を初期化
                ultrasound.Init();
                untilLaunch = Time.time;
            }
        }

    }
}
