using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace Pointel.Windows.Views.Common.Editor.HTMLConverter
{
	internal class HtmlParser
	{
		internal const string HtmlHeader = "Version:1.0\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\nStartSelection:{4:D10}\r\nEndSelection:{5:D10}\r\n";
		internal const string HtmlStartFragmentComment = "<!--StartFragment-->";
		internal const string HtmlEndFragmentComment = "<!--EndFragment-->";
		internal const string XhtmlNamespace = "http://www.w3.org/1999/xhtml";
		private HtmlLexicalAnalyzer _htmlLexicalAnalyzer;
		private XmlDocument _document;
		private Stack<XmlElement> _openedElements;
		private Stack<XmlElement> _pendingInlineElements;
		private HtmlParser(string inputString)
		{
			this._document = new XmlDocument();
			this._openedElements = new Stack<XmlElement>();
			this._pendingInlineElements = new Stack<XmlElement>();
			this._htmlLexicalAnalyzer = new HtmlLexicalAnalyzer(inputString);
			this._htmlLexicalAnalyzer.GetNextContentToken();
		}
		internal static XmlElement ParseHtml(string htmlString)
		{
			HtmlParser htmlParser = new HtmlParser(htmlString);
			return htmlParser.ParseHtmlContent();
		}
		internal static string ExtractHtmlFromClipboardData(string htmlDataString)
		{
			int num = htmlDataString.IndexOf("StartHTML:");
			if (num < 0)
			{
				return "ERROR: Urecognized html header";
			}
			num = int.Parse(htmlDataString.Substring(num + "StartHTML:".Length, "0123456789".Length));
			if (num < 0 || num > htmlDataString.Length)
			{
				return "ERROR: Urecognized html header";
			}
			int num2 = htmlDataString.IndexOf("EndHTML:");
			if (num2 < 0)
			{
				return "ERROR: Urecognized html header";
			}
			num2 = int.Parse(htmlDataString.Substring(num2 + "EndHTML:".Length, "0123456789".Length));
			if (num2 > htmlDataString.Length)
			{
				num2 = htmlDataString.Length;
			}
			return htmlDataString.Substring(num, num2 - num);
		}
		internal static string AddHtmlClipboardHeader(string htmlString)
		{
			StringBuilder stringBuilder = new StringBuilder();
			int num = "Version:1.0\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\nStartSelection:{4:D10}\r\nEndSelection:{5:D10}\r\n".Length + 6 * ("0123456789".Length - "{0:D10}".Length);
			int num2 = num + htmlString.Length;
			int num3 = htmlString.IndexOf("<!--StartFragment-->", 0);
			if (num3 >= 0)
			{
				num3 = num + num3 + "<!--StartFragment-->".Length;
			}
			else
			{
				num3 = num;
			}
			int num4 = htmlString.IndexOf("<!--EndFragment-->", 0);
			if (num4 >= 0)
			{
				num4 = num + num4;
			}
			else
			{
				num4 = num2;
			}
			stringBuilder.AppendFormat("Version:1.0\r\nStartHTML:{0:D10}\r\nEndHTML:{1:D10}\r\nStartFragment:{2:D10}\r\nEndFragment:{3:D10}\r\nStartSelection:{4:D10}\r\nEndSelection:{5:D10}\r\n", new object[]
			{
				num,
				num2,
				num3,
				num4,
				num3,
				num4
			});
			stringBuilder.Append(htmlString);
			return stringBuilder.ToString();
		}
		private void InvariantAssert(bool condition, string message)
		{
			if (!condition)
			{
				throw new Exception("Assertion error: " + message);
			}
		}
		private XmlElement ParseHtmlContent()
		{
			XmlElement xmlElement = this._document.CreateElement("html", "http://www.w3.org/1999/xhtml");
			this.OpenStructuringElement(xmlElement);
			while (this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF)
			{
				if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.OpeningTagStart)
				{
					this._htmlLexicalAnalyzer.GetNextTagToken();
					if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
					{
						string text = this._htmlLexicalAnalyzer.NextToken.ToLower();
						this._htmlLexicalAnalyzer.GetNextTagToken();
						XmlElement xmlElement2 = this._document.CreateElement(text, "http://www.w3.org/1999/xhtml");
						this.ParseAttributes(xmlElement2);
						if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.EmptyTagEnd || HtmlSchema.IsEmptyElement(text))
						{
							if (HtmlSchema.IsInlineElement(text))
							{
								this.AddEmptyInlineElement(xmlElement2);
							}
							else
							{
								this.AddEmptyElement(xmlElement2);
							}
						}
						else
						{
							if (HtmlSchema.IsInlineElement(text))
							{
								this.OpenInlineElement(xmlElement2);
								this._htmlLexicalAnalyzer.IgnoreNextWhitespace = false;
							}
							else
							{
								if (HtmlSchema.IsBlockElement(text) || HtmlSchema.IsKnownOpenableElement(text))
								{
									this.OpenStructuringElement(xmlElement2);
								}
							}
						}
					}
				}
				else
				{
					if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.ClosingTagStart)
					{
						this._htmlLexicalAnalyzer.GetNextTagToken();
						if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
						{
							string htmlElementName = this._htmlLexicalAnalyzer.NextToken.ToLower();
							this._htmlLexicalAnalyzer.GetNextTagToken();
							this.CloseElement(htmlElementName);
						}
					}
					else
					{
						if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Text)
						{
							this.AddTextContent(this._htmlLexicalAnalyzer.NextToken);
						}
						else
						{
							if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Comment)
							{
								this.AddComment(this._htmlLexicalAnalyzer.NextToken);
							}
						}
					}
				}
				this._htmlLexicalAnalyzer.GetNextContentToken();
			}
			if (xmlElement.FirstChild is XmlElement && xmlElement.FirstChild == xmlElement.LastChild && xmlElement.FirstChild.LocalName.ToLower() == "html")
			{
				xmlElement = (xmlElement.FirstChild as XmlElement);
			}
			return xmlElement;
		}
		private XmlElement CreateElementCopy(XmlElement htmlElement)
		{
			XmlElement xmlElement = this._document.CreateElement(htmlElement.LocalName, "http://www.w3.org/1999/xhtml");
			for (int i = 0; i < htmlElement.Attributes.Count; i++)
			{
				XmlAttribute xmlAttribute = htmlElement.Attributes[i];
				xmlElement.SetAttribute(xmlAttribute.Name, xmlAttribute.Value);
			}
			return xmlElement;
		}
		private void OpenInlineElement(XmlElement htmlInlineElement)
		{
			if (htmlInlineElement.LocalName == "a" && ((this._pendingInlineElements.Count > 0 && this.IsElementPending("a")) || (this._openedElements.Count > 0 && this.IsElementOpened("a"))))
			{
				return;
			}
			this._pendingInlineElements.Push(htmlInlineElement);
		}
		private void OpenStructuringElement(XmlElement htmlElement)
		{
			if ((htmlElement.LocalName == "thead" || htmlElement.LocalName == "tfoot" || htmlElement.LocalName == "tbody" || htmlElement.LocalName == "tr" || htmlElement.LocalName == "td") && (this._openedElements.Count == 0 || !this.IsElementOpened("table")))
			{
				return;
			}
			if (HtmlSchema.IsBlockElement(htmlElement.LocalName))
			{
				while (this._openedElements.Count > 0 && HtmlSchema.IsInlineElement(this._openedElements.Peek().LocalName))
				{
					XmlElement htmlElement2 = this._openedElements.Pop();
					this.InvariantAssert(this._openedElements.Count > 0, "OpenStructuringElement: stack of opened elements cannot become empty here");
					this._pendingInlineElements.Push(this.CreateElementCopy(htmlElement2));
				}
			}
			if (this._openedElements.Count > 0)
			{
				XmlElement xmlElement = this._openedElements.Peek();
				if (HtmlSchema.ClosesOnNextElementStart(xmlElement.LocalName, htmlElement.LocalName))
				{
					if (htmlElement.LocalName == "tr")
					{
						while (xmlElement != null && xmlElement.LocalName != "table" && xmlElement.LocalName != "thead" && xmlElement.LocalName != "tfoot")
						{
							if (!(xmlElement.LocalName != "tbody"))
							{
								break;
							}
							this._openedElements.Pop();
							xmlElement = ((this._openedElements.Count > 0) ? this._openedElements.Peek() : null);
						}
					}
					else
					{
						if (!(htmlElement.LocalName == "thead") && !(htmlElement.LocalName == "tfoot"))
						{
							if (!(htmlElement.LocalName == "tbody"))
							{
								this._openedElements.Pop();
								xmlElement = ((this._openedElements.Count > 0) ? this._openedElements.Peek() : null);
								goto IL_250;
							}
						}
						while (xmlElement != null)
						{
							if (!(xmlElement.LocalName != "table"))
							{
								break;
							}
							this._openedElements.Pop();
							xmlElement = ((this._openedElements.Count > 0) ? this._openedElements.Peek() : null);
						}
					}
				}
			IL_250:
				if (xmlElement != null)
				{
					xmlElement.AppendChild(htmlElement);
				}
			}
			this._openedElements.Push(htmlElement);
		}
		private bool IsElementOpened(string htmlElementName)
		{
			foreach (XmlElement current in this._openedElements)
			{
				if (current.LocalName == htmlElementName)
				{
					return true;
				}
			}
			return false;
		}
		private bool IsElementPending(string htmlElementName)
		{
			foreach (XmlElement current in this._pendingInlineElements)
			{
				if (current.LocalName == htmlElementName)
				{
					return true;
				}
			}
			return false;
		}
		private void CloseElement(string htmlElementName)
		{
			this.InvariantAssert(this._openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
			if (this._pendingInlineElements.Count > 0 && this._pendingInlineElements.Peek().LocalName == htmlElementName)
			{
				XmlElement newChild = this._pendingInlineElements.Pop();
				this.InvariantAssert(this._openedElements.Count > 0, "CloseElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
				XmlElement xmlElement = this._openedElements.Peek();
				xmlElement.AppendChild(newChild);
				return;
			}
			if (htmlElementName == "a" && this._pendingInlineElements.Count > 0 && this.IsElementPending(htmlElementName))
			{
				Stack<XmlElement> stack = new Stack<XmlElement>();
				while (this._pendingInlineElements.Peek() != null && this._pendingInlineElements.Peek().LocalName != htmlElementName)
				{
					stack.Push(this._pendingInlineElements.Pop());
				}
				XmlElement xmlElement2 = this._pendingInlineElements.Pop();
				XmlElement xmlElement3 = this._openedElements.Peek();
				if (xmlElement2 != null && xmlElement3 != null)
				{
					xmlElement3.AppendChild(xmlElement2);
				}
				while (stack.Count > 0)
				{
					this._pendingInlineElements.Push(stack.Pop());
				}
				return;
			}
			if (this.IsElementOpened(htmlElementName))
			{
				while (this._openedElements.Count > 1)
				{
					XmlElement xmlElement4 = this._openedElements.Pop();
					if (xmlElement4.LocalName == htmlElementName)
					{
						return;
					}
					if (HtmlSchema.IsInlineElement(xmlElement4.LocalName))
					{
						this._pendingInlineElements.Push(this.CreateElementCopy(xmlElement4));
					}
				}
			}
		}
		private void AddEmptyElement(XmlElement htmlEmptyElement)
		{
			this.InvariantAssert(this._openedElements.Count > 0, "AddEmptyElement: Stack of opened elements cannot be empty, as we have at least one artificial root element");
			XmlElement xmlElement = this._openedElements.Peek();
			xmlElement.AppendChild(htmlEmptyElement);
		}
		private void AddEmptyInlineElement(XmlElement htmlEmptyElement)
		{
			this.OpenPendingInlineElements();
			this.InvariantAssert(this._openedElements.Count > 0, "AddTextContent: Stack of opened elements cannot be empty, as we have at least one artificial root element");
			XmlElement xmlElement = this._openedElements.Peek();
			xmlElement.AppendChild(htmlEmptyElement);
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
		private void AddTextContent(string textContent)
		{
			if (this._pendingInlineElements.Count > 0 && this._pendingInlineElements.Peek().Name == "font" && HtmlParser.HasOnlyWhitespace(textContent))
			{
				return;
			}
			this.OpenPendingInlineElements();
			this.InvariantAssert(this._openedElements.Count > 0, "AddTextContent: Stack of opened elements cannot be empty, as we have at least one artificial root element");
			XmlElement xmlElement = this._openedElements.Peek();
			XmlText newChild = this._document.CreateTextNode(textContent);
			xmlElement.AppendChild(newChild);
		}
		private void AddComment(string comment)
		{
			this.OpenPendingInlineElements();
			this.InvariantAssert(this._openedElements.Count > 0, "AddComment: Stack of opened elements cannot be empty, as we have at least one artificial root element");
			XmlElement xmlElement = this._openedElements.Peek();
			XmlComment newChild = this._document.CreateComment(comment);
			xmlElement.AppendChild(newChild);
		}
		private void OpenPendingInlineElements()
		{
			if (this._pendingInlineElements.Count > 0)
			{
				for (int i = this._pendingInlineElements.Count - 1; i >= 0; i--)
				{
					XmlElement xmlElement = this._pendingInlineElements.ElementAt(i);
					this.InvariantAssert(this._openedElements.Count > 0, "OpenPendingInlineElements: Stack of opened elements cannot be empty, as we have at least one artificial root element");
					XmlElement xmlElement2 = this._openedElements.Peek();
					if (!(xmlElement.Name == "span") || !(xmlElement2.Name == "span") || xmlElement.HasAttributes)
					{
						xmlElement2.AppendChild(xmlElement);
						this._openedElements.Push(xmlElement);
					}
				}
				this._pendingInlineElements.Clear();
			}
		}
		private void ParseAttributes(XmlElement xmlElement)
		{
			while (this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EOF && this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.TagEnd && this._htmlLexicalAnalyzer.NextTokenType != HtmlTokenType.EmptyTagEnd)
			{
				if (this._htmlLexicalAnalyzer.NextTokenType == HtmlTokenType.Name)
				{
					string nextToken = this._htmlLexicalAnalyzer.NextToken;
					this._htmlLexicalAnalyzer.GetNextEqualSignToken();
					this._htmlLexicalAnalyzer.GetNextAtomToken();
					string nextToken2 = this._htmlLexicalAnalyzer.NextToken;
					try
					{
						xmlElement.SetAttribute(nextToken, nextToken2);
					}
					catch (XmlException)
					{
					}
				}
				this._htmlLexicalAnalyzer.GetNextTagToken();
			}
		}
	}
}
