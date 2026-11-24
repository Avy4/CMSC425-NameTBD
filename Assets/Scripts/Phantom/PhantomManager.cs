using System.Collections.Generic;
using UnityEngine;

public class PhantomManager
{
    private int maxPhantoms;
    private List<GameObject> phantoms;
    
    public PhantomManager(int maxPhantoms)
    {
        this.maxPhantoms = maxPhantoms;
        phantoms = new List<GameObject>();
    }

    public void AddPhantom(GameObject newPhantom)
    {
        phantoms.Add(newPhantom);

        if (phantoms.Count > maxPhantoms)
        {
            Debug.Log("You have more phantoms than the max phantom count!");
        }
    }
    public void ResetPhantoms()
    {
        foreach (GameObject phantom in phantoms)
        {
            // Just in case we want to destroy phantoms, this would catch a destroyed phantom so it doesn't break
            if (phantom != null)
            {
                PhantomMovement currPhantom;
                if (phantom.TryGetComponent<PhantomMovement>(out currPhantom))
                {
                    currPhantom.RestartPath();

                    // The Phantom GameObject only has one child which is the "Collider" GameObject that we SetActive(true)
                    phantom.transform.GetChild(0).gameObject.SetActive(true);     
                }
            }
        }
    }
}
