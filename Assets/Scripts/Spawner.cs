using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	// Use this for initialization
	void Start () {
        spawnNext();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void spawnNext() {
        Transform group = NextQueue.Instance.ReturnNext();
        group.transform.position = transform.position;
        group.GetComponent<Group>().MakeItCurrentGroup();
    }
}
