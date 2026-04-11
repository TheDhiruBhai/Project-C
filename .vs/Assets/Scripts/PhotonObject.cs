using Photon.Pun;
using UnityEngine;

public class PhotonObject : MonoBehaviourPun
{
    [Header("Photon — disable these for remote players")]
    [SerializeField] private GameObject[] objectsToDisableIfNotMine;
  
    void Awake()
    {
        if (!photonView.IsMine)
        {
            foreach (GameObject obj in objectsToDisableIfNotMine)
                if (obj != null) obj.SetActive(false);
            return; // skips all input setup
        }
    }
}
