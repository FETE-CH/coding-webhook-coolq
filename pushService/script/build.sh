#!/usr/bin/env bash
#Self-contained deployment
dotnet publish -o dist -c Release -r win-x64 --self-contained true

#Framework-dependent deployment 框架依赖部署
#dotnet publish -o dist -c Release

#Framework-dependent executables 框架依赖可执行程序
#dotnet publish -o dist -c Release -r win-x64 --self-contained false