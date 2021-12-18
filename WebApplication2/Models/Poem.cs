using System;
using System.Collections.Generic;
using System.IO;

public class SiirProje
{
    
    class Statements
    {
        static public int NextLine = 0;
        public int Line { get; set; }
        public string Statement { get; set; }
        public Statements(string _statement)
        {
            Line = NextLine;
            NextLine++;
            Statement = _statement;
        }
    }
    class Tokens
    {

    };
    class Symbols : Tokens
    {

    };
    class Variable
    {
        public string VarName { get; set; }
        public int i_data { get; set; }
        public Variable(string _v, int _d)
        {
            VarName = _v;
            i_data = _d;
        }
    };

    class Keyword : Tokens
    {

    };
    class Lexycal
    {
        public LinkedList<Statements> AllStatements = new LinkedList<Statements>();
        public LinkedList<Variable> AllVariables = new LinkedList<Variable>();
        public Stack<int> WhileLocations = new Stack<int>();
        public Stack<int> ReturnWhileLocations = new Stack<int>();
        public string statement;
        public int currentLine;
        public int maxLine;
        enum ComparisonOperators { Equal, Smaller, Higher };
        enum Operations { Add, Substract, Multiply, divide, mod, None };
        public string final;


        string[] StatementWords = { "dir.", "dır.", "tır.", "tir.", "dur.", "dür", "tur", "tür", "rum", "rım", "rim" };
        string[] WhileWords = { "iken", "ken" };
        string[] IfWords = { "eğer", "bilakis", "mesela", "düşün", "örneğin", "ise" };
        public void StatementFill(int no,string[] arr)
        {

                
                currentLine = 1;
            int i = 0;
                while (i<no) 
                {
                    statement = arr[i];
                    AddAsStatement();
                i++;
                Console.WriteLine(i);
                }
            
        }
        public void ProcessLine(int _line)
        {

            LinkedListNode<Statements> Node = AllStatements.First;

            for (int j = 1; j < _line && Node != null; j++)
            {
                Node = Node.Next;                  

            }
            if (Node != null)
            {

            
            statement = Node.Value.Statement;
                // Console.WriteLine(statement);
                try
                {
                    AssignmentOperation();
                    AdditionOperation();
                    PrintOperation();
                    WhileStatement();
                    IfStatement();
                    ReturnFromWhileStatement();
                }
                catch(Exception e) { final += e; }

            if (currentLine > AllStatements.Last.Value.Line)
            {
                Free();
                return;
            }

            currentLine++;
            ProcessLine(currentLine);
            }


        }
        public void Free()
        {
            WhileLocations.Clear();
            ReturnWhileLocations.Clear();
            AllVariables.Clear();
            AllStatements.Clear();
            currentLine = 1;
        }
        public void IfStatement()
        {
            string operand_1;
            string operand_2;
            Variable VOperand_1 = new Variable("s", 0);
            Variable VOperand_2 = new Variable("s", 0);
            string[] words;
            int statementIndex = 1;
            bool comparisonSuccess = false;
            int findingReturnOfWhile = 0;
            for (int i = 0; i < IfWords.Length; i++)
            {


                if (statement.Contains(IfWords[i]))
                {
   
                    LinkedListNode<Statements> thisStatementNode = AllStatements.First;
                    while (thisStatementNode != null)
                    {
                        if (thisStatementNode.Value.Statement == statement)
                        {                                                       //finds the Node where this statement is stored at
                            break;

                        }
                        thisStatementNode = thisStatementNode.Next;
                        statementIndex++;
                    }

                    statement = statement.Remove(statement.IndexOf(IfWords[i]));
                    words = statement.Split(' ');
                    operand_1 = words[0];
                    operand_2 = words[1];
                    LinkedListNode<Variable> Node = AllVariables.First;
                    while (Node != null)
                    {
                        if (Node.Value.VarName == operand_1)
                        {
                            VOperand_1 = Node.Value;
                        }
                        Node = Node.Next;

                    }
                    Node = AllVariables.First;
                    while (Node != null)
                    {
                        if (Node.Value.VarName == operand_2)
                        {
                            VOperand_2 = Node.Value;
                        }
                        Node = Node.Next;

                    }
                    LinkedListNode<Statements> _Node = thisStatementNode;
                    _Node = _Node.Next;
                    while (_Node != null)
                    {



                        string lstatement = _Node.Value.Statement;
                        if (lstatement.Contains("iken") || lstatement.Contains("ise") || lstatement.Contains("ken") || lstatement.Contains("bilakis") || lstatement.Contains("mesela") || lstatement.Contains("düşün") || lstatement.Contains("örneğin") || lstatement.Contains("eğer"))  // WHİLE OLMA KOŞULLARI
                        {
                            findingReturnOfWhile--;
                        }
                        else if (lstatement.Contains("dön") || lstatement.Contains("geç"))
                        {   //DÖNME KOŞULU SONRA DEVAM
                            findingReturnOfWhile++;
                        }

                        statementIndex++;
                        if (findingReturnOfWhile == 1)
                        {
                            ReturnWhileLocations.Push(statementIndex); // BURAYA GELMİYOR
                            break;
                        }

                        _Node = _Node.Next;
                    }
                    if (statement.Contains("çok") || statement.Contains("fazla") || statement.Contains("yükse"))
                    {

                        if (CheckIfConditionMet(ComparisonOperators.Higher, VOperand_1, VOperand_2))
                        {
                            comparisonSuccess = true;

                        }
                        else
                        {
                            comparisonSuccess = false;
                        }
                    }
                    else
                    if (statement.Contains("az") || statement.Contains("alça") || statement.Contains("kısa"))
                    {
                        if (CheckIfConditionMet(ComparisonOperators.Smaller, VOperand_1, VOperand_2))
                        {
                            comparisonSuccess = true;

                        }
                        else
                        {
                            comparisonSuccess = false;
                        }
                    }
                    else
                    {
                        if (CheckIfConditionMet(ComparisonOperators.Equal, VOperand_1, VOperand_2))
                        {
                            comparisonSuccess = true;
                        }
                        else
                        {
                            comparisonSuccess = false;
                        }
                    }
                    if (comparisonSuccess == false)
                    {
                        currentLine = ReturnWhileLocations.Pop();


                    }

                }
            }
        }
        bool CheckIfConditionMet(ComparisonOperators op, Variable v1, Variable v2)
        {
            switch (op)
            {
                case ComparisonOperators.Equal:
                    return v1.i_data == v2.i_data ? true : false;
                case ComparisonOperators.Higher:
                    return v1.i_data > v2.i_data ? true : false;
                case ComparisonOperators.Smaller:

                    return v1.i_data < v2.i_data ? true : false;
                default:
                    return false;
            }
        }
        public bool WhileStatement()
        {

            int findingReturnOfWhile = 0; // ilerlerken while görürse -1 return görürse +1 1 olduğu an işleme başlıyor
            string operand_1;
            string operand_2;
            Variable VOperand_1 = new Variable("s", 0);
            Variable VOperand_2 = new Variable("s", 0);
            string[] words;
            int statementIndex = 1;
            bool comparisonSuccess = false;
            for (int i = 0; i < WhileWords.Length; i++)
            {
                if (statement.Contains(WhileWords[i]))
                {
                    LinkedListNode<Statements> thisStatementNode = AllStatements.First;
                    while (thisStatementNode != null)
                    {
                        if (thisStatementNode.Value.Statement == statement)
                        {                                                       //finds the Node where this statement is stored at
                            break;

                        }
                        thisStatementNode = thisStatementNode.Next;
                        statementIndex++;
                    }
                    // Console.WriteLine(statementIndex);

                    //      WhileLocations.Push(statementIndex);

                    statement = statement.Remove(statement.IndexOf(WhileWords[i]));


                    words = statement.Split(' ');
                    WhileLocations.Push(currentLine);



                    operand_1 = words[0];
                    operand_2 = words[1];
                    //  Console.WriteLine(operand_1);
                    LinkedListNode<Variable> Node = AllVariables.First;
                    VOperand_1.i_data = operand_1.Length;
                    while (Node != null)
                    {
                        //YÜZYILLIK ACI BUNLAR IMMEDIATE DATA GIBI OLMUYOR HEP VARIABLE LISTTEN ARIYO BISILER
                        if (Node.Value.VarName == operand_1)
                        {
                            VOperand_1 = Node.Value;


                        }
                        Node = Node.Next;

                    }

                    Node = AllVariables.First;
                    VOperand_2.i_data = operand_2.Length;
                    while (Node != null)
                    {
                        if (Node.Value.VarName == operand_2)
                        {
                            VOperand_2 = Node.Value;
                        }
                        Node = Node.Next;

                    }
                    LinkedListNode<Statements> _Node = thisStatementNode;
                    _Node = _Node.Next;
                    while (_Node != null)
                    {



                        string lstatement = _Node.Value.Statement;

                        if (lstatement.Contains("iken") || lstatement.Contains("ise") || lstatement.Contains("ken") || lstatement.Contains("bilakis") || lstatement.Contains("mesela") || lstatement.Contains("düşün") || lstatement.Contains("örneğin") || lstatement.Contains("eğer"))  // WHİLE OLMA KOŞULLARI
                        {
                            findingReturnOfWhile--;
                        }
                        else if (lstatement.Contains("dön") || lstatement.Contains("geç"))
                        {   //DÖNME KOŞULU SONRA DEVAM
                            findingReturnOfWhile++;
                        }

                        statementIndex++;

                        if (findingReturnOfWhile == 1)
                        {
                            // Console.WriteLine(statementIndex);
                            ReturnWhileLocations.Push(statementIndex);

                            break;
                        }

                        _Node = _Node.Next;
                    }





                    //       Console.WriteLine(WhileLocations.Peek() + " -- -- -" + ReturnWhileLocations.Peek());
                    if (statement.Contains("çok") || statement.Contains("fazla") || statement.Contains("yükse"))
                    {

                        if (CheckIfConditionMet(ComparisonOperators.Higher, VOperand_1, VOperand_2))
                        {
                            comparisonSuccess = true;

                        }
                        else
                        {
                            comparisonSuccess = false;
                        }
                    }
                    else
                    if (statement.Contains("az") || statement.Contains("alça") || statement.Contains("kısa"))
                    {

                        if (CheckIfConditionMet(ComparisonOperators.Smaller, VOperand_1, VOperand_2))
                        {
                            comparisonSuccess = true;
                        }
                        else
                        {

                            comparisonSuccess = false;
                        }
                    }
                    else
                    {
                        if (CheckIfConditionMet(ComparisonOperators.Equal, VOperand_1, VOperand_2))
                        {
                            comparisonSuccess = true;
                        }
                        else
                        {
                            comparisonSuccess = false;
                        }
                    }
                    //  Console.WriteLine(comparisonSuccess);
                    //Console.WriteLine(VOperand_1.i_data + " " + VOperand_2.i_data);
                    if (!comparisonSuccess)
                    {
                        //  Console.WriteLine("*-*-*-*" + ReturnWhileLocations.Peek());
                        currentLine = ReturnWhileLocations.Pop(); //+1 ?
                        WhileLocations.Pop();

                    }
                    else
                    {
                        //while stack kalıyor
                    }










                    return true;

                }



            }


            return false;
        }
        public void ReturnFromWhileStatement()
        {
            if (statement.Contains("dön"))
            {

                currentLine = WhileLocations.Pop() - 1;
                ReturnWhileLocations.Pop();
                //                Console.WriteLine("-*              *--" + currentLine + WhileLocations.Pop());
            }
            else if (statement.Contains("geç"))
            {

                //WhileLocations.Pop();
                //ReturnWhileLocations.Pop();
            }

        }

        public void AddAsStatement()
        {
            statement = statement.ToLower();
            Statements _s = new Statements(statement);
            AllStatements.AddLast(_s);

        }

        public bool AdditionOperation()
        {
            string[] words;
            string operand_1;
            string operand_2;
            Operations ops = Operations.None;
            Variable VOperand_1 = new Variable("s", 0);
            Variable VOperand_2 = new Variable("s", 0);
            Variable Result = new Variable("s", 0);
            words = statement.Split(' ');

            if (words[0].Contains("ekle") || words[0].Contains("topla"))
            {
                ops = Operations.Add;

            }
            else if (words[0].Contains("eksi") || words[0].Contains("çık"))
            {
                ops = Operations.Substract;
            }
            else if (words[0].Contains("çarp") || words[0].Contains("vur"))
            {
                ops = Operations.Multiply;
            }
            else if (words[0].Contains("böl") || words[0].Contains("parçala"))
            {
                ops = Operations.divide;
            }
            else if (words[0].Contains("mod") || words[0].Contains("ayıkla") || words[0].Contains("tut") || words[0].Contains("git"))
            {
                ops = Operations.mod;
                
            }



            if (ops != Operations.None)
            {
                operand_1 = words[2];
                operand_2 = words[3];

                LinkedListNode<Variable> Node = AllVariables.First;

                VOperand_1.i_data = operand_1.Length;
                while (Node != null)
                {
                    if (Node.Value.VarName == operand_1) 
                    {
                        VOperand_1 = Node.Value;

                    }

                    Node = Node.Next;
                }
                Node = AllVariables.First;
                VOperand_2.i_data = operand_2.Length-1;//DIKKAT -1 YARABANDI NE İŞE YARIYO BİLMİYORUM AAAAAAAAAAAAAAAAAA YOKKEN ÇALIŞIYODU KONSOLDAYKEN

                while (Node != null)
                {
                    if (Node.Value.VarName == operand_2)
                    {
                        VOperand_2 = Node.Value;

                    }
                    Node = Node.Next;

                }
                if (ops == Operations.Add)
                    Result = new Variable(words[1], VOperand_1.i_data + VOperand_2.i_data);
                else if (ops == Operations.Substract)
                    Result = new Variable(words[1], VOperand_1.i_data - VOperand_2.i_data);
                else if (ops == Operations.Multiply)
                    Result = new Variable(words[1], VOperand_1.i_data * VOperand_2.i_data);
                else if (ops == Operations.divide)
                    Result = new Variable(words[1], VOperand_1.i_data / VOperand_2.i_data);
                else if (ops == Operations.mod)
                {
                    int veri = VOperand_1.i_data % VOperand_2.i_data;
                    Console.WriteLine(VOperand_1.i_data+" % "+ VOperand_2.i_data+"result: "+veri);
                    Result = new Variable(words[1], veri);

                }
                bool declared = false;
                Node = AllVariables.First;
                while (Node != null)
                {
                    if (Node.Value.VarName == Result.VarName)
                    {
                        declared = true;
                        Node.Value = Result;


                    }
                    Node = Node.Next;

                }
                if (declared == false)
                {
                    AllVariables.AddLast(Result);

                }
                return true;

            }
            return false;
        }
        public bool AssignmentOperation()

        {

            string variableName;
            string[] words;

            for (int i = 0; i < StatementWords.Length; i++)
            {
                if (statement.Contains(StatementWords[i]))
                {
                    int data = 0;
                    statement = statement.Remove(statement.IndexOf(StatementWords[i]));
                    //    Console.WriteLine(statement);
                    words = statement.Split(' ');
                    variableName = words[0];
                    for (int j = words.Length - 1; j > 0; j--)
                    {
                        data += (int)Math.Pow((double)10, (double)(words.Length - (j + 1))) * (words[j].Length % 10);
                    }
                    Variable var = new Variable(variableName, data);
                    //Console.WriteLine(data);
                    LinkedListNode<Variable> Node = AllVariables.First;
                    while (Node != null)
                    {
                        if (Node.Value.VarName == words[0])
                        {
                            Node.Value = var;
                            break;
                        }
                        Node = Node.Next;

                    }
                    if (Node == null)
                    {
                        AllVariables.AddLast(var);
                    }

                    // Console.WriteLine(var.VarName);
                    return true;

                }
            }
            return false;
        }
        public bool PrintOperation()
        {
            string[] words;


            words = statement.Split(' ');

            if (words[0].Contains("haykır") || words[0].Contains("söyle") || words[0].Contains("anlat") || words[0].Contains("boş"))
            {
                for (int i = 1; i < words.Length; i++)
                {
                    if (words[i].Contains("boş") || words[i].Contains("rahat"))
                    {
                        final += " ";
                    }
                    else if (words[i].Contains("bırak") || words[i].Contains("ayrıl") || words[i].Contains("kır") || words[i].Contains("terk"))
                    {
                        final += "\r\n";

                    }
                    else
                    {
                        LinkedListNode<Variable> Node = AllVariables.First;
                        bool declared = true;
                        while (Node != null)
                        {
                            if (Node.Value.VarName == words[i])
                            {

                                declared = false;
                                final+=Node.Value.i_data;
                            }
                            Node = Node.Next;

                        }
                        if (declared == true)
                            final+=words[i][0];
                        
                    }
                }
                return true;
            }

            return false;

        }
    };
    public string lastt;
    public SiirProje(string str)
    {
        Console.WriteLine("Source file to compile and run: ");
        Lexycal lex = new Lexycal();
        if (str != null)
        {
            Console.WriteLine(str);
            string[] all = str.Split('\n');
            lex.StatementFill(all.Length, all);
            lex.ProcessLine(1);
            lastt=lex.final;
            str = null;
        }
    }
}
