using UnityEngine;
using Game.Abilities;

namespace Game.Cards
{
    public enum CardType { Utility, Burst, Buff }
    public enum ElementType { Any, Fire, Water, Earth, Air }

    [CreateAssetMenu(menuName = "Cards/Card Definition")]
    public class CardDefinition : ScriptableObject
    {
        [Header("Display")]
        public string cardName;
        public Sprite icon;
        [TextArea] public string description;

        [Header("Rules")]
        public CardType cardType = CardType.Utility;
        public ElementType elementRestriction = ElementType.Any;

        [Min(0f)] public float rangeMeters = 10f;
        [Min(0f)] public float durationSeconds = 0f;

        [Header("Costs")]
        [Min(0)] public int hpCost = 0;
        [Min(0f)] public float cooldownSeconds = 3f;

        [Header("Ability")]
        public Game.Abilities.AbilitySO ability;
    }
}