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

  cat KeeAnywhere/KeeAnywhere.csproj.tmp \
  | sed 's/<!--REMOVE_FOR_LINUX //g' \
  | sed 's/ REMOVE_FOR_LINUX-->//g' \
  | sed 's/<!--COMMENT_FOR_LINUX_START-->/<!--/g' \
  | sed 's/<!--COMMENT_FOR_LINUX_END-->/-->/g' \
  | sed "s${d}NETSTANDARD_PATH${d}$NETSTANDARD_PATH${d}g" \
  | sed "s${d}SYSTEM_THREADING_TASKS_PATH${d}$SYSTEM_THREADING_TASKS_PATH${d}g" \
  > KeeAnywhere/KeeAnywhere.csproj

  # Set secrets
  cp KeeAnywhere/StorageProviders/GoogleDrive/GoogleDriveHelper.cs KeeAnywhere/StorageProviders/GoogleDrive/GoogleDriveHelper.cs.tmp

  cat KeeAnywhere/StorageProviders/GoogleDrive/GoogleDriveHelper.cs.tmp \
  | sed "s${d}GoogleDriveClientId = \"dummy\"${d}GoogleDriveClientId = \"$GOOGLEDRIVECLIENTID\"${d}g" \
  | sed "s${d}GoogleDriveClientSecret = \"dummy\"${d}GoogleDriveClientSecret = \"$GOOGLEDRIVECLIENTSECRET\"${d}g" \
  > KeeAnywhere/StorageProviders/GoogleDrive/GoogleDriveHelper.cs

  # build
  nuget restore
  msbuild KeeAnywhere.sln /p:Configuration=Release /t:Clean,Build /fl /flp:logfile=build/build.log

  # restore csproj
  cp KeeAnywhere/KeeAnywhere.csproj.tmp KeeAnywhere/KeeAnywhere.csproj
  rm KeeAnywhere/KeeAnywhere.csproj.tmp

  # restore secrets
  cp KeeAnywhere/StorageProviders/GoogleDrive/GoogleDriveHelper.cs.tmp KeeAnywhere/StorageProviders/GoogleDrive/GoogleDriveHelper.cs
  rm KeeAnywhere/StorageProviders/GoogleDrive/GoogleDriveHelper.cs.tmp
}

copy_plgx() {
  echo "Copy plgx"
  cp KeeAnywhere/bin/Release/KeeAnywhere.plgx build/dist/KeeAnywhere-linux-$VERSION.plgx
}

package_zip() {
  echo "Packaging zip"
  cd KeeAnywhere/bin/Release
  7zz a -tzip ../../../build/dist/KeeAnywhere-linux-$VERSION.zip -x!*.plgx -x!*.pdb -x!*.xml -x!*.config -x!KeePass.*
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