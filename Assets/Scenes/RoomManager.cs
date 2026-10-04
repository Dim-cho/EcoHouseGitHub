using UnityEngine;
using UnityEngine.SceneManagement;

public class Room : MonoBehaviour
{
    public void LoadRoom(string roomName)
    {
        SceneManager.LoadScene(roomName);
    }
}
