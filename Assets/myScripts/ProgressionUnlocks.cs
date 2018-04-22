/*
 * @ author: stephen collins
 * @ student number: 20061696
 * @ date: 22/04/2018
 * @ brief: This file is part of the source code for the game Isolation
*/

using UnityEngine.UI;
using UnityEngine;

public class ProgressionUnlocks : MonoBehaviour {
    public PlayerController pc;
    public static ProgressionUnlocks pu;

    public Button shieldUpgradeOne, shieldUpgradeTwo, shieldUpgradeThree;
    public Button boostUpgradeOne, boostUpgradeTwo, boostUpgradeThree;
    public Button healthUpgradeOne, healthUpgradeTwo, healthUpgradeThree;
    public Button auto, single, burst;
    public bool picked;	
	
	// Update is called once per frame
	void Start () {
        pu = this;
        pc = PlayerController.pc;
        picked = false;
	}

    public void ResetUnlocks() {
        boostUpgradeOne.GetComponent<Button>().interactable = true;
        boostUpgradeTwo.GetComponent<Button>().interactable = false;
        boostUpgradeThree.GetComponent<Button>().interactable = false;

        shieldUpgradeOne.GetComponent<Button>().interactable = true;
        shieldUpgradeTwo.GetComponent<Button>().interactable = false;
        shieldUpgradeThree.GetComponent<Button>().interactable = false;

        healthUpgradeOne.GetComponent<Button>().interactable = true;
        healthUpgradeTwo.GetComponent<Button>().interactable = false;
        healthUpgradeThree.GetComponent<Button>().interactable = false;

        single.GetComponent<Button>().interactable = true;
        auto.GetComponent<Button>().interactable = true;
        burst.GetComponent<Button>().interactable = false;
    }

    // function thats sets the booster modifier
    public void AdjustBooster(float modifier) {
        pc.boostModifier = modifier;

        // toggle the buttons based on the upgrades that have been chosen already
        if (modifier == 0.75f) {
            boostUpgradeOne.GetComponent<Button>().interactable = false;
            boostUpgradeTwo.GetComponent<Button>().interactable = true;
        } else if (modifier == 0.5f) {
            boostUpgradeTwo.GetComponent<Button>().interactable = false;
            boostUpgradeThree.GetComponent<Button>().interactable = true;
        } else boostUpgradeThree.GetComponent<Button>().interactable = false;

        picked = true;
    }

    // function thats sets the shield modifier
    public void AdjustShield(float modifier) {
        pc.shieldModifier = modifier;

        if (modifier == 0.75f) {
            shieldUpgradeOne.GetComponent<Button>().interactable = false;
            shieldUpgradeTwo.GetComponent<Button>().interactable = true;
        } else if (modifier == 0.5f) {
            shieldUpgradeTwo.GetComponent<Button>().interactable = false;
            shieldUpgradeThree.GetComponent<Button>().interactable = true;
        } else shieldUpgradeThree.GetComponent<Button>().interactable = false;

        picked = true;
    }

    // function thats sets the shield modifier
    public void AdjustHealth(float modifier) {
        pc.healthModifier = modifier;

        if (modifier == 0.75f) {
            healthUpgradeOne.GetComponent<Button>().interactable = false;
            healthUpgradeTwo.GetComponent<Button>().interactable = true;
        } else if (modifier == 0.5f) {
            healthUpgradeTwo.GetComponent<Button>().interactable = false;
            healthUpgradeThree.GetComponent<Button>().interactable = true;
        } else healthUpgradeThree.GetComponent<Button>().interactable = false;

        picked = true;
    }

    // function thats sets the laser rifle values
    public void ChangeFireModeAuto(int mode) {
        pc.SetAttackModifiers(mode);

        if (mode == 1) {
            single.GetComponent<Button>().interactable = false;
            auto.GetComponent<Button>().interactable = true;
            burst.GetComponent<Button>().interactable = true;
        } else if (mode == 3) {
            single.GetComponent<Button>().interactable = true;
            auto.GetComponent<Button>().interactable = true;
            burst.GetComponent<Button>().interactable = false;
        } else {
            single.GetComponent<Button>().interactable = true;
            auto.GetComponent<Button>().interactable = false;
            burst.GetComponent<Button>().interactable = true;
        }

        picked = true;
    }
}
