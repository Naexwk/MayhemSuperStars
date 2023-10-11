using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controlador de objetos de UI del menú principal
public class MainMenuManager : MonoBehaviour
{
    // Referencia al panel de Join Game y su estado de activación
    public GameObject joinGamePanel;
    public bool joinGamePanelState = false;

    // Referencia al panel de Settings y su estado de activación
    public GameObject settingsPanel;
    public bool settingsPanelState = false;

    // Activar o desactivar el panel de Join Game
    public void ChangeStateJoinGamePanel(){
        joinGamePanelState = !joinGamePanelState;
        joinGamePanel.SetActive(joinGamePanelState);
        // Desactivar el panel de Settings si se intenta abrir el
        // panel de Join Game
        if (settingsPanelState) {
            settingsPanelState = !settingsPanelState;
            settingsPanel.SetActive(settingsPanelState);
        }
    }

    // Activar o desactivar el panel de Settings
    public void ChangeStateSettingsPanel(){
        settingsPanelState = !settingsPanelState;
        settingsPanel.SetActive(settingsPanelState);
        // Desactivar el panel de Join Game si se intenta abrir el
        // panel de Settings
        if (joinGamePanelState) {
            joinGamePanelState = !joinGamePanelState;
            joinGamePanel.SetActive(joinGamePanelState);
        }
    }
}
