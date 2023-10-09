using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public GameObject joinGamePanel;
    bool joinGamePanelState = false;

    public GameObject settingsPanel;
    bool settingsPanelState = false;

    public void changeStateJoinGamePanel(){
        joinGamePanelState = !joinGamePanelState;
        joinGamePanel.SetActive(joinGamePanelState);
    }

    public void changeStateSettingsPanel(){
        settingsPanelState = !settingsPanelState;
        settingsPanel.SetActive(settingsPanelState);
    }
}
