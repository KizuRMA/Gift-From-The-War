using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WingFoldState : BaseState
{
    enum e_Action
    {
        none,
        sticking,
        leave,
    }

    [SerializeField] float ultrasoundCoolTime;
    [SerializeField] float ascendingSpeed;
    private Vector3 rayPosition;
    private Vector3 targetPos;
    private RaycastHit hit;
    private CharacterController playerCC;
    private NavMeshAgent agent;
    private UltraSound ultrasound;
    private Vector3 hightManager;
    private bool nextAnime;
    private e_Action nowAction;
    private float rotateY;
    private float untilLaunch;
    private float amountChangeAngX;
    private float amountChangeDis;

    // Start is called before the first frame update
    public override void Start()
    {
        rotateY = transform.eulerAngles.y;
        nextAnime = false;
        nowAction = e_Action.sticking;
        untilLaunch = 0;

        myController = GetComponent<BatController>();
        agent = GetComponent<NavMeshAgent>();
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();

        ultrasound = GetComponent<UltraSound>();
        ultrasound.Start();

        //ナビメッシュによるオブジェクトの回転を更新しない
        agent.isStopped = true;
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.updatePosition = false;

        rayPosition = new Vector3(transform.position.x, transform.position.y + myController.hight, transform.position.z);
        Ray ray = new Ray(rayPosition, Vector3.up);

        //Rayを上に飛ばす
        if (Physics.Raycast(ray, out hit))
        {
            //レイが天井に衝突している場合はターゲット座標に設定する。
            Vector3 targetVec = Vector3.up * (hit.distance - 0.0f);
            targetPos = rayPosition + targetVec;
        }
        else
        {
            BatController batCon = gameObject.GetComponent<BatController>();
            batCon.ChangeState(GetComponent<batMove>());
            return;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        //体を回転させる処理
        if (myController.forwardAngle >= 90)
        {
            transform.eulerAngles = new Vector3(180.0f - myController.forwardAngle, rotateY + 180.0f, 180.0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(myController.forwardAngle, rotateY, 0);
        }

        //現在のアクション状態毎に関数を実行する
        switch (nowAction)
        {
            case e_Action.none:
                ActionNone();
                break;
            //張り付き状態の時
            case e_Action.sticking:
                ActionSticking();
                break;
            //離れる状態の時
            case e_Action.leave:
                ActionLeave();
                break;
        }

        //コウモリの座標からワールド座標で下に向いているレイを作る
        Ray _ray = new Ray(transform.position, Vector3.down);
        RaycastHit _raycastHit;
        bool _rayHit = Physics.Raycast(_ray, out _raycastHit);

        //レイがオブジェクトに当たっている場合
        if (_rayHit == true)
        {
            //現在の高さを記録しておく
            myController.hight = _raycastHit.distance;
            Debug.Log(myController.hight);
        }
    }

    private void ActionSticking()
    {
        Animator animator = GetComponent<Animator>();

        //ターゲットとしている座標までの距離を調べる
        float _targetDis = Vector3.Distance(transform.position, targetPos);

        //天井との距離が移動量よりも大きい、または逆さまになっていない場合
        if (_targetDis >= ascendingSpeed || myController.forwardAngle < 180.0f)
        {
            //上に移動する処理
            transform.position = Vector3.MoveTowards(transform.position, targetPos, ascendingSpeed);
            _targetDis = Vector3.Distance(transform.position, targetPos);
        }
        else
        {
            //アクション状態を変更する
            nowAction = e_Action.none;
            nextAnime = false;
            return;
        }

        //天井との高さが近い場合
        if (_targetDis <= 0.3f)
        {
            //コウモリが180度回転していない場合
            if (myController.forwardAngle < 180.0f)
            {
                myController.forwardAngle += 1.0f;

                if (myController.forwardAngle >= 180.0f)
                {
                    myController.forwardAngle = 180.0f;
                }
            }

            if (nextAnime == false)
            {
                //羽を閉じるアニメーションに切り替える
                animator.SetInteger("trans", 1);
                nextAnime = true;
            }
        }
    }

    private void ActionNone()
    {
        //超音波のクールタイムが終了している場合
        if (untilLaunch + ultrasoundCoolTime <= Time.time)
        {
            //超音波を更新
            bool _hit = ultrasound.Update();

            //超音波がプレイヤーに当たっている場合
            if (_hit == true)
            {
                //アクション状態を天井から離れる状態に変化
                nowAction = e_Action.leave;
                targetPos += Vector3.down * 0.8f;
                amountChangeDis = Vector3.Distance(targetPos, transform.position);
                amountChangeAngX = myController.forwardAngle - 20.0f;
                Debug.Log(amountChangeAngX);
                ultrasound.Init();
                return;
            }
        }

        //超音波を出し切った場合
        if (ultrasound.IsAlive() == false)
        {
            ultrasound.Init();
            untilLaunch = Time.time;
        }
    }

    private void ActionLeave()
    {
        //翼を広げるアニメーションに変更
        Animator animator = GetComponent<Animator>();
        float _nowTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;

       // Debug.Log(animator.GetCurrentAnimatorStateInfo(0).normalizedTime);

        if (_nowTime <= 0.0f)
        {
            //コウモリ移動処理
            transform.position = Vector3.MoveTowards(transform.position, targetPos, amountChangeDis / 180.0f);

            //コウモリの回転処理
            if (myController.forwardAngle > 20.0f)
            {
                //変化する値
                float _changeAng = (amountChangeAngX / 180.0f);
                myController.forwardAngle -= _changeAng;

                if (myController.forwardAngle <= 20.0f)
                {
                    myController.forwardAngle = 20.0f;
                }
            }
        }

        if (nextAnime == false)
        {
            //羽を閉じるアニメーションに切り替える
            animator.SetInteger("trans", 2);
            nextAnime = true;
        }

        if (Vector3.Distance(targetPos,transform.position) <= 0.001f)
        {
            animator.SetInteger("trans", 0);

            if(animator.GetAnimatorTransitionInfo(0).IsName("FlappingWings"))
            {
                Debug.Log("切り替わっている");
            }
            ////コウモリを追跡ステートに切り替える
            //BatController batCon = gameObject.GetComponent<BatController>();
            //batCon.ChangeState(GetComponent<batMove>());
            //return;
        }
    }

}
