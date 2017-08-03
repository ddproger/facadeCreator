using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public class RegeditService
    {
        RegistryKey reg = Registry.CurrentUser;
        private const string OPEN_FILE_PATH = "OpenFilePath";
        public string getOpenFilePath()
        {
            return getKeyValue(OPEN_FILE_PATH);
        }

        public void setOpenFilePath(string path)
        {
            setKeyValue(OPEN_FILE_PATH, path);
        }

        private string getKeyValue(String key)
        {
            return (String)reg.GetValue(key, "");
        }
        private void setKeyValue(String key,string value)
        {
            if (key == null || value == null || key.Equals("") || value.Equals("")) return;

            reg.SetValue(key, value, RegistryValueKind.String);
        }
    }
}
