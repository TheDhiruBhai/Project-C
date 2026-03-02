using UnityEngine;

namespace Game.Net
{
    public struct AbilityRequest
    {
        public int cardInstanceId;
        public int casterNetId;
        public int? targetNetId;
        public Vector3 targetPoint;

        public static AbilityRequest Build(int cardInstanceId, int casterNetId, int? targetNetId, Vector3 targetPoint)
        {
            return new AbilityRequest
            {
                cardInstanceId = cardInstanceId,
                casterNetId = casterNetId,
                targetNetId = targetNetId,
                targetPoint = targetPoint
            };
        }
    }

    public struct AbilityResult
    {
        public bool approved;
        public int cardInstanceId;
        public int casterNetId;
        public int? targetNetId;
        public Vector3 targetPoint;
        public string reason;
    }
}
