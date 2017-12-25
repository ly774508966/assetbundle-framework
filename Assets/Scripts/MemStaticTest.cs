using UnityEngine;
using System.Collections;

public class MemStaticTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnGUI()
    {

        if (Application.loadedLevelName == "mem_static")
        {
            if (GUI.Button(new Rect(450, 10, 200, 80), "切换场景到test"))
            {
                Application.LoadLevel("test");
            }

            if (GUI.Button(new Rect(450, 110, 200, 80), "切换场景到Dependance"))
            {
                Application.LoadLevel("Dependance");
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
