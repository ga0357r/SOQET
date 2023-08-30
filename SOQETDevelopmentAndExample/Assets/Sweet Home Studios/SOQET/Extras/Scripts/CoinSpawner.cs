using SOQET.Inspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] coinPrefab = new GameObject[3];
    [SerializeField] private Transform[] spawnPoint = new Transform[3];

    private void OnEnable()
    {
        //subscribe to the Collect the Red Coin quest "On Start Event"
        SoqetInspector.Instance.SubscribeToQuestOnStartEvent("Collect the universal coins.", "Collect the Red Coin", OnStartRedCoinQuest);

        //subscribe to the Collect the Yellow Coin quest "On Start Event"
        SoqetInspector.Instance.SubscribeToQuestOnStartEvent("Collect the universal coins.", "Collect the Yellow Coin", OnStartYellowCoinQuest);

        //subscribe to the Collect the Green Coin quest "On Start Event"
        SoqetInspector.Instance.SubscribeToQuestOnStartEvent("Collect the universal coins.", "Collect the Yellow Coin", OnStartGreenQuest);

    }

    private void OnStartRedCoinQuest()
    {
        //Spawn Red Coin
        //Instantiate(redCoinPrefab, );
    }

    private void OnStartYellowCoinQuest()
    {
        //Spawn Yellow Coin
    }

    private void OnStartGreenQuest()
    {
        //Spawn Green Coin
    }

    private void OnDisable()
    {
        
    }
}
