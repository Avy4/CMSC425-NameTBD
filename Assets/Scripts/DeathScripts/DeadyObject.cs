using UnityEngine;

public class DeadyObject : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        GameObject otherGameObject = other.gameObject;

        // Checks tag to make sure a Player or Phantom has entered
        if (other.tag == "Player" || other.tag == "Phantom")
        {   
            // Polymorphism in the big 26
            // Deadly object doesn't need to know specifically if it's a player or phantom it's operating on
            Death deathScript = otherGameObject.GetComponent<Death>();
            deathScript.Die();
        }
    }
}
