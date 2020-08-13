﻿using Multiplayer.Debugging;
using Multiplayer.Extensions;
using Multiplayer.Networking;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Multiplayer.Core
{
    public class MainMenuManager : ModBehaviour
    {
        public override void OnActivate()
        {
            Scene currentScene = SceneManager.GetActiveScene();
            if (currentScene.name == "MainMenu")
            {
                CreateButton();
            }
            SceneManager.sceneLoaded += SceneChanged;
        }
        private void CreateButton()
        {
            RectTransform mainMenuPanel = WindowManager.FindElementPath("MainPanel/Panel");
            RectTransform toCopy = WindowManager.FindElementPath("MainPanel/Panel/Button 5");
            Button butt = Instantiate(toCopy).GetComponent<Button>();
            butt.onClick.RemoveAllListeners();
            butt.onClick = new Button.ButtonClickedEvent();
            butt.onClick.AddListener(MainMenuButtonClick);
            butt.GetComponentInChildren<Text>().text = "Multiplayer";
            Sprite texture = butt.image.sprite;           
            LaunchBehaviour.ActiveObjects.Add(butt.gameObject);
            WindowManager.AddElementToElement(butt.gameObject, mainMenuPanel.gameObject, new Rect(0, 0, texture.rect.width, texture.rect.height), Rect.zero);
            butt.transform.SetSiblingIndex(1);
            Logging.Debug("Added menu button.");
        }
        private void SceneChanged(Scene scene, LoadSceneMode mode)
        {
            if (scene.name != "MainMenu")
            {
                isWindowUp = false;
                return;
            }
            CreateButton();
        }
        private bool isWindowUp = false;
        private void MainMenuButtonClick()
        {
            if (isWindowUp)
            {
                PopupManager.NotificationSound.Warning.Play();
                return;
            }
            isWindowUp = true;
            GUIWindow window = WindowManager.SpawnWindow();

            Button OptionsButton = WindowManager.SpawnButton();
            OptionsButton.gameObject.GetComponentInChildren<Text>().text = "MPOptionsWindow_OptionsButton".LocDef("Options");


            Button ServerIButton = WindowManager.SpawnButton();
            ServerIButton.gameObject.GetComponentInChildren<Text>().text = "MPOptionsWindow_ServerIButton".LocDef("Server Info");
          
            window.NonLocTitle = "Manage Multiplayer";
            window.Title = "MPOptionsWindowTitle".LocDef("Manage Multiplayer");
            window.MinSize = new Vector2(128f + 1f + 128f, Screen.height / 2f);
            window.ShowCentered = true;            
            window.Show();
            window.SizeButton.SetActive(false);
            WindowManager.AddElementToElement(OptionsButton.gameObject, window.MainPanel, new Rect(0, 0, 128, 32), Rect.zero);
            WindowManager.AddElementToElement(ServerIButton.gameObject, window.MainPanel, new Rect(129, 0, 128, 32), Rect.zero);

            List<GameObject> OptionsPanel = new List<GameObject>() { };

            #region Options Panel

            #endregion

            List<GameObject> ServerIPanel = new List<GameObject>() { };

            #region Server Info Panel
            Text SHeader = WindowManager.SpawnLabel();
            SHeader.text = "MPOptionsWindow_SHeader".LocDef("Your server:");
            SHeader.fontSize = 16;
            
            Text SConnectIP = WindowManager.SpawnLabel();

            string serverPort;
            if(!PlayerPrefs.HasKey("cachedIP"))
            {
                serverPort = Networking.Server.Port.ToString(); //ServerClass.GetDefaultPort().ToString();
            } else
            {
                serverPort = (string)JsonConvert.DeserializeObject(PlayerPrefs.GetString("cachedIP"));
            }

            SConnectIP.text = "MPOptionsWindow_SConnectIP".LocDef("Server IP:") + $" <color=blue>{IPUtils.GetIP()}:{serverPort}</color>";

            ServerIPanel.AddBulk(SConnectIP.gameObject, SHeader.gameObject);
            #endregion

            OptionsButton.onClick.AddListener(() => {
                ServerIPanel.ForEach((GameObject e) => e.SetActive(false));
                OptionsPanel.ForEach((GameObject e) => e.SetActive(true));
            });
            ServerIButton.onClick.AddListener(() => {
                OptionsPanel.ForEach((GameObject e) => e.SetActive(false));
                ServerIPanel.ForEach((GameObject e) => e.SetActive(true));
            });
            ServerIPanel.ForEach((GameObject e) => e.SetActive(false));
            OptionsPanel.ForEach((GameObject e) => e.SetActive(true));
            WindowManager.AddElementToElement(SHeader.gameObject, window.MainPanel, new Rect(0, 38, 192, 32), Rect.zero);
            WindowManager.AddElementToElement(SConnectIP.gameObject, window.MainPanel, new Rect(0, 55, 192, 42), Rect.zero);
            window.GetComponentsInChildren<Button>().SingleOrDefault(x => x.name == "CloseButton").onClick.AddListener(() => isWindowUp = false);
        }

        public override void OnDeactivate()
        {

        }
    }
}
