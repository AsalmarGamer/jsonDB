using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro;

public class RequestHTTP : MonoBehaviour
{//ADRIAN
    private User MyUser;

    public int UserId = 1;
    public string apiRickAndMorty = "https://rickandmortyapi.com/api/character";
    public string url = "https://my-json-server.typicode.com/AsalmarGamer/jsonDB";

    [SerializeField]
    private TMP_Text UserNameLabel;
    [SerializeField]
    private RawImage[] Mydeck;
    [SerializeField]
    private TMP_Text[] cardNames;

    public void SendRequest()
    {

        StartCoroutine(GetUsers());

    }
    IEnumerator GetCharacter(int index)
    {

        int characterID = MyUser.deck[index];
        UnityWebRequest request = UnityWebRequest.Get(apiRickAndMorty + "/" + characterID);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else
        {
            //Debug.Log(request.downloadHandler.text);
            if (request.responseCode == 200)
            {
                Characters characters = JsonUtility.FromJson<Characters>(request.downloadHandler.text);
                StartCoroutine(DownloadImage(characters.image, Mydeck[index]));

                cardNames[index].text = characters.name;
                Debug.Log(characters.name);
            }
        }
    }
    IEnumerator GetUsers()
    {
        UnityWebRequest request = UnityWebRequest.Get(url + "/users/" + UserId);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log("Network Error: " + request.error);
        }
        else
        {
            Debug.Log(request.downloadHandler.text);

            if (request.responseCode == 200)
            {
                //transformacion data
                MyUser = JsonUtility.FromJson<User>(request.downloadHandler.text);
                UserNameLabel.text = MyUser.username;
                for (int i = 0; i < MyUser.deck.Length; i++)
                {
                    StartCoroutine(GetCharacter(i));

                }

                foreach (int card in MyUser.deck)
                {
                    StartCoroutine(GetCharacter(card));

                }


            }
            else
            {
                Debug.Log(request.error);
            }
        }
    }
    IEnumerator DownloadImage(string MediaURL, RawImage image)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaURL);
        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.Log(request.error);
        }
        else if (!request.isHttpError)
        {
            image.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;

        }

    }
}


[System.Serializable]
public class User
{
    public int id;
    public string username;
    public bool state;
    public int[] deck;

}
public class UsersList
{
    public List<User> users;
}

public class Characters
{
    public int id;
    public string name;
    public string image;
}
