using Newtonsoft.Json;
using Proyecto26;
using RSG;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    [SerializeField] private string hostUrl = "http://localhost:8000/";
    private string token;

    public static string HostUrl
    {
        get { return Instance.hostUrl; }
    }

    private static NetworkManager instance;

    public static NetworkManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new GameObject("NetworkManager").AddComponent<NetworkManager>();
            }
            return instance;
        }
    }

	void Start () {
	    if (instance && instance != this)
	    {
	        Destroy(this);
            return;
	    }
	    instance = this;
        DontDestroyOnLoad(this);


	}

    private enum Method
    {
        GET,
        PUT,
        POST,
        DELETE
    }

    private static IPromise<ResponseHelper> Call(string resourceUrl, Method method, object bodyJson = null, Dictionary<string, string> optionalHeader = null)
    {
        var builder = new UriBuilder(HostUrl)
        {
            Path = resourceUrl
        };
        if (Instance.token != null && !RestClient.DefaultRequestHeaders.ContainsKey("Authorization"))
        {
            RestClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + Instance.token);
        }
        IPromise<ResponseHelper> call = null;
        var request = new RequestHelper {url = builder.Uri.AbsoluteUri};
        if (optionalHeader != null)
        {
            foreach (var pair in optionalHeader)
            {
                request.headers.Add(pair.Key, pair.Value);
            }
        }
        request.timeout = 2;

        switch (method)
        {
                case Method.GET:
                    call = RestClient.Get(request);
                    break;
                case Method.POST:
                    call = RestClient.Post(request, bodyJson);
                    break;
                case Method.PUT:
                    call = RestClient.Put(request, bodyJson);
                    break;
                case Method.DELETE:
                    call = RestClient.Delete(request);
                    break;
        }
        if (call == null)
        {
            throw new Exception("Wrong Http Method.");
        }
        return call.Catch(err =>
        {
            Debug.LogError("Error from server!");
            throw err;
        });
    }

    private static IPromise<NetworkResult> FormatResult(IPromise<ResponseHelper> call)
    {
        return call.Then(response =>
        {
            var result = JsonConvert.DeserializeObject<NetworkResult>(response.text);
            return result;

        }).Catch(err =>
        {
            Debug.LogError("Cannot deserialize the response to NetworkResult format!");
            throw err;
        });
    }

    [Serializable]
    public struct LoginParams
    {
        public string username;
        public string password;
    }
    public void Login(string username, string password, Action<string> callback)
    {
        var param = new LoginParams
        {
            username = username,
            password = password
        };

        FormatResult(Call("api/login", Method.POST, param)).Then(result =>
        {
            if (result.success)
            {
                token = result.data.Value<string>("token");
            }
            callback(result.success?"":result.error);
        });
    }
    [Serializable]
    public struct RegisterParams
    {
        public string email;
        public string username;
        public string password;
    }

    public void Register(string email, string username, string password, Action<FormValidation> callback)
    {
        var param = new RegisterParams
        {
            email = email,
            username = username,
            password = password
        };

        FormatResult(Call("api/register", Method.POST, param)).Then(result =>
        {
            if (result.success)
            {
                callback(null);
            }
            else
            {
                callback(result.data.ToObject<FormValidation>());
            }
        });
    }

    //private static 
    public void GetTimelineInfo(Action<TimelineInfo> callback)
    {
        Call("api/timeline", Method.GET).Then(response =>
        {
            /*var jToken = JToken.Parse(response.text);
            var timeline = new TimelineInfo();
            List<Category> categories = new List<Category>();
            foreach (var item in jToken.Children())
            {
                var cat = new Category();
                cat = item.ToObject<Category>();
                categories.Add(cat);
            }
            timeline.Categories = categories;*/
            var categories = JsonConvert.DeserializeObject<List<Category>>(response.text);
            var timelineInfo = new TimelineInfo
            {
                Categories = categories
            };

            callback(timelineInfo);
        }).Catch(err => { throw err; });
    }

}
