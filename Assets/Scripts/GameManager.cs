using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool isPlaying = false;
    public int active_player;   //1 2
    public Text state_text;
    public Text win_text;
    public GameObject gamectrl_prefab;

    public GameObject input_panel;
    public GameObject rotate_panel;
    public InputField[] players_name_input;

    public Text endturn_but;

    public Button doRot_OK;
    public Button rotate_cell_ckw;
    public Button rotate_cell_cntckw;

    public Sprite doRot_img;
    public Sprite OK_img;

    public GameObject menugrid;
    public GameObject menupanel;
    int turn_count = 0;
    GameObject game_field;

    int winner = 0;
    string[] players_name;
    Color[] player_colors;

    bool rot_btns_visible = false;
    private void Start()
    {
        players_name = new string[2]
        {
            "player1",
            "player2"
        };
        player_colors = new Color[2]
        {
            new Color32(255,130,10,200),
            new Color32(0,150,255,200)
        };
        ShowMainMenu();      
    }

    private void ShowMainMenu()
    {
        menugrid.SetActive(true);
        menugrid.GetComponentInChildren<UnityEngine.Tilemaps.TilemapRenderer>().sortingOrder = 11;
        menupanel.SetActive(true);
        state_text.gameObject.SetActive(false);
        endturn_but.gameObject.SetActive(false);
        win_text.enabled = false;
    }


    public void ShowInputPanel() 
    {
        menugrid.GetComponentInChildren<UnityEngine.Tilemaps.TilemapRenderer>().sortingOrder = 9;
        menupanel.SetActive(false);
        input_panel.SetActive(true);
    }
    public void StartGame()
    {
        if(players_name_input[0].text.Length != 0)
            players_name[0] = players_name_input[0].text.PadRight(7);
        if (players_name_input[1].text.Length != 0)
            players_name[1] = players_name_input[1].text.PadRight(7);
        
        input_panel.SetActive(false);
        menugrid.SetActive(false);
        state_text.gameObject.SetActive(true);
        endturn_but.gameObject.SetActive(true);

        game_field = Instantiate(gamectrl_prefab, transform);
        rotate_cell_ckw.onClick.AddListener(game_field.GetComponent<GameControl>().RotateCELL_CKW);
        rotate_cell_cntckw.onClick.AddListener(game_field.GetComponent<GameControl>().RotateCELL_CounterCKW);

        active_player = 2;
        isPlaying = true;
        EndTurn();
    }
    public void EndTurn()
    {
        if (winner != 0)
            EndGame();
        active_player = SwitchPlayer(active_player);
        turn_count++;
        state_text.text = players_name[active_player-1] + "\n turn";
        state_text.color = player_colors[active_player - 1];
    }

    private void EndGame()
    {              
        isPlaying = false;
        winner = 0;
        Destroy(game_field);
        ShowMainMenu();
    }

    public void PillarHit(int pillar_owner)
    {
        Debug.Log(players_name[SwitchPlayer(pillar_owner)-1] + " win");
        winner = SwitchPlayer(pillar_owner);
        win_text.enabled = true;
        string wt = players_name[winner - 1] + "\n WIN";
        win_text.text = wt;
    }

    private int SwitchPlayer(int p)
    {
        if (p == 1)
            return 2;
        else
            return 1;
    }

    public void SetPlayerName(int player, string name)
    {
        players_name[player] = name.Length > 7 ? name.Substring(0,7) : name.PadLeft(7,' ');
    }

    public string GetPlayerName(int player)
    {
        return players_name[player-1];
    }

    public GameObject GetRotationPanel()
    {
        return rotate_panel;
    }

    public void ShowRotBtns()
    {
        if (!rot_btns_visible)
        {
            doRot_OK.image.sprite = OK_img;
            rotate_cell_ckw.gameObject.SetActive(true);
            rotate_cell_cntckw.gameObject.SetActive(true);
            rot_btns_visible = true;
            game_field.GetComponent<GameControl>().rotating = true;
        }
        else
            HideRotBtns();
        
    }

    public void HideRotBtns()
    {
        doRot_OK.image.sprite = doRot_img;
        rotate_cell_ckw.gameObject.SetActive(false);
        rotate_cell_cntckw.gameObject.SetActive(false);
        rot_btns_visible = false;
        game_field.GetComponent<GameControl>().rotating = false;
    }

    public void OnEndButMouseEnter()
    {
        endturn_but.color = new Color(1f, 0f, 0f, .7f);
    }
    public void OnEndButMouseLeave()
    {
        endturn_but.color = new Color(1f, 0f, 0f, .5f);
    }
    public void OnEndButMouseClick()
    {
        game_field.GetComponent<GameControl>().CalculateBeam();
    }
    public void OnEndButMouseDown()
    {
        endturn_but.color = new Color(1f, 0f, 0f, 1f);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        // Application.Quit() does not work in the editor so
        // UnityEditor.EditorApplication.isPlaying need to be set to false to end the game
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
