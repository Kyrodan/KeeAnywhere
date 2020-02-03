#!/bin/bash

# usage: build.sh command [execute]
# available commands:
#  1. debug         builds debug version
#  2. release       builds release version
#  3. releaseplgx   builds packaged plgx plugin

# determine build configuration
case $1 in
    debug ) echo "Building Debug Version"
        config="Debug"
        ;;
    release ) echo "Building Release Version"
        config="Release"
        ;;
    releaseplgx ) echo "Building PLGX Plugin"
        config="ReleasePlgx"
        echo "Enter Version Number > "
        read version
        ;;
    * ) exit 1
        ;;
esac

# restore NuGet packages
mono nuget.exe restore -PackagesDirectory packages

# clean and build plugin
xbuild /p:configuration=$config /t:Clean
xbuild /p:configuration=$config /t:Build

# run KeePass for testing
if [ "$2" = "execute" ]; then
    mono KeeAnywhere/bin/$config/KeePass.exe --debug -pw:sample KeeAnywhere/bin/$config/Databases/Sample.kdbx
fi

# package plugin into zip file
if [ $config = "ReleasePlgx" ]; then
    mkdir -p build/dist
    cp -v KeeAnywhere/bin/$config/KeeAnywhere.plgx build/dist/KeeAnywhere-$version.plgx
    cp -v KeeAnywhere/bin/$config/*.dll* build/dist/
    zip -j build/dist/KeeAnywhere-$version.zip build/dist/*
fi

exit 0
