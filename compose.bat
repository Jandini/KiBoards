@echo off
if "%1" equ "flush" wsl docker compose down -v
if "%1" equ "down" wsl docker compose down 
if "%1" equ "" wsl docker compose up -d
