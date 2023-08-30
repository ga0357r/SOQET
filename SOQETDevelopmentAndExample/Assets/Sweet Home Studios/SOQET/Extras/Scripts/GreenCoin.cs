using SOQET.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCoin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //complete Collect the Green Coin quest
        SoqetInspector.Instance.CompletePlayerQuest("Collect the universal coins", "Collect the Green Coin");
        gameObject.SetActive(false);
    }
}
