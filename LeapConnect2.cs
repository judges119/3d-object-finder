using UnityEngine;
using System.Collections;
using Leap;

public class LeapConnect2 : MonoBehaviour {
	
	//Public Variables
	public GameObject m_PalmTemplate; //The template object to use for the palm
	public GameObject m_FingerTemplate; //The template object to use for fingers
	public static Frame m_Frame	= null; //Frame object for access to all leap data in the frame
	public static Frame lastFrame = null; //Frame object holding all data for previous frame
	public static HandStruct[] hands = new HandStruct[3]; //Structure to hold hand data, static for easy external access
	public static FingerStruct[] fingers = new FingerStruct[10]; //Structure to hold finger data, static for easy external access
	public static GestureList gestures = new GestureList(); //Holds a list of all the gestures in the current frame
	public static InteractionBox interactionBox = new InteractionBox(); //Interaction box for normalising vectors in Leap view
	
	public struct HandStruct { //Struct to store hand data, includes  Leap (handUpd), Unity object (handObj) and palm position in Leap interaction box
		public Hand handUpd;
		public GameObject handObj;
		public Vector boxPosition;
	}
	
	public struct FingerStruct { //Struct to store finger data, includes Leap (fingerUpd), Unity object (fingerObj) and tip position in Leap interaction box
		public Finger fingerUpd;
		public GameObject fingerObj;
		public Vector boxPosition;
	}
	
	//Private Variables
	static Leap.Controller 		m_controller	= new Leap.Controller();
	
	//Catch latest frame
	public static Leap.Frame Frame {
		get { return m_Frame; }
	}
	
	//Use this for initialization
	void Start () {
		for (int i = 0; i < hands.Length; i++) {
			hands[i].handUpd = null;
			hands[i].handObj = null;
		}
		for (int i = 0; i < fingers.Length; i++) {
			fingers[i].fingerUpd = null;
			fingers[i].fingerObj = null;
		}
		
		//m_controller.EnableGesture(Gesture.GestureType.TYPECIRCLE);
		//m_controller.EnableGesture(Gesture.GestureType.TYPESCREENTAP);
	}
	
	//Update is called once per frame
	void Update () {
		if (m_controller != null) {
			lastFrame = m_Frame == null ? Frame.Invalid : m_Frame;
			m_Frame	= m_controller.Frame();
		}
		
		//Get interaction box for this frame
		interactionBox = m_Frame.InteractionBox;
		
		//Deal with hands
		addHands();
		updateHands();
		removeHands();
		
		//Deal with fingers
		addFingers();
		updateFingers();
		removeFingers();
		
		//Deal with gestures
		addGestures();
	}
	
	//Add hands, up to a maximum of three (2 real, one accidental)
	private void addHands() {
		foreach (Hand h in m_Frame.Hands) { //For each hand in Leap frame, if not existing in Unity hands array, add to spot (if available) in hand structure and create object for it
			bool newhand = true;
			for (int i = 0; i < hands.Length; i++) {
				if (hands[i].handUpd != null) {
					if (hands[i].handUpd.Id == h.Id) {
						newhand = false;
					}
				}
			}
			if (newhand) {
				for (int i = 0; i < hands.Length; i++) {
					if (hands[i].handUpd == null) {
						hands[i].handUpd = h;
						hands[i].handObj = Instantiate (m_PalmTemplate) as GameObject;
						hands[i].handObj.transform.parent = gameObject.transform;
						hands[i].boxPosition = interactionBox.NormalizePoint(h.PalmPosition, true);
						break;
					}
				}
			}
		}
	}
	
	//Update hands that are in view
	private void updateHands() {
		for (int i = 0; i < hands.Length; i++) {	//For each hand in Unity hands array, update HandStruct.handUpd info and position/rotation of HandStruct.handObj in scene
			if (hands[i].handUpd != null) {
				foreach (Hand h in m_Frame.Hands) {
					if (hands[i].handUpd.Id == h.Id) {
						hands[i].handUpd = h;
						hands[i].boxPosition = interactionBox.NormalizePoint(h.PalmPosition, true);
					}
				}
				Vector normalisedHand = interactionBox.NormalizePoint(hands[i].handUpd.PalmPosition, true);
				hands[i].handObj.transform.localPosition = new Vector3((normalisedHand.x * 10) - 5, (normalisedHand.y * 10) - 5, (-1 * normalisedHand.z * 10) + 15);
				hands[i].handObj.transform.eulerAngles = new Vector3(hands[i].handObj.transform.eulerAngles.z, -1 * (180 / Mathf.PI * hands[i].handUpd.Direction.Pitch), 180 / Mathf.PI * hands[i].handUpd.PalmNormal.Roll);
			}
		}
	}
	
	//Remove unnecessary hands as they disappear from view
	private void removeHands() {
		for (int i = 0; i < hands.Length; i++) {
			bool handlost = true;
			if (hands[i].handUpd != null) {
				foreach (Hand h in m_Frame.Hands) {
					if (hands[i].handUpd.Id == h.Id) {
						handlost = false;
					}
				}
			}
			if (handlost) { //If hand is not in frame, destroy the gameObject for it and set both HandStruct properties to null
				hands[i].handUpd = null;
				GameObject.Destroy(hands[i].handObj);
				hands[i].handObj = null;
				hands[i].boxPosition = null;
			}
		}
	}
	
	//Add fingers, up to a maximum of 10
	private void addFingers() {
		foreach (Finger f in m_Frame.Fingers) { //For each finger in Leap frame, if it's new, add it to fingers array (if spot available) and create object for it
			bool newfinger = true;
			for (int i = 0; i < fingers.Length; i++) {
				if (fingers[i].fingerUpd != null) {
					if (fingers[i].fingerUpd.Id == f.Id) {
						newfinger = false;
					}
				}
			}
			if (newfinger) {
				for (int i = 0; i < fingers.Length; i++) {
					if (fingers[i].fingerUpd == null) {
						fingers[i].fingerUpd = f;
						fingers[i].fingerObj = Instantiate (m_FingerTemplate) as GameObject;
						fingers[i].fingerObj.transform.parent = gameObject.transform;
						fingers[i].boxPosition = interactionBox.NormalizePoint(f.TipPosition, true);
						break;
					}
				}
			}
		}
	}
	
	//Update fingers that are in view
	private void updateFingers() {
		for (int i = 0; i < fingers.Length; i++) { //For each finger in Unity fingers array, update FingerStruct.fingerUpd info with new Leap frame info and FingerStruct.fingerObj location in scene
			if (fingers[i].fingerUpd != null) {
				foreach (Finger f in m_Frame.Fingers) {
					if (fingers[i].fingerUpd.Id == f.Id) {
						fingers[i].fingerUpd = f;
						fingers[i].boxPosition = interactionBox.NormalizePoint(f.TipPosition, true);
					}
				}
				Vector normalisedFinger = interactionBox.NormalizePoint(fingers[i].fingerUpd.TipPosition, true);
				fingers[i].fingerObj.transform.localPosition = new Vector3((normalisedFinger.x * 10) - 5, (normalisedFinger.y * 10) - 5, (-1 * normalisedFinger.z * 10) + 15);
				//Don't need this part, can be used for visual identification if your project invokes the adaptive touch plane
				/*if (fingers[i].fingerUpd.TouchDistance > 0) {
					fingers[i].fingerObj.renderer.material.color = new Color((float) 0, (float) (1 - fingers[i].fingerUpd.TouchDistance), (float) 0, (float) 0.2);
				} else {
					fingers[i].fingerObj.renderer.material.color = new Color((float) (fingers[i].fingerUpd.TouchDistance + 1), (float) 0, (float) 0, (float) 0.2);
				}*/
			}
		}
	}
	
	//Remove unnecessary fingers as they disappear from view
	private void removeFingers() {
		for (int i = 0; i < fingers.Length; i++) {
			bool fingerlost = true;
			if (fingers[i].fingerUpd != null) {
				foreach (Finger f in m_Frame.Fingers) {
					if (fingers[i].fingerUpd.Id == f.Id) {
						fingerlost = false;
					}
				}
			}
			if (fingerlost) { //If finger not in Leap frame, destroy finger object and set FingerStruct properties to null
				fingers[i].fingerUpd = null;
				GameObject.Destroy(fingers[i].fingerObj);
				fingers[i].fingerObj = null;
				fingers[i].boxPosition = null;
			}
		}
	}
	
	//Load all gestures from the new frame
	void addGestures() {
		gestures = m_Frame.Gestures();	
	}
}
