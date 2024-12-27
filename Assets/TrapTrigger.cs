using UnityEngine;

public class TrapTrigger : MonoBehaviour
{

    public float x;
    public float y;
    public float z;
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Trap")
        {
            Debug.Log(other.gameObject.tag);
            //GetComponent<CharacterController>().enabled = false;
            transform.position = new Vector3(x, y, z);
            //GetComponent<CharacterController>().enabled = true;
        }
    }
}
