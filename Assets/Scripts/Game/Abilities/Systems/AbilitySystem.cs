using UnityEngine;
using Game.World;
using Game.Net;

namespace Game.Abilities
{
    public sealed class AbilitySystem : MonoBehaviour
    {
        [SerializeField] private WorldQueryBehaviour worldQuery;
        [SerializeField] private CardsControllers.CardRuntimeController cardRuntime;

        public void ExecuteLocal(AbilityRequest req, out AbilityResult result)
        {
            result = new AbilityResult
            {
                approved = false,
                cardInstanceId = req.cardInstanceId,
                casterNetId = req.casterNetId,
                targetNetId = req.targetNetId,
                targetPoint = req.targetPoint,
                reason = "Invalid"
            };

            var card = cardRuntime.GetHandCardById(req.cardInstanceId);
            if (card == null || card.definition == null || card.definition.ability == null)
            {
                result.reason = "Card missing";
                return;
            }

            var ability = card.definition.ability;

            var ctx = new AbilityContext
            {
                casterNetId = req.casterNetId,
                targetNetId = req.targetNetId,
                targetPoint = req.targetPoint,
                time = Time.timeAsDouble,
                world = worldQuery
            };

            if (!ability.CanActivate(ctx, out var reason))
            {
                result.reason = string.IsNullOrEmpty(reason) ? "Cannot activate" : reason;
                return;
            }

            ability.Activate(ctx);

            result.approved = true;
            result.reason = "";
        }
    }
}
