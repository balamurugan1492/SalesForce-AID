using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Xml;

namespace Pointel.Windows.Views.Common.Editor.HTMLConverter
{
	public static class HtmlToXamlConverter
	{
		private class FontSizeInfo
		{
			public string Text
			{
				get;
				set;
			}
			public bool IsUnit
			{
				get;
				set;
			}
			public double Factor
			{
				get;
				set;
			}
			public double Value
			{
				get;
				set;
			}
			public FontSizeInfo()
			{
				this.Value = 1.0;
			}
		}
		public const string Xaml_FlowDocument = "FlowDocument";
		public const string Xaml_Tag = "Tag";
		public const string Xaml_Run = "Run";
		public const string Xaml_Span = "Span";
		public const string Xaml_Image = "Image";
		public const string Xaml_Image_Source = "Source";
		public const string Xaml_InlineUIContainer = "InlineUIContainer";
		public const string Xaml_BlockUIContainer = "BlockUIContainer";
		public const string Xaml_Hyperlink = "Hyperlink";
		public const string Xaml_Hyperlink_NavigateUri = "NavigateUri";
		public const string Xaml_Hyperlink_TargetName = "TargetName";
		public const string Xaml_Section = "Section";
		public const string Xaml_List = "List";
		public const string Xaml_List_MarkerStyle = "MarkerStyle";
		public const string Xaml_List_MarkerStyle_None = "None";
		public const string Xaml_List_MarkerStyle_Decimal = "Decimal";
		public const string Xaml_List_MarkerStyle_Disc = "Disc";
		public const string Xaml_List_MarkerStyle_Circle = "Circle";
		public const string Xaml_List_MarkerStyle_Square = "Square";
		public const string Xaml_List_MarkerStyle_Box = "Box";
		public const string Xaml_List_MarkerStyle_LowerLatin = "LowerLatin";
		public const string Xaml_List_MarkerStyle_UpperLatin = "UpperLatin";
		public const string Xaml_List_MarkerStyle_LowerRoman = "LowerRoman";
		public const string Xaml_List_MarkerStyle_UpperRoman = "UpperRoman";
		public const string Xaml_ListItem = "ListItem";
		public const string Xaml_LineBreak = "LineBreak";
		public const string Xaml_Paragraph = "Paragraph";
		public const string Xaml_Margin = "Margin";
		public const string Xaml_Padding = "Padding";
		public const string Xaml_BorderBrush = "BorderBrush";
		public const string Xaml_BorderThickness = "BorderThickness";
		public const string Xaml_Table = "Table";
		public const string Xaml_TableColumns = "Table.Columns";
		public const string Xaml_TableColumn = "TableColumn";
		public const string Xaml_TableRowGroup = "TableRowGroup";
		public const string Xaml_TableRow = "TableRow";
		public const string Xaml_TableCell = "TableCell";
		public const string Xaml_TableCell_BorderThickness = "BorderThickness";
		public const string Xaml_TableCell_BorderBrush = "BorderBrush";
		public const string Xaml_TableCell_ColumnSpan = "ColumnSpan";
		public const string Xaml_TableCell_RowSpan = "RowSpan";
		public const string Xaml_Width = "Width";
		public const string Xaml_Height = "Height";
		public const string Xaml_Brushes_Black = "Black";
		public const string Xaml_Brushes_DarkGray = "DarkGray";
		public const string Xaml_FontFamily = "FontFamily";
		public const string Xaml_FontSize = "FontSize";
		public const string Xaml_FontSize_XXLarge = "22pt";
		public const string Xaml_FontSize_XLarge = "20pt";
		public const string Xaml_FontSize_Large = "18pt";
		public const string Xaml_FontSize_Medium = "16pt";
		public const string Xaml_FontSize_Small = "12pt";
		public const string Xaml_FontSize_XSmall = "10pt";
		public const string Xaml_FontSize_XXSmall = "8pt";
		public const string Xaml_FontWeight = "FontWeight";
		public const string Xaml_FontWeight_Bold = "Bold";
		public const string Xaml_FontStyle = "FontStyle";
		public const string Xaml_Foreground = "Foreground";
		public const string Xaml_Background = "Background";
		public const string Xaml_TextDecorations = "TextDecorations";
		public const string Xaml_TextDecorations_Underline = "Underline";
		public const string Xaml_TextDecorations_Baseline = "Baseline";
		public const string Xaml_TextDecorations_OverLine = "OverLine";
		public const string Xaml_TextDecorations_Strikethrough = "Strikethrough";
		public const string Xaml_TextIndent = "TextIndent";
		public const string Xaml_TextAlignment = "TextAlignment";
		public const string Xaml_HorizontalAlignment = "HorizontalAlignment";
		public const string Xaml_HorizontalAlignment_Left = "Left";
		public const string Xaml_HorizontalAlignment_Right = "Right";
		public const string Xaml_HorizontalAlignment_Center = "Center";
		private static XmlElement InlineFragmentParentElement;
		private static Dictionary<string, HashSet<string>> propertiesByType = new Dictionary<string, HashSet<string>>();
		private static object propertiesByTypeSyncRoot = new object();
		private static double fontSizePPEM = 12.0;
		private static double htmlToXamlFontScale = 1.0;
		private static readonly HtmlToXamlConverter.FontSizeInfo[] fontSizes = new HtmlToXamlConverter.FontSizeInfo[]
		{
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "xx-small",
				IsUnit = false,
				Value = 9.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "x-small",
				IsUnit = false,
				Value = 10.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "small",
				IsUnit = false,
				Value = 11.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "medium",
				IsUnit = false,
				Value = 12.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "large",
				IsUnit = false,
				Value = 14.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "x-large",
				IsUnit = false,
				Value = 18.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "xx-large",
				IsUnit = false,
				Value = 24.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "larger",
				IsUnit = false,
				Value = 14.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "smaller",
				IsUnit = false,
				Value = 11.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "px",
				IsUnit = true,
				Factor = 1.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "mm",
				IsUnit = true,
				Factor = 3.7795275590551181
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "cm",
				IsUnit = true,
				Factor = 37.795275590551178
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "in",
				IsUnit = true,
				Factor = 96.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "pt",
				IsUnit = true,
				Factor = 1.3333333333333333
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "pc",
				IsUnit = true,
				Factor = HtmlToXamlConverter.fontSizePPEM
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "em",
				IsUnit = true,
				Factor = HtmlToXamlConverter.fontSizePPEM
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "ex",
				IsUnit = true,
				Factor = HtmlToXamlConverter.fontSizePPEM / 2.0
			},
			new HtmlToXamlConverter.FontSizeInfo
			{
				Text = "%",
				IsUnit = true,
				Factor = HtmlToXamlConverter.fontSizePPEM / 100.0
			}
		};
		private static BrushConverter brushConverter = new BrushConverter();
		private static string _xamlNamespace = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
		public static string ConvertHtmlToXaml(string htmlString, bool asFlowDocument)
		{
			htmlString = HtmlToXamlConverter.OptimizeHTML(htmlString, 100);
			XmlElement xmlElement = HtmlParser.ParseHtml(htmlString);
			string localName = asFlowDocument ? "FlowDocument" : "Section";
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement2 = xmlDocument.CreateElement(null, localName, HtmlToXamlConverter._xamlNamespace);
			CssStylesheet stylesheet = new CssStylesheet(xmlElement);
			List<XmlElement> sourceContext = new List<XmlElement>(10);
			HtmlToXamlConverter.InlineFragmentParentElement = null;
			Hashtable inheritedProperties = new Hashtable();
			HtmlToXamlConverter.AddBlock(xmlElement2, xmlElement, inheritedProperties, stylesheet, sourceContext);
			if (!asFlowDocument)
			{
				xmlElement2 = HtmlToXamlConverter.ExtractInlineFragment(xmlElement2);
			}
			xmlElement2.SetAttribute("xml:space", "preserve");
			return xmlElement2.OuterXml;
		}
		public static string OptimizeHTML(string htmlString, int maxSpanIncluding)
		{
			if (string.IsNullOrEmpty(htmlString))
			{
				return htmlString;
			}
			string text = "<SPAN>";
			string text2 = "</SPAN>";
			List<string> list = new List<string>();
			List<int> list2 = new List<int>();
			List<int> list3 = new List<int>();
			List<int> list4 = new List<int>();
			List<bool> list5 = new List<bool>();
			List<bool> list6 = new List<bool>();
			List<int> list7 = new List<int>();
			List<int> list8 = new List<int>();
			int i = 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			while (i < htmlString.Length)
			{
				if (htmlString[i] == '<')
				{
					if (i + 3 < htmlString.Length && htmlString[i + 1] == '!' && htmlString[i + 2] == '-' && htmlString[i + 3] == '-')
					{
						i += 3;
						if (i + 1 < htmlString.Length)
						{
							i++;
							while (i + 2 < htmlString.Length && htmlString[i] != '-' && htmlString[i + 1] != '-' && htmlString[i + 2] != '>')
							{
								i++;
							}
							i += 2;
							continue;
						}
						break;
					}
					else
					{
						if (i + 2 < htmlString.Length && htmlString[i + 1] == '/')
						{
							int num7 = i;
							i += 2;
							while (i + 1 < htmlString.Length && char.IsWhiteSpace(htmlString[i]))
							{
								i++;
							}
							if (i + 4 < htmlString.Length && HtmlToXamlConverter.isStringNext(htmlString, i, "SPAN"))
							{
								i += 4;
								while (i + 1 < htmlString.Length && char.IsWhiteSpace(htmlString[i]))
								{
									i++;
								}
								if (htmlString[i] != '>')
								{
									return htmlString;
								}
								num--;
								if (num < 0)
								{
									return htmlString;
								}
								if (list4[list.Count - 1] == -1)
								{
									list4[list.Count - 1] = num7 - 1;
									list6[list.Count - 1] = true;
								}
								else
								{
									for (int j = list2.Count - 1; j >= 0; j--)
									{
										if (list2[j] == num)
										{
											list.Add(string.Empty);
											list2.Add(num);
											int num8 = list4[list4.Count - 1] + 1;
											while (htmlString[num8] != '>')
											{
												num8++;
											}
											list3.Add(num8 + 1);
											list4.Add(num7 - 1);
											if (list6[j] && num4 == num5)
											{
												list8.Add(j);
												list7.Add(list.Count - 1);
												list5.Add(true);
											}
											else
											{
												list6[j] = false;
												list5.Add(false);
											}
											list6.Add(true);
											break;
										}
									}
								}
								num4 = 0;
								num5 = 0;
							}
							else
							{
								while (i + 1 < htmlString.Length && htmlString[i] != '>')
								{
									if (htmlString[i] == '<')
									{
										return htmlString;
									}
									i++;
								}
								if (htmlString[i] != '>')
								{
									return htmlString;
								}
								num5++;
							}
						}
						else
						{
							if (i + 5 < htmlString.Length && HtmlToXamlConverter.isStringNext(htmlString, i + 1, "SPAN"))
							{
								bool flag = false;
								bool flag2 = false;
								int num7 = i;
								i += 5;
								while (i + 1 < htmlString.Length && char.IsWhiteSpace(htmlString[i]))
								{
									i++;
								}
								if (htmlString[i] == '>')
								{
									flag = true;
									flag2 = true;
								}
								else
								{
									if (i + 5 < htmlString.Length && HtmlToXamlConverter.isStringNext(htmlString, i, "STYLE"))
									{
										i += 5;
										for (int k = 0; k < 2; k++)
										{
											while (i + 1 < htmlString.Length && htmlString[i] != '"')
											{
												if (htmlString[i] == '<' || htmlString[i] == '>')
												{
													return htmlString;
												}
												i++;
											}
											if (i + 1 >= htmlString.Length)
											{
												return htmlString;
											}
											i++;
										}
										while (i + 1 < htmlString.Length && char.IsWhiteSpace(htmlString[i]))
										{
											i++;
										}
										if (htmlString[i] == '>')
										{
											flag2 = true;
										}
									}
								}
								while (i + 1 < htmlString.Length && htmlString[i] != '>')
								{
									if (htmlString[i] == '<')
									{
										return htmlString;
									}
									i++;
								}
								if (htmlString[i] != '>')
								{
									return htmlString;
								}
								if (i + 1 == htmlString.Length)
								{
									return htmlString;
								}
								if (list.Count == 0)
								{
									num6 = num7 - 1;
								}
								if (list.Count > 0 && list4[list.Count - 1] == -1)
								{
									list4[list.Count - 1] = num7 - 1;
									if (num4 != num5 || !flag2)
									{
										list6[list.Count - 1] = false;
									}
									else
									{
										list8.Add(-1);
										list7.Add(list.Count);
									}
								}
								else
								{
									int l = list2.Count - 1;
									while (l >= 0)
									{
										if (num == 0)
										{
											list.Add(string.Empty);
											list2.Add(-1);
											int num9 = list4[list4.Count - 1] + 1;
											while (htmlString[num9] != '>')
											{
												num9++;
											}
											list3.Add(num9 + 1);
											list4.Add(num7 - 1);
											list5.Add(false);
											list6.Add(false);
											break;
										}
										if (list2[l] == num - 1)
										{
											list.Add(string.Empty);
											list2.Add(num - 1);
											int num10 = list4[list4.Count - 1] + 1;
											while (htmlString[num10] != '>')
											{
												num10++;
											}
											list3.Add(num10 + 1);
											list4.Add(num7 - 1);
											if (list6[l] && num4 == num5)
											{
												list8.Add(l);
												list7.Add(list.Count - 1);
												list5.Add(true);
												list6.Add(true);
												break;
											}
											list6[l] = false;
											list5.Add(false);
											list6.Add(false);
											break;
										}
										else
										{
											l--;
										}
									}
								}
								if (flag)
								{
									list.Add(text);
								}
								else
								{
									string item = htmlString.Substring(num7, i - num7 + 1);
									list.Add(item);
								}
								list2.Add(num);
								list3.Add(i + 1);
								list4.Add(-1);
								list5.Add(true);
								list6.Add(flag2);
								num++;
								if (num2 < num)
								{
									num2 = num;
								}
								num4 = 0;
								num5 = 0;
							}
							else
							{
								if (i + 1 >= htmlString.Length || char.IsWhiteSpace(htmlString[i]))
								{
									return htmlString;
								}
								i++;
								bool flag3 = false;
								if (HtmlToXamlConverter.isStringNext(htmlString, i, "AREA") || HtmlToXamlConverter.isStringNext(htmlString, i, "BASE") || HtmlToXamlConverter.isStringNext(htmlString, i, "BASEFONT") || HtmlToXamlConverter.isStringNext(htmlString, i, "BR") || HtmlToXamlConverter.isStringNext(htmlString, i, "COL") || HtmlToXamlConverter.isStringNext(htmlString, i, "FRAME") || HtmlToXamlConverter.isStringNext(htmlString, i, "HR") || HtmlToXamlConverter.isStringNext(htmlString, i, "IMG") || HtmlToXamlConverter.isStringNext(htmlString, i, "INPUT") || HtmlToXamlConverter.isStringNext(htmlString, i, "ISINDEX") || HtmlToXamlConverter.isStringNext(htmlString, i, "LINK") || HtmlToXamlConverter.isStringNext(htmlString, i, "META") || HtmlToXamlConverter.isStringNext(htmlString, i, "PARAM"))
								{
									flag3 = true;
								}
								while (i + 1 < htmlString.Length && htmlString[i] != '>')
								{
									if (htmlString[i] == '<')
									{
										return htmlString;
									}
									i++;
								}
								if (htmlString[i] != '>')
								{
									return htmlString;
								}
								if (!flag3)
								{
									num4++;
								}
							}
						}
					}
				}
				i++;
			}
			if (num > 0)
			{
				return htmlString;
			}
			if (num2 - 1 <= maxSpanIncluding || num2 == 0)
			{
				return htmlString;
			}
			for (int m = 0; m < list7.Count; m++)
			{
				if (list8[m] == -1)
				{
					if (list6[list7[m] - 1] && list[list7[m] - 1] != text)
					{
						list[list7[m]] = HtmlToXamlConverter.JoinStyles(list[list7[m] - 1], list[list7[m]]);
					}
				}
				else
				{
					list[list7[m]] = list[list8[m]];
				}
			}
			for (int n = 0; n < list.Count; n++)
			{
				if (n > 0 && list2[n] > list2[n - 1] && !list6[n - 1] && (list[n] == text || list[n].ToLower() == list[n - 1].ToLower()))
				{
					list5[n] = false;
					list6[n] = false;
				}
				if (list5[n] && list6[n])
				{
					if (list4[n] >= list3[n])
					{
						num3 += list[n].Length + text2.Length;
						num3 += list4[n] - list3[n] + 1;
					}
				}
				else
				{
					if (list5[n] && !list6[n])
					{
						num3 += list[n].Length;
						if (list4[n] >= list3[n])
						{
							num3 += list4[n] - list3[n] + 1;
						}
					}
					else
					{
						if (!list5[n] && list6[n])
						{
							num3 += text2.Length;
							if (list4[n] >= list3[n])
							{
								num3 += list4[n] - list3[n] + 1;
							}
						}
						else
						{
							if (list4[n] >= list3[n])
							{
								num3 += list4[n] - list3[n] + 1;
							}
						}
					}
				}
			}
			if (num6 >= 0)
			{
				num3 += num6 + 1;
			}
			int num11 = list4[list.Count - 1] + 1;
			while (htmlString[num11] != '>')
			{
				num11++;
			}
			num11++;
			if (num11 < htmlString.Length)
			{
				num3 += htmlString.Length - num11;
			}
			char[] array = new char[num3];
			i = 0;
			for (int num12 = 0; num12 <= num6; num12++)
			{
				array[num12] = htmlString[num12];
			}
			i += num6 + 1;
			for (int num13 = 0; num13 < list.Count; num13++)
			{
				if (list4[num13] >= list3[num13] || !list5[num13] || !list6[num13])
				{
					if (list5[num13])
					{
						for (int num14 = 0; num14 < list[num13].Length; num14++)
						{
							array[i] = list[num13][num14];
							i++;
						}
					}
					for (int num15 = list3[num13]; num15 <= list4[num13]; num15++)
					{
						array[i] = htmlString[num15];
						i++;
					}
					if (list6[num13])
					{
						for (int num16 = 0; num16 < text2.Length; num16++)
						{
							array[i] = text2[num16];
							i++;
						}
					}
				}
			}
			for (int num17 = num11; num17 < htmlString.Length; num17++)
			{
				array[i] = htmlString[num17];
				i++;
			}
			return new string(array);
		}
		private static bool isStringNext(string htmlString, int currIndex, string str)
		{
			if (currIndex + str.Length <= htmlString.Length)
			{
				for (int i = 0; i < str.Length; i++)
				{
					if (char.ToUpper(htmlString[currIndex]) != str[i])
					{
						return false;
					}
					currIndex++;
				}
				return true;
			}
			return false;
		}
		private static string JoinStyles(string parentTag, string childTag)
		{
			int num = parentTag.IndexOf('"');
			int num2 = parentTag.LastIndexOf('"');
			int num3 = childTag.IndexOf('"');
			int num4 = childTag.LastIndexOf('"');
			string text = string.Empty;
			string text2 = string.Empty;
			if (num != -1 && num2 != -1 && num2 - num > 1)
			{
				text = parentTag.Substring(num + 1, num2 - num - 1).ToLower();
			}
			if (num3 != -1 && num4 != -1 && num4 - num3 > 1)
			{
				text2 = childTag.Substring(num3 + 1, num4 - num3 - 1).ToLower();
			}
			if (text == string.Empty)
			{
				return childTag;
			}
			if (text2 == string.Empty)
			{
				return parentTag;
			}
			if (text == text2)
			{
				return childTag;
			}
			string[] array = text.Split(new char[]
			{
				';'
			});
			string[] array2 = text2.Split(new char[]
			{
				';'
			});
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			Dictionary<string, string> dictionary2 = new Dictionary<string, string>();
			StringBuilder stringBuilder = new StringBuilder(parentTag.Length + childTag.Length);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != string.Empty)
				{
					string[] array3 = array[i].Split(new char[]
					{
						':'
					});
					if (array3.Length == 2)
					{
						dictionary.Add(array3[0].Trim(), array3[1].Trim());
					}
				}
			}
			for (int j = 0; j < array2.Length; j++)
			{
				if (array2[j] != string.Empty)
				{
					string[] array4 = array2[j].Split(new char[]
					{
						':'
					});
					if (array4.Length == 2)
					{
						dictionary2.Add(array4[0].Trim(), array4[1].Trim());
					}
				}
			}
			stringBuilder.Append("<SPAN style=\"");
			foreach (string current in dictionary.Keys)
			{
				if (!dictionary2.ContainsKey(current))
				{
					stringBuilder.Append(string.Format("{0}: {1}; ", current, dictionary[current]));
				}
			}
			foreach (string current2 in dictionary2.Keys)
			{
				stringBuilder.Append(string.Format("{0}: {1}; ", current2, dictionary2[current2]));
			}
			if (stringBuilder.Length > 2)
			{
				stringBuilder.Remove(stringBuilder.Length - 2, 2);
			}
			stringBuilder.Append("\">");
			return stringBuilder.ToString();
		}
		public static string GetAttribute(XmlElement element, string attributeName)
		{
			attributeName = attributeName.ToLower();
			for (int i = 0; i < element.Attributes.Count; i++)
			{
				if (element.Attributes[i].Name.ToLower() == attributeName)
				{
					return element.Attributes[i].Value;
				}
			}
			return null;
		}
		internal static string UnQuote(string value)
		{
			if ((value.StartsWith("\"") && value.EndsWith("\"")) || (value.StartsWith("'") && value.EndsWith("'")))
			{
				value = value.Substring(1, value.Length - 2).Trim();
			}
			return value;
		}
		private static XmlNode AddBlock(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			if (htmlNode is XmlComment)
			{
				HtmlToXamlConverter.DefineInlineFragmentParent((XmlComment)htmlNode, null);
			}
			else
			{
				if (htmlNode is XmlText)
				{
					htmlNode = HtmlToXamlConverter.AddImplicitParagraph(xamlParentElement, htmlNode, inheritedProperties, stylesheet, sourceContext);
				}
				else
				{
					if (htmlNode is XmlElement)
					{
						XmlElement xmlElement = (XmlElement)htmlNode;
						string text = xmlElement.LocalName;
						string namespaceURI = xmlElement.NamespaceURI;
						if (namespaceURI != "http://www.w3.org/1999/xhtml")
						{
							return xmlElement;
						}
						sourceContext.Add(xmlElement);
						text = text.ToLower();
						string key;
						switch (key = text)
						{
							case "html":
							case "body":
							case "div":
							case "form":
							case "pre":
							case "blockquote":
							case "caption":
							case "center":
							case "cite":
								HtmlToXamlConverter.AddSection(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
								goto IL_38B;
							case "p":
							case "h1":
							case "h2":
							case "h3":
							case "h4":
							case "h5":
							case "h6":
							case "nsrtitle":
							case "textarea":
							case "tt":
								HtmlToXamlConverter.AddParagraph(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
								goto IL_38B;
							case "ol":
							case "ul":
							case "dir":
							case "menu":
							case "dl":
								HtmlToXamlConverter.AddList(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
								goto IL_38B;
							case "li":
								htmlNode = HtmlToXamlConverter.AddOrphanListItems(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
								goto IL_38B;
							case "img":
								HtmlToXamlConverter.AddImage(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext, true);
								goto IL_38B;
							case "table":
								HtmlToXamlConverter.AddTable(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
								goto IL_38B;
							case "style":
							case "meta":
							case "head":
							case "title":
							case "script":
								goto IL_38B;
						}
						htmlNode = HtmlToXamlConverter.AddImplicitParagraph(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
					IL_38B:
						sourceContext.RemoveAt(sourceContext.Count - 1);
					}
				}
			}
			return htmlNode;
		}
		private static void AddBreak(XmlElement xamlParentElement, string htmlElementName)
		{
			XmlElement newChild = xamlParentElement.OwnerDocument.CreateElement(null, "LineBreak", HtmlToXamlConverter._xamlNamespace);
			xamlParentElement.AppendChild(newChild);
			if (htmlElementName == "hr")
			{
				XmlText newChild2 = xamlParentElement.OwnerDocument.CreateTextNode("----------------------");
				xamlParentElement.AppendChild(newChild2);
				newChild = xamlParentElement.OwnerDocument.CreateElement(null, "LineBreak", HtmlToXamlConverter._xamlNamespace);
				xamlParentElement.AppendChild(newChild);
			}
		}
		private static void AddSection(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			bool flag = false;
			for (XmlNode xmlNode = htmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode is XmlElement)
				{
					string xmlElementName = ((XmlElement)xmlNode).LocalName.ToLower();
					if (HtmlSchema.IsBlockElement(xmlElementName))
					{
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
				HtmlToXamlConverter.AddParagraph(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
				return;
			}
			Hashtable localProperties;
			Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
			XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, "Section", HtmlToXamlConverter._xamlNamespace);
			HtmlToXamlConverter.ApplyLocalProperties(xmlElement, localProperties, true);
			if (!xmlElement.HasAttributes)
			{
				xmlElement = xamlParentElement;
			}
			for (XmlNode xmlNode2 = htmlElement.FirstChild; xmlNode2 != null; xmlNode2 = ((xmlNode2 != null) ? xmlNode2.NextSibling : null))
			{
				xmlNode2 = HtmlToXamlConverter.AddBlock(xmlElement, xmlNode2, elementProperties, stylesheet, sourceContext);
			}
			if (xmlElement != xamlParentElement)
			{
				HtmlToXamlConverter.AddXamlElementToParent(xamlParentElement, xmlElement);
			}
		}
		private static void AddParagraph(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			Hashtable localProperties;
			Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
			XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, "Paragraph", HtmlToXamlConverter._xamlNamespace);
			HtmlToXamlConverter.ApplyLocalProperties(xmlElement, localProperties, true);
			for (XmlNode xmlNode = htmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				HtmlToXamlConverter.AddInline(xmlElement, xmlNode, elementProperties, stylesheet, sourceContext);
			}
			HtmlToXamlConverter.AddXamlElementToParent(xamlParentElement, xmlElement);
		}
		private static XmlNode AddImplicitParagraph(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, "Paragraph", HtmlToXamlConverter._xamlNamespace);
			XmlNode result = null;
			while (htmlNode != null)
			{
				if (htmlNode is XmlComment)
				{
					HtmlToXamlConverter.DefineInlineFragmentParent((XmlComment)htmlNode, null);
				}
				else
				{
					if (htmlNode is XmlText)
					{
						if (htmlNode.Value.Trim().Length > 0)
						{
							HtmlToXamlConverter.AddTextRun(xmlElement, htmlNode.Value);
						}
					}
					else
					{
						if (htmlNode is XmlElement)
						{
							string xmlElementName = ((XmlElement)htmlNode).LocalName.ToLower();
							if (HtmlSchema.IsBlockElement(xmlElementName))
							{
								break;
							}
							HtmlToXamlConverter.AddInline(xmlElement, (XmlElement)htmlNode, inheritedProperties, stylesheet, sourceContext);
						}
					}
				}
				result = htmlNode;
				htmlNode = htmlNode.NextSibling;
			}
			if (xmlElement.FirstChild != null)
			{
				xamlParentElement.AppendChild(xmlElement);
			}
			return result;
		}
		private static void AddInline(XmlElement xamlParentElement, XmlNode htmlNode, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			if (htmlNode is XmlComment)
			{
				HtmlToXamlConverter.DefineInlineFragmentParent((XmlComment)htmlNode, xamlParentElement);
				return;
			}
			if (htmlNode is XmlText)
			{
				HtmlToXamlConverter.AddTextRun(xamlParentElement, htmlNode.Value);
				return;
			}
			if (htmlNode is XmlElement)
			{
				XmlElement xmlElement = (XmlElement)htmlNode;
				if (xmlElement.NamespaceURI != "http://www.w3.org/1999/xhtml")
				{
					return;
				}
				string text = xmlElement.LocalName.ToLower();
				sourceContext.Add(xmlElement);
				string key;
				switch (key = text)
				{
					case "a":
						HtmlToXamlConverter.AddHyperlink(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
						goto IL_147;
					case "abbr":
					case "acronym":
						HtmlToXamlConverter.AddInlineWithTooltip(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
						goto IL_147;
					case "img":
						HtmlToXamlConverter.AddImage(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext, false);
						goto IL_147;
					case "br":
					case "hr":
						HtmlToXamlConverter.AddBreak(xamlParentElement, text);
						goto IL_147;
				}
				if (HtmlSchema.IsInlineElement(text) || HtmlSchema.IsBlockElement(text))
				{
					HtmlToXamlConverter.AddSpanOrRun(xamlParentElement, xmlElement, inheritedProperties, stylesheet, sourceContext);
				}
			IL_147:
				sourceContext.RemoveAt(sourceContext.Count - 1);
			}
		}
		private static bool HasOnlyWhitespace(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return true;
			}
			int length = text.Length;
			for (int i = 0; i < length; i++)
			{
				if (!char.IsWhiteSpace(text[i]))
				{
					return false;
				}
			}
			return true;
		}
		private static XmlElement AddSpanOrRun(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			bool flag = false;
			for (XmlNode xmlNode = htmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode is XmlElement)
				{
					string text = ((XmlElement)xmlNode).LocalName.ToLower();
					if (HtmlSchema.IsInlineElement(text) || HtmlSchema.IsBlockElement(text) || text == "img" || text == "br" || text == "hr")
					{
						flag = true;
						break;
					}
				}
				else
				{
					if (xmlNode is XmlText)
					{
						string innerText = ((XmlText)xmlNode).InnerText;
						if (!HtmlToXamlConverter.HasOnlyWhitespace(innerText))
						{
							flag = true;
							break;
						}
					}
				}
			}
			string localName = flag ? "Span" : "Run";
			Hashtable localProperties;
			Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
			if (!flag && elementProperties.Count == 0)
			{
				return null;
			}
			XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, localName, HtmlToXamlConverter._xamlNamespace);
			HtmlToXamlConverter.ApplyLocalProperties(xmlElement, localProperties, false);
			if (string.Compare(xamlParentElement.Name, "Span", true) == 0 && string.Compare(xmlElement.Name, "Span", true) == 0 && !xmlElement.HasAttributes)
			{
				xmlElement = xamlParentElement;
			}
			for (XmlNode xmlNode2 = htmlElement.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
			{
				HtmlToXamlConverter.AddInline(xmlElement, xmlNode2, elementProperties, stylesheet, sourceContext);
			}
			if (xmlElement != xamlParentElement)
			{
				HtmlToXamlConverter.AddXamlElementToParent(xamlParentElement, xmlElement);
			}
			return xmlElement;
		}
		private static void AddTextRun(XmlElement xamlElement, string textData)
		{
			for (int i = 0; i < textData.Length; i++)
			{
				if (char.IsControl(textData[i]))
				{
					textData = textData.Remove(i--, 1);
				}
			}
			textData = textData.Replace('\u00a0', ' ');
			if (textData.Length > 0)
			{
				xamlElement.AppendChild(xamlElement.OwnerDocument.CreateTextNode(textData));
			}
		}
		private static void AddInlineWithTooltip(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			HtmlToXamlConverter.AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
		}
		private static void AddXamlElementToParent(XmlNode xamlParentElement, XmlNode xamlElement)
		{
			if (string.Compare(xamlParentElement.Name, "hyperlink", true) == 0)
			{
				if (string.Compare(xamlElement.Name, "hyperlink", true) == 0)
				{
					xamlParentElement.InnerXml += xamlElement.InnerXml;
					return;
				}
				if (xamlElement.HasChildNodes)
				{
					foreach (XmlNode xmlNode in xamlElement.ChildNodes)
					{
						if (string.Compare(xmlNode.Name, "hyperlink", true) == 0)
						{
							xamlParentElement.InnerXml += xmlNode.InnerXml;
							return;
						}
					}
				}
			}
			xamlParentElement.AppendChild(xamlElement);
		}

		private static void AddHyperlink(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			string attribute = GetAttribute(htmlElement, "href");
			if (attribute == null)
			{
				AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
			}
			else
			{
				string uriString = RemoveControlCharacters(HttpUtility.UrlDecode(attribute));
				Uri result = null;
				if (!Uri.TryCreate(uriString, UriKind.RelativeOrAbsolute, out result))
				{
					AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
				}
				else
				{
					Hashtable hashtable;
					Hashtable hashtable2 = GetElementProperties(htmlElement, inheritedProperties, out hashtable, stylesheet, sourceContext);
					XmlElement xamlElement = xamlParentElement.OwnerDocument.CreateElement(null, "Hyperlink", _xamlNamespace);
					ApplyLocalProperties(xamlElement, hashtable, false);
					xamlElement.SetAttribute("NavigateUri", uriString);
					xamlElement.SetAttribute("ToolTip", "LMB click or Ctrl+LMB click to open the link...");
					for (XmlNode node = htmlElement.FirstChild; node != null; node = node.NextSibling)
					{
						AddInline(xamlElement, node, hashtable2, stylesheet, sourceContext);
					}
					AddXamlElementToParent(xamlParentElement, xamlElement);
				}
			}
		}
		private static string RemoveControlCharacters(string source)
		{
			if (string.IsNullOrEmpty(source))
			{
				return source;
			}
			int num = 0;
			for (int i = 0; i < source.Length; i++)
			{
				char c = source[i];
				if (c < ' ')
				{
					num++;
				}
			}
			if (num == 0)
			{
				return source;
			}
			char[] array = new char[source.Length - num];
			num = 0;
			for (int j = 0; j < source.Length; j++)
			{
				if (source[j] < ' ')
				{
					num++;
				}
				else
				{
					array[j - num] = source[j];
				}
			}
			return new string(array);
		}
		private static void DefineInlineFragmentParent(XmlComment htmlComment, XmlElement xamlParentElement)
		{
		}
		private static XmlElement ExtractInlineFragment(XmlElement xamlFlowDocumentElement)
		{
			if (HtmlToXamlConverter.InlineFragmentParentElement != null)
			{
				if (HtmlToXamlConverter.InlineFragmentParentElement.LocalName == "Span")
				{
					xamlFlowDocumentElement = HtmlToXamlConverter.InlineFragmentParentElement;
				}
				else
				{
					xamlFlowDocumentElement = xamlFlowDocumentElement.OwnerDocument.CreateElement(null, "Span", HtmlToXamlConverter._xamlNamespace);
					while (HtmlToXamlConverter.InlineFragmentParentElement.FirstChild != null)
					{
						XmlNode firstChild = HtmlToXamlConverter.InlineFragmentParentElement.FirstChild;
						HtmlToXamlConverter.InlineFragmentParentElement.RemoveChild(firstChild);
						xamlFlowDocumentElement.AppendChild(firstChild);
					}
				}
			}
			return xamlFlowDocumentElement;
		}
		private static void AddImage(XmlElement xamlParentElement, XmlElement htmlElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext, bool isBlock)
		{
			string text = HtmlToXamlConverter.GetAttribute(htmlElement, "src");
			if (text == null)
			{
				HtmlToXamlConverter.AddSpanOrRun(xamlParentElement, htmlElement, inheritedProperties, stylesheet, sourceContext);
				return;
			}
			Hashtable hashtable;
			HtmlToXamlConverter.GetElementProperties(htmlElement, inheritedProperties, out hashtable, stylesheet, sourceContext);
			XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, isBlock ? "BlockUIContainer" : "InlineUIContainer", HtmlToXamlConverter._xamlNamespace);
			XmlElement xmlElement2 = xamlParentElement.OwnerDocument.CreateElement(null, "Image", HtmlToXamlConverter._xamlNamespace);
			xmlElement2.SetAttribute("Stretch", "None");
			XmlNode xmlNode = htmlElement.Attributes.GetNamedItem("width") ?? htmlElement.Attributes.GetNamedItem("WIDTH");
			if (xmlNode != null)
			{
				string value = HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(xmlNode.Value);
				xmlElement2.SetAttribute("Width", value);
				xmlElement2.SetAttribute("Stretch", "Fill");
			}
			xmlNode = (htmlElement.Attributes.GetNamedItem("height") ?? htmlElement.Attributes.GetNamedItem("HEIGHT"));
			if (xmlNode != null)
			{
				string value2 = HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(xmlNode.Value);
				xmlElement2.SetAttribute("Height", value2);
				xmlElement2.SetAttribute("Stretch", "Fill");
			}
			text = text.Trim();
			if (text.Length > 0)
			{
				xmlElement2.SetAttribute("Tag", text);
			}
			string text2 = "[" + text + "]";
			xmlNode = (htmlElement.Attributes.GetNamedItem("alt") ?? htmlElement.Attributes.GetNamedItem("ALT"));
			if (xmlNode != null && !string.IsNullOrEmpty(xmlNode.Value))
			{
				text2 = xmlNode.Value + "\r\n" + text2;
			}
			xmlElement2.SetAttribute("ToolTip", text2);
			xmlElement.AppendChild(xmlElement2);
			HtmlToXamlConverter.AddXamlElementToParent(xamlParentElement, xmlElement);
		}
		private static void AddList(XmlElement xamlParentElement, XmlElement htmlListElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			string a = htmlListElement.LocalName.ToLower();
			Hashtable localProperties;
			Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlListElement, inheritedProperties, out localProperties, stylesheet, sourceContext);
			XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, "List", HtmlToXamlConverter._xamlNamespace);
			if (xmlElement != null)
			{
				if (a == "ol")
				{
					xmlElement.SetAttribute("MarkerStyle", "Decimal");
				}
				else
				{
					if (a == "dl")
					{
						xmlElement.SetAttribute("MarkerStyle", "None");
					}
					else
					{
						xmlElement.SetAttribute("MarkerStyle", "Disc");
					}
				}
				xmlElement.SetAttribute("Margin", "0");
				HtmlToXamlConverter.ApplyLocalProperties(xmlElement, localProperties, true);
				bool flag = false;
				for (XmlNode xmlNode = htmlListElement.FirstChild; xmlNode != null; xmlNode = ((xmlNode != null) ? xmlNode.NextSibling : null))
				{
					if (xmlNode is XmlElement && (xmlNode.LocalName.ToLower() == "li" || xmlNode.LocalName.ToLower() == "dt" || xmlNode.LocalName.ToLower() == "dd"))
					{
						sourceContext.Add((XmlElement)xmlNode);
						HtmlToXamlConverter.AddListItem(xmlElement, (XmlElement)xmlNode, elementProperties, stylesheet, sourceContext);
						sourceContext.RemoveAt(sourceContext.Count - 1);
						flag = true;
					}
					else
					{
						if (xmlNode is XmlElement && (xmlNode.LocalName.ToLower() == "ol" || xmlNode.LocalName.ToLower() == "ul" || xmlNode.LocalName.ToLower() == "dl"))
						{
							XmlElement xmlElement2 = xmlElement.LastChild as XmlElement;
							if (xmlElement2 != null)
							{
								HtmlToXamlConverter.AddList(xmlElement2, (XmlElement)xmlNode, elementProperties, stylesheet, sourceContext);
							}
							else
							{
								HtmlToXamlConverter.AddListItem(xmlElement, (XmlElement)xmlNode, elementProperties, stylesheet, sourceContext);
							}
							flag = true;
						}
						else
						{
							XmlElement xmlElement3 = xmlElement.OwnerDocument.CreateElement(null, "ListItem", HtmlToXamlConverter._xamlNamespace);
							while (xmlNode != null)
							{
								if (xmlNode is XmlElement && (xmlNode.LocalName.ToLower() == "li" || xmlNode.LocalName.ToLower() == "dt" || xmlNode.LocalName.ToLower() == "dd"))
								{
									xmlNode = xmlNode.PreviousSibling;
									break;
								}
								xmlNode = HtmlToXamlConverter.AddBlock(xmlElement3, xmlNode, elementProperties, stylesheet, sourceContext);
								xmlNode = ((xmlNode != null) ? xmlNode.NextSibling : null);
							}
							xmlElement.AppendChild(xmlElement3);
						}
					}
				}
				if (!flag)
				{
					xmlElement.SetAttribute("MarkerStyle", "None");
				}
				if (xmlElement.HasChildNodes)
				{
					xamlParentElement.AppendChild(xmlElement);
				}
			}
		}
		private static XmlElement AddOrphanListItems(XmlElement xamlParentElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			XmlElement result = null;
			XmlNode lastChild = xamlParentElement.LastChild;
			XmlElement xmlElement;
			if (lastChild != null && lastChild.LocalName == "List")
			{
				xmlElement = (lastChild as XmlElement);
			}
			else
			{
				xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, "List", HtmlToXamlConverter._xamlNamespace);
				xamlParentElement.AppendChild(xmlElement);
			}
			XmlNode xmlNode = htmlLIElement;
			string a = (xmlNode == null) ? null : xmlNode.LocalName.ToLower();
			if (xmlElement != null)
			{
				while (xmlNode != null && a == "li")
				{
					HtmlToXamlConverter.AddListItem(xmlElement, (XmlElement)xmlNode, inheritedProperties, stylesheet, sourceContext);
					result = (XmlElement)xmlNode;
					xmlNode = xmlNode.NextSibling;
					a = ((xmlNode == null) ? null : xmlNode.LocalName.ToLower());
				}
			}
			return result;
		}
		private static void AddListItem(XmlElement xamlListElement, XmlElement htmlLIElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			Hashtable hashtable;
			Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlLIElement, inheritedProperties, out hashtable, stylesheet, sourceContext);
			XmlElement xmlElement = xamlListElement.OwnerDocument.CreateElement(null, "ListItem", HtmlToXamlConverter._xamlNamespace);
			for (XmlNode xmlNode = htmlLIElement.FirstChild; xmlNode != null; xmlNode = ((xmlNode != null) ? xmlNode.NextSibling : null))
			{
				xmlNode = HtmlToXamlConverter.AddBlock(xmlElement, xmlNode, elementProperties, stylesheet, sourceContext);
			}
			xamlListElement.AppendChild(xmlElement);
		}
		private static void AddTable(XmlElement xamlParentElement, XmlElement htmlTableElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			Hashtable hashtable;
			Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(htmlTableElement, inheritedProperties, out hashtable, stylesheet, sourceContext);
			XmlElement xmlElement = xamlParentElement.OwnerDocument.CreateElement(null, "Table", HtmlToXamlConverter._xamlNamespace);
			string attribute = HtmlToXamlConverter.GetAttribute(htmlTableElement, "cellspacing");
			if (attribute != null)
			{
				xmlElement.SetAttribute("CellSpacing", HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(attribute));
			}
			string text = (string)hashtable["border-width-top"];
			string text2 = (string)hashtable["border-width-right"];
			string text3 = (string)hashtable["border-width-bottom"];
			string text4 = (string)hashtable["border-width-left"];
			if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(text3) || !string.IsNullOrEmpty(text4))
			{
				xmlElement.SetAttribute("BorderThickness", string.Concat(new string[]
				{
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text4 ?? "0"),
					",",
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text ?? "0"),
					",",
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text2 ?? "0"),
					",",
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text3 ?? "0")
				}));
				string text5 = (string)hashtable["border-color-top"];
				if (string.IsNullOrEmpty(text5))
				{
					text5 = (string)hashtable["border-color-right"];
					if (string.IsNullOrEmpty(text5))
					{
						text5 = (string)hashtable["border-color-bottom"];
						if (string.IsNullOrEmpty(text5))
						{
							text5 = (string)hashtable["border-color-left"];
						}
					}
				}
				if (!string.IsNullOrEmpty(text5) && HtmlToXamlConverter.IsColorCorrect(text5))
				{
					xmlElement.SetAttribute("BorderBrush", text5);
				}
				else
				{
					xmlElement.SetAttribute("BorderBrush", "DarkGray");
				}
			}
			ArrayList arrayList = HtmlToXamlConverter.AnalyzeTableStructure(htmlTableElement, stylesheet);
			HtmlToXamlConverter.AddColumnInformation(htmlTableElement, xmlElement, arrayList, elementProperties, stylesheet, sourceContext);
			XmlNode xmlNode = htmlTableElement.FirstChild;
			while (xmlNode != null)
			{
				string a = xmlNode.LocalName.ToLower();
				if (a == "tbody" || a == "thead" || a == "tfoot")
				{
					XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement(null, "TableRowGroup", HtmlToXamlConverter._xamlNamespace);
					xmlElement.AppendChild(xmlElement2);
					sourceContext.Add((XmlElement)xmlNode);
					Hashtable hashtable2;
					Hashtable elementProperties2 = HtmlToXamlConverter.GetElementProperties(xmlNode as XmlElement, elementProperties, out hashtable2, stylesheet, sourceContext);
					HtmlToXamlConverter.AddTableRowsToTableBody(xamlParentElement, xmlElement2, xmlNode.FirstChild, elementProperties2, arrayList, stylesheet, sourceContext);
					if (xmlElement2.HasChildNodes)
					{
						xmlElement.AppendChild(xmlElement2);
					}
					sourceContext.RemoveAt(sourceContext.Count - 1);
					xmlNode = xmlNode.NextSibling;
				}
				else
				{
					if (a == "tr")
					{
						XmlElement xmlElement3 = xmlElement.OwnerDocument.CreateElement(null, "TableRowGroup", HtmlToXamlConverter._xamlNamespace);
						xmlNode = HtmlToXamlConverter.AddTableRowsToTableBody(xamlParentElement, xmlElement3, xmlNode, elementProperties, arrayList, stylesheet, sourceContext);
						if (xmlElement3.HasChildNodes)
						{
							xmlElement.AppendChild(xmlElement3);
						}
					}
					else
					{
						if (a == "td" || a == "th")
						{
							ArrayList activeRowSpans = null;
							if (arrayList != null)
							{
								activeRowSpans = new ArrayList();
								HtmlToXamlConverter.InitializeActiveRowSpans(activeRowSpans, arrayList.Count);
							}
							XmlElement xmlElement4 = xmlElement.OwnerDocument.CreateElement(null, "TableRowGroup", HtmlToXamlConverter._xamlNamespace);
							XmlElement xmlElement5 = xmlElement4.OwnerDocument.CreateElement(null, "TableRow", HtmlToXamlConverter._xamlNamespace);
							xmlElement4.AppendChild(xmlElement5);
							xmlNode = HtmlToXamlConverter.AddTableCellsToTableRow(xamlParentElement, xmlElement5, xmlNode, elementProperties, arrayList, activeRowSpans, stylesheet, sourceContext);
							if (xmlElement5.HasChildNodes)
							{
								xmlElement.AppendChild(xmlElement4);
							}
						}
						else
						{
							xmlNode = HtmlToXamlConverter.AddBlock(xamlParentElement, xmlNode, elementProperties, stylesheet, sourceContext);
							if (xmlNode != null)
							{
								xmlNode = xmlNode.NextSibling;
							}
						}
					}
				}
			}
			if (xmlElement.HasChildNodes)
			{
				string attribute2 = HtmlToXamlConverter.GetAttribute(htmlTableElement, "width");
				double num;
				if (arrayList == null && attribute2 != null && HtmlToXamlConverter.TryGetLengthValue(attribute2, out num) && num > 0.0)
				{
					XmlNode xmlNode2 = null;
					int num2 = 0;
					foreach (XmlNode xmlNode3 in xmlElement.ChildNodes)
					{
						if (xmlNode3.Name == "Table.Columns")
						{
							xmlNode2 = xmlNode3;
							break;
						}
					}
					if (xmlNode2 != null)
					{
						foreach (XmlNode xmlNode4 in xmlElement.ChildNodes)
						{
							if (xmlNode4.Name == "TableRowGroup")
							{
								XmlNodeList childNodes = xmlNode4.ChildNodes;
								foreach (XmlNode xmlNode5 in childNodes)
								{
									int num3 = 0;
									foreach (XmlNode xmlNode6 in xmlNode5.ChildNodes)
									{
										int num4;
										if (xmlNode6.Attributes["ColumnSpan"] != null && !string.IsNullOrEmpty(xmlNode6.Attributes["ColumnSpan"].Value) && int.TryParse(xmlNode6.Attributes["ColumnSpan"].Value, out num4))
										{
											num3 += num4;
										}
										else
										{
											num3++;
										}
									}
									if (num3 > num2)
									{
										num2 = num3;
									}
								}
							}
						}
					}
					if (num2 == 1)
					{
						XmlElement xmlElement6 = xmlElement.OwnerDocument.CreateElement(null, "TableColumn", HtmlToXamlConverter._xamlNamespace);
						xmlElement6.SetAttribute("Width", num.ToString(CultureInfo.InvariantCulture));
						xmlNode2.AppendChild(xmlElement6);
					}
				}
				xamlParentElement.AppendChild(xmlElement);
			}
		}
		private static XmlElement GetCellFromSingleCellTable(XmlElement htmlTableElement)
		{
			XmlElement xmlElement = null;
			for (XmlNode xmlNode = htmlTableElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				string a = xmlNode.LocalName.ToLower();
				if (a == "tbody" || a == "thead" || a == "tfoot")
				{
					if (xmlElement != null)
					{
						return null;
					}
					for (XmlNode xmlNode2 = xmlNode.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
					{
						if (xmlNode2.LocalName.ToLower() == "tr")
						{
							if (xmlElement != null)
							{
								return null;
							}
							for (XmlNode xmlNode3 = xmlNode2.FirstChild; xmlNode3 != null; xmlNode3 = xmlNode3.NextSibling)
							{
								string a2 = xmlNode3.LocalName.ToLower();
								if (a2 == "td" || a2 == "th")
								{
									if (xmlElement != null)
									{
										return null;
									}
									xmlElement = (xmlNode3 as XmlElement);
								}
							}
						}
					}
				}
				else
				{
					if (xmlNode.LocalName.ToLower() == "tr")
					{
						if (xmlElement != null)
						{
							return null;
						}
						for (XmlNode xmlNode4 = xmlNode.FirstChild; xmlNode4 != null; xmlNode4 = xmlNode4.NextSibling)
						{
							string a3 = xmlNode4.LocalName.ToLower();
							if (a3 == "td" || a3 == "th")
							{
								if (xmlElement != null)
								{
									return null;
								}
								xmlElement = (xmlNode4 as XmlElement);
							}
						}
					}
				}
			}
			return xmlElement;
		}
		private static void AddColumnInformation(XmlElement htmlTableElement, XmlElement xamlTableElement, ArrayList columnStartsAllRows, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			XmlElement xmlElement = xamlTableElement.OwnerDocument.CreateElement(null, "Table.Columns", HtmlToXamlConverter._xamlNamespace);
			xamlTableElement.AppendChild(xmlElement);
			if (columnStartsAllRows != null)
			{
				for (int i = 0; i < columnStartsAllRows.Count - 1; i++)
				{
					XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement(null, "TableColumn", HtmlToXamlConverter._xamlNamespace);
					double num = (double)columnStartsAllRows[i + 1] - (double)columnStartsAllRows[i];
					if (num > 0.0)
					{
						xmlElement2.SetAttribute("Width", num.ToString());
					}
					xmlElement.AppendChild(xmlElement2);
				}
				return;
			}
			for (XmlNode xmlNode = htmlTableElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode.LocalName.ToLower() == "colgroup")
				{
					HtmlToXamlConverter.AddTableColumnGroup(xmlElement, xmlNode as XmlElement, currentProperties, stylesheet, sourceContext);
				}
				else
				{
					if (xmlNode.LocalName.ToLower() == "col")
					{
						HtmlToXamlConverter.AddTableColumn(xmlElement, xmlNode as XmlElement, currentProperties, stylesheet, sourceContext);
					}
					else
					{
						if (xmlNode is XmlElement)
						{
							return;
						}
					}
				}
			}
		}
		private static void AddTableColumnGroup(XmlElement xamlTableElement, XmlElement htmlColgroupElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			Hashtable hashtable = new Hashtable();
			string attribute = HtmlToXamlConverter.GetAttribute(htmlColgroupElement, "width");
			if (attribute != null)
			{
				hashtable["width"] = attribute;
			}
			for (XmlNode xmlNode = htmlColgroupElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
			{
				if (xmlNode is XmlElement && xmlNode.LocalName.ToLower() == "col")
				{
					HtmlToXamlConverter.AddTableColumn(xamlTableElement, (XmlElement)xmlNode, hashtable, stylesheet, sourceContext);
				}
			}
		}
		private static void AddTableColumn(XmlElement xamlTableElement, XmlElement htmlColElement, Hashtable inheritedProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			Hashtable hashtable = new Hashtable(inheritedProperties);
			string attribute = HtmlToXamlConverter.GetAttribute(htmlColElement, "width");
			if (attribute != null)
			{
				hashtable["width"] = attribute;
			}
			XmlElement xmlElement = xamlTableElement.OwnerDocument.CreateElement(null, "TableColumn", HtmlToXamlConverter._xamlNamespace);
			string value = hashtable["width"] as string;
			if (!string.IsNullOrEmpty(value))
			{
				xmlElement.SetAttribute("Width", value);
			}
			xamlTableElement.AppendChild(xmlElement);
		}
		private static XmlNode AddTableRowsToTableBody(XmlElement xamlTableParentElement, XmlElement xamlTableBodyElement, XmlNode htmlTRStartNode, Hashtable currentProperties, ArrayList columnStarts, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			XmlNode xmlNode = htmlTRStartNode;
			ArrayList activeRowSpans = null;
			if (columnStarts != null)
			{
				activeRowSpans = new ArrayList();
				HtmlToXamlConverter.InitializeActiveRowSpans(activeRowSpans, columnStarts.Count);
			}
			while (xmlNode != null && xmlNode.LocalName.ToLower() != "tbody")
			{
				if (xmlNode.LocalName.ToLower() == "tr")
				{
					XmlElement xmlElement = xamlTableBodyElement.OwnerDocument.CreateElement(null, "TableRow", HtmlToXamlConverter._xamlNamespace);
					sourceContext.Add((XmlElement)xmlNode);
					Hashtable hashtable;
					Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(xmlNode as XmlElement, currentProperties, out hashtable, stylesheet, sourceContext);
					HtmlToXamlConverter.AddTableCellsToTableRow(xamlTableParentElement, xmlElement, xmlNode.FirstChild, elementProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
					if (xmlElement.HasChildNodes)
					{
						xamlTableBodyElement.AppendChild(xmlElement);
					}
					sourceContext.RemoveAt(sourceContext.Count - 1);
					xmlNode = xmlNode.NextSibling;
				}
				else
				{
					if (xmlNode.LocalName.ToLower() == "td")
					{
						XmlElement xmlElement2 = xamlTableBodyElement.OwnerDocument.CreateElement(null, "TableRow", HtmlToXamlConverter._xamlNamespace);
						xmlNode = HtmlToXamlConverter.AddTableCellsToTableRow(xamlTableParentElement, xmlElement2, xmlNode, currentProperties, columnStarts, activeRowSpans, stylesheet, sourceContext);
						if (xmlElement2.HasChildNodes)
						{
							xamlTableBodyElement.AppendChild(xmlElement2);
						}
					}
					else
					{
						xmlNode = HtmlToXamlConverter.AddBlock(xamlTableParentElement, xmlNode, currentProperties, stylesheet, sourceContext);
						if (xmlNode != null)
						{
							xmlNode = xmlNode.NextSibling;
						}
					}
				}
			}
			return xmlNode;
		}
		private static XmlNode AddTableCellsToTableRow(XmlElement xamlTableParentElement, XmlElement xamlTableRowElement, XmlNode htmlTDStartNode, Hashtable currentProperties, ArrayList columnStarts, ArrayList activeRowSpans, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			XmlNode xmlNode = htmlTDStartNode;
			int num = 0;
			while (xmlNode != null && xmlNode.LocalName.ToLower() != "tr" && xmlNode.LocalName.ToLower() != "tbody" && xmlNode.LocalName.ToLower() != "thead" && xmlNode.LocalName.ToLower() != "tfoot")
			{
				if (xmlNode.LocalName.ToLower() == "td" || xmlNode.LocalName.ToLower() == "th")
				{
					XmlElement xmlElement = xamlTableRowElement.OwnerDocument.CreateElement(null, "TableCell", HtmlToXamlConverter._xamlNamespace);
					sourceContext.Add((XmlElement)xmlNode);
					Hashtable hashtable;
					Hashtable elementProperties = HtmlToXamlConverter.GetElementProperties(xmlNode as XmlElement, currentProperties, out hashtable, stylesheet, sourceContext);
					HtmlToXamlConverter.ApplyPropertiesToTableCellElement(xmlNode as XmlElement, xmlElement, elementProperties, columnStarts);
					if (columnStarts != null)
					{
						while (num < activeRowSpans.Count && (int)activeRowSpans[num] > 0)
						{
							activeRowSpans[num] = (int)activeRowSpans[num] - 1;
							num++;
						}
						int columnSpan = HtmlToXamlConverter.GetColumnSpan(xmlNode as XmlElement);
						int rowSpan = HtmlToXamlConverter.GetRowSpan(xmlNode as XmlElement);
						xmlElement.SetAttribute("ColumnSpan", columnSpan.ToString());
						for (int i = num; i < num + columnSpan; i++)
						{
							if (i < activeRowSpans.Count)
							{
								activeRowSpans[i] = rowSpan - 1;
							}
						}
						num += columnSpan;
					}
					HtmlToXamlConverter.AddDataToTableCell(xmlElement, xmlNode.FirstChild, elementProperties, stylesheet, sourceContext);
					xamlTableRowElement.AppendChild(xmlElement);
					sourceContext.RemoveAt(sourceContext.Count - 1);
					xmlNode = xmlNode.NextSibling;
				}
				else
				{
					xmlNode = HtmlToXamlConverter.AddBlock(xamlTableParentElement, xmlNode, currentProperties, stylesheet, sourceContext);
					if (xmlNode != null)
					{
						xmlNode = xmlNode.NextSibling;
					}
				}
			}
			return xmlNode;
		}
		private static void AddDataToTableCell(XmlElement xamlTableCellElement, XmlNode htmlDataStartNode, Hashtable currentProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			for (XmlNode xmlNode = htmlDataStartNode; xmlNode != null; xmlNode = ((xmlNode != null) ? xmlNode.NextSibling : null))
			{
				xmlNode = HtmlToXamlConverter.AddBlock(xamlTableCellElement, xmlNode, currentProperties, stylesheet, sourceContext);
			}
		}
		private static ArrayList AnalyzeTableStructure(XmlElement htmlTableElement, CssStylesheet stylesheet)
		{
			if (!htmlTableElement.HasChildNodes)
			{
				return null;
			}
			bool flag = true;
			ArrayList arrayList = new ArrayList();
			ArrayList activeRowSpans = new ArrayList();
			XmlNode xmlNode = htmlTableElement.FirstChild;
			double num = 0.0;
			while (xmlNode != null && flag)
			{
				string a;
				if ((a = xmlNode.LocalName.ToLower()) != null)
				{
					if (!(a == "tbody"))
					{
						if (!(a == "tr"))
						{
							if (a == "td")
							{
								flag = false;
							}
						}
						else
						{
							double num2 = HtmlToXamlConverter.AnalyzeTRStructure(xmlNode as XmlElement, arrayList, activeRowSpans, num, stylesheet);
							if (num2 > num)
							{
								num = num2;
							}
							else
							{
								if (num2 == 0.0)
								{
									flag = false;
								}
							}
						}
					}
					else
					{
						double num3 = HtmlToXamlConverter.AnalyzeTbodyStructure(xmlNode as XmlElement, arrayList, activeRowSpans, num, stylesheet);
						if (num3 > num)
						{
							num = num3;
						}
						else
						{
							if (num3 == 0.0)
							{
								flag = false;
							}
						}
					}
				}
				xmlNode = xmlNode.NextSibling;
			}
			if (flag)
			{
				arrayList.Add(num);
				HtmlToXamlConverter.VerifyColumnStartsAscendingOrder(arrayList);
			}
			else
			{
				arrayList = null;
			}
			return arrayList;
		}
		private static double AnalyzeTbodyStructure(XmlElement htmlTbodyElement, ArrayList columnStarts, ArrayList activeRowSpans, double tableWidth, CssStylesheet stylesheet)
		{
			double num = 0.0;
			bool flag = true;
			if (!htmlTbodyElement.HasChildNodes)
			{
				return num;
			}
			HtmlToXamlConverter.ClearActiveRowSpans(activeRowSpans);
			XmlNode xmlNode = htmlTbodyElement.FirstChild;
			while (xmlNode != null && flag)
			{
				string a;
				if ((a = xmlNode.LocalName.ToLower()) != null)
				{
					if (!(a == "tr"))
					{
						if (a == "td")
						{
							flag = false;
						}
					}
					else
					{
						double num2 = HtmlToXamlConverter.AnalyzeTRStructure(xmlNode as XmlElement, columnStarts, activeRowSpans, num, stylesheet);
						if (num2 > num)
						{
							num = num2;
						}
					}
				}
				xmlNode = xmlNode.NextSibling;
			}
			HtmlToXamlConverter.ClearActiveRowSpans(activeRowSpans);
			if (!flag)
			{
				return 0.0;
			}
			return num;
		}
		private static double AnalyzeTRStructure(XmlElement htmlTRElement, ArrayList columnStarts, ArrayList activeRowSpans, double tableWidth, CssStylesheet stylesheet)
		{
			if (!htmlTRElement.HasChildNodes)
			{
				return 0.0;
			}
			bool flag = true;
			double num = 0.0;
			XmlNode xmlNode = htmlTRElement.FirstChild;
			int i = 0;
			if (i < activeRowSpans.Count && (double)columnStarts[i] == num)
			{
				while (i < activeRowSpans.Count)
				{
					if ((int)activeRowSpans[i] <= 0)
					{
						break;
					}
					activeRowSpans[i] = (int)activeRowSpans[i] - 1;
					i++;
					if (i > columnStarts.Count - 1)
					{
						num = (double)columnStarts[columnStarts.Count - 1];
					}
					else
					{
						num = (double)columnStarts[i];
					}
				}
			}
			while (xmlNode != null && flag)
			{
				HtmlToXamlConverter.VerifyColumnStartsAscendingOrder(columnStarts);
				string a;
				if ((a = xmlNode.LocalName.ToLower()) != null && a == "td")
				{
					if (i < columnStarts.Count)
					{
						if (num < (double)columnStarts[i])
						{
							columnStarts.Insert(i, num);
							activeRowSpans.Insert(i, 0);
						}
					}
					else
					{
						columnStarts.Add(num);
						activeRowSpans.Add(0);
					}
					double num2 = HtmlToXamlConverter.GetColumnWidth(xmlNode as XmlElement);
					if (num2 == -1.0 && i + 1 < columnStarts.Count)
					{
						num2 = (double)columnStarts[i + 1] - (double)columnStarts[i];
					}
					int rowSpan = HtmlToXamlConverter.GetRowSpan(xmlNode as XmlElement);
					int nextColumnIndex = HtmlToXamlConverter.GetNextColumnIndex(i, num2, columnStarts, activeRowSpans);
					if (nextColumnIndex != -1)
					{
						for (int j = i; j < nextColumnIndex; j++)
						{
							activeRowSpans[j] = rowSpan - 1;
						}
						i = nextColumnIndex;
						double num3 = num;
						if (num2 >= 0.0)
						{
							if (i < columnStarts.Count)
							{
								num3 = Math.Max(num + num2, (double)columnStarts[i]);
							}
							else
							{
								num3 = num + num2;
							}
						}
						num = num3;
						if (i < activeRowSpans.Count && (double)columnStarts[i] == num)
						{
							while (i < activeRowSpans.Count)
							{
								if ((int)activeRowSpans[i] <= 0)
								{
									break;
								}
								activeRowSpans[i] = (int)activeRowSpans[i] - 1;
								i++;
								if (i > columnStarts.Count - 1)
								{
									num = (double)columnStarts[columnStarts.Count - 1];
								}
								else
								{
									num = (double)columnStarts[i];
								}
							}
						}
					}
					else
					{
						flag = false;
					}
				}
				xmlNode = xmlNode.NextSibling;
			}
			double result;
			if (flag)
			{
				result = num;
			}
			else
			{
				result = 0.0;
			}
			return result;
		}
		private static int GetRowSpan(XmlElement htmlTDElement)
		{
			string attribute = HtmlToXamlConverter.GetAttribute(htmlTDElement, "rowspan");
			int result;
			if (attribute != null)
			{
				if (!int.TryParse(attribute, out result))
				{
					result = 1;
				}
			}
			else
			{
				result = 1;
			}
			return result;
		}
		private static int GetColumnSpan(XmlElement htmlTDElement)
		{
			string attribute = HtmlToXamlConverter.GetAttribute(htmlTDElement, "colspan");
			int result;
			if (attribute != null)
			{
				if (!int.TryParse(attribute, out result))
				{
					result = 1;
				}
			}
			else
			{
				result = 1;
			}
			return result;
		}
		private static int GetNextColumnIndex(int columnIndex, double columnWidth, ArrayList columnStarts, ArrayList activeRowSpans)
		{
			double num = (double)columnStarts[columnIndex];
			int num2 = columnIndex + 1;
			while (num2 < columnStarts.Count && num2 != -1 && (double)columnStarts[num2] < num + ((columnWidth == -1.0) ? 1.0 : columnWidth))
			{
				if ((int)activeRowSpans[num2] > 0)
				{
					num2 = -1;
				}
				else
				{
					num2++;
				}
			}
			return num2;
		}
		private static void ClearActiveRowSpans(ArrayList activeRowSpans)
		{
			for (int i = 0; i < activeRowSpans.Count; i++)
			{
				activeRowSpans[i] = 0;
			}
		}
		private static void InitializeActiveRowSpans(ArrayList activeRowSpans, int count)
		{
			for (int i = 0; i < count; i++)
			{
				activeRowSpans.Add(0);
			}
		}
		private static double GetNextColumnStart(XmlElement htmlTDElement, double columnStart)
		{
			double columnWidth = HtmlToXamlConverter.GetColumnWidth(htmlTDElement);
			double result;
			if (columnWidth == -1.0)
			{
				result = -1.0;
			}
			else
			{
				result = columnStart + columnWidth;
			}
			return result;
		}
		private static double GetColumnWidth(XmlElement htmlTDElement)
		{
			double num = -1.0;
			string text = HtmlToXamlConverter.GetAttribute(htmlTDElement, "width");
			if (text == null)
			{
				text = HtmlToXamlConverter.GetCssAttribute(HtmlToXamlConverter.GetAttribute(htmlTDElement, "style"), "width");
			}
			if (!HtmlToXamlConverter.TryGetLengthValue(text, out num) || num == 0.0)
			{
				num = -1.0;
			}
			return num;
		}
		private static int CalculateColumnSpan(int columnIndex, double columnWidth, ArrayList columnStarts)
		{
			int num = columnIndex;
			double num2 = 0.0;
			while (num2 < columnWidth && num < columnStarts.Count - 1)
			{
				double num3 = (double)columnStarts[num + 1] - (double)columnStarts[num];
				num2 += num3;
				num++;
			}
			return num - columnIndex;
		}
		private static void VerifyColumnStartsAscendingOrder(ArrayList columnStarts)
		{
			for (int i = 0; i < columnStarts.Count; i++)
			{
				double arg_10_0 = (double)columnStarts[i];
			}
		}

		private static void ApplyLocalProperties(XmlElement xamlElement, Hashtable localProperties, bool isBlock)
		{
			bool flag = false;
			string top = "0";
			string bottom = "0";
			string left = "0";
			string right = "0";
			bool flag2 = false;
			string str5 = "0";
			string str6 = "0";
			string str7 = "0";
			string str8 = "0";
			string str9 = null;
			bool flag3 = false;
			string str10 = "0";
			string str11 = "0";
			string str12 = "0";
			string str13 = "0";
			IDictionaryEnumerator enumerator = localProperties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				string str17;
				string str18;
				string str21;
				switch (((string)enumerator.Key))
				{
					case "font-family":
						{
							string str14 = enumerator.Value as string;
							if (!string.IsNullOrEmpty(str14))
							{
								SetXAMLAttributeWithCheck(xamlElement, "FontFamily", str14);
							}
							continue;
						}
					case "font-style":
						{
							SetXAMLAttributeWithCheck(xamlElement, "FontStyle", (string)enumerator.Value);
							continue;
						}
					case "font-variant":
					case "text-transform":
					case "border-style-top":
					case "border-style-right":
					case "border-style-bottom":
					case "border-style-left":
					case "display":
					case "float":
					case "clear":
						{
							continue;
						}
					case "font-weight":
						{
							SetXAMLAttributeWithCheck(xamlElement, "FontWeight", (string)enumerator.Value);
							continue;
						}
					case "font-size":
						{
							string str15 = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							if (str15 != "0")
							{
								if (str15.Length > 3)
								{
									str15 = "1000";
								}
								SetXAMLAttributeWithCheck(xamlElement, "FontSize", str15);
							}
							continue;
						}
					case "color":
						{
							SetPropertyValue(xamlElement, TextElement.ForegroundProperty, (string)enumerator.Value);
							continue;
						}
					case "background-color":
						{
							SetPropertyValue(xamlElement, TextElement.BackgroundProperty, (string)enumerator.Value);
							continue;
						}
					case "text-decoration-underline":
						{
							if (!isBlock && (((string)enumerator.Value) == "true"))
							{
								SetXAMLAttributeWithCheck(xamlElement, "TextDecorations", "Underline");
							}
							continue;
						}
					case "text-decoration-overline":
						{
							if (!isBlock && (((string)enumerator.Value) == "true"))
							{
								SetXAMLAttributeWithCheck(xamlElement, "TextDecorations", "OverLine");
							}
							continue;
						}
					case "text-decoration-line-through":
						{
							if (!isBlock && (((string)enumerator.Value) == "true"))
							{
								SetXAMLAttributeWithCheck(xamlElement, "TextDecorations", "Strikethrough");
							}
							continue;
						}
					case "text-decoration-none":
					case "text-decoration-blink":
						{
							if (isBlock)
							{
							}
							continue;
						}
					case "text-indent":
						{
							if (isBlock && (xamlElement.Name == "Paragraph"))
							{
								double num;
								double.TryParse(ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string), out num);
								if (num < 0.0)
								{
									num = 0.0;
								}
								SetXAMLAttributeWithCheck(xamlElement, "TextIndent", num.ToString());
							}
							continue;
						}
					case "text-align":
						if (!isBlock)
						{
							continue;
						}
						str17 = (string)enumerator.Value;
						switch (str17.ToLower())
						{
							case "right":
								goto Label_05E5;

							case "center":
							case "centre":
								goto Label_05EE;

							case "justify":
								goto Label_05F7;
						}
						goto Label_0600;

					case "vertical-align":
						{
							if (!isBlock)
							{
								str18 = (string)enumerator.Value;
								str18 = str18.ToLowerInvariant();
								if (!(str18 == "sub"))
								{
									goto Label_0661;
								}
								SetXAMLAttributeWithCheck(xamlElement, "BaselineAlignment", "Subscript");
							}
							continue;
						}
					case "width":
						{
							if (xamlElement.Name == "Image")
							{
								string str19 = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
								SetXAMLAttributeWithCheck(xamlElement, "Width", str19);
								xamlElement.SetAttribute("Stretch", "Fill");
							}
							continue;
						}
					case "height":
						{
							if (xamlElement.Name == "Image")
							{
								string str20 = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
								SetXAMLAttributeWithCheck(xamlElement, "Height", str20);
								xamlElement.SetAttribute("Stretch", "Fill");
							}
							continue;
						}
					case "margin-top":
						{
							flag = true;
							top = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "margin-right":
						{
							flag = true;
							right = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "margin-bottom":
						{
							flag = true;
							bottom = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "margin-left":
						{
							flag = true;
							left = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "padding-top":
						{
							flag2 = true;
							str5 = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "padding-right":
						{
							flag2 = true;
							str8 = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "padding-bottom":
						{
							flag2 = true;
							str6 = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "padding-left":
						{
							flag2 = true;
							str7 = ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string);
							continue;
						}
					case "border-color-top":
						{
							str9 = (string)enumerator.Value;
							continue;
						}
					case "border-color-right":
						{
							str9 = (string)enumerator.Value;
							continue;
						}
					case "border-color-bottom":
						{
							str9 = (string)enumerator.Value;
							continue;
						}
					case "border-color-left":
						{
							str9 = (string)enumerator.Value;
							continue;
						}
					case "border-width-top":
						{
							flag3 = true;
							if (ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string).Length <= 6)
							{
								goto Label_0860;
							}
							str10 = "1000000px";
							continue;
						}
					case "border-width-right":
						{
							flag3 = true;
							if (ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string).Length <= 6)
							{
								goto Label_089B;
							}
							str13 = "1000000px";
							continue;
						}
					case "border-width-bottom":
						{
							flag3 = true;
							if (ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string).Length <= 6)
							{
								goto Label_08D6;
							}
							str11 = "1000000px";
							continue;
						}
					case "border-width-left":
						{
							flag3 = true;
							if (ConvertCSSFontSizeToXAMLFontSize(enumerator.Value as string).Length <= 6)
							{
								goto Label_0911;
							}
							str12 = "1000000px";
							continue;
						}
					case "list-style-type":
						if (!(xamlElement.LocalName == "List"))
						{
							continue;
						}
						switch (((string)enumerator.Value).ToLower())
						{
							case "disc":
								goto Label_0A2B;

							case "circle":
								goto Label_0A34;

							case "none":
								goto Label_0A3D;

							case "square":
								goto Label_0A46;

							case "box":
								goto Label_0A4F;

							case "lower-latin":
								goto Label_0A58;

							case "upper-latin":
								goto Label_0A61;

							case "lower-roman":
								goto Label_0A6A;

							case "upper-roman":
								goto Label_0A73;

							case "decimal":
								goto Label_0A7C;
						}
						goto Label_0A85;

					default:
						{
							continue;
						}
				}
				str17 = "Left";
				goto Label_0603;
			Label_05E5:
				str17 = "Right";
				goto Label_0603;
			Label_05EE:
				str17 = "Center";
				goto Label_0603;
			Label_05F7:
				str17 = "Justify";
				goto Label_0603;
			Label_0600:
				str17 = null;
			Label_0603:
				if (!string.IsNullOrEmpty(str17))
				{
					SetXAMLAttributeWithCheck(xamlElement, "TextAlignment", str17);
				}
				continue;
			Label_0661:
				if (str18 == "super")
				{
					SetXAMLAttributeWithCheck(xamlElement, "BaselineAlignment", "Superscript");
				}
				continue;
			Label_0860:
				str10 = (string)enumerator.Value;
				continue;
			Label_089B:
				str13 = (string)enumerator.Value;
				continue;
			Label_08D6:
				str11 = (string)enumerator.Value;
				continue;
			Label_0911:
				str12 = (string)enumerator.Value;
				continue;
			Label_0A2B:
				str21 = "Disc";
				goto Label_0A8C;
			Label_0A34:
				str21 = "Circle";
				goto Label_0A8C;
			Label_0A3D:
				str21 = "None";
				goto Label_0A8C;
			Label_0A46:
				str21 = "Square";
				goto Label_0A8C;
			Label_0A4F:
				str21 = "Box";
				goto Label_0A8C;
			Label_0A58:
				str21 = "LowerLatin";
				goto Label_0A8C;
			Label_0A61:
				str21 = "UpperLatin";
				goto Label_0A8C;
			Label_0A6A:
				str21 = "LowerRoman";
				goto Label_0A8C;
			Label_0A73:
				str21 = "UpperRoman";
				goto Label_0A8C;
			Label_0A7C:
				str21 = "Decimal";
				goto Label_0A8C;
			Label_0A85:
				str21 = "Disc";
			Label_0A8C:
				SetXAMLAttributeWithCheck(xamlElement, "MarkerStyle", str21);
			}
			if (isBlock)
			{
				if (flag)
				{
					ComposeThicknessProperty(xamlElement, "Margin", left, right, top, bottom);
				}
				if (flag2)
				{
					ComposeThicknessProperty(xamlElement, "Padding", str7, str8, str5, str6);
				}
				if (str9 != null)
				{
					SetXAMLAttributeWithCheck(xamlElement, "BorderBrush", str9);
				}
				if (flag3)
				{
					ComposeThicknessProperty(xamlElement, "BorderThickness", str12, str13, str10, str11);
				}
			}
		}

		private static void SetXAMLAttributeWithCheck(XmlElement xamlElement, string property, string value)
		{
			lock (propertiesByTypeSyncRoot)
			{
				HashSet<string> set;
				if (!propertiesByType.TryGetValue(xamlElement.Name, out set))
				{
					switch (xamlElement.Name)
					{
						case "Image":
							RetrieveTypeProperties(typeof(Image), ref set);
							break;

						case "Paragraph":
							RetrieveTypeProperties(typeof(Paragraph), ref set);
							break;

						case "Section":
							RetrieveTypeProperties(typeof(Section), ref set);
							break;

						case "Hyperlink":
							RetrieveTypeProperties(typeof(Hyperlink), ref set);
							break;

						case "Run":
							RetrieveTypeProperties(typeof(Run), ref set);
							break;

						case "Span":
							RetrieveTypeProperties(typeof(Span), ref set);
							break;

						case "List":
							RetrieveTypeProperties(typeof(List), ref set);
							break;
					}
				}
				if (set != null)
				{
					if (set.Contains(property))
					{
						xamlElement.SetAttribute(property, value);
					}
				}
				else
				{
					xamlElement.SetAttribute(property, value);
				}
			}
		}


		private static void RetrieveTypeProperties(Type type, ref HashSet<string> propertiesSet)
		{
			PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
			propertiesSet = new HashSet<string>();
			PropertyInfo[] array = properties;
			for (int i = 0; i < array.Length; i++)
			{
				PropertyInfo propertyInfo = array[i];
				if (propertyInfo.CanWrite)
				{
					propertiesSet.Add(propertyInfo.Name);
				}
			}
			HtmlToXamlConverter.propertiesByType.Add(type.Name, propertiesSet);
		}
		private static string ConvertCSSFontSizeToXAMLFontSize(string cssFontSize)
		{
			cssFontSize = cssFontSize.Trim().ToLowerInvariant();
			int num = -1;
			double num2 = HtmlToXamlConverter.fontSizePPEM;
			try
			{
				HtmlToXamlConverter.FontSizeInfo[] array = HtmlToXamlConverter.fontSizes;
				int i = 0;
				while (i < array.Length)
				{
					HtmlToXamlConverter.FontSizeInfo fontSizeInfo = array[i];
					if ((num = cssFontSize.IndexOf(fontSizeInfo.Text)) != -1)
					{
						if (fontSizeInfo.IsUnit)
						{
							string s = cssFontSize.Substring(0, num);
							num2 = double.Parse(s, CultureInfo.InvariantCulture);
							num2 *= fontSizeInfo.Factor;
							break;
						}
						num2 = fontSizeInfo.Value;
						break;
					}
					else
					{
						i++;
					}
				}
				if (num == -1)
				{
					num2 = double.Parse(cssFontSize, CultureInfo.InvariantCulture);
				}
			}
			catch
			{
			}
			num2 *= HtmlToXamlConverter.htmlToXamlFontScale;
			num2 = Math.Round(num2);
			if (num2 > 1000000.0)
			{
				num2 = 1000000.0;
			}
			return num2.ToString(CultureInfo.InvariantCulture);
		}
		private static void ComposeThicknessProperty(XmlElement xamlElement, string propertyName, string left, string right, string top, string bottom)
		{
			if (left[0] == '0' || left[0] == '-')
			{
				left = "0";
			}
			if (right[0] == '0' || right[0] == '-')
			{
				right = "0";
			}
			if (top[0] == '0' || top[0] == '-')
			{
				top = "0";
			}
			if (bottom[0] == '0' || bottom[0] == '-')
			{
				bottom = "0";
			}
			string value;
			if (left == right && top == bottom)
			{
				if (left == top)
				{
					value = left;
				}
				else
				{
					value = left + "," + top;
				}
			}
			else
			{
				value = string.Concat(new string[]
				{
					left,
					",",
					top,
					",",
					right,
					",",
					bottom
				});
			}
			HtmlToXamlConverter.SetXAMLAttributeWithCheck(xamlElement, propertyName, value);
		}
		private static void SetPropertyValue(XmlElement xamlElement, DependencyProperty property, string stringValue)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
			if (converter != null)
			{
				try
				{
					object obj = converter.ConvertFromInvariantString(stringValue);
					if (obj != null)
					{
						xamlElement.SetAttribute(property.Name, stringValue);
					}
				}
				catch (Exception)
				{
				}
			}
		}
		private static Hashtable GetElementProperties(XmlElement htmlElement, Hashtable inheritedProperties, out Hashtable localProperties, CssStylesheet stylesheet, List<XmlElement> sourceContext)
		{
			Hashtable hashtable = new Hashtable();
			IDictionaryEnumerator enumerator = inheritedProperties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				hashtable[enumerator.Key] = enumerator.Value;
			}
			string text = htmlElement.LocalName.ToLower();
			string arg_3B_0 = htmlElement.NamespaceURI;
			localProperties = new Hashtable();
			string key;
			switch (key = text)
			{
				case "i":
				case "italic":
				case "em":
				case "address":
				case "cite":
				case "var":
					localProperties["font-style"] = "italic";
					break;
				case "b":
				case "bold":
				case "strong":
				case "dfn":
					localProperties["font-weight"] = "bold";
					break;
				case "ins":
				case "u":
				case "underline":
					localProperties["text-decoration-underline"] = "true";
					break;
				case "del":
				case "s":
				case "strike":
					localProperties["text-decoration-line-through"] = "true";
					break;
				case "small":
					localProperties["font-size"] = "8pt";
					break;
				case "big":
					localProperties["font-size"] = "16pt";
					break;
				case "font":
					{
						string attribute = HtmlToXamlConverter.GetAttribute(htmlElement, "face");
						if (!string.IsNullOrEmpty(attribute))
						{
							localProperties["font-family"] = attribute;
						}
						attribute = HtmlToXamlConverter.GetAttribute(htmlElement, "size");
						if (attribute != null)
						{
							double num2 = double.Parse(attribute, CultureInfo.InvariantCulture);
							if (attribute.IndexOf('+') != -1 || attribute.IndexOf('-') != -1)
							{
								num2 = 3.0 + num2;
								if (num2 > 7.0)
								{
									num2 = 7.0;
								}
							}
							if (num2 < 1.0)
							{
								num2 = 1.0;
							}
							else
							{
								if (num2 > 1000.0)
								{
									num2 = 1000.0;
								}
							}
							if (num2 == 1.0)
							{
								num2 = 10.0;
							}
							else
							{
								if (num2 == 2.0)
								{
									num2 = 13.0;
								}
								else
								{
									if (num2 == 3.0)
									{
										num2 = 16.0;
									}
									else
									{
										if (num2 == 4.0)
										{
											num2 = 18.0;
										}
										else
										{
											if (num2 == 5.0)
											{
												num2 = 24.0;
											}
											else
											{
												if (num2 == 6.0)
												{
													num2 = 32.0;
												}
												else
												{
													if (num2 == 7.0)
													{
														num2 = 48.0;
													}
												}
											}
										}
									}
								}
							}
							num2 *= HtmlToXamlConverter.htmlToXamlFontScale;
							num2 = Math.Round(num2);
							localProperties["font-size"] = num2.ToString(CultureInfo.InvariantCulture);
						}
						attribute = HtmlToXamlConverter.GetAttribute(htmlElement, "color");
						if (attribute != null)
						{
							localProperties["color"] = attribute;
						}
						break;
					}
				case "sub":
					localProperties["vertical-align"] = "sub";
					break;
				case "sup":
					localProperties["vertical-align"] = "super";
					break;
				case "p":
					{
						string attribute2 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute2 != null)
						{
							localProperties["text-align"] = attribute2;
						}
						break;
					}
				case "div":
					{
						string attribute3 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute3 != null)
						{
							localProperties["text-align"] = attribute3;
						}
						break;
					}
				case "samp":
				case "code":
				case "tt":
				case "kbd":
				case "pre":
					localProperties["font-family"] = "Courier New";
					localProperties["text-align"] = "Left";
					break;
				case "blockquote":
					localProperties["margin-left"] = "16";
					localProperties["margin-rigth"] = "16";
					localProperties["margin-top"] = "16";
					localProperties["margin-bottom"] = "16";
					break;
				case "h1":
					{
						localProperties["font-size"] = "22pt";
						string attribute4 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute4 != null)
						{
							localProperties["text-align"] = attribute4;
						}
						break;
					}
				case "h2":
					{
						localProperties["font-size"] = "20pt";
						string attribute5 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute5 != null)
						{
							localProperties["text-align"] = attribute5;
						}
						break;
					}
				case "h3":
					{
						localProperties["font-size"] = "18pt";
						string attribute6 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute6 != null)
						{
							localProperties["text-align"] = attribute6;
						}
						break;
					}
				case "h4":
					{
						localProperties["font-size"] = "16pt";
						string attribute7 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute7 != null)
						{
							localProperties["text-align"] = attribute7;
						}
						break;
					}
				case "h5":
					{
						localProperties["font-size"] = "12pt";
						string attribute8 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute8 != null)
						{
							localProperties["text-align"] = attribute8;
						}
						break;
					}
				case "h6":
					{
						localProperties["font-size"] = "10pt";
						string attribute9 = HtmlToXamlConverter.GetAttribute(htmlElement, "align");
						if (attribute9 != null)
						{
							localProperties["text-align"] = attribute9;
						}
						break;
					}
				case "ul":
					localProperties["list-style-type"] = "disc";
					break;
				case "ol":
					localProperties["list-style-type"] = "decimal";
					break;
				case "table":
					{
						string attribute10 = HtmlToXamlConverter.GetAttribute(htmlElement, "border");
						if (!string.IsNullOrEmpty(attribute10) && attribute10.Trim() != "0")
						{
							localProperties["border-width-top"] = (localProperties["border-width-right"] = (localProperties["border-width-bottom"] = (localProperties["border-width-left"] = attribute10)));
						}
						break;
					}
				case "colgroup":
				case "col":
					{
						string attribute11 = HtmlToXamlConverter.GetAttribute(htmlElement, "width");
						if (attribute11 != null)
						{
							localProperties["width"] = attribute11;
						}
						break;
					}
			}
			HtmlCssParser.GetElementPropertiesFromCssAttributes(htmlElement, text, stylesheet, localProperties, sourceContext);
			enumerator = localProperties.GetEnumerator();
			while (enumerator.MoveNext())
			{
				hashtable[enumerator.Key] = enumerator.Value;
			}
			return hashtable;
		}
		private static string GetCssAttribute(string cssStyle, string attributeName)
		{
			if (cssStyle != null)
			{
				attributeName = attributeName.ToLower();
				string[] array = cssStyle.Split(new char[]
				{
					';'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						':'
					});
					if (array2.Length == 2 && array2[0].Trim().ToLower() == attributeName)
					{
						return array2[1].Trim();
					}
				}
			}
			return null;
		}
		private static bool TryGetLengthValue(string lengthAsString, out double length)
		{
			length = double.NaN;
			if (lengthAsString != null)
			{
				lengthAsString = lengthAsString.Trim().ToLower();
				if (lengthAsString.EndsWith("pt"))
				{
					lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
					if (double.TryParse(lengthAsString, out length))
					{
						length = length * 96.0 / 72.0;
					}
					else
					{
						length = double.NaN;
					}
				}
				else
				{
					if (lengthAsString.EndsWith("px"))
					{
						lengthAsString = lengthAsString.Substring(0, lengthAsString.Length - 2);
						if (!double.TryParse(lengthAsString, out length))
						{
							length = double.NaN;
						}
					}
					else
					{
						if (!double.TryParse(lengthAsString, out length))
						{
							length = double.NaN;
						}
					}
				}
			}
			if (!double.IsNaN(length) && length > 1000000.0)
			{
				length = 1000000.0;
			}
			return !double.IsNaN(length);
		}
		private static string GetColorValue(string colorValue)
		{
			return colorValue;
		}
		private static void ApplyPropertiesToTableCellElement(XmlElement htmlChildNode, XmlElement xamlTableCellElement, Hashtable parentProperties, ArrayList columnStarts)
		{
			string text = (string)parentProperties["border-width-top"];
			string text2 = (string)parentProperties["border-width-right"];
			string text3 = (string)parentProperties["border-width-bottom"];
			string text4 = (string)parentProperties["border-width-left"];
			if (!string.IsNullOrEmpty(text) || !string.IsNullOrEmpty(text2) || !string.IsNullOrEmpty(text3) || !string.IsNullOrEmpty(text4))
			{
				xamlTableCellElement.SetAttribute("BorderThickness", string.Concat(new string[]
				{
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text4 ?? "0"),
					",",
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text ?? "0"),
					",",
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text2 ?? "0"),
					",",
					HtmlToXamlConverter.ConvertCSSFontSizeToXAMLFontSize(text3 ?? "0")
				}));
				string text5 = (string)parentProperties["border-color-top"];
				if (string.IsNullOrEmpty(text5))
				{
					text5 = (string)parentProperties["border-color-right"];
					if (string.IsNullOrEmpty(text5))
					{
						text5 = (string)parentProperties["border-color-bottom"];
						if (string.IsNullOrEmpty(text5))
						{
							text5 = (string)parentProperties["border-color-left"];
						}
					}
				}
				if (!string.IsNullOrEmpty(text5) && HtmlToXamlConverter.IsColorCorrect(text5))
				{
					xamlTableCellElement.SetAttribute("BorderBrush", text5);
				}
				else
				{
					xamlTableCellElement.SetAttribute("BorderBrush", "DarkGray");
				}
			}
			string text6 = (string)parentProperties["background-color"];
			if (string.IsNullOrEmpty(text6))
			{
				text6 = HtmlToXamlConverter.GetAttribute(htmlChildNode, "bgcolor");
			}
			if (!string.IsNullOrEmpty(text6) && HtmlToXamlConverter.IsColorCorrect(text6))
			{
				xamlTableCellElement.SetAttribute("Background", text6);
			}
			string attribute = HtmlToXamlConverter.GetAttribute(htmlChildNode, "rowspan");
			if (attribute != null)
			{
				xamlTableCellElement.SetAttribute("RowSpan", attribute);
			}
			if (columnStarts == null)
			{
				string attribute2 = HtmlToXamlConverter.GetAttribute(htmlChildNode, "colspan");
				if (attribute2 != null)
				{
					xamlTableCellElement.SetAttribute("ColumnSpan", attribute2);
				}
			}
		}
		internal static bool IsColorCorrect(string color)
		{
			try
			{
				HtmlToXamlConverter.brushConverter.ConvertFromString(color);
				return true;
			}
			catch
			{
			}
			return false;
		}
	}
}
