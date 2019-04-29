using DIApplication.Interfaces;
using System;

namespace DIApplication
{
    class Program
    {
        private readonly IAccountService _accountService;

        public Program(IAccountService accountService)
        {
            _accountService = accountService;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }
    }
}
