VERSION=2.1.0
MONO_LIB_PATH=/usr/lib/mono/4.8-api
NETSTANDARD_PATH=$MONO_LIB_PATH/Facades/netstandard.dll
SYSTEM_THREADING_TASKS_PATH=$MONO_LIB_PATH/Facades/System.Threading.Tasks.dll

d=$'\03'

clean() {
  echo "Cleaning up"

  if [ -d "build/" ]; then rm -r "build/"; fi
}

build() {
  echo "Building"

  if [ ! -d "build/" ]; then mkdir build; fi
  if [ ! -d "build/dist" ]; then mkdir build/dist; fi

  # Linux-i-fy the csproj (plgx tool doesn't do Conditional's in the csproj)
  cp KeeAnywhere/KeeAnywhere.csproj KeeAnywhere/KeeAnywhere.csproj.tmp

  cat KeeAnywhere/KeeAnywhere.csproj \
  | sed 's/<!--REMOVE_FOR_LINUX //g' \
  | sed 's/ REMOVE_FOR_LINUX-->//g' \
  | sed 's/<!--COMMENT_FOR_LINUX_START-->/<!--/g' \
  | sed 's/<!--COMMENT_FOR_LINUX_END-->/-->/g' \
  | sed "s${d}NETSTANDARD_PATH${d}$NETSTANDARD_PATH${d}g" \
  | sed "s${d}SYSTEM_THREADING_TASKS_PATH${d}$SYSTEM_THREADING_TASKS_PATH${d}g" \
  > KeeAnywhere/KeeAnywhere.csproj

  # build
  nuget restore
  msbuild KeeAnywhere.sln /p:Configuration=Release /t:Clean,Build /fl /flp:logfile=build/build.log

  # restore csproj
  cp KeeAnywhere/KeeAnywhere.csproj.tmp KeeAnywhere/KeeAnywhere.csproj
  rm KeeAnywhere/KeeAnywhere.csproj.tmp
}

copy_plgx() {
  echo "Copy plgx"
  # cp KeeAnywhere/bin/Release/KeeAnywhere.plgx build/dist/KeeAnywhere-$VERSION.plgx
}

package_zip() {
  echo "Packaging zip"
  cd KeeAnywhere/bin/Release
  7za a -tzip ../../../build/dist/KeeAnywhere-$VERSION.zip -x!*.plgx -x!*.pdb -x!*.xml -x!*.config -x!KeePass.*
  cd ../../..
}

case $1 in
  "")
    clean
    build
    copy_plgx
    package_zip
    ;;
  clean)
    clean
    ;;
  build)
    build
    ;;
  copy_plgx)
    copy_plgx
    ;;
  package_zip)
    package_zip
    ;;
  *)
    echo "You can only use no arguments or one of (clean, build, copy_plgx, package_zip)"
esac