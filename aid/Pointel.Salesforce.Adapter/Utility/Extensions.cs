/*
 Copyright (c) Pointel Inc., All Rights Reserved.

 This software is the confidential and proprietary information of
 Pointel Inc., ("Confidential Information"). You shall not
 disclose such Confidential Information and shall use it only in
 accordance with the terms of the license agreement you entered into
 with Pointel.

 POINTEL MAKES NO REPRESENTATIONS OR WARRANTIES ABOUT THE
  *SUITABILITY OF THE SOFTWARE, EITHER EXPRESS OR IMPLIED, INCLUDING
  *BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY,
  *FITNESS FOR A PARTICULAR PURPOSE, OR NON-INFRINGEMENT. POINTEL
  *SHALL NOT BE LIABLE FOR ANY DAMAGES SUFFERED BY LICENSEE AS A
  *RESULT OF USING, MODIFYING OR DISTRIBUTING THIS SOFTWARE OR ITS
  *DERIVATIVES.
 */

using Genesyslab.Platform.Commons.Collections;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;

namespace Pointel.Salesforce.Adapter.Utility
{
    public static class Extension
    {
        public static object GetValue(this IDictionary dictionary, object key, object defaultValue)
        {
            object obj = dictionary[key];
            if (obj == null)
            {
                return defaultValue;
            }
            return obj;
        }

        public static void AddRange<TListElement>(this IList<TListElement> list, IEnumerable<TListElement> collection)
        {
            foreach (TListElement current in collection)
            {
                list.Add(current);
            }
        }

        public static int GetValueAsInt(this KeyValueCollection KVP, string key, int defaultValue)
        {
            string text = KVP[key] as string;
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }
            int result;
            try
            {
                int num = int.Parse(text);
                result = num;
            }
            catch (FormatException)
            {
                result = defaultValue;
            }
            catch (OverflowException)
            {
                result = defaultValue;
            }
            return result;
        }

        public static int GetValueAsUInt(this KeyValueCollection KVP, string key, int defaultValue)
        {
            string text = KVP[key] as string;
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }
            int result;
            try
            {
                int num = int.Parse(text);
                if (num < 0)
                {
                    result = defaultValue;
                }
                else
                {
                    result = num;
                }
            }
            catch (FormatException)
            {
                result = defaultValue;
            }
            catch (OverflowException)
            {
                result = defaultValue;
            }
            return result;
        }

        public static int GetValueAsUInt(this KeyValueCollection KVP, string key, int min, int defaultValue)
        {
            string text = KVP[key] as string;
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }
            int result;
            try
            {
                int num = int.Parse(text);
                if (num < 0)
                {
                    result = defaultValue;
                }
                else if (num < min)
                {
                    result = defaultValue;
                }
                else
                {
                    result = num;
                }
            }
            catch (FormatException)
            {
                result = defaultValue;
            }
            catch (OverflowException)
            {
                result = defaultValue;
            }
            return result;
        }

        public static double GetValueAsDouble(this KeyValueCollection KVP, string key, double defaultValue)
        {
            string text = KVP[key] as string;
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }
            double result;
            try
            {
                text = text.Replace(",", ".");
                double num = double.Parse(text, CultureInfo.InvariantCulture);
                result = num;
            }
            catch (FormatException)
            {
                result = defaultValue;
            }
            catch (OverflowException)
            {
                result = defaultValue;
            }
            return result;
        }

        public static bool GetValueAsBoolean(this KeyValueCollection KVP, string key, bool defaultValue)
        {
            string text = KVP[key] as string;
            if (string.IsNullOrEmpty(text))
            {
                return defaultValue;
            }
            text = text.ToLowerInvariant();
            return text == "true" || text == "yes" || (!(text == "false") && !(text == "no") && defaultValue);
        }

        public static bool? GetValueAsNullableBoolean(this KeyValueCollection KVP, string key)
        {
            string text = KVP[key] as string;
            if (string.IsNullOrEmpty(text))
            {
                return null;
            }
            text = text.ToLowerInvariant();
            if (text == "true" || text == "yes")
            {
                return new bool?(true);
            }
            if (text == "false" || text == "no")
            {
                return new bool?(false);
            }
            return null;
        }

        public static string GetValueAsString(this KeyValueCollection KVP, string key, string defaultValue)
        {
            string text = KVP[key] as string;
            if (text == null)
            {
                return defaultValue;
            }
            return text;
        }

        public static SolidColorBrush GetValueAsSolidColorBrush(this KeyValueCollection KVP, string key, string defaultValue)
        {
            string text = KVP[key] as string;
            if (text == null)
            {
                text = defaultValue;
            }
            SolidColorBrush result;
            try
            {
                Color color = (Color)ColorConverter.ConvertFromString(text);
                result = new SolidColorBrush(color);
            }
            catch (Exception)
            {
                Color color = (Color)ColorConverter.ConvertFromString(defaultValue);
                result = new SolidColorBrush(color);
            }
            return result;
        }

        public static string[] GetValueAsStringArray(this KeyValueCollection KVP, string key, string[] defaultValue)
        {
            string text = KVP[key] as string;
            if (text == null)
            {
                return defaultValue;
            }
            string[] array = text.Split(new char[]
			{
				',',
				';',
				'|',
				'/',
				'\\'
			}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = array[i].Trim();
            }
            return array;
        }

        public static bool CheckKeyAndValue(this KeyValueCollection KVP, string key)
        {
            string text = KVP[key] as string;
            return !string.IsNullOrWhiteSpace(text);
        }
    }
}