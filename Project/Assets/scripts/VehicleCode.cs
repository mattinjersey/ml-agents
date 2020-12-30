using UnityEngine;
using System.Collections;
using Unity.MLAgents.Sensors;
using Unity.MLAgents;
using TMPro;
using Unity.MLAgents.Actuators;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;
public class VehicleCode : Agent
{
    float myReward = 0f;
    bool killPlayer = false;
    public float turnForce;
    public float moveForce;
    public float moveMax;
    //GameObject xGame1;
    public GameObject[] blaster;
    public GameObject shields;
    public GameObject xGame1;
    public GameObject blast;
    public GameObject engine;
    public GameObject explosion;
    public Vector3 aVel;
    private Rigidbody rb;
    private float heading;
    private float bigScore = 0;
    private float turn;
    private float move;
    public TextMesh scoreText;
    private double nextShotTime;
    private bool isAlive;
    private int hasTrishots;
    AsteroidController gController;
    EnvironmentParameters m_ResetParams;


    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        isAlive = true;
        xGame1 = (GameObject)this.transform.parent.gameObject;
        gController = xGame1.GetComponent<AsteroidController>();
        // Show();
        nextShotTime = Time.time;
        m_ResetParams = Academy.Instance.EnvironmentParameters;
        SetResetParameters();
        Debug.Log(this.transform.parent.name + "...Initialize");
        //scoreText =  this.transform.parent.gameObject.transform.Find("aCanvas").GetComponent<Text>();

    }
    public override void CollectObservations(VectorSensor sensor)
    {
        /*   sensor.AddObservation(gameObject.transform.rotation.z);
            sensor.AddObservation(gameObject.transform.rotation.x);
            sensor.AddObservation(gameObject.transform.position);
            sensor.AddObservation(rb.velocity);   */
    }
    private void Update()
    {
        RequestDecision();
        Vector3 aPos = transform.position;
        transform.position = new Vector3(aPos.x, 0, aPos.z);
        float x = rb.velocity.x;
        float y = rb.velocity.y;
        float z = rb.velocity.z;
        rb.velocity = new Vector3(x, 0, z);
    }
    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        var bFire = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[0], -1f, 1f);
        //   var bHyper = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
        var bRocket = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[1], -1f, 1f);
        var bTurn = 2f * Mathf.Clamp(actionBuffers.ContinuousActions[2], -1f, 1f);
        float velocity = Mathf.Clamp(rb.velocity.magnitude, 0f, moveMax * transform.lossyScale.y);
        if (rb.velocity.magnitude > moveMax)
        {
            //Debug.Log("constrain velocity");
            rb.velocity = rb.velocity.normalized * moveMax;
        }
        int aMove = 0, aTurn = 0;
        if (bFire > 0)
        {
            Fire();
        }
        /*  if (bHyper>0)
          {
              HyperspaceJump();
          }*/
        if (bRocket > 0)
        {
            if (engine) engine.SetActive(true);
            aMove = 1;
        }
        else
        {
            if (engine) engine.SetActive(false);
            aMove = 0;
        }
        if (bTurn < -0.3)
        {
            aTurn = -1;
        }
        else if (bTurn > 0.3)
        {
            aTurn = 1;
        }
        Vector3 aVel = rb.velocity;
        aVel.y = 0f;
        rb.velocity = aVel;
        Vector3 bRot = rb.transform.localRotation.eulerAngles;
        bRot.x = 0; bRot.z = 0;
        rb.transform.localRotation = Quaternion.Euler(bRot);


        ProcessTurn(aTurn, aMove);
        SetReward(myReward);
        myReward = 0f;
        if (killPlayer)
        {
            // ExplosionInfo info = new ExplosionInfo();
            //   info.origin = transform.position;
            //   info.strength = 1.0f;
            //gc.SendMessage("MakeExplosion", info);
            EndEpisode();
            Debug.Log("End episode");

            SetResetParameters();
            // GameObject.Find("GameController").SendMessage("KillPlayer");



        }
    }
    public void xSetKillPlayer()
    {
        killPlayer = true;
    }
    public override void OnEpisodeBegin()
    {
        SetResetParameters();
        myReward = 0f;
        killPlayer = false;
        rb.velocity = new Vector3(0f, 0f, 0f);
        Debug.Log("reset velocity");
        engine.SetActive(false);
        gController.ReinitLevel();
        heading = 0;
        Debug.Log("scoreText:" + scoreText);
        scoreText.text = "Score: 0";
        //GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        bigScore = 0;
        Debug.Log("OnEpisodeBegin");
    }
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var ContinousOut = actionsOut.ContinuousActions;
        ContinousOut[0] = 0;
        //  Debug.Log("Heuristic"+isAlive);

        if (isAlive)
        {
            turn = Input.GetAxis("Horizontal");
            move = Input.GetAxis("Vertical");
            ContinousOut[0] = 0;
            if (Input.GetButton("Fire1"))
            {
                ContinousOut[0] = 0.1f;
                // Debug.Log("fire!");
            }
           
            ContinousOut[1] = 0;
            if (move > 0)
            {
                ContinousOut[1] = .3f;
            }
            ContinousOut[2] = 0;
            if (turn < 0)
            {
                ContinousOut[2] = -.4f;
            }
            else if (turn > 0)
            {
                ContinousOut[2] = .5f;
            }

        }
        else
        {
            if (engine) engine.SetActive(false);
        }
        //Debug.Log("Heuristic action:" + DiscreteActionsOut[0]);
    }
    void ProcessTurn(int aTurn, int aMove)
    {
        heading += turnForce * (float)aTurn;
        if (Mathf.Abs(aMove) > 0.00001f)
        {
            //Debug.Log("moving!");
        }
        if (Mathf.Abs(aTurn) > 0.00001f)
        {
            // Debug.Log("turining!");
        }
        float aMag = (transform.forward * moveForce * (float)aMove).magnitude;
        if (aMag > 1f)
        {
            //  Debug.Log("change vel:"+aMag);
        }
        rb.velocity += transform.forward * moveForce * (float)aMove;
        rb.transform.Rotate(0, -aTurn * 4.0f, 0);
        rb.angularVelocity = rb.angularVelocity * 0.9f;
    }
    void Fire()
    {
        if (isAlive)
        {
            double now = Time.time;
            hasTrishots = 0;
            if (now >= nextShotTime)
            {
                if (hasTrishots > 0)
                {
                    hasTrishots -= 1;
                    GameObject a1 = Instantiate(blast, blaster[1].transform.position, blaster[1].transform.rotation);
                    a1.transform.parent = xGame1.transform;
                    a1.tag = "playerWeapon";
                    GameObject a2 = Instantiate(blast, blaster[2].transform.position, blaster[2].transform.rotation);
                    a2.transform.parent = xGame1.transform;
                    a2.tag = "playerWeapon";
                }
                GameObject a3 = Instantiate(blast, blaster[0].transform.position, blaster[0].transform.rotation);
                a3.transform.parent = xGame1.transform;
                a3.tag = "playerWeapon";
                GetComponent<AudioSource>().Play();
                nextShotTime = now + 0.2;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        GameObject other = collision.gameObject;
        Debug.Log("Collision found ! isAlive:" + isAlive+"  otherTag:"+other.tag);
        if (isAlive)
        {
            // I've crashed into something
            if (other.gameObject.CompareTag("Asteroid"))
            {
                Debug.Log("asteroid collision!");
                Explode();
            }

            if (other.gameObject.CompareTag("Saucer"))
            {
                other.gameObject.SendMessage("PlayerBlast");
                Explode();
            }

        }
    }

    public void SetResetParameters()
    {
        Vector3 aVector = gameObject.transform.parent.transform.position + new Vector3(4, 0, -4);
        transform.position = aVector;
    }
    void ApplyPowerup(GameObject powerup)
    {
        GameObject.Find("GameController").SendMessage("DecrementAsteroid");

        PowerupCode code = powerup.GetComponent<PowerupCode>();

        if (code.type == "Life")
        {
            GameObject.Find("GameController").SendMessage("ExtraLife");
        }
        if (code.type == "Trishot")
        {
            hasTrishots = 12;
        }

        Destroy(powerup);
    }
    public void xAddReward(float inScore)
    {
        myReward += inScore;
        bigScore += inScore;
        SetReward(inScore);
        Debug.Log("bigScore:" + bigScore + "   inScore:" + inScore);
        scoreText.text = "Score: " + (int)(bigScore * 100f);
    }
    public float ShowReward()
    {
        return GetCumulativeReward();
    }
    void Explode()
    {

        hasTrishots = 0;


        GameObject a4 = Instantiate(explosion, transform.position, Quaternion.identity);
        a4.transform.parent = xGame1.transform;
        // Hide();
        xAddReward(-1f);
        killPlayer = true;


    }

    void HyperspaceJump()
    {
        Hide();
        HyperspaceIn();
    }

    void HyperspaceIn(bool center = false)
    {

        GetComponent<Rigidbody>().velocity = new Vector3(0.0f, 0.0f, 0.0f);
        GetComponent<Rigidbody>().rotation = Quaternion.Euler(0.0f, 0.0f, 0.0f);
        transform.rotation = Quaternion.identity;

        if (center)
        {
            // center
            transform.position = new Vector3(0.0f, 0.0f, 0.0f);
            heading = 0.0f;
        }
        else
        {
            // random
            float x, y, a;
            x = Random.Range(-15, 15);
            y = Random.Range(-8, 8);
            a = Random.Range(0.0f, 360.0f);

            transform.position = new Vector3(x, 0.0f, y);
            heading = a;
        }

        //ExplosionInfo info = new ExplosionInfo();
        // info.origin = transform.position;
        //info.strength = 0.1f;
        //GameObject.Find("GameController").SendMessage("MakeExplosion", info);
        Show();
    }

    void Hide()
    {
        GetComponent<Renderer>().enabled = false;
        GetComponent<BoxCollider>().enabled = false;
        isAlive = false;
    }

    void Show()
    {
        GetComponent<Renderer>().enabled = true;
        GetComponent<BoxCollider>().enabled = true;
        isAlive = true;
    }


}
