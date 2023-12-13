using DC = Marvrus.Data.DataContainer;
using DF = Marvrus.Util.Define;
using Marvrus;
using Marvrus.Util;
using UnityEngine;
using Marvrus.UI;
using Marvrus.Episodes;
using System;
using System.Collections;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] EpisodeManager episodeManager;
    protected override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;

        if (DF.platform == DF.Platform.Android)
        {
            ApplicationChrome.statusBarState = ApplicationChrome.States.Visible;
        }

        UIManager.s.Init();
        DeepLinkManager.s.Init();
        UIManager.s.OpenPopup<UIPopup_Splash>();

        Invoke(nameof(SafeAreaInit), 1f);

    }

    private void SafeAreaInit()
    {
        UIManager.s.InitSafeArea();
    }

    public void StartEpisode(string filename, Action success, Action error)
    {
        episodeManager.StartEpisode(filename, () =>
        {
            UIManager.s.EnterEpisode();
            success?.Invoke();
        }, error);
    }

    public void StartEpisode(int _id)
    {
        bool isTrial = DC.IsTrial(_id);

        UIManager.s.Loading = true;

        if (isTrial is true)
        {
            StartEpisodeProcess(_id);
        }
        else
        {
            if (DC.IsLogin is true)
            {
                StartEpisodeProcess(_id);
            }
            else
            {
                UIManager.s.Loading = false;
                UIManager.s.OpenPopup<UIPopup_ActionSheet>(_addtive: true);
            }
        }
    }

    private void StartEpisodeProcess(int _id)
    {
        NetworkManager.s.GetEpisodeDetail(_id, (epiDetail) =>
        {
            string[] splitUrl = epiDetail.resource_url.Split("/");
            string fileName = splitUrl[splitUrl.Length - 1];

            FileLoader.s.DownloadFile(epiDetail.resource_url, fileName, (result) =>
            {
                string epiName = fileName.Split(".")[0];

                if (DC.IsLogin is true)
                {
                    NetworkManager.s.GetMyEpisodeResult(_id, (myResult) =>
                    {
                        StartEpisode(epiName, StartEpisodeSuccess, StartEpisodeError);
                    }, StartEpisodeError);
                }
                else
                {
                    StartEpisode(epiName, StartEpisodeSuccess, StartEpisodeError);
                }
            }, StartEpisodeError);
        }, StartEpisodeError);
    }

    private void StartEpisodeSuccess()
    {
        UIManager.s.Loading = false;
    }

    private void StartEpisodeError(string _error)
    {
        UIManager.s.Loading = false;
        UIManager.s.OpenErrorPopup();
    }

    private void StartEpisodeError()
    {
        UIManager.s.Loading = false;
        UIManager.s.OpenErrorPopup();
    }

    public void EndEpisode()
    {
        episodeManager.FinshEpisode();

        UIManager.s.ExitEpisode();
        UIManager.s.CloseAll();
        UIManager.s.OpenPopup<UIPopup_Home>();
    }

    public void QuitApplication()
    {
        if (DF.platform == DF.Platform.UnityEditor)
        {
            Debug.Log("Quit Applicatioin !!");
        }
        else
        {
            Application.Quit();
        }
    }
}
