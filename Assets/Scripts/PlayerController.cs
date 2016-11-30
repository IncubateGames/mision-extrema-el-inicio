using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float MaxSpeed = 2f;
    [SerializeField]
    private float MaxCrawSpeed = 2f;
    [SerializeField]
    public LayerMask ground_layers;
    [SerializeField]
    public float radius = 0.05f;
    [SerializeField]
    public float jumpForce = 350f;

    private Rigidbody2D _body = null;
    private Animator _anim;
    private bool m_FacingRight = true;
    private bool isEspecial = false;
    private bool isEspecial1 = false;    

    bool isFire = false;
    bool isJump = false;    
    bool isDown = false;
    bool isUp = false;
    bool isLeft = false;
    bool isRight = false;
    bool isRun = false;
    float OnAir = 0.0f;
    public bool isGround = false;
    float move = 0.0f;
    float velx = 0.0f;
    float vely = 0.0f;

    void Awake()
    {
        _body = GetComponentInChildren<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
    }

	// Use this for initialization
	void Start () {	
	}
	
	// Update is called once per frame
	void Update ()
    {
        Move();
        SetAnimatorState();
    }

    void LateUpdate()
    {
        
    }

    /// <summary>
    /// 
    /// </summary>
    private void SetAnimatorState()
    {
        _anim.SetBool("isGround", isGround);
        _anim.SetBool("isRun", isRun);
        _anim.SetFloat("VelX", velx);
        _anim.SetFloat("VelY", vely);
        _anim.SetFloat("OnAir", OnAir);
        _anim.SetBool("isJump", isJump);        
        _anim.SetBool("isDown", isDown);
        _anim.SetBool("isFire", isFire);
    }

    void FixedUpdate()
    {
        CheckGround();
        ApplyPhysics();
    }

    //Function which draws gizmos, showing the computer's calculations in the form of a sphere.
    void OnDrawGizmos()
    {
        //Color of gizmos is blue.
        Gizmos.color = Color.blue;
        //Gizmos is to draw a wire sphere using the grounder.transform's position, and the radius value.
        Vector2 posY = _body != null ? _body.transform.position : transform.position;
        Gizmos.DrawWireSphere(posY, radius);
    }

    public void CheckGround()
    {
        if (!isGround)
        {
            isGround = Physics2D.OverlapCircle(_body.transform.position, radius, ground_layers);
            OnAir = _body.velocity.y;
        }
    }

    public void ApplyPhysics()
    {
        if (isRun)
        {
            //if (velx > 0.9f)
            //{
            //    //if (isDown)
            //    //{
            //    //    _body.velocity = new Vector2(move * MaxCrawSpeed, _body.velocity.y);
            //    //}
            //    //else
            //    //{
            //    _body.velocity = new Vector2(move * MaxSpeed, _body.velocity.y);               
            //    //}
            //}
            _body.velocity = new Vector2(move * MaxSpeed, _body.velocity.y);
        }

        if (isGround)
        {
            if (isJump)
            {
                isJump = false;
                isGround = false;
                _body.AddForce(Vector3.up * 5,ForceMode2D.Impulse);
            }
        }
        else
        {
            OnAir = _body.velocity.y;
        }
    }

    private void Move()
    {
        isFire = CrossPlatformInputManager.GetButton("Fire"); //F
        bool especial = CrossPlatformInputManager.GetButton("Especial"); //C

        isUp = CrossPlatformInputManager.GetButton("UP"); //W
        isDown = CrossPlatformInputManager.GetButton("DOWN"); //S
        isLeft = CrossPlatformInputManager.GetButton("LEFT"); //A
        isRight = CrossPlatformInputManager.GetButton("RIGHT"); //D

        float h = CrossPlatformInputManager.GetAxis("Horizontal"); //A <-- y --> D                
        float v = CrossPlatformInputManager.GetAxis("Vertical");//W (up) y S (down)

        move = h;
        velx = Mathf.Abs(move);
        isRun = velx > 0.4f;        
        vely = v;
        
        if (isGround)
        {
            if (isUp)
            {
                isJump = true;
            }
           
            if (move > 0 && !m_FacingRight)
            {
                Flip();
                m_FacingRight = true;
            }
            else if (move < 0 && m_FacingRight)
            {
                Flip();
                m_FacingRight = false;
            }

            if (especial)
            {
                if (!isEspecial)
                {
                    isEspecial = true;

                    if (isEspecial1)
                    {
                        isEspecial1 = false;
                        _anim.SetBool("isKick", false);
                        _anim.SetBool("isThrow", true);
                    }
                    else
                    {
                        isEspecial1 = true;
                        _anim.SetBool("isKick", true);
                        _anim.SetBool("isThrow", false);
                    }
                }
            }
            else
            {
                isEspecial = false;
                if (!isEspecial)
                {
                    _anim.SetBool("isKick", false);
                    _anim.SetBool("isThrow", false);
                }
            }
        }
            
    }

    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = _body.transform.localScale;
        theScale.x *= -1;
        _body.transform.localScale = theScale;

        _body.velocity = new Vector2();
    }
}

