@echo off
if "%1" equ "redo" wsl docker compose down -v && wsl docker compose up -d && exit
if "%1" equ "flush" wsl docker compose down -v
if "%1" equ "down" wsl docker compose down 
if "%1" equ "" wsl docker compose up -d
