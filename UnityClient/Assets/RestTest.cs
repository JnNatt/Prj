using System;
using System.Collections;
using System.Collections.Generic;
using Proyecto26;
using UnityEditor;
using UnityEngine;

public class RestTest : MonoBehaviour
{
    private string url = "http://localhost:8080/";

	// Use this for initialization
	void Start ()
	{
	    UriBuilder ub = new UriBuilder(new Uri(url)) {Path = "Thaihistory/api/users" };

	    /*RestClient.Get(ub.Uri.AbsoluteUri).Then(response =>
	    {
	        var test = response.text;
	    });*/
	    RestClient.GetArray<User>(ub.Uri.AbsoluteUri).Then(allUsers =>
	    {
	        EditorUtility.DisplayDialog("JSON Array", JsonHelper.ArrayToJsonString(allUsers, true), "Ok");
	    });

	}

    [Serializable]
    public class User
    {
        public int id;
        public string username;
        public string email;
        public string status;
    }
}
