using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UIElements;
using UnityEngine.WSA;
using Game.World;

public class Torch : Obstacle, IFlammable
{
    private GameObject torchFlame;

    void Start()
    {
        torchFlame = transform.GetChild(1).gameObject;
        torchFlame.SetActive(false);
    }

    public void Ignite(float seconds) => Light();

    ///Called by gameplay systems or editor tools to light this torch.
    public void Light() => torchFlame.SetActive(true);

    ///Called to extinguish (e.g. water card in future).
    public void Extinguish() => torchFlame.SetActive(false);
}
