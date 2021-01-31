using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubManagement : MonoBehaviour
{
    private void Awake()
    {
        if (!PlayerPrefs.HasKey("alreadyHub"))
        {
            PlayerPrefs.SetInt("alreadyHub", 1);
        }

        else
        {
            GameManager gameManager = gameObject.GetComponent<GameManager>();
            gameManager.currentProgressionCp = new Vector3(52.4f, -0.69f, -168);
        }
    }
}
