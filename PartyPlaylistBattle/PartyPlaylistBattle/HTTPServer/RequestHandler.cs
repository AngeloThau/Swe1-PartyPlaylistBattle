using System;
using System.Collections.Generic;
using System.Text;

namespace PartyPlaylistBattle.HTTPServer
{
    class RequestHandler

    {
        public String Type { get; set; }
        public String Order { get; set; }
        public String Version { get; set; }
        public String authorization { get; set; }
        public String body { get; set; }
        public String[] HeadRest { get; set; }


        public RequestHandler(String request)
        {
            if (String.IsNullOrEmpty(request))
            {
                Type = "";
                Order = "";
                Version = "";
                body = "";
                authorization = "";
                HeadRest[0] = "";

            }

            else
            {
                String[] tokens = request.Split("\n");
                String[] mymessage = tokens[0].Split(" ");
                Type = mymessage[0];
                Order = mymessage[1];
                Version = mymessage[2];
                HeadRest = tokens;

                int bodystart = 0;
                for (int i = 0; i < HeadRest.Length; i++)
                {
                    if (HeadRest[i] == "\r")
                    {
                        bodystart = i + 1;
                        //body = request.HeadRest[i + 1];
                    }
                }
                for (int a = bodystart; a < HeadRest.Length; a++)
                {
                    this.body += HeadRest[a];
                }

                authorization = GetAuthorization(HeadRest, "Authorization: Basic");



            }
        }

        //gets authorization out of head
        private string GetAuthorization(string[] tokens, string wordOne)
        {
            foreach (var element in tokens)
            {
                if (element.Contains(wordOne))
                {
                    int start = element.IndexOf(wordOne) + wordOne.Length + 1;
                    int end = element.IndexOf("\r") - start;
                    return element.Substring(start, end);
                }


            }
            return "";


        }
    }
}
