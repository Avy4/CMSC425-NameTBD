using UnityEngine;

public class PhantomDeath : Death
{
    // All the Phantom needs to do on death is dissapear and become non interactable
    // We don't want to destroy it cause we still need it on the next reset
    public override void Die()
    {   
        gameObject.SetActive(false);
    }
}
