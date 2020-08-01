using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerAssignement : MonoBehaviour
{
    static InputDevice player0Device;
    static InputDevice player1Device;
    private GlobalScheme _globalScheme;

    public bool device0Paired = false;
    public bool device1Paired = false;

    void OnEnable()
    {
        _globalScheme = new GlobalScheme();
        _globalScheme.Enable();
        _globalScheme.Global.Join.performed += JoinPlayer;
        _globalScheme.Global.Cancel.performed += WithdrawPlayer;
        _globalScheme.Global.Pause.performed += Reset;

        Debug.Log("1 : " + player0Device);
        Debug.Log("2 : " + player1Device);
    }

    private void Update()
    {
        if (player0Device != null)
            device0Paired = true;
        else
            device0Paired = false;

        if (player1Device != null)
            device1Paired = true;
        else
            device1Paired = false;
    }

    void JoinPlayer(CallbackContext ctx)
    {
        if (player0Device == null && player1Device != ctx.control.device)
            player0Device = ctx.control.device;

        else if (player1Device == null && player0Device != ctx.control.device)
            player1Device = ctx.control.device;
    }

    void WithdrawPlayer(CallbackContext ctx)
    {
        if (ctx.control.device == player0Device)
            player0Device = null;

        else if (ctx.control.device == player1Device)
            player1Device = null;
    }

    void Reset(CallbackContext ctx)
    {
        //SceneManager.LoadScene("CharacterSet");
    }

    public void SpawnPlayers(PlayerInputManager playerInputManager)
    {
        if(player0Device != null)
        {
            playerInputManager.playerPrefab = playerInputManager.gameObject.GetComponent<GameManager>().player0prefab;
            playerInputManager.JoinPlayer(0, 0, null, player0Device);
        }

        if (player1Device != null)
        {
            playerInputManager.playerPrefab = playerInputManager.gameObject.GetComponent<GameManager>().player1prefab;
            playerInputManager.JoinPlayer(1, 0, null, player1Device);
        }
    }
}
