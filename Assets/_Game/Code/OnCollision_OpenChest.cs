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
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
         Debug.Log("Player collided with " + gameObject.name);
         Debug.Log(spriteRenderer.sprite.name);
            // Add logic to open the chest here
        if (collision.gameObject.CompareTag("Player"))
        {
            // Change the sprite to the open chest sprite
            spriteRenderer.sprite = spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/TX Props Chest Opened");
            Debug.Log("Chest opened!");
            Debug.Log(spriteRenderer.sprite.name);
        }
           
    }

}