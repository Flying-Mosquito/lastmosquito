using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using DG.Tweening;

// 해야할 일 : 기울기에 따라 플레이어가 너무 움직인다 - 조정필요 
// 2. 기본 가속도? 



public class Player : GameManager {

    public Animator anim;


    public RectTransform healthTransform;
    private float cachedY;
    private float minXvalue;
    private float maxXvalue;
    private int currentHealth;
    public int maxHealth;
    public Image visualHealth;
    public Text healthText;

    public RectTransform staminaTransform;
    private float cachedY1;
    private float minXvalue1;
    private float maxXvalue1;
    private int currentstamina;
    public int maxstamina;
    public Image visuastamina;

    public RectTransform bloodTransform;
    private float cachedY2;
    private float minXvalue2;
    private float maxXvalue2;
    private int currentblood;
    public int maxblood;
    public Image visuablood;





    //private CharacterController controller;
    private Transform tr;
    private Transform[] tr_Mesh;
    public Transform targetPlus;
    public Transform enemy;
    private Rigidbody rigidBody;
    Vector3 movement;
   // public FollowCam _Camera;

    // 나중에 상수로 다 처리해줘야할것들 
    const ulong ST_EDLE = 0x00000001;
    const ulong ST_BOOST = 0x00000002;
    const ulong ST_ACTIVE = 0x00000004;
    const ulong ST_WALLCOLLISION = 0x00000008;
    const ulong ST_CLING = 0x00000010;
    private ulong State = ST_EDLE;

   
    private float fStamina; // 스테미나 ( 미구현)  
    public float fSpeed;// { get; private set; }
    public float fOwnSpeed { get; private set; }
    public float fRotSpeed { get; private set; }
    public float fOwnRotSpeed { get; private set; }
    public float fBoostVal { get; private set; }
    public float fStaminaMinus { get; private set; }
    public float fBoostMax { get; private set; }
    public float fXAngle;      // 좌우 회전
    public float fYAngle;      // 위아래 회전

    private bool bCheckBoost;
    private bool bRotation;
    public  bool bWallCollision;
    public  bool isBoost { get; private set; }
    public  bool isCling;       // 벽에 붙어있는지의 상태 
    public bool bCling;         // 벽에 붙을지 안붙을지의 상태
    public bool isConfused { get; private set; }



    public float coolDown;
    private bool onCD;
    public GUIText restartText;
    public float coolDown1;
    private bool onCD1;

    public float coolDown2;
    private bool onCD2;
    public GameObject ClingObj;
    private int CurrentHealth
    {
        get { return currentHealth; }
        set
        {
            currentHealth = value;
            HandleHealth();
        }
    }
    private int Currentstamina
    {
        get { return currentstamina; }
        set
        {
            currentstamina = value;
            Handlestamina();
        }
    }
    private int Currentblood
    {
        get { return currentblood; }
        set
        {
            currentblood = value;
            Handleblood();
        }
    }
    //private float fYRotation_Ani;

    //   private Vector3 vFirstAngle;
    
    void Start()
    {
        anim = GetComponent<Animator>();

        cachedY = healthTransform.position.y;
        maxXvalue = healthTransform.position.x;
        minXvalue = healthTransform.position.x - healthTransform.rect.width;
        currentHealth = maxHealth;
        onCD = false;
        cachedY1 = staminaTransform.position.y;
        maxXvalue1 = staminaTransform.position.x;
        minXvalue1 =staminaTransform.position.x - staminaTransform.rect.width;
        currentstamina = maxstamina;
        onCD1 = false;
        cachedY2 = bloodTransform.position.y;
        maxXvalue2 = bloodTransform.position.x;
        minXvalue2 = bloodTransform.position.x - bloodTransform.rect.width;
        currentblood = maxblood;
        onCD2 = false;

        

    }
    IEnumerator CoolDownDmg()
    {
        onCD = true;
        yield return new WaitForSeconds(coolDown);
        onCD = false;
       
    }

    IEnumerator CoolDownDmg1()
    {
        onCD1 = true;
        yield return new WaitForSeconds(coolDown1);
        onCD1 = false;

    }

    IEnumerator CoolDownDmg2()
    {
        onCD2 = true;
        yield return new WaitForSeconds(coolDown2);
        onCD2 = false;

    }

    
    private void HandleHealth()
    {
        
        float currentXvalue = MapValues(currentHealth, 0, maxHealth, minXvalue, maxXvalue);

        healthTransform.position = new Vector3(currentXvalue, cachedY);


    }
    private void Handlestamina()
    {

        float currentXvalue1 = MapValues1(currentstamina, 0, maxstamina, minXvalue1, maxXvalue1);

        staminaTransform.position = new Vector3(currentXvalue1, cachedY1);


    }

    private void Handleblood()
    {

        float currentXvalue2 = MapValues2(currentblood, 0, maxblood, minXvalue2, maxXvalue2);

        bloodTransform.position = new Vector3(currentXvalue2, cachedY2);


    }

    private float MapValues(float x, float inMin,float inMax,float outMin,float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    }

    private float MapValues1(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    }
    private float MapValues2(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;

    }

    void Awake()
    {
        //임시
        // CollisionMgr = GetComponent<CollisionManager>();
        ClingObj = GameObject.Find("ClingObject");
         tr = GetComponent<Transform>();
        tr_Mesh = GetComponentsInChildren<Transform>();
        rigidBody = GetComponent<Rigidbody>();
       // _Camera = GetComponentInChildren<FollowCam>();

        // controller = GetComponent<CharacterController>();
        //---------------
        bCheckBoost = true;    // 스테미나 부스트가 일정량이상 차있어야 사용 가능하게 하는 함수 
        bWallCollision = false;
        bCling = false;
        isCling = false;
        //--------------- // 나중에 구조체로 묶을것
        fStamina = 100f;
        fXAngle = 0f;
        fYAngle = 0f;

        fSpeed = 3.0f;
        fOwnSpeed = 3.0f;
        fRotSpeed = 55f;
        fOwnRotSpeed = 55f;

        fBoostVal = 0f;
        fBoostMax = 10f;
        fStaminaMinus = 0.7f;
        
    }

    void FixedUpdate()
    {
        KeyInput();
        Move();
    }

    void Update() {
      
        Action();
        RotateAnimation();  // 플레이어 몸체 회전효과

        if (CurrentHealth < 10)
        {
            Application.LoadLevel(4);
        }
        if (!onCD && currentstamina < maxstamina)
        {
            StartCoroutine(CoolDownDmg1());
            Currentstamina += 1;
          
        }
        if (bCling ==true)
        {
            if (!onCD1 && currentblood > 0)
            {
              
                StartCoroutine(CoolDownDmg2());
                Currentblood -= 1;
                anim.Play("blooding");
            }

        }
        

    }

    void OnTriggerStay(Collider other)
    {
        if (other.name == "Health")
        {
            if (!onCD && currentHealth < maxHealth)
            {
                StartCoroutine(CoolDownDmg());
                CurrentHealth += 1;
            }
           
        }
        if (other.name == "Damage")
        {
            if (!onCD && currentHealth > 0)
            {
                StartCoroutine(CoolDownDmg());
                CurrentHealth -= 1;
            }
           
        }
    }



    private void KeyInput()
    {
        State = ST_EDLE;
        /*
        if (Input.GetMouseButton(0))
            Boost(true);
        else
            Boost(false);
        if (Input.GetMouseButtonUp(0))
        {
            if (fStamina < 10)
                bCheckBoost = false;
        }
        */
        if (Input.GetMouseButton(0))    // 마우스왼쪽 클릭
        {
            isCling = false;
            State |= ST_BOOST;
            if (!onCD1 && currentstamina > 0)
            {
                StartCoroutine(CoolDownDmg1());
                Currentstamina -= 2;
            }

            //  Debug.Log("플레이어가  흔들흔들");
            //  tr.DOShakePosition(10f, new Vector3(2,0,0), 20);
        }
        // else      // 안해도 State에 아무것도 안들어가니까
        //    Boost(false);
        if (Input.GetMouseButtonUp(0))  // 마우스 왼쪽 뗄 때 
        {

            if (fStamina < 10)
                bCheckBoost = false;



        }


        Debug.DrawRay(tr.transform.position, tr.transform.forward * 3f, Color.yellow);
        if (Input.GetMouseButtonDown(1))   // 마우스 오른쪽 클릭할때 
        {
         
            if (CollisionManager.Instance.Check_RayHit(tr.position, tr.forward, "WALL", 3f))  // 벽에 붙을지 체크 
            {

                bCling = true;

                State |= ST_CLING;
            }

            else if (CollisionManager.Instance.Check_RayHit(tr.position, tr.forward, "human", 3f))
            {

                if (!onCD1 && currentblood > 0)
                {
                    StartCoroutine(CoolDownDmg2());
                    Currentblood -= 1;
                }

                isCling = false;
                bCling = true;

                State |= ST_BOOST;
            }

        }

        if (Input.GetMouseButtonUp(1))  // 마우스 오른쪽 뗄 때 
        {
            Debug.Log("뗐다 ");
            tr.transform.parent = null;
            tr.transform.localScale = new Vector3(1, 1, 1);
            ClingObj.transform.parent = null;

            bCling = false;
            isCling = false;
        }
    }


    private void Action()
    {
        ulong mask = 0;

        // Boost인지 아닌지 체크 
        mask = (State & ST_BOOST);
        if ((mask) > 0)
            Boost(true);
        else
            Boost(false);

        mask = (State & ST_WALLCOLLISION);
       // if ((mask) > 0)
        //   StartCoroutine()

            

    }
    private void Boost(bool _bool)
    {
        //Debug.Log("_bool : " + _bool.ToString() + ", "+ "bCheckBoost : " + bCheckBoost.ToString()  );

        if ((_bool) && (fStamina > fStaminaMinus) && (bCheckBoost)) // _bool 이 true 일때 - 마우스가 클릭상태일때 이면서 , 현재 스테가 스테미나 감소량보다 크고, Boost가 가능할 때 
        {
            fStamina -= fStaminaMinus;
            isBoost = true;
            // 가속도 조절
            fBoostVal += 50f * Time.deltaTime;
            if (fBoostVal > fBoostMax)
                fBoostVal = fBoostMax;

            fSpeed = fBoostVal + fOwnSpeed;
            if (fStamina < 1)               // 스테미나가 1 이하로 떨어지면 부스터를 사용할 수 없다 
            {
                fStamina = 1;
                bCheckBoost = false;
            }
        }
        else
        {
            fStamina += 0.1f;
            isBoost = false;
            // 가속도 조절
            fBoostVal -= 80f * Time.deltaTime;
            if (fBoostVal < 1)
                fBoostVal = 1f;

            if (fStamina > 100)
                fStamina = 100;

            fSpeed = fBoostVal + fOwnSpeed;

            if (fStamina > 10f)      // 스테미나가 10이상이면 사용가능
                bCheckBoost = true;
        }

    
       //Debug.Log("fBoostVal = " + fBoostVal.ToString() + ", " + "fSpeed = " + fSpeed.ToString() + ", " + "fStamina = " + fStamina.ToString());

    }

    private void Move()     // 일단은 키보드 움직임에 따라서 각도가 변하고, 앞으로 가는것은 자동 
    {
        //# if UNITY_IOS
        //fXAngle = Input.acceleration.x;      // fYRotation : 좌우 각도 변경  
       // fYAngle = -Input.acceleration.y - 0.4f;    // fXRotatino : 상하 각도 변경 , 0.4 는 각도 좀더 세울수 있게 마이너스 한것      
//  #else
      fXAngle = Input.GetAxis("Horizontal");
      fYAngle = Input.GetAxis("Vertical");
        //#endif

        //  tr.Rotate((Vector3.up * fYRotation * Time.deltaTime * fRotSpeed) + (tr.right * -fXRotation * Time.deltaTime * fRotSpeed) , Space.World);
        if (!isCling)
        {
            tr.Rotate(Vector3.up * fXAngle * Time.deltaTime * fRotSpeed, Space.World);
            tr.Rotate(Vector3.right * -fYAngle * Time.deltaTime * fRotSpeed, Space.Self);
        }
        else
        {
           // tr.Rotate(Vector3.up * fXAngle * Time.deltaTime * fRotSpeed, Space.Self);
           // tr.Rotate(Vector3.right * -fYAngle * Time.deltaTime * fRotSpeed, Space.Self);
        }


        // tr.Rotate(tr.right * -fXRotation * Time.deltaTime * fRotSpeed);

        // Debug.DrawRay(tr.transform.position, tr.transform.forward * 100.0f, Color.blue);

        // tr.Translate(Vector3.forward * Time.deltaTime * fSpeed, Space.Self); // Vector3 Dir1 = tr.forward * 0.5f;  tr.Translate( Dir1 * Time.deltaTime * fSpeed); // 이것과 같음..

        // 뒤에 Space.Self 가 빠져 있어서 내가 생각한대로 안됐었던 것이었다고 한다... ㅠㅠㅠ

        if( !isCling)
        {
            movement.Set(tr.forward.x, tr.forward.y, tr.forward.z);
            rigidBody.MovePosition(tr.position + (movement * fSpeed * Time.deltaTime));
       }
        // if ( !isCling)   // 벽에 붙어있지 않다면 앞으로 계속해서 움직인다.
        //    tr.Translate(Vector3.forward * Time.deltaTime * fSpeed, Space.Self);//controller.Move(tr.forward * Time.deltaTime * fSpeed);
        //  Debug.Log("속도 : " + fSpeed.ToString());
        //    Debug.DrawRay(tr.transform.position, tr.forward * 100.0f, Color.blue);

       // Debug.Log("Player isConfuse : " + isConfused.ToString() + " / Player Speed : " + fSpeed.ToString() + "/ OwnSpeed : " + fOwnSpeed.ToString());
 
        Debug.DrawRay(tr.transform.position, Vector3.up * 100.0f, Color.red);
        // 내가짰던 코드 1
    }


    private void RotateAnimation()
    {
       /*  for (int i = 1; i < tr_Mesh.Length; ++i)
             {
              tr_Mesh[i].localRotation = Quaternion.Euler((Vector3.up * fXAngle * 20.0f)
                                                   + (Vector3.right * -fYAngle * 20.0f));
            // tr_Mesh[i].localRotation = Quaternion.Euler(-fYAngle * 20.0f, 0f
             //                                      , (fXAngle * 20.0f));
             }
    */
   // tr_Mesh[1].localRotation = Quaternion.Euler((Vector3.up * fXAngle * 20.0f)
                  //                                 + (Vector3.right * -fYAngle * 20.0f));
                  tr_Mesh[1].localRotation = Quaternion.Euler(-fYAngle * 20.0f, 0f
                                                  , (-fXAngle * 20.0f));
    }

    void OnCollisionEnter(Collision coll)
    {
       // Debug.Log("충돌함");
        if (coll.gameObject.tag == "WALL")
        {
            if(bCling)
            {
                Debug.Log(" 붙었습니다 ");
                isCling = true;

               
                   ClingObj.transform.parent = coll.transform;
                   tr.transform.parent = ClingObj.transform;
                
                /*
                GameObject emptyObject = new GameObject();
                emptyObject.transform.parent = coll.transform;
                tr.transform.parent = emptyObject.transform;
                */
                
               // tempObj.transform.parent = coll.transform;
              //  tempObj = this as GameObject;

                // GameObject gameObj = GameObject 

                /* Vector3 vRate = new Vector3(tr.transform.localScale.x / coll.gameObject.transform.localScale.x
                                             , tr.transform.localScale.y / coll.gameObject.transform.localScale.y
                                             , tr.transform.localScale.z / coll.gameObject.transform.localScale.z);*/
                // tr.transform.localScale = vRate;
                

                //  tr.transform.localScale = new Vector3(0, 0, 0);
                // tr.transform.localPosition = tr.transform.position - coll.gameObject.transform.position;
            }
            else
            {
               
                if (fSpeed > fOwnSpeed + 3f) // _fPower > fPower 도 포함시켜야함 - confused가 되는 상황?
                    StartCoroutine("StartConfused");
                  //   _Camera.SendMessage("Shake_Camera", 5f);
                // _Camera.StartCoroutine("Shake_Camera");
               
            }
            
            
        }
         if  (coll.gameObject.tag == "human")
            {
                if (bCling)
                {
                    Debug.Log(" 붙었습니다 ");
                    isCling = true;


                    ClingObj.transform.parent = coll.transform;
                    tr.transform.parent = ClingObj.transform;

                    /*
                    GameObject emptyObject = new GameObject();
                    emptyObject.transform.parent = coll.transform;
                    tr.transform.parent = emptyObject.transform;
                    */

                    // tempObj.transform.parent = coll.transform;
                    //  tempObj = this as GameObject;

                    // GameObject gameObj = GameObject 

                    /* Vector3 vRate = new Vector3(tr.transform.localScale.x / coll.gameObject.transform.localScale.x
                                                 , tr.transform.localScale.y / coll.gameObject.transform.localScale.y
                                                 , tr.transform.localScale.z / coll.gameObject.transform.localScale.z);*/
                    // tr.transform.localScale = vRate;


                    //  tr.transform.localScale = new Vector3(0, 0, 0);
                    // tr.transform.localPosition = tr.transform.position - coll.gameObject.transform.position;
                }
                else
                {

                    if (fSpeed > fOwnSpeed + 3f) // _fPower > fPower 도 포함시켜야함 - confused가 되는 상황?
                        StartCoroutine("StartConfused");
                    //   _Camera.SendMessage("Shake_Camera", 5f);
                    // _Camera.StartCoroutine("Shake_Camera");

                }


            }
    }

    private IEnumerator StartConfused() // 캐릭터의 상태를 3초간 confused로 바꿔주는 함수
    {
        isConfused = true;
        yield return new WaitForSeconds(3f);        

        isConfused = false;
    }
}
