using SOQET.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCoin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //complete Collect the Red Coin quest
        SoqetInspector.Instance.CompletePlayerQuest("Collect the universal coins", "Collect the Red Coin");
        gameObject.SetActive(false);
    }
}
