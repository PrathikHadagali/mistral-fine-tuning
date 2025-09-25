using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using TMPro;  // for UI Text

[System.Serializable]
public class Query
{
    public string question;
}

[System.Serializable]
public class ChatResponse
{
    public string answer;
}

public class ChatbotClient : MonoBehaviour
{
    public TMP_InputField inputField;  // user types question
    public TMP_Text outputText;        // chatbot reply display
    private string apiUrl = "http://127.0.0.1:8000/chat"; // Backend URL

    public void OnSendButton()
    {
        string userInput = inputField.text;
        StartCoroutine(SendToChatbot(userInput));
    }

    IEnumerator SendToChatbot(string userInput)
    {
        Query query = new Query { question = userInput };
        string jsonData = JsonUtility.ToJson(query);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                ChatResponse response = JsonUtility.FromJson<ChatResponse>(request.downloadHandler.text);
                outputText.text = "Bot: " + response.answer;
            }
            else
            {
                outputText.text = "Error: " + request.error;
            }
        }
    }
}
