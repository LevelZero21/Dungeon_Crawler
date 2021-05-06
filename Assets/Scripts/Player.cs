using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Transform m_AttachPoint;
    [SerializeField] private CharacterController m_Controller;
    [SerializeField] private float m_WalkingSpeed;
    [SerializeField] private float m_SprintSpeed;
    [SerializeField] private float m_CrouchSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float jumpHeight;

    [SerializeField] private Transform groundCheck;
    private float groundDistance = 0.4f;
    [SerializeField] private Transform roofCheck;
    private float roofDistance = 0.2f;
    [SerializeField] private LayerMask groundMask;
    private bool isGrounded;
    private float m_OriginalSlopeLimit;

    private float m_Speed;
    private Vector3 velocity;

    private float m_OriginalHeight;
    [SerializeField] private float m_CrouchHeight = 1.0f;
    private bool m_Crouching = false;

    private bool m_Walking = false;
    private bool m_Running = false;
    [SerializeField] private Animator m_Animator;

    void Start()
    {
        m_Speed = m_WalkingSpeed;
        m_OriginalHeight = m_Controller.height;
        m_OriginalSlopeLimit = m_Controller.slopeLimit;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            m_Crouching = !m_Crouching;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            m_Running = true;
            m_Speed = m_SprintSpeed;
            m_Animator.SetBool("IsRunning", true);
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            m_Running = false;
            m_Speed = m_WalkingSpeed;
            m_Animator.SetBool("IsRunning", false);
        }

        CheckIfWalking();

        CheckCrouch();

        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            m_Controller.slopeLimit = m_OriginalSlopeLimit;
        }

        if (Physics.CheckSphere(roofCheck.position, roofDistance, groundMask))
        {
            velocity.y = -0.5f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        m_Controller.Move(move * m_Speed * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            //m_Controller.slopeLimit = 90.0f;
        }

        velocity.y += gravity * Time.deltaTime;
        m_Controller.Move(velocity * Time.deltaTime);
    }


    private void CheckIfWalking()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.S))
        {
            m_Walking = true;
            m_Animator.SetBool("IsWalking", true);
        }
        if (Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false && Input.GetKey(KeyCode.W) == false && Input.GetKey(KeyCode.S) == false)
        {
            m_Walking = false;
            m_Animator.SetBool("IsWalking", false);
        }
    }

    private void CheckCrouch()
    {
        if(m_Crouching == true)
        {
            if(m_Speed == m_WalkingSpeed || m_Speed == m_SprintSpeed)
            {
                m_Speed = m_CrouchSpeed;
            }
            if(m_Controller.height != m_CrouchHeight)
            {
                if (m_Controller.height < m_CrouchHeight)
                {
                    m_Controller.height = m_CrouchHeight;
                }
                else
                {
                    m_Controller.height -= 8f * Time.deltaTime;
                }
            }
        }
        else
        {
            if (m_Speed == m_CrouchSpeed)
            {
                m_Speed = m_WalkingSpeed;
            }
            if (m_Controller.height != m_OriginalHeight)
            {
                if (m_Controller.height > m_OriginalHeight)
                {
                    m_Controller.height = m_OriginalHeight;
                }
                else
                {
                    m_Controller.height += 8f * Time.deltaTime;
                }
            } 
        }
    }
}
