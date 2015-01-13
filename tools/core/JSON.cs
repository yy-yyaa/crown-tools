/*
 * Copyright (c) 2012-2015 Daniele Bartolini and individual contributors.
 * License: https://github.com/taylor001/crown/blob/master/LICENSE
 */

using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;

namespace Crown
{
	public class JSON
	{
		public static string Encode(object obj)
		{
			StringBuilder sb = new StringBuilder();
			JSON.Write(obj, sb);
			return sb.ToString();
		}

		public static Hashtable Decode(byte[] json)
		{
			int i = 0;
			SkipSpaces(json, ref i);
			return ParseObject(json, ref i);
		}

		static void Write(object o, StringBuilder sb)
		{
			if (o == null)
				sb.Append("null");
			else if (o is bool && (bool)o == false)
				sb.Append("false");
			else if (o is bool)
				sb.Append("true");
			else if (o is int)
				sb.Append(o.ToString());
			else if (o is float)
				sb.Append(o.ToString());
			else if (o is double)
				sb.Append(o.ToString());
			else if (o is string)
				WriteString((string)o, sb);
			else if (o is ArrayList)
				WriteArray((ArrayList)o, sb);
			else if (o is Hashtable)
				WriteObject((Hashtable)o, sb);
			else
				throw new ArgumentException("Bad object");
		}

		static void WriteString(string s, StringBuilder sb)
		{
			sb.Append('"');
			for (int i = 0, len = s.Length; i < len; ++i)
			{
				if (s[i] == '"')
				{
					sb.Append('\\');
					sb.Append('"');
				}
				else
					sb.Append(s[i]);
			}
			sb.Append('"');
		}

		static void WriteArray(ArrayList a, StringBuilder sb)
		{
			bool comma = false;
			sb.Append("[");
			foreach (object o in a)
			{
				if (comma)
					sb.Append(", ");

				Write(o, sb);
				comma = true;
			}
			sb.Append("]");
		}

		static void WriteObject(Hashtable h, StringBuilder sb)
		{
			bool comma = false;
			sb.Append("{");
			foreach (DictionaryEntry e in h)
			{
				if (comma)
					sb.Append(", ");

				sb.AppendFormat("\"{0}\" : ", e.Key.ToString());
				Write(e.Value, sb);
				comma = true;
			}
			sb.Append("}");
		}

		static object Parse(byte[] json, ref int i)
		{
			if (json[i] == '{')
				return ParseObject(json, ref i);
			else if (json[i] == '[')
				return ParseArray(json, ref i);
			else if (json[i] == '"')
				return ParseString(json, ref i);
			else if (json[i] == 't')
			{
				i += 4;
				return true;
			}
			else if (json[i] == 'f')
			{
				i += 5;
				return false;
			}
			else if (json[i] == 'n')
			{
				i += 4;
				return null;
			}
			else if (json[i] == '-' || '0' <= json[i] && json[i] <= '9')
				return ParseNumber(json, ref i);

			throw new ArgumentException();
		}

		static void Next(byte[] json, ref int i, byte c = 0)
		{
			if (c != 0 && c != json[i])
			{
				//throw ArgumentException("Expected '{0}' got '{1}'", c, *str);
				throw new ArgumentException("Expected ? got ?");
			}

			++i;
		}

		static void SkipSpaces(byte[] json, ref int i)
		{
			while (json[i] == ' '
				|| json[i] == '\t'
				|| json[i] == '\n'
				|| json[i] == '\v'
				|| json[i] == '\f'
				|| json[i] == '\r')
			{
				Next(json, ref i);
			}
		}

		static string ParseString(byte[] json, ref int i)
		{
			List<byte> bytes = new List<byte>();

			if (json[i] == '"')
			{
				while (json[i] != '\0')
				{
					Next(json, ref i);

					if (json[i] == '"')
					{
						Next(json, ref i);
						return Encoding.UTF8.GetString(bytes.ToArray());
					}
					else if (json[i] == '\\')
					{
						Next(json, ref i);

						byte c = json[i];
						if (c == '"') bytes.Add((byte)'"');
						else if (c == '\\') bytes.Add((byte)'\\');
						else if (c == '/') bytes.Add((byte)'/');
						else if (c == 'b') bytes.Add((byte)'\b');
						else if (c == 'f') bytes.Add((byte)'\f');
						else if (c == 'n') bytes.Add((byte)'\n');
						else if (c == 'r') bytes.Add((byte)'\r');
						else if (c == 't') bytes.Add((byte)'\t');
						else throw new ArgumentException("Bad escape character");
					}
					else
					{
						bytes.Add(json[i]);
					}
				}
			}

			throw new ArgumentException("Bad string");
		}

		static double ParseNumber(byte[] json, ref int i)
		{
			List<byte> number = new List<byte>();

			if (json[i] == '-')
			{
				number.Add((byte)'-');
				Next(json, ref i, (byte)'-');
			}
			while ('0' <= json[i] && json[i] <= '9')
			{
				number.Add(json[i]);
				Next(json, ref i);
			}

			if (json[i] == '.')
			{
				number.Add((byte)'.');
				Next(json, ref i);
				while (i < json.Length && '0' <= json[i] && json[i] <= '9')
				{
					number.Add(json[i]);
					Next(json, ref i);
				}
			}

			if (i < json.Length && (json[i] == 'e' || json[i] == 'E'))
			{
				number.Add(json[i]);
				Next(json, ref i);

				if (json[i] == '-' || json[i] == '+')
				{
					number.Add(json[i]);
					Next(json, ref i);
				}
				while ('0' <= json[i] && json[i] <= '9')
				{
					number.Add(json[i]);
					Next(json, ref i);
				}
			}

			// Ensure null terminated
			number.Add((byte)'\0');
			return Double.Parse(Encoding.UTF8.GetString(number.ToArray()));
		}

		static ArrayList ParseArray(byte[] json, ref int i)
		{
			ArrayList array = new ArrayList();

			if (json[i] == '[')
			{
				Next(json, ref i, (byte)'[');
				SkipSpaces(json, ref i);

				if (json[i] == ']')
				{
					Next(json, ref i, (byte)']');
					return array;
				}

				while (i < json.Length)
				{
					array.Add(Parse(json, ref i));
					SkipSpaces(json, ref i);

					if (json[i] == ']')
					{
						Next(json, ref i, (byte)']');
						return array;
					}

					if (json[i] != ',')
						continue;

					Next(json, ref i, (byte)',');
					SkipSpaces(json, ref i);
				}
			}

			throw new ArgumentException("Bad array");
		}

		static Hashtable ParseObject(byte[] json, ref int i)
		{
			Hashtable hash = new Hashtable();

			if (json[i] == '{')
			{
				Next(json, ref i, (byte)'{');
				SkipSpaces(json, ref i);

				if (json[i] == '}')
				{
					Next(json, ref i, (byte)'}');
					return hash;
				}

				while (i < json.Length)
				{
					string key = ParseString(json, ref i);

					SkipSpaces(json, ref i);
					Next(json, ref i, (byte)':');
					SkipSpaces(json, ref i);
					hash.Add(key, Parse(json, ref i));
					SkipSpaces(json, ref i);

					if (json[i] == '}')
					{
						Next(json, ref i, (byte)'}');
						return hash;
					}

					if (json[i] != ',')
						continue;

					Next(json, ref i, (byte)',');
					SkipSpaces(json, ref i);
				}
			}

			throw new ArgumentException("Bad object");
		}
	}
}
