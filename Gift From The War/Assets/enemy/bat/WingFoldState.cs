using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WingFoldState : BaseState
{
    private Vector3 rayPosition;
    private Vector3 targetPos;
    private RaycastHit hit;
    private CharacterController playerCC;
    private NavMeshAgent agent;
    private Vector3 hightManager;

    // Start is called before the first frame update
    public override void Start()
    {
        myController = GetComponent<BatController>();
        agent = GetComponent<NavMeshAgent>();
        playerCC = GameObject.Find("player").GetComponent<CharacterController>();

        //ナビメッシュによるオブジェクトの回転を更新しない
        // agent.updateRotation = false;

        //体を前に傾ける
        Vector3 _localAngle = transform.localEulerAngles;
        _localAngle.x = 20.0f;
        transform.localEulerAngles = _localAngle;


        rayPosition = new Vector3(transform.position.x, myController.hight, transform.position.z);
        Ray ray = new Ray(rayPosition, Vector3.up);

        //Rayを上に飛ばす
        if (Physics.Raycast(ray, out hit))
        {
            //レイが天井に衝突している場合はターゲット座標に設定する。
            Vector3 targetVec = Vector3.up * hit.distance;
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
        //体を前に傾ける
        Vector3 _localAngle = transform.localEulerAngles;
        _localAngle.x += myController.forwardAngle;
        transform.localEulerAngles = _localAngle;

        transform.position = new Vector3(transform.position.x, myController.hight, transform.position.z);

        //上に上昇する処理
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.001f);
        myController.hight = transform.position.y;

        //アニメーション切り替え処理
        //Animator animator = GetComponent<Animator>();

        //int trans = animator.GetInteger("trans");

        //trans++;

        //animator.SetInteger("trans", 1);
    }
}
