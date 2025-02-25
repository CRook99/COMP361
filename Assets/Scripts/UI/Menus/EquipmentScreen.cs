using System;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using Entities;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace UI
{
    public class EquipmentScreen : MonoBehaviour
    {
        private static readonly string MAIN_SCENE = "MainMenu";  
        private static readonly string COMBAT_SCENE = "Combat"; 

        [SerializeField] private List<SoldierWidget> SoldierWidgets;

        public void OnClickBackButton()
        {
            SceneManager.LoadScene(MAIN_SCENE);
        }

        public void OnClickCombatButton()
        {
            foreach (var soldierWidget in SoldierWidgets)
            {
                EquipmentCarrier.Instance.SetSoldierEquipment(soldierWidget.soldierName, EquipmentType.Armor, soldierWidget.GetEquipment(EquipmentType.Armor));
                EquipmentCarrier.Instance.SetSoldierEquipment(soldierWidget.soldierName, EquipmentType.Boots, soldierWidget.GetEquipment(EquipmentType.Boots));   
            }

            SceneManager.LoadScene(COMBAT_SCENE);
            
            Debug.Log("Alpha Armor: " + EquipmentCarrier.Instance.GetSoldierEquipment("ALPHA", EquipmentType.Armor).title);
            Debug.Log("Alpha Boots: " + EquipmentCarrier.Instance.GetSoldierEquipment("ALPHA", EquipmentType.Boots).title);

            Debug.Log("Omega Armor: " + EquipmentCarrier.Instance.GetSoldierEquipment("OMEGA", EquipmentType.Armor).title);
            Debug.Log("Omega Boots: " + EquipmentCarrier.Instance.GetSoldierEquipment("OMEGA", EquipmentType.Boots).title);

            Debug.Log("Gamma Armor: " + EquipmentCarrier.Instance.GetSoldierEquipment("GAMMA", EquipmentType.Armor).title);
            Debug.Log("Gamma Boots: " + EquipmentCarrier.Instance.GetSoldierEquipment("GAMMA", EquipmentType.Boots).title);
        }
    }
}
