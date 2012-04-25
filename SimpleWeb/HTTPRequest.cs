using System;
using System.Collections;
using Microsoft.SPOT;

namespace SimpleWeb
{
    public class HTTPRequest
    {
        public HTTPVerbs Verb { get; private set; }
        public String Path { get; private set; }
        public Hashtable Properties { get; private set; }

        public static HTTPRequest ParseRequestString(String request)
        {
            HTTPRequest result = new HTTPRequest();

            foreach(var line in request.Split('\n'))
                Debug.Print(line);

            return result;
        }
    }

    public enum HTTPVerbs
    {
        GET
    }
}
