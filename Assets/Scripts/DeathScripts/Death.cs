using UnityEngine;

// Death Superclass, used so the DeadlyObject doesn't need to know anything about the object entering it
public class Death : MonoBehaviour
{
    public virtual void Die()
    {
        Debug.Log("Ahhh, I have died.");
    }
}
