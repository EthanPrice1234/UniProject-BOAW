using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class BindingBookController : MonoBehaviour
{
    public Transform player; 
    public Vector3 offset = new Vector3(-1f, 2f, -1f); 
    public float followSpeed = 8f;
    public float floatAmplitude = 0.2f; 
    public float floatFrequency = 1f; 

    private Vector3 initialOffset;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
        initialOffset = offset;
    }

    void Update()
    {
        if (player != null)
        {
            // Floating offset
            float floatY = Mathf.Sin(Time.time * floatFrequency) * floatAmplitude;
            float floatX = Mathf.Cos(Time.time * floatFrequency * 0.7f) * floatAmplitude * 0.5f;
            Vector3 floatingOffset = initialOffset + new Vector3(floatX, floatY, 0);

            // Move to player
            Vector3 desiredPosition = player.position + player.TransformDirection(floatingOffset);
            transform.position = Vector3.Lerp(transform.position, desiredPosition, followSpeed * Time.deltaTime);

            // Match rotation to player, but with an extra -35 degrees on the x-axis
            Quaternion targetRotation = player.rotation * Quaternion.Euler(-35f, 30f, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, followSpeed * Time.deltaTime);
        }
    }
}
