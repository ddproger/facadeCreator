using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FacadeCreatorApi.Services
{
    public class IniFileReader
    {
        public static string getValueFromFile(string block, string key)
        {
            //Для получения значения
            try
            {
                StringBuilder buffer = new StringBuilder(SIZE);

                //Получить значение в buffer
                GetPrivateString(block, key, null, buffer, SIZE, StringResources.getPathToSpaceIni());

                //Вернуть полученное значение
                return buffer.ToString();
            }
            catch (Exception)
            {           
                return "";
            }            
        }
        private const int SIZE = 1024; //Максимальный размер (для чтения значения из файла)
       

        //Импорт функции GetPrivateProfileString (для чтения значений) из библиотеки kernel32.dll
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        private static extern int GetPrivateString(string section, string key, string def, StringBuilder buffer, int size, string path);

        //Импорт функции WritePrivateProfileString (для записи значений) из библиотеки kernel32.dll
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        private static extern int WritePrivateString(string section, string key, string str, string path);
    }

}

