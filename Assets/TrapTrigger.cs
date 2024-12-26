using UnityEngine;

public class TrapTrigger : MonoBehaviour
{

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Trap")
        {
            Debug.Log(other.gameObject.tag);
            GetComponent<CharacterController>().enabled = false;
            transform.position = new Vector3(0.078f, 3.3f, -1.49f);
            GetComponent<CharacterController>().enabled = true;
        }
    }
}
