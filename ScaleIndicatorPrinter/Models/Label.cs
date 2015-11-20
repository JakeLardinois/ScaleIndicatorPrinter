using System;
using Microsoft.SPOT;


namespace ScaleIndicatorPrinter.Models
{
    public class Label
    {
        public string[] Parameters { get; set; }
        public string LabelFormat { get; set; }
        public static string DefaultLabel
        {
            get
            {
                return "FK\"*\"\r\n" + //this command clears the forms from memory. I was having an issue where whenever i changed the label format, I would have to print the labels twice until I used this command.
                    "q780\r\n" + //the 'q' command sets the label width in dots where 20mm=160dots; my label is 10cm(800dots) and so here I give it a 10dot margin on each side
                    "N\r\n" +                                             //since the printer automatically centers it pg137 of epl2-pm-en.pdf. the height can also be set using the 'q' command
                    "A250,10,0,2,1,1,N,\"QSF-7.1.A REV: 1-30-12\"\r\n" +  //but I just let the printer automatically sense it.
                    "A0,35,0,4,1,2,N,\"Item:~p0\"\r\n" +
                    "B0,80,0,3,3,7,50,N,\"~p0\"\r\n" +
                    "A0,140,0,4,1,2,N,\"Job:~p1\"\r\n" +
                    "B0,185,0,3,3,7,50,N,\"~p1\"\r\n" +
                    "A0,245,0,4,1,2,N,\"Emp(s):~p3\"\r\n" +
                    "LO100,285,680,0\r\n" +
                    "A0,310,0,4,1,2,N,\"Qty:~p4\"\r\n" +
                    "A320,300,0,4,1,2,N,\"Oper ~p2\"\r\n" +
                    "B460,300,0,3,3,7,50,N,\"~p2\"\r\n" +
                    "A320,350,0,4,1,2,N,\"NextOper:\"\r\n" +
                    "LO455,390,325,0\r\n" +
                    "A30,355,0,2,1,1,N,\"~p5\"\r\n" +
                    "A30,375,0,2,1,1,N,\"~p6\"\r\n" +
                    "P1\r\n";
            }
        }

        public string LabelText { 
            get { 
                return LabelFormat.SetParameters(Parameters); 
            } 
        }

        public Label(string[] objParams)
        {
            Parameters = objParams;
        }
    }
}
