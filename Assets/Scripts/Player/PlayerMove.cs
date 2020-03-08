﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    public float speed = 7.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public Transform playerCameraParent;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 60.0f;

    CharacterController characterController;
    Vector3 moveDirection = Vector3.zero;
    Vector2 rotation = Vector2.zero;

    public bool canMove = true;
    public bool climbable = false;

    public float jumpTime;
    public float fasterSpeed = 10;
    public float originalSpeed = 7.5f;
    public float speedTimer = 0;

    public float fasterJumpSpeed = 35;
    public float jumpBoostTimer;

    public Image SpeedBoostImage;
    public Image JumpBoostImage;

    void Start()
    {
        Screen.SetResolution(720, 480, true);

        characterController = GetComponent<CharacterController>();
        rotation.y = transform.eulerAngles.y;
    }

    void Update()
    {
        #region ui stuff

        speedTimer -= Time.deltaTime;
        SpeedBoostImage.fillAmount = speedTimer / 5;

        if (speedTimer <= 0) {
            speedTimer = 0;
            speed = 7.5f;
        }

        jumpBoostTimer -= Time.deltaTime;
        JumpBoostImage.fillAmount = jumpBoostTimer / 5;

        if (jumpBoostTimer <= 0)
        {
            jumpBoostTimer = 0;
            jumpSpeed = 25.0f;
        }

        #endregion


        //various movement settings
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);
        float curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);


        //making it so the player can climb walls
        if (climbable)
        {
            forward = transform.TransformDirection(Vector3.up);
            curSpeedX = canMove ? speed * Input.GetAxis("Vertical") : 0;
            curSpeedY = canMove ? speed * Input.GetAxis("Horizontal") : 0;
            moveDirection = (forward * curSpeedX) + (right * curSpeedY);
            gravity = 0;
        }

        if (Input.GetButton("Fire3")) {
            gravity = 800;
        }
        else if(!climbable)
        {
            gravity = 600;
        }

        //jump functionality
        if (Input.GetButton("Jump")){
            jumpTime+= 1 * Time.deltaTime;
            if(jumpTime <= 0.2f) {
                 moveDirection.y = jumpSpeed;
            }
        }

        //resets the jump counter
        if (characterController.isGrounded) {
            jumpTime = 0;
        }

        //applies gravity
        moveDirection.y -= gravity * Time.deltaTime;

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove)
        {
            rotation.y += Input.GetAxis("Mouse X") * lookSpeed;
            rotation.x += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotation.x = Mathf.Clamp(rotation.x, -lookXLimit, lookXLimit);
            playerCameraParent.localRotation = Quaternion.Euler(rotation.x, 0, 0);
            transform.eulerAngles = new Vector2(0, rotation.y);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Speed") {
            speed = fasterSpeed;
            speedTimer = 5;
            Destroy(other.gameObject);
        }
        if (other.tag == "JumpBoost")
        {
            jumpSpeed = fasterJumpSpeed;
            jumpBoostTimer = 5;
            Destroy(other.gameObject);
        }

        //checks to see if they are colliding with a climable object
        if (other.tag == "Climbable") {
            climbable = true;
        }
    }

    //when the player leaves the climbable zone
    private void OnTriggerExit(Collider other)
    {
        if(other.tag == "Climbable"){
            climbable = false;
            moveDirection.y = jumpSpeed;
        }  
            
    }
}
