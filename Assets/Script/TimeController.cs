using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class TimeController : MonoBehaviour
{
    [SerializeField] Button _button;
    
    private const string _api            = "http://worldtimeapi.org/api/timezone/";
    private const string _timeZone       = "Europe/Moscow";

    private void Start()
    {
        _button.onClick.AddListener( () => StartCoroutine( GetMoscowTime() ) );
    }

#region DllImport

    [DllImport("__Internal")]
    private static extern void Alert(string str);
    
#endregion

    private IEnumerator GetMoscowTime()
    {
        using UnityWebRequest www = UnityWebRequest.Get($"{_api}{_timeZone}");
        
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            string response        = www.downloadHandler.text;
            string parsedTime      = ParseResponseToTimeString( response );
            
            #if !UNITY_EDITOR
            Alert( parsedTime );
            #else
            Debug.Log( parsedTime );
            #endif
        }
    }

    private string ParseResponseToTimeString( string response )
    {
        string pattern     = "\"datetime\":\"(.*?)\"";
        Match match        = Regex.Match(response, pattern);
        
        return match.Groups[1].Value;
    }
}
