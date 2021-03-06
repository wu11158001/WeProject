using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 玩家控制
/// </summary>
public class PlayerControl : MonoBehaviour
{
    Animator animator;
    CharactersCollision charactersCollision;

    //共通
    float gravity;//重力

    //碰撞框
    Vector3 boxCenter;
    Vector3 boxSize;

    //移動
    bool isLockMove;//是否鎖住移動
    float inputX;//輸入X值
    float inputZ;//輸入Z值
    float moveSpeed;//移動速度
    Vector3 forwardVector;//前方向量
    Vector3 horizontalCross;//水平軸

    //跳躍
    bool isJump;//是否跳躍
    float jumpForce;//跳躍力        

    //普通攻擊
    bool isNormalAttack;//是否普通攻擊
    bool isTrick;//是否使用絕招
    int normalAttackNumber;//普通攻擊編號
    float[] playerNormalAttackDamge;//玩家普通攻擊傷害
    float[] playerNormalAttackMoveDistance;//玩家普通攻擊移動距離
    float[] playerNormalAttackRepelDistance;//玩家普通攻擊 擊退/擊飛距離
    float[] playerNormalAttackRepelDirection;//玩家普通攻擊方向(0:擊退 1:擊飛)
    string[] playerNormalAttackEffect;//玩家普通攻擊效果(受擊者播放的動畫名稱)
    Vector3[] playerNormalAttackBoxSize;//玩家普通攻擊攻擊框Size

    //跳躍攻擊
    bool isJumpAttack;//是否跳躍攻擊
    float playerJumpAttackDamage;//玩家跳躍攻擊傷害
    string playerJumpAttackEffect;//玩家跳躍攻擊效果(受擊者播放的動畫名稱)
    float playerJumpAttackRepelDistance;//玩家跳躍攻擊 擊退距離
    Vector3 playerJumpAttackBoxSize;//玩家跳躍攻擊攻擊框Size

    //技能攻擊
    bool isSkillAttack;//是否技能攻擊
    //技能攻擊_1
    float playerSkillAttack_1_Damage;//技能攻擊_1_攻擊傷害
    string playerSkillAttack_1_Effect;//技能攻擊_1_攻擊效果(受擊者播放的動畫名稱)
    float playerSkillAttack_1_FlyingSpeed;//技能攻擊_1_物件飛行速度
    float playerSkillAttack_1_LifeTime;//技能攻擊_1_生存時間
    float playerSkillAttack_1_Repel;//技能攻擊_1_擊退距離

    //物件池物件
    int playerSkill_1_Number;//玩家技能1_物件編號

    //其他
    LayerMask attackMask;//攻擊對象

    private void Awake()
    {
        gameObject.layer = LayerMask.NameToLayer("Player");//設定Layer                

        animator = GetComponent<Animator>();
        if (GetComponent<CharactersCollision>() == null) gameObject.AddComponent<CharactersCollision>();
        charactersCollision = GetComponent<CharactersCollision>();
    }

    void Start()
    {
        //鼠標
        Cursor.visible = false;//鼠標隱藏
        Cursor.lockState = CursorLockMode.Locked;//鎖定中央

        //碰撞框
        boxCenter = GetComponent<BoxCollider>().center;
        boxSize = GetComponent<BoxCollider>().size;

        //共通
        gravity = GameData.Instance.OnGetFloatValue("gravity");//重力

        //移動
        forwardVector = transform.forward;
        moveSpeed = GameData.Instance.OnGetFloatValue("playerMoveSpeed");//移動速度
        jumpForce = GameData.Instance.OnGetFloatValue("playerJumpForce");//跳躍力

        //普通攻擊
        playerNormalAttackDamge = GameData.Instance.OnGetFloatArrayValue("playerNormalAttackDamge");//玩家普通攻擊傷害
        playerNormalAttackMoveDistance = GameData.Instance.OnGetFloatArrayValue("playerNormalAttackMoveDistance");//玩家普通攻擊移動距離
        playerNormalAttackRepelDistance = GameData.Instance.OnGetFloatArrayValue("playerNormalAttackRepelDistance");//玩家普通攻擊 擊退/擊飛距離
        playerNormalAttackRepelDirection = GameData.Instance.OnGetFloatArrayValue("playerNormalAttackRepelDirection");//玩家普通攻擊 擊退/擊飛距離
        playerNormalAttackEffect = GameData.Instance.OnGetStringArrayValue("playerNormalAttackEffect");//玩家普通攻擊效果(受擊者播放的動畫名稱)
        playerNormalAttackBoxSize = GameData.Instance.OnGetVectorArrayValue("playerNormalAttackBoxSize");//玩家普通攻擊攻擊框Size

        //跳躍攻擊
        playerJumpAttackDamage = GameData.Instance.OnGetFloatValue("playerJumpAttackDamage");//玩家跳躍攻擊傷害
        playerJumpAttackEffect = GameData.Instance.OnGetStringValue("playerJumpAttackEffect");//玩家跳躍攻擊效果(受擊者播放的動畫名稱)
        playerJumpAttackRepelDistance = GameData.Instance.OnGetFloatValue("playerJumpAttackRepelDistance");//玩家跳躍攻擊 擊退距離
        playerJumpAttackBoxSize = GameData.Instance.OnGetVectorValue("playerJumpAttackBoxSize");//玩家跳躍攻擊攻擊框Size

        //技能攻擊_1
        playerSkillAttack_1_Damage = GameData.Instance.OnGetFloatValue("playerSkillAttack_1_Damage");//技能攻擊_1_攻擊傷害
        playerSkillAttack_1_Effect = GameData.Instance.OnGetStringValue("playerSkillAttack_1_Effect");//技能攻擊_1_攻擊效果(受擊者播放的動畫名稱)
        playerSkillAttack_1_FlyingSpeed = GameData.Instance.OnGetFloatValue("playerSkillAttack_1_FlyingSpeed");//技能攻擊_1_物件飛行速度
        playerSkillAttack_1_LifeTime = GameData.Instance.OnGetFloatValue("playerSkillAttack_1_LifeTime");//技能攻擊_1_生存時間
        playerSkillAttack_1_Repel = GameData.Instance.OnGetFloatValue("playerSkillAttack_1_Repel");//技能攻擊_1_擊退距離

        //物件池物件
        playerSkill_1_Number = GameManagement.Instance.OnGetObjectNumber("playerSkill_1_Number");//玩家技能1_物件編號

        //其他
        attackMask = LayerMask.GetMask("Enemy");//攻擊對象
    }
   
    void Update()
    {        
        OnInput();
        OnJumpControl();
        OnAttackControl();
        OnJumpBehavior();

        if (!isNormalAttack && !isSkillAttack && !isTrick)
        {
            OnMovementControl();            
        }
    }

    /// <summary>
    /// 技能攻擊行為
    /// </summary>
    void OnSkillAttackBehavior()
    {
        //判斷目前普通攻擊編號
        switch(normalAttackNumber)
        {
            case 1://技能1
                GameObject obj = GameManagement.Instance.OnRequestOpenObject(playerSkill_1_Number);
                obj.transform.position = transform.position + boxCenter;
                GameManagement.Instance.flyingAttackObject_List.Add(new FlyingAttackObject
                {
                    flyingObject = obj,//飛行物件 
                    speed = playerSkillAttack_1_FlyingSpeed,//飛行速度
                    diration = transform.forward,//飛行方向
                    lifeTime = playerSkillAttack_1_LifeTime,//生存時間                                                                                             
                    layer = gameObject.layer,//攻擊者layer
                    damage = playerSkillAttack_1_Damage,//造成傷害
                    animationName = playerSkillAttack_1_Effect,//攻擊效果(受擊者播放的動畫名稱)
                    repel = playerSkillAttack_1_Repel//擊退距離
                });
                break;
        }
    }
 
    /// <summary>
    /// 跳躍攻擊行為
    /// </summary>
    void OnJumpAttackBehavior()
    {
        //攻擊框        
        Collider[] hits = Physics.OverlapBox(transform.position + boxCenter + transform.forward, playerJumpAttackBoxSize, Quaternion.identity, attackMask);
        foreach (var hit in hits)
        {
            CharactersCollision collision = hit.GetComponent<CharactersCollision>();
            if (collision != null)
            {
                collision.OnGetHit(attacker: gameObject,//攻擊者物件
                                   layer: gameObject.layer,//攻擊者layer
                                   damage: playerJumpAttackDamage,//造成傷害
                                   animationName: playerJumpAttackEffect,//播放動畫名稱
                                   effect: 0,//擊中效果(0:擊退, 1:擊飛)
                                   repel: playerJumpAttackRepelDistance);//擊退距離
            }
        }
    }

    /// <summary>
    /// 普通攻擊行為
    /// </summary>
    void OnNormalAttackBehavior()
    {
        //攻擊移動
        transform.position = transform.position + transform.forward * playerNormalAttackMoveDistance[normalAttackNumber - 1] * Time.deltaTime;

        //攻擊框
        Collider[] hits = Physics.OverlapBox(transform.position + boxCenter + transform.forward, playerNormalAttackBoxSize[normalAttackNumber - 1], Quaternion.identity, attackMask);
        foreach(var hit in hits)
        {         
            CharactersCollision collision = hit.GetComponent<CharactersCollision>();
            if (collision != null)
            {
                collision.OnGetHit(attacker: gameObject,//攻擊者物件
                                   layer: gameObject.layer,//攻擊者layer
                                   damage: playerNormalAttackDamge[normalAttackNumber - 1],//造成傷害 
                                   animationName: playerNormalAttackEffect[normalAttackNumber - 1],//播放動畫名稱
                                   effect: (int)playerNormalAttackRepelDirection[normalAttackNumber - 1],//擊中效果(0:擊退, 1:擊飛)
                                   repel: playerNormalAttackRepelDistance[normalAttackNumber - 1]);//擊退距離
            }
        }
    }    

    /// <summary>
    /// 攻擊控制
    /// </summary>
    void OnAttackControl()
    {
        AnimatorStateInfo info = animator.GetCurrentAnimatorStateInfo(0);

        //普通攻擊
        if (Input.GetMouseButton(0) && !info.IsTag("SkillAttack") && !isTrick)
        {
            //技能攻擊
            if (Input.GetMouseButtonDown(1))
            {
                //普通攻擊時間內按下
                if (info.IsTag("NormalAttack") && info.normalizedTime < 1)
                {                                       
                    isSkillAttack = true;                    
                }
            }

            //等待普通攻擊結束再執行技能
            if (isSkillAttack && info.IsTag("NormalAttack") && info.normalizedTime >= 1)
            {
                //轉向               
                if (inputX != 0 && inputZ != 0) transform.forward = (horizontalCross * inputX) + (forwardVector * inputZ);//斜邊
                else if (inputX != 0) transform.forward = horizontalCross * inputX;//左右
                else if (inputZ != 0) transform.forward = forwardVector * inputZ;//前後

                animator.SetBool("SkillAttack", isSkillAttack);
                return;
            }            

            //跳躍攻擊
            if (isJump)
            {
                isJumpAttack = true;
                animator.SetBool("JumpAttack", isJumpAttack);
                return;
            }

            //普通攻擊(第一次攻擊)
            if (!isSkillAttack && !isNormalAttack)
            {
                isNormalAttack = true;
                normalAttackNumber = 1;
                animator.SetBool("NormalAttack", isNormalAttack);              
            }

            //切換普通攻擊招式
            if (info.IsTag("NormalAttack") && info.normalizedTime >= 1)
            {                                
                normalAttackNumber++;//普通攻擊編號                
                if (normalAttackNumber > 3) normalAttackNumber = 0;
              
                //轉向               
                if (inputX != 0 && inputZ != 0) transform.forward = (horizontalCross * inputX) + (forwardVector * inputZ);//斜邊
                else if (inputX != 0) transform.forward = horizontalCross * inputX;//左右
                else if (inputZ != 0) transform.forward = forwardVector * inputZ;//前後
                animator.SetInteger("NormalAttackNumber", normalAttackNumber);
            }          
        }       
        else
        {
            //動畫/攻擊結束
            if (info.normalizedTime >= 1)
            {                                
                if (info.IsTag("NormalAttack") || info.IsTag("SkillAttack") || info.IsTag("JumpAttack"))
                {                    
                    normalAttackNumber = 0;//普通攻擊編號
                    animator.SetInteger("NormalAttackNumber", normalAttackNumber);

                    isNormalAttack = false;
                    animator.SetBool("NormalAttack", isNormalAttack);

                    isSkillAttack = false;
                    animator.SetBool("SkillAttack", isSkillAttack);

                    isJumpAttack = false;
                    animator.SetBool("JumpAttack", isJumpAttack);
                }              
            }
        }     

        //技能攻擊中關閉普通攻擊
        if(info.IsTag("SkillAttack") && isNormalAttack)
        {
            isNormalAttack = false;
            animator.SetBool("NormalAttack", isNormalAttack);
        }
        
        //絕招
        if(Input.GetKeyDown(KeyCode.R))
        {
            if (!isJump)
            {
                isNormalAttack = false;
                isTrick = true;
                animator.SetBool("Trick", isTrick);
                animator.SetBool("NormalAttack", isNormalAttack);
            }
        }

        if(isTrick)//轉向
        {
            //轉向               
            if (inputX != 0 && inputZ != 0) transform.forward = (horizontalCross * inputX) + (forwardVector * inputZ);//斜邊
            else if (inputX != 0) transform.forward = horizontalCross * inputX;//左右
            else if (inputZ != 0) transform.forward = forwardVector * inputZ;//前後
        }

        //絕招結束
        if(info.IsTag("Trick") && info.normalizedTime > 1)
        {
            isTrick = false;
            animator.SetBool("Trick", isTrick);
        }        
    }      
    
    /// <summary>
    /// 跳躍行為
    /// </summary>
    void OnJumpBehavior()
    {       
        if (isJump) StartCoroutine(OnWaitJump());       
    }

    /// <summary>
    /// 等待跳躍(避免無法觸發動畫)
    /// </summary>
    /// <returns></returns>
    IEnumerator OnWaitJump()
    {
        yield return new WaitForSeconds(0.1f);

        //碰撞偵測
        LayerMask mask = LayerMask.GetMask("StageObject");
        if (Physics.CheckBox(transform.position + boxCenter, new Vector3(boxSize.x / 4, boxSize.y / 2, boxSize.z / 4), Quaternion.identity, mask))
        {
            isLockMove = false;
            isJump = false;
            animator.SetBool("Jump", isJump);
            animator.SetBool("JumpAttack", isJump);
        }
    }

    /// <summary>
    /// 跳躍控制
    /// </summary>
    void OnJumpControl()
    {
        if(Input.GetKeyDown(KeyCode.Space) && !isJump)
        {
            charactersCollision.floating_List.Add(new CharactersFloating { target = transform, force = jumpForce, gravity = gravity });//浮空List

            isJump = true;         
            isNormalAttack = false;
            animator.SetBool("NormalAttack", isNormalAttack);
            animator.SetBool("Jump", isJump);
        }        
    }

    /// <summary>
    /// 移動控制
    /// </summary>
    void OnMovementControl()
    {                
        //轉向
        float maxRadiansDelta = 0.025f;
        if (inputX != 0 && inputZ != 0) transform.forward = Vector3.RotateTowards(transform.forward, (horizontalCross * inputX) + (forwardVector * inputZ), maxRadiansDelta, maxRadiansDelta);//斜邊
        else if (inputX != 0) transform.forward = Vector3.RotateTowards(transform.forward, horizontalCross * inputX, maxRadiansDelta, maxRadiansDelta);//左右
        else if (inputZ != 0) transform.forward = Vector3.RotateTowards(transform.forward, forwardVector * inputZ, maxRadiansDelta, maxRadiansDelta);//前後      
        
        float inputValue = Mathf.Abs(inputX) + Mathf.Abs(inputZ);//輸入值

        //跳躍中不可再增加推力
        if (isJump && inputValue == 0) isLockMove = true;       
        if (isLockMove) inputValue = 0;

        //移動
        if (inputValue > 1) inputValue = 1;
        transform.position = transform.position + transform.forward * inputValue * moveSpeed * Time.deltaTime;

        animator.SetFloat("Run", inputValue);
    }           

    /// <summary>
    /// 輸入值
    /// </summary>
    void OnInput()
    {
        inputX = Input.GetAxis("Horizontal");//輸入X值
        inputZ = Input.GetAxis("Vertical");//輸入Z值

        forwardVector = Quaternion.AngleAxis(Input.GetAxis("Mouse X"), Vector3.up) * forwardVector;//前方向量
        horizontalCross = Vector3.Cross(Vector3.up, forwardVector);//水平軸       

        //滑鼠
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Cursor.visible = !Cursor.visible;//鼠標 顯示/隱藏
            if (!Cursor.visible) Cursor.lockState = CursorLockMode.Locked;//鎖定中央
            else Cursor.lockState = CursorLockMode.None;
        }
    }

   /* private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(transform.position + boxCenter, playerJumpAttackBoxSize);
    }*/
}
