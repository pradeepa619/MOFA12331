using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MOFAAPI.Helpers;

namespace MOFAAPI.Helpers
{
    public class GConvert
    {
        public static short ToInt16(object objValue)
        {
            short Value;
            try
            {
                Value = Convert.ToInt16(objValue);
            } 
            catch (Exception ex) 
            {
                Value = 0;
            }
            return Value;
        }

        public static int ToInt32(object objValue)
        {
            int Value;
            try
            {
                Value = Convert.ToInt32(objValue);
            } 
            catch (Exception ex) 
            {
                Value = 0;
            }
            return Value;
        }

        public static int? ToNullableInt32(object objValue)
        {
            int? Value = null;
            try
            {
                if (objValue != null)
                    Value = Convert.ToInt32(objValue);
            } 
            catch (Exception ex) 
            {
                Value = null;
            }
            return Value;
        }

        public static long ToInt64(object objValue)
        {
            long Value;
            try
            {
                Value = Convert.ToInt64(objValue);
            } 
            catch (Exception ex) 
            {
                Value = 0;
            }
            return Value;
        }

        public static long? ToNullableInt64(object objValue)
        {
            long? Value = null;
            try
            {
                if (objValue != null)
                    Value = Convert.ToInt64(objValue);
            } 
            catch (Exception ex) 
            {
                Value = null;
            }
            return Value;
        }

        public static double ToDouble(object objValue)
        {
            double Value;
            try
            {
                Value = Convert.ToDouble(objValue);
            } 
            catch (Exception ex) 
            {
                Value = 0;
            }
            return Value;
        }

        public static double? ToNullableDouble(object objValue)
        {
            double? Value = null;
            try
            {
                if (objValue != null)
                    Value = Convert.ToDouble(objValue);
            } 
            catch (Exception ex) 
            {
                Value = null;
            }
            return Value;
        }

        public static decimal ToDecimal(object objValue)
        {
            decimal Value;
            try
            {
                Value = Convert.ToDecimal(objValue);
            } 
            catch (Exception ex) 
            {
                Value = 0;
            }
            return Value;
        }

        public static decimal? ToNullableDecimal(object objValue)
        {
            decimal? Value = null;
            try
            {
                if (objValue != null)
                    Value = Convert.ToDecimal(objValue);
            }

            catch (Exception ex)

            {
                Value = null;
            }
            return Value;
        }

        public static bool ToBoolean(object objValue)
        {
            bool Value;
            try
            {
                Value = Convert.ToBoolean(objValue);
            }

            catch (Exception ex)

            {
                Value = false;
            }
            return Value;
        }

        public static bool? ToNullableBoolean(object objValue)
        {
            bool? Value = null;
            try
            {
                if (objValue != null)
                    Value = Convert.ToBoolean(objValue);
            }

            catch (Exception ex)

            {
                Value = null;
            }
            return Value;
        }

        public static DateTime ToDateTime(object objValue)
        {
            DateTime Value;
            try
            {
                if (objValue == null)
                {
                    Value = Convert.ToDateTime("1/1/1900");
                }
                else
                {
                    Value = Convert.ToDateTime(objValue);
                }
            }

            catch (Exception ex)

            {
                Value = Convert.ToDateTime("1/1/1900");
            }
            return Value;
        }

        public static DateTime? ToNullableDateTime(object objValue)
        {
            DateTime? Value = null;
            try
            {
                if (objValue != null)
                {
                    Value = Convert.ToDateTime(objValue);
                }
            }

            catch (Exception ex)

            {
                Value = null;
            }
            return Value;
        }

        public static string ToDateTimeString(object objValue)
        {
            string Value;
            try
            {
                if (objValue == null)
                {
                    Value = "";
                }
                else if (Convert.ToDateTime(objValue).Year.Equals(1900))
                {
                    Value = "";
                }
                else
                {
                    Value = Convert.ToDateTime(objValue).ToString("MM/dd/yyyy HH:mm");
                }
            }

            catch (Exception ex)

            {
                Value = "";
            }
            return Value;
        }

        public static string DisplayDateTime(object objValue)
        {
            string Value;
            try
            {
                if (objValue == null)
                {
                    Value = "";
                }
                else if (Convert.ToDateTime(objValue).Year.Equals(1900))
                {
                    Value = "";
                }
                else
                {
                    Value = Convert.ToDateTime(objValue).ToString("dd-MMM-yyyy HH:mm");
                }
            }

            catch (Exception ex)

            {
                Value = "";
            }
            return Value;
        }

        public static string DisplayDateTimeWithSeconds(object objValue)
        {
            string Value;
            try
            {
                if (objValue == null)
                {
                    Value = "";
                }
                else if (Convert.ToDateTime(objValue).Year.Equals(1900))
                {
                    Value = "";
                }
                else
                {
                    Value = Convert.ToDateTime(objValue).ToString("dd-MMM-yyyy HH:mm:ss");
                }
            }

            catch (Exception ex)

            {
                Value = "";
            }
            return Value;
        }

        public static string DisplayDate(object objValue)
        {
            string Value;
            try
            {
                if (objValue == null)
                {
                    Value = "";
                }
                else if (Convert.ToDateTime(objValue).Year.Equals(1900))
                {
                    Value = "";
                }
                else
                {
                    Value = Convert.ToDateTime(objValue).ToString("dd-MMM-yyyy");
                }
            }

            catch (Exception ex)

            {
                Value = "";
            }
            return Value;
        }

        public static string DisplayTime(object objValue)
        {
            string Value;
            try
            {
                if (objValue == null)
                {
                    Value = "";
                }
                else if (Convert.ToDateTime(objValue).Year.Equals(1900))
                {
                    Value = "";
                }
                else
                {
                    Value = Convert.ToDateTime(objValue).ToString("HH:mm");
                }
            }

            catch (Exception ex)

            {
                Value = "";
            }
            return Value;
        }

        //No Need to convert the date from one timezone to other, bec here we are processing all as UTC to epoch(UnixTime) vice versa.
        public static long? ToEpochTime(object objValue)
        {
            long? epochTime = null;

            try
            {
                DateTime dateObject = Convert.ToDateTime(objValue);
                dateObject = DateTime.SpecifyKind(dateObject, DateTimeKind.Utc);

                if (dateObject.Year == 1970 && dateObject.Month == 1 && dateObject.Day == 1)
                    epochTime = 0;
                else
                {
                    DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    int seconds = dateObject.Second;
                    var ticks = dateObject.Ticks - epoch.Ticks;
                    epochTime = (ticks / TimeSpan.TicksPerSecond) - seconds;
                }

            }
            catch (Exception exp) { epochTime = null; }

            return epochTime;
        }

        public static DateTime? ToNullableDateTimeFromEpochTime(object epochTime, int dateTimeSecondsResetFor = (int)Common.DateTimeSecondsResetFor.Common)
        {
            DateTime? datetime = null;

            try
            {
                if (epochTime != null && !string.IsNullOrEmpty(Convert.ToString(epochTime)))
                {
                    datetime = ToDateTimeFromEpochTime(epochTime, dateTimeSecondsResetFor);
                }

            }
            catch (Exception exp) { datetime = null; }

            return datetime;
        }

        public static DateTime ToDateTimeFromEpochTime(object epochTime, int dateTimeSecondsResetFor = (int)Common.DateTimeSecondsResetFor.Common)
        {
            DateTime datetime;

            try
            {
                var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                datetime = epoch.AddSeconds(GConvert.ToDouble(epochTime));

                TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, DateTime.Now.Millisecond);
                if (dateTimeSecondsResetFor == (int)Common.DateTimeSecondsResetFor.FromDate)
                    timeSpan = new TimeSpan(0, 0, 0, 0 - datetime.Second);
                else if (dateTimeSecondsResetFor == (int)Common.DateTimeSecondsResetFor.ToDate)
                    timeSpan = new TimeSpan(0, 0, 0, 59 - datetime.Second);

                datetime = datetime.Add(timeSpan);

            }
            catch (Exception exp) { datetime = Convert.ToDateTime("1/1/1900"); }

            return datetime;
        }

        public static string ToDateTimeStringFromEpochTime(object epochTime, int dateTimeSecondsResetFor = (int)Common.DateTimeSecondsResetFor.Common)
        {

            string result = string.Empty;
            DateTime datetime;

            try
            {
                if (epochTime != null)
                {

                    var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    datetime = epoch.AddSeconds(GConvert.ToDouble(epochTime));

                    TimeSpan timeSpan = new TimeSpan(0, 0, 0, 0, DateTime.Now.Millisecond);
                    if (dateTimeSecondsResetFor == (int)Common.DateTimeSecondsResetFor.FromDate)
                        timeSpan = new TimeSpan(0, 0, 0, 0 - datetime.Second);
                    else if (dateTimeSecondsResetFor == (int)Common.DateTimeSecondsResetFor.ToDate)
                        timeSpan = new TimeSpan(0, 0, 0, 59 - datetime.Second);

                    datetime = datetime.Add(timeSpan);

                    result = datetime.ToString("yyyy/MM/dd HH:mm:ss.FFF");
                }

            }
            catch (Exception exp) { }

            return result;
        }
        public static string ToBase64String(object objValue)
        {
            string result = string.Empty;

            try
            {
                if (objValue != null)
                    result = Convert.ToBase64String((byte[])objValue);
            }
            catch (Exception exp) { }
            return result;
        }

        public static DynamicData ToConvertDynamicDataFromJSON(string JsonString)
        {
            DynamicData dynamicData = new DynamicData();
            try
            {
                dynamicData = JsonConvert.DeserializeObject<DynamicData>(JsonString);
            } 
            catch (Exception ex) 
            {

            }

            return dynamicData;
        }
    }
}