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
        SoqetInspector.Instance.SubscribeToQuestOnStartEvent("Collect the universal coins", "Collect the Red Coin", OnStartRedCoinQuest);

        //subscribe to the Collect the Yellow Coin quest "On Start Event"
        SoqetInspector.Instance.SubscribeToQuestOnStartEvent("Collect the universal coins", "Collect the Yellow Coin", OnStartYellowCoinQuest);

        //subscribe to the Collect the Green Coin quest "On Start Event"
        SoqetInspector.Instance.SubscribeToQuestOnStartEvent("Collect the universal coins", "Collect the Green Coin", OnStartGreenCoinQuest);

    }

    private void OnStartRedCoinQuest()
    {
        //Spawn Red Coin
        Instantiate(coinPrefab[0], spawnPoint[0].position, Quaternion.identity, transform);
    }

    private void OnStartYellowCoinQuest()
    {
        //Spawn Yellow Coin
        Instantiate(coinPrefab[1], spawnPoint[1].position, Quaternion.identity, transform);
    }

    private void OnStartGreenCoinQuest()
    {
        //Spawn Green Coin
        Instantiate(coinPrefab[2], spawnPoint[2].position, Quaternion.identity, transform);
    }

    private void OnDisable()
    {
        //unsubscribe to the Collect the Red Coin quest "On Start Event"
        SoqetInspector.Instance.UnsubscribeFromQuestOnStartEvent("Collect the universal coins", "Collect the Red Coin", OnStartRedCoinQuest);

        //unsubscribe to the Collect the Yellow Coin quest "On Start Event"
        SoqetInspector.Instance.UnsubscribeFromQuestOnStartEvent("Collect the universal coins", "Collect the Yellow Coin", OnStartYellowCoinQuest);

        //unsubscribe to the Collect the Green Coin quest "On Start Event"
        SoqetInspector.Instance.UnsubscribeFromQuestOnStartEvent("Collect the universal coins", "Collect the Yellow Coin", OnStartGreenCoinQuest);

    }
}
