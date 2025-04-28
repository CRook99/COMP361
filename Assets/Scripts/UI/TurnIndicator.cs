using UnityEngine;
using Managers;
using TMPro;
using System.Collections;

namespace UI
{
    public class TurnIndicator : MonoBehaviour
    {
        
        private TMP_Text turnText;

        private void Awake()
        {
            turnText = GetComponent<TMP_Text>();
            ShowAllyTurn();
        }

        private void OnEnable()
        {
            EventManager.Subscribe(EventTypes.OnEndEnemyTurn, ShowAllyTurn);
            EventManager.Subscribe(EventTypes.OnStartEnemyTurn, ShowEnemyTurn); 
        }

        private void OnDisable()
        {
            EventManager.Unsubscribe(EventTypes.OnEndEnemyTurn, ShowAllyTurn);
            EventManager.Unsubscribe(EventTypes.OnStartEnemyTurn, ShowEnemyTurn);
        }

        private void ShowAllyTurn()
        {
            turnText.text = "Ally Turn";
            turnText.color = Color.blue;
            turnText.gameObject.SetActive(true);
        }

        private void ShowEnemyTurn()
        {
            turnText.text = "Enemy Turn";
            turnText.color = Color.red;
            turnText.gameObject.SetActive(true);
        }

        private IEnumerator HideAfterDelay()
        {
            yield return new WaitForSeconds(2);
            turnText.gameObject.SetActive(false);
        }
    }
}


