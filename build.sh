#!/usr/bin/env bash

SCRIPT_DIR=$( cd "$ dirname "${BASH_SOURCE[0]}" )" && pwd )
TOOLS_DIR=$SCRIPT_DIR/tools
SCRIPT="./build.cake"
CAKE_ARGUMENTS=()

echo Preparing to run build script...

# parse arguemtns
for i in "$@"; do
	case $1 in
		-s|--script) SCRIPT="$2"; shift ;;
		--) shift; CAKE_ARGUMENTS+=("$@"); break ;;
		*) CAKE_ARGUMENTS+=("$1") ;;
	esac
	shift
done

# Restoring all dotnet tools
echo Restoring all dotnet tools...
dotnet tool restore

echo Bootstrapping Cake...
dotnet cake $SCRIPT --Bootstrapping

echo Running build script...
exec dotnet cake $SCRIPT "${CAKE_ARGUMENTS[@]}"
