using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorOpener : MonoBehaviour
{
    [SerializeField] Transform leftDoorPivot;
    [SerializeField] Transform rightDoorPivot;
    [SerializeField] float openDoorAngle = 130;
    [SerializeField] float rotateSpeed = 1f;

    bool openDoor = false;

    Quaternion rightTarget;
    Quaternion leftTarget;

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Person>())
        {
            openDoor = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Person>())
        {
            openDoor = false;
        }
    }

    private void OpenDoor()
    {
        if (openDoor)
        {
            rightTarget = Quaternion.Euler(new Vector3(0, -openDoorAngle, 0));
            leftTarget = Quaternion.Euler(new Vector3(0, openDoorAngle, 0));
        }
    }

    private void CloseDoor()
    {
        if (!openDoor)
        {
            rightTarget = Quaternion.Euler(new Vector3(0, 0, 0));
            leftTarget = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        
    }

    private void Update()
    {
        OpenDoor();
        CloseDoor();

        rightDoorPivot.rotation = Quaternion.Lerp(rightDoorPivot.rotation, rightTarget, rotateSpeed * Time.deltaTime);
        leftDoorPivot.rotation = Quaternion.Lerp(leftDoorPivot.rotation, leftTarget, rotateSpeed * Time.deltaTime);
    }
}
