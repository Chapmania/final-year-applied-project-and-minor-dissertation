﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.Windows.Speech;


public class SpeechToText : MonoBehaviour
{
    #region == Private Variables == 
    [SerializeField]
    private Text m_HypothesesText;

    [SerializeField]
    private Text m_OutputText;
    
    private DictationRecognizer m_DictationRecognizerObj;
    #endregion




    public void sendText()
    {
        StartCoroutine(GetText());
    }

    IEnumerator GetText()
    {

        WWWForm form = new WWWForm();

        form.AddField("myField", m_OutputText.text);
        


        Debug.Log(form.data);

        string jsonStringTrial = JsonUtility.ToJson(form);

        Debug.Log(jsonStringTrial);

        UnityWebRequest www = UnityWebRequest.Post("localhost:5000", form);
        www.SetRequestHeader("Content-Type", "application/json");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            byte[] results = www.downloadHandler.data;
        }
    }

    public void textToSpeech()
    {

    }

    // Called when button is pressed. Listens for speech and predicts what it hears.
    public void listenForSpeech()
    {
        // Code adapted from Unity documentation here: https://docs.unity3d.com/540/Documentation/ScriptReference/Windows.Speech.DictationRecognizer.html
        m_DictationRecognizerObj = new DictationRecognizer();

        // Sets the 
        m_DictationRecognizerObj.DictationResult += (text, confidence) =>
        {
            //Debug.LogFormat("Dictation result: {0}", text);
            m_OutputText.text = text;
        };

        // Guesses what it thinks initally and then corrects it to it's best knowledge.
        m_DictationRecognizerObj.DictationHypothesis += (text) =>
        {
           // Debug.LogFormat("Dictation hypothesis: {0}", text);
            m_HypothesesText.text = text;
        };

        // Once complete, output the error if occured and shutdown.
        m_DictationRecognizerObj.DictationComplete += (completionCause) =>
        {
            if (completionCause != DictationCompletionCause.Complete)
            {
                Debug.LogErrorFormat("Dictation completed unsuccessfully: {0}.", completionCause);
            }

            m_DictationRecognizerObj.Stop();
        };

        // If error occurs handle and shutdown.
        m_DictationRecognizerObj.DictationError += (error, hresult) =>
        {
            Debug.LogErrorFormat("Dictation error: {0}; HResult = {1}.", error, hresult);
            m_DictationRecognizerObj.Stop();
        };

        // Start listening.
        m_DictationRecognizerObj.Start();
    }


    public class RequestS
    {
        public string s;

        public RequestS(string sentence)
        {
            s = sentence;

        }

        //This method is required by the IComparable
        //interface. 
    }
}