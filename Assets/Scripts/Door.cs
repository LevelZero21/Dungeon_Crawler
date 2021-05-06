using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] private Transform m_Player;
    [SerializeField] private float m_DoorRadius;
    [SerializeField] private Animator m_Animator;

    private bool m_DoorOpen = false;
    private bool m_OpeningDoor = false;
    private bool m_ClosingDoor = false;

    private float m_Counter = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if(m_OpeningDoor && !m_ClosingDoor)
        {
            if (m_Counter >= 1.0f)
            {
                m_Animator.SetBool("DoorOpen", true);
                m_DoorOpen = true;
                m_Animator.SetBool("DoorOpening", false);
                m_OpeningDoor = false;
                m_Counter = 0.0f;
            }
            else
                m_Counter += Time.deltaTime;
        }

        if (m_ClosingDoor && !m_OpeningDoor)
        {
            if (m_Counter >= 1.0f)
            {
                m_Animator.SetBool("DoorOpen", false);
                m_DoorOpen = false;
                m_Animator.SetBool("DoorClosing", false);
                m_ClosingDoor = false;
                m_Counter = 0.0f;
            }
            else
                m_Counter += Time.deltaTime;
        }

        if (WithinRange())
        {
            if(Input.GetKeyDown(KeyCode.E))
            {
                if(!m_ClosingDoor && !m_OpeningDoor)
                {
                    if (!m_DoorOpen)
                        OpenDoor();
                    if (m_DoorOpen)
                        CloseDoor();
                }
            }
        }
    }

    private bool WithinRange()
    {
        if(Vector3.Distance(m_Player.position, transform.position) < m_DoorRadius)
        {
            return true;
        }
        return false;
    }

    private void OpenDoor()
    {
        m_Animator.SetBool("DoorOpening", true);
        m_OpeningDoor = true;
    }

    private void CloseDoor()
    {
        m_Animator.SetBool("DoorClosing", true);
        m_ClosingDoor = true;
    }
}
