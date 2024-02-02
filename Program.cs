using System;
using System.Collections.Generic;

namespace vsi
{
    public enum ETokenType
    {
        INPUT, OUTPUT, VAR, AT, NUM, SUM, SUB, MULT, DIV, OPEN, CLOSE, EOL, EOF
    }

    public class Token
    {
        public ETokenType Type { get; set; }
        public string Value { get; set; }

        public Token(ETokenType type, string value = "")
        {
            Type = type;
            Value = value;
        }
    }

    public class Interpreter
    {
        static string _input;
        static int _position = -1;
        static Token _lookahead;
        static readonly char[] _whiteSpaceChars = { ' ', '\t' };
        static Dictionary<string, int> _symbolTable = new Dictionary<string, int>();

        static Token NextToken()
        {
            while (_position < _input.Length - 1)
            {
                _position++;
                char currentChar = _input[_position];

                if (Char.IsDigit(currentChar))
                {
                    string num = "";
                    while (_position < _input.Length && Char.IsDigit(_input[_position]))
                    {
                        num += _input[_position];
                        _position++;
                    }
                    _position--; // Move back one position since we went one step too far
                    return new Token(ETokenType.NUM, num);
                }
                else if (Char.IsLetter(currentChar))
                {
                    string varName = "";
                    while (_position < _input.Length && (Char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
                    {
                        varName += _input[_position];
                        _position++;
                    }
                    _position--; // Move back one position since we went one step too far
                    return new Token(ETokenType.VAR, varName);
                }
                else if (currentChar == '+') return new Token(ETokenType.SUM);
                else if (currentChar == '-') return new Token(ETokenType.SUB);
                else if (currentChar == '*') return new Token(ETokenType.MULT);
                else if (currentChar == '/') return new Token(ETokenType.DIV);
                else if (currentChar == '(') return new Token(ETokenType.OPEN);
                else if (currentChar == ')') return new Token(ETokenType.CLOSE);
                else if (currentChar == '=') return new Token(ETokenType.AT);
                else if (currentChar == '\n' || currentChar == '\r') return new Token(ETokenType.EOL);
                else if (_whiteSpaceChars.Contains(currentChar)) continue;
                else if (currentChar == '$' && _position + 1 < _input.Length && Char.IsLetter(_input[_position + 1]))
                {
                    string varName = "$";
                    _position++;
                    while (_position < _input.Length && (Char.IsLetterOrDigit(_input[_position]) || _input[_position] == '_'))
                    {
                        varName += _input[_position];
                        _position++;
                    }
                    _position--; // Move back one position since we went one step too far
                    return new Token(ETokenType.VAR, varName);
                }
                else
                {
                    Error("Erro Léxico");
                }
            }

            return new Token(ETokenType.EOF);
        }

        static void Error(string message)
        {
            Console.WriteLine("********************************");
            Console.WriteLine("Erro! " + message);
            Console.WriteLine(_input);
            Console.WriteLine("^".PadLeft(_position + 1, ' '));
            Console.WriteLine("********************************");
            Environment.Exit(0);
        }

        static void Match(ETokenType type)
        {
            if (_lookahead.Type == type)
                _lookahead = NextToken();
            else
                Error("Token inválido");
        }

        static void Prog()
        {
            Line();
        }

        static void Line()
        {
            Stmt();
            Match(ETokenType.EOL);
            if (_lookahead.Type != ETokenType.EOF)
                Prog();
        }

        static void Stmt()
        {
            if (_lookahead.Type == ETokenType.INPUT)
                In();
            else if (_lookahead.Type == ETokenType.OUTPUT)
                Out();
            else
                Atrib();
        }

        static void In()
        {
            Match(ETokenType.INPUT);
            Match(ETokenType.VAR);
        }

        static void Out()
        {
            Match(ETokenType.OUTPUT);
            Match(ETokenType.VAR);
        }

        static void Atrib()
        {
            Match(ETokenType.VAR);
            Match(ETokenType.AT);
            int exprResult = Expr();
            _symbolTable[_lookahead.Value] = exprResult; // Armazena o resultado da expressão na tabela de símbolos
        }

        static int Expr()
        {
            int termResult = Term();
            int exprPrimeResult = ExprPrime(termResult);
            return exprPrimeResult;
        }

        static int ExprPrime(int inheritedValue)
        {
            if (_lookahead.Type == ETokenType.SUM)
            {
                Match(ETokenType.SUM);
                int termResult = Term();
                int exprPrimeResult = ExprPrime(inheritedValue + termResult);
                return exprPrimeResult;
            }
            else if (_lookahead.Type == ETokenType.SUB)
            {
                Match(ETokenType.SUB);
                int termResult = Term();
                int exprPrimeResult = ExprPrime(inheritedValue - termResult);
                return exprPrimeResult;
            }
            else
            {
                return inheritedValue;
            }
        }

        static int Term()
        {
            int factResult = Fact();
            int termPrimeResult = TermPrime(factResult);
            return termPrimeResult;
        }

        static int TermPrime(int inheritedValue)
        {
            if (_lookahead.Type == ETokenType.MULT)
            {
                Match(ETokenType.MULT);
                int factResult = Fact();
                int termPrimeResult = TermPrime(inheritedValue * factResult);
                return termPrimeResult;
            }
            else if (_lookahead.Type == ETokenType.DIV)
            {
                Match(ETokenType.DIV);
                int factResult = Fact();
                int termPrimeResult = TermPrime(inheritedValue / factResult);
                return termPrimeResult;
            }
            else
            {
                return inheritedValue;
            }
        }

        static int Fact()
        {
            if (_lookahead.Type == ETokenType.NUM)
            {
                int num = Int32.Parse(_lookahead.Value);
                Match(ETokenType.NUM);
                return num;
            }
            else if (_lookahead.Type == ETokenType.VAR)
            {
                string varName = _lookahead.Value;
                Match(ETokenType.VAR);
                if (_lookahead.Type == ETokenType.AT)
                {
                    Match(ETokenType.AT);
                    return Expr(); // Avalia a expressão após a atribuição
                }
                else
                {
                    if (_symbolTable.ContainsKey(varName))
                        return _symbolTable[varName];
                    else
                        Error("Variável não declarada: " + varName);
                }
            }
            else if (_lookahead.Type == ETokenType.OPEN)
            {
                Match(ETokenType.OPEN);
                int exprResult = Expr();
                Match(ETokenType.CLOSE);
                return exprResult;
            }
            else
            {
                Error("Símbolo inesperado em Fact");
                return 0;
            }
        }

        public static void Main(string[] args)
        {
            Console.WriteLine("Insira a expressão");
            _input = Console.ReadLine();
            _lookahead = NextToken();
            Prog();
            Console.WriteLine("Sucesso");
        }
    }
}
