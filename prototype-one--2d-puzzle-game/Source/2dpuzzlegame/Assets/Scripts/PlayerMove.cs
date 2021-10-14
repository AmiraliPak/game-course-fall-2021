﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    
    public float factor = 0.01f;
    public float jumpAmount = 0.5f;

    public SpriteRenderer spriteRenderer;
    public Rigidbody2D rb;

    public GameObject clones;
    public CloneMove[] cloneMoves;

    private bool canJump;

    private Vector3 moveVector;
    public EventSystemCustom eventSystem;
    GameObject adjacentKey, adjacentDoor;
    int collectedKeysCount = 0;
    void Start()
    {
        cloneMoves = clones.GetComponentsInChildren<CloneMove>();

        canJump = true;
        moveVector = new Vector3(1 * factor, 0, 0);
        adjacentKey = null;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += moveVector;

            MoveClones(moveVector, true);

            spriteRenderer.flipX = false;

        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= moveVector;

            MoveClones(moveVector, false);

            spriteRenderer.flipX = true;
        }

        if (Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            rb.AddForce(transform.up * jumpAmount, ForceMode2D.Impulse);
            JumpClones(jumpAmount);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (adjacentKey != null)
            {
                Debug.Log("pick the key up dude!");
                eventSystem.OnKeyPickup.Invoke();
                collectedKeysCount++;
                adjacentKey.SetActive(false);
            }
            if (adjacentDoor != null)
            {
                var doorController = adjacentDoor.GetComponent<DoorController>();
                if (doorController.OpenDoor(collectedKeysCount))
                    this.gameObject.SetActive(false);
                // game ends if door opens so no need to update key counter text
            }
        }

        // This was added to answer a question.
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Destroy(this.gameObject);
        }


        // This is too dirty. We must decalare/calculate the bounds in another way. 
        /*if (transform.position.x < -0.55f) 
        {
            transform.position = new Vector3(0.51f, transform.position.y, transform.position.z);
        }
        else if (transform.position.x > 0.53f)
        {
            transform.position = new Vector3(-0.53f, transform.position.y, transform.position.z);
        }*/
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.DeathZone.ToString()))
        {
            Debug.Log("DEATH ZONE");
            //invoke event to show lose text
        }
        
        if (collision.gameObject.CompareTag(TagNames.CollectableItem.ToString()))
        {
            collision.gameObject.SetActive(false);
            Debug.Log("POTION!");
        }
        
        if (collision.gameObject.CompareTag(TagNames.Key.ToString()))
        {
            Debug.Log("next to key!");
            adjacentKey = collision.gameObject;
        }
        
        if (collision.gameObject.CompareTag(TagNames.Door.ToString()))
        {
            Debug.Log("next to door!");
            adjacentDoor = collision.gameObject;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (collision.gameObject.CompareTag(TagNames.Key.ToString()))
        {
            Debug.Log("NOT next to key!");
            adjacentKey = null;
        }
        if (collision.gameObject.CompareTag(TagNames.Door.ToString()))
        {
            Debug.Log("NOT next to door!");
            adjacentDoor = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky");
            canJump = false;
        }

        if (collision.gameObject.CompareTag(TagNames.ExitDoor.ToString()))
        {
            Debug.Log("exit door");
        }

       

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(TagNames.StickyPlatform.ToString()))
        {
            Debug.LogWarning("sticky no more bruh");
            canJump = true;
        }
    }

    public void MoveClones(Vector3 vec, bool isDirRight)
    {
        foreach (var c in cloneMoves)
            c.Move(vec, isDirRight);
    }

    public void JumpClones(float amount)
    {
        foreach (var c in cloneMoves)
            c.Jump(amount);
    }
}
