using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
    #region Variables
    [Header("Public References")]
    public Rigidbody rBody;
    public bool useColliderFitness;
    public int LAYERS;
    public int NEURONS;

    [Header("Movement Variables")]
    public float maxVel;
    public float speedinc;
    public float speed;
    public float turn;

    [Header("Fitness Vars")]
    public float distanceMultiplier;
    public float sensorMultiplier;
    public float avgSpeedMultiplier;

    [Header("Visual Variables")]
    public Vector3 accel;
    public Vector3 outAccel;

    //Management Variables
    private Vector3 startP;
    private Vector3 startE;
    int checkPoints = 0;
    float maxRot = 90; Vector3 inp;
    private float a;
    private float t;
   
    float timeSinceStart = 0f;
    Vector3 lastPosition;
    private Vector3 pA;
    private Vector3 pB;
    private Vector3 pC;


    [Header("Input Variables")]
    public float range;
    public float fitnessReward = 10;
    public float aM, bM, cM;

    [Header("Statistics")]
    public float overallFitness = 0;
    [Header("Other Statistics")]
    public float totalDistanceTravelled;
    public float avgSpeed;
    [Header("Sensory Statistics")]
    public float aSensor;
    public float bSensor;
    public float cSensor;
    #endregion
    
    #region Awake and Gizmos
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(pA, 0.2f);
        Gizmos.DrawWireSphere(pB, 0.2f);
        Gizmos.DrawWireSphere(pC, 0.2f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, pA);
        Gizmos.DrawLine(transform.position, pB);
        Gizmos.DrawLine(transform.position, pC);

    }

    private Network network;
  

    private void Awake()
    {
        startP = transform.position;
        startE = transform.eulerAngles;
        //network = new Network();
       // network.Initialise(LAYERS,NEURONS);
    }

    #endregion

    #region Updates

    private void Update()
    {
        //This is a manual override if the car is retarded
        if (Input.GetKeyDown(KeyCode.R))
        {
            Death();
        }
    }

    //This manages getting the input values, sending it, getting back the output values and running it through
    private void FixedUpdate()
    {
        //Run Inputs
        InputSensors();

        //Get Last Position for Calculation of Distance and AVG Speed
        lastPosition = transform.position;

        //Run the network (send it to the back)
        (a, t) = network.RunNetwork(aSensor, bSensor, cSensor);

        accel.x = a;
        accel.z = t;

        MoveCar(accel.x, accel.z);

        timeSinceStart += Time.deltaTime;
        CalculateFitness();

        //Quick Reset
        a = 0;
        t = 0;
        

    }

    #endregion

    #region Inputs
    public void InputSensors()
    {
        Vector3 a = (transform.forward + transform.right)*range;
        
        Vector3 b = (transform.forward) * range;
      
        Vector3 c = (transform.forward - transform.right) * range;
   

        Ray aRay = new Ray(transform.position, a);
        Ray bRay = new Ray(transform.position, b);
        Ray cRay = new Ray(transform.position, c);

        RaycastHit aHit;
        if (Physics.Raycast(aRay, out aHit,range))
        {
            pA = aHit.point;

           // if (aHit.distance > aM)
           //     aM = aHit.distance;

            aSensor = aHit.distance/aM;
            
        }

        RaycastHit bHit;
        if (Physics.Raycast(bRay, out bHit, range))
        {
            pB = bHit.point;

          //  if (bHit.distance > bM)
          //      bM = bHit.distance;

            bSensor = bHit.distance / bM;

        }

        RaycastHit cHit;
        if (Physics.Raycast(cRay, out cHit, range))
        {
            pC = cHit.point;

            //if (cHit.distance > cM)
            //    cM = cHit.distance;

            cSensor = aHit.distance /cM;
        }



    }
    #endregion

   
    #region Fitness Calcualtion
    public void CalculateFitness()
    {
       
        //Calculates tDist and avgSpeed
        totalDistanceTravelled += Vector3.Distance(transform.position, lastPosition);
        avgSpeed = totalDistanceTravelled / timeSinceStart;
        
        if (!useColliderFitness)
        overallFitness = (totalDistanceTravelled*distanceMultiplier)+(avgSpeed*avgSpeedMultiplier)+((bSensor+aSensor+cSensor)/3)*sensorMultiplier;
        

        //Kills Network if it is too bad
        if ((timeSinceStart > 20 && overallFitness < 4))
        {
            Death();
        }

        //Kills Network if too good
        if (overallFitness >= 500)
        {
            Death();
        }
    }
    #endregion

    #region Resetting
    public void Reset()
    {
        //Reset Network
        

        //Reset Variables
        timeSinceStart = 0f;
        checkPoints = 0;
 
        totalDistanceTravelled = 0;
        avgSpeed = 0;
 
        lastPosition = startP;
        overallFitness = 0f;
        transform.position = startP;
        transform.eulerAngles = startE;
    }

    public void ResetWithNetwork (Network net)
    {
        network = net;
        Reset();
    }
    #endregion

    #region Movement + Collision
    public void MoveCar (float v, float h)
    {
        inp = Vector3.Lerp(Vector3.zero, new Vector3(0, 0, v * speed), speedinc);
        inp = transform.TransformDirection(inp);
        inp = Vector3.ClampMagnitude(inp, maxVel);

        transform.position += inp;

        transform.eulerAngles += new Vector3(0, Mathf.Lerp(0, h * maxRot, turn), 0);
    }

    public void Death()
    {
        GameObject.FindObjectOfType<GeneticManagerTwo>().Death(overallFitness, network);

    }

    //Now check collision for the walls as well
    private void OnCollisionEnter(Collision collision)
    {
        Death();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Fitness" && useColliderFitness)
        {
            checkPoints += 1;
            overallFitness += fitnessReward;
        }

    }
    #endregion

    #region Pointless
    //Sigmoid Activation Function Local
    private float SigmoidActivate(float s)
    {
        return (1 / (1 + Mathf.Exp(-s)));
    }
    #endregion

}


//Comments : 

/*
 *float half = accel.x / 2;
    float resultX =0.5f + half;
    outAccel.x = resultX; */
