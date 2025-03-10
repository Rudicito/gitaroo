#!/bin/sh

# Run this script to use a local copy of osu rather than fetching it from nuget.
# It expects the osu directory to be at the same level as the ruleset directory
#
# https://github.com/ppy/osu-framework/wiki/Testing-local-framework-checkout-with-other-projects

RULESET_CSPROJ="osu.Game.Rulesets.Gitaroo/osu.Game.Rulesets.Gitaroo.csproj"
SLN="osu.Game.Rulesets.Gitaroo.sln"

dotnet remove $RULESET_CSPROJ reference ppy.osu.Game

dotnet sln $SLN add ../osu/osu.Game/osu.Game.csproj

dotnet add $RULESET_CSPROJ reference ../osu/osu.Game/osu.Game.csproj


dotnet sln $SLN add ../osu-framework/osu.Framework/osu.Framework.csproj

# workaround for dotnet add not inserting $(MSBuildThisFileDirectory) on props files
#sed -i.bak 's:"..\\osu-framework:"$(MSBuildThisFileDirectory)..\\osu-framework:g' ./osu.Android.props && rm osu.Android.props.bak
#sed -i.bak 's:"..\\osu-framework:"$(MSBuildThisFileDirectory)..\\osu-framework:g' ./osu.iOS.props && rm osu.iOS.props.bak

# needed because iOS framework nupkg includes a set of properties to work around certain issues during building,
# and those get ignored when referencing framework via project, threfore we have to manually include it via props reference.
#sed -i.bak '/<\/Project>/i\
#  <Import Project=\"$(MSBuildThisFileDirectory)../osu-framework/osu.Framework.iOS.props\"/>\
#' ./osu.iOS.props && rm osu.iOS.props.bak

#tmp=$(mktemp)
#
#jq '.solution.projects += ["../osu-framework/osu.Framework/osu.Framework.csproj", "../osu-framework/osu.Framework.NativeLibs/osu.Framework.NativeLibs.csproj"]' osu.Desktop.slnf > $tmp
#mv -f $tmp osu.Desktop.slnf
#
#jq '.solution.projects += ["../osu-framework/osu.Framework/osu.Framework.csproj", "../osu-framework/osu.Framework.NativeLibs/osu.Framework.NativeLibs.csproj", "../osu-framework/osu.Framework.Android/osu.Framework.Android.csproj"]' osu.Android.slnf > $tmp
#mv -f $tmp osu.Android.slnf
#
#jq '.solution.projects += ["../osu-framework/osu.Framework/osu.Framework.csproj", "../osu-framework/osu.Framework.NativeLibs/osu.Framework.NativeLibs.csproj", "../osu-framework/osu.Framework.iOS/osu.Framework.iOS.csproj"]' osu.iOS.slnf > $tmp
#mv -f $tmp osu.iOS.slnf
