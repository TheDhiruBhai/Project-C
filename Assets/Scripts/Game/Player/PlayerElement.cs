using UnityEngine;
using Game.Cards;

namespace Game.Player
{
    public sealed class PlayerElement : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The element this player is assigned. Any = no restriction (not normally used for players).")]
        private ElementType element = ElementType.Fire;

        public ElementType Element => element;

        /// <summary>Assign a new element at runtime (e.g. during character select).</summary>
        public void SetElement(ElementType newElement) => element = newElement;
    }
}
