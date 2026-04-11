using System.Collections.Generic;
using UnityEngine;
using Game.Cards;
using Photon.Pun;

// PHOTON CHANGE: MonoBehaviour → MonoBehaviourPun
// This gives every Obstacle subclass a built-in "photonView" property automatically.
// Make sure a PhotonView component is attached to every Obstacle prefab in the Inspector.
public class Obstacle : MonoBehaviourPun
{
    [SerializeField]
    [Tooltip("Card definitions whose abilities interact with this obstacle.")]
    private List<CardDefinition> cardsToActivate = new List<CardDefinition>();

    public IReadOnlyList<CardDefinition> CardsToActivate => cardsToActivate;
}
