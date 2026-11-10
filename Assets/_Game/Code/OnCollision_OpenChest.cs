using System.Drawing;
using UnityEngine;

public class OnCollision_OpenChest : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
         Debug.Log("Player collided with " + gameObject.name);
            // Add logic to open the chest here
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform child = transform.Find("PF Props - Chest 01");
            // Change the object to the open chest object
            child.GetComponent<SpriteRenderer>().enabled = false;
            Transform childOpen = transform.Find("PF Props - Chest 01 Open");
            childOpen.GetComponent<SpriteRenderer>().enabled = true;
            Debug.Log("Chest opened!");
        }
           
    }

}