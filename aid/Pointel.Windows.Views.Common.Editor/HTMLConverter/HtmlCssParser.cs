using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

namespace Pointel.Windows.Views.Common.Editor.HTMLConverter
{
	internal static class HtmlCssParser
	{
		private static readonly string[] _colors = new string[]
		{
			"aliceblue",
			"antiquewhite",
			"aqua",
			"aquamarine",
			"azure",
			"beige",
			"bisque",
			"black",
			"blanchedalmond",
			"blue",
			"blueviolet",
			"brown",
			"burlywood",
			"cadetblue",
			"chartreuse",
			"chocolate",
			"coral",
			"cornflowerblue",
			"cornsilk",
			"crimson",
			"cyan",
			"darkblue",
			"darkcyan",
			"darkgoldenrod",
			"darkgray",
			"darkgreen",
			"darkkhaki",
			"darkmagenta",
			"darkolivegreen",
			"darkorange",
			"darkorchid",
			"darkred",
			"darksalmon",
			"darkseagreen",
			"darkslateblue",
			"darkslategray",
			"darkturquoise",
			"darkviolet",
			"deeppink",
			"deepskyblue",
			"dimgray",
			"dodgerblue",
			"firebrick",
			"floralwhite",
			"forestgreen",
			"fuchsia",
			"gainsboro",
			"ghostwhite",
			"gold",
			"goldenrod",
			"gray",
			"green",
			"greenyellow",
			"honeydew",
			"hotpink",
			"indianred",
			"indigo",
			"ivory",
			"khaki",
			"lavender",
			"lavenderblush",
			"lawngreen",
			"lemonchiffon",
			"lightblue",
			"lightcoral",
			"lightcyan",
			"lightgoldenrodyellow",
			"lightgreen",
			"lightgrey",
			"lightpink",
			"lightsalmon",
			"lightseagreen",
			"lightskyblue",
			"lightslategray",
			"lightsteelblue",
			"lightyellow",
			"lime",
			"limegreen",
			"linen",
			"magenta",
			"maroon",
			"mediumaquamarine",
			"mediumblue",
			"mediumorchid",
			"mediumpurple",
			"mediumseagreen",
			"mediumslateblue",
			"mediumspringgreen",
			"mediumturquoise",
			"mediumvioletred",
			"midnightblue",
			"mintcream",
			"mistyrose",
			"moccasin",
			"navajowhite",
			"navy",
			"oldlace",
			"olive",
			"olivedrab",
			"orange",
			"orangered",
			"orchid",
			"palegoldenrod",
			"palegreen",
			"paleturquoise",
			"palevioletred",
			"papayawhip",
			"peachpuff",
			"peru",
			"pink",
			"plum",
			"powderblue",
			"purple",
			"red",
			"rosybrown",
			"royalblue",
			"saddlebrown",
			"salmon",
			"sandybrown",
			"seagreen",
			"seashell",
			"sienna",
			"silver",
			"skyblue",
			"slateblue",
			"slategray",
			"snow",
			"springgreen",
			"steelblue",
			"tan",
			"teal",
			"thistle",
			"tomato",
			"turquoise",
			"violet",
			"wheat",
			"white",
			"whitesmoke",
			"yellow",
			"yellowgreen"
		};
		private static readonly string[] _systemColors = new string[]
		{
			"activeborder",
			"activecaption",
			"appworkspace",
			"background",
			"buttonface",
			"buttonhighlight",
			"buttonshadow",
			"buttontext",
			"captiontext",
			"graytext",
			"highlight",
			"highlighttext",
			"inactiveborder",
			"inactivecaption",
			"inactivecaptiontext",
			"infobackground",
			"infotext",
			"menu",
			"menutext",
			"scrollbar",
			"threeddarkshadow",
			"threedface",
			"threedhighlight",
			"threedlightshadow",
			"threedshadow",
			"window",
			"windowframe",
			"windowtext"
		};
		private static Regex regexRGB = new Regex("^rgb\\(\\s*(?<red>\\d+)\\s*,\\s*(?<green>\\d+)\\s*,\\s*(?<blue>\\d+)\\s*\\)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		private static Regex regexRGBA = new Regex("^rgba\\(\\s*(?<red>\\d+)\\s*,\\s*(?<green>\\d+)\\s*,\\s*(?<blue>\\d+)\\s*,\\s*(?<alpha>\\d+)\\s*\\)$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
		private static readonly string[] _fontGenericFamilies = new string[]
		{
			"serif",
			"sans-serif",
			"monospace",
			"cursive",
			"fantasy"
		};
		private static readonly string[] _fontStyles = new string[]
		{
			"normal",
			"italic",
			"oblique"
		};
		private static readonly string[] _fontVariants = new string[]
		{
			"normal",
			"small-caps"
		};
		private static readonly string[] _fontWeights = new string[]
		{
			"normal",
			"bold",
			"bolder",
			"lighter",
			"100",
			"200",
			"300",
			"400",
			"500",
			"600",
			"700",
			"800",
			"900"
		};
		private static readonly string[] _fontAbsoluteSizes = new string[]
		{
			"xx-small",
			"x-small",
			"small",
			"medium",
			"large",
			"x-large",
			"xx-large"
		};
		private static readonly string[] _fontRelativeSizes = new string[]
		{
			"larger",
			"smaller"
		};
		private static readonly string[] _fontSizeUnits = new string[]
		{
			"px",
			"mm",
			"cm",
			"in",
			"pt",
			"pc",
			"em",
			"ex",
			"%"
		};
		private static readonly string[] _listStyleTypes = new string[]
		{
			"disc",
			"circle",
			"square",
			"decimal",
			"lower-roman",
			"upper-roman",
			"lower-alpha",
			"upper-alpha",
			"none"
		};
		private static readonly string[] _listStylePositions = new string[]
		{
			"inside",
			"outside"
		};
		private static readonly string[] _textDecorations = new string[]
		{
			"none",
			"underline",
			"overline",
			"line-through",
			"blink"
		};
		private static readonly string[] _textTransforms = new string[]
		{
			"none",
			"capitalize",
			"uppercase",
			"lowercase"
		};
		private static readonly string[] _textAligns = new string[]
		{
			"left",
			"right",
			"center",
			"justify"
		};
		private static readonly string[] _verticalAligns = new string[]
		{
			"baseline",
			"sub",
			"super",
			"top",
			"text-top",
			"middle",
			"bottom",
			"text-bottom"
		};
		private static readonly string[] _floats = new string[]
		{
			"left",
			"right",
			"none"
		};
		private static readonly string[] _clears = new string[]
		{
			"none",
			"left",
			"right",
			"both"
		};
		private static readonly string[] _borderStyles = new string[]
		{
			"none",
			"dotted",
			"dashed",
			"solid",
			"double",
			"groove",
			"ridge",
			"inset",
			"outset"
		};
		private static string[] _blocks = new string[]
		{
			"block",
			"inline",
			"list-item",
			"none"
		};
		internal static void GetElementPropertiesFromCssAttributes(XmlElement htmlElement, string elementName, CssStylesheet stylesheet, Hashtable localProperties, List<XmlElement> sourceContext)
		{
			string style = stylesheet.GetStyle(elementName, sourceContext);
			string attribute = HtmlToXamlConverter.GetAttribute(htmlElement, "style");
			string text = (style != null) ? style : null;
			if (attribute != null)
			{
				text = ((text == null) ? attribute : (text + ";" + attribute));
			}
			if (text != null)
			{
				string[] array = text.Split(new char[]
				{
					';'
				});
				for (int i = 0; i < array.Length; i++)
				{
					string[] array2 = array[i].Split(new char[]
					{
						':'
					});
					if (array2.Length == 2)
					{
						string text2 = array2[0].Trim().ToLower();
						string text3 = array2[1].Trim();
						string styleValue = text3.ToLower();
						int num = 0;
						string key;
						switch (key = text2)
						{
							case "font":
								HtmlCssParser.ParseCssFont(text3, localProperties);
								break;
							case "font-family":
								HtmlCssParser.ParseCssFontFamily(text3, ref num, localProperties);
								break;
							case "font-size":
								HtmlCssParser.ParseCssSize(styleValue, ref num, localProperties, "font-size", true);
								break;
							case "font-style":
								HtmlCssParser.ParseCssFontStyle(styleValue, ref num, localProperties);
								break;
							case "font-weight":
								HtmlCssParser.ParseCssFontWeight(styleValue, ref num, localProperties);
								break;
							case "font-variant":
								HtmlCssParser.ParseCssFontVariant(styleValue, ref num, localProperties);
								break;
							case "line-height":
								HtmlCssParser.ParseCssSize(styleValue, ref num, localProperties, "line-height", true);
								break;
							case "color":
								HtmlCssParser.ParseCssColor(styleValue, ref num, localProperties, "color");
								break;
							case "text-decoration":
								HtmlCssParser.ParseCssTextDecoration(styleValue, ref num, localProperties);
								break;
							case "text-transform":
								HtmlCssParser.ParseCssTextTransform(styleValue, ref num, localProperties);
								break;
							case "background-color":
								HtmlCssParser.ParseCssColor(styleValue, ref num, localProperties, "background-color");
								break;
							case "background":
								HtmlCssParser.ParseCssColor(styleValue, ref num, localProperties, "background-color");
								break;
							case "text-align":
								HtmlCssParser.ParseCssTextAlign(styleValue, ref num, localProperties);
								break;
							case "vertical-align":
								HtmlCssParser.ParseCssVerticalAlign(styleValue, ref num, localProperties);
								break;
							case "text-indent":
								HtmlCssParser.ParseCssSize(styleValue, ref num, localProperties, "text-indent", false);
								break;
							case "width":
							case "height":
								HtmlCssParser.ParseCssSize(styleValue, ref num, localProperties, text2, true);
								break;
							case "margin":
								HtmlCssParser.ParseCssRectangleProperty(styleValue, ref num, localProperties, text2);
								break;
							case "margin-top":
							case "margin-right":
							case "margin-bottom":
							case "margin-left":
								HtmlCssParser.ParseCssSize(styleValue, ref num, localProperties, text2, true);
								break;
							case "padding":
								HtmlCssParser.ParseCssRectangleProperty(styleValue, ref num, localProperties, text2);
								break;
							case "padding-top":
							case "padding-right":
							case "padding-bottom":
							case "padding-left":
								HtmlCssParser.ParseCssSize(styleValue, ref num, localProperties, text2, true);
								break;
							case "border":
								HtmlCssParser.ParseCssBorder(styleValue, ref num, localProperties);
								break;
							case "border-style":
							case "border-width":
							case "border-color":
								HtmlCssParser.ParseCssRectangleProperty(styleValue, ref num, localProperties, text2);
								break;
							case "border-top":
							case "border-right":
							case "border-left":
							case "border-bottom":
								HtmlCssParser.ParseCssRectangleSideProperty(styleValue, ref num, localProperties, text2);
								break;
							case "float":
								HtmlCssParser.ParseCssFloat(styleValue, ref num, localProperties);
								break;
							case "clear":
								HtmlCssParser.ParseCssClear(styleValue, ref num, localProperties);
								break;
						}
					}
				}
			}
		}
		private static void ParseWhiteSpace(string styleValue, ref int nextIndex)
		{
			while (nextIndex < styleValue.Length && char.IsWhiteSpace(styleValue[nextIndex]))
			{
				nextIndex++;
			}
		}
		private static bool ParseWord(string word, string styleValue, ref int nextIndex)
		{
			HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
			for (int i = 0; i < word.Length; i++)
			{
				if (nextIndex + i >= styleValue.Length || word[i] != styleValue[nextIndex + i])
				{
					return false;
				}
			}
			if (nextIndex + word.Length < styleValue.Length && char.IsLetterOrDigit(styleValue[nextIndex + word.Length]))
			{
				return false;
			}
			nextIndex += word.Length;
			return true;
		}
		private static string ParseWordEnumeration(string[] words, string styleValue, ref int nextIndex)
		{
			for (int i = 0; i < words.Length; i++)
			{
				if (HtmlCssParser.ParseWord(words[i], styleValue, ref nextIndex))
				{
					return words[i];
				}
			}
			return null;
		}
		private static void ParseWordEnumeration(string[] words, string styleValue, ref int nextIndex, Hashtable localProperties, string attributeName)
		{
			string text = HtmlCssParser.ParseWordEnumeration(words, styleValue, ref nextIndex);
			if (text != null)
			{
				localProperties[attributeName] = text;
			}
		}
		private static string ParseCssBorderSize(string styleValue, ref int nextIndex, bool mustBeNonNegative)
		{
			string text = HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, mustBeNonNegative);
			if (text == null)
			{
				string[] words = new string[]
				{
					"none",
					"hidden",
					"thin",
					"medium",
					"thick"
				};
				string text2 = HtmlCssParser.ParseWordEnumeration(words, styleValue, ref nextIndex);
				string a;
				if (text2 != null && (a = text2) != null)
				{
					if (a == "none" || a == "hidden")
					{
						return "0px";
					}
					if (a == "thin")
					{
						return "1px";
					}
					if (a == "medium")
					{
						return "2px";
					}
					if (a == "thick")
					{
						return "3px";
					}
				}
			}
			return text;
		}
		private static string ParseCssSize(string styleValue, ref int nextIndex, bool mustBeNonNegative)
		{
			HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
			int num = nextIndex;
			if (nextIndex < styleValue.Length && styleValue[nextIndex] == '-')
			{
				nextIndex++;
			}
			if (nextIndex < styleValue.Length)
			{
				if (!char.IsDigit(styleValue[nextIndex]))
				{
					if (styleValue[nextIndex] != '.')
					{
						goto IL_EB;
					}
				}
				while (nextIndex < styleValue.Length && (char.IsDigit(styleValue[nextIndex]) || styleValue[nextIndex] == '.'))
				{
					nextIndex++;
				}
				string str = styleValue.Substring(num, nextIndex - num);
				if ((nextIndex > 0 && styleValue[nextIndex - 1] == '.') || (nextIndex < styleValue.Length && styleValue[nextIndex] == ','))
				{
					str = "0";
				}
				string text = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontSizeUnits, styleValue, ref nextIndex);
				if (text == null)
				{
					text = "px";
				}
				if (mustBeNonNegative && styleValue[num] == '-')
				{
					return "0";
				}
				return str + text;
			}
		IL_EB:
			return null;
		}
		private static void ParseCssSize(string styleValue, ref int nextIndex, Hashtable localValues, string propertyName, bool mustBeNonNegative)
		{
			string text = HtmlCssParser.ParseCssSize(styleValue, ref nextIndex, mustBeNonNegative);
			if (text != null)
			{
				localValues[propertyName] = text;
			}
		}
		private static string ParseCssColor(string styleValue, ref int nextIndex)
		{
			HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
			string text = null;
			if (nextIndex < styleValue.Length)
			{
				int num = nextIndex;
				char c = styleValue[nextIndex];
				if (c == '#')
				{
					for (nextIndex++; nextIndex < styleValue.Length; nextIndex++)
					{
						c = char.ToUpper(styleValue[nextIndex]);
						if (('0' > c || c > '9') && ('A' > c || c > 'F'))
						{
							break;
						}
					}
					if (nextIndex > num + 1)
					{
						text = styleValue.Substring(num, nextIndex - num);
					}
				}
				else
				{
					if (styleValue.Substring(nextIndex, 3).ToLower() == "rgb")
					{
						text = "gray";
						while (nextIndex < styleValue.Length && styleValue[nextIndex] != ')')
						{
							nextIndex++;
						}
						if (nextIndex < styleValue.Length)
						{
							nextIndex++;
						}
						Match match = HtmlCssParser.regexRGB.Match(styleValue);
						if (match.Success)
						{
							int num2 = 128;
							int num3 = 128;
							int num4 = 128;
							if (int.TryParse(match.Groups["red"].ToString(), out num2) && int.TryParse(match.Groups["green"].ToString(), out num3) && int.TryParse(match.Groups["blue"].ToString(), out num4))
							{
								text = string.Format("#{0:X2}{1:X2}{2:X2}", num2, num3, num4);
							}
						}
						else
						{
							match = HtmlCssParser.regexRGBA.Match(styleValue);
							if (match.Success)
							{
								int num5 = 128;
								int num6 = 128;
								int num7 = 128;
								double num8 = 1.0;
								if (int.TryParse(match.Groups["red"].ToString(), out num5) && int.TryParse(match.Groups["green"].ToString(), out num6) && int.TryParse(match.Groups["blue"].ToString(), out num7) && double.TryParse(match.Groups["alpha"].ToString(), out num8))
								{
									text = string.Format("#{3:X2}{0:X2}{1:X2}{2:X2}", new object[]
									{
										num5,
										num6,
										num7,
										Math.Max(255, (int)(num8 * 255.0))
									});
								}
							}
						}
					}
					else
					{
						if (char.IsLetter(c))
						{
							text = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._colors, styleValue, ref nextIndex);
							if (text == null)
							{
								text = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._systemColors, styleValue, ref nextIndex);
								if (text != null)
								{
									text = "black";
								}
							}
						}
					}
				}
			}
			return text;
		}
		private static void ParseCssColor(string styleValue, ref int nextIndex, Hashtable localValues, string propertyName)
		{
			try
			{
				string text = HtmlCssParser.ParseCssColor(styleValue, ref nextIndex);
				if (text != null)
				{
					localValues[propertyName] = text;
				}
			}
			catch
			{
			}
		}
		private static void ParseCssFont(string originalStyleValue, Hashtable localProperties)
		{
			string text = originalStyleValue.ToLower();
			int num = 0;
			HtmlCssParser.ParseCssFontStyle(text, ref num, localProperties);
			HtmlCssParser.ParseCssFontVariant(text, ref num, localProperties);
			HtmlCssParser.ParseCssFontWeight(text, ref num, localProperties);
			HtmlCssParser.ParseCssSize(text, ref num, localProperties, "font-size", true);
			HtmlCssParser.ParseWhiteSpace(text, ref num);
			if (num < text.Length && text[num] == '/')
			{
				num++;
				HtmlCssParser.ParseCssSize(text, ref num, localProperties, "line-height", true);
			}
			HtmlCssParser.ParseCssFontFamily(originalStyleValue, ref num, localProperties);
		}
		private static void ParseCssFontStyle(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontStyles, styleValue, ref nextIndex, localProperties, "font-style");
		}
		private static void ParseCssFontVariant(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontVariants, styleValue, ref nextIndex, localProperties, "font-variant");
		}
		private static void ParseCssFontWeight(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			int num = nextIndex;
			string text = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontWeights, styleValue, ref num);
			if (text != null && HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontSizeUnits, styleValue, ref num) == null)
			{
				nextIndex = num;
				localProperties["font-weight"] = text;
			}
		}
		private static void ParseCssFontFamily(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			string text = null;
			while (nextIndex < styleValue.Length)
			{
				string text2 = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._fontGenericFamilies, styleValue, ref nextIndex);
				if (text2 == null)
				{
					if (nextIndex < styleValue.Length && (styleValue[nextIndex] == '"' || styleValue[nextIndex] == '\''))
					{
						char c = styleValue[nextIndex];
						nextIndex++;
						int num = nextIndex;
						while (nextIndex < styleValue.Length && styleValue[nextIndex] != c)
						{
							nextIndex++;
						}
						text2 = '"' + styleValue.Substring(num, nextIndex - num) + '"';
						nextIndex++;
					}
					if (text2 == null)
					{
						int num2 = nextIndex;
						while (nextIndex < styleValue.Length && styleValue[nextIndex] != ',' && styleValue[nextIndex] != ';')
						{
							nextIndex++;
						}
						if (nextIndex > num2)
						{
							text2 = styleValue.Substring(num2, nextIndex - num2).Trim();
							if (text2.Length == 0)
							{
								text2 = null;
							}
						}
					}
				}
				HtmlCssParser.ParseWhiteSpace(styleValue, ref nextIndex);
				if (nextIndex < styleValue.Length && styleValue[nextIndex] == ',')
				{
					nextIndex++;
				}
				if (text2 == null)
				{
					break;
				}
				if (text == null && text2.Length > 0)
				{
					if (text2[0] == '"' || text2[0] == '\'')
					{
						text2 = text2.Substring(1, text2.Length - 2);
					}
					text = text2;
				}
			}
			if (text != null)
			{
				localProperties["font-family"] = text;
			}
		}
		private static void ParseCssListStyle(string styleValue, Hashtable localProperties)
		{
			int i = 0;
			while (i < styleValue.Length)
			{
				string text = HtmlCssParser.ParseCssListStyleType(styleValue, ref i);
				if (text != null)
				{
					localProperties["list-style-type"] = text;
				}
				else
				{
					string text2 = HtmlCssParser.ParseCssListStylePosition(styleValue, ref i);
					if (text2 != null)
					{
						localProperties["list-style-position"] = text2;
					}
					else
					{
						string text3 = HtmlCssParser.ParseCssListStyleImage(styleValue, ref i);
						if (text3 == null)
						{
							break;
						}
						localProperties["list-style-image"] = text3;
					}
				}
			}
		}
		private static string ParseCssListStyleType(string styleValue, ref int nextIndex)
		{
			return HtmlCssParser.ParseWordEnumeration(HtmlCssParser._listStyleTypes, styleValue, ref nextIndex);
		}
		private static string ParseCssListStylePosition(string styleValue, ref int nextIndex)
		{
			return HtmlCssParser.ParseWordEnumeration(HtmlCssParser._listStylePositions, styleValue, ref nextIndex);
		}
		private static string ParseCssListStyleImage(string styleValue, ref int nextIndex)
		{
			return null;
		}
		private static void ParseCssTextDecoration(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			for (int i = 1; i < HtmlCssParser._textDecorations.Length; i++)
			{
				localProperties["text-decoration-" + HtmlCssParser._textDecorations[i]] = "false";
			}
			while (nextIndex < styleValue.Length)
			{
				string text = HtmlCssParser.ParseWordEnumeration(HtmlCssParser._textDecorations, styleValue, ref nextIndex);
				if (text == null)
				{
					break;
				}
				if (text == "none")
				{
					return;
				}
				localProperties["text-decoration-" + text] = "true";
			}
		}
		private static void ParseCssTextTransform(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			HtmlCssParser.ParseWordEnumeration(HtmlCssParser._textTransforms, styleValue, ref nextIndex, localProperties, "text-transform");
		}
		private static void ParseCssTextAlign(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			HtmlCssParser.ParseWordEnumeration(HtmlCssParser._textAligns, styleValue, ref nextIndex, localProperties, "text-align");
		}
		private static void ParseCssVerticalAlign(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			HtmlCssParser.ParseWordEnumeration(HtmlCssParser._verticalAligns, styleValue, ref nextIndex, localProperties, "vertical-align");
		}
		private static void ParseCssFloat(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			HtmlCssParser.ParseWordEnumeration(HtmlCssParser._floats, styleValue, ref nextIndex, localProperties, "float");
		}
		private static void ParseCssClear(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			HtmlCssParser.ParseWordEnumeration(HtmlCssParser._clears, styleValue, ref nextIndex, localProperties, "clear");
		}
		private static bool ParseCssRectangleProperty(string styleValue, ref int nextIndex, Hashtable localProperties, string propertyName)
		{
			styleValue = styleValue.Replace("em", "").Replace("EM", "");
			string text = (propertyName == "border-color") ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : ((propertyName == "border-style") ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssBorderSize(styleValue, ref nextIndex, true));
			if (text != null)
			{
				localProperties[propertyName + "-top"] = text;
				localProperties[propertyName + "-bottom"] = text;
				localProperties[propertyName + "-right"] = text;
				localProperties[propertyName + "-left"] = text;
				text = ((propertyName == "border-color") ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : ((propertyName == "border-style") ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssBorderSize(styleValue, ref nextIndex, true)));
				if (text != null)
				{
					localProperties[propertyName + "-right"] = text;
					localProperties[propertyName + "-left"] = text;
					text = ((propertyName == "border-color") ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : ((propertyName == "border-style") ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssBorderSize(styleValue, ref nextIndex, true)));
					if (text != null)
					{
						localProperties[propertyName + "-bottom"] = text;
						text = ((propertyName == "border-color") ? HtmlCssParser.ParseCssColor(styleValue, ref nextIndex) : ((propertyName == "border-style") ? HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex) : HtmlCssParser.ParseCssBorderSize(styleValue, ref nextIndex, true)));
						if (text != null)
						{
							localProperties[propertyName + "-left"] = text;
						}
					}
				}
				return true;
			}
			return false;
		}
		private static void ParseCssBorder(string styleValue, ref int nextIndex, Hashtable localProperties)
		{
			while (HtmlCssParser.ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-width") || HtmlCssParser.ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-style") || HtmlCssParser.ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-color") || HtmlCssParser.ParseCssRectangleProperty(styleValue, ref nextIndex, localProperties, "border-width"))
			{
			}
		}
		private static bool ParseCssRectangleSideProperty(string styleValue, ref int nextIndex, Hashtable localProperties, string propertyName)
		{
			styleValue = styleValue.Replace("em", "").Replace("EM", "");
			int num = propertyName.LastIndexOf('-');
			if (num >= 0)
			{
				string str = propertyName.Substring(num);
				bool flag = false;
				bool flag2 = false;
				string text = HtmlCssParser.ParseCssBorderSize(styleValue, ref nextIndex, true);
				if (text != null)
				{
					localProperties["border-width" + str] = text;
					flag = true;
				}
				string text2 = HtmlCssParser.ParseCssBorderStyle(styleValue, ref nextIndex);
				if (text2 != null)
				{
					localProperties["border-style" + str] = text2;
					if (text2 != "none")
					{
						flag2 = true;
					}
				}
				string text3 = HtmlCssParser.ParseCssColor(styleValue, ref nextIndex);
				if (text3 != null)
				{
					localProperties["border-color" + str] = text3;
				}
				text = HtmlCssParser.ParseCssBorderSize(styleValue, ref nextIndex, true);
				if (text != null)
				{
					localProperties["border-width" + str] = text;
					flag = true;
				}
				if (flag || !flag2)
				{
				}
				return true;
			}
			return false;
		}
		private static string ParseCssBorderStyle(string styleValue, ref int nextIndex)
		{
			return HtmlCssParser.ParseWordEnumeration(HtmlCssParser._borderStyles, styleValue, ref nextIndex);
		}
		private static void ParseCssBackground(string styleValue, ref int nextIndex, Hashtable localValues)
		{
		}
	}
}
