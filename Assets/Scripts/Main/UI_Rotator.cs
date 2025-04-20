using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Rotator : MonoBehaviour
{
    [SerializeField] private GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotToPlayer = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotToPlayer, 1f);
    }
}
