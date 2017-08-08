using UnityEngine;
using System.Collections;

public class TestJSON : MonoBehaviour {

	// Use this for initialization
	void Start ()
	{
		//Encoding.
		ArrayList ListE=new ArrayList();
		for(int i=0;i<3;i++)
		{	
			var ht=new Hashtable();
			ht["Level"]=Random.Range(5,10);
			ht["P"]=Random.Range(90,100);
			ht["D"]=Random.Range(90,100);
			ht["A"]=Random.Range(90,100);
			
			ListE.Add(ht);
		}
		
		JSONParser.setDictionary("Level",ListE);
		
		//Decoding.
		ArrayList ListD=(ArrayList)JSONParser.getDictionary("Level");
		foreach(Hashtable Tmpht in ListD)
		{
			Debug.Log(Tmpht["Level"]);
			Debug.Log(Tmpht["P"]);
			Debug.Log(Tmpht["D"]);
			Debug.Log(Tmpht["A"]);
		}
	}
}