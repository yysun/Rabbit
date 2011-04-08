Rabbit Framework for WebMatrix
==============================

Introduction
------------

Rabbit is lightweight framework for building web sites using WebMatrix (ASP.NET Web Pages with Razor Syntax). 

Features
--------
* Easy to build sites with Modules, Plug-ins and Templates
* Sinatra style MVC
* Monadic method chain
* Use .NET dynamic
* Built-in Unite Testing Framework
* Support WebMatrix, works with any existing web sites 
* Distributed as NuGet Package 

Installation
------------
* Install it from "ASP.NET Web Pages Administration | Package Manager" (http://<Your Site>/_Admin)
* Merger Rabbit_AppStart.cshtml into your _AppStart.cshtml
* Replace your Default.cshtml with Rabbit_Default.cshtml

Project Site
------------
http://rabbit.codeplex.com

Source Code
-----------
http://github.com/yysun/Rabbit

Change log
----------
* V 0.4.0 Added code generating module, hook based section rendering
* V 0.3.0 Change ContentStore to Repository, Push data to actions
* V 0.2.1 Added testing module for unit testing and mock testing
* V 0.2.0 Improved the Content Storage to use JSON
* V 0.1.0 Proof of concept for hooks, templates and pages