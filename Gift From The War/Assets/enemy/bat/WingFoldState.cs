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

        //�i�r���b�V���ɂ��I�u�W�F�N�g�̉�]���X�V���Ȃ�
        agent.isStopped = true;
        agent.updateUpAxis = false;
        agent.updateRotation = false;
        agent.updatePosition = false;

        rayPosition = new Vector3(transform.position.x, transform.position.y + myController.hight, transform.position.z);
        Ray ray = new Ray(rayPosition, Vector3.up);

        //Ray����ɔ�΂�
        if (Physics.Raycast(ray, out hit))
        {
            //���C���V��ɏՓ˂��Ă���ꍇ�̓^�[�Q�b�g���W�ɐݒ肷��B
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
        //�̂���]�����鏈��
        if (myController.forwardAngle >= 90)
        {
            transform.eulerAngles = new Vector3(180.0f - myController.forwardAngle, rotateY + 180.0f, 180.0f);
        }
        else
        {
            transform.eulerAngles = new Vector3(myController.forwardAngle, transform.eulerAngles.y, transform.eulerAngles.z);
        }

        //���݂̃A�N�V������Ԗ��Ɋ֐������s����
        switch(nowAction)
        {
            case e_Action.none:
                ActionNone();
                break;
                //����t����Ԃ̎�
            case e_Action.sticking:
                ActionSticking();
                break;
                //������Ԃ̎�
            case e_Action.leave:
                ActionLeave();
                break;
        }

        //�R�E�����̍��W���烏�[���h���W�ŉ��Ɍ����Ă��郌�C�����
        Ray _ray = new Ray(transform.position, Vector3.down);
        RaycastHit _raycastHit;
        bool _rayHit = Physics.Raycast(_ray, out _raycastHit);

        //���C���I�u�W�F�N�g�ɓ������Ă���ꍇ
        if (_rayHit == true)
        {
            //���݂̍������L�^���Ă���
            myController.hight = _raycastHit.distance;
        }
    }

    private void ActionSticking()
    {
        //�^�[�Q�b�g�Ƃ��Ă�����W�܂ł̋����𒲂ׂ�
        float _targetDis = Vector3.Distance(transform.position, targetPos);

        //�V��Ƃ̋����������ł�����Ă���ꍇ�A�܂��͋t���܂ɂȂ��Ă��Ȃ��ꍇ
        if (_targetDis >= 0.001f || myController.forwardAngle < 180.0f)
        {
            //��ɏ㏸���鏈��
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.001f);
            _targetDis = Vector3.Distance(transform.position, targetPos);
        }
        //�V��Ƃ̋������O�ɋ߂��ꍇ
        else
        {
            //�A�N�V������Ԃ��Ȃ��ɂ���
            nowAction = e_Action.none;
        }

        //�V��Ƃ̍������߂��ꍇ
        if (_targetDis <= 0.3f)
        {
            //�R�E������180�x��]���Ă��Ȃ��ꍇ
            if (myController.forwardAngle < 180.0f)
            {
                myController.forwardAngle += 0.3f;

                if (myController.forwardAngle >= 180.0f)
                {
                    myController.forwardAngle = 180.0f;
                }
            }

            if (nextAnime == false)
            {
                //�H�����A�j���[�V�����ɐ؂�ւ���
                Animator animator = GetComponent<Animator>();
                animator.SetInteger("trans", 1);
                nextAnime = true;
            }
        }
    }

    private void ActionNone()
    {
        //�����g�̃N�[���^�C�����I�����Ă���ꍇ
        if (untilLaunch + ultrasoundCoolTime <= Time.time)
        {
            //�����g���X�V
            bool _hit = ultrasound.Update();

            //�����g���v���C���[�ɓ������Ă���ꍇ
            if (_hit == true)
            {
                //�A�N�V������Ԃ�V�䂩�痣����Ԃɕω�
                nowAction = e_Action.leave;
                targetPos += Vector3.down * 0.8f;
                ultrasound.Init();
                return;
            }
        }

        //�����g���o���؂����ꍇ
        if (ultrasound.IsAlive() == false)
        {
            ultrasound.Init();
            untilLaunch = Time.time;
        }
    }

    private void ActionLeave()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPos, 0.005f);

        //�R�E������180�x��]���Ă��Ȃ��ꍇ
        if (myController.forwardAngle > 20.0f)
        {
            myController.forwardAngle -= 2.0f;

            if (myController.forwardAngle <= 20.0f)
            {
                myController.forwardAngle = 20.0f;
            }
        }

        //�A�j���[�V�����؂�ւ�����
        Animator animator = GetComponent<Animator>();
        animator.SetInteger("trans", 2);

        ////�R�E������ǐՃX�e�[�g�ɐ؂�ւ���
        //BatController batCon = gameObject.GetComponent<BatController>();
        //batCon.ChangeState(GetComponent<batMove>());
        //return;
    }

}
