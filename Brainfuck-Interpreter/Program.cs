namespace Brainfuck_Interpreter
{
    /// <summary>
    /// The program's startup class
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entry point
        /// </summary>
        /// <param name="args">Startup arguments</param>
        static void Main(string[] args)
        {
            //We create a new interpreter instance and pass the arguments to it
            new Interpreter(args);
        }
    }
}