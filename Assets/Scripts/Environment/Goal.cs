using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Goal : NetworkBehaviour
{
    public static Action OnWin;
    private int m_PlayerCount;

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Player"))
        {
            m_PlayerCount++;
            if (m_PlayerCount >= NetworkManager.Singleton.ConnectedClients.Count)
            {
                Debug.Log("Win!");
                StartCoroutine(C_Win());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;
        if (other.CompareTag("Player"))
        {
            m_PlayerCount--;
        }
    }

    IEnumerator C_Win()
    {
        OnWin?.Invoke();
        yield return new WaitForSeconds(5.0f);
        NetworkManager.SceneManager.UnloadScene(SceneManager.GetSceneByName("Game"));
    }
}
