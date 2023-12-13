using Marvrus.Episodes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Marvrus.Util
{

    public class FileLoader : Singleton<FileLoader>
    {
        private bool isThreadRunning = false;
        private Queue<IEnumerator> queue = new();

        protected override void Awake()
        {
            base.Awake();
        }

        public void GetImage(string _url, Action<Texture2D> _success, Action<string> _error = null)
        {
            if (string.IsNullOrEmpty(_url))
            {
                Debug.LogError($"FileCache.GetImage url is null");
                return;
            }

            string[] splitUrl = _url.Split("/");
            string fileName = splitUrl[splitUrl.Length - 1];
            string fullPath = $"{Const.FILE_PAHT}/{fileName}";

            Debug.Log($"LoadImage :: {fullPath}");

            if (File.Exists(fullPath))
            {
                LoadImage(fullPath, (texture) =>
                {
                    _success(texture);
                }, _error);
            }
            else
            {
                DownloadFile(_url, fileName, (path) =>
                {
                    LoadImage(fullPath, (texture) =>
                    {
                        _success(texture);
                    }, _error);
                }, (error) =>
                {
                    if (_error is null)
                        Debug.LogError($"FileCache.GetImage Error :: {error}");
                    else
                        _error(error);
                });
            }
        }

        private void LoadImage(string _path, Action<Texture2D> _success, Action<string> _error = null)
        {
            _ = LoadImageAync(_path, (texture) =>
            {
                _success(texture);
            }, (error) =>
            {
                _error?.Invoke(error);
            });
        }
        public  void LoadEpisode(string fileName, Action<Episode> _success, Action<string> _error = null)
        {
            
            if (Directory.Exists(Const.FILE_PAHT) is false)
                Directory.CreateDirectory(Const.FILE_PAHT);
            try
            {
                string fullPath = $"{Const.FILE_PAHT}/{fileName}.json";
                string saveString = File.ReadAllText(fullPath);

                Episode _episode = JsonUtility.FromJson<Episode>(saveString);
                _success.Invoke(_episode);
            }
            catch (Exception e) 
            {
                if (_error is null)
                {
                    Debug.LogError($"LoadEpisode Error !! {fileName} :: {e}");
                }
                else
                {
                    _error(e.ToString());
                }
            }

        }


        private async Task LoadImageAync(string _path, Action<Texture2D> _callback, Action<string> _error)
        {
            try
            {
                byte[] bytes = await File.ReadAllBytesAsync(_path);

                Texture2D texture = new Texture2D(1, 1);
                texture.LoadImage(bytes);

                _callback(texture);
            }
            catch (Exception e)
            {
                if (_error is null)
                {
                    Debug.LogError($"LoadIamge Error !! {_path} :: {e}");
                }
                else
                {
                    _error(e.ToString());
                }
            }
        }

        public void DownloadFile(string _url, string _fileName, Action<string> _success, Action<string> _error)
        {
            queue.Enqueue(Co_DownloadFile(_url, _fileName, _success, _error));

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

        private IEnumerator Co_DownloadFile(string _url, string _fileName, Action<string> _success, Action<string> _error)
        {
            if (string.IsNullOrEmpty(_url))
            {
                Debug.Log($"{_url} is null.. send fail callback");
                _error?.Invoke(string.Empty);
            }

            UnityWebRequest req = UnityWebRequest.Get(_url);
            Debug.Log($"download File :: {_url}");
            yield return req.SendWebRequest();

            if (req.result is UnityWebRequest.Result.Success)
            {
                if (Directory.Exists(Const.FILE_PAHT) is false)
                    Directory.CreateDirectory(Const.FILE_PAHT);

                string path = $"{Const.FILE_PAHT}/{_fileName}";

                _ = WriteImageAyncAsync(path, req.downloadHandler.data, () =>
                {
                    Debug.Log($"Write File :: {path}");
                    _success?.Invoke(path);
                });
            }
            else
            {
                _error?.Invoke(req.error);
            }

            req.Dispose();
        }

        private async Task WriteImageAyncAsync(string _path, byte[] _data, Action _callback)
        {
            await File.WriteAllBytesAsync(_path, _data);
            _callback();
        }
    }
}