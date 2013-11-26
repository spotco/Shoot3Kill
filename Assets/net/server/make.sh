#!/bin/bash
rm *.exe
gmcs ServerAsync.cs ../../util/IOut.cs ../SPDefs.cs ../../util/JSONObject.cs
