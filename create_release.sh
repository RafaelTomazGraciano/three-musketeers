#!/bin/bash
set -e
SOURCE_PATH=Three_Musketeers
PROJECT=$(find ./Three_Musketeers -name *.csproj)

dotnet publish $PROJECT -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:PublishTrimmed=false -p:DebugType=None -p:DebugSymbols=false -o ./release

