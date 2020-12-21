using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;




public class AsteroidController : MonoBehaviour {

    // SCORING SYSTEM
    static int pts_AsteroidLarge  = 10;
    static int pts_AsteroidMedium = 20;
    static int pts_AsteroidSmall  = 50;
    static int pts_SaucerLarge = 200;
    static int pts_SaucerSmall = 500;
    public GameObject xGame1;
    static int pts_TimeBonusBeforeSeconds = 180;
    static int pts_TimeBonusPerSecond = 10;

  

    //static int bonus_every = 2000;

    static int[] level_numAsteroids = new int[] { 6, 6, 6, 6, 8, 8, 8, 8, 10 }; // ... ad infinitum
    static int[][] level_saucerAppearanceOdds = new int[][] {
        // large, small
        //new int[] { 0, 0 },
        new int[] { 1, 0 },
        new int[] { 1, 0 },
        new int[] { 3, 1 },
        new int[] { 3, 1 },
        new int[] { 1, 3 },
        new int[] { 1, 3 },
        new int[] { 0, 1 }  // ... ad infinitum
    };

    //static int bonus_every = 2000; // 2500;
   // public GameObject InnerGameBoundary;
    //public  static Bounds gameBoundary;
    public static bool init;

    public static int currentLevel;
    public static float score;

    VehicleCode refVehicle;
    //public int initialObstacles;
    public GameObject[] obstacles;
    public GameObject[] powerups;
    public GameObject player;
    public GameObject saucerLarge;
    public GameObject saucerSmall;

    //private Text scoreText=(Text)"placeholder";
    
  

    private int numAsteroids;
    private int playerLives;
    private float levelStartTime;
    private float gameoverTime;
   // private bool showCredits;

    private bool newSaucerAllowed;



    // Use this for initialization
    void Start() {

     
       // player= GameObject.Find("PlayerShip");
        refVehicle = (VehicleCode)player.GetComponent(typeof(VehicleCode));
       // Debug.Log(gameBoundary);
        init = true;
        ReinitLevel();
    }



    public void ReinitLevel() {

        GameObject[] aster = GameObject.FindGameObjectsWithTag("Asteroid");
        foreach (GameObject ast in aster)
        {
            Destroy(ast);
        }
        GameObject[] saucer = GameObject.FindGameObjectsWithTag("Saucer");
        foreach (GameObject ast in saucer)
        {
            Destroy(ast);
        }
        numAsteroids = 0;
        newSaucerAllowed = true;

        currentLevel += 1;
        Debug.Log("Current Level "+currentLevel);
        int initialObstacles = Random.Range(4,10);
        Debug.Log("Initializing "+initialObstacles+" asteroids.");
        for (int i = 0; i < initialObstacles; i++) {
            // placed anywhere within edges but outside a specified radius from where the player is
            //GameObject.Find("edges");
            // how can I get the params from inside "Edges"?
            float x, z;
            Vector3 delta;
            
                z = Random.Range(-15, 15);
                x = Random.Range(-5, 5);
                delta = player.transform.position - new Vector3(x, 0.0f, z);
            

            AsterInfo info = new AsterInfo();
            info.level = 1;
            info.position = new Vector3(x, 0.0f, z);
            NewAsteroid(info);
        }
        levelStartTime = Time.time;
        UpdateScore(0);
        SpawnSaucerAfterRandomDelay();
    }

    int getInitNumAsteroidsByLevel(int level) {
        int idx = level - 1;
        int len = level_numAsteroids.Length;
        if (idx >= len) return level_numAsteroids[len - 1];
        return level_numAsteroids[idx];
    }

    int[] getSaucerOddsByLevel(int level) {
        int idx = level - 1;
        int len = level_saucerAppearanceOdds.Length;
        if (idx >= len) return level_saucerAppearanceOdds[len - 1];
        return level_saucerAppearanceOdds[idx];
    }

    void NewAsteroid(AsterInfo info) {
        numAsteroids += 1;

        // random motion is imparted when the new obstacle calls Start()
        GameObject obstacle = Instantiate(
            obstacles[Random.Range(0, obstacles.Length)],
            info.position, Random.rotation) as GameObject;
   
        obstacle.transform.parent = this.transform;
        obstacle.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        obstacle.transform.localPosition = info.position;
        float aRand = 25;
        Vector3 astVel = new Vector3(Random.Range(-aRand, aRand), Random.Range(-aRand, aRand), Random.Range(-aRand, aRand));

        obstacle.GetComponent<Rigidbody>().velocity = astVel;
        
      //  obstacle.SendMessage("SetLevel", info.level);
    }

    void NewPowerup(AsterInfo info) {
        numAsteroids += 1; // its fine to count this as an asteroid

        // random motion is imparted when the new obstacle calls Start()
        GameObject a1=Instantiate(
            powerups[Random.Range(0, powerups.Length)],
            info.position, Random.rotation);
        a1.transform.parent = xGame1.transform;

    }


    /*
    void MakeExplosion(ExplosionInfo info) {
        GameObject[] objs = GameObject.FindObjectsOfType<GameObject>();
        for (int i=0;i<objs.Length;i++) {
            Rigidbody rb = objs[i].GetComponent<Rigidbody>();
            if (rb) {
                rb.AddExplosionForce(info.strength, info.origin, 10f, 0f, ForceMode.Impulse);
            }
        }
    }
    */
    void UpdateScore(int points) {

        
        //long before = score / bonus_every;
        refVehicle.xAddReward(points*0.01f);
        // score += points;
        score = refVehicle.ShowReward();
       // long after = score / bonus_every;


        string display = "Score: " + score.ToString();

       // Debug.Log("Score:" + score);

      //  scoreText.text = display + "\n " + lives;
       // scoreText.text = display + "\n " + lives;

     //   scoreText.enabled = true;
        //replayText.enabled = false;

    }

    void ScoreAsteroid(int astLevel) {
        switch ( astLevel ) {
            case 1:
                UpdateScore(pts_AsteroidLarge);
                break;
            case 2:
                UpdateScore(pts_AsteroidMedium);
                break;
            case 3:
                UpdateScore(pts_AsteroidSmall);
                break;
        }
    }

    void DecrementAsteroid() {
        numAsteroids -= 1;
        // if saucer is allowed that means its not here
        if (numAsteroids == 0 && newSaucerAllowed == true) LevelCleared();
    }

    void ScoreSaucer(bool isSmall) {

        newSaucerAllowed = true;

        if (isSmall) {
            UpdateScore(pts_SaucerSmall);
        }
        else {
            UpdateScore(pts_SaucerLarge);
        }

        if (numAsteroids == 0) {
            LevelCleared();
        }
        else {
            SpawnSaucerAfterRandomDelay();
        }
    }

    void SpawnSaucerAfterRandomDelay() {
        // ater a random time, spawn a saucer
        float wait = 30.0f * Random.Range(1.0f, 2.0f) / Mathf.Pow(currentLevel,0.3f);
        Invoke("SpawnSaucer", wait);
    }

    void SpawnSaucer() {
        if (newSaucerAllowed == false) return;

        // decide whether to spawn Large or Small
        GameObject saucer;
        int[] odds = getSaucerOddsByLevel(currentLevel);

        Debug.Log("choosing between " + odds[0]+","+odds[1]);

        int range = odds[0] + odds[1];
        float value = Random.value * range; 
        if (range > 0) {
            if (value < odds[0]) {
                saucer = saucerLarge;
            }
            else {
                saucer = saucerSmall;
            }
            GameObject aSaucer=Instantiate(saucer);
            aSaucer.transform.parent = xGame1.transform;
            newSaucerAllowed = false;
        }
    }

    IEnumerator CountTimeBonus(int leveltime, int convergetime) {
        string clear = "Level Cleared!";
        string message = clear;

        while (leveltime < convergetime) {
            string min = (convergetime / 60).ToString();
            string sec = (convergetime % 60).ToString();
            if (sec.Length < 2) sec = "0" + sec;
            convergetime -= 1;
            message = clear + "\nTime bonus " + min + ":" + sec;
            //gameoverText.text = message;
            UpdateScore(pts_TimeBonusPerSecond);
            yield return new WaitForSeconds(0.05f);
        }
       
        Invoke("ReinitLevel", 3);
    }

    void LevelCleared() {
        newSaucerAllowed = false;

        int leveltime = (int)(Time.time - levelStartTime);
       
        StartCoroutine( CountTimeBonus(leveltime,pts_TimeBonusBeforeSeconds) );
    }


}
