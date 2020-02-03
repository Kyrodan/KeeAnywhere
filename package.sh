#!/bin/bash    
printf "Enter Version Number > "
read version
mkdir -p build/dist
cp -v KeeAnywhere/bin/ReleasePlgx/KeeAnywhere.plgx build/dist/KeeAnywhere-$version.plgx
cp -v KeeAnywhere/bin/ReleasePlgx/*.dll* build/dist/
zip -j build/dist/KeeAnywhere-$version.zip build/dist/*
