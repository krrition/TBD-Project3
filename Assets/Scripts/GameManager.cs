using System;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
   [SerializeField]
   private GameObject PF_Char, SpawnP1, SpawnP2;
   
   [SerializeField]
   private float RoundTime = 10;
   
   private bool TurnP2;
   
   private bool ReplayOnly;

   private int RoundNum;
   
   private float RoundTimer;

   private GameObject[] P1Cs = new GameObject[6];
   private GameObject[] P2Cs = new GameObject[6];

   private Vector3 P1Spawn, P2Spawn;

   public List<GameObject> Abilities1;
   public List<GameObject> Abilities2;
   
   /* Round begins, spawn in player 1 character 1, start recording
    * Timer ends, stop the recording of p1c1
    * Round begins, reset timer, spawn in p2c1, replay p1c1
    * Timer ends, stop recording of p2c1, stop replay p1c1
    * 
    */

   private void Start()
   {
       Abilities2 = new List<GameObject>(Abilities1);
       RoundNum = 1;
       P1Spawn = SpawnP1.transform.position;
       P2Spawn = SpawnP2.transform.position;
       P1Cs[RoundNum] = Instantiate(PF_Char, P1Spawn, Quaternion.identity);
       P1Cs[RoundNum].tag = "P1";
       P1Cs[RoundNum].name = "P1C"+RoundNum;
       RoundTimer = RoundTime;
   }

   private void Update()
   {
       RoundTimerReset();
       
   }

   private void RoundTimerReset()
   {
       RoundTimer -= Time.deltaTime;

       if (RoundTimer<=0)
       {
           if (!TurnP2 && RoundNum >= P1Cs.Length && !ReplayOnly || TurnP2 && RoundNum >= P2Cs.Length && !ReplayOnly)
           {
               ReplayOnly = true;
               Debug.Log(ReplayOnly);
           }
           
           else if (!TurnP2 && RoundNum < P1Cs.Length)
           {
               P1Cs[RoundNum].GameObject().GetComponent<PlayerController>().StopRecording();
               P1Spawn = new Vector3(P1Spawn.x, P1Spawn.y, P1Spawn.z - 1);
               TurnP2 = true;
           }
           else if (TurnP2 && RoundNum < P2Cs.Length)
           {
               P2Cs[RoundNum].GameObject().GetComponent<PlayerController>().StopRecording();
               P2Spawn = new Vector3(P2Spawn.x, P2Spawn.y, P2Spawn.z - 1);
               TurnP2 = false;
               RoundNum = !ReplayOnly ? RoundNum + 1: RoundNum;
           }
           
           

           
           HandleCharacters();
           RoundTimer = RoundTime;
       }
   }
   
   
   private void HandleCharacters()
   {
       if (ReplayOnly)
       {
           for (var i = RoundNum-1; i > 0 ; i--)
           {
               
               if (P1Cs[i] != null)
                   P1Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();
               
               if (P2Cs[i] != null)
                   P2Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();

           }
       }

       else 
       {
           if (!TurnP2)
           {
               for (var i = RoundNum; i > 0 ; i--)
               {
               
                   if (P1Cs[i] != null)
                       P1Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();
               
                   if (P2Cs[i] != null)
                       P2Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();

               }

               P1Cs[RoundNum] = Instantiate(PF_Char, P1Spawn, Quaternion.identity);
               P1Cs[RoundNum].tag = "P1";
               P1Cs[RoundNum].name = "P1C"+RoundNum;

           }
           else
           {
               for (var i = RoundNum; i > 0; i--)
               {
                   if (P1Cs[i] != null)
                       P1Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();
               
                   if (P2Cs[i] != null)
                       P2Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();

               }

               P2Cs[RoundNum] = Instantiate(PF_Char, P2Spawn, Quaternion.identity);
               P2Cs[RoundNum].tag = "P2";
               P2Cs[RoundNum].name = "P2C"+RoundNum;

               
           
           }
       }
   }

   private void CreateCharacters()
   {
       
   }

}
