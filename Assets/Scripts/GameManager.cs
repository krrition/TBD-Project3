using System;
using Unity.Mathematics;
using UnityEngine;

public class GameManager : MonoBehaviour
{
   [SerializeField]
   private GameObject PF_Char, SpawnP1, SpawnP2;
   
   [SerializeField]
   private float RoundTimer = 10;
   
   private bool TurnP2, DoOnce;

   private float RoundNum;
   
   private float RoundTime;

   private GameObject[] P1Cs = new GameObject[5];
   private GameObject[] P2Cs = new GameObject[5];
   
   /* Round begins, spawn in player 1 character 1, start recording
    * Timer ends, stop the recording of p1c1
    * Round begins, reset timer, spawn in p2c1, replay p1c1
    * Timer ends, stop recording of p2c1, stop replay p1c1
    * 
    */

   private void Start()
   {
      Instantiate(PF_Char, SpawnP1.transform.position, quaternion.identity);
   }


   private void HandleCharacters()
   {
       if (!TurnP2)
       {
           
       }
       else
       {
           
       }
   }
}
