﻿using System;
using System.Linq;
using System.Threading;
using Konsole.Drawing;
using Konsole.Internal;
using Konsole.Menus;

namespace Konsole.Sample.Demos
{
    public class WindowDemo
    {
        private readonly IConsole _con2;

        public void ScrollingDemo()
        {
            var w = Window.Open(5,5,50,20, "");
            var names = TestData.MakeNames(150);
            _con2.ForegroundColor = ConsoleColor.DarkGray;
            foreach (var name in names)
            {
                w.WriteLine(name);
            }
        
        }

        public static void Run2(IConsole console)
        {
            var con = console;
            con.WriteLine("starting client server demo");
            var client = new Window(1, 4, 20, 20, ConsoleColor.Gray, ConsoleColor.DarkBlue, con);
            var server = new Window(25, 4, 25, 20, con);
            client.WriteLine("CLIENT");
            client.WriteLine("------");
            server.WriteLine("SERVER");
            server.WriteLine("------");
            client.WriteLine("<-- PUT some long text to show wrapping");
            server.WriteLine(ConsoleColor.DarkYellow, "--> PUT some long text to show wrapping");
            server.WriteLine(ConsoleColor.Red, "<-- 404|Not Found|some long text to show wrapping|");
            client.WriteLine(ConsoleColor.Red, "--> 404|Not Found|some long text to show wrapping|");
            
            // line below will cause some faulty (offset) scrolling.
            //for(int i = 0; i<30; i++) client.WriteLine(i.ToString());

            con.WriteLine("starting names demo");
            // let's open a window with a box around it by using Window.Open
            var names = Window.Open(78, 4, 40, 10, "names");
            TestData.MakeNames(40).OrderByDescending(n => n).ToList().ForEach(n => names.WriteLine(n));

            con.WriteLine("starting numbers demo");
            var numbers = Window.Open(78, 15, 40, 10, "numbers", LineThickNess.Double,ConsoleColor.White,ConsoleColor.Blue);
            Enumerable.Range(1,200).ToList().ForEach(i => numbers.WriteLine(i.ToString()));

        }

        public static void Run(IConsole con)
        {
            con.WriteLine("starting client server demo");
            var height = 20;
            int width = (con.WindowWidth / 3) - 3;
            var client = new Window(1, 3, width, height, ConsoleColor.Gray, ConsoleColor.DarkBlue, con);
            var server = new Window(width + 2, 3, width, height, con);

            Console.CursorTop = 22;
            // simulate a bunch of messages from my fake REST server

            var messages = new[]
            {
                new { Request = "put foo", Response = "404|Not Found|foo|"},
                new { Request = "post animals/cat", Response = "201|Created|animals/cat|"},
                new { Request = "post animals/dog", Response = "201|Created|animals/dog|"},
                new { Request = "get animals", Response = "206 | Partial content | animals |[`animals/cat`, `animals/dog`]"}

            };
            client.WriteLine("CLIENT");
            client.WriteLine("------");
            server.WriteLine("SERVER");
            server.WriteLine("------");

            foreach (var m in messages)
            {
                client.Send(m.Request);
                server.Recieve(m.Request);
                server.Send(m.Response);
                client.Recieve(m.Response);
                client.WriteLine("");
                server.WriteLine("");
            }

            client.WriteLine("this is a long line that will wrap over the end of the window.");


            var nameWindow = Window.Open(width * 2 + 3, 3, width, height + 1,"names");

            // let's print 300 names (300 WriteLine statements) that will roll right off 
            // the bottom of the screen, and should be clipped.
            var names = TestData.MakeNames(300);
            con.ForegroundColor = ConsoleColor.DarkGray;
            con.WriteLine("If you see ??? in any of the names, that's because Windows terminal does not print all UTF8 characters. (This prints correctly in Linux and Mac).");
            foreach (var name in names)
                nameWindow.WriteLine(name);
        }

    }

    public static class SendRec
    {
        public static void Send(this Window con, string text)
        {
            con.WriteLine(text);
        }

        public static void Recieve(this Window con, string text)
        {
            // awful, but will do...for rudimentary testing!
            var c = con.BackgroundColor;
            con.BackgroundColor = ConsoleColor.DarkYellow;
            con.WriteLine(text);
            con.BackgroundColor = c;
        }

    }

}
