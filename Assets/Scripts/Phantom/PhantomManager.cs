using System.Collections.Generic;
using UnityEngine;

public class PhantomManager
{
    private int maxPhantoms;
    private List<GameObject> phantoms;
    private GameObject lastAdded;
    
    public PhantomManager(int maxPhantoms)
    {
        this.maxPhantoms = maxPhantoms;
        phantoms = new List<GameObject>();
    }

    public void AddPhantom(GameObject newPhantom)
    {
        phantoms.Add(newPhantom);
        lastAdded = newPhantom;

        if (phantoms.Count > maxPhantoms)
        {
            Debug.Log("You have more phantoms than the max phantom count!");
        }
    }
    public void ResetPhantoms()
    {
        foreach (GameObject phantom in phantoms)
        {
            if (phantom != lastAdded)
            {
                phantom.GetComponent<PhantomMovement>().RestartPath();
                
                // The Phantom GameObject only has one child which is the "Collider" GameObject that we SetActive(false)
                phantom.transform.GetChild(0).gameObject.SetActive(true);      
            }
        }
    }
}
