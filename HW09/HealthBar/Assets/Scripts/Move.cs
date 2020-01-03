using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float translationX = Input.GetAxis("Horizontal");
        float translationZ = Input.GetAxis("Vertical");
        MovePlayer(translationX, translationZ);
    }
    
    public void MovePlayer(float translationX, float translationZ){
        translationX *= Time.deltaTime;
        translationZ *= Time.deltaTime;
        this.transform.LookAt(new Vector3(this.transform.position.x + translationX, this.transform.position.y, this.transform.position.z + translationZ));
        if (translationX == 0)
            this.transform.Translate(0, 0, Mathf.Abs(translationZ) * 5);
        else if (translationZ == 0)
            this.transform.Translate(0, 0, Mathf.Abs(translationX) * 5);
        else
            this.transform.Translate(0, 0, (Mathf.Abs(translationZ) + Mathf.Abs(translationX)) * 2.5f);
    }
}
