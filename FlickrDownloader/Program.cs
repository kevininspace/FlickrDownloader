﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FlickrDownloader
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null || args.Length == 0)
            {
                 return;
            }

            new FormFlickr();
        }
    }
}
