using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalOneScreenLeaderboard : MonoBehaviour
{
    [SerializeField] private GameObject _leaderboard, _winScreen;
    [SerializeField] private GameObject confetti;
    [SerializeField] private GameObject images;

    void Awake(){
        GameManager.state.OnValueChanged += GameManagerOnGameStateChanged;
        GameManager.handleLeaderboard.OnValueChanged += UpdateLeaderboard;
    }

    private void UpdateLeaderboard(bool prev, bool curr){
        if (_leaderboard != null) {
            if(_leaderboard.activeSelf){
                _leaderboard.GetComponent<Leaderboard>().UpdateLeaderboard(true, true);
            }
        }
    }

    private void GameManagerOnGameStateChanged(GameState prev, GameState curr){

        if (this == null) {
            return;
        }

        _leaderboard.SetActive(curr == GameState.Leaderboard);
        _winScreen.SetActive(curr == GameState.WinScreen);

        if (curr == GameState.WinScreen) {
            //Instantiate(shower, transform.position, transform.rotation, transform);
            //Instantiate(bomb, transform.position, transform.rotation, transform);
            images.SetActive(false);
            Instantiate(confetti, new Vector3(41.2f, 23.5f, -10f), transform.rotation);
        }
    }
}
