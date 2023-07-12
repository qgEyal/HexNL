
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*

This script includes variables for the lifespan, speed, size, reproduction rate, mutation rate, hunger, thirst, and temperature 
of the artificial life object. It also includes functions for birth, growth, evolution, mutation, and death of the artificial life 
object, as well as collision detection with food and water, and mating triggers.

The script assumes that there are objects in the game world tagged as "Food", "Water", and "Mating" that the artificial life object 
can interact with. The script also assumes that the artificial life object has a sprite renderer component with a textured 
square sprite.
*/
public class ArtificialLife : MonoBehaviour
{
    public float lifespan = 10f; // The lifespan of the artificial life object
    public float speed = 1f; // The speed of the artificial life object
    public float size = 1f; // The size of the artificial life object
    public float reproductionRate = 0.1f; // The reproduction rate of the artificial life object
    public float mutationRate = 0.01f; // The mutation rate of the artificial life object
    public float hunger = 0f; // The hunger level of the artificial life object
    public float maxHunger = 10f; // The maximum hunger level of the artificial life object
    public float hungerRate = 0.1f; // The rate at which the artificial life object gets hungry
    public float thirst = 0f; // The thirst level of the artificial life object
    public float maxThirst = 10f; // The maximum thirst level of the artificial life object
    public float thirstRate = 0.1f; // The rate at which the artificial life object gets thirsty
    public float temperature = 20f; // The temperature of the artificial life object's environment
    public float minTemperature = -10f; // The minimum temperature the artificial life object can tolerate
    public float maxTemperature = 50f; // The maximum temperature the artificial life object can tolerate

    private float age = 0f; // The age of the artificial life object
    private bool isDead = false; // Whether the artificial life object is dead
    private bool isMature = false; // Whether the artificial life object is mature enough to reproduce
    private Vector2 direction; // The direction of the artificial life object's movement
    private SpriteRenderer spriteRenderer; // The sprite renderer component of the artificial life object
    private Color color; // The color of the artificial life object's sprite

    // Start is called before the first frame update
    void Start()
    {
        direction = Random.insideUnitCircle.normalized;
        spriteRenderer = GetComponent<SpriteRenderer>();
        color = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            age += Time.deltaTime;

            // If the artificial life object is too hungry or thirsty, it dies
            if (hunger >= maxHunger || thirst >= maxThirst)
            {
                Die();
            }

            // If the artificial life object is too hot or too cold, it gets slower
            if (temperature < minTemperature || temperature > maxTemperature)
            {
                speed *= 0.5f;
            }

            // If the artificial life object is mature enough and has enough resources, it reproduces
            if (isMature && Random.value < reproductionRate && hunger < maxHunger * 0.5f && thirst < maxThirst * 0.5f)
            {
                Reproduce();
            }

            // Move the artificial life object
            transform.Translate(direction * speed * Time.deltaTime, Space.World);

            // Check if the artificial life object is out of bounds
            if (!GetComponent<Renderer>().isVisible)
            {
                Die();
            }

            // Update hunger and thirst levels
            hunger += hungerRate * Time.deltaTime;
            thirst += thirstRate * Time.deltaTime;

            // Update sprite color based on hunger and thirst levels
            color.r = 1f - hunger / maxHunger;
            color.b = 1f - thirst / maxThirst;
            spriteRenderer.color = color;
            
            // If the artificial life object has reached the end of its lifespan, it dies
            if (age >= lifespan)
            {
                Die();
            }
        }
    }

    // Called when the artificial life object dies
    private void Die()
    {
        isDead = true;
        spriteRenderer.enabled = false;
        Destroy(gameObject, 1f); // Destroy the game object after a delay of 1 second
    }

    // Called when the artificial life object reproduces
    private void Reproduce()
    {
        Vector2 offspringDirection = Quaternion.Euler(0f, 0f, Random.Range(-45f, 45f)) * direction;
        float offspringSpeed = Mathf.Clamp(speed + Random.Range(-0.1f, 0.1f), 0f, 10f);
        float offspringSize = Mathf.Clamp(size + Random.Range(-0.1f, 0.1f), 0.1f, 5f);
        float offspringMutationRate = Mathf.Clamp(mutationRate + Random.Range(-0.001f, 0.001f), 0f, 0.1f);

        GameObject offspring = Instantiate(gameObject, transform.position, Quaternion.identity);
        offspring.GetComponent<ArtificialLife>().direction = offspringDirection;
        offspring.GetComponent<ArtificialLife>().speed = offspringSpeed;
        offspring.GetComponent<ArtificialLife>().size = offspringSize;
        offspring.GetComponent<ArtificialLife>().mutationRate = offspringMutationRate;
        offspring.GetComponent<ArtificialLife>().age = 0f;
        offspring.GetComponent<ArtificialLife>().isDead = false;
        offspring.GetComponent<ArtificialLife>().isMature = false;
        offspring.GetComponent<ArtificialLife>().hunger = 0f;
        offspring.GetComponent<ArtificialLife>().thirst = 0f;
        offspring.GetComponent<ArtificialLife>().spriteRenderer.enabled = true;
        offspring.GetComponent<ArtificialLife>().spriteRenderer.color = color;

        isMature = false;
    }

    // Called when the artificial life object collides with another object
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Food"))
        {
            hunger = 0f;
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Water"))
        {
            thirst = 0f;
            Destroy(collision.gameObject);
        }
    }

    // Called when the artificial life object triggers another object
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Mating"))
        {
            isMature = true;
        }
    }
}
