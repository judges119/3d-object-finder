using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Leap;

public class LeapMovement : MonoBehaviour {
	
	//Private variables
	Finger[] pointerFinger = new Finger[2];
	/*Vector3 beginningLocation = new Vector3();
	float oldDistance = -1;*/
	
	// Use this for initialization
	void Start () {
		pointerFinger[0] = null;
		pointerFinger[1] = null;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rotateCamera();
		translateCamera();
	}
	
	//Move camera based on movement of a controlling hand
	void rotateCamera() {
		int availableHands = 0;
		foreach (LeapConnect2.HandStruct h in LeapConnect2.hands) { //For each hand...
			if (h.handUpd != null) { //If it's not null...
				availableHands++; //Increment available hands value
			}
		}
		if (availableHands == 1) {
			foreach (LeapConnect2.HandStruct h in LeapConnect2.hands) { //For each hand...
				if (h.handUpd != null) { //If it's not null...
					if (h.handUpd.Fingers.Count > 3) { //If there's more than three fingers...
						if (h.handUpd.RotationAxis(LeapConnect2.lastFrame).z >= 0) {
							gameObject.transform.Rotate(Vector3.forward, h.handUpd.RotationAngle(LeapConnect2.lastFrame) * 50); //Rotate around the forward (z-axis local to main camera) axis by the angle of the hand's rotation (compared to last frame)
						} else {
							gameObject.transform.Rotate(Vector3.forward * -1, h.handUpd.RotationAngle(LeapConnect2.lastFrame) * 50); //Rotate around the backward (z-axis local to main camera) axis by the angle of the hand's rotation (compared to last frame)
						}
						if (h.handUpd.TranslationProbability(LeapConnect2.lastFrame) > 0.5) { //If probability of translation is greater than 50%...
							gameObject.transform.Rotate(Vector3.up, h.handUpd.Translation(LeapConnect2.lastFrame).x / 5); //Horizontal translation equals y-axis rotation
							gameObject.transform.Rotate(Vector3.left, h.handUpd.Translation(LeapConnect2.lastFrame).y / 5); //Vertical translation equals x-axis rotation
						}
					}
				}
			}
		}
	}
	
	//Move camera based on instance/distance of two open hands
	void translateCamera() {
		float[] closestZHands = new float[3] {-1, -1, 10}; //Array to hold pair of hands with smallest z-axis difference, difference set higher than possible normalised values
		for (int i = 0; i < LeapConnect2.hands.Length; i++) { //Compare each hand
			for (int j = i + 1; j < LeapConnect2.hands.Length; j++) { //Against each other hand in the LeapConnect2.hands array, without doubling up on comparisons
				if ((LeapConnect2.hands[i].handUpd != null) && (LeapConnect2.hands[j].handUpd != null)) { //If both hands are not null
					if ((LeapConnect2.hands[i].handUpd.Fingers.Count > 3) && (LeapConnect2.hands[j].handUpd.Fingers.Count > 3)) { //If both hands have more than three fingers showing
						if (closestZHands[2] > Mathf.Abs(LeapConnect2.hands[i].boxPosition.z - LeapConnect2.hands[j].boxPosition.z)) { //If stored record of highest z-axis difference is greater than new z-axis difference...
							closestZHands[0] = i; //Update record with new hand references and z-axis difference (absolute)
							closestZHands[1] = j; 
							closestZHands[2] = Mathf.Abs(LeapConnect2.hands[i].boxPosition.z - LeapConnect2.hands[j].boxPosition.z);
						}
					}
				}
			}
		}
		if (closestZHands[0] >= 0) { //If closestZHands has real values (not initial useless ones)
			Vector firstHand = new Vector(LeapConnect2.hands[(int) closestZHands[0]].boxPosition.x, LeapConnect2.hands[(int) closestZHands[0]].boxPosition.y, 0); //First vector without z-axis
			Vector secondHand = new Vector(LeapConnect2.hands[(int) closestZHands[1]].boxPosition.x, LeapConnect2.hands[(int) closestZHands[1]].boxPosition.y, 0); //Second vector without z-axis
			float distance = firstHand.DistanceTo(secondHand); //Distance between both vectors
			gameObject.transform.Translate(Vector3.forward * distance / 5); //Move camera forward by distance
		}
	}
}
