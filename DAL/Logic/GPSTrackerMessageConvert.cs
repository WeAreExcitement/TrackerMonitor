﻿using DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Logic
{
    public class GPSTrackerMessageConverter
    {
        /// <summary>
        /// Метод для парсинга сообщения от маячка в объект сообщения
        /// </summary>
        /// <param name="bytes">сообщение от маячка</param>
        /// <returns>объект сообщения</returns>
        public static GPSTrackerMessage BytesToMessage(byte[] bytes)
        {
            GPSTrackerMessage message = new GPSTrackerMessage();

            // Парсинг идентификатора маячка
            message.GPSTrackerId = Encoding.UTF8.GetString(bytes.Take(20).ToArray<byte>());

            // Парсинг долготы
            byte[] longitudeBytes = new byte[20];
            Array.Copy(bytes, 20, longitudeBytes, 0, 20);
            message.Longitude = Double.Parse(Encoding.UTF8.GetString(longitudeBytes));

            // Парсинг широты
            byte[] latitudeBytes = new byte[19];
            Array.Copy(bytes, 40, latitudeBytes, 0, 19);
            message.Latitude = Double.Parse(Encoding.UTF8.GetString(latitudeBytes));

            int year = (int)BitConverter.ToInt16(new byte[] { bytes[59], bytes[60] }, 0);
            int month =  Convert.ToInt32(bytes[61]);
            int day =  Convert.ToInt32(bytes[62]);
            int hour = Convert.ToInt32(bytes[63]);
            int minute = Convert.ToInt32(bytes[64]);
            int second = Convert.ToInt32(bytes[65]);

            message.Time = new DateTime(year, month, day,hour, minute, second);

            return message;
        }

        public static byte[] MessageToBytes(GPSTrackerMessage message)
        {
            byte[] bytes = new byte[70];

            byte[] identifier = Encoding.UTF8.GetBytes(message.GPSTrackerId);
            byte[] longitude = Encoding.UTF8.GetBytes(message.Longitude.ToString("+000.000000000000000;-000.000000000000000"));
            byte[] latitude = Encoding.UTF8.GetBytes(message.Latitude.ToString("+00.000000000000000;-00.000000000000000"));
            byte[] year = BitConverter.GetBytes((ushort)message.Time.Year);

            Array.Copy(identifier, bytes, identifier.Length); // Первые 20 байт идентификатор трекера
            Array.Copy(longitude, 0, bytes, 20, longitude.Length);          // 20 байт долгота
            Array.Copy(latitude, 0, bytes, 40, latitude.Length);           // 19 байт широта
            Array.Copy(year, 0, bytes, 59, 2);                // 2 байта год
            bytes[61] = Convert.ToByte(message.Time.Month);   // Месяц
            bytes[62] = Convert.ToByte(message.Time.Day);    // Дата
            bytes[63] = Convert.ToByte(message.Time.Hour);    // Часы
            bytes[64] = Convert.ToByte(message.Time.Minute);  // Минуты
            bytes[65] = Convert.ToByte(message.Time.Second);  // Секунды

            return bytes;
        }
    }
}