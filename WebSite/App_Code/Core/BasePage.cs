﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.WebPages;

public abstract class BasePage : WebPage
{
    public string T(string text)
    {
        return text;
    }
}