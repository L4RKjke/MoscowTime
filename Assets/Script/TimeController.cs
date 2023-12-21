using System.Collections;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;


public class TimeController : MonoBehaviour
{
    [SerializeField] Button _button;
    
    private const string _api            = "http://worldtimeapi.org/api/timezone/";
    private const string _timeZone       = "Europe/Moscow";
    private const string _javaFunc       = "alert";

    private void Start()
    {
        _button.onClick.AddListener( () => StartCoroutine( GetMoscowTime() ) );
    }
    
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

            Application
                .ExternalCall(_javaFunc,  parsedTime );
        }
    }

    private string ParseResponseToTimeString( string response )
    {
        string pattern     = "\"datetime\":\"(.*?)\"";
        Match match        = Regex.Match(response, pattern);
        
        return match.Groups[1].Value;
    }
}
