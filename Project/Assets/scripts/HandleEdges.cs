using UnityEngine;
using System.Collections;

public class HandleEdges : MonoBehaviour {

    public float DeltaX;
    public float DeltaY;


    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update () {
	
	}

    void OnTriggerExit(Collider other) {

            if (other.gameObject.name.Contains("Weapon")) {
                Destroy(other.gameObject);
                return;
            }
            if (other.gameObject.CompareTag("playerBlaster") || other.gameObject.CompareTag("playerThruster") || other.gameObject.CompareTag("playerEngine"))
            {
                return;
            }
        // wrap it back to the opposite edge
        Vector3 position = other.transform.position;
        float extra = 0.0f;

        if (other.gameObject.CompareTag("Asteroid"))
        {
            extra = CalcSize(other.gameObject); // what is the size of this asteroid?
        }
        if (name == "left")
        {
            if (other.gameObject.CompareTag("playerShip") )
                {
                Debug.Log("left collide"+position.x);
            }
                position.x += (DeltaX) + extra;
        }
        else if (name == "right")
        { 
            position.x -= (DeltaX) + extra;
            if (other.gameObject.CompareTag("playerShip"))
            {
                Debug.Log("right collide");
            }
        }
        else if (name == "bottom")
        {
            position.y += (DeltaY) + extra;
        }
        else if (name == "bottom")
        {
            position.y -= (DeltaY) + extra;
        }

        other.transform.position = position;

    }

    float CalcSize(GameObject obj) {
        //return obj.GetComponent<Collider>().height * obj.transform.localScale.y;
        return 3f;
    }

}
