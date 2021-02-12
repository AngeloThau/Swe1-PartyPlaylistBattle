@echo off

REM --------------------------------------------------
REM Party Playlist Battle
REM --------------------------------------------------
title Party Playlist Battle
echo CURL Testing for Party Playlist Battle
echo.



REM --------------------------------------------------
echo 2) Login Users
curl -X POST http://localhost:10001/sessions --header "Content-Type: application/json" -d "{\"Username\":\"Firsty\", \"Password\":\"first\"}"


pause
@echo on
