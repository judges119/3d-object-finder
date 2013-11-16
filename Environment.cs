using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {
	
	//Private variables
	GameObject[] enviroObjects = new GameObject[50]; //Extra objects in scene
	GameObject targetObject; //Target object in scene
	
	// Use this for initialization
	void Start () {
		GameObject tempObject; //Temporary object
		tempObject = GameObject.CreatePrimitive(PrimitiveType.Sphere); //Sphere type for target object
		targetObject = Instantiate(tempObject) as GameObject; //Create a sphere for the target object
		targetObject.transform.position = new Vector3(Random.value * 100 - 50, Random.value * 100 - 50, Random.value * 100 - 50); //Set location of target object sphere
		tempObject = GameObject.CreatePrimitive(PrimitiveType.Cube); //Cube type for target object
		for (int i = 0; i < 50; i++) {
			enviroObjects[i] = Instantiate(tempObject) as GameObject; //Create cube objects for each extra object
			enviroObjects[i].transform.position = new Vector3(Random.value * 100 - 50, Random.value * 100 - 50, Random.value * 100 - 50); //Set locations of extra objects
		}
	}
	
	// Update is called once per frame
	void Update () {
		foreach (LeapConnect2.FingerStruct f in LeapConnect2.fingers) { //For each finger
			if (f.fingerUpd != null) { //If the finger isn't null
				if (f.fingerObj.renderer.bounds.Intersects(targetObject.renderer.bounds)) { //If the finger collides with target sphere
					targetObject.renderer.material.color = new Color(1, 0, 0, 0); //Set the target sphere colour to red
				}
			}
		}
	}
}