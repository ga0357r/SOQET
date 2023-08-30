using SOQET.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YellowCoin : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //complete Collect the Yellow Coin quest
        SoqetInspector.Instance.CompletePlayerQuest("Collect the universal coins", "Collect the Yellow Coin");
        gameObject.SetActive(false);
    }
}
