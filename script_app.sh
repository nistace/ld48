readonly UNITY_VERSION="2020.1.5f1"
readonly ROOT="D:\Documents\Projects\LudumDare\LudumDare48"
readonly BUILD_PATH="$ROOT/Builds"
readonly CURRENT_BUILD_PATH="$BUILD_PATH/Complete"
readonly UNITY_BUILDER="C:/Program Files/Unity/Hub/Editor/$UNITY_VERSION/Editor/Unity.exe"



echo Cleaning...
rm -rf "$CURRENT_BUILD_PATH"
mkdir "$CURRENT_BUILD_PATH"

#Build Linux
echo Build Linux...
"$UNITY_BUILDER" -batchmode -logFile "$CURRENT_BUILD_PATH/build-log-Linux.txt" -buildLinux64Player "$CURRENT_BUILD_PATH/CompulsiveDiggers_linux/CompulsiveDiggers" -quit
linuxSuccess=$?

#Build MacOS
echo Build MacOS...
"$UNITY_BUILDER" -batchmode -logFile "$CURRENT_BUILD_PATH/build-log-MacOS.txt" -buildOSXUniversalPlayer "$CURRENT_BUILD_PATH/CompulsiveDiggers_macos/CompulsiveDiggers.app" -quit
macOsSuccess=$?

#### Leave Windows in last position so that Unity is automatically reopened to build to Windows		
#Build Windows
echo Build Windows...
"$UNITY_BUILDER" -batchmode -logFile "$CURRENT_BUILD_PATH/build-log-Windows.txt" -buildWindowsPlayer "$CURRENT_BUILD_PATH/CompulsiveDiggers_windows/CompulsiveDiggers.exe" -quit
winSuccess=$?

#Final report
echo "-----------------------"
if [ "$winSuccess" = "1" ]; then
	echo " - Build Windows : Failed"
else 
	echo " - Build Windows : OK"
fi		
if [ "$linuxSuccess" = "1" ]; then
	echo " - Build Linux : Failed"
else 
	echo " - Build Linux : OK"
fi				
if [ "$macOsSuccess" = "1" ]; then
	echo " - Build MacOS : Failed"
else 
	echo " - Build MacOS : OK"
fi				
echo "-----------------------"		
read -r -p "Press enter to continue"
