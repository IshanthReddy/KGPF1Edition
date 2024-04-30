using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarController : MonoBehaviour
{
    public float acceleration;
    public float turnSpeed;

    public Transform carModel;
    private Vector3 startModelOffset;

    public float groundCheckRate;
    private float lastGroundCheckTime;

    private float curYRot;

    public bool canControl;

    private bool accelerateInput;
    private float turnInput;

    public TrackZone curTrackZone;
    public int zonesPassed;
    public int racePosition;
    public int curLap;

    public Rigidbody rig;

    void Start()
    {
        startModelOffset = carModel.transform.localPosition;
        GameManager.Instance.cars.Add(this);
        transform.position = GameManager.Instance.spawnPoints[GameManager.Instance.cars.Count - 1].position;
        rig.position = GameManager.Instance.spawnPoints[GameManager.Instance.cars.Count - 1].position;
    }
    void Update()
    {
        if (!canControl)
            turnInput = 0.0f;
        float turnRate = Vector3.Dot(rig.velocity.normalized, carModel.forward);
        turnRate = Mathf.Abs(turnRate);

        curYRot += turnInput * turnSpeed * turnRate * Time.deltaTime;

        carModel.position = transform.position + startModelOffset;
        //carModel.eulerAngles = new Vector3(0, curYRot, 0);

        CheckGround();
    }

    void FixedUpdate()
    {
        if (!canControl)
            return;

        if (accelerateInput == true)
        {
            rig.AddForce(carModel.forward * acceleration, ForceMode.Acceleration);
        }
    }

    void CheckGround()
    {
        Ray ray = new Ray(transform.position + new Vector3(0, -0.75f, 0), Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 1.0f)) 
        {
            carModel.up = hit.normal;
        }
        else 
        { 
            carModel.up = Vector3.up;
        }

        carModel.Rotate(new Vector3(0, curYRot, 0), Space.Self);
    }
    public void OnAccelerateInput(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
            accelerateInput = true;
        else
            accelerateInput = false;
    }

    public void OnTurnInput(InputAction.CallbackContext context)
    {
        turnInput = context.ReadValue<float>();
    }
}
