using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Milestone1
{
    public enum Token_class
    {
        Number, String, Int, Float, Read, Write, Repeat, Until, If, ElseIf, Else, Then, Return, Endl,
        Comment, RightParentheses, LeftParentheses, PlusOp, MinusOp, MultiplyOp, DivideOp, AssignmentOp, SemiColon,
        Coma, LessThanOp, GreaterThanOp, IsEqualOp, NotEqualOp, AndOp, OrOp, LeftBraces, RightBraces,
        Main, Identifier, End,
        EndOfFile, Invalid

    }
    public class Token
    {
        public Token()
        {

        }
        public Token(string lex, Token_class tc)
        {
            this.lex = lex;
            this.type = tc;
        }
        public string lex;
        public Token_class type;
    }

    public class Scanner
    {
        public List<Token> tokens = new List<Token>();
        Dictionary<string, Token_class> reservedWords = new Dictionary<string, Token_class>();
        Dictionary<string, Token_class> operators = new Dictionary<string, Token_class>();
        Dictionary<char, Token_class> specialChar = new Dictionary<char, Token_class>();

        public Scanner()
        {
            reservedWords.Add("int", Token_class.Int);
            reservedWords.Add("float", Token_class.Float);
            reservedWords.Add("read", Token_class.Read);
            reservedWords.Add("write", Token_class.Write);
            reservedWords.Add("repeat", Token_class.Repeat);
            reservedWords.Add("until", Token_class.Until);
            reservedWords.Add("if", Token_class.If);
            reservedWords.Add("elseif", Token_class.ElseIf);
            reservedWords.Add("else", Token_class.Else);
            reservedWords.Add("then", Token_class.Then);
            reservedWords.Add("return", Token_class.Return);
            reservedWords.Add("endl", Token_class.Endl);
            reservedWords.Add("main", Token_class.Main);
            reservedWords.Add("string", Token_class.String);
            reservedWords.Add("end", Token_class.End);

            operators.Add("+", Token_class.PlusOp);
            operators.Add("-", Token_class.MinusOp);
            operators.Add("*", Token_class.MultiplyOp);
            operators.Add("/", Token_class.DivideOp);
            operators.Add("<", Token_class.LessThanOp);
            operators.Add(">", Token_class.GreaterThanOp);
            operators.Add("=", Token_class.IsEqualOp);
            operators.Add("<>", Token_class.NotEqualOp);
            operators.Add("&&", Token_class.AndOp);
            operators.Add("||", Token_class.OrOp);
            operators.Add(":=", Token_class.AssignmentOp);


            specialChar.Add(';', Token_class.SemiColon);
            specialChar.Add(',', Token_class.Coma);
            specialChar.Add('(', Token_class.LeftParentheses);
            specialChar.Add(')', Token_class.RightParentheses);
            specialChar.Add('{', Token_class.LeftBraces);
            specialChar.Add('}', Token_class.RightBraces);

        }

        public List<Token> startScan(string source)
        {
            // outer loop to parse the whole source code char by char
            for (int i = 0; i < source.Length; ++i)
            {
                // j for inner loops
                int j = i;
                char c = source[i];
                // add initail char to current lexeme 
                string curr = c.ToString();
                // ignore if space, tab or new line
                if (char.IsWhiteSpace(c))
                    continue;

                //check if current char is letter [a-z]
                if (char.IsLetter(c))
                {

                    Token token;
                    // only lexemes starting with letter is reserved words or identifier 
                    // if the first char is letter then search for next till finding non letter or didgit char
                    if (j != source.Length - 1)
                    {
                        ++j;
                        c = source[j];
                        // loop till there is no more letters or digits
                        while (char.IsLetterOrDigit(c) && (j <= source.Length - 1))
                        {
                            // add every letter or digit we find
                            curr += c;

                            // update char
                            if (j != source.Length - 1)
                            {
                                ++j;
                                c = source[j];
                            }
                            else
                                break;
                        }
                        // update outer loop index
                        if (j != source.Length - 1)
                            i = j - 1;
                        else
                            i = j;
                    }

                    // see if teh lexeme is reserved word or identifier
                    if (this.isReservedWord(curr))
                        token = new Token(curr, this.reservedWords[curr]);
                    else if (this.isIdentifier(curr))
                        token = new Token(curr, Token_class.Identifier);
                    // if not one of them then it's invalid input
                    else
                        token = new Token(curr, Token_class.Invalid);
                    // add the result to the list of tokens to return at the end
                    this.tokens.Add(token);
                    continue;
                }
                // if first char is digit then it must be number or invalid input
                else if (char.IsDigit(c))
                {
                    Token token;
                    if (j != source.Length - 1)
                    {
                        ++j;
                        c = source[j];
                        // parse till finding non digit nor dot
                        while ((char.IsDigit(c) || c == '.') && (j <= source.Length - 1))
                        {
                            // add the digit or dot to current lexeme
                            curr += c;
                            // update char
                            if (j != source.Length - 1)
                            {
                                ++j;
                                c = source[j];
                            }
                            else
                                break;
                        }
                    }
                    // update outer loop index
                    if (j != source.Length - 1)
                        i = j - 1;
                    else
                        i = j;
                    // validiate if it's a number or not
                    if (this.isNumber(curr))
                        token = new Token(curr, Token_class.Number);
                    else
                        token = new Token(curr, Token_class.Invalid);

                    this.tokens.Add(token);
                    continue;
                }
                // if there is / and not the last char in source then search for the * if exist loop till finding the ending * and /
                else if ((j < source.Length - 2) && (source[j] == '/' && source[j + 1] == '*'))
                {
                    Token token;
                    curr += source[j + 1];
                    j += 2;
                    c = source[j];
                    while (source[j] != '*' && source[j + 1] != '/')
                    {
                        curr += c;
                        if (j != source.Length - 2)
                        {
                            ++j;
                            c = source[j];
                        }
                        else
                            break;
                    }

                    if (j != source.Length - 2)
                    {
                        i = j + 1;
                        curr += c;
                        curr += source[j + 1];
                        token = new Token(curr, Token_class.Comment);
                        this.tokens.Add(token);
                    }
                    else
                    {
                        i = j + 1;
                        curr += c;
                        curr += source[j + 1];
                        token = new Token(curr, Token_class.Invalid);
                        this.tokens.Add(token);
                    }

                }
                // find if the char is sepcial one
                else if (this.specialChar.ContainsKey(c))
                {
                    Token token = new Token(c.ToString(), specialChar[c]);
                    this.tokens.Add(token);
                    continue;
                }
                // find if the char is double char operator && || <> := 
                else if (
                    (c == '&' && source[j + 1] == '&')
                    || (c == '|' && source[j + 1] == '|')
                    || (c == '<' && source[j + 1] == '>')
                    || (c == ':' && source[j + 1] == '='))
                {
                    string temp = c.ToString() + source[j + 1];
                    Token token = new Token(temp, this.operators[c.ToString() + source[j + 1]]);
                    this.tokens.Add(token);
                    ++j;
                    i = j;
                    continue;

                }
                // find if the char is operator
                else if (this.operators.ContainsKey(c.ToString()))
                {
                    Token token = new Token(c.ToString(), operators[c.ToString()]);
                    this.tokens.Add(token);
                    continue;
                }
                // search for comment, if " is found then parse till finding another one
                else if (c == '"')
                {
                    Token token;
                    ++j;
                    c = source[j];
                    while ((c != '"') && (j < source.Length - 1))
                    {
                        curr += c;
                        if (j != source.Length - 1)
                        {
                            ++j;
                            c = source[j];
                        }
                    }
                    if (j != source.Length - 1)
                    {
                        i = j;
                        curr += c;
                        token = new Token(curr, Token_class.String);
                        this.tokens.Add(token);
                    }

                }
                // if the char is none of the above then add it as invalid
                else
                {
                    this.tokens.Add(new Token(curr, Token_class.Invalid));
                }
            }
            // here we parsed teh whole source code
            this.tokens.Add(new Token("", Token_class.EndOfFile));
            return this.tokens;
        }

        private bool isReservedWord(string lexeme) => this.reservedWords.ContainsKey(lexeme);
        private bool isIdentifier(string lexeme) => Regex.IsMatch(lexeme, @"^[a-zA-Z_]([a-zA-Z_]|\d)*$");
        private bool isOperator(string lexeme) => this.operators.ContainsKey(lexeme);
        private bool isSpecialChar(char lexeme) => this.specialChar.ContainsKey(lexeme);
        private bool isNumber(string lexeme) => Regex.IsMatch(lexeme, @"^(\d+|(\d+\.\d+))$");
        private bool isComment(string lexeme) => Regex.IsMatch(lexeme, @"^/\*.*\*/$");
        private bool isString(string lexeme) => Regex.IsMatch(lexeme, "^\".*\"$");
    }
}
