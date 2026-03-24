using System.Collections.Generic;
using UnityEngine;
using Game.Cards;

public class Obstacle : MonoBehaviour
{
    [SerializeField]
    [Tooltip("Card definitions whose abilities interact with this obstacle.")]
    private List<CardDefinition> cardsToActivate = new List<CardDefinition>();

    public IReadOnlyList<CardDefinition> CardsToActivate => cardsToActivate;
}