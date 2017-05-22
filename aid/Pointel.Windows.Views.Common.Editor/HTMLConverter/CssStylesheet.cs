using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Pointel.Windows.Views.Common.Editor.HTMLConverter
{
    internal class CssStylesheet
    {
        private class StyleDefinition
        {
            public string Selector;
            public string Definition;
            public StyleDefinition(string selector, string definition)
            {
                this.Selector = selector;
                this.Definition = definition;
            }
        }
        private List<CssStylesheet.StyleDefinition> _styleDefinitions;
        public CssStylesheet(XmlElement htmlElement)
        {
            if (htmlElement != null)
            {
                this.DiscoverStyleDefinitions(htmlElement);
            }
        }
        public void DiscoverStyleDefinitions(XmlElement htmlElement)
        {
            if (htmlElement.LocalName.ToLower() == "link")
            {
                return;
            }
            if (htmlElement.LocalName.ToLower() != "style")
            {
                for (XmlNode xmlNode = htmlElement.FirstChild; xmlNode != null; xmlNode = xmlNode.NextSibling)
                {
                    if (xmlNode is XmlElement)
                    {
                        this.DiscoverStyleDefinitions((XmlElement)xmlNode);
                    }
                }
                return;
            }
            StringBuilder stringBuilder = new StringBuilder();
            for (XmlNode xmlNode2 = htmlElement.FirstChild; xmlNode2 != null; xmlNode2 = xmlNode2.NextSibling)
            {
                if (xmlNode2 is XmlText || xmlNode2 is XmlComment)
                {
                    stringBuilder.Append(this.RemoveComments(xmlNode2.Value));
                }
            }
            int i = 0;
            while (i < stringBuilder.Length)
            {
                int num = i;
                while (i < stringBuilder.Length && stringBuilder[i] != '{')
                {
                    if (stringBuilder[i] == '@')
                    {
                        while (i < stringBuilder.Length && stringBuilder[i] != ';')
                        {
                            i++;
                        }
                        num = i + 1;
                    }
                    i++;
                }
                if (i < stringBuilder.Length)
                {
                    int num2 = i;
                    while (i < stringBuilder.Length && stringBuilder[i] != '}')
                    {
                        i++;
                    }
                    if (i - num2 > 2)
                    {
                        this.AddStyleDefinition(stringBuilder.ToString(num, num2 - num), stringBuilder.ToString(num2 + 1, i - num2 - 1));
                    }
                    if (i < stringBuilder.Length)
                    {
                        i++;
                    }
                }
            }
        }
        private string RemoveComments(string text)
        {
            int num = text.IndexOf("/*");
            if (num < 0)
            {
                return text;
            }
            int num2 = text.IndexOf("*/", num + 2);
            if (num2 < 0)
            {
                return text.Substring(0, num);
            }
            return text.Substring(0, num) + " " + this.RemoveComments(text.Substring(num2 + 2));
        }
        public void AddStyleDefinition(string selector, string definition)
        {
            selector = selector.Trim().ToLower();
            definition = definition.Trim().ToLower();
            if (selector.Length == 0 || definition.Length == 0)
            {
                return;
            }
            if (this._styleDefinitions == null)
            {
                this._styleDefinitions = new List<CssStylesheet.StyleDefinition>();
            }
            string[] array = selector.Split(new char[]
			{
				','
			});
            for (int i = 0; i < array.Length; i++)
            {
                string text = array[i].Trim();
                if (text.Length > 0)
                {
                    this._styleDefinitions.Add(new CssStylesheet.StyleDefinition(text, definition));
                }
            }
        }
        public string GetStyle(string elementName, List<XmlElement> sourceContext)
        {
            if (this._styleDefinitions != null)
            {
                for (int i = this._styleDefinitions.Count - 1; i >= 0; i--)
                {
                    string[] array = this._styleDefinitions[i].Selector.Split(new char[]
					{
						','
					});
                    bool flag = true;
                    string[] array2 = array;
                    for (int j = 0; j < array2.Length; j++)
                    {
                        string text = array2[j];
                        string[] array3 = text.Trim().Split(new char[]
						{
							' '
						});
                        int num = sourceContext.Count - 1;
                        flag = true;
                        for (int k = array3.Length - 1; k >= 0; k--)
                        {
                            string selectorLevel = array3[k].Trim();
                            if (k == array3.Length - 1 && !this.MatchSelectorLevel(selectorLevel, sourceContext[num]))
                            {
                                flag = false;
                                break;
                            }
                            while (num >= 0 && !this.MatchSelectorLevel(selectorLevel, sourceContext[num]))
                            {
                                num--;
                            }
                            if (num < 0)
                            {
                                flag = false;
                                break;
                            }
                            num--;
                        }
                        if (flag)
                        {
                            break;
                        }
                    }
                    if (flag)
                    {
                        return this._styleDefinitions[i].Definition;
                    }
                }
            }
            return null;
        }
        private bool MatchSelectorLevel(string selectorLevel, XmlElement xmlElement)
        {
            if (selectorLevel.Length == 0)
            {
                return false;
            }
            int num = selectorLevel.IndexOf('.');
            int num2 = selectorLevel.IndexOf('#');
            string text = null;
            string text2 = null;
            string text3 = null;
            if (num >= 0)
            {
                if (num > 0)
                {
                    text3 = selectorLevel.Substring(0, num);
                }
                text = selectorLevel.Substring(num + 1);
            }
            else
            {
                if (num2 >= 0)
                {
                    if (num2 > 0)
                    {
                        text3 = selectorLevel.Substring(0, num2);
                    }
                    text2 = selectorLevel.Substring(num2 + 1);
                }
                else
                {
                    text3 = selectorLevel;
                }
            }
            return (text3 == null || !(text3 != xmlElement.LocalName)) && (text2 == null || !(HtmlToXamlConverter.GetAttribute(xmlElement, "id") != text2)) && (text == null || !(HtmlToXamlConverter.GetAttribute(xmlElement, "class") != text));
        }
    }
}
