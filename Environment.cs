using UnityEngine;
using System.Collections;

public class Environment : MonoBehaviour {
	
	//Private variables
	GameObject[] enviroObjects = new GameObject[50];
	GameObject targetObject;
	
	// Use this for initialization
	void Start () {
		GameObject tempObject;
		tempObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		targetObject = Instantiate(tempObject) as GameObject; 
		tempObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
		for (int i = 0; i < 50; i++) {
			enviroObjects[i] = Instantiate(tempObject) as GameObject;
			enviroObjects[i].transform.position = new Vector3(Random.value * 100 - 50, Random.value * 100 - 50, Random.value * 100 - 50);
		}
		targetObject.transform.position = new Vector3(Random.value * 100 - 50, Random.value * 100 - 50, Random.value * 100 - 50);
	}
	
	// Update is called once per frame
	void Update () {
		foreach (LeapConnect2.FingerStruct f in LeapConnect2.fingers) {
			if (f.fingerUpd != null) {
				if (f.fingerObj.renderer.bounds.Intersects(targetObject.renderer.bounds)) {
					targetObject.renderer.material.color = new Color(1, 0, 0, 0);
				}
			}
		}
	}
}