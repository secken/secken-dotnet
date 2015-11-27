using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;

namespace Secken.ServerAuth.Demo
{
    internal static class BaseSer
    {
        #region BaseSer

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="objectToSerialize"></param>
        /// <returns></returns>
        public static string Serialize(this object objectToSerialize)
        {
            #region 序列化

            if (objectToSerialize != null)
            {
                using (var ms = new MemoryStream())
                {
                    var serializer =
                        new DataContractJsonSerializer(objectToSerialize.GetType());
                    serializer.WriteObject(ms, objectToSerialize);
                    ms.Position = 0;
                    using (var reader = new StreamReader(ms))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;

            #endregion
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public static T Deserialize<T>(this string jsonString)
        {
            #region 反序列化

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                var deserializer = new DataContractJsonSerializer(typeof(T));
                return (T)deserializer.ReadObject(ms);
            }

            #endregion
        }

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="objectToSerialize"></param>
        public static string SerializeXml(this object objectToSerialize)
        {
            #region 序列化

            if (objectToSerialize != null)
            {
                using (var ms = new MemoryStream())
                {
                    var serializer = new XmlSerializer(objectToSerialize.GetType());
                    serializer.Serialize(ms, objectToSerialize);
                    ms.Position = 0;
                    using (var reader = new StreamReader(ms))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            return null;

            #endregion
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static T DeserializeXml<T>(this string xmlString)
        {
            #region 反序列化

            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xmlString)))
            {
                var deserializer = new XmlSerializer(typeof(T));
                return (T)deserializer.Deserialize(ms);
            }

            #endregion
        }

        #endregion
    }
}