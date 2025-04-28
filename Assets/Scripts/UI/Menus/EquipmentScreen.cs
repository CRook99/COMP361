using System;
using System.Collections.Generic;
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
                foreach (EquipmentType equipmentType in Enum.GetValues(typeof(EquipmentType)))
                {
                    EquipmentCarrier.Instance.SetSoldierEquipment(soldierWidget.soldierName, equipmentType, soldierWidget.GetEquipment(equipmentType));
                }
            }

            SceneManager.LoadScene(COMBAT_SCENE);
        }
    }
}
