using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

    [SerializeField]
    private float MaxSpeed = 2f;
    [SerializeField]
    private float MaxCrawSpeed = 0.5f;
    [SerializeField]
    public LayerMask ground_layers;
    [SerializeField]
    public float radius = 0.065f;
    [SerializeField]
    public float jumpForce = 5f;

    private Rigidbody2D _body = null;
    private Animator _anim;
    private bool m_FacingRight = true;
    private bool isEspecial = false;
    private bool isEspecial1 = false;    
        
    public bool isFire = false;
    public bool isJump = false;
    public bool isDown = false;
    public bool isUp = false;
    public bool isLeft = false;
    public bool isRight = false;
    public bool isRun = false;
    public bool isWalk = false;
    public float OnAir = 0.0f;
    public bool isGround = false;
    public float Direccion = 1.0f;

    float move = 0.0f;
    float velx = 0.0f;
    float vely = 0.0f;

    public float VelX;
    public float VelY;

    void Awake()
    {
        _body = GetComponentInChildren<Rigidbody2D>();
        _anim = GetComponentInChildren<Animator>();
    }
    	
	void Start () {	
	}
	
    	
	void Update ()
    {
        Move();
        SetAnimatorState();
    }

    void LateUpdate()
    {
        
    }

    /// <summary>
    /// Asigna el estado a las variables del animator
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

    /// <summary>
    /// Verifica si el personaje esta tocando el piso
    /// </summary>
    public void CheckGround()
    {
        isGround = Physics2D.OverlapCircle(_body.transform.position, radius, ground_layers);
        OnAir = _body.velocity.y;

        if (!isGround)
        {                       
            if (isGround)
            {
                if(Mathf.Abs(_body.velocity.x) > Vector2.kEpsilon)
                {
                    _body.AddForce(Vector2.right * Direccion * 1.25f, ForceMode2D.Impulse);
                    
                }
            }
        }
    }

    /// <summary>
    /// Aplica las modificaciones que interactuan con 
    /// el motor de fisica 
    /// </summary>
    public void ApplyPhysics()
    {
        VelX = _body.velocity.x;
        VelY = _body.velocity.y;

        if (isGround)
        {
            if (isJump)
            {
                isJump = false;                
                isGround = false;
                if (isWalk)
                {
                    Vector2 vect = new Vector2( Direccion * 0.3f , 1.0f);
                    _body.AddForce(vect * jumpForce, ForceMode2D.Impulse);
                }
                else
                {
                    _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                }
                isWalk = false;
            }

            if (isRun)
            {
                if (isDown)
                {
                    _body.velocity = new Vector2(move * MaxCrawSpeed, _body.velocity.y);
                }
                else
                {
                    _body.velocity = new Vector2(move * MaxSpeed, _body.velocity.y);
                }
            }
            else
            {
                if (_body.velocity.y < Vector2.kEpsilon  && _body.velocity.x > Vector2.kEpsilon)
                {
                    //_body.velocity = new Vector2(_body.velocity.y, 0);
                }
            }
        }
        else
        {
            OnAir = _body.velocity.y;
        }
    }

    /// <summary>
    /// Captura y activa los flags de movimiento
    /// </summary>
    private void Move()
    {
        //Remapear Entradas
        isFire = CrossPlatformInputManager.GetButton("Fire"); //F
        bool especial = CrossPlatformInputManager.GetButton("Especial"); //C

        isUp = CrossPlatformInputManager.GetButton("UP"); //W
        isDown = CrossPlatformInputManager.GetButton("DOWN"); //S
        isLeft = CrossPlatformInputManager.GetButton("LEFT"); //A
        isRight = CrossPlatformInputManager.GetButton("RIGHT"); //D

        float h = CrossPlatformInputManager.GetAxis("Horizontal"); //A <-- y --> D                
        float v = CrossPlatformInputManager.GetAxis("Vertical");//W (up) y S (down)

        //Assignar variables
        move = h;
        velx = Mathf.Abs(move);
        isRun = velx > 0.3f;        
        vely = v;

        

        if (isGround)
        {
            if (isUp)
            {
                isJump = true;

                if (isLeft || isRight)
                {
                    isWalk = true;
                }
            }
           
            if (move > 0 && !m_FacingRight)
            {
                Flip();
                m_FacingRight = true;
                Direccion = 1.0f;
            }
            else if (move < 0 && m_FacingRight)
            {
                Flip();
                m_FacingRight = false;
                Direccion = -1.0f;
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

    /// <summary>
    /// Orienta la imagen del personaje en relacion a la direccion 
    /// del movimiento del mismo
    /// </summary>
    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        m_FacingRight = !m_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = _body.transform.localScale;
        theScale.x *= -1;
        _body.transform.localScale = theScale;        
    }
}

