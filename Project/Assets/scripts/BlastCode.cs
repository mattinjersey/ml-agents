using UnityEngine;
using System.Collections;

public class BlastCode : MonoBehaviour {
    public GameObject playerShip;
    private float speed;
    Rigidbody rb;
	// Use this for initialization
	void Start () {
        speed = 15.0f;
        this.GetComponent<Rigidbody>().velocity = speed * transform.forward;
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        // Vector3 position = transform.position;
        // transform.position = position + transform.forward * speed*Time.fixedDeltaTime;
        this.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
        Vector3 aPos = transform.position;
        transform.position = new Vector3(aPos.x, 0, aPos.z);
        float x = rb.velocity.x;
        float y = rb.velocity.y;
        float z = rb.velocity.z;
        rb.velocity = new Vector3(x, 0, z);
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("wall"))
        {

            Destroy(gameObject);
            return;
        }
        Debug.Log("collison:" + this.name);
        if (this.name.Contains("enemy") )
            {
            if (other.gameObject.CompareTag("Asteroid"))
            {
                other.gameObject.GetComponent<AsteroidCode>().Break(false);
                //remove this bullet
                Destroy(gameObject);
                return;
            }
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.SendMessage("xSetKillPlayer");
                Destroy(gameObject);
                return;
            }
        }
        else
        {
            if (other.gameObject.CompareTag("Asteroid")) {
                other.gameObject.SendMessage("BreakByPlayer");
                //remove this bullet
                Destroy(gameObject);
                return;
            }
            if (other.gameObject.CompareTag("wall"))
            {

                Destroy(gameObject);
                return;
            }
            if (other.gameObject.CompareTag("Saucer")) {
                other.gameObject.SendMessage("PlayerBlast");
                this.transform.parent.gameObject.GetComponent<AsteroidController>().SendMessage("ScoreSaucer", true);
                //remove this bullet
                Destroy(gameObject);
                return;
            }
        }
    }

}
