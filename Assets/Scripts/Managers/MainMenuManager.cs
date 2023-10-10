using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject joinGamePanel;
    public bool joinGamePanelState = false;

    public GameObject settingsPanel;
    public bool settingsPanelState = false;

    public void changeStateJoinGamePanel(){
        joinGamePanelState = !joinGamePanelState;
        joinGamePanel.SetActive(joinGamePanelState);
        if (settingsPanelState) {
            settingsPanelState = !settingsPanelState;
            settingsPanel.SetActive(settingsPanelState);
        }
    }

    public void changeStateSettingsPanel(){
        settingsPanelState = !settingsPanelState;
        settingsPanel.SetActive(settingsPanelState);
        if (joinGamePanelState) {
            joinGamePanelState = !joinGamePanelState;
            joinGamePanel.SetActive(joinGamePanelState);
        }
    }
}
