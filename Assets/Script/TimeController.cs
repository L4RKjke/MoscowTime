using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MoscowTime
{
   public class TimeController : MonoBehaviour
   {
       [SerializeField] Button _button;
       
       private Coroutine _getTimeRoutine;
       
       private const string _api            = "http://worldtimeapi.org/api/timezone/";
       private const string _timeZone       = "Europe/Moscow";

       #region Mono

       private void OnEnable()
       {
           _button.onClick.AddListener( HandleGetMoscowTime );
       }

       private void OnDisable()
       {
           _button.onClick.RemoveListener( HandleGetMoscowTime );
           
           if (_getTimeRoutine != null)
               StopCoroutine( _getTimeRoutine );
       }

       #endregion
       #region DllImport
   
       [DllImport("__Internal")]
       private static extern void Alert(string str);
       
       #endregion

       private IEnumerator GetMoscowTime()
       {
           using UnityWebRequest www = UnityWebRequest.Get($"{_api}{_timeZone}");
           
           yield return www.SendWebRequest();
   
           if ( www.result != UnityWebRequest.Result.Success )
           {
               Debug.LogError( "Error: " + www.error );
               yield break;
           }
   
           string response        = www.downloadHandler.text;
   
           if ( !TryParseResponseToTimeString( response, out string parsedString ) )
           {
               Debug.LogError("Can't parse!");
               yield break;
           }
           
#if !UNITY_EDITOR
            Alert( parsedString );
#else
            Debug.Log( parsedString );
#endif
       }
   
       private bool TryParseResponseToTimeString( string response, out string parsedString )
       {
           string pattern     = "\"datetime\":\"(.*?)\"";
           Match match        = Regex.Match(response, pattern);
           string matchText   = match.Groups[1].Value;
           
           parsedString = string.Empty;
           
           if ( DateTimeOffset.TryParse( matchText, out DateTimeOffset parsedDateTime ) )
               parsedString = parsedDateTime.ToString( "HH:mm:ss" );
   
           return parsedString != string.Empty;
       }

       private void HandleGetMoscowTime() => _getTimeRoutine = StartCoroutine( GetMoscowTime() );
   } 
}

