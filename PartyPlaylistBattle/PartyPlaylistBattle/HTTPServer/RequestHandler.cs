using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.HTTPServer
{
    class RequestHandler

    {
        public string Type { get; set; }
        public string Command { get; set; }
        public string Version { get; set; }
        public string Authorization { get; set; }
        public string Body { get; set; }
        public string[] Rest { get; set; }

        public RequestHandler(string request)
        {
            if (String.IsNullOrEmpty(request))
            {
                Type = "";
                Command = "";
                Version = "";
                Body = "";
                Authorization = "";
                Rest[0] = "";

            }
            else
            {
                string[] allLines = request.Split("\n");
                string[] partsofFirstLine = allLines[0].Split(" ");
                Type = partsofFirstLine[0];
                Command = partsofFirstLine[1];
                Version = partsofFirstLine[2];
                Rest = allLines;
                int go = 0;
                for (int i = 0; i < Rest.Length; i++)
                {
                    if (Rest[i] == "\r")
                    {
                        go = i + 1;
                    }
                }
                for (int x = go; x < Rest.Length; x++)
                {
                    Body += Rest[x];
                }

                Authorization = CheckAuthorization(Rest, "Authorization: Basic");
            }
        }

        private string CheckAuthorization(string[] rest, string aut)
        {
            foreach (var x in rest)
            {
                if (x.Contains(aut))
                {
                    int start = x.IndexOf(aut) + aut.Length + 1;
                    int end = x.IndexOf("\r") - start;
                    return x.Substring(start, end);
                }
            }
            return "";
        }
    }
}
