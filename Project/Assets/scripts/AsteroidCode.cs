using UnityEngine;
using System.Collections;

public class AsterInfo {
    public int level;
    public Vector3 position;
}

public class AsteroidCode : MonoBehaviour {

    public GameObject finalExplosion;

    public AudioSource sndLevel1;
    public AudioSource sndLevel2;
    public AudioSource sndLevel3;

    private int level;
    private Rigidbody rb;
    private Collider cc;
 //   private float outsideCount;

	// Use this for initialization
	void Start () {
        cc = GetComponent<BoxCollider>();
        rb = GetComponent<Rigidbody>();
        rb.angularVelocity = Random.insideUnitSphere * Random.Range(.5f,4.0f);
        rb.velocity = new Vector3(
            Random.Range(0.0f, 1.0f),
            0.0f,
            Random.Range(0.0f, 1.0f)
            );
    }
	
	void FixedUpdate () {
        // limit the velocity
        float speed = Mathf.Clamp(rb.velocity.magnitude, 2f, 25.0f);
        rb.velocity = rb.velocity.normalized * speed;
        float x = rb.velocity.x;
        float y = rb.velocity.y;
        float z = rb.velocity.z;
        rb.velocity = new Vector3(x, 0, z);
        Vector3 aPos = transform.position;
        transform.position = new Vector3(aPos.x, 0,aPos.z);
    }

    void RedirectInwards() {
        //Debug.Log("GET THERE!");

        float dx = transform.position.x;
        float dy = transform.position.z;

        // direct bearing to center of screen (in radians)
        float angle = Mathf.Atan2(-dx, -dy);
        float speed = Random.Range(1.0f, 3.0f);

        // apply variance to angle
        angle += Random.Range(-45 * Mathf.Deg2Rad, 45 * Mathf.Deg2Rad);

        float x = Mathf.Sin(angle) * speed;
        float y = Mathf.Cos(angle) * speed;

        rb.velocity = new Vector3(x, 0.0f, y);
       
    }

    void SetLevel(int n) {
        level = n;
        float mult = 1 / Mathf.Pow(2, n - 1);
        // resize according to level
        transform.localScale = transform.localScale * mult;

        rb = GetComponent<Rigidbody>();
        rb.mass = rb.mass * mult*mult*mult; 
    }
    
    void Break(bool scorePoints) {
        AsterInfo info = new AsterInfo();
        info.level = level + 1;
        info.position = transform.position;
        Debug.Log("asteroid hit. Level:" + level);
        if ( level<3 ) {
            GameObject g = this.transform.parent.gameObject;
            //Debug.Log(g);
            g.GetComponent<AsteroidController>().NewAsteroid(info);
        }
        else {
           // Instantiate(finalExplosion, transform.position, Quaternion.identity);
        }

        if (scorePoints)
        {
            GameObject g;
            g = this.transform.parent.gameObject;
            g.GetComponent<AsteroidController>().SendMessage("ScoreAsteroid", info.level-1);
        }
            Explode();
    }

    void BreakByPlayer() {
        Break(true);
    }

    void BreakByEnemy() {
        Break(false);
    }

    void Explode() {

        //   Debug.Log("destroy asteroid..." + this.transform.parent.name);
        AsteroidController g;
        g = this.transform.parent.gameObject.GetComponent<AsteroidController>();
        g.DecrementAsteroid();
        Destroy(gameObject);
    }


}
