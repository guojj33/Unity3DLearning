using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class VirtualButtonEventHandler : MonoBehaviour, IVirtualButtonEventHandler
{
    public GameObject vb;
    public Animator ani;

    // Start is called before the first frame update
    void Start()
    {
        VirtualButtonBehaviour vbb = vb.GetComponent<VirtualButtonBehaviour>();
        if (vbb)
        {
            vbb.RegisterEventHandler(this);
        }   
    }

    public void OnButtonPressed(VirtualButtonBehaviour vb)
    {
        ani.SetTrigger("Fly");
        Debug.Log("按钮按下");
    }

    public void OnButtonReleased(VirtualButtonBehaviour vb)
    {
        ani.SetTrigger("Land");
        Debug.Log("按钮释放");
    }
}
