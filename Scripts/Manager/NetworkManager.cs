using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using BestHTTP;
using DC = Marvrus.Data.DataContainer;
using DF = Marvrus.Util.Define;
using PPM = Marvrus.Util.PlayerPrefsManager;
using Marvrus.Util;

namespace Marvrus
{
    #region Protocol.
    public class Protocol
    {
       
    }
    #endregion

    public class NetworkManager : Singleton<NetworkManager>
    {
        #region Base
        public bool isThreadRunning { get; private set; } = false;
        Queue<IEnumerator> queue = new Queue<IEnumerator>();

        public void Send_Immediately<T>(string _uri, string _data, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isAuth = false)
        {
            StartCoroutine(Co_Send(_uri, _data, _method, _success, _fail, _isIncludeToken, _isAuth));
        }

        public void Send(string _uri, HTTPMethods _method, Action<string> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isAuth = false)
        {
            Send<string>(_uri, string.Empty, _method, _success, _fail, _isIncludeToken, _isAuth);
        }

        public void Send<T>(string _uri, string _data, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isAuth = false)
        {
            queue.Enqueue(Co_Send(_uri, _data, _method, _success, _fail, _isIncludeToken, _isAuth));

            if (isThreadRunning is false)
            {
                StartCoroutine(ThreadQueue());
            }
        }

        public void SendForm_Send_Immediately<T>(string _uri, Dictionary<string, string> _fieldData, Dictionary<string, List<byte[]>> _fileData, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null)
        {
            StartCoroutine(Co_SendForm(_uri, _fieldData, _fileData, _method, _success, _fail));
        }

        public void SendForm<T>(string _uri, Dictionary<string, string> _fieldData, Dictionary<string, List<byte[]>> _fileData, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null)
        {
            queue.Enqueue(Co_SendForm(_uri, _fieldData, _fileData, _method, _success, _fail));

            if (isThreadRunning is false)
            {
                StartCoroutine(ThreadQueue());
            }
        }

        private IEnumerator ThreadQueue()
        {
            isThreadRunning = true;

            while (queue.Count > 0)
            {
                yield return StartCoroutine(queue.Dequeue());
            }

            isThreadRunning = false;
        }

        private IEnumerator Co_Send<T>(string _uri, string _data, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null, bool _isIncludeToken = false, bool _isAuth = false)
        {
            string targetServeruri = _isAuth ? mainAuthUrl : mainUrl;

            Uri uri = new Uri($"{targetServeruri}/{_uri}");
            HTTPRequest req = new HTTPRequest(uri, _method);

            req.AddHeader("Content-Type", "application/json");

            Debug.Log($"Send :: {uri}");

            if (_isIncludeToken is true)
            {
                req.AddHeader("Authorization", $"Bearer {DC.Token}");
            }

            if (string.IsNullOrEmpty(_data) is false)
            {
                req.RawData = Encoding.UTF8.GetBytes(_data);
                Debug.Log($"Data :: {_data}");
            }

            yield return req.Send();

            ResponseResult(req, _success, _fail);

            req.Dispose();
            yield break;
        }

        private IEnumerator Co_SendForm<T>(string _uri, Dictionary<string, string> _fieldData, Dictionary<string, List<byte[]>> _fileData, HTTPMethods _method, Action<T> _success = null, Action<string> _fail = null)
        {
            Uri uri = new Uri($"{mainUrl}/{_uri}");
            HTTPRequest req = new HTTPRequest(uri, _method);
            req.FormUsage = BestHTTP.Forms.HTTPFormUsage.Multipart;
            req.AddHeader("Authorization", $"Bearer {DC.Token}");

            if (_fieldData != null)
            {
                foreach (var v in _fieldData)
                {
                    req.AddField(v.Key, v.Value);
                }
            }

            if (_fileData != null)
            {
                foreach (var v in _fileData)
                {
                    for (int i = 0; i < v.Value.Count; i++)
                    {
                        req.AddBinaryData(v.Key, v.Value[i]);
                    }
                }
            }

            Debug.Log($"Send :: {uri}");
            Debug.Log($"Data :: {JsonConvert.SerializeObject(_fieldData)}");
            Debug.Log($"Media Data :: {JsonConvert.SerializeObject(_fileData)}");

            yield return req.Send();

            ResponseResult(req, _success, _fail);

            req.Dispose();
        }

        private void ResponseResult<T>(HTTPRequest _req, Action<T> _success = null, Action<string> _fail = null)
        {
            switch (_req.State)
            {
                case HTTPRequestStates.Finished:
                    if (_req.Response.IsSuccess)
                    {
                        Debug.Log($"Receive :: {_req.Response.DataAsText}");
                        T resultData = JsonConvert.DeserializeObject<T>(_req.Response.DataAsText);
                        _success?.Invoke(resultData);
                    }
                    else
                    {
                        Debug.LogError(string.Format("{3} :: Request finished Successfully, but the server sent an error. Status Code: {0}-{1} Message: {2}",
                                                        _req.Response.StatusCode, _req.Response.Message, _req.Response.DataAsText, _req.Uri));
                        if (_req.Response.DataAsText.Contains("NOT_FOUND"))    // SNS Login : Token NOT_FOUND
                        {
                            _fail?.Invoke(_req.Response.DataAsText);
                        }
                        else
                        {
                            _fail?.Invoke(_req.Response.DataAsText);
                        }
                    }
                    break;

                case HTTPRequestStates.Aborted:
                case HTTPRequestStates.ConnectionTimedOut:
                case HTTPRequestStates.TimedOut:
                case HTTPRequestStates.Error:
                    if (_fail is null)
                    {
                        Debug.LogError($"{_req.Uri} || {_req.State} || Request Finished with Error! " + (_req.Exception != null ? (_req.Exception.Message + "\n" + _req.Exception.StackTrace) : "No Exception"));
                    }
                    else
                    {
                        //UIManager.s.Dim = true;
                        //UIManager.s.OpenOneButton("", Const.NETWORK_ERROR, () =>
                        //{
                        //    UIManager.s.Dim = false;
                        //}, Const.OK);

                        if (_req.Exception is null)
                        {
                            Debug.LogError($"{_req.State} || Unknown Error..");
                        }
                        else
                        {
                            _fail.Invoke(_req.Exception.Message);
                        }
                    }
                    break;

                default:
                    Debug.LogError($"{_req.State} || Request Finished with Error! " + (_req.Exception != null ? (_req.Exception.Message + "\n" + _req.Exception.StackTrace) : "No Exception"));
                    break;
            }
        }
        #endregion

        #region Login.
        public void Login(string _id, string _password, Action<Protocol.LoginResponse> _success, Action<string> _error)
        {
            var uri = "-/-";
            var data = new Protocol.LoginRequest(_id, RSAModule.RSAEncrypt(_password), PPM.ClientID);
            var jsonData = JsonConvert.SerializeObject(data);
            Send<Protocol.LoginResponse>(uri, jsonData, HTTPMethods.Post, (result) =>
            {
                DC.Login(result);
                GetMyInfo(() =>
                {
                    _success?.Invoke(result);
                });

            }, (error) =>
            {
                _error?.Invoke(error);
            },
            _isIncludeToken: false,
            _isAuth: true);
        }

        public void AutoLogin(Protocol.AutoLoginRequest _data, Action<Protocol.LoginResponse> _success, Action<string> _error)
        {
            var uri = string.Empty;

            if (DF.platform == DF.Platform.UnityEditor)
            {
                uri = "*****";
            }
            else
            {
                uri = "*****";
            }

            var data = JsonConvert.SerializeObject(_data);
            Send<Protocol.LoginResponse>(uri, data, HTTPMethods.Post, (result) =>
            {
                DC.Login(result);
                GetMyInfo(() =>
                {
                    _success?.Invoke(result);
                });

            }, (error) =>
            {
                _error?.Invoke(error);
            },
            _isIncludeToken: false,
            _isAuth: true);
        }

        public void LogOut(Action<Protocol.MeemzDefault> _success, Action<string> _error)
        {
            var uri = "*****";
            Dictionary<string, string> dic = new Dictionary<string, string>()
            {
                { "refreshToken", PPM.RefreshToken }
            };

            var data = JsonConvert.SerializeObject(dic);
            Send<Protocol.MeemzDefault>(uri, data, HTTPMethods.Post, (result) =>
            {
                DC.LogOut();
                _success?.Invoke(result);
            }, (error) =>
            {
                _error?.Invoke(error);
            },
            _isIncludeToken: true,
            _isAuth: true);
        }
        #endregion

        #region Episodes.
        public void GetHome(Action<Protocol.Home> _success, Action<string> _fail)
        {
            string uri = "*****";

            Send<Protocol.Home>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                DC.GetHome(result);
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: DC.IsLogin);
        }

        public void GetEpisodesBySubCategory(int _id, Action<Protocol.EpisodesBySubCategory> _success, Action<string> _fail)
        {
            string uri = "*****";

            Send<Protocol.EpisodesBySubCategory>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                DC.GetEpisodeSubCategory(result);
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: DC.IsLogin);
        }

        public void GetEpisodeDetail(int _id, Action<Protocol.EpisodeDetail> _success, Action<string> _fail)
        {
            string uri = "*****";

            Send<Protocol.EpisodeDetail>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                DC.GetEpisodeDetail(result);
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: DC.IsLogin);
        }

        public void SendEpisodeStep(int _id, Protocol.MyEpisodeSaveStep _data, Action<Protocol.MyEpisodeResult> _success, Action<string> _fail)
        {
            string uri = "*****";

            var data = JsonConvert.SerializeObject(_data);

            Send<Protocol.MyEpisodeResult>(uri, data, HTTPMethods.Post, (result) =>
            {
                DC.GetMyEpisodeResult(result);
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: true);
        }

        public void SendEpisodeResult(int _id, Protocol.MyEpisodeSaveResult _data, Action<Protocol.MyEpisodeResult> _success, Action<string> _fail)
        {
            string uri = "*****";

            var data = JsonConvert.SerializeObject(_data);

            Send<Protocol.MyEpisodeResult>(uri, data, HTTPMethods.Post, (result) =>
            {
                DC.GetMyEpisodeResult(result);
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: true);
        }

        public void InitEpisodeResult(int _id)
        {
            string uri = "*****";

            Send<Protocol.MyEpisodeResult>(uri, string.Empty, HTTPMethods.Delete,
            _isIncludeToken: true);
        }
        public void InitEpisodeStep(int _episodeId, int _stepId)
        {
            string uri = "*****";

            Send<Protocol.MyEpisodeResult>(uri, string.Empty, HTTPMethods.Delete,
            _isIncludeToken: true);
        }


        #endregion

        #region My.
        public void GetVersionInfo(Action<Protocol.VersionInfo> _success, Action<string> _fail)
        {
            string uri = "*****";

            Send<Protocol.VersionInfo>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                DC.GetVersionInfo(result);
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            });
        }

        public void GetMyInfo(Action _success, Action<string> _fail = null)
        {
            string uri = "*****";

            Send<Protocol.MyInfo>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                DC.GetMyInfo(result);
                _success?.Invoke();
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: true);
        }

        public void GetMyEpisodesResult(Action<Protocol.MyEpisodesResult> _success, Action<string> _fail)
        {
            string uri = "*****";

            Send<Protocol.MyEpisodesResult>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: true);
        }

        public void GetMyEpisodeResult(int _id, Action<Protocol.MyEpisodeResult> _success, Action<string> _fail)
        {
            string uri = "*****";

            Send<Protocol.MyEpisodeResult>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                DC.GetMyEpisodeResult(result);
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: true);
        }
        public void GetNextEpisodeResult(int _id, Action<Protocol.MyEpisodeResult> _success, Action<string> _fail)
        {
            string uri = "*****";

            Send<Protocol.MyEpisodeResult>(uri, string.Empty, HTTPMethods.Get, (result) =>
            {
                _success?.Invoke(result);
            }, (fail) =>
            {
                _fail?.Invoke(fail);
            },
            _isIncludeToken: true);
        }

        #endregion
    }
}