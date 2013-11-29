#!/bin/bash
rm Shoot3KillServer.exe
gmcs /define:SERVER Shoot3KillServer.cs ../../util/IOut.cs ../SPDefs.cs ../../util/JSONObject.cs SocketPolicyServer.cs IUtil.cs
