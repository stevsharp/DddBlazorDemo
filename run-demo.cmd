@echo off
cd /d "%~dp0"
title DddBlazorDemo
echo Starting DddBlazorDemo on http://localhost:55400 ...
dotnet run --project DddBlazorDemo\DddBlazorDemo.csproj
pause
