using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LightChange : MonoBehaviour
{
    public int speed;
    public float totalRotation = -10f;


    // Update is called once per frame
    void Update()
    {
        // Rotate the directional light around the x-axis based on speed
        transform.Rotate(Vector3.right, speed * Time.deltaTime);

        // Update the total rotation
        totalRotation += speed * Time.deltaTime;

        // Check if the total rotation completes a full circle (360 degrees)
        if (totalRotation >= 380f)
        {
            // Reload the scene
            SceneManager.LoadScene(0);
        }

    }
}
