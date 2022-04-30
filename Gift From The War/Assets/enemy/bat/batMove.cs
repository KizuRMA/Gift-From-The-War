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
        //�̂�O�ɌX����
        Vector3 _localAngle = transform.localEulerAngles;
        _localAngle.x = myController.forwardAngle;
        transform.localEulerAngles = _localAngle;

        //�ړ�����ꍇ
        if (moveFlg)
        {
            Vector3 _playerPos = playerCC.transform.position;
            Vector3 _myPos = transform.position;

            //�v���C���[�ɋ߂Â������Ȃ�����
            float dis = Vector3.Distance(_myPos, _playerPos);

            //�v���C���[�Ƃ̋����𒲂ׂ�
            if (dis <= playerFromInterval)
            {
                //�߂Â������Ă���ꍇ
                _agent.destination = _myPos;
                BatController batCon = gameObject.GetComponent<BatController>();

                batCon.ChangeState(GetComponent<WingFoldState>());
                //�������^�[��
                return;
            }
            else
            {
                //����Ă���ꍇ
                _agent.destination = _playerPos;
            }

        }

        transform.position = new Vector3(transform.position.x, myController.hight, transform.position.z);
    }
}
