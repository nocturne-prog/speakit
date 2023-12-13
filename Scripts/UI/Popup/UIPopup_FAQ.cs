using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Marvrus.UI
{
    public class UIPopup_FAQ : UIPopup
    {
        public FAQItem[] items;

        public override void UpdateData(params object[] _args)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].Init();
            }

            base.UpdateData(_args);
        }
    }
}
