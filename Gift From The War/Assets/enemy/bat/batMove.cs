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
        //�̂�O�ɌX����
        Transform _myTrans = this.transform;

        Vector3 _localAngle = _myTrans.localEulerAngles;

        _localAngle.x = forwardAngle;

        _myTrans.eulerAngles = _localAngle;

        //�ړ�����ꍇ
        if (moveFlg)
        {
            Vector3 _playerPos = playerCC.transform.position;
            Vector3 _myPos = this.transform.position;

            //�v���C���[�ɋ߂Â������Ȃ�����
            float dis = Vector3.Distance(_myPos, _playerPos);

            //�v���C���[�Ƃ̋����𒲂ׂ�
            if (dis <= playerFromInterval)
            {
                //�߂Â������Ă���ꍇ
                _agent.destination = _myPos;
                BatController batCon = gameObject.GetComponent<BatController>();

                BaseState state = GetComponent<WingFoldState>();

                batCon.ChangeState(state);
                //�������^�[��
                return;
            }
            else
            {
                //����Ă���ꍇ
                _agent.destination = _playerPos;
            }

        }

        rantanTransform.position += new Vector3(0, hight, 0);
    }
}
