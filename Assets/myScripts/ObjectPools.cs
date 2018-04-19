using System.Collections.Generic;
using UnityEngine;

public class ObjectPools : MonoBehaviour {
    // ALLOWS ACCESS TO THE SCRIPT WITHOUT GET COMPONENT REFERENCE 
    public static ObjectPools SharedInstance;

    // Pool for collectables
    public List<GameObject> collectables;
    public int collectableSize;
    public GameObject collectable;

    // Pool for ammo
    public List<GameObject> ammos;
    public int ammoSize;
    public GameObject ammo;

    // Pool for ammo
    public List<GameObject> shields;
    public int shieldSize;
    public GameObject shield;

    // Pool for ammo
    public List<GameObject> healths;
    public int healthSize;
    public GameObject health;

    // Pool for enemy laser objects
    public List<GameObject> enemyLasers;
    public int enemyLaserPoolSize;
    public GameObject enemyLaser;

    // Pool for enemy bomb objects
    public List<GameObject> enemyBombs;
    public int enemyBombPoolSize;
    public GameObject enemyBomb;

    // Pool for enemy EMP objects
    public List<GameObject> enemyEMPs;
    public int enemyEMPPoolSize;
    public GameObject enemyEMP;

    // Pool for enemy laser objects
    public List<GameObject> playerLasers;
    public int playerLaserPoolSize;
    public GameObject playerLaser;


    // Pool for enemy objects
    public List<GameObject> enemies;
    public int enemiesSize;
    public GameObject enemyOne;
    public GameObject enemyTwo;
    public GameObject enemyThree;
    public GameObject enemyFour;

    // Pool for traps
    public List<GameObject> traps;
    public int trapSize;
    public GameObject trapOne;
    public GameObject trapTwo;
    public GameObject trapThree;
    public GameObject trapFour;

    // Pool for wall objects
    public List<GameObject> walls;
    public int wallsSize;
    public GameObject wallOne;
    public GameObject wallTwo;

    public GameObject go;

    void Awake() {
        SharedInstance = this;

        // collectable pool
        collectableSize = 20;
        collectables = new List<GameObject>();
        AddToPool(collectables, collectableSize, collectable);

        // ammo pool
        ammoSize = 20;
        ammos = new List<GameObject>();
        AddToPool(ammos, ammoSize, ammo);

        // shield pool
        shieldSize = 20;
        shields = new List<GameObject>();
        AddToPool(shields, shieldSize, shield);

        // health pool
        healthSize = 20;
        healths = new List<GameObject>();
        AddToPool(healths, healthSize, health);

        // Player laser pool
        playerLaserPoolSize = 20;
        playerLasers = new List<GameObject>();
        AddToPool(playerLasers, playerLaserPoolSize, playerLaser);

        // Enemy laser pool
        enemyLaserPoolSize = 25;
        enemyLasers = new List<GameObject>();
        AddToPool(enemyLasers, enemyLaserPoolSize, enemyLaser);

        // Enemy BOMB pool
        enemyBombPoolSize = 5;
        enemyBombs = new List<GameObject>();
        AddToPool(enemyBombs, enemyBombPoolSize, enemyBomb);

        // Enemy EMP pool
        enemyEMPPoolSize = 5;
        enemyEMPs = new List<GameObject>();
        AddToPool(enemyEMPs, enemyEMPPoolSize, enemyEMP);

        // Enemy pool
        enemiesSize = 80;
        enemies = new List<GameObject>();
        for (int i = 1; i <= enemiesSize; ) {
            enemies.Add(Instantiate(enemyFour));
            enemies.Add(Instantiate(enemyThree));
            enemies.Add(Instantiate(enemyTwo));
            enemies.Add(Instantiate(enemyOne));
            i += 4;
        }

        // Walls pool
        wallsSize = 550;
        walls = new List<GameObject>();
        for (int i = 0; i < wallsSize; ++i) {
            go = (i % 5== 0) ? Instantiate(wallOne) : Instantiate(wallTwo);
            walls.Add(go);
        }

        // Trap pool
        trapSize = 60;
        traps = new List<GameObject>();
        for (int i = 0; i < trapSize;) {
            traps.Add(Instantiate(trapOne));
            traps.Add(Instantiate(trapTwo));
            traps.Add(Instantiate(trapThree));
            traps.Add(Instantiate(trapFour));
            i += 4;
        }

        // Set all objects to inactve
        ResetPools();
    }

    void AddToPool(List<GameObject> list, int listSize, GameObject go) {
        for (int i = 0; i < listSize; ++i) {
            GameObject obj = Instantiate(go);
            list.Add(obj);
        }

    }

    public GameObject GetCollectable(int ranNum) {
        if (ranNum <= 60) return GetObjectFromPool(collectables);
        else if (ranNum > 60 && ranNum <= 70) return GetObjectFromPool(healths);
        else if (ranNum > 70 && ranNum <= 80) return GetObjectFromPool(ammos);
        else if (ranNum > 80 && ranNum <= 90) return GetObjectFromPool(shields);

        return null;
    }

    // Function tha gets the object from the correct list based on a string parameter
    public GameObject GetObject(string objType) {
        GameObject obj = null;
        if (objType.Equals("WALL")) obj = GetObjectFromPool(walls);
        else if (objType.Equals("ENEMY_LASER")) obj = GetObjectFromPool(enemyLasers);
        else if (objType.Equals("ENEMY_BOMB")) obj = GetObjectFromPool(enemyBombs);
        else if (objType.Equals("ENEMY_EMP")) obj = GetObjectFromPool(enemyEMPs);
        else if (objType.Equals("PLAYER_LASER")) obj = GetObjectFromPool(playerLasers);

        else if (objType.Equals("TRAP")) {
            for (int i = 0; i < traps.Count; ++i) {
                if (!(traps[i].tag == "TrapType_TWO") && !traps[i].activeInHierarchy) {
                    obj = GetObjectFromPool(traps, i);
                    break;
                }
            }
        } else if (objType.Equals("TRAP2")) {
            obj = null;
            for (int i = 0; i < traps.Count; ++i) {
                if (traps[i].tag == "TrapType_TWO" && !traps[i].activeInHierarchy) {
                    obj = GetObjectFromPool(traps, i);
                    break;
                }
            }
            if (obj == null) {
                obj = Instantiate(trapTwo);
                trapSize++;
                traps.Add(obj);
            }
        } else if (objType.Equals("ENEMY")) obj = GetObjectFromPool(enemies);

        return obj;
    }

    // Function that retrieves an object from the object pool or adds one to the pool if needed
    public GameObject GetObjectFromPool(List<GameObject> pool) {
        GameObject obj = null;
        for (int i = 0; i< pool.Count; i++) {
            if (!pool[i].activeInHierarchy) {
                obj = pool[i];
                obj.SetActive(true);
                return obj;
            } 
        }

        // Allow pool to grow in the unlikely event of all objects used from the pool
        if (obj == null) {
            obj = Instantiate(pool[0]);
            obj.SetActive(true);
            pool.Add(obj);
        }

        return obj;
    }

    // Function that retrieves an object from the object pool by index
    public GameObject GetObjectFromPool(List<GameObject> pool, int index) {
        pool[index].SetActive(true);
        return pool[index];
    }

    // Function that returns an enmy chosen by type
    public GameObject GetEnemyByType(int type) {
        GameObject temp = null;
        for (int i = 0; i < enemies.Count; ++i) {
            if (type == 1 && enemies[i].tag == "E1") 
                temp = enemies[i];
            else if (type == 2 && enemies[i].tag == "E2") 
                temp = enemies[i];
            else if (type == 3 && enemies[i].tag == "E3")
                temp = enemies[i];
            else if (type == 4 && enemies[i].tag == "E4")
                temp = enemies[i];

            if (temp != null && !temp.activeInHierarchy) break;
        }

        if (temp != null) temp.SetActive(true);
        return temp;
    }

    // Return a list of active enemies and traps
    public List<GameObject> GetActiveLevelObjects() {
        List<GameObject> active = new List<GameObject>();

        foreach (GameObject go in enemies) {
            if (go.activeInHierarchy) {
                active.Add(go);
            }
        }

        foreach (GameObject go in traps) {
            if (go.activeInHierarchy) {
                active.Add(go);
            }
        }
        return active;
    }

    // Function to reset active state of all objects in pools
    public void ResetPools() {
        foreach (GameObject go in enemyLasers) go.SetActive(false);
        foreach (GameObject go in enemyBombs) go.SetActive(false);
        foreach (GameObject go in enemyEMPs) go.SetActive(false);
        foreach (GameObject go in playerLasers) go.SetActive(false);
        foreach (GameObject go in walls) go.SetActive(false);
        foreach (GameObject go in enemies) go.SetActive(false);
        foreach (GameObject go in traps) go.SetActive(false);
        foreach (GameObject go in healths) go.SetActive(false);
        foreach (GameObject go in shields) go.SetActive(false);
        foreach (GameObject go in ammos) go.SetActive(false);
        foreach (GameObject go in collectables) go.SetActive(false);
    }
}
