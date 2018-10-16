using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{

    public float radius;
    CharacterController controller;
    public LayerMask playerMask;
    private void Awake () {
       // controller = GetComponent<CharacterController>();
    }

    //Input Gains
    private void Update() {
        ControllerGainData();

    }

    private void ControllerGainData() {
        
    }

    //Physics Movements
    private void FixedUpdate() {
        isGrounded = Grounded();
    }

    #region  Inputs

    private Vector3 inputVector;
    private void GainInput() {

    }


    private void OnDrawGizmos() {


    }

    #endregion


    #region  Checks

    public float maxHitDistance;
    public RaycastHit groundHit;
    public RaycastHit lastGroundHit;
    public bool failedCheck;
    public bool isGrounded = false;
        

    public bool Grounded () {

        Ray ray1 = new Ray(transform.position-(Vector3.right*radius/2),Vector3.down);
        Ray ray2 = new Ray(transform.position, Vector2.down);
        Ray ray3 = new Ray(transform.position+(Vector3.right*radius/2),Vector3.down);

        RaycastHit hit1;
        RaycastHit hit2;
        RaycastHit hit3;


        if (Physics.Raycast(ray1, out hit1)) {
            if (hit1.distance < maxHitDistance) {
                Debug.DrawRay(ray1.origin, ray1.direction, Color.green, 0.2f);
            }
        }

        if (Physics.Raycast(ray2, out hit2)) {
             if (hit2.distance < maxHitDistance) {
                Debug.DrawRay(ray2.origin, ray2.direction, Color.green, 0.2f);
            }
        }

        
        if (Physics.Raycast(ray2, out hit3)) {
             if (hit3.distance < maxHitDistance) {
                Debug.DrawRay(ray3.origin, ray3.direction, Color.green, 0.2f);
            }
        }

        return true;

    }



    #endregion


}
