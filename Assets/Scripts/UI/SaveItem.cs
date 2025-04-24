using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class SaveItem : MonoBehaviour
    {
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI TurnText;
        public TextMeshProUGUI EnemiesText;
        public string SaveFileName;
        public Button Button;

        public void LoadData(string saveName, int turn, int enemies)
        {
            SaveFileName = saveName;
            NameText.text = saveName.Replace(".json", "");
            TurnText.text = $"Turn: {turn.ToString()}";
            EnemiesText.text = $"Enemies: {enemies.ToString()}";

            Button = GetComponent<Button>();
        }
    }
}
