using UnityEngine;
using System.Collections;

public class BlastCode : MonoBehaviour {
    public GameObject playerShip;
    private float speed;

	// Use this for initialization
	void Start () {
        speed = 30.0f*transform.lossyScale.y;
    
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 position = transform.position;
        transform.position = position + transform.forward * speed*Time.fixedDeltaTime;
	}

    void OnCollisionEnter(Collision other) {
        if ( other.gameObject.CompareTag("Asteroid") ) {
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
        if ( other.gameObject.CompareTag("Saucer") ) {
            other.gameObject.SendMessage("PlayerBlast");
            this.transform.parent.gameObject.GetComponent<AsteroidController>().SendMessage("ScoreSaucer", true); 
            //remove this bullet
            Destroy(gameObject);
            return;
        }
    }

}
