using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    private float moveSpeed;
    [SerializeField] private float normalSpeed = 3; // 移動速度
    [SerializeField] private float dashSpeedRaito = 3;//走る速さの倍率
    [SerializeField] private float jumpPower = 3; // ジャンプ力
    [SerializeField] private float fallSpeed = 3; //落下スピード
    [SerializeField] private float jumpingMoveSpeedRaito = 3; //ジャンプ中の移動速度補正
    [SerializeField] private float landingSpeed = 3; //着地スピード
    [SerializeField] private float airResistRaito = 3; //空気抵抗の変化量
    private float airResist = 0; //今の空気抵抗
    [SerializeField] private float jumpingAirResistRaito = 3; //ジャンプ中の空気抵抗の変化量
    private float jumpingAirResist = 1; //ジャンプ中の今の空気抵抗
    [SerializeField] private float turnSpeed = 20; //振り向く速さ
    private float turnRaito = 180; //振り向く段階

    private CharacterController _characterController; // CharacterControllerのキャッシュ
    private Transform _transform; // Transformのキャッシュ
    private Vector3 _moveVelocity; // キャラの移動速度情報
    private Vector3 _moveVec; // 合成用

    public GameObject cam;
    Quaternion cameraRot, characterRot;
    float Xsensityvity = 3f, Ysensityvity = 3f;

    bool cursorLock = true;

    //変数の宣言(角度の制限用)
    float minX = -60f, maxX = 60f;

    // Start is called before the first frame update
    void Start()
    {
        _characterController = GetComponent<CharacterController>(); // 毎フレームアクセスするので、負荷を下げるためにキャッシュしておく
        _transform = transform; // Transformもキャッシュすると少しだけ負荷が下がる
        cameraRot = cam.transform.localRotation;
        characterRot = transform.localRotation;

        moveSpeed = normalSpeed;
        airResist = landingSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        if (turnRaito >= turnSpeed) //180ターンを使っていなかったら
        {
            float xRot = Input.GetAxis("Mouse X") * Ysensityvity;
            float yRot = Input.GetAxis("Mouse Y") * Xsensityvity;

            cameraRot *= Quaternion.Euler(-yRot, 0, 0);
            characterRot *= Quaternion.Euler(0, xRot, 0);
        }

        //Updateの中で作成した関数を呼ぶ
        cameraRot = ClampRotation(cameraRot);

        MiddleClick();

        cam.transform.localRotation = cameraRot;
        transform.localRotation = characterRot;

        UpdateCursorLock();
    }

    private void FixedUpdate()
    {
        _moveVelocity.x = Input.GetAxis("Horizontal");
        _moveVelocity.z = Input.GetAxis("Vertical");

        IsGroundProcess();

        _moveVec = cam.transform.forward * _moveVelocity.z + cam.transform.right * _moveVelocity.x;
        _moveVec.y = 0;

        _moveVec.Normalize();
        _moveVec *= moveSpeed;
        _moveVec.y = _moveVelocity.y;

        _characterController.Move(_moveVec);

    }

    //--------------------------------------------------------------------
    //自作関数
    //--------------------------------------------------------------------
    public void UpdateCursorLock()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            cursorLock = false;
        }
        else if (Input.GetMouseButton(0))
        {
            cursorLock = true;
        }


        if (cursorLock)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else if (!cursorLock)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }

    public void MiddleClick()
    {
        if (Input.GetMouseButtonDown(2))
        {
            turnRaito = 0;
        }

        if (turnRaito < turnSpeed)
        {
            cameraRot[0] = 0;
            cameraRot[2] = 0;
            turnRaito++;
            cameraRot *= Quaternion.Euler(0, 180 / turnSpeed, 0);
        }
    }

    public void IsGroundProcess()
    {
        if (_characterController.isGrounded)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                // ジャンプ処理
                _moveVelocity.y = jumpPower; // ジャンプの際は上方向に移動させる

                jumpingAirResist = 1.0f;
            }

            if (airResist < 1)
            {
                airResist += airResistRaito;
                if (airResist > 1)
                {
                    airResist = 1.0f;
                }

                moveSpeed *= airResist;
                moveSpeed *= airResist;
            }
        }
        else
        {
            // 重力による加速
            _moveVelocity.y += Physics.gravity.y * fallSpeed;

            moveSpeed *= jumpingMoveSpeedRaito * jumpingAirResist;
            moveSpeed *= jumpingMoveSpeedRaito * jumpingAirResist;

            jumpingAirResist -= jumpingAirResistRaito;

            airResist = landingSpeed;
        }
    }

    //角度制限関数の作成
    public Quaternion ClampRotation(Quaternion q)
    {
        //q = x,y,z,w (x,y,zはベクトル（量と向き）：wはスカラー（座標とは無関係の量）)

        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1f;

        float angleX = Mathf.Atan(q.x) * Mathf.Rad2Deg * 2f;

        angleX = Mathf.Clamp(angleX, minX, maxX);

        q.x = Mathf.Tan(angleX * Mathf.Deg2Rad * 0.5f);

        return q;
    }

    //--------------------------------------------------------------------
    //ゲッターセッター
    //--------------------------------------------------------------------
    public float GetSetMoveSpeed
    {
        get { return moveSpeed; }
        set { moveSpeed = value; }
    }

    public float GetSetNormalSpeed
    {
        get { return normalSpeed; }
        set { normalSpeed = value; }
    }

    public GameObject GetCamera
    {
        get { return cam; }
    }
}
