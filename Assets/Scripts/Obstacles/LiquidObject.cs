using UnityEngine;

public class LiquidObject : Obstacle
{

    private Transform solidBlock;
    private Transform liquidBlock;
    [SerializeField]
    private bool isSolid = true;
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        solidBlock = transform.GetChild(0);
        liquidBlock = transform.GetChild(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Activate();
        }
    }


    void Activate() {
        if (isSolid)
        {
            solidBlock.gameObject.SetActive(false);
            liquidBlock.gameObject.SetActive(true);
            isSolid = false;
        }
        else { 
            solidBlock.gameObject.SetActive(true);
            liquidBlock.gameObject.SetActive(false);
            isSolid = true;
        }
    
    }
}
