using System;
using System.IO;
using System.Text;
using System.Collections;


namespace Brainfuck_Interpreter
{
    /// <summary>
    /// This class handles all the interpreting in the program
    /// </summary>
    class Interpreter
    {
        /// <summary>
        /// The highest value a field can have
        /// </summary>
        private const int m_fieldMax = 127;

        /// <summary>
        /// The lowest value a field can have
        /// </summary>
        private const int m_fieldMin = 0;

        /// <summary>
        /// The maximum amount of fields
        /// </summary>
        private const int m_maxFields = 1024;

        /// <summary>
        /// The current index of our pointer
        /// </summary>
        private int m_currentPointerIDX = 0;

        /// <summary>
        /// All of our fields
        /// </summary>
        private byte[] m_pointerFields = new byte[m_maxFields];

        /// <summary>
        /// The stack to store all of the loops in
        /// </summary>
        private Stack m_loopStack = new Stack();

        /// <summary>
        /// A string that contains the code that is being interpreted
        /// </summary>
        private string m_allCode;

       /// <summary>
       /// Class constructor, starts a new interpreter flow
       /// </summary>
       /// <param name="args">Startup arguments</param>
        public Interpreter(string[] args)
        {
            string allCode = null;

            try { allCode = File.ReadAllText(args[0]); }
            catch { Console.WriteLine("[ERROR]: Something went wrong while reading " + args[0]); }

            if(allCode != null) InterpreteAllCode(allCode);
            Console.WriteLine("\n\nFinished interpreting. Press any key to close this window.");
            Console.ReadKey();
        }

        /// <summary>
        /// Interpret all the code that is passed
        /// </summary>
        /// <param name="code">The code to interpret</param>
        private void InterpreteAllCode(string code)
        {
            m_allCode = code;
            for (int i = 0; i < code.Length; i++)
            {
                char c = code[i];
                switch (c)
                {
                    case ',': ReadInputToField();
                        break;
                    case '<': MovePointer(EMyPointerMoveDirection.LEFT);
                        break;

                    case '>':
                        MovePointer(EMyPointerMoveDirection.RIGHT);
                        break;
                    case '+':
                        ModifyField(EMyPointerModifyDirection.UP);
                        break;
                    case '-':
                        ModifyField(EMyPointerModifyDirection.DOWN);
                        break;
                    case '.':
                        PrintCurrentField();
                        break;
                    case '[':
                        i = StartLoop(i);
                        break;
                    case ']':
                        i = EndLoop(i);
                        break;
                }
            }
        }

        /// <summary>
        /// Read a key from the console into the current active field
        /// </summary>
        private void ReadInputToField()
        {
            int key = Console.Read();
            m_pointerFields[m_currentPointerIDX] = Convert.ToByte(key);
        }

        /// <summary>
        /// Ends the loop at the index provided
        /// </summary>
        /// <param name="idx">The index the loop ends at</param>
        /// <returns>The index to jump to (or stay at)</returns>
        private int EndLoop(int idx)
        {
            if (m_pointerFields[m_currentPointerIDX] > 0)
            {
                return (int)m_loopStack.Peek();
            }
            else m_loopStack.Pop(); //We pop this loop from our loop stack, but we don't want to use it anywhere here.

            return idx;
        }

        /// <summary>
        /// Starts a loop at the given index
        /// </summary>
        /// <param name="idx">The index to start the loop at</param>
        /// <returns>The index to go to or stay at</returns>
        private int StartLoop(int idx)
        {
            if (m_pointerFields[m_currentPointerIDX] > 0)
            {
                //loop
                m_loopStack.Push(idx);
                return idx;
            }
            else return FindLoopEnd(idx);
        }

        /// <summary>
        /// Finds where the loop that starts at the given index, ends at
        /// </summary>
        /// <param name="start">Index of the start of the loop</param>
        /// <returns>The index at which the loop ends</returns>
        private int FindLoopEnd(int start)
        {
            int foundSinceLoop = 0;
            for (int i = start + 1; i < m_allCode.Length; i++)
            {
                if (m_allCode[i] == '[') foundSinceLoop++;
                else if(m_allCode[i] == ']')
                {
                    if (foundSinceLoop > 0) foundSinceLoop--;
                    else if(foundSinceLoop == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// Prints the ASCII character of current active field
        /// </summary>
        private void PrintCurrentField()
        {
            Console.Write(Encoding.ASCII.GetChars(new byte[] { m_pointerFields[m_currentPointerIDX] }));
        }

        /// <summary>
        /// Modifies the current active field
        /// </summary>
        /// <param name="direction">Direction to modify the field in. 'Up' will add one to the current value, 'down' will subtract one of the current value</param>
        private void ModifyField(EMyPointerModifyDirection direction)
        {
            switch (direction)
            {
                case EMyPointerModifyDirection.UP:
                {
                    if (m_pointerFields[m_currentPointerIDX] == m_fieldMax) m_pointerFields[m_currentPointerIDX] = 0;
                    else m_pointerFields[m_currentPointerIDX]++;
                    break;
                }
                case EMyPointerModifyDirection.DOWN:
                {
                    if (m_pointerFields[m_currentPointerIDX] == m_fieldMin) m_pointerFields[m_currentPointerIDX] = 127;
                    else m_pointerFields[m_currentPointerIDX]--;
                    break;
                }
            }
        }

        /// <summary>
        /// Moves the pointer to another field
        /// </summary>
        /// <param name="direction">The direction to move the pointer in</param>
        private void MovePointer(EMyPointerMoveDirection direction)
        {
            switch (direction)
            {
                case EMyPointerMoveDirection.LEFT:
                {
                    if (m_currentPointerIDX == 0) m_currentPointerIDX = m_maxFields - 1;
                    else m_currentPointerIDX--;
                    break;
                }
                case EMyPointerMoveDirection.RIGHT:
                {
                    if (m_currentPointerIDX == m_maxFields - 1) m_currentPointerIDX = 0;
                    else m_currentPointerIDX++;
                    break;
                }
            }
        }

        /// <summary>
        /// Enumerator that holds the directions our field values can go (up = add to, down = subtract from)
        /// </summary>
        private enum EMyPointerModifyDirection
        {
            UP,
            DOWN
        }

        /// <summary>
        /// Enumerator that holds the directions our pointer can move in
        /// </summary>
        private enum EMyPointerMoveDirection
        {
            LEFT,
            RIGHT
        }


    }
}
