using UnityEngine;

public class ExitCollisionController: MonoBehaviour {
    public LevelGen lg;
    public PuaseMenu pm;

    void Start() {
        lg = LevelGen.lg;
        pm = PuaseMenu.pm;
    }
     
    void OnTriggerEnter2D(Collider2D otherCol) {
        // Call new level function if player collides with exit
        if (otherCol.tag == "Player") LevelGen.lg.NewLevel();
    }

}
