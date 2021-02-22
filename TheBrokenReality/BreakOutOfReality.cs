using PixelCrushers.DialogueSystem;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using ThrowObjects;

namespace MyBrokenReality
{
    public class BreakOutOfReality : MonoBehaviour
    {
        EdgeLord player;
        Camera headcam, freecam;
        GameObject head, freecam_obj, can;
        IScrController pauser;
        bool disconnect_camera = false,
            do_once_on = true,
            do_once_off = false,
            show_ui = false,
            show_log = false;
        float speed = 1f;
        List<string> log = new List<string>();
        Vector2 rotation = Vector2.zero;

        class LevelInfo
        {
            public string display_name;
            public string level_name;
        }
        Dictionary<int, LevelInfo> levels = new Dictionary<int, LevelInfo>()
        {
            {1, new LevelInfo{display_name="Start Screen", level_name="01Menu" } },
            {2, new LevelInfo{display_name="Domo Paradisso", level_name="02Domo" } },
            {3, new LevelInfo{display_name="Aquanet", level_name="03Atlantis" } },
            {4,  new LevelInfo{display_name="Axis Plaza", level_name="04Axis" } },
            {5,  new LevelInfo{display_name="Love Cruise 64", level_name="05Cruise" } },
            {6,  new LevelInfo{display_name="Geocity", level_name="06Geocity" } },
            {7,  new LevelInfo{display_name="Innernet", level_name="07Innernet" } },
            {8,  new LevelInfo{display_name="Real World", level_name="08Room" } },
            {9,  new LevelInfo{display_name="Ending Credits", level_name="09Credits" } }
        };

        public void Start()
        {
            //Do stuff here once for initialization
            AddToLog(log, "Break free!");

            speed = player.Speed;
            UnityEngine.Application.targetFrameRate = 300;
        }

        public void AddToLog(List<string> log, string msg)
        {
            if (log.Count > 25)
                log.RemoveAt(0);

            log.Add(msg);
        }

        public void Update()
        {
            //Do stuff here on every tick

            if (!player)
            {
                player = FindObjectOfType<EdgeLord>();

                if (player)
                {
                    head = GameObject.Find("Head");
                    headcam = head.GetComponent<Camera>();
                    if (ReferenceEquals(headcam, null))
                        headcam = Camera.main;

                    //var components = new List<string>();
                    //foreach (var component in headcam.GetComponents<Component>())
                    //{
                    //    if (component != this) components.Add(component.ToString());
                    //    AddToLog(log, $"\t{component}");
                    //}

                    freecam_obj = new GameObject("Free Camera Object");
                    freecam = freecam_obj.AddComponent<Camera>();
                    freecam.CopyFrom(headcam);
                    freecam.enabled = false;

                    pauser = GameObject.Find("Game").GetComponent<TheBrokenReality>()._ScreenController.GetComponent<IScrController>();

                    AddToLog(log, $"head: {head}");
                    AddToLog(log, $"freecam: {freecam}");
                    AddToLog(log, $"headcam: {headcam}");
                    AddToLog(log, $"Camera.main: {Camera.main}");
                    AddToLog(log, $"Pauser: {pauser}");
                    AddToLog(log, $"Level: {UnityEngine.SceneManagement.SceneManager.GetActiveScene().name}");
                }
            }

            if (Input.GetKeyDown(KeyCode.Insert))
            {
                disconnect_camera = !disconnect_camera;
                DialogueManager.ShowAlert(string.Format("Freecam: {0}", disconnect_camera ? "ON" : "OFF"));
                AddToLog(log, string.Format("Freecam: {0}", disconnect_camera ? "ON" : "OFF"));
                sleep(2);
            }

            if (Input.GetKeyDown(KeyCode.Home))
            {
                show_ui = !show_ui;

                pauser.Pause();
                GameObject.Find("[UI]PauseController").SetActive(!show_ui);
            }

            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                show_log = !show_log;
                if (!ReferenceEquals(can, null))
                {
                    var playerRotation = player.transform.rotation;
                    float spawnDistance = 10;

                    var spawnPos = player.transform.position + player.transform.forward * spawnDistance;

                    Instantiate(can, spawnPos, playerRotation);
                }
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                speed = Mathf.Clamp(speed += 0.2f, 0f, 20f);
                AddToLog(log, $"Speed: {speed}");
                sleep(2);
            }

            if (Input.GetKeyDown(KeyCode.LeftControl))
            {
                speed = Mathf.Clamp(speed -= 0.2f, 0f, 20f);
                AddToLog(log, $"Speed: {speed}");
                sleep(2);
            }
        }

        IEnumerator sleep(float time)
        {
            yield return new WaitForSecondsRealtime(time);
        }

        public void FixedUpdate()
        {
            if (ReferenceEquals(can, null))
            {
                var holdables = GameObject.FindObjectsOfType<Holdable>();
                if (holdables.Length > 0)
                {
                    foreach(var holdable in holdables)
                    {
                        if(holdable.gameObject.GetComponent<Prop>().Reeciclable)
                        {
                            can = holdable.gameObject;
                            DontDestroyOnLoad(can);
                            AddToLog(log, "Copied can data!");
                            break;
                        }   
                    }
                }
                else
                    can = null;
            }

            if (disconnect_camera)
            {
                if (do_once_on)
                {
                    freecam_obj = new GameObject("Free Camera Game Object");
                    freecam = freecam_obj.AddComponent<Camera>();
                    freecam.CopyFrom(headcam);

                    freecam.enabled = true;
                    headcam.enabled = false;
                    //AddToLog(log, $"Camera.main: {Camera.main}");
                    do_once_on = false;
                }

                do_once_off = true;
                player.Speed = 0f;

                if (Input.GetButton("DMTForward"))
                    freecam.transform.position += freecam.transform.forward * speed;

                if (Input.GetButton("DMTBackward"))
                    freecam.transform.position -= freecam.transform.forward * speed;

                if (Input.GetButton("DMTLeftward"))
                    freecam.transform.position -= freecam.transform.right * speed;

                if (Input.GetButton("DMTRightward"))
                    freecam.transform.position += freecam.transform.right * speed;

                if (Input.GetKey(KeyCode.E))
                    freecam.transform.position += freecam.transform.up * speed;

                if (Input.GetKey(KeyCode.Q))
                    freecam.transform.position -= freecam.transform.up * speed;

                rotation.y += Input.GetAxis("Mouse X") * Time.deltaTime;
                rotation.x += -Input.GetAxis("Mouse Y") * Time.deltaTime;

                //freecam.transform.eulerAngles = new Vector2(0, rotation.y) * 5;
                freecam.transform.localRotation = Quaternion.Euler(rotation.x * 3, rotation.y * 3, 0);
            }
            else if (do_once_off)
            {
                freecam.enabled = false;
                headcam.enabled = true;
               // AddToLog(log, $"Camera.main: {Camera.main}");

                player.Speed = 2f;
                player.AccomodateHead();
                do_once_off = false;
                do_once_on = true;
            }

        }
        public void OnGUI()
        {
            if (show_ui)
            {
                // Make a background box
                GUI.Box(new Rect(10, 10, 150, 300), "The Broken Reality");

                int y_offset = 40;

                foreach (var level in levels)
                {
                    if (GUI.Button(new Rect(20, y_offset, 120, 20), level.Value.display_name))
                    {
                        show_ui = false;
                        GameObject.Find("Game").GetComponent<TheBrokenReality>().LvMgr.LoadLevel(level.Value.level_name);
                        AkSoundEngine.StopAll();
                        //UnityEngine.SceneManagement.SceneManager.LoadScene(level.Key);
                    }
                    y_offset += 30;
                }
            }

            if (show_log)
            {
                GUI.Box(new Rect(180, 10, 400, 480), "Log");
                ZatsRenderer.DrawString(new Vector2(190, 30), string.Join("\n", log.ToArray()), false);

            }

        }
    }
}
