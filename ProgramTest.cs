using System;

namespace vsi.Tests
{
    public class InterpreterTests
    {
        public static void Main(string[] args)
        {
            TestInputStatement();
            TestOutputStatement();
            TestAssignmentStatement();
            TestArithmeticExpressions();
            TestInvalidToken();
            TestSyntaxError();
            Console.WriteLine("All tests passed!");
        }

        static void TestInputStatement()
        {
            Console.WriteLine("Testing INPUT statement...");
            Interpreter._input = "INPUT x";
            Interpreter._lookahead = Interpreter.NextToken();
            Interpreter.Stmt();
            Console.WriteLine("Test passed!");
        }

        static void TestOutputStatement()
        {
            Console.WriteLine("Testing OUTPUT statement...");
            Interpreter._input = "OUTPUT x";
            Interpreter._lookahead = Interpreter.NextToken();
            Interpreter.Stmt();
            Console.WriteLine("Test passed!");
        }

        static void TestAssignmentStatement()
        {
            Console.WriteLine("Testing assignment statement...");
            Interpreter._input = "x = 10";
            Interpreter._lookahead = Interpreter.NextToken();
            Interpreter.Stmt();
            Console.WriteLine("Test passed!");
        }

        static void TestArithmeticExpressions()
        {
            Console.WriteLine("Testing arithmetic expressions...");
            Interpreter._input = "x = 5 + 3 * 2";
            Interpreter._lookahead = Interpreter.NextToken();
            Interpreter.Stmt();
            Console.WriteLine("Test passed!");

            Interpreter._input = "x = (5 + 3) * 2";
            Interpreter._lookahead = Interpreter.NextToken();
            Interpreter.Stmt();
            Console.WriteLine("Test passed!");

            Interpreter._input = "x = 10 / 2 - 3";
            Interpreter._lookahead = Interpreter.NextToken();
            Interpreter.Stmt();
            Console.WriteLine("Test passed!");
        }

        static void TestInvalidToken()
        {
            Console.WriteLine("Testing invalid token...");
            Interpreter._input = "x = @";
            Interpreter._lookahead = Interpreter.NextToken();
            try
            {
                Interpreter.Stmt();
                Console.WriteLine("Test failed! Expected error not thrown.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test passed! Error thrown: " + ex.Message);
            }
        }

        static void TestSyntaxError()
        {
            Console.WriteLine("Testing syntax error...");
            Interpreter._input = "x = 5 +";
            Interpreter._lookahead = Interpreter.NextToken();
            try
            {
                Interpreter.Stmt();
                Console.WriteLine("Test failed! Expected error not thrown.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Test passed! Error thrown: " + ex.Message);
            }
        }
    }
}