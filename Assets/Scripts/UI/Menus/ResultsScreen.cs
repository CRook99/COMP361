using UnityEngine;
using UnityEngine.UI; 
using TMPro;
using UnityEngine.SceneManagement;


namespace UI
{
    public static class SceneParams
    {
        public static string ResultsMessage;
    }
    
    public class ResultsScreen : MonoBehaviour
    {
        public Button MenuButton;

        public TMP_Text resultsText;
        
        public TMP_Text enemiesKilledText;
        public TMP_Text damageDealtText;
        public TMP_Text shotsLandedText;
        public TMP_Text spacesMovedText;
        public TMP_Text alliesFallenText;
        public TMP_Text damageTakenText;
        public TMP_Text shotsTakenText;
        public TMP_Text chanceShotsDodgedText;

        private void Start()
        {
            UpdateResultsScreen();
            
            MenuButton.onClick.AddListener(OnMainMenuButtonClicked);
        }

        private void UpdateResultsScreen()
        {
            resultsText.text = SceneParams.ResultsMessage;
            
            enemiesKilledText.text = "Enemies Killed: " + StatisticsManager.Instance.GetEnemiesVanquished();
            damageDealtText.text = "Damage Dealt: " + StatisticsManager.Instance.GetDamageDealt();
            shotsLandedText.text = "Shots Landed: " + StatisticsManager.Instance.GetShotsLanded();
            spacesMovedText.text = "Spaces Moved: " + StatisticsManager.Instance.GetSpacesMoved();
            alliesFallenText.text = "Allies Fallen: " + StatisticsManager.Instance.GetFallenSoldiers();
            damageTakenText.text = "Damage Taken: " + StatisticsManager.Instance.GetDamageReceived();
            shotsTakenText.text = "Shots Taken: " + StatisticsManager.Instance.GetShotsTaken();
            chanceShotsDodgedText.text = "Chance Shots Dodged: " + StatisticsManager.Instance.GetChanceShotsDodged();
        }

        public void OnMainMenuButtonClicked()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
