using System;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class GameManager : MonoBehaviour
{
   [SerializeField]
   private GameObject PF_Char, SpawnP1, SpawnP2;
   
   [SerializeField]
   private float RoundTime;
   
   private bool TurnP2, doOnce;
   
   private bool ReplayOnly;

   private int RoundNum;

   private float RoundTimer;

   private GameObject[] P1Cs = new GameObject[6];
   private GameObject[] P2Cs = new GameObject[6];

   private Sprite p1sp;
   private Sprite p2sp;

   private Vector3 P1Spawn, P2Spawn;

   public List<GameObject> Abilities1;
   public List<GameObject> Abilities2;
   
   public Canvas UISelect1;
   public Canvas UISelect2;
   public Canvas UIObjInfo;
   public Canvas UIPointCount;
   
   public TextMeshProUGUI UINum1;
   public TextMeshProUGUI UINum2;

   private GameObject feedAbility;

   public GameObject ob_nest;
   
   public GameObject ob_crown;
   
   public GameObject ob_tnt1;
   public GameObject ob_tnt2;
   
   public GameObject ob_mite;

   private GameObject storeObj;
   private StoredController SC;
   
   public TextMeshProUGUI Rnd1Nest;
   public TextMeshProUGUI Rnd2Nest;
   public TextMeshProUGUI Rnd1Crown;
   public TextMeshProUGUI Rnd2Crown;
   public TextMeshProUGUI Rnd1Tnt;
   public TextMeshProUGUI Rnd2Tnt;

   public GameObject Camera;



   private void Start()
   {
       if (storeObj == null) storeObj = GameObject.Find("Stored");
       SC = storeObj.GetComponent<StoredController>();
       TurnP2 = SC.TurnP2;
       Abilities2 = new List<GameObject>(Abilities1);
       RoundNum = 1;
       P1Spawn = SpawnP1.transform.position;
       P2Spawn = SpawnP2.transform.position;
       RoundTimer = RoundTime;
       FirstPause();
   }

   private void Update()
   {
       RoundTimerReset();
       
   }
   
   private void RoundTimerReset()
   {
       RoundTimer -= Time.deltaTime;

       if (RoundTimer <= 0)
       {
           if (!SC.roundSwitch && !TurnP2 && RoundNum >= P1Cs.Length && !ReplayOnly || SC.roundSwitch && TurnP2 && RoundNum >= P2Cs.Length && !ReplayOnly)
           {
               ReplayOnly = true;
               Debug.Log(ReplayOnly);
           }

           else if (!TurnP2 && RoundNum < P1Cs.Length)
           {
               P1Cs[RoundNum].GameObject().GetComponent<PlayerController>().StopRecording();
               P1Spawn = new Vector3(P1Spawn.x, P1Spawn.y, P1Spawn.z - 1);
               TurnP2 = true;
               if (SC.roundSwitch)RoundNum = !ReplayOnly ? RoundNum + 1 : RoundNum;
           }
           else if (TurnP2 && RoundNum < P2Cs.Length)
           {
               P2Cs[RoundNum].GameObject().GetComponent<PlayerController>().StopRecording();
               P2Spawn = new Vector3(P2Spawn.x, P2Spawn.y, P2Spawn.z - 1);
               TurnP2 = false;
               if (!SC.roundSwitch)RoundNum = !ReplayOnly ? RoundNum + 1 : RoundNum;
           }
           
           
           HandleCharacters();
           if (ob_nest != null) ob_nest.GetComponent<NestController>().RoundReset();
           if (ob_crown != null) ob_crown.GetComponent<CrownController>().RoundReset();
           if (ob_tnt1 != null) ob_tnt1.GetComponent<TntController>().RoundReset();
           if (ob_tnt2 != null) ob_tnt2.GetComponent<TntController>().RoundReset();
           if (ob_mite != null) ob_mite.GetComponent<DynamiteController>().RoundReset();
           RoundTimer = RoundTime;
           
       }




       
   }
   
   
   private void HandleCharacters()
   {
       
       if (ReplayOnly && !doOnce)
       {
           Camera.GetComponent<CameraController>().ZoomOut(SpawnP1.transform.position.x + SpawnP2.transform.position.x);
           
           for (var i = RoundNum-1; i > 0 ; i--)
           {
               
               if (P1Cs[i] != null)
                   P1Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();
               
               if (P2Cs[i] != null)
                   P2Cs[i].GameObject().GetComponent<PlayerController>().StartReplay();

           }
           
           doOnce = true;
       }

       else if (ReplayOnly && doOnce)
       {
           MidPause();
       }

       else if (!ReplayOnly)
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

               Pause();

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
               
               Pause();


           }
           
           
       }
   }

   private void CreateCharacter()
   {
       if (!TurnP2)
       {
           P1Cs[RoundNum] = Instantiate(PF_Char, P1Spawn, Quaternion.identity);
           P1Cs[RoundNum].tag = "P1";
           P1Cs[RoundNum].name = "P1C"+RoundNum;
           P1Cs[RoundNum].GetComponent<PlayerController>().Ability = feedAbility;
           
           if (Camera.GetComponent<CameraController>().target == null)
               Camera.transform.position =
                   new Vector3(SpawnP1.transform.position.x, Camera.transform.position.y, Camera.transform.position.z);
           
           Camera.GetComponent<CameraController>().target = P1Cs[RoundNum].transform;
       }

       else
       {
           P2Cs[RoundNum] = Instantiate(PF_Char, P2Spawn, Quaternion.identity);
           P2Cs[RoundNum].tag = "P2";
           P2Cs[RoundNum].name = "P2C"+RoundNum;
           P2Cs[RoundNum].GetComponent<PlayerController>().Ability = feedAbility;
           
           if (Camera.GetComponent<CameraController>().target == null)
               Camera.transform.position =
                   new Vector3(SpawnP2.transform.position.x, Camera.transform.position.y, Camera.transform.position.z);
           
           Camera.GetComponent<CameraController>().target = P2Cs[RoundNum].transform;

       }
   }

   private void FirstPause()
   {
       Time.timeScale = 0f;
       UIObjInfo.GameObject().SetActive(true);
   }

   public void Pause()
   {
       Time.timeScale = 0f;
       if (!TurnP2)
       {
           UISelect1.GameObject().SetActive(true);
           UINum1.text = RoundNum.ToString();
       }
       else
       {
           UISelect2.GameObject().SetActive(true);
           UINum2.text = RoundNum.ToString();
       }
   }

   public void ButtonTrap()
   {
       if (!TurnP2)
       {
           feedAbility = Abilities1.Find(o => o.CompareTag("Trap1"));
           Abilities1.Remove(feedAbility);
           PlaynCreate();
           UISelect1.GameObject().SetActive(false);
       }
       else
       {
           feedAbility = Abilities2.Find(o => o.CompareTag("Trap1"));
           Abilities2.Remove(feedAbility);
           PlaynCreate();
           UISelect2.GameObject().SetActive(false);
       }
   }
   
   public void ButtonTurret()
   {
       if (!TurnP2)
       {
           feedAbility = Abilities1.Find(o => o.CompareTag("Turret"));
           Abilities1.Remove(feedAbility);
           PlaynCreate();
           UISelect1.GameObject().SetActive(false);

       }
       else
       {
           feedAbility = Abilities2.Find(o => o.CompareTag("Turret"));
           Abilities2.Remove(feedAbility);
           PlaynCreate();
           UISelect2.GameObject().SetActive(false);

       }
   }
   
   public void ButtonSprint()
   {
       if (!TurnP2)
       {
           feedAbility = Abilities1.Find(o => o.CompareTag("Sprint"));
           Abilities1.Remove(feedAbility);
           PlaynCreate();
           UISelect1.GameObject().SetActive(false);

       }
       else
       {
           feedAbility = Abilities2.Find(o => o.CompareTag("Sprint"));
           Abilities2.Remove(feedAbility);
           PlaynCreate();
           UISelect2.GameObject().SetActive(false);

       }
   }
   
   public void ButtonArmor()
   {
       if (!TurnP2)
       {
           feedAbility = Abilities1.Find(o => o.CompareTag("Armor"));
           Abilities1.Remove(feedAbility);
           PlaynCreate();
           UISelect1.GameObject().SetActive(false);

       }
       else
       {
           feedAbility = Abilities2.Find(o => o.CompareTag("Armor"));
           Abilities2.Remove(feedAbility);
           PlaynCreate();
           UISelect2.GameObject().SetActive(false);

       }
   }
   
   public void ButtonHammer()
   {
       if (!TurnP2)
       {
           feedAbility = Abilities1.Find(o => o.CompareTag("Hammer"));
           Abilities1.Remove(feedAbility);
           PlaynCreate();
           UISelect1.GameObject().SetActive(false);

       }
       else
       {
           feedAbility = Abilities2.Find(o => o.CompareTag("Hammer"));
           Abilities2.Remove(feedAbility);
           PlaynCreate();
           UISelect2.GameObject().SetActive(false);
       }
   }
   
   private void PlaynCreate()
   {
       Time.timeScale = 1f;
       CreateCharacter();
   }
   
   private void MidPause()
   {
       Time.timeScale = 0f;
       UIPointCount.GameObject().SetActive(true);
       var nest = ob_nest.GetComponent<NestController>();
       var crown = ob_crown.GetComponent<CrownController>();
       var tnt1 = ob_tnt1.GetComponent<TntController>();
       var tnt2 = ob_tnt2.GetComponent<TntController>();
       if (!SC.roundSwitch)
       {
           
           if (nest.p1W)
           {
               Rnd1Nest.text = "P1";
               SC.Rnd1Nest = 1;
           }
           else if (nest.p2W)
           {
               Rnd1Nest.text = "P2";
               SC.Rnd1Nest = 2;
           }
           
           
           if (crown.p1W)
           {
               Rnd1Crown.text = "P1";
               SC.Rnd1Crown = 1;
           }
           else if (crown.p2W)
           {
               Rnd1Crown.text = "P2";
               SC.Rnd1Crown = 2;
           }
           
           
           if (tnt2.p1W)
           {
               Rnd1Tnt.text = "P1";
               SC.Rnd1Tnt = 1;
           }
           else if (tnt1.p2W)
           {
               Rnd1Tnt.text = "P2";
               SC.Rnd1Tnt = 2;
           }
       }
       else
       {
           if (SC.Rnd1Nest == 1)
               Rnd1Nest.text = "P1";
           else if (SC.Rnd1Nest == 2) 
               Rnd1Nest.text = "P2";
           
           

           if (SC.Rnd1Crown == 1)
               Rnd1Crown.text = "P1";
           else if (SC.Rnd1Crown == 2)
               Rnd1Crown.text = "P2";
               
           
           
           if (SC.Rnd1Tnt == 1)
               Rnd1Tnt.text = "P1";
           
           else if (SC.Rnd1Tnt == 2)
               Rnd1Tnt.text = "P2";
           
           
           
           
           if (nest.p1W)
           {
               Rnd2Nest.text = "P1";
               SC.Rnd2Nest = 1;
           }
           else if (nest.p2W)
           {
               Rnd2Nest.text = "P2";
               SC.Rnd2Nest = 2;
           }
           
           
           if (crown.p1W)
           {
               Rnd2Crown.text = "P1";
               SC.Rnd2Crown = 1;
           }
           else if (crown.p2W)
           {
               Rnd2Crown.text = "P2";
               SC.Rnd2Crown = 2;
           }
           
           
           if (tnt2.p1W)
           {
               Rnd2Tnt.text = "P1";
               SC.Rnd2Tnt = 1;
           }
           else if (tnt1.p2W)
           {
               Rnd2Tnt.text = "P2";
               SC.Rnd2Tnt = 2;
           }

       }
   }

   public void RoundSwitch()
   {
       SC.TurnP2 = TurnP2;
       SC.roundSwitch = true;
       SceneManager.LoadScene(SceneManager.GetActiveScene().name);

   }

}
