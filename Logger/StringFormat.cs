using System;
using System.Collections.Generic;
using System.Text;

namespace Logger.Tools
{
    public class StringFormat
    {
     // Fields
        private readonly object[] v_args;
        private readonly string v_format;
        private readonly IFormatProvider v_provider;

        // Methods
        public StringFormat(IFormatProvider provider, string format, params object[] args)
        {
            this.v_provider = provider;
            this.v_format = format;
            this.v_args = args;
        }

        private static void RenderArray(Array array, StringBuilder buffer)
        {
            if (array == null)
            {
                buffer.Append((string)null);
            }
            else if (array.Rank != 1)
            {
                buffer.Append(array.ToString());
            }
            else
            {
                buffer.Append("{");
                int length = array.Length;
                if (length > 0)
                {
                    RenderArray(array.GetValue(0) as Array, buffer);
                    for (int i = 1; i < length; i++)
                    {
                        buffer.Append(", ");
                        RenderArray(array.GetValue(i) as Array, buffer);
                    }
                }
                buffer.Append("}");
            }
        }

        private static string Format(IFormatProvider provider, string format, params object[] args)
        {
            try
            {
                if (format == null)
                {
                    return null;
                }
                if (args == null)
                {
                    return format;
                }
                return string.Format(provider, format, args);
            }
            catch (Exception exception)
            {
                //LogLog.Warn("StringFormat: Exception while rendering format [" + format + "]", exception);
                return StringFormatError(exception, format, args);
            }
            catch
            {
                //LogLog.Warn("StringFormat: Exception while rendering format [" + format + "]");
                return StringFormatError(null, format , args);
            }

        }

        private static string StringFormatError(Exception formatException, string format, object[] args)
        {
            try
            {
                StringBuilder buffer = new StringBuilder("<Logger.Error>");
                if (formatException != null)
                {
                    buffer.Append("Exception during StringFormat: ").Append(formatException.Message);
                }
                else
                {
                    buffer.Append("Exception during StringFormat");
                }
                buffer.Append(" <format>").Append(format).Append("</format>");
                buffer.Append("<args>");
                RenderArray(args, buffer);
                buffer.Append("</args>");
                buffer.Append("</Logger.Error>");
                return buffer.ToString();
            }
            catch (Exception exception)
            {
                //LogLog.Error("StringFormat: INTERNAL ERROR during StringFormat error handling", exception);
                return "<Error>Exception during StringFormat. See Internal Log.</Error>";
            }
            catch
            {
                //LogLog.Error("StringFormat: INTERNAL ERROR during StringFormat error handling");
                return "<Error>Exception during StringFormat. See Internal Log.</Error>";
            }
        }

        public override string ToString()
        {
            return Format(this.v_provider, this.v_format, this.v_args);
        }

        public static string FormatOutput(string value)
        {
            value = value.Replace("%t", "\t");
            value = value.Replace("%n", "\n");
            value = value.Replace("%r", "\r");
            value = EscapeAllAmpersands(value);
            return value;
        }

        private static int[] GetIndices(string value, string what)
        {
            List<int> list = new List<int>();
            int first = value.IndexOf(what);
            if (first >= 0)
            {
                list.Add(first);
                for (int other = first; other > 0; other = value.IndexOf(what, other + 1))
                {
                    if (other > 0 && other != first)
                        list.Add(other);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// This is for escaping all ampersands within text since they pose a problem to xml.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="what"></param>
        /// <returns></returns>
        private static string EscapeAllAmpersands(string value)
        {
            int[] ampersandIndices = GetIndices(value, "&");
            StringBuilder sb = new StringBuilder();
            //int j = 0, k = 0, l = 0, m = 0;
            if (ampersandIndices.Length > 0)
            {
                int j = 0, k = 0, l = -1;
                for (int i = 0; i < ampersandIndices.Length; i++)
                {
                    j = i + 1;
                    k = ampersandIndices[i] + 4;
                    sb.Append(value.Substring(l + 1, ampersandIndices[i] - l));
                    if (k < value.Length && j < ampersandIndices.Length)
                    {
                        if (value.Substring(ampersandIndices[i] + 1, k - ampersandIndices[i]).ToLower().CompareTo("amp;") != 0)
                        {
                            sb.Append("amp;");
                            l = ampersandIndices[i];
                        }
                        else
                            l = k;
                    }
                    else
                    {
                        sb.Append("amp;");
                        //l++;
                        if (j < ampersandIndices.Length)
                        {
                            sb.Append(value.Substring(ampersandIndices[i] + 1, ampersandIndices[j] - (ampersandIndices[i] + 1)));
                            l = ampersandIndices[i] + ampersandIndices[j] - (ampersandIndices[i] + 1);
                        }
                        else
                        {
                            sb.Append(value.Substring(ampersandIndices[i] + 1, value.Length - ampersandIndices[i] - 1));
                        }
                    }

                }
                return sb.ToString();
            }
            else return value;
        }
    }
}
