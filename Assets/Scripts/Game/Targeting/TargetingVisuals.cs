using UnityEngine;

namespace Game.CardsControllers
{
    public sealed class TargetingVisuals : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;

        public void Show()
        {
            if (line != null) line.enabled = true;
        }

        public void HideAll()
        {
            if (line != null) line.enabled = false;
        }

        public void SetLine(Vector3 from, Vector3 to, bool valid)
        {
            if (line == null) return;
            if (!line.enabled) line.enabled = true;
            line.positionCount = 2;
            line.SetPosition(0, from);
            line.SetPosition(1, to);

        }
    }
}
