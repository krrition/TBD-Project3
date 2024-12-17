using System;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    public bool isGhost, isP1;

    private SpriteRenderer SP;

    private void Start()
    {
        SP = GetComponent<SpriteRenderer>();
        if (isGhost)SP.color = isP1 ? Color.clear/5 + Color.cyan / 2 : Color.clear/5 + Color.green / 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
