using UnityEngine;
using Game.Cards;

namespace Game.CardsRuntime
{
    public sealed class CardInstance
    {
        public int instanceId;
        public int ownerPlayerNetId;

        public CardDefinition definition;

        public float cooldownRemaining;
        public int charges;
        public int upgradeLevel;

        public bool IsOnCooldown => cooldownRemaining > 0f;

        public CardInstance(int instanceId, int ownerPlayerNetId, CardDefinition definition)
        {
            this.instanceId = instanceId;
            this.ownerPlayerNetId = ownerPlayerNetId;
            this.definition = definition;
            cooldownRemaining = 0f;
            charges = 1;
            upgradeLevel = 0;
        }
    }
}
