using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marvrus.UI
{
    public class PageIndicator : MonoBehaviour
    {
        public GameObject obj_on;
        public GameObject obj_off;

        public bool IsOn
        {
            get { return obj_on.activeInHierarchy; }
            set
            {
                obj_on.SetActive(value);
                obj_off.SetActive(!value);
            }
        }
    }
}


