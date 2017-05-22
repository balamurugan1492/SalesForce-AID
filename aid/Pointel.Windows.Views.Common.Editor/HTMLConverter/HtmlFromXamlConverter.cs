using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Pointel.Windows.Views.Common.Editor.HTMLConverter
{
	internal static class HtmlFromXamlConverter
	{
		private static Regex regexXAMLToolTip1 = new Regex("^(.*)\r\n\\[(http.*)\\]$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		private static Regex regexXAMLToolTip2 = new Regex("^\\[(http.*)\\]$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		private static Regex regexXAMLLineBreak = new Regex("(<Paragraph[^<>]*>(<Span[^<>]*>)*<Run[^<>]*>)(</Run>(</Span>)*</Paragraph>)", RegexOptions.Singleline);
		internal static string ConvertXamlToHtml(string xamlString)
		{
			string result = "";
			xamlString = HtmlFromXamlConverter.regexXAMLLineBreak.Replace(xamlString, "$1&nbsp;$3");
			xamlString = HtmlFromXamlConverter.regexXAMLLineBreak.Replace(xamlString, "$1&nbsp;$3");
			using (StringReader stringReader = new StringReader(xamlString))
			{
				if (stringReader != null)
				{
					XmlTextReader xmlTextReader = new XmlTextReader(stringReader);
					if (xmlTextReader != null)
					{
						StringBuilder stringBuilder = new StringBuilder(100);
						if (stringBuilder != null)
						{
							using (StringWriter stringWriter = new StringWriter(stringBuilder))
							{
								if (stringWriter != null)
								{
									XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
									if (xmlTextWriter != null)
									{
										if (!HtmlFromXamlConverter.WriteFlowDocument(xmlTextReader, xmlTextWriter))
										{
											return "";
										}
										stringBuilder.Replace("<SPAN />", "");
										result = stringBuilder.ToString();
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
		private static bool WriteFlowDocument(XmlTextReader xamlReader, XmlTextWriter htmlWriter)
		{
			if (!HtmlFromXamlConverter.ReadNextToken(xamlReader))
			{
				return false;
			}
			if (xamlReader.NodeType != XmlNodeType.Element || xamlReader.Name != "FlowDocument")
			{
				return false;
			}
			StringBuilder inlineStyle = new StringBuilder();
			htmlWriter.WriteStartElement("HTML");
			htmlWriter.WriteStartElement("BODY");
			HtmlFromXamlConverter.WriteFormattingProperties(xamlReader, htmlWriter, inlineStyle, "BODY");
			HtmlFromXamlConverter.WriteElementContent(xamlReader, htmlWriter, inlineStyle, new List<string>(), new List<string>());
			htmlWriter.WriteEndElement();
			htmlWriter.WriteEndElement();
			return true;
		}
		private static void WriteFormattingProperties(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, string currentHtmlElementName)
		{
			inlineStyle.Remove(0, inlineStyle.Length);
			if (!xamlReader.HasAttributes)
			{
				return;
			}
			bool flag = false;
			string text = null;
			double[] array = null;
			while (xamlReader.MoveToNextAttribute())
			{
				string text2 = null;
				string name;
				switch (name = xamlReader.Name)
				{
					case "Background":
						text2 = "background-color:" + HtmlFromXamlConverter.ParseXamlColor(xamlReader.Value) + ";";
						break;
					case "FontFamily":
						text2 = "font-family:" + xamlReader.Value + ";";
						break;
					case "FontStyle":
						text2 = "font-style:" + xamlReader.Value.ToLower() + ";";
						break;
					case "FontWeight":
						text2 = "font-weight:" + xamlReader.Value.ToLower() + ";";
						break;
					case "FontSize":
						text2 = "font-size:" + xamlReader.Value + "px;";
						break;
					case "Foreground":
						text2 = "color:" + HtmlFromXamlConverter.ParseXamlColor(xamlReader.Value) + ";";
						break;
					case "TextDecorations":
						if (xamlReader.Value != null && xamlReader.Value.IndexOf("Underline", StringComparison.InvariantCultureIgnoreCase) != -1)
						{
							text2 = "text-decoration:underline;";
						}
						break;
					case "BaselineAlignment":
						if (xamlReader.Value != null)
						{
							string value = xamlReader.Value;
							if (value == "Subscript")
							{
								text2 = "vertical-align:sub;";
							}
							else
							{
								if (value == "Superscript")
								{
									text2 = "vertical-align:super;";
								}
							}
						}
						break;
					case "Padding":
						text2 = "padding:" + HtmlFromXamlConverter.ParseXamlThickness(xamlReader.Value) + ";";
						break;
					case "Margin":
						text2 = "margin:" + HtmlFromXamlConverter.ParseXamlThickness(xamlReader.Value) + ";";
						break;
					case "BorderThickness":
						array = HtmlFromXamlConverter.ParseXamlBorderThickness(xamlReader.Value);
						flag = true;
						break;
					case "BorderBrush":
						text = HtmlFromXamlConverter.ParseXamlColor(xamlReader.Value);
						flag = true;
						break;
					case "TextIndent":
						text2 = "text-indent:" + xamlReader.Value + ";";
						break;
					case "TextAlignment":
						text2 = "text-align:" + xamlReader.Value + ";";
						break;
					case "Width":
						text2 = "width:" + xamlReader.Value + "px;";
						break;
					case "ColumnSpan":
						htmlWriter.WriteAttributeString("COLSPAN", xamlReader.Value);
						break;
					case "RowSpan":
						htmlWriter.WriteAttributeString("ROWSPAN", xamlReader.Value);
						break;
					case "CellSpacing":
						{
							htmlWriter.WriteAttributeString("CELLSPACING", xamlReader.Value);
							int num2 = 0;
							if (int.TryParse(xamlReader.Value, out num2) && num2 == 0)
							{
								text2 = "border-collapse:collapse;";
								htmlWriter.WriteAttributeString("BORDER", "0");
							}
							break;
						}
					case "NavigateUri":
						htmlWriter.WriteAttributeString("HREF", xamlReader.Value);
						break;
					case "ToolTip":
						if (currentHtmlElementName != null)
						{
							if (!(currentHtmlElementName == "IMG"))
							{
								if (currentHtmlElementName == "ABBR" || currentHtmlElementName == "ACRONYM")
								{
									string text3 = xamlReader.Value;
									if (text3 != null)
									{
										if (text3.Length > 0)
										{
											text3 = text3.Substring(0, text3.Length - 1);
										}
										htmlWriter.WriteAttributeString("TITLE", text3);
									}
								}
							}
							else
							{
								string value2 = "";
								string value3 = "";
								Match match = HtmlFromXamlConverter.regexXAMLToolTip1.Match(xamlReader.Value);
								if (match.Success)
								{
									value2 = match.Groups[1].ToString();
									value3 = match.Groups[2].ToString();
								}
								else
								{
									match = HtmlFromXamlConverter.regexXAMLToolTip2.Match(xamlReader.Value);
									if (match.Success)
									{
										value3 = match.Groups[1].ToString();
									}
								}
								if (!string.IsNullOrEmpty(value2))
								{
									htmlWriter.WriteAttributeString("ALT", value2);
								}
								if (!string.IsNullOrEmpty(value3))
								{
									htmlWriter.WriteAttributeString("SRC", value3);
								}
							}
						}
						break;
				}
				if (text2 != null)
				{
					inlineStyle.Append(text2);
				}
			}
			if (flag)
			{
				if (string.IsNullOrEmpty(text))
				{
					text = "#000000";
				}
				int num3 = 0;
				if (array != null)
				{
					num3 = array.Length;
				}
				switch (num3)
				{
					case 2:
						inlineStyle.Append(string.Concat(new string[]
					{
						"border-top:",
						HtmlFromXamlConverter.GetCSSBorder(array[1], text),
						"border-right:",
						HtmlFromXamlConverter.GetCSSBorder(array[0], text),
						"border-bottom:",
						HtmlFromXamlConverter.GetCSSBorder(array[1], text),
						"border-left:",
						HtmlFromXamlConverter.GetCSSBorder(array[0], text)
					}));
						goto IL_775;
					case 4:
						inlineStyle.Append(string.Concat(new string[]
					{
						"border-top:",
						HtmlFromXamlConverter.GetCSSBorder(array[1], text),
						"border-right:",
						HtmlFromXamlConverter.GetCSSBorder(array[2], text),
						"border-bottom:",
						HtmlFromXamlConverter.GetCSSBorder(array[3], text),
						"border-left:",
						HtmlFromXamlConverter.GetCSSBorder(array[0], text)
					}));
						goto IL_775;
				}
				inlineStyle.Append("border:" + HtmlFromXamlConverter.GetCSSBorder(array[0], text));
			}
		IL_775:
			xamlReader.MoveToElement();
		}
		private static string GetCSSBorder(double borderWidth, string color)
		{
			if (borderWidth <= 0.0)
			{
				return "none;";
			}
			return borderWidth.ToString("F1") + "px solid " + color + ";";
		}
		private static string ParseXamlColor(string color)
		{
			if (color.StartsWith("#"))
			{
				color = "#" + color.Substring(3);
			}
			return color;
		}
		private static string ParseXamlThickness(string thickness)
		{
			string[] array = thickness.Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				double a;
				if (double.TryParse(array[i], out a))
				{
					array[i] = Math.Ceiling(a).ToString();
				}
				else
				{
					array[i] = "1";
				}
			}
			string result;
			switch (array.Length)
			{
				case 1:
					result = thickness + "px";
					return result;
				case 2:
					result = array[1] + "px " + array[0] + "px";
					return result;
				case 4:
					result = string.Concat(new string[]
				{
					array[1],
					"px ",
					array[2],
					"px ",
					array[3],
					"px ",
					array[0],
					"px"
				});
					return result;
			}
			result = array[0];
			return result;
		}
		private static double[] ParseXamlBorderThickness(string thickness)
		{
			string[] array = thickness.Split(new char[]
			{
				','
			});
			double[] array2 = new double[array.Length];
			for (int i = 0; i < array.Length; i++)
			{
				double num;
				if (double.TryParse(array[i], out num))
				{
					array2[i] = num;
				}
				else
				{
					array2[i] = 1.0;
				}
			}
			return array2;
		}
		private static void WriteElementContent(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, List<string> xamlAncestors, List<string> htmlAncestors)
		{
			bool flag = false;
			if (xamlReader.IsEmptyElement)
			{
				if (htmlWriter != null && !flag && inlineStyle.Length > 0)
				{
					htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
					inlineStyle.Remove(0, inlineStyle.Length);
				}
				return;
			}
			while (HtmlFromXamlConverter.ReadNextToken(xamlReader) && xamlReader.NodeType != XmlNodeType.EndElement)
			{
				XmlNodeType nodeType = xamlReader.NodeType;
				switch (nodeType)
				{
					case XmlNodeType.Element:
						if (xamlReader.Name.Contains(".") && xamlReader.Name != "Table.Columns")
						{
							HtmlFromXamlConverter.AddComplexProperty(xamlReader, inlineStyle, xamlAncestors, htmlAncestors);
							continue;
						}
						if (htmlWriter != null && !flag && inlineStyle.Length > 0)
						{
							htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
							inlineStyle.Remove(0, inlineStyle.Length);
						}
						flag = true;
						HtmlFromXamlConverter.WriteElement(xamlReader, htmlWriter, inlineStyle, xamlAncestors, htmlAncestors);
						continue;
					case XmlNodeType.Attribute:
					case XmlNodeType.Entity:
					case XmlNodeType.ProcessingInstruction:
						continue;
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
						break;
					case XmlNodeType.EntityReference:
						if (htmlWriter != null)
						{
							htmlWriter.WriteRaw("&nbsp;");
						}
						flag = true;
						continue;
					case XmlNodeType.Comment:
						if (htmlWriter != null)
						{
							if (!flag && inlineStyle.Length > 0)
							{
								htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
							}
							htmlWriter.WriteComment(xamlReader.Value);
						}
						flag = true;
						continue;
					default:
						if (nodeType != XmlNodeType.SignificantWhitespace)
						{
							continue;
						}
						break;
				}
				if (htmlWriter != null)
				{
					if (!flag && inlineStyle.Length > 0)
					{
						htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
					}
					if (xamlReader.NodeType == XmlNodeType.SignificantWhitespace)
					{
						for (int i = 0; i < xamlReader.Value.Length; i++)
						{
							if (xamlReader.Value[i] == ' ')
							{
								htmlWriter.WriteRaw("&nbsp;");
							}
							else
							{
								htmlWriter.WriteString(xamlReader.Value[i].ToString());
							}
						}
					}
					else
					{
						htmlWriter.WriteString(xamlReader.Value);
					}
				}
				flag = true;
			}
		}
		private static void AddComplexProperty(XmlTextReader xamlReader, StringBuilder inlineStyle, List<string> xamlAncestors, List<string> htmlAncestors)
		{
			if (inlineStyle != null && xamlReader.Name.EndsWith(".TextDecorations"))
			{
				HtmlFromXamlConverter.WriteElementContent(xamlReader, null, inlineStyle, xamlAncestors, htmlAncestors);
				return;
			}
			HtmlFromXamlConverter.WriteElementContent(xamlReader, null, null, xamlAncestors, htmlAncestors);
		}
		private static void WriteElement(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, List<string> xamlAncestors, List<string> htmlAncestors)
		{
			if (htmlWriter == null)
			{
				if (inlineStyle != null && xamlReader.Name == "TextDecoration")
				{
					string attribute = xamlReader.GetAttribute("Location");
					string a;
					if ((a = attribute) != null)
					{
						if (!(a == "Underline"))
						{
							if (!(a == "OverLine"))
							{
								if (a == "Strikethrough")
								{
									inlineStyle.Append("text-decoration:line-through;");
								}
							}
							else
							{
								inlineStyle.Append("text-decoration:overline;");
							}
						}
						else
						{
							inlineStyle.Append("text-decoration:underline;");
						}
					}
				}
				HtmlFromXamlConverter.WriteElementContent(xamlReader, null, null, xamlAncestors, htmlAncestors);
				return;
			}
			string name;
			string text;
			switch (name = xamlReader.Name)
			{
				case "Run":
				case "Span":
					text = "SPAN";
					goto IL_38A;
				case "InlineUIContainer":
					text = "SPAN";
					goto IL_38A;
				case "Bold":
					text = "B";
					goto IL_38A;
				case "Italic":
					text = "I";
					goto IL_38A;
				case "Paragraph":
					if (xamlAncestors.Count > 0 && xamlAncestors[xamlAncestors.Count - 1] == "ListItem")
					{
						text = "SPAN";
						goto IL_38A;
					}
					text = "P";
					goto IL_38A;
				case "BlockUIContainer":
					text = "DIV";
					goto IL_38A;
				case "Section":
					text = "DIV";
					goto IL_38A;
				case "Table":
					text = "TABLE";
					goto IL_38A;
				case "Table.Columns":
					text = "COLGROUP";
					goto IL_38A;
				case "TableColumn":
					text = "COL";
					goto IL_38A;
				case "TableRowGroup":
					text = "TBODY";
					goto IL_38A;
				case "TableRow":
					text = "TR";
					goto IL_38A;
				case "TableCell":
					text = "TD";
					goto IL_38A;
				case "List":
					{
						string attribute2 = xamlReader.GetAttribute("MarkerStyle");
						if (attribute2 == null || attribute2 == "Disc" || attribute2 == "Circle" || attribute2 == "Square" || attribute2 == "Box")
						{
							text = "UL";
							goto IL_38A;
						}
						if (attribute2 == null || attribute2 == "None")
						{
							text = "DL";
							goto IL_38A;
						}
						text = "OL";
						goto IL_38A;
					}
				case "ListItem":
					if (htmlAncestors.Count > 0 && htmlAncestors[htmlAncestors.Count - 1] == "DL")
					{
						text = "DD";
						goto IL_38A;
					}
					text = "LI";
					goto IL_38A;
				case "LineBreak":
					text = "BR";
					goto IL_38A;
				case "Hyperlink":
					text = "A";
					goto IL_38A;
				case "Image":
					text = "IMG";
					goto IL_38A;
			}
			text = null;
		IL_38A:
			xamlAncestors.Add(xamlReader.Name);
			if (text != null)
			{
				htmlAncestors.Add(text);
			}
			if (htmlWriter != null && text != null)
			{
				htmlWriter.WriteStartElement(text);
				HtmlFromXamlConverter.WriteFormattingProperties(xamlReader, htmlWriter, inlineStyle, text);
				if (text == "IMG")
				{
					string attribute3 = xamlReader.GetAttribute("Width");
					if (!string.IsNullOrEmpty(attribute3))
					{
						htmlWriter.WriteAttributeString("WIDTH", attribute3);
					}
					string attribute4 = xamlReader.GetAttribute("Height");
					if (!string.IsNullOrEmpty(attribute4))
					{
						htmlWriter.WriteAttributeString("HEIGHT", attribute4);
					}
				}
				else
				{
					if (text == "COL")
					{
						inlineStyle.Length = 0;
						string attribute5 = xamlReader.GetAttribute("Width");
						if (!string.IsNullOrEmpty(attribute5))
						{
							htmlWriter.WriteAttributeString("WIDTH", attribute5);
						}
					}
				}
				HtmlFromXamlConverter.WriteElementContent(xamlReader, htmlWriter, inlineStyle, xamlAncestors, htmlAncestors);
				htmlWriter.WriteEndElement();
			}
			else
			{
				HtmlFromXamlConverter.WriteElementContent(xamlReader, null, null, xamlAncestors, htmlAncestors);
			}
			xamlAncestors.RemoveAt(xamlAncestors.Count - 1);
			if (text != null)
			{
				htmlAncestors.RemoveAt(htmlAncestors.Count - 1);
			}
		}
		private static bool ReadNextToken(XmlReader xamlReader)
		{
			while (xamlReader.Read())
			{
				switch (xamlReader.NodeType)
				{
					case XmlNodeType.None:
					case XmlNodeType.Element:
					case XmlNodeType.Text:
					case XmlNodeType.CDATA:
					case XmlNodeType.SignificantWhitespace:
					case XmlNodeType.EndElement:
						return true;
					case XmlNodeType.EntityReference:
					case XmlNodeType.EndEntity:
						if (xamlReader.Name == "nbsp")
						{
							return true;
						}
						break;
					case XmlNodeType.Comment:
						return true;
					case XmlNodeType.Whitespace:
						if (xamlReader.XmlSpace == XmlSpace.Preserve)
						{
							return true;
						}
						break;
				}
			}
			return false;
		}
	}
}
