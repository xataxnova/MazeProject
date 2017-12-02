using UnityEngine;
using System.Collections;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace TileMazeMaker 
{

    public class KGenerator
    {
        public const string C_GUID_KEY = "DEVICE_UNITUE_IDENTIFIER";

        private static void SetGUIDUniqueAndUnchangeable()
        {
            string old_key = PlayerPrefs.GetString(C_GUID_KEY, "");
            if (string.IsNullOrEmpty(old_key))
            {
                PlayerPrefs.SetString(C_GUID_KEY, SystemInfo.deviceUniqueIdentifier);
            }
        }

        public static string GetGUID()
        {
            SetGUIDUniqueAndUnchangeable();
            return PlayerPrefs.GetString(C_GUID_KEY);
        }

        public static byte[] GenKeyFromGUID()
        {
            string GUID = GetGUID();
            string F8 = GUID.Substring(0, 8);
            return System.Text.ASCIIEncoding.ASCII.GetBytes(F8);
        }

        public static byte[] GenIVFromGUID()
        {
            string GUID = GetGUID();
            string L8 = GUID.Substring(GUID.Length - 9, 8);
            return System.Text.ASCIIEncoding.ASCII.GetBytes(L8);
        }
    }

    public class FileUtil
    {
        private static ulong last_global_time;
        private static ulong frame_differ;

        //支持在同一个毫秒内，一口气创建最多65535个对象。
        //高48位存放时间戳，低16位存放标识符。用来区分一秒内创建多个数据的情况。
        //对于已经有存档的物件。会在一开始生成一个GUID，但是，读取之后，这个会被存档的GUID覆盖掉。
        //再次存盘的时候，自然也会保存的是，之前存档的GUID，所以这里没问题。
        public static ulong NextGUID
        {
            get
            {
                System.DateTime startTime = System.TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
                ulong time = (ulong)(System.DateTime.Now - startTime).TotalMilliseconds;
                if (time != last_global_time)
                {
                    last_global_time = time;
                    frame_differ = 1;
                }

                time = (time << 16) + frame_differ;
                frame_differ++;
                return time;
            }            
        }

        static BinaryFormatter bin_for;
        static bool mInitDataSL = false;
        static byte[] mKeys = { 0x44, 0x38, 0x12, 0x75, 0x32, 0x45, 0x91, 0xAA };
        static byte[] mIV = { 0x84, 0x19, 0x16, 0x03, 0x88, 0x19, 0x01, 0x24 };

        public delegate void voidDelegateVoid();
        public static event voidDelegateVoid eventOnDeserializeFailed;
        public static event voidDelegateVoid eventOnLoadClassFailed;


        public static void SetEnvironment()
        {
            if (mInitDataSL == false)
            {
                mInitDataSL = true;
            }
            else
            {
                return;
            }

            mKeys = KGenerator.GenKeyFromGUID();
            mIV = KGenerator.GenIVFromGUID();

            //设置环境变量
            System.Environment.SetEnvironmentVariable("MONO_REFLECTION_SERIALIZER", "yes");

            //声明二进制序列化
            bin_for = new BinaryFormatter();
        }

        public static byte[] SerializeObject(object obj, bool need_encrypt)
        {
            if (obj == null)
            {
                return null;
            }

            MemoryStream ms = new MemoryStream();

            try
            {
                bin_for.Serialize(ms, obj);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Unable to serlialize target object" + ex.Message);
            }

            ms.Position = 0;
            byte[] bytes = new byte[ms.Length];
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();

            if (need_encrypt)
            {
                DESCryptoServiceProvider dCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, dCSP.CreateEncryptor(mKeys, mIV), CryptoStreamMode.Write);

                cStream.Write(bytes, 0, bytes.Length);
                cStream.FlushFinalBlock();

                mStream.Position = 0;
                byte[] after = new byte[mStream.Length];
                mStream.Read(after, 0, after.Length);
                mStream.Close();

                return after;
            }

            return bytes;
        }

        /// <summary>
        /// 把字节数组反序列化成对象
        /// </summary>
        public static object DeserializeObject(byte[] bytes, bool need_encrypt)
        {
            if (need_encrypt)
            {
                DESCryptoServiceProvider DCSP = new DESCryptoServiceProvider();
                MemoryStream mStream = new MemoryStream();
                CryptoStream cStream = new CryptoStream(mStream, DCSP.CreateDecryptor(mKeys, mIV), CryptoStreamMode.Write);

                cStream.Write(bytes, 0, bytes.Length);
                cStream.FlushFinalBlock();

                bytes = null;
                bytes = mStream.ToArray();
            }

            object obj = null;
            if (bytes == null)
                return obj;
            MemoryStream ms = new MemoryStream(bytes);
            ms.Position = 0;

            try
            {
                obj = bin_for.Deserialize(ms);
                ms.Close();
                return obj;
            }
            catch (System.Exception ex)
            {
                Debug.LogError("Error Deserialize Failed " + ex.Message);
                if (eventOnDeserializeFailed != null)
                {
                    eventOnDeserializeFailed();
                }
            }

            return null;
        }

        public static bool SaveClass(string file_name, object obj, bool need_encrypt)
        {
            SetEnvironment();

            try
            {
                File.WriteAllBytes(file_name, SerializeObject(obj, need_encrypt));
            }
            catch (System.Exception ex)
            {
                Debug.Log("Error: " + ex.Message);
                return false;
            }

            if (File.Exists(file_name))
            {
                return true;
            }
            return false;
        }

        public static bool IsExist(string file_name)
        {
            return File.Exists(file_name);
        }

        public static bool DeleteFile(string file_name)
        {
            if (File.Exists(file_name) == true)
            {
                File.Delete(file_name);

                return File.Exists(file_name) == false;
            }

            return true;
        }

        public static T LoadClass<T>(string file_name, bool need_encrypt)
        {
            SetEnvironment();

            object obj = null;

            if (File.Exists(file_name))
            {
                try
                {
                    byte[] all_byte = File.ReadAllBytes(file_name);
                    obj = DeserializeObject(all_byte, need_encrypt);
                    return (T)obj;
                }
                catch (System.Exception ex)
                {
                    Debug.Log("Error: " + ex.Message);
                    if (eventOnLoadClassFailed != null)
                    {
                        eventOnLoadClassFailed();
                    }
                }
            }

            return default(T);
        }
    }

}
