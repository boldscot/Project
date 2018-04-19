/*
	@author Stephen Collins
	@date 30/10/2017
*/

using System.Collections.Generic;
using UnityEngine;

public class LevelGen : MonoBehaviour {
    public static LevelGen lg;
    public PuaseMenu pm;

    private const int TUTORIAL = 0;
	private const int START_ROOM=1;
	private const int PATH_LEFT=2;
	private const int PATH_RIGHT=3;
	private const int PATH_DOWN=4;
    private const int PATH_END = 5;
    private const int OFF_PATH=6;

    public int levelNum = 0;
    public int enemySpawnChance = 60;
    public bool spawnEnemy = false;

    // Struct used for tracking position of rooms
    public struct Position {
		public int row, col;
		public Position(int p1, int p2){
        	row = p1;
        	col = p2;
    	}
	}

	// Level Gen data structures
	public int[ , ] level;
	public List<Position> thePath = new List<Position>();

    //Game Objects
    public GameObject gc;
	public GameObject player;
    public GameObject circle;
	public GameObject wallAndGround;
	public GameObject wallAndGround2;
    public GameObject lev;
    public GameObject sp;
	public GameObject outerWall;
	public GameObject start;
    public GameObject exit;
    public GameObject camera;
    public Templates templates;

    //Leviathan spawn 
    public float spawnTime;
    public bool isSpawed;
    public Vector2 leviathanPos;
    public bool levCanSpawn;
    public float damageModifier;

    public int wallCount = 0;

    void Awake() {
    }
    

    void Start() {
        lg = this;
        pm = PuaseMenu.pm;

        level = new int[4, 4];		// level is represented by a 4x4 grid

        templates = GetComponent<Templates>();
        leviathanPos = lev.transform.position;
        Init();
    }

    public void Init() {
        lev.transform.position = leviathanPos;
        spawnTime = 10.0f;
        damageModifier = 1.0f;
        levCanSpawn = false;
        CreateOuterWall();
    }


	// Use this for initialization
	public void NewLevel() {
        if (GameObject.FindGameObjectsWithTag("sp").Length > 0)
            foreach(GameObject go in GameObject.FindGameObjectsWithTag("sp"))
                Destroy(go);

        if (levelNum < 9) ++levelNum;
        else {
            pm.DisplayHighScores();
        }

        isSpawed = false;
        
        wallCount = 0;
        ObjectPools.SharedInstance.ResetPools();
        //create a path through the level and then create the level
        thePath = createThePath();
        CreateLevel(-0.5f, 0.5f);
        // first staTe is ISALIVE
        LeviathanController.lc.state =0;
        // move to pos out of level
        lev.transform.position = leviathanPos;
        levCanSpawn = true;
        spawnTime = 10.0f;
        // Add active level objects to players enemy list 
        PlayerController.pc.activeEnemies = ObjectPools.SharedInstance.GetActiveLevelObjects();
        PlayerController.pc.activeEnemies.Add(lev);

        //----------FOR DEBUGGING---------------------------------------
        string theLevel= "";
		for (int r = 0; r < level.GetLength (0); ++r) {
			string str = "";
			for (int c = 0; c < level.GetLength (1); ++c) {
				str += "[" + level [r, c] + "], ";
			}
			theLevel+=str+"\n";
		}
		print (theLevel);

		string path = "";
		foreach(Position pos in thePath)
			path+="[" + pos.row+", " + pos.col +"], ";
		
		print (path);
        //-------------------------------------------------------------
    }

    void Update() {
        // spawn the leveiathan into the level at the start position
        if (levCanSpawn) {
            if (spawnTime <= 0.0f) {
                LeviathanController.lc.state += 1;
                lev.transform.position = start.transform.position;
                levCanSpawn = false;
                spawnTime = 10.0f;
            } else spawnTime -= Time.deltaTime;
        }

        if (Input.GetKeyUp(KeyCode.N)) {
            if (levelNum == 9) pm.DisplayHighScores();
            else NewLevel();
        }

        if (Input.GetKeyUp(KeyCode.T)) {
            CreateTutorialLevel();
        }

        if (levelNum > 0 && levelNum <= 3) {
            damageModifier = 1.0f;
            enemySpawnChance = 50;
        } else if (levelNum > 3 && levelNum <= 6) {
            damageModifier = 2.0f;
            enemySpawnChance = 75;
        } else if (levelNum > 6 && levelNum <= 9) {
            damageModifier = 3.0f;
            enemySpawnChance = 100;
        }
    }

	/*
	 * Function to create a path through the level 
	*/
	public List<Position> createThePath() {
		List<Position> path = new List<Position>();
		
		bool isPathComplete = false;
		int direction = 0;

		//Fill level with off path rooms
		for (int r = 0; r < level.GetLength (0); ++r)
			for (int c = 0; c < level.GetLength (1); ++c)
				level[r, c] = OFF_PATH;
		
		//Pick random room on top row and set it as start room
		Position nextRoom= new Position(0, Random.Range(0, 4));
		level[nextRoom.row, nextRoom.col] = START_ROOM;
		path.Add (nextRoom);
		//Loop until path is complete
		while(!isPathComplete) {
			// Get direction of path
			direction = Random.Range(PATH_LEFT, PATH_DOWN+1);

			switch (direction) {
			case PATH_DOWN:
                if (nextRoom.row < 3) {
                    nextRoom.row++; // move room position down to the next row
                    level[nextRoom.row, nextRoom.col] = PATH_DOWN;
                    path.Add(nextRoom);
                } else {
                    isPathComplete = true; // cant go any lower, path is complete
                    level[nextRoom.row, nextRoom.col] = PATH_END;
                }	
				break;
			case PATH_LEFT:
				if (nextRoom.col - 1 > 0) {
					nextRoom.col--; 	// move col position by -1
					if (level [nextRoom.row, nextRoom.col] == OFF_PATH) {
						level [nextRoom.row, nextRoom.col] = PATH_LEFT;
						path.Add (nextRoom);
					} else {
						nextRoom.col++; 	// revert earlier decrement
					}
				} else if (level [nextRoom.row, nextRoom.col + 1] == OFF_PATH) direction = PATH_RIGHT;
				else direction = PATH_DOWN;
				break;
			case PATH_RIGHT:
				if (nextRoom.col + 1 < 3) {
					nextRoom.col++; 	// move col position by 1
					if (level [nextRoom.row, nextRoom.col] == OFF_PATH) {
						level [nextRoom.row, nextRoom.col] = PATH_RIGHT;
						path.Add (nextRoom);
					} else {
						nextRoom.col--; 	// revert earlier increment
					}
				} else if (level [nextRoom.row, nextRoom.col - 1] == OFF_PATH) direction = PATH_LEFT;
				else direction = PATH_DOWN;
				break;
			}
		}
		return path;
	}

    /*
		Function to create a level
	*/
    public void CreateLevel(float startX, float startY) {
        float x = startX;
        float y = startY;
        int roomType = 0;
        int startIndex = 0;

        for (int r = 0; r < level.GetLength(0); r++) {
            for (int c = 0; c < level.GetLength(1); c++) {
                roomType = level[r, c];
                
                // create rooms based on the type of room on the path
                switch (roomType) {
                    case START_ROOM:
                        startIndex = (startIndex + 1 == PATH_DOWN) ? 5 : 0;
                        CreateRoom(templates.T_Start[Random.Range(startIndex, (startIndex + 5))], x, y, true);
                        break;
                    case PATH_LEFT:
                        if (c - 1 >= 0) {
                            if (r + 1 < 4) {
                                if (level[r + 1, c - 1] == PATH_DOWN)
                                    CreateRoom(templates.T_TypeOne[Random.Range(0, templates.T_TypeOne.Length)], x, y, true);
                                else
                                    CreateRoom(templates.T_TypeTwo[Random.Range(0, templates.T_TypeTwo.Length)], x, y, true);
                            } else {
                                CreateRoom(templates.T_TypeOne[Random.Range(0, templates.T_TypeOne.Length)], x, y, true);
                            }
                        } else CreateRoom(templates.T_TypeTwo[Random.Range(0, templates.T_TypeOne.Length)], x, y, true);

                        break;
                    case PATH_RIGHT:
                        if (c + 1 >= 0) {
                            if (r + 1 < 4) {
                                if (level[r + 1, c - 1] == PATH_DOWN)
                                    CreateRoom(templates.T_TypeOne[Random.Range(0, templates.T_TypeOne.Length)], x, y, true);
                                else
                                    CreateRoom(templates.T_TypeTwo[Random.Range(0, templates.T_TypeTwo.Length)], x, y, true);
                            } else {
                                CreateRoom(templates.T_TypeOne[Random.Range(0, templates.T_TypeOne.Length)], x, y, true);
                            }
                        } else
                            CreateRoom(templates.T_TypeTwo[Random.Range(0, templates.T_TypeOne.Length)], x, y, true);
                        break;
                    case PATH_DOWN:
                        CreateRoom(templates.T_TypeTwo[Random.Range(0, templates.T_TypeTwo.Length)], x, y, true);
                        break;
                    case OFF_PATH:
                        CreateRoom(RandomRoom(), x, y, false);
                        break;
                    case PATH_END:
                        CreateRoom(templates.T_Exit[Random.Range(0, templates.T_Exit.Length)], x, y, true);
                        break;
                }
                x += 10;
            }
            x = startX;
            y -= 8;
        }
        Debug.Log("WALL COUNT=" + wallCount);
    }

    /*
		Function for creating an outer wall around the level
	*/
    void CreateOuterWall() {
		//left + right wall
		float y=0.5f;
		float x=-1.5f;
		for (; y>= -31.5; y--) {
			Instantiate(outerWall, new Vector3 (x, y, 0f), Quaternion.identity);
			Instantiate(outerWall, new Vector3 (x+ 41, y, 0f), Quaternion.identity);
		}
		//top and bottom walls
		y=1.5f;
		for (; x< 40.5; x++) {
			Instantiate(outerWall, new Vector3 (x, y, 0f), Quaternion.identity);
			Instantiate(outerWall, new Vector3 (x, y-33, 0f), Quaternion.identity);
		}
	}

    /*
    *   Function that creates the tutorial area
    */
    public void CreateTutorialLevel() {
        levCanSpawn = false;
        //left + right wall
        float y = 0.5f;
        float x = -50.5f;
        for (; y >= -15.5; y--) {
            Instantiate(outerWall, new Vector3(x, y, 0f), Quaternion.identity);
            Instantiate(outerWall, new Vector3(x + 31, y, 0f), Quaternion.identity);
        }
        //top and bottom walls
        y = 1.5f;
        for (; x <= -19.5; x++) {
            Instantiate(outerWall, new Vector3(x, y, 0f), Quaternion.identity);
            Instantiate(outerWall, new Vector3(x, y - 17.0f, 0f), Quaternion.identity);
        }

        float xPos = -49.5f;
        float yPos = 0.5f;
        int index = 0;
        for (int r = 0; r < 2; ++r) {
            for (int c = 0; c < 3; ++c) {
                CreateRoom(templates.T_Tutorial[index], xPos, yPos, true);
                xPos += 10;
                index++;
            }
            xPos = -49.5f;
            yPos -= 8;
        }

        // Add active level objects to players enemy list 
        PlayerController.pc.activeEnemies = ObjectPools.SharedInstance.GetActiveLevelObjects();
    }


	/*
		Function to create a room from a string
	*/
	public void CreateRoom(string roomString, float x, float y, bool isOnPath) {
		float xPos= x;
		float yPos= y;
		float endPos= x+9;
        int tutorialEnemy = 1;
        GameObject obj = null;

        // parse the room string
		for (int i = 0; i< roomString.Length; i++) {
            if (roomString[i] == 'w') {
                Debug.Log("GOT A w");
                obj = ObjectPools.SharedInstance.GetObject("WALL");
                obj.transform.position = new Vector3(xPos, yPos, 0f);
                ++wallCount;
            } else if (roomString[i] == 'X') {
                Debug.Log("GOT A X");
                exit.transform.position = new Vector3(xPos, yPos, 0f);
            } else if (roomString[i] == 'E') {
                Debug.Log("GOT A E");
                if (levelNum == 0) {
                    spawnEnemy = true;
                } else if (GetRandumNumber(1, 100) <= enemySpawnChance) {
                    spawnEnemy = true;
                }

                // Get an enemy from the object pool
                if (spawnEnemy && levelNum == 0) {
                    ObjectPools.SharedInstance.GetEnemyByType(++tutorialEnemy%2+1).transform.position = new Vector3(xPos, yPos, 0f);
                    spawnEnemy = false;
                }

                // Get an enemy from the object pool
                if (spawnEnemy) {
                    ObjectPools.SharedInstance.GetObject("ENEMY").transform.position = new Vector3(xPos, yPos, 0f);
                    spawnEnemy = false;
                }

            } else if (roomString[i] == 'T' || roomString[i] == 't') {
                if (levelNum == 0) spawnEnemy = true;
                else if (GetRandumNumber(1, 100) <= enemySpawnChance) {
                    spawnEnemy = true;
                }

                if (spawnEnemy) {
                    if (roomString[i] == 'T')
                        ObjectPools.SharedInstance.GetObject("TRAP").transform.position = new Vector3(xPos, yPos + 0.4f, 0f);
                    else if (roomString[i] == 't')
                        ObjectPools.SharedInstance.GetObject("TRAP2").transform.position = new Vector3(xPos, yPos - 0.27f, 0f);

                    spawnEnemy = false;
                }
               
            } else if (roomString[i] == 's') {
                Debug.Log("GOT A S");
                start.transform.position = new Vector3(xPos, yPos, 0f);     //set start object position
                player.transform.position = new Vector3(xPos, yPos, 0f);    //set player object position
                camera.transform.position = new Vector3(xPos, yPos, -20f);  //set camera object position
            } else if (isOnPath && roomString[i] == '0') {
                Debug.Log("GOT A 0");
                //obj = Instantiate(sp, new Vector3(xPos, yPos, 0f), Quaternion.identity);
            } else if (roomString[i] == 'c') {
                Debug.Log("GOT A c");
                obj = ObjectPools.SharedInstance.GetCollectable(GetRandumNumber(1, 200));
                if (obj != null) obj.transform.position = new Vector3(xPos, yPos - 0.27f, 0f);
            }

            if (xPos < endPos) xPos++;
			else {
				xPos=x;
				yPos--;
			}
		} 
    }

    /*
        Function that creates a room with random placement of objects
    */
	public string RandomRoom() {
		int ranNum = 0;
		string roomString = "";
		const char Z = '0';

        // Room is 8 * 10 tiles
		char[,] room = new char[8, 10];

        // Place wall tiles randomly, greater chance of empty tiles in the center of the room
        for (int r = 0; r < room.GetLength(0); ++r) {
            for (int c = 0; c < room.GetLength(1); ++c) {
                if (r < 1 || r > 6) {
                    ranNum = Random.Range(0, 101);
                    room[r, c] = (ranNum < 80) ? 'w' : Z;
                } else if (c < 1 && r >= 1 || c > 8 && r <= 6) {
                    ranNum = Random.Range(0, 101);
                    room[r, c] = (ranNum < 60) ? 'w' : Z;
                } else {
                    ranNum = Random.Range(0, 101);
                    room[r, c] = (ranNum < 20) ? 'w' : Z;
                }
            }
        }

        // Randomly place enemies and traps
        for (int r = 0; r < room.GetLength(0); ++r) {
            for (int c = 0; c < room.GetLength(1); ++c) {
                if (c > 0 && r > 0) {
                    if (GetRandumNumber(1, 100) < 10 && GetRandumNumber(1, 100) > 5) {
                        if (room[r, c] == 'w' && room[r-1, c] == Z)
                            room[r-1, c] = 'E';
                    } else if (GetRandumNumber(1, 100) >= 10 && GetRandumNumber(1, 100) <= 13) {
                        if (r+1 < room.GetLength(0)) {
                            if (room[r, c] == 'w' && room[r + 1, c] == Z)
                                room[r + 1, c] = 'T';
                        }
                    } else if (GetRandumNumber(1, 100) <= 5 && GetRandumNumber(1, 100) > 2) {
                        if (r - 1 < room.GetLength(0)) {
                            if (room[r, c] == 'w' && room[r - 1, c] == Z)
                                room[r - 1, c] = 't';
                        }
                    } else if (GetRandumNumber(1, 100) >= 31 || GetRandumNumber(1, 100) < 35) {
                        if (r - 1 < room.GetLength(0)) {
                            if (room[r, c] == 'w' && room[r - 1, c] == Z)
                                room[r - 1, c] = 'c';
                        }
                    }
                }      
            }
        }
        
        // Build string to represent room
        for (int r = 0; r < room.GetLength(0); ++r)
            for (int c = 0; c < room.GetLength(1); ++c)
                roomString += room[r, c];
		
		return roomString;
	}

    // Function tnat returna a random number
    private int GetRandumNumber(int min, int max) {
        return Random.Range(min, max + 1);
    }
}
