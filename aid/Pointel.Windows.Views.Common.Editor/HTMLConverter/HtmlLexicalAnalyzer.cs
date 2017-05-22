using System;
using System.IO;
using System.Text;

namespace Pointel.Windows.Views.Common.Editor.HTMLConverter
{
    internal class HtmlLexicalAnalyzer
    {
        private StringReader _inputStringReader;
        private int _nextCharacterCode;
        private char _nextCharacter;
        private int _lookAheadCharacterCode;
        private char _lookAheadCharacter;
        private char _previousCharacter;
        private bool _ignoreNextWhitespace;
        private bool _isNextCharacterEntity;
        private StringBuilder _nextToken;
        private HtmlTokenType _nextTokenType;
        internal bool IgnoreNextWhitespace
        {
            get
            {
                return this._ignoreNextWhitespace;
            }
            set
            {
                if (this._ignoreNextWhitespace != value)
                {
                    this._ignoreNextWhitespace = value;
                }
            }
        }
        internal HtmlTokenType NextTokenType
        {
            get
            {
                return this._nextTokenType;
            }
        }
        internal string NextToken
        {
            get
            {
                return this._nextToken.ToString();
            }
        }
        private char NextCharacter
        {
            get
            {
                return this._nextCharacter;
            }
        }
        private bool IsAtEndOfStream
        {
            get
            {
                return this._nextCharacterCode == -1;
            }
        }
        private bool IsAtTagStart
        {
            get
            {
                return this._nextCharacter == '<' && (this._lookAheadCharacter == '/' || this.IsGoodForNameStart(this._lookAheadCharacter)) && !this._isNextCharacterEntity;
            }
        }
        private bool IsAtTagEnd
        {
            get
            {
                return (this._nextCharacter == '>' || (this._nextCharacter == '/' && this._lookAheadCharacter == '>')) && !this._isNextCharacterEntity;
            }
        }
        private bool IsAtDirectiveStart
        {
            get
            {
                return this._nextCharacter == '<' && this._lookAheadCharacter == '!' && !this.IsNextCharacterEntity;
            }
        }
        private bool IsNextCharacterEntity
        {
            get
            {
                return this._isNextCharacterEntity;
            }
        }
        internal HtmlLexicalAnalyzer(string inputTextString)
        {
            this._inputStringReader = new StringReader(inputTextString);
            this._nextCharacterCode = 0;
            this._nextCharacter = ' ';
            this._lookAheadCharacterCode = this._inputStringReader.Read();
            this._lookAheadCharacter = (char)this._lookAheadCharacterCode;
            this._previousCharacter = ' ';
            this._ignoreNextWhitespace = true;
            this._nextToken = new StringBuilder(100);
            this._nextTokenType = HtmlTokenType.Text;
            this.GetNextCharacter();
        }
        internal void GetNextContentToken()
        {
            this._nextToken.Length = 0;
            if (this.IsAtEndOfStream)
            {
                this._nextTokenType = HtmlTokenType.EOF;
                return;
            }
            if (this.IsAtTagStart)
            {
                this.GetNextCharacter();
                if (this.NextCharacter == '/')
                {
                    this._nextToken.Append("</");
                    this._nextTokenType = HtmlTokenType.ClosingTagStart;
                    this.GetNextCharacter();
                    this._ignoreNextWhitespace = false;
                    return;
                }
                this._nextTokenType = HtmlTokenType.OpeningTagStart;
                this._nextToken.Append("<");
                this._ignoreNextWhitespace = true;
                return;
            }
            else
            {
                if (!this.IsAtDirectiveStart)
                {
                    this._nextTokenType = HtmlTokenType.Text;
                    while (!this.IsAtTagStart && !this.IsAtEndOfStream && !this.IsAtDirectiveStart)
                    {
                        if (this.NextCharacter == '<' && !this.IsNextCharacterEntity && this._lookAheadCharacter == '?')
                        {
                            this.SkipProcessingDirective();
                        }
                        else
                        {
                            if (this.NextCharacter <= ' ')
                            {
                                if (!this._ignoreNextWhitespace)
                                {
                                    this._nextToken.Append(' ');
                                }
                                this._ignoreNextWhitespace = true;
                            }
                            else
                            {
                                this._nextToken.Append(this.NextCharacter);
                                this._ignoreNextWhitespace = false;
                            }
                            this.GetNextCharacter();
                        }
                    }
                    return;
                }
                this.GetNextCharacter();
                if (this._lookAheadCharacter == '[')
                {
                    this.ReadDynamicContent();
                    return;
                }
                if (this._lookAheadCharacter == '-')
                {
                    this.ReadComment();
                    return;
                }
                this.ReadUnknownDirective();
                return;
            }
        }
        internal void GetNextTagToken()
        {
            this._nextToken.Length = 0;
            if (this.IsAtEndOfStream)
            {
                this._nextTokenType = HtmlTokenType.EOF;
                return;
            }
            this.SkipWhiteSpace();
            if (this.NextCharacter == '>' && !this.IsNextCharacterEntity)
            {
                this._nextTokenType = HtmlTokenType.TagEnd;
                this._nextToken.Append('>');
                this.GetNextCharacter();
                return;
            }
            if (this.NextCharacter == '/' && this._lookAheadCharacter == '>')
            {
                this._nextTokenType = HtmlTokenType.EmptyTagEnd;
                this._nextToken.Append("/>");
                this.GetNextCharacter();
                this.GetNextCharacter();
                this._ignoreNextWhitespace = false;
                return;
            }
            if (this.IsGoodForNameStart(this.NextCharacter))
            {
                this._nextTokenType = HtmlTokenType.Name;
                while (this.IsGoodForName(this.NextCharacter))
                {
                    if (this.IsAtEndOfStream)
                    {
                        return;
                    }
                    this._nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
            }
            else
            {
                this._nextTokenType = HtmlTokenType.Atom;
                this._nextToken.Append(this.NextCharacter);
                this.GetNextCharacter();
            }
        }
        internal void GetNextEqualSignToken()
        {
            this._nextToken.Length = 0;
            this._nextToken.Append('=');
            this._nextTokenType = HtmlTokenType.EqualSign;
            this.SkipWhiteSpace();
            if (this.NextCharacter == '=')
            {
                this.GetNextCharacter();
            }
        }
        internal void GetNextAtomToken()
        {
            this._nextToken.Length = 0;
            this.SkipWhiteSpace();
            this._nextTokenType = HtmlTokenType.Atom;
            if ((this.NextCharacter == '\'' || this.NextCharacter == '"') && !this.IsNextCharacterEntity)
            {
                char nextCharacter = this.NextCharacter;
                this.GetNextCharacter();
                while ((this.NextCharacter != nextCharacter || this.IsNextCharacterEntity) && !this.IsAtEndOfStream)
                {
                    this._nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
                if (this.NextCharacter == nextCharacter)
                {
                    this.GetNextCharacter();
                    return;
                }
            }
            else
            {
                while (!this.IsAtEndOfStream && !char.IsWhiteSpace(this.NextCharacter) && this.NextCharacter != '>')
                {
                    this._nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
            }
        }
        private void GetNextCharacter()
        {
            if (this._nextCharacterCode == -1)
            {
                throw new InvalidOperationException("GetNextCharacter method called at the end of a stream");
            }
            this._previousCharacter = this._nextCharacter;
            this._nextCharacter = this._lookAheadCharacter;
            this._nextCharacterCode = this._lookAheadCharacterCode;
            this._isNextCharacterEntity = false;
            this.ReadLookAheadCharacter();
            if (this._nextCharacter == '&')
            {
                string text = this._nextToken.ToString();
                if (text.StartsWith(Uri.UriSchemeHttp + "://") || text.StartsWith(Uri.UriSchemeHttps + "://"))
                {
                    return;
                }
                if (this._lookAheadCharacter == '#')
                {
                    int num = 0;
                    this.ReadLookAheadCharacter();
                    int num2 = 0;
                    while (num2 < 7 && char.IsDigit(this._lookAheadCharacter))
                    {
                        num = 10 * num + (this._lookAheadCharacterCode - 48);
                        this.ReadLookAheadCharacter();
                        num2++;
                    }
                    if (this._lookAheadCharacter == ';')
                    {
                        this.ReadLookAheadCharacter();
                        this._nextCharacterCode = num;
                        this._nextCharacter = (char)this._nextCharacterCode;
                        this._isNextCharacterEntity = true;
                        return;
                    }
                    this._nextCharacter = this._lookAheadCharacter;
                    this._nextCharacterCode = this._lookAheadCharacterCode;
                    this.ReadLookAheadCharacter();
                    this._isNextCharacterEntity = false;
                    return;
                }
                else
                {
                    if (char.IsLetter(this._lookAheadCharacter))
                    {
                        string text2 = "";
                        int num3 = 0;
                        while (num3 < 10 && (char.IsLetter(this._lookAheadCharacter) || char.IsDigit(this._lookAheadCharacter)))
                        {
                            text2 += this._lookAheadCharacter;
                            this.ReadLookAheadCharacter();
                            num3++;
                        }
                        if (this._lookAheadCharacter == ';')
                        {
                            this.ReadLookAheadCharacter();
                            if (HtmlSchema.IsEntity(text2))
                            {
                                this._nextCharacter = HtmlSchema.EntityCharacterValue(text2);
                                this._nextCharacterCode = (int)this._nextCharacter;
                                this._isNextCharacterEntity = true;
                                return;
                            }
                            this._nextCharacter = this._lookAheadCharacter;
                            this._nextCharacterCode = this._lookAheadCharacterCode;
                            this.ReadLookAheadCharacter();
                            this._isNextCharacterEntity = false;
                            return;
                        }
                        else
                        {
                            this._nextCharacter = this._lookAheadCharacter;
                            this.ReadLookAheadCharacter();
                            this._isNextCharacterEntity = false;
                        }
                    }
                }
            }
        }
        private void ReadLookAheadCharacter()
        {
            if (this._lookAheadCharacterCode != -1)
            {
                this._lookAheadCharacterCode = this._inputStringReader.Read();
                this._lookAheadCharacter = (char)this._lookAheadCharacterCode;
            }
        }
        private void SkipWhiteSpace()
        {
            while (true)
            {
                if (this._nextCharacter == '<' && (this._lookAheadCharacter == '?' || this._lookAheadCharacter == '!'))
                {
                    this.GetNextCharacter();
                    if (this._lookAheadCharacter == '[')
                    {
                        while (!this.IsAtEndOfStream && (this._previousCharacter != ']' || this._nextCharacter != ']' || this._lookAheadCharacter != '>'))
                        {
                            this.GetNextCharacter();
                        }
                        if (this._nextCharacter == '>')
                        {
                            this.GetNextCharacter();
                        }
                    }
                    else
                    {
                        while (!this.IsAtEndOfStream && this._nextCharacter != '>')
                        {
                            this.GetNextCharacter();
                        }
                        if (this._nextCharacter == '>')
                        {
                            this.GetNextCharacter();
                        }
                    }
                }
                if (!char.IsWhiteSpace(this.NextCharacter))
                {
                    break;
                }
                this.GetNextCharacter();
            }
        }
        private bool IsGoodForNameStart(char character)
        {
            return character == '_' || char.IsLetter(character);
        }
        private bool IsGoodForName(char character)
        {
            return this.IsGoodForNameStart(character) || character == '.' || character == '-' || character == ':' || char.IsDigit(character) || this.IsCombiningCharacter(character) || this.IsExtender(character);
        }
        private bool IsCombiningCharacter(char character)
        {
            return false;
        }
        private bool IsExtender(char character)
        {
            return false;
        }
        private void ReadDynamicContent()
        {
            this._nextTokenType = HtmlTokenType.Text;
            this._nextToken.Length = 0;
            this.GetNextCharacter();
            this.GetNextCharacter();
            while ((this._nextCharacter != ']' || this._lookAheadCharacter != '>') && !this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
            }
            if (!this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
                this.GetNextCharacter();
            }
        }
        private void ReadComment()
        {
            this._nextTokenType = HtmlTokenType.Comment;
            this._nextToken.Length = 0;
            this.GetNextCharacter();
            this.GetNextCharacter();
            this.GetNextCharacter();
            while (true)
            {
                if (this.IsAtEndOfStream || (this._nextCharacter == '-' && this._lookAheadCharacter == '-') || (this._nextCharacter == '!' && this._lookAheadCharacter == '>'))
                {
                    this.GetNextCharacter();
                    if (this._previousCharacter == '-' && this._nextCharacter == '-' && this._lookAheadCharacter == '>')
                    {
                        break;
                    }
                    if (this._previousCharacter == '!' && this._nextCharacter == '>')
                    {
                        goto IL_C6;
                    }
                    this._nextToken.Append(this._previousCharacter);
                }
                else
                {
                    this._nextToken.Append(this.NextCharacter);
                    this.GetNextCharacter();
                }
            }
            this.GetNextCharacter();
        IL_C6:
            if (this._nextCharacter == '>')
            {
                this.GetNextCharacter();
            }
        }
        private void ReadUnknownDirective()
        {
            this._nextTokenType = HtmlTokenType.Text;
            this._nextToken.Length = 0;
            this.GetNextCharacter();
            while ((this._nextCharacter != '>' || this.IsNextCharacterEntity) && !this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
            }
            if (!this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
            }
        }
        private void SkipProcessingDirective()
        {
            this.GetNextCharacter();
            this.GetNextCharacter();
            while (((this._nextCharacter != '?' && this._nextCharacter != '/') || this._lookAheadCharacter != '>') && !this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
            }
            if (!this.IsAtEndOfStream)
            {
                this.GetNextCharacter();
                this.GetNextCharacter();
            }
        }
    }
}
